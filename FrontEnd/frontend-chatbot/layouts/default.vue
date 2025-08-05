<template>
  <div class="min-h-screen flex flex-col bg-white">
    <AppNavbar />
    <main class="flex-grow pt-16">
      <slot />
    </main>
    <AppFooter />
    <ChatFloatingButtonSimple />
  </div>
</template>