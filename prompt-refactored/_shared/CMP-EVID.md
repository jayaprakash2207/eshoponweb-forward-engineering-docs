# CMP-EVID — Evidence Block (v1.0.0)

> Component per `../../prompt-governance/09_REUSABLE_PROMPT_COMPONENTS.md`. Resolves GR-2/GR-3.
> Replaces the Evidence Strength Hierarchy duplicated verbatim in DA prompts and scattered citation rules.

**Parameters:** `live_source` ∈ {true, false} — toggles rank-1 (live system/DB) instructions.

```
─────────────────────────────────────────────
Evidence per GR-2. Strength hierarchy (high → low), used to RESOLVE conflicts:
  1 live system / DB query            (only if live_source=true)
  2 migration / IaC / manifest declarations
  3 entity / ORM / source declarations
  4 tests
  5 source logic / usage
  6 naming conventions
  7 docs / comments / git history
Higher rank wins on conflict; NEVER average (GR-2.4). Rank 7 wins only if it cites a hard, named constraint.
Citations are machine-resolvable: `path/to/file.ext:line` (line optional when not derivable) (GR-3.1).
Every emitted node carries ≥1 citation OR `unknown` + Open Question (GR-3.2, GR-1.2).
When consuming an upstream artifact, cite the OWNER's node ID, never a re-derived local copy (GR-3.3/3.4).
Preserve verbatim values — versions, thresholds, enum/state values, config (GR-1.4). Never write secrets (GR-5.4).

live_source=true  → attempt the live query first; on failure, fall back to code evidence and record the exact
                    error + command; mark affected findings with the appropriate confidence (GOV-04).
live_source=false → do not attempt live connections; rely on declared/source evidence only.
─────────────────────────────────────────────
```
