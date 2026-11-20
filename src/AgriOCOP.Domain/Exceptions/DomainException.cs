namespace AgriOCOP.Domain.Exceptions;
/// <summary>
/// Ngoại lệ cơ sở cho tất cả vi phạm nghiệp vụ trong tầng Domain.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
        /// <summary>
        /// Lệnh : base(message) có nghĩa là: 
        /// "Tôi nhận câu thông báo này từ lớp con, và tôi bàn giao lại cho lớp cha Exception lưu giữ hộ". 
        /// Nhờ đó, sau này tầng Web API chỉ cần gọi ex.Message là lấy ra được câu thông báo này để hiển thị cho người dùng.
        /// </summary>
    }
}