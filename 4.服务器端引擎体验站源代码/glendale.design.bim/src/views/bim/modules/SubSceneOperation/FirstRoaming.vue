<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-tabs v-model="tab" size="small" :animated="false" @change="ChangeTab">
      <a-tab-pane :key="1" tab="漫游设置" class="scroll-box roam-history ">
        <a-form-model class="roaming-set" ref="ruleForm" :model="form" :rules="rules" :labelCol="{ span: 8 }"
          :hideRequiredMark="true" :wrapperCol="{ span: 16 }" @cancel="handleCancel">
          <a-form-model-item label="漫游名称" prop="name">
            <a-input v-model="form.name" style="width: 100%" />
          </a-form-model-item>
          <a-form-model-item label="移动速度" prop="moveRate">
            <a-input-number placeholder="0.00" v-model="form.moveRate" :precision="2" :step="0.01" style="width: 100%"
              :min="0" />
          </a-form-model-item>
          <a-form-model-item label="旋转速度" prop="lookFactor">
            <a-input-number placeholder="0.00" v-model="form.turnRate" :precision="2" :step="0.01" style="width: 100%"
              :min="0" />
          </a-form-model-item>
          <a-form-model-item :wrapper-col="{ span: 18, offset: 2 }">
            <a-space :size="20">
              <a-button ghost @click="startImmersionRoaming" @keyup.prevent @keydown.enter.prevent>开始漫游</a-button>
              <a-button ghost @click="endImmersionRoaming" @keyup.prevent @keydown.enter.prevent>结束漫游</a-button>
            </a-space>
          </a-form-model-item>
        </a-form-model>
        <a-list size="small" :data-source="data" bordered class="option-tip">
          <div slot="header">操作指南</div>
          <a-list-item slot="renderItem" slot-scope="item">{{ item.option }}<span slot="actions">{{ item.title
          }}</span></a-list-item>
        </a-list>
      </a-tab-pane>
      <a-tab-pane :key="2" tab="漫游历史" class="scroll-box roam-history roam-list-scroll">
        <a-list size="small" :data-source="historys" :locale="{ emptyText: `暂无漫游历史` }" :loading="loading">
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
                <a-icon type="stop" :style="{ fontSize: '16px', color: '#05a081' }" @click="playCancle(item)" />
              </a-tooltip>
              <a-tooltip title="重命名">
                <a-icon type="edit" :style="{ fontSize: '16px', color: '#05a081' }" @click="renamed(item)"></a-icon>
              </a-tooltip>
              <a-tooltip title="删除">
                <a-icon type="delete" :style="{ fontSize: '16px', color: '#05a081' }" @click="delCamera(item, index)" />
              </a-tooltip>
            </a-space>
          </a-list-item>
        </a-list>
      </a-tab-pane>
    </a-tabs>

    <a-modal title="漫游重命名" :width="280" :visible="visibleRenamed" :destroyOnClose="true" :maskClosable="false"
      @ok="handSaveRenamed" @cancel="visibleRenamed = !visibleRenamed" :getContainer="() => this.$refs.wrap">
      <a-form :form="formRenamed" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" @submit="handSaveRenamed">
        <a-form-item label="漫游名称">
          <a-input v-decorator="[
            'name',
            {
              rules: [
                { required: true, message: '请输入' },
                { max: 50, message: '最多50字符' },
              ],
            },
          ]" />
        </a-form-item>
        <a-form-item label="备注" v-show="false">
          <a-textarea placeholder="备注" v-decorator="['remark', { rules: [{ max: 100, message: '最多100字符' }] }]"
            :rows="4" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
  import {
    setRoamingTrack,
    getRoamingTrack,
    deleteRoamingTrack,
    updateRoamingTrack
  } from '@/api/roaming'

  const data = [{
      option: '鼠标左键',
      title: '前进、后退、旋转',
    },
    {
      option: '鼠标滚轮',
      title: '上扬、下俯',
    },
    {
      option: '按键(↑)或(↓)',
      title: '前后平移',
    },
    {
      option: '按键(←)或(→)',
      title: '左右旋转',
    },
  ]
  import store from '@/store'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    name: 'FirstRoaming',
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        isRoaming: false,
        isRoamingHistory: false,
        tab: 1,
        data,
        historys: [],
        form: {
          name: "",
          moveRate: 0.5, //移动速度系数
          turnRate: 1, //视角旋转速度(3°)
        },
        rules: {
          name: [{
            required: true,
            message: '请输入漫游名称'
          }],
          moveRate: [{
            required: true,
            message: '请输入移动速度系数'
          }],
          turnRate: [{
            required: true,
            message: '请输入旋转速度系数'
          }],
        },
        formRenamed: this.$form.createForm(this),
        visibleRenamed: false, //重命名显示
        modelRenamed: null,
        pagination: {
          current: 1,
          pageSize: 10
        },
        loading: true,
        isMobile: false,

      }
    },
    mounted() {
      this.isMobile = _isMobile() ? true : false;
      //初始化漫游历史
      api.Plugin.addMiniMap();
      let options = {
        Anchor: 3,
        Offset: [70, -380],
        Size: 300,
        // Height: 1200
      }
      api.Plugin.updateMiniMap(options);
      api.Camera.setImmersiveRoamConfig({
        roamingMode: "Third",
        moveRate: 0.5,
        turnRate: 1,
        bRecordLocus: true,
      });
    },
    methods: {
      ChangeTab(data) {
        const that = this;
        if (data == 2) {
          that.historys = [];
          that.pagination.current = 1;
          that.pagination.pageSize = Math.ceil(document.getElementsByClassName("roam-history")[0].offsetHeight / 40) + 1
          that.getRoamingList()
        }
      },
      async getRoamingList() {
        const that = this
        await getRoamingTrack({
          MaxResultCount: that.pagination.pageSize,
          SkipCount: (that.pagination.current - 1) * that.pagination.pageSize,
          ModelId: that.projectMessage.modelList[0].id
        }).then(res => {
          that.loading = false;
          const pagination = {
            ...that.pagination
          }
          pagination.total = res.totalCount
          //
          if (res.items.length > 0) res.items.forEach((x) => (x.play = 0))
          that.historys = that.historys.concat(res.items)
          that.pagination = pagination
          if (res.totalCount > (that.pagination.current * that.pagination.pageSize)) {
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
                that.getRoamingList()
              }
            }
          };
        });
      },
      startImmersionRoaming(e) {
        //开始漫游
        e.preventDefault()
        const that = this;
        if (!that.form.name) {
          that.$message.warning('请输入漫游名称!')
          return
        }
        that.$refs.ruleForm.validate((valid) => {
          if (valid) {
            that.$message.info('请点击选择漫游起始点')
            store.dispatch('GetObtainCoordinates', {
              clickStatus: true,
              callback: (data) => {
                store.dispatch('GetObtainCoordinates', {
                  clickStatus: false
                })

                //配置漫游
                api.Camera.setImmersiveRoamConfig({
                  roamingMode: 'Third',
                  moveRate: that.form.moveRate,
                  turnRate: that.form.turnRate,
                  bRecordLocus: true,
                  onIREnd: function (result) {
                    that.$message.success('漫游结束')
                    if (result) {
                      setRoamingTrack({
                        name: that.form.name,
                        modelId: that.projectMessage.modelList[0].id,
                        projectId: that.projectMessage.projectId,
                        time: 0,
                        remark: '',
                        sort: 0,
                        roamingPoints: result,
                      }).then((res) => {
                        res.record = result
                        res.play = 0
                        that.historys.push(res) //play 0未播放 1播放 2暂停
                        that.form = {
                          name: "",
                          moveRate: 0.5,
                          turnRate: 1,
                        }
                      })
                    }
                  },
                })
                //启动漫游
                that.isRoaming = true
                api.Camera.startImmersiveRoam(data)
              }
            })
          }
        })
      },
      endImmersionRoaming() {
        if (this.isRoaming) {
          this.isRoaming = false
          api.Camera.stopImmersiveRoam()
          this.projectMessage.camera ? api.Camera.setViewPort(this.projectMessage.camera) : api.Model.location(this
            .projectMessage.modelId);
        } else {
          this.$message.warning('请先开启漫游')
        }
      },
      handleCancel() {
        this.endImmersionRoaming()
        this.$notification.destroy();
      },
      playIR(data) {
        //播放 play 0未播放 1播放 2暂停
        const that = this
        that.isRoamingHistory = true;
        if (that.historys.findIndex((x) => x.play === 1) > -1) {
          //暂停正在播放的
          that.historys.find((x) => x.play === 1).play = 0
        }
        data.play = 1
        let record = typeof data.record == 'string' ? JSON.parse(data.record) : data.record
        api.Camera.playImmersiveRoam({
          records: record, //轨迹数据>
          isLoopPlay: false,
          complete: function () {
            data.play = 0
          },
        });
      },
      playIRPause(data) {
        //暂停播放
        data.play = 2
        api.Camera.pauseImmersiveRoam(false);
      },
      playContinue(data) {
        //继续播放
        data.play = 1;
        api.Camera.pauseImmersiveRoam(true);
        if (this.historys.findIndex((x) => x.play === 1) > -1) {
          this.historys.find((x) => x.play === 1).play = 0
        }
      },
      playCancle(data) {
        //取消播放
        if (data.play === 1) {
          data.play = 0
        }
        api.Camera.cancelPlayImmersiveRoam(); //取消播放
        this.isRoamingHistory = false;
        this.projectMessage.camera ? api.Camera.setViewPort(this.projectMessage.camera) : api.Model.location(this
          .projectMessage.modelId);
      },
      //重命名
      renamed(record) {
        this.visibleRenamed = true
        this.modelRenamed = record
        this.$nextTick(() => {
          this.formRenamed.setFieldsValue({
            name: record.name,
            remark: record.remark
          }) //,
        })
      },
      //重命名保存
      handSaveRenamed() {
        const that = this;
        that.formRenamed.validateFields((err, values) => {
          if (!err) {
            var data = Object.assign(that.modelRenamed, values)
            updateRoamingTrack(data).then((res) => {
              if (res) {
                that.visibleRenamed = false
                that.$message.success('保存成功')
              } else {
                that.$message.error('保存失败')
              }
            })
          }
        })
      },
      delCamera(data, index) {
        //删除漫游
        const that = this
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要删除漫游 “${data.name}” 吗？`,
          onOk() {
            that.historys.splice(index, 1)
            deleteRoamingTrack(data.id).then((res) => {})
            that.$message.success('删除成功！')
          },
        })
      },
    },
    destroyed() {
      const that = this;
      if (that.isRoaming) {
        api.Camera.stopImmersiveRoam()
      }
      if (that.isRoamingHistory) {
        api.Camera.cancelPlayImmersiveRoam();
      }
      api.Plugin.deleteMiniMap();
      that.projectMessage.camera ? api.Camera.setViewPort(that.projectMessage.camera) : api.Model.location(that
        .projectMessage.modelId);
    },
  }
</script>

<style lang="less" scoped>
  .option-tip {
    margin-top: 18px;
  }

  .roam-history {
    max-height: 40vh !important;
    padding-right: 6px;
  }

  /deep/.ant-tabs-bar {
    margin: 0 0 0px 0;
  }

  .side-frame {
    .scroll-box {
      margin-top: 0;
    }
  }
</style>