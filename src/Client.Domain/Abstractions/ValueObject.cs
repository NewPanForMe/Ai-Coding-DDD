namespace Client.Domain.Abstractions;

/// <summary>
/// 值对象基类，提供基于属性值的相等性比较。
/// 值对象没有唯一标识，通过所有属性的值来判断相等性。
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// 重写相等比较，基于业务属性值判断两个值对象是否相等。
    /// </summary>
    /// <param name="obj">要比较的对象。</param>
    /// <returns>类型相同且所有业务属性值均相等时返回 true。</returns>
    public override bool Equals(object? obj) =>
        obj is not null
        && obj.GetType() == GetType()
        && GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());

    /// <summary>
    /// 获取哈希码，基于所有业务属性值计算。
    /// </summary>
    /// <returns>基于业务属性值组合的哈希码。</returns>
    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);

    /// <summary>
    /// 相等操作符。
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

    /// <summary>
    /// 不等操作符。
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);

    /// <summary>
    /// 获取用于相等性比较的业务属性值集合。
    /// 子类必须重写此方法，返回所有参与相等比较的属性值。
    /// </summary>
    /// <returns>参与相等比较的属性值枚举。</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();
}
