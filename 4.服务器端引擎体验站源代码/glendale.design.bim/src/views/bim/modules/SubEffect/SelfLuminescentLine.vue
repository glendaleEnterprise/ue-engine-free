<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateSelfLuminescentLine" @keyup.prevent @keydown.enter.prevent>新建自发光线条</a-button>
        <a-button ghost v-if="isMobileOver" @click="MobileOver">结束取点</a-button>
      </a-space>
      <a-list size="small" :data-source="selfLuminescentLineList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.title }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DeleteSelfLuminescentLine(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="自发光线条设置" :width="320" :visible="visible" @ok="SaveSelfLuminescentLine" @cancel="handleCancel"
      cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :destroyOnClose="false"
      :maskClosable="false" :mask="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="线条名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入自发光线条名称"
            v-decorator="['title', { rules: [{ required: true, message: '请输入自发光线条名称' }] }]" />
        </a-form-item>
        <a-form-item label="自发光强度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入自发光强度"
            v-decorator="['intensity', { rules: [{ required: true, message: '请输入自发光强度' }], initialValue: 1 }]"
            :min="0" />
        </a-form-item>
        <a-form-item label="线条宽度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入线条宽度"
            v-decorator="['size', { rules: [{ required: true, message: '请输入线条宽度' }], initialValue: 0.1 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" :min="0" />
        </a-form-item>
        <a-form-item label="线条颜色" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
          <a-input disabled v-model="colorRgba" />
          <colorPicker v-model="color" show-alpha @change="ColorPickerChange"></colorPicker>
        </a-form-item>
        <!-- <a-form-item label="轨迹创建" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-button ghost @click="GetMotionTrail"> 添加轨迹点 </a-button>
        </a-form-item>
        <div>
          <a-list size="small" :data-source="motionTrailList" bordered class="motionTrail-list scroll-box"
            :locale="{ emptyText: `暂无数据，至少添加两个轨迹点！` }">
            <a-list-item slot="renderItem" slot-scope="item,index">
              {{ '轨迹点' + (++index) }}
              <a-space slot="actions">
                <a-tooltip title="删除">
                  <a-icon type="delete" :style="{ fontSize: '16px', color: '#05a081' }"
                    @click="DelMotionTrail(index)" />
                </a-tooltip>
              </a-space>
            </a-list-item>
          </a-list>
        </div> -->
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
        selfLuminescentLineList: [], //热力线列表
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
      that.selfLuminescentLineList = store.state.bim.selfLuminescentLineList;
    },
    methods: {
      CreateSelfLuminescentLine() {
        const that = this;
        that.form.resetFields() //清空表单
        that.motionTrailList = []
        that.$message.info(that.isMobile ? '请在场景中点击拾取关键点' : '请在场景中点击拾取关键点，右键结束取点')
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
          that.visible = true
          api.Public.clearHandler("RIGHT_CLICK");
          store.dispatch('GetObtainCoordinates', {
            clickStatus: false
          })
        });
      },
      MobileOver() {
        const that = this;
        that.isMobileOver = false;
        that.visible = true
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        })
      },
      // //添加轨迹点
      // GetMotionTrail() {
      //   const that = this;
      //   api.Feature.getByEvent(false); //关闭构件拾取事件
      //   that.visible = false

      // },
      // //删除轨迹点
      // DelMotionTrail(i) {
      //   this.motionTrailList.splice(--i, 1);
      // },
      SaveSelfLuminescentLine() {
        const that = this
        api.Public.clearAllDrawObject()
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        }); //终止拾取坐标事件
        if (that.motionTrailList.length < 2) {
          that.$message.warning('至少选择两个轨迹点!')
          return
        }
        that.form.validateFields((err, values) => {
          if (!err) {
            const params = {
              "ID": "selfLuminescentLine" + genlabID(6),
              "Positions": that.motionTrailList,
              "Intensity": values.intensity,
              "Color": that.colorRgba,
              "Scale": values.size,
              "title": values.title
            }
            that.selfLuminescentLineList.push(params)
            api.Plugin.addSelfLuminousLines(params)
            that.visible = false;
          }
        })
      },
      DeleteSelfLuminescentLine(item, index) {
        const that = this
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要删除自发光线条 “${item.title}” 吗？`,
          onOk() {
            api.Plugin.deleteSelfLuminousLines(item.ID);
            that.selfLuminescentLineList.splice(index, 1)
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
      store.dispatch('GetSelfLuminescentLineList', that.selfLuminescentLineList)
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