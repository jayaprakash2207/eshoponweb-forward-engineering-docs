# 10 — Prompt Architecture Target State

**Document ID:** GOV-10
**Version:** 1.0.0
**Status:** Canonical — end-state design
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) (all findings & scores)
**Synthesizes:** GOV-01 … GOV-09

---

## 1. Current State (from audit)

| Dimension | Current | Evidence |
|---|---|---|
| Paradigms | **3 incompatible** (Scout+Analyst MD · AA staged · SLM/LLM JSON) | §3.3 |
| Confidence schemes | **3** (numeric · categorical · gate) | §3.2 |
| Cross-layer violations | **4 confirmed** (TA→Data, TA→App/Sec, AA→Business, AA→Data) | §4.1 |
| Duplicate extraction | data-store ×5, business-rules ×4, stack ×3, components ×3 | §4.2 |
| Governance | duplicated 2–7×; centralized only in AA | §3.1 |
| Synthesis layer | **none in-pipeline** (graph built externally) | finding 10 |
| Model pinning / versioning | **none** | finding 6 |
| Scores | Uniformity 46 · Duplication 63 (severity) · Layer-Sep 54 · Quality 64 | §0 |

```
CURRENT (schematic)
 Layer1 ──► BA (A) ─┐  inline gov, numeric? no—categorical
        ──► DA (A) ─┤  inline gov, numeric confidence, owns data (clean)
        ──► AA (B) ─┤  central gov (AA-only), BUT Stage05 leaks BA+DA
        ──► TA (A) ─┘  inline gov, OUT4/OUT5 leak Data+App
 (no convergence) ─► graph built OUTSIDE pipeline
```

---

## 2. Target State

```
TARGET
 Layer1 (raw feed, non-authoritative)
   │
   ├─► BA  ┐   one template (GOV-03), one gov (CMP-GOV→GOV-01),
   ├─► DA  ┤   one confidence model (CMP-CONF→GOV-04),
   ├─► AA  ┤   single-owner extraction (GOV-02), boundaries enforced (GOV-08)
   └─► TA  ┘
          │  owner-cited facts via contracts C-1..C-4 (GOV-07)
          ▼
        FN — Foundation/Synthesis (GOV-05): reconcile · resolve · graph · validate
          │
          ▼
        Forward-Engineering Package (consumes FN only)
```

**Defining properties of the target:**
1. **One paradigm** — the AA staged model generalized: every prompt = GOV-03's 12 sections; Scout→Analyst→Review intra-layer; FN synthesis terminal.
2. **One governance source** — GOV-01, included via CMP-GOV; zero inline duplication.
3. **One confidence model** — GOV-04, included via CMP-CONF; numeric band derived for tooling only.
4. **Single ownership** — GOV-02; each fact extracted once; others consume + cite.
5. **Hard boundaries** — GOV-08 "Must Not Produce/Own" makes the four violations un-representable.
6. **First-class Foundation** — reconciliation moves inside the pipeline (GOV-05).
7. **Reproducible** — model pinned + prompt/component versions in the run manifest (GR-10).
8. **Reusable** — five components (GOV-09) replace copied blocks.

---

## 3. Transition Plan (waves — see GOV-06 §3)

| Wave | Theme | Breaking? | Closes |
|---|---|---|---|
| 0 | Publish GOV-01..04, GOV-08; demote AA `00-global-rules` to pointer | No | F4, F6, §3.2 |
| 1 | Apply GOV-03 metadata + GOV-04 + components to clean prompts; archive P3 | No | F1, F7, finding 9 |
| 2 | Relocate mis-owned outputs (TA OUT4→DA, TA OUT5→AA, AA Stage05→BA/DA) | Behavior-preserving (same artifacts, new owner) | F3 |
| 3 | Enforce single-owner extraction; non-owners switch to consume+cite | Behavior-preserving | F2 |
| 4 | Build Foundation layer (FN-SYNTH/REVIEW); wire after BA/DA/AA/TA | Additive (supersets output) | finding 10, R1 |
| 5 | `common/` orchestration module + model pinning + run manifest | Non-functional plumbing | F5, F6 |

**Behavioral-equivalence guarantee:** Waves 0–1, 5 change governance/metadata/plumbing only. Waves 2–3
change *which prompt* emits an artifact, not the artifact set. Wave 4 adds reconciliation that previously
happened outside the pipeline. Net: **functional behavior preserved; outputs reconciled and traceable.**

---

## 4. Benefits

| Benefit | Mechanism | Audit metric improved |
|---|---|---|
| Single source of truth for rules | GOV-01 + CMP-GOV | Duplication ↓, Uniformity ↑ |
| Comparable confidence everywhere | GOV-04 + CMP-CONF | Uniformity ↑ |
| No contradictory facts | GOV-05 reconciliation + single ownership | R1 eliminated, Quality ↑ |
| Boundaries can't be breached | GOV-08 Must-Not lists + review gate | Layer-Sep ↑ |
| ~15–25% fewer instructional tokens | GOV-09 includes + caching | Token efficiency ↑ |
| Reproducible audits | GR-10 model/version pinning | R3 eliminated |
| One-edit rule changes | components, not copies | R2 (maintenance) eliminated |
| Forward-engineering-ready | FN canonical graph + traceability | FE readiness ↑ |

---

## 5. Risks (of the transition)

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Relocation (Wave 2–3) accidentally drops an output | Medium | High | Output-set diff before/after each wave; FN traceability proves coverage |
| Consume-instead-of-extract weakens a layer that relied on its own copy | Medium | Medium | Contracts C-1..C-6 guarantee owner facts + confidence are available first |
| Foundation reconciliation surfaces many DISCREPANCYs initially | High | Medium | Expected & healthy; route to open_questions; resolve by GR-2.2, not arbitrarily |
| Include assembler not yet built (code deferred) | High | Low | Interim: prompts cite component IDs; review gate checks no inline dup |
| Model pin changes results vs historical runs | Low | Medium | Record model+versions in manifest; treat as a versioned baseline |
| Team adheres to old paradigms out of habit | Medium | Medium | Conformance checklist (GOV-03 §4) is a release blocker |

---

## 6. Readiness Score

Projected scores **after full transition** (same methodology as audit §0), with current → target:

| Score | Current | Target | Driver |
|---|---:|---:|---|
| Uniformity (↑ better) | 46 | **90** | 1 paradigm, 1 confidence model, 1 template |
| Duplication severity (↓ better) | 63 | **15** | components replace 2–7× copies |
| Layer Separation (↑ better) | 54 | **92** | 4 violations removed; single ownership; GOV-08 |
| Architecture Quality (↑ better) | 64 | **88** | + Foundation, + reproducibility, − drift |

**Composite enterprise readiness:**

| Phase | Readiness | Meaning |
|---|---|---|
| Today (audit) | **NOT READY** | fragmented governance, active violations |
| After Wave 1 | **PARTIAL** | unified governance/confidence/metadata; clean prompts conformant |
| After Wave 3 | **MOSTLY READY** | violations removed, single ownership enforced |
| After Wave 4–5 | **ENTERPRISE READY** | Foundation reconciliation + reproducibility complete |

> Mapping per GOV-04 §5: NOT READY = FAIL, PARTIAL = PARTIAL, MOSTLY/ENTERPRISE READY = PASS.

---

## 7. Definition of Done (enterprise prompt architecture)

- [ ] Every prompt is GOV-03 conformant (12 sections, metadata, version).
- [ ] Zero inline governance; all reference GOV-01 via CMP-GOV.
- [ ] One confidence model (GOV-04) in use; the other two schemes deleted.
- [ ] GOV-02 single ownership enforced; no duplicate extraction.
- [ ] GOV-08 boundaries pass the review gate for every prompt.
- [ ] Foundation layer produces the knowledge graph + 4 views in-pipeline (GOV-05).
- [ ] Dependency graph is an acyclic DAG terminating at FN (GOV-07).
- [ ] Model + prompt/component versions pinned in the run manifest (GR-10).
- [ ] P3 archived; AA `00-global-rules` demoted to a GOV-01 pointer.

When all are checked, the architecture is **TOGAF-aligned, Enterprise-Architecture-aligned,
AI-Governance-aligned, and Forward-Engineering-ready.**
