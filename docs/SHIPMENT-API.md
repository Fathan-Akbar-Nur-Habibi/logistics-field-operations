# Shipment & Logistics Tracking API

## 1. Overview

The Shipment API is part of the Logistics & Field Operations Management System, which is used to manage the process of shipping goods from the origin to the destination.

Key features:
- Create and manage shipments.
- Retrieve shipment details.
- Search shipments by shipment number, tracking number, carrier, location, item name, and item code.
- Manage shipment items.
- Update shipment status.
- Record shipment tracking events.
- Track shipment delivery progress.

**Base URL**

```text
/api/logistics/shipments
```

## 2. API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/logistics/shipments` | Get all shipments |
| GET | `/api/logistics/shipments/{id}` | Get shipment details |
| POST | `/api/logistics/shipments` | Create shipment |
| PUT | `/api/logistics/shipments/{id}` | Update shipment |
| PATCH | `/api/logistics/shipments/{id}/status` | Update shipment status |
| POST | `/api/logistics/shipments/{id}/items` | Add item to shipment |

## 3. Get Shipments

**GET** `/api/logistics/shipments`

Query parameters:

| Parameter | Type | Description |
|---|---|---|
| search | string | Search by shipment number, tracking number, carrier, location, item name, or item code |
| status | string | Filter by shipment status |
| originLocationId | GUID | Filter by origin location |
| destinationLocationId | GUID | Filter by destination location |
| materialRequestId | GUID | Filter by material request |
| shippedAtFrom | DateTimeOffset | Filter starting shipped date |
| shippedAtTo | DateTimeOffset | Filter ending shipped date |

Response: `200 OK`

## 4. Get Shipment by ID

**GET** `/api/logistics/shipments/{id}`

Returns shipment information, origin and destination details, shipment items, and tracking history.

Responses:
- `200 OK`
- `404 Not Found`

## 5. Create Shipment

**POST** `/api/logistics/shipments`

Request body:

```json
{
  "shipmentNumber": "SHP-001",
  "materialRequestId": null,
  "originLocationId": "GUID",
  "destinationLocationId": "GUID",
  "trackingNumber": "TRK-001",
  "carrierName": "Logistics Carrier",
  "shippedAt": null,
  "estimatedArrivalAt": "2026-11-20T10:00:00+07:00",
  "notes": "Shipment description"
}
```

Business rules:
- Shipment number must be unique.
- Tracking number must be unique when provided.
- Origin and destination must be different active locations.
- Material request is optional.
- Initial status is `PREPARING`.
- An initial tracking event is automatically created.

Responses:
- `201 Created`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

## 6. Update Shipment

**PUT** `/api/logistics/shipments/{id}`

Request body:

```json
{
  "originLocationId": "GUID",
  "destinationLocationId": "GUID",
  "trackingNumber": "TRK-001",
  "carrierName": "Updated Carrier",
  "shippedAt": null,
  "estimatedArrivalAt": "2026-11-21T10:00:00+07:00",
  "notes": "Updated shipment description"
}
```

Business rules:
- Shipment number cannot be changed.
- Tracking number must remain unique.
- Origin and destination must be different active locations.
- Delivered and cancelled shipments cannot be updated.

Responses:
- `200 OK`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

## 7. Add Shipment Item

**POST** `/api/logistics/shipments/{id}/items`

Request body:

```json
{
  "itemId": "GUID",
  "quantity": 5,
  "notes": "Shipment item description"
}
```

Business rules:
- Item must exist.
- Quantity must be greater than zero.
- The same item cannot be added twice to one shipment.
- Items cannot be added to delivered or cancelled shipments.

Responses:
- `201 Created`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

## 8. Update Shipment Status

**PATCH** `/api/logistics/shipments/{id}/status`

Request body:

```json
{
  "status": "IN_TRANSIT",
  "description": "Shipment has departed",
  "locationName": "Jakarta Main Warehouse",
  "eventDate": "2026-11-17T17:00:00+07:00",
  "latitude": -6.2000000,
  "longitude": 106.8166660
}
```

Every successful status update creates a new tracking event.

### Shipment Status Workflow

| Current Status | Allowed Next Status |
|---|---|
| PREPARING | IN_TRANSIT, CANCELLED |
| IN_TRANSIT | DELAYED, DELIVERED, CANCELLED |
| DELAYED | IN_TRANSIT, DELIVERED, CANCELLED |
| DELIVERED | No further transition |
| CANCELLED | No further transition |

Additional rules:
- `ShippedAt` is automatically assigned when transitioning to `IN_TRANSIT`, if it is empty.
- `DeliveredAt` is assigned when transitioning to `DELIVERED`.
- A shipment must have a shipped date before it can be delivered.
- Latitude must be between -90 and 90.
- Longitude must be between -180 and 180.
- Latitude and longitude must be provided together.
- Tracking event dates are normalized to UTC.

Responses:
- `200 OK`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

## 9. Shipment Tracking Example

Example shipment lifecycle:

```text
PREPARING
    |
    v
IN_TRANSIT
    |
    v
DELAYED
    |
    v
DELIVERED
```

Each status transition creates a tracking event containing:
- Status
- Description
- Location name
- Event date
- Latitude
- Longitude

## 10. HTTP Status Codes

| HTTP Status | Description |
|---|---|
| 200 | Request successful |
| 201 | Resource created |
| 400 | Invalid request or business validation |
| 404 | Resource not found |
| 409 | Business rule conflict or duplicate data |

## 11. Technology

- .NET 10 Web API
- C#
- Entity Framework Core
- PostgreSQL
- Supabase
- Clean Architecture
- Repository Pattern
- Swagger / OpenAPI

## 12. Testing

Shipment API was tested through Swagger.

Test scenarios:
- Create shipment.
- Retrieve shipment list and details.
- Update shipment.
- Add shipment items.
- Search by item name and item code.
- Update shipment status.
- Verify tracking event history.
- Validate shipment status transitions.
- Validate delivered shipment restrictions.
- Validate invalid quantities and duplicate data.

All reported main workflow tests completed successfully with the expected HTTP responses.
