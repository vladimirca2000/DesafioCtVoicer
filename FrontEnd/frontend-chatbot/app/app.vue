<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>

<script setup>
// Sem script específico aqui, apenas usando os componentes de layout e página do Nuxt.
</script>

<style>
/* Custom F1 styles */
.bg-f1-red {
  background-color: #E10600 !important;
}

.text-f1-red {
  color: #E10600 !important;
}

.hover\:bg-f1-red:hover {
  background-color: #c50500 !important;
}

.border-f1-red {
  border-color: #E10600 !important;
}

/* Focus improvements */
:focus {
  outline: 2px solid #E10600;
  outline-offset: 2px;
}

/* Estilos globais adicionais, se necessário, ou use assets/css/main.css */
/* Garante que o body não tenha margens padrão */
body {
  margin: 0;
  overflow-x: hidden; /* Evita scroll horizontal */
}
</style>