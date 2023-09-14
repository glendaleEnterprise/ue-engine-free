<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateDiffusion" @keyup.prevent @keydown.enter.prevent>
          {{ type == 1 ? '新建圆扩散' : '新建自定义扩散' }}</a-button>
      </a-space>
      <a-list size="small" :data-source="diffusionList" :locale="{ emptyText: `暂无数据` }"
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
    <a-modal :title="type == 1 ? '新建圆扩散' : '新建自定义扩散'" :width="320" :visible="visible" :confirm-loading="confirmLoading"
      @ok="SaveSpotlight" :maskClosable="false" @cancel="handleCancel" cancel-text="取消" ok-text="确定"
      :getContainer="() => this.$refs.wrap">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="扩散名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入扩散名称" v-decorator="['title', { rules: [{ required: true, message: '请输入扩散名称' }] }]" />
        </a-form-item>
        <a-form-item label="扩散半径" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入扩散半径"
            v-decorator="['radius', { rules: [{ required: true, message: '请输入扩散半径' }], initialValue: 5 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" :min="0" />
        </a-form-item>
        <a-form-item label="扩散速率" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入扩散速率" :max="10" :min="1"
            v-decorator="['speed', { rules: [{ required: true, message: '请输入扩散速率' }], initialValue: 5 }]" />
        </a-form-item>
        <a-form-item label="扩散圈数" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" v-if="type == 1">
          <a-input-number placeholder="请输入扩散圈数"
            v-decorator="['numberTurns', { rules: [{ required: true, message: '请输入扩散圈数' }], initialValue: 2 }]"
            :min="0" />
        </a-form-item>
        <a-form-item label="扩散夹角" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" v-if="type == 2">
          <a-input-number placeholder="请输入扩散夹角"
            v-decorator="['includedAngle', { rules: [{ required: true, message: '请输入扩散夹角' }], initialValue: 30 }]"
            :formatter="(value) => `${value}度`" :parser="(value) => value.replace('度', '')" :min="0" />
        </a-form-item>
        <a-form-item label="旋转角度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" v-if="type == 2">
          <a-input-number placeholder="请输入旋转角度"
            v-decorator="['angle', { rules: [{ required: true, message: '请输入旋转角度' }], initialValue: 0 }]"
            :formatter="(value) => `${value}度`" :parser="(value) => value.replace('度', '')" :min="0" />
        </a-form-item>
        <a-form-item label="扩散颜色" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
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
      typeDiffusion: {
        type: String,
      }
    },
    data() {
      return {
        visible: false,
        confirmLoading: false,
        form: this.$form.createForm(this, {
          name: 'coordinated'
        }),
        diffusionList: [],
        color: 'rgba(0,0,255,1)',
        colorRgba: 'rgba(0,0,255,1)',
        setPosition: [],
        type: 1,
        isMobile: false
      }
    },
    mounted() {
      const that = this
      this.isMobile = _isMobile() ? true : false;
      that.typeDiffusion == 'CircularDiffusion' ? that.type = 1 : that.type = 2;
      that.diffusionList = that.type == 1 ? store.state.bim.diffusionList.circleType : store.state.bim.diffusionList
        .customType;
    },
    methods: {
      CreateDiffusion() {
        const that = this;
        that.form.resetFields() //清空表单
        that.$message.info('请点击拾取添加扩散的位置')
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
              "ID": genlabID(6),
              "Name": values.title,
              "Position": that.setPosition,
              "Color": that.colorRgba,
              "Type": 1,
              "ScalingFactor": values.radius,
              "SpawnRate": values.speed,
            }

            if (that.type == 1) {
              params = Object.assign(params, {
                "Type": 1,
                "CirclesNumber": values.numberTurns,
              })
            } else {
              params = Object.assign(params, {
                "Type": 2,
                "IncludedAngleDegree": values.includedAngle,
                "RotationAngle": values.angle,
              })
            }
            that.diffusionList.push(params)
            api.Plugin.addDiffusionEffect(params);
            that.visible = false;
          }
        })
      },
      DelPointSource(item, index) {
        const that = this
        that.diffusionList.splice(index, 1)
        api.Plugin.deleteDiffusionEffect(item.ID);
        that.$message.success('扩散删除成功')
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
      store.dispatch('GetDiffusionList', {
        list: that.diffusionList,
        type: that.type
      })
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
  }
</style>