<template>
  <div id="app">
    <NavBar />
    <main class="main-content">
      <RouterView />
    </main>
  </div>
</template>

<script setup>
import NavBar from './components/NavBar.vue'
import { RouterView } from 'vue-router'
</script>

<style scoped>
.main-content {
  padding: 2rem 0;
  min-height: calc(100vh - 64px);
}
</style>
