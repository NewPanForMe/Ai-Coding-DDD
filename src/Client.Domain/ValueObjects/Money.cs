using Client.Domain.Abstractions;

namespace Client.Domain.ValueObjects;

/// <summary>
/// 货币金额值对象，包含金额和货币代码。
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>
    /// 初始化货币金额。
    /// </summary>
    /// <param name="amount">金额数值。</param>
    /// <param name="currency">货币代码（如 CNY、USD），默认为 CNY。</param>
    /// <exception cref="ArgumentException">金额为负数时抛出。</exception>
    public Money(decimal amount, string currency = "CNY")
    {
        if (amount < 0)
        {
            throw new ArgumentException("金额不能为负数。", nameof(amount));
        }

        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// 私有构造函数，供 EF Core 使用。
    /// </summary>
    private Money()
    {
        Amount = 0;
        Currency = "CNY";
    }

    /// <summary>
    /// 获取金额数值。
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// 获取货币代码。
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// 加法运算。
    /// </summary>
    /// <param name="other">另一个金额。</param>
    /// <returns>相加后的新金额。</returns>
    /// <exception cref="InvalidOperationException">货币代码不一致时抛出。</exception>
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// 减法运算。
    /// </summary>
    /// <param name="other">另一个金额。</param>
    /// <returns>相减后的新金额。</returns>
    /// <exception cref="InvalidOperationException">货币代码不一致时抛出。</exception>
    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    /// <summary>
    /// 乘以系数。
    /// </summary>
    /// <param name="multiplier">乘数。</param>
    /// <returns>乘法运算后的新金额。</returns>
    public Money Multiply(decimal multiplier) => new(Amount * multiplier, Currency);

    /// <inheritdoc />
    public override string ToString() => $"{Currency} {Amount:F2}";

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException($"货币代码不一致: {Currency} vs {other.Currency}");
        }
    }
}
