<template>
  <div>
    <a-tabs v-model="tab" size="small" :animated="false" @change="PropertyChange">
      <a-tab-pane v-for="value in attributeType" :key="value.key" :tab="value.tab">
        <div class="viewpoint-list scroll-box">
          <a-table :columns="columns" v-if="choose && attributeList.length > 0" :data-source="tableList"
            :showHeader="false" bordered :rowKey="(record, index) => {
                return index
              }
              " :pagination="false">
          </a-table>
          <span v-else class="tips">{{ attributeInformation }}</span>
        </div>
      </a-tab-pane>
    </a-tabs>
  </div>
</template>
<script>
import { getFeatureProperty } from '@/api/model'
const columns = [
  {
    dataIndex: 'propertyName',
    key: 'propertyName',
    customRender: (text, row, index) => {
      const obj = {
        children: text,
        attrs: {},
      }
      if (row.index == 'space') {
        obj.attrs.colSpan = 2
      }
      return obj
    },
    width: 50,
  },
  {
    dataIndex: 'value',
    key: 'value',
    customRender: (value, row, index) => {
      const obj = {
        children: value,
        attrs: {},
      }
      if (row.index == 'space') {
        obj.attrs.colSpan = 0
      }
      return obj
    },
    width: 50,
  },
]
export default {
  name: 'FeatureProperties',
  data() {
    return {
      attributeList: [],
      tableList: [],
      selFeatureId: '',
      typeList: [],
      tab: 'properties',
      choose: false,
      columns,
      attributeType: [
        {
          key: 'properties',
          tab: '属性',
        },
        {
          key: 'type',
          tab: '类型',
        },
      ],
      attributeInformation: '点击查询构件~',
    }
  },
  props: {
    projectMessage: {
      type: Object,
      default: null,
    },
  },
  async mounted() {
    await this.GetComponentProperties()
  },
  methods: {
    GetComponentProperties() {
      const that = this
      that.$message.info('请点击拾取构件')
      api.Feature.getByEvent(true, (featureId) => {
        if (featureId.split("^")[1]) {
          if (that.selFeatureId) api.Feature.setColor(that.selFeatureId, "rgba(255,255,255,1)");
          api.Feature.setColor(featureId, "rgba(255,255,0,1)");
          that.selFeatureId = featureId;
          that.DataFiltering(featureId)
          that.attributeInformation = '查询中，请稍候~'
        }
      })
    },
    async DataFiltering(id) {
      const that = this
      that.attributeList = []
      that.tab = 'properties'
      const gild = id.split('^')
      let modelName = ''
      that.projectMessage.modelList.forEach((item) => {
        if (item.id == gild[0]) {
          modelName = item.modelName
          return modelName
        }
      })
      if (!modelName || !gild[1]) {
        return false
      }
      const res = await getFeatureProperty({ externalId: gild[1].split('_')[0], modelName: modelName })
      if (res.length == 0) {
        that.attributeInformation = '无数据 ~'
        return
      }

      const type = []
      let typeName = ''
      for (let i = 0; i < res.length; i++) {
        var propertySetName = res[i].propertySetName
        delete res[i].children
        if (type.indexOf(res[i].propertyTypeName) == -1) {
          that.attributeList.push({
            type: res[i].propertyTypeName,
            content: [{ propertyName: res[i].propertySetName, index: 'space' }, res[i]],
            setName: [res[i].propertySetName],
          })
          typeName = propertySetName
          type.push(res[i].propertyTypeName)
        } else {
          var typeIndex = type.indexOf(res[i].propertyTypeName)
          if (propertySetName == typeName) {
            that.attributeList[typeIndex].content.push(res[i])
          } else {
            that.attributeList[typeIndex].content.push({ propertyName: res[i].propertySetName, index: 'space' })
            that.attributeList[typeIndex].content.push(res[i])
            typeName = propertySetName
          }
        }
      }
      that.choose = true
      that.tableList = that.attributeList[0].content.filter(function (re) {
        return (
          re.propertySetName != '几何中心点' &&
          re.propertySetName != '包围盒顶点坐标' &&
          re.propertyName != '几何中心点' &&
          re.propertyName != '包围盒顶点坐标'
        )
      })
      that.tableList.unshift({
        id: 0,
        propertyName: "构件ID",
        value: id.split("^")[1]
      })
      that.attributeList[0].content.unshift({
        id: 0,
        propertyName: "构件ID",
        value: id.split("^")[1]
      })
      that.attributeList[1].content.unshift({
        id: 0,
        propertyName: "构件ID",
        value: id.split("^")[1]
      })
    },
    PropertyChange(activeKey) {
      const that = this
      if (activeKey == 'properties') {
        if (that.attributeList[0] && that.attributeList[0].content.length > 0) {
          that.tableList = that.attributeList[0].content
        } else {
          that.attributeInformation = '暂无数据~'
        }
      } else {
        if (that.attributeList[1] && that.attributeList[1].content.length > 0) {
          that.tableList = that.attributeList[1].content
        } else {
          that.attributeInformation = '暂无数据~'
        }
      }
    },
  },
  destroyed() {
    if (this.selFeatureId) {
      api.Feature.setColor(this.selFeatureId, "rgba(255,255,255,1)");
    }
    api.Feature.getByEvent(false) //关闭构件拾取事件
  },
}
</script>

<style lang="less" scoped>
/deep/.ant-tabs {
  .ant-tabs-bar {
    margin-bottom: -16px;
  }

  .ant-tabs-nav {
    width: 100%;

    >div {
      .ant-tabs-tab {
        padding: 8px 16px;
        width: 50%;
        margin: 0;
        text-align: center;
      }
    }

  }
}

.tips {
  display: inline-block;
  padding: 10px;
}
</style>

