import fs from "node:fs";
import path from "node:path";
import process from "node:process";

const repoRoot = process.cwd();
const appRoot = path.join(repoRoot, "Frontend", "antigal.client");
const srcRoot = path.join(appRoot, "src");
const entrypoint = path.join(srcRoot, "index.jsx");
const extensions = [".js", ".jsx", ".mjs", ".cjs"];

const importPattern =
  /\b(?:import\s+(?:[^"'\x60]*?\s+from\s+)?|export\s+[^"'\x60]*?\s+from\s+|require\s*\(|import\s*\()\s*["']([^"'\x60]+)["']/g;

function walk(dir) {
  return fs.readdirSync(dir, { withFileTypes: true }).flatMap((entry) => {
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) return walk(full);
    return [full];
  });
}

function resolveLocal(importer, specifier) {
  const base = path.resolve(path.dirname(importer), specifier);
  const candidates = [
    base,
    ...extensions.map((ext) => base + ext),
    ...extensions.map((ext) => path.join(base, "index" + ext)),
  ];
  return candidates.find(
    (candidate) => fs.existsSync(candidate) && fs.statSync(candidate).isFile(),
  ) ?? null;
}

const sourceFiles = walk(srcRoot)
  .filter((file) => extensions.includes(path.extname(file)))
  .sort();

const localEdges = new Map(sourceFiles.map((file) => [file, new Set()]));
const reverseEdges = new Map(sourceFiles.map((file) => [file, new Set()]));
const unresolved = [];

for (const file of sourceFiles) {
  const source = fs.readFileSync(file, "utf8");
  importPattern.lastIndex = 0;
  let match;
  while ((match = importPattern.exec(source))) {
    const specifier = match[1];
    if (!specifier.startsWith(".") && !specifier.startsWith("/")) continue;

    const resolved = resolveLocal(file, specifier);
    if (!resolved) {
      if (!/\.(css|scss|sass|svg|png|jpe?g|gif|webp|woff2?|ttf|ico|json)$/i.test(specifier)) {
        unresolved.push({
          importer: path.relative(repoRoot, file),
          specifier,
        });
      }
      continue;
    }

    if (!localEdges.has(resolved)) continue;
    localEdges.get(file).add(resolved);
    reverseEdges.get(resolved).add(file);
  }
}

const reachable = new Set();
const queue = [entrypoint];
while (queue.length) {
  const file = queue.shift();
  if (!localEdges.has(file) || reachable.has(file)) continue;
  reachable.add(file);
  for (const target of localEdges.get(file)) queue.push(target);
}

const emptyFiles = sourceFiles
  .filter((file) => fs.readFileSync(file, "utf8").trim().length === 0)
  .map((file) => ({
    path: path.relative(repoRoot, file),
    reachable: reachable.has(file),
    importers: [...(reverseEdges.get(file) ?? [])]
      .map((importer) => path.relative(repoRoot, importer))
      .sort(),
  }));

const unreachableFiles = sourceFiles
  .filter((file) => !reachable.has(file))
  .map((file) => path.relative(repoRoot, file));

console.log("frontend-source-files=" + sourceFiles.length);
console.log("frontend-reachable-source-files=" + reachable.size);
console.log("frontend-unreachable-source-files=" + unreachableFiles.length);
console.log("frontend-empty-source-files=" + emptyFiles.length);
console.log("frontend-unresolved-local-imports=" + unresolved.length);

console.log("--- EMPTY_SOURCE_FILES ---");
for (const item of emptyFiles) {
  console.log(
    JSON.stringify({
      path: item.path,
      reachable: item.reachable,
      importers: item.importers,
    }),
  );
}

console.log("--- UNREACHABLE_SOURCE_FILES ---");
for (const file of unreachableFiles) console.log(file);

if (unresolved.length) {
  console.error("--- UNRESOLVED_LOCAL_IMPORTS ---");
  for (const item of unresolved) console.error(JSON.stringify(item));
  process.exitCode = 1;
}
