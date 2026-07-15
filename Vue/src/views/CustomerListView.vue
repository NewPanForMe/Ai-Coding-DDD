<script setup lang="ts">
import { ref, onMounted } from 'vue'
import PageHeader from '../components/PageHeader.vue'
import DataTable from '../components/DataTable.vue'
import { customerApi } from '../api/customerApi'
import type { CustomerInfo } from '../types/customer'
import type { PagedResult } from '../types/common'

const loading = ref(false)
const customers = ref<CustomerInfo[]>([])
const pagination = ref({ pageIndex: 1, pageSize: 10, totalCount: 0, totalPages: 0 })

const columns = [
  { key: 'name', label: '姓名', width: '100px' },
  { key: 'email', label: '邮箱', width: '220px' },
  { key: 'phone', label: '电话', width: '140px' },
  { key: 'address', label: '地址', width: '280px' },
  { key: 'createdAt', label: '注册时间', width: '160px' }
]

async function fetchCustomers(pageIndex = 1) {
  loading.value = true
  try {
    const result: PagedResult<CustomerInfo> = await customerApi.getList(pageIndex, 10)
    customers.value = result.items
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
  fetchCustomers(page)
}

function formatDate(value: unknown): string {
  if (typeof value !== 'string') return '-'
  return new Date(value).toLocaleDateString('zh-CN')
}

onMounted(() => fetchCustomers())
</script>

<template>
  <div class="customer-list-page">
    <PageHeader title="客户管理" subtitle="查看已注册客户信息" />

    <DataTable
      :columns="columns"
      :items="customers"
      :loading="loading"
    >
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
.customer-list-page {
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
</style>
