# 06 — Forward Engineering Risk Report

**Board:** Chief EA + SQA Lead + AI FE Specialist
**Date:** 2026-06-25
**Purpose:** Risk that AI-driven generation from this package yields an incorrect, insecure, or non-buildable system. Each risk: likelihood × impact, evidence, mitigation.

---

## 1. Risk register

| ID | Risk | Likelihood | Impact | Severity | Evidence | Mitigation (in package) |
|---|---|---|---|---|---|---|
| **R-01** | Generated system **reproduces the module cycle** | Med | High | 🔴 | APP-DEP-001 / OQ-004 | AR-03 + VR-03 DAG [GATE] block release even if cycle is unreal (ASMP-FE-151) |
| **R-02** | **Insecure generation** — no auth on catalog mutations, plaintext secrets carried forward | High (if gates skipped) | Critical | 🔴 | TECH-SEC-008/009/010/011 | SR-02/03/04/06 + VR-05/VR-09 [GATE]; but depends on enforcement, not just spec |
| **R-03** | **Non-buildable schema** — generator lacks physical types/nullability | High | High | 🔴 | doc 07 logical-only; G-C2 | DB-04 neutral-type mapping exists but **physical model must be authored first** |
| **R-04** | **Wrong DTO shapes** for 40 UI routes (inferred, not evidenced) | Med | Med | 🟠 | doc 11; ASMP-FE-105 | Verify against source before implementation; 8 REST endpoints are safe (fully contracted) |
| **R-05** | **Version drift / CVEs** — 19 deps unpinned | High | Med | 🟠 | doc 12 §2.2 | ASMP-FE-016 requires pinning + SAST/dep scan (VR-09) before build |
| **R-06** | **Aspirational features generated as real** (Buyer/Payment/CatalogItemDetails, EVT-11/12) | Low (if GR-05 honored) | High | 🟠 | RC-002; GR-05/VR-07 | Status-flag gates enforce SKIP; risk only if gates bypassed |
| **R-07** | **Data loss on context split** — shared CatalogContext → per-context DBs | Med | High | 🟠 | RISK-SHARED-DBCTX-001; DB-01 | Requires explicit migration plan (not in package — G-M9) |
| **R-08** | **Cross-DB integrity violations** — soft refs unenforced | Med | Med | 🟡 | DATA-REL-008/009 | Application-level enforcement must be generated; document the rule |
| **R-09** | **Unvalidated NFRs** — derived targets fail in production | Med | Med | 🟡 | doc 14 derived | Establish measured baselines post-generation (ASMP-FE-001..019) |
| **R-10** | **No production deployment** — generated app cannot be deployed beyond local | High | Med | 🟠 | doc 18 §18.7 | Author IaC/K8s/release (doc 18 §18.6 neutral options) |
| **R-11** | **System name/branding wrong** (system_name=unknown) | Low | Low | 🟡 | OQ-007/ASSUMP-007 | Confirm canonical name before branding artifacts |
| **R-12** | **Manifest drift** from graph over time | Low | Med | 🟡 | doc 17 RR-08 | Regenerate + re-validate manifest on any graph change (manifest currently valid) |
| **R-13** | **Identity columns missing** (lockout/2FA/security-stamp) | Med | Med | 🟠 | ASMP-DD-003 | Verify actual IdentityDb schema before final DDL |

---

## 2. Risk concentration

```
        IMPACT →     Low            Medium                 High/Critical
  LIKELIHOOD ↓
  High                              R-05, R-10             R-02 🔴, R-03 🔴
  Medium             R-11           R-08, R-09, R-12       R-01 🔴, R-07, R-13
  Low                              R-06 (gated)
```

**Three risks sit in the high-impact/high-likelihood zone:** R-02 (insecure generation), R-03 (non-buildable schema), R-01 (cycle reproduction). All three have **gates in doc 15**, but R-02/R-03 depend on **prerequisite work** (security remediation, physical model) the package does not itself contain.

---

## 3. Residual risk after applying package gates

| If the team... | Residual risk |
|---|---|
| Honors all 68 [GATE] rules in doc 15 | R-01, R-06 → **Low** (DAG + status gates effective) |
| Authors a physical data model first | R-03 → **Low** |
| Completes security remediation + verifies OQ-005 | R-02 → **Low** |
| Verifies DTO shapes + pins versions | R-04, R-05 → **Low** |
| Authors production IaC + migration plan | R-07, R-10 → **Medium→Low** |
| Skips/short-circuits gates | R-01/R-02/R-03 → **Critical** |

**The package's gates are necessary but not sufficient** — three risks require prerequisite artifacts (physical model, security remediation, IaC) that live outside it.

---

## 4. SQA position

From a quality-assurance standpoint, the package is **safe to generate from ONLY under enforced gating + mandatory human code review** (doc 15 VR-01..12). The strongest safeguards are the status-flag discipline (prevents aspirational generation) and the DAG/auth gates. The weakest points are the **derived NFRs** (no measured acceptance baseline) and the **absent physical/production artifacts** (schema + deployment).

**Highest-priority risk controls (ordered):**
1. Physical data model authored + reviewed (controls R-03).
2. Security remediation + OQ-005 verification (controls R-02).
3. Stack decision + version pinning (controls R-05, enables build).
4. Migration plan for CatalogContext split (controls R-07).
5. Production IaC authored (controls R-10).

**Overall FE risk: MODERATE — manageable with enforced gating and the five prerequisite controls above; HIGH if gates are bypassed.**
