<template>
  <a-result status="404" title="404" sub-title="Sorry, the page you visited does not exist.">
    <template #extra>
      <a-button type="primary" @click="toHome">
       返回登录
      </a-button>
    </template>
  </a-result>
</template>

<script>
export default {
  name: 'Exception404',
  methods: {
    toHome () {
      this.$store.dispatch('Logout').then(() => {
        this.$router.replace({ path: '/login'  }, () => {
          location.reload()
        })
      })
      // this.$router.push({ path: '/login' })
    }
  }
}
</script>
