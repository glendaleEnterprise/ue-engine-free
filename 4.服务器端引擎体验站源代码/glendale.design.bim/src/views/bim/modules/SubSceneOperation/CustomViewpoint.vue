<template>
  <div class="side-frame" @contextmenu.prevent="" :class="{ 'mobile-Model':isMobile  }">
    <a-tabs v-model="tab" size="small" :animated="false" @change="ChangeTab">
      <a-tab-pane :key="1" tab="漫游设置" class="scroll-box roam-history">
        <a-form-model class="roaming-set" ref="ruleForm" :model="form" :rules="rules" :labelCol="{ span: 7 }"
          :wrapperCol="{ span: 16 }" :hideRequiredMark="true">
          <a-form-model-item label="漫游名称" prop="name">
            <a-input v-model="form.name" style="width: 100%" />
          </a-form-model-item>
          <a-form-model-item label="漫游时长" prop="time">
            <a-input-number v-model="form.time" :step="1" :formatter="(value) => `${value}秒`" :min="1"
              :parser="(value) => value.replace('秒', '')" style="width: 100%" />
          </a-form-model-item>
          <a-form-model-item label="轨迹创建" prop="lookFactor">
            <a-button ghost @click="GetViewPort" @keyup.prevent @keydown.enter.prevent> 添加轨迹点 </a-button>
          </a-form-model-item>
          <div>
            <a-list size="small" :data-source="form.viewPortPoints" bordered class="viewpoint-list scroll-box"
              :locale="{ emptyText: `暂无数据，至少添加两个视点！` }">
              <a-list-item slot="renderItem" slot-scope="item,index">
                {{ '视点' + (++index) }}
                <a-space slot="actions">
                  <a-tooltip title="删除">
                    <a-icon type="delete" :style="{ fontSize: '16px', color: '#05a081' }"
                      @click="DelViewPoint(index)" />
                  </a-tooltip>
                </a-space>
              </a-list-item>
            </a-list>
          </div>
          <a-form-model-item :wrapper-col="{ span: 24 }">
            <a-space class="options-btn">
              <a-button ghost @click="StartViewPortRoam" @keyup.prevent @keydown.enter.prevent>开始漫游</a-button>
              <a-button ghost @click="StopViewPortRoam" @keyup.prevent @keydown.enter.prevent>结束漫游</a-button>
            </a-space>
          </a-form-model-item>
        </a-form-model>
      </a-tab-pane>
      <a-tab-pane :key="2" tab="漫游历史" class="scroll-box roam-history roam-list-scroll">
        <a-list size="small" :data-source="roamList" :locale="{ emptyText: `暂无漫游历史` }" class="roam-list">
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
  import {
    setViewPort,
    getRoamingTrack,
    deleteRoamingTrack
  } from '@/api/roaming'
  import {
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
        roamList: [],
        playState: false,
        form: {
          name: '', //移动速度系数
          time: 10, //视角旋转速度(3°)
          viewPortPoints: [],
        },
        rules: {
          name: [{
            required: true,
            message: '请输入漫游名称'
          }],
          time: [{
            required: true,
            message: '请输入漫游时间'
          }],
        },
        sharedFatherState: false,
        formRenamed: this.$form.createForm(this),
        visibleRenamed: false, //重命名显示
        modelRenamed: null,
        pagination: {
          current: 1,
          pageSize: 10
        },
        isRoaming: false,
        isRoamingHistory: false,
        isMobile: false
      }
    },
    mounted() {
      this.isMobile = _isMobile() ? true : false
    },
    methods: {
      ChangeTab(data) {
        const that = this;
        if (data == 2) {
          that.roamList = []
          that.pagination.current = 1;
          that.pagination.pageSize = Math.ceil(document.getElementsByClassName("roam-history")[0].offsetHeight / 40) + 1
          that.GetRoamList()
        }
      },
      //获取漫游列表
      async GetRoamList() {
        const that = this;
        const params = {
          MaxResultCount: that.pagination.pageSize,
          SkipCount: (that.pagination.current - 1) * that.pagination.pageSize,
          modelId: that.projectMessage.modelList[0].id,
          projectId: that.projectMessage.projectId,
          RoamingType: 1
        }
        await getRoamingTrack(params).then(data => {
          const pagination = {
            ...that.pagination
          }
          pagination.total = data.totalCount
          if (data.items.length > 0) data.items.forEach((x) => (x.play = 0))
          that.roamList = that.roamList.concat(data.items)
          that.pagination = pagination
          if (data.totalCount > (that.pagination.current * that.pagination.pageSize)) {
            that.updated();
          }
        })
      },
      updated() {
        const that = this;
        that.$nextTick(() => {
          const el = document.querySelector('.roam-list-scroll');
          const offsetHeight = el.offsetHeight;
          el.onscroll = () => {
            const scrollTop = el.scrollTop;
            const scrollHeight = el.scrollHeight;
            if ((offsetHeight + scrollTop) - scrollHeight >= -1) {
              // 需要执行的代码
              if (that.pagination.total > that.pagination.current * that.pagination.pageSize) {
                that.pagination.current++
                that.GetRoamList()
              }
            }
          };
        });
      },
      //添加轨迹点
      GetViewPort() {
        const that = this;
        api.Camera.getViewPort((data) => {
          that.form.viewPortPoints.push(data)
          that.$message.info('已添加')
        })
      },
      //删除轨迹点
      DelViewPoint(i) {
        this.form.viewPortPoints.splice(i - 1, 1)
      },
      //开始漫游
      StartViewPortRoam() {
        const that = this;
        if (that.form.viewPortPoints.length < 2) {
          that.$message.warning('至少增加两个轨迹点!')
          return
        }
        if (that.form.name.match(/^[ ]*$/)) {
          that.$message.warning('请输入漫游名称!')
          return
        }
        if (that.form.time == "" || that.form.time == null) {
          that.$message.warning('请输入漫游时长!')
          return
        }
        that.playState = true
        that.isRoaming = true
        api.Camera.startViewPortRoam(that.form.viewPortPoints, that.form.time, (res) => {
          that.playState = false
          that.$message.success('漫游完成!')
          that.SaveRoam();
        })
      },
      //停止漫游
      StopViewPortRoam() {
        const that = this;
        if (that.form.viewPortPoints.length == 0) {
          that.$message.warning('请先录制漫游轨迹')
          return false
        }
        if (that.form.name.match(/^[ ]*$/)) {
          that.$message.warning('请输入漫游名称!')
          return
        }
        if (that.playState) {
          that.SaveRoam();
        }
        that.$message.success('漫游结束')
        that.isRoaming = true
        api.Camera.stopViewPortRoam()
      },
      //保存漫游
      SaveRoam() {
        const params = {
          modelId: this.projectMessage.modelList[0].id,
          projectId: this.projectMessage.projectId,
          remark: '',
          sort: 0,
          ...this.form
        }
        setViewPort(params).then(() => {
          this.$message.success('已保存!')
          this.GetRoamList()
          this.form = {
            viewPortPoints: [],
            time: 10,
            name: ''
          }
        })
      },
      //开始播放
      playIR(data) {
        this.isRoamingHistory = true;
        if (this.roamList.findIndex((x) => x.play === 1) > -1) {
          //停止正在播放的
          api.Camera.stopViewPortRoam()
          this.roamList.find((x) => x.play === 1).play = 0
        }
        data.play = 1
        api.Camera.startViewPortRoam(JSON.parse(data.record), data.time, (res) => {
          this.isRoamingHistory = false;
          data.play = 0
          this.$message.info('漫游结束!')
        })
      },
      //暂停播放
      playIRPause(data) {
        data.play = 2
        api.Camera.pauseViewPortRoam(false)
      },
      //继续播放
      playContinue(data) {
        data.play = 1
        api.Camera.pauseViewPortRoam(true)
        // this.$forceUpdate()
      },
      //取消播放
      playCancel(data) {
        if (data.play !== 0) {
          data.play = 0
          this.isRoamingHistory = false;
          api.Camera.stopViewPortRoam()
          // this.$forceUpdate()
        }
      },
      //删除漫游
      delCamera(data, index) {
        const that = this
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要删除漫游 “${data.name}” 吗？`,
          onOk() {
            data.play != 0 ? api.Camera.stopViewPortRoam() : null
            that.roamList.splice(index, 1)
            deleteRoamingTrack(data.id).then((res) => {})
            that.$message.success('删除成功！')
          },
        })
      },
    },
    destroyed() {
      const that = this;
      if (that.isRoaming || that.isRoamingHistory) {
        api.Camera.stopViewPortRoam()
      }
      that.projectMessage.camera ? api.Camera.setViewPort(that.projectMessage.camera) : api.Model.location(that
        .projectMessage.modelId);
    },
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

  .roam-history {
    max-height: 40vh !important;
  }

  .side-frame {
    .scroll-box {
      margin-top: 0;
    }
  }
</style>