# eShopOnWeb — Enterprise Forward Engineering Package

> **Reverse-engineered. Evidence-anchored. Ready for code generation.**

A complete enterprise architecture package reconstructed from the [eShopOnWeb](https://github.com/dotnet-architecture/eShopOnWeb) .NET 8 reference application — built for **AI-driven forward engineering** on any modern stack, without requiring access to the legacy source code.

---

## What This Is

This repository is the output of a multi-stage reverse-engineering pipeline that:

1. **Extracted** every class, interface, API, dependency, and infrastructure component from the legacy .NET 8 codebase using a Python pipeline
2. **Analyzed** the extracted data through 4 LLM architecture layers (Business → Data → Application → Technology)
3. **Built** a 274-node Enterprise Knowledge Graph — the single source of truth for everything downstream
4. **Produced** 20 architecture documents covering all TOGAF layers including Frontend and UI/UX
5. **Verified** all findings against the real eShopOnWeb GitHub source — 97% accuracy

Every fact, node, API, entity, and capability in this package traces back to a graph node. **Nothing was invented.**

---

## Pipeline Overview

```
eShopOnWeb .NET 8 Source Code
           │
           ▼
   Python Extraction Layer
           │
           ▼
   LLM Analysis — 4 Architecture Layers
   ├── Business Architect  →  capabilities, actors, processes
   ├── Data Architect      →  entities, aggregates, repositories
   ├── Application Arch.   →  services, APIs, interfaces
   └── Technology Arch.    →  stack, infrastructure, security
           │
           ▼
   Enterprise Knowledge Graph
   274 nodes · 9 sections · confidence-rated · source-cited
           │
           ▼
   Enterprise Foundation Package (5 docs)
           │
           ▼
   Forward Engineering Package (20 docs)
           │
           ▼
   Accuracy: 97% · Status: Complete · Pending: Stack Decision (GR-08)
```

---

## Repository Structure

### `enterprise-foundation-package/` — Evidence Base

The canonical foundation layer. Also mirrored to `bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/foundation/` as the pipeline output location.

| File | Description |
|---|---|
| `ENTERPRISE_KNOWLEDGE_GRAPH.json` | Single source of truth — 274 nodes, 9 sections |
| `CANONICAL_ENTERPRISE_MODEL.md` | Human-readable normalized enterprise model |
| `TRACEABILITY_MATRIX.md` | Capability → Process → Entity → Service → API traceability |
| `ARCHITECTURE_INVENTORY.md` | Flat inventory of every graph node |
| `FORWARD_ENGINEERING_INPUT_MAP.md` | Graph → forward-engineering document mapping |

---

### `forward-engineering-package/` — 20-Document FE Specification

#### Business Layer
| # | Document | Coverage |
|---|---|---|
| 01 | Business Requirements Document (BRD) | 39 capabilities, business rules, actors |
| 02 | Business Capability Model | Capability hierarchy, BC-01..BC-07 |
| 03 | Use Case Specification | All actor-system interactions |
| 04 | Business Process Model | 10 processes with flow diagrams |

#### Data Layer
| # | Document | Coverage |
|---|---|---|
| 05 | Domain Model (DDD) | 15 entities, 4 aggregates, value objects |
| 06 | Data Dictionary | All fields, types, constraints |
| 07 | Data Model Specification | Logical + **Physical** model + **PostgreSQL DDL** |
| 08 | Entity Relationship Diagram | Full ERD with FK relationships |
| 09 | Data Flow Diagram | L0 / L1 / L2 DFDs |

#### Application Layer
| # | Document | Coverage |
|---|---|---|
| 10 | Service Catalog | 47 services/modules with interfaces |
| 11 | API Contract Specification | All **55 APIs** with request/response contracts |

#### Technology Layer
| # | Document | Coverage |
|---|---|---|
| 12 | Technology Blueprint | Current stack, target options, infra |
| 13 | Security Architecture | Findings + **Modernization Plan** + **RBAC Authorization Model** |
| 14 | NFR Specification | Performance, availability, scalability targets |

#### Governance & Deployment
| # | Document | Coverage |
|---|---|---|
| 15 | Forward Engineering Specification | 89 rules · 68 gates · Generation Policy · Implementation Guidelines |
| 16 | Generation Manifest (JSON) | Machine-readable generation input |
| 17 | Forward Engineering Readiness Report | Audit trail, accuracy assessment, amendments |
| 18 | Deployment Architecture | Infrastructure, containers, CI/CD |

#### Frontend Layer
| # | Document | Coverage |
|---|---|---|
| 19 | Frontend Architecture | 2 surfaces · 43 routes · component inventory · security wiring |
| 20 | UI/UX Specification | 20-page inventory · 4 user flows · field-level validation rules |

---

### `forward-engineering-completion-package/` — Merged Artifacts (Historical)

Completion documents produced during the EARB audit cycle. All content has been merged into the core 20 documents above. Retained as historical record.

---

### `prompts-ready-to-use/` — Run These

**8 complete, fully assembled prompts. Paste directly into Claude. No setup needed.**

| # | File | What it does |
|---|---|---|
| 01 | `01_BA_Agent1_StructuralScout.md` | Business layer — scan codebase structure |
| 02 | `02_BA_Agent2_DeepAnalyst.md` | Business layer — deep analysis, rules, processes |
| 03 | `03_DA_Agent1_DataExtractor.md` | Data layer — extract schema, entities, migrations |
| 04 | `04_DA_Agent2_DataReviewer.md` | Data layer — review and validate findings |
| 05 | `05_TA_Agent1_StackScout.md` | Technology layer — scan stack, infra, CI/CD |
| 06 | `06_TA_Agent2_DeepAnalyst.md` | Technology layer — deep analysis, risks, NFRs |
| 07 | `07_AA_Agent1_AppExtractor.md` | Application layer — services, APIs, dependencies |
| 08 | `08_AA_Agent2_QualityReview.md` | Application layer — quality review, PASS/FAIL verdict |

**Run order:** 01 → 02 → 03 → 04 → 05 → 06 → 07 → 08
**Each pair:** Agent 1 scans → Agent 2 deep-analyses Agent 1's output
**Works on any codebase** — not just .NET

> See `prompts-ready-to-use/00_README.md` for the full step-by-step run guide.

---

### Prompt Architecture — Historical (Do Not Use for New Runs)

| Folder | Contents | Status |
|---|---|---|
| `prompt-governance/` | GOV-01..10 rules, 22 governed prompts | Superseded by prompts-ready-to-use |
| `prompt-v2/` | 10 spec stubs (2 per layer) — not fully assembled | Superseded by prompts-ready-to-use |
| `prompt-refactored/` | Intermediate refactored prompts | Historical |
| `prompt-resolved/` | Resolved prompt generation | Historical |

---

## Knowledge Graph — Key Facts

| Category | Count |
|---|---|
| Business Capabilities | 39 |
| Business Processes | 10 |
| Domain Entities | 15 |
| DDD Aggregates | 4 |
| Services / Modules | 47 |
| Interfaces | 13 |
| APIs | 55 |
| Runtime Dependencies | 19 |
| Current Stack Technologies | 26 |
| Infrastructure Components | 8 |
| Security Components | 17 |
| **Total Graph Nodes** | **274** |

---

## Accuracy & Verification

| Item | Status |
|---|---|
| EARB Audit Score | 79/100 → **97% after corrections** |
| Ground-truth verification | Verified against real eShopOnWeb GitHub source |
| DISC-001 (stock fields) | Found, annotated, physically removed from DDL |
| All 20 docs node-traced | Every fact has a graph node citation |
| Docs 09, 12, 13 | Independently verified clean |

---

## Generation Targets

Technology-neutral — designed to generate on any modern stack:

| Layer | Options |
|---|---|
| **Backend** | Java Spring Boot · ASP.NET Core · Node.js · Python/FastAPI |
| **Frontend (Storefront)** | Next.js (React SSR) · Angular Universal · Nuxt (Vue SSR) · Razor Pages |
| **Frontend (Admin SPA)** | React (Vite) · Angular SPA · Vue (Vite) · Blazor WASM |
| **Database** | PostgreSQL · SQL Server · MySQL |
| **Auth** | OIDC + PKCE · ASP.NET Core Identity |
| **Deployment** | Docker · Kubernetes · Cloud-native |
| **Architecture style** | Modular Monolith · Microservices |

---

## Readiness Status

| Gate | Status |
|---|---|
| All 20 documents complete | **DONE** |
| 97% accuracy verified | **DONE** |
| Physical data model + DDL | **DONE** |
| Security modernization plan | **DONE** |
| RBAC authorization model | **DONE** |
| Frontend + UI/UX spec | **DONE** |
| Implementation guidelines (89 rules) | **DONE** |
| **Target stack decision (GR-08)** | **PENDING — human decision required** |

> One decision unlocks code generation: choose a target stack and record it in `16_GENERATION_MANIFEST.json`.

---

## How to Use

**Step 1 — Read the readiness report**
```
forward-engineering-package/17_FORWARD_ENGINEERING_READINESS_REPORT.md
```

**Step 2 — Resolve the one open gate**
Record your target stack decision in `16_GENERATION_MANIFEST.json` under `target_stack`.

**Step 3 — Feed to a code generator**
```
Primary inputs:
  16_GENERATION_MANIFEST.json          ← machine-readable generation plan
  15_FORWARD_ENGINEERING_SPECIFICATION.md  ← 89 rules + 68 gates

Supporting inputs (by layer):
  01-04  Business requirements and processes
  05-09  Data model, DDL, ERD
  10-11  Service catalog + 55 API contracts
  12-14  Tech blueprint, security, NFRs
  19-20  Frontend architecture + UI/UX
```

**Step 4 — Generate in wave order**
The manifest defines generation waves (data → domain → application → API → frontend). Follow wave order to respect dependency constraints.

---

## Important Notes

- `Buyer` / `CustomerProfile` — **aspirational**, not implemented in legacy (RC-002)
- `PaymentMethod` — **inferred/low-confidence** (BIZ-CAP-027/028); no payment UI generated
- `target_stack` — **empty by design**; no legacy evidence for target technologies; all options are neutral
- `DISC-001` — `CatalogItem` stock fields (`AvailableStock`, `RestockThreshold`, `MaxStockThreshold`, `OnReorder`) are absent from the real eShopOnWeb source and have been removed from all data artifacts
- `OQ-001` — Admin module (APP-SVC-005) vs BlazorAdmin (APP-SVC-016) merge decision is unresolved; keep separate until decided

---

*Documentation and architecture artifacts only — contains no application source code or compiled binaries.*
