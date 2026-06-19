import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

type TranslationState = {
  source: string;
  rendered: string;
};

const textStates = new WeakMap<Text, TranslationState>();
const attributeStates = new WeakMap<Element, Map<string, TranslationState>>();
const translatedAttributes = ['placeholder', 'title', 'aria-label'];

function replaceText(value: string, dictionary: Record<string, string>) {
  const leading = value.match(/^\s*/)?.[0] || '';
  const trailing = value.match(/\s*$/)?.[0] || '';
  const normalized = value.replace(/\s+/g, ' ').trim();
  return dictionary[normalized] ? `${leading}${dictionary[normalized]}${trailing}` : value;
}

function translateTree(root: Node, dictionary: Record<string, string>) {
  const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
  const nodes: Text[] = [];
  while (walker.nextNode()) nodes.push(walker.currentNode as Text);

  for (const node of nodes) {
    const parent = node.parentElement;
    if (!parent || ['SCRIPT', 'STYLE', 'CODE'].includes(parent.tagName)) continue;
    const current = node.nodeValue ?? '';
    const state = textStates.get(node) ?? { source: current, rendered: current };
    if (current !== state.rendered) state.source = current;
    const translated = replaceText(state.source, dictionary);
    state.rendered = translated;
    textStates.set(node, state);
    if (node.nodeValue !== translated) node.nodeValue = translated;
  }

  const elements = root instanceof Element ? [root, ...root.querySelectorAll('*')] : [];
  for (const element of elements) {
    let states = attributeStates.get(element);
    if (!states) {
      states = new Map();
      attributeStates.set(element, states);
    }
    for (const attribute of translatedAttributes) {
      const current = element.getAttribute(attribute);
      if (!current) continue;
      const state = states.get(attribute) ?? { source: current, rendered: current };
      if (current !== state.rendered) state.source = current;
      const translated = dictionary[state.source.trim()] || state.source;
      state.rendered = translated;
      states.set(attribute, state);
      if (current !== translated) element.setAttribute(attribute, translated);
    }
  }
}

export const AutoTranslate = () => {
  const { i18n } = useTranslation();

  useEffect(() => {
    let scheduled = false;
    const apply = () => {
      scheduled = false;
      const language = (i18n.resolvedLanguage || i18n.language || 'ru').split('-')[0];
      const bundle = i18n.getResourceBundle(language, 'translation') as {
        auto?: Record<string, string>;
      };
      translateTree(document.body, bundle?.auto || {});
      document.documentElement.lang = language;
    };
    const schedule = () => {
      if (scheduled) return;
      scheduled = true;
      requestAnimationFrame(apply);
    };
    const observer = new MutationObserver(schedule);
    observer.observe(document.body, {
      childList: true,
      subtree: true,
      characterData: true,
      attributes: true,
      attributeFilter: translatedAttributes,
    });
    i18n.on('languageChanged', schedule);
    schedule();
    return () => {
      observer.disconnect();
      i18n.off('languageChanged', schedule);
    };
  }, [i18n]);

  return null;
};
