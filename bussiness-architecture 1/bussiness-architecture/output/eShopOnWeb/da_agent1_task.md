I'm doing a data architecture review of a .NET codebase (eShopOnWeb-style
app) and I'd like your help producing a set of analysis documents.

Below is data already extracted from the codebase by a static-analysis tool:
EF Core entity definitions, configuration files, connection strings, and the
results of connection attempts against those connection strings. The full
source tree isn't included in this message, so please base your analysis on
this extracted data together with the entity names, types, and structure
shown, plus the detailed instructions further down in this message.

For anything that can't be directly confirmed from the data given (e.g. exact
PII field classifications, row counts, or constraints not visible in the EF
entities), give your best assessment based on naming conventions and typical
patterns for this kind of app, and label those items "INFERRED" so the
reader knows it wasn't confirmed against live data.

Please write each of the 13 files below directly to disk, using your file
tools, as soon as each one is ready — don't wait until the end, and don't
print file contents in your reply. Save each file to:

  C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/da-outputs/<filename>

e.g. C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/da-outputs/schema-catalogue.json

Once all 13 files are written, reply with a short checklist (one line per
filename, ✅ written or ❌ skipped with a reason) — that's all that needs to
be in your reply.

--- EXTRACTED DATA ---

```json
{
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
  },
  "connection_strings": [
    {
      "key": "ConnectionStrings.CatalogConnection",
      "value": "***REDACTED***",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.Docker.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "ConnectionStrings.IdentityConnection",
      "value": "***REDACTED***",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.Docker.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "ConnectionStrings.CatalogConnection",
      "value": "***REDACTED***",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "ConnectionStrings.IdentityConnection",
      "value": "***REDACTED***",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\PublicApi\\appsettings.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "ConnectionStrings.CatalogConnection",
      "value": "***REDACTED***",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\appsettings.Docker.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "ConnectionStrings.IdentityConnection",
      "value": "***REDACTED***",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\src\\Web\\appsettings.Docker.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    }
  ],
  "db_connection_results": [],
  "layer1_db_artifacts": {
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
    "triggers": [],
    "views": []
  },
  "layer1_data_entities": [
    {
      "name": "CatalogSettings",
      "type": "class",
      "source_file": "src/ApplicationCore/CatalogSettings.cs",
      "content": "class CatalogSettings",
      "metadata": {
        "namespace": "Microsoft.eShopWeb",
        "is_service_class": false
      }
    },
    {
      "name": "AuthorizationConstants",
      "type": "class",
      "source_file": "src/ApplicationCore/Constants/AuthorizationConstants.cs",
      "content": "class AuthorizationConstants",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Constants",
        "is_service_class": false
      }
    },
    {
      "name": "BaseEntity",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BaseEntity.cs",
      "content": "class BaseEntity",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "is_service_class": false
      }
    },
    {
      "name": "CatalogBrand",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/CatalogBrand.cs",
      "content": "class CatalogBrand",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "is_service_class": false
      }
    },
    {
      "name": "CatalogItem",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "class CatalogItem",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "is_service_class": false
      }
    },
    {
      "name": "CatalogItemDetails",
      "type": "struct",
      "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
      "content": "struct CatalogItemDetails",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "is_service_class": false
      }
    },
    {
      "name": "CatalogType",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/CatalogType.cs",
      "content": "class CatalogType",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities",
        "is_service_class": false
      }
    },
    {
      "name": "Basket",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/Basket.cs",
      "content": "class Basket",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate",
        "is_service_class": false
      }
    },
    {
      "name": "BasketItem",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BasketAggregate/BasketItem.cs",
      "content": "class BasketItem",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate",
        "is_service_class": false
      }
    },
    {
      "name": "Buyer",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs",
      "content": "class Buyer",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BuyerAggregate",
        "is_service_class": false
      }
    },
    {
      "name": "PaymentMethod",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs",
      "content": "class PaymentMethod",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.BuyerAggregate",
        "is_service_class": false
      }
    },
    {
      "name": "Address",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Address.cs",
      "content": "class Address",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "is_service_class": false
      }
    },
    {
      "name": "CatalogItemOrdered",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs",
      "content": "class CatalogItemOrdered",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "is_service_class": false
      }
    },
    {
      "name": "Order",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/Order.cs",
      "content": "class Order",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "is_service_class": false
      }
    },
    {
      "name": "OrderItem",
      "type": "class",
      "source_file": "src/ApplicationCore/Entities/OrderAggregate/OrderItem.cs",
      "content": "class OrderItem",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate",
        "is_service_class": false
      }
    },
    {
      "name": "BasketNotFoundException",
      "type": "class",
      "source_file": "src/ApplicationCore/Exceptions/BasketNotFoundException.cs",
      "content": "class BasketNotFoundException",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Exceptions",
        "is_service_class": false
      }
    },
    {
      "name": "DuplicateException",
      "type": "class",
      "source_file": "src/ApplicationCore/Exceptions/DuplicateException.cs",
      "content": "class DuplicateException",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Exceptions",
        "is_service_class": false
      }
    },
    {
      "name": "EmptyBasketOnCheckoutException",
      "type": "class",
      "source_file": "src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs",
      "content": "class EmptyBasketOnCheckoutException",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Exceptions",
        "is_service_class": false
      }
    },
    {
      "name": "BasketGuards",
      "type": "class",
      "source_file": "src/ApplicationCore/Extensions/GuardExtensions.cs",
      "content": "class BasketGuards",
      "metadata": {
        "namespace": "Ardalis.GuardClauses",
        "is_service_class": false
      }
    },
    {
      "name": "JsonExtensions",
      "type": "class",
      "source_file": "src/ApplicationCore/Extensions/JsonExtensions.cs",
      "content": "class JsonExtensions",
      "metadata": {
        "namespace": "Microsoft.eShopWeb",
        "is_service_class": false
      }
    },
    {
      "name": "IAggregateRoot",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IAggregateRoot.cs",
      "content": "interface IAggregateRoot",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IAppLogger",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IAppLogger.cs",
      "content": "interface IAppLogger",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IBasketQueryService",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IBasketQueryService.cs",
      "content": "interface IBasketQueryService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IBasketService",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IBasketService.cs",
      "content": "interface IBasketService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IEmailSender",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IEmailSender.cs",
      "content": "interface IEmailSender",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IOrderService",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IOrderService.cs",
      "content": "interface IOrderService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IReadRepository",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IReadRepository.cs",
      "content": "interface IReadRepository",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IRepository",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IRepository.cs",
      "content": "interface IRepository",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "ITokenClaimsService",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/ITokenClaimsService.cs",
      "content": "interface ITokenClaimsService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "IUriComposer",
      "type": "interface",
      "source_file": "src/ApplicationCore/Interfaces/IUriComposer.cs",
      "content": "interface IUriComposer",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Interfaces"
      }
    },
    {
      "name": "BasketService",
      "type": "class",
      "source_file": "src/ApplicationCore/Services/BasketService.cs",
      "content": "class BasketService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "is_service_class": true
      }
    },
    {
      "name": "OrderService",
      "type": "class",
      "source_file": "src/ApplicationCore/Services/OrderService.cs",
      "content": "class OrderService",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "is_service_class": true
      }
    },
    {
      "name": "UriComposer",
      "type": "class",
      "source_file": "src/ApplicationCore/Services/UriComposer.cs",
      "content": "class UriComposer",
      "metadata": {
        "namespace": "Microsoft.eShopWeb.ApplicationCore.Services",
        "is_service_class": false
      }
    },
    {
      "name": "CustomAuthStateProvider",
      "type": "class",
      "source_file": "src/BlazorAdmin/CustomAuthStateProvider.cs",
      "content": "class CustomAuthStateProvider",
      "metadata": {
        "namespace": "BlazorAdmin",
        "is_service_class": false
      }
    },
    {
      "name": "ServicesConfiguration",
      "type": "class",
      "source_file": "src/BlazorAdmin/ServicesConfiguration.cs",
      "content": "class ServicesConfiguration",
      "metadata": {
        "namespace": "BlazorAdmin",
        "is_service_class": false
      }
    },
    {
      "name": "BlazorComponent",
      "type": "class",
      "source_file": "src/BlazorAdmin/Helpers/BlazorComponent.cs",
      "content": "class BlazorComponent",
      "metadata": {
        "namespace": "BlazorAdmin.Helpers",
        "is_service_class": false
      }
    },
    {
      "name": "BlazorLayoutComponent",
      "type": "class",
      "source_file": "src/BlazorAdmin/Helpers/BlazorLayoutComponent.cs",
      "content": "class BlazorLayoutComponent",
      "metadata": {
        "namespace": "BlazorAdmin.Helpers",
        "is_service_class": false
      }
    },
    {
      "name": "RefreshBroadcast",
      "type": "class",
      "source_file": "src/BlazorAdmin/Helpers/RefreshBroadcast.cs",
      "content": "class RefreshBroadcast",
      "metadata": {
        "namespace": "BlazorAdmin.Helpers",
        "is_service_class": false
      }
    },
    {
      "name": "ToastComponent",
      "type": "class",
      "source_file": "src/BlazorAdmin/Helpers/ToastComponent.cs",
      "content": "class ToastComponent",
      "metadata": {
        "namespace": "BlazorAdmin.Helpers",
        "is_service_class": false
      }
    },
    {
      "name": "Cookies",
      "type": "class",
      "source_file": "src/BlazorAdmin/JavaScript/Cookies.cs",
      "content": "class Cookies",
      "metadata": {
        "namespace": "BlazorAdmin.JavaScript",
        "is_service_class": false
      }
    },
    {
      "name": "JSInteropConstants",
      "type": "class",
      "source_file": "src/BlazorAdmin/JavaScript/JSInteropConstants.cs",
      "content": "class JSInteropConstants",
      "metadata": {
        "namespace": "BlazorAdmin.JavaScript",
        "is_service_class": false
      }
    },
    {
      "name": "Route",
      "type": "class",
      "source_file": "src/BlazorAdmin/JavaScript/Route.cs",
      "content": "class Route",
      "metadata": {
        "namespace": "BlazorAdmin.JavaScript",
        "is_service_class": false
      }
    },
    {
      "name": "List",
      "type": "class",
      "source_file": "src/BlazorAdmin/Pages/CatalogItemPage/List.razor.cs",
      "content": "class List",
      "metadata": {
        "namespace": "BlazorAdmin.Pages.CatalogItemPage",
        "is_service_class": false
      }
    },
    {
      "name": "CachedCatalogItemServiceDecorator",
      "type": "class",
      "source_file": "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
      "content": "class CachedCatalogItemServiceDecorator",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": false
      }
    },
    {
      "name": "CachedCatalogLookupDataServiceDecorator",
      "type": "class",
      "source_file": "src/BlazorAdmin/Services/CachedCatalogLookupDataServiceDecorator .cs",
      "content": "class CachedCatalogLookupDataServiceDecorator",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": false
      }
    },
    {
      "name": "CacheEntry",
      "type": "class",
      "source_file": "src/BlazorAdmin/Services/CacheEntry.cs",
      "content": "class CacheEntry",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": false
      }
    },
    {
      "name": "CatalogItemService",
      "type": "class",
      "source_file": "src/BlazorAdmin/Services/CatalogItemService.cs",
      "content": "class CatalogItemService",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": true
      }
    },
    {
      "name": "CatalogLookupDataService",
      "type": "class",
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
      "source_file": "src/BlazorAdmin/Services/HttpService.cs",
      "content": "class HttpService",
      "metadata": {
        "namespace": "BlazorAdmin.Services",
        "is_service_class": true
      }
    },
    {
      "name": "ToastService",
      "type": "class",
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
      "name": "CustomInputSelect",
      "type": "class",
      "source_file": "src/BlazorAdmin/Shared/CustomInputSelect.cs",
      "content": "class CustomInputSelect",
      "metadata": {
        "namespace": "BlazorAdmin.Shared",
        "is_service_class": false
      }
    },
    {
      "name": "BaseUrlConfiguration",
      "type": "class",
      "source_file": "src/BlazorShared/BaseUrlConfiguration.cs",
      "content": "class BaseUrlConfiguration",
      "metadata": {
        "namespace": "BlazorShared",
        "is_service_class": false
      }
    },
    {
      "name": "EndpointAttribute",
      "type": "class",
      "source_file": "src/BlazorShared/Attributes/EndpointAttribute.cs",
      "content": "class EndpointAttribute",
      "metadata": {
        "namespace": "BlazorShared.Attributes",
        "is_service_class": false
      }
    },
    {
      "name": "ClaimValue",
      "type": "class",
      "source_file": "src/BlazorShared/Authorization/ClaimValue.cs",
      "content": "class ClaimValue",
      "metadata": {
        "namespace": "BlazorShared.Authorization",
        "is_service_class": false
      }
    },
    {
      "name": "Constants",
      "type": "class",
      "source_file": "src/BlazorShared/Authorization/Constants.cs",
      "content": "class Constants",
      "metadata": {
        "namespace": "BlazorShared.Authorization",
        "is_service_class": false
      }
    },
    {
      "name": "Roles",
      "type": "class",
      "source_file": "src/BlazorShared/Authorization/Constants.cs",
      "content": "class Roles",
      "metadata": {
        "namespace": "BlazorShared.Authorization",
        "is_service_class": false
      }
    },
    {
      "name": "UserInfo",
      "type": "class",
      "source_file": "src/BlazorShared/Authorization/UserInfo.cs",
      "content": "class UserInfo",
      "metadata": {
        "namespace": "BlazorShared.Authorization",
        "is_service_class": false
      }
    },
    {
      "name": "ICatalogItemService",
      "type": "interface",
      "source_file": "src/BlazorShared/Interfaces/ICatalogItemService.cs",
      "content": "interface ICatalogItemService",
      "metadata": {
        "namespace": "BlazorShared.Interfaces"
      }
    },
    {
      "name": "ICatalogLookupDataService",
      "type": "interface",
      "source_file": "src/BlazorShared/Interfaces/ICatalogLookupDataService.cs",
      "content": "interface ICatalogLookupDataService",
      "metadata": {
        "namespace": "BlazorShared.Interfaces"
      }
    }
  ],
  "layer1_config_params": [
    {
      "key": "name",
      "value": "eShopOnWeb",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\azure.yaml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.web.project",
      "value": "./src/Web",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\azure.yaml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.web.language",
      "value": "csharp",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\azure.yaml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.web.host",
      "value": "appservice",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\azure.yaml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "version",
      "value": "3.4",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.override.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "version",
      "value": "3.4",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.eshopwebmvc.image",
      "value": "${DOCKER_REGISTRY-}eshopwebmvc",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.eshopwebmvc.build.context",
      "value": ".",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.eshopwebmvc.build.dockerfile",
      "value": "src/Web/Dockerfile",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.eshoppublicapi.image",
      "value": "${DOCKER_REGISTRY-}eshoppublicapi",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.eshoppublicapi.build.context",
      "value": ".",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.eshoppublicapi.build.dockerfile",
      "value": "src/PublicApi/Dockerfile",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "services.sqlserver.image",
      "value": "mcr.microsoft.com/azure-sql-edge",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\docker-compose.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "sdk.version",
      "value": "8.0.x",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\global.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "sdk.rollForward",
      "value": "latestFeature",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\global.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "version",
      "value": "2",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\dependabot.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "updates[0].package-ecosystem",
      "value": "nuget",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\dependabot.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "updates[0].directory",
      "value": "/",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\dependabot.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "updates[0].schedule.interval",
      "value": "daily",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\dependabot.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "name",
      "value": "eShopOnWeb Build and Test",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.runs-on",
      "value": "ubuntu-latest",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[0].uses",
      "value": "actions/checkout@v2",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[1].name",
      "value": "Setup .NET",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[1].uses",
      "value": "actions/setup-dotnet@v1",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[1].with.dotnet-version",
      "value": "8.0.x",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[1].with.include-prerelease",
      "value": "True",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[2].name",
      "value": "Build with dotnet",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[2].run",
      "value": "dotnet build ./eShopOnWeb.sln --configuration Release",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[3].name",
      "value": "Test with dotnet",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "jobs.build.steps[3].run",
      "value": "dotnet test ./eShopOnWeb.sln --configuration Release",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\.github\\workflows\\dotnetcore.yml",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "analysisServicesServers",
      "value": "as",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "apiManagementService",
      "value": "apim-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "appConfigurationConfigurationStores",
      "value": "appcs-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "appManagedEnvironments",
      "value": "cae-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "appContainerApps",
      "value": "ca-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "authorizationPolicyDefinitions",
      "value": "***REDACTED***",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "automationAutomationAccounts",
      "value": "aa-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "blueprintBlueprints",
      "value": "bp-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "blueprintBlueprintsArtifacts",
      "value": "bpa-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "cacheRedis",
      "value": "redis-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "cdnProfiles",
      "value": "cdnp-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "cdnProfilesEndpoints",
      "value": "cdne-",
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
      "key": "cognitiveServicesFormRecognizer",
      "value": "cog-fr-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "cognitiveServicesTextAnalytics",
      "value": "cog-ta-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeAvailabilitySets",
      "value": "avail-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeCloudServices",
      "value": "cld-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeDiskEncryptionSets",
      "value": "des",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeDisks",
      "value": "disk",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeDisksOs",
      "value": "osdisk",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeGalleries",
      "value": "gal",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeSnapshots",
      "value": "snap-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeVirtualMachines",
      "value": "vm",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "computeVirtualMachineScaleSets",
      "value": "vmss-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "containerInstanceContainerGroups",
      "value": "ci",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "containerRegistryRegistries",
      "value": "cr",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "containerServiceManagedClusters",
      "value": "aks-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "databricksWorkspaces",
      "value": "dbw-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "dataFactoryFactories",
      "value": "adf-",
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
      "key": "dataMigrationServices",
      "value": "dms-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "dBforMySQLServers",
      "value": "mysql-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "dBforPostgreSQLServers",
      "value": "psql-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "devicesIotHubs",
      "value": "iot-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "devicesProvisioningServices",
      "value": "provs-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "devicesProvisioningServicesCertificates",
      "value": "***REDACTED***",
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
      "key": "eventGridDomains",
      "value": "evgd-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "eventGridDomainsTopics",
      "value": "evgt-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "eventGridEventSubscriptions",
      "value": "evgs-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "eventHubNamespaces",
      "value": "evhns-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "eventHubNamespacesEventHubs",
      "value": "evh-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "hdInsightClustersHadoop",
      "value": "hadoop-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "hdInsightClustersHbase",
      "value": "hbase-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "hdInsightClustersKafka",
      "value": "kafka-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "hdInsightClustersMl",
      "value": "mls-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "hdInsightClustersSpark",
      "value": "spark-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "hdInsightClustersStorm",
      "value": "storm-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    },
    {
      "key": "hybridComputeMachines",
      "value": "arcs-",
      "source_file": "C:\\Users\\BrianRoyS\\Downloads\\eShopOnWeb-main\\eShopOnWeb-main\\infra\\abbreviations.json",
      "source_type": "json",
      "extraction_timestamp": "2026-06-15T06:36:59.369330+00:00"
    }
  ]
}
```

---

---
name: da-extractor
description: Full data architecture reverse engineering. Use when user says "reverse engineer this", 
  "analyse my data architecture", "document the schema", "map the database", or drops a codebase 
  for data analysis. Do NOT use for general code review, UI analysis, or business logic questions.
---

# DA Agent 1 — Data Architecture Extractor
> Pair with: `DA_REVIEW_PROMPT.md` | Version: June 2026 v2

---

## When to Activate

- User says "reverse engineer this project", "analyse my data architecture", "document the schema"
- User drops a codebase folder and asks about data, tables, entities, or migration
- User asks "what databases does this use?" or "map the data flow"
- `DA_REVIEW_PROMPT.md` is present and user wants a full DA pipeline run

---

## What This Skill Does NOT Do

- Does not interpret business intent or make design decisions — surfaces facts only
- Does not produce Business Requirements Documents (BRD) — that is Agent 2's job
- Does not review UI, frontend, or non-data code
- Does not write new code or fix bugs
- Does not skip the database connection and silently fall back to code-only — if DB is unreachable, it says exactly why

---

## Steps

**Phase 0 — Auto-Detection** *(always first, no files opened yet)*
1. Scan folder structure — detect language, framework, ORM, database engine, migration tool
2. Extract ALL connection strings from config files — record host, port, database name, username
3. List all entity files, route files, config files, integration clients
4. Produce a chunk plan — one chunk per domain

**Phase 1 — Code Discovery** *(read entities, migrations, services, queries)*
1. Read all entity/model classes — fields, types, validation rules, relationships
2. Read all migration files chronologically — columns, indexes, FK rules, sequences
3. Read all repository and query classes — specifications, raw SQL, N+1 risks
4. Read service classes — transaction boundaries, business rules in constructors
5. **Mandatory spot check** — read at least 5 files from: `*/Config*` `*/Extensions*` `*/HealthChecks*` `*/Cache*` `*/Background*` — look for caching layers, feature flags, background DB writers, and health check dependencies
6. **Business-meaning & governance capture** — read XML doc comments, README/glossary text, validation messages, `[Authorize]` attributes and role/policy names, and check for soft-delete flags or audit columns (`IsDeleted`, `CreatedAt`, etc.). This evidence feeds the data dictionary, conceptual model, retention policy, and access control matrix.
7. **Canonical / shadow entity detection** — for every business concept that appears more than once (e.g. "customer" represented as `AspNetUsers`, `Buyer`, and a loose `BuyerId` string), identify which representation is canonical (source of truth) and which are shadow/duplicate. Check whether each representation is actually constructed/queried anywhere — an entity that is only ever defined but never instantiated is a strong shadow signal.

**Phase 2 — Database Connection** *(mandatory — no skipping)*
1. Find the database CLI tool on this machine
2. Run a connection test command
3. On success — run row counts, schema verification, FK rules, indexes, sequences, triggers, DQ checks. **If the live result conflicts with what code/migrations suggested, the live DB wins.** Use this Evidence Strength Hierarchy (highest wins): live DB > migration files > entity/ORM code > repository code > naming convention > docs/README/git history (docs only win if they cite a hard constraint, e.g. a documented legal retention period). Document both sides and which one prevailed.
4. On failure — document exact error + command run, continue code-only
5. Repeat for every database found in Phase 0

**Phase 3 — Write Output Files** *(write each file as its phase completes — do not batch)*

| # | File | Write After |
|---|---|---|
| 1 | `da-outputs/schema-catalogue.json` | Phase 2 complete |
| 2 | `da-outputs/erd.md` | Phase 2 complete |
| 3 | `da-outputs/data-source-inventory.json` | Phase 1e + Phase 2 complete |
| 4 | `da-outputs/data-flow-map.md` | Phase 1 complete |
| 5 | `da-outputs/pii-inventory.json` | Phase 2 complete |
| 6 | `da-outputs/data-quality-report.md` | Phase 2 complete |
| 7 | `da-outputs/migration-complexity.json` | Phase 2 complete |
| 8 | `da-outputs/hidden-business-rules.json` | Phase 1 complete |
| 9 | `da-outputs/storage-pattern-analysis.md` | Phase 1e complete |
| 10 | `da-outputs/redundancy-analysis.json` | Phase 1 step 7 complete |
| 11 | `da-outputs/data-dictionary.md` | Phase 1 step 6 complete |
| 12 | `da-outputs/conceptual-data-model.md` | Phase 1 step 6 complete |
| 13 | `da-outputs/access-control-matrix.md` | Phase 1 step 6 + Phase 2 complete |

**Phase 4 — Self-Check** *(fix any ❌ before finishing)*

| Check | Pass? |
|---|---|
| DB connection attempted? (YES or exact error documented) | |
| `db_connection` field set to CONNECTED or CODE-ONLY with reason? | |
| Row counts from live DB where connected? | |
| Caching section in `storage-pattern-analysis.md`? | |
| At least 5 files read from `*/Config*` `*/Extensions*` `*/HealthChecks*`? | |
| Every UNKNOWN value has an inline reason? | |
| All soft references listed in `erd.md` WARNING section? | |
| Caching TTL documented as a business rule in `hidden-business-rules.json`? | |
| Code-vs-DB conflicts resolved using Evidence Strength Hierarchy (not guessed)? | |
| At least one canonical/shadow entity check completed — even if the result is "no duplicates found"? | |
| Every table/column in `schema-catalogue.json` has a matching entry in `data-dictionary.md`? | |
| Every role found in `[Authorize]` attributes appears in `access-control-matrix.md`? | |
| `conceptual-data-model.md` uses business language only — no table names, types, or FK syntax? | |

---

## Output Format

All 13 files go in `da-outputs/`. Create the folder if absent.

**Real example of a correct `db_connection` value:**
```
"db_connection": "CONNECTED — psql at C:\\Program Files\\PostgreSQL\\18\\bin\\psql.exe"
"db_connection": "CODE-ONLY — connection refused at localhost:5432. Command run: psql -h localhost -p 5432 -U postgres -d eShopCatalog -c \\dt"
```

**Real example of a correct volume value:**
```
"volume": "LOW",
"volume_detail": "0 rows — live DB confirmed. No real orders placed yet."

"volume": "UNKNOWN",
"volume_detail": "DB not connected — connection refused at localhost:5432. Command attempted: psql -h localhost -p 5432 -U postgres -d eShopCatalog -c \\dt"
```

**Confidence scoring scale** (use for any `confidence` field):

| Score | Meaning |
|---|---|
| 1.0 | Confirmed by live DB query |
| 0.9 | Confirmed by migration file |
| 0.8 | Confirmed by entity/ORM code (EF model, annotations) |
| 0.75 | Confirmed by repository/query code |
| 0.70 | Inferred from naming convention or framework default only |

Anything below 0.70 should be marked `UNKNOWN` with an inline reason rather than guessed.

---

## Error Handling

| If | Then |
|---|---|
| DB connection fails with auth error | Run `dotnet ef database update` (or equivalent), retry once |
| DB connection fails with "connection refused" | Mark CODE-ONLY with exact error + command. Check for `docker-compose.yml` and note start command. |
| Entity/model files not found after full folder scan | Stop and ask the user |
| More than 40% of files are binary with no source | Stop and ask the user |
| Connection string is encrypted or redacted | Stop and ask the user |
| Caching class found in `*/Services*` | Document in BOTH `data-source-inventory.json` AND `storage-pattern-analysis.md` |
| FK ON DELETE = NO ACTION | Flag in `data-quality-report.md` — deleting parent row with children will error |
| File appears relevant in multiple domains | Read once, mark 🔁 SHARED, reference by path in all chunks |
| Confidence cannot reach 0.80 | Include the finding, mark ⚠️ LOW CONFIDENCE — Agent 2 to verify |

---

## Final Report to User

```
## 📋 DA Agent 1 — Complete

Language(s):        [detected]
Framework(s):       [detected]  
Database(s):        [detected + host:port]
DB connection:      CONNECTED | CODE-ONLY (reason)
Files scanned:      [N]
Domains found:      [N] — [list]

Outputs written:
  da-outputs/schema-catalogue.json         ✅
  da-outputs/erd.md                        ✅
  da-outputs/data-source-inventory.json    ✅
  da-outputs/data-flow-map.md              ✅
  da-outputs/pii-inventory.json            ✅
  da-outputs/data-quality-report.md        ✅
  da-outputs/migration-complexity.json     ✅
  da-outputs/hidden-business-rules.json    ✅
  da-outputs/storage-pattern-analysis.md   ✅
  da-outputs/redundancy-analysis.json      ✅
  da-outputs/data-dictionary.md            ✅
  da-outputs/conceptual-data-model.md      ✅
  da-outputs/access-control-matrix.md      ✅

⚠️ Validation Queue: [list any LOW confidence items]

🤝 Handoff to DA Agent 2:
[3–5 sentences: connection status, key entities, most important gaps,
what Agent 2 should investigate first]

Ready for DA Agent 2 → run DA_REVIEW_PROMPT.md
```

---

*DA Reverse Engineering System — Agent 1 of 2 | v2 | June 2026*

---

## Reminder on output

Please write all 13 files directly to C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/da-outputs/ using your file tools as
you go, then reply with just the ✅/❌ checklist:

1. schema-catalogue.json
2. erd.md
3. data-source-inventory.json
4. data-flow-map.md
5. pii-inventory.json
6. data-quality-report.md
7. migration-complexity.json
8. hidden-business-rules.json
9. storage-pattern-analysis.md
10. redundancy-analysis.json
11. data-dictionary.md
12. conceptual-data-model.md
13. access-control-matrix.md
