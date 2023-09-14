<template>
  <div ref="wrap"  :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateGasFlow" @keyup.prevent @keydown.enter.prevent>新建烟雾扩散</a-button>
      </a-space>
      <a-list size="small" :data-source="gasFlowList" :locale="{ emptyText: `暂无数据` }" class="viewpoint-list scroll-box">
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
    <a-modal title="新建烟雾扩散" :width="320" :visible="visible" :confirm-loading="confirmLoading" @ok="SaveSpotlight"
      @cancel="handleCancel" cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="扩散名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入扩散名称" v-decorator="['title', { rules: [{ required: true, message: '请输入扩散名称' }] }]" />
        </a-form-item>
        <a-form-item label="扩散半径" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入扩散半径"
            v-decorator="['radius', { rules: [{ required: true, message: '请输入扩散半径' }], initialValue: 3 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" :min="0" />
        </a-form-item>
        <a-form-item label="扩散速度(X)" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入扩散速度(X)"
            v-decorator="['xvelocity', { rules: [{ required: true, message: '请输入扩散速度(X)' }], initialValue: 0 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" />
        </a-form-item>
        <a-form-item label="扩散速度(Y)" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入扩散速度(Y)"
            v-decorator="['yvelocity', { rules: [{ required: true, message: '请输入扩散速度(Y)' }], initialValue: 0 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" />
        </a-form-item>
        <a-form-item label="扩散速度(Z)" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入扩散速度(Z)"
            v-decorator="['zvelocity', { rules: [{ required: true, message: '请输入扩散速度(Z)' }], initialValue: 0 }]"
            :formatter="(value) => `${value}米`" :parser="(value) => value.replace('米', '')" />
        </a-form-item>
        <a-form-item label="烟雾颜色" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
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
        gasFlowList: [],
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
      that.gasFlowList = store.state.bim.gasFlowList;
    },
    methods: {
      CreateGasFlow() {
        const that = this;
        that.form.resetFields() //清空表单
        that.$message.info('请点击拾取添加烟雾扩散的位置')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            that.setPosition = data
            that.visible = true;
            that.$message.success('拾取成功')
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
              "InitialSize": values.radius, //半径
              "MovingSpeed": [values.xvelocity, values.yvelocity, values.zvelocity], //XYZ轴移动速度
            }
            that.gasFlowList.push(params)
            api.Plugin.addEffectSteam(params);
            that.visible = false;
          }
        })
      },
      DelPointSource(item, index) {
        const that = this
        that.gasFlowList.splice(index, 1)
        api.Plugin.deleteEffectSteam(item.ID);
        that.$message.success('烟雾扩散删除成功')
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
      store.dispatch('GetGasFlowList', that.gasFlowList)
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
  }
</style>