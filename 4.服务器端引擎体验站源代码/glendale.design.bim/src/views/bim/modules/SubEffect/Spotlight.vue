<template>
  <div ref="wrap"  :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreatePointLight" @keyup.prevent @keydown.enter.prevent>新建聚光灯</a-button>
      </a-space>
      <a-list size="small" :data-source="spotlightList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.Name }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelPointSource(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="新建聚光灯" :width="320" :visible="visible" :confirm-loading="confirmLoading" @ok="SaveSpotlight"
      @cancel="handleCancel" cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="光源名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入光源名称" v-decorator="['title', { rules: [{ required: true, message: '请输入光源名称' }] }]" />
        </a-form-item>
        <a-form-item label="光源位置" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
          <a-input placeholder="请选择光源位置" v-model="startPosition" />
          <a-button type="primary" @click="LightSourceLocation" @keyup.prevent @keydown.enter.prevent> 选择 </a-button>
        </a-form-item>
        <a-form-item label="目标点位置" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
          <a-input placeholder="请选择目标点位置" v-model="endPosition" />
          <a-button type="primary" @click="TargetPointLocation" @keyup.prevent @keydown.enter.prevent> 选择 </a-button>
        </a-form-item>
        <a-form-item label="光圆锥体角度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入光圆锥体角度"
            v-decorator="['angle', { rules: [{ required: true, message: '请输入光圆锥体角度' }], initialValue: 45 }]"
            :formatter="(value) => `${value}度`" :parser="(value) => value.replace('度', '')" :min="0" />
        </a-form-item>
        <a-form-item label="光照强度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入光照强度"
            v-decorator="['intensity', { rules: [{ required: true, message: '请输入光照强度' }], initialValue: 3 }]"
            :min="0" />
        </a-form-item>
        <a-form-item label="光源颜色" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
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
        spotlightList: [],
        pagination: {
          current: 1,
          pageSize: 1000
        },
        color: 'rgba(255,255,0,1)',
        colorRgba: 'rgba(255,255,0,1)',
        startPosition: [],
        endPosition: [],
        isMobile: false
      }
    },
    mounted() {
      const that = this
      this.isMobile = _isMobile() ? true : false;
      that.spotlightList = store.state.bim.spotlightList;
    },
    methods: {
      CreatePointLight() {
        const that = this;
        that.form.resetFields() //清空表单
        that.startPosition = [];
        that.endPosition = [];
        that.visible = true
      },
      LightSourceLocation() {
        const that = this;
        that.visible = false
        that.$message.info('请点击拾取点光源位置')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            that.startPosition = data
            that.visible = true;
            that.$message.success('拾取成功')
            store.dispatch('GetObtainCoordinates', {
              clickStatus: false
            })
          }
        })
      },
      TargetPointLocation() {
        const that = this;
        that.visible = false
        that.$message.info('请点击拾取目标点位置')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            that.endPosition = data
            that.visible = true;
            that.$message.success('拾取成功')
            store.dispatch('GetObtainCoordinates', {
              clickStatus: false
            })
          }
        })
      },
      DelPointSource(item, index) {
        const that = this
        that.spotlightList.splice(index, 1)
        api.Lights.deleteSpotLight(item.ID);
        that.$message.success('删除成功')
      },
      SaveSpotlight() {
        const that = this
        that.form.validateFields((err, values) => {
          if (!err) {
            let params = {
              "ID": genlabID(6),
              "Name": values.title,
              "Position": that.startPosition,
              "TargetPosition": that.endPosition,
              "Intensity": values.intensity,
              "Color": that.colorRgba,
              "OuterConeAngle": values.angle
            }
            that.spotlightList.push(params)
            api.Lights.addSpotLight(params);
            that.visible = false;
          }
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
      store.dispatch('GetSpotlightList', that.spotlightList)
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
  }
</style>