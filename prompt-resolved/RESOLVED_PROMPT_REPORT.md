# Resolved Prompt Report

**Package:** `prompt-resolved/`
**Date:** 2026-06-24
**Source:** `../prompt-refactored/` (governed prompts) + `../prompt-refactored/_shared/` (components)
**Authority:** `../prompt-governance/` (GOV-01…GOV-10); `../FINAL_PROMPT_CONFORMANCE_REPORT.md`
**Status:** Production-ready. Every `{{include}}` resolved. **No code. No forward-engineering artifacts. Originals unmodified.**

---

## 1. Purpose

This package closes **Gate G1** from the cutover (`../migration-output/REGRESSION_RISK_REPORT.md`
R-CRIT-1): the governed prompts referenced governance via `{{include: CMP-*}}` directives that a runner
would otherwise send to the model literally. Here every include is **materialized into full text**, so
each prompt is directly executable by its runner with no resolver in the loop.

## 2. What was resolved

The five reusable components (GOV-09) were inlined into each prompt at their declared positions:

| Component | Resolves | Lands in section |
|---|---|---|
| CMP-GOV | GOV-01 governance header + the prompt's one role line | §11 |
| CMP-EVID | GR-2/GR-3 evidence hierarchy + the prompt's live_source line | §11 |
| CMP-OUT | GR-8 output discipline + marker + the prompt's audience line | §11 |
| CMP-CONF | GOV-04 confidence model + the prompt's material_nodes | §9 |
| CMP-VALID | GR-7 validation gates + the prompt's outputs list | §8 |

Resolution rule applied (deterministic): each include line → the component's block with parameters
substituted; for parameterized **selector** lines (role / live_source / audience) only the **matching**
line is retained, the others dropped. Fence lines retained as visual block delimiters. All other prompt
text (metadata §1, §2–7, §10, §12) preserved **verbatim**.

## 3. Package contents (22 resolved prompts)

```
prompt-resolved/
├── business-architecture/    BA-SCOUT-01, BA-ANALYST-01, BA-ANALYST-02, BA-ANALYST-03
├── data-architecture/        DA-SCOUT-01, DA-ANALYST-01, DA-REVIEW-01
├── application-architecture/ AGENTS, AA-ANALYST-00, AA-SCOUT-01, AA-SCOUT-02,
│                             AA-ANALYST-03, AA-ANALYST-04, AA-ANALYST-05, AA-ANALYST-06,
│                             AA-REVIEW-06, AA-REVIEW-07
├── technology-architecture/  TA-SCOUT-01, TA-ANALYST-01
└── foundation/               FN-SYNTH-01, FN-SYNTH-02, FN-REVIEW-01
```

(`_archived/` pointers from `prompt-refactored/` are not re-materialized; they carry no includes.)

## 4. Per-prompt resolution summary

| Prompt | role | live_source | marker | audience | material_nodes (CMP-CONF) | Includes resolved |
|---|---|---|---|---|---|---|
| BA-SCOUT-01 | Scout | false | DOCUMENT | business | domain, capability, role | 5 |
| BA-ANALYST-01 | Analyst | false | DOCUMENT | business | capability, business-rule, value-stream | 5 |
| BA-ANALYST-02 | Analyst | false | JSON | business | business-rule, capability_candidate | 5 |
| BA-ANALYST-03 | Analyst | false | DOCUMENT | business | capability, business-rule | 5 |
| DA-SCOUT-01 | Scout | **true** | DA_FILE | technical | entity, data-store, pii-field | 5 |
| DA-ANALYST-01 | Analyst | **true** | DA_FILE | technical | entity, data-store, data-owner, pii-field | 5 |
| DA-REVIEW-01 | Review | **true** | DA_FILE | technical | entity, data-store, pii-field, data-owner | 5 |
| TA-SCOUT-01 | Scout | false | TA_FILE | technical | technology, data-store, integration | 5 |
| TA-ANALYST-01 | Analyst | false | TA_FILE | technical | technology, nfr, technical-debt, security-control | 5 |
| AA-ANALYST-00 | Analyst | false | AA_FILE | technical | component, interface, dependency, architecture-pattern | 5 |
| AA-SCOUT-01 | Scout | false | AA_FILE | technical | project, deployable | 5 |
| AA-SCOUT-02 | Scout | false | AA_FILE | technical | symbol, route, entry-point | 5 |
| AA-ANALYST-03 | Analyst | false | AA_FILE | technical | module-boundary, component, call-flow | 5 |
| AA-ANALYST-04 | Analyst | false | AA_FILE | technical | component, interface, dependency, architecture-pattern, violation | 5 |
| AA-ANALYST-05 | Analyst | false | AA_FILE | technical | service-boundary, migration-wave, api-contract | 5 |
| AA-ANALYST-06 | Analyst | false | AA_FILE | technical | security-control, endpoint-exposure | 5 |
| AA-REVIEW-06 | Review | false | AA_FILE | technical | component, dependency, violation | 5 |
| AA-REVIEW-07 | Review | false | AA_FILE | technical | stage-contract, schema | 5 |
| FN-SYNTH-01 | Synthesis | false | FN_FILE | technical | capability, entity, component, technology, security-control | 5 |
| FN-SYNTH-02 | Synthesis | false | FN_FILE | technical | capability, entity, service, api | 5 |
| FN-REVIEW-01 | Review | false | FN_FILE | technical | capability, entity, component, technology, security-control | 5 |
| AGENTS.md (orch) | Analyst | — | — | — | — | 1 (CMP-GOV only) |

**Total includes resolved:** 21 prompts × 5 + AGENTS × 1 = **106**.

## 5. Preservation guarantees

| Preserved | How verified |
|---|---|
| Prompt metadata (§1) | Verbatim copy; spot-checked DA-REVIEW-01, BA-SCOUT-01 metadata identical to source. |
| Ownership (`owner_layer`, `produces`) | Untouched §1/§4/§7; no relocation changed. |
| Outputs (filenames + markers) | CMP-OUT resolved to the exact legacy marker per prompt (DOCUMENT/AA_FILE/TA_FILE/DA_FILE/JSON/FN_FILE). |
| Compatibility | Marker substitution preserves downstream parser contracts (e.g. `===DOCUMENT_START===`). |
| Confidence model | CMP-CONF resolved to GOV-04 only; no legacy numeric/categorical scale reintroduced. |

## 6. Production-readiness statement

All 22 resolved prompts are **directly runner-executable**: a runner can read the resolved `.md` and send
it to the model with no include-resolution step. See `INCLUDE_RESOLUTION_REPORT.md` for the mechanical
proof of completeness and `CUTOVER_READINESS_REPORT.md` for the gate status this unblocks.
