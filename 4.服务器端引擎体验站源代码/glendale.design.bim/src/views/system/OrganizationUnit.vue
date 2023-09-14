<template>
  <a-card size="small" :bordered="false">
    <a-space slot="title">
      <a-input placeholder="名称" v-model="filter" style="width: 300px" />
      <a-button type="primary" @click="search()">查询</a-button>
    </a-space>
    <a-button @click="$refs.modal.add()" slot="extra">添加组织机构</a-button>
    <a-table
      ref="table"      
      size="middle"
      :rowKey="(record) => record.id"
      :columns="columns"
      :data-source="loadData"
      bordered
      @change="handleTableChange"
      :pagination="pagination"       
    >
      <span slot="serial" slot-scope="text, record, index">{{ index + 1 }}</span>
      <span slot="action" slot-scope="text, record">
        <a @click="$refs.modal.edit(record)" :disabled="record.system">编辑</a>
        <a-divider type="vertical" />
        <a @click="deleteOrganizationUnit(record)" :disabled="record.system">删除</a>
      </span>
    </a-table>
    <organization-unit-modal ref="modal" @ok="onOk"></organization-unit-modal>
  </a-card>
</template>

<script>
import OrganizationUnitModal from './modules/OrganizationUnitModal'
import { getOrganizationUnitList, delOrganizationUnit, getUserList } from '@/api/system'

export default {
  name: 'OrganizationUnit',
  components: {     
    OrganizationUnitModal,
  },
  data() {
    return {
      visible: false,
      filter: undefined, //搜索条件
      form: null,
      organizationUnit: {},
      // 表头
      columns: [
        {
          title: '序号',
          width: '120px',
          align: 'center',
          scopedSlots: { customRender: 'serial' },
        },
        // {
        //   title: '编码',
        //   dataIndex: 'code',
        //   width: '20%',
        // },
        {
          title: '名称',
          dataIndex: 'displayName',
        },
        {
          title: '备注',
          dataIndex: 'describe',
        },
        {
          title: '创建时间',
          dataIndex: 'creationTime',
          width: '250px',
          align: 'center',
        },
        {
          title: '操作',
          width: '150px',
          dataIndex: 'action',
          scopedSlots: { customRender: 'action' },
          align: 'center',
        },
      ],
      pagination: {
        current: 1,
        maxResultCount: 10, //每页中显示10条数据
      },
      loadData: [],
      loading:false,      
    }
  },
  created() {
    this.fetch()
  },
  mounted() {    
  },
  methods: {
    fetch(params = {}) {
      const that = this
      this.loading = true
      getOrganizationUnitList({
        maxResultCount: this.pagination.maxResultCount,
        skipCount: (this.pagination.current - 1) * this.pagination.maxResultCount,
        ...params,
      }).then(({ result }) => {
        const pagination = { ...this.pagination }
        pagination.total = result.totalCount
        that.pagination = pagination
        this.loading = false
        this.loadData = that.formatData(result.data)
      })
    },
    handleTableChange(pagination, filters, sorter) {
      const pager = { ...this.pagination }
      pager.current = pagination.current
      this.pagination = pager
      this.fetch({
        MaxResultCount: pagination.pageSize,
        sortField: sorter.field,
        sortOrder: sorter.order,
        ...filters,
      })
    },
    search() {
      this.pagination.current=1
      this.fetch({ filter: this.filter })
    },
    onOk() {
      this.fetch({ filter: this.filter })
      this.$store.dispatch('getOrganizationUnitTrees')
    },
    //重新计算当前页码（如果删除第二页最后一条，则刷新到上一页）
    resetCurrenPage() {
      const totalPage = Math.ceil((this.pagination.total - 1) / this.pagination.pageSize) // 总页数
      this.pagination.current = this.pagination.current > totalPage ? totalPage : this.pagination.current
      this.pagination.current = this.pagination.current < 1 ? 1 : this.pagination.current
    },
    deleteOrganizationUnit(record) {
      const that = this
      getUserList(record.id, { filter: undefined }).then(({ totalCount }) => {         
        if (totalCount == 0) {
          that.$confirm({
            title: `确定要删除组织机构 “${record.displayName}” 吗？`,
            onOk() {
              delOrganizationUnit(record.id).then(() => {
                that.resetCurrenPage()
                that.onOk()
                that.$message.success(that.$t('list.deleteSuccess'))
              })
            },
          })
        } else {
          that.$warning({
            content: `组织机构${record.displayName}有${totalCount}个用户，删除用户后才能执行此操作`,
            okText: that.$t('list.cancel'),
          })
        }
      })
    },
    formatData(items) {
      const that = this
      items.forEach((item) => {
        if (item.children && item.children.length > 0) {
          that.formatData(item.children)
        } else {           
          delete item.children
        }
      })
      return items
    },     
  },
}
</script>

<style scoped>
 
</style>