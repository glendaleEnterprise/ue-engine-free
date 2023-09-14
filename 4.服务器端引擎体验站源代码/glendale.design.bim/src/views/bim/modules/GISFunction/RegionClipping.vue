<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateCropping" @keyup.prevent @keydown.enter.prevent>新建裁剪</a-button>
        <a-button ghost v-if="isMobileOver" @click="MobileOver">结束取点</a-button>
      </a-space>
      <a-list size="small" :data-source="croppingList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.name }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelCropping(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="新建裁剪" :width="320" :visible="visible" @ok="SaveCropping" @cancel="handleCancel" cancel-text="取消"
      ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :destroyOnClose="true" :maskClosable="false"
      :mask="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="裁剪名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入裁剪名称" v-decorator="['title', { rules: [{ required: true, message: '请输入裁剪名称' }] }]" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>
<script>
  import store from '@/store'
  import {
    genlabID,
    _isMobile
  } from '@/api/public'
  export default {
    name: 'Clipping',
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        visible: false,
        form: this.$form.createForm(this, {
          name: 'coordinated'
        }),
        croppingList: [],
        setPosition: [],
        modelId: '',
        isMobile: false,
        isMobileOver: false,
      }
    },
    mounted() {
      const that = this
      this.isMobile = _isMobile() ? true : false;
      that.croppingList = store.state.bim.croppingList;
    },
    methods: {
      CreateCropping() {
        const that = this;
        that.form.resetFields() //清空表单
        that.setPosition = [];
        that.$message.info(that.isMobile ? '请点击拾取裁剪范围' : '请点击拾取裁剪范围，右键结束取点')
        that.isMobileOver = that.isMobile ? true : false;
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            that.setPosition.push(data)
            api.Public.drawPoint({
              "position": data,
              "size": "5",
              "persistent": "1",
              "color": "rgba(255,255,0,1)"
            });
            if (that.modelId == "") {
              api.Feature.getByEvent(true, (featureId) => {
                if (featureId.split("^").length > 0) {
                  that.projectMessage.modelList.forEach((item) => {
                    if (item.id == featureId.split("^")[0] && item.documentType == 2) {
                      that.modelId = featureId.split("^")[0]
                    }
                  })
                  api.Feature.getByEvent(false);
                }
              });
            }
            api.Public.event("RIGHT_CLICK", function (e) {
              //执行事件
              if (that.setPosition.length > 2) {
                that.visible = true;
                store.dispatch('GetObtainCoordinates', {
                  clickStatus: false
                })
                api.Public.clearHandler("RIGHT_CLICK");
              } else {
                that.$message.info('请拾取三个及以上点形成裁剪范围，右键结束取点')
              }
            });
          }
        })
      },
      MobileOver() {
        const that = this;
        that.isMobileOver = false;
        if (that.setPosition.length > 2) {
          that.visible = true;
          store.dispatch('GetObtainCoordinates', {
            clickStatus: false
          })
          api.Public.clearHandler("RIGHT_CLICK");
        } else {
          that.$message.info('请拾取三个及以上点形成裁剪范围，右键结束取点')
        }
      },
      SaveCropping() {
        const that = this
        api.Public.clearAllDrawObject()
        that.form.validateFields((err, values) => {
          if (!err) {
            var opt = {
              id: "clip" + genlabID(6),
              positions: that.setPosition,
              name: values.title,
              modelId: that.modelId ? that.modelId : that.projectMessage.modelId,
            }
            that.croppingList.push(opt)
            api.Model.addCutPart(that.modelId ? that.modelId : that.projectMessage.modelId, opt);
            that.setPosition = []
            that.visible = false;
          }
        })
      },
      DelCropping(item, index) {
        const that = this
        that.croppingList.splice(index, 1)
        api.Model.removeCutPart(item.modelId, item.id);
        that.$message.success('删除成功')
      },
      handleCancel() {
        this.visible = false
      },
    },
    destroyed() {
      const that = this
      store.dispatch('GetCroppingList', that.croppingList)
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
  }
</style>