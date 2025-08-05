<!-- pages/contact.vue -->
<template>
  <div class="min-h-screen bg-white py-8">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Page Header -->
      <div class="text-center mb-12">
        <h1 class="text-4xl sm:text-5xl font-bold text-gray-900 mb-4">
          Contato
        </h1>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-12">
        <!-- Contact Information -->
        <div class="space-y-6">
          <div class="bg-gray-50 rounded-lg p-6">
            <h2 class="text-2xl font-bold text-gray-900 mb-6">Informações de Contato</h2>
            
            <div class="space-y-4">
              <div class="flex items-center space-x-3">
                <span class="font-semibold text-gray-700 w-20">Nome:</span>
                <span class="text-gray-900">Vladimir Carlos Alves</span>
              </div>

              <div class="flex items-center space-x-3">
                <span class="font-semibold text-gray-700 w-20">WhatsApp:</span>
                <a 
                  href="tel:+5511999999999"
                  class="text-blue-600 hover:text-blue-800 transition-colors duration-200"
                >
                  (11) 99999-9999
                </a>
              </div>

              <div class="flex items-center space-x-3">
                <span class="font-semibold text-gray-700 w-20">E-mail:</span>
                <a 
                  href="mailto:vladimir@exemplo.com"
                  class="text-blue-600 hover:text-blue-800 transition-colors duration-200"
                >
                  vladimir@exemplo.com
                </a>
              </div>

              <div class="flex items-center space-x-3">
                <span class="font-semibold text-gray-700 w-20">LinkedIn:</span>
                <a 
                  href="https://linkedin.com/in/vladimir-carlos-alves"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="text-blue-600 hover:text-blue-800 transition-colors duration-200"
                >
                  linkedin.com/in/vladimir-carlos-alves
                </a>
              </div>

              <div class="flex items-center space-x-3">
                <span class="font-semibold text-gray-700 w-20">GitHub:</span>
                <a 
                  href="https://github.com/vladimir-carlos-alves"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="text-blue-600 hover:text-blue-800 transition-colors duration-200"
                >
                  github.com/vladimir-carlos-alves
                </a>
              </div>
            </div>
          </div>
        </div>

        <!-- Map Section -->
        <div class="space-y-6">
          <div class="bg-gray-50 rounded-lg p-6">
            <h2 class="text-2xl font-bold text-gray-900 mb-6">Localização</h2>
            
            <div class="h-80 bg-gray-200 rounded-lg flex items-center justify-center">
              <iframe
                src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3657.1975605576845!2d-46.6634!3d-23.5615!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x94ce59c8da0aa315%3A0xd59f9431f2c9776a!2sSao%20Paulo%2C%20SP%2C%20Brazil!5e0!3m2!1sen!2s!4v1644123456789!5m2!1sen!2s"
                width="100%"
                height="100%"
                style="border:0"
                loading="lazy"
                allowfullscreen
                referrerpolicy="no-referrer-when-downgrade"
                class="w-full h-full rounded-lg"
                title="Mapa da localização de Vladimir Carlos Alves"
              ></iframe>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
// Página de contato simples conforme especificação
useHead({
  title: 'Contato - Vladimir Carlos Alves',
  meta: [
    {
      name: 'description',
      content: 'Entre em contato com Vladimir Carlos Alves'
    }
  ]
})
</script>