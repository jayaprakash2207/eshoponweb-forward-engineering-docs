# 04 — Security Modernization Architecture (closes C3)

**Closes:** Audit C3 — "Critical security remediation." Defines the target security architecture that closes
the 10 evidenced findings (TECH-SEC-008..017) and hardens the 7 controls (TECH-SEC-001..007).
**Authority:** doc 13 (Security Architecture), graph `technology.security` (17 nodes), doc 15 SR-01..09.
**Nature:** Target-state security architecture (no code). Each control maps to the finding it closes.

---

## 1. Findings → remediation map (the core of C3)

| Finding | Severity | Remediation (target control) | Doc 15 gate |
|---|---|---|---|
| **TECH-SEC-008** hardcoded PostgreSQL creds | **CRITICAL** | Remove from VCS, rotate, move to secret store (§7); secret scanning in CI | SR-06 / VR-09 |
| **TECH-SEC-009** hardcoded SQL Server SA password | **CRITICAL** | Same as above; no secrets in compose/images | SR-06 / VR-09 |
| **TECH-SEC-010** no JWT enforcement on PublicApi | HIGH | Enforce bearer validation on all non-public endpoints (§2/§5) | SR-03 / VR-05 |
| **TECH-SEC-011** no CORS policy | HIGH | Explicit origin allow-list (§5) | SR-04 / VR-05 |
| **TECH-SEC-012** no secret scanning in CI | HIGH | gitleaks/trufflehog gate (§7/§11) | SR-06 |
| **TECH-SEC-013** port 1433 published to host | MEDIUM | Bind DB to private network only (§9) | IR-05 |
| **TECH-SEC-014** no TLS for container traffic | MEDIUM | TLS at ingress/mesh; HTTPS-only (§8/§9) | SR-05 |
| **TECH-SEC-015** AllowedHosts `*` + TrustServerCertificate=true | MEDIUM | Explicit host allow-list; validate DB cert (§9) | SR-05 |
| **TECH-SEC-016** no SAST/dependency/container scan | HIGH | Add scanning gates (§11) | VR-09 |
| **TECH-SEC-017** no audit logging/compliance | MEDIUM | Structured security audit log + retention (§6) | SR-08 |

## 2. Authentication

| Aspect | Target | Source |
|---|---|---|
| Interactive users | **OAuth 2.0 / OIDC Authorization Code + PKCE** | TECH-SEC-001/003; SR-01 |
| Service-to-service | **OAuth 2.0 Client Credentials** | SR-01 |
| Token format | **JWT** (signed; short-lived access + refresh) | TECH-SEC-002 |
| Identity store | ASP.NET Core Identity (or stack equivalent) over `identity` schema | DATA-REPO-004 |
| Password storage | adaptive hash (bcrypt/argon2/PBKDF2) via `PasswordHash` | DATA-ENT-008 |
| MFA | TwoFactorEnabled column present → enable 2FA (TOTP) | doc 02 §2.8 |

> Legacy JWT/OIDC are **inferred from packages only** (TECH-SEC-002/003 LOW; OQ-005). Target therefore
> **defines enforcement as net-new** (ASMP-FE-001 in doc 13) — not assumed from legacy.

## 3. Authorization (summary — full model in `05_AUTHORIZATION_MODEL.md`)

- **Deny-by-default.** Every endpoint requires an explicit policy.
- **RBAC** anchored on the one confirmed role `Administrators` (RC-008) + an inferred `Customer`/authenticated tier.
- Catalog mutations (APP-API-005/006/007) require `Administrators`; order/manage/basket-checkout require authenticated user **+ row-level ownership** (BuyerId match).

## 4. RBAC / OAuth2 / OIDC / JWT design

```
[User] ──OIDC AuthCode+PKCE──▶ [Identity Provider] ──issues──▶ JWT (access+refresh)
   │                                                              │
   └──── Bearer JWT ─────────▶ [PublicApi / Web] ── validates ───┘
                                     │  (signature, exp, aud, iss, scope/role claims)
                                     ▼
                          [Authorization policies — deny by default]
                                     ▼
                          [Resource + row-level ownership check]
```

JWT claims: `sub` (→ BuyerId), `role` (Administrators/Customer), `scope`, standard `iss/aud/exp/iat`.
**Enforce, don't just issue** (SR-03) — closes TECH-SEC-010.

## 5. API security

| Control | Target | Closes |
|---|---|---|
| AuthN on protected endpoints | bearer validation, deny-by-default | TECH-SEC-010 |
| CORS | explicit allow-list (BlazorAdmin/SPA origin → PublicApi/Web); no `*` | TECH-SEC-011 |
| Rate limiting | per-IP + per-token throttle ⚠ (neutral baseline) | hardening |
| Input validation | FluentValidation/Bean-Validation/Pydantic per stack; reject on violation | BR001–BR012 |
| Error model | RFC 9457 problem-detail; no stack traces to clients (API-04) | hardening |
| TLS | HTTPS-only; HSTS | TECH-SEC-014 |

## 6. Audit logging (closes TECH-SEC-017)

- Structured (JSON) security events: authn success/failure, authz denials, admin actions on catalog (APP-API-005/006/007), order placement, role changes.
- Each event carries `actor (sub)`, `action`, `resource id`, `timestamp`, `outcome`, correlation id.
- Retention 🟦 (default ⚠ 1 year hot / per compliance) — REQUIRES HUMAN DECISION (regulatory scope unknown).
- No PII/secret values in logs (PII entities: Order, ApplicationUser, Address).

## 7. Secrets management (closes TECH-SEC-008/009/012)

- **No secret in source, config, or image.** Externalize to a secret store (Azure Key Vault — already
  referenced TECH-SEC-006/TECH-INF-008 — or Vault/AWS Secrets Manager per cloud choice).
- Workload identity / managed identity for store access; no static store credentials.
- **Rotate** the leaked PostgreSQL + SQL Server credentials immediately (TECH-SEC-008/009).
- CI secret scanning gate (gitleaks/trufflehog) blocks merges (TECH-SEC-012).

## 8. Encryption

| Data state | Target | Note |
|---|---|---|
| In transit | TLS 1.2+ everywhere (ingress, service-to-service, DB) | closes TECH-SEC-014 |
| At rest | DB-level encryption (TDE/native) + field-level for PII (ApplicationUser, Order, Address) | 🟦 no legacy evidence (ASMP-FE-004) — net-new requirement |
| Secrets | encrypted in store + in transit | §7 |
| Passwords | adaptive one-way hash | not "encryption" — hashing |

## 9. Database & infrastructure security

| Control | Target | Closes |
|---|---|---|
| DB network exposure | private subnet only; **do not publish 1433/5432 to host** | TECH-SEC-013 |
| DB cert | validate (no `TrustServerCertificate=true`) | TECH-SEC-015 |
| Host allow-list | explicit hosts (no `AllowedHosts=*`) | TECH-SEC-015 |
| Least privilege | app DB user scoped per schema (catalog/basket/ordering/identity) | DB-01 |
| Container | non-root, read-only FS, minimal base image | hardening |

## 10. Security headers

`Strict-Transport-Security`, `Content-Security-Policy`, `X-Content-Type-Options: nosniff`,
`X-Frame-Options: DENY`, `Referrer-Policy`, `Permissions-Policy`. Applied at ingress/app layer.

## 11. OWASP Top 10 (2021) mapping

| OWASP | Risk in legacy | Control |
|---|---|---|
| A01 Broken Access Control | TECH-SEC-010 (no enforcement); soft-ref ownership | SR-02/03, row-level ownership (§3/§5) |
| A02 Cryptographic Failures | TECH-SEC-014 (no TLS); no at-rest encryption | §8 |
| A03 Injection | input validation gaps | parameterized queries (ORM) + validation (§5) |
| A04 Insecure Design | module cycle, weak boundaries | doc 15 AR-03/AR-06 |
| A05 Security Misconfiguration | TECH-SEC-013/015 (1433 exposed, AllowedHosts *) | §9/§10 |
| A06 Vulnerable Components | TECH-SEC-016; 19 unpinned versions | §11 scanning + version pinning (ASMP-FE-016) |
| A07 Auth Failures | TECH-SEC-002/003 (unverified); no lockout config | §2 + Identity lockout columns |
| A08 Integrity Failures | no supply-chain scanning | dependency/container scan (§11) |
| A09 Logging/Monitoring Failures | TECH-SEC-017 | §6 audit logging + observability (doc 07) |
| A10 SSRF | external calls (UriComposer) | egress allow-list ⚠ |

## 12. Threat model (STRIDE — key threats)

| Threat | Vector | Mitigation |
|---|---|---|
| **Spoofing** | stolen/forged JWT | short-lived signed tokens, PKCE, validate iss/aud (§2/§4) |
| **Tampering** | order/price manipulation via API | server-side recompute (BR010), authZ (§3), input validation |
| **Repudiation** | admin denies catalog change | audit log (§6) on APP-API-005/006/007 |
| **Information disclosure** | PII leak (Order/User/Address) | encryption (§8), authZ, no PII in logs |
| **Denial of service** | unthrottled API | rate limiting (§5), resource limits (TECH-INF-004 gap) |
| **Elevation of privilege** | missing authZ on catalog mutations (TECH-SEC-010) | deny-by-default + Administrators policy (§3) |

## 13. Security best-practices checklist (DevSecOps)

- [ ] Secrets externalized + rotated (TECH-SEC-008/009)
- [ ] Secret scanning gate (TECH-SEC-012)
- [ ] SAST + dependency + container scanning gates (TECH-SEC-016)
- [ ] AuthN enforced, deny-by-default authZ (TECH-SEC-010)
- [ ] CORS allow-list (TECH-SEC-011)
- [ ] TLS everywhere + headers (TECH-SEC-014)
- [ ] DB private + cert-validated (TECH-SEC-013/015)
- [ ] Audit logging + retention (TECH-SEC-017)
- [ ] At-rest + field-level PII encryption (ASMP-FE-004)
- [ ] No payment/PCI surface (BC-06 aspirational — SR-09)

## 14. 🟦 Human decisions in security scope

| # | Decision |
|---|---|
| S1 | Identity provider choice (self-hosted Identity vs Entra ID/Auth0/Keycloak) |
| S2 | Audit-log retention period + regulatory scope (GDPR/PCI/none) |
| S3 | At-rest encryption strategy (TDE vs field-level vs both) |
| S4 | Rate-limit thresholds |
| S5 | Whether `Customer` role is explicit or implicit-authenticated (see doc 05) |
