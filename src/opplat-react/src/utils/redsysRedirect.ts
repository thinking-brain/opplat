import type { RedsysCardSetupRedirect } from '../types';

/**
 * Auto-submits a hidden HTML form to Redsys's hosted payment page, handing the browser off
 * for the 3DS challenge. The webhook (not this redirect) is the source of truth for whether
 * the card setup ultimately succeeded.
 */
export const submitRedsysCardSetupRedirect = (redirect: RedsysCardSetupRedirect): void => {
  const form = document.createElement('form');
  form.method = 'POST';
  form.action = redirect.actionUrl;

  for (const [name, value] of Object.entries(redirect.formFields)) {
    const input = document.createElement('input');
    input.type = 'hidden';
    input.name = name;
    input.value = value;
    form.appendChild(input);
  }

  document.body.appendChild(form);
  form.submit();
};
