using Client.Application.DTOs;
using Client.Application.Interfaces;
using Client.Domain.Entities;
using Client.Domain.ValueObjects;
using Client.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Client.Application.Services;

/// <summary>
/// 客户应用服务，负责客户 CRUD 操作的编排。
/// </summary>
public sealed class CustomerAppService : ICustomerAppService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomerAppService> _logger;

    /// <summary>
    /// 初始化客户应用服务。
    /// </summary>
    /// <param name="customerRepository">客户仓储。</param>
    /// <param name="logger">日志记录器。</param>
    public CustomerAppService(ICustomerRepository customerRepository, ILogger<CustomerAppService> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CustomerInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(ToInfo).ToList();
    }

    /// <inheritdoc />
    public async Task<CustomerInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : ToInfo(customer);
    }

    /// <inheritdoc />
    public async Task<CustomerInfo> CreateAsync(CustomerInfo dto, CancellationToken cancellationToken = default)
    {
        var address = new Address(
            dto.Country ?? "中国",
            dto.Province ?? "",
            dto.City ?? "",
            dto.District ?? "",
            dto.Detail ?? "",
            dto.PostalCode ?? "");

        var customer = new Customer(dto.Name, dto.Email, dto.Phone, address);
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("客户 {CustomerId} ({CustomerName}) 创建成功", customer.Id, customer.Name);
        return ToInfo(customer);
    }

    /// <inheritdoc />
    public async Task<CustomerInfo?> UpdateAsync(Guid id, CustomerInfo dto, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("客户更新失败: 客户 {CustomerId} 不存在", id);
            return null;
        }

        customer.UpdateContact(dto.Email, dto.Phone);

        if (dto.Country is not null)
        {
            var address = new Address(
                dto.Country, dto.Province ?? "", dto.City ?? "",
                dto.District ?? "", dto.Detail ?? "", dto.PostalCode ?? "");
            customer.UpdateAddress(address);
        }

        await _customerRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("客户 {CustomerId} ({CustomerName}) 更新成功", customer.Id, customer.Name);
        return ToInfo(customer);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("客户删除失败: 客户 {CustomerId} 不存在", id);
            return false;
        }

        _customerRepository.Remove(customer);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("客户 {CustomerId} ({CustomerName}) 删除成功", customer.Id, customer.Name);
        return true;
    }

    /// <summary>
    /// 将客户实体转换为 DTO。
    /// </summary>
    /// <param name="customer">客户实体。</param>
    /// <returns>客户信息 DTO。</returns>
    private static CustomerInfo ToInfo(Customer customer)
    {
        return new CustomerInfo
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Country = customer.Address.Country,
            Province = customer.Address.Province,
            City = customer.Address.City,
            District = customer.Address.District,
            Detail = customer.Address.Street,
            PostalCode = customer.Address.PostalCode
        };
    }
}
