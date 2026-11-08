CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE asset_categories (
    "Id" uuid NOT NULL,
    "Code" character varying(30) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_asset_categories" PRIMARY KEY ("Id")
);

CREATE TABLE employees (
    "Id" uuid NOT NULL,
    "EmployeeNumber" character varying(30) NOT NULL,
    "FullName" character varying(150) NOT NULL,
    "Email" character varying(150),
    "PhoneNumber" character varying(30),
    "Position" character varying(100),
    "Department" character varying(100),
    "HireDate" date,
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_employees" PRIMARY KEY ("Id")
);

CREATE TABLE item_categories (
    "Id" uuid NOT NULL,
    "Code" character varying(30) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_item_categories" PRIMARY KEY ("Id")
);

CREATE TABLE locations (
    "Id" uuid NOT NULL,
    "Code" character varying(30) NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Description" character varying(500),
    "Address" character varying(500),
    "City" character varying(100),
    "Province" character varying(100),
    "Latitude" numeric(10,7),
    "Longitude" numeric(10,7),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_locations" PRIMARY KEY ("Id")
);

CREATE TABLE permissions (
    "Id" uuid NOT NULL,
    "Code" character varying(50) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Module" character varying(100) NOT NULL,
    "Description" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_permissions" PRIMARY KEY ("Id")
);

CREATE TABLE roles (
    "Id" uuid NOT NULL,
    "Code" character varying(30) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_roles" PRIMARY KEY ("Id")
);

CREATE TABLE shifts (
    "Id" uuid NOT NULL,
    "Code" character varying(30) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "StartTime" time without time zone NOT NULL,
    "EndTime" time without time zone NOT NULL,
    "Description" character varying(500),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_shifts" PRIMARY KEY ("Id")
);

CREATE TABLE units_of_measure (
    "Id" uuid NOT NULL,
    "Code" character varying(20) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(300),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_units_of_measure" PRIMARY KEY ("Id")
);

CREATE TABLE users (
    "Id" uuid NOT NULL,
    "Username" character varying(50) NOT NULL,
    "Email" character varying(150) NOT NULL,
    "FullName" character varying(150) NOT NULL,
    "PasswordHash" character varying(500) NOT NULL,
    "PhoneNumber" character varying(30),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "LastLoginAt" timestamp with time zone,
    "FailedLoginAttempts" integer NOT NULL DEFAULT 0,
    "LockoutEnd" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_users" PRIMARY KEY ("Id")
);

CREATE TABLE assets (
    "Id" uuid NOT NULL,
    "AssetCode" character varying(50) NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Description" character varying(1000),
    "AssetCategoryId" uuid NOT NULL,
    "LocationId" uuid NOT NULL,
    "SerialNumber" character varying(100),
    "Brand" character varying(100),
    "Model" character varying(100),
    "PurchaseDate" date,
    "PurchasePrice" numeric(18,2),
    "WarrantyExpiryDate" date,
    "Status" character varying(30) NOT NULL DEFAULT 'AVAILABLE',
    "QRCode" character varying(200),
    "Notes" character varying(1000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_assets" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_assets_asset_categories_AssetCategoryId" FOREIGN KEY ("AssetCategoryId") REFERENCES asset_categories ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_assets_locations_LocationId" FOREIGN KEY ("LocationId") REFERENCES locations ("Id") ON DELETE RESTRICT
);

CREATE TABLE role_permissions (
    "Id" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    "PermissionId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_role_permissions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_role_permissions_permissions_PermissionId" FOREIGN KEY ("PermissionId") REFERENCES permissions ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_role_permissions_roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES roles ("Id") ON DELETE CASCADE
);

CREATE TABLE crew_schedules (
    "Id" uuid NOT NULL,
    "EmployeeId" uuid NOT NULL,
    "ShiftId" uuid NOT NULL,
    "LocationId" uuid NOT NULL,
    "WorkDate" date NOT NULL,
    "Status" character varying(30) NOT NULL DEFAULT 'SCHEDULED',
    "Notes" character varying(1000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_crew_schedules" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_crew_schedules_employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES employees ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_crew_schedules_locations_LocationId" FOREIGN KEY ("LocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_crew_schedules_shifts_ShiftId" FOREIGN KEY ("ShiftId") REFERENCES shifts ("Id") ON DELETE RESTRICT
);

CREATE TABLE items (
    "Id" uuid NOT NULL,
    "Code" character varying(30) NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Description" character varying(500),
    "ItemCategoryId" uuid NOT NULL,
    "UnitOfMeasureId" uuid NOT NULL,
    "MinimumStock" numeric(18,3) NOT NULL,
    "UnitPrice" numeric(18,2),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_items_item_categories_ItemCategoryId" FOREIGN KEY ("ItemCategoryId") REFERENCES item_categories ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_items_units_of_measure_UnitOfMeasureId" FOREIGN KEY ("UnitOfMeasureId") REFERENCES units_of_measure ("Id") ON DELETE RESTRICT
);

CREATE TABLE audit_logs (
    "Id" uuid NOT NULL,
    "UserId" uuid,
    "Action" character varying(100) NOT NULL,
    "EntityName" character varying(150) NOT NULL,
    "EntityId" character varying(100),
    "OldValues" jsonb,
    "NewValues" jsonb,
    "IpAddress" character varying(50),
    "UserAgent" character varying(1000),
    "OccurredAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_audit_logs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_audit_logs_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE field_assignments (
    "Id" uuid NOT NULL,
    "AssignmentNumber" character varying(50) NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Description" character varying(2000),
    "LocationId" uuid NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "Priority" character varying(20) NOT NULL DEFAULT 'MEDIUM',
    "Status" character varying(30) NOT NULL DEFAULT 'PLANNED',
    "CreatedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_field_assignments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_field_assignments_locations_LocationId" FOREIGN KEY ("LocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_field_assignments_users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE login_histories (
    "Id" uuid NOT NULL,
    "UserId" uuid,
    "Username" character varying(50) NOT NULL,
    "IsSuccessful" boolean NOT NULL,
    "FailureReason" character varying(500),
    "IpAddress" character varying(50),
    "UserAgent" character varying(1000),
    "LoginAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_login_histories" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_login_histories_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE material_requests (
    "Id" uuid NOT NULL,
    "RequestNumber" character varying(50) NOT NULL,
    "RequestingLocationId" uuid NOT NULL,
    "DestinationLocationId" uuid NOT NULL,
    "RequestDate" timestamp with time zone NOT NULL,
    "RequiredDate" date,
    "Priority" character varying(20) NOT NULL DEFAULT 'MEDIUM',
    "Status" character varying(30) NOT NULL DEFAULT 'PENDING',
    "Notes" character varying(1000),
    "RequestedByUserId" uuid,
    "ApprovedByUserId" uuid,
    "ApprovedAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_material_requests" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_material_requests_locations_DestinationLocationId" FOREIGN KEY ("DestinationLocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_material_requests_locations_RequestingLocationId" FOREIGN KEY ("RequestingLocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_material_requests_users_ApprovedByUserId" FOREIGN KEY ("ApprovedByUserId") REFERENCES users ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_material_requests_users_RequestedByUserId" FOREIGN KEY ("RequestedByUserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE stock_opnames (
    "Id" uuid NOT NULL,
    "OpnameNumber" character varying(50) NOT NULL,
    "LocationId" uuid NOT NULL,
    "OpnameDate" timestamp with time zone NOT NULL,
    "Status" character varying(30) NOT NULL DEFAULT 'DRAFT',
    "Notes" character varying(1000),
    "CreatedByUserId" uuid,
    "ApprovedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_stock_opnames" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_stock_opnames_locations_LocationId" FOREIGN KEY ("LocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_stock_opnames_users_ApprovedByUserId" FOREIGN KEY ("ApprovedByUserId") REFERENCES users ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_stock_opnames_users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE user_roles (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    "AssignedAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_user_roles" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_user_roles_roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES roles ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_user_roles_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
);

CREATE TABLE asset_movements (
    "Id" uuid NOT NULL,
    "AssetId" uuid NOT NULL,
    "FromLocationId" uuid NOT NULL,
    "ToLocationId" uuid NOT NULL,
    "MovementDate" timestamp with time zone NOT NULL,
    "Reason" character varying(500),
    "MovedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_asset_movements" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_asset_movements_assets_AssetId" FOREIGN KEY ("AssetId") REFERENCES assets ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_asset_movements_locations_FromLocationId" FOREIGN KEY ("FromLocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_asset_movements_locations_ToLocationId" FOREIGN KEY ("ToLocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_asset_movements_users_MovedByUserId" FOREIGN KEY ("MovedByUserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE maintenance_plans (
    "Id" uuid NOT NULL,
    "AssetId" uuid NOT NULL,
    "PlanName" character varying(150) NOT NULL,
    "Description" character varying(1000) NOT NULL,
    "IntervalDays" integer NOT NULL,
    "StartDate" date NOT NULL,
    "NextMaintenanceDate" date,
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_maintenance_plans" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_maintenance_plans_assets_AssetId" FOREIGN KEY ("AssetId") REFERENCES assets ("Id") ON DELETE RESTRICT
);

CREATE TABLE inventory_stocks (
    "Id" uuid NOT NULL,
    "ItemId" uuid NOT NULL,
    "LocationId" uuid NOT NULL,
    "Quantity" numeric(18,3) NOT NULL,
    "ReservedQuantity" numeric(18,3) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_inventory_stocks" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_inventory_stocks_items_ItemId" FOREIGN KEY ("ItemId") REFERENCES items ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_inventory_stocks_locations_LocationId" FOREIGN KEY ("LocationId") REFERENCES locations ("Id") ON DELETE RESTRICT
);

CREATE TABLE stock_transactions (
    "Id" uuid NOT NULL,
    "TransactionNumber" character varying(50) NOT NULL,
    "ItemId" uuid NOT NULL,
    "LocationId" uuid NOT NULL,
    "TransactionType" character varying(30) NOT NULL,
    "Quantity" numeric(18,3) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "ReferenceNumber" character varying(100),
    "Notes" character varying(1000),
    "PerformedByUserId" uuid,
    "TransactionDate" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_stock_transactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_stock_transactions_items_ItemId" FOREIGN KEY ("ItemId") REFERENCES items ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_stock_transactions_locations_LocationId" FOREIGN KEY ("LocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_stock_transactions_users_PerformedByUserId" FOREIGN KEY ("PerformedByUserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE field_assignment_members (
    "Id" uuid NOT NULL,
    "FieldAssignmentId" uuid NOT NULL,
    "EmployeeId" uuid NOT NULL,
    "Responsibility" character varying(200),
    "Status" character varying(30) NOT NULL DEFAULT 'ASSIGNED',
    "CheckInTime" timestamp with time zone,
    "CheckOutTime" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_field_assignment_members" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_field_assignment_members_employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES employees ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_field_assignment_members_field_assignments_FieldAssignmentId" FOREIGN KEY ("FieldAssignmentId") REFERENCES field_assignments ("Id") ON DELETE RESTRICT
);

CREATE TABLE material_request_items (
    "Id" uuid NOT NULL,
    "MaterialRequestId" uuid NOT NULL,
    "ItemId" uuid NOT NULL,
    "RequestedQuantity" numeric(18,3) NOT NULL,
    "ApprovedQuantity" numeric(18,3),
    "Notes" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_material_request_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_material_request_items_items_ItemId" FOREIGN KEY ("ItemId") REFERENCES items ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_material_request_items_material_requests_MaterialRequestId" FOREIGN KEY ("MaterialRequestId") REFERENCES material_requests ("Id") ON DELETE RESTRICT
);

CREATE TABLE shipments (
    "Id" uuid NOT NULL,
    "ShipmentNumber" character varying(50) NOT NULL,
    "MaterialRequestId" uuid,
    "OriginLocationId" uuid NOT NULL,
    "DestinationLocationId" uuid NOT NULL,
    "TrackingNumber" character varying(100),
    "CarrierName" character varying(150),
    "ShippedAt" timestamp with time zone,
    "EstimatedArrivalAt" timestamp with time zone,
    "DeliveredAt" timestamp with time zone,
    "Status" character varying(30) NOT NULL DEFAULT 'PREPARING',
    "Notes" character varying(1000),
    "CreatedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_shipments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_shipments_locations_DestinationLocationId" FOREIGN KEY ("DestinationLocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_shipments_locations_OriginLocationId" FOREIGN KEY ("OriginLocationId") REFERENCES locations ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_shipments_material_requests_MaterialRequestId" FOREIGN KEY ("MaterialRequestId") REFERENCES material_requests ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_shipments_users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE stock_opname_details (
    "Id" uuid NOT NULL,
    "StockOpnameId" uuid NOT NULL,
    "ItemId" uuid NOT NULL,
    "SystemQuantity" numeric(18,3) NOT NULL,
    "PhysicalQuantity" numeric(18,3) NOT NULL,
    "Notes" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_stock_opname_details" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_stock_opname_details_items_ItemId" FOREIGN KEY ("ItemId") REFERENCES items ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_stock_opname_details_stock_opnames_StockOpnameId" FOREIGN KEY ("StockOpnameId") REFERENCES stock_opnames ("Id") ON DELETE RESTRICT
);

CREATE TABLE maintenance_work_orders (
    "Id" uuid NOT NULL,
    "WorkOrderNumber" character varying(50) NOT NULL,
    "AssetId" uuid NOT NULL,
    "MaintenancePlanId" uuid,
    "MaintenanceType" character varying(30) NOT NULL DEFAULT 'PREVENTIVE',
    "Title" character varying(200) NOT NULL,
    "Description" character varying(2000) NOT NULL,
    "ScheduledDate" date NOT NULL,
    "CompletedDate" date,
    "Priority" character varying(20) NOT NULL DEFAULT 'MEDIUM',
    "Status" character varying(30) NOT NULL DEFAULT 'OPEN',
    "EstimatedCost" numeric(18,2),
    "ActualCost" numeric(18,2),
    "AssignedToEmployeeId" uuid,
    "CreatedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_maintenance_work_orders" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_maintenance_work_orders_assets_AssetId" FOREIGN KEY ("AssetId") REFERENCES assets ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_maintenance_work_orders_employees_AssignedToEmployeeId" FOREIGN KEY ("AssignedToEmployeeId") REFERENCES employees ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_maintenance_work_orders_maintenance_plans_MaintenancePlanId" FOREIGN KEY ("MaintenancePlanId") REFERENCES maintenance_plans ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_maintenance_work_orders_users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES users ("Id") ON DELETE SET NULL
);

CREATE TABLE shipment_items (
    "Id" uuid NOT NULL,
    "ShipmentId" uuid NOT NULL,
    "ItemId" uuid NOT NULL,
    "Quantity" numeric(18,3) NOT NULL,
    "Notes" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_shipment_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_shipment_items_items_ItemId" FOREIGN KEY ("ItemId") REFERENCES items ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_shipment_items_shipments_ShipmentId" FOREIGN KEY ("ShipmentId") REFERENCES shipments ("Id") ON DELETE RESTRICT
);

CREATE TABLE shipment_tracking_events (
    "Id" uuid NOT NULL,
    "ShipmentId" uuid NOT NULL,
    "Status" character varying(30) NOT NULL,
    "Description" character varying(1000) NOT NULL,
    "LocationName" character varying(200),
    "EventDate" timestamp with time zone NOT NULL,
    "Latitude" numeric(10,7),
    "Longitude" numeric(10,7),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_shipment_tracking_events" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_shipment_tracking_events_shipments_ShipmentId" FOREIGN KEY ("ShipmentId") REFERENCES shipments ("Id") ON DELETE RESTRICT
);

CREATE TABLE maintenance_records (
    "Id" uuid NOT NULL,
    "MaintenanceWorkOrderId" uuid NOT NULL,
    "PerformedByEmployeeId" uuid,
    "MaintenanceDate" timestamp with time zone NOT NULL,
    "WorkDescription" character varying(2000) NOT NULL,
    "PartsReplaced" character varying(1000),
    "LaborCost" numeric(18,2),
    "PartsCost" numeric(18,2),
    "Notes" character varying(1000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_maintenance_records" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_maintenance_records_employees_PerformedByEmployeeId" FOREIGN KEY ("PerformedByEmployeeId") REFERENCES employees ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_maintenance_records_maintenance_work_orders_MaintenanceWork~" FOREIGN KEY ("MaintenanceWorkOrderId") REFERENCES maintenance_work_orders ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX "IX_asset_categories_Code" ON asset_categories ("Code");

CREATE INDEX "IX_asset_movements_AssetId" ON asset_movements ("AssetId");

CREATE INDEX "IX_asset_movements_FromLocationId" ON asset_movements ("FromLocationId");

CREATE INDEX "IX_asset_movements_MovedByUserId" ON asset_movements ("MovedByUserId");

CREATE INDEX "IX_asset_movements_MovementDate" ON asset_movements ("MovementDate");

CREATE INDEX "IX_asset_movements_ToLocationId" ON asset_movements ("ToLocationId");

CREATE INDEX "IX_assets_AssetCategoryId" ON assets ("AssetCategoryId");

CREATE UNIQUE INDEX "IX_assets_AssetCode" ON assets ("AssetCode");

CREATE INDEX "IX_assets_LocationId" ON assets ("LocationId");

CREATE UNIQUE INDEX "IX_assets_QRCode" ON assets ("QRCode") WHERE "QRCode" IS NOT NULL;

CREATE INDEX "IX_assets_SerialNumber" ON assets ("SerialNumber");

CREATE INDEX "IX_assets_Status" ON assets ("Status");

CREATE INDEX "IX_audit_logs_Action" ON audit_logs ("Action");

CREATE INDEX "IX_audit_logs_EntityId" ON audit_logs ("EntityId");

CREATE INDEX "IX_audit_logs_EntityName" ON audit_logs ("EntityName");

CREATE INDEX "IX_audit_logs_OccurredAt" ON audit_logs ("OccurredAt");

CREATE INDEX "IX_audit_logs_UserId" ON audit_logs ("UserId");

CREATE UNIQUE INDEX "IX_crew_schedules_EmployeeId_ShiftId_WorkDate" ON crew_schedules ("EmployeeId", "ShiftId", "WorkDate");

CREATE INDEX "IX_crew_schedules_LocationId" ON crew_schedules ("LocationId");

CREATE INDEX "IX_crew_schedules_ShiftId" ON crew_schedules ("ShiftId");

CREATE INDEX "IX_crew_schedules_Status" ON crew_schedules ("Status");

CREATE INDEX "IX_crew_schedules_WorkDate" ON crew_schedules ("WorkDate");

CREATE UNIQUE INDEX "IX_employees_Email" ON employees ("Email") WHERE "Email" IS NOT NULL;

CREATE UNIQUE INDEX "IX_employees_EmployeeNumber" ON employees ("EmployeeNumber");

CREATE INDEX "IX_field_assignment_members_EmployeeId" ON field_assignment_members ("EmployeeId");

CREATE UNIQUE INDEX "IX_field_assignment_members_FieldAssignmentId_EmployeeId" ON field_assignment_members ("FieldAssignmentId", "EmployeeId");

CREATE INDEX "IX_field_assignment_members_Status" ON field_assignment_members ("Status");

CREATE UNIQUE INDEX "IX_field_assignments_AssignmentNumber" ON field_assignments ("AssignmentNumber");

CREATE INDEX "IX_field_assignments_CreatedByUserId" ON field_assignments ("CreatedByUserId");

CREATE INDEX "IX_field_assignments_EndDate" ON field_assignments ("EndDate");

CREATE INDEX "IX_field_assignments_LocationId" ON field_assignments ("LocationId");

CREATE INDEX "IX_field_assignments_Priority" ON field_assignments ("Priority");

CREATE INDEX "IX_field_assignments_StartDate" ON field_assignments ("StartDate");

CREATE INDEX "IX_field_assignments_Status" ON field_assignments ("Status");

CREATE UNIQUE INDEX "IX_inventory_stocks_ItemId_LocationId" ON inventory_stocks ("ItemId", "LocationId");

CREATE INDEX "IX_inventory_stocks_LocationId" ON inventory_stocks ("LocationId");

CREATE UNIQUE INDEX "IX_item_categories_Code" ON item_categories ("Code");

CREATE UNIQUE INDEX "IX_items_Code" ON items ("Code");

CREATE INDEX "IX_items_ItemCategoryId" ON items ("ItemCategoryId");

CREATE INDEX "IX_items_UnitOfMeasureId" ON items ("UnitOfMeasureId");

CREATE UNIQUE INDEX "IX_locations_Code" ON locations ("Code");

CREATE INDEX "IX_login_histories_IsSuccessful" ON login_histories ("IsSuccessful");

CREATE INDEX "IX_login_histories_LoginAt" ON login_histories ("LoginAt");

CREATE INDEX "IX_login_histories_UserId" ON login_histories ("UserId");

CREATE INDEX "IX_login_histories_Username" ON login_histories ("Username");

CREATE INDEX "IX_maintenance_plans_AssetId" ON maintenance_plans ("AssetId");

CREATE INDEX "IX_maintenance_plans_IsActive" ON maintenance_plans ("IsActive");

CREATE INDEX "IX_maintenance_plans_NextMaintenanceDate" ON maintenance_plans ("NextMaintenanceDate");

CREATE INDEX "IX_maintenance_records_MaintenanceDate" ON maintenance_records ("MaintenanceDate");

CREATE INDEX "IX_maintenance_records_MaintenanceWorkOrderId" ON maintenance_records ("MaintenanceWorkOrderId");

CREATE INDEX "IX_maintenance_records_PerformedByEmployeeId" ON maintenance_records ("PerformedByEmployeeId");

CREATE INDEX "IX_maintenance_work_orders_AssetId" ON maintenance_work_orders ("AssetId");

CREATE INDEX "IX_maintenance_work_orders_AssignedToEmployeeId" ON maintenance_work_orders ("AssignedToEmployeeId");

CREATE INDEX "IX_maintenance_work_orders_CreatedByUserId" ON maintenance_work_orders ("CreatedByUserId");

CREATE INDEX "IX_maintenance_work_orders_MaintenancePlanId" ON maintenance_work_orders ("MaintenancePlanId");

CREATE INDEX "IX_maintenance_work_orders_Priority" ON maintenance_work_orders ("Priority");

CREATE INDEX "IX_maintenance_work_orders_ScheduledDate" ON maintenance_work_orders ("ScheduledDate");

CREATE INDEX "IX_maintenance_work_orders_Status" ON maintenance_work_orders ("Status");

CREATE UNIQUE INDEX "IX_maintenance_work_orders_WorkOrderNumber" ON maintenance_work_orders ("WorkOrderNumber");

CREATE INDEX "IX_material_request_items_ItemId" ON material_request_items ("ItemId");

CREATE UNIQUE INDEX "IX_material_request_items_MaterialRequestId_ItemId" ON material_request_items ("MaterialRequestId", "ItemId");

CREATE INDEX "IX_material_requests_ApprovedByUserId" ON material_requests ("ApprovedByUserId");

CREATE INDEX "IX_material_requests_DestinationLocationId" ON material_requests ("DestinationLocationId");

CREATE INDEX "IX_material_requests_Priority" ON material_requests ("Priority");

CREATE INDEX "IX_material_requests_RequestDate" ON material_requests ("RequestDate");

CREATE INDEX "IX_material_requests_RequestedByUserId" ON material_requests ("RequestedByUserId");

CREATE INDEX "IX_material_requests_RequestingLocationId" ON material_requests ("RequestingLocationId");

CREATE UNIQUE INDEX "IX_material_requests_RequestNumber" ON material_requests ("RequestNumber");

CREATE INDEX "IX_material_requests_Status" ON material_requests ("Status");

CREATE UNIQUE INDEX "IX_permissions_Code" ON permissions ("Code");

CREATE INDEX "IX_permissions_Module" ON permissions ("Module");

CREATE INDEX "IX_role_permissions_PermissionId" ON role_permissions ("PermissionId");

CREATE UNIQUE INDEX "IX_role_permissions_RoleId_PermissionId" ON role_permissions ("RoleId", "PermissionId");

CREATE UNIQUE INDEX "IX_roles_Code" ON roles ("Code");

CREATE INDEX "IX_roles_IsActive" ON roles ("IsActive");

CREATE UNIQUE INDEX "IX_shifts_Code" ON shifts ("Code");

CREATE INDEX "IX_shipment_items_ItemId" ON shipment_items ("ItemId");

CREATE UNIQUE INDEX "IX_shipment_items_ShipmentId_ItemId" ON shipment_items ("ShipmentId", "ItemId");

CREATE INDEX "IX_shipment_tracking_events_ShipmentId" ON shipment_tracking_events ("ShipmentId");

CREATE INDEX "IX_shipment_tracking_events_ShipmentId_EventDate" ON shipment_tracking_events ("ShipmentId", "EventDate");

CREATE INDEX "IX_shipments_CreatedByUserId" ON shipments ("CreatedByUserId");

CREATE INDEX "IX_shipments_DestinationLocationId" ON shipments ("DestinationLocationId");

CREATE INDEX "IX_shipments_EstimatedArrivalAt" ON shipments ("EstimatedArrivalAt");

CREATE INDEX "IX_shipments_MaterialRequestId" ON shipments ("MaterialRequestId");

CREATE INDEX "IX_shipments_OriginLocationId" ON shipments ("OriginLocationId");

CREATE UNIQUE INDEX "IX_shipments_ShipmentNumber" ON shipments ("ShipmentNumber");

CREATE INDEX "IX_shipments_ShippedAt" ON shipments ("ShippedAt");

CREATE INDEX "IX_shipments_Status" ON shipments ("Status");

CREATE UNIQUE INDEX "IX_shipments_TrackingNumber" ON shipments ("TrackingNumber") WHERE "TrackingNumber" IS NOT NULL;

CREATE INDEX "IX_stock_opname_details_ItemId" ON stock_opname_details ("ItemId");

CREATE UNIQUE INDEX "IX_stock_opname_details_StockOpnameId_ItemId" ON stock_opname_details ("StockOpnameId", "ItemId");

CREATE INDEX "IX_stock_opnames_ApprovedByUserId" ON stock_opnames ("ApprovedByUserId");

CREATE INDEX "IX_stock_opnames_CreatedByUserId" ON stock_opnames ("CreatedByUserId");

CREATE INDEX "IX_stock_opnames_LocationId" ON stock_opnames ("LocationId");

CREATE INDEX "IX_stock_opnames_OpnameDate" ON stock_opnames ("OpnameDate");

CREATE UNIQUE INDEX "IX_stock_opnames_OpnameNumber" ON stock_opnames ("OpnameNumber");

CREATE INDEX "IX_stock_transactions_ItemId" ON stock_transactions ("ItemId");

CREATE INDEX "IX_stock_transactions_LocationId" ON stock_transactions ("LocationId");

CREATE INDEX "IX_stock_transactions_PerformedByUserId" ON stock_transactions ("PerformedByUserId");

CREATE INDEX "IX_stock_transactions_TransactionDate" ON stock_transactions ("TransactionDate");

CREATE UNIQUE INDEX "IX_stock_transactions_TransactionNumber" ON stock_transactions ("TransactionNumber");

CREATE UNIQUE INDEX "IX_units_of_measure_Code" ON units_of_measure ("Code");

CREATE INDEX "IX_user_roles_RoleId" ON user_roles ("RoleId");

CREATE UNIQUE INDEX "IX_user_roles_UserId_RoleId" ON user_roles ("UserId", "RoleId");

CREATE UNIQUE INDEX "IX_users_Email" ON users ("Email");

CREATE INDEX "IX_users_IsActive" ON users ("IsActive");

CREATE UNIQUE INDEX "IX_users_Username" ON users ("Username");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261107234656_InitialCreate', '10.0.14');

COMMIT;

