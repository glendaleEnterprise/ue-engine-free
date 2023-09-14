<template>
  <div ref="wrap" class="layer-box">
    <a-modal title="图层管理" :width="480" :visible="visible" v-drag :destroyOnClose="true" :maskClosable="false"
      :mask="false" @cancel="handleCancel" :getContainer="() => this.$refs.wrap" :footer="null">
      <a-table :columns="columns" :rowKey="(record) => record.id" :data-source="tableData" :pagination="false"
        :bordered="true">
        <template slot="documentName" slot-scope="text">
          <a-space>{{ text.split(".")[0] }}</a-space>
        </template>
        <template slot="modelFormat" slot-scope="text">
          <a-space>{{ text.toUpperCase() }}</a-space>
        </template>
        <template slot="modelSize" slot-scope="text">
          <a-space>{{ text | byteToMB }}MB</a-space>
        </template>
        <template slot="renderAction" slot-scope="text, record">
          <a-switch v-model="record.isShow" @change="checked => { onChange(checked, record) }" />
          &nbsp;<a-icon type="environment" title="定位到模型" @click="onLocation(record)"></a-icon>
        </template>
        <template slot="visibleDistance" slot-scope="text, record">
          <a-input-number placeholder="请输入可视距离" v-model="record.modelConfig.visibleDistance" :min="0"
            @blur="ChangeVisibleDistance(record)" />
        </template>
        <template slot="modelSize" slot-scope="text">
          <a-space>{{ text | byteToMB }}MB</a-space>
        </template>
        <template slot="isMain" slot-scope="text, record">
          <a-switch v-model="record.isMain" @change="setMainModel(text, record)" />
        </template>
      </a-table>
    </a-modal>
  </div>
</template>
<script>
  import {
    setModelConfig
  } from '@/api/document'
  import {
    combineModelConfig
  } from '@/api/combine'
  export default {
    name: 'LayerList',
    props: {
      projectMessage: {
        type: Object,
        default: undefined,
      },
      layerListVisible: {
        type: Boolean,
        required: true,
      },
    },
    data() {
      return {
        visible: this.layerListVisible,
        columns: [{
            title: '模型名称',
            dataIndex: 'documentName',
            ellipsis: true,
            scopedSlots: {
              customRender: 'documentName'
            },
            align: 'center',
          },
          {
            title: '建模工具',
            dataIndex: 'modelFormat',
            ellipsis: true,
            align: 'center',
            scopedSlots: {
              customRender: 'modelFormat'
            },
          },
          {
            title: '模型大小',
            dataIndex: 'size',
            scopedSlots: {
              customRender: 'modelSize'
            },
            ellipsis: true,
            align: 'center',
          },
          {
            title: '加载状态',
            dataIndex: 'id',
            scopedSlots: {
              customRender: 'renderAction'
            },
            width: '80px',
            align: 'center',
          },
          {
            title: '可视距离',
            dataIndex: 'visible',
            scopedSlots: {
              customRender: 'visibleDistance'
            },
            ellipsis: true,
            align: 'center',
          },
        ],
        tableData: [],
        confirmTag: undefined
      }
    },
    mounted() {
      if (!(this.projectMessage.sceneClassification != 2 && this.projectMessage.functionalPermissions)) {
        this.columns.splice(this.columns.length - 1, 1)
      }
      if (this.projectMessage.sceneType == 1 && this.projectMessage.modelList.length > 1) {
        this.columns.push({
          title: '主模型',
          dataIndex: 'isMain',
          scopedSlots: {
            customRender: 'isMain'
          },
          width: '60px',
          align: 'center',
        }, )
      }

      this.fetch()
    },
    methods: {
      fetch() {
        this.tableData = this.projectMessage.modelList
      },
      onChange(checked, record) {
        if (checked) {
          api.Model.setVisible(record.id, true)
        } else {
          api.Model.setVisible(record.id, false)
        }
      },
      onLocation(record) {
        api.Model.location(record.id)
      },
      handleCancel() {
        this.visible = false;
        this.$emit('update:layerListVisible', this.visible)
      },
      setMainModel(text, record) {
        const that = this;
        let haveMain = that.projectMessage.modelList.filter(item => item.isMain == true)
        if (haveMain.length == 0 && text) {
          that.projectMessage.modelList.forEach(element => {
            element.id == record.id ? element.isMain = true : null;
          });
          that.$message.warning('必须保留一个主模型设置');
        } else {
          that.confirmTag ? that.confirmTag.destroy() : null;
          that.confirmTag = that.$confirm({
            cancelText: '取消',
            okText: '确定',
            title: `确定要设置模型${record.documentName.split(".")[0]}为主模型吗？`,
            onOk() {
              that.projectMessage.modelId = record.id;
              that.projectMessage.documentType = record.documentType;
              that.projectMessage.modelList.forEach(element => {
                element.id != record.id ? element.isMain = false : element.isMain = true;
              });
              that.tableData = that.projectMessage.modelList;
              that.$message.success('设置成功');
            },
            onCancel() {
              that.projectMessage.modelList.forEach(element => {
                element.id == record.id ? element.isMain = false : null;
              });
            },
          })
        }
      },
      ChangeVisibleDistance(data) {
        const that = this;
        if (data.modelConfig.visibleDistance) {
          api.Model.setVisualRange(data.visibleDistance, data.id);
          if (that.projectMessage.sceneType == 0) {
            setModelConfig(that.projectMessage.id, JSON.stringify(data.modelConfig)).then(res => {
              that.$message.success('可视距离保存成功');
            }).catch(res => {
              that.$message.success('可视距离保存失败');
            })
          } else {
            combineModelConfig({
              id: that.projectMessage.id,
              DocumentId: data.documentId,
              ModelConfig: JSON.stringify(data.modelConfig),
            }).then(res => {

            })
          }
        }
      }
    },
  }
</script>

<style lang="less" scoped>
  .layer-box {
    /deep/.ant-table {
      .ant-table-thead {
        background: rgba(5, 160, 129, 0.7);
      }

      .ant-table-thead>tr>th {
        background-color: transparent;
        color: #ffffff;
      }

      .ant-input-number {
        .ant-input-number-handler-wrap {
          display: none;
        }

        .ant-input-number-input {
          text-align: center;
        }
      }
    }

  }

  /deep/.showColumns {
    display: none;
  }
</style>