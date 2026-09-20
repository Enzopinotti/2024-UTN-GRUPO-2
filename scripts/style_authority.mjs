import { execFileSync } from "node:child_process";
import fs from "node:fs";
import path from "node:path";

const root = process.cwd();
const frontend = path.join(root, "Frontend", "antigal.client");
const packagePath = path.join(frontend, "package.json");
const lockPath = path.join(frontend, "package-lock.json");
const entryPath = path.join(frontend, "src", "index.jsx");
const sassEntryPath = path.join(frontend, "src", "styles", "scss", "index.scss");

const pkg = JSON.parse(fs.readFileSync(packagePath, "utf8"));
const lock = JSON.parse(fs.readFileSync(lockPath, "utf8"));
const lockRoot = lock.packages?.[""] ?? {};

function fail(message) {
  console.error(`style-authority failure: ${message}`);
  process.exit(1);
}

if (JSON.stringify(pkg.dependencies ?? {}) !== JSON.stringify(lockRoot.dependencies ?? {})) {
  fail("package dependencies and lockfile root dependencies diverged");
}
if (JSON.stringify(pkg.devDependencies ?? {}) !== JSON.stringify(lockRoot.devDependencies ?? {})) {
  fail("package devDependencies and lockfile root devDependencies diverged");
}

if (pkg.scripts?.start !== "vite" || pkg.scripts?.dev !== "vite") {
  fail("Vite must be the sole start/dev server authority");
}
if ("sass" in (pkg.scripts ?? {})) {
  fail("legacy standalone Sass watcher script returned");
}
if (pkg.devDependencies?.sass == null) {
  fail("Sass compiler must remain available to Vite");
}
if (pkg.devDependencies?.concurrently != null || pkg.dependencies?.concurrently != null) {
  fail("concurrently must not remain package authority");
}
if (lock.packages?.["node_modules/concurrently"]) {
  fail("concurrently remains in lockfile authority");
}

const entry = fs.readFileSync(entryPath, "utf8");
if (!entry.includes("./styles/scss/index.scss")) {
  fail("frontend entrypoint does not import native Sass authority");
}
if (entry.includes("/styles/css/") || entry.includes("./styles/css/")) {
  fail("frontend entrypoint still references generated CSS authority");
}
if (!fs.existsSync(sassEntryPath)) {
  fail("Sass entrypoint is missing");
}

const trackedGenerated = execFileSync(
  "git",
  ["ls-files", "Frontend/antigal.client/src/styles/css"],
  { cwd: root, encoding: "utf8" }
).trim();
if (trackedGenerated) {
  fail(`tracked generated CSS remains:\n${trackedGenerated}`);
}

const trackedScss = execFileSync(
  "git",
  ["ls-files", "Frontend/antigal.client/src/styles/scss"],
  { cwd: root, encoding: "utf8" }
).trim().split("\n").filter(Boolean);

if (trackedScss.length < 20) {
  fail(`unexpectedly small Sass source authority: ${trackedScss.length} files`);
}

console.log(`frontend-sass-source-files=${trackedScss.length}`);
console.log("frontend-generated-css-tracked=0");
console.log("frontend-concurrently-authority=absent");
console.log("frontend-native-sass-authority=clean");
