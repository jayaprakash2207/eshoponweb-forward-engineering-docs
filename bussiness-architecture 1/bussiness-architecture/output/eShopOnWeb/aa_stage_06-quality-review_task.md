I'm working through an application architecture analysis in stages, and this is
stage "06-quality-review". Earlier stages (inventory, parser, evidence packs) were
already produced deterministically by a Python analyzer and are provided below
as INPUT CONTEXT.

Please don't write to disk or rescan the repo — instead, write out each output
file listed in this stage's "Output" section, wrapping each one in these markers
so I can save them to disk programmatically:

===AA_FILE_START:<relative/path/filename.ext>===
<full file content>
===AA_FILE_END===

Please use the exact relative paths from the stage's Output section (e.g. final/system-inventory.json), and emit valid JSON for .json files.

--- 00 GLOBAL RULES (apply to this stage) ---
# 00 - Global Rules

Use this file for every Application Architecture extraction stage.

## Scope

This workflow reverse engineers Application Architecture from a legacy repository and produces SDLC reverse-engineering and forward-engineering inputs.

Do not perform business architecture, security deep dive, data migration design, cloud design, or testing strategy unless a later prompt explicitly asks for it.

## Safety

- Do not modify legacy application source code.
- Do not delete files.
- Do not refactor or format the legacy application.
- Do not install heavy dependencies without approval.
- Write generated outputs only under `architecture-output/`.
- Write analyzer tooling only under `tools/application_architecture_analyzer/`.

## Evidence Rules

- Every major claim must include source evidence.
- Use file paths and line numbers where available.
- Use confidence scores.
- If evidence is missing, write `unknown`.
- Add unresolved uncertainty to `architecture-output/final/open-questions.md` or the relevant stage output.
- Do not invent modules, flows, deployment topology, cloud platform, database ownership, queue ownership, or business rules.

## Ignore Rules

Do not analyze these as architecture source:

```text
.git/
node_modules/
bin/
obj/
target/
dist/
build/
coverage/
logs/
generated/
*.min.js
*.map
compiled binaries
large generated files
```

## Process Rule

Parse first, reason second:

```text
inventory -> parsed facts -> evidence packs -> final architecture -> enterprise forward engineering -> quality review
```

Never jump directly from raw source files to final architecture.


--- STAGE PROMPT: 06-quality-review-agent.md ---
# 06 - Quality Review Agent

## Role

Review generated architecture outputs for completeness, traceability, consistency, and usefulness.

## Input

```text
architecture-output/final/
```

## Output

```text
architecture-output/final/quality-review.md
architecture-output/final/executive-summary-for-review.md
architecture-output/final/final-sanity-check.md
```

## Check

- required files exist
- JSON is valid
- modules match component registry
- dependency edges resolve to nodes
- call-flow steps reference components
- diagrams match JSON artifacts
- claims have evidence
- risks have affected module/component
- unknowns are open questions
- no invented cloud/platform/runtime assumptions
- forward-engineering files are actionable

## Mark

Use:

```text
PASS / PARTIAL / FAIL
```

Explain PARTIAL or FAIL items clearly.


--- INPUT CONTEXT ---
### final/api-contract-preservation-map.json
```
{
  "schema_version": "1.0",
  "stage": "05-enterprise-forward-engineering",
  "generated_from": [
    "final/architecture-pattern-report.md",
    "final/architecture-violation-register.json"
  ],
  "notes": "Only contracts with direct evidence in this stage's input context are listed. The full PublicApi endpoint inventory was not provided; additional endpoints likely exist and should be appended once the component inventory is available.",
  "contracts": [
    {
      "contract_id": "API-001",
      "endpoint_name": "CreateCatalogItemEndpoint",
      "module": "PublicApi (CatalogItemEndpoints)",
      "file": "src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs",
      "known_consumers": [
        {
          "consumer": "BlazorAdmin",
          "via": "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
          "confidence": 0.5,
          "evidence_note": "Violation register states this decorator 'appears to wrap calls into PublicApi's CatalogItemEndpoints' — relationship is inferred, not confirmed by direct call-site evidence in this stage's input."
        }
      ],
      "preservation_recommendation": "preserve",
      "rationale": "BlazorAdmin's catalog services are coupled to this endpoint's request/response shape (ARCH-VIOL-003). Breaking changes to this contract would require coordinated BlazorAdmin changes.",
      "related_violations": ["ARCH-VIOL-003"],
      "confidence": 0.5
    },
    {
      "contract_id": "API-002",
      "endpoint_name": "CatalogItemEndpoints (full set: list/update/delete)",
      "module": "PublicApi (CatalogItemEndpoints)",
      "file": "unknown",
      "known_consumers": [
        {
          "consumer": "BlazorAdmin (CatalogItemPage)",
          "via": "src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor and related services",
          "confidence": 0.4,
          "evidence_note": "Referenced generically in architecture-pattern-report.md as 'src/PublicApi/CatalogItemEndpoints/*'; specific endpoint files beyond Create were not enumerated in provided evidence."
        }
      ],
      "preservation_recommendation": "review",
      "rationale": "Likely subject to the same coupling as API-001, but individual endpoint files were not confirmed in this stage's input. Recommend full enumeration before finalizing preservation scope.",
      "related_violations": ["ARCH-VIOL-003"],
      "confidence": 0.4
    }
  ]
}
```

### final/architecture-decision-inputs.md
```
# Architecture Decision Inputs

> Compiled questions and decision points for architects, derived from this stage's analysis. None of these are pre-decided.

## Decisions Stemming from Confirmed Violations

### AD-001: Canonical Catalog Admin UI (ARCH-VIOL-002, severity Medium)
- **Question:** Is `src/Web/Pages/Admin/EditCatalogItem.cshtml` (Razor Pages) or `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor` (Blazor) the canonical/actively-maintained Catalog Administration surface?
- **Why it matters:** Drives MCC-001 (module-consolidation-map.json), CAP-005 ownership, and Wave 1 of migration-wave-plan.md.
- **Evidence:** Both files exist and implement overlapping functionality; no usage data available to distinguish.
- **Confidence of question's validity:** 0.5

### AD-002: Intent of `CatalogItemOrdered` Snapshot (ARCH-VIOL-001, severity Low)
- **Question:** Was the `CatalogItemOrdered` value object (embedding Catalog data inside `OrderItem`) designed as a deliberate anti-corruption/historical-record pattern, or is it accidental coupling that arose organically?
- **Why it matters:** If deliberate, it should be documented as a bounded-context boundary and explicitly preserved in any Catalog/Order service split (per the violation register's own migration_impact note). If accidental, it may warrant redesign.
- **Evidence:** `src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs`
- **Confidence of question's validity:** 0.55

### AD-003: PublicApi Contract Formalization (ARCH-VIOL-003, severity Medium)
- **Question:** Should the `CatalogItemEndpoints` consumed by `BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs` be formally versioned/published as a contract, and who owns changes to it?
- **Why it matters:** Determines how much coordination overhead is needed for future Catalog changes, and informs Wave 2 of migration-wave-plan.md.
- **Evidence:** `src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs`, `src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs`
- **Confidence of question's validity:** 0.5

## Decisions Requiring Additional Evidence (Not Resolvable This Stage)

### AD-004: Service Boundary Option Selection
- **Question:** Which of Option A (status quo), Option B (aggregate-aligned services), or Option C (front-end decoupling only) — or a hybrid — should guide forward engineering?
- **Blocking gaps:** Database/persistence topology (`data-ownership-map.md` marks this `unknown`), test/runtime evidence (`test-runtime-evidence-map.md`).

### AD-005: Existence and Scope of Non-Evidenced Capabilities
- **Question:** Does the system include Identity/Authentication, Payment, Shipping/Notification, or other capabilities not referenced in `architecture-pattern-report.md` or `architecture-violation-register.json`?
- **Why it matters:** Forward engineering backlog and capability map (`business-capability-map.json`) would be incomplete if these exist.
- **Recommended action:** Re-run capability extraction once full `architecture-output/final/system-inventory.json` and component catalog are available.

### AD-006: Database/Schema Ownership Per Aggregate
- **Question:** Is there a single shared database, or per-module/aggregate schema separation?
- **Why it matters:** Directly gates feasibility of service-boundary Option B.
- **Blocking gap:** No `Infrastructure`/persistence evidence was provided to this stage.

## Summary

| ID | Decision | Owner | Blocking |
|----|----------|-------|----------|
| AD-001 
... [truncated]
```

### final/architecture-pattern-report.md
```
# Architecture Pattern Report

## Detected Pattern

**Primary: Clean Architecture / Onion Architecture, organized as a Modular Monolith with N-tier separation.**

Confidence: **0.7**

## Evidence

- `src/ApplicationCore/` contains domain entities (`Entities/BasketAggregate`, `Entities/BuyerAggregate`, `Entities/OrderAggregate`, `Entities/CatalogItem.cs`, etc.), interfaces (`Interfaces/IRepository.cs`, `IReadRepository.cs`, `IAggregateRoot.cs`), application services (`Services/BasketService.cs`, `Services/OrderService.cs`), and specifications (`Specifications/*`) — a textbook Domain + Application layer separated from infrastructure and presentation, consistent with Clean/Onion Architecture.
- `src/Infrastructure/` is referenced as a project dependency from `src/PublicApi/PublicApi.csproj` (system-inventory-pack), consistent with the "Infrastructure depends inward on ApplicationCore, outer layers depend on Infrastructure for composition" pattern.
- Two separate presentation/API projects (`src/Web` — Razor Pages/MVC, `src/PublicApi` — Ardalis.ApiEndpoints minimal API) both sit at the outer ring and (per evidence and convention) depend on `ApplicationCore` and `Infrastructure`, rather than on each other — consistent with a **Modular Monolith** where multiple front-ends share one core domain library.
- A third UI, `src/BlazorAdmin` (Blazor WASM/Server admin), consumes `src/PublicApi` endpoints (`src/PublicApi/CatalogItemEndpoints/*`) rather than `ApplicationCore` directly, indicating an N-tier client/API split for the admin surface.
- Aggregate roots (`Basket`, `Order`, `Buyer`) implementing `IAggregateRoot` and using `Specifications` for queries (`BasketWithItemsSpecification`, `CustomerOrdersWithItemsSpecification`, `OrderWithItemsByIdSpec`) reflect DDD-influenced patterns (Specification pattern, Aggregate Root, Repository).

## Why This Pattern Was Selected

The folder/namespace structure cleanly separates Domain+Application concerns (`ApplicationCore`) from Infrastructure and from multiple presentation/API projects, with dependency direction (PublicApi → ApplicationCore, PublicApi → Infrastructure) flowing inward as Clean Architecture prescribes. The presence of multiple deployable front-ends (`Web`, `PublicApi`, `BlazorAdmin`) sharing a single `ApplicationCore`/`Infrastructure` core is the defining trait of a **Modular Monolith** rather than independently deployable microservices — there is no evidence of independent data stores per module, message brokers, or service-to-service network contracts between `Web` and `PublicApi`.

## Competing Possible Patterns

1. **Layered Monolith / N-tier Architecture** (confidence 0.5) — If `ApplicationCore` services (`BasketService`, `OrderService`) are thin pass-throughs over repositories with most logic in controllers/pages, this would look more like a traditional layered (UI → Service → Data) architecture than true Clean Architecture. Evidence pack truncation prevents confirming whether business rules live in the entities (rich domain model) or in the services/controllers (anemic domain model + layered).
2. **Anemic Domain Model** (confidence 0.45) — Entities such as `Order`, `Basket`, `CatalogItem` appear in the evidence primarily as data holders (`BaseEntity`-derived classes); whether they contain behavior/invariants (rich domain model) or are pure DTtype POCOs with logic in `OrderService`/`BasketService` could not be confirmed from the truncated evidence.
3. **Big Ball of Mud (localized)** (con
... [truncated]
```

### final/architecture-violation-register.json
```
{
  "violations": [
    {
      "violation_id": "ARCH-VIOL-001",
      "type": "Cross-Module Leakage",
      "description": "The Order aggregate's CatalogItemOrdered value object embeds a denormalized snapshot of Catalog module data (name, price, image) inside the Order module's persistence model.",
      "affected_module": "MOD-003",
      "affected_components": ["COMP-008"],
      "evidence": [
        {
          "file": "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
          "reason": "Value object capturing catalog item details at order time, embedded in OrderItem"
        }
      ],
      "severity": "Low",
      "migration_impact": "If Catalog and Order become separate services, this snapshot pattern is actually beneficial (avoids runtime cross-service calls for historical order data) and should likely be preserved, not removed.",
      "recommendation": "Confirm this is an intentional anti-corruption/snapshot pattern rather than accidental coupling; document it as a deliberate bounded-context boundary in forward engineering.",
      "confidence": 0.55
    },
    {
      "violation_id": "ARCH-VIOL-002",
      "type": "Cross-Module Leakage",
      "description": "Two separate UI surfaces appear to provide catalog item administration: src/Web/Pages/Admin/EditCatalogItem.cshtml (Razor Pages) and src/BlazorAdmin/Pages/CatalogItemPage/* (Blazor). This suggests overlapping ownership of the 'Admin' responsibility between the Web module/project and the BlazorAdmin app.",
      "affected_module": "MOD-006",
      "affected_components": ["COMP-030"],
      "evidence": [
        {
          "file": "src/Web/Pages/Admin/EditCatalogItem.cshtml",
          "reason": "Razor Pages admin catalog edit surface"
        },
        {
          "file": "src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor",
          "reason": "Blazor admin catalog edit surface for the same entity"
        }
      ],
      "severity": "Medium",
      "migration_impact": "Forward engineering an 'Admin' module/service would need to decide which UI is canonical; carrying both forward duplicates effort and risks divergent business rules.",
      "recommendation": "Confirm with the team whether src/Web/Pages/Admin is legacy/deprecated in favor of BlazorAdmin, or whether both are actively maintained and serve different audiences.",
      "confidence": 0.5
    },
    {
      "violation_id": "ARCH-VIOL-003",
      "type": "Frontend-Backend Tight Coupling",
      "description": "BlazorAdmin's catalog services (e.g. CachedCatalogItemServiceDecorator) appear to wrap calls into PublicApi's CatalogItemEndpoints, meaning the admin frontend's data contracts are coupled to PublicApi's endpoint request/response shapes.",
      "affected_module": "MOD-006",
      "affected_components": ["COMP-029", "COMP-018", "COMP-020", "COMP-021", "COMP-022"],
      "evidence": [
        {
          "file": "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
          "reason": "Decorator service for catalog item operations in the admin frontend"
        },
        {
          "file": "src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs",
          "reason": "Corresponding backend endpoint for catalog item creation, likely consumed by the above decorator"
        }
      ],
      "severity": "Medium",
      "migration_impact": "Any change to PublicApi catalog endpoint contracts (request/response DTOs, routes) requires coordinated changes in BlazorAdmin - a
... [truncated]
```

### final/business-capability-map.json
```
{
  "schema_version": "1.0",
  "stage": "05-enterprise-forward-engineering",
  "generated_from": [
    "final/architecture-pattern-report.md",
    "final/architecture-violation-register.json"
  ],
  "notes": "Capability candidates derived from the architecture pattern report and violation register only. The full module/component inventory packs were not present in this stage's input context, so several supporting_modules entries reference project/folder names rather than MOD-xxx IDs. Treat all entries as CANDIDATES pending architect review.",
  "capabilities": [
    {
      "capability_id": "CAP-001",
      "name": "Catalog Management",
      "type": "business",
      "status": "candidate",
      "description": "Maintains the product catalog: items, brands, and types, including specifications used to query catalog data.",
      "supporting_modules": [
        "ApplicationCore (Entities/CatalogItem.cs, Specifications/*)",
        "PublicApi (CatalogItemEndpoints/*)"
      ],
      "evidence": [
        {
          "file": "src/ApplicationCore/Entities/CatalogItem.cs",
          "reason": "Core catalog entity referenced in architecture-pattern-report.md"
        },
        {
          "file": "src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs",
          "reason": "API endpoint surface for catalog item management, referenced in ARCH-VIOL-003"
        }
      ],
      "confidence": 0.6,
      "open_questions": [
        "Full set of catalog endpoints (list/update/delete) and Brand/Type entities were not enumerated in the provided evidence packs; confirm against component inventory."
      ]
    },
    {
      "capability_id": "CAP-002",
      "name": "Shopping Basket Management",
      "type": "business",
      "status": "candidate",
      "description": "Manages a buyer's in-progress basket/cart, including basket items and basket-related queries (e.g. basket with items).",
      "supporting_modules": [
        "ApplicationCore (Entities/BasketAggregate, Services/BasketService.cs, Specifications/BasketWithItemsSpecification)"
      ],
      "evidence": [
        {
          "file": "src/ApplicationCore/Entities/BasketAggregate",
          "reason": "Aggregate root referenced in architecture-pattern-report.md as implementing IAggregateRoot"
        },
        {
          "file": "src/ApplicationCore/Services/BasketService.cs",
          "reason": "Application service for basket operations"
        }
      ],
      "confidence": 0.6,
      "open_questions": [
        "Whether BasketService contains business rules (rich domain) or is a thin pass-through is unresolved per architecture-pattern-report.md competing pattern #1."
      ]
    },
    {
      "capability_id": "CAP-003",
      "name": "Order Management",
      "type": "business",
      "status": "candidate",
      "description": "Manages order placement and order history, including a denormalized snapshot of catalog item details at the time of order.",
      "supporting_modules": [
        "ApplicationCore (Entities/OrderAggregate, Services/OrderService.cs, Specifications/CustomerOrdersWithItemsSpecification, OrderWithItemsByIdSpec)"
      ],
      "evidence": [
        {
          "file": "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
          "reason": "Value object capturing catalog item snapshot at order time (ARCH-VIOL-001)"
        },
        {
          "file": "src/ApplicationCore/Services/OrderService.cs",
          "reason": "Application s
... [truncated]
```

### final/business-capability-map.md
```
# Business Capability Map (Candidate)

> Status: **CANDIDATE** — all capabilities below require architect review before being treated as authoritative. Derived from `final/architecture-pattern-report.md` and `final/architecture-violation-register.json` only.

## Candidate Capabilities

| ID | Capability | Type | Supporting Module(s) | Confidence |
|----|------------|------|------------------------|------------|
| CAP-001 | Catalog Management | Business | ApplicationCore (CatalogItem, Specifications), PublicApi (CatalogItemEndpoints) | 0.6 |
| CAP-002 | Shopping Basket Management | Business | ApplicationCore (BasketAggregate, BasketService) | 0.6 |
| CAP-003 | Order Management | Business | ApplicationCore (OrderAggregate, OrderService, CatalogItemOrdered) | 0.6 |
| CAP-004 | Buyer / Customer Profile Management | Business | ApplicationCore (BuyerAggregate) | 0.55 |
| CAP-005 | Catalog Administration | Application | Web (Pages/Admin), BlazorAdmin (CatalogItemPage) | 0.5 |
| CAP-006 | Storefront Web Experience | Application | Web | 0.5 |
| CAP-007 | Public API / Integration Layer | Application | PublicApi | 0.55 |
| CAP-008 | Admin Application Experience | Application | BlazorAdmin | 0.55 |

## Notes

- **CAP-005 (Catalog Administration)** is duplicated across two front-ends (`src/Web/Pages/Admin/EditCatalogItem.cshtml` and `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor`) — see `ARCH-VIOL-002`. This duplication must be resolved (canonical surface chosen) before this capability can be assigned to a single forward-engineering owner.
- **CAP-001/CAP-003** are linked by a deliberate (or possibly accidental) data snapshot: `CatalogItemOrdered` embeds Catalog data inside the Order aggregate (`ARCH-VIOL-001`). This relationship should be preserved as a documented bounded-context boundary, not silently removed, if Catalog and Order are later split.
- **CAP-007/CAP-008** are tightly coupled via `CachedCatalogItemServiceDecorator` → `CatalogItemEndpoints` (`ARCH-VIOL-003`). Any capability boundary drawn around "Catalog" must account for this dependency.
- No evidence was provided in this stage's input context for Identity/Authentication, Payment, or Shipping capabilities. These may exist in the legacy system but are marked `unknown` here pending review of the full inventory pack — do **not** assume they are absent.

## Open Questions

- Confirm whether Buyer/Customer capability has dedicated endpoints/services (no direct evidence in this run).
- Confirm presence/absence of Identity, Payment, and Notification capabilities against the full system inventory (`architecture-output/final/system-inventory.json`), which was not included in this stage's input context.
```

### final/confidence-report.md
```
# Confidence Report — Stage 05 (Enterprise Forward Engineering)

## Input Context Limitations

This stage normally consumes the full contents of `architecture-output/final/`. The input context provided to this run contained only **two** files:

1. `final/architecture-pattern-report.md` (truncated)
2. `final/architecture-violation-register.json` (truncated, 3 of an unknown total number of violations shown)

The following expected inputs were **not** present: system inventory, module catalog, component catalog, data flow / API contract evidence packs, and test/runtime evidence packs. As a result, all outputs in this stage carry a confidence ceiling below what would be possible with the full evidence set, and several items are marked `unknown` rather than inferred.

## Per-Output Confidence Summary

| Output | Overall Confidence | Primary Basis | Key Gaps |
|--------|---------------------|----------------|----------|
| business-capability-map.json/.md | 0.5–0.6 | architecture-pattern-report.md entity/project structure | No full component inventory to confirm Brand/Type/Address sub-entities; Identity/Payment/etc. capabilities unconfirmed |
| module-consolidation-map.json/.md | 0.5–0.6 | architecture-violation-register.json (VIOL-002, VIOL-003) | Module IDs (MOD-xxx) mostly unavailable; only MOD-003/MOD-006 known |
| service-boundary-options.md | 0.45–0.6 | architecture-pattern-report.md (Modular Monolith determination, 0.7) | No database/persistence evidence; options are directional only |
| migration-wave-plan.md | 0.45–0.6 | Derived from violation register + capability map | Sequencing validity depends on Wave 0 confirmations and missing test/runtime evidence |
| preserve-redesign-retire-map.md | 0.5–0.65 | architecture-pattern-report.md + violation register | No retire decisions possible (no usage evidence) — intentional, per rules |
| api-contract-preservation-map.json | 0.4–0.5 | ARCH-VIOL-003 | Only 1 of likely many CatalogItemEndpoints confirmed; consumer relationship inferred ("appears to wrap calls") |
| data-ownership-map.md | 0.5–0.6 | ApplicationCore entity evidence | Physical DB/schema ownership entirely `unknown`; sub-entities inferred |
| test-runtime-evidence-map.json/.md | unknown (by design) | n/a | No test or runtime evidence supplied at all |
| architecture-decision-inputs.md | n/a (question list) | All of the above | — |
| forward-engineering-backlog.md | 0.45–0.6 | All of the above | Effort/priority estimates are illustrative, not measured |

## Overall Assessment

The provided evidence is internally consistent and sufficient to produce **directionally useful candidates**, particularly around the two confirmed violations (ARCH-VIOL-002 Admin UI duplication, ARCH-VIOL-003 PublicApi/BlazorAdmin coupling) and the Modular Monolith determination (0.7 confidence). However, this stage's outputs should be treated as a **first pass** pending:

1. The full module/component inventory (for module IDs and complete entity lists).
2. Test and runtime evidence (for any preserve/redesign/retire decisions beyond "preserve" defaults).
3. Infrastructure/persistence evidence (for data ownership and service-boundary feasibility).

No final architecture JSON was found to be invalid; the quality gate for this stage is satisfied on the data provided, but **completeness**, not validity, is the limiting factor.
```

### final/data-ownership-map.md
```
# Data Ownership Map (Candidate)

> This map reflects entity/aggregate-level ownership as visible in the domain model evidence provided. **Physical database/schema ownership is `unknown`** — no infrastructure/persistence evidence (connection strings, DbContext mappings, schema files) was included in this stage's input context.

## Candidate Data Ownership by Aggregate

| Data / Entity | Candidate Owning Module | Evidence | Notes | Confidence |
|---------------|--------------------------|----------|-------|------------|
| `CatalogItem` (and implied Brand/Type entities) | Catalog (CAP-001) | `src/ApplicationCore/Entities/CatalogItem.cs` | Brand/Type entity files not directly evidenced in this stage's input; assumed to exist alongside CatalogItem per typical Catalog aggregate composition — confirm against inventory. | 0.55 |
| `Basket`, basket items (BasketAggregate) | Shopping Basket (CAP-002) | `src/ApplicationCore/Entities/BasketAggregate`, `BasketWithItemsSpecification` | — | 0.6 |
| `Order`, `OrderItem` (OrderAggregate) | Order Management (CAP-003) | `src/ApplicationCore/Entities/OrderAggregate`, `CustomerOrdersWithItemsSpecification`, `OrderWithItemsByIdSpec` | — | 0.6 |
| `CatalogItemOrdered` (value object, embedded in OrderItem) | Order Management (CAP-003), **contains a denormalized snapshot of Catalog data** | `src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs` (ARCH-VIOL-001) | This is a cross-module data dependency: Order owns the snapshot record, but its *origin* is Catalog data at order time. Document as a deliberate boundary if confirmed (see Wave 0 in migration-wave-plan.md). | 0.55 |
| `Buyer`, `Address` (BuyerAggregate) | Buyer / Customer Profile (CAP-004) | `src/ApplicationCore/Entities/BuyerAggregate` | Address sub-entity inferred from typical BuyerAggregate composition, not directly evidenced — confirm against inventory. | 0.5 |

## Open Questions / Gaps

- **Physical database/schema ownership**: `unknown`. No `Infrastructure`/EF Core `DbContext` or migration evidence was provided in this stage's input context. A single shared database vs. per-aggregate schemas cannot be determined here.
- **Cross-module data dependency** (`CatalogItemOrdered` snapshot): flagged for explicit review in `preserve-redesign-retire-map.md` and `migration-wave-plan.md` Wave 0/3.
- **Brand/Type and Address sub-entities**: presence inferred from standard aggregate composition referenced in architecture-pattern-report.md, but no direct file evidence — verify against `architecture-output/final/component-catalog.json` (not provided to this stage).
- **Read-models / caching**: `BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs` implies a caching layer over Catalog data in the Admin app — ownership of this cache (and its invalidation) is `unknown`.
```

### final/forward-engineering-backlog.md
```
# Forward Engineering Backlog (Candidate)

> Backlog items below are derived from confirmed violations and candidate capability/consolidation maps. Priority and effort are **illustrative placeholders** pending architect review and the additional evidence noted in `confidence-report.md`.

| ID | Title | Type | Related | Priority (indicative) | Effort (indicative) | Confidence |
|----|-------|------|---------|------------------------|----------------------|------------|
| FEB-001 | Decide canonical Catalog Admin UI (Razor Pages vs Blazor) | Decision | AD-001, ARCH-VIOL-002, MCC-001 | High | Low (decision only) | 0.5 |
| FEB-002 | Document `CatalogItemOrdered` snapshot as intentional bounded-context boundary (or redesign if accidental) | Documentation / Decision | AD-002, ARCH-VIOL-001, CAP-001, CAP-003 | Medium | Low–Medium | 0.55 |
| FEB-003 | Formalize PublicApi `CatalogItemEndpoints` as a versioned contract for BlazorAdmin consumers | Technical | AD-003, ARCH-VIOL-003, API-001 | Medium | Medium | 0.5 |
| FEB-004 | Retire or merge non-canonical Catalog Admin UI once FEB-001 is decided | Technical | MCC-001, CAP-005 | Medium (depends on FEB-001) | Medium–High | 0.45 |
| FEB-005 | Enumerate full PublicApi endpoint surface (beyond CreateCatalogItemEndpoint) for contract preservation map | Discovery | API-002, CAP-007 | High | Low | 0.45 |
| FEB-006 | Obtain test/runtime evidence pack and re-run coverage analysis for all candidate capabilities | Discovery | test-runtime-evidence-map.json, all CAP-* | High | Low–Medium | 0.5 |
| FEB-007 | Obtain Infrastructure/persistence evidence to resolve database/schema ownership per aggregate | Discovery | AD-006, data-ownership-map.md | High | Low–Medium | 0.5 |
| FEB-008 | Confirm existence/scope of non-evidenced capabilities (Identity, Payment, etc.) against full system inventory | Discovery | AD-005, business-capability-map.json | Medium | Low | 0.45 |
| FEB-009 | Re-evaluate service boundary options (A/B/C) once FEB-006/FEB-007 complete | Decision | AD-004, service-boundary-options.md | Medium (sequenced after discovery) | n/a | 0.4 |

## Sequencing Note

FEB-001 through FEB-003 are **decision/documentation items** that can proceed independent of additional evidence and directly address the two Medium-severity violations (ARCH-VIOL-002, ARCH-VIOL-003). FEB-005 through FEB-008 are **discovery items** that unblock more confident versions of `business-capability-map.json`, `data-ownership-map.md`, and `service-boundary-options.md` in a future run of this stage. FEB-009 (service boundary decision) is intentionally sequenced last, consistent with the rule that this stage must not claim final service boundaries.
```

### final/migration-wave-plan.md
```
# Migration Wave Plan (Candidate)

> This plan sequences forward-engineering activities based on **evidence-backed risk and coupling**, not a chosen target architecture. Each wave should be re-validated against the full inventory before execution.

## Wave 0 — Clarify Open Questions (Pre-requisite, no code changes)

- Resolve ARCH-VIOL-002: confirm canonical Admin UI (`src/Web/Pages/Admin` vs `src/BlazorAdmin/Pages/CatalogItemPage`).
- Resolve ARCH-VIOL-001: confirm whether `CatalogItemOrdered` is an intentional snapshot/anti-corruption boundary.
- Obtain full module/component inventory and test/runtime evidence packs (not present in this stage's input context) to validate capability and data-ownership maps.
- **Evidence:** ARCH-VIOL-001, ARCH-VIOL-002 (architecture-violation-register.json)
- **Confidence:** 0.6 — this wave is procedural and low-risk regardless of eventual target architecture.

## Wave 1 — Admin Surface Consolidation

- Once Wave 0 confirms the canonical Admin UI, plan retirement/redesign of the non-canonical surface (`MCC-001`).
- Depends on usage evidence (not yet available) before any retirement — per global rules, nothing is marked retire without usage evidence.
- **Related:** ARCH-VIOL-002, CAP-005, MCC-001
- **Confidence:** 0.5

## Wave 2 — Formalize PublicApi ↔ BlazorAdmin Contract

- Treat `CatalogItemEndpoints` request/response shapes consumed by `CachedCatalogItemServiceDecorator` as a versioned, documented contract (see `api-contract-preservation-map.json`).
- This reduces coupling risk identified in ARCH-VIOL-003 ahead of any service-boundary changes.
- **Related:** ARCH-VIOL-003, CAP-007, CAP-008, MCC-003
- **Confidence:** 0.5

## Wave 3 — Domain Boundary Validation (ApplicationCore)

- Validate aggregate boundaries (`BasketAggregate`, `OrderAggregate`, `BuyerAggregate`, Catalog entities) against actual usage/test evidence (currently `unknown` — see `test-runtime-evidence-map.md`).
- Document the `CatalogItemOrdered` snapshot explicitly as a deliberate boundary artifact if confirmed in Wave 0.
- **Related:** ARCH-VIOL-001, CAP-001, CAP-002, CAP-003, CAP-004, MCC-002
- **Confidence:** 0.45 — depends heavily on evidence not available in this run.

## Wave 4 — Service Boundary Decision (Architect-Led)

- Using outputs of Waves 0–3 plus `service-boundary-options.md` and `data-ownership-map.md`, select a target boundary option (A/B/C or a hybrid).
- This wave is explicitly **not** pre-decided by this stage.
- **Confidence:** unknown — intentionally deferred to architects.

## Sequencing Rationale

Waves are ordered by **evidence dependency**: items with direct, file-level evidence (Admin duplication, API contract coupling) are sequenced before items that require additional evidence (domain boundary validation, service split) that was not available in this stage's input context.
```

### final/module-consolidation-map.json
```
{
  "schema_version": "1.0",
  "stage": "05-enterprise-forward-engineering",
  "generated_from": [
    "final/architecture-pattern-report.md",
    "final/architecture-violation-register.json"
  ],
  "consolidation_candidates": [
    {
      "consolidation_id": "MCC-001",
      "title": "Unify Catalog Administration UI",
      "status": "candidate",
      "description": "src/Web/Pages/Admin (Razor Pages) and src/BlazorAdmin/Pages/CatalogItemPage (Blazor) both implement catalog item administration. Forward engineering should consolidate this into a single Admin module/owner.",
      "current_modules": [
        {
          "name": "Web (Pages/Admin)",
          "module_ref": "MOD-006 (per architecture-violation-register.json affected_module)"
        },
        {
          "name": "BlazorAdmin (Pages/CatalogItemPage)",
          "module_ref": "MOD-006 (per architecture-violation-register.json affected_module)"
        }
      ],
      "evidence": [
        {
          "file": "src/Web/Pages/Admin/EditCatalogItem.cshtml",
          "reason": "Razor Pages admin catalog edit surface (ARCH-VIOL-002)"
        },
        {
          "file": "src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor",
          "reason": "Blazor admin catalog edit surface for the same entity (ARCH-VIOL-002)"
        }
      ],
      "recommendation": "Do not consolidate code until the team confirms which surface (Razor Pages Admin vs BlazorAdmin) is canonical. Treat as a review item, not an automatic merge.",
      "related_violations": ["ARCH-VIOL-002"],
      "confidence": 0.5
    },
    {
      "consolidation_id": "MCC-002",
      "title": "Preserve ApplicationCore as a Single Domain/Application Module",
      "status": "candidate",
      "description": "Basket, Order, Buyer, and Catalog entities, specifications, and application services already live in one cohesive ApplicationCore project. This module is a strong candidate to remain a single forward-engineering unit (or split internally along aggregate lines, see service-boundary-options.md) rather than being merged with Infrastructure or presentation projects.",
      "current_modules": [
        {
          "name": "ApplicationCore",
          "module_ref": "unknown (module ID not present in provided evidence; referenced by project name only)"
        }
      ],
      "evidence": [
        {
          "file": "src/ApplicationCore/Entities/BasketAggregate",
          "reason": "Aggregate root"
        },
        {
          "file": "src/ApplicationCore/Entities/OrderAggregate",
          "reason": "Aggregate root"
        },
        {
          "file": "src/ApplicationCore/Entities/BuyerAggregate",
          "reason": "Aggregate root"
        },
        {
          "file": "src/ApplicationCore/Entities/CatalogItem.cs",
          "reason": "Core catalog entity"
        }
      ],
      "recommendation": "No consolidation action required at this stage; flag as the likely 'core domain' anchor for future service boundary decisions.",
      "related_violations": [],
      "confidence": 0.6
    },
    {
      "consolidation_id": "MCC-003",
      "title": "Review Contract Coupling Between PublicApi CatalogItemEndpoints and BlazorAdmin Catalog Services",
      "status": "candidate",
      "description": "BlazorAdmin's CachedCatalogItemServiceDecorator wraps PublicApi's CatalogItemEndpoints request/response shapes directly. This is not a structural consolidation candidate, but a contract-ownership review candidate: either fo
... [truncated]
```

### final/module-consolidation-map.md
```
# Module Consolidation Map (Candidate)

> Status: **CANDIDATE** — none of these represent committed consolidation decisions. They are inputs for architect review.

## MCC-001: Unify Catalog Administration UI

- **Modules involved:** `Web` (Pages/Admin), `BlazorAdmin` (Pages/CatalogItemPage)
- **Evidence:** `src/Web/Pages/Admin/EditCatalogItem.cshtml`, `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor`
- **Related violation:** ARCH-VIOL-002 (severity: Medium)
- **Recommendation:** Do not merge until the team confirms which UI is canonical (legacy vs actively maintained). Carrying both forward duplicates effort and risks divergent business rules.
- **Confidence:** 0.5

## MCC-002: Preserve ApplicationCore as a Single Domain/Application Module

- **Modules involved:** `ApplicationCore` (Basket/Order/Buyer aggregates, CatalogItem, Specifications, Services)
- **Evidence:** `src/ApplicationCore/Entities/BasketAggregate`, `src/ApplicationCore/Entities/OrderAggregate`, `src/ApplicationCore/Entities/BuyerAggregate`, `src/ApplicationCore/Entities/CatalogItem.cs`
- **Related violation:** none
- **Recommendation:** Keep as a single cohesive unit for now; it is the likely anchor for any future aggregate-aligned service split (see `service-boundary-options.md`).
- **Confidence:** 0.6

## MCC-003: Review Contract Coupling Between PublicApi and BlazorAdmin Catalog Services

- **Modules involved:** `PublicApi` (CatalogItemEndpoints), `BlazorAdmin` (CachedCatalogItemServiceDecorator)
- **Evidence:** `src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs`, `src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs`
- **Related violation:** ARCH-VIOL-003 (severity: Medium)
- **Recommendation:** Not a code-merge candidate — a contract-formalization task. See `api-contract-preservation-map.json`.
- **Confidence:** 0.5

## Out of Scope for This Stage

- No consolidation candidates were identified that involve `Infrastructure`, since no violations or pattern-report evidence directly implicated it beyond its role as a dependency target.
- Module-level identifiers (`MOD-xxx`) for `ApplicationCore`, `Web`, `PublicApi`, and `BlazorAdmin` were not present in the input context provided to this stage (only `MOD-003` and `MOD-006` appear, via the violation register). Cross-reference against `architecture-output/final/module-catalog.json` (not included in this run) to attach full module IDs.
```

### final/preserve-redesign-retire-map.md
```
# Preserve / Redesign / Review / Retire Map (Candidate)

> Per stage rules, decisions are conservative. **Nothing is marked "Retire" without usage evidence**, and no such evidence was available in this stage's input context.

| Item | Decision | Rationale | Evidence | Confidence |
|------|----------|-----------|----------|------------|
| ApplicationCore domain model (Basket/Order/Buyer aggregates, Specifications, IAggregateRoot, IRepository/IReadRepository) | **Preserve** | Cohesive Domain+Application separation; consistent dependency direction (Clean/Onion pattern, confidence 0.7) | `src/ApplicationCore/Entities/*`, `src/ApplicationCore/Interfaces/*`, `src/ApplicationCore/Specifications/*` (architecture-pattern-report.md) | 0.65 |
| `CatalogItemOrdered` snapshot in Order aggregate | **Preserve (pending Wave 0 confirmation)** | Violation register's own migration_impact notes this snapshot is "actually beneficial" if Catalog/Order become separate services — avoids runtime cross-service calls for historical order data | `src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs` (ARCH-VIOL-001) | 0.55 |
| Duplicated Catalog Admin UI (`Web/Pages/Admin` vs `BlazorAdmin/Pages/CatalogItemPage`) | **Review** | Overlapping ownership of Admin responsibility; canonical surface unconfirmed | `src/Web/Pages/Admin/EditCatalogItem.cshtml`, `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor` (ARCH-VIOL-002) | 0.5 |
| PublicApi ↔ BlazorAdmin catalog contract coupling (`CachedCatalogItemServiceDecorator` ↔ `CatalogItemEndpoints`) | **Review** | Contract coupling is functional but undocumented; formalize rather than restructure first | `src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs`, `src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs` (ARCH-VIOL-003) | 0.5 |
| Two-presentation-project structure (`Web` + `PublicApi`, both depending on `ApplicationCore`/`Infrastructure`) | **Preserve** | Defining trait of the current Modular Monolith; no evidence of need to collapse | architecture-pattern-report.md | 0.6 |
| Any component not covered by the two evidence files provided to this stage | **Review (unknown)** | No claim can be made without evidence | n/a | unknown |

## Explicitly NOT Decided in This Stage

- Nothing is marked **Retire**. The violation register provided contains no usage/runtime evidence (e.g., traffic, call counts, deprecation markers) that would justify retirement of either Admin surface or any other component.
- Database/queue ownership decisions are out of scope here — see `data-ownership-map.md`.
```

### final/service-boundary-options.md
```
# Service Boundary Options (Candidate)

> **These are options for discussion, not final service boundaries.** Per stage rules, no future technology stack or final boundary is asserted here.

## Context

The current system is a Modular Monolith (confidence 0.7, per `architecture-pattern-report.md`):
- `ApplicationCore` — shared domain/application layer (Basket, Order, Buyer, Catalog aggregates + specifications)
- `Infrastructure` — implementation of repositories etc., depended on by presentation/API projects
- `Web` — Razor Pages/MVC storefront + admin pages
- `PublicApi` — Ardalis.ApiEndpoints minimal API (consumed by BlazorAdmin)
- `BlazorAdmin` — Blazor WASM/Server admin app, calls `PublicApi`

There is **no evidence** of independent data stores per module, message brokers, or service-to-service network contracts between `Web` and `PublicApi` (per `architecture-pattern-report.md`). Any service boundary option below that assumes independent databases or async messaging is **speculative** and flagged accordingly.

---

## Option A — Status Quo / Strangler-Friendly (Modular Monolith Retained)

Keep `ApplicationCore` + `Infrastructure` as a single deployable "Core Commerce" module. `Web`, `PublicApi`, and `BlazorAdmin` remain separate front-ends/deployables as they are today.

- **Pros:** Lowest risk; matches current dependency direction (pattern-report evidence); no data-ownership questions to resolve immediately.
- **Cons:** Does not resolve ARCH-VIOL-002 (duplicated Admin UI) or ARCH-VIOL-003 (contract coupling) — these would need to be addressed within the monolith regardless.
- **Evidence:** `architecture-pattern-report.md` (Modular Monolith determination, confidence 0.7).
- **Confidence:** 0.6 (as a *viable starting point*, not as a final decision).

## Option B — Aggregate-Aligned Service Candidates

Split `ApplicationCore` along its existing aggregate boundaries into candidate services:
- **Catalog Service** — `CatalogItem`, Catalog specifications, `PublicApi/CatalogItemEndpoints`
- **Ordering Service** — `OrderAggregate`, `BasketAggregate`, `BuyerAggregate`, order/basket specifications
- **Admin/BFF** — consolidated admin surface (pending resolution of ARCH-VIOL-002)

- **Pros:** Aligns with existing DDD aggregate boundaries already visible in code (Basket/Buyer/Order as `IAggregateRoot`).
- **Cons:** The `CatalogItemOrdered` snapshot (ARCH-VIOL-001) creates a direct data dependency between Catalog and Ordering — per the violation register's own migration_impact note, this snapshot is "actually beneficial" if these become separate services and should be **preserved as an anti-corruption pattern**, not eliminated.
- **Open question:** No evidence of per-aggregate database ownership was provided — confirm via `data-ownership-map.md` and the (not-yet-seen) full inventory before treating this as feasible.
- **Confidence:** 0.45 (directional only; database/transaction-boundary evidence is missing).

## Option C — Front-End Decoupling Only

Leave the domain/data layer (`ApplicationCore` + `Infrastructure`) as one deployable, but formally separate the front-ends:
- Resolve ARCH-VIOL-002 by choosing one canonical Admin UI (`Web/Pages/Admin` or `BlazorAdmin`).
- Formalize the `PublicApi` ↔ `BlazorAdmin` contract (ARCH-VIOL-003) as a versioned API contract rather than an internal coupling.

- **Pros:** Addresses both Medium-severity violations without requiring a domain-model split or database ownership decisions.
- **Cons:** Does not redu
... [truncated]
```

### final/test-runtime-evidence-map.json
```
{
  "schema_version": "1.0",
  "stage": "05-enterprise-forward-engineering",
  "generated_from": [
    "final/architecture-pattern-report.md",
    "final/architecture-violation-register.json"
  ],
  "status": "no_test_or_runtime_evidence_in_input_context",
  "test_evidence": [],
  "runtime_evidence": [],
  "coverage_by_capability": [
    {
      "capability_id": "CAP-001",
      "name": "Catalog Management",
      "test_evidence_found": false,
      "runtime_evidence_found": false,
      "confidence": "unknown"
    },
    {
      "capability_id": "CAP-002",
      "name": "Shopping Basket Management",
      "test_evidence_found": false,
      "runtime_evidence_found": false,
      "confidence": "unknown"
    },
    {
      "capability_id": "CAP-003",
      "name": "Order Management",
      "test_evidence_found": false,
      "runtime_evidence_found": false,
      "confidence": "unknown"
    },
    {
      "capability_id": "CAP-004",
      "name": "Buyer / Customer Profile Management",
      "test_evidence_found": false,
      "runtime_evidence_found": false,
      "confidence": "unknown"
    },
    {
      "capability_id": "CAP-005",
      "name": "Catalog Administration",
      "test_evidence_found": false,
      "runtime_evidence_found": false,
      "confidence": "unknown"
    },
    {
      "capability_id": "CAP-006",
      "name": "Storefront Web Experience",
      "test_evidence_found": false,
      "runtime_evidence_found": false,
      "confidence": "unknown"
    },
    {
      "capability_id": "CAP-007",
      "name": "Public API / Integration Layer",
      "test_evidence_found": false,
      "runtime_evidence_found": false,
      "confidence": "unknown"
    },
    {
      "capability_id": "CAP-008",
      "name": "Admin Application Experience",
      "test_evidence_found": false,
      "runtime_evidence_found": false,
      "confidence": "unknown"
    }
  ],
  "notes": "No test inventory, test result, or runtime/telemetry evidence packs were included in this stage's input context (only architecture-pattern-report.md and architecture-violation-register.json were provided). All retire/redesign decisions in preserve-redesign-retire-map.md were therefore made without usage evidence, per global rules. This file should be regenerated once architecture-output/final/test-evidence-pack.json (or equivalent) is available."
}
```

### final/test-runtime-evidence-map.md
```
# Test / Runtime Evidence Map

## Status: No Evidence Available in This Stage's Input

This stage's input context contained only:
- `final/architecture-pattern-report.md`
- `final/architecture-violation-register.json`

Neither file contains test inventory, test execution results, code coverage, or runtime/telemetry (logs, traces, usage metrics) information.

## Coverage by Candidate Capability

| Capability | Test Evidence | Runtime Evidence | Confidence |
|------------|---------------|-------------------|------------|
| CAP-001 Catalog Management | unknown | unknown | unknown |
| CAP-002 Shopping Basket Management | unknown | unknown | unknown |
| CAP-003 Order Management | unknown | unknown | unknown |
| CAP-004 Buyer / Customer Profile Management | unknown | unknown | unknown |
| CAP-005 Catalog Administration | unknown | unknown | unknown |
| CAP-006 Storefront Web Experience | unknown | unknown | unknown |
| CAP-007 Public API / Integration Layer | unknown | unknown | unknown |
| CAP-008 Admin Application Experience | unknown | unknown | unknown |

## Impact on This Stage's Other Outputs

- **`preserve-redesign-retire-map.md`**: No items are marked "Retire" — this is a direct consequence of the absence of usage evidence here, consistent with global rule "Do not mark anything retire without usage evidence."
- **`migration-wave-plan.md`**: Wave 3 (Domain Boundary Validation) explicitly depends on this evidence being supplied.
- **`confidence-report.md`**: Reflects reduced confidence across all capability/consolidation candidates due to this gap.

## Recommended Follow-up

Obtain and re-run this stage with:
- `architecture-output/final/test-evidence-pack.json` (or equivalent test inventory output from earlier stages)
- Any available runtime/telemetry evidence (APM traces, access logs, feature-usage analytics)
```


---

## Reminder on output format
Please output all files from this stage's Output section, each wrapped in
===AA_FILE_START:<path>=== / ===AA_FILE_END=== markers. I'll be parsing your
response for these exact markers, so please give full file contents rather
than descriptions.
