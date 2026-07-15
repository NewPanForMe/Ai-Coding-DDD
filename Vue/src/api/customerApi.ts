import apiClient from './client'
import type { CustomerInfo, CreateCustomerRequest } from '../types/customer'
import type { PagedResult } from '../types/common'

/**
 * 客户 API 封装。
 */
export const customerApi = {
  /**
   * 分页获取客户列表。
   */
  getList(pageIndex = 1, pageSize = 20): Promise<PagedResult<CustomerInfo>> {
    return apiClient
      .get('/Customers', { params: { pageIndex, pageSize } })
      .then((res) => res.data)
  },

  /**
   * 根据 ID 获取客户详情。
   */
  getById(id: string): Promise<CustomerInfo> {
    return apiClient.get(`/Customers/${id}`).then((res) => res.data)
  },

  /**
   * 注册新客户。
   */
  create(data: CreateCustomerRequest): Promise<CustomerInfo> {
    return apiClient.post('/Customers', data).then((res) => res.data)
  }
}
