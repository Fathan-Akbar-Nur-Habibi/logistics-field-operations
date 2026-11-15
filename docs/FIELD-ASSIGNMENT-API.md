# Field Assignment Management API

## 1. Overview

Field Assignment Management is part of the Crew & Field Operations module in the Logistics & Field Operations Management System.

This module is used to manage employee assignments at field operations sites, including:
- Create and manage field assignments.
- Specify the location and duration of assignments.
- Set assignment priorities and statuses.
- Add employees as assignment members.
- Check team members in and out.
- Monitor the progress of field assignments.
  
## 2. Base URL

```text
/api/crew/field-assignments
```

## 3. API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/crew/field-assignments` | Get all field assignments |
| GET | `/api/crew/field-assignments/{id}` | Get field assignment by ID |
| POST | `/api/crew/field-assignments` | Create field assignment |
| PUT | `/api/crew/field-assignments/{id}` | Update field assignment |
| PATCH | `/api/crew/field-assignments/{id}/status` | Update assignment status |
| POST | `/api/crew/field-assignments/{id}/members` | Add employee to assignment |
| POST | `/api/crew/field-assignments/{id}/members/{memberId}/check-in` | Check-in member |
| POST | `/api/crew/field-assignments/{id}/members/{memberId}/check-out` | Check-out member |

---

## 4. Get All Field Assignments

**Endpoint**

```http
GET /api/crew/field-assignments
```

**Query Parameters**

| Parameter | Type | Description |
|---|---|---|
| search | string | Search assignment number, title, description, location, or employee information |
| locationId | Guid | Filter by location |
| status | string | Filter by assignment status |
| priority | string | Filter by priority |
| startDateFrom | DateTimeOffset | Filter assignments starting from a date |
| startDateTo | DateTimeOffset | Filter assignments starting until a date |

**Example**

```http
GET /api/crew/field-assignments?search=Bekasi&status=PLANNED&priority=HIGH
```

**Expected Response: 200 OK**

```json
[
  {
    "id": "71fb4d56-9936-4779-9202-29dd6c7d079e",
    "assignmentNumber": "FA-TEST-001",
    "title": "Bekasi Field Inspection",
    "description": "Field inspection and equipment verification.",
    "locationId": "30000000-0000-0000-0000-000000000002",
    "locationCode": "DC-BKS",
    "locationName": "Bekasi Distribution Center",
    "startDate": "2026-11-17T01:00:00+00:00",
    "endDate": "2026-11-17T10:00:00+00:00",
    "priority": "HIGH",
    "status": "PLANNED",
    "createdByUserId": null,
    "members": [],
    "createdAt": "2026-11-15T17:39:52.048575+00:00",
    "updatedAt": null
  }
]
```

## 5. Get Field Assignment by ID

**Endpoint**

```http
GET /api/crew/field-assignments/{id}
```

**Path Parameter**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Field assignment ID |

**Expected Response: 200 OK**

Returns one field assignment with its Location and assigned Members information.

Possible error:
- `404 Not Found`: Assignment does not exist.

## 6. Create Field Assignment

**Endpoint**

```http
POST /api/crew/field-assignments
```

**Request Body**

```json
{
  "assignmentNumber": "FA-TEST-001",
  "title": "Bekasi Field Inspection",
  "description": "Field inspection and equipment verification.",
  "locationId": "30000000-0000-0000-0000-000000000002",
  "startDate": "2026-11-17T08:00:00+07:00",
  "endDate": "2026-11-17T17:00:00+07:00",
  "priority": "HIGH"
}
```

**Expected Response: 201 Created**

Returns the created assignment with:
- Generated assignment ID.
- Location information.
- Default status `PLANNED`.
- Empty members collection.
- CreatedAt timestamp.

**Business Rules**
- AssignmentNumber is required and unique.
- Title is required.
- Location must exist and be active.
- EndDate must be later than StartDate.
- Priority must be one of the supported values.
- New assignments automatically receive status `PLANNED`.

Possible errors:
- `400 Bad Request`: Invalid request or validation error.
- `409 Conflict`: Duplicate assignment number or invalid business condition.

## 7. Update Field Assignment

**Endpoint**

```http
PUT /api/crew/field-assignments/{id}
```

**Request Body**

```json
{
  "title": "Bekasi Field Inspection - Updated",
  "description": "Updated field inspection and equipment verification.",
  "locationId": "30000000-0000-0000-0000-000000000002",
  "startDate": "2026-11-17T08:00:00+07:00",
  "endDate": "2026-11-17T17:00:00+07:00",
  "priority": "URGENT"
}
```

**Business Rules**
- AssignmentNumber cannot be changed.
- Assignment must exist.
- Completed or cancelled assignments cannot be updated.
- Location must be active.
- EndDate must be later than StartDate.
- UpdatedAt is updated after a successful modification.

Possible errors:
- `400 Bad Request`: Invalid input.
- `404 Not Found`: Assignment does not exist.
- `409 Conflict`: Assignment is completed/cancelled or another business constraint is violated.

## 8. Update Assignment Status

**Endpoint**

```http
PATCH /api/crew/field-assignments/{id}/status
```

**Request Body**

```json
{
  "status": "IN_PROGRESS"
}
```

Supported statuses:
- `PLANNED`
- `IN_PROGRESS`
- `COMPLETED`
- `CANCELLED`

**Status Workflow**

```text
PLANNED
   |
   +----> IN_PROGRESS
   |          |
   |          +----> COMPLETED
   |          |
   |          +----> CANCELLED
   |
   +----> CANCELLED
```

Completed and cancelled assignments are terminal states.

Invalid status transitions are rejected.

## 9. Add Assignment Member

**Endpoint**

```http
POST /api/crew/field-assignments/{id}/members
```

**Request Body**

```json
{
  "employeeId": "6ec86173-8888-45e3-9ae6-66e241da53e2",
  "responsibility": "Field Inspection Operator"
}
```

**Expected Response: 200 OK**

```json
{
  "id": "7b7fbcc9-ea79-4ac8-b7b0-10925b5aac6b",
  "employeeId": "6ec86173-8888-45e3-9ae6-66e241da53e2",
  "employeeNumber": "EMP-TEST-001",
  "employeeName": "John Doe Updated",
  "responsibility": "Field Inspection Operator",
  "status": "ASSIGNED",
  "checkInTime": null,
  "checkOutTime": null
}
```

**Business Rules**
- Assignment must exist.
- Assignment cannot be completed or cancelled.
- Employee must exist and be active.
- An employee can only be added once to the same assignment.
- Initial member status is `ASSIGNED`.

## 10. Check-in Assignment Member

**Endpoint**

```http
POST /api/crew/field-assignments/{id}/members/{memberId}/check-in
```

No request body is required.

**Business Rules**
- Assignment must be in `IN_PROGRESS` status.
- Member must exist in the specified assignment.
- Member must currently have status `ASSIGNED`.
- CheckInTime is recorded in UTC.
- Member status changes to `CHECKED_IN`.

**Expected Response: 200 OK**

Returns the updated member information with CheckInTime populated.

## 11. Check-out Assignment Member

**Endpoint**

```http
POST /api/crew/field-assignments/{id}/members/{memberId}/check-out
```

No request body is required.

**Business Rules**
- Member must have status `CHECKED_IN`.
- CheckInTime must exist.
- Check-out time cannot be earlier than check-in time.
- CheckOutTime is recorded in UTC.
- Member status changes to `CHECKED_OUT`.

**Expected Response: 200 OK**

Returns the updated member information with CheckOutTime populated.

---

## 12. Priority Values

| Priority | Description |
|---|---|
| LOW | Low priority |
| MEDIUM | Normal priority |
| HIGH | High priority |
| URGENT | Urgent assignment |

## 13. Member Status Workflow

```text
ASSIGNED
   |
   v
CHECKED_IN
   |
   v
CHECKED_OUT
```

The member must check in before checking out.

## 14. DateTime and Timezone Handling

The API uses `DateTimeOffset` for assignment dates and timestamps.

PostgreSQL `timestamp with time zone` requires UTC values when writing through Npgsql.

Therefore:
- Incoming DateTimeOffset values are normalized using `ToUniversalTime()` before persistence.
- CreatedAt and UpdatedAt timestamps use UTC.
- CheckInTime and CheckOutTime use UTC.
- API responses return timestamps in UTC (`+00:00`).

Example:

```text
Input:
2026-11-17T08:00:00+07:00

Stored/Returned:
2026-11-17T01:00:00+00:00
```

---

## 15. Swagger Testing Results

The following tests were performed using Swagger UI.

| No. | Test Scenario | Expected HTTP | Result |
|---:|---|---:|---|
| 1 | Create Field Assignment | 201 | Passed |
| 2 | Get All Assignments | 200 | Passed |
| 3 | Get Assignment by ID | 200 | Passed |
| 4 | Filter Assignments | 200 | Passed |
| 5 | Filter by Date | 200 | Passed |
| 6 | Duplicate Assignment Number | 409 | Passed |
| 7 | Update Assignment | 200 | Passed |
| 8 | Update Status to IN_PROGRESS | 200 | Passed |
| 9 | Add Assignment Member | 200 | Passed |
| 10 | Member Check-in | 200 | Passed |
| 11 | Member Check-out | 200 | Passed |
| 12 | Update Status to COMPLETED | 200 | Passed |
| 13 | Update Completed Assignment | 409 | Passed |

## 16. Summary

Field Assignment Management API provides the core functionality required to manage field operations assignments and employee participation.

Implemented capabilities:
- Assignment CRUD.
- Assignment filtering.
- Assignment status workflow.
- Employee assignment management.
- Member check-in and check-out.
- DateTime UTC normalization.
- Business rule validation.

**Implementation Status: Completed**

**Module:** Crew & Field Operations Management  
**Stage:** 11I – Field Assignment Management