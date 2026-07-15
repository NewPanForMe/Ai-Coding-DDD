<script setup lang="ts" generic="T extends Record<string, unknown>">
defineProps<{
  columns: { key: string; label: string; width?: string }[]
  items: T[]
  loading?: boolean
}>()

defineEmits<{
  (e: 'row-click', item: T): void
}>()
</script>

<template>
  <div class="data-table-wrapper">
    <div v-if="loading" class="table-loading">加载中...</div>
    <table v-else class="data-table">
      <thead>
        <tr>
          <th v-for="col in columns" :key="col.key" :style="{ width: col.width }">
            {{ col.label }}
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="items.length === 0">
          <td :colspan="columns.length" class="empty-row">暂无数据</td>
        </tr>
        <tr
          v-for="(item, idx) in items"
          :key="idx"
          class="data-row"
          @click="$emit('row-click', item)"
        >
          <td v-for="col in columns" :key="col.key">
            <slot :name="'cell-' + col.key" :item="item" :value="item[col.key]">
              {{ item[col.key] }}
            </slot>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.data-table-wrapper {
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  overflow: hidden;
}

.table-loading {
  padding: 40px;
  text-align: center;
  color: #6c757d;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th {
  background: #f8f9fa;
  padding: 12px 16px;
  text-align: left;
  font-weight: 600;
  font-size: 14px;
  color: #495057;
  border-bottom: 2px solid #dee2e6;
}

.data-table td {
  padding: 12px 16px;
  border-bottom: 1px solid #e9ecef;
  font-size: 14px;
  color: #212529;
}

.data-row {
  cursor: pointer;
  transition: background-color 0.15s;
}

.data-row:hover {
  background-color: #f0f4ff;
}

.empty-row {
  text-align: center;
  color: #adb5bd;
  padding: 40px 16px;
}
</style>
