<template>
  <div class="set-box">
    <div>日照时间：</div>
    <a-slider @change="ModelExplosionChange" :max="780" :min="0" v-model="sunshineTime" :step="1"
      :tip-formatter="formatter" />
    <span>{{ timeValue }}</span>
  </div>
</template>

<script>
import store from '@/store'
export default {
  data() {
    return {
      sunshineTime: 420,
      timeValue: '13:00',
    }
  },
  mounted() {
    const that = this;
    that.sunshineTime = store.state.bim.sunshineTime;
    let time = [6 + parseInt(that.sunshineTime / 60), (that.sunshineTime % 60)];
    let minute = parseInt(time[1]) < 10 ? '0' + time[1] : JSON.stringify(time[1])
    that.timeValue = time[0] + ":" + minute;
  },
  methods: {
    ModelExplosionChange(value) {
      const that = this;
      let time = [6 + parseInt(value / 60), (value % 60)];
      let minute = parseInt(time[1]) < 10 ? '0' + time[1] : JSON.stringify(time[1])
      api.Public.setTime({
        'Time': time[0] + ":" + minute
      })
      that.timeValue = time[0] + ":" + minute;
    },
    formatter(value) {
      var time = 6 + parseInt(value / 60) + ":" + (String(Math.round(value % 60)).length == 1 ? '0' + Math.round(value % 60) : Math.round(value % 60))
      return `${time}`;
    },
  },
  destroyed() {
    store.dispatch('GetSunshineTime', this.sunshineTime)
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
    width: 55%;
    margin: 0;
  }
}
</style>