I'm working through an application architecture analysis in stages, and this is
stage "05-enterprise-forward-engineering". Earlier stages (inventory, parser, evidence packs) were
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


--- STAGE PROMPT: 05-enterprise-forward-engineering-agent.md ---
# 05 - Enterprise Forward Engineering Agent

## Role

Convert final architecture outputs into enterprise forward-engineering planning inputs.

## Input

```text
architecture-output/final/
```

## Output

```text
architecture-output/final/business-capability-map.json
architecture-output/final/business-capability-map.md
architecture-output/final/module-consolidation-map.json
architecture-output/final/module-consolidation-map.md
architecture-output/final/service-boundary-options.md
architecture-output/final/migration-wave-plan.md
architecture-output/final/preserve-redesign-retire-map.md
architecture-output/final/api-contract-preservation-map.json
architecture-output/final/data-ownership-map.md
architecture-output/final/test-runtime-evidence-map.json
architecture-output/final/test-runtime-evidence-map.md
architecture-output/final/confidence-report.md
architecture-output/final/architecture-decision-inputs.md
architecture-output/final/forward-engineering-backlog.md
```

## Rules

- Do not choose a future technology stack.
- Do not claim final service boundaries.
- Treat capabilities as candidates until reviewed.
- Use preserve/redesign/review/retire decisions conservatively.
- Do not mark anything retire without usage evidence.

## Must Produce

- candidate business/application capabilities
- consolidated module candidates
- service boundary options
- migration waves
- API contract preservation map
- data ownership review
- test/runtime evidence map
- confidence report
- decision inputs for architects
- forward engineering backlog

## Quality Gate

Stop if final architecture JSON is invalid.


--- INPUT CONTEXT ---
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


---

## Reminder on output format
Please output all files from this stage's Output section, each wrapped in
===AA_FILE_START:<path>=== / ===AA_FILE_END=== markers. I'll be parsing your
response for these exact markers, so please give full file contents rather
than descriptions.
