<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateSelfLuminous" @keyup.prevent @keydown.enter.prevent>新建自发光</a-button>
      </a-space>
      <a-list size="small" :data-source="selfLuminousList" :locale="{ emptyText: `暂无数据` }"
        class="label-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-switch @change="SelfLuminousVisible($event, item)" v-model="item.state" />
          <a-space class="attribute-title">{{ item.name }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelSelfLuminous(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="自发光效果" :width="320" :visible="visible" @ok="SaveSelfLuminous" @cancel="handleCancel"
      cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap">
      <a-form :form="formFolder">
        <a-form-item label="发光名称" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <a-input placeholder="请输入发光名称" v-decorator="['title', { rules: [{ required: true, message: '请输入发光名称' }] }]" />
        </a-form-item>
        <a-form-item label="光照强度" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <a-input-number placeholder="请输入光照强度" :min="3" :max="1000"
            v-decorator="['strength', { rules: [{ required: true, message: '请输入光照强度' }], initialValue: 6 }]" />
        </a-form-item>
        <a-form-item label="发光颜色" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }" class="color-picker">
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
    _isMobile
  } from '@/api/public'
  export default {
    name: 'LabelManagement',
    data() {
      return {
        selfLuminousList: [], //列表
        visible: false, //创建框
        color: 'rgba(255,255,0,1)',
        colorRgba: 'rgba(255,255,0,1)',
        formFolder: this.$form.createForm(this, {
          name: 'coordinated2'
        }),
        currentId: undefined,
        isMobile: false
      }
    },
    props: {
      projectMessage: {
        type: Object,
        default: undefined,
      },
    },
    mounted() {
      const that = this;
      this.isMobile = _isMobile() ? true : false;
      that.formFolder.resetFields(); //清空表单
      that.selfLuminousList = store.state.bim.featureSelfLuminousList;
    },
    methods: {
      CreateSelfLuminous() {
        const that = this;
        that.formFolder.resetFields(); //清空表单
        that.$message.info('请点击选择自发光构件')
        api.Feature.getByEvent(true, (featureId) => {
          api.Feature.setColor(featureId, "rgba(255,255,0,1)");
          that.currentId = featureId;
          that.visible = true;
          api.Feature.getByEvent(false);
        });
      },
      SaveSelfLuminous() {
        const that = this
        that.formFolder.validateFields((err, values) => {
          if (!err) {
            api.Feature.setColor(that.currentId, "rgba(255,255,255,1)");
            const params = {
              modelId: that.currentId.split("^")[0],
              featureId: that.currentId,
              color: that.colorRgba,
              strength: values.strength,
              name: values.title,
              state: true
            }
            api.Feature.setEmissiveColor(params);
            let index = that.selfLuminousList.findIndex(item => item.featureId == that.currentId.split("^")[1])
            index == -1 ? that.selfLuminousList.push(params) : that.selfLuminousList[index] = params;
            that.currentId = undefined;
            that.visible = false;
          }
        })
      },
      DelSelfLuminous(data, index) {
        const that = this;
        data.state = false
        let parame = {
          ...data
        }
        parame.strength = 0;
        api.Feature.setEmissiveColor(parame);
        that.selfLuminousList.splice(index, 1)
        that.$message.success('删除成功')
      },
      handleCancel() {
        const that = this
        that.visible = false;
        that.formFolder.resetFields() //清空表单
        that.currentId ? api.Feature.setColor(that.currentId, "rgba(255,255,255,1)") : null
        that.currentId = undefined;
      },
      SelfLuminousVisible(state, data) {
        const that = this;
        if (state) {
          data.state = true
          api.Feature.setEmissiveColor(data);
        } else {
          data.state = false;
          let parame = {
            ...data
          }
          parame.strength = 0;
          api.Feature.setEmissiveColor(parame);
        }
      },
      ColorPickerChange() {
        const that = this;
        that.colorRgba = colorHextoRGB(that.color)
      }
    },
    destroyed() {
      const that = this;
      api.Feature.getByEvent(false);
      that.currentId ? api.Feature.setColor(that.currentId, "rgba(255,255,255,1)") : null
      store.dispatch('GetFeatureSelfLuminousList', that.selfLuminousList)
    }
  }
</script>
<style lang='less' scoped>
  .attribute-title {
    width: 50%;
    cursor: pointer;
  }
</style>