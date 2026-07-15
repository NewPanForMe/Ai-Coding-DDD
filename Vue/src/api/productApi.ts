import apiClient from './client'
import type { ProductInfo, CreateProductRequest, UpdateProductRequest } from '../types/product'
import type { PagedResult } from '../types/common'

/**
 * 产品 API 封装。
 */
export const productApi = {
  /**
   * 分页获取产品列表。
   */
  getList(pageIndex = 1, pageSize = 20): Promise<PagedResult<ProductInfo>> {
    return apiClient
      .get('/Products', { params: { pageIndex, pageSize } })
      .then((res) => res.data)
  },

  /**
   * 根据 ID 获取产品详情。
   */
  getById(id: string): Promise<ProductInfo> {
    return apiClient.get(`/Products/${id}`).then((res) => res.data)
  },

  /**
   * 创建新产品。
   */
  create(data: CreateProductRequest): Promise<ProductInfo> {
    return apiClient.post('/Products', data).then((res) => res.data)
  },

  /**
   * 更新产品信息。
   */
  update(id: string, data: UpdateProductRequest): Promise<ProductInfo> {
    return apiClient.put(`/Products/${id}`, data).then((res) => res.data)
  },

  /**
   * 删除产品。
   */
  delete(id: string): Promise<void> {
    return apiClient.delete(`/Products/${id}`)
  }
}
