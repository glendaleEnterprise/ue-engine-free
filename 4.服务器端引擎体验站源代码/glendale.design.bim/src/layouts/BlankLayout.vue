<template>
  <div>
    <router-view :key="$route.fullPath" />
  </div>
</template>

<script>

export default {
  name: 'BlankLayout'
}
</script>

<style scoped>
</style>
