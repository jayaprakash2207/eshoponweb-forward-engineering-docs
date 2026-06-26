# 01 — Target Technology Stack Decision (closes C1)

**Closes:** Audit C1 — "No target technology stack decision" (target_stack = 0 nodes, GR-08 halt).
**Authority:** doc 12 (Technology Blueprint), graph `technology.current_stack` (26 nodes), doc 15 GR-08.
**Nature:** This document provides the **decision framework + a recommendation**. The final selection is
**🟦 REQUIRES HUMAN DECISION** (GR-08) — generation halts until an owner records the choice in §7.

---

## 1. Current technology (DERIVED — legacy, evidence-anchored)

| Layer | Current (legacy) | Node | Confidence |
|---|---|---|---|
| Runtime/SDK | .NET 8.0 | TECH-CUR-001 | HIGH |
| Web framework | ASP.NET Core MVC + Razor Pages | TECH-CUR-002 | HIGH |
| Admin SPA | Blazor WebAssembly (hosted) | TECH-CUR-003 | HIGH |
| ORM | EF Core (SqlServer/Npgsql/InMemory providers) | TECH-CUR-005..008 | LOW (versions undeclared) |
| DB engine | Azure SQL Edge (container); PostgreSQL provider also present | TECH-CUR-020/021 | LOW (EOL/untagged) |
| Patterns | MediatR, Ardalis (Specification/Result/ApiEndpoints), FluentValidation | TECH-CUR-009..015 | LOW (declared-only) |
| Containerization | Docker + docker-compose (3 services) | TECH-INF-001..004 | HIGH |
| CI/CD | GitHub Actions (build/test only) | TECH-INF-005 | HIGH |

## 2. Supported target technologies (NEUTRAL OPTIONS — mandated set)

Backend: **Java Spring Boot · ASP.NET Core · Node.js · Python**
Frontend: **React · Angular · Vue**
Database: **PostgreSQL · MySQL · SQL Server**
Architecture style: **Modular Monolith · Microservices · Cloud Native**

> Per the neutrality contract (graph target_stack empty), none of these is in legacy evidence. They are
> candidate options; selection is the human decision this document frames.

## 3. Evaluation criteria

Each option scored 1–5 (5 best) on the audit-mandated criteria, weighted for an evidence-grounded
modernization of a layered .NET monolith with a known module cycle and weak boundaries.

| Criterion | Weight | Rationale |
|---|---|---|
| Migration impact (closeness to legacy) | 0.20 | lower translation risk preserves the 55-API contract surface |
| Complexity | 0.15 | team ramp-up + idiom distance |
| Risk | 0.15 | correctness risk during translation |
| Cost | 0.10 | licensing + hosting + staffing |
| Maintainability | 0.15 | long-term change cost |
| AI generation compatibility | 0.15 | density of conventions/scaffolding an agent can target |
| Cloud-native fit | 0.10 | containerization + 12-factor readiness |

## 4. Backend decision matrix

| Backend | Migration impact | Complexity | Risk | Cost | Maintainability | AI-gen compat | Cloud-native | **Weighted** |
|---|--:|--:|--:|--:|--:|--:|--:|--:|
| **ASP.NET Core** | 5 | 5 | 5 | 4 | 4 | 5 | 5 | **4.70** |
| Java Spring Boot | 3 | 3 | 4 | 4 | 5 | 5 | 5 | 4.05 |
| Node.js (NestJS) | 3 | 4 | 3 | 5 | 4 | 4 | 5 | 3.85 |
| Python (FastAPI) | 2 | 4 | 3 | 5 | 4 | 4 | 5 | 3.70 |

**Per-option pros/cons:**

- **ASP.NET Core** — *Pros:* identical runtime to legacy (TECH-CUR-001/002), EF Core/MediatR/FluentValidation carry over 1:1, lowest translation risk, native DDD/Clean-Architecture idioms. *Cons:* keeps the org on .NET (no diversification); Windows-licensing perception (mitigated — runs on Linux containers). *Migration impact:* minimal. *Risk:* lowest.
- **Java Spring Boot** — *Pros:* mature DDD/hexagonal ecosystem (Spring Modulith, JPA, Bean Validation), strongest enterprise governance tooling, excellent AI-scaffolding density. *Cons:* full language/idiom rewrite from C#; higher translation risk on EF→JPA mapping. *Migration impact:* high.
- **Node.js (NestJS)** — *Pros:* NestJS DI mirrors ASP.NET structure, TypeScript types help the AI, cheapest hosting. *Cons:* weaker for heavy transactional/aggregate invariants; ORM (TypeORM/Prisma) maturity gap vs EF Core. *Migration impact:* high.
- **Python (FastAPI)** — *Pros:* fastest scaffolding, Pydantic validation maps cleanly to VO/DTO. *Cons:* furthest idiom distance from a layered C# monolith; weakest static-typing guarantees for aggregate invariants. *Migration impact:* highest.

## 5. Frontend decision matrix

| Frontend | Migration impact (from Blazor WASM) | Complexity | AI-gen compat | Maintainability | **Weighted** |
|---|--:|--:|--:|--:|--:|
| **React** | 3 | 4 | 5 | 4 | **4.10** |
| Angular | 3 | 3 | 5 | 5 | 4.05 |
| Vue | 3 | 4 | 4 | 4 | 3.80 |

- **React** — largest ecosystem + highest AI-scaffolding density; *con:* opinionated patterns vary.
- **Angular** — closest structural analogue to Blazor (components, DI, TypeScript, routing); strongest for an enterprise admin SPA; *con:* steeper learning curve.
- **Vue** — simplest; *con:* smaller enterprise footprint.

> The legacy frontend is a **single Blazor WASM admin SPA** (APP-SVC-016) — small surface. All three are low-risk.

## 6. Database decision matrix

| Database | Migration impact | Risk | Cost | Maintainability | Cloud-native | **Weighted** |
|---|--:|--:|--:|--:|--:|--:|
| **PostgreSQL** | 4 | 4 | 5 | 5 | 5 | **4.55** |
| SQL Server | 5 | 5 | 3 | 4 | 4 | 4.40 |
| MySQL | 3 | 3 | 5 | 4 | 5 | 3.85 |

- **PostgreSQL** — *Pros:* legacy already carries an Npgsql provider (TECH-CUR-007/021 — DERIVED), open-source/low-cost, best cloud-native fit, native JSONB. *Cons:* EF/ORM provider nuances vs SQL Server.
- **SQL Server** — *Pros:* identical to the dominant legacy provider (Azure SQL Edge, TECH-CUR-020), zero data-type translation. *Cons:* licensing cost; Azure SQL Edge is **EOL** (must move to Azure SQL DB / SQL Server 2022).
- **MySQL** — *Pros:* cheap, ubiquitous. *Cons:* no legacy provider evidence; weakest for the owned-type/embedded-VO mapping (Address, CatalogItemOrdered).

## 7. Architecture-style decision

| Style | When | Evidence |
|---|---|---|
| **Modular Monolith (RECOMMENDED FIRST)** | Start here | doc 12 §8; the module **cycle APP-DEP-001** + weak boundaries make in-process modules the safe first cut; clean seams via AR-06 |
| Microservices (incremental) | After cycle broken + CatalogContext split | doc 15 MR-01..07; extract **BC-04 Identity** first (coupling 8, isolated DB), then **BC-03 Ordering** (coupling 4) |
| Cloud Native | Cross-cutting target posture | 12-factor (doc 15 IR-02), containers (TECH-INF-001..003), externalized config/secrets |

## 8. Board recommendation (⚠ NEUTRAL DEFAULT — overridable)

> **Lowest-risk, evidence-aligned default:**
> **ASP.NET Core (.NET 8) + Angular + PostgreSQL, Modular-Monolith-first, Cloud-Native (Docker→K8s).**
>
> Rationale: minimizes the migration-impact/risk dimensions the audit weighted highest (identical runtime,
> EF Core/MediatR/FluentValidation carry over, Npgsql provider already present), while PostgreSQL removes
> the SQL-Server licensing + Azure-SQL-Edge-EOL problems and maximizes cloud-native fit. Angular is the
> closest structural analogue to the legacy Blazor admin SPA.
>
> **Alternative if technology diversification is a strategic goal:** Java Spring Boot + React + PostgreSQL
> (highest maintainability/AI-gen scores, at higher migration cost).

## 9. 🟦 THE DECISION (must be recorded before generation — GR-08 halt)

| Slot | Options | Selected | Decided by | Date |
|---|---|---|---|---|
| Backend | ASP.NET Core / Spring Boot / Node.js / Python | ☐ __________ | __________ | ____ |
| Frontend | React / Angular / Vue | ☐ __________ | __________ | ____ |
| Database | PostgreSQL / SQL Server / MySQL | ☐ __________ | __________ | ____ |
| Style | Modular Monolith / Microservices / Cloud Native | ☐ __________ | __________ | ____ |

Until this table is completed by a human owner, the AI agent **MUST halt** (doc 15 GR-08). This document
makes the decision *informed and trivial*; it does not make it *for* the organization.

## 10. Migration impact summary (for the recommended default)

| From (legacy) | To (default) | Effort | Notes |
|---|---|---|---|
| .NET 8 / ASP.NET Core | .NET 8 / ASP.NET Core | **Minimal** | same runtime; re-architect, don't rewrite |
| Blazor WASM admin | Angular SPA | Medium | small surface (1 SPA); re-implement against PublicApi |
| Azure SQL Edge | PostgreSQL | Medium | EF provider swap (Npgsql already present); physical model in doc 02 |
| docker-compose | Docker → Kubernetes | Medium | author IaC (gap G-M7) |
| Module cycle APP-DEP-001 | DAG (AR-03) | **High** | re-architecture, gated VR-03 — the dominant effort regardless of stack |
