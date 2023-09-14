<template>
  <div ref="wrap">
    <div class="clamp-box">
      <a-button type="primary" @click="MoldClosingSaving()" class="close-btn">合模保存</a-button>
      <!-- <a-button type="primary" @click="CancelMoldClosing()" class="close-btn">{{ $t('list.cancel') }}</a-button> -->
    </div>
    <a-modal :visible="visible" width="300px" title="合模保存" @ok="SaveClamping" centered @cancel="ResetClamping"
      :ok-text="$t('public.OK')" :cancel-text="$t('public.cancel')" v-drag :destroyOnClose="true" :maskClosable="false"
      :mask="false" :getContainer="() => this.$refs.wrap">
      <div class="edit-item">
        <div>合模名称：</div>
        <div>
          <a-input v-model="clampingName" placeholder="请输入合模名称" />
        </div>
      </div>
      <div class="edit-item">
        <div>合模说明：</div>
        <div>
          <a-textarea v-model="clampingSort" placeholder="合模说明" :rows="4" />
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script>
  import {
    saveModelClosing
  } from '@/api/combine'
  export default {
    props: {
      projectMessage: {
        type: Object,
      },
      movementPosition: {
        type: Array,
      },
      historyFlattenList: {
        type: Array,
      },
      GetSaveMessage: {
        type: Function,
      },
      mapValue: {
        type: Object
      },
      closePage: {
        type: Function
      },
      folderIdClamping: {
        type: String
      },
    },
    data() {
      return {
        visible: false,
        clampingName: '',
        clampingSort: '',
      }
    },
    methods: {
      MoldClosingSaving() {
        const that = this
        that.clampingName = '';
        that.clampingSort = '';
        that.visible = true
        that.$emit('GetSaveMessage', {
          type: 'matrix',
        })
      },
      CancelMoldClosing() {
        const that = this
        that.visible = false
        that.$emit('closePage')
      },
      SaveClamping() {
        const that = this
        let modelList = []
        if (that.movementPosition.length == 0) {
          that.$message.error('当前没有可以合模的模型，请先选择模型')
          return
        }
        if (!this.clampingName) {
          this.$message.error('请输入合模名称')
          return
        }
        for (let i = 0; i < that.movementPosition.length; i++) {
          modelList.push({
            documentId: that.movementPosition[i].key,
            matrix: JSON.stringify(that.movementPosition[i].matrix),
            matrixGis: that.movementPosition[i].matrixGis ?
              JSON.stringify(that.movementPosition[i].matrixGis) : '',
            modelConfig: that.movementPosition[i].modelConfig
          })
        }
        let flattenList = []
        for (let i = 0; i < that.historyFlattenList.length; i++) {
          flattenList.push({
            flattenName: that.historyFlattenList[i].name,
            flattenHeight: that.historyFlattenList[i].height,
            flattenScope: typeof that.historyFlattenList[i].positions != 'string' ? JSON.stringify(that
              .historyFlattenList[i].positions) : that.historyFlattenList[i].positions,
          })
        }
        that.$confirm({
          cancelText: '否',
          okText: '是',
          title: `同时设置当前视角为合模“${that.clampingName}”的默认加载视角吗？`,
          onOk() {
            api.Camera.getViewPort((data) => {
              that.SaveClamp(modelList, flattenList, JSON.stringify(data))
            })
          },
          onCancel() {
            that.SaveClamp(modelList, flattenList, "")
          },
        })

      },
      SaveClamp(modelList, flattenList, camera) {
        const that = this;
        saveModelClosing({
          folderId: that.folderIdClamping,
          projectId: that.$store.getters.currProjectId,
          combineName: that.clampingName,
          camera: camera,
          isGis: that.mapValue.state,
          remark: that.clampingSort,
          isOpen: false,
          combineDetails: modelList,
          combineFlattens: flattenList,
          gisType: that.mapValue.type,
          sceneConfig: that.projectMessage.sceneConfig ? JSON.stringify(that.projectMessage.sceneConfig) : null,
        }).then((res) => {
          that.$message.success('合模保存成功！')
          that.visible = false;
        })
      },
      ResetClamping() {
        const that = this
        that.visible = false
      },
    },
  }
</script>
<style lang="less" scoped>
  .clamp-box {
    position: absolute;
    bottom: 20px;
    right: 35px;

    >.ant-btn {
      margin: 0 5px;
    }
  }


  .edit-item {
    display: flex;
    align-items: flex-start;
    margin: 10px 0;

    >div {
      &:first-child {
        width: 30%;
        text-align: right;
        line-height: 32px;
      }

      &:last-child {
        width: 70%;
      }
    }
  }

  /deep/.ant-input,
  /deep/.ant-input-number {
    background-color: transparent;
    color: #fff;
    width: 100%;
  }
</style>