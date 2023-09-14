<template>
  <a-card size="small" :bordered="false">
    <a-space slot="title">
      <a-input placeholder="请输入名称" style="width: 300px" v-model="queryParams.keyword" />
      <a-button type="primary" @click="search">查询</a-button>
    </a-space>
    <a-button type="primary" v-if="hostname == 'localhost'" @click="editBasicdata(null, 0)" slot="extra">新增字典</a-button>
    <a-button @click="editBasicdata(null, 1)" slot="extra">新增字典明细</a-button>
    <a-table
      size="middle"
      bordered
      :columns="columns"
      :data-source="data"
      :pagination="pagination"
      @change="handleTableChange"
      :rowKey="(record) => record.id"
      :loading="loading"
    >
      <template slot="no" slot-scope="text, record, index"> {{ index + 1 }} </template>
      <template slot="action" slot-scope="text, record">
        <a @click="editBasicdata(record, record.parentId == null ? 0 : 1)" :disabled="record.system"> 编辑 </a>
        <a-divider type="vertical" />
        <a @click="deleteSpace(record)" :disabled="record.system"> 删除 </a>
      </template>
    </a-table>
    <basic-model
      v-if="basicVisible"
      :visible.sync="basicVisible"
      :actionType="actionType"
      :basic="basic"
      @getBasics="fetch"
    />
  </a-card>
</template>

<script>
const columns = [
  {
    title: '序号',
    width: '100px',
    align: 'center',
    scopedSlots: { customRender: 'no' },
  },
  {
    title: '名称',
    dataIndex: 'name',
    key: 'name',
    width: '30%',
  },
  {
    title: '值',
    dataIndex: 'value',
    key: 'value',
  },
  {
    title: '操作',
    key: 'action',
    width: '160px',
    align: 'center',
    scopedSlots: { customRender: 'action' },
  },
]
import BasicModel from './modules/DictionaryModal.vue'
import { getList, deleted } from '@/api/dictionary'

export default {
  name: 'Basic',
  components: {
    BasicModel,
  },
  data() {
    return {
      columns,
      loading: false,
      hostname: window.location.hostname,
      basicVisible: false,
      basic: undefined,
      queryParams: {
        keyword: '',
      },
      data: [],
      pagination: { current: 1, pageSize: 10 },
      actionType: 0, //0=字典 1=字典明细
    }
  },
  created() {
    this.fetch()
  },
  methods: {
    fetch(params = {}) {
      this.loading = true
      getList({
        MaxResultCount: this.pagination.pageSize,
        SkipCount: (this.pagination.current - 1) * this.pagination.pageSize,
        ...params,
      }).then((res) => {
        const pagination = { ...this.pagination }
        pagination.total = res.totalCount
        this.loading = false
        this.data = this.filterData(res.items)
        this.pagination = pagination
      })
    },
    handleTableChange(pagination) {
      const pager = { ...this.pagination }
      pager.current = pagination.current
      this.pagination = pager
      this.fetch({
        MaxResultCount: pagination.pageSize,
        ...this.queryParams,
      })
    },
    search() {
      this.pagination.current = 1
      this.fetch(this.queryParams)
    },
    editBasicdata(data, actionType) {
      this.basic = data
      this.actionType = actionType
      this.basicVisible = true
    },
    deleteSpace(data) {
      const that = this
      that.$confirm({
        title: `确定要删除 “${data.name}” 吗？`,
        onOk() {
          deleted(data.id).then(() => {
            that.$message.success(that.$t('list.deleteSuccess'))             
            that.resetCurrenPage()
            that.fetch(that.queryParams)
          })
        },
      })
    },
    resetCurrenPage() {
      const totalPage = Math.ceil((this.pagination.total - 1) / this.pagination.pageSize) // 总页数
      this.pagination.current = this.pagination.current > totalPage ? totalPage : this.pagination.current
      this.pagination.current = this.pagination.current < 1 ? 1 : this.pagination.current
    },
    filterData(items) {
      items.forEach((item) => {
        if (item.children.length === 0) {
          delete item.children
        } else {
          this.filterData(item.children)
        }
      })
      return items
    },
  },
}
</script>

<style lang="less" scoped>
</style>
