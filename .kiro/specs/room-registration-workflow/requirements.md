# Requirements Document

## Introduction

This document specifies the requirements for the Room Registration Workflow feature in the Dormitory Management System. The feature enables students to register for specific dormitory rooms, allows authorized staff to review and approve applications, and automatically creates contracts upon student acceptance. The system implements role-based access control with building-level permissions for staff members.

## Glossary

- **Student**: A user with the role "Student" who can register for dormitory rooms
- **Admin**: A user with the role "Admin" who has full system access and can approve room applications for all buildings
- **Staff**: A user with the role "Staff" who can view all data but can only approve, reject, or edit applications, contracts, and requests for their assigned buildings
- **Room_Application_System**: The subsystem that manages room registration applications
- **Contract_System**: The subsystem that manages dormitory rental contracts
- **Authorization_System**: The subsystem that manages role-based access control and building-level permissions
- **Approval_Workflow**: The process of reviewing and approving or rejecting a room application
- **Building_Assignment**: The association between a Staff member and one or more buildings they are authorized to manage
- **Available_Room**: A room with status "Available" and CurrentOccupants less than MaxOccupants
- **Room_Application**: An entity representing a student's request to register for a specific room
- **Contract**: An entity representing a rental agreement between a student and the dormitory
- **Notification_System**: The subsystem that sends notifications to users

## Requirements

### Requirement 1: Student Room Registration

**User Story:** As a Student, I want to register for a specific dormitory room, so that I can secure accommodation in the building and room of my choice.

#### Acceptance Criteria

1. WHEN a Student accesses the room registration interface, THE Room_Application_System SHALL display all Available_Rooms grouped by building
2. WHEN a Student selects a specific room, THE Room_Application_System SHALL display the room details including room number, building name, capacity, current occupants, available slots, amenities, and monthly rent
3. WHEN a Student submits a room application for an Available_Room, THE Room_Application_System SHALL create a Room_Application with status "Pending" and store the selected room information including RoomId and RoomNumber
4. WHEN a Student submits a room application, THE Room_Application_System SHALL record the StudentId, PreferredBuildingId, PreferredBuildingName, PreferredRoomTypeId, PreferredRoomTypeName, PreferredRoomPrice, RequestedStartDate, RequestedEndDate, SpecialRequirements, Note, and IsLocalStudent fields
5. IF a Student attempts to submit a room application without providing required fields (StudentId, PreferredBuildingId, PreferredRoomTypeId, RequestedStartDate, RequestedEndDate), THEN THE Room_Application_System SHALL reject the submission and display validation error messages
6. IF a Student attempts to submit a room application for a room that is not an Available_Room, THEN THE Room_Application_System SHALL reject the submission and display an error message
7. WHEN a room application is successfully created, THE Notification_System SHALL send a confirmation notification to the Student
8. THE Room_Application_System SHALL store the application timestamp as CreatedAt

### Requirement 2: Application Approval Authorization

**User Story:** As an Admin or Staff member, I want to review and approve room applications according to my assigned permissions, so that I can manage dormitory occupancy within my authorized scope.

#### Acceptance Criteria

1. WHEN an Admin accesses the application approval interface, THE Authorization_System SHALL grant access to review all Room_Applications for all buildings
2. WHEN a Staff member accesses the application approval interface, THE Authorization_System SHALL grant access to view all Room_Applications but restrict approval actions based on Building_Assignment
3. WHEN a Staff member attempts to approve, reject, or edit a Room_Application for a building not in their Building_Assignment, THEN THE Authorization_System SHALL deny the action and display an authorization error message
4. WHEN an Admin approves, rejects, or edits any Room_Application, THE Authorization_System SHALL allow the action regardless of Building_Assignment
5. THE Authorization_System SHALL enforce building-level permissions for Staff members on all approval, rejection, and edit operations for Room_Applications, Contracts, and maintenance requests
6. WHEN a Staff member views the application list, THE Room_Application_System SHALL display visual indicators distinguishing between applications they can manage and applications outside their Building_Assignment

### Requirement 3: Application Approval Workflow

**User Story:** As an Admin or authorized Staff member, I want to approve or reject room applications, so that I can control dormitory occupancy and assign rooms to eligible students.

#### Acceptance Criteria

1. WHEN an authorized user approves a Room_Application, THE Room_Application_System SHALL update the application status to "Approved"
2. WHEN an authorized user approves a Room_Application, THE Room_Application_System SHALL record the ReviewedByUserId, ReviewedByName, and ReviewedAt timestamp
3. WHEN an authorized user approves a Room_Application, THE Room_Application_System SHALL assign the room by setting AssignedRoomId, AssignedRoomNumber, and AssignedBuildingName
4. WHEN an authorized user rejects a Room_Application, THE Room_Application_System SHALL update the application status to "Rejected"
5. WHEN an authorized user rejects a Room_Application, THE Room_Application_System SHALL require and store a RejectReason
6. WHEN an authorized user rejects a Room_Application, THE Room_Application_System SHALL record the ReviewedByUserId, ReviewedByName, and ReviewedAt timestamp
7. WHEN a Room_Application is approved or rejected, THE Notification_System SHALL send a notification to the Student
8. IF an authorized user attempts to approve a Room_Application without selecting a valid room, THEN THE Room_Application_System SHALL reject the approval and display an error message
9. WHILE a Room_Application has status "Approved", THE Room_Application_System SHALL prevent further status changes until the student responds

### Requirement 4: Automatic Contract Creation on Student Acceptance

**User Story:** As a Student, I want my contract to be automatically created when I accept an approved application, so that I can immediately secure my room without manual administrative processing.

#### Acceptance Criteria

1. WHEN a Room_Application status is "Approved", THE Room_Application_System SHALL present an acceptance interface to the Student
2. WHEN a Student accepts an approved Room_Application, THE Contract_System SHALL automatically create a Contract entity
3. WHEN the Contract_System creates a Contract, THE Contract_System SHALL populate the Contract with StudentId, ApplicationId, RoomId, RoomNumber, BuildingId, BuildingName, RoomTypeId, RoomTypeName from the Room_Application
4. WHEN the Contract_System creates a Contract, THE Contract_System SHALL set StartDate and EndDate from the Room_Application RequestedStartDate and RequestedEndDate
5. WHEN the Contract_System creates a Contract, THE Contract_System SHALL generate a unique ContractCode following the pattern "HD{YEAR}{SEQUENCE}"
6. WHEN the Contract_System creates a Contract, THE Contract_System SHALL set MonthlyRent to the PreferredRoomPrice from the Room_Application
7. WHEN the Contract_System creates a Contract, THE Contract_System SHALL set the initial Contract status to "Pending"
8. WHEN the Contract_System creates a Contract, THE Contract_System SHALL set DepositAmount, ElectricityRate, WaterRate, and PaymentDueDay according to system default values or building-specific rates
9. WHEN a Contract is successfully created, THE Notification_System SHALL send a notification to the Student with contract details
10. WHEN a Contract is successfully created, THE Room_Application_System SHALL update the linked Room_Application to reference the created Contract
11. IF the Contract creation fails for any reason, THEN THE Contract_System SHALL log the error, notify the Admin, and maintain the Room_Application status as "Approved" for manual retry

### Requirement 5: Staff Data Access Control

**User Story:** As a Staff member, I want to view all dormitory data while only being able to modify data for my assigned buildings, so that I can monitor overall occupancy while maintaining proper authorization boundaries.

#### Acceptance Criteria

1. THE Authorization_System SHALL allow Staff members to view all Room_Applications, Contracts, and maintenance requests across all buildings
2. WHEN a Staff member attempts to approve, reject, or edit a Room_Application, THE Authorization_System SHALL verify the application's building matches the Staff member's Building_Assignment
3. WHEN a Staff member attempts to create, update, or terminate a Contract, THE Authorization_System SHALL verify the contract's building matches the Staff member's Building_Assignment
4. WHEN a Staff member attempts to process a maintenance request, THE Authorization_System SHALL verify the request's building matches the Staff member's Building_Assignment
5. THE Authorization_System SHALL display read-only indicators on data items outside the Staff member's Building_Assignment
6. IF a Staff member attempts a restricted action on data outside their Building_Assignment, THEN THE Authorization_System SHALL deny the action, log the attempt, and display a permission denied message

### Requirement 6: Room Selection and Availability

**User Story:** As a Student, I want to select a specific room from a list of available rooms, so that I can choose the exact location and room type that meets my preferences.

#### Acceptance Criteria

1. WHEN a Student accesses the room selection interface, THE Room_Application_System SHALL display only rooms with status "Available"
2. WHEN displaying available rooms, THE Room_Application_System SHALL show the room number, building name, floor, capacity, current occupants, available slots, amenities, and monthly rent for each room
3. WHEN a Student filters rooms by building, THE Room_Application_System SHALL display only rooms in the selected building
4. WHEN a Student filters rooms by room type, THE Room_Application_System SHALL display only rooms matching the selected capacity or amenities
5. WHEN a Student selects a specific room, THE Room_Application_System SHALL store the exact RoomId and RoomNumber in the Room_Application
6. THE Room_Application_System SHALL prevent students from selecting rooms with CurrentOccupants equal to or greater than MaxOccupants
7. WHEN a room is selected by multiple students simultaneously, THE Room_Application_System SHALL allow all applications to be submitted but enforce availability validation during the approval process

### Requirement 7: Application Status Tracking

**User Story:** As a Student, I want to track the status of my room application, so that I know whether my application is pending, approved, or rejected.

#### Acceptance Criteria

1. THE Room_Application_System SHALL maintain the application status as one of "Pending", "UnderReview", "Approved", "Rejected", or "Cancelled"
2. WHEN a Student views their applications, THE Room_Application_System SHALL display the current status, submission date, preferred room, and review information if available
3. WHEN a Room_Application status is "Rejected", THE Room_Application_System SHALL display the RejectReason to the Student
4. WHEN a Room_Application status is "Approved", THE Room_Application_System SHALL display the AssignedRoomNumber and AssignedBuildingName to the Student
5. WHEN a Room_Application status is "Approved" and awaiting student acceptance, THE Room_Application_System SHALL display an "Accept" action button
6. WHEN a Student cancels a pending application, THE Room_Application_System SHALL update the status to "Cancelled", record CancelledAt timestamp, store the CancelledReason, and set CancelledBySelf to true
7. THE Room_Application_System SHALL display status change history with timestamps and responsible user names

### Requirement 8: Building Assignment Management

**User Story:** As an Admin, I want to assign Staff members to specific buildings, so that I can distribute workload and maintain clear responsibility boundaries.

#### Acceptance Criteria

1. THE Authorization_System SHALL allow Admin users to create and modify Building_Assignment records associating Staff members with one or more buildings
2. WHEN an Admin assigns a building to a Staff member, THE Authorization_System SHALL record the Staff UserId and BuildingId
3. WHEN an Admin removes a building assignment, THE Authorization_System SHALL revoke the Staff member's approval permissions for that building
4. THE Authorization_System SHALL allow a single Staff member to be assigned to multiple buildings
5. WHEN a Staff member logs in, THE Authorization_System SHALL load and cache their Building_Assignment for permission checks
6. THE Authorization_System SHALL provide an interface for Admin users to view and manage all Staff building assignments
7. IF an Admin attempts to assign a non-Staff user to a building, THEN THE Authorization_System SHALL reject the assignment and display an error message

### Requirement 9: Application Data Integrity

**User Story:** As a system administrator, I want application and contract data to remain consistent and auditable, so that I can maintain accurate records and resolve disputes.

#### Acceptance Criteria

1. THE Room_Application_System SHALL store snapshot data (PreferredBuildingName, PreferredRoomTypeName, PreferredRoomPrice, AssignedRoomNumber, AssignedBuildingName, ReviewedByName) that remains unchanged even if referenced master data changes
2. THE Contract_System SHALL store snapshot data (RoomNumber, BuildingName, RoomTypeName, MonthlyRent) that remains unchanged even if referenced master data changes
3. WHEN a Room_Application is created, THE Room_Application_System SHALL validate that the StudentId references an existing Student record
4. WHEN a Contract is created, THE Contract_System SHALL validate that the ApplicationId references an existing Room_Application record
5. THE Room_Application_System SHALL enforce that a Student cannot have multiple Room_Applications with status "Pending" or "Approved" simultaneously
6. THE Contract_System SHALL enforce that a Student cannot have multiple Contracts with status "Active" simultaneously
7. WHEN a Contract is created from a Room_Application, THE Room_Application_System SHALL establish a bidirectional reference between the two entities

### Requirement 10: Priority and Local Student Handling

**User Story:** As an Admin or authorized Staff member, I want to prioritize applications based on student type and other factors, so that I can allocate rooms fairly according to university policies.

#### Acceptance Criteria

1. WHEN a Student registers as a local student (IsLocalStudent is true), THE Room_Application_System SHALL set the Priority value to 0
2. WHEN a Student registers as a non-local student (IsLocalStudent is false), THE Room_Application_System SHALL set the Priority value to 1
3. WHEN displaying Room_Applications to authorized users, THE Room_Application_System SHALL provide sorting options including Priority in ascending order
4. THE Room_Application_System SHALL allow authorized users to manually adjust the Priority value
5. WHEN sorting applications by Priority, THE Room_Application_System SHALL display applications with higher priority values first (non-local students before local students)
6. THE Room_Application_System SHALL store the IsLocalStudent boolean value from the student registration form
