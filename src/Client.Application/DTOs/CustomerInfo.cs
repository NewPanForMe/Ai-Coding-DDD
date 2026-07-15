namespace Client.Application.DTOs;

/// <summary>
/// 客户信息 DTO。
/// </summary>
public sealed class CustomerInfo
{
    /// <summary>
    /// 获取或设置客户 ID。
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 获取或设置客户姓名。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置客户邮箱。
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置客户电话。
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置国家。
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// 获取或设置省份。
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// 获取或设置城市。
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 获取或设置区/县。
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// 获取或设置详细地址。
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// 获取或设置邮政编码。
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// 获取或设置注册时间。
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
