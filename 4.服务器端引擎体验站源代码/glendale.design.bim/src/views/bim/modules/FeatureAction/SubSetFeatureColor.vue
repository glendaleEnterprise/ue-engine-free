<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="DelAllColor" @keyup.prevent @keydown.enter.prevent>全部删除</a-button>
      </a-space>
      <a-list size="small" :data-source="componentList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <span class="list-sign" :style="{ 'background': item.Color }"></span>
          <a-space class="viewpoint-title">{{ item.Name }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelFeature(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="设置构件颜色" :width="320" :visible="visible" @ok="SaveFeatureColor" @cancel="handleCancel"
      cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :mask="false" :destroyOnClose="true"
      :maskClosable="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="选择颜色" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }" class="color-picker">
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
        color: 'rgba(255,0,0,1)',
        colorRgba: 'rgba(255,0,0,1)',
        currentId: '',
        isMobile: false
      }
    },
    mounted() {
      this.isMobile = _isMobile() ? true :false
      // this.componentList = store.state.bim.featureColorList
      this.CreateFeatureColor()
    },
    methods: {
      CreateFeatureColor() {
        const that = this;
        that.$message.info('请点击拾取要设置颜色的构件')
        api.Feature.getByEvent(true, function (data) {
          if (data.split("^")[1]) {
            that.currentId = data
            that.visible = true;
          }
        });
      },
      SaveFeatureColor() {
        const that = this
        let index = that.componentList.findIndex(item => item.ID == that.currentId)
        let params = {
          "ID": that.currentId,
          "Name": that.currentId.split("^")[1],
          "Color": that.colorRgba,
        }
        index == -1 ? that.componentList.push(params) : that.componentList[index] = params;
        let list = JSON.parse(JSON.stringify(that.componentList));
        that.componentList = []
        that.componentList = list;
        that.currentId ? api.Feature.setColor(that.currentId, that.colorRgba, that.currentId.split("^")[0]) : null;
        that.visible = false;
      },
      DelFeature(item, index) {
        const that = this
        that.componentList.splice(index, 1)
        api.Feature.setColor(item.ID, "rgba(255,255,255,1)");
        that.$message.success('删除成功')
      },
      handleCancel() {
        this.visible = false
      },
      ColorPickerChange() {
        const that = this;
        that.colorRgba = colorHextoRGB(that.color)
      },
      DelAllColor() {
        const that = this;
        if (that.componentList.length > 0) {
          that.componentList.forEach(item => {
            api.Feature.setColor(item.ID, "rgba(255,255,255,1)");
          })
          that.componentList = []
          that.$message.success('删除成功')
        }
      }
    },
    destroyed() {
      const that = this
      that.DelAllColor();
      api.Feature.getByEvent(false); //关闭构件拾取事件
    }
  }
</script>
<style lang="less" scoped>
  .list-sign {
    width: 40px;
    height: 22px;
    display: inline-block;
  }

  .viewpoint-title {
    width: 60%;
    cursor: pointer;
    text-indent: 6px;
  }
</style>