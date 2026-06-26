# 06 — Prompt Refactoring Plan (Per-Prompt Migration)

**Document ID:** GOV-06
**Version:** 1.0.0
**Status:** Canonical — migration plan
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) (all findings)
**Depends on:** GOV-01 … GOV-05, GOV-07 … GOV-10

---

## 0. Scope & rules

Covers all **20 prompts** in the audit inventory (P1–P20) plus the new Foundation prompts (FN-*). For
each: current state · ownership · violations · duplication · recommended changes. **No prompt file is
rewritten in this package** — this is the plan and sequencing. New `prompt_id`s follow `03`.

Severity: 🔴 blocker · 🟡 important · 🟢 hygiene.

---

## 1. Per-prompt migration cards

### P1 — `BA_Agent1_StructuralScout_v3.md`
- **Current:** Business Scout; 6 inventory tables. Paradigm A.
- **Ownership:** BA (Scout). Mostly clean.
- **Violations:** 🟡 Extracts entity relationships (DA-owned) and config/constants (TA-owned hints).
- **Duplication:** Exclusion list (×5), confidence legend, chunk-continuity, "never read bodies".
- **Changes:** → `BA-SCOUT-01`. Adopt GOV-03 template. Remove inline governance; cite GR-4/GR-6/GR-1. Entity-relationship rows become **DA-consumed references** (cite DA node IDs), not BA extractions. Confidence → GOV-04.

### P2 — `BA_Agent2_DeepAnalyst_v3.md`
- **Current:** Business Analyst; 8 business docs. Paradigm A.
- **Ownership:** BA (Analyst).
- **Violations:** 🟡 Touches Data (access patterns), App (call chains), Tech (exception/retry logic).
- **Duplication:** Scout/Analyst scaffold, confidence, never-reset registry, plain-English mandate.
- **Changes:** → `BA-ANALYST-01`. Keep business reasoning; for data/app/tech facts, **consume** DA/AA/TA node IDs instead of re-deriving. GOV-03 sections; GOV-04 confidence; GR-6.2 registry rule (cited, not restated).

### P3 — `BA_Pipeline_Execution_Plan.md`
- **Current:** 1,566-line aspirational SLM+vector-DB design. **Not executed** by any runner.
- **Ownership:** BA (meta) — but describes DA (DDL/procs) + TA (infra) work.
- **Violations:** 🔴 BA→DA (DDL, stored procs, triggers, FKs) and BA→TA (cloud/infra) in the described design.
- **Duplication:** 🔴 Re-describes L2/L3 tasks (P4/P5), tool matrices, risk patterns (~55% overlap).
- **Changes:** **Archive as superseded** (doc drift, audit F7). Replace with a 1-page pointer to P4/P5 + this governance package. No behavior lost (nothing executes it).

### P4 — `layer2/layer2_prompt.md`
- **Current:** JSON-contract business extraction. Paradigm C.
- **Ownership:** BA (Core L2).
- **Violations:** 🟡 Entity relationships (DA), config keys (TA), controller access (AA).
- **Duplication:** "5 tasks" duplicated in P3; anti-infra rule.
- **Changes:** → `BA-ANALYST-02` (JSON variant). Keep JSON schema output. Business rules/capabilities stay BA; data relationships **reference DA**; tech config **reference TA**. Cite GOV-01; confidence GOV-04. Resolve as the single owner of business-rule *extraction* (DA stops extracting business rules — §3 of GOV-02).

### P5 — `layer3/layer3_prompt.md`
- **Current:** Renders L2 JSON → 10 business MD docs. Paradigm C.
- **Ownership:** BA (Core L3).
- **Violations:** 🟡 Emits a data-model doc + operating/HR model.
- **Duplication:** Doc templates overlap P2 outputs; plain-English mandate.
- **Changes:** → `BA-ANALYST-03`. Data-model doc becomes a **DA-sourced view** (cite DA). Keep business docs. GOV-03 sections; GR-8 output rules.

### P6 — `DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md`
- **Current:** Data Scout/extraction; 13 artifacts; live DB. Paradigm A.
- **Ownership:** DA (Scout/Analyst). **Cleanest layer.**
- **Violations:** None.
- **Duplication:** 🟡 Evidence Strength Hierarchy verbatim with P7; SHARED-marking; numeric confidence.
- **Changes:** → `DA-SCOUT-01` (+`DA-ANALYST-01` for the analytic parts). Replace local hierarchy with GR-2.2 reference. Numeric confidence → GOV-04 labels (band preserved for tooling). Becomes **sole owner** of data-store/entity/schema discovery (others consume).

### P7 — `DA_REVIEW_PROMPT.md`
- **Current:** Data review/gate; change records; Gate G1. Paradigm A.
- **Ownership:** DA (Review).
- **Violations:** None.
- **Duplication:** 🟡 Evidence hierarchy verbatim from P6.
- **Changes:** → `DA-REVIEW-01`. Cite GR-2.2 + GR-9 (change records already match GR-9 — make it a reference). Gate verdict → GOV-04 §5 (PASS/PARTIAL/FAIL).

### P8 — `TA_STACKSCOUT_PROMPT.md`
- **Current:** Technology Scout; 6 inventory files. Paradigm A.
- **Ownership:** TA (Scout). Clean.
- **Violations:** None.
- **Duplication:** 🟡 Scout scaffold (mirrors P1), exclusion list (×5), confidence, chunk-continuity.
- **Changes:** → `TA-SCOUT-01`. Cite GR-4/GR-6; confidence GOV-04. Becomes **sole owner** of tech-stack inventory (AA framework-detect downgraded to hint citing TA).

### P9 — `TA_DEEPANALYST_PROMPT.md`
- **Current:** Technology Analyst; 8 assessments. Paradigm A.
- **Ownership:** TA (Analyst).
- **Violations:** 🔴 OUTPUT 4 (Data Architecture Assessment) = DA work; 🔴 OUTPUT 5 (app/security assessment) = AA/SEC work.
- **Duplication:** 🟡 Analyst scaffold (mirrors P2), NEVER rules, confidence.
- **Changes:** → `TA-ANALYST-01`. **Remove OUTPUT 4** → consume DA's data-store transaction/consistency artifact. **Split OUTPUT 5** → app/data-level security to AA (`AA-ANALYST-*`), keep only infra/transport security in TA. Cite GOV-01; confidence GOV-04.

### P10 — `application-architecture/AGENTS.md`
- **Current:** AA orchestrator. Paradigm B.
- **Ownership:** AA (orchestration).
- **Violations:** None.
- **Duplication:** 🔴 Restates `00-global-rules.md` golden rules verbatim (×3 with P11/P12).
- **Changes:** → keep as orchestrator manifest; **replace inline rules with a GOV-01 pointer**. References stage prompt_ids.

### P11 — `application_architecture_extraction_agent_prompt.md`
- **Current:** 1,385-line AA master spec. Paradigm B.
- **Ownership:** AA.
- **Violations:** 🟡 References inventing cloud/k8s/API-gateway (TA terms) + framework detection (TA hint).
- **Duplication:** 🔴 Inlines global rules + junk list (~30% redundant tokens).
- **Changes:** → `AA-ANALYST-00` (master). Strip inlined GR-1/GR-4/GR-5 → GOV-01 pointer. Tech-stack language → cite TA. Consider splitting into the existing staged prompts (already exist as P13–P19) to shrink.

### P12 — `architecture-prompts/00-global-rules.md`
- **Current:** AA governance source. Paradigm B. **Best-isolated governance.**
- **Ownership:** AA-local governance.
- **Violations:** None.
- **Duplication:** It is the *good* pattern — but scoped to AA only.
- **Changes:** **Demote to a thin pointer** to enterprise GOV-01 (`01_GLOBAL_PROMPT_RULES.md`). Its content is absorbed/generalized into GOV-01. Keeps AA stages working while removing the AA-only fork.

### P13–P19 — AA stage prompts `01`…`07`
- **Current:** Lean, one-concern-per-stage, cite global rules. Paradigm B. **Reference model.**
- **Ownership:** AA (per stage).
- **Violations:** 🔴 **P17 (Stage 05)** emits `business-capability-map` (BA) + `data-ownership-map` (DA).
- **Duplication:** 🟢 Minimal (5–10%).
- **Changes:**
  - P13–P16, P18, P19 → rename to `AA-*-0x`, repoint governance to GOV-01, confidence GOV-04. Otherwise **keep as the template** other layers adopt.
  - **P17 → `AA-ANALYST-05`:** remove `business-capability-map` (relocate to BA) and `data-ownership-map` (relocate to DA). AA *consumes* both as forward-engineering inputs and cites their IDs.

### P20 — `ARCHITECTURE_EXTRACTION_WORKFLOW.md`
- **Current:** Automation doc. Paradigm B.
- **Ownership:** AA (automation).
- **Violations:** None.
- **Duplication:** 🟢 Restates safety rules in automation context.
- **Changes:** Repoint safety rules to GR-5; keep automation specifics.

### NEW — Foundation prompts
- `FN-SYNTH-01`, `FN-SYNTH-02`, `FN-REVIEW-01` per `05`. New, no current state. Build after extraction layers conform.

---

## 2. Cross-cutting workstreams

| WS | Action | Findings closed |
|---|---|---|
| **WS-A Governance** | Publish GOV-01; demote P12 to pointer; strip inline rules from P1/P2/P8/P10/P11. | F4, F6 |
| **WS-B Confidence** | Adopt GOV-04 everywhere; delete 3 local schemes. | F1/§3.2 |
| **WS-C Ownership relocation** | Move TA OUT4→DA, TA OUT5→AA, AA Stage05 maps→BA/DA. | F3 |
| **WS-D De-dup extraction** | DA sole owner of data-store/entity; BA sole owner of business rules; TA sole owner of stack; AA sole owner of components. Others consume. | F2 |
| **WS-E Template** | Apply GOV-03 to all prompts; assign `prompt_id`s + versions. | F1, finding 9 |
| **WS-F Foundation** | Build FN layer (`05`). | finding 10, R1 |
| **WS-G Orchestration** | `common/` for CLI/markers/task-IO/caps; pin model in run manifest. | F5, F6 (impl-side, code deferred) |
| **WS-H Hygiene** | Archive P3; repoint P20. | F7 |

---

## 3. Sequencing (waves)

```
Wave 0  Governance foundation:   GOV-01..04, GOV-08 published; P12 → pointer.           [WS-A, WS-B]
Wave 1  Non-breaking conformance: apply GOV-03 metadata + GOV-04 to clean prompts
        (P6,P7,P8,P13–P16,P18,P19); archive P3.                                          [WS-E, WS-H]
Wave 2  Ownership relocation:    TA OUT4→DA, TA OUT5→AA, AA Stage05 maps→BA/DA.          [WS-C]
Wave 3  De-dup extraction:       enforce single owners; others switch to consume+cite.   [WS-D]
Wave 4  Foundation layer:        author FN-SYNTH/REVIEW; wire after BA/DA/AA/TA.          [WS-F]
Wave 5  Orchestration cleanup:   common/ module + model pinning + run manifest.          [WS-G]
```

**Behavioral-equivalence rule:** Waves 0–1 and 5 are non-functional (governance/metadata/plumbing).
Waves 2–4 relocate *where* an artifact is produced but preserve *what* is produced — the union of
emitted artifacts is unchanged; only the owning prompt differs. Foundation (Wave 4) adds reconciliation
that today happens outside the pipeline, so net outputs are a superset, not a change to existing ones.

---

## 4. Per-prompt summary table

| Audit | New prompt_id | Severity | Primary action |
|---|---|---|---|
| P1 | BA-SCOUT-01 | 🟡 | de-dup gov; relocate entity-rel to DA-consume |
| P2 | BA-ANALYST-01 | 🟡 | consume DA/AA/TA facts |
| P3 | — (archived) | 🟢 | supersede; pointer to P4/P5 |
| P4 | BA-ANALYST-02 | 🟡 | sole owner business rules; cite DA/TA |
| P5 | BA-ANALYST-03 | 🟡 | data-model doc → DA view |
| P6 | DA-SCOUT-01 / DA-ANALYST-01 | 🟡 | sole owner data-store; GR-2.2 ref |
| P7 | DA-REVIEW-01 | 🟡 | GR-9 ref; GOV-04 verdicts |
| P8 | TA-SCOUT-01 | 🟡 | sole owner stack; de-dup gov |
| P9 | TA-ANALYST-01 | 🔴 | drop OUT4→DA; split OUT5→AA |
| P10 | AA orchestrator | 🔴(dup) | rules → GOV-01 pointer |
| P11 | AA-ANALYST-00 | 🟡 | strip inline rules; cite TA |
| P12 | → GOV-01 pointer | 🔴(dup) | demote to pointer |
| P13–P16,P18,P19 | AA-*-0x | 🟢 | repoint gov; keep as template |
| P17 | AA-ANALYST-05 | 🔴 | relocate capability+data-ownership maps |
| P20 | AA automation doc | 🟢 | repoint safety → GR-5 |
| new | FN-SYNTH-01/02, FN-REVIEW-01 | 🟡 | build Foundation layer |
