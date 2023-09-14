<template>
  <div>
    <div class="set-box">
      <div>X：</div>
      <a-slider @change="XDragCutChange" :max="1" :min="-1" :step="0.01" :range="true" :default-value="[-1, 1]" />
    </div>
    <div class="set-box">
      <div>Y：</div>
      <a-slider @change="YDragCutChange" :max="1" :min="-1" :step="0.01" :range="true" :default-value="[-1, 1]" />
    </div>
    <div class="set-box">
      <div>Z：</div>
      <a-slider @change="ZDragCutChange" :max="1" :min="-1" :step="0.01" :range="true" :default-value="[-1, 1]" />
    </div>
  </div>
</template>

<script>
export default {
  props: {
    projectMessage: {
      type: Object,
      default: undefined
    }
  },
  data() {
    return {
      xValue: [-1, 1],
      yValue: [-1, 1],
      zValue: [-1, 1],
    }
  },
  methods: {
    XDragCutChange(value) {
      if (value[0] != this.xValue[0]) {
        api.Model.clip('X-MIN', value[0], this.projectMessage.modelId);//从小到大，
      } else {
        api.Model.clip('X-MAX', value[1], this.projectMessage.modelId);//从大到小
      }
      this.xValue = value;
    },
    YDragCutChange(value) {
      if (value[0] != this.yValue[0]) {
        api.Model.clip('Y-MIN', value[0], this.projectMessage.modelId);//从小到大，
      } else {
        api.Model.clip('Y-MAX', value[1], this.projectMessage.modelId);//从大到小
      }
      this.yValue = value;
    },
    ZDragCutChange(value) {
      if (value[0] != this.zValue[0]) {
        api.Model.clip('Z-MIN', value[0], this.projectMessage.modelId);//从小到大，
      } else {
        api.Model.clip('Z-MAX', value[1], this.projectMessage.modelId);//从大到小
      }
      this.zValue = value;
    },
  },
  destroyed() {
    api.Model.closeClip();
  }
}
</script>

<style lang="less" scoped>
.set-box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  line-height: 40px;

  >div:first-child {
    width: 20%;
    text-align: center;
  }

  /deep/.ant-slider {
    width: 80%;
    margin: 0;
  }
}
</style>