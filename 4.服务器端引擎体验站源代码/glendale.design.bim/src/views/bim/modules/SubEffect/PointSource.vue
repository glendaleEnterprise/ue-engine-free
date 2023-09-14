<template>
  <div ref="wrap"  :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreatePointLight" @keyup.prevent @keydown.enter.prevent>新建点光源</a-button>
      </a-space>
      <a-list size="small" :data-source="pointSourceList" :locale="{ emptyText: `暂无数据` }"
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
    <a-modal title="保存点光源" :width="280" :visible="visible" :confirm-loading="confirmLoading" @ok="SavePointLight"
      @cancel="handleCancel" cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="光源名称" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <a-input placeholder="请输入" v-decorator="['title', { rules: [{ required: true, message: '请输入!' }] }]" />
        </a-form-item>
        <a-form-item label="光照距离" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <a-input-number placeholder="请输入光照距离"
            v-decorator="['distance', { rules: [{ required: true, message: '请输入光照距离' }], initialValue: 100 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" :min="0" />
        </a-form-item>
        <a-form-item label="光照强度" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <a-input-number placeholder="请输入光照强度"
            v-decorator="['intensity', { rules: [{ required: true, message: '请输入光照强度' }], initialValue: 10 }]"
            :min="0" />
        </a-form-item>
        <a-form-item label="光源颜色" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }" class="color-picker">
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
        pointSourceList: [],
        pagination: {
          current: 1,
          pageSize: 1000
        },
        color: 'rgba(255,255,0,1)',
        colorRgba: 'rgba(255,255,0,1)',
        pointLightPosition: [],
        isMobile: false
      }
    },
    mounted() {
      const that = this
      this.isMobile = _isMobile() ? true : false;
      that.pointSourceList = store.state.bim.labelVisibleNow;
    },
    methods: {
      CreatePointLight() {
        const that = this;
        that.$message.info('请点击选择点光源的位置')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            that.pointLightPosition = data;
            that.form.resetFields() //清空表单
            that.visible = true
            store.dispatch('GetObtainCoordinates', {
              clickStatus: false
            })
          }
        })
      },
      DelPointSource(item, index) {
        const that = this
        that.pointSourceList.splice(index, 1)
        api.Lights.deletePointLight(item.ID);
        that.$message.success('点光源删除成功')
      },
      SavePointLight() {
        const that = this
        that.form.validateFields((err, values) => {
          if (!err) {
            let params = {
              "Name": values.title,
              "ID": genlabID(6),
              "Position": that.pointLightPosition,
              "Intensity": values.intensity,
              "Color": that.colorRgba,
              "AttenuationRadius": values.distance
            }
            that.pointSourceList.push(params)
            api.Lights.addPointLight(params);
            that.visible = false
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
      store.dispatch('GetPointSourceList', that.pointSourceList)
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
  }
</style>