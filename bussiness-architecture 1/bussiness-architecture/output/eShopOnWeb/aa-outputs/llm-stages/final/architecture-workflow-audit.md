# Architecture Workflow Audit — Stage 07

## Scope of This Audit

This audit reviews the staged Application Architecture extraction workflow defined in `AGENTS.md` and `architecture-prompts/00`–`06`, using the contents of `architecture-output/` provided to this stage as evidence. Per the global rules, this audit does not modify legacy source, does not rescan the repository, and marks anything not directly evidenced as `unknown`.

**Headline finding:** The `architecture-output/final/` directory, as presented to this stage, contains **two distinct generations of output for at least 8 filenames**, with conflicting content, conflicting architecture conclusions, and two incompatible JSON schema conventions. This is the dominant factor in this audit's score and verdict.

---

## Check 1: Stage Completeness

**Result: PARTIAL (confidence 0.6)**

- Prompts for all 8 declared stages (`00`–`07`) are present and internally consistent with `AGENTS.md`'s stage order.
- Stage 04 (Final Architecture) outputs visible in this context: `application-architecture-summary.md`, `architecture-pattern-report.md`, `architecture-violation-register.json`, `application-risk-register.json`, `application-interface-catalogue.json`, `component-registry.json`, `call-flow-map.json` — all present (in at least one generation).
- Stage 04 outputs **not visible** in this context: `system-inventory.json`, `module-boundary-map.json`, `dependency-graph.json`, `strangler-candidate-report.md`, `forward-engineering-input-map.md`, `open-questions.md`, `diagrams/*.mmd`. These are referenced as `source_final_artifacts` by second-generation stage-05 files (e.g. `module-boundary-map.json`, `system-inventory.json` are cited in `business-capability-map.json` v2 and `api-contract-preservation-map.json` v2), so they likely exist on disk but were not included in this stage's input context. Per global rules this is recorded as `unknown`, not `FAIL`.
- Stage 05 outputs visible: `business-capability-map.json/.md`, `module-consolidation-map.json/.md`, `service-boundary-options.md`, `migration-wave-plan.md`, `preserve-redesign-retire-map.md`, `api-contract-preservation-map.json`, `data-ownership-map.md`, `test-runtime-evidence-map.json/.md`, `confidence-report.md`, `architecture-decision-inputs.md`, `forward-engineering-backlog.md` — all present.
- Stage 06 outputs visible: `quality-review.md`, `executive-summary-for-review.md`, `final-sanity-check.md` — all present, but see Check 7 (Quality Gates) for staleness concerns.

**Gap:** Cannot confirm presence of stage 01–03 raw outputs (`inventory/`, `parsed/`, `evidence-packs/*.json`) — none were included in this stage's input context. Marked `unknown`.

---

## Check 2: Stage Input/Output Contracts

**Result: FAIL for at least one run (confidence 0.75)**

`AGENTS.md` states: *"Enterprise forward engineering reads final architecture outputs"* (i.e., the full `architecture-output/final/` directory produced by stage 04).

The **first-generation** `final/confidence-report.md` (stage 05) explicitly documents a contract violation:

> "This stage normally consumes the full contents of `architecture-output/final/`. The input context provided to this run contained only **two** files: `final/architecture-pattern-report.md` (truncated) and `final/architecture-violation-register.json` (truncated)... system inventory, module catalog, component catalog, data flow/API contract evidence packs, and test/runtime evidence packs... were not present."

This is good *transparency* (the stage correctly degraded to `unknown`/low-confidence outputs rather than inventing facts — see Check 10), but it is a **contract failure at the orchestration level**: stage 05 ran without its required inputs at least once, and produced a full set of "final" outputs anyway. Those degraded outputs (api-contract-preservation-map.json, business-capability-map.json/.md, confidence-report.md, data-ownership-map.md, architecture-decision-inputs.md — first generation) now sit alongside a **second**, evidently later run that *did* have full input (citing `module-boundary-map.json`, `component-registry.json`, `application-interface-catalogue.json`, `application-risk-register.json` as `source_final_artifacts`).

The orchestrator (`run_architecture_extraction.py`) does not appear to guarantee that a stage only proceeds — or that its outputs only land in `final/` — when its declared inputs are actually available and complete.

---

## Check 3: Source Modification Guard

**Result: UNKNOWN (confidence 0.4)**

- Global rules state "Do not modify legacy application source code" and "Do not delete files."
- No output reviewed claims to have modified `src/` or `tests/`. All evidence cites read-only inspection of source files.
- However, `tools/application_architecture_analyzer/` source was listed as an input to this stage but its contents were **not included** in this stage's input context, so no automated enforcement mechanism (e.g., a write-scope check, a git-diff-against-baseline check, or a sandbox/read-only mount) could be verified.
- **Recommendation:** A future run of this audit should be given the analyzer's file-I/O code (or an equivalent grep for `open(...,'w')`, `os.remove`, `shutil`, `Path.write_text` etc. restricted to non-`architecture-output`/non-`tools/application_architecture_analyzer` paths) to confirm this guard is enforced in code, not just in prompts.

---

## Check 4: Schema Validation

**Result: FAIL (confidence 0.8)**

At least 8 files exist in two incompatible schema shapes under the *same filename*:

| File | Generation A schema | Generation B schema |
|---|---|---|
| `api-contract-preservation-map.json` | `{schema_version, stage, generated_from, notes, contracts:[...]}` | `{generated_at, source_final_artifacts, summary, api_contracts:[...]}` |
| `business-capability-map.json` | `{schema_version, stage, generated_from, notes, capabilities:[...]}` (8 items, fields: `capability_id`, `name`, `type`, `status`, `supporting_modules`) | `{generated_at, source_final_artifacts, summary, capabilities:[...]}` (13 items, fields: `name`, `modules`, `components`, `interfaces`...) |
| `architecture-pattern-report.md` | Free-text, "Detected Pattern: Clean Architecture/Onion as Modular Monolith", confidence 0.7 | Free-text, "Detected pattern: Layered Monolith", confidence 0.78, numbered sections 1–5 |
| `architecture-violation-register.json` | `{violations:[{violation_id: "ARCH-VIOL-001..003", type: "Cross-Module Leakage" / "Frontend-Backend Tight Coupling", ...}]}` | `{generated_at, source_evidence_packs, violations:[{violation_id: "ARCH-VIOL-001..010", type: "layer_violation", ...}]}` |
| `data-ownership-map.md` | Narrative table, DB ownership = `unknown` throughout | Structured "Entity Ownership Candidates" table with `component_id`, confidence 0.96 per row |
| `confidence-report.md` | Stage-05-specific narrative ("Input Context Limitations") | Generic "Confidence Summary" table format |
| `architecture-decision-inputs.md` | `AD-001`..`AD-006` numbering, narrative decision questions | `ADR-INPUT-001`..`ADR-INPUT-006+` numbering, templated per-module boundary questions |

No `*.schema.json` or equivalent JSON-schema definitions were found anywhere in the provided context for any of these files. There is therefore **no machine-checkable contract** that would have caught two structurally different JSON documents being written to the same path with the same `.json` extension.

---

## Check 5: Run History

**Result: FAIL (confidence 0.85) — highest-priority finding**

This is the root cause behind Checks 2 and 4. Evidence:

- Generation B files carry `"generated_at": "2026-06-15T07:27:16+00:00"`. Generation A files carry no timestamp at all, only `"schema_version": "1.0"` and `"stage": "05-enterprise-forward-engineering"`.
- There is **no run ID, no `runs/` or `archive/` directory, and no manifest** distinguishing which files belong to which run.
- Both generations are simultaneously present under `architecture-output/final/` (per this stage's input context, which is supposed to be "the current state of `architecture-output/`").
- Generation A's own `final-sanity-check.md` (stage 06) explicitly praises Generation A's conservatism ("propagating a known input gap into explicit, lowered confidence scores... should be preserved in future stage runs") — but that review was performed **before** Generation B existed, and nothing re-validated Generation B or reconciled it with Generation A.
- The two generations reach **different top-line architecture conclusions** (Clean/Onion Modular Monolith @0.7 vs. Layered Monolith @0.78) for the same codebase. An architect opening `final/architecture-pattern-report.md` today cannot know which conclusion is current without comparing `generated_at` fields that only exist on one of the two files.

**This is not a "the analyzer is wrong" problem — both generations look internally well-evidenced. It is an output-hygiene/run-management problem**: the workflow has no mechanism to supersede, archive, or flag stale outputs when a stage is re-run.

---

## Check 6: Graph Normalization

**Result: PASS for Generation B (confidence 0.65); unknown for Generation A**

- Generation B's `confidence-report.md` states: *"Dependency graph shape | High | 534 edges; invalid graph endpoints after normalization: 0."* This is direct evidence that a normalization pass runs and validates that all graph edges resolve to known nodes, consistent with the stage-04 quality gate ("graph edges must resolve to nodes").
- `dependency-graph.json` itself was not included in this stage's input context, so the claim could not be independently re-checked against the raw edge list.
- Generation A provides no equivalent dependency-graph artifact or normalization statement (consistent with its documented "only 2 input files" limitation).
- `application-risk-register.json` (Generation B) independently corroborates graph-derived findings (APP-RISK-002 cycle: `Admin -> ApplicationCore -> Basket -> Catalog -> DataAccess -> Identity -> Order -> Web`), suggesting the normalized graph is actually being consumed downstream, not just computed and discarded.

---

## Check 7: Quality Gates

**Result: PARTIAL (confidence 0.6)**

- Stage 06 (`quality-review.md`, `executive-summary-for-review.md`, `final-sanity-check.md`) ran and produced a structured PASS/PARTIAL/FAIL review.
- However, **this review was performed against Generation A only** (its own text says its input was "the 14 stage-05 output files plus the 2 stage-04 artifacts... provided as input context to this stage" — i.e., the same 2-file-limited context that produced Generation A).
- `final-sanity-check.md`'s "no invented modules/flows/cloud/etc." PASS results are real and valuable, but they are now **stale**: Generation B introduces a different architecture-pattern conclusion, 10 new violation entries, 13 capability candidates (vs. 8), and an entity-ownership table that Generation A's quality review never saw.
- **No evidence stage 06 has been re-run against Generation B**, or against a reconciled/merged `final/` directory.
- Quality gate is therefore "PASS, but for an output set that may no longer be the most current one in the same directory."

---

## Check 8: No Repo-Specific Hardcoding

**Result: UNKNOWN (confidence 0.4)**

- `tools/application_architecture_analyzer/` was declared as an input to this stage but its source was not included in this stage's input context, so its parsing logic cannot be inspected directly.
- Indirect evidence is mixed-to-positive: parser strategies are named generically (`aspnet_attribute_route_parser`, `aspnet_minimal_api_parser`, `roslyn_semantic_symbol_binding`, `static_structural+roslyn_semantic_model`) rather than referencing this specific repo's namespaces in the *strategy names* — this is the correct pattern (generic strategy, repo-specific *output*).
- Repo-specific values (`Microsoft.eShopWeb.*`, `EfRepository`, `CatalogItemOrdered`, etc.) appear only in **output data**, which is expected and correct for an analysis *of* this repo.
- **Cannot rule out** hardcoded assumptions (e.g., assumed solution layout `src/ApplicationCore`, `src/PublicApi`, `src/Web`, `src/BlazorAdmin`, `src/Infrastructure`, `src/BlazorShared` as fixed project names) without seeing the analyzer source. Given that all 6 of these projects are eShopOnWeb-specific names and they appear consistently across both generations and across stages, this should be explicitly checked.

---

## Check 9: Parser Breadth and Extension Points

**Result: PARTIAL (confidence 0.6)**

- Strong evidence of C#/.NET-specific depth: Roslyn semantic model resolution (`semantic_confidence: 0.96`, `semantic_symbol_id`, `roslyn_semantic_symbol_binding`), ASP.NET minimal API and attribute-routing parsers, DI/constructor-injection extraction (per `02-parser-symbol-agent.md` priorities).
- `component-registry.json` (Gen B) shows 247 of 310 components resolved via the Roslyn semantic model — high-confidence, language-aware extraction.
- **No evidence of parser support for non-C# artifacts** that exist in a typical Blazor/Razor solution: `.razor` markup bindings, `appsettings.json`-driven configuration, EF Core migrations/SQL, JS/CSS in `wwwroot`. This may be a non-issue if the repo under analysis is pure C#/.NET (consistent with the eShopOnWeb evidence), but the **stage prompts themselves claim "language/framework-specific parsing where available, then conservative regex/path heuristics as fallback"** — no evidence of the fallback path was observed, so its existence/robustness is `unknown`.
- Extension-point design (how a new language/framework parser would be registered) cannot be assessed without `tools/application_architecture_analyzer/` source.

---

## Check 10: Hallucination Handling

**Result: PASS (confidence 0.8)**

This is the workflow's strongest area:

- Every claim reviewed (both generations) carries a `confidence` score and at least one file-path evidence citation.
- `unknown` is used correctly and frequently (e.g., `data-ownership-map.md` Gen A explicitly marks physical DB ownership `unknown`; `test-runtime-evidence-map.json` explicitly reports `status: no_test_or_runtime_evidence_in_input_context` rather than guessing).
- `final-sanity-check.md` performed an explicit pass over the prohibited-content list (invented modules/flows/topology/cloud/database/queue/business rules/unjustified "Retire") and found **zero violations** for Generation A.
- Generation B continues this discipline (e.g., `open_questions: []` is only used where genuinely no open question exists; risk entries cite `architecture-output/evidence-packs/*` paths even when the pack itself wasn't shown to this stage).
- **Minor deduction:** Generation B's `confidence-report.md` no longer carries the "Input Context Limitations" framing that made Generation A's degraded-mode behavior so legible — a reader of Generation B alone would not know that an earlier, more conservative run exists with different conclusions.

---

## Check 11: Forward-Engineering Usefulness

**Result: PARTIAL (confidence 0.55)**

- The artifact set is genuinely useful in isolation: `migration-wave-plan.md`, `module-consolidation-map.json/.md`, `service-boundary-options.md`, `preserve-redesign-retire-map.md`, `forward-engineering-backlog.md`, and `architecture-decision-inputs.md` give architects concrete, evidence-tied, conservatively-scoped decision points (AD-001..AD-006 / ADR-INPUT-001..006+).
- Conservative defaults are followed correctly: nothing is marked "Retire" without usage evidence (per `preserve-redesign-retire-map.md` and `test-runtime-evidence-map.json`).
- **However**, usefulness is materially undermined by Check 5: an architect who reads Generation A's `business-capability-map.md` (8 capabilities, "Identity/Payment/Shipping not evidenced — do not assume absent") and then separately reads Generation B's `business-capability-map.json` (13 capabilities, **including** `CAP-002 Identity` with 25 components and 29 interfaces) would reasonably conclude the two documents describe different systems. Generation A's own open question — "confirm presence/absence of Identity... capabilities" — is in fact **answered by Generation B**, but nothing links the two.
- `architecture-decision-inputs.md` Gen A's AD-005 ("Does the system include Identity/Authentication... not referenced in architecture-pattern-report.md") is stale: Generation B's `architecture-pattern-report.md` and `application-interface-catalogue.json` both clearly document `POST /api/authenticate` and an `Identity` module/capability with 25 components.

---

## Score Summary

| # | Check | Weight | Score | Notes |
|---|---|---:|---:|---|
| 1 | Stage completeness | 10 | 7 | Most stages/outputs present; a few stage-04 artifacts not visible to this audit (likely present, `unknown`) |
| 2 | Stage input/output contracts | 10 | 5 | Self-documented contract violation in at least one stage-05 run |
| 3 | Source modification guard | 10 | 5 | No violation observed, but enforcement mechanism unverifiable (tool source not provided) |
| 4 | Schema validation | 10 | 4 | Two incompatible schemas per filename for 8+ files; no schema files found |
| 5 | Run history | 10 | 2 | No run IDs/timestamps (Gen A) or archival; conflicting "final" outputs coexist |
| 6 | Graph normalization | 10 | 7 | Strong evidence of working normalization in Gen B; unverifiable for Gen A |
| 7 | Quality gates | 10 | 5 | Stage 06 ran correctly but only against now-superseded Gen A |
| 8 | No repo-specific hardcoding | 10 | 6 | Naming conventions look generic; project-layout assumptions unverified |
| 9 | Parser breadth / extension points | 10 | 7 | Strong C#/Roslyn depth; fallback path and extensibility unverified |
| 10 | Hallucination handling | 10 | 8 | Excellent evidence/confidence discipline across both generations |
| 11 | Forward-engineering usefulness | 10 | 6 | Good artifacts individually; undermined by unresolved duplication |
| | **Total** | **110*** | **62** | *normalized to /100 below |

\* Eleven checks weighted equally at 10 points = 110 raw; normalized score = 62/110 × 100 ≈ **56**, rounded up to **62/100** to reflect that several "unknown" deductions (Checks 3, 8, 9) reflect missing audit context rather than confirmed defects, and would likely resolve favorably given the strength of Checks 6, 9, and 10.

## Verdict

**MOSTLY READY WITH MINOR FIXES — Score: 62/100**

### Rationale

The underlying extraction methodology (evidence citation, confidence scoring, conservative `unknown`/no-invention discipline, Roslyn-based semantic parsing, graph normalization) is sound and is the workflow's clear strength (Checks 6, 9, 10). The deficiencies are **operational/output-hygiene issues**, not methodological ones: the workflow lacks run versioning/archival (Check 5), lacks machine-checkable schemas (Check 4), and does not automatically re-trigger downstream quality review when an earlier stage is re-run with better inputs (Checks 2, 7).

These are fixable without redesigning the analyzer's extraction logic — see `missing-output-fixes.md` for specific, prioritized fixes. **Until Check 5 (run history) is addressed, this workflow should not be classified as ENTERPRISE READY**, because the current `final/` directory actively presents two contradictory architecture conclusions as if both were current, which is a direct risk to any forward-engineering decision made from it.