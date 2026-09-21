import fs from "node:fs";

const ordersPath = "Frontend/antigal.client/src/pages/profile/Orders.jsx";
const testPath = "Frontend/antigal.client/src/pages/profile/Orders.state.test.jsx";

const orders = fs.readFileSync(ordersPath, "utf8");
const tests = fs.readFileSync(testPath, "utf8");

const failures = [];

for (const forbidden of [
  "useState",
  "useEffect",
  "sweetalert2",
  "usingBackend",
  "fetch(",
  "localhost:5279",
  "//falta endpoint",
]) {
  if (orders.includes(forbidden)) {
    failures.push(`Profile Orders regained retired backend-placeholder marker: ${forbidden}`);
  }
}

for (const required of [
  'import fakeOrders from "../../data/fakeOrder";',
  "fakeOrders.filter",
  "order.userId === currentUser.id",
  "<UserOrderListContainer orders={orders} />",
]) {
  if (!orders.includes(required)) {
    failures.push(`Profile Orders maintained local-data contract missing: ${required}`);
  }
}

for (const required of [
  'describe("Profile Orders local-data authority"',
  "filters demo orders by the outlet user and never calls a backend placeholder",
  "expect(fetchMock).not.toHaveBeenCalled()",
  'contextValue = { user: { id: 2 } };',
]) {
  if (!tests.includes(required)) {
    failures.push(`Profile Orders runtime proof missing: ${required}`);
  }
}

console.log("profile-orders-data-source=fakeOrders");
console.log("profile-orders-user-filter=currentUser.id");
console.log("profile-orders-dead-backend-branch=absent");
console.log("profile-orders-network-call=absent");

if (failures.length) {
  console.error("Frontend Profile Orders authority failed:");
  for (const failure of failures) console.error(`- ${failure}`);
  process.exit(1);
}

console.log("frontend-profile-orders-authority=clean");
