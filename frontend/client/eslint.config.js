import js from "@eslint/js";
import globals from "globals";
import { fixupPluginRules } from "@eslint/compat";
import reactPlugin from "eslint-plugin-react";
import reactHooks from "eslint-plugin-react-hooks";
import reactRefresh from "eslint-plugin-react-refresh";
import tseslint from "typescript-eslint";
import eslintPluginPrettierRecommended from "eslint-plugin-prettier/recommended";
import tailwind from "eslint-plugin-tailwindcss";
import pluginRouter from "@tanstack/eslint-plugin-router";
import pluginQuery from "@tanstack/eslint-plugin-query";
import pluginReactHookForm from "eslint-plugin-react-hook-form";
import reactCompiler from "eslint-plugin-react-compiler";

export default tseslint.config(
  {
    ignores: [
      "dist",
      "eslint.config.js",
      "tailwind.config.js",
      "postcss.config.js",
      "orval.config.cjs",
      "src/api/types/*",
    ],
  },
  {
    extends: [
      js.configs.recommended,
      ...tseslint.configs.strictTypeChecked,
      ...tseslint.configs.stylisticTypeChecked,
      reactPlugin.configs.flat.recommended,
      reactPlugin.configs.flat["jsx-runtime"],
      tailwind.configs["flat/recommended"],
      ...pluginRouter.configs["flat/recommended"],
      ...pluginQuery.configs["flat/recommended"],
      eslintPluginPrettierRecommended,
    ],
    files: ["**/*.{js,jsx,mjs,cjs,ts,tsx}"],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
      parserOptions: {
        ecmaFeatures: {
          jsx: true,
        },
        project: ["./tsconfig.node.json", "./tsconfig.app.json"],
        tsconfigRootDir: import.meta.dirname,
      },
    },
    settings: { react: { version: "detect" } },
    plugins: {
      reactPlugin,
      "react-hooks": reactHooks,
      "react-refresh": reactRefresh,
      "react-hook-form": fixupPluginRules(pluginReactHookForm),
      "react-compiler": reactCompiler,
    },
    rules: {
      "react-compiler/react-compiler": "error",
      ...reactPlugin.configs.recommended.rules,
      ...reactPlugin.configs["jsx-runtime"].rules,
      ...reactHooks.configs.recommended.rules,
      ...pluginReactHookForm.configs.recommended.rules,
      "react-hooks/exhaustive-deps": "error",
      "react-refresh/only-export-components": [
        "warn",
        { allowConstantExport: true },
      ],
      "@typescript-eslint/no-misused-promises": "off",
      "prettier/prettier": [
        "error",
        {
          plugins: ["prettier-plugin-tailwindcss"],
          tailwindFunctions: ["twJoin", "twMerge"],
        },
      ],
    },
  },
);
