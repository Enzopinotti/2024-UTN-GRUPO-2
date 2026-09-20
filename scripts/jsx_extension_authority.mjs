import fs from "node:fs";
import path from "node:path";
import process from "node:process";
import { transformWithOxc } from "vite";

const appRoot = path.join(process.cwd(), "Frontend", "antigal.client");
const srcRoot = path.join(appRoot, "src");

function walk(dir) {
  return fs.readdirSync(dir, { withFileTypes: true }).flatMap((entry) => {
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) return walk(full);
    return [full];
  });
}

const jsFiles = walk(srcRoot)
  .filter((file) => file.endsWith(".js"))
  .sort();

const jsxBearingJs = [];
const parseFailures = [];

for (const file of jsFiles) {
  const code = fs.readFileSync(file, "utf8");
  try {
    await transformWithOxc(code, file, { lang: "js" });
  } catch (jsError) {
    try {
      await transformWithOxc(code, file, {
        lang: "jsx",
        jsx: { runtime: "automatic" },
      });
      jsxBearingJs.push(path.relative(appRoot, file));
    } catch (jsxError) {
      parseFailures.push({
        file: path.relative(appRoot, file),
        js: String(jsError),
        jsx: String(jsxError),
      });
    }
  }
}

console.log("frontend_js_files=" + jsFiles.length);
console.log("frontend_jsx_bearing_js_files=" + jsxBearingJs.length);
console.log("frontend_js_parse_failures=" + parseFailures.length);

if (jsxBearingJs.length) {
  console.error("JSX-bearing .js files must use .jsx:");
  for (const file of jsxBearingJs) console.error("- " + file);
  process.exitCode = 1;
}

if (parseFailures.length) {
  console.error("JavaScript parse failures:");
  for (const failure of parseFailures) {
    console.error("- " + failure.file);
    console.error(failure.js);
    console.error(failure.jsx);
  }
  process.exitCode = 1;
}

if (!process.exitCode) {
  console.log("frontend-jsx-extension-authority=clean");
}
