<!-- pages/index.vue -->
<template>
  <div class="min-h-screen bg-white">
    <!-- Main Content -->
    <section class="flex flex-col items-center justify-center min-h-[calc(100vh-8rem)] px-4">
      <!-- Vladimir name centered -->
      <h1 class="text-4xl sm:text-6xl lg:text-7xl font-bold text-gray-900 mb-4 text-center">
        Vladimir
      </h1>

      <!-- Fã de Formula 1 text -->
      <p class="text-xl sm:text-2xl lg:text-3xl text-gray-700 mb-8 text-center">
        Fã de Formula 1
      </p>

      <!-- F1 Logo -->
      <div class="mb-8">
        <img
          src="/images/f1-logo.png"
          alt="Logo da Formula 1"
          class="w-48 sm:w-64 lg:w-80 h-auto mx-auto"
          loading="eager"
        />
      </div>
    </section>
  </div>
</template>

<script setup>
// Página Home simples conforme especificação
useHead({
  title: 'Home - Vladimir Carlos Alves',
  meta: [
    {
      name: 'description',
      content: 'Vladimir Carlos Alves - Fã de Formula 1'
    }
  ]
})
</script>