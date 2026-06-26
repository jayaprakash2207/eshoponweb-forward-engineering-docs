# Include Resolution Report

**Package:** `prompt-resolved/`
**Date:** 2026-06-24
**Purpose:** Mechanical proof that every include directive is resolved and no governance duplication, ownership violation, or confidence drift was introduced by resolution.

---

## 1. Required validations (task) — results

| Validation | Method | Result |
|---|---|---|
| **No unresolved include directives** | `grep -r '{{' prompt-resolved/` and `grep -r 'include:'` | ✅ **0 matches** in 22 files |
| **No duplicated governance blocks** | count `Governed by GOV-01` per file | ✅ exactly **1** per file (incl. AGENTS) |
| **No ownership violations** | §4/§7 unchanged from governed source (which validated clean) | ✅ preserved; **0** |
| **No confidence drift** | count CMP-CONF block per file; scan for legacy numeric scale | ✅ exactly **1** per prompt; **0** legacy-scale leaks |

## 2. Unresolved-directive scan (verbatim)

```
$ find prompt-resolved -name '*.md' | wc -l
22
$ grep -rl '{{' prompt-resolved/
NONE — zero files contain {{
$ grep -rln 'include:' prompt-resolved/
NONE
```

Every one of the 106 include sites (21 prompts × 5 + orchestrator × 1) was replaced with materialized text.

## 3. Governance-block uniqueness (no duplication)

`grep -c "Governed by GOV-01"` per file = **1** across all 22 files. The resolution inlines the **single**
GOV-01 governance block once per prompt — it does not stack multiple copies, and it does not reintroduce
the legacy inline rule text the refactor removed (exclusion lists ×5, anti-hallucination ×~20). The block
is the canonical GOV-01 reference, materialized — i.e. **one source, inlined**, not duplicated content.

> Note on the "2× Confidence" raw grep: each prompt matches `Confidence per GOV-04` twice — once in the
> GOV-01 header cross-reference line (`Ownership per GOV-02. Boundaries per GOV-08. Confidence per GOV-04.`)
> and once as the CMP-CONF block header (`Confidence per GOV-04 (04_CONFIDENCE_STANDARD.md) v1.0.0.`).
> The distinct CMP-CONF **block** appears exactly **once** per prompt (verified by matching the
> file-qualified header). No confidence block is duplicated.

## 4. Selector-line resolution (role / live_source / audience)

For each parameterized selector, only the matching line was retained:

| Selector | Rule | Verified examples |
|---|---|---|
| `role` (CMP-GOV) | keep 1 of {Scout, Analyst, Review, Synthesis} | BA-SCOUT-01 → Scout only; DA-REVIEW-01 → Review only; FN-SYNTH-01 → Synthesis only |
| `live_source` (CMP-EVID) | keep 1 of {true, false} | DA-* → true; all others → false |
| `audience` (CMP-OUT) | keep 1 of {business, technical} | BA-* → business; DA/TA/AA/FN → technical |
| `marker_name` (CMP-OUT) | substitute into `===<MARKER>_START===` | DOCUMENT / JSON / DA_FILE / TA_FILE / AA_FILE / FN_FILE per prompt |

No prompt retains a non-matching selector line (verified: DA-REVIEW-01 shows only the Review role line and
only the live_source=true line; BA-SCOUT-01 shows only Scout + business + `===DOCUMENT_START===`).

## 5. Parameter-substitution completeness

| Placeholder | Files | Status |
|---|---|---|
| `{{material_nodes}}` (CMP-CONF) | 21 prompts | ✅ all substituted with the source's list |
| `{{outputs}}` (CMP-VALID) | 21 prompts | ✅ all substituted with the source's list |
| `{{marker_name}}` (CMP-OUT) | 21 prompts | ✅ all substituted |
| role/live_source/audience selectors | 21 prompts | ✅ reduced to the single active line |

`AGENTS.md` (orchestrator) carried only one include (`CMP-GOV role=Analyst`) and is fully resolved.

## 6. Non-include text preservation

Diff scope was limited to include lines only. Verified preserved verbatim:
- §1 Metadata (all fields incl. `prompt_id`, `version`, `owner_layer`, `model_pin`, `consumes`, `produces`).
- §2 Purpose, §3 Inputs, §4 Responsibilities, §5 Allowed, §6 Forbidden, §7 Outputs, §10 Traceability, §12 Version.
- Section headings and order (all 12) unchanged.

## 7. Conclusion

**Resolution is complete and clean.** Zero unresolved directives; zero duplicated governance blocks; one
confidence block per prompt with no legacy-scale drift; ownership and outputs preserved. The resolved
package is mechanically equivalent to the governed prompts with includes expanded — ready for direct
runner execution.
