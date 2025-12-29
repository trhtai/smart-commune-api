# problem 1: ko nen "khon loi" hay "tiet kiem" khi cai dat thu vien trong CA
- vd: o domain da cai thu vien MediatR va nhan thay application tham chieu den domain 
=> ko can cai MediatR o application va api tham chieu den application 
=> ko can cai MediatR o api
- viec ""xai ke" nay lam cho du an khong tuong minh
- nho dau sau nay domain ko dung MediatR nua se gay ra loi cho api va application

# Vì ApplicationUser có các thuộc tính string không cho phép null (UserName, PasswordHash...), 
# trình biên dịch lo lắng rằng khi private ApplicationUser() được gọi, 
# các thuộc tính này sẽ bị null. Tuy nhiên, chúng ta biết rằng constructor này chỉ dùng cho EF Core (thông qua Reflection) 
# và EF Core sẽ map dữ liệu vào ngay sau đó.
- khởi tạo các giá trị này bằng null!. 
Dấu ! nói với trình biên dịch: 
"Tôi biết nó đang là null, nhưng hãy tin tôi, nó sẽ có dữ liệu hợp lệ khi sử dụng (do EF Core nạp vào)"

hoặc có thể sử dụng để tắt cảnh báo:

#pragma warning disable CS8618
private ApplicationUser() { }
#pragma warning restore CS8618

# theo clean code thì trong class thứ tự sẽ là: 
- fields -> constructors -> properties -> operators -> methods

- luôn xuống dòng trước từ khóa where

- bung {} ra nhiều dòng kể cả khi bên trong rỗng

- Generic type constraints (where ...): Khi class có nhiều tham số generic (ví dụ TId, TUser, TRole), dòng khai báo sẽ rất dài. StyleCop bắt buộc xuống dòng ngay từ đầu để tạo thói quen tốt, giúp mắt dễ quét code theo chiều dọc.

- Constructor initializers (: base(...)): Tương tự, nếu bạn gọi base() với 4-5 tham số, viết cùng 1 dòng sẽ bị tràn màn hình. Việc xuống dòng giúp tách biệt rõ ràng giữa "Tên hàm" và "Hàm cha được gọi".

- Element on single line ({ }): StyleCop ghét kiểu viết tắt { } vì nó dễ gây lỗi khi merge code hoặc debug (khó đặt breakpoint vào bên trong). Nó muốn mọi block code đều phải tường minh.