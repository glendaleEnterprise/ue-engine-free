<template>
  <div>
    <div class="set-box">
      <div>X：</div>
      <a-slider @afterChange="XOffsetChange" :max="10" :min="-10" v-model="currentList.x" />
      <a-input-number v-model="currentInputList.x" :formatter="(value) => `${value}米`"
        :parser="(value) => value.replace('米', '')" @blur="XOffsetInputChange" />
    </div>
    <div class="set-box">
      <div>Y：</div>
      <a-slider @afterChange="YOffsetChange" :max="10" :min="-10" v-model="currentList.y" />
      <a-input-number v-model="currentInputList.y" :formatter="(value) => `${value}米`"
        :parser="(value) => value.replace('米', '')" @blur="YOffsetInputChange" />
    </div>
    <div class="set-box">
      <div>Z：</div>
      <a-slider @afterChange="ZOffsetChange" :max="10" :min="-10" v-model="currentList.z" />
      <a-input-number v-model="currentInputList.z" :formatter="(value) => `${value}米`"
        :parser="(value) => value.replace('米', '')" @blur="ZOffsetInputChange" />
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
        offsetList: {
          modelId: this.projectMessage.modelId,
          x: 0,
          y: 0,
          z: 0
        },
        currentList: {
          modelId: this.projectMessage.modelId,
          x: 0,
          y: 0,
          z: 0
        },
        currentInputList: {
          modelId: this.projectMessage.modelId,
          x: 0,
          y: 0,
          z: 0
        },
        currentFeature: '',
        offsetHistoryList: [],
      }
    },
    mounted() {
      const that = this;
      that.offsetHistoryList = store.state.bim.featureOffsetList;
      that.$message.info('请点击拾取要偏移的构件')
      api.Feature.getByEvent(true, (featureId) => {
        if (featureId.split("^")[1]) {
          if (that.currentFeature != '' && that.currentFeature != featureId) {
            let saveState = false
            for (var p in that.offsetList) {
              if (that.offsetList[p] != 0) {
                saveState = true
              }
            }
            saveState ? that.SaveOffsetList() : null
            that.currentFeature ? api.Feature.setColor(that.currentFeature, "rgba(255,255,255,1)") : null;
          }
          that.currentFeature = featureId;
          api.Feature.setColor(featureId, "rgba(255,255,0,1)");
          let index = that.offsetHistoryList.findIndex(item => item.id == featureId);
          if (index != -1) {
            that.offsetList = {
              ...that.offsetHistoryList[index].offsetList
            };
            that.currentList = {
              ...that.offsetHistoryList[index].offsetList
            };
          } else {
            that.offsetList = {
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
      api.Public.event("RIGHT_CLICK", function (e) {
        //执行事件
        that.SaveOffsetList()
        if (that.offsetHistoryList.length > 0) {
          that.offsetHistoryList.forEach(item => {
            api.Feature.original(item.id, item.id.split("^")[0])
          })
          store.dispatch('GetFeatureOffsetList', [])
          that.offsetList = {
            x: 0,
            y: 0,
            z: 0
          }
          that.currentList = {
            x: 0,
            y: 0,
            z: 0
          }
          that.offsetHistoryList = []
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
      XOffsetChange(value) {
        const that = this;
        that.currentInputList.x = value;
        api.Feature.offset(value * 100, 0, 0, that.currentFeature, that.currentFeature.split("^")[0]);
        that.offsetList.x += value;
        setTimeout(() => {
          that.currentList.x = 0;
          that.currentInputList.x = 0;
        }, 100)
      },
      XOffsetInputChange() {
        const that = this;
        api.Feature.offset(that.currentInputList.x * 100, 0, 0, that.currentFeature, that.currentFeature.split("^")[0]);
        that.offsetList.x += that.currentInputList.x;
        setTimeout(() => {
          that.currentInputList.x = 0;
        }, 100)
      },
      YOffsetChange(value) {
        const that = this;
        api.Feature.offset(0, value * 100, 0, that.currentFeature, that.currentFeature.split("^")[0]);
        that.offsetList.y += value;
        setTimeout(() => {
          that.currentList.y = 0;
        }, 100)
      },
      YOffsetInputChange() {
        const that = this;
        api.Feature.offset(0, that.currentInputList.y * 100, 0, that.currentFeature, that.currentFeature.split("^")[0]);
        that.offsetList.y += that.currentInputList.y;
        setTimeout(() => {
          that.currentInputList.y = 0;
        }, 100)
      },
      ZOffsetChange(value) {
        const that = this;
        api.Feature.offset(0, 0, value * 100, that.currentFeature, that.currentFeature.split("^")[0]);
        that.offsetList.z += value;
        setTimeout(() => {
          that.currentList.z = 0;
        }, 100)
      },
      ZOffsetInputChange() {
        const that = this;
        api.Feature.offset(0, 0, that.currentInputList.z * 100, that.currentFeature, that.currentFeature.split("^")[0]);
        that.offsetList.z += that.currentInputList.z;
        setTimeout(() => {
          that.currentInputList.z = 0;
        }, 100)
      },
      SaveOffsetList() {
        const that = this;
        if (that.currentFeature) {
          let index = that.offsetHistoryList.findIndex(item => item.id == that.currentFeature)
          index != -1 ? that.offsetHistoryList[index].offsetList = that.offsetList : that.offsetHistoryList.push({
            id: that.currentFeature,
            offsetList: that.offsetList
          });
        }
      }
    },
    destroyed() {
      const that = this;
      that.SaveOffsetList()
      store.dispatch('GetFeatureOffsetList', that.offsetHistoryList)
      that.currentFeature ? api.Feature.setColor(that.currentFeature, "rgba(255,255,255,1)") : null;
      api.Feature.getByEvent(false);
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