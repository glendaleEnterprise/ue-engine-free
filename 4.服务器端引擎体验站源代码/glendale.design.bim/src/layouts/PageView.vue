<template>
  <router-view :key="$route.fullPath" />
</template>

<script>
export default {
  name: 'PageView',
}
</script>
