# 05 — Architecture Comparison

**Date:** 2026-06-24 · Current optimized (18 prompts) vs Two-Prompt-Per-Layer v2 (10 prompts).

---

## 1. Side-by-side

| Dimension | Current (18-prompt optimized) | prompt-v2 (10-prompt) |
|---|---|---|
| **Executable prompts** | 18 | **10** (−44%) |
| **Shared components** | 5 (CMP-GOV/CONF/VALID/EVID/OUT) | **3** (GOV/CONFIDENCE/VALIDATION) |
| **Prompts per layer** | uneven (BA 2, DA 2, AA 9, TA 2, FN 3) | **uniform 2/layer** |
| **Structure** | one concern per prompt | EXTRACT (phased) + VALIDATE per layer |
| **Parse-first** | across prompts (explicit stages) | inside EXTRACT (mandatory internal phases) |
| **Layers** | 5 | 5 (unchanged) |
| **Owners** | 5 | 5 (unchanged) |

## 2. Complexity

| Aspect | Current | v2 |
|---|---|---|
| Files to open to understand a layer | up to 9 (AA) | 2 |
| Prompt-count cognitive load | uneven; AA dominates | uniform, predictable |
| Phase visibility | explicit (separate files) | explicit (labeled phases in one file) |
| Hidden complexity risk | low (each file small) | **moderate — AA-EXTRACT is large; phases must not be skipped** |

> Trade-off: v2 reduces *file count* but increases *per-file size and internal complexity*, especially
> AA-EXTRACT (6 phases). The parse-first invariant moves from being structurally enforced (separate
> prompts) to being **instruction-enforced** (phase rules + VALIDATION gate). This is the central risk.

## 3. Maintainability

| Aspect | Current | v2 |
|---|---|---|
| Governance edit propagation | 5 components | **3 components** (simpler) |
| Add a new layer concern | new prompt | new phase inside EXTRACT |
| Per-layer mental model | varies | **uniform 2-prompt** (easier onboarding) |
| Risk of editing a large prompt | low | higher (AA-EXTRACT) |
| Drift between phases | n/a (separate) | must keep phases coherent in one file |

## 4. Extensibility

| Scenario | Current | v2 |
|---|---|---|
| Add a 6th layer (e.g. Security) | add N prompts | add EXTRACT+VALIDATE (fits the pattern cleanly) |
| Add a new AA stage | new stage prompt | new phase in AA-EXTRACT (file grows) |
| Swap a confidence rule | edit 1 component | edit 1 component |
| **Verdict** | flexible, granular | **cleaner pattern, but EXTRACT files accrete phases over time** |

## 5. Governance

Both are 100% compliant (GOV-01/02/03/04/07/08). v2 **improves** GOV-01 (5→3 shared files) and keeps
ownership/boundaries identical. No governance regression in either direction.

## 6. Execution flow

Identical layer order and dependencies (BA→DA→AA→TA→FN, DAG terminating at Foundation). v2 moves the
parse-first stages from inter-prompt to intra-prompt phases. **Runtime model-call count is similar** (the
phases still execute); the difference is one prompt document driving them vs many.

## 7. Risks

| Risk | Current | v2 | Severity |
|---|---|---|---|
| Parse-first skipped (raw→final) | structurally impossible (separate prompts) | **possible if a phase is omitted** | 🟡 Medium — mitigated by phase rules + VALIDATION gate + runner persistence |
| Large-prompt maintenance error | low | AA-EXTRACT is big | 🟡 Low–Medium |
| Intermediate artifact loss | low | must ensure runner persists each phase | 🟡 Medium (M4) |
| Output/graph incompatibility | — | none (100% compat verified) | 🟢 None |
| Governance/ownership regression | — | none | 🟢 None |
| Runner rework | none | required (deferred code) | 🟡 Medium |

## 8. Benefits

- **−44% prompt count** (18→10), **−40% shared components** (5→3).
- **Uniform 2-prompt-per-layer** mental model — easier onboarding, predictable structure.
- **Restores 1-prompt-per-extraction** within DA/TA (closes the split that needed special runner handling).
- **Independent validate gate** per layer (some layers gain a formal gate they lacked).

## 9. Recommendation

### This deliverable: **prompt-v2 is delivered and is fully compatible (100% on all dimensions).**

**However — engineering recommendation on adoption:**

| Option | When to choose |
|---|---|
| **Adopt prompt-v2** | If the organization values a **uniform, low-file-count, easy-to-onboard** structure and is willing to fund the runner rework (M4) and accept larger EXTRACT prompts with instruction-enforced parse-first. |
| **Keep the 18-prompt optimized set** | If the organization values **structurally-enforced** parse-first (separate stage prompts make phase-skipping impossible) and the AA pipeline's granular auditability over file-count reduction. |

**My recommendation: ADOPT prompt-v2 for BA, DA, TA, FN; KEEP the AA pipeline staged (or treat
AA-EXTRACT's 6 phases as a documented wrapper over the existing 6 AA stage prompts).**

Rationale: BA/DA/TA/FN merges are clean, low-risk, and genuinely simplify maintenance. The AA layer is the
one place where collapsing 6 parse-first stages into one prompt trades **structural safety** for
**file-count** — the audit specifically praised AA's staged model and the optimization step declined to
merge it. Two-prompt-per-layer is achievable for AA **on paper** (AA-EXTRACT + AA-VALIDATE are written and
compatible), but the safest production posture is to let AA-EXTRACT **orchestrate the existing 6 stage
prompts as named phases** rather than inline them — preserving structural parse-first while presenting the
uniform 2-prompt interface.

> Net: **Two-prompt-per-layer = YES as the interface/standard. For AA, implement EXTRACT as a phase
> orchestrator over the proven stage prompts rather than a monolithic inline prompt.** This gives the
> uniformity benefit without surrendering AA's structural correctness guarantee.
