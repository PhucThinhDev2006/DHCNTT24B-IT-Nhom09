namespace AgriOCOP.Domain.Entities;
using AgriOCOP.Domain.Common;
/// <summary>
/// Thực thể Nông hộ.
/// Đơn vị trực tiếp sản xuất nông sản trực thuộc Hợp tác xã.
/// </summary>
public class Farm : BaseEntity
{
    public Guid CooperativeId { get; private set; }
    public string FarmCode {get; private set;} = string.Empty;
    public string FarmerName {get; private set;} = string.Empty;
    public string LocationAddress {get; private set;} = string.Empty;
    public string PhoneNumber {get; private set;} = string.Empty;
    public double AcreageHectares {get; private set;}
    public bool IsActive {get; private set;} = true;
    private Farm() { }
    public static Farm Create(Guid cooperativeId, string farmCode, string farmerName, string locationAddress, string phoneNumber, double acreageHectares)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(farmCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(farmerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);
        if (acreageHectares <= 0) 
        {
            throw new ArgumentOutOfRangeException(nameof(acreageHectares), "Diện tích canh tác phải lớn hơn 0.");
        }
        if (cooperativeId == Guid.Empty)
        {
            throw new ArgumentException("Nông hộ bắt buộc phải trực thuộc 1 Hợp tác xã hợp lệ.", nameof(cooperativeId));
        }
        return new Farm
        {
            FarmCode = farmCode.Trim().ToUpperInvariant(),
            FarmerName = farmerName.Trim(),
            LocationAddress = locationAddress.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            AcreageHectares = acreageHectares,
            CooperativeId = cooperativeId,
            IsActive = true
        };
    }
    /// <summary>
    /// Cập nhật thông tin nông hộ và diện tích canh tác.
    /// </summary>
    public void Update(string farmerName, string locationAddress, string phoneNumber, double acreageHectares)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(farmerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);
        if (acreageHectares <= 0) 
        {
            throw new ArgumentOutOfRangeException(nameof(acreageHectares), "Diện tích canh tác phải lớn hơn 0.");
        }
        FarmerName = farmerName.Trim();
        LocationAddress = locationAddress.Trim();
        PhoneNumber = phoneNumber.Trim();
        AcreageHectares = acreageHectares;
        // Tự động cập nhật mốc thời gian sửa đổi (thừa hưởng từ BaseEntity)
        MarkAsUpdated();
    }
    /// <summary> Tạm ngưng hoạt động của HTX </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }
    /// <summary> Kích hoạt lại hoạt động của HTX </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }
}