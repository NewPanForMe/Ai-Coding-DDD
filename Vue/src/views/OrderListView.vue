<script setup lang="ts">
import { ref, onMounted } from 'vue'
import PageHeader from '../components/PageHeader.vue'
import DataTable from '../components/DataTable.vue'
import { orderApi } from '../api/orderApi'
import type { OrderInfo } from '../types/order'
import type { PagedResult } from '../types/common'

const loading = ref(false)
const orders = ref<OrderInfo[]>([])
const pagination = ref({ pageIndex: 1, pageSize: 10, totalCount: 0, totalPages: 0 })

const columns = [
  { key: 'id', label: '订单号', width: '280px' },
  { key: 'customerName', label: '客户', width: '100px' },
  { key: 'totalAmount', label: '金额', width: '100px' },
  { key: 'status', label: '状态', width: '80px' },
  { key: 'createdAt', label: '下单时间', width: '180px' }
]

async function fetchOrders(pageIndex = 1) {
  loading.value = true
  try {
    const result: PagedResult<OrderInfo> = await orderApi.getList(pageIndex, 10)
    orders.value = result.items
    pagination.value = {
      pageIndex: result.pageIndex,
      pageSize: result.pageSize,
      totalCount: result.totalCount,
      totalPages: result.totalPages
    }
  } finally {
    loading.value = false
  }
}

function goToPage(page: number) {
  fetchOrders(page)
}

function formatPrice(value: unknown): string {
  const num = Number(value)
  return isNaN(num) ? '-' : `¥${num.toFixed(2)}`
}

function formatDate(value: unknown): string {
  if (typeof value !== 'string') return '-'
  return new Date(value).toLocaleDateString('zh-CN')
}

const statusMap: Record<string, string> = {
  Pending: '待处理',
  Confirmed: '已确认',
  Shipped: '已发货',
  Completed: '已完成',
  Cancelled: '已取消'
}

onMounted(() => fetchOrders())
</script>

<template>
  <div class="order-list-page">
    <PageHeader title="订单管理" subtitle="查看和管理所有客户订单" />

    <DataTable
      :columns="columns"
      :items="orders"
      :loading="loading"
    >
      <template #cell-totalAmount="{ value }">{{ formatPrice(value) }}</template>
      <template #cell-status="{ value }">
        <span :class="['status-tag', String(value).toLowerCase()]">
          {{ statusMap[String(value)] || value }}
        </span>
      </template>
      <template #cell-createdAt="{ value }">{{ formatDate(value) }}</template>
    </DataTable>

    <div v-if="pagination.totalPages > 1" class="pagination">
      <button
        :disabled="pagination.pageIndex <= 1"
        @click="goToPage(pagination.pageIndex - 1)"
      >
        上一页
      </button>
      <span class="page-info">
        第 {{ pagination.pageIndex }} / {{ pagination.totalPages }} 页（共 {{ pagination.totalCount }} 条）
      </span>
      <button
        :disabled="pagination.pageIndex >= pagination.totalPages"
        @click="goToPage(pagination.pageIndex + 1)"
      >
        下一页
      </button>
    </div>
  </div>
</template>

<style scoped>
.order-list-page {
  max-width: 960px;
  margin: 0 auto;
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  margin-top: 20px;
}

.pagination button {
  padding: 6px 16px;
  border: 1px solid #dee2e6;
  border-radius: 6px;
  background: #fff;
  cursor: pointer;
  font-size: 14px;
}

.pagination button:disabled {
  color: #adb5bd;
  cursor: not-allowed;
}

.pagination button:hover:not(:disabled) {
  background: #f0f4ff;
  border-color: #4a90d9;
}

.page-info {
  font-size: 14px;
  color: #6c757d;
}

.status-tag {
  display: inline-block;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-tag.pending {
  background: #fff3cd;
  color: #856404;
}

.status-tag.confirmed {
  background: #cce5ff;
  color: #004085;
}

.status-tag.shipped {
  background: #d4edda;
  color: #155724;
}

.status-tag.completed {
  background: #d1ecf1;
  color: #0c5460;
}

.status-tag.cancelled {
  background: #f8d7da;
  color: #721c24;
}
</style>
