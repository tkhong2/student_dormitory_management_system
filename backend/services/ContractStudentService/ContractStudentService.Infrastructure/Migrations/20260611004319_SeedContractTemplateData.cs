using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractStudentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedContractTemplateData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Default Contract Template
            migrationBuilder.Sql(@"
                DECLARE @TemplateId INT;

                -- Insert default template
                IF NOT EXISTS (SELECT 1 FROM ContractTemplates WHERE Code = 'STANDARD')
                BEGIN
                    INSERT INTO ContractTemplates (Name, Code, Description, IsDefault, IsActive, MinDurationMonths, MaxDurationMonths, IsDeleted, CreatedAt)
                    VALUES (N'Hợp đồng KTX tiêu chuẩn', 'STANDARD', N'Mẫu hợp đồng tiêu chuẩn áp dụng cho tất cả sinh viên', 1, 1, 3, 12, 0, GETUTCDATE());
                    
                    SET @TemplateId = SCOPE_IDENTITY();

                    -- Insert terms for standard template
                    INSERT INTO ContractTerms (ContractTemplateId, Title, Content, OrderIndex, IsRequired, IsHighlighted, Icon, IsDeleted, CreatedAt)
                    VALUES
                        (@TemplateId, N'Thanh toán tiền phòng trước ngày 5 hàng tháng', N'Sinh viên có trách nhiệm thanh toán tiền phòng và các dịch vụ trước ngày 5 hàng tháng. Trường hợp chậm trễ sẽ bị phạt theo quy định.', 1, 1, 1, 'mdi-numeric-1-circle', 0, GETUTCDATE()),
                        (@TemplateId, N'Giữ gìn tài sản chung, bồi thường nếu hư hỏng', N'Sinh viên phải giữ gìn tài sản chung của KTX. Trường hợp làm hỏng hoặc mất mát phải bồi thường theo giá trị thực tế.', 2, 1, 1, 'mdi-numeric-2-circle', 0, GETUTCDATE()),
                        (@TemplateId, N'Chấm dứt hợp đồng phải báo trước 30 ngày', N'Khi muốn chấm dứt hợp đồng trước thời hạn, sinh viên phải thông báo bằng văn bản cho Ban Quản lý KTX trước ít nhất 30 ngày.', 3, 1, 1, 'mdi-numeric-3-circle', 0, GETUTCDATE()),
                        (@TemplateId, N'Tuân thủ nội quy KTX do Ban Quản lý ban hành', N'Sinh viên có trách nhiệm tuân thủ nghiêm chỉnh nội quy KTX, giờ giấc ra vào, quy định về vệ sinh, an toàn và các quy định khác.', 4, 1, 1, 'mdi-numeric-4-circle', 0, GETUTCDATE()),
                        (@TemplateId, N'Không được tự ý thay đổi kết cấu phòng', N'Nghiêm cấm tự ý sửa chữa, thay đổi kết cấu phòng, hệ thống điện nước mà không có sự đồng ý của Ban Quản lý.', 5, 1, 0, 'mdi-numeric-5-circle', 0, GETUTCDATE()),
                        (@TemplateId, N'Cấm để người ngoài lưu trú trái phép', N'Nghiêm cấm để người ngoài lưu trú, qua đêm tại phòng ký túc xá. Vi phạm sẽ bị xử lý nghiêm theo nội quy.', 6, 1, 0, 'mdi-numeric-6-circle', 0, GETUTCDATE());
                END

                -- Insert VIP template
                IF NOT EXISTS (SELECT 1 FROM ContractTemplates WHERE Code = 'VIP')
                BEGIN
                    INSERT INTO ContractTemplates (Name, Code, Description, IsDefault, IsActive, MinDurationMonths, MaxDurationMonths, IsDeleted, CreatedAt)
                    VALUES (N'Hợp đồng phòng VIP', 'VIP', N'Mẫu hợp đồng dành cho phòng VIP có tiện nghi cao cấp', 0, 1, 6, 12, 0, GETUTCDATE());
                    
                    SET @TemplateId = SCOPE_IDENTITY();

                    -- Insert terms for VIP template
                    INSERT INTO ContractTerms (ContractTemplateId, Title, Content, OrderIndex, IsRequired, IsHighlighted, Icon, IsDeleted, CreatedAt)
                    VALUES
                        (@TemplateId, N'Thanh toán tiền phòng trước ngày 5 hàng tháng', N'Thanh toán đầy đủ tiền phòng VIP và các dịch vụ cao cấp trước ngày 5 hàng tháng. Được ưu tiên thanh toán online.', 1, 1, 1, 'mdi-numeric-1-circle', 0, GETUTCDATE()),
                        (@TemplateId, N'Bảo quản trang thiết bị cao cấp', N'Phòng VIP có nhiều trang thiết bị cao cấp. Sinh viên phải bảo quản cẩn thận, bồi thường theo giá trị thực tế nếu làm hư hỏng.', 2, 1, 1, 'mdi-numeric-2-circle', 0, GETUTCDATE()),
                        (@TemplateId, N'Chấm dứt hợp đồng phải báo trước 60 ngày', N'Do phòng VIP có thời gian chờ dài, khi chấm dứt hợp đồng phải báo trước 60 ngày và có thể mất một phần tiền cọc.', 3, 1, 1, 'mdi-numeric-3-circle', 0, GETUTCDATE()),
                        (@TemplateId, N'Tuân thủ nội quy và quy định phòng VIP', N'Ngoài nội quy chung, phòng VIP có thêm các quy định về sử dụng tiện nghi, giữ vệ sinh, và tiêu chuẩn cao hơn.', 4, 1, 1, 'mdi-numeric-4-circle', 0, GETUTCDATE());
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
