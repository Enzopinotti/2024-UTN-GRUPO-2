import js from "@eslint/js";
import globals from "globals";
import reactHooks from "eslint-plugin-react-hooks";

const testGlobals = {
  afterEach: "readonly",
  beforeEach: "readonly",
  describe: "readonly",
  expect: "readonly",
  test: "readonly",
  vi: "readonly",
};

export default [
  {
    ignores: [
      "build/**",
      "node_modules/**",
      "src/styles/css/**",
    ],
  },
  js.configs.recommended,
  {
    files: ["src/**/*.{js,jsx}"],
    languageOptions: {
      ecmaVersion: "latest",
      sourceType: "module",
      parserOptions: {
        ecmaFeatures: {
          jsx: true,
        },
      },
      globals: {
        ...globals.browser,
        ...globals.es2025,
      },
    },
    plugins: {
      "react-hooks": reactHooks,
    },
    rules: {
      ...reactHooks.configs.recommended.rules,
      "react-hooks/set-state-in-effect": "warn",
      "no-unused-vars": [
        "warn",
        {
          argsIgnorePattern: "^_",
          varsIgnorePattern: "^_",
        },
      ],
    },
  },
  {
    files: ["src/**/*.test.{js,jsx}", "src/setupTests.js"],
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.es2025,
        ...testGlobals,
      },
    },
  },
];
