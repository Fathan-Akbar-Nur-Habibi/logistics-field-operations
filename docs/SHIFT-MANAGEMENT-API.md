# Shift Management API

## 1. Overview

Shift Management is part of the Crew & Field Operations Management module within the Logistics & Field Operations Management System.

This module is designed to manage employee work schedules, including regular shifts and overnight shifts.

**Base Endpoint:**

`/api/crew/shifts`

## 2. API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/crew/shifts` | Get all shifts |
| GET | `/api/crew/shifts/{id}` | Get shift by ID |
| POST | `/api/crew/shifts` | Create new shift |
| PUT | `/api/crew/shifts/{id}` | Update existing shift |

## 3. Get All Shifts

**Endpoint:** `GET /api/crew/shifts`

Retrieves all available shift records.

### Query Parameters

| Parameter | Type | Required | Description |
|---|---|---|---|
| search | string | No | Search berdasarkan Shift Code atau Shift Name |
| isActive | boolean | No | Filter berdasarkan status aktif |

### Example Request

```http
GET /api/crew/shifts
GET /api/crew/shifts?isActive=true
GET /api/crew/shifts?search=NIGHT
GET /api/crew/shifts?search=SHIFT&isActive=true
```

**Success Response:** `200 OK`

Returns an array of ShiftResponse objects.
`ShiftResponse`.

## 4. Get Shift By ID

**Endpoint:** `GET /api/crew/shifts/{id}`

Retrieves detailed shift information based on its ID.

### Example Request

```http
GET /api/crew/shifts/{id}
```

**Success Response:** `200 OK`

Returns a ShiftResponse object containing the shift details.`ShiftResponse`.

**Error Response:**

- `400 Bad Request`: The provided Shift ID is invalid.
- `404 Not Found`: Data shift Not Found.

## 5. Create Shift

**Endpoint:** `POST /api/crew/shifts`

Creates a new shift record.

### Request Body

```json
{
  "code": "SHIFT-DAY",
  "name": "Day Shift",
  "startTime": "08:00:00",
  "endTime": "16:00:00",
  "description": "Regular day shift"
}
```

### Field Description

| Field | Type | Required | Description |
|---|---|---|---|
| code | string | Yes | Unique shift code, maximum 30 characters |
| name | string | Yes | Shift name, maximum 100 characters |
| startTime | TimeOnly | Yes | Shift starting time |
| endTime | TimeOnly | Yes | Shift ending time |
| description | string | No | Shift description, maximum 500 characters |

**Success Response:** `201 Created`

Returns the newly created shift record, including its ID and active status.

**Error Response:**

- `400 Bad Request`: The provided input is invalid.
- `409 Conflict`:  The Shift Code is already in use.

## 6. Update Shift

**Endpoint:** `PUT /api/crew/shifts/{id}`

Updates an existing shift record based on its ID.

### Request Body

```json
{
  "name": "Day Shift Updated",
  "startTime": "07:00:00",
  "endTime": "15:00:00",
  "description": "Updated day shift schedule",
  "isActive": true
}
```

### Field Description

| Field | Type | Required | Description |
|---|---|---|---|
| name | string | Yes | Updated shift name |
| startTime | TimeOnly | Yes | Updated starting time |
| endTime | TimeOnly | Yes | Updated ending time |
| description | string | No | Updated shift description |
| isActive | boolean | Yes | Shift active status |

**Success Response:** `200 OK`

Returns the updated shift record.

**Error Response:**

- `400 Bad Request`: The provided input is invalid.
- `404 Not Found`: The specified Shift ID was not found.

**Note:** The Shift Code is immutable and cannot be modified through the Update endpoint.

## 7. Business Rules & Validation

### Shift Code

- Required field.
- Maximum length of 30 characters.
- Leading and trailing whitespace is automatically trimmed.
- Must be unique.
- Cannot be modified after the shift has been created.

### Shift Name

- Required field.
- Maximum length of 100 characters.
- Cannot be empty or contain only whitespace.

### Working Hours

- Start Time and End Time must not be identical.
- End Time can be earlier than Start Time to support overnight shifts.
- There is no validation requiring End Time to be greater than Start Time.


### Active Status

- Newly created shifts are automatically assigned `IsActive = true`.
- The active status can be modified through the PUT endpoint.

## 8. Overnight Shift Support

The system supports shifts that extend beyond midnight.

Examples:

| Shift | Start Time | End Time | Status |
|---|---|---|---|
| Day Shift | 08:00 | 16:00 | Valid |
| Afternoon Shift | 14:00 | 22:00 | Valid |
| Night Shift | 22:00 | 06:00 | Valid |
| Overnight Shift | 18:00 | 02:00 | Valid |
| Invalid Shift | 08:00 | 08:00 | Invalid |

The validation only rejects shifts where the Start Time and End Time are identical.

This allows the system to accommodate both regular working hours and overnight schedules.

## 9. ShiftResponse

The ShiftResponse DTO is used to return shift information to API consumers.

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "code": "SHIFT-DAY",
  "name": "Day Shift",
  "startTime": "08:00:00",
  "endTime": "16:00:00",
  "description": "Regular day shift",
  "isActive": true,
  "createdAt": "2026-11-15T00:00:00+00:00",
  "updatedAt": null
}
```

The ID and timestamp values above are illustrative examples.

## 10. Testing

API testing is performed using Swagger UI.

Test scenarios:

| Test Case | Expected Result |
|---|---|
| Create Day Shift | 201 Created |
| Create Night Shift (22:00–06:00) | 201 Created |
| Duplicate Shift Code | 409 Conflict |
| Empty Shift Name | 400 Bad Request |
| Same Start Time and End Time | 400 Bad Request |
| Get All Shifts | 200 OK |
| Get Shift by ID | 200 OK |
| Update Shift | 200 OK |

## 11. Implementation Notes

- Architecture: Clean Architecture.
- Backend: ASP.NET Core Web API (.NET 10).
- Database: PostgreSQL.
- ORM: Entity Framework Core.
- Repository Pattern Used for database access and data management.
  
- Service Layer Responsible for business logic and validation.

- Controller andles HTTP requests and responses.

**Module:** Crew & Field Operations Management  
**Feature:** Shift Management  
**Status:** Implemented and tested through Swagger.