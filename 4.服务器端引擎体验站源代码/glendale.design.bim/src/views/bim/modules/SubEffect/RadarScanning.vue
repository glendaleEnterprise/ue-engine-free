<template>
  <div ref="wrap"  :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateRadarScanning" @keyup.prevent @keydown.enter.prevent>新建雷达扫描</a-button>
      </a-space>
      <a-list size="small" :data-source="radarScannList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.Name }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelRadarScanning(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="新建雷达扫描" :width="320" :visible="visible" :confirm-loading="confirmLoading" @ok="SaveSpotlight"
      @cancel="handleCancel" cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="扫描名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入扫描名称" v-decorator="['title', { rules: [{ required: true, message: '请输入扫描名称' }] }]" />
        </a-form-item>
        <a-form-item label="扫描半径" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入扫描半径"
            v-decorator="['radius', { rules: [{ required: true, message: '请输入扫描半径' }], initialValue: 1 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" :min="0" />
        </a-form-item>
        <a-form-item label="扫描速率" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入扫描速率" :max="10" :min="1"
            v-decorator="['speed', { rules: [{ required: true, message: '请输入扫描速率' }], initialValue: 5 }]" />
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
        confirmLoading: false,
        form: this.$form.createForm(this, {
          name: 'coordinated'
        }),
        radarScannList: [],
        pagination: {
          current: 1,
          pageSize: 1000
        },
        color: 'rgba(255,255,255,1)',
        colorRgba: 'rgba(255,255,255,1)',
        setPosition: [],
        isMobile: false
      }
    },
    mounted() {
      const that = this
      this.isMobile = _isMobile() ? true : false;
      that.radarScannList = store.state.bim.radarScannList;
    },
    methods: {
      CreateRadarScanning() {
        const that = this;
        that.form.resetFields() //清空表单
        that.$message.info('请点击拾取添加雷达扫描的位置')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            data[2] = data[2] + 1
            that.setPosition = data
            that.visible = true;
            store.dispatch('GetObtainCoordinates', {
              clickStatus: false
            })
          }
        })
      },
      SaveSpotlight() {
        const that = this
        that.form.validateFields((err, values) => {
          if (!err) {
            let params = {
              "ID": "scann_" + genlabID(6),
              "Name": values.title,
              "Position": that.setPosition,
              "SpawnRate": values.speed,
              "ScalingFactor": values.radius * 2,
            }
            that.radarScannList.push(params);
            api.Plugin.addRadarScanning(params);
            that.visible = false;
          }
        })
      },
      DelRadarScanning(item, index) {
        const that = this
        that.radarScannList.splice(index, 1)
        api.Plugin.deleteRadarScanning(item.ID);
        that.$message.success('雷达扫描删除成功')
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
      store.dispatch('GetRadarScannList', that.radarScannList)
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
  }
</style>