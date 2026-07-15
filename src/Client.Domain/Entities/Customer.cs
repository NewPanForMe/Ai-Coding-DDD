using Client.Domain.Abstractions;
using Client.Domain.ValueObjects;

namespace Client.Domain.Entities;

/// <summary>
/// 客户聚合根，代表系统中的注册客户。
/// </summary>
public sealed class Customer : IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// 初始化客户。
    /// </summary>
    /// <param name="name">客户姓名。</param>
    /// <param name="email">客户邮箱。</param>
    /// <param name="phone">客户电话。</param>
    /// <param name="address">客户地址。</param>
    /// <exception cref="ArgumentException">姓名或邮箱为空时抛出。</exception>
    public Customer(string name, string email, string phone, Address address)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("客户姓名不能为空。", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("客户邮箱不能为空。", nameof(email));
        }

        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 私有构造函数，供 EF Core 使用。
    /// </summary>
    private Customer()
    {
        Name = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Address = new Address("", "", "", "", "", "");
    }

    /// <summary>
    /// 获取客户唯一标识。
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// 获取客户姓名。
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 获取客户邮箱。
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// 获取客户电话。
    /// </summary>
    public string Phone { get; private set; }

    /// <summary>
    /// 获取客户地址。
    /// </summary>
    public Address Address { get; private set; }

    /// <summary>
    /// 获取客户注册时间。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 获取领域事件集合。
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// 更新客户联系方式。
    /// </summary>
    /// <param name="email">新邮箱。</param>
    /// <param name="phone">新电话。</param>
    public void UpdateContact(string email, string phone)
    {
        Email = string.IsNullOrWhiteSpace(email) ? Email : email;
        Phone = string.IsNullOrWhiteSpace(phone) ? Phone : phone;
    }

    /// <summary>
    /// 更新客户地址。
    /// </summary>
    /// <param name="address">新地址。</param>
    public void UpdateAddress(Address address)
    {
        Address = address;
    }

    /// <summary>
    /// 清除所有领域事件。
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// 添加领域事件。
    /// </summary>
    /// <param name="domainEvent">要添加的领域事件。</param>
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
