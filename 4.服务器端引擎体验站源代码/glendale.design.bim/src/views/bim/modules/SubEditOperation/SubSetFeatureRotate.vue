<template>
  <div id="FeatureRotate">
    <div class="set-box">
      <div>X：</div>
      <a-slider @afterChange="XRotateChange" :max="360" :min="0" v-model="currentList.x" />
      <a-input-number v-model="currentList.x" :formatter="(value) => `${value}度`"
        :parser="(value) => value.replace('度', '')" :disabled="true" />
    </div>
    <div class="set-box">
      <div>Y：</div>
      <a-slider @afterChange="YRotateChange" :max="360" :min="0" v-model="currentList.y" />
      <a-input-number v-model="currentList.y" :formatter="(value) => `${value}度`"
        :parser="(value) => value.replace('度', '')" :disabled="true" />
    </div>
    <div class="set-box">
      <div>Z：</div>
      <a-slider @afterChange="ZRotateChange" :max="360" :min="0" v-model="currentList.z" />
      <a-input-number v-model="currentList.z" :formatter="(value) => `${value}度`"
        :parser="(value) => value.replace('度', '')" :disabled="true" />
    </div>
  </div>
</template>

<script>
  import store from '@/store'
  export default {
    props: {
      projectMessage: {
        type: Object,
        default: undefined
      }
    },
    data() {
      return {
        rotateList: {
          x: 0,
          y: 0,
          z: 0
        },
        currentList: {
          x: 0,
          y: 0,
          z: 0
        },
        currentFeature: '',
        rotateHistoryList: [],
      }
    },
    mounted() {
      const that = this;
      that.rotateHistoryList = store.state.bim.featureRotateList;
      that.$message.info('请点击拾取要旋转的构件')
      api.Feature.getByEvent(true, (featureId) => {
        if (featureId.split("^")[1]) {
          if (that.currentFeature != '' && that.currentFeature != featureId) {
            let saveState = false
            for (var p in that.rotateList) {
              if (that.rotateList[p] != 0) {
                saveState = true
              }
            }
            saveState ? that.SaveRotateList() : null
            api.Feature.setColor(that.currentFeature, "rgba(255,255,255,1)");
            that.rotateList = {
              x: 0,
              y: 0,
              z: 0
            }
          }
          that.currentFeature = featureId;
          api.Feature.setColor(featureId, "rgba(255,255,0,1)");
          let index = that.rotateHistoryList.findIndex(item => item.id == featureId);
          if (index != -1) {
            that.rotateList = {
              ...that.rotateHistoryList[index].rotateList
            };
            that.currentList = {
              ...that.rotateHistoryList[index].rotateList
            };
          } else {
            that.rotateList = {
              x: 0,
              y: 0,
              z: 0
            }
            that.currentList = {
              x: 0,
              y: 0,
              z: 0
            }
          }
        }
      });
      api.Public.event("RIGHT_CLICK", (e) => {
        //执行事件
        that.SaveRotateList()
        if (that.rotateHistoryList.length > 0) {
          that.rotateHistoryList.forEach(item => {
            api.Feature.original(item.id, item.id.split("^")[0])
          })
          that.rotateHistoryList = []
          store.dispatch('GetFeatureRotateList', [])
          that.rotateList = {
            x: 0,
            y: 0,
            z: 0
          }
          that.currentList = {
            x: 0,
            y: 0,
            z: 0
          }
        }
      });
      that.$notification.open({
        key: 'EngineKey',
        description: '右键恢复模型',
        class: 'original-notification tips-notification',
        duration: null,
        placement: 'bottomRight',
      })
    },
    methods: {
      XRotateChange(value) {
        const that = this;
        if (Math.abs(value - that.rotateList.x) != 0 && that.currentFeature != "") {
          api.Feature.getGeometrySizeById(that.currentFeature, (res) => {
            if (Math.abs(value - that.rotateList.x) != 0) {
              api.Feature.rotateByAnyAxis({
                "featureId": that.currentFeature,
                "point1": [res.center.x, res.center.y, res.center.z],
                "point2": [res.center.x + 0.01, res.center.y, res.center.z],
                "speed": value > that.rotateList.x ? 1000 : -1000,
                "angle": Math.abs(value - that.rotateList.x)
              });
              that.rotateList.x = value;
            }
          });
        }
      },
      YRotateChange(value) {
        const that = this;
        if (Math.abs(value - that.rotateList.y) != 0 && that.currentFeature != "") {
          api.Feature.getGeometrySizeById(that.currentFeature, (res) => {
            if (Math.abs(value - that.rotateList.y) != 0) {
              api.Feature.rotateByAnyAxis({
                "featureId": that.currentFeature,
                "point1": [res.center.x, res.center.y, res.center.z],
                "point2": [res.center.x, res.center.y + 0.01, res.center.z],
                "speed": value > that.rotateList.y ? 1000 : -1000,
                "angle": Math.abs(value - that.rotateList.y)
              });
              that.rotateList.y = value;
            }
          });
        }
      },
      ZRotateChange(value) {
        const that = this;
        if (Math.abs(value - that.rotateList.z) != 0 && that.currentFeature != "") {
          api.Feature.getGeometrySizeById(that.currentFeature, (res) => {
            if (Math.abs(value - that.rotateList.z) != 0) {
              api.Feature.rotateByAnyAxis({
                "featureId": that.currentFeature,
                "point1": [res.center.x, res.center.y, res.center.z],
                "point2": [res.center.x, res.center.y, res.center.z + 0.01],
                "speed": value > that.rotateList.z ? 1000 : -1000,
                "angle": Math.abs(value - that.rotateList.z)
              });
              that.rotateList.z = value;
            }
          });
        }
      },
      SaveRotateList() {
        const that = this;
        if (that.currentFeature) {
          let index = that.rotateHistoryList.findIndex(item => item.id == that.currentFeature)
          index != -1 ? that.rotateHistoryList[index].rotateList = that.rotateList : that.rotateHistoryList.push({
            id: that.currentFeature,
            rotateList: that.rotateList
          });
        }
      }
    },
    destroyed() {
      const that = this;
      that.SaveRotateList()
      store.dispatch('GetFeatureRotateList', that.rotateHistoryList)
      that.currentFeature ? api.Feature.setColor(that.currentFeature, "rgba(255,255,255,1)") : null;
      api.Feature.getByEvent(false);
      api.Public.clearHandler("RIGHT_CLICK");
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

    /deep/.ant-input-number {
      background: transparent;
      color: #ffffff;
      margin-left: 5px;
      width: 65px;

      .ant-input-number-handler-wrap {
        display: none;
      }

      input {
        text-align: center;
        padding: 0;
      }
    }
  }
</style>

<style lang="less">
  .ant-notification-bottomRight {
    .original-notification {
      padding: 15px;

      .ant-notification-notice-message {
        display: none;
      }

      .ant-notification-notice-close {
        display: none;
      }
    }
  }
</style>