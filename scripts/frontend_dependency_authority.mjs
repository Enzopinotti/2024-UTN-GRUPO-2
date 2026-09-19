import fs from "node:fs";
import path from "node:path";
import process from "node:process";

const repoRoot = process.cwd();
const appRoot = path.join(repoRoot, "Frontend", "antigal.client");
const srcRoot = path.join(appRoot, "src");
const entrypoint = path.join(srcRoot, "index.js");
const packageJsonPath = path.join(appRoot, "package.json");

const packageJson = JSON.parse(fs.readFileSync(packageJsonPath, "utf8"));
const declared = new Set([
  ...Object.keys(packageJson.dependencies ?? {}),
  ...Object.keys(packageJson.devDependencies ?? {}),
]);

const sourceExtensions = [".js", ".jsx", ".mjs", ".cjs"];
const modulePattern =
  /\b(?:import\s+(?:[^"'\x60]*?\s+from\s+)?|export\s+[^"'\x60]*?\s+from\s+|require\s*\(|import\s*\()\s*["']([^"'\x60]+)["']/g;

function packageName(specifier) {
  if (specifier.startsWith("@")) {
    return specifier.split("/").slice(0, 2).join("/");
  }
  return specifier.split("/")[0];
}

function resolveLocal(importer, specifier) {
  const base = path.resolve(path.dirname(importer), specifier);
  const candidates = [
    base,
    ...sourceExtensions.map((extension) => `${base}${extension}`),
    ...sourceExtensions.map((extension) => path.join(base, `index${extension}`)),
  ];

  for (const candidate of candidates) {
    if (fs.existsSync(candidate) && fs.statSync(candidate).isFile()) {
      return candidate;
    }
  }

  return null;
}

const visited = new Set();
const queue = [entrypoint];
const externalUses = new Map();
const unresolvedLocal = [];

while (queue.length > 0) {
  const file = queue.shift();
  if (visited.has(file)) continue;
  visited.add(file);

  const source = fs.readFileSync(file, "utf8");
  modulePattern.lastIndex = 0;

  let match;
  while ((match = modulePattern.exec(source))) {
    const specifier = match[1];

    if (specifier.startsWith(".") || specifier.startsWith("/")) {
      const resolved = resolveLocal(file, specifier);
      if (resolved && sourceExtensions.includes(path.extname(resolved))) {
        queue.push(resolved);
      } else if (!resolved && !/\.(css|scss|sass|svg|png|jpe?g|gif|webp|woff2?|ttf|ico)$/i.test(specifier)) {
        unresolvedLocal.push({
          importer: path.relative(repoRoot, file),
          specifier,
        });
      }
      continue;
    }

    if (specifier.startsWith("node:")) continue;

    const dependency = packageName(specifier);
    if (!externalUses.has(dependency)) {
      externalUses.set(dependency, new Set());
    }
    externalUses.get(dependency).add(path.relative(repoRoot, file));
  }
}

const missing = [...externalUses.entries()]
  .filter(([dependency]) => !declared.has(dependency))
  .map(([dependency, importers]) => ({
    dependency,
    importers: [...importers].sort(),
  }))
  .sort((a, b) => a.dependency.localeCompare(b.dependency));

console.log(`frontend-production-modules=${visited.size}`);
console.log(
  "frontend-production-external-packages=" +
    [...externalUses.keys()].sort().join(",")
);

if (unresolvedLocal.length > 0) {
  console.error("Unresolved production-local imports:");
  for (const item of unresolvedLocal) {
    console.error(`- ${item.importer} -> ${item.specifier}`);
  }
  process.exitCode = 1;
}

if (missing.length > 0) {
  console.error("Undeclared production dependencies:");
  for (const item of missing) {
    console.error(
      `- ${item.dependency}: ${item.importers.join(", ")}`
    );
  }
  process.exitCode = 1;
}

if (!process.exitCode) {
  console.log("frontend-production-dependency-authority=clean");
}
