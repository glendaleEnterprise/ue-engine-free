<template>
  <div>
    <a-space>提示：右键恢复模型。</a-space>
    <a-space>鼠标左键点击开始框选，随着鼠标移动显示框选范围，再次点击结束框选</a-space>
  </div>
</template>

<script>
export default {
  props: {
    projectMessage: {
      type: Object,
      default: undefined,
    },
  },
  data() {
    return {
      featureList: []
    }
  },
  mounted() {
    const that = this;
    that.StartBoxSelect()
    api.Public.event("RIGHT_CLICK", function (e) {
      // 执行事件
      api.Feature.setColor(that.featureList.join("#"), "rgba(255,255,255,1)");  //恢复构件颜色
      that.featureList = []
    });
  },
  methods: {
    StartBoxSelect() {
      const that = this;
      api.Feature.boxSelect(
        (data) => {
          if (data.length != 1 && data[0] != "") {
            that.featureList = that.uniqueArr(that.featureList, data)
            api.Feature.setColor(data.join("#"), "rgba(255,0,0,1)");
          }
        }
      );
    },
    uniqueArr(arr1, arr2) {
      //合并两个数组
      arr1.push(...arr2)
      //去重
      let arr3 = Array.from(new Set(arr1))
      return arr3
    }
  },
  destroyed() {
    this.featureList.length > 0 ? api.Feature.setColor(this.featureList.join("#"), "rgba(255,255,255,1)") : null;  //恢复构件颜色
    api.Public.clearHandler("RIGHT_CLICK");
    api.Feature.closeBoxSelect();
  }
}
</script>

