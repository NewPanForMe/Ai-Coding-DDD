<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <header>
    <nav>
      <RouterLink to="/">Home</RouterLink>
      <RouterLink to="/products">产品</RouterLink>
      <RouterLink to="/orders">订单</RouterLink>
      <RouterLink to="/customers">客户</RouterLink>
      <RouterLink to="/about">About</RouterLink>
    </nav>
  </header>
  <main>
    <RouterView />
  </main>
</template>

<style scoped>
nav {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  background: #f5f5f5;
  border-bottom: 1px solid #e0e0e0;
}
nav a {
  text-decoration: none;
  color: #333;
  font-weight: 500;
  font-size: 15px;
  padding: 4px 8px;
  border-radius: 4px;
  transition: background-color 0.2s;
}
nav a:hover {
  background-color: #e8e8e8;
}
nav a.router-link-exact-active {
  color: #42b883;
  background-color: #e8f5e9;
}
main {
  padding: 2rem;
}
</style>
