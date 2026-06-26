# VALIDATION — Quality Gates (prompt-v2)

**Component ID:** V2-VALID · **Version:** 2.0.0 · **Supersedes:** CMP-VALID
**Authority:** `../../prompt-governance/01_GLOBAL_PROMPT_RULES.md` GR-7.

---

```
Before completion every prompt runs the GR-7 gate:
  - GR-7.1  Stop (FAIL) if any REQUIRED input is missing, empty, or schema-invalid.
  - GR-7.2  Emitted JSON parses; every graph edge resolves to an existing node.
  - GR-7.3  Sibling cross-references agree (counts match; cited items exist in their sets).
  - GR-7.4  Every `unknown` / ambiguity becomes an Open Question with a stable ID.
  - GR-7.5  Self-check: no invented facts (GR-1), all claims cited (GR-2/3),
            no excluded paths used (GR-4), no source modified (GR-5).
  - Per declared output: confirm it exists, is well-formed, traceable.
  - Parse-first integrity: confirm intermediate artifacts were emitted before final
    (inventory/parsed/evidence/JSON/graph) — a unified EXTRACT prompt MUST NOT skip phases.
Emit a run verdict: PASS | PARTIAL | FAIL (CONFIDENCE §5).
```
