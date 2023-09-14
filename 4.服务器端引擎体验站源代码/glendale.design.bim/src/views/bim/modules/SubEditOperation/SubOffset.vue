<template>
  <div>
    <a-space class="operate-btn" v-if="projectMessage.functionalPermissions">
      <a-button ghost @click="SaveOffset" @keyup.prevent @keydown.enter.prevent>保存偏移</a-button>
      <a-button ghost @click="ClearOffset" @keyup.prevent @keydown.enter.prevent>清除偏移</a-button>
    </a-space>
    <div class="set-box">
      <div>X：</div>
      <a-slider @afterChange="XOffsetChange" :max="10" :min="-10" v-model="currentList[0]" />
      <a-input-number v-model="currentInputList[0]" :formatter="(value) => `${value}米`"
        :parser="(value) => value.replace('米', '')" @blur="XOffsetInputChange" />
    </div>
    <div class="set-box">
      <div>Y：</div>
      <a-slider @afterChange="YOffsetChange" :max="10" :min="-10" v-model="currentList[1]" />
      <a-input-number v-model="currentInputList[1]" :formatter="(value) => `${value}米`"
        :parser="(value) => value.replace('米', '')" @blur="YOffsetInputChange" />
    </div>
    <div class="set-box">
      <div>Z：</div>
      <a-slider @afterChange="ZOffsetChange" :max="10" :min="-10" v-model="currentList[2]" />
      <a-input-number v-model="currentInputList[2]" :formatter="(value) => `${value}米`"
        :parser="(value) => value.replace('米', '')" @blur="ZOffsetInputChange" />
    </div>
  </div>
</template>

<script>
  import store from '@/store'
  import {
    setMatrix
  } from '@/api/document'
  import {
    setCombineSetting
  } from '@/api/viewpoint'
  export default {
    props: {
      projectMessage: {
        type: Object,
        default: undefined
      }
    },
    data() {
      return {
        offectList: [0, 0, 0],
        currentList: [0, 0, 0],
        currentInputList: [0, 0, 0]
      }
    },
    mounted() {
      const that = this;
      let list = store.state.bim.modelEditList;
      let index = list.length > 0 ? list.findIndex(item => item.modelId == that.projectMessage.modelId) : -1;
      index != -1 ? that.offectList = JSON.parse(JSON.stringify(list[index].offset)) : null;
    },
    methods: {
      XOffsetChange(value) {
        const that = this;
        that.currentInputList[0] = value;
        api.Model.offset(value, 0, 0, that.projectMessage.modelId);
        that.offectList[0] += value;
        setTimeout(() => {
          that.currentList = [0, 0, 0];
          that.currentInputList = [0, 0, 0];
        }, 100)
      },
      XOffsetInputChange() {
        const that = this;
        api.Model.offset(that.currentInputList[0], 0, 0, that.projectMessage.modelId);
        that.offectList[0] += that.currentInputList[0];
        setTimeout(() => {
          that.currentList = [0, 0, 0];
          that.currentInputList = [0, 0, 0];
        }, 100)
      },
      YOffsetChange(value) {
        const that = this;
        api.Model.offset(0, value, 0, that.projectMessage.modelId);
        that.offectList[1] += value;
        setTimeout(() => {
          that.currentList = [0, 0, 0];
          that.currentInputList = [0, 0, 0];
        }, 100)
      },
      YOffsetInputChange() {
        const that = this;
        api.Model.offset(0, that.currentInputList[1], 0, that.projectMessage.modelId);
        that.offectList[1] += that.currentInputList[1];
        setTimeout(() => {
          that.currentList = [0, 0, 0];
          that.currentInputList = [0, 0, 0];
        }, 100)
      },
      ZOffsetChange(value) {
        const that = this;
        api.Model.offset(0, 0, value, that.projectMessage.modelId);
        that.offectList[2] += value;
        setTimeout(() => {
          that.currentList = [0, 0, 0];
          that.currentInputList = [0, 0, 0];
        }, 100)
      },
      ZOffsetInputChange() {
        const that = this;
        api.Model.offset(0, 0, that.currentInputList[2], that.projectMessage.modelId);
        that.offectList[2] += that.currentInputList[2];
        setTimeout(() => {
          that.currentList = [0, 0, 0];
          that.currentInputList = [0, 0, 0];
        }, 100)
      },
      OffsetData() {
        const that = this;
        let list = JSON.parse(JSON.stringify(store.state.bim.modelEditList));
        let index = list.findIndex(item => item.modelId == that.projectMessage.modelId);
        if (index != -1) {
          list[index].offset = that.offectList
        }
        store.dispatch('GetModelEditList', list)
      },
      SaveData() {
        const that = this;
        let list = JSON.parse(JSON.stringify(store.state.bim.modelEditList));
        let index = list.findIndex(item => item.modelId == that.projectMessage.modelId);
        if (index != -1) {
          that.projectMessage.modelList[index].matrix.offset = JSON.parse(JSON.stringify(list[index].offset))
        }
        if (that.projectMessage.sceneType == 0) {
          setMatrix(that.projectMessage.modelList[0].id, JSON.stringify(that.projectMessage.modelList[0].matrix))
        } else {
          let combineDetails = JSON.parse(JSON.stringify(that.projectMessage.modelList))
          combineDetails.forEach((item) => {
            item.matrix = JSON.stringify(item.matrix)
          })
          setCombineSetting(that.projectMessage.id, {
            "folderId": that.projectMessage.folderId,
            "combineName": that.projectMessage.combineName,
            "projectId": that.projectMessage.projectId,
            "projectFolderId": that.projectMessage.modelList[0].projectFolderId,
            "documentName": that.projectMessage.modelList[0].documentName,
            "documentType": 1,
            "modelName": that.projectMessage.modelList[0].modelName,
            "modelFormat": that.projectMessage.modelList[0].modelFormat,
            "camera": that.projectMessage.camera ? JSON.stringify(that.projectMessage.camera) : undefined,
            "size": that.projectMessage.modelList[0].size,
            "sceneConfig": that.projectMessage.sceneConfig ? JSON.stringify(that.projectMessage.sceneConfig) :
              undefined,
            "combineDetails": combineDetails,
          })
        }

      },
      SaveOffset() {
        const that = this;
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要保存当前偏移数据吗？`,
          onOk() {
            that.OffsetData()
            that.SaveData()
            that.$message.success('保存成功')
          },
        })
      },
      ClearOffset() {
        const that = this;
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要清除偏移吗？`,
          onOk() {
            api.Model.clearOffset(that.projectMessage.modelId)
            // api.Model.offset(
            //   that.offectList[0] != 0 ? -that.offectList[0] : 0,
            //   that.offectList[1] != 0 ? -that.offectList[1] : 0,
            //   that.offectList[2] != 0 ? -that.offectList[2] : 0,
            //   that.projectMessage.modelId
            // );
            that.currentList = [0, 0, 0];
            that.offectList = [0, 0, 0];
            that.currentInputList = [0, 0, 0]
            that.OffsetData()
            that.SaveData();
            that.$message.success('清除成功')
          },
        })
      },
    },
    destroyed() {
      const that = this;
      that.OffsetData()
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

  .operate-btn {
    width: calc(100% - 30px);
    justify-content: space-around;
    margin: 10px;
  }
</style>