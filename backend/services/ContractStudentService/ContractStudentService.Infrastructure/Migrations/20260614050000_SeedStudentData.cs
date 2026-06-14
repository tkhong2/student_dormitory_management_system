using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractStudentService.Infrastructure.Migrations
{
    public partial class SeedStudentData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert student seed data if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Students WHERE StudentCode = 'SV001')
                BEGIN
                    SET IDENTITY_INSERT Students ON;
                    
                    INSERT INTO Students (
                        Id, StudentCode, Faculty, Major, AcademicYear, ClassCode, FullName, Gender,
                        DateOfBirth, PlaceOfBirth, Ethnicity, Religion, Nationality, Phone, Email,
                        IdentityCard, IdentityCardIssuedDate, IdentityCardIssuedPlace,
                        PermanentAddress, PermanentProvince, EmergencyContactName,
                        EmergencyContactPhone, EmergencyContactRelation, EmergencyContactAddress,
                        UserId, IsActive, ProfileCompletionPct, CreatedAt, UpdatedAt
                    )
                    VALUES (
                        1, 'SV001', N'Công nghệ thông tin', N'Công nghệ thông tin', 3, 'K65-CNTT',
                        N'Trần Thị Bình', 'Female', '2003-05-15', N'Hà Nội', N'Kinh', N'Không',
                        N'Việt Nam', '0903456789', 'student01@student.dormitory.com',
                        '001203012345', '2021-01-15', N'Hà Nội', 
                        N'123 Đường ABC, Quận 1, TP.HCM', N'TP. Hồ Chí Minh',
                        N'Trần Văn A', '0909123456', N'Bố', N'123 Đường ABC, Quận 1, TP.HCM',
                        3, 1, 100, GETUTCDATE(), GETUTCDATE()
                    );
                    
                    SET IDENTITY_INSERT Students OFF;
                END
            ");

            // Insert student document seed data if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM StudentDocuments WHERE StudentId = 1)
                BEGIN
                    INSERT INTO StudentDocuments (
                        StudentId, DocumentType, DocumentName, FileUrl, FileType, FileSizeBytes,
                        IsVerified, VerifiedByUserId, VerifiedAt, Notes, CreatedAt
                    )
                    VALUES
                    (
                        1, N'CMND/CCCD', N'Chứng minh nhân dân - Trần Thị Bình',
                        '/uploads/documents/cmnd-sv001.pdf', 'application/pdf', 512000,
                        1, 1, GETUTCDATE(), N'Đã xác minh', GETUTCDATE()
                    ),
                    (
                        1, N'Giấy xác nhận sinh viên', N'Giấy xác nhận SV - K65-CNTT',
                        '/uploads/documents/xacnhan-sv001.pdf', 'application/pdf', 256000,
                        0, NULL, NULL, N'Chờ xác minh', GETUTCDATE()
                    );
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seed data
            migrationBuilder.Sql("DELETE FROM StudentDocuments WHERE StudentId = 1");
            migrationBuilder.Sql("DELETE FROM Students WHERE StudentCode = 'SV001'");
        }
    }
}
