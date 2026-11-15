# Crew Schedule Management API

## Overview

Crew Schedule Management is used to manage employee work schedules based on Employee, Shift, Location, and WorkDate.

Base URL:

`/api/crew/schedules`

## Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/crew/schedules` | Get all crew schedules |
| GET | `/api/crew/schedules/{id}` | Get crew schedule by ID |
| POST | `/api/crew/schedules` | Create crew schedule |
| PUT | `/api/crew/schedules/{id}` | Update crew schedule |

## 1. Get All Crew Schedules

**GET** `/api/crew/schedules`

Query Parameters:

| Parameter | Type | Required | Description |
|---|---|---|---|
| workDate | DateOnly | No | Filter by work date |
| employeeId | Guid | No | Filter by employee |
| shiftId | Guid | No | Filter by shift |
| locationId | Guid | No | Filter by location |
| status | String | No | Filter by schedule status |

Response: `200 OK`

Returns a list of crew schedules including Employee, Shift, and Location information.

## 2. Get Crew Schedule by ID

**GET** `/api/crew/schedules/{id}`

Response:
- `200 OK` – Schedule found.
- `404 Not Found` – Schedule does not exist.

## 3. Create Crew Schedule

**POST** `/api/crew/schedules`

Request Body:

```json
{
  "employeeId": "EMPLOYEE_GUID",
  "shiftId": "SHIFT_GUID",
  "locationId": "LOCATION_GUID",
  "workDate": "2026-11-16",
  "notes": "Crew schedule for day shift"
}
```

Response: `201 Created`

The initial schedule status is automatically set to `SCHEDULED`.

## 4. Update Crew Schedule

**PUT** `/api/crew/schedules/{id}`

Request Body:

```json
{
  "employeeId": "EMPLOYEE_GUID",
  "shiftId": "SHIFT_GUID",
  "locationId": "LOCATION_GUID",
  "workDate": "2026-11-16",
  "status": "CONFIRMED",
  "notes": "Schedule confirmed by supervisor"
}
```

Response:
- `200 OK` – Schedule updated.
- `404 Not Found` – Schedule does not exist.
- `409 Conflict` – Duplicate schedule or inactive/missing reference.

## Business Rules and Validations

1. One employee can only have one crew schedule per work date.
2. Employee must exist and be active.
3. Shift must exist and be active.
4. Location must exist and be active.
5. EmployeeId, ShiftId, and LocationId are required.
6. WorkDate must be provided.
7. Notes cannot exceed 1,000 characters.
8. Schedule status must be one of:
   - `SCHEDULED`
   - `CONFIRMED`
   - `IN_PROGRESS`
   - `COMPLETED`
   - `CANCELLED`
9. Duplicate schedules are rejected with HTTP 409 Conflict.
10. Updating an existing schedule excludes its own ID during duplicate validation.

## Database Constraints

Crew Schedule uses the existing `crew_schedules` table.

Unique index:

`(EmployeeId, WorkDate)`

This ensures that one employee cannot have multiple schedules on the same work date, even if the shifts are different.

## Swagger Testing

The following scenarios were tested:

- Get all crew schedules.
- Get crew schedule by ID.
- Create crew schedule.
- Update crew schedule.
- Filter by WorkDate.
- Filter by EmployeeId.
- Filter by ShiftId.
- Filter by LocationId.
- Filter by Status.
- Reject duplicate schedules.
- Validate request fields and referenced records.
- Handle non-existent schedule IDs.

## Related Entities

- Employee
- Shift
- Location
- CrewSchedule
