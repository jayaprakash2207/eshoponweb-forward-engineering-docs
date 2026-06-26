# Pilot Cutover Validation — Method & Scope

**Pilot project:** eShopOnWeb
**Date:** 2026-06-24
**Reviewer roles:** Enterprise Architect · TOGAF Reviewer · Prompt Architect · QA Lead · Migration Validation Specialist
**Authority:** `../prompt-governance/`, `../prompt-refactored/`, `../prompt-resolved/`, `../migration-output/`, `../FINAL_PROMPT_CONFORMANCE_REPORT.md`, `../enterprise-foundation-package/`

---

## 1. Critical scoping statement (read first)

This is a **production cutover validation**, not a forward-engineering or code task. One hard constraint
shapes the method:

> **The pipeline cannot be *executed* in this environment.** Running BA/DA/AA/TA/FN end-to-end requires
> invoking the `claude` CLI runners and live model calls — that is code execution, explicitly out of
> scope, and fabricating model output would invalidate the validation.

Therefore this pilot is an **evidence-grounded contract validation**, which is the correct and honest
form of validation available:

- **Legacy outputs are real.** The legacy pipeline already ran on eShopOnWeb; its actual artifacts exist
  in `output/eShopOnWeb/` (BA 10 docs, DA 14 files, TA 16 files, AA ~33 files, `layer2_output.json`) and
  the reconciled result exists in `enterprise-foundation-package/ENTERPRISE_KNOWLEDGE_GRAPH.json`
  (**274 nodes**). These are the legacy baseline.
- **Resolved-prompt outputs are validated at the contract level.** Each resolved prompt's §7 Outputs
  declares the exact files, markers, and schemas it emits. We compare the legacy artifact set against the
  resolved output **contracts** + the four governed relocations, and verify knowledge preservation by
  mapping every legacy node to its post-cutover owner.

What this **can** prove: output compatibility, knowledge preservation (node coverage), ownership/boundary
compliance, governance/confidence compliance, no-duplicate-extraction, no-unresolved-references, graph
integrity, traceability. What it **cannot** prove without a live run: exact token-level wording parity of
regenerated prose (addressed as a residual risk + the G3 live-pilot recommendation).

## 2. Baseline inventory (legacy, real)

| Layer | Legacy artifacts (count) | Location |
|---|---|---|
| Business | 10 docs + `layer2_output.json` | `output/eShopOnWeb/ba_documents/`, root |
| Data | 14 files (incl. `review-summary.md`) | `output/eShopOnWeb/da-outputs/` |
| Technology | 10 in `ta-outputs/` + 6 in `ta_agent1/` | `output/eShopOnWeb/ta-outputs/` |
| Application | ~33 files | `output/eShopOnWeb/aa-outputs/final/` + `llm-stages/final/` |
| Foundation | graph (274 nodes) + 4 views | `enterprise-foundation-package/` |

Knowledge graph node distribution (real, parsed):
`business 54 (capabilities 39, actors 5, processes 10)` · `data 35 (entities 15, relationships 12,
aggregates 4, repositories 4)` · `application 134 (services 47, interfaces 13, apis 55, dependencies 19)`
· `technology 51 (stack 26, infra 8, security 17)` · `cross_links 117` · `assumptions 7` · `open_questions 9`.

## 3. Reports in this package

| # | Report |
|---|---|
| 01 | `01_OUTPUT_COMPARISON_REPORT.md` |
| 02 | `02_KNOWLEDGE_GRAPH_DIFF_REPORT.md` |
| 03 | `03_REGRESSION_ANALYSIS.md` |
| 04 | `04_GOVERNANCE_COMPLIANCE_REPORT.md` |
| 05 | `05_CUTOVER_DECISION_REPORT.md` |
| 06 | `06_EXECUTIVE_SUMMARY.md` |

## 4. Validation type label

Every score in this package is a **contract/coverage validation** score, not a live-run diff. This is
stated wherever a number appears so the decision-maker is not misled. The live end-to-end pilot remains
the recommended final gate (G3) and is called out in report 05.
