# Antigal · e-commerce de alimentos saludables

[![Quality](https://github.com/Enzopinotti/2024-UTN-GRUPO-2/actions/workflows/quality.yml/badge.svg?branch=main)](https://github.com/Enzopinotti/2024-UTN-GRUPO-2/actions/workflows/quality.yml)
[![Security](https://github.com/Enzopinotti/2024-UTN-GRUPO-2/actions/workflows/security-baseline.yml/badge.svg?branch=main)](https://github.com/Enzopinotti/2024-UTN-GRUPO-2/actions/workflows/security-baseline.yml)

Proyecto académico full stack desarrollado originalmente en 2024 para **Antigal**, una dietética nacida en el Mercado Municipal de Ensenada. El repositorio fue retomado y modernizado en 2026 para llevar una base histórica de React + ASP.NET Core a un stack mantenido, testeado y con CI permanente.

> **Estado actual:** modernización 2026 cerrada en **B46**. El repositorio queda en modo mantenimiento: frontend y backend compilan y testean en CI con **0 warnings**, auditorías de dependencias limpias y gates permanentes que protegen las decisiones cerradas.

## Qué incluye

La aplicación cubre el flujo principal de un e-commerce:

- catálogo, búsqueda, filtros, detalle y productos destacados;
- categorías y asociación producto/categoría;
- carrito y confirmación de carrito como orden;
- checkout, pagos con Mercado Pago, órdenes, ventas y envíos;
- registro, login, JWT, confirmación de email y recuperación de contraseña;
- perfil de usuario, direcciones, órdenes y favoritos;
- carga y gestión de imágenes con Cloudinary;
- formulario de contacto y bandeja administrativa de mensajes;
- panel administrativo para productos, categorías, usuarios y mensajes;
- importación de productos desde Excel;
- rutas públicas, rutas autenticadas y operaciones protegidas por rol Admin.

## Stack actual

### Frontend

- React **19.3.0**
- React Router **7.18.4**
- Vite **8.2.2**
- Vitest **5.0.1**
- Testing Library
- Sass nativo
- ESLint **10**
- React Hook Form + Yup
- React Toastify / SweetAlert2

Runtime fijado por el repositorio:

- Node.js **24.20.0** mediante `.nvmrc`
- npm instalado con el runtime de Node usado en CI

### Backend

- ASP.NET Core / .NET **10**
- SDK fijado en **10.0.401** mediante `global.json`
- Entity Framework Core + SQL Server
- ASP.NET Core Identity
- JWT
- FluentValidation + SharpGrip auto-validation
- CloudinaryDotNet
- Mercado Pago SDK
- MailKit / MimeKit mediante el proyecto `EmailService`
- NPOI para importación Excel
- Swashbuckle / OpenAPI

La solución mantenida usa:

`Backend/antigal.server.slnx`

## Arquitectura

La solución se divide en:

```text
.
├── Backend/
│   ├── antigal.server/          # API ASP.NET Core
│   ├── antigal.server.Tests/    # tests MSTest
│   ├── EmailService/            # envío de emails
│   └── antigal.server.slnx
├── Frontend/
│   └── antigal.client/          # React + Vite
├── docs/
│   └── modernization-2026/      # evidencia y decisiones de modernización
├── scripts/                     # gates de arquitectura / seguridad / calidad
├── .github/workflows/           # CI
├── .nvmrc
└── global.json
```

En el backend, las responsabilidades de persistencia fueron explicitándose durante la modernización:

- `UnitOfWork` coordina repositorios y la frontera transaccional explícita;
- los repositorios son dueños de la persistencia de sus mutaciones;
- `IUnitOfWork.SaveChangesAsync()` fue retirado al quedar sin consumidores productivos;
- `LikeService` ya delega en `IUnitOfWork.Likes` / `LikeRepository`;
- los servicios y controllers mantenidos ya no consumen `AppDbContext` directamente;
- `ImageService` conserva Cloudinary como integración externa y delega persistencia a `IUnitOfWork.Images` / `ImageRepository`;
- CI exige exactamente **0** consumidores de `AppDbContext` y **0** `SaveChangesAsync` fuera de repositories.

## Requisitos

Para trabajar con el estado mantenido actual:

- Node.js **24.20.0**
- .NET SDK **10.0.401**
- SQL Server accesible desde el backend
- credenciales/configuración local para:
  - JWT
  - Cloudinary
  - SMTP
  - Mercado Pago
  - cadena de conexión SQL Server

Los valores sensibles no están versionados en `appsettings.json`; el archivo conserva las claves esperadas con valores vacíos.

### Migraciones de base de datos

Los cambios de esquema mantenidos están versionados en `Backend/antigal.server/Migrations/`. Desde B36, la integridad de favoritos depende de la migración `20260921141500_LikeConcurrencyIntegrity`. Una base existente debe aplicar las migraciones pendientes como parte de su proceso de despliegue **antes** de recibir escrituras con el nuevo código.

## Configuración backend

Completar localmente `Backend/antigal.server/appsettings.json` o usar el mecanismo de configuración de ASP.NET Core apropiado para el entorno.

Se requieren estas secciones:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "JWTSettings": {
    "securityKey": "...",
    "validIssuer": "...",
    "validAudience": "...",
    "expiryInMinutes": 60
  },
  "Cloudinary": {
    "CloudName": "...",
    "ApiKey": "...",
    "ApiSecret": "..."
  },
  "EmailConfiguration": {
    "From": "...",
    "Port": 587,
    "Username": "...",
    "Password": "...",
    "SmtpServer": "..."
  },
  "MercadoPago": {
    "AccessToken": "..."
  }
}
```

### Nota sobre bootstrap de desarrollo

El bootstrap de administrador está **deshabilitado por defecto**, no contiene credenciales embebidas y sólo puede habilitarse explícitamente en entorno Development mediante configuración externa. Una configuración incompleta o un intento de habilitarlo fuera de Development falla de forma cerrada durante el arranque.

## Ejecutar el frontend

Desde la raíz:

```bash
cd Frontend/antigal.client
npm ci
npm run dev
```

Vite está configurado para usar el puerto **3000**.

Otros comandos útiles:

```bash
npm run lint
npm test
npm run build
```

El build de producción se genera en:

`Frontend/antigal.client/build`

## Ejecutar el backend

Restaurar primero la solución:

```bash
dotnet restore Backend/antigal.server.slnx
```

Ejecutar la API:

```bash
dotnet run --project Backend/antigal.server/antigal.server.csproj
```

El endpoint/puerto efectivo depende de la configuración local de ASP.NET Core; el repositorio no versiona un `launchSettings.json` como autoridad de entorno.

Para validar todo el backend:

```bash
dotnet build Backend/antigal.server.slnx -c Release
dotnet test Backend/antigal.server.Tests/antigal.server.Tests.csproj -c Release
```

## Rutas principales del frontend

Entre las rutas mantenidas están:

- `/`
- `/products`
- `/products/:id`
- `/cart`
- `/checkout`
- `/login`
- `/register`
- `/profile`
- `/profile/orders`
- `/profile/favorites`
- `/profile/addresses`
- `/admin/*`
- `/contacto`
- `/sobre-nosotros`
- `/tienda-fisica`

Las vistas pesadas se cargan con `React.lazy` + `Suspense` para mantener el entry bundle dentro de los límites controlados por CI.

## API

La API expone módulos para:

- Accounts / Identity
- Products
- Categories
- Product categories
- Cart
- Orders (incluye la creación transaccional de ventas)
- Payments
- Shipping / Envios
- Likes / favoritos
- Images
- Contact messages
- Admin operations

Swagger/OpenAPI está habilitado en entorno Development.

## Calidad y CI

El repositorio mantiene dos workflows principales:

### Quality

Valida, entre otras cosas:

- Node y .NET fijados;
- manifest/lockfile del frontend;
- autoridad Vite/Vitest y ausencia de tooling CRA;
- dependencias directas permitidas;
- extensiones JSX;
- fuente frontend muerta;
- Sass nativo;
- lint con cero warnings;
- tests frontend;
- build Vite y límites de chunks;
- restore/build de .NET;
- **0 warnings del compilador C#**;
- tests backend;
- auditoría npm;
- auditoría de vulnerabilidades NuGet;
- gates de arquitectura backend.

### Current-tree security baseline

Valida el árbol actual contra la política de seguridad versionada en `scripts/security_baseline.py`.

Las GitHub Actions de terceros están fijadas por SHA y usan runtimes Node 24 mantenidos.

## Checkpoint de validación

En el cierre funcional **B46** del carril de modernización:

- frontend: **63/63 tests**
- backend: **93/93 tests**
- C# Release: **0 warnings**
- npm audit: **0 vulnerabilidades**
- NuGet vulnerability audit: **clean**
- `OrdersController`: **2/2 rutas Admin**, **0** acciones anónimas
- `IOrderService`: **3** operaciones mantenidas
- `IOrderRepository`: **4** operaciones mantenidas
- `ImageController`: **3/3 rutas Admin**, **0** acciones anónimas
- panel Admin de productos: **5** mutaciones con transporte Bearer autenticado

La evidencia detallada de cada bloque está en:

`docs/modernization-2026/`

## Modernización 2026

El trabajo se hizo incrementalmente y con validación antes de cada promoción. Entre los cambios ya cerrados se encuentran:

- migración del frontend a Vite/Vitest;
- actualización a React 19 y Router 7;
- eliminación de tooling y paquetes muertos;
- Sass nativo;
- code splitting y límites de bundle;
- migración backend a .NET 10 y solución SLNX;
- actualización conservadora de dependencias backend;
- auditorías de seguridad limpias;
- CI con actions mantenidas/pinneadas;
- limpieza de DI, JWT y modelos duplicados;
- consolidación de autoridad de repositorios y persistencia;
- eliminación de saves redundantes;
- protección de la transacción de confirmación de órdenes;
- retiro del contrato muerto `IUnitOfWork.SaveChangesAsync()`;
- reducción y allowlist de accesos directos a `AppDbContext`;
- extracción de `LikeRepository` bajo `UnitOfWork`;
- extracción de `ContactoRepository` + `ContactoService`, dejando cero controllers con acceso directo a `AppDbContext`;
- bootstrap de administrador externalizado, deshabilitado por defecto, Development-only y fail-closed;
- extracción de persistencia de imágenes a `ImageRepository`, dejando en **0** los consumidores directos de `AppDbContext` fuera de repositories;
- integridad concurrente de favoritos mediante índice único filtrado `(UserId, ProductoId)`, deduplicación de datos históricos y manejo idempotente del conflicto;
- retiro del endpoint/DTO de email de prueba públicamente alcanzable, conservando intactos confirmación y recuperación de contraseña;
- protección Admin de las mutaciones producto/categoría, manteniendo públicos sólo sus dos GET de catálogo;
- autenticación obligatoria para crear preferencias de pago, con identidad derivada del JWT `sub`;
- retiro completo del callback de pago legado que permitía mutar estados desde `paymentId` + `status` aportados por el caller; no se expone webhook público hasta contar con verificación real de Mercado Pago;
- autenticación + ownership explícito en las seis rutas de carrito: el `userId` de la ruta debe coincidir con el `sub` autenticado del JWT;
- retiro de la API paralela de ventas (`SaleController` / `SaleService` / DTOs) sin tocar la creación transaccional de ventas desde `OrderService`; `ISaleRepository` queda create-only;
- protección Admin por defecto de las dos rutas mantenidas de `OrdersController`;
- reducción de `IOrderService` a 3 operaciones y de `IOrderRepository` a 4 operaciones realmente consumidas, preservando la transacción de confirmación;
- retiro de la rama backend permanentemente deshabilitada en `Mis Pedidos`; la vista declara explícitamente su fuente demo local y CI prueba que no realiza llamadas de red;
- transporte autenticado reutilizable para mutaciones Admin de productos/imágenes, con fail-closed sin token;
- `ImageController` protegido por rol Admin y contrato de upload frontend alineado con la respuesta real `{ id, url }`.

Para decisiones, evidencia, SHAs y runs concretos, ver el índice de modernización.

## Estado de mantenimiento y backlog de producto

La modernización 2026 queda cerrada. Nuevos cambios deben preservar los gates actuales y justificarse como mantenimiento o producto, no como continuación automática del carril.

El backlog histórico de integración frontend/backend está separado en **issue #42 — Post-modernization product integration backlog**. Allí quedan documentados, entre otros, perfil demo-local, Admin Users/Messages, mutaciones de categorías todavía locales, foto de perfil sin backend, contacto con endpoint histórico y recuperación de contraseña desalineada.

También permanecen decisiones deliberadas:

- un futuro endpoint real de `Mis Pedidos` debe ser owner-bound al JWT, no reutilizar la superficie Admin de Orders;
- un webhook real de Mercado Pago requiere verificación de autenticidad y resolución de estado canónico;
- NPOI permanece en **2.7.6** porque una actualización posterior produjo regresiones de comportamiento medidas.

Para retomar el repositorio, empezar desde `main`, revisar el issue #42 y mantener verdes Quality + Current-tree security.

## Origen del proyecto

Antigal es una dietética nacida en 2022 en el Mercado Municipal de Ensenada, con foco en alimentos saludables, productos naturales y una identidad vinculada a las raíces del NOA.

Este repositorio conserva el origen académico de 2024, pero su rama mantenida fue modernizada en 2026 con foco en calidad de código, seguridad, mantenibilidad y trazabilidad de los cambios.
