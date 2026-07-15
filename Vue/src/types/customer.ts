/**
 * 客户信息。
 */
export interface CustomerInfo {
  /** 客户 ID */
  id: string
  /** 客户姓名 */
  name: string
  /** 邮箱 */
  email: string
  /** 电话 */
  phone: string
  /** 地址 */
  address: string
  /** 注册时间 */
  createdAt: string
}

/**
 * 创建客户请求。
 */
export interface CreateCustomerRequest {
  name: string
  email: string
  phone: string
  country: string
  province: string
  city: string
  district: string
  street: string
  postalCode: string
}
