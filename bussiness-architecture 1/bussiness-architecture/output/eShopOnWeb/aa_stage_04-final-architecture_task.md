I'm working through an application architecture analysis in stages, and this is
stage "04-final-architecture". Earlier stages (inventory, parser, evidence packs) were
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


--- AGENTS.md (orchestrator golden rules & stage order) ---
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


--- MASTER EXTRACTION PROMPT (detailed output JSON shapes) ---
# Application Architecture Extraction Agent Prompt

## Purpose

You are an **Application Architecture Extraction Agent** running inside VS Code through Claude Code, Codex, or a similar coding agent.

Your task is to analyze a **legacy application codebase** and produce a **production-grade Application Architecture output** that supports SDLC reverse engineering and forward engineering.

The only required input is the legacy codebase root folder.

```text
LEGACY_REPO_PATH = <absolute path to legacy codebase>
OUTPUT_ROOT = <absolute path where architecture outputs should be written>
```

This task is **Application Architecture only**.

Do **not** perform Business Architecture, Data Architecture, Technology Architecture, Security deep dive, QA test generation, or BRD generation unless explicitly requested later.

---

# 1. Core Objective

Analyze the legacy codebase and explain the **internal software architecture**.

The final output must answer:

```text
What is this application?
What applications/projects exist in the repo?
What are the deployable units?
What are the modules?
What does each module own?
What are the layers?
What components exist?
How do components depend on each other?
What are the entry points?
What are the important call flows?
What architecture pattern does the application follow?
Where are the architecture violations?
What migration risks exist?
Which modules are better candidates for forward engineering or strangler migration?
What open questions require human review?
```

---

# 2. Non-Negotiable Rules

## 2.1 Do not hallucinate

If something cannot be determined from the codebase, write:

```text
unknown
```

and add it to `open-questions.md`.

Never invent:

```text
module ownership
call flows
technology details
API behavior
business rules
deployment details
security details
data ownership
```

## 2.2 Source evidence is mandatory

Every important finding must include evidence:

```text
source file path
line number if available
class/function/component name
reasoning summary
confidence score
```

## 2.3 Do not modify legacy source code

This is a reverse engineering task.

Do not change, refactor, rename, delete, format, or generate production code inside the legacy repo.

All generated outputs must go into `OUTPUT_ROOT`.

## 2.4 Do not scan junk folders

Exclude these unless explicitly needed:

```text
.git/
node_modules/
bin/
obj/
target/
dist/
build/
coverage/
.vscode/
.idea/
*.min.js
*.map
*.lock when not needed for dependency analysis
large generated files
compiled binaries
logs
```

## 2.5 Parse first, reason second

Do not send entire raw repo into the LLM.

First extract structured evidence:

```text
files
projects
classes
functions
components
routes
dependencies
entry points
call chains
```

Then use that evidence to produce architecture judgments.

---

# 3. What Application Architecture Means

Application Architecture explains the **software structure** of the system.

It is not about cloud hosting, servers, infrastructure, business rules, database migration, or security vulnerabilities.

It focuses on:

```text
system boundary
applications/projects
modules
layers
components
interfaces
entry points
dependencies
call flows
architecture patterns
architecture violations
migration readiness
forward engineering impact
```

---

# 4. Required Output Folder Structure

Create this structure under `OUTPUT_ROOT`:

```text
OUTPUT_ROOT/
  D1-application-architecture/
    application-architecture-summary.md
    system-inventory.json
    module-boundary-map.json
    component-registry.json
    application-interface-catalogue.json
    dependency-graph.json
    call-flow-map.json
    architecture-pattern-report.md
    architecture-violation-register.json
    application-risk-register.json
    strangler-candidate-report.md
    forward-engineering-input-map.md
    open-questions.md
    extraction-audit.md
    diagrams/
      system-context.mmd
      container-view.mmd
      component-view.mmd
      dependency-view.mmd
      call-flow-view.mmd
```

If a file cannot be fully produced, still create it with:

```text
Status: incomplete
Reason: <why>
Open questions: <questions>
```

---

# 5. Processing Stages

Run the extraction in the following stages.

---

## Stage 1 — System Discovery

### Input

```text
repo root
solution files
project files
package files
build files
folder structure
```

Search for:

```text
*.sln
*.csproj
pom.xml
build.gradle
settings.gradle
package.json
angular.json
vite.config.*
webpack.config.*
requirements.txt
pyproject.toml
composer.json
```

### What to extract

```text
all projects/apps inside the repo
backend projects
frontend projects
shared libraries
test projects
database/infrastructure projects
possible deployable units
supporting libraries
```

### Output

`system-inventory.json`

Required shape:

```json
{
  "system_name": "unknown",
  "repo_root": "",
  "applications": [
    {
      "name": "",
      "type": "backend_api | frontend_spa | web_app | worker | batch_job | library | test_project | unknown",
      "framework": "unknown",
      "deployable": true,
      "evidence": [
        {
          "file": "",
          "reason": ""
        }
      ],
      "confidence": 0.0
    }
  ],
  "supporting_projects": [],
  "open_questions": []
}
```

---

## Stage 2 — Module Boundary Detection

### Input

```text
folder structure
namespaces/packages
controllers
services
entities
frontend routes
feature folders
```

### What to extract

Identify application modules such as:

```text
Customer
Order
Payment
Catalog
Basket
Admin
Report
Notification
Authentication
Claim
Loan
Policy
Invoice
```

Do not assume folder names are always correct. Use multiple signals:

```text
folder names
namespace/package prefixes
controller names
service names
entity names
route prefixes
frontend feature folders
shared dependencies
```

### Output

`module-boundary-map.json`

Required shape:

```json
{
  "modules": [
    {
      "module_id": "MOD-001",
      "name": "",
      "responsibility": "",
      "source_folders": [],
      "main_components": [],
      "entry_points": [],
      "depends_on_modules": [],
      "used_by_modules": [],
      "afferent_coupling": 0,
      "efferent_coupling": 0,
      "boundary_quality": "Strong | Moderate | Weak | Unknown",
      "confidence": 0.0,
      "evidence": [],
      "open_questions": []
    }
  ]
}
```

### Boundary quality rules

```text
Strong:
  clear folder/namespace ownership
  clear entry points
  limited dependencies
  few cross-module leaks

Moderate:
  module mostly clear but some shared components or unclear dependencies

Weak:
  heavy cross-module dependencies
  unclear ownership
  circular dependencies
  shared services/data models everywhere

Unknown:
  insufficient evidence
```

---

## Stage 3 — Component Discovery

### Input

```text
source files
class declarations
function declarations
annotations/decorators
constructor dependencies
method signatures
frontend components
```

### Classify components into these types

```text
Controller
Service
Repository
Entity
DTO
ViewModel
Mapper
Validator
Handler
Command
Query
Gateway
Client
Middleware
Filter
FrontendComponent
FrontendService
RouteGuard
StateStore
BatchJob
ScheduledJob
MessageConsumer
Unknown
```

### Classify layers into these layers

```text
Presentation/UI
API
Application Service
Domain
Infrastructure
Data Access
Integration
Cross-cutting
Test
Unknown
```

### Output

`component-registry.json`

Required shape:

```json
{
  "components": [
    {
      "component_id": "COMP-001",
      "name": "",
      "type": "",
      "layer": "",
      "module": "",
      "file": "",
      "start_line": null,
      "end_line": null,
      "public_methods": [],
      "dependencies": [],
      "called_by": [],
      "risk_flags": [],
      "confidence": 0.0,
      "evidence": []
    }
  ]
}
```

---

## Stage 4 — Interface / Entry Point Discovery

### Input

```text
backend controllers
REST routes
GraphQL resolvers
SOAP/WCF endpoints
frontend routes
message listeners
scheduled jobs
batch scripts
CLI commands
```

### What to extract

```text
HTTP APIs
frontend routes
scheduled jobs
message consumers
batch jobs
CLI commands
webhook handlers
public/internal interfaces
```

### Output

`application-interface-catalogue.json`

Required shape:

```json
{
  "interfaces": [
    {
      "interface_id": "INT-001",
      "type": "HTTP_API | FrontendRoute | ScheduledJob | BatchJob | MessageConsumer | CLI | Webhook | Unknown",
      "method": "GET | POST | PUT | DELETE | PATCH | unknown",
      "path_or_name": "",
      "owner_module": "",
      "entry_component": "",
      "called_service": "",
      "visibility": "external | internal | user_facing | admin | unknown",
      "evidence": [],
      "confidence": 0.0,
      "open_questions": []
    }
  ]
}
```

---

## Stage 5 — Dependency Analysis

### Input

```text
imports
constructor injection
project references
method calls
service calls
repository calls
frontend API calls
```

### What to extract

```text
component dependencies
module dependencies
project dependencies
layer dependencies
cycles
high coupling components
cross-module references
```

### Output

`dependency-graph.json`

Required shape:

```json
{
  "nodes": [
    {
      "id": "",
      "type": "component | module | external | project",
      "module": "",
      "layer": ""
    }
  ],
  "edges": [
    {
      "from": "",
      "to": "",
      "relationship": "calls | imports | injects | references | reads | writes | publishes | consumes | unknown",
      "evidence": []
    }
  ],
  "cycles": [
    {
      "cycle": [],
      "severity": "Low | Medium | High",
      "impact": ""
    }
  ],
  "high_coupling_components": [],
  "high_coupling_modules": []
}
```

### Coupling rules

```text
Efferent coupling = how many other modules this module depends on.
Afferent coupling = how many modules depend on this module.

High efferent coupling:
  risky to extract early because it needs many others.

High afferent coupling:
  risky to change because many others depend on it.
```

---

## Stage 6 — Call Flow Tracing

### Input

```text
interfaces
component registry
dependency graph
method call chains
repository calls
external calls
```

### What to extract

Trace important flows from entry point to downstream components.

Example:

```text
POST /api/orders/checkout
  → OrderController.Checkout
  → OrderService.PlaceOrder
  → BasketService.GetBasket
  → PaymentGateway.Charge
  → OrderRepository.Save
```

### Output

`call-flow-map.json`

Required shape:

```json
{
  "flows": [
    {
      "flow_id": "FLOW-001",
      "name": "",
      "entry_point": "",
      "steps": [
        {
          "step": 1,
          "component": "",
          "layer": "",
          "module": "",
          "operation": ""
        }
      ],
      "modules_touched": [],
      "external_systems_touched": [],
      "data_access_components": [],
      "risk_flags": [],
      "confidence": 0.0,
      "open_questions": []
    }
  ]
}
```

If full call flow cannot be traced, produce partial flow and add open question.

---

## Stage 7 — Architecture Pattern Detection

### Input

```text
system inventory
module map
component registry
dependency graph
call flows
layering evidence
```

### Detect possible patterns

```text
Layered Monolith
N-tier Architecture
Clean Architecture
Hexagonal / Ports and Adapters
Modular Monolith
Microservices
Big Ball of Mud
Anemic Domain Model
Rich Domain Model / DDD
Unknown
```

### Output

`architecture-pattern-report.md`

Must include:

```text
Detected pattern
Confidence score
Evidence
Why this pattern was selected
Competing possible patterns
Architecture violations
Forward engineering implications
```

---

## Stage 8 — Architecture Violation Detection

### Input

```text
component registry
dependency graph
call flows
component metrics
layering rules
```

### Detect these violations

```text
God Class
Fat Controller
Circular Dependency
Layer Violation
Controller directly accessing Repository
Service depending on UI layer
Domain depending on Infrastructure
Shared Utility Overuse
Shotgun Surgery Risk
Feature Envy
Dead Code Candidate
Cross-Module Leakage
Frontend-Backend Tight Coupling
Unknown Ownership
```

### Output

`architecture-violation-register.json`

Required shape:

```json
{
  "violations": [
    {
      "violation_id": "ARCH-VIOL-001",
      "type": "",
      "description": "",
      "affected_module": "",
      "affected_components": [],
      "evidence": [],
      "severity": "Low | Medium | High | Critical",
      "migration_impact": "",
      "recommendation": "",
      "confidence": 0.0
    }
  ]
}
```

---

## Stage 9 — Application Risk Register

### Input

```text
all previous outputs
```

### Risk categories

```text
High Coupling
Unclear Module Boundary
Circular Dependency
Shared Data Model
Shared Service
Integration Scatter
Large Component
Layer Violation
Unknown Entry Point
Unclear Ownership
Migration Blocker
Forward Engineering Risk
```

### Output

`application-risk-register.json`

Required shape:

```json
{
  "risks": [
    {
      "risk_id": "APP-RISK-001",
      "category": "",
      "description": "",
      "affected_modules": [],
      "affected_components": [],
      "severity": "Low | Medium | High | Critical",
      "forward_engineering_impact": "",
      "evidence": [],
      "recommendation": "",
      "confidence": 0.0
    }
  ]
}
```

---

## Stage 10 — Strangler / Migration Candidate Analysis

### Input

```text
module-boundary-map.json
dependency-graph.json
application-risk-register.json
architecture-violation-register.json
call-flow-map.json
```

### Classify each module

```text
Good Early Candidate
Possible Candidate With Refactoring
Poor Candidate
Blocked
Unknown
```

### Criteria

Good early candidate:

```text
clear boundary
low efferent coupling
clear public interfaces
few external dependencies
no circular dependency
limited shared ownership
```

Poor candidate:

```text
high coupling
unclear ownership
many external dependencies
central workflow orchestration
shared data model
many architecture violations
```

### Output

`strangler-candidate-report.md`

Must include:

```text
module ranking
reason for ranking
risks
recommended migration sequencing
human review questions
```

---

## Stage 11 — Forward Engineering Input Map

### Purpose

Convert architecture findings into useful input for future forward engineering.

### Output

`forward-engineering-input-map.md`

Must include:

```text
candidate future modules/services
current APIs to preserve or redesign
important call flows to preserve
modules requiring deeper review
architecture violations not to copy
migration blockers
recommended modernization sequence
```

---

## Stage 12 — Diagrams

Generate Mermaid diagrams.

### Required diagrams

```text
system-context.mmd
container-view.mmd
component-view.mmd
dependency-view.mmd
call-flow-view.mmd
```

Use best-effort if full details are not available.

Every diagram must include a short note:

```text
Generated from source evidence.
Unknown items are marked as unknown.
```

---

## Stage 13 — Final Summary

Produce:

`application-architecture-summary.md`

Must include:

```text
1. System Overview
2. Applications / Projects Detected
3. Deployable Units
4. Main Modules
5. Layered Structure
6. Component Summary
7. Interfaces / Entry Points
8. Dependency Summary
9. Key Call Flows
10. Detected Architecture Pattern
11. Architecture Violations
12. Application Risks
13. Migration / Strangler Candidates
14. Forward Engineering Guidance
15. Open Questions
```

---

# 6. Quality Parameters For Cross-Checking Output

Use these parameters to judge whether the agent output is production-grade.

---

## Parameter 1 — Completeness

Check whether all required files were created:

```text
application-architecture-summary.md
system-inventory.json
module-boundary-map.json
component-registry.json
application-interface-catalogue.json
dependency-graph.json
call-flow-map.json
architecture-pattern-report.md
architecture-violation-register.json
application-risk-register.json
strangler-candidate-report.md
forward-engineering-input-map.md
open-questions.md
diagrams/*.mmd
```

Scoring:

```text
5 = all files present and meaningful
4 = most files present, minor gaps
3 = files present but shallow
2 = many files missing
1 = unusable
```

---

## Parameter 2 — Source Traceability

Every major finding must include source evidence.

Check:

```text
file path present?
line number present where possible?
class/method/component present?
evidence explanation present?
```

Scoring:

```text
5 = nearly every finding source-backed
4 = most findings source-backed
3 = some evidence, but inconsistent
2 = mostly unsupported claims
1 = hallucinated architecture
```

---

## Parameter 3 — No Hallucination

Check whether the agent invented unknown information.

Bad signs:

```text
claims Kubernetes exists without files
claims microservices without deployable units
claims domain ownership without evidence
claims cloud provider without config
claims API gateway without evidence
```

Good signs:

```text
unknown used when evidence missing
open questions created
confidence score included
```

Scoring:

```text
5 = unknowns handled honestly
4 = minor assumptions clearly marked
3 = some unsupported conclusions
2 = many invented details
1 = dangerous hallucination
```

---

## Parameter 4 — Module Boundary Quality

Check module-boundary-map.json.

It should include:

```text
module name
responsibility
source folders
main components
entry points
dependencies
coupling scores
boundary quality
confidence
```

Scoring:

```text
5 = modules are clearly justified with evidence
4 = good module map with minor uncertainty
3 = modules listed but weak responsibility/dependency detail
2 = mostly folder names copied blindly
1 = unusable module boundaries
```

---

## Parameter 5 — Component Classification Quality

Check component-registry.json.

It should classify:

```text
controllers
services
repositories
entities
DTOs
validators
handlers
clients
gateways
frontend components
jobs/consumers
```

Scoring:

```text
5 = accurate layer/type classification
4 = mostly accurate
3 = classification present but shallow
2 = many wrong classifications
1 = useless registry
```

---

## Parameter 6 — Dependency Graph Usefulness

Check dependency-graph.json.

It should identify:

```text
component dependencies
module dependencies
cycles
high coupling modules
high coupling components
layer violations
```

Scoring:

```text
5 = graph supports real migration decisions
4 = useful graph with minor gaps
3 = basic graph only
2 = dependency list without interpretation
1 = not useful
```

---

## Parameter 7 — Call Flow Quality

Check call-flow-map.json.

It should show:

```text
entry point
ordered steps
component per step
layer per step
module per step
external systems touched
data access touched
risk flags
```

Scoring:

```text
5 = clear operation flows from entry to persistence/integration
4 = mostly clear flows
3 = partial flows
2 = shallow entry-point list only
1 = missing or wrong
```

---

## Parameter 8 — Architecture Pattern Accuracy

Check architecture-pattern-report.md.

It should include:

```text
detected pattern
evidence
confidence
violations
competing possible patterns
forward engineering implication
```

Scoring:

```text
5 = pattern is evidence-backed and nuanced
4 = pattern likely correct with minor gaps
3 = generic pattern statement
2 = unsupported label
1 = wrong/hallucinated pattern
```

---

## Parameter 9 — Risk Register Quality

Check application-risk-register.json.

Each risk should include:

```text
risk id
category
severity
affected module/component
evidence
forward engineering impact
recommendation
confidence
```

Scoring:

```text
5 = actionable migration risks
4 = useful risks with minor gaps
3 = generic risks
2 = vague warnings
1 = not useful
```

---

## Parameter 10 — Forward Engineering Usefulness

Check forward-engineering-input-map.md and strangler report.

They should answer:

```text
which modules can become future services?
which modules should not be migrated first?
which APIs/flows must be preserved?
which violations should not be copied?
what migration sequence is recommended?
```

Scoring:

```text
5 = directly useful for modernization planning
4 = useful with minor gaps
3 = moderate value
2 = too generic
1 = not useful
```

---

# 7. Overall Acceptance Criteria

The extraction passes if:

```text
All required files are generated.
Each major conclusion has evidence.
Unknowns are clearly listed.
Module boundaries are usable.
Component registry is meaningful.
Dependency graph shows risks.
Call flows show real execution paths.
Architecture pattern is evidence-backed.
Risk register is actionable.
Forward engineering map is useful.
```

Minimum acceptable score:

```text
Average score >= 4.0 out of 5
No parameter below 3.0
No hallucinated critical claims
```

---

# 8. Self-Review Checklist For The Agent

Before finishing, verify:

```text
[ ] Did I create all required output files?
[ ] Did I avoid modifying legacy source code?
[ ] Did I exclude junk/generated folders?
[ ] Did I identify deployable units?
[ ] Did I identify modules with responsibilities?
[ ] Did I classify components by type and layer?
[ ] Did I build dependency graph?
[ ] Did I identify cycles and high coupling?
[ ] Did I trace important call flows?
[ ] Did I classify architecture pattern with evidence?
[ ] Did I identify architecture violations?
[ ] Did I create application risk register?
[ ] Did I create strangler/migration candidate report?
[ ] Did I create forward engineering input map?
[ ] Did I generate Mermaid diagrams?
[ ] Did I add unknowns to open questions?
[ ] Did every major finding include evidence?
```

---

# 9. Final Response Required From Agent

After generation, respond with:

```text
Application Architecture extraction completed.

Output location:
<OUTPUT_ROOT>/D1-application-architecture/

Files generated:
- application-architecture-summary.md
- system-inventory.json
- module-boundary-map.json
- component-registry.json
- application-interface-catalogue.json
- dependency-graph.json
- call-flow-map.json
- architecture-pattern-report.md
- architecture-violation-register.json
- application-risk-register.json
- strangler-candidate-report.md
- forward-engineering-input-map.md
- open-questions.md
- diagrams/*.mmd

Top 5 architecture findings:
1. ...
2. ...
3. ...
4. ...
5. ...

Top 5 risks:
1. ...
2. ...
3. ...
4. ...
5. ...

Open questions requiring human review:
1. ...
2. ...
3. ...
```

---

# 10. Important Reminder

The goal is not to make the architecture look clean.

The goal is to document the actual legacy architecture exactly as it is:

```text
messy parts
violations
weak boundaries
tight coupling
unclear ownership
risky flows
migration blockers
unknowns
```

Forward engineering will decide what to fix later.

This extraction is about truth, evidence, and production-grade architecture understanding.


--- STAGE PROMPT: 04-final-architecture-agent.md ---
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


--- INPUT CONTEXT ---
### evidence-packs/call-flow-pack.json
```
{
  "evidence_pack_type": "call_flow",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/ApplicationCore/ApplicationCore.csproj",
    "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
    "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
    "src/ApplicationCore/Entities/CatalogBrand.cs",
    "src/ApplicationCore/Entities/CatalogItem.cs",
    "src/ApplicationCore/Entities/CatalogType.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
    "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
    "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs",
    "src/ApplicationCore/Exceptions/BasketNotFoundException.cs",
    "src/ApplicationCore/Exceptions/DuplicateException.cs",
    "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs",
    "src/ApplicationCore/Extensions/GuardExtensions.cs",
    "src/ApplicationCore/Interfaces/IBasketService.cs",
    "src/ApplicationCore/Interfaces/IOrderService.cs",
    "src/ApplicationCore/Services/BasketService.cs",
    "src/ApplicationCore/Services/OrderService.cs",
    "src/ApplicationCore/Services/UriComposer.cs",
    "src/ApplicationCore/Specifications/BasketWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterPaginatedSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemNameSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/OrderWithItemsByIdSpec.cs",
    "src/BlazorAdmin/BlazorAdmin.csproj",
    "src/BlazorAdmin/CustomAuthStateProvider.cs",
    "src/BlazorAdmin/Helpers/BlazorComponent.cs",
    "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs",
    "src/BlazorAdmin/Helpers/ToastComponent.cs",
    "src/BlazorAdmin/JavaScript/Cookies.cs",
    "src/BlazorAdmin/JavaScript/Css.cs",
    "src/BlazorAdmin/JavaScript/Route.cs",
    "src/BlazorAdmin/Pages/CatalogItemPage/Create.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Delete.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Details.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/List.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs",
    "src/BlazorAdmin/Pages/Logout.razor",
    "src/BlazorAdmin/Program.cs",
    "src/BlazorAdmin/Services/CacheEntry.cs",
    "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
    "src/BlazorAdmin/Services/CachedCatalogLookupDataServiceDecorator .cs",
    "src/BlazorAdmin/Services/Catalo
... [truncated]
```

### evidence-packs/component-registry-pack.json
```
{
  "evidence_pack_type": "component_registry",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/ApplicationCore/CatalogSettings.cs",
    "src/ApplicationCore/Constants/AuthorizationConstants.cs",
    "src/ApplicationCore/Entities/BaseEntity.cs",
    "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
    "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs",
    "src/ApplicationCore/Entities/CatalogBrand.cs",
    "src/ApplicationCore/Entities/CatalogItem.cs",
    "src/ApplicationCore/Entities/CatalogType.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
    "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
    "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs",
    "src/ApplicationCore/Exceptions/BasketNotFoundException.cs",
    "src/ApplicationCore/Exceptions/DuplicateException.cs",
    "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs",
    "src/ApplicationCore/Extensions/GuardExtensions.cs",
    "src/ApplicationCore/Extensions/JsonExtensions.cs",
    "src/ApplicationCore/Interfaces/IAggregateRoot.cs",
    "src/ApplicationCore/Interfaces/IAppLogger.cs",
    "src/ApplicationCore/Interfaces/IBasketQueryService.cs",
    "src/ApplicationCore/Interfaces/IBasketService.cs",
    "src/ApplicationCore/Interfaces/IEmailSender.cs",
    "src/ApplicationCore/Interfaces/IOrderService.cs",
    "src/ApplicationCore/Interfaces/IReadRepository.cs",
    "src/ApplicationCore/Interfaces/IRepository.cs",
    "src/ApplicationCore/Interfaces/ITokenClaimsService.cs",
    "src/ApplicationCore/Interfaces/IUriComposer.cs",
    "src/ApplicationCore/Services/BasketService.cs",
    "src/ApplicationCore/Services/OrderService.cs",
    "src/ApplicationCore/Services/UriComposer.cs",
    "src/ApplicationCore/Specifications/BasketWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterPaginatedSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemNameSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/OrderWithItemsByIdSpec.cs",
    "src/BlazorAdmin/App.razor",
    "src/BlazorAdmin/CustomAuthStateProvider.cs",
    "src/BlazorAdmin/Helpers/BlazorComponent.cs",
    "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs",
    "src/BlazorAdmin/Helpers/RefreshBroadcast.cs",
    "src/BlazorAdmin/Helpers/ToastComponent.cs",
    "src/BlazorAdmin/JavaScript/Cookies.cs",
 
... [truncated]
```

### evidence-packs/dependency-pack.json
```
{
  "evidence_pack_type": "dependency",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/ApplicationCore/ApplicationCore.csproj",
    "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
    "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
    "src/ApplicationCore/Entities/CatalogBrand.cs",
    "src/ApplicationCore/Entities/CatalogItem.cs",
    "src/ApplicationCore/Entities/CatalogType.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
    "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
    "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs",
    "src/ApplicationCore/Exceptions/BasketNotFoundException.cs",
    "src/ApplicationCore/Exceptions/DuplicateException.cs",
    "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs",
    "src/ApplicationCore/Extensions/GuardExtensions.cs",
    "src/ApplicationCore/Interfaces/IBasketService.cs",
    "src/ApplicationCore/Interfaces/IOrderService.cs",
    "src/ApplicationCore/Services/BasketService.cs",
    "src/ApplicationCore/Services/OrderService.cs",
    "src/ApplicationCore/Services/UriComposer.cs",
    "src/ApplicationCore/Specifications/BasketWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterPaginatedSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemNameSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/OrderWithItemsByIdSpec.cs",
    "src/BlazorAdmin/BlazorAdmin.csproj",
    "src/BlazorAdmin/CustomAuthStateProvider.cs",
    "src/BlazorAdmin/Helpers/BlazorComponent.cs",
    "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs",
    "src/BlazorAdmin/Helpers/ToastComponent.cs",
    "src/BlazorAdmin/JavaScript/Cookies.cs",
    "src/BlazorAdmin/JavaScript/Css.cs",
    "src/BlazorAdmin/JavaScript/Route.cs",
    "src/BlazorAdmin/Pages/CatalogItemPage/Create.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Delete.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Details.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/List.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs",
    "src/BlazorAdmin/Pages/Logout.razor",
    "src/BlazorAdmin/Program.cs",
    "src/BlazorAdmin/Services/CacheEntry.cs",
    "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
    "src/BlazorAdmin/Services/CachedCatalogLookupDataServiceDecorator .cs",
    "src/BlazorAdmin/Services/Catal
... [truncated]
```

### evidence-packs/entry-point-pack.json
```
{
  "evidence_pack_type": "entry_points",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/BlazorAdmin/Pages/CatalogItemPage/List.razor",
    "src/BlazorAdmin/Pages/Logout.razor",
    "src/BlazorAdmin/Program.cs",
    "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
    "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
    "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
    "src/PublicApi/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs",
    "src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs",
    "src/PublicApi/CatalogItemEndpoints/DeleteCatalogItemEndpoint.cs",
    "src/PublicApi/CatalogItemEndpoints/UpdateCatalogItemEndpoint.cs",
    "src/PublicApi/CatalogTypeEndpoints/CatalogTypeListEndpoint.cs",
    "src/PublicApi/Program.cs",
    "src/Web/Areas/Identity/Pages/Account/ConfirmEmail.cshtml",
    "src/Web/Areas/Identity/Pages/Account/Login.cshtml",
    "src/Web/Areas/Identity/Pages/Account/Logout.cshtml",
    "src/Web/Areas/Identity/Pages/Account/Register.cshtml",
    "src/Web/Controllers/ManageController.cs",
    "src/Web/Controllers/OrderController.cs",
    "src/Web/Controllers/UserController.cs",
    "src/Web/Pages/Admin/EditCatalogItem.cshtml",
    "src/Web/Pages/Admin/Index.cshtml",
    "src/Web/Pages/Basket/Checkout.cshtml",
    "src/Web/Pages/Basket/Index.cshtml",
    "src/Web/Pages/Basket/Success.cshtml",
    "src/Web/Pages/Error.cshtml",
    "src/Web/Pages/Index.cshtml",
    "src/Web/Pages/Privacy.cshtml",
    "src/Web/Program.cs"
  ],
  "confidence": 0.836,
  "extracted_facts": {
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
  "entry_points": [
    {
      "entry_point_id": "ENTRY-0001",
      "type": "HTTP_API",
      "method": "POST",
      "path_or_name": "/api/authenticate",
      "owning_component": "AuthenticateEndpoint",
      "owning_module_guess": "Identity",
      "called_service_or_handler": [
        "ITokenClaimsService.GetTokenAsync",
        "SignInManager<ApplicationUser>.PasswordSignInAsync"
      ],
      "source_file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
      "line": 36,
      "confidence": 0.9,
      "evidence": [
        {
          "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
          "line": 36,
          "reason": "ASP.NET HTTP method attribute"
        }
      ],
      "uncertainty": [],
      "parser_strategy": "aspnet_attribute_route_parser",
      "route_action": "HandleAsync",
      "source_chunks": [
        {
          "chunk_id": "CHUNK-000227",
          "file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
          "start_line": 1,
... [truncated]
```

### evidence-packs/external-boundary-pack.json
```
{
  "evidence_pack_type": "external_boundary",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/ApplicationCore/ApplicationCore.csproj",
    "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
    "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
    "src/ApplicationCore/Entities/CatalogBrand.cs",
    "src/ApplicationCore/Entities/CatalogItem.cs",
    "src/ApplicationCore/Entities/CatalogType.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
    "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
    "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs",
    "src/ApplicationCore/Exceptions/BasketNotFoundException.cs",
    "src/ApplicationCore/Exceptions/DuplicateException.cs",
    "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs",
    "src/ApplicationCore/Extensions/GuardExtensions.cs",
    "src/ApplicationCore/Interfaces/IBasketService.cs",
    "src/ApplicationCore/Interfaces/IOrderService.cs",
    "src/ApplicationCore/Services/BasketService.cs",
    "src/ApplicationCore/Services/OrderService.cs",
    "src/ApplicationCore/Services/UriComposer.cs",
    "src/ApplicationCore/Specifications/BasketWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterPaginatedSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemNameSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/OrderWithItemsByIdSpec.cs",
    "src/BlazorAdmin/BlazorAdmin.csproj",
    "src/BlazorAdmin/CustomAuthStateProvider.cs",
    "src/BlazorAdmin/Helpers/BlazorComponent.cs",
    "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs",
    "src/BlazorAdmin/Helpers/ToastComponent.cs",
    "src/BlazorAdmin/JavaScript/Cookies.cs",
    "src/BlazorAdmin/JavaScript/Css.cs",
    "src/BlazorAdmin/JavaScript/Route.cs",
    "src/BlazorAdmin/Pages/CatalogItemPage/Create.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Delete.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Details.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/List.razor",
    "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs",
    "src/BlazorAdmin/Pages/Logout.razor",
    "src/BlazorAdmin/Program.cs",
    "src/BlazorAdmin/Services/CacheEntry.cs",
    "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
    "src/BlazorAdmin/Services/CachedCatalogLookupDataServiceDecorator .cs",
    "src/BlazorAdmin/Service
... [truncated]
```

### evidence-packs/frontend-application-pack.json
```
{
  "evidence_pack_type": "frontend_application",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/ApplicationCore/ApplicationCore.csproj",
    "src/ApplicationCore/CatalogSettings.cs",
    "src/ApplicationCore/Constants/AuthorizationConstants.cs",
    "src/ApplicationCore/Entities/BaseEntity.cs",
    "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
    "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs",
    "src/ApplicationCore/Entities/CatalogBrand.cs",
    "src/ApplicationCore/Entities/CatalogItem.cs",
    "src/ApplicationCore/Entities/CatalogType.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
    "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
    "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs",
    "src/ApplicationCore/Exceptions/BasketNotFoundException.cs",
    "src/ApplicationCore/Exceptions/DuplicateException.cs",
    "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs",
    "src/ApplicationCore/Extensions/GuardExtensions.cs",
    "src/ApplicationCore/Extensions/JsonExtensions.cs",
    "src/ApplicationCore/Interfaces/IAggregateRoot.cs",
    "src/ApplicationCore/Interfaces/IAppLogger.cs",
    "src/ApplicationCore/Interfaces/IBasketQueryService.cs",
    "src/ApplicationCore/Interfaces/IBasketService.cs",
    "src/ApplicationCore/Interfaces/IEmailSender.cs",
    "src/ApplicationCore/Interfaces/IOrderService.cs",
    "src/ApplicationCore/Interfaces/IReadRepository.cs",
    "src/ApplicationCore/Interfaces/IRepository.cs",
    "src/ApplicationCore/Interfaces/ITokenClaimsService.cs",
    "src/ApplicationCore/Interfaces/IUriComposer.cs",
    "src/ApplicationCore/Services/BasketService.cs",
    "src/ApplicationCore/Services/OrderService.cs",
    "src/ApplicationCore/Services/UriComposer.cs",
    "src/ApplicationCore/Specifications/BasketWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterPaginatedSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemNameSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/OrderWithItemsByIdSpec.cs",
    "src/BlazorAdmin/App.razor",
    "src/BlazorAdmin/BlazorAdmin.csproj",
    "src/BlazorAdmin/CustomAuthStateProvider.cs",
    "src/BlazorAdmin/Helpers/BlazorComponent.cs",
    "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs",
    "src/BlazorAdmin/Helpers/RefreshBroadcast.cs",
 
... [truncated]
```

### evidence-packs/layering-pattern-pack.json
```
{
  "evidence_pack_type": "layering_pattern",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/ApplicationCore/CatalogSettings.cs",
    "src/ApplicationCore/Constants/AuthorizationConstants.cs",
    "src/ApplicationCore/Entities/BaseEntity.cs",
    "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
    "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs",
    "src/ApplicationCore/Entities/CatalogBrand.cs",
    "src/ApplicationCore/Entities/CatalogItem.cs",
    "src/ApplicationCore/Entities/CatalogType.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
    "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
    "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs",
    "src/ApplicationCore/Exceptions/BasketNotFoundException.cs",
    "src/ApplicationCore/Exceptions/DuplicateException.cs",
    "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs",
    "src/ApplicationCore/Extensions/GuardExtensions.cs",
    "src/ApplicationCore/Extensions/JsonExtensions.cs",
    "src/ApplicationCore/Interfaces/IAggregateRoot.cs",
    "src/ApplicationCore/Interfaces/IAppLogger.cs",
    "src/ApplicationCore/Interfaces/IBasketQueryService.cs",
    "src/ApplicationCore/Interfaces/IBasketService.cs",
    "src/ApplicationCore/Interfaces/IEmailSender.cs",
    "src/ApplicationCore/Interfaces/IOrderService.cs",
    "src/ApplicationCore/Interfaces/IReadRepository.cs",
    "src/ApplicationCore/Interfaces/IRepository.cs",
    "src/ApplicationCore/Interfaces/ITokenClaimsService.cs",
    "src/ApplicationCore/Interfaces/IUriComposer.cs",
    "src/ApplicationCore/Services/BasketService.cs",
    "src/ApplicationCore/Services/OrderService.cs",
    "src/ApplicationCore/Services/UriComposer.cs",
    "src/ApplicationCore/Specifications/BasketWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterPaginatedSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemNameSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/OrderWithItemsByIdSpec.cs",
    "src/BlazorAdmin/App.razor",
    "src/BlazorAdmin/CustomAuthStateProvider.cs",
    "src/BlazorAdmin/Helpers/BlazorComponent.cs",
    "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs",
    "src/BlazorAdmin/Helpers/RefreshBroadcast.cs",
    "src/BlazorAdmin/Helpers/ToastComponent.cs",
    "src/BlazorAdmin/JavaScript/Cookies.cs",
   
... [truncated]
```

### evidence-packs/module-boundary-pack.json
```
{
  "evidence_pack_type": "module_boundary",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/ApplicationCore/CatalogSettings.cs",
    "src/ApplicationCore/Constants/AuthorizationConstants.cs",
    "src/ApplicationCore/Entities/BaseEntity.cs",
    "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
    "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
    "src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs",
    "src/ApplicationCore/Entities/CatalogBrand.cs",
    "src/ApplicationCore/Entities/CatalogItem.cs",
    "src/ApplicationCore/Entities/CatalogType.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
    "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
    "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
    "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs",
    "src/ApplicationCore/Exceptions/BasketNotFoundException.cs",
    "src/ApplicationCore/Exceptions/DuplicateException.cs",
    "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs",
    "src/ApplicationCore/Extensions/GuardExtensions.cs",
    "src/ApplicationCore/Extensions/JsonExtensions.cs",
    "src/ApplicationCore/Interfaces/IAggregateRoot.cs",
    "src/ApplicationCore/Interfaces/IAppLogger.cs",
    "src/ApplicationCore/Interfaces/IBasketQueryService.cs",
    "src/ApplicationCore/Interfaces/IBasketService.cs",
    "src/ApplicationCore/Interfaces/IEmailSender.cs",
    "src/ApplicationCore/Interfaces/IOrderService.cs",
    "src/ApplicationCore/Interfaces/IReadRepository.cs",
    "src/ApplicationCore/Interfaces/IRepository.cs",
    "src/ApplicationCore/Interfaces/ITokenClaimsService.cs",
    "src/ApplicationCore/Interfaces/IUriComposer.cs",
    "src/ApplicationCore/Services/BasketService.cs",
    "src/ApplicationCore/Services/OrderService.cs",
    "src/ApplicationCore/Services/UriComposer.cs",
    "src/ApplicationCore/Specifications/BasketWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterPaginatedSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogFilterSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemNameSpecification.cs",
    "src/ApplicationCore/Specifications/CatalogItemsSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersSpecification.cs",
    "src/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs",
    "src/ApplicationCore/Specifications/OrderWithItemsByIdSpec.cs",
    "src/BlazorAdmin/App.razor",
    "src/BlazorAdmin/CustomAuthStateProvider.cs",
    "src/BlazorAdmin/Helpers/BlazorComponent.cs",
    "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs",
    "src/BlazorAdmin/Helpers/RefreshBroadcast.cs",
    "src/BlazorAdmin/Helpers/ToastComponent.cs",
    "src/BlazorAdmin/JavaScript/Cookies.cs",
    
... [truncated]
```

### evidence-packs/system-inventory-pack.json
```
{
  "evidence_pack_type": "system_inventory",
  "generated_at": "2026-06-15T07:27:15+00:00",
  "generator_version": "0.1.0",
  "source_artifacts_used": [
    "architecture-output/inventory/file-inventory.json",
    "architecture-output/inventory/project-inventory.json",
    "architecture-output/inventory/language-summary.json",
    "architecture-output/inventory/ignored-files-report.json",
    "architecture-output/parsed/symbol-registry.json",
    "architecture-output/parsed/route-registry.json",
    "architecture-output/parsed/dependency-candidates.json",
    "architecture-output/parsed/entry-point-candidates.json",
    "architecture-output/parsed/roslyn-semantic-facts.json"
  ],
  "source_files_used": [
    "src/ApplicationCore/ApplicationCore.csproj",
    "src/BlazorAdmin/BlazorAdmin.csproj",
    "src/BlazorShared/BlazorShared.csproj",
    "src/Infrastructure/Infrastructure.csproj",
    "src/PublicApi/PublicApi.csproj",
    "src/Web/Web.csproj",
    "tests/FunctionalTests/FunctionalTests.csproj",
    "tests/IntegrationTests/IntegrationTests.csproj",
    "tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj",
    "tests/UnitTests/UnitTests.csproj"
  ],
  "confidence": 0.882,
  "system_name": "unknown",
  "extracted_facts": {
    "solution_count": 2,
    "project_count": 10,
    "deployable_unit_count": 2,
    "docker_compose_service_count": 3
  },
  "backend_projects": [
    {
      "name": "PublicApi",
      "path": "src/PublicApi/PublicApi.csproj",
      "project_kind": "dotnet",
      "type": "backend_web_api",
      "category": "backend",
      "framework": ".NET (Microsoft.NET.Sdk.Web; target=unknown)",
      "deployable": true,
      "source_path": "src/PublicApi",
      "framework_indicators": [
        "backend_indicator:Microsoft.NET.Sdk.Web",
        "config_file:appsettings.json",
        "deployment_file:Dockerfile",
        "entry_point_file:Program.cs",
        "sdk:Microsoft.NET.Sdk.Web"
      ],
      "package_references": [
        "Ardalis.ApiEndpoints",
        "AutoMapper.Extensions.Microsoft.DependencyInjection",
        "Microsoft.AspNetCore.Authentication.JwtBearer",
        "Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore",
        "Microsoft.AspNetCore.Identity.EntityFrameworkCore",
        "Microsoft.AspNetCore.Identity.UI",
        "Microsoft.EntityFrameworkCore.InMemory",
        "Microsoft.EntityFrameworkCore.SqlServer",
        "Microsoft.EntityFrameworkCore.Tools",
        "Microsoft.VisualStudio.Azure.Containers.Tools.Targets",
        "Microsoft.VisualStudio.Web.CodeGeneration.Design",
        "MinimalApi.Endpoint",
        "Swashbuckle.AspNetCore",
        "Swashbuckle.AspNetCore.Annotations",
        "Swashbuckle.AspNetCore.SwaggerUI",
        "System.IdentityModel.Tokens.Jwt"
      ],
      "project_references": [
        "../ApplicationCore/ApplicationCore.csproj",
        "../Infrastructure/Infrastructure.csproj"
      ],
      "evidence": [
        {
          "file": "src/PublicApi/PublicApi.csproj",
          "reason": ".NET project file using Microsoft.NET.Sdk.Web; deployable inferred from web SDK"
        }
      ],
      "confidence": 0.92
    },
    {
      "name": "Web",
      "path": "src/Web/Web.csproj",
      "project_kind": "dotnet",
      "type": "backend_web_app",
      "category": "backend",
      "framework": ".NET (Microsoft.NET.Sdk.Web; target=unknown)",
      "deployable": true,
      "source_path": "src/Web",
      "framework_indicators": [
        "backend_ind
... [truncated]
```


---

## Reminder on output format
Please output all files from this stage's Output section, each wrapped in
===AA_FILE_START:<path>=== / ===AA_FILE_END=== markers. I'll be parsing your
response for these exact markers, so please give full file contents rather
than descriptions.
