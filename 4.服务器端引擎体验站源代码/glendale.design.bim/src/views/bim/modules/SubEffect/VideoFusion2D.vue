<template>
  <div class="side-frame" @contextmenu.prevent="" :class="{ 'mobile-Model':isMobile  }">
    <a-tabs v-model="tab" size="small" :animated="false">
      <a-tab-pane :key="1" tab="视频设置">
        <a-form class="roaming-set scroll-box" ref="ruleForm" :form="form" :labelCol="{ span: 7 }" :wrapperCol="{ span: 16 }"
          :hideRequiredMark="true">
          <a-form-item label="视频名称">
            <a-input placeholder="请输入视频名称"
              v-decorator="['title', { rules: [{ required: true, message: '请输入视频名称' }] }]" />
          </a-form-item>
          <a-form-item label="视频位置" prop="lookFactor">
            <a-button ghost @click="GetVideoLocation" @keyup.prevent @keydown.enter.prevent> 点选位置 </a-button>
          </a-form-item>
          <div>
            <a-list size="small" :data-source="VideoLocationList" bordered class="viewpoint-list scroll-box"
              :locale="{ emptyText: `暂无数据。请按照固定顺序，依次拾取四个点，围成视频播放位置` }">
              <a-list-item slot="renderItem" slot-scope="item,index">
                {{ '位置' + (++index) }}
                <a-space slot="actions">
                  <a-tooltip title="删除">
                    <a-icon type="delete" :style="{ fontSize: '16px', color: '#05a081' }"
                      @click="DelVideoLocation(index)" />
                  </a-tooltip>
                </a-space>
              </a-list-item>
            </a-list>
          </div>
          <a-form-item label="旋转角度">
            <a-select v-decorator="['rotate', { rules: [{ required: true }], initialValue: '90' },]">
              <a-select-option value="0"> 0 </a-select-option>
              <a-select-option value="90"> 90 </a-select-option>
              <a-select-option value="180"> 180 </a-select-option>
              <a-select-option value="270"> 270 </a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item label="翻转">
            <a-switch v-decorator="['overturn', { valuePropName: 'checked', initialValue: 'checked' }]" />
          </a-form-item>
          <a-form-item :wrapper-col="{ span: 24 }">
            <a-space class="options-btn">
              <a-button ghost @click="StartVideoFusion2D" @keyup.prevent @keydown.enter.prevent>保存视频</a-button>
              <a-button ghost @click="StopVideoFusion2D" @keyup.prevent @keydown.enter.prevent>删除视频</a-button>
            </a-space>
          </a-form-item>
        </a-form>
      </a-tab-pane>
      <a-tab-pane :key="2" tab="历史列表">
        <a-list size="small" :data-source="videoFusion2DHistoryList" :locale="{ emptyText: `暂无数据` }"
          class="roam-list scroll-box">
          <a-list-item slot="renderItem" slot-scope="item,index">
            <a-space class="label-control">
              <a-switch checked-children="显示" un-checked-children="隐藏" v-model="item.isopen"
                @change="displayStatusChange($event, item)" />
            </a-space>
            <a-space class="label-title">{{ item.title }}</a-space>
            <a-space slot="actions">
              <a-tooltip title="删除">
                <a-icon type="delete" @click="delVideoFusion2D(item, index)" />
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
    genlabID,
    _isMobile
  } from '@/api/public'
  export default {
    name: 'VideoFusion2D',
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        tab: 1,
        videoFusion2DHistoryList: [],
        playState: false,
        form: this.$form.createForm(this, {
          name: 'coordinated'
        }),
        sharedFatherState: false,
        formRenamed: this.$form.createForm(this),
        visibleRenamed: false, //重命名显示
        modelRenamed: null,
        VideoLocationList: [],
        videoPlugin: undefined,
        videoExample: store.state.videoAddress,
        currentId: '',
        isMobile: false
      }
    },
    mounted() {
      const that = this;
      this.isMobile = _isMobile() ? true : false;
      that.videoFusion2DHistoryList = store.state.bim.videoFusion2DList
      that.videoPlugin = api.Plugin.initVideo();
    },
    methods: {
      //添加视频位置
      GetVideoLocation() {
        const that = this;
        api.Feature.getByEvent(false); //关闭构件拾取事件
        if (that.VideoLocationList.length == 4) {
          that.form.resetFields() //清空表单
          that.VideoLocationList = []
          that.currentId = "";
        }
        that.$message.info('请在场景中按照固定顺序，依次拾取四个点，围成视频播放位置')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            if (that.VideoLocationList.length < 4) {
              that.VideoLocationList.push(data)
              api.Public.drawPoint({
                "position": data,
                "size": "5",
                "persistent": "1",
                "color": "rgba(255,255,0,1)"
              });
            }
          }
        })
      },
      //删除轨迹点
      DelVideoLocation(i) {
        this.VideoLocationList.splice(--i, 1)
      },
      //开始视频
      StartVideoFusion2D() {
        const that = this;
        if (that.currentId) {
          that.$message.warning('该视频已保存，新建请重新点选位置!')
          return
        }
        if (that.VideoLocationList.length != 4) {
          that.$message.warning('请先完成视频位置拾取!')
          return
        }
        that.form.validateFields((err, values) => {
          if (!err) {
            const params = {
              "ID": "video" + genlabID(6),
              "title": values.title,
              "positions": that.VideoLocationList,
              "positionOffset": {
                "x": 0,
                "y": 0,
                "z": 0
              },
              "url": that.videoExample,
              "overturn": values.overturn == "checked" ? true : false,
              "rotate": values.rotate * 1,
              "isopen": true
            }
            that.currentId = params.ID
            that.videoPlugin.addByPlane(params.ID, params);
            api.Public.clearAllDrawObject()
            that.videoFusion2DHistoryList.push(params)
            that.visible = false;
          }
        })
      },
      //停止视频
      StopVideoFusion2D() {
        const that = this;
        that.form.resetFields() //清空表单
        that.VideoLocationList = [];
        api.Public.clearAllDrawObject()
        that.$message.success('视频结束')
        that.videoPlugin.delete(that.currentId);
        let index = that.videoFusion2DHistoryList.findIndex(item => item.ID == that.currentId)
        index != -1 ? that.videoFusion2DHistoryList.splice(index, 1) : null
        that.currentId = "";
      },
      //删除视频
      delVideoFusion2D(data, index) {
        const that = this
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要删除视频 “${data.title}” 吗？`,
          onOk() {
            that.videoFusion2DHistoryList.splice(index, 1)
            that.videoPlugin.delete(data.ID);
            if (data.ID == that.currentId) {
              that.currentId = "";
              that.form.resetFields() //清空表单
              that.VideoLocationList = [];
            }
            that.$message.success('删除成功！')
            that.form.resetFields() //清空表单
          },
        })
      },
      displayStatusChange(event, data) {
        const that = this;
        if (event) {
          that.videoPlugin.addByPlane(data.ID, data);
        } else {
          that.videoPlugin.delete(data.ID);
        }
        data.isopen = event;
      }
    },
    destroyed() {
      const that = this
      api.Public.clearAllDrawObject()
      store.dispatch('GetVideoFusion2DList', that.videoFusion2DHistoryList)
      api.Feature.getByEvent(false); //关闭构件拾取事件
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
</style>