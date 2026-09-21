import fs from "node:fs";

const helperPath = "Frontend/antigal.client/src/utils/authenticatedFetch.js";
const helperTestPath = "Frontend/antigal.client/src/utils/authenticatedFetch.test.js";
const productsPath = "Frontend/antigal.client/src/components/admin/products/ProductListContainer.jsx";

const helper = fs.readFileSync(helperPath, "utf8");
const helperTests = fs.readFileSync(helperTestPath, "utf8");
const products = fs.readFileSync(productsPath, "utf8");

const failures = [];

for (const required of [
  'localStorage.getItem("accessToken")',
  'headers.set("Authorization", `Bearer ${accessToken}`)',
  "return fetch(input,",
]) {
  if (!helper.includes(required)) failures.push("authenticatedFetch authority missing: " + required);
}

for (const required of [
  "fails closed before making a request when the access token is absent",
  "adds the persisted Bearer token while preserving caller headers",
  "expect(fetch).not.toHaveBeenCalled()",
  'expect(options.headers.get("Authorization")).toBe("Bearer jwt-access-token")',
]) {
  if (!helperTests.includes(required)) failures.push("authenticatedFetch runtime proof missing: " + required);
}

if (!products.includes("import { authenticatedFetch } from '../../../utils/authenticatedFetch';")) {
  failures.push("Admin products authenticatedFetch import missing");
}

const protectedCalls = [
  "authenticatedFetch('https://www.antigal.somee.com/api/Product/addProduct'",
  "authenticatedFetch('https://www.antigal.somee.com/api/Product/updateProduct'",
  "authenticatedFetch(`https://www.antigal.somee.com/api/Product/deleteProduct/${idProducto}`",
];

for (const required of protectedCalls) {
  if (!products.includes(required)) failures.push("Admin product protected transport missing: " + required);
}

const imageUploadCalls = (products.match(/authenticatedFetch\('https:\/\/www\.antigal\.somee\.com\/api\/Image\/upload'/g) || []).length;
if (imageUploadCalls !== 2) failures.push("Expected 2 authenticated Image upload calls, found " + imageUploadCalls);

const totalAuthenticatedCalls = (products.match(/authenticatedFetch\(/g) || []).length;
if (totalAuthenticatedCalls !== 5) failures.push("Expected 5 Admin authenticated mutation calls, found " + totalAuthenticatedCalls);

if (!products.includes("fetch('https://www.antigal.somee.com/api/Product/getProducts')")) {
  failures.push("Public Product/getProducts fetch must remain unauthenticated");
}

for (const forbidden of [
  "fetch('https://www.antigal.somee.com/api/Product/addProduct'",
  "fetch('https://www.antigal.somee.com/api/Product/updateProduct'",
  "fetch(`https://www.antigal.somee.com/api/Product/deleteProduct/${idProducto}`",
  "const uploadResponse = await fetch('https://www.antigal.somee.com/api/Image/upload'",
  "uploadData.isSuccess",
]) {
  if (products.includes(forbidden)) failures.push("Retired Admin product/image contract returned: " + forbidden);
}

const uploadUrlChecks = (products.match(/if \(!uploadData\.url\)/g) || []).length;
if (uploadUrlChecks !== 2) failures.push("Expected 2 Image upload URL contract checks, found " + uploadUrlChecks);

console.log("frontend-auth-token-source=localStorage.accessToken");
console.log("frontend-auth-header=Bearer");
console.log("admin-product-authenticated-mutation-count=5");
console.log("admin-product-public-read=Product/getProducts");
console.log("admin-image-upload-response-authority=url");

if (failures.length) {
  console.error("Frontend authenticated API authority failed:");
  for (const failure of failures) console.error("- " + failure);
  process.exit(1);
}

console.log("frontend-authenticated-api-authority=clean");
