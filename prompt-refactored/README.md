# Prompt Refactored Package

**Project:** Forward-/Reverse-Engineering Enterprise Pipeline (`frwd engg - op's`)
**Package type:** Refactored, governed prompt architecture (migration execution)
**Created:** 2026-06-24
**Authority:** `../prompt-governance/` (GOV-01 … GOV-10) — authoritative
**Status:** Refactored prompt specifications + validation reports. **No application code. No forward-engineering artifacts.**

---

## What this package is

This is the **execution** of the migration defined in `../prompt-governance/06_PROMPT_REFACTORING_PLAN.md`.
Every existing prompt has been refactored into the GOV-03 canonical structure, with:

- local governance replaced by `{{include: CMP-GOV}}` → GOV-01,
- all confidence schemes replaced by `{{include: CMP-CONF}}` → GOV-04,
- ownership corrected per GOV-02, boundaries enforced per GOV-08,
- duplicated extraction removed (single owner; others **consume-and-cite**),
- the new Foundation layer authored (FN-SYNTH-01/02, FN-REVIEW-01).

Functional behavior and output compatibility are preserved (see migration report §"Output compatibility").

## Folder layout

```
prompt-refactored/
├── README.md
├── _shared/                         # GOV-09 components, materialized as includable files
│   ├── CMP-GOV.md
│   ├── CMP-CONF.md
│   ├── CMP-VALID.md
│   ├── CMP-EVID.md
│   └── CMP-OUT.md
├── business-architecture/
│   ├── BA-SCOUT-01.md               # was BA_Agent1_StructuralScout_v3.md
│   ├── BA-ANALYST-01.md             # was BA_Agent2_DeepAnalyst_v3.md
│   ├── BA-ANALYST-02.md             # was layer2/layer2_prompt.md
│   └── BA-ANALYST-03.md             # was layer3/layer3_prompt.md
├── data-architecture/
│   ├── DA-SCOUT-01.md               # was DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md (extraction half)
│   ├── DA-ANALYST-01.md             # was DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md (analysis half)
│   └── DA-REVIEW-01.md              # was DA_REVIEW_PROMPT.md
├── application-architecture/
│   ├── AGENTS.md                    # orchestrator (was AGENTS.md)
│   ├── AA-ANALYST-00.md             # master (was application_architecture_extraction_agent_prompt.md)
│   ├── AA-SCOUT-01.md               # was 01-inventory-agent.md
│   ├── AA-SCOUT-02.md               # was 02-parser-symbol-agent.md
│   ├── AA-ANALYST-03.md             # was 03-evidence-pack-agent.md
│   ├── AA-ANALYST-04.md             # was 04-final-architecture-agent.md
│   ├── AA-ANALYST-05.md             # was 05-enterprise-forward-engineering-agent.md (relocations applied)
│   ├── AA-ANALYST-06.md             # app/data-level security (relocated from TA P9 OUTPUT 5)
│   ├── AA-REVIEW-06.md              # was 06-quality-review-agent.md
│   └── AA-REVIEW-07.md              # was 07-workflow-audit-agent.md
├── technology-architecture/
│   ├── TA-SCOUT-01.md               # was TA_STACKSCOUT_PROMPT.md
│   └── TA-ANALYST-01.md             # was TA_DEEPANALYST_PROMPT.md (OUT4/OUT5 relocated)
├── foundation/
│   ├── FN-SYNTH-01.md               # NEW — knowledge graph synthesis
│   ├── FN-SYNTH-02.md               # NEW — canonical model + views
│   └── FN-REVIEW-01.md              # NEW — reconciliation review/gate
├── _archived/
│   ├── P3_BA_Pipeline_Execution_Plan.POINTER.md   # superseded (audit F7)
│   └── AA_00-global-rules.POINTER.md              # demoted to GOV-01 pointer
└── reports/
    ├── MIGRATION_REPORT.md
    ├── CONFORMANCE_REPORT.md
    ├── OWNERSHIP_VALIDATION_REPORT.md
    └── DEPENDENCY_VALIDATION_REPORT.md
```

## Include convention

Prompts reference components by ID, e.g. `{{include: CMP-GOV v1.0.0 role=Scout}}`. The component files
live in `_shared/`. Until a prompt assembler exists (orchestration; code deferred per governance rules),
the include line is authoritative and the conformance gate verifies no inline governance duplication.

## Reports

| Report | Answers |
|---|---|
| `MIGRATION_REPORT.md` | What changed per prompt; behavior/output compatibility |
| `CONFORMANCE_REPORT.md` | GOV-03 12-section + GOV-01/04 conformance per prompt |
| `OWNERSHIP_VALIDATION_REPORT.md` | GOV-02 single-owner + GOV-08 boundary compliance |
| `DEPENDENCY_VALIDATION_REPORT.md` | GOV-07 DAG, contracts, no forbidden edges |
