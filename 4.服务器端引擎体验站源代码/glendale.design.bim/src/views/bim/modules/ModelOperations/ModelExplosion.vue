<template>
  <div class="set-box">
    <div>爆炸：</div>
    <a-slider @change="ModelExplosionChange" :max="1" :min="0" :step="0.01" />
  </div>
</template>

<script>
export default {
  methods: {
    ModelExplosionChange(value) {
      api.Model.blow(value);
    }
  },
  destroyed() {
    api.Model.blow(0);
  }
}
</script>

<style lang="less" scoped>
.set-box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  line-height: 50px;
  // margin: 0 8px;

  /deep/.ant-slider {
    width: 80%;
    margin: 0;
  }
}
</style>