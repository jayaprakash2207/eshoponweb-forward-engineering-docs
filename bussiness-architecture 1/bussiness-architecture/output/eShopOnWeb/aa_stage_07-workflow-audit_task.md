I'm working through an application architecture analysis in stages, and this is
stage "07-workflow-audit". Earlier stages (inventory, parser, evidence packs) were
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


--- STAGE PROMPT: 07-workflow-audit-agent.md ---
# 07 - Workflow Audit Agent

## Role

Audit the workflow itself for enterprise readiness and reuse.

## Input

```text
AGENTS.md
architecture-prompts/
tools/application_architecture_analyzer/
architecture-output/
```

## Output

```text
architecture-output/final/architecture-workflow-audit.md
architecture-output/final/missing-output-fixes.md
architecture-output/final/fix-implementation-summary.md when fixes are made
```

## Check

- stage completeness
- stage input/output contracts
- source modification guard
- schema validation
- run history
- graph normalization
- quality gates
- no repo-specific hardcoding
- parser breadth and extension points
- hallucination handling
- forward-engineering usefulness

## Verdict

Use one:

```text
ENTERPRISE READY
MOSTLY READY WITH MINOR FIXES
NOT READY
```

Include score out of 100 and specific fixes.


--- INPUT CONTEXT ---
### AGENTS.md
```
# AGENTS.md - Application Architecture Extraction Orchestrator

## Purpose

This repository uses a staged Application Architecture extraction workflow for SDLC reverse engineering and forward engineering.

Do not place every instruction in this file. This file is only the lightweight orchestrator. Stage-specific rules live under:

```text
architecture-prompts/
```

## Golden Rules

- Do not modify legacy application source code.
- Do not invent architecture facts.
- Use `unknown` when evidence is missing.
- Every major claim must have source evidence.
- Keep generated outputs under `architecture-output/`.
- Keep analyzer tooling under `tools/application_architecture_analyzer/`.
- Parser/structured extraction comes before architecture reasoning.

## Stage Order

Run the workflow in this order:

```text
1. Inventory
2. Source chunking
3. Parser / symbol extraction
4. Semantic extraction where supported
5. Evidence packs
6. Final architecture
7. Enterprise forward engineering
8. Enterprise application architecture blueprint
9. Quality review
10. Workflow audit when requested
```

Use the stage prompts:

```text
architecture-prompts/00-global-rules.md
architecture-prompts/01-inventory-agent.md
architecture-prompts/02-parser-symbol-agent.md
architecture-prompts/03-evidence-pack-agent.md
architecture-prompts/04-final-architecture-agent.md
architecture-prompts/05-enterprise-forward-engineering-agent.md
architecture-prompts/06-quality-review-agent.md
architecture-prompts/07-workflow-audit-agent.md
```

## One-Command Workflow

From the repository root:

```powershell
python tools/application_architecture_analyzer/run_architecture_extraction.py --repo-root . --output-root architecture-output
```

## Required Discipline

Each stage must use only the approved input from the previous stage:

```text
Inventory reads repo source.
Source chunking reads inventory plus relevant source files.
Parser reads inventory, source chunks, and relevant source files.
Semantic extraction reads inventory plus relevant source files when a supported compiler backend is available.
Evidence reads inventory and parsed outputs.
Final architecture reads evidence packs.
Enterprise forward engineering reads final architecture outputs.
Enterprise application architecture blueprint reads final and forward-engineering outputs.
Quality review reads generated outputs.
```

Do not jump directly from raw repo files to final architecture conclusions.

## Current Acceptance Standard

The output is acceptable only when it is:

- source-backed
- machine-readable
- human-readable
- diagrammed
- explicit about unknowns
- useful for migration and forward engineering
- validated by quality review

```

### architecture-prompts/00-global-rules.md
```
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

```

### architecture-prompts/01-inventory-agent.md
```
# 01 - Inventory Agent

## Role

Scan the repository and create a factual inventory. Do not infer architecture.

## Input

```text
repo root
```

## Read

Repository files only, excluding ignored folders/files from `00-global-rules.md`.

## Output

```text
architecture-output/inventory/file-inventory.json
architecture-output/inventory/project-inventory.json
architecture-output/inventory/language-summary.json
architecture-output/inventory/ignored-files-report.json
```

## Extract

- files
- extensions
- languages
- line counts
- file hashes
- candidate file categories
- project files
- solution/build/deployment clues
- backend/frontend/library/test/database/support candidates
- deployable candidates
- framework indicators

## Do Not

- Do not classify modules.
- Do not classify components.
- Do not create architecture summaries.
- Do not create migration recommendations.

## Quality Gate

Stop if required inventory JSON files are missing, empty, or invalid.

```

### architecture-prompts/02-parser-symbol-agent.md
```
# 02 - Parser / Symbol Extraction Agent

## Role

Extract structured facts from source files selected by inventory. This stage produces facts, not final architecture.

## Input

```text
architecture-output/inventory/
```

## Output

```text
architecture-output/parsed/symbol-registry.json
architecture-output/parsed/route-registry.json
architecture-output/parsed/dependency-candidates.json
architecture-output/parsed/entry-point-candidates.json
```

## Extraction Priorities

Use language/framework-specific parsing where available, then conservative regex/path heuristics as fallback.

Extract:

- classes/types/components
- methods/functions
- imports/usings/requires
- constructor/property injection
- DI registrations
- route declarations
- frontend routes
- API calls
- component calls
- repositories/data access
- scheduled jobs, consumers, batch jobs, CLI/bootstrap entries

## Confidence Rules

High confidence:

- explicit class/type declaration
- explicit route attribute or framework route call
- explicit constructor injection
- explicit project reference

Medium confidence:

- path/name/framework heuristic
- method-call candidate without full type resolution

Low confidence:

- ambiguous dynamic expression
- unknown module ownership
- unresolved external target

## Do Not

- Do not generate final module maps.
- Do not write architecture pattern conclusions.
- Do not produce migration plans.

## Quality Gate

Stop if inventory inputs are invalid. Every extracted item must include source file and confidence.

```

### architecture-prompts/03-evidence-pack-agent.md
```
# 03 - Evidence Pack Agent

## Role

Convert inventory and parsed facts into technology-agnostic evidence packs.

## Input

```text
architecture-output/inventory/
architecture-output/parsed/
```

## Output

```text
architecture-output/evidence-packs/system-inventory-pack.json
architecture-output/evidence-packs/module-boundary-pack.json
architecture-output/evidence-packs/component-registry-pack.json
architecture-output/evidence-packs/dependency-pack.json
architecture-output/evidence-packs/entry-point-pack.json
architecture-output/evidence-packs/call-flow-pack.json
architecture-output/evidence-packs/layering-pattern-pack.json
architecture-output/evidence-packs/external-boundary-pack.json
architecture-output/evidence-packs/frontend-application-pack.json
```

## Rules

- Evidence packs are not final architecture.
- Preserve source evidence.
- Preserve confidence scores.
- Use `unknown` where evidence is insufficient.
- Mark partial call flows as partial.
- Do not invent module responsibilities.

## Evidence Pack Purpose

- System inventory: applications, projects, deployables, support/test/database/frontend/backend.
- Module boundary: candidate modules from folders, namespaces, routes, components, dependencies.
- Component registry: components grouped by type/layer/module.
- Dependency: component/module/project/layer dependency candidates.
- Entry point: APIs/routes/jobs/consumers/CLI.
- Call flow: evidence-backed flows only.
- Layering pattern: layer signals, candidate patterns, violations.
- External boundary: external dependency candidates.
- Frontend application: frontend apps/routes/components/API calls.

## Quality Gate

Stop if parsed outputs are invalid or missing required keys.

```

### architecture-prompts/04-final-architecture-agent.md
```
# 04 - Final Architecture Agent

## Role

Produce the final Application Architecture package from evidence packs only.

## Input

```text
architecture-output/evidence-packs/
```

## Output

```text
architecture-output/final/application-architecture-summary.md
architecture-output/final/system-inventory.json
architecture-output/final/module-boundary-map.json
architecture-output/final/component-registry.json
architecture-output/final/dependency-graph.json
architecture-output/final/application-interface-catalogue.json
architecture-output/final/call-flow-map.json
architecture-output/final/architecture-pattern-report.md
architecture-output/final/architecture-violation-register.json
architecture-output/final/application-risk-register.json
architecture-output/final/strangler-candidate-report.md
architecture-output/final/forward-engineering-input-map.md
architecture-output/final/open-questions.md
architecture-output/final/diagrams/*.mmd
```

## Rules

- Use only evidence packs.
- Do not rescan the full repo.
- Do not invent module ownership or call flows.
- Every major architecture claim needs evidence.
- Risks must include affected module/component and evidence.
- Unknowns must go to open questions.

## Must Answer

- What applications/projects exist?
- What deployable units exist?
- What modules and layers exist?
- What components and interfaces exist?
- What depends on what?
- What call flows are known or partial?
- What architecture pattern is supported by evidence?
- What violations and risks affect migration?
- Which candidates are safer or riskier for modernization?

## Quality Gate

Stop if evidence packs are invalid. Final JSON must be valid and graph edges must resolve to nodes.

```

### architecture-prompts/05-enterprise-forward-engineering-agent.md
```
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

```

### architecture-prompts/06-quality-review-agent.md
```
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

```

### architecture-prompts/07-workflow-audit-agent.md
```
# 07 - Workflow Audit Agent

## Role

Audit the workflow itself for enterprise readiness and reuse.

## Input

```text
AGENTS.md
architecture-prompts/
tools/application_architecture_analyzer/
architecture-output/
```

## Output

```text
architecture-output/final/architecture-workflow-audit.md
architecture-output/final/missing-output-fixes.md
architecture-output/final/fix-implementation-summary.md when fixes are made
```

## Check

- stage completeness
- stage input/output contracts
- source modification guard
- schema validation
- run history
- graph normalization
- quality gates
- no repo-specific hardcoding
- parser breadth and extension points
- hallucination handling
- forward-engineering usefulness

## Verdict

Use one:

```text
ENTERPRISE READY
MOSTLY READY WITH MINOR FIXES
NOT READY
```

Include score out of 100 and specific fixes.

```

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

### final/executive-summary-for-review.md
```
# Executive Summary for Review — Application Architecture (Stage 06)

## Purpose

This summary is intended for architects and stakeholders deciding how to act on the reverse-engineered architecture analysis. It synthesizes the stage-05 (Enterprise Forward Engineering) outputs and the stage-06 quality review (`final/quality-review.md`).

## What This Analysis Covers

The system under review is a **.NET reference application** structured as a **Modular Monolith with Clean/Onion Architecture** (confidence **0.7**):

- `ApplicationCore` — shared domain/application layer (Basket, Order, Buyer, Catalog aggregates and specifications)
- `Infrastructure` — repository implementations etc.
- `Web` — Razor Pages/MVC storefront and admin pages
- `PublicApi` — Ardalis.ApiEndpoints minimal API
- `BlazorAdmin` — Blazor admin app, consuming `PublicApi`

No evidence of independent per-module databases, message brokers, or service-to-service network contracts was found — this rules out treating the system as a microservices architecture today.

## Key Findings Requiring Decisions

Three architectural issues were confirmed with direct file-level evidence:

1. **Duplicated Catalog Admin UI (ARCH-VIOL-002, Medium severity).** Both `src/Web/Pages/Admin/EditCatalogItem.cshtml` (Razor Pages) and `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor` (Blazor) implement catalog item administration. **Decision needed (AD-001):** which surface is canonical?

2. **PublicApi ↔ BlazorAdmin contract coupling (ARCH-VIOL-003, Medium severity).** `BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs` is coupled to `PublicApi/CatalogItemEndpoints` request/response shapes with no formal contract. **Decision needed (AD-003):** should this be formalized as a versioned API contract?

3. **Order/Catalog data snapshot (ARCH-VIOL-001, Low severity).** The `CatalogItemOrdered` value object embeds a denormalized Catalog snapshot inside the Order aggregate. The analysis suggests this is **likely beneficial** for any future service split (avoids cross-service calls for historical order data), but it is unclear whether it was designed deliberately. **Decision needed (AD-002):** confirm intent and document as a bounded-context boundary if confirmed.

## What Cannot Yet Be Decided

Three architectural questions are explicitly **not resolved** and should not be treated as decided:

- **Target service-boundary option (AD-004).** Three directional options were drafted — Option A (status quo), Option B (aggregate-aligned services), Option C (front-end decoupling only) — but none is recommended. Selecting one requires database/persistence evidence that was not available.
- **Scope of non-evidenced capabilities (AD-005).** Identity/Authentication, Payment, Shipping/Notification, etc. were not referenced in the available evidence. **This does not mean they don't exist** — it means the evidence packs reviewed in this run did not cover them. Re-run capability extraction once the full component inventory is available.
- **Database/schema ownership per aggregate (AD-006).** No infrastructure/persistence evidence (DbContext mappings, connection strings, schema files) was available, so it is `unknown` whether the system uses one shared database or per-module schemas. This directly blocks evaluating Option B.

## Quality of This Analysis

The stage-06 quality review (`final/quality-review.md`) found:

- **No invented modules, flows, deployment topology, cloud platform, or business rules** anywhere i
... [truncated]
```

### final/final-sanity-check.md
```
# Final Sanity Check — Stage 06

## Purpose

This is the last gate before the stage-05 outputs (and the stage-04 inputs they depend on) are considered ready for architect consumption. It checks for the failure modes the global rules specifically prohibit, independent of the detailed per-check results in `final/quality-review.md`.

## Prohibited-Content Checks

| Prohibition (Global Rules) | Result | Notes |
|---|---|---|
| Invented modules | PASS | All modules referenced (`ApplicationCore`, `Infrastructure`, `Web`, `PublicApi`, `BlazorAdmin`) are tied to real project paths cited in `architecture-pattern-report.md`. Where a module ID (`MOD-xxx`) is used, it is sourced from `architecture-violation-register.json` and never asserted as new. |
| Invented flows | PASS | No data-flow or call-flow diagrams were produced or asserted in this evidence set. None of the reviewed files claim a runtime flow that isn't directly tied to a cited file. |
| Invented deployment topology | PASS | `architecture-pattern-report.md` and `service-boundary-options.md` explicitly state there is no evidence of independent deployables beyond the three known front-ends (`Web`, `PublicApi`, `BlazorAdmin`) sharing `ApplicationCore`/`Infrastructure`. |
| Invented cloud platform | PASS | No cloud provider, hosting service, or managed infrastructure is named anywhere. |
| Invented database ownership | PASS | `data-ownership-map.md` explicitly marks physical database/schema ownership as `unknown` rather than assuming shared or per-module databases. |
| Invented queue ownership | PASS | No message broker or queue is referenced anywhere in the reviewed set; `architecture-pattern-report.md` explicitly notes the absence of evidence for message brokers. |
| Invented business rules | PASS | All capability descriptions (`business-capability-map.json/.md`) are framed as "candidate" and tied to entity/service file paths; no business rule is stated as fact beyond what the cited code structure implies. |
| Items marked "Retire" without usage evidence | PASS | `preserve-redesign-retire-map.md` marks zero items as "Retire" and explicitly documents why (no usage/runtime evidence available), consistent with `test-runtime-evidence-map.json/.md` (`status: no_test_or_runtime_evidence_in_input_context`). |

**No violations of the prohibited-content rules were found.** This is the most important outcome of this sanity check: the analysis is conservative and evidence-bound throughout.

## Evidence & Confidence Discipline

- Every reviewed claim carries a `confidence` score (range observed: 0.4–0.7).
- Every reviewed claim cites at least one file path as evidence.
- `confidence-report.md` itself transparently documents the input limitations of stage 05 (only 2 of the expected `final/` files were available) and propagates that limitation into every downstream confidence score rather than hiding it.

This pattern — propagating a known input gap into explicit, lowered confidence scores rather than papering over it — is the correct behavior and should be preserved in future stage runs.

## Carried-Forward Gaps (Blockers for Next Run)

These gaps were already identified within the reviewed files themselves (primarily `confidence-report.md` and `architecture-decision-inputs.md`) and are restated here as the consolidated blocker list for the next iteration of this pipeline:

1. **`system-inventory.json`, `module-catalog.json`, `component-catalog.json` not available** to either stage 05 or this review. 
... [truncated]
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

### final/quality-review.md
```
# Quality Review — Architecture Output (`architecture-output/final/`)

## Review Scope and Input Limitation

This review evaluates the 14 stage-05 (Enterprise Forward Engineering) output files plus the 2 stage-04 artifacts (`architecture-pattern-report.md`, `architecture-violation-register.json`) that were provided as input context to this stage.

**Important constraint:** This review's own input context did **not** include the full `architecture-output/final/` directory. In particular, the following commonly-expected files were **not provided** to this review and therefore cannot be checked directly:

- `system-inventory.json`
- `module-catalog.json`
- `component-catalog.json`
- `data-flow-diagram` / call-flow documents
- diagram artifacts (Mermaid/PlantUML/etc.)
- `open-questions.md`
- any "final architecture" narrative/JSON produced by an earlier stage

Where a check depends on one of these files, the result is marked `PARTIAL` or `unknown`, not `FAIL` — absence of evidence in this review's context is not proof of absence on disk, consistent with the global rule "if evidence is missing, write `unknown`."

---

## Checklist Results

### 1. Required files exist — **PARTIAL**

Files reviewed and confirmed present (in this context):

- `final/api-contract-preservation-map.json`
- `final/architecture-decision-inputs.md`
- `final/architecture-pattern-report.md`
- `final/architecture-violation-register.json`
- `final/business-capability-map.json`
- `final/business-capability-map.md`
- `final/confidence-report.md`
- `final/data-ownership-map.md`
- `final/forward-engineering-backlog.md`
- `final/migration-wave-plan.md`
- `final/module-consolidation-map.json`
- `final/module-consolidation-map.md`
- `final/preserve-redesign-retire-map.md`
- `final/service-boundary-options.md`
- `final/test-runtime-evidence-map.json`
- `final/test-runtime-evidence-map.md`

All 14 stage-05 outputs that this stage's prompt declares it should produce are present. The 2 stage-04 inputs consumed by stage 05 are also present.

**Gap:** Files referenced by stage 05 as "not provided to this stage" — `system-inventory.json`, `module-catalog.json`, `component-catalog.json`, and any test/runtime evidence pack — could not be checked for existence in this review's context. This is flagged as an open question (see `final/architecture-decision-inputs.md`, AD-005/AD-006/AD-007 equivalents and FEB-005 through FEB-008) and is the dominant limiting factor across this entire review.

### 2. JSON is valid — **PARTIAL**

Visible JSON content in all five `.json` files (`api-contract-preservation-map.json`, `architecture-violation-register.json`, `business-capability-map.json`, `module-consolidation-map.json`, `test-runtime-evidence-map.json`) is well-formed where shown.

- `api-contract-preservation-map.json` and `test-runtime-evidence-map.json` were shown in full and are structurally valid (consistent `schema_version`, `stage`, `generated_from`, top-level arrays of well-formed objects).
- `architecture-violation-register.json`, `business-capability-map.json`, and `module-consolidation-map.json` were shown **truncated** ("... [truncated]") in this review's input context. The visible portions are valid JSON fragments with correctly matched braces/brackets up to the truncation point, but full-file validation (e.g., confirming the closing braces and that no trailing content is malformed) **cannot be completed** from this context.

**Recommendation:** Run a JSON schema/syntax validator (e
... [truncated]
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

### final/api-contract-preservation-map.json
```
{
  "generated_at": "2026-06-15T07:27:16+00:00",
  "source_final_artifacts": [
    "architecture-output/final/application-interface-catalogue.json",
    "architecture-output/final/component-registry.json",
    "architecture-output/final/module-boundary-map.json",
    "architecture-output/final/business-capability-map.json"
  ],
  "summary": {
    "contract_count": 55,
    "preserve_count": 46,
    "review_count": 9
  },
  "api_contracts": [
    {
      "contract_id": "API-CONTRACT-001",
      "interface_id": "INT-001",
      "type": "HTTP_API",
      "method": "POST",
      "path_or_name": "/api/authenticate",
      "owner_module_id": "MOD-007",
      "owner_module": "Identity",
      "owner_capability_id": "CAP-002",
      "owner_capability": "Identity",
      "entry_component_id": "COMP-0221",
      "entry_component": "AuthenticateEndpoint",
      "called_service": [
        "ITokenClaimsService.GetTokenAsync",
        "SignInManager<ApplicationUser>.PasswordSignInAsync"
      ],
      "source_file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
      "line": 36,
      "preserve_redesign_review": "preserve",
      "forward_engineering_note": "Preserve behavior contract or provide an explicit compatibility plan.",
      "confidence": 0.9,
      "evidence": [
        {
          "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
          "line": 36,
          "reason": "ASP.NET HTTP method attribute"
        }
      ],
      "open_questions": []
    },
    {
      "contract_id": "API-CONTRACT-002",
      "interface_id": "INT-002",
      "type": "HTTP_API",
      "method": "GET",
      "path_or_name": "/api/catalog-brands",
      "owner_module_id": "MOD-004",
      "owner_module": "Catalog",
      "owner_capability_id": "CAP-001",
      "owner_capability": "Catalog",
      "entry_component_id": "COMP-0118",
      "entry_component": "CatalogBrandListEndpoint",
      "called_service": [
        "IRepository<CatalogBrand>.ListAsync"
      ],
      "source_file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
      "line": 27,
      "preserve_redesign_review": "preserve",
      "forward_engineering_note": "Preserve behavior contract or provide an explicit compatibility plan.",
      "confidence": 0.9,
      "evidence": [
        {
          "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
          "line": 27,
          "reason": "ASP.NET Minimal API Map* call"
        }
      ],
      "open_questions": []
    },
    {
      "contract_id": "API-CONTRACT-003",
      "interface_id": "INT-003",
      "type": "HTTP_API",
      "method": "GET",
      "path_or_name": "/api/catalog-items/{catalogItemId}",
      "owner_module_id": "MOD-004",
      "owner_module": "Catalog",
      "owner_capability_id": "CAP-001",
      "owner_capability": "Catalog",
      "entry_component_id": "COMP-0122",
      "entry_component": "CatalogItemGetByIdEndpoint",
      "called_service": [
        "GetByIdCatalogItemRequest.CorrelationId",
        "IRepository<CatalogItem>.GetByIdAsync",
        "IUriComposer.ComposePicUri"
      ],
      "source_file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
      "line": 25,
      "preserve_redesign_review": "preserve",
      "forward_engineering_note": "Preserve behavior contract or provide an explicit compatibility plan.",
      "confidence": 0.9,
      "evidence": [
        {
          "file": "src/PublicApi/CatalogItemEndpoints/CatalogI
... [truncated]
```

### final/application-architecture-summary.md
```
# Application Architecture Summary

## 1. System Overview

The repository contains 6 detected application/support project records and 2 deployable unit candidates. The authoritative system name is unknown in the evidence, so final artifacts keep `system_name` as `unknown`.

Evidence: `system-inventory-pack.json` detects 6 projects and deployable units PublicApi, Web.

Source anchors: src/PublicApi/PublicApi.csproj, src/Web/Web.csproj, src/BlazorAdmin/BlazorAdmin.csproj, src/ApplicationCore/ApplicationCore.csproj, src/BlazorShared/BlazorShared.csproj, src/Infrastructure/Infrastructure.csproj.

## 2. Detected Application Style

Detected style: Layered Monolith. Secondary candidates: Clean Architecture, Modular Monolith. The pattern statement is based on project/deployable evidence and detected component layers.

## 3. Deployable Units

Deployable units detected: PublicApi, Web. Deployment/build clues are retained in `system-inventory.json` and `system-inventory-pack.json`.

## 4. Main Modules

Module candidates are evidence-derived, not final business-owned bounded contexts. Largest candidates by component count:

- Catalog: 25 components, 9 entry points, boundary Weak
- Identity: 25 components, 29 entry points, boundary Weak
- Verification: 25 components, 0 entry points, boundary Medium
- Admin: 23 components, 2 entry points, boundary Weak
- Basket: 23 components, 3 entry points, boundary Weak
- Order: 21 components, 2 entry points, boundary Weak
- Web: 21 components, 3 entry points, boundary Weak
- ApplicationCore: 13 components, 0 entry points, boundary Weak

Representative module evidence: Catalog uses src/ApplicationCore/CatalogSettings.cs, Identity uses src/ApplicationCore/Constants/AuthorizationConstants.cs, Verification uses tests/FunctionalTests/PublicApi/ApiTestFixture.cs, Admin uses src/BlazorAdmin/App.razor, Basket uses src/ApplicationCore/Entities/BasketAggregate/Basket.cs.

## 5. Main Layers

Detected layer counts: {"API": 17, "Application": 69, "CrossCutting": 62, "DataAccess": 15, "Domain": 18, "Infrastructure": 9, "Integration": 2, "Presentation/UI": 115, "Unknown": 3}.

## 6. Main Interfaces

Entry points detected: {"CLI": 3, "FrontendRoute": 3, "HTTP_API": 49}. Representative HTTP APIs: POST /api/authenticate, GET /api/catalog-brands, GET /api/catalog-items/{catalogItemId}, GET /api/catalog-items, POST /api/catalog-items, DELETE /api/catalog-items/{catalogItemId}, PUT /api/catalog-items, GET /api/catalog-types. Representative frontend routes: /index.html, /logout, /admin.

Representative interface evidence is retained in `application-interface-catalogue.json` and `entry-point-pack.json`.

## 7. Major Dependencies

Dependency evidence contains 534 graph edges. High-coupling modules include Catalog, Basket, Identity, Web, ApplicationCore, DataAccess. High-coupling components include EfRepository, UriComposer.

## 8. Architecture Pattern

Primary pattern: Layered Monolith with confidence 0.78. Service separation is not claimed unless deployable and dependency evidence supports it.

Pattern evidence anchors: src/PublicApi/PublicApi.csproj, src/Web/Web.csproj, src/BlazorAdmin/BlazorAdmin.csproj, src/ApplicationCore/ApplicationCore.csproj, src/BlazorShared/BlazorShared.csproj, src/Infrastructure/Infrastructure.csproj.

## 9. Architecture Risks

Top risks:

- APP-RISK-001: Module candidate Catalog has weak or uncertain boundary evidence with coupling score 13.
- APP-RISK-002: Module dependency cycle detected: Admin -> Applicat
... [truncated]
```

### final/application-interface-catalogue.json
```
{
  "generated_at": "2026-06-15T07:27:16+00:00",
  "source_evidence_pack": "architecture-output/evidence-packs/entry-point-pack.json",
  "summary": {
    "entry_point_count": 55,
    "entry_points_by_type": {
      "CLI": 3,
      "FrontendRoute": 3,
      "HTTP_API": 49
    },
    "graphql_endpoint_count": 0,
    "soap_endpoint_count": 0,
    "scheduled_job_count": 0,
    "message_consumer_count": 0,
    "batch_job_count": 0
  },
  "interfaces": [
    {
      "interface_id": "INT-001",
      "type": "HTTP_API",
      "method": "POST",
      "path_or_name": "/api/authenticate",
      "owner_module": "Identity",
      "entry_component": "AuthenticateEndpoint",
      "called_service": [
        "ITokenClaimsService.GetTokenAsync",
        "SignInManager<ApplicationUser>.PasswordSignInAsync"
      ],
      "visibility": "external_system",
      "source_file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
      "line": 36,
      "source_chunks": [
        {
          "chunk_id": "CHUNK-000227",
          "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
          "start_line": 1,
          "end_line": 59,
          "chunk_sha256": "2ae1af5dc8257d055f73705470add93b9cc85ae42292769be6198f46c36d125d"
        }
      ],
      "parser_strategy": "aspnet_attribute_route_parser",
      "route_action": "HandleAsync",
      "confidence": 0.9,
      "evidence": [
        {
          "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
          "line": 36,
          "reason": "ASP.NET HTTP method attribute"
        }
      ],
      "open_questions": []
    },
    {
      "interface_id": "INT-002",
      "type": "HTTP_API",
      "method": "GET",
      "path_or_name": "/api/catalog-brands",
      "owner_module": "Catalog",
      "entry_component": "CatalogBrandListEndpoint",
      "called_service": [
        "IRepository<CatalogBrand>.ListAsync"
      ],
      "visibility": "external_system",
      "source_file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
      "line": 27,
      "source_chunks": [
        {
          "chunk_id": "CHUNK-000230",
          "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
          "start_line": 1,
          "end_line": 46,
          "chunk_sha256": "2939f631257459ec8afa6a9aaf1861edae15ce4fbfafc6a5dcc1c21e8155b09d"
        }
      ],
      "parser_strategy": "aspnet_minimal_api_parser",
      "route_action": "HandleAsync",
      "confidence": 0.9,
      "evidence": [
        {
          "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
          "line": 27,
          "reason": "ASP.NET Minimal API Map* call"
        }
      ],
      "open_questions": []
    },
    {
      "interface_id": "INT-003",
      "type": "HTTP_API",
      "method": "GET",
      "path_or_name": "/api/catalog-items/{catalogItemId}",
      "owner_module": "Catalog",
      "entry_component": "CatalogItemGetByIdEndpoint",
      "called_service": [
        "GetByIdCatalogItemRequest.CorrelationId",
        "IRepository<CatalogItem>.GetByIdAsync",
        "IUriComposer.ComposePicUri"
      ],
      "visibility": "external_system",
      "source_file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
      "line": 25,
      "source_chunks": [
        {
          "chunk_id": "CHUNK-000234",
          "file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
          "start_line": 1,
          "end_line": 54,
     
... [truncated]
```

### final/application-risk-register.json
```
{
  "generated_at": "2026-06-15T07:27:16+00:00",
  "risks": [
    {
      "risk_id": "APP-RISK-001",
      "category": "unclear_boundary",
      "description": "Module candidate Catalog has weak or uncertain boundary evidence with coupling score 13.",
      "affected_module": "Catalog",
      "affected_component": "unknown",
      "severity": "High",
      "forward_engineering_impact": "Forward engineering could create artificial service boundaries or duplicate responsibilities if this candidate boundary is accepted without review.",
      "evidence": [
        {
          "file": "architecture-output/evidence-packs/module-boundary-pack.json",
          "reason": "weak or high-coupling module candidate"
        }
      ],
      "recommendation": "Confirm module ownership, public interfaces, and dependency direction before using this candidate as a modernization boundary.",
      "confidence": 0.879
    },
    {
      "risk_id": "APP-RISK-002",
      "category": "circular_dependency",
      "description": "Module dependency cycle detected: Admin -> ApplicationCore -> Basket -> Catalog -> DataAccess -> Identity -> Order -> Web",
      "affected_module": "Admin, ApplicationCore, Basket, Catalog, DataAccess, Identity, Order, Web",
      "affected_component": "unknown",
      "severity": "High",
      "forward_engineering_impact": "Cycle participants are risky extraction candidates until dependency direction and ownership are clarified.",
      "evidence": [
        {
          "file": "architecture-output/evidence-packs/dependency-pack.json",
          "reason": "module cycle evidence"
        }
      ],
      "recommendation": "Review cycle edges and break the cycle with clearer contracts or orchestration boundaries.",
      "confidence": 0.7
    },
    {
      "risk_id": "APP-RISK-003",
      "category": "high_coupling",
      "description": "High-coupling module candidates include Catalog, Basket, Identity, Web, ApplicationCore.",
      "affected_module": "multiple",
      "affected_component": "unknown",
      "severity": "High",
      "forward_engineering_impact": "High-coupling modules are poor first extraction candidates and need dependency review before rewrite.",
      "evidence": [
        {
          "file": "architecture-output/evidence-packs/dependency-pack.json",
          "reason": "high coupling module candidates"
        }
      ],
      "recommendation": "Start modernization with lower-coupled modules and treat these as later-stage candidates.",
      "confidence": 0.72
    },
    {
      "risk_id": "APP-RISK-004",
      "category": "shared_dependency",
      "description": "EfRepository is a high-coupling component candidate with total coupling 16.",
      "affected_module": "unknown",
      "affected_component": "EfRepository",
      "severity": "High",
      "forward_engineering_impact": "Shared high-coupling components can make migration sequencing and replacement risky.",
      "evidence": [
        {
          "file": "architecture-output/evidence-packs/dependency-pack.json",
          "reason": "high coupling component candidate"
        }
      ],
      "recommendation": "Map consumers and ownership before extracting modules that depend on this component.",
      "confidence": 0.72
    },
    {
      "risk_id": "APP-RISK-005",
      "category": "unknown",
      "description": "0 call flows are partial because parsed evidence did not fully resolve runtime dispatch and downstream calls.",
      "affected_module": "mu
... [truncated]
```

### final/architecture-decision-inputs.md
```
# Architecture Decision Inputs

These are decision prompts for architects. They are derived from extracted evidence and should be resolved before committing to future boundaries.

## ADR-INPUT-001: Confirm `Catalog` Boundary

Decision needed: Should `Catalog` be a separate future module/service boundary, remain internal, or merge with another capability?

Evidence: CAP-001; modules MOD-004 Catalog; source files src/ApplicationCore/CatalogSettings.cs, src/ApplicationCore/Entities/CatalogBrand.cs, src/ApplicationCore/Entities/CatalogItem.cs, src/ApplicationCore/Entities/CatalogType.cs, and 16 more.

Risk signal: risks 3, weak modules 1, coupling 13.

Recommended review: confirm ownership, public interfaces, data ownership, and call flows before extraction.

## ADR-INPUT-002: Confirm `Identity` Boundary

Decision needed: Should `Identity` be a separate future module/service boundary, remain internal, or merge with another capability?

Evidence: CAP-002; modules MOD-007 Identity; source files src/ApplicationCore/Constants/AuthorizationConstants.cs, src/ApplicationCore/Interfaces/ITokenClaimsService.cs, src/BlazorAdmin/CustomAuthStateProvider.cs, src/BlazorAdmin/Pages/Logout.razor, and 16 more.

Risk signal: risks 1, weak modules 1, coupling 8.

Recommended review: confirm ownership, public interfaces, data ownership, and call flows before extraction.

## ADR-INPUT-003: Confirm `Admin` Boundary

Decision needed: Should `Admin` be a separate future module/service boundary, remain internal, or merge with another capability?

Evidence: CAP-004; modules MOD-001 Admin; source files src/BlazorAdmin/App.razor, src/BlazorAdmin/Helpers/BlazorComponent.cs, src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs, src/BlazorAdmin/Helpers/RefreshBroadcast.cs, and 16 more.

Risk signal: risks 1, weak modules 1, coupling 3.

Recommended review: confirm ownership, public interfaces, data ownership, and call flows before extraction.

## ADR-INPUT-004: Confirm `Basket` Boundary

Decision needed: Should `Basket` be a separate future module/service boundary, remain internal, or merge with another capability?

Evidence: CAP-005; modules MOD-003 Basket; source files src/ApplicationCore/Entities/BasketAggregate/Basket.cs, src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs, src/ApplicationCore/Exceptions/BasketNotFoundException.cs, src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs, and 16 more.

Risk signal: risks 1, weak modules 1, coupling 9.

Recommended review: confirm ownership, public interfaces, data ownership, and call flows before extraction.

## ADR-INPUT-005: Confirm `Controllers` Boundary

Decision needed: Should `Controllers` be a separate future module/service boundary, remain internal, or merge with another capability?

Evidence: CAP-006; modules MOD-013 Web; source files src/Web/Controllers/Api/BaseApiController.cs, src/Web/Extensions/CacheHelpers.cs, src/Web/Extensions/EmailSenderExtensions.cs, src/Web/Extensions/UrlHelperExtensions.cs, and 16 more.

Risk signal: risks 1, weak modules 1, coupling 7.

Recommended review: confirm ownership, public interfaces, data ownership, and call flows before extraction.

## ADR-INPUT-006: Confirm `Order` Boundary

Decision needed: Should `Order` be a separate future module/service boundary, remain internal, or merge with another capability?

Evidence: CAP-007; modules MOD-009 Order; source files src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs, src/ApplicationCore/Entities/BuyerAggregate/Paym
... [truncated]
```

### final/architecture-pattern-report.md
```
# Architecture Pattern Report

## 1. Detected Architecture Pattern

Detected pattern: Layered Monolith.

## 2. Evidence

- Candidate pattern evidence from layering pack: [
  {
    "pattern": "Layered Monolith",
    "evidence": "Multiple projects/layers detected within one solution/repository and shared deployable units.",
    "confidence": 0.78
  },
  {
    "pattern": "Clean Architecture",
    "evidence": "Parsed layers include application/domain/infrastructure-style components; project names are used only as supporting evidence.",
    "confidence": 0.66
  },
  {
    "pattern": "Modular Monolith",
    "evidence": "Multiple module candidates exist inside shared backend/frontend projects; service separation is not established by evidence packs.",
    "confidence": 0.52
  }
]
- System inventory detects 2 deployable unit candidates and 10 project records.
- Component evidence detects layers across Presentation/UI, API, Application, Domain, Infrastructure, DataAccess, Integration, CrossCutting, and Unknown.

Source file anchors:

- src/ApplicationCore/ApplicationCore.csproj
- src/BlazorAdmin/BlazorAdmin.csproj
- src/BlazorShared/BlazorShared.csproj
- src/Infrastructure/Infrastructure.csproj
- src/PublicApi/PublicApi.csproj
- src/Web/Web.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj

## 3. Layer Structure

- API: 17 components
- Application: 69 components
- CrossCutting: 62 components
- DataAccess: 15 components
- Domain: 18 components
- Infrastructure: 9 components
- Integration: 2 components
- Presentation/UI: 115 components
- Unknown: 3 components

## 4. Pattern Confidence

Primary pattern confidence is 0.78. Competing pattern candidates and confidence scores are shown in the evidence block above, so this is an evidence-bounded observation rather than a pure pattern claim.

## 5. Pattern Violations

- ARCH-VIOL-001: Controller-like component CatalogBrandListEndpoint depends directly on repository EfRepository.
- ARCH-VIOL-002: Controller-like component CatalogItemGetByIdEndpoint depends directly on repository EfRepository.
- ARCH-VIOL-003: Controller-like component CreateCatalogItemEndpoint depends directly on repository EfRepository.
- ARCH-VIOL-004: Controller-like component DeleteCatalogItemEndpoint depends directly on repository EfRepository.
- ARCH-VIOL-005: Controller-like component UpdateCatalogItemEndpoint depends directly on repository EfRepository.
- ARCH-VIOL-006: Controller-like component CatalogTypeListEndpoint depends directly on repository EfRepository.
- ARCH-VIOL-007: Controller-like component IndexModel depends directly on repository EfRepository.
- ARCH-VIOL-008: Module dependency cycle detected: Admin -> ApplicationCore -> Basket -> Catalog -> DataAccess -> Identity -> Order -> Web
- ARCH-VIOL-009: Component EfRepository has high coupling score 16.
- ARCH-VIOL-010: Component UriComposer has high coupling score 8.

Violation source anchors: src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs, src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs, src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs, src/PublicApi/CatalogItemEndpoints/DeleteCatalogItemEndpoint.cs, src/PublicApi/CatalogItemEndpoints/UpdateCatalogItemEndpoint.cs, src/PublicApi/CatalogTypeEndpoints/CatalogTypeListEndpoint.cs, src/Web/Pages/Basket/Index.cshtml.cs, architecture-output/evidence-packs/dependency-pack.json. Dependency-cycle and high-coupling claims are deri
... [truncated]
```

### final/architecture-violation-register.json
```
{
  "generated_at": "2026-06-15T07:27:16+00:00",
  "source_evidence_packs": [
    "architecture-output/evidence-packs/layering-pattern-pack.json",
    "architecture-output/evidence-packs/dependency-pack.json"
  ],
  "violations": [
    {
      "violation_id": "ARCH-VIOL-001",
      "type": "layer_violation",
      "description": "Controller-like component CatalogBrandListEndpoint depends directly on repository EfRepository.",
      "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
      "affected_components": [
        "CatalogBrandListEndpoint",
        "EfRepository"
      ],
      "affected_modules": [
        "DataAccess",
        "Catalog"
      ],
      "severity": "Medium",
      "migration_impact": "This dependency should be reviewed before extracting or rewriting the affected UI/API flow.",
      "recommendation": "Route the flow through an application service or query abstraction before forward engineering.",
      "confidence": 0.72,
      "evidence": [
        {
          "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
          "line": 16,
          "reason": "method_parameter dependency IRepository<CatalogBrand> catalogBrandRepository on method HandleAsync"
        },
        {
          "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
          "line": 36,
          "reason": "CatalogBrandListEndpoint.HandleAsync calls catalogBrandRepository.ListAsync()"
        },
        {
          "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
          "line": 16,
          "reason": "CatalogBrandListEndpoint dependency IRepository<CatalogBrand> can resolve to registered implementation EfRepository"
        }
      ]
    },
    {
      "violation_id": "ARCH-VIOL-002",
      "type": "layer_violation",
      "description": "Controller-like component CatalogItemGetByIdEndpoint depends directly on repository EfRepository.",
      "file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
      "affected_components": [
        "CatalogItemGetByIdEndpoint",
        "EfRepository"
      ],
      "affected_modules": [
        "DataAccess",
        "Catalog"
      ],
      "severity": "Medium",
      "migration_impact": "This dependency should be reviewed before extracting or rewriting the affected UI/API flow.",
      "recommendation": "Route the flow through an application service or query abstraction before forward engineering.",
      "confidence": 0.72,
      "evidence": [
        {
          "file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
          "line": 14,
          "reason": "method_parameter dependency IRepository<CatalogItem> itemRepository on method HandleAsync"
        },
        {
          "file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
          "line": 34,
          "reason": "CatalogItemGetByIdEndpoint.HandleAsync calls itemRepository.GetByIdAsync()"
        },
        {
          "file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
          "line": 14,
          "reason": "CatalogItemGetByIdEndpoint dependency IRepository<CatalogItem> can resolve to registered implementation EfRepository"
        }
      ]
    },
    {
      "violation_id": "ARCH-VIOL-003",
      "type": "layer_violation",
      "description": "Controller-like component CreateCatalogItemEndpoint depends directly on repository EfRepository.",
      "file": 
... [truncated]
```

### final/business-capability-map.json
```
{
  "generated_at": "2026-06-15T07:27:16+00:00",
  "source_final_artifacts": [
    "architecture-output/final/module-boundary-map.json",
    "architecture-output/final/component-registry.json",
    "architecture-output/final/application-interface-catalogue.json",
    "architecture-output/final/application-risk-register.json"
  ],
  "summary": {
    "capability_count": 13,
    "module_count": 13,
    "component_count": 310,
    "interface_count": 55,
    "risk_count": 9
  },
  "capabilities": [
    {
      "name": "Catalog",
      "modules": [
        {
          "module_id": "MOD-004",
          "name": "Catalog",
          "boundary_quality": "Weak",
          "migration_readiness": "Blocked",
          "afferent_coupling": 7,
          "efferent_coupling": 6,
          "confidence": 0.879
        }
      ],
      "components": [
        {
          "component_id": "COMP-0055",
          "name": "CachedCatalogItemServiceDecorator",
          "type": "FrontendService",
          "layer": "Presentation/UI",
          "file": "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs"
        },
        {
          "component_id": "COMP-0056",
          "name": "CachedCatalogLookupDataServiceDecorator",
          "type": "FrontendService",
          "layer": "Presentation/UI",
          "file": "src/BlazorAdmin/Services/CachedCatalogLookupDataServiceDecorator .cs"
        },
        {
          "component_id": "COMP-0181",
          "name": "CachedCatalogViewModelService",
          "type": "Service",
          "layer": "Application",
          "file": "src/Web/Services/CachedCatalogViewModelService.cs"
        },
        {
          "component_id": "COMP-0004",
          "name": "CatalogBrand",
          "type": "Entity",
          "layer": "Domain",
          "file": "src/ApplicationCore/Entities/CatalogBrand.cs"
        },
        {
          "component_id": "COMP-0072",
          "name": "CatalogBrand",
          "type": "DTO",
          "layer": "Application",
          "file": "src/BlazorShared/Models/CatalogBrand.cs"
        },
        {
          "component_id": "COMP-0091",
          "name": "CatalogBrandConfiguration",
          "type": "Configuration",
          "layer": "Infrastructure",
          "file": "src/Infrastructure/Data/Config/CatalogBrandConfiguration.cs"
        },
        {
          "component_id": "COMP-0116",
          "name": "CatalogBrandDto",
          "type": "DTO",
          "layer": "Application",
          "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandDto.cs"
        },
        {
          "component_id": "COMP-0118",
          "name": "CatalogBrandListEndpoint",
          "type": "Controller",
          "layer": "API",
          "file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs"
        },
        {
          "component_id": "COMP-0073",
          "name": "CatalogBrandResponse",
          "type": "DTO",
          "layer": "Application",
          "file": "src/BlazorShared/Models/CatalogBrandResponse.cs"
        },
        {
          "component_id": "COMP-0085",
          "name": "CatalogContext",
          "type": "Repository",
          "layer": "DataAccess",
          "file": "src/Infrastructure/Data/CatalogContext.cs"
        },
        {
          "component_id": "COMP-0086",
          "name": "CatalogContextSeed",
          "type": "BatchJob",
          "layer": "DataAccess",
          "file": "src/Infrastructure/Data/CatalogContextSeed.cs"
        },
        
... [truncated]
```

### final/business-capability-map.md
```
# Business Capability Map

This file groups evidence-derived module candidates into higher-level forward-engineering capability candidates. These are not final bounded contexts until reviewed by architects and product/domain owners.

## Summary

- Capability candidates: 13
- Module candidates grouped: 13
- Components considered: 310
- Interfaces considered: 55

## Capability Candidates

| Capability | Class | Modules | Components | Interfaces | Data Components | Risks | Coupling | Forward Decision | Confidence | Evidence |
|---|---|---:|---:|---:|---:|---:|---:|---|---:|---|
| CAP-001 Catalog | DataInfrastructure | 1 | 25 | 9 | 7 | 3 | 13 | review_before_extraction | 0.874 | src/ApplicationCore/CatalogSettings.cs, src/ApplicationCore/Entities/CatalogBrand.cs, src/ApplicationCore/Entities/CatalogItem.cs, and 17 more |
| CAP-002 Identity | DataInfrastructure | 1 | 25 | 29 | 3 | 1 | 8 | review_before_extraction | 0.873 | src/ApplicationCore/Constants/AuthorizationConstants.cs, src/ApplicationCore/Interfaces/ITokenClaimsService.cs, src/BlazorAdmin/CustomAuthStateProvider.cs, and 17 more |
| CAP-003 Verification | TestVerification | 1 | 25 | 0 | 0 | 0 | 0 | review | 0.9 | tests/FunctionalTests/PublicApi/ApiTestFixture.cs, tests/FunctionalTests/PublicApi/ApiTokenHelper.cs, tests/FunctionalTests/Web/Controllers/AccountControllerSignIn.cs, and 17 more |
| CAP-004 Admin | InterfaceCapabilityCandidate | 1 | 23 | 2 | 0 | 1 | 3 | review_before_extraction | 0.776 | src/BlazorAdmin/App.razor, src/BlazorAdmin/Helpers/BlazorComponent.cs, src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs, and 17 more |
| CAP-005 Basket | DataInfrastructure | 1 | 23 | 3 | 3 | 1 | 9 | review_before_extraction | 0.786 | src/ApplicationCore/Entities/BasketAggregate/Basket.cs, src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs, src/ApplicationCore/Exceptions/BasketNotFoundException.cs, and 17 more |
| CAP-006 Controllers | InterfaceCapabilityCandidate | 1 | 21 | 3 | 0 | 1 | 7 | review_before_extraction | 0.711 | src/Web/Controllers/Api/BaseApiController.cs, src/Web/Extensions/CacheHelpers.cs, src/Web/Extensions/EmailSenderExtensions.cs, and 17 more |
| CAP-007 Order | DataInfrastructure | 1 | 21 | 2 | 5 | 1 | 4 | review_before_extraction | 0.894 | src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs, src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs, src/ApplicationCore/Entities/OrderAggregate/Address.cs, and 17 more |
| CAP-008 Application | TechnicalSupport | 1 | 13 | 0 | 2 | 1 | 6 | review | 0.68 | src/ApplicationCore/Entities/BaseEntity.cs, src/ApplicationCore/Exceptions/DuplicateException.cs, src/ApplicationCore/Extensions/JsonExtensions.cs, and 10 more |
| CAP-009 Contracts | UICapabilityCandidate | 1 | 12 | 0 | 0 | 0 | 1 | review | 0.88 | src/BlazorShared/Attributes/EndpointAttribute.cs, src/BlazorShared/BaseUrlConfiguration.cs, src/BlazorShared/Interfaces/ICatalogItemService.cs, and 9 more |
| CAP-010 Cross | InterfaceCapabilityCandidate | 1 | 10 | 7 | 0 | 0 | 4 | review | 0.725 | src/PublicApi/CustomSchemaFilters.cs, src/PublicApi/Middleware/ExceptionMiddleware.cs, src/PublicApi/Program.cs, and 7 more |
| CAP-011 Message | ApplicationCapabilityCandidate | 1 | 5 | 0 | 0 | 0 | 2 | review | 0.68 | src/PublicApi/BaseMessage.cs, src/PublicApi/BaseRequest.cs, src/PublicApi/BaseResponse.cs, and 2 more |
| CAP-012 Infrastructure | ApplicationCapabilityCandidate | 1 | 3 | 0 | 0 | 0 | 4 | review | 0.78 | src/Infrastructure/Dependencies.cs, src/Infrastructure
... [truncated]
```

### final/call-flow-map.json
```
{
  "generated_at": "2026-06-15T07:27:16+00:00",
  "source_evidence_pack": "architecture-output/evidence-packs/call-flow-pack.json",
  "summary": {
    "flow_count": 55,
    "partial_flow_count": 0,
    "semantic_trace_flow_count": 43,
    "flows_with_data_access_count": 22,
    "flows_with_external_system_count": 2
  },
  "flows": [
    {
      "flow_id": "FLOW-0001",
      "name": "POST /api/authenticate",
      "entry_point": "POST /api/authenticate",
      "entry_point_type": "HTTP_API",
      "status": "traced_from_dependency_candidates",
      "steps": [
        {
          "step": 1,
          "component": "AuthenticateEndpoint",
          "layer": "API",
          "module": "Identity",
          "action": "/api/authenticate",
          "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
          "source_chunks": [
            {
              "chunk_id": "CHUNK-000227",
              "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
              "start_line": 1,
              "end_line": 59,
              "chunk_sha256": "2ae1af5dc8257d055f73705470add93b9cc85ae42292769be6198f46c36d125d"
            }
          ],
          "evidence": {
            "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
            "line": 36,
            "reason": "entry point owner"
          }
        },
        {
          "step": 2,
          "component": "BaseMessage",
          "layer": "Application",
          "module": "PublicApi",
          "action": "CorrelationId",
          "file": "src/PublicApi/BaseMessage.cs",
          "source_chunks": [],
          "evidence": {
            "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
            "line": 39,
            "reason": "AuthenticateEndpoint.HandleAsync semantically resolves to Microsoft.eShopWeb.PublicApi.BaseMessage.CorrelationId()",
            "dependency_id": "RDEP-00094",
            "source_method": "HandleAsync",
            "resolution_quality": "roslyn_semantic_symbol_binding"
          }
        },
        {
          "step": 3,
          "component": "IdentityTokenClaimService",
          "layer": "Application",
          "module": "Identity",
          "action": "GetTokenAsync",
          "file": "src/Infrastructure/Identity/IdentityTokenClaimService.cs",
          "source_chunks": [],
          "evidence": {
            "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
            "line": 54,
            "reason": "AuthenticateEndpoint.HandleAsync semantically resolves to Microsoft.eShopWeb.ApplicationCore.Interfaces.ITokenClaimsService.GetTokenAsync()",
            "dependency_id": "RDEP-00095",
            "source_method": "HandleAsync",
            "resolution_quality": "roslyn_semantic_symbol_binding"
          }
        }
      ],
      "modules_touched": [
        "Identity",
        "PublicApi"
      ],
      "external_systems_touched": [],
      "data_access_components": [],
      "semantic_step_count": 2,
      "risk_flags": [
        "data_access_not_resolved"
      ],
      "confidence": 0.74,
      "open_questions": []
    },
    {
      "flow_id": "FLOW-0002",
      "name": "GET /api/catalog-brands",
      "entry_point": "GET /api/catalog-brands",
      "entry_point_type": "HTTP_API",
      "status": "traced_from_dependency_candidates",
      "steps": [
        {
          "step": 1,
          "component": "CatalogBrandListEndpoint",
          "layer": "API",
          "module": "Catalog",
      
... [truncated]
```

### final/component-registry.json
```
{
  "generated_at": "2026-06-15T07:27:16+00:00",
  "source_evidence_pack": "architecture-output/evidence-packs/component-registry-pack.json",
  "summary": {
    "component_count": 310,
    "components_by_type": {
      "BatchJob": 2,
      "Configuration": 27,
      "Controller": 32,
      "DTO": 58,
      "Entity": 19,
      "ExternalClient": 2,
      "FrontendComponent": 68,
      "FrontendService": 8,
      "Handler": 4,
      "Mapper": 1,
      "Middleware": 1,
      "Repository": 13,
      "Service": 34,
      "Unknown": 40,
      "Validator": 1
    },
    "components_by_layer": {
      "API": 17,
      "Application": 69,
      "CrossCutting": 62,
      "DataAccess": 15,
      "Domain": 18,
      "Infrastructure": 9,
      "Integration": 2,
      "Presentation/UI": 115,
      "Unknown": 3
    },
    "components_by_module_guess": {
      "Admin": 23,
      "ApplicationCore": 13,
      "Basket": 23,
      "Catalog": 66,
      "CrossCutting": 10,
      "DataAccess": 2,
      "Identity": 66,
      "Infrastructure": 3,
      "Order": 21,
      "PublicApi": 5,
      "SharedContracts": 12,
      "Verification": 45,
      "Web": 21
    },
    "roslyn_semantic_component_count": 247
  },
  "components": [
    {
      "component_id": "COMP-0001",
      "name": "CatalogSettings",
      "type": "Configuration",
      "layer": "CrossCutting",
      "module": "Catalog",
      "project": "ApplicationCore",
      "parser_backend": "static_structural+roslyn_semantic_model",
      "semantic_symbol_id": "Microsoft.eShopWeb.CatalogSettings",
      "semantic_full_name": "Microsoft.eShopWeb.CatalogSettings",
      "semantic_parser_backend": "roslyn_semantic_model",
      "semantic_methods": [],
      "semantic_constructor_dependencies": [],
      "semantic_confidence": 0.96,
      "architecture_significance": "Supporting",
      "is_major_application_component": true,
      "architecture_significance_reason": "component supports application architecture and ownership decisions",
      "file": "src/ApplicationCore/CatalogSettings.cs",
      "source_chunks": [
        {
          "chunk_id": "CHUNK-000056",
          "file": "src/ApplicationCore/CatalogSettings.cs",
          "start_line": 1,
          "end_line": 6,
          "chunk_sha256": "556a5b04bfb534a25cd3edd49d22e28c5c228e40511fd6fe9a581c43c3babeee"
        }
      ],
      "public_methods": [],
      "dependencies": [],
      "risk_flags": [],
      "confidence": 0.96,
      "evidence": [
        {
          "file": "src/ApplicationCore/CatalogSettings.cs",
          "line": 2,
          "reason": "settings/constants/options naming or folder pattern"
        },
        {
          "file": "src/ApplicationCore/CatalogSettings.cs",
          "line": 3,
          "reason": "Roslyn semantic model resolved component symbol and members"
        }
      ]
    },
    {
      "component_id": "COMP-0002",
      "name": "AuthorizationConstants",
      "type": "Configuration",
      "layer": "CrossCutting",
      "module": "Identity",
      "project": "ApplicationCore",
      "parser_backend": "static_structural+roslyn_semantic_model",
      "semantic_symbol_id": "Microsoft.eShopWeb.ApplicationCore.Constants.AuthorizationConstants",
      "semantic_full_name": "Microsoft.eShopWeb.ApplicationCore.Constants.AuthorizationConstants",
      "semantic_parser_backend": "roslyn_semantic_model",
      "semantic_methods": [],
      "semantic_constructor_dependencies": [],
      "semantic_confidence": 0.96,
      "arch
... [truncated]
```

### final/confidence-report.md
```
# Confidence Report

## Confidence Summary

| Area | Confidence | Evidence |
|---|---|---|
| Project/deployable inventory | High | `system-inventory.json` and inventory outputs identify project files and deployable candidates. |
| Component detection | Medium/High | 310 components detected; 0 major production components have Unknown type/layer; 40 total Unknown including support/test artifacts. |
| API/interface detection | Medium/High | 55 interfaces detected; 33 API contracts have confidence >= 0.85. |
| Dependency graph shape | High | 534 edges; invalid graph endpoints after normalization: 0. |
| Capability grouping | Medium | 13 capability candidates; average capability confidence 0.8. |
| Call flows | Medium | 55 flows detected; 55 traced/coverage-marker flows and 0 partial flows. |
| Test/runtime evidence | Medium/High | Static test-source evidence is captured; runtime status: not_run; runtime projects: 0. |
| External boundary purpose | Medium/Low | External targets are detected as candidates; purpose/ownership still needs confirmation. |

## Why Confidence Is Lower In Some Areas

- Static parsing cannot prove complete runtime route coverage where framework conventions expand routes dynamically.
- Dynamic dispatch and framework conventions can still limit call-flow completeness.
- Capability grouping is inferred from names, folders, modules, routes, components, and dependencies.
- Unknown component classifications may hide support classes or architecture-significant pieces.

## How To Increase Confidence

1. Confirm open questions with application owners.
2. Add contract tests for preserved API contracts.
3. Add parser support for language/framework-specific call graphs.
4. Review any remaining major Unknown components and test-project inclusion policy.
5. Validate external dependency purpose from runtime config and deployment knowledge.

```

### final/data-ownership-map.md
```
# Data Ownership Map

This is application-architecture ownership evidence, not a database design document.

## Entity Ownership Candidates

| Entity | Component ID | Module | Capability | File | Confidence |
|---|---|---|---|---|---:|
| BaseEntity | COMP-0003 | ApplicationCore | CAP-008 Application | src/ApplicationCore/Entities/BaseEntity.cs | 0.96 |
| CatalogBrand | COMP-0004 | Catalog | CAP-001 Catalog | src/ApplicationCore/Entities/CatalogBrand.cs | 0.96 |
| CatalogItem | COMP-0005 | Catalog | CAP-001 Catalog | src/ApplicationCore/Entities/CatalogItem.cs | 0.96 |
| CatalogItemDetails | COMP-0006 | Catalog | CAP-001 Catalog | src/ApplicationCore/Entities/CatalogItem.cs | 0.96 |
| CatalogType | COMP-0007 | Catalog | CAP-001 Catalog | src/ApplicationCore/Entities/CatalogType.cs | 0.96 |
| Basket | COMP-0008 | Basket | CAP-005 Basket | src/ApplicationCore/Entities/BasketAggregate/Basket.cs | 0.96 |
| BasketItem | COMP-0009 | Basket | CAP-005 Basket | src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs | 0.96 |
| Buyer | COMP-0010 | Order | CAP-007 Order | src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs | 0.96 |
| PaymentMethod | COMP-0011 | Order | CAP-007 Order | src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs | 0.96 |
| Address | COMP-0012 | Order | CAP-007 Order | src/ApplicationCore/Entities/OrderAggregate/Address.cs | 0.96 |
| CatalogItemOrdered | COMP-0013 | Catalog | CAP-001 Catalog | src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs | 0.96 |
| Order | COMP-0014 | Order | CAP-007 Order | src/ApplicationCore/Entities/OrderAggregate/Order.cs | 0.96 |
| OrderItem | COMP-0015 | Order | CAP-007 Order | src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs | 0.96 |
| IAggregateRoot | COMP-0021 | ApplicationCore | CAP-008 Application | src/ApplicationCore/Interfaces/IAggregateRoot.cs | 0.96 |
| ApplicationUser | COMP-0099 | Identity | CAP-002 Identity | src/Infrastructure/Identity/ApplicationUser.cs | 0.96 |
| BasketAddItem | COMP-0226 | Verification | CAP-003 Verification | tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketAddItem.cs | 0.96 |
| BasketRemoveEmptyItems | COMP-0227 | Verification | CAP-003 Verification | tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs | 0.96 |
| BasketTotalItems | COMP-0228 | Verification | CAP-003 Verification | tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketTotalItems.cs | 0.96 |
| OrderTotal | COMP-0229 | Verification | CAP-003 Verification | tests/UnitTests/ApplicationCore/Entities/OrderTests/OrderTotal.cs | 0.96 |

## Repository / Data Access Candidates

| Component | Component ID | Type | Layer | Module | Capability | File | Risk Note |
|---|---|---|---|---|---|---|---|
| IReadRepository | COMP-0027 | Repository | DataAccess | ApplicationCore | CAP-008 Application | src/ApplicationCore/Interfaces/IReadRepository.cs | review shared ownership |
| IRepository | COMP-0028 | Repository | DataAccess | ApplicationCore | CAP-008 Application | src/ApplicationCore/Interfaces/IRepository.cs | review shared ownership |
| BasketWithItemsSpecification | COMP-0034 | Repository | DataAccess | Basket | CAP-005 Basket | src/ApplicationCore/Specifications/BasketWithItemsSpecification.cs | review shared ownership |
| CatalogFilterPaginatedSpecification | COMP-0035 | Repository | DataAccess | Catalog | CAP-001 Catalog | src/ApplicationCore/Specifications/CatalogFilterPaginatedSpecification.cs | review shared ownership |
| Cat
... [truncated]
```


---

## Reminder on output format
Please output all files from this stage's Output section, each wrapped in
===AA_FILE_START:<path>=== / ===AA_FILE_END=== markers. I'll be parsing your
response for these exact markers, so please give full file contents rather
than descriptions.
