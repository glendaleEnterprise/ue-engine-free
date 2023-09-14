<template>
  <div>
    <a-space class="operate-btn" v-if="projectMessage.functionalPermissions">
      <a-button ghost @click="SaveRotation" @keyup.prevent @keydown.enter.prevent>保存旋转</a-button>
      <a-button ghost @click="ClearRotation" @keyup.prevent @keydown.enter.prevent>清除旋转</a-button>
    </a-space>
    <div class="set-box">
      <div>X：</div>
      <a-slider @change="XRotateChange" :max="360" :min="0" v-model="currentList[0]" />
    </div>
    <div class="set-box">
      <div>Y：</div>
      <a-slider @change="YRotateChange" :max="360" :min="0" v-model="currentList[1]" />
    </div>
    <div class="set-box">
      <div>Z：</div>
      <a-slider @change="ZRotateChange" :max="360" :min="0" v-model="currentList[2]" />
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
        rotateList: [0, 0, 0],
        currentList: [0, 0, 0],
        allRotate: 0
      }
    },
    mounted() {
      const that = this;
      let list = store.state.bim.modelEditList;
      let index = list.length > 0 ? list.findIndex(item => item.modelId == that.projectMessage.modelId) : -1;
      index != -1 ? that.rotateList = JSON.parse(JSON.stringify(list[index].rotate)) : null;
      that.currentList = JSON.parse(JSON.stringify(that.rotateList))
    },
    methods: {
      XRotateChange(value) {
        const that = this;
        api.Model.rotate(value - that.rotateList[0], 0, 0, that.projectMessage.modelId);
        that.rotateList[0] = value;
      },
      YRotateChange(value) {
        const that = this;
        api.Model.rotate(0, value - that.rotateList[1], 0, that.projectMessage.modelId);
        that.rotateList[1] = value;
      },
      ZRotateChange(value) {
        const that = this;
        that.allRotate += (value - that.rotateList[2])
        api.Model.rotate(0, 0, value - that.rotateList[2], that.projectMessage.modelId);
        that.rotateList[2] = value;
      },
      RotationData() {
        const that = this;
        let list = JSON.parse(JSON.stringify(store.state.bim.modelEditList));
        let index = list.findIndex(item => item.modelId == that.projectMessage.modelId);
        if (index != -1) {
          list[index].rotate = that.rotateList
        }
        store.dispatch('GetModelEditList', list)
      },
      SaveData() {
        const that = this;
        let list = JSON.parse(JSON.stringify(store.state.bim.modelEditList));
        let index = list.findIndex(item => item.modelId == that.projectMessage.modelId);
        if (index != -1) {
          that.projectMessage.modelList[index].matrix.rotate = JSON.parse(JSON.stringify(list[index].rotate))
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
      SaveRotation() {
        const that = this;
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要保存当前旋转数据吗？`,
          onOk() {
            that.RotationData()
            that.SaveData()
            that.$message.success('保存成功')
          },
        })

      },
      ClearRotation() {
        const that = this;
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要清除旋转吗？`,
          onOk() {
            api.Model.clearRotation(that.projectMessage.modelId)
            // api.Model.rotate(
            //   that.rotateList[0] != 0 ? -that.rotateList[0] : 0,
            //   that.rotateList[1] != 0 ? -that.rotateList[1] : 0,
            //   that.rotateList[2] != 0 ? -that.rotateList[2] : 0,
            //   that.projectMessage.modelId
            // );
            that.currentList = [0, 0, 0];
            that.rotateList = [0, 0, 0];
            that.RotationData();
            that.SaveData();
            that.$message.success('清除成功')
          },
        })
      },
    },
    destroyed() {
      const that = this;
      that.RotationData()
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

  .operate-btn {
    width: calc(100% - 30px);
    justify-content: space-around;
    margin: 10px;
  }
</style>