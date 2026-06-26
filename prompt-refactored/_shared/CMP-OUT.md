# CMP-OUT — Output Block (v1.0.0)

> Component per `../../prompt-governance/09_REUSABLE_PROMPT_COMPONENTS.md`. Resolves GR-8 output discipline.
> Replaces the 6 near-identical marker schemes and divergent output-capture instructions across runners.

**Parameters:** `marker_name` (default `FILE`) · `audience` ∈ {business, technical}

```
─────────────────────────────────────────────
Output per GR-8. Produce EXACTLY the artifacts listed in this prompt's `Outputs` — no more, no fewer (GR-8.1).
Multi-file output uses the project's standard markers; do NOT invent new schemes (GR-8.2):
   ==={{marker_name}}_START:<relative/path>===
   <content>
   ==={{marker_name}}_END===
Structure is deterministic: stable field names, stable ordering keys, stable IDs (GR-8.3).
Audience language (GR-8.4):
   audience=business  → business language; no code terms, method names, or file paths in the narrative
                        (citations still carry paths in evidence fields).
   audience=technical → technical terms permitted; still evidence-cited.
If an output cannot be fully produced, still emit it with: status: incomplete, reason, Open Questions (GR-8.5).
Legacy delimiter compatibility (preserve downstream parsers):
   - BA doc generator expects ===DOCUMENT_START:<file>=== / ===DOCUMENT_END===
   - AA/TA runners expect ===AA_FILE_START / ===TA_FILE_START style markers
   Use the marker_name the consuming runner already parses (see each prompt's Outputs).
─────────────────────────────────────────────
```
