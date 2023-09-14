<template>
  <div class="side-frame">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="DeleteAllList" @keyup.prevent @keydown.enter.prevent>全部删除</a-button>
      </a-space>
      <a-list size="small" :data-source="componentList" :locale="{ emptyText: `暂无数据` }" class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-switch v-model="item.isHidden" @change="onChangeHidden($event, index)" checked-children="显示"
            un-checked-children="隐藏" />
          <a-space @click="ZoomViewpoint(item)" class="list-title"> {{ item.id.split("^")[1] }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="onDelete(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
  </div>
</template>
<script>
import store from '@/store'
export default {
  name: 'FeatureVisible',
  data() {
    return {
      componentList: []
    }
  },
  props: {
    projectMessage: {
      type: Object,
      default: undefined,
    },
  },
  mounted() {
    // this.componentList = store.state.bim.featureVisibleList
    this.GetComponents()
  },
  methods: {
    GetComponents() {
      const that = this
      that.$message.info('请点击拾取构件')
      api.Feature.getByEvent(true, (featureId) => {
        if (featureId.split("^")[1]) {
          //判断是否存在componentList中
          var clickComponent = that.componentList.find((e) => {
            return e.id === featureId
          })
          if (clickComponent) {
            clickComponent.isHidden = false
          } else {
            that.componentList.push({
              id: featureId,
              isHidden: false
            })
          }
          api.Feature.setVisible(featureId, false, featureId.split("^")[0])
        }
      })
    },
    onChangeHidden(checked, index) {
      const that = this
      that.componentList[index].isHidden = checked;
      api.Feature.setVisible(that.componentList[index].id, checked, that.componentList[index].id.split("^")[0])
    },
    onDelete(data, index) {
      const that = this;
      api.Feature.setVisible(data.id, true, data.id.split("^")[0])
      that.componentList.splice(index, 1)
    },
    DeleteAllList() {
      const that = this;
      that.componentList.forEach(item => {
        api.Feature.setVisible(item.id, true, item.id.split("^")[0])
      })
      that.componentList = []
    }
  },
  destroyed() {
    const that = this;
    api.Feature.getByEvent(false) //关闭构件拾取事件
    // store.dispatch('GetFeatureVisibleList', that.componentList)
    that.DeleteAllList()
  }
}
</script>

<style lang="less" scoped>
.list-title {
  width: 50%;
  white-space: nowrap;
  text-overflow: ellipsis;
  overflow: hidden;
}
</style>