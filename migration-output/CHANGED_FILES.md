# Changed Files

**Date:** 2026-06-24
**Cutover model:** parallel-primary (repoint runners to governed prompts; retain legacy read-only for rollback). No legacy file deleted by this package.

---

## 0. Legend

- **PRIMARY** — governed prompt is now the canonical implementation.
- **REPOINT** — a runner's prompt reference is updated to load the governed prompt.
- **RETAIN(RO)** — legacy file kept read-only as rollback; not loaded after cutover.
- **ARCHIVE** — superseded; replaced by a pointer.
- **ADD** — new file (no legacy equivalent).
- **DEFERRED** — requires orchestration code (out of scope); listed for completeness.

---

## 1. Governed prompts now PRIMARY (`prompt-refactored/`)

| Governed file | Status | Replaces (RETAIN RO) |
|---|---|---|
| `business-architecture/BA-SCOUT-01.md` | PRIMARY | `BA_Agent1_StructuralScout_v3.md` |
| `business-architecture/BA-ANALYST-01.md` | PRIMARY | `BA_Agent2_DeepAnalyst_v3.md` |
| `business-architecture/BA-ANALYST-02.md` | PRIMARY | `layer2/layer2_prompt.md` |
| `business-architecture/BA-ANALYST-03.md` | PRIMARY | `layer3/layer3_prompt.md` |
| `data-architecture/DA-SCOUT-01.md` | PRIMARY | `DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md` (split) |
| `data-architecture/DA-ANALYST-01.md` | PRIMARY | `DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md` (split) |
| `data-architecture/DA-REVIEW-01.md` | PRIMARY | `DA_REVIEW_PROMPT.md` |
| `technology-architecture/TA-SCOUT-01.md` | PRIMARY | `TA_STACKSCOUT_PROMPT.md` |
| `technology-architecture/TA-ANALYST-01.md` | PRIMARY | `TA_DEEPANALYST_PROMPT.md` |
| `application-architecture/AGENTS.md` (governed) | PRIMARY | legacy `AGENTS.md` |
| `application-architecture/AA-ANALYST-00.md` | PRIMARY | `application_architecture_extraction_agent_prompt.md` |
| `application-architecture/AA-SCOUT-01.md` | PRIMARY | `architecture-prompts/01-inventory-agent.md` |
| `application-architecture/AA-SCOUT-02.md` | PRIMARY | `architecture-prompts/02-parser-symbol-agent.md` |
| `application-architecture/AA-ANALYST-03.md` | PRIMARY | `architecture-prompts/03-evidence-pack-agent.md` |
| `application-architecture/AA-ANALYST-04.md` | PRIMARY | `architecture-prompts/04-final-architecture-agent.md` |
| `application-architecture/AA-ANALYST-05.md` | PRIMARY | `architecture-prompts/05-enterprise-forward-engineering-agent.md` |
| `application-architecture/AA-REVIEW-06.md` | PRIMARY | `architecture-prompts/06-quality-review-agent.md` |
| `application-architecture/AA-REVIEW-07.md` | PRIMARY | `architecture-prompts/07-workflow-audit-agent.md` |
| `_shared/CMP-GOV.md … CMP-OUT.md` | ADD (PRIMARY) | (collapses inline governance from all legacy prompts) |

## 2. Added (no legacy equivalent)

| File | Status |
|---|---|
| `application-architecture/AA-ANALYST-06.md` | ADD — app/data-level security (relocated from TA OUTPUT 5) |
| `foundation/FN-SYNTH-01.md` | ADD |
| `foundation/FN-SYNTH-02.md` | ADD |
| `foundation/FN-REVIEW-01.md` | ADD |

## 3. Archived / demoted

| Legacy file | Status | Pointer |
|---|---|---|
| `BA_Pipeline_Execution_Plan.md` | ARCHIVE | `prompt-refactored/_archived/P3_BA_Pipeline_Execution_Plan.POINTER.md` |
| `architecture-prompts/00-global-rules.md` | ARCHIVE/DEMOTE | `prompt-refactored/_archived/AA_00-global-rules.POINTER.md` → GOV-01 |

---

## 4. Runner reference updates (REPOINT)

These are the **only** changes to executable orchestration — each runner's prompt-path constant is
repointed from the legacy `.md` to the governed `.md`. **Runner control flow, model invocation, output
parsing, and retry logic are unchanged** (GOV requirement 5: preserve orchestration flow).

| Runner | Legacy reference (constant) | Repoint to |
|---|---|---|
| `layer2/layer2_runner.py` | `PROMPT_FILE = .../layer2_prompt.md` | `prompt-refactored/business-architecture/BA-ANALYST-02.md` |
| `layer3/layer3_runner.py` | `PROMPT_FILE = .../layer3_prompt.md` | `prompt-refactored/business-architecture/BA-ANALYST-03.md` |
| `data-architecture/da_agent1_runner.py` | `PROMPT_FILE = .../DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md` | `DA-SCOUT-01.md` then `DA-ANALYST-01.md` (two-phase; see note) |
| `data-architecture/da_agent2_runner.py` | `PROMPT_FILE = .../DA_REVIEW_PROMPT.md` | `data-architecture/DA-REVIEW-01.md` |
| `technology-architecture/ta_agent1_runner.py` | `PROMPT_FILE = .../TA_STACKSCOUT_PROMPT.md` | `technology-architecture/TA-SCOUT-01.md` |
| `technology-architecture/ta_agent2_runner.py` | `PROMPT_FILE = .../TA_DEEPANALYST_PROMPT.md` | `technology-architecture/TA-ANALYST-01.md` |
| `application-architecture/aa_runner.py` | stage map → `architecture-prompts/04..07` + `AGENTS.md` + master | governed `AA-ANALYST-04/05/06`, `AA-REVIEW-06/07`, governed `AGENTS.md`, `AA-ANALYST-00` |
| `tools/.../run_architecture_extraction.py` | Python phases (deterministic) | **no change** (not prompt-driven) |

**Note (DA two-phase):** legacy `da_agent1_runner.py` loaded a single prompt. The governed design splits
it into `DA-SCOUT-01` (inventory) + `DA-ANALYST-01` (deliverables). Minimal-change cutover: the runner
loads the two governed prompts sequentially within the same stage, preserving the single `da-outputs/`
13-file contract. The runner *edit* itself is orchestration code → **DEFERRED** (see §6); until then the
governed prompts can be concatenated to reproduce single-pass behavior.

---

## 5. Files NOT changed (preserved)

- `run_pipeline.py` — stage order, CLI, cleanup unchanged (GOV req 5).
- `layer1/*` — deterministic Python extraction; provides raw feed; unchanged.
- All `tools/application_architecture_analyzer/*.py` deterministic phases — unchanged.
- All generated `output/eShopOnWeb/**` artifacts — untouched (existing outputs preserved).
- Downstream foundation/forward-engineering packages — untouched.

---

## 6. DEFERRED (orchestration code — out of this package's scope)

> These are required for a *fully executable* cutover but are **code**, which this task excludes. They
> are listed so the cutover is honest about what remains. Tracked in `prompt-governance/06` Wave 5.

| Item | Why deferred | Blocking? |
|---|---|---|
| **Include resolver** for `{{include: CMP-*}}` | The governed prompts reference governance via includes; a loader must materialize them before the prompt reaches the model | **YES** — see REGRESSION_RISK R-CRIT-1 |
| `da_agent1_runner.py` two-phase edit | Code change | Partial (workaround: concatenate) |
| `aa_runner.py` new stage 05b (AA-ANALYST-06) | Code change | Partial |
| New Foundation runner (FN-SYNTH/REVIEW) | New code | Additive only — does not block legacy parity |
| `common/` module + run manifest (model pin) | Code change (GR-10) | No (declared in metadata) |

**No application code is generated by this package; the above are the required follow-on engineering tasks.**
