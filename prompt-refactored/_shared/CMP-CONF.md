# CMP-CONF — Confidence Block (v1.0.0)

> Component per `../../prompt-governance/09_REUSABLE_PROMPT_COMPONENTS.md`.
> Resolves GOV-04. Replaces all three legacy schemes (numeric / categorical / gate verdict).

**Parameters:** `material_nodes` = list of node types that trigger escalation when `ASSUMED`.

```
─────────────────────────────────────────────
Confidence per GOV-04 (04_CONFIDENCE_STANDARD.md) v1.0.0.
Emit exactly ONE label per finding: HIGH | MEDIUM | LOW | ASSUMED | DISCREPANCY.

Decision order (first match wins):
  1 contradicts another source / upstream owner   → DISCREPANCY  (resolve via GR-2.4, then relabel result)
  2 direct evidence, GR-2.2 rank 1–3, cited        → HIGH
  3 supported, rank 3–5, partial/indirect, cited   → MEDIUM
  4 only rank 6–7 / lock-file fallback / 1 weak    → LOW   (+reason)
  5 no qualifying evidence                          → ASSUMED (+rationale, +Open Question)

Rules:
  - HIGH requires a direct code/config/declaration citation (GR-2.5).
  - LOW / ASSUMED / DISCREPANCY require a short reason string.
  - Upstream confidence is preserved; never silently raised (GR-1.6); raising needs a change record (GR-9.2).
  - Numeric bands (HIGH .90–1.0, MEDIUM .70–.89, LOW .50–.69, ASSUMED <.50) are DERIVED for tooling only.
  - Do NOT define any local numeric or categorical scale.

Escalation: an ASSUMED on a material node {{material_nodes}} → raise Open Question and block HIGH-only
downstream gates that depend on it. Unresolved cross-track DISCREPANCY → escalate to Foundation (GOV-05).
─────────────────────────────────────────────
```
