# Asset Management API

## 1. Overview

The Asset Management API is a module within the Logistics & Field Operations Management System used to manage the company’s operational assets.

This module provides the following features:
- View a list of assets.
- Search for assets by keyword.
- Filter assets by category, location, and status.
- View asset details.
- Add new assets.
- Update asset information.
- Move assets between locations.
- View the asset transfer history.

## 2. Architecture

The Asset Management Module uses a Modular Monolith approach with Clean Architecture.

Flow request:

```text
Client / Swagger
      |
      v
AssetController
      |
      v
IAssetService
      |
      v
AssetService
      |
      v
IAssetRepository
      |
      v
AssetRepository
      |
      v
LogisticsDbContext
      |
      v
PostgreSQL (Supabase)
```

Responsibilities of each layer:

| Layer | Responsibility |
|---|---|
| API | Receiving HTTP requests and returning responses |
| Application | Business logic, validation, DTO, and service |
| Domain | Entity and business model |
| Infrastructure | Repository and database access |

## 3. API Endpoints

Base URL (local development):

```text
http://localhost:5152
```

### 3.1 Get All Assets

**GET** `/api/assets`

Retrieving all asset data.

Query parameters:

| Parameter | Type | Description |
|---|---|---|
| search | string | Search for assets by keyword |
| assetCategoryId | Guid | Filter by asset category |
| locationId | Guid | Filter by location |
| status | string | Filter by asset status |

### 3.2 Get Asset Detail

**GET** `/api/assets/{id}`

Retrieve the details of a single asset by ID.

Path parameter:

| Parameter | Type | Description |
|---|---|---|
| id | Guid | ID aset |

### 3.3 Create Asset

**POST** `/api/assets`

Adding a new asset.

Example request:

```json
{
  "assetCode": "AST-VEH-TEST-001",
  "name": "Test Delivery Truck",
  "description": "Asset untuk pengujian Asset Management API",
  "assetCategoryId": "60000000-0000-0000-0000-000000000001",
  "locationId": "30000000-0000-0000-0000-000000000001",
  "serialNumber": "SN-TRUCK-TEST-001",
  "brand": "Mitsubishi",
  "model": "L300",
  "purchaseDate": "2026-01-15",
  "purchasePrice": 250000000,
  "warrantyExpiryDate": "2028-01-15",
  "qrCode": "QR-AST-VEH-TEST-001",
  "notes": "Data testing"
}
```

The initial asset status is set to `AVAILABLE` by the service.

### 3.4 Update Asset

**PUT** `/api/assets/{id}`

Update asset information based on ID.

Example request:

```json
{
  "name": "Test Delivery Truck Updated",
  "description": "Updated asset description",
  "assetCategoryId": "60000000-0000-0000-0000-000000000001",
  "serialNumber": "SN-TRUCK-TEST-001",
  "brand": "Mitsubishi",
  "model": "L300",
  "purchaseDate": "2026-01-15",
  "purchasePrice": 260000000,
  "warrantyExpiryDate": "2028-01-15",
  "status": "IN_USE",
  "qrCode": "QR-AST-VEH-TEST-001",
  "notes": "Updated through API"
}
```

Catatan:
- AssetCode is not modified via the update endpoint.
- LocationId is not updated via the update endpoint.
- Location changes are made through Asset Movement.

### 3.5 Get Asset Movements

**GET** `/api/assets/movements`

Retrieve the asset transfer history.

The endpoint supports the `assetId` parameter to retrieve history based on a specific asset.

### 3.6 Move Asset

**POST** `/api/assets/{id}/movements`

Move assets to another location and record the transfer history.

Path parameter:

| Parameter | Type | Description |
|---|---|---|
| id | Guid | ID of the transferred asset |

Example request:

```json
{
  "toLocationId": "30000000-0000-0000-0000-000000000002",
  "reason": "Asset relocation for operational needs"
}
```

Transfer process:
1. Validate the asset ID and destination location.
2. Lock the asset data during the transaction to prevent concurrent changes.
3. Ensure the asset is available and not marked as RETIRED.
4. Ensure the destination location is active and different from the current location.
5. Record the source and destination locations in the `asset_movements` table.
6. Update the LocationId in the `assets` table.
7. Save the changes in the transaction database.

## 4. Asset Status

Status aset yang digunakan:

| Status | Description |
|---|---|
| AVAILABLE | Available assets |
| IN_USE | The asset is currently in use |
| MAINTENANCE | The asset is currently undergoing maintenance |
| RETIRED | The asset is no longer in use |

## 5. Database Entities

The Asset Management module uses the following main entities:

### Asset

Stores operational asset information, including:
- AssetCode
- Name
- Description
- AssetCategoryId
- LocationId
- SerialNumber
- Brand
- Model
- PurchaseDate
- PurchasePrice
- WarrantyExpiryDate
- Status
- QRCode
- Notes

### AssetMovement

Stores the history of asset movements, including:
- AssetId
- FromLocationId
- ToLocationId
- MovementDate
- Reason
- MovedByUserId

### Related Entities

- AssetCategory
- Location
- User

## 6. Validation and Business Rules

Applied business rules:

- AssetCode must be unique.
- QRCode must be unique if entered.
- The asset category must be available and active.
- The asset location must be available and active.
- PurchasePrice cannot be negative.
- Assets with a RETIRED status cannot be moved.
- The destination location must be different from the asset’s current location.
- Asset transfers must be recorded in AssetMovement.
- Location changes are made in a database transaction.

## 7. Testing

The Asset Management API has been tested using Swagger.

| Test | Description | Result |
|---|---|---|
| Get All Assets | Seizing all assets | Passed |
| Search Asset | Asset Search| Passed |
| Filter Asset | Filter by category, location, and status | Passed |
| Get Asset Detail | Retrieving asset details | Passed |
| Create Asset | Creating a new asset | Passed |
| Update Asset | Updating asset information | Passed |
| Get Asset Movements | Retrieving the migration history | Passed |
| Move Asset | Transferring assets between locations | Passed |

## 8. Running the API

Run the following command from the project root directory:

```powershell
dotnet run --project .\backend\src\Logistics.API
```

Open Swagger UI:

```text
http://localhost:5152/swagger
```

---

**Project:** Logistics & Field Operations Management System  
**Module:** Asset Management  
**Backend:** ASP.NET Core 10 Web API  
**ORM:** Entity Framework Core 10  
**Database:** PostgreSQL (Supabase)  
**Architecture:** Modular Monolith + Clean Architecture