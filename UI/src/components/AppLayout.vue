<template>
  <div class="app-layout">
    <MainMenu />
    <main class="content">
      <router-view></router-view>
    </main>
  </div>
</template>

<script setup>
import MainMenu from './MainMenu.vue'
</script>

<style scoped>
.app-layout {
  display: flex;
  min-height: 100vh;
}

.content {
  flex: 1;
  margin-left: 0px; /* Same as menu width */
  padding: 20px;
  background-color: #f5f5f5;
  width: 600px;
}
</style> 