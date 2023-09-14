<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateElectronicFence" @keyup.prevent @keydown.enter.prevent>新建电子围栏</a-button>
        <a-button ghost v-if="isMobileOver" @click="MobileOver">结束取点</a-button>
      </a-space>
      <a-list size="small" :data-source="electronicFenceList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.title }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DeleteElectronicFence(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="电子围栏设置" :width="320" :visible="visible" @ok="SaveElectronicFence" @cancel="handleCancel"
      cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :destroyOnClose="false"
      :maskClosable="false" :mask="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="围栏名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入电子围栏名称"
            v-decorator="['title', { rules: [{ required: true, message: '请输入电子围栏名称' }] }]" />
        </a-form-item>
        <a-form-item label="围栏高度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入围栏高度"
            v-decorator="['height', { rules: [{ required: true, message: '请输入围栏高度' }], initialValue: 5 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" :min="0" />
        </a-form-item>
        <a-form-item label="围栏颜色" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
          <a-input disabled v-model="colorRgba" />
          <colorPicker v-model="color" show-alpha @change="ColorPickerChange"></colorPicker>
        </a-form-item>
        <!-- <a-form-item label="围栏区域" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-button ghost @click="GetMotionTrail"> 添加坐标点 </a-button>
        </a-form-item>
        <div>
          <a-list size="small" :data-source="motionTrailList" bordered class="motionTrail-list scroll-box"
            :locale="{ emptyText: `暂无数据，至少添加两个坐标点！` }">
            <a-list-item slot="renderItem" slot-scope="item,index">
              {{ '位置' + (++index) }}
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
        electronicFenceList: [], //热力线列表
        color: 'rgba(255,255,0,1)',
        colorRgba: 'rgba(255,255,0,1)',
        motionTrailList: [], //轨迹列表
        motionTrailListTime: [],
        isMobile: false,
        isMobileOver: false,
      }
    },
    mounted() {
      const that = this;
      this.isMobile = _isMobile() ? true : false;
      that.electronicFenceList = store.state.bim.electronicFenceList;
    },
    methods: {
      CreateElectronicFence() {
        const that = this;
        that.form.resetFields() //清空表单
        that.motionTrailList = []
        that.$message.info(that.isMobile ? '请在场景中点击拾取围栏关键点' : '请在场景中点击拾取围栏关键点，右键结束取点')
        that.isMobileOver = that.isMobile ? true : false;
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            api.Public.convertWorldToCartographicLocation(data, (position) => {
              that.motionTrailList.push(position)
            })
            data[2] = data[2] + 1
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
          store.dispatch('GetObtainCoordinates', {
            clickStatus: false
          })
          api.Public.clearHandler("RIGHT_CLICK");
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
      //   that.visible = false

      // },
      // //删除轨迹点
      // DelMotionTrail(i) {
      //   this.motionTrailList.splice(--i, 1);
      // },
      SaveElectronicFence() {
        const that = this
        api.Public.clearAllDrawObject()
        if (that.motionTrailList.length < 2) {
          that.$message.warning('至少选择两个轨迹点!')
          return
        }
        that.form.validateFields((err, values) => {
          if (!err) {
            that.changePosition(values.height / 2, (position) => {
              const params = {
                id: "electronicFence" + genlabID(6),
                positions: position,
                color: that.colorRgba,
                scale: values.height,
                title: values.title
              }
              api.Public.addWallEffect(params);
              that.electronicFenceList.push(params)
              that.visible = false;
            })
          }
        })
      },
      changePosition(height, callback) {
        const that = this;
        let positionList = []
        that.getPosition(that.motionTrailList.length, 0, positionList, height, (position) => {
          callback(position)
        })
      },
      getPosition(length, count, positionList, height, callback) {
        const that = this;
        that.motionTrailList[count][2] += height
        api.Public.convertCartographicToWorldLocation(that.motionTrailList[count], (position) => {
          positionList.push(position)
          length--;
          count++;
          if (length == 0) {
            callback(positionList)
          } else {
            that.getPosition(length, count, positionList, height, callback)
          }
        })
      },
      DeleteElectronicFence(item, index) {
        const that = this
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要删除电子围栏 “${item.title}” 吗？`,
          onOk() {
            api.Public.removeWallEffect(item.id);
            that.electronicFenceList.splice(index, 1)
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
      // store.dispatch('GetObtainCoordinates', {
      //   clickStatus: false
      // })
      store.dispatch('GetElectronicFenceList', that.electronicFenceList)
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