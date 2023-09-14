<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateUnitization" @keyup.prevent @keydown.enter.prevent>新建单体化</a-button>
        <a-button ghost v-if="isMobileOver" @click="MobileOver">结束取点</a-button>
      </a-space>
      <a-list size="small" :data-source="unitizationList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.name }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelUnitization(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="新建单体化" :width="320" :visible="visible" @ok="SaveUnitization" @cancel="handleCancel" cancel-text="取消"
      ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :destroyOnClose="true" :maskClosable="false"
      :mask="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="单体化名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入单体化名称"
            v-decorator="['title', { rules: [{ required: true, message: '请输入单体化名称' }] }]" />
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
    name: 'Unitization',
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
        unitizationList: [],
        setPosition: [],
        modelId: '',
        isNew: false,
        isMobile: false,
        isMobileOver: false,
      }
    },
    mounted() {
      const that = this
      this.isMobile = _isMobile() ? true : false;
      that.unitizationList = store.state.bim.unitizationList;
      that.$notification.open({
        key: 'EngineKey',
        description: '左键点击已单体化范围，拾取单体化结果',
        class: 'original-notification tips-notification',
        duration: null,
        placement: 'bottomRight',
      })
      store.dispatch('GetObtainCoordinates', {
        clickStatus: true,
        callback: (data) => {
          api.Model.pickPolygonTint(data, false, function (data) {});
        }
      })
    },
    methods: {
      CreateUnitization() {
        const that = this;
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        })
        that.form.resetFields() //清空表单
        that.setPosition = [];
        that.$message.info(that.isMobile ? '请点击拾取单体化范围' : '请点击拾取单体化范围，右键结束取点')
        that.isMobileOver = that.isMobile ? true : false;
        that.isNew = true
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            if (that.isNew) {
              that.setPosition.push(data)
              api.Public.drawPoint({
                "position": data,
                "size": "5",
                "persistent": "1",
                "color": "rgba(255,255,0,1)"
              });
              if (that.modelId == "") {
                api.Feature.getByEvent(true, (featureId) => {
                  if (featureId.split("^")[1]) {
                    that.projectMessage.modelList.forEach((item) => {
                      if (item.id == featureId.split("^")[0] && item.modelFormat == "osgb") {
                        that.modelId = featureId.split("^")[0]
                      }
                    })
                    api.Feature.getByEvent(false);
                  }
                });
              }
            }
            api.Model.pickPolygonTint(data, false, function (data) {});
          }
        })
        api.Public.event("RIGHT_CLICK", function (e) {
          //执行事件
          if (that.setPosition.length > 2 && that.isNew) {
            that.visible = true;
            that.isNew = false;
            store.dispatch('GetObtainCoordinates', {
              clickStatus: false
            })
            store.dispatch('GetObtainCoordinates', {
              clickStatus: true,
              callback: (data) => {
                api.Model.pickPolygonTint(data, false, function (data) {});
              }
            })
          } else if (!that.isNew) {
            that.$message.info('请点击新建按钮再次创建')
          } else {
            that.$message.info('请拾取三个及以上点形成单体化范围，右键结束取点')
          }
        });
      },
      MobileOver() {
        const that = this;
        that.isMobileOver = false;
        if (that.setPosition.length > 2 && that.isNew) {
            that.visible = true;
            that.isNew = false;
            store.dispatch('GetObtainCoordinates', {
              clickStatus: false
            })
            store.dispatch('GetObtainCoordinates', {
              clickStatus: true,
              callback: (data) => {
                api.Model.pickPolygonTint(data, false, function (data) {});
              }
            })
          } else if (!that.isNew) {
            that.$message.info('请点击新建按钮再次创建')
          } else {
            that.$message.info('请拾取三个及以上点形成单体化范围，右键结束取点')
          }
      },
      SaveUnitization() {
        const that = this
        api.Public.clearAllDrawObject()
        that.form.validateFields((err, values) => {
          if (!err) {
            var opt = {
              id: "unitization" + genlabID(6),
              positions: that.setPosition,
              name: values.title,
              modelId: that.modelId ? that.modelId : that.projectMessage.modelId,
            }
            that.unitizationList.push(opt)
            api.Model.addPolygonTint(that.modelId ? that.modelId : that.projectMessage.modelId, opt);
            that.setPosition = []
            that.visible = false;
          }
        })
      },
      DelUnitization(item, index) {
        const that = this
        that.unitizationList.splice(index, 1)
        api.Model.removePolygonTint(that.modelId ? that.modelId : that.projectMessage.modelId, item.id);
        that.$message.success('删除成功')
      },
      handleCancel() {
        this.visible = false;
      },
    },
    destroyed() {
      const that = this
      store.dispatch('GetUnitizationList', that.unitizationList);
      store.dispatch('GetObtainCoordinates', {
        clickStatus: false
      })
      api.Model.pickPolygonTint([999999, 999999, 999999], false);
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
  }
</style>