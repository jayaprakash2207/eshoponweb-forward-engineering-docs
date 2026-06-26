# Layer 2 Agent — Business Architecture Extraction Instructions

## Your Role
You are a Senior Business Architect analyzing extracted artifacts from a legacy application.
You will receive structured JSON data from Layer 1 (code extraction) and must produce a structured business analysis in JSON format.

## What You Are Receiving
The data passed to you contains:
- `source_code` — extracted methods, classes, interfaces, enums from the codebase
- `database` — tables, EF entities, stored procedures, triggers
- `config` — business parameters, feature flags, connection strings

## Your 5 Tasks

---

### Task 1 — Extract Business Rules
Look at every method marked `is_business_artifact: true`.
Find IF/THEN conditions, validations, calculations, approvals.

For each rule produce:
```json
{
  "rule_id": "BR001",
  "rule_name": "Short business name",
  "business_statement": "IF [condition] THEN [action]",
  "category": "validation | calculation | approval | restriction | notification",
  "priority": "critical | high | medium | low",
  "source_method": "MethodName",
  "source_file": "path/to/file.cs",
  "config_driven": true,
  "config_key": "appsettings key if applicable"
}
```

---

### Task 2 — Extract Business Entities
Look at classes, EF entities, database tables, enums.
Identify what real-world business objects they represent.

For each entity produce:
```json
{
  "entity_name": "Order",
  "business_definition": "A customer request to purchase one or more products",
  "attributes": ["Id", "Status", "TotalAmount", "BuyerId"],
  "states": ["Pending", "Confirmed", "Cancelled"],
  "states_source": "enum OrderStatus | inferred",
  "relationships": ["belongs to Customer", "contains OrderItems"],
  "source_file": "path/to/file.cs"
}
```

> **Fix 4 rule:** For `states_source`, write the enum name if a status/state enum exists in the source data (e.g. `"enum OrderStatus"`). Write `"inferred"` if no such enum is present — this flags that the lifecycle was reasoned from context, not confirmed by code.

---

### Task 3 — Map Process Sequences
Look at method names, class names, and business_category tags.
Reconstruct the end-to-end business processes step by step.

For each process produce:
```json
{
  "process_name": "Place Order",
  "trigger": "What starts this process",
  "actors": ["Customer", "System", "Manager"],
  "steps": [
    {
      "step": 1,
      "action": "Customer adds items to basket",
      "actor": "Customer",
      "method_reference": "BasketService.AddItem"
    }
  ],
  "decision_points": [
    {
      "at_step": 2,
      "condition": "Is customer authenticated?",
      "yes_path": "Continue to checkout",
      "no_path": "Redirect to login"
    }
  ],
  "end_result": "Order is created and confirmed"
}
```

---

### Task 4 — Identify User Roles
Look at class names containing Controller, Service, Authorization.
Look at config role definitions and permission entries.

For each role produce:
```json
{
  "role_name": "Buyer",
  "responsibilities": ["Browse catalog", "Place orders", "View order history"],
  "system_access": ["BasketController", "OrderController", "CatalogController"],
  "permission_level": "standard | elevated | admin"
}
```

---

### Task 5 — Map Capability Candidates
Group all findings into business capability areas.
Use the method names, class names, and namespaces as signals.

For each capability produce:
```json
{
  "capability_name": "Order Management",
  "description": "What the business can do in this area",
  "supporting_classes": ["OrderService", "OrderRepository"],
  "business_rules_count": 4,
  "complexity": "high | medium | low"
}
```

---

## Output Format
Return a single valid JSON object — no markdown, no explanation outside the JSON:

```json
{
  "analysis_metadata": {
    "source_application": "app name if identifiable",
    "analysis_date": "today's date",
    "total_business_rules": 0,
    "total_entities": 0,
    "total_processes": 0,
    "total_roles": 0,
    "total_capabilities": 0
  },
  "business_rules": [],
  "business_entities": [],
  "process_sequences": [],
  "user_roles": [],
  "capability_candidates": []
}
```

## Important Rules
- Focus ONLY on business logic — skip infrastructure, logging, error handling code
- Write business_statement in plain English — no code syntax
- If a rule value comes from config (appsettings), note the config key
- Mark priority as `critical` if the rule blocks a core action (order, payment, login)
- Be specific — avoid vague statements like "processes data"

---
## DATA SECTION BELOW — analyze everything that follows:


```json
{
  "business_methods": [
    {
      "name": "UpdateDetails",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "public void UpdateDetails(CatalogItemDetails details)\n    {\n        Guard.Against.NullOrEmpty(details.Name, nameof(details.Name));\n        Guard.Against.NullOrEmpty(details.Description, nameof(details.Description));\n        Guard.Against.NegativeOrZero(details.Price, nameof(details.Price));\n\n        Name = details.Name;\n        Description = details.Description;\n        Price = details.Price;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "return_type": "void",
        "line_number": 33
      }
    },
    {
      "name": "UpdateBrand",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "public void UpdateBrand(int catalogBrandId)\n    {\n        Guard.Against.Zero(catalogBrandId, nameof(catalogBrandId));\n        CatalogBrandId = catalogBrandId;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "return_type": "void",
        "line_number": 44
      }
    },
    {
      "name": "UpdateType",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "public void UpdateType(int catalogTypeId)\n    {\n        Guard.Against.Zero(catalogTypeId, nameof(catalogTypeId));\n        CatalogTypeId = catalogTypeId;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "return_type": "void",
        "line_number": 50
      }
    },
    {
      "name": "UpdatePictureUri",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "public void UpdatePictureUri(string pictureName)\n    {\n        if (string.IsNullOrEmpty(pictureName))\n        {\n            PictureUri = string.Empty;\n            return;\n        }\n        PictureUri = $\"images\\\\products\\\\{pictureName}?{new DateTime().Ticks}\";\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "return_type": "void",
        "line_number": 56
      }
    },
    {
      "name": "Price",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "decimal Price",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "property_type": "decimal"
      }
    },
    {
      "name": "CatalogTypeId",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "int CatalogTypeId",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "property_type": "int"
      }
    },
    {
      "name": "CatalogType",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "CatalogType? CatalogType",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "property_type": "CatalogType?"
      }
    },
    {
      "name": "Type",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/ApplicationCore/Entities/CatalogType.cs",
      "content": "string Type",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "property_type": "string"
      }
    },
    {
      "name": "AddItem",
      "type": "method",
      "business_category": "process",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
      "content": "public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1)\n    {\n        if (!Items.Any(i => i.CatalogItemId == catalogItemId))\n        {\n            _items.Add(new BasketItem(catalogItemId, quantity, unitPrice));\n            return;\n        }\n        var existingItem = Items.First(i => i.CatalogItemId == catalogItemId);\n        existingItem.AddQuantity(quantity);\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate",
        "return_type": "void",
        "line_number": 22
      }
    },
    {
      "name": "RemoveEmptyItems",
      "type": "method",
      "business_category": "process",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
      "content": "public void RemoveEmptyItems()\n    {\n        _items.RemoveAll(i => i.Quantity == 0);\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate",
        "return_type": "void",
        "line_number": 33
      }
    },
    {
      "name": "AddQuantity",
      "type": "method",
      "business_category": "process",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
      "content": "public void AddQuantity(int quantity)\n    {\n        Guard.Against.OutOfRange(quantity, nameof(quantity), 0, int.MaxValue);\n\n        Quantity += quantity;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate",
        "return_type": "void",
        "line_number": 20
      }
    },
    {
      "name": "UnitPrice",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
      "content": "decimal UnitPrice",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate",
        "property_type": "decimal"
      }
    },
    {
      "name": "Quantity",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
      "content": "int Quantity",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate",
        "property_type": "int"
      }
    },
    {
      "name": "Buyer_constructor",
      "type": "constructor",
      "business_category": "validation",
      "source_file": "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
      "content": "public Buyer(string identity) : this()\n    {\n        Guard.Against.NullOrEmpty(identity, nameof(identity));\n        IdentityGuid = identity;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BuyerAggregate",
        "constructor_class": "Buyer",
        "guard_checks": [
          "NullOrEmpty"
        ],
        "line_number": 18
      }
    },
    {
      "name": "State",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
      "content": "string State",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "property_type": "string"
      }
    },
    {
      "name": "Country",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
      "content": "string Country",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "property_type": "string"
      }
    },
    {
      "name": "CatalogItemOrdered_constructor",
      "type": "constructor",
      "business_category": "validation",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
      "content": "public CatalogItemOrdered(int catalogItemId, string productName, string pictureUri)\n    {\n        Guard.Against.OutOfRange(catalogItemId, nameof(catalogItemId), 1, int.MaxValue);\n        Guard.Against.NullOrEmpty(productName, nameof(productName));\n        Guard.Against.NullOrEmpty(pictureUri, nameof(pictureUri));\n\n        CatalogItemId = catalogItemId;\n        ProductName = productName;\n        PictureUri = pictureUri;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "constructor_class": "CatalogItemOrdered",
        "guard_checks": [
          "OutOfRange",
          "NullOrEmpty",
          "NullOrEmpty"
        ],
        "line_number": 11
      }
    },
    {
      "name": "Total",
      "type": "method",
      "business_category": "calculation",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
      "content": "public decimal Total()\n    {\n        var total = 0m;\n        foreach (var item in _orderItems)\n        {\n            total += item.UnitPrice * item.Units;\n        }\n        return total;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "return_type": "decimal",
        "line_number": 38
      }
    },
    {
      "name": "Order_constructor",
      "type": "constructor",
      "business_category": "validation",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
      "content": "public Order(string buyerId, Address shipToAddress, List<OrderItem> items)\n    {\n        Guard.Against.NullOrEmpty(buyerId, nameof(buyerId));\n\n        BuyerId = buyerId;\n        ShipToAddress = shipToAddress;\n        _orderItems = items;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "constructor_class": "Order",
        "guard_checks": [
          "NullOrEmpty"
        ],
        "line_number": 13
      }
    },
    {
      "name": "EmptyBasketOnCheckout",
      "type": "method",
      "business_category": "validation",
      "source_file": "src/ApplicationCore/Extensions/GuardExtensions.cs",
      "content": "public static void EmptyBasketOnCheckout(this IGuardClause guardClause, IReadOnlyCollection<BasketItem> basketItems)\n    {\n        if (!basketItems.Any())\n            throw new EmptyBasketOnCheckoutException();\n    }",
      "metadata": {
        "namespace": "Ardalis.GuardClauses",
        "return_type": "void",
        "line_number": 10
      }
    },
    {
      "name": "IAggregateRoot",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IAggregateRoot.cs",
      "content": "interface IAggregateRoot",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IAppLogger",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IAppLogger.cs",
      "content": "interface IAppLogger",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IBasketQueryService",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IBasketQueryService.cs",
      "content": "interface IBasketQueryService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IBasketService",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IBasketService.cs",
      "content": "interface IBasketService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IEmailSender",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IEmailSender.cs",
      "content": "interface IEmailSender",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IOrderService",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IOrderService.cs",
      "content": "interface IOrderService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IReadRepository",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IReadRepository.cs",
      "content": "interface IReadRepository",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IRepository",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IRepository.cs",
      "content": "interface IRepository",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "ITokenClaimsService",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/ITokenClaimsService.cs",
      "content": "interface ITokenClaimsService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IUriComposer",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/ApplicationCore/Interfaces/IUriComposer.cs",
      "content": "interface IUriComposer",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "BasketService",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/ApplicationCore/Services/BasketService.cs",
      "content": "class BasketService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "is_service_class": true
      }
    },
    {
      "name": "AddItemToBasket",
      "type": "method",
      "business_category": "process",
      "source_file": "src/ApplicationCore/Services/BasketService.cs",
      "content": "public async Task<Basket> AddItemToBasket(string username, int catalogItemId, decimal price, int quantity = 1)\n    {\n        var basketSpec = new BasketWithItemsSpecification(username);\n        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);\n\n        if (basket == null)\n        {\n            basket = new Basket(username);\n            await _basketRepository.AddAsync(basket);\n        }\n\n        basket.AddItem(catalogItemId, price, quantity);\n\n        await _basketRepository.UpdateAsync(basket);\n        return basket;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "return_type": "Task<Basket>",
        "line_number": 23
      }
    },
    {
      "name": "DeleteBasketAsync",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/ApplicationCore/Services/BasketService.cs",
      "content": "public async Task DeleteBasketAsync(int basketId)\n    {\n        var basket = await _basketRepository.GetByIdAsync(basketId);\n        Guard.Against.Null(basket, nameof(basket));\n        await _basketRepository.DeleteAsync(basket);\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "return_type": "Task",
        "line_number": 40
      }
    },
    {
      "name": "TransferBasketAsync",
      "type": "method",
      "business_category": "process",
      "source_file": "src/ApplicationCore/Services/BasketService.cs",
      "content": "public async Task TransferBasketAsync(string anonymousId, string userName)\n    {\n        var anonymousBasketSpec = new BasketWithItemsSpecification(anonymousId);\n        var anonymousBasket = await _basketRepository.FirstOrDefaultAsync(anonymousBasketSpec);\n        if (anonymousBasket == null) return;\n        var userBasketSpec = new BasketWithItemsSpecification(userName);\n        var userBasket = await _basketRepository.FirstOrDefaultAsync(userBasketSpec);\n        if (userBasket == null)\n        {\n            userBasket = new Basket(userName);\n            await _basketRepository.AddAsync(userBasket);\n        }\n        foreach (var item in anonymousBasket.Items)\n        {\n            userBasket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity);\n        }\n        await _basketRepos",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "return_type": "Task",
        "line_number": 66
      }
    },
    {
      "name": "OrderService",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/ApplicationCore/Services/OrderService.cs",
      "content": "class OrderService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "is_service_class": true
      }
    },
    {
      "name": "CreateOrderAsync",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/ApplicationCore/Services/OrderService.cs",
      "content": "public async Task CreateOrderAsync(int basketId, Address shippingAddress)\n    {\n        var basketSpec = new BasketWithItemsSpecification(basketId);\n        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);\n\n        Guard.Against.Null(basket, nameof(basket));\n        Guard.Against.EmptyBasketOnCheckout(basket.Items);\n\n        var catalogItemsSpecification = new CatalogItemsSpecification(basket.Items.Select(item => item.CatalogItemId).ToArray());\n        var catalogItems = await _itemRepository.ListAsync(catalogItemsSpecification);\n\n        var items = basket.Items.Select(basketItem =>\n        {\n            var catalogItem = catalogItems.First(c => c.Id == basketItem.CatalogItemId);\n            var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, _u",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "return_type": "Task",
        "line_number": 30
      }
    },
    {
      "name": "GetAuthenticationStateAsync",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/CustomAuthStateProvider.cs",
      "content": "public override async Task<AuthenticationState> GetAuthenticationStateAsync()\n    {\n        return new AuthenticationState(await GetUser(useCache: true));\n    }",
      "metadata": {
        "namespace": "BlazorAdmin",
        "return_type": "Task<AuthenticationState>",
        "line_number": 31
      }
    },
    {
      "name": "AddBlazorServices",
      "type": "method",
      "business_category": "process",
      "source_file": "src/BlazorAdmin/ServicesConfiguration.cs",
      "content": "public static IServiceCollection AddBlazorServices(this IServiceCollection services)\n    {\n        services.AddScoped<ICatalogLookupDataService<CatalogBrand>, CachedCatalogLookupDataServiceDecorator<CatalogBrand, CatalogBrandResponse>>();\n        services.AddScoped<CatalogLookupDataService<CatalogBrand, CatalogBrandResponse>>();\n        services.AddScoped<ICatalogLookupDataService<CatalogType>, CachedCatalogLookupDataServiceDecorator<CatalogType, CatalogTypeResponse>>();\n        services.AddScoped<CatalogLookupDataService<CatalogType, CatalogTypeResponse>>();\n        services.AddScoped<ICatalogItemService, CachedCatalogItemServiceDecorator>();\n        services.AddScoped<CatalogItemService>();\n\n        return services;\n    }",
      "metadata": {
        "namespace": "BlazorAdmin",
        "return_type": "IServiceCollection",
        "line_number": 10
      }
    },
    {
      "name": "BuildToastSettings",
      "type": "method",
      "business_category": "general",
      "source_file": "src/BlazorAdmin/Helpers/ToastComponent.cs",
      "content": "private void BuildToastSettings(ToastLevel level, string message)\n    {\n        switch (level)\n        {\n            case ToastLevel.Info:\n                BackgroundCssClass = \"bg-info\";\n                IconCssClass = \"info\";\n                Heading = \"Info\";\n                break;\n            case ToastLevel.Success:\n                BackgroundCssClass = \"bg-success\";\n                IconCssClass = \"check\";\n                Heading = \"Success\";\n                break;\n            case ToastLevel.Warning:\n                BackgroundCssClass = \"bg-warning\";\n                IconCssClass = \"exclamation\";\n                Heading = \"Warning\";\n                break;\n            case ToastLevel.Error:\n                BackgroundCssClass = \"bg-danger\";\n                IconCssClass = \"times\";\n          ",
      "metadata": {
        "namespace": "BlazorAdmin.Helpers",
        "return_type": "void",
        "line_number": 56
      }
    },
    {
      "name": "DeleteCookie",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/JavaScript/Cookies.cs",
      "content": "public async Task DeleteCookie(string name)\n    {\n        await _jsRuntime.InvokeAsync<string>(JSInteropConstants.DeleteCookie, name);\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.JavaScript",
        "return_type": "Task",
        "line_number": 15
      }
    },
    {
      "name": "OnAfterRenderAsync",
      "type": "method",
      "business_category": "integration",
      "source_file": "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs",
      "content": "protected override async Task OnAfterRenderAsync(bool firstRender)\n    {\n        if (firstRender)\n        {\n            catalogItems = await CatalogItemService.List();\n            catalogTypes = await CatalogTypeService.List();\n            catalogBrands = await CatalogBrandService.List();\n\n            CallRequestRefresh();\n        }\n\n        await base.OnAfterRenderAsync(firstRender);\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.Pages.CatalogItemPage",
        "return_type": "Task",
        "line_number": 29
      }
    },
    {
      "name": "CreateClick",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs",
      "content": "private async Task CreateClick()\n    {\n        await CreateComponent.Open();\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.Pages.CatalogItemPage",
        "return_type": "Task",
        "line_number": 48
      }
    },
    {
      "name": "DeleteClick",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs",
      "content": "private async Task DeleteClick(int id)\n    {\n        await DeleteComponent.Open(id);\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.Pages.CatalogItemPage",
        "return_type": "Task",
        "line_number": 58
      }
    },
    {
      "name": "CatalogTypeService",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs",
      "content": "ICatalogLookupDataService<CatalogType> CatalogTypeService",
      "metadata": {
        "namespace": "BlazorAdmin.Pages.CatalogItemPage",
        "property_type": "ICatalogLookupDataService<CatalogType>"
      }
    },
    {
      "name": "Create",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
      "content": "public async Task<CatalogItem> Create(CreateCatalogItemRequest catalogItem)\n    {\n        var result = await _catalogItemService.Create(catalogItem);\n        await RefreshLocalStorageList();\n\n        return result;\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "return_type": "Task<CatalogItem>",
        "line_number": 80
      }
    },
    {
      "name": "Delete",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
      "content": "public async Task<string> Delete(int id)\n    {\n        var result = await _catalogItemService.Delete(id);\n        await RefreshLocalStorageList();\n\n        return result;\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "return_type": "Task<string>",
        "line_number": 96
      }
    },
    {
      "name": "CatalogItemService",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/BlazorAdmin/Services/CatalogItemService.cs",
      "content": "class CatalogItemService",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": true
      }
    },
    {
      "name": "Create",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/Services/CatalogItemService.cs",
      "content": "public async Task<CatalogItem> Create(CreateCatalogItemRequest catalogItem)\n    {\n        var response = await _httpService.HttpPost<CreateCatalogItemResponse>(\"catalog-items\", catalogItem);\n        return response?.CatalogItem;\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "return_type": "Task<CatalogItem>",
        "line_number": 29
      }
    },
    {
      "name": "Delete",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/Services/CatalogItemService.cs",
      "content": "public async Task<string> Delete(int catalogItemId)\n    {\n        return (await _httpService.HttpDelete<DeleteCatalogItemResponse>(\"catalog-items\", catalogItemId)).Status;\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "return_type": "Task<string>",
        "line_number": 40
      }
    },
    {
      "name": "CatalogLookupDataService",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/BlazorAdmin/Services/CatalogLookupDataService.cs",
      "content": "class CatalogLookupDataService",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": true
      }
    },
    {
      "name": "HttpService",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/BlazorAdmin/Services/HttpService.cs",
      "content": "class HttpService",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": true
      }
    },
    {
      "name": "HttpDelete",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/BlazorAdmin/Services/HttpService.cs",
      "content": "public async Task<T> HttpDelete<T>(string uri, int id)\n        where T : class\n    {\n        var result = await _httpClient.DeleteAsync($\"{_apiUrl}{uri}/{id}\");\n        if (!result.IsSuccessStatusCode)\n        {\n            return null;\n        }\n\n        return await FromHttpResponseMessage<T>(result);\n    }",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "return_type": "Task<T>",
        "line_number": 37
      }
    },
    {
      "name": "ToastService",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/BlazorAdmin/Services/ToastService.cs",
      "content": "class ToastService",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": true
      }
    },
    {
      "name": "ToastLevel",
      "type": "enum",
      "business_category": "business_state",
      "source_file": "src/BlazorAdmin/Services/ToastService.cs",
      "content": "public enum ToastLevel\n{\n    Info,\n    Success,\n    Warning,\n    Error\n}",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "values": [
          "Info",
          "Success",
          "Warning",
          "Error"
        ]
      }
    },
    {
      "name": "NameClaimType",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/BlazorShared/Authorization/UserInfo.cs",
      "content": "string NameClaimType",
      "metadata": {
        "namespace": "BlazorShared.Authorization",
        "property_type": "string"
      }
    },
    {
      "name": "RoleClaimType",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/BlazorShared/Authorization/UserInfo.cs",
      "content": "string RoleClaimType",
      "metadata": {
        "namespace": "BlazorShared.Authorization",
        "property_type": "string"
      }
    },
    {
      "name": "ICatalogItemService",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/BlazorShared/Interfaces/ICatalogItemService.cs",
      "content": "interface ICatalogItemService",
      "metadata": {
        "namespace": "BlazorShared.Interfaces"
      }
    },
    {
      "name": "ICatalogLookupDataService",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/BlazorShared/Interfaces/ICatalogLookupDataService.cs",
      "content": "interface ICatalogLookupDataService",
      "metadata": {
        "namespace": "BlazorShared.Interfaces"
      }
    },
    {
      "name": "ILookupDataResponse",
      "type": "interface",
      "business_category": "contract_definition",
      "source_file": "src/BlazorShared/Interfaces/ILookupDataResponse.cs",
      "content": "interface ILookupDataResponse",
      "metadata": {
        "namespace": "BlazorShared.Interfaces"
      }
    },
    {
      "name": "CatalogType",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/BlazorShared/Models/CatalogItem.cs",
      "content": "string CatalogType",
      "metadata": {
        "namespace": "BlazorShared.Models",
        "property_type": "string"
      }
    },
    {
      "name": "PageCount",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/BlazorShared/Models/PagedCatalogItemResponse.cs",
      "content": "int PageCount",
      "metadata": {
        "namespace": "BlazorShared.Models",
        "property_type": "int"
      }
    },
    {
      "name": "CatalogTypes",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/Infrastructure/Data/CatalogContext.cs",
      "content": "DbSet<CatalogType> CatalogTypes",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Data",
        "property_type": "DbSet<CatalogType>"
      }
    },
    {
      "name": "SeedAsync",
      "type": "method",
      "business_category": "process",
      "source_file": "src/Infrastructure/Data/CatalogContextSeed.cs",
      "content": "public static async Task SeedAsync(CatalogContext catalogContext,\n        ILogger logger,\n        int retry = 0)\n    {\n        var retryForAvailability = retry;\n        try\n        {\n            if (catalogContext.Database.IsRelational())\n            {\n                await catalogContext.Database.MigrateAsync();\n            }\n\n            if (!await catalogContext.CatalogBrands.AnyAsync())\n            {\n                await catalogContext.CatalogBrands.AddRangeAsync(\n                    GetPreconfiguredCatalogBrands());\n\n                await catalogContext.SaveChangesAsync();\n            }\n\n            if (!await catalogContext.CatalogTypes.AnyAsync())\n            {\n                await catalogContext.CatalogTypes.AddRangeAsync(\n                    GetPreconfiguredCatalogTypes());\n\n   ",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Data",
        "return_type": "Task",
        "line_number": 12
      }
    },
    {
      "name": "EfRepository",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/Infrastructure/Data/EfRepository.cs",
      "content": "class EfRepository",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Data",
        "is_service_class": true
      }
    },
    {
      "name": "Type",
      "type": "property",
      "business_category": "business_attribute",
      "source_file": "src/Infrastructure/Data/FileItem.cs",
      "content": "string? Type",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Data",
        "property_type": "string?"
      }
    },
    {
      "name": "BasketQueryService",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/Infrastructure/Data/Queries/BasketQueryService.cs",
      "content": "class BasketQueryService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Data.Queries",
        "is_service_class": true
      }
    },
    {
      "name": "CountTotalBasketItems",
      "type": "method",
      "business_category": "calculation",
      "source_file": "src/Infrastructure/Data/Queries/BasketQueryService.cs",
      "content": "public async Task<int> CountTotalBasketItems(string username)\n    {\n        var totalItems = await _dbContext.Baskets\n            .Where(basket => basket.BuyerId == username)\n            .SelectMany(item => item.Items)\n            .SumAsync(sum => sum.Quantity);\n\n        return totalItems;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Data.Queries",
        "return_type": "Task<int>",
        "line_number": 22
      }
    },
    {
      "name": "SeedAsync",
      "type": "method",
      "business_category": "process",
      "source_file": "src/Infrastructure/Identity/AppIdentityDbContextSeed.cs",
      "content": "public static async Task SeedAsync(AppIdentityDbContext identityDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)\n    {\n\n        if (identityDbContext.Database.IsRelational())\n        {\n            await identityDbContext.Database.MigrateAsync();\n        }\n\n        await roleManager.CreateAsync(new IdentityRole(BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS));\n\n        var defaultUser = new ApplicationUser { UserName = \"demouser@microsoft.com\", Email = \"demouser@microsoft.com\" };\n        await userManager.CreateAsync(defaultUser, AuthorizationConstants.DEFAULT_PASSWORD);\n\n        string adminUserName = \"admin@microsoft.com\";\n        var adminUser = new ApplicationUser { UserName = adminUserName, Email = adminUserName };\n        await use",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Identity",
        "return_type": "Task",
        "line_number": 10
      }
    },
    {
      "name": "IdentityTokenClaimService",
      "type": "class",
      "business_category": "class_definition",
      "source_file": "src/Infrastructure/Identity/IdentityTokenClaimService.cs",
      "content": "class IdentityTokenClaimService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Identity",
        "is_service_class": true
      }
    },
    {
      "name": "GetTokenAsync",
      "type": "method",
      "business_category": "data_operation",
      "source_file": "src/Infrastructure/Identity/IdentityTokenClaimService.cs",
      "content": "public async Task<string> GetTokenAsync(string userName)\n    {\n        var tokenHandler = new JwtSecurityTokenHandler();\n        var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);\n        var user = await _userManager.FindByNameAsync(userName);\n        if (user == null) throw new UserNotFoundException(userName);\n        var roles = await _userManager.GetRolesAsync(user);\n        var claims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };\n\n        foreach (var role in roles)\n        {\n            claims.Add(new Claim(ClaimTypes.Role, role));\n        }\n\n        var tokenDescriptor = new SecurityTokenDescriptor\n        {\n            Subject = new ClaimsIdentity(claims.ToArray()),\n            Expires = DateTime.UtcNow.AddDays(7),\n            SigningCredentials",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Identity",
        "return_type": "Task<string>",
        "line_number": 23
      }
    },
    {
      "name": "LogInformation",
      "type": "method",
      "business_category": "authentication",
      "source_file": "src/Infrastructure/Logging/LoggerAdapter.cs",
      "content": "public void LogInformation(string message, params object[] args)\n    {\n        _logger.LogInformation(message, args);\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Logging",
        "return_type": "void",
        "line_number": 19
      }
    },
    {
      "name": "SendEmailAsync",
      "type": "method",
      "business_category": "notification",
      "source_file": "src/Infrastructure/Services/EmailSender.cs",
      "content": "public Task SendEmailAsync(string email, string subject, string message)\n    {\n        // TODO: Wire this up to actual email sending logic via SendGrid, local SMTP, etc.\n        return Task.CompletedTask;\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.Infrastructure.Services",
        "return_type": "Task",
        "line_number": 10
      }
    },
    {
      "name": "Apply",
      "type": "method",
      "business_category": "process",
      "source_file": "src/PublicApi/CustomSchemaFilters.cs",
      "content": "public void Apply(OpenApiSchema schema, SchemaFilterContext context)\n    {\n        var excludeProperties = new[] { \"CorrelationId\" };\n\n        foreach (var prop in excludeProperties)\n            if (schema.Properties.ContainsKey(prop))\n                schema.Properties.Remove(prop);\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.PublicApi",
        "return_type": "void",
        "line_number": 8
      }
    },
    {
      "name": "HandleAsync",
      "type": "method",
      "business_category": "process",
      "source_file": "src/PublicApi/AuthEndpoints/AuthenticateEndpoint.cs",
      "content": "public override async Task<ActionResult<AuthenticateResponse>> HandleAsync(AuthenticateRequest request,\n        CancellationToken cancellationToken = default)\n    {\n        var response = new AuthenticateResponse(request.CorrelationId());\n\n        // This doesn't count login failures towards account lockout\n        // To enable password failures to trigger account lockout, set lockoutOnFailure: true\n        //var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);\n        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);\n\n        response.Result = result.Succeeded;\n        response.IsLockedOut = result.IsLockedOut;\n        response.IsNotAllowed = result.IsNotAllowed;\n",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.PublicApi.AuthEndpoints",
        "return_type": "Task<ActionResult<AuthenticateResponse>>",
        "line_number": 36
      }
    },
    {
      "name": "AddRoute",
      "type": "method",
      "business_category": "process",
      "source_file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
      "content": "public void AddRoute(IEndpointRouteBuilder app)\n    {\n        app.MapGet(\"api/catalog-brands\",\n            async (IRepository<CatalogBrand> catalogBrandRepository) =>\n            {\n                return await HandleAsync(catalogBrandRepository);\n            })\n           .Produces<ListCatalogBrandsResponse>()\n           .WithTags(\"CatalogBrandEndpoints\");\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.PublicApi.CatalogBrandEndpoints",
        "return_type": "void",
        "line_number": 25
      }
    },
    {
      "name": "HandleAsync",
      "type": "method",
      "business_category": "process",
      "source_file": "src/PublicApi/CatalogBrandEndpoints/CatalogBrandListEndpoint.cs",
      "content": "public async Task<IResult> HandleAsync(IRepository<CatalogBrand> catalogBrandRepository)\n    {\n        var response = new ListCatalogBrandsResponse();\n\n        var items = await catalogBrandRepository.ListAsync();\n\n        response.CatalogBrands.AddRange(items.Select(_mapper.Map<CatalogBrandDto>));\n\n        return Results.Ok(response);\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.PublicApi.CatalogBrandEndpoints",
        "return_type": "Task<IResult>",
        "line_number": 36
      }
    },
    {
      "name": "AddRoute",
      "type": "method",
      "business_category": "process",
      "source_file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
      "content": "public void AddRoute(IEndpointRouteBuilder app)\n    {\n        app.MapGet(\"api/catalog-items/{catalogItemId}\",\n            async (int catalogItemId, IRepository<CatalogItem> itemRepository) =>\n            {\n                return await HandleAsync(new GetByIdCatalogItemRequest(catalogItemId), itemRepository);\n            })\n            .Produces<GetByIdCatalogItemResponse>()\n            .WithTags(\"CatalogItemEndpoints\");\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints",
        "return_type": "void",
        "line_number": 23
      }
    },
    {
      "name": "HandleAsync",
      "type": "method",
      "business_category": "process",
      "source_file": "src/PublicApi/CatalogItemEndpoints/CatalogItemGetByIdEndpoint.cs",
      "content": "public async Task<IResult> HandleAsync(GetByIdCatalogItemRequest request, IRepository<CatalogItem> itemRepository)\n    {\n        var response = new GetByIdCatalogItemResponse(request.CorrelationId());\n\n        var item = await itemRepository.GetByIdAsync(request.CatalogItemId);\n        if (item is null)\n            return Results.NotFound();\n\n        response.CatalogItem = new CatalogItemDto\n        {\n            Id = item.Id,\n            CatalogBrandId = item.CatalogBrandId,\n            CatalogTypeId = item.CatalogTypeId,\n            Description = item.Description,\n            Name = item.Name,\n            PictureUri = _uriComposer.ComposePicUri(item.PictureUri),\n            Price = item.Price\n        };\n        return Results.Ok(response);\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints",
        "return_type": "Task<IResult>",
        "line_number": 34
      }
    },
    {
      "name": "AddRoute",
      "type": "method",
      "business_category": "process",
      "source_file": "src/PublicApi/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs",
      "content": "public void AddRoute(IEndpointRouteBuilder app)\n    {\n        app.MapGet(\"api/catalog-items\",\n            async (int? pageSize, int? pageIndex, int? catalogBrandId, int? catalogTypeId, IRepository<CatalogItem> itemRepository) =>\n            {\n                return await HandleAsync(new ListPagedCatalogItemRequest(pageSize, pageIndex, catalogBrandId, catalogTypeId), itemRepository);\n            })\n            .Produces<ListPagedCatalogItemResponse>()\n            .WithTags(\"CatalogItemEndpoints\");\n    }",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints",
        "return_type": "void",
        "line_number": 29
      }
    },
    {
      "name": "HandleAsync",
      "type": "method",
      "business_category": "process",
      "source_file": "src/PublicApi/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs",
      "content": "public async Task<IResult> HandleAsync(ListPagedCatalogItemRequest request, IRepository<CatalogItem> itemRepository)\n    {\n        await Task.Delay(1000);\n        var response = new ListPagedCatalogItemResponse(request.CorrelationId());\n\n        var filterSpec = new CatalogFilterSpecification(request.CatalogBrandId, request.CatalogTypeId);\n        int totalItems = await itemRepository.CountAsync(filterSpec);\n\n        var pagedSpec = new CatalogFilterPaginatedSpecification(\n            skip: request.PageIndex * request.PageSize,\n            take: request.PageSize,\n            brandId: request.CatalogBrandId,\n            typeId: request.CatalogTypeId);\n\n        var items = await itemRepository.ListAsync(pagedSpec);\n\n        response.CatalogItems.AddRange(items.Select(_mapper.Map<CatalogItemD",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints",
        "return_type": "Task<IResult>",
        "line_number": 40
      }
    }
  ],
  "structural_types": [
    {
      "name": "CatalogSettings",
      "type": "class",
      "source_file": "src/ApplicationCore/CatalogSettings.cs"
    },
    {
      "name": "AuthorizationConstants",
      "type": "class",
      "source_file": "src/ApplicationCore/Constants/AuthorizationConstants.cs"
    },
    {
      "name": "BaseEntity",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BaseEntity.cs"
    },
    {
      "name": "CatalogBrand",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/CatalogBrand.cs"
    },
    {
      "name": "CatalogItem",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs"
    },
    {
      "name": "CatalogType",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/CatalogType.cs"
    },
    {
      "name": "Basket",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/Basket.cs"
    },
    {
      "name": "BasketItem",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs"
    },
    {
      "name": "Buyer",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs"
    },
    {
      "name": "PaymentMethod",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs"
    },
    {
      "name": "Address",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Address.cs"
    },
    {
      "name": "CatalogItemOrdered",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs"
    },
    {
      "name": "Order",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Order.cs"
    },
    {
      "name": "OrderItem",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs"
    },
    {
      "name": "BasketNotFoundException",
      "type": "class",
      "source_file": "src/ApplicationCore/Exceptions/BasketNotFoundException.cs"
    },
    {
      "name": "DuplicateException",
      "type": "class",
      "source_file": "src/ApplicationCore/Exceptions/DuplicateException.cs"
    },
    {
      "name": "EmptyBasketOnCheckoutException",
      "type": "class",
      "source_file": "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs"
    },
    {
      "name": "BasketGuards",
      "type": "class",
      "source_file": "src/ApplicationCore/Extensions/GuardExtensions.cs"
    },
    {
      "name": "JsonExtensions",
      "type": "class",
      "source_file": "src/ApplicationCore/Extensions/JsonExtensions.cs"
    },
    {
      "name": "UriComposer",
      "type": "class",
      "source_file": "src/ApplicationCore/Services/UriComposer.cs"
    },
    {
      "name": "CustomAuthStateProvider",
      "type": "class",
      "source_file": "src/BlazorAdmin/CustomAuthStateProvider.cs"
    },
    {
      "name": "ServicesConfiguration",
      "type": "class",
      "source_file": "src/BlazorAdmin/ServicesConfiguration.cs"
    },
    {
      "name": "BlazorComponent",
      "type": "class",
      "source_file": "src/BlazorAdmin/Helpers/BlazorComponent.cs"
    },
    {
      "name": "BlazorLayoutComponent",
      "type": "class",
      "source_file": "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs"
    },
    {
      "name": "RefreshBroadcast",
      "type": "class",
      "source_file": "src/BlazorAdmin/Helpers/RefreshBroadcast.cs"
    },
    {
      "name": "ToastComponent",
      "type": "class",
      "source_file": "src/BlazorAdmin/Helpers/ToastComponent.cs"
    },
    {
      "name": "Cookies",
      "type": "class",
      "source_file": "src/BlazorAdmin/JavaScript/Cookies.cs"
    },
    {
      "name": "JSInteropConstants",
      "type": "class",
      "source_file": "src/BlazorAdmin/JavaScript/JSInteropConstants.cs"
    },
    {
      "name": "Route",
      "type": "class",
      "source_file": "src/BlazorAdmin/JavaScript/Route.cs"
    },
    {
      "name": "List",
      "type": "class",
      "source_file": "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs"
    },
    {
      "name": "CachedCatalogItemServiceDecorator",
      "type": "class",
      "source_file": "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs"
    },
    {
      "name": "CachedCatalogLookupDataServiceDecorator",
      "type": "class",
      "source_file": "src/BlazorAdmin/Services/CachedCatalogLookupDataServiceDecorator .cs"
    },
    {
      "name": "CacheEntry",
      "type": "class",
      "source_file": "src/BlazorAdmin/Services/CacheEntry.cs"
    },
    {
      "name": "CustomInputSelect",
      "type": "class",
      "source_file": "src/BlazorAdmin/Shared/CustomInputSelect.cs"
    },
    {
      "name": "BaseUrlConfiguration",
      "type": "class",
      "source_file": "src/BlazorShared/BaseUrlConfiguration.cs"
    },
    {
      "name": "EndpointAttribute",
      "type": "class",
      "source_file": "src/BlazorShared/Attributes/EndpointAttribute.cs"
    },
    {
      "name": "ClaimValue",
      "type": "class",
      "source_file": "src/BlazorShared/Authorization/ClaimValue.cs"
    },
    {
      "name": "Constants",
      "type": "class",
      "source_file": "src/BlazorShared/Authorization/Constants.cs"
    },
    {
      "name": "Roles",
      "type": "class",
      "source_file": "src/BlazorShared/Authorization/Constants.cs"
    },
    {
      "name": "UserInfo",
      "type": "class",
      "source_file": "src/BlazorShared/Authorization/UserInfo.cs"
    }
  ],
  "database": {
    "tables": [],
    "ef_entities": [
      {
        "entity_name": "CatalogType",
        "source_file": "src/Infrastructure/Data/CatalogContext.cs",
        "type": "ef_entity",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "entity_name": "CatalogBrand",
        "source_file": "src/Infrastructure/Data/CatalogContext.cs",
        "type": "ef_entity",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "entity_name": "Order",
        "source_file": "src/Infrastructure/Data/CatalogContext.cs",
        "type": "ef_entity",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "entity_name": "OrderItem",
        "source_file": "src/Infrastructure/Data/CatalogContext.cs",
        "type": "ef_entity",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "entity_name": "BasketItem",
        "source_file": "src/Infrastructure/Data/CatalogContext.cs",
        "type": "ef_entity",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "entity_name": "Basket",
        "source_file": "src/Infrastructure/Data/CatalogContext.cs",
        "type": "ef_entity",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "entity_name": "CatalogItem",
        "source_file": "src/Infrastructure/Data/CatalogContext.cs",
        "type": "ef_entity",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      }
    ],
    "stored_procedures": [],
    "triggers": []
  },
  "config": {
    "business_params": [
      {
        "key": "automationAutomationAccounts",
        "value": "aa-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "cognitiveServicesAccounts",
        "value": "cog-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "dataLakeAnalyticsAccounts",
        "value": "dla",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "dataLakeStoreAccounts",
        "value": "dls",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "documentDBDatabaseAccounts",
        "value": "cosmos-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "logicIntegrationAccounts",
        "value": "ia-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "migrateAssessmentProjects",
        "value": "migr-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "networkLoadBalancersExternal",
        "value": "lbe-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "networkLoadBalancersInternal",
        "value": "lbi-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "networkLoadBalancersInboundNatRules",
        "value": "rule-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "purviewAccounts",
        "value": "pview-",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "storageStorageAccounts",
        "value": "st",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "storageStorageAccountsVm",
        "value": "stvm",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "parameters.sqlAdminPassword.value",
        "value": "***REDACTED***",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\main.parameters.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "profiles.BlazorAdmin.commandName",
        "value": "Project",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\Properties\\launchSettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "profiles.BlazorAdmin.launchBrowser",
        "value": "True",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\Properties\\launchSettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "profiles.BlazorAdmin.inspectUri",
        "value": "{wsProtocol}://{url.hostname}:{url.port}/_framework/debug/ws-proxy?browser={browserInspectUri}",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\Properties\\launchSettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "profiles.BlazorAdmin.applicationUrl",
        "value": "https://localhost:5001;http://localhost:5000",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\Properties\\launchSettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "profiles.BlazorAdmin.environmentVariables.ASPNETCORE_ENVIRONMENT",
        "value": "Development",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\Properties\\launchSettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Default",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\wwwroot\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\wwwroot\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.System",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\wwwroot\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Default",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\wwwroot\\appsettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\wwwroot\\appsettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.System",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\BlazorAdmin\\wwwroot\\appsettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Default",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft.Hosting.Lifetime",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Default",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.Docker.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.Docker.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft.Hosting.Lifetime",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.Docker.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Default",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.System",
        "value": "Warning",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.AllowedHosts",
        "value": "*",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Default",
        "value": "Debug",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.System",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\appsettings.Development.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Default",
        "value": "Debug",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\appsettings.Docker.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.System",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\appsettings.Docker.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "Logging.LogLevel.Microsoft",
        "value": "Information",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\appsettings.Docker.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "[1].minify.enabled",
        "value": "True",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\bundleconfig.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      },
      {
        "key": "[1].minify.renameLocals",
        "value": "True",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\bundleconfig.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      }
    ],
    "feature_flags": [
      {
        "key": "[1].minify.enabled",
        "value": "True",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\bundleconfig.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      }
    ],
    "role_definitions": [
      {
        "key": "authorizationPolicyDefinitions",
        "value": "***REDACTED***",
        "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
        "source_type": "json",
        "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
      }
    ]
  },
  "extraction_summary": {
    "extraction_date": "2026-06-15T06:36:59.715224+00:00",
    "source": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main",
    "source_type": "local",
    "language": "dotnet",
    "total_methods": 203,
    "total_classes": 164,
    "total_interfaces": 16,
    "total_enums": 1,
    "total_business_artifacts": 165,
    "total_db_objects": 7,
    "total_config_params": 298,
    "total_business_params": 43,
    "total_feature_flags": 1,
    "log_events_found": 0,
    "process_sequences": 0,
    "business_categories": {
      "data_operation": 24,
      "business_attribute": 25,
      "process": 36,
      "validation": 7,
      "calculation": 5,
      "contract_definition": 16,
      "class_definition": 19,
      "general": 7,
      "integration": 7,
      "business_state": 1,
      "authentication": 6,
      "notification": 2,
      "restriction": 9,
      "transformation": 1
    },
    "output_files": {
      "source_code": "output/eShopOnWeb\\source_code.json",
      "database": "output/eShopOnWeb\\database.json",
      "config": "output/eShopOnWeb\\config.json",
      "logs": "output/eShopOnWeb\\logs.json"
    },
    "ready_for_layer2": true
  }
}
```