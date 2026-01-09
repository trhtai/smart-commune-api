using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using SmartCommune.Domain.MenuItemAggregate;
using SmartCommune.Domain.MenuItemAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems");

        builder.HasKey(sm => sm.Id);

        builder.Property(sm => sm.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => MenuItemId.Create(value));

        builder.Property(sm => sm.ParentId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value != null ? MenuItemId.Create(value.Value) : null);

        builder.Property(sm => sm.Label)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(sm => sm.SortOrder)
            .HasDefaultValue(0);

        // Cấu hình JSON COLUMN.
        // =====
        // Tại sao phải là Json?
        // Vì nếu không dùng Json ta sẽ phải thiết kế phần cấu hình này theo kiểu quan hệ,
        // sẽ phải có bảng CheckRoutes, RelatedPaths,...
        // Do bài toán Menu của hệ thống này không cần quản lý các bảng trên nên việc dùng Json là hợp lý.
        // =====
        // Tại sao không convert dữ liệu user nhập thành Json rồi lưu vào kiểu chuỗi trong db như bình thường?
        // - Tận dụng việc MySql sẽ validate kiểu dữ liệu lúc lưu, tránh việc lưu trữ dữ liệu sai format.
        // - Kiểu Json trong MySql được lưu dưới dạng nhị phân nên được tối ưu hóa, giúp đọc nhanh hơn text thuần túy.
        // - Khả năng truy vấn sâu:
        // + Nếu lưu Text bình thường, bạn sẽ phải dùng LIKE '%dashboard$' -> chậm và không chính xác.
        // + Nếu lưu Json, bạn có thể dùng các hàm JSON của MySql để truy vấn chính xác hơn như sau:
        // SELECT * FROM SystemMenus WHERE JSON_CONTAINS(Config, '"/dashboard"', '$.Path');
        // =====
        // 1. Định nghĩa Converter: Object <-> JSON String.
        var configConverter = new ValueConverter<MenuItemConfig, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), // Lưu: Object -> String.
            v => JsonSerializer.Deserialize<MenuItemConfig>(v, (JsonSerializerOptions?)null)!); // Đọc: String -> Object.

        // 2. Định nghĩa Comparer: Giúp EF Core biết khi nào JSON thay đổi để Update
        var configComparer = new ValueComparer<MenuItemConfig>(
            (c1, c2) => c1!.GetEqualityComponents().SequenceEqual(c2!.GetEqualityComponents()),
            c => c.GetHashCode(),
            c => JsonSerializer.Deserialize<MenuItemConfig>(
                JsonSerializer.Serialize(c, (JsonSerializerOptions?)null),
                (JsonSerializerOptions?)null)!);

        builder.Property(x => x.Config)
            .HasColumnType("json") // Bắt buộc: Khai báo kiểu cột trong MySQL là 'json'.
            .HasConversion(configConverter)
            .Metadata.SetValueComparer(configComparer);

        // Cấu hình quan hệ đệ quy.
        builder.HasOne<MenuItem>()
            .WithMany(sm => sm.Children)
            .HasForeignKey(sm => sm.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa cha có con.
    }
}