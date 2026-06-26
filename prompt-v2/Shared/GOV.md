# GOV — Global Rules (prompt-v2 single source of truth)

**Component ID:** V2-GOV · **Version:** 2.0.0 · **Supersedes:** CMP-GOV + CMP-EVID + CMP-OUT (consolidated)
**Authority:** `../../prompt-governance/01_GLOBAL_PROMPT_RULES.md` (GOV-01). This file is the resolved,
self-contained governance block every prompt-v2 prompt references. **No prompt restates these rules inline.**

> Consolidation note: prompt-v2 has **3** shared files (GOV, CONFIDENCE, VALIDATION) instead of 5.
> The evidence rules (former CMP-EVID) and output discipline (former CMP-OUT) are folded into this GOV
> file, because both are governance. This reduces the shared surface 5→3 with no rule lost.

---

## 1. Governance (GR-1…GR-10, inherited from GOV-01)
```
Governed by GOV-01 v1.0.0 / GOV (V2-GOV) v2.0.0. Rules are INHERITED, not restated.
  GR-1  anti-hallucination: never invent; missing/ambiguous → `unknown` + Open Question; preserve verbatim values.
  GR-2/3 evidence & citation (see §2).
  GR-4  exclusions: never scan .git/ node_modules/ dist/ build/ bin/ obj/ target/ .venv/ coverage/ *.min.js *.map …
  GR-5  no-modification: never edit/delete/refactor source; outputs only to OUTPUT_ROOT; never write secrets.
  GR-6  chunk processing: PARSE FIRST, REASON SECOND (inventory → parsed → evidence → final); cumulative IDs never reset.
  GR-7  validation (see Shared/VALIDATION.md).
  GR-8  output discipline (see §3).
  GR-9  change records (Validate/Review prompts).
  GR-10 model & versions pinned in the run manifest; reproducible runs.
Ownership per GOV-02. Boundaries per GOV-08. Confidence per GOV-04 (Shared/CONFIDENCE.md).
```

## 2. Evidence (GR-2/GR-3 — folded from CMP-EVID)
```
Evidence strength hierarchy (high→low), used to RESOLVE conflicts (never average — GR-2.4):
  1 live system/DB query · 2 migration/IaC/manifest · 3 entity/ORM/source decl · 4 tests ·
  5 source logic/usage · 6 naming · 7 docs/comments/git (wins only if it cites a hard constraint).
Citations machine-resolvable: path/to/file.ext:line (GR-3.1). Every node carries ≥1 citation OR `unknown`+Open Question.
When consuming an upstream artifact, cite the OWNER's node ID, never a re-derived copy (GR-3.4).
live_source: EXTRACT prompts that can reach a live system attempt it first (rank-1); on failure fall back to
  code evidence and record the exact error+command. Prompts with no live source rely on declared/source evidence only.
```

## 3. Output discipline (GR-8 — folded from CMP-OUT)
```
Produce EXACTLY the artifacts listed in the prompt's Outputs — no more, no fewer (GR-8.1).
Multi-file output uses the project's standard markers; do NOT invent new schemes (GR-8.2):
   ===<MARKER>_START:<relative/path>===
   <content>
   ===<MARKER>_END===
Use the marker the consuming runner already parses (DOCUMENT / JSON / DA_FILE / AA_FILE / TA_FILE / FN_FILE).
Structure deterministic: stable field names, ordering keys, IDs (GR-8.3).
Audience: business artifacts in business language; technical artifacts may use technical terms; both always cited.
If an output cannot be fully produced, still emit it with status: incomplete + reason + Open Questions (GR-8.5).
Intermediate artifacts (layer JSON, inventory, parsed facts, evidence packs, knowledge graph) MUST still be
  emitted even when produced inside a unified EXTRACT prompt — downstream parse-first depends on them.
```

## 4. Roles
```
EXTRACT     → discover + analyze + produce a layer's architecture, PRESERVING parse-first phasing internally
              (declaration-level discovery before deep analysis; intermediate artifacts emitted at each phase).
VALIDATE    → validate/review/finalize a layer's artifacts; emit change records (GR-9) + verdict (CONFIDENCE §5);
              never extract new primary facts; never silently raise confidence.
SYNTHESIZE  → Foundation only: consume owner artifacts, reconcile, build knowledge graph + canonical model;
              never extract primary facts (FN-1); never invent (GR-1).
```

A prompt adds only **narrower** constraints in its own Forbidden Actions — never looser.
