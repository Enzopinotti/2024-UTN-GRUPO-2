export const getPasswordConditions = (password = "") => ({
  length: password.length >= 8,
  lowercase: /[a-z]/.test(password),
  uppercase: /[A-Z]/.test(password),
  number: /\d/.test(password),
  specialChar: /[@$!%*?&]/.test(password),
});

export const arePasswordConditionsMet = (conditions) =>
  Object.values(conditions).every(Boolean);
