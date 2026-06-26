# Layer 3 Agent — BA Document Generation Instructions

## Your Role
You are a Senior Business Architect writing final Business Architecture documents.
You will receive structured JSON output from Layer 2 (business analysis) and must generate 10 BA documents in Markdown format.

## What You Are Receiving
```
business_rules        — extracted IF/THEN rules with source locations
business_entities     — real-world objects the system manages
process_sequences     — step-by-step business processes
user_roles            — who uses the system and what they do
capability_candidates — grouped business capability areas
```

## Your Output — 10 BA Documents
Generate all 10 documents. Each document starts with `===DOCUMENT_START:<filename>===` and ends with `===DOCUMENT_END===` so the runner can split and save them.

---

### Document 1 — Capability Map (01_capability_map.md)

Structure as a 3-level hierarchy. Use the capability_candidates as L1, break into L2 sub-capabilities, and L3 specific functions.

```
===DOCUMENT_START:01_capability_map.md===
# Business Capability Map
## [Application Name]

> Extracted from legacy codebase. Status: Active/Dormant per code evidence.

### L1: [Capability Area]
#### L2: [Sub-Capability]
- **L3: [Function]** — [one line description] `[ACTIVE]`

[repeat for all capabilities]

---
**Summary**
| Level | Count |
|---|---|
| L1 Capabilities | N |
| L2 Sub-Capabilities | N |
| L3 Functions | N |
===DOCUMENT_END===
```

---

### Document 2 — Value Stream Map (02_value_stream.md)

Identify the main end-to-end value streams from the processes. Show stages, actors, and handoffs.

```
===DOCUMENT_START:02_value_stream.md===
# Value Stream Map
## [Application Name]

### Value Stream: [Name] (e.g. Customer Order to Delivery)

| Stage | Actor | Description | Value-Add |
|---|---|---|---|
| 1. [Stage] | [Who] | [What happens] | Yes/No |

**Handoff Points:**
- Stage N -> Stage N+1: [what is handed off]

[repeat for each value stream]
===DOCUMENT_END===
```

---

### Document 3 — Business Process Models (03_process_models.md)

One section per process. Include trigger, steps, decision points, and end result.

```
===DOCUMENT_START:03_process_models.md===
# Business Process Models
## [Application Name]

---
### Process: [Name]
**Trigger:** [what starts this process]
**Actors:** [list]
**End Result:** [what is produced]

**Steps:**
1. [Actor] — [Action]
2. [Actor] — [Action]
   - Decision: [condition]?
     - YES -> Step N
     - NO  -> Step N

**Business Rules Applied:** BR001, BR002

---
[repeat for each process]
===DOCUMENT_END===
```

---

### Document 4 — Business Rules Inventory (04_business_rules.md)

Full catalog of all extracted business rules. Group by category.

```
===DOCUMENT_START:04_business_rules.md===
# Business Rules Inventory
## [Application Name]

> Total Rules: N | Critical: N | High: N | Medium: N | Low: N

### Validation Rules
| Rule ID | Business Statement | Priority | Source |
|---|---|---|---|
| BR001 | IF [condition] THEN [action] | Critical | ClassName.cs |

### Calculation Rules
[same table format]

### Approval Rules
[same table format]

### Restriction Rules
[same table format]

---
### Config-Driven Rules
Rules whose values come from configuration (can be changed without code deployment):
| Rule ID | Parameter | Current Value | Effect |
|---|---|---|---|
===DOCUMENT_END===
```

---

### Document 5 — Information / Data Model (05_data_model.md)

Business-language description of all entities and their relationships.

```
===DOCUMENT_START:05_data_model.md===
# Information / Data Model
## [Application Name]

### Core Entities

#### [Entity Name]
- **Business Definition:** [what this represents in business terms]
- **Key Attributes:** [list the important ones with business meaning]
- **States/Lifecycle:** [e.g. Pending -> Confirmed -> Shipped -> Delivered]
- **Relationships:**
  - Has many [Entity]
  - Belongs to [Entity]
- **Business Rules:** BR001, BR004

[repeat for all entities]

---
### Entity Relationship Summary
| Entity | Relates To | Relationship Type |
|---|---|---|
===DOCUMENT_END===
```

---

### Document 6 — Stakeholder Map (06_stakeholder_map.md)

Based on user_roles extracted. Describe each stakeholder's interaction with the system.

```
===DOCUMENT_START:06_stakeholder_map.md===
# Stakeholder Map
## [Application Name]

### Internal Stakeholders (System Users)

#### [Role Name]
- **Responsibilities:** [what they do]
- **System Access:** [which modules/screens they use]
- **Key Processes:** [which processes they participate in]
- **Permission Level:** [standard/elevated/admin]

[repeat for each role]

### External Stakeholders
[customers, partners, regulators — infer from code evidence]

---
> Note: Influence levels and org hierarchy require business validation.
===DOCUMENT_END===
```

---

### Document 7 — KPIs & Metrics (07_kpis_metrics.md)

Infer measurable business metrics from the rules, processes, and config values.

```
===DOCUMENT_START:07_kpis_metrics.md===
# KPIs & Metrics
## [Application Name]

### Operational Metrics (extracted from code/config)
| Metric | Source | Current Threshold | Business Meaning |
|---|---|---|---|
| [metric name] | config key / rule | value | [what it measures] |

### Process Metrics
| Process | Metric | How Measured |
|---|---|---|

### Business Health Indicators
[inferred from business rules — e.g. order approval rate, discount usage]

---
> Note: Target values and benchmarks require business input.
===DOCUMENT_END===
```

---

### Document 8 — Business Motivation Model (08_motivation_model.md)

Infer from application purpose, entity names, process names. Mark clearly as inferred.

```
===DOCUMENT_START:08_motivation_model.md===
# Business Motivation Model
## [Application Name]

> INFERRED from code analysis — requires business validation

### Mission (inferred)
[What this application exists to do — one paragraph]

### Business Drivers (inferred)
- [Driver 1]: [evidence from code]
- [Driver 2]: [evidence from code]

### Goals (inferred from processes)
| Goal | Evidence | Process |
|---|---|---|

### Constraints (extracted from rules)
| Constraint | Source Rule | Business Impact |
|---|---|---|
===DOCUMENT_END===
```

---

### Document 9 — Operating Model (09_operating_model.md)

Infer organizational structure from roles, processes, and access patterns.

```
===DOCUMENT_START:09_operating_model.md===
# Operating Model
## [Application Name]

> INFERRED from code analysis — requires business validation

### Organizational Roles Identified
[from user_roles — who does what]

### Decision Authority (inferred)
| Decision | Who Decides | Rule Reference |
|---|---|---|
| [e.g. Approve large orders] | [Manager role] | BR003 |

### Process Ownership (inferred)
| Process | Owner Role | Supporting Roles |
|---|---|---|

---
> Note: Org chart and reporting lines require HR/business input.
===DOCUMENT_END===
```

---

### Document 10 — Business Roadmap (10_business_roadmap.md)

Based on what is active, dormant, or missing — what should be improved.

```
===DOCUMENT_START:10_business_roadmap.md===
# Business Roadmap
## [Application Name]

> INFERRED from code analysis — requires business strategy input

### Current State Summary
- Capabilities working well: [list]
- Capabilities needing improvement: [list based on complexity/rules count]
- Missing capabilities (gaps): [infer from process gaps]

### Recommended Upgrade Priorities
| Priority | Capability | Reason | Complexity |
|---|---|---|---|
| 1 | [capability] | [why] | High/Med/Low |

### Quick Wins (Low effort, high value)
[list from config-driven rules — easy to change]

### Technical Debt Identified
[patterns from code that signal poor structure]

---
> Note: Timeline and investment decisions require business leadership input.
===DOCUMENT_END===
```

---

## Rules for Generation
- Replace `[Application Name]` with the actual app name from the data
- Replace all `[placeholders]` with real content from the Layer 2 data
- Mark inferred content clearly with `> INFERRED` blocks
- Every business rule reference must match an actual BR ID from the data
- Keep language business-friendly — no code terms, no technical jargon
- Be specific — use actual entity names, rule values, method names translated to business language

---
## LAYER 2 DATA SECTION BELOW — generate all 10 documents from this:


```json
{
  "analysis_metadata": {
    "source_application": "eShopOnWeb",
    "analysis_date": "2026-06-15",
    "total_business_rules": 12,
    "total_entities": 8,
    "total_processes": 5,
    "total_roles": 3,
    "total_capabilities": 6
  },
  "business_rules": [
    {
      "rule_id": "BR001",
      "rule_name": "Catalog item details validation",
      "business_statement": "IF a catalog item's name, description, or price is missing or the price is zero/negative THEN reject the update to catalog item details",
      "category": "validation",
      "priority": "high",
      "source_method": "UpdateDetails",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR002",
      "rule_name": "Catalog item brand assignment validation",
      "business_statement": "IF a catalog brand ID of zero is provided THEN reject the brand assignment for the catalog item",
      "category": "validation",
      "priority": "medium",
      "source_method": "UpdateBrand",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR003",
      "rule_name": "Catalog item type assignment validation",
      "business_statement": "IF a catalog type ID of zero is provided THEN reject the type assignment for the catalog item",
      "category": "validation",
      "priority": "medium",
      "source_method": "UpdateType",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR004",
      "rule_name": "Catalog item picture URI generation",
      "business_statement": "IF a picture file name is provided for a catalog item THEN generate a unique image path under images/products; IF no picture name is provided THEN clear the picture URI",
      "category": "calculation",
      "priority": "low",
      "source_method": "UpdatePictureUri",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR005",
      "rule_name": "Basket item consolidation",
      "business_statement": "IF a customer adds a catalog item already present in their basket THEN increase the quantity of the existing basket line instead of creating a new line; ELSE add a new basket line item",
      "category": "calculation",
      "priority": "critical",
      "source_method": "AddItem",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR006",
      "rule_name": "Remove empty basket lines",
      "business_statement": "IF a basket line item has a quantity of zero THEN remove it from the basket",
      "category": "restriction",
      "priority": "medium",
      "source_method": "RemoveEmptyItems",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR007",
      "rule_name": "Basket item quantity must be non-negative",
      "business_statement": "IF a quantity adjustment to a basket item would result in a negative value THEN reject the quantity update",
      "category": "validation",
      "priority": "high",
      "source_method": "AddQuantity",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR008",
      "rule_name": "Buyer identity required",
      "business_statement": "IF a buyer record is created without a valid identity reference THEN reject buyer creation",
      "category": "validation",
      "priority": "critical",
      "source_method": "Buyer_constructor",
      "source_file": "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR009",
      "rule_name": "Ordered item details required",
      "business_statement": "IF an order line item is created without a valid catalog item ID, product name, or picture URI THEN reject creation of that order line item",
      "category": "validation",
      "priority": "critical",
      "source_method": "CatalogItemOrdered_constructor",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR010",
      "rule_name": "Order total calculation",
      "business_statement": "An order's total is calculated as the sum of each ordered item's unit price multiplied by its quantity",
      "category": "calculation",
      "priority": "critical",
      "source_method": "Total",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR011",
      "rule_name": "Order requires buyer reference",
      "business_statement": "IF an order is created without a buyer ID THEN reject order creation",
      "category": "validation",
      "priority": "critical",
      "source_method": "Order_constructor",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
      "config_driven": false,
      "config_key": null
    },
    {
      "rule_id": "BR012",
      "rule_name": "Empty basket cannot be checked out",
      "business_statement": "IF a customer attempts checkout with no items in their basket THEN block the checkout and raise an empty-basket error",
      "category": "restriction",
      "priority": "critical",
      "source_method": "EmptyBasketOnCheckout / CreateOrderAsync",
      "source_file": "src/ApplicationCore/Extensions/GuardExtensions.cs",
      "config_driven": false,
      "config_key": null
    }
  ],
  "business_entities": [
    {
      "entity_name": "CatalogItem",
      "business_definition": "A product available for purchase in the store catalog, including its name, description, price, image, brand, and type classification",
      "attributes": [
        "Id",
        "Name",
        "Description",
        "Price",
        "PictureUri",
        "CatalogBrandId",
        "CatalogTypeId"
      ],
      "states": [],
      "states_source": "inferred",
      "relationships": [
        "belongs to CatalogBrand",
        "belongs to CatalogType",
        "referenced by BasketItem",
        "referenced by CatalogItemOrdered"
      ],
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs"
    },
    {
      "entity_name": "CatalogType",
      "business_definition": "A classification/category of catalog items (e.g., product type)",
      "attributes": [
        "Id",
        "Type"
      ],
      "states": [],
      "states_source": "inferred",
      "relationships": [
        "categorizes CatalogItem"
      ],
      "source_file": "src/ApplicationCore/Entities/CatalogType.cs"
    },
    {
      "entity_name": "CatalogBrand",
      "business_definition": "The manufacturer or brand associated with a catalog item",
      "attributes": [
        "Id",
        "Brand"
      ],
      "states": [],
      "states_source": "inferred",
      "relationships": [
        "branding for CatalogItem"
      ],
      "source_file": "src/ApplicationCore/Entities/CatalogBrand.cs"
    },
    {
      "entity_name": "Basket",
      "business_definition": "A shopping cart that holds items a buyer intends to purchase, either as an anonymous or registered user",
      "attributes": [
        "Id",
        "BuyerId",
        "Items"
      ],
      "states": [],
      "states_source": "inferred",
      "relationships": [
        "belongs to Buyer",
        "contains BasketItems",
        "converted into Order"
      ],
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/Basket.cs"
    },
    {
      "entity_name": "BasketItem",
      "business_definition": "A single line item in a shopping basket representing a quantity of a catalog item at a given price",
      "attributes": [
        "Id",
        "CatalogItemId",
        "UnitPrice",
        "Quantity"
      ],
      "states": [],
      "states_source": "inferred",
      "relationships": [
        "belongs to Basket",
        "references CatalogItem"
      ],
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs"
    },
    {
      "entity_name": "Buyer",
      "business_definition": "A registered customer who places orders and stores payment methods, identified by their identity provider account",
      "attributes": [
        "Id",
        "IdentityGuid",
        "PaymentMethods"
      ],
      "states": [],
      "states_source": "inferred",
      "relationships": [
        "owns Basket",
        "places Order",
        "has PaymentMethod"
      ],
      "source_file": "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs"
    },
    {
      "entity_name": "Order",
      "business_definition": "A confirmed purchase placed by a buyer, containing ordered items and a shipping address",
      "attributes": [
        "Id",
        "BuyerId",
        "ShipToAddress",
        "OrderItems"
      ],
      "states": [],
      "states_source": "inferred",
      "relationships": [
        "belongs to Buyer",
        "contains OrderItems",
        "created from Basket",
        "ships to Address"
      ],
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Order.cs"
    },
    {
      "entity_name": "OrderItem",
      "business_definition": "A single purchased line within an order, capturing the ordered catalog item, its price at time of purchase, and quantity",
      "attributes": [
        "Id",
        "ItemOrdered",
        "UnitPrice",
        "Units"
      ],
      "states": [],
      "states_source": "inferred",
      "relationships": [
        "belongs to Order",
        "references CatalogItemOrdered (snapshot of CatalogItem)"
      ],
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs"
    }
  ],
  "process_sequences": [
    {
      "process_name": "Add Item to Basket",
      "trigger": "Customer (or anonymous user) selects a catalog item to purchase",
      "actors": [
        "Customer",
        "System"
      ],
      "steps": [
        {
          "step": 1,
          "action": "System retrieves the customer's existing basket, or creates a new one if none exists",
          "actor": "System",
          "method_reference": "BasketService.AddItemToBasket"
        },
        {
          "step": 2,
          "action": "System adds the item to the basket, consolidating quantity if the item is already present",
          "actor": "System",
          "method_reference": "Basket.AddItem"
        },
        {
          "step": 3,
          "action": "System persists the updated basket",
          "actor": "System",
          "method_reference": "BasketService.AddItemToBasket"
        }
      ],
      "decision_points": [
        {
          "at_step": 1,
          "condition": "Does a basket already exist for this user?",
          "yes_path": "Use existing basket",
          "no_path": "Create a new basket for the user"
        },
        {
          "at_step": 2,
          "condition": "Is the catalog item already in the basket?",
          "yes_path": "Increase quantity of existing basket item",
          "no_path": "Add a new basket line item"
        }
      ],
      "end_result": "Basket contains the requested item with correct quantity"
    },
    {
      "process_name": "Transfer Anonymous Basket to Registered User",
      "trigger": "An anonymous user logs in or registers",
      "actors": [
        "Customer",
        "System"
      ],
      "steps": [
        {
          "step": 1,
          "action": "System retrieves the anonymous user's basket",
          "actor": "System",
          "method_reference": "BasketService.TransferBasketAsync"
        },
        {
          "step": 2,
          "action": "System retrieves or creates a basket for the now-registered user",
          "actor": "System",
          "method_reference": "BasketService.TransferBasketAsync"
        },
        {
          "step": 3,
          "action": "System copies each item from the anonymous basket into the user's basket",
          "actor": "System",
          "method_reference": "Basket.AddItem"
        }
      ],
      "decision_points": [
        {
          "at_step": 1,
          "condition": "Does the anonymous basket exist?",
          "yes_path": "Continue transfer",
          "no_path": "End process - nothing to transfer"
        },
        {
          "at_step": 2,
          "condition": "Does the registered user already have a basket?",
          "yes_path": "Merge items into existing basket",
          "no_path": "Create a new basket for the user"
        }
      ],
      "end_result": "Registered user's basket contains all items from their anonymous session"
    },
    {
      "process_name": "Checkout / Place Order",
      "trigger": "Customer initiates checkout from their basket",
      "actors": [
        "Customer",
        "System"
      ],
      "steps": [
        {
          "step": 1,
          "action": "System retrieves the customer's basket with items",
          "actor": "System",
          "method_reference": "OrderService.CreateOrderAsync"
        },
        {
          "step": 2,
          "action": "System verifies the basket exists and is not empty",
          "actor": "System",
          "method_reference": "OrderService.CreateOrderAsync / GuardExtensions.EmptyBasketOnCheckout"
        },
        {
          "step": 3,
          "action": "System retrieves full catalog item details for each basket item",
          "actor": "System",
          "method_reference": "OrderService.CreateOrderAsync"
        },
        {
          "step": 4,
          "action": "System creates a snapshot of each ordered item (name, picture, price) for the order record",
          "actor": "System",
          "method_reference": "CatalogItemOrdered_constructor"
        },
        {
          "step": 5,
          "action": "System creates the order with the buyer ID, shipping address, and order items",
          "actor": "System",
          "method_reference": "Order_constructor"
        },
        {
          "step": 6,
          "action": "System calculates the order total from item prices and quantities",
          "actor": "System",
          "method_reference": "Order.Total"
        }
      ],
      "decision_points": [
        {
          "at_step": 2,
          "condition": "Is the basket empty?",
          "yes_path": "Block checkout and throw EmptyBasketOnCheckoutException",
          "no_path": "Continue to order creation"
        }
      ],
      "end_result": "Order is created from the basket contents with a calculated total and shipping address"
    },
    {
      "process_name": "Catalog Item Administration",
      "trigger": "Administrator manages catalog items via the admin (Blazor) interface",
      "actors": [
        "Administrator",
        "System"
      ],
      "steps": [
        {
          "step": 1,
          "action": "Administrator views the list of catalog items, types, and brands",
          "actor": "Administrator",
          "method_reference": "List.razor.OnAfterRenderAsync"
        },
        {
          "step": 2,
          "action": "Administrator creates a new catalog item",
          "actor": "Administrator",
          "method_reference": "CachedCatalogItemServiceDecorator.Create / CatalogItemService.Create"
        },
        {
          "step": 3,
          "action": "Administrator deletes an existing catalog item",
          "actor": "Administrator",
          "method_reference": "CachedCatalogItemServiceDecorator.Delete / CatalogItemService.Delete"
        },
        {
          "step": 4,
          "action": "System refreshes the cached local catalog item list after create/delete",
          "actor": "System",
          "method_reference": "CachedCatalogItemServiceDecorator.RefreshLocalStorageList"
        }
      ],
      "decision_points": [],
      "end_result": "Catalog item data is updated and reflected in the admin UI"
    },
    {
      "process_name": "User Authentication",
      "trigger": "User submits login credentials via the API",
      "actors": [
        "Customer",
        "Administrator",
        "System"
      ],
      "steps": [
        {
          "step": 1,
          "action": "User submits username and password to the authentication endpoint",
          "actor": "Customer",
          "method_reference": "AuthenticateEndpoint.HandleAsync"
        },
        {
          "step": 2,
          "action": "System validates credentials against identity store",
          "actor": "System",
          "method_reference": "AuthenticateEndpoint.HandleAsync"
        },
        {
          "step": 3,
          "action": "System generates a signed JWT token containing user identity and role claims",
          "actor": "System",
          "method_reference": "IdentityTokenClaimService.GetTokenAsync"
        }
      ],
      "decision_points": [
        {
          "at_step": 2,
          "condition": "Are the credentials valid and is the account not locked out?",
          "yes_path": "Issue authentication token with success result",
          "no_path": "Return failed result with IsLockedOut/IsNotAllowed flags"
        }
      ],
      "end_result": "User receives a JWT token containing their identity and role claims for subsequent API access"
    }
  ],
  "user_roles": [
    {
      "role_name": "Customer / Buyer",
      "responsibilities": [
        "Browse catalog items, types, and brands",
        "Manage shopping basket (add items, adjust quantities)",
        "Transfer anonymous basket to account on login",
        "Place orders and view order history",
        "Manage payment methods and shipping address"
      ],
      "system_access": [
        "CatalogBrandEndpoints",
        "CatalogItemEndpoints",
        "BasketService",
        "OrderService",
        "AuthenticateEndpoint"
      ],
      "permission_level": "standard"
    },
    {
      "role_name": "Administrator",
      "responsibilities": [
        "Create, update, and delete catalog items",
        "Manage catalog brands and types",
        "View and manage application data via Blazor Admin"
      ],
      "system_access": [
        "CatalogItemService (BlazorAdmin)",
        "CachedCatalogItemServiceDecorator",
        "CatalogLookupDataService",
        "List.razor (CatalogItemPage)"
      ],
      "permission_level": "admin"
    },
    {
      "role_name": "System / Service Account",
      "responsibilities": [
        "Seed initial catalog and identity data on startup",
        "Issue authentication tokens",
        "Compose image URIs and compute basket/order totals",
        "Send notification emails (not yet implemented)"
      ],
      "system_access": [
        "CatalogContextSeed",
        "AppIdentityDbContextSeed",
        "IdentityTokenClaimService",
        "UriComposer",
        "EmailSender"
      ],
      "permission_level": "elevated"
    }
  ],
  "capability_candidates": [
    {
      "capability_name": "Catalog Management",
      "description": "Manage the catalog of products, including their details, pricing, classification (type/brand), and images",
      "supporting_classes": [
        "CatalogItem",
        "CatalogType",
        "CatalogBrand",
        "CatalogItemService",
        "CachedCatalogItemServiceDecorator",
        "CatalogLookupDataService",
        "CatalogContextSeed"
      ],
      "business_rules_count": 4,
      "complexity": "medium"
    },
    {
      "capability_name": "Basket / Shopping Cart Management",
      "description": "Manage customer shopping carts, including adding items, consolidating quantities, removing empty lines, and merging anonymous and registered baskets",
      "supporting_classes": [
        "Basket",
        "BasketItem",
        "BasketService",
        "BasketQueryService"
      ],
      "business_rules_count": 3,
      "complexity": "medium"
    },
    {
      "capability_name": "Order Management",
      "description": "Convert a customer's basket into a confirmed order, snapshot ordered item details, and calculate order totals",
      "supporting_classes": [
        "Order",
        "OrderItem",
        "CatalogItemOrdered",
        "Address",
        "OrderService"
      ],
      "business_rules_count": 4,
      "complexity": "high"
    },
    {
      "capability_name": "Buyer / Customer Profile Management",
      "description": "Maintain customer identity and associated payment methods",
      "supporting_classes": [
        "Buyer",
        "PaymentMethod"
      ],
      "business_rules_count": 1,
      "complexity": "low"
    },
    {
      "capability_name": "Identity & Authentication",
      "description": "Authenticate users, manage roles, and issue access tokens for API and admin access",
      "supporting_classes": [
        "AuthenticateEndpoint",
        "IdentityTokenClaimService",
        "AppIdentityDbContextSeed",
        "AuthorizationConstants"
      ],
      "business_rules_count": 0,
      "complexity": "medium"
    },
    {
      "capability_name": "Admin Catalog Operations (Blazor)",
      "description": "Provide an administrative interface for managing catalog items, types, and brands with cached data services",
      "supporting_classes": [
        "CatalogItemService",
        "CachedCatalogItemServiceDecorator",
        "CachedCatalogLookupDataServiceDecorator",
        "CatalogLookupDataService",
        "List (CatalogItemPage)",
        "ToastService"
      ],
      "business_rules_count": 0,
      "complexity": "medium"
    }
  ]
}
```