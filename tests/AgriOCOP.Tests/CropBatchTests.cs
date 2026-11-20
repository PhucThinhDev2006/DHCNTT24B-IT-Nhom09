namespace AgriOCOP.Tests;
using Xunit;
using AgriOCOP.Domain.Entities;
using AgriOCOP.Domain.Enums;
using AgriOCOP.Domain.Exceptions;
/// <summary>
/// Bộ kiểm thử tự động cho Lô mùa vụ và Quy tắc an toàn cách ly thuốc BVTV (QT01 - PHI).
/// Tuân thủ chuẩn AAA.
/// </summary>
public class CropBatchTests
{
    [Fact]
    public void Harvest_BeforeSafePhiDate_ThrowsPreHarvestIntervalViolationException()
    {
        // 1. Arrange: Chuẩn bị lô Xoài Cát Chu, phun thuốc ngày 01/11, yêu cầu cách ly 14 ngày (ngày an toàn: 15/11)
        var farmID = Guid.NewGuid();
        var plantingDate = new DateTime(2026, 9, 1);
        var cropBatch = CropBatch.Create("LO_XOAI_001", "Xoài Cát Chu", farmID, plantingDate);
        var sprayDate = new DateTime(2026, 11, 1);
        int requiredPhiDays = 14;
        cropBatch.RecordPesticideSpraying(sprayDate, requiredPhiDays);
        // Nông dân cố tình thu hoạch sớm vào ngày 10/11 (chỉ mới cách ly được 9 ngày)
        var earlyHarvestDate = new DateTime(2026, 11, 10);
        double yieldKg = 500.0;
        // 2 & 3. Act & Assert: Khẳng định hệ thống PHẢI chặn đứng và ném lỗi vi phạm cách ly PHI
        var ex = Assert.Throws<PreHarvestIntervalViolationException>(() =>
        {
            cropBatch.Harvest(earlyHarvestDate, yieldKg);
        });
        // Kiểm tra thông điệp lỗi có chứa đúng cảnh báo an toàn thực phẩm hay không
        Assert.Contains("Vi phạm thời gian cách ly thuốc BVTV", ex.Message);
    }
    [Fact]
    public void Harvest_AfterSafePhiDate_SucceedsAndSetsHarvestedStatus()
    {
        // 1. Arrange: Chuẩn bị mùa vụ phun thuốc ngày 01/11, cách ly 14 ngày (ngày an toàn: 15/11)
        var farmID = Guid.NewGuid();
        var plantingDate = new DateTime(2026, 9, 1);
        var cropBatch = CropBatch.Create("LO_XOAI_002", "Xoài Cát Chu", farmID, plantingDate);
        var sprayDate = new DateTime(2026, 11, 1);
        int requiredPhiDays = 14;
        cropBatch.RecordPesticideSpraying(sprayDate, requiredPhiDays);
        // Thu hoạch hợp lệ vào ngày 16/11 (đã cách ly đủ 15 ngày, quá mốc 14 ngày an toàn)
        var safeHarvestDate = new DateTime(2026, 11, 16);
        double yieldKg = 750.5;
        // 2. Act: Thực hiện thu hoạch
        cropBatch.Harvest(safeHarvestDate, yieldKg);
         // 3. Assert: Kiểm tra trạng thái đã chuyển sang Harvested và cập nhật đúng sản lượng
        Assert.Equal(CropBatchStatus.Harvested, cropBatch.Status);
        Assert.Equal(safeHarvestDate, cropBatch.Harvested);
        Assert.Equal(yieldKg, cropBatch.YieldKg);
    }
    [Fact]
    public void Create_WithEmptyBatchCode_ThrowsArgumentException()
    {
        // 1. Arrange: Mã lô bị rỗng
        var farmID = Guid.NewGuid();
        var plantingDate = new DateTime(2026, 9, 1);
        // 2 & 3. Act & Assert: Khẳng định hệ thống ném ArgumentException ngay tại cửa ngõ Factory
        Assert.Throws<ArgumentException>(() =>
        {
            CropBatch.Create("", "Lúa ST25", farmID, plantingDate);
        });
    }
}