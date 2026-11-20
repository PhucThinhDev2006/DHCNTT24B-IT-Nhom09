namespace AgriOCOP.Domain.Entities;
using AgriOCOP.Domain.Common;
/// <summary>
/// Thực thể Hợp tác xã Nông nghiệp.
/// Đơn vị quản lý các nông hộ thành viên và liên kết tiêu thụ sản phẩm OCOP.
/// </summary>
public class Cooperative : BaseEntity
{
    public string Code {get; private set;} = string.Empty;
    public string Name {get; private set;} = string.Empty;
    public string Address {get; private set;} = string.Empty;
    public string RepresentativeName {get; private set;} = string.Empty;
    public string PhoneNumber {get; private set;} = string.Empty;
    public bool IsActive {get; private set;} = true;
    /// <summary>
    /// Constructor rỗng bảo vệ để EF Core nạp dữ liệu  
    /// </summary>
    private Cooperative() { }
    /// <summary>
    /// Phương thức Factory để khởi tạo Hợp tác xã hợp lệ.
    /// </summary>
    public static Cooperative Create(string code, string name, string address, string representativeName, string phoneNumber)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(code);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(address);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(representativeName);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);
        return new Cooperative
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Address = address.Trim(),
            RepresentativeName = representativeName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            IsActive = true
        };
    }
    /// <summary>
    /// Cập nhật thông tin hành chính của Hợp tác xã.
    /// </summary>
    public void Update(string name, string address, string representativeName, string phoneNumber)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(address);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(representativeName);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);
        Name = name.Trim();
        Address = address.Trim();
        RepresentativeName = representativeName.Trim();
        PhoneNumber = phoneNumber.Trim();
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