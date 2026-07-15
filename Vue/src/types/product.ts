/**
 * 产品信息。
 */
export interface ProductInfo {
  /** 产品 ID */
  id: string
  /** 产品名称 */
  name: string
  /** 产品描述 */
  description: string
  /** SKU 编码 */
  sku: string
  /** 单价 */
  price: number
  /** 货币代码 */
  currency: string
  /** 库存数量 */
  stockQuantity: number
  /** 创建时间 */
  createdAt: string
}

/**
 * 创建产品请求。
 */
export interface CreateProductRequest {
  name: string
  description: string
  sku: string
  price: number
  stockQuantity: number
}

/**
 * 更新产品请求。
 */
export interface UpdateProductRequest {
  name: string
  description: string
  price: number
}
