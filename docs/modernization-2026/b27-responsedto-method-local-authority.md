# B27 — ResponseDto method-local authority

## Goal

B26 left ProductService with one remaining unusual dependency:
a mutable `ResponseDto` injected by DI and stored as service state, while most
backend call paths allocated response DTOs locally.

B27 characterizes the entire backend and removes only that exceptional DI
authority.

## Starting authority

Carrier before B27:

`0eb4831604f2ee0a92e09c982e1756c888617e8d`

B26 closure:

- Quality `35561678162` — success
- Current-tree security `35561678221` — success
- frontend tests: 60/60
- backend tests: 32/32
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean

## Characterization

Disposable characterization commit:

`c8723f690966f9985c8c6667e6225b70b7117110`

Characterization run:

`35561803699` — success

Exact-tree findings:

- `ResponseDto` DI registration: present
- constructor consumers: exactly 1
- field consumers: exactly 1
- both consumers were `ProductService`
- local `new ResponseDto` allocations: 46
- local-allocation files: 8

The local allocations were distributed across controllers, repositories and
services. This proved that DI was not the backend response authority; it was a
single ProductService exception.

## Maintained change

Implementation commit:

`5329ae09b7eabba4e6ae56cf81b6623e2ece056b`

### ProductService

Removed:

- `private readonly ResponseDto _response`
- `ResponseDto response` constructor dependency

The constructor now depends only on:

`IUnitOfWork`

`GetProducts` now allocates a fresh local `ResponseDto`, matching the
dominant backend pattern and the other ProductService methods.

No response payload, messages, product queries, routes or status semantics were
changed.

### Program.cs

Removed:

`AddTransient<ResponseDto>()`

The DTO type remains fully active as a normal response model. Only its
container registration was removed.

## Permanent authority

Added:

`scripts/backend_response_dto_authority.py`

Quality now requires:

- no Transient/Scoped/Singleton `ResponseDto` registration;
- no production constructor injection of `ResponseDto`;
- no production `ResponseDto` service fields;
- ProductService to allocate its response method-locally;
- ProductService not to regain `_response` shared state.

Current authority output:

- `responsedto-di-registration=absent`
- `responsedto-constructor-consumer-count=0`
- `responsedto-field-consumer-count=0`
- `responsedto-local-allocation-count=47`
- `responsedto-local-allocation-file-count=8`
- `productservice-response-authority=method-local`
- `backend-responsedto-authority=clean`

## Behavioral coverage

The ProductService repository tests were updated for the new constructor and
two additional behavior tests were added.

The new tests prove:

1. repeated `GetProducts` calls return distinct ResponseDto instances;
2. caller mutation of one response does not leak into the next;
3. repository exceptions produce a fresh failure response with the historical
   error message;
4. ProductService remains free of a direct ProductRepository dependency.

Backend maintained test count increases from 32 to 34.

## Lab validation

Quality `35561877021` — success

Current-tree security `35561876956` — success

Regression baseline:

- frontend: 60/60
- backend: 34/34
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean

## Explicit non-goals

B27 does not:

- redesign ResponseDto itself;
- replace response DTOs with Results/ActionResult wrappers;
- alter controller contracts;
- change HTTP status handling;
- modify repository behavior;
- alter ProductService repository ownership;
- change database, schema or external integrations.

## Rollback boundary

Rollback requires only restoring the transient ResponseDto registration and
the ProductService constructor/field usage. There are no schema, data or
external-service migrations.

Temporary characterization workflow and lab triggers are removed before
carrier promotion.


## Carrier promotion evidence

The cleaned B27 tree was promoted by non-forced fast-forward.

Promoted carrier SHA:

`e88b7345f410aa2b68114f9b10011a2f9bbeb06b`

Exact-SHA carrier validation:

- Quality `35561978213` — success
- Current-tree security `35561978265` — success
- frontend tests: 60/60
- backend tests: 34/34
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean
- `responsedto-di-registration=absent`
- `responsedto-constructor-consumer-count=0`
- `responsedto-field-consumer-count=0`
- `responsedto-local-allocation-count=47`
- `productservice-response-authority=method-local`

A documentation-only closure commit follows this promotion evidence. B27 is
closed only after permanent Quality and Current-tree security pass again on that
exact closure SHA.
