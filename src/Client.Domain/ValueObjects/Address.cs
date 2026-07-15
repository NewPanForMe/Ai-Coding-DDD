using Client.Domain.Abstractions;

namespace Client.Domain.ValueObjects;

/// <summary>
/// 地址值对象，表示客户或订单的收货地址。
/// </summary>
public sealed class Address : ValueObject
{
    /// <summary>
    /// 初始化地址。
    /// </summary>
    /// <param name="country">国家。</param>
    /// <param name="province">省份/州。</param>
    /// <param name="city">城市。</param>
    /// <param name="district">区/县。</param>
    /// <param name="street">街道/详细地址。</param>
    /// <param name="postalCode">邮政编码。</param>
    /// <exception cref="ArgumentException">国家或城市为空时抛出。</exception>
    public Address(
        string country,
        string province,
        string city,
        string district,
        string street,
        string postalCode)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("国家不能为空。", nameof(country));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("城市不能为空。", nameof(city));
        }

        Country = country;
        Province = province;
        City = city;
        District = district;
        Street = street;
        PostalCode = postalCode;
    }

    /// <summary>
    /// 私有构造函数，供 EF Core 使用。
    /// </summary>
    private Address()
    {
        Country = string.Empty;
        Province = string.Empty;
        City = string.Empty;
        District = string.Empty;
        Street = string.Empty;
        PostalCode = string.Empty;
    }

    /// <summary>
    /// 获取国家。
    /// </summary>
    public string Country { get; private set; }

    /// <summary>
    /// 获取省份/州。
    /// </summary>
    public string Province { get; private set; }

    /// <summary>
    /// 获取城市。
    /// </summary>
    public string City { get; private set; }

    /// <summary>
    /// 获取区/县。
    /// </summary>
    public string District { get; private set; }

    /// <summary>
    /// 获取街道/详细地址。
    /// </summary>
    public string Street { get; private set; }

    /// <summary>
    /// 获取邮政编码。
    /// </summary>
    public string PostalCode { get; private set; }

    /// <inheritdoc />
    public override string ToString() => $"{Country} {Province} {City} {District} {Street}";

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return Province;
        yield return City;
        yield return District;
        yield return Street;
        yield return PostalCode;
    }
}
