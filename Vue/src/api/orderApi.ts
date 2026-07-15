import apiClient from './client'
import type { OrderInfo, CreateOrderRequest } from '../types/order'
import type { PagedResult } from '../types/common'

/**
 * 订单 API 封装。
 */
export const orderApi = {
  /**
   * 分页获取订单列表。
   */
  getList(pageIndex = 1, pageSize = 20): Promise<PagedResult<OrderInfo>> {
    return apiClient
      .get('/Orders', { params: { pageIndex, pageSize } })
      .then((res) => res.data)
  },

  /**
   * 根据 ID 获取订单详情。
   */
  getById(id: string): Promise<OrderInfo> {
    return apiClient.get(`/Orders/${id}`).then((res) => res.data)
  },

  /**
   * 创建新订单。
   */
  create(data: CreateOrderRequest): Promise<OrderInfo> {
    return apiClient.post('/Orders', data).then((res) => res.data)
  },

  /**
   * 取消订单。
   */
  cancel(id: string): Promise<void> {
    return apiClient.put(`/Orders/${id}/cancel`)
  }
}
