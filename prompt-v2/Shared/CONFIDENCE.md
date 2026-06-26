# CONFIDENCE — Confidence Model (prompt-v2)

**Component ID:** V2-CONF · **Version:** 2.0.0 · **Supersedes:** CMP-CONF (content identical to GOV-04)
**Authority:** `../../prompt-governance/04_CONFIDENCE_STANDARD.md` (GOV-04). One model. No local scales.

---

```
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
  - Upstream confidence preserved; never silently raised (GR-1.6); raising needs a change record (GR-9.2).
  - Numeric bands (HIGH .90–1.0, MEDIUM .70–.89, LOW .50–.69, ASSUMED <.50) are DERIVED for tooling only.
  - Do NOT define any local numeric or categorical scale.
Escalation: ASSUMED on a material node → Open Question + block HIGH-only downstream gates.
  Unresolved cross-track DISCREPANCY → escalate to Foundation (FN-SYNTHESIZE / FN-VALIDATE).

§5 — Run/gate verdict (orthogonal to finding confidence):
  PASS    = all required outputs present, valid, traceable; no unresolved material DISCREPANCY.
  PARTIAL = produced with documented gaps (status: partial + Open Questions).
  FAIL    = missing/invalid required outputs, or unresolved material DISCREPANCY.
```
