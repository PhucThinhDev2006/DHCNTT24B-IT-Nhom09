namespace AgriOCOP.Domain.Exceptions;
/// <summary>
/// Ngoại lệ ném ra khi cố ý thu hoạch mùa vụ nhưng chưa hết thời gian cách ly thuốc BVTV (Quy tắc nghiệp vụ QT01 - PHI).
/// </summary>
public class PreHarvestIntervalViolationException : DomainException
{
    // Constructor 1: Cho phép truyền thông điệp lỗi tự do
    public PreHarvestIntervalViolationException(string message) : base(message) { }
    // Constructor 2: Tự động ghép chuỗi thông báo chi tiết khi vi phạm ngày thu hoạch
    public PreHarvestIntervalViolationException(DateTime harvestDate, 
                                                DateTime safeHarvestDate, 
                                                int daysReamaining) 
        : base($"Vi phạm thời gian cách ly thuốc BVTV! Ngày thu hoạch dự kiến ({harvestDate: dd/MM/yyyy}) sớm hơn ngày an toàn ({safeHarvestDate: dd/MM/yyyy}). Cần cách ly thêm {daysReamaining} ngày để đảm bảo an toàn thực phẩm") { }
}