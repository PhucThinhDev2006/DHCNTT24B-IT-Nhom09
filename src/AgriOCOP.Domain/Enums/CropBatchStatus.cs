namespace AgriOCOP.Domain.Enums;
/// <summary> 
/// Trạng thái vòng đời của 1 lô màu vụ (CropBatch) 
/// </summary>
public enum CropBatchStatus
{
    /// <sumary> Đang lên kế hoạch canh tác </sumary>
    Plamned = 1,
    /// <sumary> Đang trong giai đoạn chăm sóc, sinh trưởng </sumary>
    Growing = 2,
    /// <sumary> Đủ thời gian cách ly thuốc BVTV, sẵn sàng thu hoạch </sumary>
    SafeHarvest = 3,
    /// <sumary> Đã thu hoạch thành công </sumary>
    Harvested = 4,
    /// <sumary> Đã hủy do thiên tai dịch bệnh </sumary>
    Cancelled = 5
}