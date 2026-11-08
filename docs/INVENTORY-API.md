# Inventory Management API

**Project:** Logistics & Field Operations Management System  
**Module:** Inventory Management  
**Backend:** ASP.NET Core 10 Web API  
**Database:** PostgreSQL (Supabase)  
**Architecture:** Modular Monolith + Clean Architecture

---

## 1. Overview

The Inventory Management API is used to manage item master data, inventory balances, and inventory transaction history at each storage location.

### Features

- Item master management (CRUD).
- Inventory stock monitoring.
- Stock In transaction.
- Stock Out transaction.
- Stock availability validation.
- Automatic transaction number generation.
- Stock transaction history.
- Database transaction and concurrency protection.

## 2. Base URL

Development:

`http://localhost:5152`

Swagger UI:

`http://localhost:5152/swagger`

## 3. API Endpoints

### 3.1 Item Management

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/inventory/items` | Get all items |
| GET | `/api/inventory/items/{id}` | Get item by ID |
| POST | `/api/inventory/items` | Create new item |
| PUT | `/api/inventory/items/{id}` | Update item |

#### GET Items

Endpoint:

`GET /api/inventory/items`

Query parameters:

| Parameter | Type | Required | Description |
|---|---|---|---|
| search | string | No | Search by item code or name |
| isActive | boolean | No | Filter active/inactive items |

#### POST Create Item

Endpoint:

`POST /api/inventory/items`

Request body:

```json
{
  "code": "ITM-ELEC-003",
  "name": "Wireless Keyboard",
  "description": "Wireless keyboard for office operations",
  "itemCategoryId": "40000000-0000-0000-0000-000000000001",
  "unitOfMeasureId": "50000000-0000-0000-0000-000000000001",
  "minimumStock": 5,
  "unitPrice": 350000
}
```

#### PUT Update Item

Endpoint:

`PUT /api/inventory/items/{id}`

Request body:

```json
{
  "name": "Wireless Keyboard Logitech",
  "description": "Wireless keyboard for warehouse operations",
  "itemCategoryId": "40000000-0000-0000-0000-000000000001",
  "unitOfMeasureId": "50000000-0000-0000-0000-000000000001",
  "minimumStock": 8,
  "unitPrice": 425000,
  "isActive": true
}
```

The item code cannot be changed through this update endpoint.

### 3.2 Inventory Stock

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/inventory/stocks` | Get inventory stock |

Query parameters:

| Parameter | Type | Required | Description |
|---|---|---|---|
| itemId | GUID | No | Filter by item |
| locationId | GUID | No | Filter by warehouse/location |

Stock response fields include:

- ItemId, ItemCode, ItemName
- LocationId, LocationCode, LocationName
- Quantity
- ReservedQuantity
- AvailableQuantity
- MinimumStock
- IsBelowMinimumStock

Available stock calculation:

`AvailableQuantity = Quantity - ReservedQuantity`

### 3.3 Stock Transactions

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/inventory/transactions` | Get stock transaction history |
| POST | `/api/inventory/stock-in` | Record incoming stock |
| POST | `/api/inventory/stock-out` | Record outgoing stock |

GET transaction query parameters:

| Parameter | Type | Required | Description |
|---|---|---|---|
| itemId | GUID | No | Filter by item |
| locationId | GUID | No | Filter by location |
| transactionType | string | No | IN or OUT |

#### POST Stock In

Endpoint:

`POST /api/inventory/stock-in`

Request body:

```json
{
  "itemId": "28d1c476-388e-4aeb-87c4-bc0a3c92ff9d",
  "locationId": "30000000-0000-0000-0000-000000000001",
  "quantity": 20,
  "unitPrice": 425000,
  "referenceNumber": "PO-DEMO-001",
  "notes": "Initial stock in"
}
```

The API automatically sets `TransactionType` to `IN`.

#### POST Stock Out

Endpoint:

`POST /api/inventory/stock-out`

Request body:

```json
{
  "itemId": "28d1c476-388e-4aeb-87c4-bc0a3c92ff9d",
  "locationId": "30000000-0000-0000-0000-000000000001",
  "quantity": 5,
  "unitPrice": 425000,
  "referenceNumber": "DO-DEMO-001",
  "notes": "Stock out for operational usage"
}
```

The API automatically sets `TransactionType` to `OUT`.

The backend generates the transaction ID, transaction number, and transaction date.

## 4. Business Rules

### Item

- Item code is required and unique.
- Item name is required.
- Item category must exist.
- Unit of measure must exist.
- Minimum stock cannot be negative.
- Unit price cannot be negative.

### Stock In

- Item must exist.
- Location must exist.
- Quantity must be greater than zero.
- Unit price cannot be negative.
- Creates an inventory stock record if one does not exist.
- Increases inventory quantity.
- Creates a stock transaction history record.

### Stock Out

- Item must exist.
- Location must exist.
- Quantity must be greater than zero.
- Stock Out quantity cannot exceed available stock.
- Reserved stock cannot be issued.
- Decreases inventory quantity.
- Creates a stock transaction history record.

### Database Consistency

Stock balance updates and transaction history inserts are executed within a database transaction.

The implementation uses PostgreSQL row-level locking (`SELECT ... FOR UPDATE`) on the item record to serialize stock operations for the same item.

## 5. HTTP Response Codes

| Status | Description |
|---|---|
| 200 OK | Request processed successfully |
| 201 Created | Item successfully created |
| 400 Bad Request | Invalid request or validation failure |
| 404 Not Found | Requested item not found |
| 409 Conflict | Duplicate item code or insufficient stock |
| 500 Internal Server Error | Unexpected server error |

## 6. Verified Test Scenarios

| Test Scenario | Expected Result | Test Result |
|---|---|---|
| Create item | 201 Created | Passed |
| Get item detail | 200 OK | Passed |
| Update item | 200 OK | Passed |
| Duplicate item code | 409 Conflict | Passed |
| Invalid item category | 400 Bad Request | Passed |
| Invalid unit of measure | 400 Bad Request | Passed |
| Stock In | 200 OK | Passed |
| Stock Out | 200 OK | Passed |
| Stock Out exceeding available quantity | 409 Conflict | Passed |
| Concurrent Stock Out | One success and one conflict | Passed |

## 7. Architecture

The Inventory Management module follows Clean Architecture:

- **Logistics.API:** HTTP controllers and API routing.
- **Logistics.Application:** DTOs, service interfaces, business logic.
- **Logistics.Domain:** Inventory entities and domain rules.
- **Logistics.Infrastructure:** EF Core repository implementations and PostgreSQL persistence.

## 8. Database Tables

| Table | Purpose |
|---|---|
| `items` | Item master data |
| `item_categories` | Item categories |
| `units_of_measure` | Units of measurement |
| `locations` | Warehouse and storage locations |
| `inventory_stocks` | Current stock balance per item and location |
| `stock_transactions` | Incoming and outgoing stock history |

---

**Documentation Status:** Initial Inventory Management API documentation.  
**Note:** This documentation describes the currently implemented and tested endpoints. Authentication and authorization requirements should be documented when that feature is integrated.