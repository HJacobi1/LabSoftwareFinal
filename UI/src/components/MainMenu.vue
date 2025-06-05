<template>
  <div class="main-menu">
    <div class="menu-header">
      <img src="../assets/lab-96.png" alt="Lab Icon" class="lab-icon" />
      <h1>LabManager</h1>
    </div>
    
    <nav class="menu-nav">
      <router-link to="/pessoas" class="menu-item">
        <i class="fas fa-users"></i>
        <span>Pessoas</span>
      </router-link>
      <router-link to="/equipamentos" class="menu-item">
        <i class="fas fa-microscope"></i>
        <span>Equipamentos</span>
      </router-link>      
    </nav>
  </div>
</template>

<script setup>
// Component logic can be added here if needed
</script>

<style scoped>
.main-menu {
  width: 250px;
  height: 100vh;
  background-color: #2c3e50;
  color: white;
  position: fixed;
  left: 0;
  top: 0;
  padding: 20px 0;
  box-shadow: 2px 0 5px rgba(0, 0, 0, 0.1);
}

.menu-header {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 0 20px;
  margin-bottom: 30px;
}

.lab-icon {
  width: 40px;
  height: 40px;
  object-fit: contain;
}

.menu-header h1 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
}

.menu-nav {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 20px;
  color: #ecf0f1;
  text-decoration: none;
  transition: background-color 0.3s;
}

.menu-item:hover {
  background-color: #34495e;
}

.menu-item.router-link-active {
  background-color: #3498db;
}

.menu-item i {
  width: 20px;
  text-align: center;
}

/* Add Font Awesome for icons */
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css');
</style> 