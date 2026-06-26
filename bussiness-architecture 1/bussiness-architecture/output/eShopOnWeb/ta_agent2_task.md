I'm doing a deeper technology architecture analysis of a codebase, building
on an earlier "Stack Scout" inventory pass. That pass produced 6 inventory
files, included below as ground truth context, followed by deeper repo files
for pattern/NFR/risk analysis (including CI/CD pipeline files for the
Operational Architecture Assessment).

Please write each of the 9 output files below directly to disk, using your
file tools, as soon as each one is ready — don't wait until the end, and
don't print file contents in your reply. Save each file to:

  C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/ta-outputs/<filename>

e.g. C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/ta-outputs/technology-stack-assessment.md

Once all 9 files are written, reply with a short checklist (one line per
filename, ✅ written or ❌ skipped with a reason).

--- STACK SCOUT INVENTORY FILES (from the earlier pass) ---

### technology-stack-inventory.md
```
## Technology Stack Inventory

| Component Name | Version | Category | Layer | Package Manager / Source | Source File | Confidence |
|---|---|---|---|---|---|---|
| .NET SDK/Runtime | 8.0.x | Runtime / SDK | Build | dotnet CLI | .github/workflows/dotnetcore.yml | HIGH |
| mcr.microsoft.com/dotnet/sdk | 8.0 | Container Base Image (build stage) | Build | Docker Hub (MCR) | src/Web/Dockerfile, src/PublicApi/Dockerfile | HIGH |
| mcr.microsoft.com/dotnet/aspnet | 8.0 | Container Base Image (runtime stage) | Infrastructure | Docker Hub (MCR) | src/Web/Dockerfile, src/PublicApi/Dockerfile | HIGH |
| mcr.microsoft.com/azure-sql-edge | (not declared / latest) | Container Base Image / Database | Data | Docker Hub (MCR) | docker-compose.yml | LOW - no tag declared, defaults to latest |
| Microsoft.NET.Sdk.Web | (project SDK, no version) | Web App SDK | Application | NuGet/MSBuild SDK | src/Web/Web.csproj, src/PublicApi/PublicApi.csproj | HIGH |
| Microsoft.NET.Sdk.BlazorWebAssembly | (project SDK, no version) | Frontend Framework SDK | Application | NuGet/MSBuild SDK | src/BlazorAdmin/BlazorAdmin.csproj | HIGH |
| Ardalis.GuardClauses | (not declared) | Guard Clause Library | Application | nuget | src/ApplicationCore/ApplicationCore.csproj | LOW - VERSION UNKNOWN, central package mgmt (Directory.Packages.props referenced but not provided) |
| Ardalis.Result | (not declared) | Result/Outcome Library | Application | nuget | src/ApplicationCore/ApplicationCore.csproj | LOW - VERSION UNKNOWN |
| Ardalis.Specification | (not declared) | Specification Pattern Library | Application | nuget | src/ApplicationCore/ApplicationCore.csproj, src/Web/Web.csproj | LOW - VERSION UNKNOWN |
| System.Security.Claims | (not declared) | Identity/Claims Library | Security | nuget | src/ApplicationCore/ApplicationCore.csproj | LOW - VERSION UNKNOWN |
| System.Text.Json | (not declared) | JSON Serialization | Application | nuget | src/ApplicationCore/ApplicationCore.csproj | LOW - VERSION UNKNOWN |
| Blazored.LocalStorage | (not declared) | Browser Storage Library | Application | nuget | src/BlazorAdmin/BlazorAdmin.csproj | LOW - VERSION UNKNOWN |
| BlazorInputFile | (not declared) | File Upload Component | Application | nuget | src/BlazorAdmin/BlazorAdmin.csproj, src/BlazorShared/BlazorShared.csproj | LOW - VERSION UNKNOWN |
| Microsoft.AspNetCore.Components.Authorization | (not declared) | Auth Components | Security | nuget | src/BlazorAdmin/BlazorAdmin.csproj | LOW - VERSION UNKNOWN |
| Microsoft.AspNetCore.Components.WebAssembly | (not declared) | Blazor WASM Framework | Application | nuget | src/BlazorAdmin/BlazorAdmin.csproj | LOW - VERSION UNKNOWN |
| Microsoft.AspNetCore.Components.WebAssembly.Authentication | (not declared) | Blazor WASM Auth | Security | nuget | src/BlazorAdmin/BlazorAdmin.csproj | LOW - VERSION UNKNOWN |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | (not declared) | Blazor WASM Dev Server | Build | nuget | src/BlazorAdmin/BlazorAdmin.csproj 
... [truncated]
```

### component-service-map.md
```
## Component & Service Map

| Service / Component Name | Type | Exposed Port(s) | Communication Protocol(s) | Primary Technology | Source File | Notes |
|---|---|---|---|---|---|---|
| eshopwebmvc (Web) | API Service / Frontend App (ASP.NET Core MVC + Razor + Blazor Server host) | Container: 8080 (env ASPNETCORE_URLS); Host mapping 5106:8080; Dev: https://localhost:44315 | HTTP / HTTPS | .NET 8.0, ASP.NET Core MVC, Blazor Server hosting | src/Web/Dockerfile, docker-compose.yml, docker-compose.override.yml | depends_on: sqlserver (docker-compose.yml) |
| eshoppublicapi (PublicApi) | API Service (REST, Swagger-documented) | Dockerfile EXPOSE 80, 443; Container runtime: 8080 (env ASPNETCORE_URLS); Host mapping 5200:8080; Dev: https://localhost:5099 | HTTP / HTTPS | .NET 8.0, ASP.NET Core Web API, Swashbuckle | src/PublicApi/Dockerfile, docker-compose.yml, docker-compose.override.yml | LOW - Dockerfile EXPOSE (80/443) differs from compose port mapping (8080); depends_on: sqlserver |
| sqlserver | Database (Container) | 1433 (mapped 1433:1433) | TCP / TDS (SQL) | Azure SQL Edge (SQL Server-compatible) | docker-compose.yml | Provisioned for both eshopwebmvc and eshoppublicapi |
| BlazorAdmin | Frontend App (Blazor WebAssembly, served via Web host) | N/A (hosted, no direct container port) | HTTP / HTTPS | Blazor WebAssembly (.NET 8.0) | src/BlazorAdmin/BlazorAdmin.csproj, src/BlazorAdmin/wwwroot/appsettings*.json | Communicates with PublicApi (apiBase) and Web (webBase) per environment config |
| ApplicationCore | Class Library (domain/application layer) | N/A | N/A (in-process) | .NET 8.0 class library | src/ApplicationCore/ApplicationCore.csproj | Referenced by Infrastructure, PublicApi, Web (via project references) |
| BlazorShared | Class Library (shared models/validation) | N/A | N/A (in-process) | .NET class library, FluentValidation | src/BlazorShared/BlazorShared.csproj | Referenced by ApplicationCore and BlazorAdmin |
| Infrastructure | Class Library (data access / EF Core / Identity) | N/A | N/A (in-process) | EF Core (SQL Server, PostgreSQL, InMemory providers) | src/Infrastructure/Infrastructure.csproj | Referenced by PublicApi and Web |
| FunctionalTests | Test Project (in-process host via WebApplicationFactory) | N/A | N/A | xunit, Microsoft.AspNetCore.Mvc.Testing, EF Core InMemory | tests/FunctionalTests/FunctionalTests.csproj | References ApplicationCore, PublicApi, Web |
| IntegrationTests | Test Project | N/A | N/A | xunit, NSubstitute, EF Core InMemory | tests/IntegrationTests/IntegrationTests.csproj | References Infrastructure, UnitTests |
| UnitTests | Test Project | N/A | N/A | xunit, NSubstitute | tests/UnitTests/UnitTests.csproj | References ApplicationCore, Web |
| PublicApiIntegrationTests | Test Project (in-process host via WebApplicationFactory) | N/A | N/A | MSTest, Microsoft.AspNetCore.Mvc.Testing, coverlet | tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj | References PublicApi, Web; uses `UseOnlyInMemoryD
... [truncated]
```

### data-store-registry.md
```
## Data Store Registry

| Store Name | Category | Engine / Technology | Version | Declared Database / Collection Name | Connected Services (if detectable) | Source File | Confidence |
|---|---|---|---|---|---|---|---|
| sqlserver | Relational Database | Azure SQL Edge (SQL Server-compatible) | (not declared / latest) | Microsoft.eShopOnWeb.CatalogDb, Microsoft.eShopOnWeb.Identity | eshopwebmvc (Web), eshoppublicapi (PublicApi) | docker-compose.yml; src/Web/appsettings.Docker.json; src/PublicApi/appsettings.Docker.json | HIGH - image/service declared in compose; DB names from Docker connection strings |
| PostgreSQL (eShopCatalog) | Relational Database | PostgreSQL (via Npgsql.EntityFrameworkCore.PostgreSQL) | (not declared) | eShopCatalog | Web (CatalogConnection), PublicApi (CatalogConnection) | src/Web/appsettings.json; src/PublicApi/appsettings.json; src/Infrastructure/Infrastructure.csproj | LOW - VERSION UNKNOWN; no IaC declaration found, host=localhost:5432 implies external/local dev instance |
| PostgreSQL (eShopIdentity) | Relational Database | PostgreSQL (via Npgsql.EntityFrameworkCore.PostgreSQL) | (not declared) | eShopIdentity | Web (IdentityConnection), PublicApi (IdentityConnection) | src/Web/appsettings.json; src/PublicApi/appsettings.json; src/Infrastructure/Infrastructure.csproj | LOW - VERSION UNKNOWN; no IaC declaration found, host=localhost:5432 implies external/local dev instance |
| EF Core InMemory Database | In-Memory / Key-Value Store (test & fallback) | Microsoft.EntityFrameworkCore.InMemory | (not declared) | (not declared - provider only) | Infrastructure, PublicApi, Web, FunctionalTests, IntegrationTests, PublicApiIntegrationTests (`UseOnlyInMemoryDatabase: true`) | src/Infrastructure/Infrastructure.csproj; src/PublicApi/PublicApi.csproj; src/Web/Web.csproj; tests/PublicApiIntegrationTests/appsettings.test.json | LOW - VERSION UNKNOWN; SHARED COMPONENT across Application and Test layers |

---

### VERSION / TECHNOLOGY CONFLICT
- **Primary relational database technology conflict**: `src/Web/appsettings.json` and `src/PublicApi/appsettings.json` (default config) declare PostgreSQL connection strings (`Host=localhost;Port=5432;...`) for `CatalogConnection` and `IdentityConnection`, while `src/Web/appsettings.Docker.json` and `src/PublicApi/appsettings.Docker.json` declare SQL Server connection strings for the same keys (`CatalogConnection`, `IdentityConnection`) pointing at the `sqlserver` (Azure SQL Edge) container declared in `docker-compose.yml`. `src/Infrastructure/Infrastructure.csproj` references both `Npgsql.EntityFrameworkCore.PostgreSQL` and (via PublicApi/Web) `Microsoft.EntityFrameworkCore.SqlServer`. Agent 2 to determine which engine is authoritative per environment (default/local vs Docker) and whether this is intentional dual-provider support or drift.

---

### Chunk Inventory - Data Store Registry
- Technology components found this chunk: Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.EntityFrameworkCo
... [truncated]
```

### infrastructure-deployment-blueprint.md
```
## Infrastructure & Deployment Blueprint

### Compute & Container Resources

| Resource Name | Resource Type | Platform / Provider | Image / Runtime Version | Environments Declared | Key Configuration (non-secret) | Source File | Confidence |
|---|---|---|---|---|---|---|---|
| eshopwebmvc | Container (Docker Compose service) | Docker | Build: mcr.microsoft.com/dotnet/sdk:8.0; Runtime: mcr.microsoft.com/dotnet/aspnet:8.0 | Docker (ASPNETCORE_ENVIRONMENT=Docker) | env: ASPNETCORE_ENVIRONMENT, ASPNETCORE_URLS; ports: 5106:8080; volumes: ~/.aspnet/https:/root/.aspnet/https:ro, ~/.microsoft/usersecrets:/root/.microsoft/usersecrets:ro | docker-compose.yml, docker-compose.override.yml, src/Web/Dockerfile | HIGH |
| eshoppublicapi | Container (Docker Compose service) | Docker | Build: mcr.microsoft.com/dotnet/sdk:8.0; Runtime: mcr.microsoft.com/dotnet/aspnet:8.0 | Docker (ASPNETCORE_ENVIRONMENT=Docker) | env: ASPNETCORE_ENVIRONMENT, ASPNETCORE_URLS; ports: 5200:8080; volumes: ~/.aspnet/https:/root/.aspnet/https:ro, ~/.microsoft/usersecrets:/root/.microsoft/usersecrets:ro; Dockerfile EXPOSE 80, EXPOSE 443 | docker-compose.yml, docker-compose.override.yml, src/PublicApi/Dockerfile | HIGH |
| sqlserver | Container (Docker Compose service) | Docker | mcr.microsoft.com/azure-sql-edge (no tag declared) | Docker | env: SA_PASSWORD, ACCEPT_EULA=Y; ports: 1433:1433 | docker-compose.yml | HIGH |
| Azure deployment environment (azd) | Resource Group / Subscription scope (parameters only) | Azure (via azd / Bicep, main.bicep not provided) | N/A | environmentName = `${AZURE_ENV_NAME}` | parameters: environmentName, location (`${AZURE_LOCATION}`), principalId (`${AZURE_PRINCIPAL_ID}`), sqlAdminPassword, appUserPassword (sourced via `secretOrRandomPassword` from `${AZURE_KEY_VAULT_NAME}`) | infra/main.parameters.json | LOW - parameters file present but no resource-defining Bicep/ARM template (main.bicep) provided in scan input |
| Azure resource naming convention map | Reference / Convention file | Azure (azd abbreviations) | N/A | N/A | Maps ~80 Azure resource type abbreviations (e.g. `kv-` Key Vault, `aks-` AKS, `redis-` Cache for Redis, `cosmos-` Cosmos DB, `app-` App Service, `sql-` SQL) - none of these resource types have corresponding resource declarations in the provided files | infra/abbreviations.json | LOW - inferred from naming convention reference only; no actual resource declarations found for any listed type |

---

### Environments Identified

| Environment Name | Trigger / Target | Source File |
|---|---|---|
| Development | Local development run (ASPNETCORE_ENVIRONMENT=Development, implicit) | src/Web/appsettings.Development.json, src/PublicApi/appsettings.Development.json, src/BlazorAdmin/wwwroot/appsettings.Development.json |
| Docker | Container run via docker-compose (ASPNETCORE_ENVIRONMENT=Docker) | docker-compose.override.yml, src/Web/appsettings.Docker.json, src/PublicApi/appsettings.Docker.json, src/BlazorAdmin/wwwroot/appsettings.Docker.json 
... [truncated]
```

### integration-dependency-graph.md
```
## Integration & Dependency Graph

### External Integrations

| Integration Name | Category | Protocol / Interface | Direction | Config Key / Env Var | Source File | Confidence |
|---|---|---|---|---|---|---|
| actions/checkout | CI-CD Action | GitHub Actions | CI-CD Pipeline | N/A | .github/workflows/dotnetcore.yml, .github/workflows/richnav.yml | HIGH |
| actions/setup-dotnet | CI-CD Action | GitHub Actions | CI-CD Pipeline | N/A (dotnet-version: '8.0.x') | .github/workflows/dotnetcore.yml, .github/workflows/richnav.yml | HIGH |
| microsoft/RichCodeNavIndexer | CI-CD Action (code indexing) | GitHub Actions | CI-CD Pipeline | repo-token: `${{ github.token }}`; languages: 'csharp'; environment: 'internal' | .github/workflows/richnav.yml | HIGH |
| Dependabot (NuGet) | Dependency Management | GitHub native integration | Inbound (automated PRs) | N/A | .github/dependabot.yml | HIGH |
| Azure Key Vault | Secrets Management / Cloud Service | Azure SDK (Azure.Identity, Azure.Extensions.AspNetCore.Configuration.Secrets) | Outbound | `AZURE_KEY_VAULT_NAME` (referenced for `sqlAdminPassword`, `appUserPassword`) | infra/main.parameters.json, src/Web/Web.csproj | LOW - referenced via env var / package reference only; no Key Vault resource declaration found |
| host.docker.internal (Web service) | Internal Cross-Container Reference | HTTP | Outbound | `baseUrls.webBase` | src/BlazorAdmin/wwwroot/appsettings.Docker.json | LOW - referenced via config key only; no explicit network/DNS declaration |

---

### Internal Service Dependencies (for multi-service / microservice projects)

| Caller Service | Target Service | Protocol | Dependency Type | Config Key / Env Var | Source File |
|---|---|---|---|---|---|
| BlazorAdmin | eshoppublicapi (PublicApi) | HTTP/HTTPS | Synchronous | `baseUrls.apiBase` | src/BlazorAdmin/wwwroot/appsettings.json, appsettings.Development.json, appsettings.Docker.json |
| BlazorAdmin | eshopwebmvc (Web) | HTTP/HTTPS | Synchronous | `baseUrls.webBase` | src/BlazorAdmin/wwwroot/appsettings.json, appsettings.Development.json, appsettings.Docker.json |
| eshopwebmvc (Web) | sqlserver / PostgreSQL (catalog data) | TCP / SQL | Synchronous | `ConnectionStrings:CatalogConnection` | src/Web/appsettings.json, src/Web/appsettings.Docker.json |
| eshopwebmvc (Web) | sqlserver / PostgreSQL (identity data) | TCP / SQL | Synchronous | `ConnectionStrings:IdentityConnection` | src/Web/appsettings.json, src/Web/appsettings.Docker.json |
| eshoppublicapi (PublicApi) | sqlserver / PostgreSQL (catalog data) | TCP / SQL | Synchronous | `ConnectionStrings:CatalogConnection` | src/PublicApi/appsettings.json, src/PublicApi/appsettings.Docker.json |
| eshoppublicapi (PublicApi) | sqlserver / PostgreSQL (identity data) | TCP / SQL | Synchronous | `ConnectionStrings:IdentityConnection` | src/PublicApi/appsettings.json, src/PublicApi/appsettings.Docker.json |
| eshopwebmvc | sqlserver | TCP | Synchronous (compose `depends_on`) | N/A | docker-compose.yml |
| eshopp
... [truncated]
```

### security-configuration-snapshot.md
```
## Security & Configuration Snapshot

### Authentication & Authorisation Mechanisms

| Mechanism Name | Type | Provider / Library | Scope | Config Key / Annotation | Source File | Confidence |
|---|---|---|---|---|---|---|
| JWT Bearer Tokens | Authentication | Microsoft.AspNetCore.Authentication.JwtBearer, System.IdentityModel.Tokens.Jwt | API (PublicApi), Web | N/A - package reference only, no issuer/audience/secret config keys found in provided appsettings | src/PublicApi/PublicApi.csproj, src/Web/Web.csproj | LOW - inferred from package references; no explicit JWT configuration block found |
| ASP.NET Core Identity | Authentication / Authorisation (Both) | Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.AspNetCore.Identity.UI | All (Web, PublicApi - identity store) | `ConnectionStrings:IdentityConnection` | src/Web/Web.csproj, src/PublicApi/PublicApi.csproj, src/Web/appsettings.json, src/PublicApi/appsettings.json | HIGH |
| Blazor WebAssembly Authentication | Authentication | Microsoft.AspNetCore.Components.WebAssembly.Authentication, Microsoft.Extensions.Identity.Core | Frontend (BlazorAdmin) | N/A - package reference only, no provider/authority config found | src/BlazorAdmin/BlazorAdmin.csproj | LOW - inferred from package references; no explicit auth provider/authority configuration found |
| Authorization Components | Authorisation | Microsoft.AspNetCore.Components.Authorization | Frontend (BlazorAdmin) | N/A | src/BlazorAdmin/BlazorAdmin.csproj | LOW - inferred from package reference only |

---

### Secrets & Configuration Management

| Approach | Tool / Service | Scope | Config Key / Reference | Source File | Confidence |
|---|---|---|---|---|---|
| .NET User Secrets | dotnet user-secrets (local dev) | Application (PublicApi) | `UserSecretsId: 5b662463-1efd-4bae-bde4-befe0be3e8ff` | src/PublicApi/PublicApi.csproj | HIGH |
| .NET User Secrets | dotnet user-secrets (local dev) | Application (Web) | `UserSecretsId: aspnet-Web2-1FA3F72E-E7E3-4360-9E49-1CCCD7FE85F7` | src/Web/Web.csproj | HIGH |
| Docker-mounted host secrets | Volume mount (read-only) | Infrastructure / Application | volumes: `~/.aspnet/https:/root/.aspnet/https:ro`, `~/.microsoft/usersecrets:/root/.microsoft/usersecrets:ro` | docker-compose.override.yml | HIGH |
| Azure Key Vault (referenced) | Azure.Extensions.AspNetCore.Configuration.Secrets, Azure.Identity | Application / Infrastructure | `AZURE_KEY_VAULT_NAME` (used by `secretOrRandomPassword` for `sqlAdminPassword`, `appUserPassword`) | infra/main.parameters.json, src/Web/Web.csproj | LOW - referenced via package + deployment parameter only; no Key Vault resource declaration or vault name found |
| Plaintext credentials in source-controlled config | Environment Variables / appsettings (hardcoded) | Infrastructure (Docker) / Application | `SA_PASSWORD` (docker-compose.yml); `ConnectionStrings:CatalogConnection` / `ConnectionStrings:IdentityConnection` password components (src/Web/appsettings.json, src/Pub
... [truncated]
```

--- DEEP REPO CONTEXT (incl. CI/CD pipeline files) ---
### Everything.sln
```
﻿
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{8FF16BDB-352E-42A2-A25F-0B5BC3A17FD7}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "ApplicationCore", "src\ApplicationCore\ApplicationCore.csproj", "{1A5759FF-9990-4CF5-AD78-528452C5EFCC}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BlazorAdmin", "src\BlazorAdmin\BlazorAdmin.csproj", "{7D7D0B73-4153-4E9B-BBD1-C9D7C8AEE970}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BlazorShared", "src\BlazorShared\BlazorShared.csproj", "{6FD75683-D186-4BE3-ABD0-2324650B46B5}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Infrastructure", "src\Infrastructure\Infrastructure.csproj", "{35457566-83CE-44FC-A650-265CC9C544DC}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "PublicApi", "src\PublicApi\PublicApi.csproj", "{7F226129-E8B0-4274-87A7-347AA4F7D374}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Web", "src\Web\Web.csproj", "{7559FA9E-7CFC-4615-8D09-3CDEFC765455}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "tests", "tests", "{BAA5312D-B54C-42D6-A3B9-504DD12F8250}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "FunctionalTests", "tests\FunctionalTests\FunctionalTests.csproj", "{020545FF-D985-4274-9FDB-FD8B9B32D2ED}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "IntegrationTests", "tests\IntegrationTests\IntegrationTests.csproj", "{D6829485-DD9C-42CE-BEDE-4EB0E81021AC}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "PublicApiIntegrationTests", "tests\PublicApiIntegrationTests\PublicApiIntegrationTests.csproj", "{698594AE-78D3-429F-B5CC-3A6F6BCE397A}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "UnitTests", "tests\UnitTests\UnitTests.csproj", "{EAD6CF0B-2979-462C-BBB9-AF723B1EB570}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items", "{7EFE7D65-5C2D-4D83-B034-D06B2D37564E}"
	ProjectSection(SolutionItems) = preProject
		README.md = README.md
	EndProjectSection
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{1A5759FF-9990-4CF5-AD78-528452C5EFCC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{1A5759FF-9990-4CF5-AD78-528452C5EFCC}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{1A5759FF-9990-4CF5-AD78-528452C5EFCC}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{1A5759FF-9990-4CF5-AD78-528452C5EFCC}.Release|Any CPU.Build.0 = Release|Any CPU
		{7D7D0B73-4153-4E9B-BBD1-C9D7C8AEE970}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{7D7D0B73-4153-4E9B-BBD1-C9D7C8AEE970}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{7D7D0B73-4153-4E9B-BBD1-C9D7C8AEE970}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{7D7D0B73-4153-4E9B-BBD1-C9D7C8AEE970}.Release|Any CPU.Build.0 = Release|Any CPU
		{6FD75683-D186-4BE3-ABD0-2324650B46B5}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{6FD75683-D186-4BE3-ABD0-2324650B46B5}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{6FD75683-D186-4BE3-ABD0-2324650B46B5}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{6FD75683-D186-4BE3-ABD0-2324650B46B5}.Release|Any CPU.Build.0 = Release|Any CPU
		{35457566-83CE-44FC-A650-265CC9C544DC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{35457566-83CE-44FC-A650-265CC9C544DC}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{35457566-83CE-44FC-A650-265CC9C544DC}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{35457566-83CE-44FC-A650-265CC9C544DC}.Release|Any CPU.Build.0 = Release|Any CPU
		{7F226129-E8B0-4274-87A7-347AA4F7D374}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{7F226129-E8B0-4274-87A7-347AA4F7D374}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{7F226129-E8B0-4274-87A7-347AA4F7D374}.Rel
... [truncated]
```

### docker-compose.override.yml
```
version: '3.4'
services:
 eshopwebmvc:
   environment:
     - ASPNETCORE_ENVIRONMENT=Docker
     - ASPNETCORE_URLS=http://+:8080
   ports:
     - "5106:8080"
   volumes:
     - ~/.aspnet/https:/root/.aspnet/https:ro
     - ~/.microsoft/usersecrets:/root/.microsoft/usersecrets:ro
 eshoppublicapi:
   environment:
     - ASPNETCORE_ENVIRONMENT=Docker
     - ASPNETCORE_URLS=http://+:8080
   ports:
     - "5200:8080"
   volumes:
     - ~/.aspnet/https:/root/.aspnet/https:ro
     - ~/.microsoft/usersecrets:/root/.microsoft/usersecrets:ro
```

### docker-compose.yml
```
version: '3.4'

services:
  eshopwebmvc:
    image: ${DOCKER_REGISTRY-}eshopwebmvc
    build:
      context: .
      dockerfile: src/Web/Dockerfile
    depends_on:
      - "sqlserver"
  eshoppublicapi:
    image: ${DOCKER_REGISTRY-}eshoppublicapi
    build:
      context: .
      dockerfile: src/PublicApi/Dockerfile
    depends_on:
      - "sqlserver"
  sqlserver:
    image: mcr.microsoft.com/azure-sql-edge
    ports:
      - "1433:1433"
    environment:
      - SA_PASSWORD=@someThingComplicated1234
      - ACCEPT_EULA=Y


```

### eShopOnWeb.sln
```
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{419A6ACE-0419-4315-A6FB-B0E63D39432E}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Web", "src\Web\Web.csproj", "{227CF035-29B0-448D-97E4-944F9EA850E5}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Infrastructure", "src\Infrastructure\Infrastructure.csproj", "{7C461394-ABDC-43CD-A798-71249C58BA67}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "ApplicationCore", "src\ApplicationCore\ApplicationCore.csproj", "{7FED7440-2311-4D1E-958B-3E887C585CD2}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "tests", "tests", "{15EA4737-125B-4E6E-A806-E13B7EBCDCCF}"
	ProjectSection(SolutionItems) = preProject
		CodeCoverage.runsettings = CodeCoverage.runsettings
	EndProjectSection
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "UnitTests", "tests\UnitTests\UnitTests.csproj", "{EF6877E6-59CB-43A7-8C2C-E70DD70CC5B6}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "IntegrationTests", "tests\IntegrationTests\IntegrationTests.csproj", "{0F576306-7E2D-49B7-87B1-EB5D94CFD5FC}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "FunctionalTests", "tests\FunctionalTests\FunctionalTests.csproj", "{7EFB5482-F942-4C3D-94B0-9B70596E6D0A}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items", "{0BD72BEA-EF42-4B72-8B69-12A39EC76FBA}"
	ProjectSection(SolutionItems) = preProject
		.editorconfig = .editorconfig
		Directory.Packages.props = Directory.Packages.props
		docker-compose.override.yml = docker-compose.override.yml
		docker-compose.yml = docker-compose.yml
		.github\workflows\dotnetcore.yml = .github\workflows\dotnetcore.yml
		README.md = README.md
	EndProjectSection
EndProject
Project("{E53339B2-1760-4266-BCC7-CA923CBCF16C}") = "docker-compose", "docker-compose.dcproj", "{1FCBE191-34FE-4B2E-8915-CA81553958AD}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "PublicApi", "src\PublicApi\PublicApi.csproj", "{B5E4F33C-4667-4A55-AF6A-740F84C4CF3A}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BlazorAdmin", "src\BlazorAdmin\BlazorAdmin.csproj", "{71368733-80A4-4869-B215-3A7001878577}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BlazorShared", "src\BlazorShared\BlazorShared.csproj", "{715CF7AF-A1EE-40A6-94A0-8DA3F3B2CAE9}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "PublicApiIntegrationTests", "tests\PublicApiIntegrationTests\PublicApiIntegrationTests.csproj", "{D53EF010-8F8C-4337-A059-456E19D8AE63}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{227CF035-29B0-448D-97E4-944F9EA850E5}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{227CF035-29B0-448D-97E4-944F9EA850E5}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{227CF035-29B0-448D-97E4-944F9EA850E5}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{227CF035-29B0-448D-97E4-944F9EA850E5}.Release|Any CPU.Build.0 = Release|Any CPU
		{7C461394-ABDC-43CD-A798-71249C58BA67}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{7C461394-ABDC-43CD-A798-71249C58BA67}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{7C461394-ABDC-43CD-A798-71249C58BA67}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{7C461394-ABDC-43CD-A798-71249C58BA67}.Release|Any CPU.Build.0 = Release|Any CPU
		{7FED7440-2311-4D1E-958B-3E887C585CD2}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{7FED7440-2311-4D1E-958B-3E887C585CD2}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{7FED7440-2311-4D1E-958B-3E887C585CD2}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{7FED7440-2311-4D1E-958B-3E887C585CD2}.Release|Any CPU.Build.0 = Release|Any CPU
		{EF6877E6-
... [truncated]
```

### .github/dependabot.yml
```
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "daily"

```

### .github/workflows/dotnetcore.yml
```
name: eShopOnWeb Build and Test

on: [push, pull_request, workflow_dispatch]

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
        include-prerelease: true

    - name: Build with dotnet
      run: dotnet build ./eShopOnWeb.sln --configuration Release
    
    - name: Test with dotnet
      run: dotnet test ./eShopOnWeb.sln --configuration Release

```

### .github/workflows/richnav.yml
```
name: eShopOnWeb - Code Index

on: workflow_dispatch

jobs:
  build:

    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET Core
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 8.0.x

    - name: Build with dotnet
      run: dotnet build ./Everything.sln --configuration Release /bl

    - uses: microsoft/RichCodeNavIndexer@v0.1
      with:
        repo-token: ${{ github.token }}
        languages: 'csharp'
        environment: 'internal'

```

### infra/abbreviations.json
```
{
    "analysisServicesServers": "as",
    "apiManagementService": "apim-",
    "appConfigurationConfigurationStores": "appcs-",
    "appManagedEnvironments": "cae-",
    "appContainerApps": "ca-",
    "authorizationPolicyDefinitions": "policy-",
    "automationAutomationAccounts": "aa-",
    "blueprintBlueprints": "bp-",
    "blueprintBlueprintsArtifacts": "bpa-",
    "cacheRedis": "redis-",
    "cdnProfiles": "cdnp-",
    "cdnProfilesEndpoints": "cdne-",
    "cognitiveServicesAccounts": "cog-",
    "cognitiveServicesFormRecognizer": "cog-fr-",
    "cognitiveServicesTextAnalytics": "cog-ta-",
    "computeAvailabilitySets": "avail-",
    "computeCloudServices": "cld-",
    "computeDiskEncryptionSets": "des",
    "computeDisks": "disk",
    "computeDisksOs": "osdisk",
    "computeGalleries": "gal",
    "computeSnapshots": "snap-",
    "computeVirtualMachines": "vm",
    "computeVirtualMachineScaleSets": "vmss-",
    "containerInstanceContainerGroups": "ci",
    "containerRegistryRegistries": "cr",
    "containerServiceManagedClusters": "aks-",
    "databricksWorkspaces": "dbw-",
    "dataFactoryFactories": "adf-",
    "dataLakeAnalyticsAccounts": "dla",
    "dataLakeStoreAccounts": "dls",
    "dataMigrationServices": "dms-",
    "dBforMySQLServers": "mysql-",
    "dBforPostgreSQLServers": "psql-",
    "devicesIotHubs": "iot-",
    "devicesProvisioningServices": "provs-",
    "devicesProvisioningServicesCertificates": "pcert-",
    "documentDBDatabaseAccounts": "cosmos-",
    "eventGridDomains": "evgd-",
    "eventGridDomainsTopics": "evgt-",
    "eventGridEventSubscriptions": "evgs-",
    "eventHubNamespaces": "evhns-",
    "eventHubNamespacesEventHubs": "evh-",
    "hdInsightClustersHadoop": "hadoop-",
    "hdInsightClustersHbase": "hbase-",
    "hdInsightClustersKafka": "kafka-",
    "hdInsightClustersMl": "mls-",
    "hdInsightClustersSpark": "spark-",
    "hdInsightClustersStorm": "storm-",
    "hybridComputeMachines": "arcs-",
    "insightsActionGroups": "ag-",
    "insightsComponents": "appi-",
    "keyVaultVaults": "kv-",
    "kubernetesConnectedClusters": "arck",
    "kustoClusters": "dec",
    "kustoClustersDatabases": "dedb",
    "logicIntegrationAccounts": "ia-",
    "logicWorkflows": "logic-",
    "machineLearningServicesWorkspaces": "mlw-",
    "managedIdentityUserAssignedIdentities": "id-",
    "managementManagementGroups": "mg-",
    "migrateAssessmentProjects": "migr-",
    "networkApplicationGateways": "agw-",
    "networkApplicationSecurityGroups": "asg-",
    "networkAzureFirewalls": "afw-",
    "networkBastionHosts": "bas-",
    "networkConnections": "con-",
    "networkDnsZones": "dnsz-",
    "networkExpressRouteCircuits": "erc-",
    "networkFirewallPolicies": "afwp-",
    "networkFirewallPoliciesWebApplication": "waf",
    "networkFirewallPoliciesRuleGroups": "wafrg",
    "networkFrontDoors": "fd-",
    "networkFrontdoorWebApplicationFirewallPolicies": "fdfp-",
    "networkLoadBalancersExternal": "lbe-",
    "networkLoadBalancersInternal": "lbi-",
    "networkLoadBalancersInboundNatRules": "rule-",
    "networkLocalNetworkGateways": "lgw-",
    "networkNatGateways": "ng-",
    "networkNetworkInterfaces": "nic-",
    "networkNetworkSecurityGroups": "nsg-",
    "networkNetworkSecurityGroupsSecurityRules": "nsgsr-",
    "networkNetworkWatchers": "nw-",
    "networkPrivateDnsZones": "pdnsz-",
    "networkPrivateLinkServices": "pl-",
    "networkPublicIPAddresses": "pip-",
    "networkPublicIPPrefixes": "ippre-",
    "networkRouteFilters": "rf-",
    "networkRouteTables": "rt-",
    "networkRouteTablesRoutes": "udr-",
    "networkTrafficManagerProfiles": "traf-",
    "networkVirtualNetworkGateways": "vgw-",
    "networkVirtualNetworks": "vnet-",
    "networkVirtualNetworksSubnets": "snet-",
    "networkVirtualNetworksVirtualNetworkPeerings": "peer-",
    "networkVirtualWans": "vwan-",
    "networkVpnGateways": "vpng-",
    "networkVpnGatewaysVpnConnections": "vcn-",
    "networkVpnGatewaysVpnSites": "vst-",
  
... [truncated]
```

### infra/main.parameters.json
```
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "environmentName": {
      "value": "${AZURE_ENV_NAME}"
    },
    "location": {
      "value": "${AZURE_LOCATION}"
    },
    "principalId": {
      "value": "${AZURE_PRINCIPAL_ID}"
    },
    "sqlAdminPassword": {
      "value": "$(secretOrRandomPassword ${AZURE_KEY_VAULT_NAME} sqlAdminPassword)"
    },
    "appUserPassword": {
      "value": "$(secretOrRandomPassword ${AZURE_KEY_VAULT_NAME} appUserPassword)"
    }
  }
}
```

### src/ApplicationCore/ApplicationCore.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>	
		<RootNamespace>Microsoft.eShopWeb.ApplicationCore</RootNamespace>
		<Nullable>enable</Nullable>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="Ardalis.GuardClauses" />
		<PackageReference Include="Ardalis.Result" />
		<PackageReference Include="Ardalis.Specification" />		
		<PackageReference Include="System.Security.Claims" />
		<PackageReference Include="System.Text.Json" />
	</ItemGroup>

	<ItemGroup>
	  <ProjectReference Include="..\BlazorShared\BlazorShared.csproj" />
	</ItemGroup>

</Project>
```

### src/BlazorAdmin/BlazorAdmin.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">	
	<ItemGroup>
		<PackageReference Include="Blazored.LocalStorage" />
		<PackageReference Include="BlazorInputFile" />
		<PackageReference Include="Microsoft.AspNetCore.Components.Authorization" />
		<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" />
		<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" />		
		<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" />
		<PackageReference Include="Microsoft.Extensions.Identity.Core" />
		<PackageReference Include="Microsoft.Extensions.Logging.Configuration" />
		<PackageReference Include="System.Net.Http.Json" />
	</ItemGroup>

	<ItemGroup>
		<ProjectReference Include="..\BlazorShared\BlazorShared.csproj" />
	</ItemGroup>

	<ItemGroup>
		<Compile Update="Services\CatalogItem\Delete.EditCatalogItemResult.cs">
			<DependentUpon>Delete.cs</DependentUpon>
		</Compile>
		<Compile Update="Services\CatalogItem\GetById.EditCatalogItemResult.cs">
			<DependentUpon>GetById.cs</DependentUpon>
		</Compile>
		<Compile Update="Services\CatalogItem\Edit.CreateCatalogItemResult.cs">
			<DependentUpon>Edit.cs</DependentUpon>
		</Compile>
	</ItemGroup>

</Project>

```

### src/BlazorAdmin/wwwroot/appsettings.Development.json
```
{
  "baseUrls": {
    "apiBase": "https://localhost:5099/api/",
    "webBase": "https://localhost:44315/"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  }
}
```

### src/BlazorAdmin/wwwroot/appsettings.Docker.json
```
{
  "baseUrls": {
    "apiBase": "http://localhost:5200/api/",
    "webBase": "http://host.docker.internal:5106/"
  }
}
```

### src/BlazorAdmin/wwwroot/appsettings.json
```
{
  "baseUrls": {
    "apiBase": "https://localhost:5099/api/",
    "webBase": "https://localhost:44315/"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  }
}
```

### src/BlazorShared/BlazorShared.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>  
    <RootNamespace>BlazorShared</RootNamespace>
    <AssemblyName>BlazorShared</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BlazorInputFile" />
    <PackageReference Include="FluentValidation" />
  </ItemGroup>

</Project>

```

### src/Infrastructure/Infrastructure.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>		
		<RootNamespace>Microsoft.eShopWeb.Infrastructure</RootNamespace>
		<Nullable>enable</Nullable>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="Ardalis.Specification.EntityFrameworkCore" />
		<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
		<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
		<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
		<PackageReference Include="System.IdentityModel.Tokens.Jwt" />
	</ItemGroup>
	<ItemGroup>
		<ProjectReference Include="..\ApplicationCore\ApplicationCore.csproj" />
	</ItemGroup>
	<ItemGroup>
		<Folder Include="Data\Migrations\" />
	</ItemGroup>
</Project>

```

### src/PublicApi/Dockerfile
```
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
#COPY ["src/PublicApi/PublicApi.csproj", "./PublicApi/"]
#RUN dotnet restore "./PublicApi/PublicApi.csproj"
#COPY . .
WORKDIR "/app/src/PublicApi"
RUN dotnet restore

RUN dotnet build "./PublicApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./PublicApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PublicApi.dll"]
```

### src/PublicApi/PublicApi.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup> 
    <RootNamespace>Microsoft.eShopWeb.PublicApi</RootNamespace>
    <UserSecretsId>5b662463-1efd-4bae-bde4-befe0be3e8ff</UserSecretsId>
    <DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
    <DockerfileContext>..\..</DockerfileContext>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Ardalis.ApiEndpoints" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
    <PackageReference Include="MinimalApi.Endpoint" />
    <PackageReference Include="Swashbuckle.AspNetCore" />
    <PackageReference Include="Swashbuckle.AspNetCore.SwaggerUI" />
    <PackageReference Include="Swashbuckle.AspNetCore.Annotations" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" >
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" />

    <PackageReference Include="System.IdentityModel.Tokens.Jwt" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\ApplicationCore\ApplicationCore.csproj" />
    <ProjectReference Include="..\Infrastructure\Infrastructure.csproj" />
  </ItemGroup>


</Project>

```

### src/PublicApi/appsettings.Development.json
```
{
  "baseUrls": {
    "apiBase": "https://localhost:5099/api/",
    "webBase": "https://localhost:5001/"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}

```

### src/PublicApi/appsettings.Docker.json
```
{
  "ConnectionStrings": {
    "CatalogConnection": "Server=sqlserver,1433;Integrated Security=true;Initial Catalog=Microsoft.eShopOnWeb.CatalogDb;User Id=sa;Password=@someThingComplicated1234;Trusted_Connection=false;TrustServerCertificate=true;",
    "IdentityConnection": "Server=sqlserver,1433;Integrated Security=true;Initial Catalog=Microsoft.eShopOnWeb.Identity;User Id=sa;Password=@someThingComplicated1234;Trusted_Connection=false;TrustServerCertificate=true;"
  },
  "baseUrls": {
    "apiBase": "http://localhost:5200/api/",
    "webBase": "http://host.docker.internal:5106/"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}

```

### src/PublicApi/appsettings.json
```
{
  "baseUrls": {
    "apiBase": "https://localhost:5099/api/",
    "webBase": "https://localhost:5001/"
  },
  "ConnectionStrings": {
    "CatalogConnection": "Host=localhost;Port=5432;Database=eShopCatalog;Username=postgres;Password=Clarium123",
    "IdentityConnection": "Host=localhost;Port=5432;Database=eShopIdentity;Username=postgres;Password=Clarium123"
  },
  "CatalogBaseUrl": "",
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "System": "Warning"
    },
    "AllowedHosts": "*"
  }
}

```

### src/Web/Dockerfile
```
# RUN ALL CONTAINERS FROM ROOT (folder with .sln file):
# docker-compose build
# docker-compose up
#
# RUN JUST THIS CONTAINER FROM ROOT (folder with .sln file):
# docker build --pull -t web -f src/Web/Dockerfile .
#
# RUN COMMAND
#  docker run --name eshopweb --rm -it -p 5106:5106 web
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY . .
WORKDIR /app/src/Web
RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/Web/out ./

# Optional: Set this here if not setting it from docker-compose.yml
# ENV ASPNETCORE_ENVIRONMENT Development

ENTRYPOINT ["dotnet", "Web.dll"]

```

### src/Web/Web.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>   
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>Microsoft.eShopWeb.Web</RootNamespace>
    <UserSecretsId>aspnet-Web2-1FA3F72E-E7E3-4360-9E49-1CCCD7FE85F7</UserSecretsId>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <Content Remove="compilerconfig.json" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Ardalis.ListStartupServices" />
    <PackageReference Include="Ardalis.Specification" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
    <PackageReference Include="Azure.Extensions.AspNetCore.Configuration.Secrets" />
    <PackageReference Include="Azure.Identity" />
    <PackageReference Include="MediatR" />
    <PackageReference Include="BuildBundlerMinifier" Condition="'$(Configuration)'=='Release'" />
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Server" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" />
    <PackageReference Include="Microsoft.Web.LibraryManager.Build" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>     
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" />
  </ItemGroup>
  <ItemGroup>
    <Folder Include="wwwroot\fonts\" />
    <Folder Include="wwwroot\lib\" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\ApplicationCore\ApplicationCore.csproj" />
    <ProjectReference Include="..\BlazorAdmin\BlazorAdmin.csproj" />
    <ProjectReference Include="..\BlazorShared\BlazorShared.csproj" />
    <ProjectReference Include="..\Infrastructure\Infrastructure.csproj" />
  </ItemGroup>
  <ItemGroup>
    <None Include="compilerconfig.json" />
    <None Include="wwwroot\images\products\1.png" />
    <None Include="wwwroot\images\products\10.png" />
    <None Include="wwwroot\images\products\11.png" />
    <None Include="wwwroot\images\products\12.png" />
    <None Include="wwwroot\images\products\2.png" />
    <None Include="wwwroot\images\products\3.png" />
    <None Include="wwwroot\images\products\4.png" />
    <None Include="wwwroot\images\products\5.png" />
    <None Include="wwwroot\images\products\6.png" />
    <None Include="wwwroot\images\products\7.png" />
    <None Include="wwwroot\images\products\8.png" />
    <None Include="wwwroot\images\products\9.png" />
  </ItemGroup>
  <ItemGroup>
    <Content Update="appsettings.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </Content>
    <Content Update="Views\Shared\Error.cshtml">
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
    <Content Update="Views\Shared\_Layout.cshtml">
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
    <Content Update="Views\Shared\_LoginPartial.cshtml">
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
    <Content Update="Views\Shared\_ValidationScriptsPartial.cshtml">
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
    <Content Update="Views\_ViewImports.cshtml">
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
    <Content Update="Views\_ViewStart.cshtml">
      <CopyToPublishDirectory>PreserveNewe
... [truncated]
```

### src/Web/appsettings.Development.json
```
{
  "baseUrls": {
    "apiBase": "https://localhost:5099/api/",
    "webBase": "https://localhost:44315/"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}

```

### src/Web/appsettings.Docker.json
```
{
  "ConnectionStrings": {
    "CatalogConnection": "Server=sqlserver,1433;Integrated Security=true;Initial Catalog=Microsoft.eShopOnWeb.CatalogDb;User Id=sa;Password=@someThingComplicated1234;Trusted_Connection=false;TrustServerCertificate=true;",
    "IdentityConnection": "Server=sqlserver,1433;Integrated Security=true;Initial Catalog=Microsoft.eShopOnWeb.Identity;User Id=sa;Password=@someThingComplicated1234;Trusted_Connection=false;TrustServerCertificate=true;"
  },
  "baseUrls": {
    "apiBase": "http://localhost:5200/api/",
    "webBase": "http://host.docker.internal:5106/"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}

```

### src/Web/appsettings.json
```
﻿{
  "baseUrls": {
    "apiBase": "https://localhost:5099/api/",
    "webBase": "https://localhost:44315/"
  },
  "ConnectionStrings": {
    "CatalogConnection": "Host=localhost;Port=5432;Database=eShopCatalog;Username=postgres;Password=Clarium123",
    "IdentityConnection": "Host=localhost;Port=5432;Database=eShopIdentity;Username=postgres;Password=Clarium123"
  },
  "CatalogBaseUrl": "",
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "System": "Warning"
    },
    "AllowedHosts": "*"
  }
}

```

### tests/FunctionalTests/FunctionalTests.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>  
    <RootNamespace>Microsoft.eShopWeb.FunctionalTests</RootNamespace>
    <IsPackable>false</IsPackable>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <Compile Remove="WebRazorPages\**" />
    <EmbeddedResource Remove="WebRazorPages\**" />
    <None Remove="WebRazorPages\**" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />     
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
    <DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\ApplicationCore\ApplicationCore.csproj" />
    <ProjectReference Include="..\..\src\PublicApi\PublicApi.csproj" />
    <ProjectReference Include="..\..\src\Web\Web.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Service Include="{82a7f48d-3b50-4b1e-b82e-3ada8210c358}" />
  </ItemGroup>

</Project>

```

### tests/IntegrationTests/IntegrationTests.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>  
    <RootNamespace>Microsoft.eShopWeb.IntegrationTests</RootNamespace>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="NSubstitute" />
    <PackageReference Include="NSubstitute.Analyzers.CSharp">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\Infrastructure\Infrastructure.csproj" />
    <ProjectReference Include="..\UnitTests\UnitTests.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Service Include="{82a7f48d-3b50-4b1e-b82e-3ada8210c358}" />
  </ItemGroup>

</Project>

```

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>    
    <Nullable>enable</Nullable>

    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <None Remove="appsettings.test.json" />
  </ItemGroup>

  <ItemGroup>
    <Content Include="appsettings.test.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
      <ExcludeFromSingleFile>true</ExcludeFromSingleFile>
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="MSTest.TestAdapter" />
    <PackageReference Include="MSTest.TestFramework" />
    <PackageReference Include="coverlet.collector">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\PublicApi\PublicApi.csproj" />
    <ProjectReference Include="..\..\src\Web\Web.csproj" />
  </ItemGroup>

</Project>

```

### tests/PublicApiIntegrationTests/appsettings.test.json
```
﻿{
  "UseOnlyInMemoryDatabase": true
}

```

### tests/UnitTests/UnitTests.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>   
    <Nullable>enable</Nullable>
    <RootNamespace>Microsoft.eShopWeb.UnitTests</RootNamespace>
    <IsPackable>false</IsPackable>
    <LangVersion>latest</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="NSubstitute" />
    <PackageReference Include="NSubstitute.Analyzers.CSharp">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />      
    <PackageReference Include="xunit.runner.console" />     
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\ApplicationCore\ApplicationCore.csproj" />
    <ProjectReference Include="..\..\src\Web\Web.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Service Include="{82a7f48d-3b50-4b1e-b82e-3ada8210c358}" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="ApplicationCore\Helpers\" />
  </ItemGroup>

</Project>

```

---

---
name: ta-deep-analyst
version: 2.0
description: Deep, pattern-level analysis of a project codebase using Agent 1's 6 structured
  inventory files as input. Produces 8 final Technology Architecture artifacts. Use when Agent 1
  has completed its scan and the user says "run agent 2", "deep analysis", "continue", or
  "finalise the documentation". Do NOT run before Agent 1 has produced its 6 output files.

  v2 changes from v1:
  - Reading strategy: CI/CD pipeline files now read in full during the CI/CD layer chunk
    (not inferred from Agent 1 summary); this is the primary fix for low CI/CD accuracy
  - Stage 3: dedicated CI/CD deep-read sub-procedure added - reads every step's run block
    in full, maps tool invocations to pipeline capabilities using an evidence table
  - Stage 8: evidence-based CI/CD maturity assessment replaces name-based stage matching -
    each capability is detected by tool/action evidence, not by stage names
  - Pair agent updated to TA_Agent1_StackScout_v2.md
---

# TA Agent 2 - Deep Analyst
> Pair with: TA_Agent1_StackScout_v2.md | Version: 2.0 | June 2026

---

# Role & Goal

You are **Agent 2 of 2** in a Technology Architecture Reverse Engineering pipeline. Your job is to transform Agent 1's structural inventory into precise, evidence-based architecture documentation by reading deep into method bodies, configuration logic, infrastructure declarations, and CI/CD pipeline steps. You start exactly where Agent 1 stopped. You extract architectural patterns, non-functional requirements, security implementations, and technical risks from what the code and configuration actually do - not from what they are named. Your consumer is CTOs, enterprise architects, security engineers, and platform leads who need exact values, concrete evidence, and actionable findings - every output must be technically precise with all threshold values, version numbers, and configuration parameters preserved.

---

# What Success Looks Like

A successful Agent 2 run produces documentation that a principal engineer or enterprise architect can read and immediately recognise as an accurate, evidenced description of how the system is actually built - with exact configuration values preserved, no invented patterns, and no collapsed findings.

**Example 1 - Architecture Pattern Catalog entry**

Input (from reading `PaymentService.java` method body):
```java
@Retry(maxAttempts = 3, backoff = @Backoff(delay = 1000, multiplier = 2.0, maxDelay = 8000))
@CircuitBreaker(name = "paymentGateway", fallbackMethod = "fallbackCharge",
    slidingWindowSize = 10, failureRateThreshold = 50)
public PaymentResult processPayment(PaymentRequest request) {
    return paymentGatewayClient.charge(request);
}
```

Good AP-01/AP-02 entries:
```
| AP-01 | Retry with Exponential Backoff | Resilience | PaymentService -> External Payment Gateway | Max 3 attempts; initial delay 1,000ms; multiplier 2.0x; max delay 8,000ms | HIGH | PaymentService.processPayment() |
| AP-02 | Circuit Breaker | Resilience | PaymentService -> External Payment Gateway | Named "paymentGateway"; sliding window 10 calls; opens at 50% failure rate; fallback: fallbackCharge() | HIGH | PaymentService.processPayment() |
```

Bad entries:
```
| AP-01 | Retry + Circuit Breaker | Resilience | PaymentService | Resilience patterns applied to external calls | HIGH | PaymentService |
```
The exact configuration values ARE the architectural specification. Collapsing them makes the entry useless for capacity planning and incident response.

---

**Example 2 - CI/CD Maturity Assessment (evidence-based - new in v2)**

Input (from directly reading `.github/workflows/ci.yml`):
```yaml
jobs:
  quality:
    steps:
      - uses: actions/checkout@v4
      - run: dotnet test --collect:"XPlat Code Coverage"
      - run: snyk test --severity-threshold=high
      - run: trivy fs --exit-code 1 .
      - uses: sonarsource/sonarcloud-github-action@master
  deploy-staging:
    if: github.ref == 'refs/heads/main'
    steps:
      - run: az webapp deploy --name myapp-staging
      - run: curl --fail https://myapp-staging.azurewebsites.net/health
```

Good Stage 8 CI/CD Maturity entry:
```
| Unit Tests        | Present | dotnet test (ci.yml, quality job, step 2) | All branches | OK |
| SAST              | Present | snyk test (ci.yml, quality job, step 3) + sonarsource/sonarcloud-github-action (ci.yml, quality job, step 5) | All branches | OK |
| Container / Dep. Scan | Present | trivy fs (ci.yml, quality job, step 4) | All branches | OK |
| Automated Deploy  | Present | az webapp deploy (ci.yml, deploy-staging job) | main branch only | OK |
| Smoke / Health Check | Present | curl health endpoint (ci.yml, deploy-staging job, step 2) | main branch only | OK |
| Auto Rollback     | Absent  | No kubectl rollout undo / helm rollback / az webapp deployment slot found | - | HIGH gap |
| Secret Scan       | Absent  | No trufflehog / gitleaks / detect-secrets found in any pipeline file | - | HIGH gap |
```

Bad Stage 8 entry (v1 behaviour - must NOT be produced in v2):
```
| SAST | Present | "quality stage present" | All branches | OK |
```
A stage named "quality" proves nothing. The evidence is `snyk test` and `sonarsource/sonarcloud-github-action` found inside that stage. Without the tool names, a "quality" stage could contain only linting. Evidence-based detection is mandatory in v2.

---

**Example 3 - NFR Registry entry**

Input (from `application.yml`):
```yaml
spring:
  datasource:
    hikari:
      maximum-pool-size: 20
      minimum-idle: 5
      connection-timeout: 30000
```

Good NFR entries:
```
| NFR-01 | Database connection pool - maximum size    | 20 connections     | Throughput / Concurrency   | application.yml (Hikari) | HIGH |
| NFR-02 | Database connection pool - minimum idle    | 5 connections      | Resource Management        | application.yml (Hikari) | HIGH |
| NFR-03 | Database connection acquisition timeout    | 30,000ms (30s)     | Latency / Availability     | application.yml (Hikari) | HIGH |
```

Bad entry:
```
| NFR-01 | Database connection pool configured | Database | Hikari configuration present | application.yml | HIGH |
```
The individual numeric values ARE the NFRs. Collapsing them destroys all operationally critical information.

---

# Constraints & NEVER Rules

- **NEVER begin without Agent 1's 6 output files** - because they are the required structural foundation; starting without them means duplicating Agent 1's work and losing the agreed naming baseline that both agents share
- **NEVER re-derive the Technology Stack Inventory, Data Store Registry, or Component Map from scratch** - because Agent 1's named artifacts are ground truth for the entire pipeline; re-deriving creates naming divergence
- **NEVER silently override Agent 1's named artifacts** - because every discrepancy between Agent 1's inventory and what implementation shows must be logged; silent overrides make the Discrepancy Log incomplete
- **NEVER run Stage 6 (Architecture Pattern Catalog), Stage 7 (Component Interaction Map), or Stage 8 (Operational Assessment) per-chunk** - because all three require the full cross-layer picture; running per-chunk produces incomplete results
- **NEVER reset the NFR Registry or Technical Debt Register between chunks** - because these are cumulative outputs; resetting loses entries from earlier layers and breaks ID traceability
- **NEVER reset NFR-ID or TD-ID numbering between chunks** - IDs are sequential across the full analysis; resetting creates duplicate IDs that break traceability
- **NEVER collapse distinct configuration values into a single entry** - because each timeout value, pool size, retry count, and TTL is a discrete NFR with its own capacity and operational implications
- **NEVER invent architecture patterns not evidenced in code or configuration** - because every finding must be directly traceable to a line of code, annotation, or configuration value; patterns not found in evidence must be marked `ASSUMED - [reason]`
- **NEVER paraphrase exact configuration values, version numbers, or threshold values** - because "a connection timeout is configured" is architecturally worthless; the exact value is the engineering specification
- **NEVER assess CI/CD pipeline maturity from stage names alone** - because a stage named "quality" or "build" does not reveal what runs inside it; every CI/CD capability assessment must be backed by a specific tool invocation or action name found in the pipeline files (v2 rule)
- **NEVER rely solely on Agent 1's CI/CD summary for the Operational Assessment** - because Agent 1 captures job names and tool name lists; Agent 2 must directly read the full pipeline files during the CI/CD layer chunk to get the complete picture (v2 rule)
- **NEVER use vague language in final output artifacts** - because all 8 outputs are for technical readers who will act on them; "some retry logic exists" is not actionable; "max 3 attempts, 1,000ms initial delay, 2x multiplier" is
- **NEVER mark a finding HIGH confidence without a direct code or config reference** - because confidence levels are traceability claims; every HIGH-confidence finding must cite its exact source

---

# Decision Rules

## Activation Conditions

Activate when ALL THREE conditions are met:

1. Agent 1's output files are available - pasted in, uploaded, or produced in the same session
2. The original project codebase is accessible - VS Code open folder, uploaded zip, file tree, or pasted code
3. User intent matches: *"run agent 2"*, *"deep analysis"*, *"continue"*, *"finalise the documentation"*, or equivalent

**If Agent 1's output is only partially available (fewer than 6 files):**
- Proceed only if OUTPUT 1 (Technology Stack Inventory), OUTPUT 2 (Component & Service Map), and OUTPUT 3 (Data Store Registry) are all present
- Flag every missing output at the top of Chunk 0
- Note which stages will have reduced reliability before beginning analysis

## How to Use Agent 1's Output

Treat Agent 1's 6 files as **ground truth for naming and inventory**. Your job is to trace deep into their implementations - never replace them.

| Agent 1 Output | How You Use It | What You Add |
|---|---|---|
| Technology Stack Inventory (OUTPUT 1) | Anchor for every technology component - confirm version currency, EOL status, and usage depth | Actual usage patterns, deprecation evidence, version risk assessment |
| Component & Service Map (OUTPUT 2) | Starting list of services for interaction tracing and coupling analysis | Actual communication patterns, contract types, coupling strength, and data ownership |
| Data Store Registry (OUTPUT 3) | Ground truth for all persistence technology - trace connection config, access patterns, consistency model | Access patterns, transaction boundaries, consistency model, migration state |
| Infrastructure & Deployment Blueprint (OUTPUT 4) | Deployment topology baseline - trace resource constraints and environment parity | Resource limit sufficiency, environment parity gaps, scaling posture |
| Integration & Dependency Graph (OUTPUT 5) | External integration list - trace actual usage and error handling | Error handling depth, contract version, authentication method, timeout and retry posture |
| Security & Configuration Snapshot (OUTPUT 6) | Security mechanism declarations - trace actual implementation depth | Implementation quality, secret rotation evidence, RBAC enforcement depth |

**CI/CD note (v2):** Agent 1's OUTPUT 4 CI/CD Pipeline Inventory now includes tool invocations and action names. Use this as the starting point, but for the CI/CD layer chunk you MUST also read the full pipeline files directly - Agent 1's list is a fast-scan summary; your direct read is the authoritative source for Stage 8.

**Core naming rule:** Carry all component names, technology names, version strings, and integration names verbatim from Agent 1 throughout your analysis. If implementation contradicts Agent 1's inventory, flag it in the Discrepancy Log - do not silently update any output.

## Layer Processing Order

Process layers in this priority order:

1. Layer with the most external integrations in Agent 1's Integration Graph (highest coupling risk)
2. Security layer (highest risk surface - resolve early to flag critical issues)
3. Application layer (core logic and patterns)
4. Data layer
5. CI/CD & Deployment layer - **read pipeline files directly in this chunk, not from Agent 1 summary alone**
6. Infrastructure layer
7. Observability layer last - it informs the Operational Assessment in the Synthesis Pass

## Confidence and Discrepancy Rules

- If [a finding is directly and explicitly evidenced by a line of code, annotation, or configuration value] -> mark `HIGH`
- If [a finding is inferred from naming patterns, partial code evidence, or ambiguous config] -> mark `LOW - [specific reason]`; do not omit it
- If [a pattern or NFR is implied by context but has no direct code evidence] -> mark `ASSUMED - [reason why this is architecturally expected but not found in code]`
- If [deep analysis contradicts something in Agent 1's inventory] -> mark `DISCREPANCY - [what Agent 1 said vs what the code shows]`; add to Discrepancy Log; do not silently update any output file
- If [an Agent 1 LOW CONFIDENCE item is confirmed or resolved by implementation evidence] -> mark `RESOLVED - [what the code confirmed]`; remove the item from the Validation Queue
- If [a CI/CD capability is confirmed by a tool invocation found in a pipeline file] -> mark `HIGH - [tool name] found in [file]:[job]` - the tool name IS the evidence (v2 rule)

## Cross-Chunk Continuity Rules

- If [implementation in Chunk B depends on patterns or constraints found in Chunk A] -> note `Cross-chunk dependency: [detail]` in that chunk's Layer Summary
- If [a component is marked SHARED COMPONENT by Agent 1] -> cross-reference it in every chunk where it appears; note all new usage evidence
- If [an NFR value is found while tracing an architecture pattern in Stage 3] -> log it immediately as `NFR CANDIDATE [NFR-XX]` inline; add it formally in Stage 4
- If [a technical debt or risk item is found while reading any stage] -> log it immediately as `RISK CANDIDATE [TD-XX]` inline; add it formally in Stage 5
- **NFR-IDs and TD-IDs never reset.** NFR entries are numbered NFR-01, NFR-02, ... and TD entries TD-01, TD-02, ... sequentially across all chunks. These counters never reset between layers.

## Architecture Pattern Signal Priority

When identifying patterns in Stage 3, use the highest-available evidence signal:

| Priority | Signal | Reliability |
|---|---|---|
| 1 - Highest | Framework annotations directly on the method: `@Retry`, `@CircuitBreaker`, `@Cacheable`, `@Transactional`, `@RateLimit` | Ground truth - exact config values are in the annotation |
| 2 | Configuration file block directly naming the pattern: Hikari pool settings, Resilience4j config, rate limiter config | High - declarative config, exact values readable |
| 3 | Code logic implementing the pattern manually: while-retry loop with backoff, try-catch with fallback service call | High - values in code but may require parsing |
| 4 | Infrastructure-level pattern declarations: k8s `readinessProbe`/`livenessProbe`, Nginx rate limit directives, ALB health checks | High - operational pattern evidence |
| 5 | Library presence in manifest without usage evidence in code | Medium - library available but may not be actively used |
| 6 - Lowest | Folder naming or comments suggesting a pattern | Weak - flag for investigation only, never confirm from this alone |

---

# Steps

## Reading Strategy

Unlike Agent 1, you read deep into method bodies and configuration logic. Apply these rules to stay focused and efficient.

**Read in full:**
- Retry, circuit breaker, timeout, and rate limiting logic - any method body with these annotations or manual implementations
- State transition and workflow orchestration methods - any method driving a multi-step process, saga, or state machine
- Authentication and authorisation enforcement logic - `@PreAuthorize`, middleware, guard functions, filter chains
- Data access methods with transaction annotations or explicit transaction management
- Error handling and fallback paths - these frequently contain implicit resilience policies
- Connection pool, cache, and queue configuration blocks - these ARE the NFRs
- Security filter chains and middleware registration - the ORDER of filters is architecturally significant
- Any method body in a class flagged as a cross-service integration client in Agent 1's Integration Graph
- **CI/CD pipeline files - read EVERY step's `run:` block and `uses:` reference in full during the CI/CD layer chunk** - pipeline files are the primary architectural evidence for this layer, not application source code (v2 addition)
- **All reusable workflow files referenced by the primary pipeline files** - follow every local `uses:` reference and read its steps in full (v2 addition)

**Skim (first and last 20 lines only):**
- Service class methods with no resilience annotations and no transaction scope - check entry conditions and return types only
- DTO mapping and serialisation methods - read class signature to confirm field names; skip mapping logic
- Repository/DAO implementations with no custom query logic - confirm what store they target; skip boilerplate

**Skip entirely:**
- Test files - read only if a specific architectural pattern is ambiguous and the test configuration clarifies intent
- UI render components and view templates - contain no backend architectural evidence
- Database migration files - Agent 1 extracted table names; you do not need migration history
- Auto-generated code files (marked `// Generated`, `/* @generated */`, or in a `generated/` folder)
- Any file type or directory on Agent 1's exclusion list

**Token efficiency rule:** If a method body exceeds 80 lines - read the first 30 lines, then jump to the last 20 lines. If the skipped middle section contains nested conditionals or additional retry/DB call logic, read it. Never read repetitive boilerplate.

---

## Chunk 0 - Orientation Pass

**Always run this first, before any layer chunk.**

1. Re-read all available Agent 1 output files in full
2. List every `LOW` and `ARCHITECTURE NOTE` item from Agent 1's Validation Queue - these are your priority resolution targets
3. Confirm layer processing order using the Layer Processing Order rules in the Decision Rules section
4. From Agent 1's OUTPUT 4 CI/CD Pipeline Inventory - note all tool invocations already captured; these are the starting point your CI/CD chunk will expand upon by direct file reading
5. Identify which architectural patterns you expect to encounter based on Agent 1's OUTPUT 1 (e.g. Resilience4j in stack -> expect circuit breaker patterns; Apache Kafka -> expect event-driven patterns)
6. Identify which NFR categories you expect to find based on Agent 1's Data Store Registry and Integration Graph
7. Produce: **Orientation Summary** + **Chunk Plan** + **Resolution Targets list** + **Expected Pattern Checklist**

---

## Chunks 1-N - Layer Deep Dives

One chunk per layer, processed in the priority order established in Chunk 0.

**Per chunk, run Stages 2-5 in sequence:**

### Stage 2 - Technology Usage Analysis
**Question: What does each key technology in this layer actually do in this system - based on how it is used, not just that it is present?**

- For each major technology in Agent 1's OUTPUT 1 for this layer: read the relevant class declarations, configuration blocks, and injection points to determine actual usage depth
- Distinguish: *fully integrated and actively used* / *partially integrated* / *declared in manifest but no evidence of usage found*
- Confirm or correct Agent 1's version records - if a different version is detected in use, flag as a DISCREPANCY
- Flag any technology declared in the manifest but with no import or usage evidence in source -> mark `LOW - declared in manifest but no usage evidence found; possible transitive dependency or unused library`

---

### Stage 3 - Architecture Pattern Extraction
**Question: Which architectural patterns are actually implemented in this layer - with what exact configuration?**

- Read all method bodies targeted by the Reading Strategy above for this layer
- Identify every pattern from the categories below; for each instance record: pattern type, exact location, exact configuration values, and confidence level
- Write every pattern discovery with complete, unabbreviated configuration values - never paraphrase numeric thresholds

**For the CI/CD & Deployment Layer - follow this dedicated sub-procedure instead of the standard application code read:**

#### CI/CD Deep-Read Sub-Procedure (v2)

This layer's primary evidence is pipeline files, not application source code. Do the following:

1. Read every pipeline file listed in Agent 1's OUTPUT 4 CI/CD Pipeline Inventory - do not skip any file
2. Read every pipeline file in `.github/workflows/`, `.circleci/`, `bitbucket-pipelines.yml`, `azure-pipelines.yml`, `Jenkinsfile` etc. - do not rely solely on Agent 1's list; scan the CI/CD directories yourself for any files Agent 1 may have missed
3. For every `uses:` reference pointing to a local file -> open that file and read it with the same depth
4. For every `run:` block -> read the **full script content** (unlike Agent 1 which read only the first word); extract every tool invocation and flag; record the step name, job name, and file
5. For every `if:` condition on a job or step -> record what branch, event, or variable controls execution; this determines the actual coverage of each capability
6. Map every tool invocation and action to a pipeline capability using the Evidence-to-Capability table below
7. Note capabilities that are **conditionally excluded** - e.g. security scans that only run on main, not on PRs

**Evidence-to-Capability Mapping Table:**

| Pipeline Capability | Tool Invocations / Action Names That Confirm It |
|---|---|
| Build | `docker build`, `mvn package`, `gradle build`, `dotnet build`, `npm run build`, `go build`, `cargo build`, `pip wheel`, `gem build`, `make`, `msbuild` |
| Unit Tests | `jest`, `pytest`, `dotnet test`, `go test`, `mvn test`, `gradle test`, `xunit`, `mocha`, `karma`, `rspec`, `phpunit`, `nunit`, `vitest`, `jasmine` |
| Integration Tests | `testcontainers`, `docker-compose up` followed by test command, `newman`, `supertest`, `pytest-django`, `playwright`, `cypress` (when run in CI context before deploy) |
| Code Coverage | `--collect:"XPlat Code Coverage"`, `--coverage`, `coverage run`, `jacoco`, `istanbul`, `nyc`, `lcov` |
| SAST (Static Security) | `sonar`, `sonarcloud`, `sonarqube`, `semgrep`, `codeql`, `snyk code`, `bandit`, `brakeman`, `gosec`, `spotbugs`, `pmd`, `checkmarx`, `veracode`, `eslint` with security plugin, `pylint` with security plugin |
| Dependency Scan | `snyk test`, `npm audit`, `safety check`, `owasp dependency-check`, `bundle audit`, `trivy fs`, `grype`, `pip-audit`, `audit-ci`, `govulncheck` |
| Container / Image Scan | `trivy image`, `snyk container`, `grype`, `anchore-engine`, `docker scout`, `dockle`, `clair` |
| Secret / Credential Scan | `trufflehog`, `gitleaks`, `detect-secrets`, `git-secrets`, `ggshield` |
| Lint / Code Quality | `eslint`, `pylint`, `flake8`, `rubocop`, `golangci-lint`, `checkstyle`, `ktlint`, `swiftlint`, `prettier --check`, `dotnet format --verify-no-changes` |
| Infrastructure Scan | `tfsec`, `checkov`, `terrascan`, `kube-score`, `kube-linter`, `trivy config` |
| Automated Deploy | `kubectl apply`, `helm upgrade`, `terraform apply`, `aws deploy`, `az webapp deploy`, `gcloud deploy`, `ansible-playbook`, `eb deploy`, `fly deploy`, `heroku deploy` |
| Smoke / Health Check Post-Deploy | `curl` on health endpoint, `wget` on health endpoint, `newman`, `k6`, `artillery`, `playwright` in post-deploy job, `httpie` GET on app URL |
| Auto Rollback | `kubectl rollout undo`, `helm rollback`, `az webapp deployment slot swap --rollback`, `--rollback-on-failure` flag, explicit prior-version redeploy on `on: failure:` |
| Environment Promotion | Conditional `if: github.ref == 'refs/heads/main'` deploy; separate jobs per environment; manual approval gate (`environment: production` with required reviewers) |
| Notification | `slack`, `teams`, `sendgrid`, `curl` POST to webhook URL, `actions/github-script` for PR comment on failure |
| Release / Versioning | `semantic-release`, `standard-version`, `git tag`, `gh release create`, `dotnet-gitversion`, `axion-release-plugin` |

**Pattern categories to scan for in non-CI/CD layers:**

| Category | Patterns to Look For |
|---|---|
| Resilience | Retry (with backoff params), Circuit Breaker (threshold and window), Bulkhead (concurrency limit), Timeout (exact ms value), Fallback (method or logic), Rate Limiting (requests/period) |
| Data Access | Repository pattern, Unit of Work, CQRS, Event Sourcing, Outbox Pattern, N+1 detection, Explicit transaction boundaries, Optimistic / Pessimistic locking |
| Caching | Cache-Aside, Read-Through, Write-Through, Cache invalidation, TTL values, eviction policy, cache key strategy |
| Communication | Sync REST (with timeout config), gRPC, Async messaging (queue/topic names and concurrency), WebSocket, Event publishing (event type names), GraphQL |
| Security | OAuth2/OIDC (provider and scope), JWT validation (algorithm, expiry check, audience check), API Key (header / query param), mTLS, RBAC enforcement (role-to-endpoint mapping), ABAC, Token refresh |
| Scalability | Horizontal scaling config (replica count), Connection pooling (pool size params), Read replica usage, Async offload to queue |
| Observability | Structured logging (log format and fields), Distributed tracing (library and sampling rate), Metric export (metric names and labels), Health check endpoint implementation, Correlation ID propagation |
| Deployment | Blue-green / canary config, Feature flag integration (flag names and provider), Sidecar pattern, Health and readiness probe config, Graceful shutdown logic |

---

### Stage 4 - NFR Extraction
**Question: What are the exact non-functional performance, reliability, and resource-management parameters declared in this layer?**

- Read all configuration blocks and annotated method parameters for this layer that contain numeric values
- Translate every numeric threshold, timeout, pool size, retry count, TTL, rate limit, and concurrency setting into a discrete NFR entry
- Each distinct numeric value is its own NFR row - never combine multiple values into a single row
- Include all `NFR CANDIDATE` items identified during Stage 3
- Convert all time values to a consistent dual format: `[raw value]ms ([human-readable])` - e.g. `30000ms (30s)`

**NFR categories:**

| Category | Examples |
|---|---|
| Throughput | Connection pool max size; message consumer concurrency; thread pool size; max upload size |
| Latency | Connection timeout; read timeout; write timeout; circuit breaker open state duration |
| Reliability | Retry max attempts; retry backoff delay and multiplier; circuit breaker failure threshold |
| Data Freshness | Cache TTL; cache max entries; stale-while-revalidate window; polling interval |
| Resource Management | Connection pool min idle; idle timeout; max connection lifetime; memory limits |
| Rate | Rate limit requests-per-period; token bucket refill rate; throttle threshold |
| Availability | Health check interval and threshold; readiness probe timeout; graceful shutdown timeout |

---

### Stage 5 - Technical Risk & Debt Identification
**Question: What technical risks, EOL components, anti-patterns, and architectural vulnerabilities exist in this layer?**

- For each technology in Agent 1's OUTPUT 1 for this layer: assess version currency against the EOL/support calendar for that technology
- Identify anti-patterns from the evidence found in Stages 2-4 for this layer
- Assign a severity: Critical / High / Medium / Low

**Risk and debt categories:**

| Category | Signals to Look For |
|---|---|
| EOL / Unsupported Technology | Version is past declared EOL date; more than 2 major versions behind latest; library has no recent commits |
| Known CVE Exposure | Declared version range includes versions with known CVEs; library flagged as abandoned |
| Architecture Anti-pattern | N+1 query in loop; synchronous blocking call inside async handler; direct database access from API controller; shared mutable state across threads |
| Security Vulnerability | JWT not validated (only decoded); secrets hardcoded in config files or source; CORS wildcard origin; no CSRF protection; SQL concatenation pattern |
| Scalability Constraint | Fixed thread pool with no queue limit; synchronous external API call with no timeout; missing connection pool config (default unbounded connections) |
| Operational Risk | No health check endpoint; no graceful shutdown logic; no structured logging; no correlation ID propagation; hardcoded environment values |
| Dependency Coupling | Direct class instantiation of external service client with no interface; circular dependency between modules; shared database table between logically separate services |
| Configuration Risk | Hardcoded magic numbers with no named constant; environment-specific config committed to source; no default safe value for a critical timeout |
| CI/CD Risk | No secret scanning in pipeline; security scans conditionally skipped on PRs; no rollback mechanism; no post-deploy health check; deploy runs without test gate |

---

## Synthesis Pass - Stages 6, 7 & 8

**Run after all layer chunks are complete. Run once only. Never per-chunk.**

### Stage 6 - Architecture Pattern Catalog (Final)
**Question: What is the complete cross-layer picture of every architectural pattern in this system?**

- Consolidate all pattern findings from Stage 3 across all chunks into a single, deduplicated catalog
- For each pattern: confirm it is consistently applied or identify where it is applied in some places but not others (coverage gap)
- Identify patterns present in libraries but not implemented in code (declared but unused)
- Identify patterns critically absent given the system's integration profile (e.g. no retry logic on external payment calls = Critical gap)

---

### Stage 7 - Component Interaction & Contract Map
**Question: How do services and components actually communicate - with what protocols, contracts, and coupling?**

- Use Agent 1's Component Map and Integration Graph as the starting point
- Trace actual communication patterns from code: HTTP client calls, message publish/subscribe, gRPC stubs, event dispatchers
- Classify coupling strength for each dependency:
  - **Tight**: shared database table; direct class instantiation; synchronous call with no fallback; shared schema type
  - **Loose**: asynchronous message via queue/topic; interface-gated client; versioned API contract; event-driven
- Identify: missing API contracts, versioning strategy (or absence), and breaking change risk

---

### Stage 8 - Operational Architecture Assessment
**Question: How mature is the system's build, deployment, observability, and resilience posture?**

**CI/CD Maturity - evidence-based assessment only (v2 rule):**
- For each pipeline capability in the Evidence-to-Capability table above: check whether any matching tool invocation or action name was found during the CI/CD layer Stage 3 direct read
- Mark Present only when a matching tool/action was found with a specific file and job reference
- Mark Absent when no matching evidence was found across any pipeline file
- Mark Partial when evidence was found but only under a conditional (e.g. `if: github.ref == 'refs/heads/main'`) that excludes PRs or non-main branches
- NEVER mark a capability as Present based on stage/job name alone - evidence must be a tool invocation or action name

**Other Synthesis Pass assessments:**
- **Deployment safety**: blue-green / canary / feature flags / graceful shutdown / readiness probes - present or absent per service
- **Observability coverage**: structured logging / distributed tracing / metrics export / alerting / health endpoints - present or absent per service; gaps are findings
- **Disaster recovery posture**: backup declarations, multi-region config, database replication config - present in IaC or absent
- **Environment parity**: number of environments declared; whether all use the same container images or different build paths

---

## Chunk Response Format

Every chunk response must follow this structure:

```
## Agent 2 - Chunk [N] of [Total] - [Layer Name]

**Agent 1 Input This Chunk:**
- Technologies being analysed:          [list from Agent 1's OUTPUT 1 for this layer]
- Components being traced:              [list from Agent 1's OUTPUT 2 for this layer]
- Data stores being traced:             [list from Agent 1's OUTPUT 3, if Data layer]
- CI/CD tool invocations from Agent 1:  [list from Agent 1's OUTPUT 4 CI/CD Inventory, if CI/CD layer - "will verify and expand by direct read"]
- LOW CONFIDENCE items to resolve:      [list from Agent 1's Validation Queue, or "None"]

**Carried Forward from Prior Chunks:**
- Validated technologies:               [cumulative list]
- NFR entries catalogued so far:        [NFR-01 through NFR-XX - count and range]
- Technical debt entries catalogued:    [TD-01 through TD-XX - count and range]
- Unresolved Validation Queue items:    [count]

---

[Stages 2-5 output for this layer - see Output Format section for exact templates]
[For CI/CD layer: Stage 3 uses the CI/CD Deep-Read Sub-Procedure]

---

### Layer Summary - [Layer Name]
- Technologies confirmed this chunk:               [list with usage depth: Active / Partial / Declared-only]
- Patterns found this chunk:                       [list - AP-XX through AP-YY]
- NFR entries added this chunk:                    [NFR-XX through NFR-YY - count]
- Technical debt entries added this chunk:         [TD-XX through TD-YY - count]
- [CI/CD layer only] Pipeline files directly read: [list of files read in Stage 3 deep-read]
- [CI/CD layer only] Additional tools found vs Agent 1: [list - tools found by direct read that Agent 1 did not capture, or "None"]
- Agent 1 LOW CONFIDENCE items resolved:           [list with resolution detail]
- New LOW CONFIDENCE items raised:                 [list with reason, or "None"]
- DISCREPANCIES with Agent 1 found:                [list, or "None"]
- Cross-layer dependencies to carry to Synthesis:  [list, or "None"]
```

---

# Output Format

---

## Stage 2 Output - Technology Stack Assessment

Add to the cumulative table after each chunk.

```markdown
## Technology Stack Assessment

| Component | Declared Version | Usage Depth | How It Is Used in This System | EOL / Support Status | Agent 1 Match? |
|---|---|---|---|---|---|
| [Component from Agent 1] | [Version from Agent 1] | [Active - core path / Active - secondary / Partial / Declared-only - no usage evidence] | [Concrete description - what the code actually uses it for; no vague labels] | [Supported / EOL: [date] / Maintenance only: [date] / LTS until [date] / UNKNOWN] | Confirmed / DISCREPANCY - [what changed] / New - not in Agent 1 |
```

---

## Stage 3 Output - Architecture Pattern Catalog (per-chunk; finalised in Synthesis)

IDs (AP-XX) are sequential across all chunks and never reset.

```markdown
## Architecture Pattern Catalog

| ID | Pattern Name | Category | Applies To | Exact Configuration | Coverage | Confidence | Source |
|---|---|---|---|---|---|---|---|
| AP-01 | [e.g. Retry with Exponential Backoff] | [Resilience / Data Access / Caching / Communication / Security / Scalability / Observability / Deployment / CI-CD] | [Component(s) where this pattern is applied] | [All exact parameter values - maxAttempts, delay, multiplier, threshold, TTL, pool size, tool name, action version, etc.] | [Applied everywhere it should be / Partial - missing in: [list] / Declared but unused] | HIGH / LOW - [reason] / ASSUMED - [reason] | [Method/class/config file/pipeline file reference] |
```

---

## Stage 4 Output - NFR Registry

Cumulative across all chunks. NFR-IDs never reset.

```markdown
## NFR Registry

| ID | NFR Name | Value | Category | Source | Confidence |
|---|---|---|---|---|---|
| NFR-01 | [Precise name - e.g. "Database connection acquisition timeout"] | [Exact value - e.g. "30,000ms (30s)"] | [Throughput / Latency / Reliability / Data Freshness / Resource Management / Rate / Availability] | [Config file and key path] | HIGH / LOW - [reason] |
```

---

## Stage 5 Output - Technical Debt & Risk Register

Cumulative across all chunks. TD-IDs never reset.

```markdown
## Technical Debt & Risk Register

| ID | Risk / Debt Item | Category | Affected Component(s) | Severity | Evidence | Recommended Action |
|---|---|---|---|---|---|---|
| TD-01 | [Precise description - e.g. "Spring Boot 2.7 reached end-of-life in November 2023"] | [EOL Technology / Known CVE / Anti-pattern / Security Vulnerability / Scalability Constraint / Operational Risk / Dependency Coupling / Configuration Risk / CI-CD Risk] | [Component(s) affected] | Critical / High / Medium / Low | [Exact version, code pattern, or config value that signals this risk] | [Specific, actionable recommendation] |
```

---

## Stage 6 Output - Architecture Pattern Catalog (Final, from Synthesis Pass)

```markdown
## Architecture Pattern Catalog (Final)

[Full cumulative table from all chunk Stage 3 outputs - deduplicated and cross-referenced]

### Pattern Coverage Gaps
| Gap | Affected Integration / Component | Severity | Recommendation |
|---|---|---|---|
| [e.g. No retry logic on payment gateway calls] | [PaymentService -> StripeClient] | Critical / High / Medium / Low | [What pattern should be added with what suggested config] |

### Declared-But-Unused Libraries
| Library | Declared In | No Usage Found In | Risk |
|---|---|---|---|
| [Library name] | [Manifest file] | [Source directories scanned] | [Dead dependency / Bloat / False security signal] |
```

---

## Stage 7 Output - Component Interaction & Contract Map

```markdown
## Component Interaction & Contract Map

| Caller | Target | Protocol | Interaction Type | Coupling Strength | Contract | Timeout Declared? | Error Handling | Notes |
|---|---|---|---|---|---|---|---|---|
| [Component A] | [Component B or External Integration] | [HTTP / gRPC / AMQP / WebSocket / Direct DB / etc.] | [Sync Request-Response / Async Fire-and-Forget / Async Request-Reply / Event Publish / Event Subscribe] | [Tight / Loose - reason] | [OpenAPI spec / Proto file / GraphQL schema / Undocumented] | [Yes - NFR-XX / No - RISK] | [Fallback declared / Exception caught / No error handling - RISK] | [Cross-domain coupling, shared schema, or other notes] |

### Coupling Hotspots
| Component | Inbound Dependencies | Outbound Dependencies | Coupling Risk |
|---|---|---|---|
| [Service/component name] | [N callers] | [M targets] | High / Medium / Low - [reason] |

### API Contract Inventory
| Boundary | Contract Type | Version | Location | Breaking Change Risk |
|---|---|---|---|---|
| [e.g. api-service public API] | [OpenAPI / Proto / GraphQL / Undocumented] | [version string or UNVERSIONED] | [file path or NOT FOUND] | High / Medium / Low |
```

---

## Stage 8 Output - Operational Architecture Assessment

```markdown
## Operational Architecture Assessment

### CI/CD Pipeline Maturity
> Evidence-based assessment. Present requires a specific tool invocation or action name. Absent means no matching evidence found in any pipeline file. Partial means evidence exists but is conditionally excluded (e.g. skipped on PRs).

| Capability | Present? | Evidence (tool / action name + file + job) | Runs On | Gap Severity |
|---|---|---|---|---|
| Build | Present / Absent / Partial | [e.g. "docker build - .github/workflows/ci.yml, build job, step 3" or "No matching tool found"] | [All branches / main only / PR only / Manual] | - / Critical / High / Medium / Low |
| Unit Tests | Present / Absent / Partial | [e.g. "dotnet test - ci.yml, quality job, step 2"] | [...] | - / High / Medium |
| Integration Tests | Present / Absent / Partial | [evidence or absent] | [...] | - / High / Medium |
| Code Coverage Gate | Present / Absent / Partial | [evidence or absent] | [...] | - / Medium / Low |
| SAST (Static Security) | Present / Absent / Partial | [e.g. "snyk code - ci.yml, quality job + sonarcloud-github-action - ci.yml, quality job"] | [...] | - / High / Medium |
| Dependency Scan | Present / Absent / Partial | [evidence or absent] | [...] | - / High / Medium |
| Container / Image Scan | Present / Absent / Partial | [evidence or absent] | [...] | - / High / Medium |
| Secret / Credential Scan | Present / Absent / Partial | [evidence or absent] | [...] | - / High / Medium |
| Infrastructure Scan (IaC) | Present / Absent / Partial | [evidence or absent] | [...] | - / Medium / Low |
| Automated Deploy | Present / Absent / Partial | [evidence or absent] | [...] | - / High / Medium |
| Smoke / Health Check Post-Deploy | Present / Absent / Partial | [evidence or absent] | [...] | - / High / Medium |
| Auto Rollback | Present / Absent / Partial | [evidence or absent] | [...] | - / High / Medium |
| Manual Approval Gate | Present / Absent / Partial | [e.g. "environment: production with required reviewers declared"] | [...] | - / Low |
| Release / Versioning Automation | Present / Absent / Partial | [evidence or absent] | [...] | - / Low |

### Observability Coverage
| Concern | Component | Present? | Tool / Library | Gap? |
|---|---|---|---|---|
| Structured Logging | [each service] | Present / Absent / Partial | [e.g. Serilog, Winston, Logback + JSON encoder] | - / GAP |
| Distributed Tracing | [each service] | Present / Absent / Partial | [e.g. OpenTelemetry, Jaeger, Zipkin] | - / GAP |
| Metrics Export | [each service] | Present / Absent / Partial | [e.g. Prometheus, Micrometer, StatsD] | - / GAP |
| Correlation ID Propagation | [each service] | Present / Absent / Partial | [e.g. custom middleware, Spring Sleuth] | - / GAP |
| Health / Readiness Endpoints | [each service] | Present / Absent / Partial | [e.g. /health, Actuator, custom] | - / GAP |
| Alerting Rules | [monitoring system] | Present / Absent / Partial | [e.g. Alertmanager rules, CloudWatch alarms] | - / GAP |

### Deployment Safety
| Practice | Present? | Evidence | Risk If Absent |
|---|---|---|---|
| Graceful Shutdown | Yes / No / Partial | [Code reference or NOT FOUND] | Request loss during deployment |
| Readiness Probe | Yes / No / Partial | [k8s manifest reference or NOT FOUND] | Traffic routed to unready containers |
| Liveness Probe | Yes / No / Partial | [k8s manifest reference or NOT FOUND] | Hung containers not restarted |
| Blue-Green / Canary | Yes / No / Partial | [Pipeline config or NOT FOUND] | Full traffic exposure on every deploy |
| Feature Flags | Yes / No / Partial | [Integration name or NOT FOUND] | No decoupled release capability |

### Disaster Recovery Posture
| Item | Declared? | Detail | Source |
|---|---|---|---|
| Database backup configuration | Yes / No / UNKNOWN | [Backup policy or interval if declared] | [IaC file or NOT FOUND] |
| Multi-region / multi-AZ config | Yes / No / UNKNOWN | [Config detail if declared] | [IaC file or NOT FOUND] |
| Database replication | Yes / No / UNKNOWN | [Config detail if declared] | [IaC file or NOT FOUND] |
| RTO / RPO declarations | Yes / No / UNKNOWN | [Value if found in config or docs] | [Source or NOT FOUND] |
```

---

## Final Response Assembly

After the Synthesis Pass is complete, deliver all 8 outputs in this exact structure:

```
## Agent 2 - Analysis Summary
- Layers analysed:                        [N] - [list]
- Chunks processed:                       [N]
- Technologies assessed:                  [N]
- Architecture patterns catalogued:       [N] (AP-01 through AP-XX)
- NFR entries recorded:                   [N] (NFR-01 through NFR-XX)
- Technical debt items identified:        [N] (TD-01 through TD-XX) - Critical: [X], High: [X], Medium: [X], Low: [X]
- CI/CD pipeline files directly read:     [N] (including [N] reusable workflow files)
- CI/CD capabilities confirmed present:   [N of 14]
- Agent 1 LOW CONFIDENCE items resolved:  [N of N total]
- Discrepancies with Agent 1:             [N]

---

## OUTPUT 1 - Technology Stack Assessment
[Full cumulative table - all layers]

## OUTPUT 2 - Architecture Pattern Catalog
[Full final catalog - deduplicated + Pattern Coverage Gaps + Declared-But-Unused sections]

## OUTPUT 3 - Component Interaction & Contract Map
[Full table + Coupling Hotspots + API Contract Inventory]

## OUTPUT 4 - Data Architecture Assessment

### Data Store Deep Dive
| Store | Access Pattern | ORM / Query Style | Transaction Scope | Consistency Model | Connection Pool Config | Migration State | Agent 1 Match? |
|---|---|---|---|---|---|---|---|
| [Store from Agent 1 OUTPUT 3] | [Repository / Direct SQL / ODM / Raw driver] | [ORM name / raw queries / stored procedures] | [Method-level / Manual / None found] | [Strong / Eventual / Unknown] | [NFR-ID reference, or DEFAULT - not declared] | [Migrations present and current / Schema drift risk / No migrations found] | Confirmed / DISCREPANCY |

### Data Ownership Map
| Entity / Table | Owning Service | Other Services With Access | Access Type | Coupling Risk |
|---|---|---|---|---|
| [Entity name] | [Service that owns the write path] | [Other services if any] | [Read-only / Read-write / Shared write] | Tight / Loose / ANTIPATTERN |

### Data Flow & Consistency Notes
[List of notable data consistency patterns, cross-store transactions, or event-driven consistency implementations - with evidence references]

## OUTPUT 5 - Security Architecture Assessment

### Authentication & Authorisation Implementation
| Mechanism | Declared (Agent 1) | Implemented How | Validation Completeness | Gaps | Severity |
|---|---|---|---|---|---|
| [Mechanism from Agent 1 OUTPUT 6] | [Agent 1 finding] | [How it is actually implemented] | [Full / Partial - missing: [list] / Minimal - [detail]] | [Specific gap or RISK] | Critical / High / Medium / Low |

### Secrets Posture
| Item | Finding | Severity | Evidence |
|---|---|---|---|
| [e.g. Database connection string] | [Managed via Vault / Env variable / Hardcoded - CRITICAL] | Critical / High / Medium / Low | [File reference] |

### Attack Surface Summary
| Surface | Exposure | Mitigations Found | Gaps |
|---|---|---|---|
| [e.g. Public REST API] | [e.g. 12 endpoints, 3 unauthenticated] | [Rate limiting on /auth; JWT on all other routes] | [No CORS policy - HIGH] |

## OUTPUT 6 - NFR Registry
[Full cumulative register - NFR-01 through NFR-N, all layers]

## OUTPUT 7 - Technical Debt & Risk Register
[Full cumulative register - TD-01 through TD-N, all layers, sorted by severity descending]

## OUTPUT 8 - Operational Architecture Assessment
[Full four-section assessment: CI/CD Pipeline Maturity (evidence-based) + Observability Coverage + Deployment Safety + DR Posture]

---

## Validation Queue
[All unresolved LOW CONFIDENCE and ASSUMED items from all chunks and the Synthesis Pass -
listed with chunk number, layer, and reason for uncertainty]

## Agent 1 Discrepancy Log
[Every case where deep analysis contradicted Agent 1's inventory -
what Agent 1 said, what the implementation showed, and whether it has been resolved]

---
Agent 2 Analysis Complete.
Documentation is ready for technical review.
Highest-priority action item: [top Critical or High severity TD entry, or "None - no critical risks identified"]
```

---

# Escalation Triggers

**Stop and ask the user** if any of the following conditions are met before proceeding:

- **Agent 1's minimum required outputs are missing** - OUTPUT 1, OUTPUT 2, and OUTPUT 3 must all be present; if any are absent -> stop; ask the user to run Agent 1 first
- **More than 50% of Agent 1's items are flagged LOW CONFIDENCE** - ask the user to review Agent 1's Validation Queue before Agent 2 proceeds
- **Agent 1's Component Map contains only 1 component with no data store or integration** - most likely a scanning failure or a library project; confirm with the user before proceeding
- **Deep analysis in Chunk 1 reveals a fundamentally different architecture than Agent 1 described** (event sourcing, CQRS read-model, multi-tenancy, service mesh) -> stop; ask whether Agent 1 should re-run with this knowledge first
- **Security layer analysis reveals plaintext secrets committed to source control** -> stop immediately; alert the user to this Critical security risk before completing any remaining analysis; include location only, never the secret value

**Flag and continue** (do not stop) if:

- A single layer produces no architecture patterns -> note it in the Layer Summary and continue
- A technology from Agent 1's inventory has no usage evidence in source -> mark `LOW - declared in manifest but no usage found` and continue
- An NFR category has no declared values -> log `[Category] NFRs: None declared - system is using framework defaults; defaults may be unbounded and represent a scalability or reliability risk` and continue
- No CI/CD pipeline files are found when performing the CI/CD layer direct read -> flag `CI/CD LAYER: No pipeline files found in expected locations; Stage 8 CI/CD Maturity will be assessed as all Absent with Critical severity gaps` and continue

---

# References

| File | Purpose |
|---|---|
| `TA_Agent1_StackScout_v2.md` | Required pair agent - Agent 2 cannot begin without Agent 1's 6 output files; run Agent 1 first and provide its output before activating Agent 2 |

---

*Technology Architecture Reverse Engineering System - Agent 2 of 2 | v2 | June 2026*
*Pair with: TA_Agent1_StackScout_v2.md*

---

## Reminder on output

Please write all 9 files directly to C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/ta-outputs/ using your file tools as you
go, then reply with the ✅/❌ checklist:

1. technology-stack-assessment.md
2. architecture-pattern-catalog.md
3. component-interaction-contract-map.md
4. data-architecture-assessment.md
5. security-architecture-assessment.md
6. nfr-registry.md
7. technical-debt-risk-register.md
8. operational-architecture-assessment.md
9. ta-review-summary.md
