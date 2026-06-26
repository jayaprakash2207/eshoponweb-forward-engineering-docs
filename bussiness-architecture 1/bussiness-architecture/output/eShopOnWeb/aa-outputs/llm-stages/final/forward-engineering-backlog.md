# Forward Engineering Backlog (Candidate)

> Backlog items below are derived from confirmed violations and candidate capability/consolidation maps. Priority and effort are **illustrative placeholders** pending architect review and the additional evidence noted in `confidence-report.md`.

| ID | Title | Type | Related | Priority (indicative) | Effort (indicative) | Confidence |
|----|-------|------|---------|------------------------|----------------------|------------|
| FEB-001 | Decide canonical Catalog Admin UI (Razor Pages vs Blazor) | Decision | AD-001, ARCH-VIOL-002, MCC-001 | High | Low (decision only) | 0.5 |
| FEB-002 | Document `CatalogItemOrdered` snapshot as intentional bounded-context boundary (or redesign if accidental) | Documentation / Decision | AD-002, ARCH-VIOL-001, CAP-001, CAP-003 | Medium | Low–Medium | 0.55 |
| FEB-003 | Formalize PublicApi `CatalogItemEndpoints` as a versioned contract for BlazorAdmin consumers | Technical | AD-003, ARCH-VIOL-003, API-001 | Medium | Medium | 0.5 |
| FEB-004 | Retire or merge non-canonical Catalog Admin UI once FEB-001 is decided | Technical | MCC-001, CAP-005 | Medium (depends on FEB-001) | Medium–High | 0.45 |
| FEB-005 | Enumerate full PublicApi endpoint surface (beyond CreateCatalogItemEndpoint) for contract preservation map | Discovery | API-002, CAP-007 | High | Low | 0.45 |
| FEB-006 | Obtain test/runtime evidence pack and re-run coverage analysis for all candidate capabilities | Discovery | test-runtime-evidence-map.json, all CAP-* | High | Low–Medium | 0.5 |
| FEB-007 | Obtain Infrastructure/persistence evidence to resolve database/schema ownership per aggregate | Discovery | AD-006, data-ownership-map.md | High | Low–Medium | 0.5 |
| FEB-008 | Confirm existence/scope of non-evidenced capabilities (Identity, Payment, etc.) against full system inventory | Discovery | AD-005, business-capability-map.json | Medium | Low | 0.45 |
| FEB-009 | Re-evaluate service boundary options (A/B/C) once FEB-006/FEB-007 complete | Decision | AD-004, service-boundary-options.md | Medium (sequenced after discovery) | n/a | 0.4 |

## Sequencing Note

FEB-001 through FEB-003 are **decision/documentation items** that can proceed independent of additional evidence and directly address the two Medium-severity violations (ARCH-VIOL-002, ARCH-VIOL-003). FEB-005 through FEB-008 are **discovery items** that unblock more confident versions of `business-capability-map.json`, `data-ownership-map.md`, and `service-boundary-options.md` in a future run of this stage. FEB-009 (service boundary decision) is intentionally sequenced last, consistent with the rule that this stage must not claim final service boundaries.