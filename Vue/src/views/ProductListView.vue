<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import PageHeader from '../components/PageHeader.vue'
import DataTable from '../components/DataTable.vue'
import { productApi } from '../api/productApi'
import type { ProductInfo } from '../types/product'
import type { PagedResult } from '../types/common'

const router = useRouter()

const loading = ref(false)
const products = ref<ProductInfo[]>([])
const pagination = ref({ pageIndex: 1, pageSize: 10, totalCount: 0, totalPages: 0 })

const columns = [
  { key: 'name', label: '产品名称', width: '220px' },
  { key: 'sku', label: 'SKU', width: '130px' },
  { key: 'price', label: '单价', width: '100px' },
  { key: 'stockQuantity', label: '库存', width: '80px' },
  { key: 'createdAt', label: '创建时间', width: '180px' }
]

async function fetchProducts(pageIndex = 1) {
  loading.value = true
  try {
    const result: PagedResult<ProductInfo> = await productApi.getList(pageIndex, 10)
    products.value = result.items
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

function goToDetail(product: ProductInfo) {
  router.push(`/products/${product.id}`)
}

function goToPage(page: number) {
  fetchProducts(page)
}

function formatPrice(value: unknown): string {
  const num = Number(value)
  return isNaN(num) ? '-' : `¥${num.toFixed(2)}`
}

function formatDate(value: unknown): string {
  if (typeof value !== 'string') return '-'
  return new Date(value).toLocaleDateString('zh-CN')
}

onMounted(() => fetchProducts())
</script>

<template>
  <div class="product-list-page">
    <PageHeader title="产品管理" subtitle="管理所有产品信息与库存" />

    <DataTable
      :columns="columns"
      :items="products"
      :loading="loading"
      @row-click="goToDetail"
    >
      <template #cell-price="{ value }">{{ formatPrice(value) }}</template>
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
.product-list-page {
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
