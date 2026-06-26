# eShopOnWeb — Enterprise Forward Engineering Package

Technology-neutral, evidence-anchored architecture documentation reverse-engineered from the
[eShopOnWeb](https://github.com/dotnet-architecture/eShopOnWeb) reference application and
re-packaged for **AI-driven forward engineering** — i.e. regenerating the application on a
modern stack **without access to the legacy source code**.

Every artifact traces back to a single source of truth: the **Enterprise Knowledge Graph**
(274 evidence-anchored nodes). No capabilities, services, entities, APIs, or technologies were invented.

## Contents

### `enterprise-foundation-package/` — the evidence base
| File | Description |
|---|---|
| `ENTERPRISE_KNOWLEDGE_GRAPH.json` | Single source of truth — 274 nodes across business, data, application, technology + cross-links |
| `CANONICAL_ENTERPRISE_MODEL.md` | Human-readable canonical model |
| `TRACEABILITY_MATRIX.md` | Capability → Process → Entity → Service → API traceability |
| `ARCHITECTURE_INVENTORY.md` | Flat inventory of every node |
| `FORWARD_ENGINEERING_INPUT_MAP.md` | Maps the graph to forward-engineering inputs |

### `forward-engineering-package/` — the 17-document FE specification
| # | Document |
|---|---|
| 01 | Business Requirements Document |
| 02 | Business Capability Model |
| 03 | Use Case Specification |
| 04 | Business Process Model |
| 05 | Domain Model (DDD) |
| 06 | Data Dictionary |
| 07 | Data Model Specification |
| 08 | ERD |
| 09 | Data Flow Diagram (L0/L1/L2) |
| 10 | Service Catalog |
| 11 | API Contract Specification |
| 12 | Technology Blueprint |
| 13 | Security Architecture |
| 14 | NFR Specification |
| 15 | Forward Engineering Specification (master) |
| 16 | Generation Manifest (`.json`, machine-readable) |
| 17 | Forward Engineering Readiness Report |

## Graph facts
39 capabilities · 10 processes · 15 entities · 4 aggregates · 47 services/modules · 13 interfaces ·
55 APIs · 19 dependencies · 26 current-stack technologies · 8 infrastructure components · 17 security components.

## Supported generation targets
The package is technology-neutral and designed to support generation of:
- **Backend:** Java Spring Boot · ASP.NET Core · Node.js · Python
- **Frontend:** React · Angular · Vue
- **Databases:** PostgreSQL · SQL Server · MySQL
- **Deployment:** Docker · Kubernetes
- **Styles:** Modular Monolith · Microservices · Cloud-native

## Readiness

**Conditional — 79/100.** Suitable for forward-engineering the implemented bounded contexts once
the conditions in document 17 are met: an explicit target-stack decision, breaking the legacy
module dependency cycle (`APP-DEP-001`), and resolving the documented open questions. See
`forward-engineering-package/17_FORWARD_ENGINEERING_READINESS_REPORT.md` for the full assessment.

## How to use
1. Read `17_FORWARD_ENGINEERING_READINESS_REPORT.md` and resolve the pre-generation conditions.
2. Choose a target stack.
3. Feed `16_GENERATION_MANIFEST.json` + `15_FORWARD_ENGINEERING_SPECIFICATION.md` to an agentic code generator, ordered by the manifest's generation priorities.

## Provenance & notes
- Status flags from the legacy analysis are preserved: `Buyer` and `PaymentMethod` are **aspirational/unimplemented** (not in the persisted schema); payment capabilities are **inferred/low-confidence**; the legacy target stack is **empty by design** (target technologies are offered as neutral options, never asserted as discovered).
- The authoritative legacy system name is **unknown** in the source evidence; `eShopOnWeb` is used as the project label.

---
*Documentation package only — contains no source code or implementation artifacts.*
