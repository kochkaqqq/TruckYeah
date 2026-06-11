import { type ButtonHTMLAttributes, forwardRef } from 'react';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  ({ variant = 'primary', size = 'md', className, children, ...props }, ref) => {
    const base = 'inline-flex items-center justify-center font-medium transition-all rounded-md focus:outline-none focus:ring-2 focus:ring-primary/50 disabled:opacity-50 disabled:cursor-not-allowed';
    
    const variants: Record<string, string> = {
      primary: 'bg-primary text-white hover:bg-primary-hover active:scale-[0.98]',
      secondary: 'bg-surface text-text border border-border hover:bg-gray-50',
      ghost: 'bg-transparent text-text-muted hover:bg-gray-100 hover:text-text',
      danger: 'bg-error text-white hover:bg-red-600',
    };
    
    const sizes: Record<string, string> = {
      sm: 'px-3 py-1.5 text-xs',
      md: 'px-4 py-2 text-sm',
      lg: 'px-6 py-3 text-base',
    };

    return (
      <button
        ref={ref}
        className={`${base} ${variants[variant]} ${sizes[size]} ${className || ''}`}
        {...props}
      >
        {children}
      </button>
    );
  }
);
Button.displayName = 'Button';