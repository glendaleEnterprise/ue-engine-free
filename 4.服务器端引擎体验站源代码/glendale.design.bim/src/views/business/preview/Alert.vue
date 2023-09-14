<template>
  <div class="box" :style="{ height: `${searchHeight}px` }">
    <a-result status="warning" title="格式暂不支持浏览">       
    </a-result>
  </div>
</template>
<script>
export default {
  name: 'Alert',
  data() {
    return {
      searchHeight: 0,
    }
  },
  props: {
    share: {
      type: Boolean,
      default: false,
    },
  },
  mounted() {
    this.onResize()
    window.onresize = () => {
      this.onResize()
    }
  },
  methods: {
    onResize() {
      if (this.share) {
        this.searchHeight = window.innerHeight
      } else {
        this.searchHeight = window.innerHeight - 128
      }
    },
  },
  destroyed() {
    // 组件销毁后解绑事件
    window.onresize = null
  },
}
</script>
<style lang="less" scoped >
.box {  
  background-color: #fff; 
}
</style>