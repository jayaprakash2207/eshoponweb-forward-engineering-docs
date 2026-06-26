# CMP-GOV — Governance Block (v1.0.0)

> Component per `../../prompt-governance/09_REUSABLE_PROMPT_COMPONENTS.md`.
> Resolves the GOV-01 reference header. **Included by every prompt. Never copied inline.**

**Parameters:** `role` ∈ {Scout, Analyst, Review, Synthesis}

```
─────────────────────────────────────────────
Governed by GOV-01 (01_GLOBAL_PROMPT_RULES.md) v1.0.0. Rules are INHERITED, not restated.
Applicable rule groups:
  GR-1  anti-hallucination      GR-2/3 evidence & citation     GR-4 exclusions
  GR-5  no-modification         GR-6   chunk processing         GR-7 validation
  GR-8  output discipline       GR-9   change records (Review)  GR-10 model/reproducibility
Ownership per GOV-02. Boundaries per GOV-08. Confidence per GOV-04 (see CMP-CONF).

role=Scout     → declaration/inventory level only; never read logic/method bodies; no interpretation.
role=Analyst   → may read logic/bodies as scoped in Allowed Actions; reasons over Scout inventory + evidence.
role=Review    → validates/enriches upstream owner artifacts; emits change records (GR-9); verdict (GOV-04 §5).
role=Synthesis → consumes owner artifacts only; never extracts primary facts (FN-1); reconciles, never invents.
─────────────────────────────────────────────
```

This block replaces, across the project: AA golden rules (×3), the exclusion list (×5), and
anti-hallucination/no-modify clauses (~×20). Prompts add only **narrower** constraints in their own
`Forbidden Actions` — never looser.
