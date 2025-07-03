<template>
  <div class="main-menu">
    <div class="menu-header">
      <img src="../assets/lab-96.png" alt="Lab Icon" class="lab-icon" />
      <h1>LabManager</h1>
    </div>    
    <nav class="menu-nav">
      <router-link v-if="isAdmin" to="/laboratorios" class="menu-item">
        <i class="fas fa-flask"></i>
        <span>Laboratórios</span>
      </router-link>
      <router-link v-if="isAdmin" to="/pessoas" class="menu-item">
        <i class="fas fa-users"></i>
        <span>Pessoas</span>
      </router-link>
      <router-link v-if="isAdmin" to="/equipamentos" class="menu-item">
        <i class="fas fa-microscope"></i>
        <span>Equipamentos</span>
      </router-link>
      <router-link to="/gerenciador-laboratorio" class="menu-item">
        <i class="fas fa-cogs"></i>
        <span>Gerenciador de Laboratório</span>
      </router-link>
      <router-link v-if="isAdmin" to="/usuarios">Usuários</router-link>
      <button @click="logout">Sair</button>
      <div class="user-info">
        <span v-if="userEmail">Usuário Logado: {{ userEmail }}</span>
      </div>
    </nav>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const logout = () => {
  localStorage.removeItem('token');
  router.push('/login');
};

function parseJwt(token) {
  if (!token) return null;
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
}

const userPayload = computed(() => {
  const token = localStorage.getItem('token');
  return parseJwt(token);
});

const isAdmin = computed(() => {
  return userPayload.value && (userPayload.value.isAdmin === true || userPayload.value.isAdmin === 'true');
});

const userEmail = computed(() => {  
  console.log(userPayload.value["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]);
  const email = userPayload.value["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
  return email;
});
</script>

<style scoped>
/* Add Font Awesome for icons */
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css');
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
  display: flex;
  flex-direction: column;
  /*justify-content: space-between;*/
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
  flex: 1;
}

.menu-nav .user-info {
  margin-top: auto;
}

.menu-nav button {
  margin-top: 20px;
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

.user-info {
  width: 100%;
  padding: 18px 20px 18px 20px;
  font-size: 1rem;
  color: #bdc3c7;
  background: none;
  text-align: left;
}
</style> 