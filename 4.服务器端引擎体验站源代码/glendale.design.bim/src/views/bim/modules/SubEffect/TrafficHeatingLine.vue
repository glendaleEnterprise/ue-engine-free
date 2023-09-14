<template>
  <div class="side-frame" @contextmenu.prevent="" ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-tabs v-model="tab" size="small" :animated="false">
      <a-tab-pane :key="1" tab="热力线设置" class="scroll-box roam-history">
        <a-form-model class="roaming-set" ref="ruleForm" :model="form" :rules="rules" :labelCol="{ span: 9 }"
          :wrapperCol="{ span: 15 }" :hideRequiredMark="true">
          <a-form-model-item label="热力线名称" prop="name">
            <a-input v-model="form.title" style="width: 100%" />
          </a-form-model-item>
          <a-form-model-item label="热力线宽度" prop="time">
            <a-input-number v-model="form.width" :step="1" :formatter="(value) => `${value}米`" :min="0.1"
              :parser="(value) => value.replace('米', '')" style="width: 100%" />
          </a-form-model-item>
          <a-form-model-item label="轨迹创建" prop="lookFactor">
            <a-button ghost @click="GetMotionTrail" @keyup.prevent @keydown.enter.prevent> 添加轨迹点 </a-button>
          </a-form-model-item>
          <div>
            <a-list size="small" :data-source="form.motionTrailList" bordered class="viewpoint-list scroll-box"
              :locale="{ emptyText: `暂无数据，至少添加两个关键点！` }">
              <a-list-item slot="renderItem" slot-scope="item,index">
                {{ '视点' + (++index) }}
                <a-space slot="actions">
                  <a-tooltip title="关键点颜色">
                    <colorPicker v-model="item.color" show-alpha @change="ColorPickerChangeList(item, index)">
                    </colorPicker>
                    <!-- <a-icon type="delete" :style="{ fontSize: '16px', color: '#05a081' }" @click="DelViewPoint(index)" /> -->
                  </a-tooltip>
                </a-space>
              </a-list-item>
            </a-list>
          </div>
          <a-form-model-item :wrapper-col="{ span: 24 }">
            <a-space class="options-btn">
              <a-button ghost @click="SaveTrafficHeatingLine" @keyup.prevent @keydown.enter.prevent>添加热力线</a-button>
              <a-button ghost @click="StopTrafficHeatingLine" @keyup.prevent @keydown.enter.prevent>删除热力线</a-button>
            </a-space>
          </a-form-model-item>
        </a-form-model>
      </a-tab-pane>
      <a-tab-pane :key="2" tab="历史列表" class="scroll-box roam-history roam-list-scroll">
        <a-list size="small" :data-source="trailList" :locale="{ emptyText: `暂无历史数据` }" class="roam-list">
          <a-list-item slot="renderItem" slot-scope="item, index">
            {{ item.title }}
            <a-space slot="actions">
              <a-tooltip title="删除">
                <a-icon type="delete" :style="{ fontSize: '16px', color: '#05a081' }"
                  @click="DeleteTrafficHeatingLine(item, index)" />
              </a-tooltip>
            </a-space>
          </a-list-item>
        </a-list>
      </a-tab-pane>
    </a-tabs>
    <a-modal title="关键点颜色" :width="320" :visible="visibleColor" @ok="SaveColor" @cancel="handleColorCancel"
      cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :destroyOnClose="true"
      :maskClosable="false" :mask="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="关键点颜色" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
          <a-input disabled v-model="colorRgba" />
          <colorPicker v-model="color" show-alpha @change="ColorPickerChange"></colorPicker>
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
  import store from '@/store'
  import {
    setViewPort,
    getRoamingTrack,
    deleteRoamingTrack
  } from '@/api/roaming'
  import {
    colorHextoRGB,
    genlabID,
    _isMobile
  } from '@/api/public'
  export default {
    name: 'ViewPortRoam',
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        tab: 1,
        trailList: [],
        form: {
          title: '',
          width: 1,
          motionTrailList: [],
        },
        rules: {
          title: [{
            required: true,
            message: '请输入热力线名称'
          }],
          width: [{
            required: true,
            message: '请输入热力线宽度'
          }],
        },
        visibleColor: false,
        color: 'rgba(0,255,0,1)',
        colorRgba: 'rgba(0,255,0,1)',
        currentId: undefined,
        isMobile: false
      }
    },
    mounted() {
      const that = this;
      this.isMobile = _isMobile() ? true : false;
      that.trailList = store.state.bim.trafficLinesList;
    },
    methods: {
      //添加轨迹点
      GetMotionTrail() {
        const that = this;
        if (that.currentId) {
          that.form.motionTrailList = []
          that.currentId = undefined;
        }
        that.$message.info('请在场景中点击拾取关键点坐标')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            that.color = 'rgba(0,255,0,1)';
            that.colorRgba = 'rgba(0,255,0,1)';
            that.visibleColor = true
            data[2] = data[2] + 0.1
            that.form.motionTrailList.push({
              position: data,
              color: 'rgba(0,255,0,1)'
            })
            api.Public.drawPoint({
              "position": data,
              "size": "5",
              "persistent": "1",
              "color": "rgba(255,255,0,1)"
            });
          }
        })
      },
      //停止漫游
      StopTrafficHeatingLine() {
        const that = this;
        if (that.currentId) {
          api.Plugin.deleteTrafficLines(that.currentId);
          that.trailList.splice(that.trailList.length - 1, 1)
          that.$message.success('删除成功！')
          that.form = {
            motionTrailList: [],
            width: 1,
            title: ''
          }
          that.currentId = undefined;
        }
      },
      SaveTrafficHeatingLine() {
        const that = this
        api.Public.clearAllDrawObject()
        if (that.currentId) {
          api.Plugin.deleteTrafficLines(that.currentId);
        } else {
          that.currentId = "TrafficLine" + genlabID(6)
        }
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        }); //终止拾取坐标事件
        if (that.form.motionTrailList.length < 2) {
          that.$message.warning('至少选择两个关键点!')
          return
        }
        let colorList = []
        let motionTrailList = []
        that.form.motionTrailList.forEach((item) => {
          colorList.push(item.color)
          motionTrailList.push(item.position)
        })
        const params = {
          "ID": that.currentId,
          "Positions": motionTrailList,
          "PointColorIndex": [...new Array(colorList.length).keys()],
          "Color": colorList,
          "PointIndexVisibility": false,
          "Scale": that.form.width,
          "title": that.form.title
        }
        api.Plugin.addTrafficLines(params)
        let index = that.trailList.findIndex(item => item.ID == that.currentId)
        index == -1 ? that.trailList.push(params) : null
      },
      //删除漫游
      DeleteTrafficHeatingLine(item, index) {
        const that = this
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要删除交通热力线 “${item.title}” 吗？`,
          onOk() {
            if (item.ID == that.currentId) {
              that.form = {
                motionTrailList: [],
                width: 1,
                title: ''
              }
              that.currentId = undefined;
            }
            api.Plugin.deleteTrafficLines(item.ID);
            that.trailList.splice(index, 1)
            that.$message.success('删除成功！')
          },
        })
      },
      SaveColor() {
        this.form.motionTrailList[this.form.motionTrailList.length - 1].color = this.colorRgba
        this.visibleColor = false
      },
      handleColorCancel() {
        this.visibleColor = false
      },
      ColorPickerChange() {
        const that = this;
        that.colorRgba = colorHextoRGB(that.color)
      },
      ColorPickerChangeList(item, index) {
        item.color = colorHextoRGB(item.color)
      }
    },
    destroyed() {
      const that = this
      store.dispatch('GetObtainCoordinates', {
        clickStatus: false
      }); //终止拾取坐标事件 
      store.dispatch('GetTrafficLinesList', that.trailList)
    }
  }
</script>

<style lang="less" scoped>
  .viewpoint-list {
    height: 18vh !important;
    margin: 8px 10px 15px !important;

    /deep/.m-colorPicker .box {
      position: fixed;
      right: 50px;
    }
  }

  .options-btn {
    display: flex;
    justify-content: space-around;
    margin: 10px 0;
  }

  /deep/.ant-tabs {
    .roam-history {
      max-height: 40vh !important;
      margin-top: 0;
    }
  }
</style>