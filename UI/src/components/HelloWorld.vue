<script setup>
import { onMounted, ref } from 'vue'
// import axios from 'axios'

defineProps({
  msg: String,
})

const count = ref(0)

const message = ref('Carregando...')

onMounted(async () => {
  try {
    const response = await fetch('/api/hello')

    if (!response.ok) throw new Error('Erro na API')

    const data = await response.text()
    message.value = data
  } catch (err) {
    message.value = 'Erro ao buscar na API'
    console.error(err)
  }
})

async function HelloName(name) {
  try{
    const response = await fetch(`api/hello/${name}`)

    if (!response.ok) throw new Error('Erro na API')
    debugger
    const data = await response.text()
    message.value = data
  } catch (err) {
    message.value = 'Erro ao buscar na API'
    console.error(err)
  }
}
</script>

<template>
  <h1>{{ msg }}</h1> 

  <div class="card">
    <button type="button" @click="count++">count is {{ count }}</button>
    <button type="button" @click="HelloName('teste')">{{message}}</button>
    <p>
      Edit
      <code>components/HelloWorld.vue</code> to test HMR
    </p>
  </div>

  <p>
    Check out
    <a href="https://vuejs.org/guide/quick-start.html#local" target="_blank">create-vue</a>, the official Vue + Vite
    starter
  </p>
  <p>
    Learn more about IDE Support for Vue in the
    <a href="https://vuejs.org/guide/scaling-up/tooling.html#ide-support" target="_blank">Vue Docs Scaling up Guide</a>.
  </p>
  <p class="read-the-docs">Click on the Vite and Vue logos to learn more</p>
</template>

<style scoped>
.read-the-docs {
  color: #888;
}
</style>