# Forward Engineering Completion Package

**Project:** eShopOnWeb (reverse-engineered) · **Date:** 2026-06-25
**Purpose:** Close the four critical blockers from the EARB audit (`forward-engineering-audit/`, verdict
**APPROVED WITH REMEDIATION, 79/100**) by generating ONLY the missing enterprise documentation.
**Authority (inputs, read-only):** `enterprise-foundation-package/ENTERPRISE_KNOWLEDGE_GRAPH.json` (274 nodes),
`CANONICAL_ENTERPRISE_MODEL.md`, `TRACEABILITY_MATRIX.md`, `16_GENERATION_MANIFEST.json`,
`17_FORWARD_ENGINEERING_READINESS_REPORT.md`, `forward-engineering-package/` docs 01–18.

> **Rules honored:** No existing document regenerated or modified. No application code. No invented business
> functionality. Every spec traces to a graph node id. Genuine choices are marked **🟦 REQUIRES HUMAN
> DECISION** rather than silently decided.

---

## Blocker → document map

| Audit blocker | Closed by | Section |
|---|---|---|
| **C1** No target stack decision | `01_TARGET_STACK_DECISION.md` | 1 |
| **C2** No physical data model | `02_PHYSICAL_DATA_MODEL.md` + `03_DATABASE_DDL_SPECIFICATION.md` | 2 |
| **C3** Critical security remediation | `04_SECURITY_MODERNIZATION.md` | 3 |
| **C4** Authorization verification | `05_AUTHORIZATION_MODEL.md` | 4 |
| (FE governance to make C1–C4 generatable) | `06_GENERATION_POLICY.md`, `07_IMPLEMENTATION_GUIDELINES.md` | 5–6 |
| Re-score after remediation | `08_FORWARD_ENGINEERING_COMPLETION_REPORT.md` | 7 |

## Evidence baseline (verified, reused — not re-derived)

- 15 entities (11 implemented/persisted, 3 aspirational empty, 1 abstract); 12 relationships (6 hard FK, 3 soft cross-DB, 2 owned, 1 aspirational).
- 4 aggregates (Basket, Order, CatalogItem; Buyer aspirational).
- 55 APIs (8 REST fully contracted on PublicApi; 40 Web routes; 7 synthetic/CLI).
- 17 security nodes (7 controls + 10 findings; 2 CRITICAL: TECH-SEC-008/009).
- 5 actors; 1 confirmed role (`Administrators`, RC-008); 12 business rules (BR001–BR012).

## Decision-marking convention

- ✅ **DERIVED** — fact present in the knowledge graph / FE package; cited.
- 🟦 **REQUIRES HUMAN DECISION** — a real choice not derivable from evidence; options given, no silent default.
- ⚠ **NEUTRAL DEFAULT** — a safe, reversible default offered to keep generation unblocked; overridable by the decision above.
