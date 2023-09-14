<template>
  <div ref="wrap">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="DelAllFeature" @keyup.prevent @keydown.enter.prevent>全部删除</a-button>
      </a-space>
      <a-list size="small" :data-source="componentList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.Name }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelFeature(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="设置构件自转" :width="320" :visible="visible" @ok="SaveFeatureRotation" @cancel="handleCancel"
      cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :mask="false" :destroyOnClose="true"
      :maskClosable="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="旋转轴" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-select v-decorator="['axis', { rules: [{ required: true, message: '请选择旋转轴' }], initialValue: 'x' },]">
            <a-select-option value="x"> X轴 </a-select-option>
            <a-select-option value="y"> Y轴 </a-select-option>
            <a-select-option value="z"> Z轴 </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="旋转速度" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input-number placeholder="请输入旋转速度"
            v-decorator="['speed', { rules: [{ required: true, message: '请输入旋转速度' }], initialValue: 3 }]"
            :formatter="(value) => `${value}rad/s`" :parser="(value) => value.replace('rad/s', '')" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>
<script>
  import store from '@/store'
  export default {
    name: 'FeatureColor',
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        visible: false,
        form: this.$form.createForm(this, {
          name: 'coordinated'
        }),
        componentList: [],
        currentId: '',
      }
    },
    mounted() {
      this.componentList = store.state.bim.featureRotationList
      this.CreateFeatureRotation()
    },
    methods: {
      CreateFeatureRotation() {
        const that = this;
        that.$message.info('请点击拾取要旋转的构件')
        api.Feature.getByEvent(true, function (data) {
          if (data.split("^")[1]) {
            let index = that.componentList.findIndex(item => item.ID == data)
            if (index == -1) {
              that.currentId = data
              that.visible = true;
            } else {
              that.$message.info('当前构件已设置旋转效果')
            }
          }
        });
      },
      SaveFeatureRotation() {
        const that = this
        that.form.validateFields((err, values) => {
          if (!err) {
            let params = {
              "ID": that.currentId,
              "Name": that.currentId.split("^")[1],
              "Speed": values.speed,
              "Axis": values.axis,
            }
            that.componentList.push(params)
            api.Feature.autoRotate(
              that.currentId,
              values.axis == 'x' ? 1 : 0,
              values.axis == 'y' ? 1 : 0,
              values.axis == 'z' ? 1 : 0,
              values.speed
            );
            that.visible = false;
          }
        })
      },
      DelFeature(item, index) {
        const that = this
        api.Feature.autoRotate(
          item.ID,
          item.Axis == 'x' ? 1 : 0,
          item.Axis == 'y' ? 1 : 0,
          item.Axis == 'z' ? 1 : 0,
          0
        );
        api.Feature.original(item.ID, item.ID.split("^")[0])
        that.componentList.splice(index, 1)
        that.$message.success('删除成功')
      },
      handleCancel() {
        this.visible = false
      },
      DelAllFeature() {
        const that = this;
        if (that.componentList.length > 0) {
          that.componentList.forEach(item => {
            api.Feature.autoRotate(
              item.ID,
              item.Axis == 'x' ? 1 : 0,
              item.Axis == 'y' ? 1 : 0,
              item.Axis == 'z' ? 1 : 0,
              0
            );
            api.Feature.original(item.ID, item.ID.split("^")[0])
          })
          that.componentList = []
          that.$message.success('删除成功')
        }
      }
    },
    destroyed() {
      const that = this
      store.dispatch('GetFeatureRotationList', that.componentList)
      api.Feature.getByEvent(false); //关闭构件拾取事件
    }
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 70%;
    cursor: pointer;
    text-indent: 6px;
  }
</style>