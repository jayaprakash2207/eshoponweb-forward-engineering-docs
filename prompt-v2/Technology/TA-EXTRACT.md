# TA-EXTRACT — Technology Architecture Extraction

## 1. Metadata
- prompt_id:        TA-EXTRACT
- version:          2.0.0
- owner_layer:      TA
- role:             EXTRACT (Scout→Analyst, internally phased)
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [repo.manifests, repo.containers, repo.iac, repo.cicd, repo.config, repo.source_bodies, aa.component-registry?, da.data-store-registry?]
- produces:         [ta.inventory(6), ta.technology-stack-assessment, ta.architecture-pattern-catalog, ta.component-interaction-contract-map, ta.nfr-registry, ta.technical-debt-risk-register, ta.operational-architecture-assessment, ta.infra-transport-security-assessment]
- merges:           [TA-SCOUT-01, TA-ANALYST-01]
- last_updated:     2026-06-24

## 2. Purpose
Discover technologies, analyze infrastructure/deployment/security, and produce the Technology
Architecture — folding the two wired TA runners (StackScout + DeepAnalyst) into one phased EXTRACT.

> Merge note: legacy TA used **two separate runners** (ta_agent1 + ta_agent2). This merge is at the
> authoring surface; **Phase A (inventory) MUST emit before Phase B (analysis)** to preserve the
> Scout→Analyst evidence flow. The two-runner wiring becomes a two-phase single load (see migration guide).

## 3. Inputs
- Manifests, containers, IaC, CI/CD, config (Phase A); source bodies + full pipelines (Phase B).
- Consumes (cite): `aa.component-registry` (C-2), `da.data-store-registry` (C-1).
- Quality gate: escalate if no manifests / <60% readable (GR-7.1).

## 4. Responsibilities  (GOV-02: TA = sole owner of tech-stack)
- Tech-stack inventory, infra/deployment, CI/CD, integration graph, security-config (Phase A);
  stack assessment, architecture/infra patterns, component interactions, NFRs, tech debt, operational
  maturity, infra/transport security (Phase B).

## 5. Allowed Actions — MANDATORY PHASES
- **Phase A — Inventory (Scout)**: declarations only — manifests, Dockerfiles, IaC, CI/CD (job names, `uses:` actions, first word of each `run:` command — not full scripts), config → 6 inventory files.
- **Phase B — Analysis (Analyst)**: read method bodies for resilience/transaction/cache config + exact thresholds; assess CI/CD maturity from tool evidence; NFRs, debt, infra/transport security → 7 assessment files. *Phase A emitted first.*

## 6. Forbidden Actions
- MUST NOT produce a Data Architecture Assessment (transaction/consistency = DA) or app-level security (= AA) — keep infra/transport security only (GOV-08).
- MUST NOT skip Phase A inventory (GR-6.1); MUST NOT capture secret values (GR-5.4).

## 7. Outputs  (TA_FILE marker; legacy filenames preserved)
- Phase A (6): `technology-stack-inventory.md, component-service-map.md, data-store-registry.md, infrastructure-deployment-blueprint.md, integration-dependency-graph.md, security-configuration-snapshot.md`.
- Phase B (7): `technology-stack-assessment.md, architecture-pattern-catalog.md, component-interaction-contract-map.md, nfr-registry.md, technical-debt-risk-register.md, operational-architecture-assessment.md, infra-transport-security-assessment.md`.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. Versions verbatim; VERSION CONFLICTs flagged; each pattern/NFR cites a line; CI/CD capability cites a tool.

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: technology, nfr, technical-debt, security-control, integration.

## 10. Traceability Rules
Cumulative `NFR-/TD-` never reset; `TECH-/INFRA-/CICD-/INTEG-` cited; components/data-stores referenced by AA/DA owner IDs.

## 11. Governance Reference
Per `Shared/GOV.md` (role=EXTRACT, live_source=false, marker=TA_FILE, audience=technical).

## 12. Version Information
- 2.0.0 — 2026-06-24 — Merge of TA-SCOUT-01 + TA-ANALYST-01 into one phased EXTRACT (two-phase load replaces two runners). — Prompt Architect
- supersedes: TA-SCOUT-01, TA-ANALYST-01
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#ta
