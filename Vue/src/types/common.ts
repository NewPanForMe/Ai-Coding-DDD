/**
 * 通用分页结果类型。
 */
export interface PagedResult<T> {
  /** 当前页数据项 */
  items: T[]
  /** 总记录数 */
  totalCount: number
  /** 当前页码 */
  pageIndex: number
  /** 每页数量 */
  pageSize: number
  /** 总页数 */
  totalPages: number
  /** 是否有上一页 */
  hasPreviousPage: boolean
  /** 是否有下一页 */
  hasNextPage: boolean
}
