namespace AgriOCOP.Domain.Entities;
using AgriOCOP.Domain.Common;
using AgriOCOP.Domain.Enums;
using AgriOCOP.Domain.Exceptions;
/// <summary>
/// Thực thể Lô mùa vụ.
/// Quản lý vòng đời gieo trồng, phun thuốc BVTV và kiểm soát thu hoạch an toàn theo chuẩn OCOP.
/// </summary>
public class CropBatch : BaseEntity
{
    public Guid FarmID { get; private set; }
    public string BatchCode {get; private set;} = string.Empty;
    public string CropName {get; private set;} = string.Empty;
    public DateTime PlantingDate {get; private set;}
    public DateTime? LastPesticideApplicationDate {get; private set;}
    public int RequiredPhiDays {get; private set;}
    public CropBatchStatus Status {get; private set;} = CropBatchStatus.Planned;
    public DateTime? Harvested {get; private set;}
    public double? YieldKg {get; private set;}
    private CropBatch() { }
    /// <summary>
    /// Factory Method khởi tạo Lô mùa vụ mới.
    /// </summary>
    public static CropBatch Create(string batchCode, string cropName, Guid farmId, DateTime plantingDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(batchCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(cropName);
        if (farmId == Guid.Empty)
        {
            throw new ArgumentException("Lô mùa vụ bắt buộc phải trực thuộc 1 Nông hộ hợp lệ.", nameof(farmId));
        }
        return new CropBatch
        {
            BatchCode = batchCode.Trim().ToUpperInvariant(),
            CropName = cropName.Trim(),
            FarmID = farmId,
            PlantingDate = plantingDate,
            Status = CropBatchStatus.Growing
        };
    }
     /// <summary>
    /// Ghi nhận nhật ký phun thuốc bảo vệ thực vật và thiết lập số ngày cách ly bắt buộc (PHI).
    /// </summary>
    public void RecordPesticideSpraying(DateTime sprayDate, int requiredPhiDays)
    {
        if (Status == CropBatchStatus.Harvested)
        {
            throw new InvalidOperationException("Không thể ghi nhận phun thuốc cho mùa vụ đã thu hoạch hoặc đã bị hủy.");
        }
        if (requiredPhiDays < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requiredPhiDays), "Số ngày cách ly bắt buộc không thể là số âm.");
        }
        LastPesticideApplicationDate = sprayDate;
        RequiredPhiDays = requiredPhiDays;
        Status = CropBatchStatus.Growing;
        MarkAsUpdated();
    }
    /// <summary>
    /// Tính toán ngày thu hoạch an toàn dựa trên ngày phun thuốc cuối và số ngày cách ly (Quy tắc QT01).
    /// </summary>
    public DateTime CalculateSafeHarvestDate()
    {
        if (!LastPesticideApplicationDate.HasValue || RequiredPhiDays <= 0)
        {
            throw new InvalidOperationException("Không thể tính toán ngày thu hoạch an toàn khi chưa có thông tin phun thuốc hoặc số ngày cách ly chưa được thiết lập.");
        }
        return LastPesticideApplicationDate.Value.AddDays(RequiredPhiDays);
    }
    /// <summary>
    /// Thu hoạch mùa vụ và kiểm tra nghiêm ngặt quy tắc an toàn cách ly thuốc BVTV (QT01 - PHI).
    /// </summary>
    public void Harvest(DateTime harvestDate, double yieldKg)
    {
        if (Status == CropBatchStatus.Harvested)
        {
            throw new InvalidOperationException("Lô mùa vụ đã được thu hoạch trước đó.");
        }
        if (Status == CropBatchStatus.Cancelled)
        {
            throw new InvalidOperationException("Không thể thu hoạch lô mùa vụ đã bị hủy.");
        }
        if (harvestDate < PlantingDate)
        {
            throw new ArgumentException("Ngày thu hoạch không thể trước ngày gieo trồng.", nameof(harvestDate));
        }
        if (yieldKg <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(yieldKg), "Sản lượng thu hoạch phải lớn hơn 0 kg.");
        }
        // KIỂM TRA QUY TẮC AN TOÀN PHI (QT01)
        if (LastPesticideApplicationDate.HasValue && RequiredPhiDays > 0)
        {
            var safeHarvestDate = CalculateSafeHarvestDate();
            if (harvestDate < safeHarvestDate)
            {
            var daysRemaining = (safeHarvestDate.Date - harvestDate.Date).Days;
                throw new PreHarvestIntervalViolationException(harvestDate, safeHarvestDate, daysRemaining);
            }
        }
         // Đạt chuẩn an toàn -> Ghi nhận thu hoạch thành công
        Harvested = harvestDate;
        YieldKg = yieldKg;
        Status = CropBatchStatus.Harvested;
        MarkAsUpdated();
    }
     /// <summary> Hủy lô mùa vụ do thiên tai hoặc sâu bệnh </summary>
    public void Cancel()
    {
        if (Status == CropBatchStatus.Harvested)
        {
            throw new InvalidOperationException("Không thể hủy lô mùa vụ đã thu hoạch xong.");
        }
        Status = CropBatchStatus.Cancelled;
        MarkAsUpdated();
    }
}