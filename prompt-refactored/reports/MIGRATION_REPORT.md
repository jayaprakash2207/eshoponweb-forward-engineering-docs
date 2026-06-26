# Migration Report

**Package:** `prompt-refactored/`
**Date:** 2026-06-24
**Authority:** `../../prompt-governance/` (GOV-01 … GOV-10); audit `../../PROMPT_AUDIT_REPORT.md`
**Scope:** Execution of the GOV-06 migration. No application code; no forward-engineering artifacts.

---

## 1. Summary

- **20 legacy prompts** migrated → **22 governed prompts** + 1 orchestrator + 5 shared components.
- **2 legacy files archived** (P3 superseded; AA `00-global-rules` demoted to GOV-01 pointer).
- **3 new Foundation prompts** authored (FN-SYNTH-01/02, FN-REVIEW-01).
- **4 cross-layer violations** closed; **5-way** and **4-way** duplicate extraction collapsed to single owners.
- All governance/confidence externalized to `_shared/` components.
- Functional behavior and output filenames/markers preserved (see §4).

---

## 2. Prompt-by-prompt migration

| Legacy (audit ID) | New prompt_id | Governance change | Confidence change | Ownership/boundary change | Output compatibility |
|---|---|---|---|---|---|
| P1 BA Scout | `BA-SCOUT-01` | inline → CMP-GOV | ✅/⚠️ → GOV-04 | entity-relationships removed → consume DA | 6 tables, DOCUMENT markers preserved |
| P2 BA DeepAnalyst | `BA-ANALYST-01` | inline → CMP-GOV | categorical → GOV-04 | data/app facts → consume DA/AA | 8 docs preserved |
| P3 BA Exec Plan | **archived** | n/a | n/a | superseded (drift, leaks) | nothing executed it — no loss |
| P4 layer2 | `BA-ANALYST-02` | inline → CMP-GOV | none → GOV-04 | sole owner of business rules; entities → DA refs | layer2_output.json schema preserved |
| P5 layer3 | `BA-ANALYST-03` | inline → CMP-GOV | none → GOV-04 | data-model doc → DA view | 10 docs + `===DOCUMENT_*===` preserved |
| P6 DA Reverse-Eng | `DA-SCOUT-01` + `DA-ANALYST-01` | inline → CMP-GOV | numeric → GOV-04 (band kept) | sole owner data-store/entity; **+** received TA OUT4, AA data-ownership | 13 DA filenames preserved + 2 received |
| P7 DA Review | `DA-REVIEW-01` | inline → CMP-GOV | numeric → GOV-04 | unchanged (clean) | review-summary.md + change records preserved |
| P8 TA StackScout | `TA-SCOUT-01` | inline → CMP-GOV | categorical → GOV-04 | sole owner tech-stack | 6 TA filenames + TA_FILE markers preserved |
| P9 TA DeepAnalyst | `TA-ANALYST-01` | inline → CMP-GOV | categorical → GOV-04 | **OUT4 → DA**, **OUT5 app-sec → AA**; keep infra/transport security | 6 outputs kept, 2 relocated (pointer noted) |
| P10 AA AGENTS.md | `AGENTS.md` (orch) | golden rules → CMP-GOV | n/a | unchanged | orchestration manifest |
| P11 AA master | `AA-ANALYST-00` | inline rules + junk list → CMP-GOV/GR-4 | → GOV-04 | tech-stack language → consume TA | AA package filenames preserved |
| P12 AA 00-global-rules | **demoted → pointer** | becomes GOV-01 pointer | n/a | n/a | rules now single-sourced |
| P13 inventory | `AA-SCOUT-01` | global-rules ref → CMP-GOV | tiers → GOV-04 | unchanged | 4 inventory JSONs preserved |
| P14 parser-symbol | `AA-SCOUT-02` | → CMP-GOV | tiers → GOV-04 | unchanged | 4 parsed JSONs preserved |
| P15 evidence-pack | `AA-ANALYST-03` | → CMP-GOV | → GOV-04 | unchanged | 9 evidence packs preserved |
| P16 final-arch | `AA-ANALYST-04` | → CMP-GOV | → GOV-04 | unchanged | AA final set preserved |
| P17 enterprise-fwd | `AA-ANALYST-05` | → CMP-GOV | → GOV-04 | **capability-map → BA**, **data-ownership → DA** | AA-owned FE inputs preserved; 2 relocated |
| (new) app-security | `AA-ANALYST-06` | CMP-GOV | GOV-04 | **receives** app/data-level security from TA OUT5 | app-security-assessment.md (new home) |
| P18 quality-review | `AA-REVIEW-06` | → CMP-GOV | PASS/PARTIAL/FAIL → GOV-04 §5 | unchanged | 3 review docs preserved |
| P19 workflow-audit | `AA-REVIEW-07` | → CMP-GOV | maturity → GOV-04 §5 | + manifest/version checks | audit docs preserved |
| P20 AA workflow doc | (kept; safety → GR-5) | → GOV-01 ref | n/a | unchanged | automation doc |
| — | `FN-SYNTH-01` | CMP-GOV | GOV-04 | **new** cross-track reconciliation | ENTERPRISE_KNOWLEDGE_GRAPH.json (9 sections) |
| — | `FN-SYNTH-02` | CMP-GOV | GOV-04 | **new** canonical views | canonical model + 3 views |
| — | `FN-REVIEW-01` | CMP-GOV | GOV-04 §5 | **new** reconciliation gate | reconciliation-report.md |

---

## 3. Audit findings closed by this migration

| Finding | Closure mechanism |
|---|---|
| 1. Three paradigms | All prompts now GOV-03 (12 sections, Scout/Analyst/Review/Synthesis roles). |
| 2. Three confidence systems | All `{{include: CMP-CONF}}` → GOV-04; legacy numeric band derived only. |
| 3. Four cross-layer violations | TA OUT4→DA-ANALYST-01; TA OUT5→AA-ANALYST-06; AA Stage05 capability-map→BA, data-ownership→DA. |
| 4. Data/entity discovery ×5 | Single owner **DA-SCOUT-01**; BA/AA/TA/layer1 consume-and-cite. |
| 5. Business-rule extraction ×4 | Single owner **BA** (BA-ANALYST-01/02); DA tags data-layer rules for BA. |
| 6. Governance duplicated | All inline rules → CMP-GOV/GR-*; AA `00-global-rules` demoted. |
| 7. No centralized ownership | GOV-02 enforced in every prompt's §4/§6; validated in ownership report. |
| 8. No model pinning | `model_pin: required` in every metadata block (GR-10.1); manifest checks in AA-REVIEW-07/FN-REVIEW-01. |
| 9. No prompt metadata/versioning | GOV-03 §1 metadata + §12 changelog in every prompt. |
| 10. No synthesis layer | Foundation layer authored (FN-SYNTH-01/02, FN-REVIEW-01). |

---

## 4. Functional behavior & output compatibility

**Preserved (behavior-equivalent):**
- All output **filenames** retained (DA 13-file set, TA 6-file set, AA final set, BA 10 docs, layer2_output.json schema).
- All output **markers** retained (`===DOCUMENT_*===`, `===AA_FILE_*===`, `===TA_FILE_*===`, `===DA_FILE_*===`) via `CMP-OUT` legacy-compatibility note — downstream runner parsers unchanged.
- Extraction *logic* unchanged where a layer owned it; only **where** an artifact is produced moved for the 4 relocations.

**Relocations (same artifact, new owning prompt — net output set unchanged):**
- `data-architecture-assessment` content: produced by `DA-ANALYST-01` instead of TA.
- app-level `security-architecture-assessment` content: produced by `AA-ANALYST-06` instead of TA; TA keeps `infra-transport-security-assessment`.
- `business-capability-map`: produced by BA, consumed by AA-ANALYST-05.
- `data-ownership-map`: produced by DA, consumed by AA-ANALYST-05.

**Additive (supersets output, no existing artifact changed):**
- Foundation layer adds `ENTERPRISE_KNOWLEDGE_GRAPH.json` + canonical views + reconciliation-report — previously produced outside the pipeline.

**De-duplication (fewer producers, same facts):**
- Data-store/entity facts now produced once (DA) and referenced; consumers cite DA IDs. The *information* each consumer needs is still present — via citation instead of re-extraction.

---

## 5. Residual / deferred (per governance rules)

- **Prompt assembler** (resolves `{{include:}}`) is orchestration code — **deferred** (no code in this task). Until built, include lines are authoritative and the conformance gate checks no inline duplication.
- **`common/` orchestration module + run manifest** (GOV-06 WS-G / Wave 5) — code, deferred.
- Original legacy prompt files are **not modified** (audit-only constraint upheld); the refactored versions live here in `prompt-refactored/` for adoption.
