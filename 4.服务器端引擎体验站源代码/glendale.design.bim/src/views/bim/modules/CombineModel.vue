<template>
  <div>
    <a-tabs default-active-key="1" @change="callback">
      <a-tab-pane key="1" tab="合模压平" :forceRender="true">
        <a-form-model ref="ruleForm" layout="horizontal" :model="form" :rules="rules" :label-col="{ span: 7 }"
          labelAlign="right" :wrapper-col="{ span: 16 }">
          <a-form-model-item label="压平名称" prop="name">
            <a-input v-model="form.name" />
          </a-form-model-item>
          <a-form-model-item label="压平高度" prop="height">
            <a-input v-model="form.height" type="number" />
          </a-form-model-item>
          <a-form-model-item label="压平范围" prop="content">
            <p style="color:white;margin-bottom: 0;height: 36px;line-height: 24px;">请在模型上点选三个以上的坐标位置</p>
          </a-form-model-item>
          <div class="points">
            <div class="points-line" v-for="(item, i) in posArr" :key="i">
              <span>坐标点{{ i + 1 }}</span>
              <a-button size="small" type="link" ghost icon="delete" @click="removeViewPort(i)" @keyup.prevent
                @keydown.enter.prevent></a-button>
            </div>
          </div>
          <a-form-model-item>
            <a-button size="small" style="margin:0 20px 0 30px;" @click="Preview" @keyup.prevent @keydown.enter.prevent>
              预览压平
            </a-button>
            <a-button type="primary" size="small" @click="addLists" @keyup.prevent @keydown.enter.prevent>
              保存压平
            </a-button>
          </a-form-model-item>
        </a-form-model>
      </a-tab-pane>
      <a-tab-pane key="2" tab="压平记录" :forceRender="true">
        <a-table :data-source="lists" :columns="columns" size="small" :pagination="false">
          <template slot="action" slot-scope="text, record">
            <div class="tools">
              <a-switch @click="onChange($event, record)" size="small" style="margin-right:10px;" />
              <a-popconfirm v-if="lists.length" title="是否删除" @confirm="() => deleteList(record)">
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
  import {
    getCombineFlatten,
    addCombineFlatten,
    deleteCombineFlatten
  } from '@/api/combine'
  export default {
    name: 'CombineModel',
    props: {
      modelArrCopy: {
        type: Array,
        default: () => [],
      },
      combineID: {
        type: String,
        default: () => ''
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
            dataIndex: 'flattenName',
            ellipsis: true,
          },
          {
            title: '压平高度',
            dataIndex: 'flattenHeight',
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
        idEnum: {}
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
    mounted() {
      if (this.tabKey == 1) {
        this.initPickupCoordinate()
      }
      // this.combineID
      this.combineflatten()
    },
    methods: {
      // 获取合模压平列表
      combineflatten() {
        const data = {
          CombineId: this.combineID,
          MaxResultCount: 1000,
          SkipCount: 0
        }
        getCombineFlatten(data).then(res => {
          const {
            items
          } = res
          this.lists = items
        })
      },
      // 压平开关
      onChange(checked, option) {
        const {
          flattenHeight,
          flattenScope
        } = option
        if (checked) {
          this.modelArrCopy.forEach(item => {
            const id = this.getUuid() + '@' + item.id
            if (item.documentType == 2) { // osgb
              const opt = {
                id: id,
                positions: JSON.parse(flattenScope),
                height: parseInt(flattenHeight),
              }
              this.$sapi.Model.addFlatten(item.id, opt)
              this.idEnum[option.id] = id
            }
          })
        } else {
          const modelId = this.idEnum[option.id].split('@')[1]
          this.$sapi.Model.removeFlatten(modelId, this.idEnum[option.id])
        }
      },
      // 删除压平
      deleteList(option) {
        if (this.idEnum[option.id]) {
          const modelId = this.idEnum[option.id].split('@')[1]
          this.$sapi.Model.removeFlatten(modelId, this.idEnum[option.id])
          this.$delete(this.idEnum, `${option.id}`)
        }
        deleteCombineFlatten(option.id).then(res => {
          this.combineflatten()
        })
      },
      // 删除坐标点
      removeViewPort(i) {
        this.posArr.splice(i, 1)
      },
      // 预览压平效果
      Preview() {
        const {
          height
        } = this.form
        if (height == '') {
          this.$message.info('高度不能为空！')
          return
        }
        if (this.posArr.length < 3) {
          this.$message.info('至少点击三个坐标位置！')
          return
        }
        // 第一次压平
        if (this.flattenArray.length == 0) {
          this.modelArrCopy.forEach(item => {
            const id = this.getUuid() + '@' + item.id
            if (item.documentType == 2) { // osgb
              const opt = {
                id: id,
                positions: this.posArr,
                height: parseInt(height),
              }
              this.$sapi.Model.addFlatten(item.id, opt)
              this.flattenArray.push(id)
            }
          })
        } else {
          // 再次调整高度
          this.flattenArray.forEach(item => {
            const model = item.split('@')
            const opt = {
              id: item,
              positions: this.posArr,
              height: parseInt(height),
            }
            this.$sapi.Model.addFlatten(model[1], opt)
          })
        }
      },
      // 保存压平
      addLists() {
        this.$refs.ruleForm.validate(valid => {
          if (valid && this.posArr.length > 2) {
            const {
              name,
              height
            } = this.form
            const data = {
              combineId: this.combineID,
              flattenName: name,
              flattenHeight: height,
              flattenScope: JSON.stringify(this.posArr)
            }
            addCombineFlatten(data).then(res => {
              this.form.name = ''
              this.form.height = -10
              this.posArr = []
              this.flattenArray = []
              this.combineflatten()
            })

          } else {
            return
          }
        })
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
      // 初始化压平
      initPickupCoordinate() {
        const that = this
        this.$message.info('请点击选择三个及以上测量点')
        this.$sapi.Public.pickupCoordinate(true, function (data) {
          that.posArr.push(data)
          that.$sapi.Public.drawPoint({
            position: data,
            size: '5',
            persistent: '0',
            time: '1',
            color: 'rgba(255,255,0,1)'
          })
        })
      },
      // tab页切换函数
      callback(key) {
        this.tabKey = key
        if (key == 1) {
          this.initPickupCoordinate()
        } else {
          this.$sapi.Public.clearHandler()
        }
      },
      getUuid() {
        return 'xxxx-4xxx'.replace(/[xy]/g, function (c) {
          const r = Math.random() * 9 | 0
          const v = c === 'x' ? r : (r & 0x3 | 0x8)
          return v.toString(9)
        })
      }
    }
  }
</script>
<style scoped lang='less'>
  /deep/ .ant-table-row-cell-ellipsis {
    color: white;
  }

  /deep/ .ant-table-placeholder {
    background: transparent;
  }

  .tools {
    display: flex;
    align-items: center;
  }

  .points {
    border: 1px solid #1c6bb6;
    height: 10vh;
    margin-bottom: 0.5rem;
    color: white;
    width: 100%;
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
</style>