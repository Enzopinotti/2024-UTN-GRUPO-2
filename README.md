# Antigal · e-commerce de alimentos saludables

[![Quality](https://github.com/Enzopinotti/2024-UTN-GRUPO-2/actions/workflows/quality.yml/badge.svg?branch=main)](https://github.com/Enzopinotti/2024-UTN-GRUPO-2/actions/workflows/quality.yml)
[![Security](https://github.com/Enzopinotti/2024-UTN-GRUPO-2/actions/workflows/security-baseline.yml/badge.svg?branch=main)](https://github.com/Enzopinotti/2024-UTN-GRUPO-2/actions/workflows/security-baseline.yml)

Proyecto académico full stack desarrollado originalmente en 2024 para **Antigal**, una dietética nacida en el Mercado Municipal de Ensenada. El repositorio fue retomado y modernizado en 2026 para llevar una base histórica de React + ASP.NET Core a un stack mantenido, testeado y con CI permanente.

> **Estado actual:** modernization checkpoint B34. El frontend y backend compilan y testean en CI con una política de **0 warnings**, auditorías de dependencias limpias y gates de arquitectura que protegen decisiones de modernización ya cerradas.

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
- el único consumidor de `AppDbContext` fuera de repositories que queda permitido por CI es `ImageService`;
- `ImageService` usa un único flush de base por operación mutante.

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
- Orders
- Sales
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

En el cierre **B34** del carril de modernización:

- frontend: **60/60 tests**
- backend: **57/57 tests**
- C# Release: **0 warnings**
- npm audit: **0 vulnerabilidades**
- NuGet vulnerability audit: **clean**
- PRs abiertos al cierre: **0**
- carrier de modernización: **231 commits ahead / 0 behind** respecto del `main` histórico previo a la promoción

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
- bootstrap de administrador externalizado, deshabilitado por defecto, Development-only y fail-closed.

Para decisiones, evidencia, SHAs y runs concretos, ver el índice de modernización.

## Deuda conocida / próximos cortes

El estado actual es mucho más mantenible que el histórico, pero todavía hay trabajo explícito:

- seguir evaluando si `ImageService` debe conservar acceso directo a `AppDbContext`;
- caracterizar duplicados existentes antes de agregar una restricción única `(UserId, ProductoId)` en Likes;
- continuar actualización conservadora de dependencias mayores que quedaron deliberadamente fuera de los bloques previos;
- NPOI permanece en **2.7.6** porque una actualización posterior produjo regresiones de comportamiento medidas.

## Origen del proyecto

Antigal es una dietética nacida en 2022 en el Mercado Municipal de Ensenada, con foco en alimentos saludables, productos naturales y una identidad vinculada a las raíces del NOA.

Este repositorio conserva el origen académico de 2024, pero su rama mantenida fue modernizada en 2026 con foco en calidad de código, seguridad, mantenibilidad y trazabilidad de los cambios.
