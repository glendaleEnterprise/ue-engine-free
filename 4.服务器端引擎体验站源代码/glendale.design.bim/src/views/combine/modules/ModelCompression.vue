<template>
  <div>
    <a-tabs default-active-key="1">
      <a-tab-pane key="1" tab="合模压平" :forceRender="true">
        <a-form-model ref="ruleForm" layout="horizontal" :model="form" :rules="rules" :label-col="{ span: 8 }"
          labelAlign="right" :wrapper-col="{ span: 15 }">
          <a-form-model-item label="压平名称" prop="name">
            <a-input v-model="form.name" />
          </a-form-model-item>
          <a-form-model-item label="压平高度" prop="height">
            <a-input v-model="form.height" type="number" />
          </a-form-model-item>
          <a-form-model-item label="压平范围" prop="content">
            <a-button ghost @click="initPickupCoordinate"> 添加轨迹点 </a-button>
          </a-form-model-item>
          <div class="points">
            <div class="points-line" v-for="(item, i) in posArr" :key="i">
              <span>坐标点{{ i + 1 }}</span>
              <a-button size="small" type="link" ghost icon="delete" @click="removeViewPort(i)"></a-button>
            </div>
          </div>
          <a-form-model-item :wrapper-col="{ span: 24 }" class="btn-style">
            <a-button type="primary" @click="Preview">
              预览压平
            </a-button>
            <a-button type="primary" @click="addLists">
              保存压平
            </a-button>
            <a-button type="primary" @click="delFlatten">
              删除压平
            </a-button>
          </a-form-model-item>
        </a-form-model>
      </a-tab-pane>
      <a-tab-pane key="2" tab="压平记录" :forceRender="true">
        <a-table :data-source="lists" :columns="columns" size="small" :pagination="false"
          :locale="{ emptyText: '暂无数据' }"  class="scroll-box">
          <template slot="action" slot-scope="text, record, index">
            <div class="tools">
              <a-switch @click="flattenStateChange($event, record)" :defaultChecked="record.isCheck" size="small"
                style="margin-right:10px;" />
              <a-popconfirm v-if="lists.length" title="是否删除" @confirm="() => deleteList(record, index)" okText="确定"
                cancelText="取消">
                <a-icon type="delete" :style="{ fontSize: '16px', color: '#05a081' }" />
              </a-popconfirm>
            </div>
          </template>
        </a-table>
      </a-tab-pane>
    </a-tabs>
  </div>
</template>

<script>
  import {
    eventBus
  } from '@/utils/bus'
  export default {
    name: 'ModelCompression',
    props: {
      modelArrCopy: {
        type: Array,
        default: () => {
          return []
        },
      },
    },
    data() {
      return {
        flattenArray: [],
        posArr: [],
        tabKey: 1,
        switchState: false,
        form: {
          name: '',
          height: -10,
          content: '请在模型上点选三个以上的坐标位置'
        },
        columns: [{
            title: '压平名称',
            dataIndex: 'name',
            ellipsis: true,
            scopedSlots: {
              customRender: 'name'
            },
          },
          {
            title: '压平高度',
            dataIndex: 'height',
            ellipsis: true
          },
          {
            title: '操作',
            dataIndex: 'action',
            ellipsis: true,
            width: 80,
            scopedSlots: {
              customRender: 'action'
            },
          },
        ],
        rules: {
          name: [{
            required: true,
            message: '请输入模型压平名称'
          }],
          height: [{
            required: true,
            message: '请输入模型压平高度'
          }],
          content: [{
            required: true,
            message: '请点选三个以上的坐标位置'
          }],
        },
        lists: [],
        currentId: undefined
      }
    },
    watch: {
      lists: {
        handler(newV, oldV) {
          eventBus.$emit('giveFlattenList', newV)
        },
        deep: true
      }
    },
    mounted() {},
    methods: {
      //压平轨迹点选择
      initPickupCoordinate() {
        const that = this
        this.$message.info('请在场景中点击拾取三个或以上点作为压平范围')
        api.Public.pickupCoordinate(true, function (data) {
          that.posArr.push(data)
          api.Public.drawPoint({
            "position": data,
            "size": "5",
            "persistent": "1",
            "color": "rgba(255,255,0,1)"
          });
        })
      },
      // 删除轨迹点
      removeViewPort(i) {
        this.posArr.splice(i, 1)
      },
      // 预览压平效果
      Preview() {
        api.Public.clearAllDrawObject()
        const {
          height
        } = this.form
        if (height == '') {
          this.$message.info('高度不能为空！')
          return
        }
        if (this.posArr.length < 3) {
          this.$message.info('至少选择三个点！')
          return
        }
        // 第一次压平
        if (this.currentId == undefined) {
          this.modelArrCopy.forEach(item => {
            const id = this.getUuid() + '@' + item.id
            if (item.documentType == 2) { // osgb
              const opt = {
                id: id,
                positions: this.posArr,
                height: parseInt(height),
              }
              api.Model.addFlatten(item.id, opt)
              this.currentId = id;
            }
          })
        } else {
          // 再次调整高度
          const model = this.currentId.split('@')
          const opt = {
            id: this.currentId,
            positions: this.posArr,
            height: parseInt(height),
          }
          api.Model.addFlatten(model[1], opt)
        }
      },
      // 压平显示状态
      flattenStateChange(checked, option) {
        const {
          id,
          height,
          positions
        } = option
        if (checked) {
          const model = id.split('@')
          const opt = {
            id: id,
            positions: JSON.parse(positions),
            height: parseInt(height),
          }
          api.Model.addFlatten(model[1], opt)
        } else {
          const modelId = id.split('@')[1]
          api.Model.removeFlatten(modelId, id)
        }
      },
      // 删除压平
      deleteList(option, index) {
        api.Public.clearAllDrawObject()
        // 删除列
        this.lists.splice(index, 1)
        // 删除压平
        const modelId = option.id.split('@')[1]
        api.Model.removeFlatten(modelId, option.id)
      },
      // 保存压平
      addLists() {
        api.Public.clearAllDrawObject()
        this.$refs.ruleForm.validate(valid => {
          if (valid && this.posArr.length > 2) {
            const {
              name,
              height
            } = this.form
            this.lists.push({
              id: this.currentId ? this.currentId : this.getflattenId(),
              isCheck: this.currentId ? true : false,
              name: name,
              height: height,
              positions: JSON.stringify(this.posArr)
            })
            this.form.name = ''
            this.form.height = -10
            this.posArr = []
            this.currentId = undefined
          }
        })
      },
      // 删除压平
      delFlatten() {
        api.Public.clearAllDrawObject()
        // 删除压平
        if (this.currentId) {
          const modelId = this.currentId.split('@')[1]
          api.Model.removeFlatten(modelId, this.currentId)
          this.form.name = ''
          this.form.height = -10
          this.posArr = []
          this.currentId = undefined
        } else {
          this.$message.warning('请先添加效果或在压平记录中删除！')
        }
      },
      // 当不预览压平直接保存时
      getflattenId() {
        let id = ''
        this.modelArrCopy.forEach(item => {
          if (item.documentType == 2) { // osgb
            id = this.getUuid() + '@' + item.id
          }
        })
        return id
      },
      getUuid() {
        return 'xxxx-4xxx'.replace(/[xy]/g, function (c) {
          const r = Math.random() * 9 | 0
          const v = c === 'x' ? r : (r & 0x3 | 0x8)
          return v.toString(9)
        })
      }
    },
    destroyed() {
      api.Public.clearAllDrawObject()
    }
  }
</script>
<style scoped lang='less'>
  /deep/ .ant-table-row-cell-ellipsis {
    color: white;
  }

  /deep/ .ant-table-placeholder {
    background: transparent;
    color: #ffffff;
  }

  .tools {
    display: flex;
    align-items: center;
  }

  .points {
    border: 1px solid #ffffff;
    height: 10vh;
    margin: 0.3rem 5% 0.5rem;
    color: white;
    width: 90%;
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;
    align-content: flex-start;
    overflow: auto;

    &::-webkit-scrollbar {
      /*滚动条整体样式*/
      width: 2px;
      /*高宽分别对应横竖滚动条的尺寸*/
      height: 1px;
    }

    &::-webkit-scrollbar-thumb {
      /*滚动条里面小方块*/
      border-radius: 2px;
      -webkit-box-shadow: inset 0 0 5px #0EF1E9;
      background: #11ddda;
      scrollbar-arrow-color: #11ddda;
    }

    &::-webkit-scrollbar-track {
      /*滚动条里面轨道*/
      -webkit-box-shadow: inset 0 0 3px rgba(148, 202, 255, 0.2);
      border-radius: 0;
      background: rgba(148, 202, 255, 0.2);
    }

    .points-line {
      width: 46%;
      margin: 5px 2% 0 2%;
      display: flex;
      justify-content: space-around;
      border-bottom: 1px dotted #eee;

      &:hover {
        background: rgba(255, 255, 255, 0.1);
      }
    }
  }

  .btn-style {
    /deep/.ant-form-item-children {
      display: flex;
      justify-content: space-around;

      .ant-btn {
        background-color: transparent;
        border-color: #ffffff;
        padding: 0 10px;
      }
    }
  }
</style>