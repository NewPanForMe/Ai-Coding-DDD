/**
 * 订单项信息。
 */
export interface OrderItemInfo {
  /** 订单项 ID */
  id: string
  /** 产品 ID */
  productId: string
  /** 产品名称 */
  productName: string
  /** SKU */
  sku: string
  /** 单价 */
  unitPrice: number
  /** 货币代码 */
  currency: string
  /** 数量 */
  quantity: number
  /** 小计 */
  subTotal: number
}

/**
 * 订单信息。
 */
export interface OrderInfo {
  /** 订单 ID */
  id: string
  /** 客户 ID */
  customerId: string
  /** 客户姓名 */
  customerName: string
  /** 总金额 */
  totalAmount: number
  /** 货币代码 */
  currency: string
  /** 订单状态 */
  status: string
  /** 创建时间 */
  createdAt: string
  /** 订单项列表 */
  orderItems: OrderItemInfo[]
}

/**
 * 创建订单请求。
 */
export interface CreateOrderRequest {
  customerId: string
  orderItems: CreateOrderItemRequest[]
}

/**
 * 订单项创建请求。
 */
export interface CreateOrderItemRequest {
  productId: string
  quantity: number
}
