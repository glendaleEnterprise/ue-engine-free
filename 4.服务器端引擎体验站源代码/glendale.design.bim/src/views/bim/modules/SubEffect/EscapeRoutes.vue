<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateEscapeRoutes" @keyup.prevent @keydown.enter.prevent>新建逃生路线</a-button>
        <a-button ghost v-if="isMobileOver" @click="MobileOver">结束取点</a-button>
      </a-space>
      <a-list size="small" :data-source="escapeRoutesList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.title }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DeleteEscapeRoutes(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="逃生路线设置" :width="320" :visible="visible" @ok="SaveEscapeRoutes" @cancel="handleCancel"
      cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :destroyOnClose="false"
      :maskClosable="false" :mask="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="线条名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入逃生路线名称"
            v-decorator="['title', { rules: [{ required: true, message: '请输入逃生路线名称' }] }]" />
        </a-form-item>
        <a-form-item label="自发光强度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入自发光强度" :min="0.1"
            v-decorator="['intensity', { rules: [{ required: true, message: '请输入自发光强度' }], initialValue: 1 }]" />
        </a-form-item>
        <a-form-item label="移动速度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入移动速度"
            v-decorator="['speed', { rules: [{ required: true, message: '请输入移动速度' }], initialValue: 1 }]"
            :formatter="(value) => `${value}米/秒`" :parser="(value) => value.replace('米/秒', '')" :min="0" />
        </a-form-item>
        <a-form-item label="线条颜色" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
          <a-input disabled v-model="colorRgba" />
          <colorPicker v-model="color" show-alpha @change="ColorPickerChange"></colorPicker>
        </a-form-item>
        <a-form-item label="移动方向" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-switch checked-children="正向" un-checked-children="反向"
            v-decorator="['direction', { valuePropName: 'checked', initialValue: false }]" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>
<script>
  import store from '@/store'
  import {
    colorHextoRGB,
    genlabID,
    _isMobile
  } from '@/api/public'
  export default {
    name: 'PointSourse',
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
        escapeRoutesList: [], //热力线列表
        color: 'rgba(255,255,0,1)',
        colorRgba: 'rgba(255,255,0,1)',
        motionTrailList: [], //轨迹列表
        isMobile: false,
        isMobileOver: false,
      }
    },
    mounted() {
      const that = this;
      this.isMobile = _isMobile() ? true : false;
      that.escapeRoutesList = store.state.bim.escapeRoutesList;
    },
    methods: {
      CreateEscapeRoutes() {
        const that = this;
        that.form.resetFields() //清空表单
        that.motionTrailList = []
        api.Public.clearAllDrawObject()
        that.$message.info(that.isMobile ? '请在场景中点击拾取逃生路线轨迹点' : '请在场景中点击拾取逃生路线轨迹点，右键结束取点')
        that.isMobileOver = that.isMobile ? true : false;
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            data[2] = data[2] + 3
            that.motionTrailList.push(data)
            api.Public.drawPoint({
              "position": data,
              "size": "5",
              "persistent": "1",
              "color": "rgba(255,255,0,1)"
            });
          }
        })
        api.Public.event("RIGHT_CLICK", function (e) {
          if (that.motionTrailList.length > 1) {
            that.visible = true
            api.Public.clearHandler("RIGHT_CLICK");
            store.dispatch('GetObtainCoordinates', {
              clickStatus: false
            })
          } else {
            that.$message.info('至少需要两个点，形成一段完整路径')
          }
        });
      },
      MobileOver() {
        const that = this;
        that.isMobileOver = false;
        if (that.motionTrailList.length > 1) {
          that.visible = true
          store.dispatch('GetObtainCoordinates', {
            clickStatus: false
          })
        } else {
          that.$message.info('至少需要两个点，形成一段完整路径')
        }
      },
      SaveEscapeRoutes() {
        const that = this
        api.Public.clearAllDrawObject()
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        }) //终止拾取坐标事件
        if (that.motionTrailList.length < 2) {
          that.$message.warning('至少选择两个轨迹点!')
          return
        }
        that.form.validateFields((err, values) => {
          if (!err) {
            const params = {
              "ID": "escapeRoutes" + genlabID(6),
              "Positions": that.motionTrailList,
              "Intensity": values.intensity,
              "Color": that.colorRgba,
              "title": values.title,
              "Speed": values.direction ? values.speed : -values.speed,
              "Scale": 5,
              "Density": 0.3
            }
            that.escapeRoutesList.push(params)
            api.Plugin.drawEscapeArrow(params)
            that.visible = false;
          }
        })
      },
      DeleteEscapeRoutes(item, index) {
        const that = this
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要删除逃生路线 “${item.title}” 吗？`,
          onOk() {
            api.Plugin.deleteEscapeArrow(item.ID);
            that.escapeRoutesList.splice(index, 1)
            that.$message.success('删除成功！')
          },
        })
      },
      handleCancel() {
        this.visible = false
      },
      ColorPickerChange() {
        const that = this;
        that.colorRgba = colorHextoRGB(that.color)
      }
    },
    destroyed() {
      const that = this
      store.dispatch('GetObtainCoordinates', {
        clickStatus: false
      }); //终止拾取坐标事件 
      store.dispatch('GetEscapeRoutesList', that.escapeRoutesList)
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
  }

  .motionTrail-list {
    height: 12vh !important;
    margin: 0 13px;
  }
</style>