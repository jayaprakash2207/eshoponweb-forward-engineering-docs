# 04 — Confidence Standard (One Enterprise Model)

**Document ID:** GOV-04
**Version:** 1.0.0
**Status:** Canonical — replaces all existing confidence systems
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) §3.2 (three confidence schemes), F1
**Depends on:** `01_GLOBAL_PROMPT_RULES.md` (GR-2 evidence hierarchy)

---

## 0. Problem replaced

The audit found **three incompatible confidence schemes** running at once:

1. **Numeric** (DA): `1.0 … 0.70 … <0.70→UNKNOWN`.
2. **Categorical** (BA/TA): `HIGH / LOW / ASSUMED / DISCREPANCY`.
3. **Gate verdicts** (AA): `PASS/PARTIAL/FAIL`, `ENTERPRISE READY/…`.

No translation table existed, so confidence could not be compared across layers. This document defines
**one** model. Gate verdicts (#3) are retained but redefined as a **separate review dimension**, not a
confidence level (see §5), so they no longer compete with finding-level confidence.

---

## 1. The five-level model

Every emitted finding/node carries exactly one confidence label:

| Label | Meaning | Numeric band (for tooling) | Required evidence (GR-2.2 rank) |
|---|---|---|---|
| **HIGH** | Directly observed, unambiguous fact. | `0.90 – 1.00` | Rank 1–3 (live system, declarations, entity/ORM/source) with a direct citation. |
| **MEDIUM** | Well-supported but indirect or partial. | `0.70 – 0.89` | Rank 3–5 (source logic, tests, usage) with citation; minor inference. |
| **LOW** | Weakly supported; inferred from naming, lock files, or single weak signal. | `0.50 – 0.69` | Rank 6–7 (naming, docs) or fallback evidence; reason mandatory. |
| **ASSUMED** | No direct evidence; reasoned placeholder to keep model coherent. | `< 0.50` | No qualifying evidence; assumption + rationale mandatory; routed to Open Questions. |
| **DISCREPANCY** | Sources conflict, or output contradicts an upstream owner. | n/a (flag) | Both sources cited + resolution per GR-2.4. |

> The numeric band is a **derived convenience** for dashboards/sorting only. Prompts emit the **label**;
> tooling may attach the band midpoint. This preserves the DA numeric workflow without keeping a separate
> scheme.

---

## 2. Label decision rules

Apply in order; first match wins:

1. If the finding contradicts another source or an upstream owner → **DISCREPANCY** (then resolve via GR-2.4, and the *resolved* value gets its own HIGH/MEDIUM/LOW label).
2. Else if direct evidence at GR-2.2 rank 1–3 with citation → **HIGH**.
3. Else if supported at rank 3–5 with citation, partial/indirect → **MEDIUM**.
4. Else if only rank 6–7 / lock-file fallback / single weak signal → **LOW** (+ reason).
5. Else (no qualifying evidence) → **ASSUMED** (+ rationale, + Open Question).

**Mandatory reason strings:** `LOW`, `ASSUMED`, and `DISCREPANCY` must carry a short reason
(`LOW — inferred from naming only`, `ASSUMED — no config evidence`, `DISCREPANCY — fileA=X vs fileB=Y`).
`HIGH`/`MEDIUM` carry the citation instead.

---

## 3. Evidence requirements per label

| Label | Citation required? | Reason required? | Open Question raised? |
|---|---|---|---|
| HIGH | Yes (GR-3.1) | No | No |
| MEDIUM | Yes | Optional (note the indirect step) | No |
| LOW | Yes (the weak source) | **Yes** | Optional |
| ASSUMED | n/a | **Yes** | **Yes** (GR-7.4) |
| DISCREPANCY | Yes (both sources) | **Yes** | **Yes** until resolved |

No finding may be `HIGH` without a direct code/config/declaration reference (GR-2.5). Upstream confidence
is never silently raised (GR-1.6); raising it requires a change record (GR-9.2).

---

## 4. Escalation rules

| Trigger | Action |
|---|---|
| `ASSUMED` finding on a **material** node (capability, owned entity, public API, security control) | Raise Open Question; block `HIGH`-only downstream gates that depend on it. |
| `DISCREPANCY` unresolved after applying GR-2.4 | Escalate to Foundation layer conflict resolution (`05`); do not pick arbitrarily. |
| Conflict between two **owner** layers for one fact | Foundation owns resolution; extraction layers may not self-resolve. |
| Question answerable by reading more in-scope evidence | Do **not** escalate to a human (GR-9.3); read more first. |
| Aggregate `LOW`+`ASSUMED` share of an artifact exceeds the prompt's stated threshold | Mark artifact `status: partial`; report in validation summary. |

---

## 5. Review/gate verdicts (separate dimension)

Gate verdicts assess an **artifact/run**, not a single finding, and are orthogonal to confidence:

| Verdict | Definition |
|---|---|
| **PASS** | All required outputs present, valid, traceable; no unresolved DISCREPANCY on material nodes. |
| **PARTIAL** | Produced with documented gaps (`status: partial`, Open Questions present). |
| **FAIL** | Missing/invalid required outputs, or unresolved material DISCREPANCY. |

Maturity phrases formerly used by AA (`ENTERPRISE READY` / `MOSTLY READY` / `NOT READY`) map 1:1 to
`PASS / PARTIAL / FAIL` and are retired as separate vocabulary.

---

## 6. Cross-scheme migration map (from audit's three schemes)

| Old signal | New label/verdict |
|---|---|
| DA `1.0` (confirmed by live DB) | HIGH |
| DA `0.85` (entity/migration code) | HIGH/MEDIUM (by rank) |
| DA `0.70` (naming convention) | LOW |
| DA `<0.70 → UNKNOWN` | ASSUMED (+ Open Question) |
| BA/TA `HIGH` | HIGH |
| BA/TA `LOW — reason` | LOW (+ reason) or MEDIUM if rank 3–5 |
| BA/TA `ASSUMED — reason` | ASSUMED |
| BA/TA `DISCREPANCY` | DISCREPANCY |
| AA `PASS` / `PARTIAL` / `FAIL` | (verdict dimension — unchanged) |
| AA `ENTERPRISE READY` / `MOSTLY READY` / `NOT READY` | PASS / PARTIAL / FAIL |

---

## 7. Conformance

A prompt is GOV-04 conformant when its `Confidence Rules` section references this model only, emits the
five labels (and DISCREPANCY handling), raises Open Questions/escalations as in §4, and never defines a
local numeric or categorical scale. Tooling may render the numeric band but must not require prompts to
emit it.
