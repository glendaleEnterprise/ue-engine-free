<template>
  <a-table :columns="columns" :data-source="data" bordered :showHeader="false" :pagination="false">
    <template slot="name" slot-scope="text">
      <a>{{ text }}</a>
    </template>
    <template slot="title"> 操作指南 </template>
  </a-table>
</template>

<script>
const columns = [
  {
    dataIndex: 'name',
  },
  {
    dataIndex: 'operation',
  },
]
const data = [
  {
    key: '1',
    name: '鼠标左键按住剖面框中心箭头线',
    operation: '拖动可平移剖面框，移出剖面框的部分将被剖切掉',
  },
  {
    key: '2',
    name: '鼠标左键按住剖面框中心箭头线之间的弧线',
    operation: '拖动可旋转剖面框，移出剖面框的部分将被剖切掉',
  }
]
export default {
  props: {
    projectMessage: {
      type: Object,
      default: undefined
    }
  },
  data() {
    return {
      data,
      columns,
    }
  },
  mounted() {
    api.Model.clipByBox(this.projectMessage.modelId);
  },
  destroyed() {
    api.Model.closeClip();
  },
}
</script>