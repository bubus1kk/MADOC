import { copyFile, cp, mkdir } from "node:fs/promises";
import { fileURLToPath } from "node:url";

const projectRoot = fileURLToPath(new URL("../", import.meta.url));
const sourceRoot = fileURLToPath(
  new URL("../node_modules/pdfjs-dist/", import.meta.url),
);
const targetRoot = fileURLToPath(new URL("../ui/vendor/pdfjs/", import.meta.url));

await mkdir(targetRoot, { recursive: true });
await Promise.all([
  copyFile(`${sourceRoot}build/pdf.min.mjs`, `${targetRoot}pdf.min.js`),
  copyFile(
    `${sourceRoot}build/pdf.worker.min.mjs`,
    `${targetRoot}pdf.worker.min.js`,
  ),
  cp(`${sourceRoot}cmaps`, `${targetRoot}cmaps`, {
    recursive: true,
    force: true,
  }),
  cp(`${sourceRoot}standard_fonts`, `${targetRoot}standard_fonts`, {
    recursive: true,
    force: true,
  }),
  cp(`${sourceRoot}wasm`, `${targetRoot}wasm`, {
    recursive: true,
    force: true,
  }),
]);

console.log(`PDF.js assets prepared in ${targetRoot.replace(projectRoot, "")}`);
