# DEMOTED — application-architecture/architecture-prompts/00-global-rules.md → GOV-01 pointer

**Status:** Demoted to a pointer (content generalized into enterprise GOV-01).
**Date:** 2026-06-24
**Authority:** `../../prompt-governance/06_PROMPT_REFACTORING_PLAN.md` (P12), GOV-01.

## Why demoted
The legacy AA `00-global-rules.md` was the *best* governance pattern in the project — but scoped to AA
only, forcing BA/DA/TA to inline their own equivalents (audit §3.1). Its content (safety, evidence,
ignore rules, parse-first) has been absorbed and generalized into the enterprise single source of truth.

## Replacement
- **GOV-01** (`../../prompt-governance/01_GLOBAL_PROMPT_RULES.md`) — canonical rules GR-1…GR-10.
- Materialized for inclusion as **`../_shared/CMP-GOV.md`**.

## Pointer content (what the demoted file now says)
> This file is retained only as a pointer. All governance rules live in GOV-01
> (`01_GLOBAL_PROMPT_RULES.md`), included via `CMP-GOV`. Do not add rules here.
> Mapping of legacy AA rules → GOV-01: safety → GR-5; evidence → GR-2/GR-3; ignore rules → GR-4;
> parse-first → GR-6.1.

## Action
AA stage prompts (AA-SCOUT-01 … AA-REVIEW-07) and `AGENTS.md` now `{{include: CMP-GOV}}` instead of
referencing the AA-local rules file. No rule text is duplicated anywhere.
