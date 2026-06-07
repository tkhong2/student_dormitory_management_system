# Implementation Plan: Room Registration Workflow

## Overview

This implementation plan covers the complete Room Registration Workflow feature for the Dormitory Management System. The feature enables students to browse available rooms and submit applications, allows authorized staff to review and approve applications with building-level authorization, and automatically creates rental contracts when students accept approved applications.

The implementation follows the existing microservices architecture with:
- **Backend**: C# (.NET 9.0) with Entity Framework Core across three services
- **Frontend**: Vue.js 3 with Ant Design Vue components
- **API Gateway**: Ocelot for routing requests

## Tasks

- [ ] 1. Set up backend infrastructure and data models
  - [ ] 1.1 Create RoomApplication entity and repository in ContractStudentService
    - Add RoomApplication entity to ContractStudentService.Domain/Entities with all properties from design
    - Create IRoomApplicationRepository interface in Application/Interfaces
    - Implement RoomApplicationRepository in Infrastructure/Repositories
    - Add DbSet<RoomApplication> to ContractStudentDbContext
    - _Requirements: 1.3, 1.4, 7.1, 9.1_
  
  - [ ] 1.2 Create Contract entity extensions in ContractStudentService
    - Verify Contract entity matches design specifications
    - Add navigation property to RoomApplication
    - Create IContractRepository interface in Application/Interfaces
    - Implement ContractRepository with sequence generation method
    - _Requirements: 4.3, 4.5, 9.2_
  
  - [ ] 1.3 Create BuildingAssignment entity in BillingMaintenanceService
    - Add BuildingAssignment entity to BillingMaintenanceService.Domain/Entities
    - Create IBuildingAssignmentRepository interface
    - Implement BuildingAssignmentRepository in Infrastructure
    - Add DbSet<BuildingAssignment> to BillingMaintenanceDbContext
    - _Requirements: 8.1, 8.2_
  
  - [ ] 1.4 Create and run database migrations
    - Generate migration for RoomApplication in ContractStudentService
    - Generate migration for Contract updates in ContractStudentService
    - Generate migration for BuildingAssignment in BillingMaintenanceService
    - Apply all migrations to development database
    - Verify schema with SQL queries
    - _Requirements: 9.1, 9.2, 8.1_

- [ ] 2. Implement backend API endpoints for Room Applications
  - [ ] 2.1 Create RoomApplicationsController in ContractStudentService
    - Create controller with all CRUD endpoints (GET, POST, PUT)
    - Implement GET /api/roomapplications (list all)
    - Implement GET /api/roomapplications/{id} (get by ID)
    - Implement GET /api/roomapplications/student/{studentId} (student's applications)
    - Implement POST /api/roomapplications (create application)
    - _Requirements: 1.3, 1.4, 7.2_
  
  - [ ] 2.2 Implement application creation logic with validation
    - Validate required fields (StudentId, PreferredBuildingId, PreferredRoomTypeId, dates)
    - Check for existing active applications
    - Validate room availability by calling RoomBuildingService
    - Set priority based on IsLocalStudent flag
    - Create application with status "Pending"
    - _Requirements: 1.5, 1.6, 9.5, 10.1, 10.2_
  
  - [ ] 2.3 Implement application approval endpoint
    - Create PUT /api/roomapplications/{id}/approve endpoint
    - Validate application status is Pending or UnderReview
    - Validate assigned room availability
    - Update status to Approved with reviewer info and assigned room
    - Store snapshot data (AssignedRoomNumber, AssignedBuildingName)
    - _Requirements: 3.1, 3.2, 3.3, 3.8, 9.1_
  
  - [ ] 2.4 Implement application rejection endpoint
    - Create PUT /api/roomapplications/{id}/reject endpoint
    - Require and validate RejectReason parameter
    - Update status to Rejected with reviewer info and reason
    - _Requirements: 3.4, 3.5, 3.6_
  
  - [ ] 2.5 Implement application cancellation endpoint
    - Create PUT /api/roomapplications/{id}/cancel endpoint
    - Validate application is Pending
    - Update status to Cancelled with timestamp and reason
    - Set CancelledBySelf flag appropriately
    - _Requirements: 7.6_

- [ ] 3. Implement authorization service and building assignments
  - [ ] 3.1 Create AuthorizationService in BillingMaintenanceService
    - Implement AuthorizeApplicationActionAsync method
    - Check user role (Admin gets full access)
    - Check Staff building assignments
    - Throw UnauthorizedException for unauthorized access
    - _Requirements: 2.1, 2.3, 2.4, 5.2, 5.6_
  
  - [ ] 3.2 Create BuildingAssignmentsController in BillingMaintenanceService
    - Implement GET /api/buildingassignments (list all)
    - Implement GET /api/buildingassignments/staff/{userId} (staff's assignments)
    - Implement POST /api/buildingassignments (create assignment)
    - Implement DELETE /api/buildingassignments/{id} (remove assignment)
    - _Requirements: 8.1, 8.2, 8.3_
  
  - [ ] 3.3 Implement building assignment validation
    - Validate user role is Staff when creating assignment
    - Prevent duplicate (UserId, BuildingId) assignments
    - Allow multiple building assignments per staff member
    - _Requirements: 8.4, 8.7_
  
  - [ ] 3.4 Add authorization middleware to application endpoints
    - Add authorization checks to approve/reject/edit endpoints
    - Call BillingMaintenanceService authorization API
    - Return 403 Forbidden for unauthorized requests
    - _Requirements: 2.3, 5.2, 5.6_

- [ ] 4. Implement automatic contract creation
  - [ ] 4.1 Create contract acceptance endpoint
    - Create POST /api/roomapplications/{id}/accept endpoint
    - Validate application status is Approved
    - Validate student ownership of application
    - Check for existing active contracts
    - _Requirements: 4.1, 9.6_
  
  - [ ] 4.2 Implement contract generation logic
    - Generate unique ContractCode in format HD{YEAR}{SEQUENCE}
    - Copy all required fields from RoomApplication
    - Store snapshot data (RoomNumber, BuildingName, RoomTypeName, MonthlyRent)
    - Fetch default rates from system settings
    - Set initial status to Pending
    - _Requirements: 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8, 9.2_
  
  - [ ] 4.3 Link contract to application
    - Create bidirectional reference between Contract and Application
    - Update application to reference created contract
    - Handle transaction rollback on error
    - _Requirements: 4.10, 9.7_
  
  - [ ] 4.4 Implement error handling for contract creation
    - Log contract creation failures
    - Keep application status as Approved on error
    - Return error response to client
    - _Requirements: 4.11_

- [ ] 5. Checkpoint - Verify backend APIs
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 6. Implement frontend student room browsing
  - [ ] 6.1 Update StudentRoomListView.vue for room browsing
    - Fetch available rooms from RoomBuildingService API
    - Display room cards with all required information (number, building, capacity, price)
    - Implement building filter dropdown
    - Implement room type filter dropdown
    - Show only rooms with status "Available"
    - _Requirements: 1.1, 6.1, 6.2, 6.3, 6.4_
  
  - [ ] 6.2 Update StudentRoomRegistrationView.vue for application submission
    - Pre-populate form with user information
    - Display selected room details (from localStorage or route params)
    - Collect all required fields (dates, special requirements, note)
    - Add IsLocalStudent checkbox
    - Validate required fields before submission
    - _Requirements: 1.2, 1.4, 10.6_
  
  - [ ] 6.3 Implement application submission handler
    - Get studentId from user's userId via API
    - Prepare application data with snapshot fields
    - Call POST /api/roomapplications endpoint
    - Handle validation errors and display messages
    - Show success message and redirect to dashboard
    - _Requirements: 1.3, 1.5, 1.7_
  
  - [ ] 6.4 Create MyApplicationsView.vue for tracking
    - Display list of student's applications with status
    - Show submission date, preferred room, and review info
    - Display reject reason if status is Rejected
    - Display assigned room if status is Approved
    - Add Accept button for approved applications
    - Add Cancel button for pending applications
    - _Requirements: 7.2, 7.3, 7.4, 7.5_

- [ ] 7. Implement frontend staff application review
  - [ ] 7.1 Update RoomApplicationListView.vue for staff
    - Fetch all applications from ContractStudentService API
    - Display application cards with student info and preferred room
    - Show status badges (Pending, Approved, Rejected, Cancelled)
    - Implement filters (status, building, priority)
    - Add sorting options (CreatedAt, Priority)
    - _Requirements: 2.2, 10.3_
  
  - [ ] 7.2 Add visual indicators for building authorization
    - Fetch staff's building assignments from BillingMaintenanceService
    - Highlight applications within staff's authorized buildings
    - Display lock icon or disabled state for unauthorized applications
    - Show tooltip explaining authorization restrictions
    - _Requirements: 2.6, 5.5_
  
  - [ ] 7.3 Implement application approval modal
    - Create modal dialog with room selection dropdown
    - Fetch available rooms for the preferred building
    - Display selected room details before approval
    - Call PUT /api/roomapplications/{id}/approve endpoint
    - Handle authorization errors (403) gracefully
    - _Requirements: 3.1, 3.3, 3.8_
  
  - [ ] 7.4 Implement application rejection modal
    - Create modal dialog with reject reason textarea
    - Require reject reason before submission
    - Call PUT /api/roomapplications/{id}/reject endpoint
    - Handle authorization errors (403) gracefully
    - _Requirements: 3.4, 3.5_

- [ ] 8. Implement frontend admin building assignments
  - [ ] 8.1 Update UserListView.vue for building assignment management
    - Add building assignment section for Staff users
    - Display list of assigned buildings for each staff member
    - Add button to open building assignment modal
    - _Requirements: 8.6_
  
  - [ ] 8.2 Create BuildingAssignmentModal.vue component
    - Display all buildings with checkboxes
    - Load staff's current assignments and check appropriate boxes
    - Allow multi-select for multiple building assignments
    - Call POST /api/buildingassignments to create assignments
    - Call DELETE /api/buildingassignments/{id} to remove assignments
    - _Requirements: 8.1, 8.2, 8.3, 8.4_
  
  - [ ] 8.3 Add validation for building assignment
    - Prevent assignment for non-Staff users
    - Display error message if user role is not Staff
    - _Requirements: 8.7_

- [ ] 9. Implement frontend contract acceptance
  - [ ] 9.1 Update MyContractView.vue for approved applications
    - Fetch student's contracts from ContractStudentService
    - Display contract details (code, room, dates, monthly rent)
    - Show contract status badges
    - _Requirements: 4.9_
  
  - [ ] 9.2 Add acceptance flow to MyApplicationsView.vue
    - Display Accept button for applications with status Approved
    - Show confirmation modal with application and room details
    - Call POST /api/roomapplications/{id}/accept endpoint
    - Display success message with generated ContractCode
    - Redirect to MyContractView after acceptance
    - _Requirements: 4.1, 4.2_
  
  - [ ] 9.3 Handle contract creation errors
    - Display error message if contract creation fails
    - Keep application in Approved state for retry
    - Show contact admin message for persistent errors
    - _Requirements: 4.11_

- [ ] 10. Add API Gateway routes
  - [ ] 10.1 Update ocelot.json with new endpoints
    - Add routes for /api/roomapplications/* to ContractStudentService (port 5059)
    - Add routes for /api/buildingassignments/* to BillingMaintenanceService (port 5002)
    - Add routes for /api/contracts/* to ContractStudentService (port 5059)
    - Configure authentication requirements
    - _Requirements: All_
  
  - [ ] 10.2 Test API Gateway routing
    - Verify all endpoints are accessible through gateway (port 5052)
    - Test authentication token forwarding
    - Test error response forwarding
    - _Requirements: All_

- [ ] 11. Implement database indexing for performance
  - [ ] 11.1 Add indexes to RoomApplication table
    - Create index on StudentId
    - Create index on Status
    - Create index on PreferredBuildingId
    - Create composite index on CreatedAt DESC
    - _Requirements: Performance_
  
  - [ ] 11.2 Add indexes to BuildingAssignment table
    - Create index on UserId
    - Create index on BuildingId
    - _Requirements: Performance_
  
  - [ ] 11.3 Add indexes to Contract table
    - Create index on StudentId
    - Create index on ApplicationId
    - Create index on Status
    - _Requirements: Performance_

- [ ] 12. Final checkpoint - End-to-end testing
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- The implementation follows the existing microservices architecture
- Building authorization is implemented with API calls between services
- Snapshot data ensures historical records remain accurate
- Frontend uses Ant Design Vue components for consistency
- All API calls go through the Ocelot API Gateway

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 0, "tasks": ["1.1", "1.2", "1.3"] },
    { "id": 1, "tasks": ["1.4"] },
    { "id": 2, "tasks": ["2.1", "3.1", "3.2"] },
    { "id": 3, "tasks": ["2.2", "2.3", "2.4", "2.5", "3.3", "3.4"] },
    { "id": 4, "tasks": ["4.1", "4.2"] },
    { "id": 5, "tasks": ["4.3", "4.4"] },
    { "id": 6, "tasks": ["6.1", "6.2", "7.1", "8.1"] },
    { "id": 7, "tasks": ["6.3", "6.4", "7.2", "7.3", "7.4", "8.2", "8.3"] },
    { "id": 8, "tasks": ["9.1", "9.2", "9.3"] },
    { "id": 9, "tasks": ["10.1"] },
    { "id": 10, "tasks": ["10.2"] },
    { "id": 11, "tasks": ["11.1", "11.2", "11.3"] }
  ]
}
```
