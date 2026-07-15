<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import PageHeader from '../components/PageHeader.vue'
import { productApi } from '../api/productApi'
import type { ProductInfo } from '../types/product'

const route = useRoute()
const router = useRouter()

const productId = route.params.id as string
const product = ref<ProductInfo | null>(null)
const loading = ref(true)
const error = ref('')

async function fetchProduct() {
  loading.value = true
  error.value = ''
  try {
    product.value = await productApi.getById(productId)
  } catch {
    error.value = '产品不存在或加载失败'
  } finally {
    loading.value = false
  }
}

function goBack() {
  router.push('/products')
}

function formatPrice(price: number, currency: string): string {
  return `${currency} ${price.toFixed(2)}`
}

onMounted(() => fetchProduct())
</script>

<template>
  <div class="product-detail-page">
    <button class="back-btn" @click="goBack">← 返回列表</button>

    <div v-if="loading" class="loading-state">加载中...</div>

    <div v-else-if="error" class="error-state">{{ error }}</div>

    <div v-else-if="product" class="detail-content">
      <PageHeader :title="product.name" :subtitle="'SKU: ' + product.sku" />

      <div class="detail-card">
        <div class="detail-row">
          <span class="detail-label">产品 ID</span>
          <span class="detail-value">{{ product.id }}</span>
        </div>
        <div class="detail-row">
          <span class="detail-label">描述</span>
          <span class="detail-value">{{ product.description || '暂无描述' }}</span>
        </div>
        <div class="detail-row">
          <span class="detail-label">单价</span>
          <span class="detail-value price">{{ formatPrice(product.price, product.currency) }}</span>
        </div>
        <div class="detail-row">
          <span class="detail-label">库存</span>
          <span class="detail-value" :class="{ 'low-stock': product.stockQuantity < 10 }">
            {{ product.stockQuantity }} 件
          </span>
        </div>
        <div class="detail-row">
          <span class="detail-label">创建时间</span>
          <span class="detail-value">{{ new Date(product.createdAt).toLocaleString('zh-CN') }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.product-detail-page {
  max-width: 720px;
  margin: 0 auto;
}

.back-btn {
  background: none;
  border: none;
  color: #4a90d9;
  cursor: pointer;
  font-size: 14px;
  padding: 0;
  margin-bottom: 16px;
}

.back-btn:hover {
  text-decoration: underline;
}

.loading-state,
.error-state {
  text-align: center;
  padding: 40px;
  font-size: 16px;
  color: #6c757d;
}

.error-state {
  color: #dc3545;
}

.detail-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 24px;
}

.detail-row {
  display: flex;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
}

.detail-row:last-child {
  border-bottom: none;
}

.detail-label {
  width: 100px;
  font-weight: 600;
  color: #495057;
  font-size: 14px;
  flex-shrink: 0;
}

.detail-value {
  color: #212529;
  font-size: 14px;
}

.price {
  color: #e74c3c;
  font-weight: 600;
}

.low-stock {
  color: #e67e22;
  font-weight: 600;
}
</style>
