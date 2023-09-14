<template>
  <a-card size="small" :bordered="false">
    <a-space slot="title">
      <a-input placeholder="关键字" style="width: 300px" v-model="queryParams.name" />
      <a-select
        v-model="userId"
        :dropdownClassName="'project-search-select'"
        :filter-option="filterOption"
        style="width: 180px"
        @change="userChange"
        :showSearch="true"
        placeholder="操作人员"
      >
        <a-select-option v-for="d in userData" :key="d.id">
          {{ d.name }}
        </a-select-option>
      </a-select>
      <a-button type="primary" @click="search">搜索</a-button>
      时间范围：
      <a-radio-group v-model="dateType" @change="dateChange">
        <a-radio-button value="0"> 当天 </a-radio-button>
        <a-radio-button value="7"> 7天内 </a-radio-button>
        <a-radio-button value="30"> 30天内 </a-radio-button>
        <a-radio-button value="180"> 180天内 </a-radio-button>
        <a-radio-button value="-1"> 全部 </a-radio-button>
      </a-radio-group>
    </a-space>
    <a-table
      size="middle"
      bordered
      :columns="columns"
      childrenColumnName="child"
      :data-source="data"
      :pagination="pagination"
      @change="handleTableChange"
      :rowKey="(record) => record.id"
      :loading="loading"
    >
      <template slot="no" slot-scope="text, record, index"> {{ pagination.skipCount + index + 1 }} </template>
    </a-table>
  </a-card>
</template>

<script>
const columns = [
  {
    title: '序号',
    width: '70px',
    align: 'center',
    scopedSlots: { customRender: 'no' },
  },
  {
    title: '操作时间',
    dataIndex: 'creationTime',
    key: 'creationTime',
    width: '170px',
    align: 'center',
  },
  {
    title: '操作人',
    dataIndex: 'userName',
    key: 'userName',
    width: '150px',
    align: 'center',
  },
  {
    title: '操作事件',
    dataIndex: 'name',
    key: 'name',
    width: '150px',
    align: 'center',
  },
  {
    title: '影响模块',
    dataIndex: 'moduleName',
    key: 'moduleName',
    width: '150px',
    align: 'center',
  },
  {
    title: '说明',
    dataIndex: 'content',
    key: 'content',
    ellipsis: true,
  },
]
import { GetLogsList } from '@/api/logs'
import { getAllUsers } from '@/api/system'
export default {
  name: 'Basic',
  components: {},
  data() {
    return {
      columns,
      loading: false,
      basicVisible: false,
      basic: undefined,       
      dates: [],
      queryParams: {
        keyword: undefined,
        startDate: undefined,
        endDate: undefined,
      },
      data: [],
      userId: undefined,
      userData: [],
      dateType: '0',
      pagination: {
        current: 0,
        total: 0,
        skipCount: 0,
        pageSize: 10, //每页中显示10条数据
        showTotal: (total) => `共 ${total} 条`, //分页中显示总的数据
      },
    }
  },
  async created() {
    await this.getUserData()
    await this.getData()
  },
  methods: {
    async getData() {
      this.loading = true
      const par = {
        userId: this.userId,
        dateType: this.dateType,
        keyword: this.queryParams.name,
        skipCount: this.pagination.skipCount,
        maxResultCount: this.pagination.pageSize,
      }
      const res = await GetLogsList(par)
      this.data = res.items

      this.pagination.total = res.totalCount
      this.loading = false
    },
    async getUserData() {
      await getAllUsers().then((res) => {
        this.userData = res
        this.userData.unshift({
          name:"全部",
          id:''
        }) 
      })
    },
    userChange() {
      this.search()
    },
    dateChange() {
      this.search()
    },
    handleTableChange(pagination) {
      this.pagination = pagination
      this.pagination.skipCount = (pagination.current - 1) * pagination.pageSize
      this.getData()
    },
    async search() {
      this.pagination.current = 0
      this.pagination.skipCount = 0
      this.getData()
    },
    filterOption(input, option) {
      return option.componentOptions.children[0].text.toLowerCase().indexOf(input.toLowerCase()) >= 0
    },
    handleChange(val) {
      console.log(`selected ${val}`)
    },
  },
}
</script>

<style lang="less" scoped>
</style>
