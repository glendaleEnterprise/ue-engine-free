<template>
  <div class="side-frame" @contextmenu.prevent=""  :class="{ 'mobile-Model':isMobile  }">
    <a-tabs v-model="tab" size="small" :animated="false">
      <a-tab-pane :key="1" tab="移动设置">
        <a-form-model class="roaming-set scroll-box" ref="ruleForm" :model="form" :rules="rules" :labelCol="{ span: 7 }"
          :wrapperCol="{ span: 16 }" :hideRequiredMark="true">
          <a-form-model-item label="移动构件" prop="feature" class="select-feature">
            <a-input v-model="form.feature.split('^')[1]" style="width: 100%" />
            <a-button ghost @click="FeatureChoose" @keyup.prevent @keydown.enter.prevent>选择</a-button>
          </a-form-model-item>
          <a-form-model-item label="轨迹名称" prop="name">
            <a-input v-model="form.name" style="width: 100%" />
          </a-form-model-item>
          <a-form-model-item label="移动时长" prop="time">
            <a-input-number v-model="form.time" :step="1" :formatter="(value) => `${value}秒`" :min="1"
              :parser="(value) => value.replace('秒', '')" style="width: 100%" />
          </a-form-model-item>
          <a-form-model-item label="轨迹创建" prop="lookFactor">
            <a-button ghost @click="GetMotionTrail" @keyup.prevent @keydown.enter.prevent> 添加轨迹点 </a-button>
          </a-form-model-item>
          <div>
            <a-list size="small" :data-source="form.motionTrailList" bordered class="viewpoint-list scroll-box"
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
          </div>
          <a-form-model-item :wrapper-col="{ span: 24 }">
            <a-space class="options-btn">
              <a-button ghost @click="StartMotionTrail" @keyup.prevent @keydown.enter.prevent>开始移动</a-button>
              <a-button ghost @click="StopMotionTrail" @keyup.prevent @keydown.enter.prevent>结束移动</a-button>
            </a-space>
          </a-form-model-item>
        </a-form-model>
      </a-tab-pane>
      <a-tab-pane :key="2" tab="播放历史">
        <a-list size="small" :data-source="trailList" :locale="{ emptyText: `暂无历史数据` }" class="roam-list scroll-box">
          <a-list-item slot="renderItem" slot-scope="item, index">
            {{ item.name }}
            <a-space slot="actions">
              <a-tooltip title="播放" v-if="item.play === 0">
                <a-icon type="play-circle" :style="{ fontSize: '16px', color: '#05a081' }" @click="playIR(item)" />
              </a-tooltip>
              <a-tooltip v-else-if="item.play === 1" title="暂停">
                <a-icon type="pause-circle" :style="{ fontSize: '16px', color: '#05a081' }"
                  @click="playIRPause(item)" />
              </a-tooltip>
              <a-tooltip v-else-if="item.play === 2" title="继续">
                <a-icon type="play-circle" :style="{ fontSize: '16px', color: '#05a081' }"
                  @click="playContinue(item)" />
              </a-tooltip>
              <a-tooltip title="取消">
                <a-icon type="stop" :style="{ fontSize: '16px', color: '#05a081' }" @click="playCancel(item)" />
              </a-tooltip>
              <a-tooltip title="删除">
                <a-icon type="delete" :style="{ fontSize: '16px', color: '#05a081' }" @click="delCamera(item, index)" />
              </a-tooltip>
            </a-space>
          </a-list-item>
        </a-list>
      </a-tab-pane>

    </a-tabs>
  </div>
</template>

<script>
  import store from '@/store'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    name: 'TrolleyMobile',
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
          feature: '',
          name: '', //移动速度系数
          time: 10, //视角旋转速度(3°)
          motionTrailList: [],
        },
        rules: {
          feature: [{
            required: true,
            message: '请选择移动构件'
          }],
          name: [{
            required: true,
            message: '请输入轨迹名称'
          }],
          time: [{
            required: true,
            message: '请输入移动时长'
          }],
        },
        isMobile: false,
      }
    },
    mounted() {
      this.isMobile = _isMobile() ? true : false;
      let list = store.state.bim.featureTrolleyMobileList
      if (list.length > 0) list.forEach((x) => (x.play = 0))
      this.trailList = list
    },
    methods: {
      FeatureChoose() {
        const that = this;
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        }) //终止拾取坐标事件
        that.$message.info('请点击选择移动的构件')
        api.Feature.getByEvent(true, (featureId) => {
          if (that.form.feature != '' && that.form.feature != featureId.split("^")[1]) {
            api.Feature.setColor(that.form.feature, "rgba(255,255,255,1)");
          }
          api.Feature.setColor(featureId, "rgba(255,255,0,1)");
          if (featureId.split("^")[1]) {
            that.form.feature = featureId;
          }
        });
      },
      //添加轨迹点
      GetMotionTrail() {
        const that = this;
        api.Feature.getByEvent(false); //关闭构件拾取事件
        that.$message.info('请在场景中点击拾取移动轨迹坐标')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            that.form.motionTrailList.push(data)
            api.Public.drawPoint({
              "position": data,
              "size": "5",
              "persistent": "1",
              "color": "rgba(255,255,0,1)"
            });
          }
        })
      },
      //删除轨迹点
      DelMotionTrail(i) {
        this.form.motionTrailList.splice(--i, 1)
      },
      //开始漫游
      StartMotionTrail() {
        const that = this;
        api.Public.clearAllDrawObject()
        api.Feature.getByEvent(false); //关闭构件拾取事件
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        }) //终止拾取坐标事件
        api.Public.pickupCoordinate(false); //moveByTrack接口调用期间调用拾取坐标接口回调错误，且运动完不在回调
        if (that.form.motionTrailList.length < 2) {
          that.$message.warning('至少增加两个轨迹点!')
          return
        }
        if (!that.form.name) {
          that.$message.warning('请输入轨迹名称!')
          return
        }
        if (that.form.time == "" || that.form.time == null) {
          that.$message.warning('请输入移动时长!')
          return
        }
        api.Feature.setColor(that.form.feature, "rgba(255,255,255,1)")
        const params = {
          Positions: that.form.motionTrailList,
          FeatureID: that.form.feature,
          Time: that.form.time,
          angleX: 90,
          play: 0,
          name: that.form.name
        }
        that.trailList.push(params)
        api.Feature.moveByTrack(params, (res) => {
          if (res.indexOf("^") != -1) {
            store.dispatch('GetObtainCoordinates', {
              clickStatus: true
            })
            api.Feature.original(params.FeatureID, params.FeatureID.split("^")[0])
            that.$message.success('移动完成!')
            this.form = {
              motionTrailList: [],
              time: 10,
              name: '',
              feature: ''
            }
          }
        })
      },
      //停止漫游
      StopMotionTrail() {
        const that = this;
        api.Feature.getByEvent(false); //关闭构件拾取事件
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        }) //终止拾取坐标事件
        // that.$message.success('移动结束')
        api.Feature.stopMoveByTrack(that.form.feature);
        api.Feature.original(that.form.feature, that.form.feature.split("^")[0])
      },
      //开始播放
      playIR(data) {
        if (this.trailList.findIndex((x) => x.play === 1) > -1) {
          //停止正在播放的
          api.Feature.stopMoveByTrack(x.FeatureID);
          this.trailList.find((x) => x.play === 1).play = 0
        }
        data.play = 1
        for (let i = 0; i < data.Positions.length; i++) {
          data.Positions[i] = data.Positions[i].split("|")
        }
        api.Feature.moveByTrack(data, (res) => {
          api.Feature.original(data.FeatureID, data.FeatureID.split("^")[0])
          data.play = 0
          this.$message.info('移动结束!')
        })
      },
      //暂停播放
      playIRPause(data) {
        data.play = 2
        api.Feature.pauseMoveByTrack(data.FeatureID, false);
      },
      //继续播放
      playContinue(data) {
        data.play = 1
        api.Feature.pauseMoveByTrack(data.FeatureID, true);
      },
      //取消播放
      playCancel(data) {
        if (data.play !== 0) {
          data.play = 0
          api.Feature.stopMoveByTrack(data.FeatureID);
          api.Feature.original(data.FeatureID, data.FeatureID.split("^")[0])
        }
      },
      //删除漫游
      delCamera(data, index) {
        const that = this
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要删除移动轨迹 “${data.name}” 吗？`,
          onOk() {
            data.play != 0 ? api.Feature.stopMoveByTrack(data.FeatureID) : null
            that.trailList.splice(index, 1)
            that.$message.success('删除成功！')
          },
        })
      },
    },
    destroyed() {
      const that = this;
      that.form.feature ? api.Feature.setColor(that.form.feature, "rgba(255,255,255,1)") : null;
      api.Feature.getByEvent(false); //关闭构件拾取事件
      store.dispatch('GetObtainCoordinates', {
        clickStatus: true
      }) //终止拾取坐标事件
      that.trailList.forEach(item => {
        api.Feature.original(item.FeatureID, item.FeatureID.split("^")[0])
      })
      store.dispatch('GetFeatureTrolleyMobileList', that.trailList)
    }
  }
</script>

<style lang="less" scoped>
  .viewpoint-list {
    height: 18vh !important;
    margin: 8px 10px 15px !important;
  }

  .options-btn {
    display: flex;
    justify-content: space-around;
    margin: 10px 0;
  }

  .select-feature {
    /deep/.ant-form-item-children {
      display: flex;

      .ant-btn {
        padding: 0 5px;
        margin-left: 5px;
        color: #21ad8d;
        border-color: #21ad8d;
      }
    }

  }
</style>