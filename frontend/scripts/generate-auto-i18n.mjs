import fs from 'node:fs';
import path from 'node:path';
import ts from 'typescript';

const root = path.resolve(import.meta.dirname, '..');
const sourceRoot = path.join(root, 'src');
const localeRoot = path.join(sourceRoot, 'i18n', 'locales');
const separator = '\n<<<TRUCKYEAH_I18N_SEPARATOR>>>\n';
const cyrillic = /[А-Яа-яЁё]/;

const localeFiles = fs.readdirSync(localeRoot).filter((file) => file.endsWith('.json')).sort();

function walk(directory) {
  return fs.readdirSync(directory, { withFileTypes: true }).flatMap((entry) => {
    const fullPath = path.join(directory, entry.name);
    if (entry.isDirectory()) return walk(fullPath);
    return entry.name.endsWith('.tsx') || entry.name.endsWith('.ts') ? [fullPath] : [];
  });
}

const normalize = (value) => value.replace(/\s+/g, ' ').trim();

function addText(collection, value) {
  const text = normalize(value);
  if (text && cyrillic.test(text) && text.length <= 1000) collection.add(text);
}

function extractTexts(filePath, collection) {
  const source = fs.readFileSync(filePath, 'utf8');
  const sourceFile = ts.createSourceFile(
    filePath,
    source,
    ts.ScriptTarget.Latest,
    true,
    filePath.endsWith('.tsx') ? ts.ScriptKind.TSX : ts.ScriptKind.TS,
  );
  const visit = (node) => {
    if (ts.isJsxText(node)) addText(collection, node.getText(sourceFile));
    if (ts.isStringLiteralLike(node) || ts.isNoSubstitutionTemplateLiteral(node)) {
      addText(collection, node.text);
    }
    if (ts.isTemplateExpression(node)) {
      addText(collection, node.head.text);
      for (const span of node.templateSpans) addText(collection, span.literal.text);
    }
    ts.forEachChild(node, visit);
  };
  visit(sourceFile);
}

async function translateBatch(texts, language) {
  if (language === 'ru') return texts;
  let response;
  for (let attempt = 0; attempt < 6; attempt += 1) {
    response = await fetch(
      `https://translate.google.ru/translate_a/single?client=at&sl=ru&tl=${language}&dt=t`,
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8' },
        body: new URLSearchParams({ q: texts.join(separator) }),
      },
    );
    if (response.ok) break;
    if (response.status !== 429 || attempt === 5) {
      throw new Error(`Translation ru->${language} failed: ${response.status}`);
    }
    await new Promise((resolve) => setTimeout(resolve, 5000 * (attempt + 1)));
  }
  const payload = await response.json();
  const translated = payload[0].map((part) => part[0]).join('');
  const parts = translated.split(/<<<\s*TRUCKYEAH_I18N_SEPARATOR\s*>>>/);
  if (parts.length !== texts.length) {
    throw new Error(`${language}: received ${parts.length} translations for ${texts.length} texts`);
  }
  return parts.map(normalize);
}

async function translateAll(texts, language, existing, onProgress) {
  const result = { ...existing };
  const pending = texts.filter((text) => !result[text]);
  for (let index = 0; index < pending.length; index += 25) {
    const batch = pending.slice(index, index + 25);
    const translated = await translateBatch(batch, language);
    batch.forEach((text, itemIndex) => {
      result[text] = translated[itemIndex] || text;
    });
    await onProgress(Object.fromEntries(texts.map((text) => [text, result[text] || text])));
    process.stdout.write(`\r${language}: ${Math.min(index + 25, pending.length)}/${pending.length}`);
    await new Promise((resolve) => setTimeout(resolve, 900));
  }
  if (pending.length) process.stdout.write('\n');
  return Object.fromEntries(texts.map((text) => [text, result[text] || text]));
}

const targetLanguage = process.env.TARGET_LANG;
const selectedLocaleFiles = targetLanguage
  ? localeFiles.filter((file) => path.basename(file, '.json') === targetLanguage)
  : localeFiles;
if (targetLanguage && selectedLocaleFiles.length === 0) {
  throw new Error(`Unknown TARGET_LANG: ${targetLanguage}`);
}

const extracted = new Set();
for (const filePath of walk(sourceRoot)) extractTexts(filePath, extracted);
const texts = [...extracted].sort((a, b) => a.localeCompare(b, 'ru'));
console.log(`Extracted ${texts.length} Russian UI strings.`);

for (const localeFile of selectedLocaleFiles) {
  const language = path.basename(localeFile, '.json');
  const localePath = path.join(localeRoot, localeFile);
  const locale = JSON.parse(fs.readFileSync(localePath, 'utf8'));
  locale.auto = await translateAll(texts, language, locale.auto || {}, async (auto) => {
    locale.auto = auto;
    fs.writeFileSync(localePath, `${JSON.stringify(locale, null, 2)}\n`, 'utf8');
  });
  fs.writeFileSync(localePath, `${JSON.stringify(locale, null, 2)}\n`, 'utf8');
}
console.log(`Updated ${selectedLocaleFiles.length} locale files.`);
