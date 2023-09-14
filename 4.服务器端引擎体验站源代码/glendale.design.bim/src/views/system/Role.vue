<template>
  <a-card size="small" :bordered="false">
    <a-space slot="title">
      <a-input placeholder="名称" v-model="queryParams.filter" style="width: 300px" />
      <a-button type="primary" @click="fetch">查询</a-button>
    </a-space>
    <a-button @click="$refs.modal.add()" slot="extra">添加角色</a-button>
    <a-table
      ref="table"
      size="middle"
      rowKey="id"
      :columns="columns"
      :data-source="data"
      bordered
      :showSizeChanger="false"
    >
      <span slot="serial" slot-scope="text, record, index">{{ index + 1 }}</span>
      <span slot="action" slot-scope="text, record">
        <a @click="$refs.range.show(record)">数据范围设置</a>
           <a-divider type="vertical" />
        <a @click="$refs.permission.show(record)">功能权限设置</a>
        <a-divider type="vertical" />
        <a @click="$refs.modal.edit(record)" :disabled="record.isStatic">编辑</a>
        <a-divider type="vertical" />
        <a @click="deleteRole(record)" :disabled="record.isStatic">删除</a>
      </span>
    </a-table>
    <permission-trees ref="permission" />
    <range-trees ref="range" />
    <role-modal ref="modal" @ok="handleOk" />
  </a-card>
</template>

<script>
import RoleModal from './modules/RoleModal'
import PermissionTrees from './modules/PermissionTrees'
import RangeTrees from './modules/RangeTrees'
import { getRoleList, delRole } from '@/api/system'

export default {
  name: 'Role',
  components: {
    RoleModal,
    PermissionTrees,
    RangeTrees,
  },
  watch: {},
  data() {
    return {
      visible: false,
      userVisible: false,
      organizationUnitId: undefined,
      queryParams: { filter: undefined, pageNo: 1, pageSize: 1000 },
      form: null,
      users: undefined,
      // 表头
      columns: [
        {
          title: '序号',
          width: '70px',
          align: 'center',
          scopedSlots: { customRender: 'serial' },
        },
        {
          title: '名称',
          width: '20%',
          dataIndex: 'name',
          align: 'center',
        },
        {
          title: '角色类型',
          width: '20%',
          align: 'center',
          dataIndex: 'extraProperties.RoleType',
        },
        {
          title: '描述',
          dataIndex: 'extraProperties.Describe',
        },
        {
          title: '操作',
          width: '300px',
          dataIndex: 'action',
          scopedSlots: { customRender: 'action' },
          align: 'center',
        },
      ],
      data: [],
    }
  },
  created() {
    this.fetch()
  },
  methods: {
    fetch() {
      getRoleList(this.organizationUnitId, Object.assign({}, this.queryParams)).then((res) => {
        this.data = res.items
      })
    },
    handleOk() {
      this.fetch()
    },
    deleteRole(record) {
      const that = this
      that.$confirm({
        title: `确定要删除角色 “${record.name}” 吗？`,
        onOk() {
          delRole(record.id).then(() => {
            //that.$refs.table.refresh()
            that.$message.success(that.$t('list.deleteSuccess'))
          })
        },
      })
    },
    saveUser(data) {
      console.log(data)
      this.$refs.selectUser.hide()
    },
  },
}
</script>
