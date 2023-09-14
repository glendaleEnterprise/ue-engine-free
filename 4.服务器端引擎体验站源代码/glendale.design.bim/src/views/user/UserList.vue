<template>
  <a-modal v-model="show" title="ddddd" @cancel="cancel" @ok="onSubmit" width='1000' ok-text="确定" cancel-text="取消" :zIndex=1049>
<!--    <div>-->
<!--      <a-input placeholder="手机号码" v-model="queryParams.filter" style="width: 300px" />-->
<!--      <a-button type="primary" @click="fetch({ current: 1 })">查询</a-button>-->
<!--    </div>-->
<!--    <br/>-->
    <a-table
      :columns="columns"
      :row-key="(record) => record.id"
      :data-source="sourceData"
      bordered
      :pagination="pagination"
      size="middle"
      @change="handleTableChange"
      :rowSelection='{
        type:rowSelType,
         selectedRowKeys: selectedRowKeys,
         onChange: onSelectChange,
         onSelect: onSelect,
         onSelectAll: onSelectAll,
      }'
    >
      <template slot="serial" slot-scope="text, record, index">{{ index + 1 }}</template>

      <template slot="orgIds" slot-scope="text">{{ text.join('/') }}</template>
      <template slot="position" slot-scope="text" v-if="positions">{{
          text ? positions.find((x) => x.value == text).name : ''
        }}</template>

    </a-table>
  </a-modal>
</template>

<script>
import { delUser, getUserList } from '@/api/system'
import { getProjectUsers, hasJoin } from '@/api/project'
import { mapGetters } from 'vuex'
import { getPositionList } from '@/api/dictionary'

export default {
  name: 'UserList',
  computed: {
    ...mapGetters(['currProject']),
  },
  props:{
    projectId: {
      type: String,
    },
    rowSelType: {
      type: String,
      default:'radio'  //checkbox or radio
    },
  },
  data(){
    return {
      show:false,
      columns: [
        {
          title: '序号',
          width: '70px',
          align: 'center',
          scopedSlots: { customRender: 'serial' },
        },
        {
          title: '手机号码',
          dataIndex: 'phoneNumber',
          align: 'center',
          width: '150px',
        },
        {
          title: '姓名',
          dataIndex: 'name',
          align: 'center',
          width: '120px',
        },
        {
          title: '组织机构',
          dataIndex: 'orgNames',
          scopedSlots: { customRender: 'orgIds' },
          align: 'center',
          ellipsis: true,
        },
        {
          title: '职位',
          dataIndex: 'position',
          scopedSlots: { customRender: 'position' },
          align: 'center',
          width: '120px',
        },

      ],
      sourceData: [],
      positions: [],
      selectedRowKeys: [],
      queryParams: { filter: undefined, sorting: 'creationTime desc' },
      pagination: {
        current: 1,
        pageSize: 10,
      },
    }
  },
  async mounted() {
    this.fetch({ current: 1 })
    this.positions = await getPositionList()
  },
  methods:{
    fetch(params = {}) {
      const that = this
      this.pagination = Object.assign(this.pagination, params)
      this.queryParams.MaxResultCount = this.pagination.pageSize
      this.queryParams.SkipCount = (this.pagination.current - 1) * this.pagination.pageSize
      this.queryParams.projectId =  this.projectId
      getProjectUsers( this.queryParams).then((res) => {
        const pagination = { ...this.pagination }
        pagination.total = res.totalCount
        this.sourceData = res.items
        this.pagination = pagination
      })
    },
    handleTableChange(pagination) {
      const that = this
      this.pagination = pagination
      this.pagination.skipCount = (pagination.current - 1) * pagination.pageSize
      this.fetch({
        ...this.queryParams,
      })
    },
    onOk() {
      // 新增/修改 成功时，重载列表
      this.fetch({ current: 1 })
    },
    reset(record) {
      const that = this
      that.$confirm({
        title: `确定要重置用户 “${record.name}” 的密码为'123456'吗？`,
        onOk() {
          that.$refs.modal.reset(record)
        },
      })
    },
    deleteUser(record) {
      const that = this
      hasJoin(record.id).then((res) => {
        if (res) {
          that.$confirm({
            title: `确定要删除用户 “${record.name}” 吗？`,
            onOk() {
              delUser(record.id).then(() => {
                that.$message.success(that.$t('list.deleteSuccess'))
                that.resetCurrenPage()
                that.onOk()
              })
            },
          })
        } else {
          that.$warning({
            content: `用户“${record.name}”已经参与项目，在项目中移除后才能删除`,
            okText: that.$t('list.cancel'),
          })
        }
      })
    },
    onSelect(record, selected) {
      if (this.rowSelType === 'checkbox') {
        const rows = [...this.value]
        if (!selected) {
          rows.splice(
            rows.findIndex((x) => x.id == record.id),
            1
          )
          this.$emit('handlerUser', rows)
        } else {
          if (!this.value?.some((x) => x.id === record.id)) {
            const { id, name, phoneNumber, userName } = record
            rows.push({
              id,
              name,
              phoneNumber,
              userName,
             ...record
            })
            this.$emit('handlerUser', rows)
          }
        }
      } else {
        const { id, name, phoneNumber, userName } = record
        this.$emit('handlerUser', [
          {
            id,
            name,
            phoneNumber,
            userName,
            ...record
          },
        ])
      }
    },
    onSelectAll(selected, selectedRows, changeRows) {
      const rows = this.value || []
      if (selected) {
        this.$emit('input', [
          ...rows,
          ...changeRows.map((x) => ({
            id: x.id,
            name: x.name,
            phoneNumber: x.phoneNumber,
            position: x['extraProperties']['Position'],
            orgNames: x['extraProperties']['OrgIds'],
          })),
        ])
      } else {
        changeRows.forEach((row) => {
          rows.splice(
            rows.findIndex((x) => x.id == row.id),
            1
          )
          this.$emit('input', rows)
        })
      }
    },
    onSelectChange(selectedRowKeys) {
      this.selectedRowKeys = selectedRowKeys
    },
    resetCurrenPage() {
      const totalPage = Math.ceil((this.pagination.total - 1) / this.pagination.pageSize) // 总页数
      this.pagination.current = this.pagination.current > totalPage ? totalPage : this.pagination.current
      this.pagination.current = this.pagination.current < 1 ? 1 : this.pagination.current
    },
    showModal(){
      this.show=true
    },
    cancel(){
      this.show=false
    },
    onSubmit(){
      this.show=false
    }
  }
}
</script>

<style scoped>

</style>