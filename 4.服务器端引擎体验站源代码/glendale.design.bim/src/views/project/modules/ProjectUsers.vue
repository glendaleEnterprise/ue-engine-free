<template>
  <a-modal title="用户选择" :width="600" :visible="visible" @ok="handleOk" @cancel="handleCancel">
    <a-table
      :columns="columns"
      :row-key="(record) => record.userId"
      :data-source="data"
      :pagination="false"
      size="middle"
      bordered
      :row-selection="{ selectedRowKeys: selectedRowKeys, onChange: onChangeUsers }"
    >
    </a-table>
  </a-modal>
</template>

<script>
import { mapGetters } from 'vuex'
import { getProjectUsers } from '@/api/project'
export default {
  name: 'ProjectUsers',
  computed: {
    ...mapGetters(['currProject']),
  },
  data() {
    return {
      columns: [
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
          align: 'center',
          ellipsis: true,
        },
      ],
      data: [],
      selectedRowKeys: [], //选中的行Id
      selectedRow: [], //选中的行集合
      visible: false,
    }
  },
  created() {
    getProjectUsers({ projectId: this.currProject.id, maxResultCount: 20 }).then((res) => {
      this.data = res.items
    })
  },
  methods: {
    showModal(selectedUsers) {
      this.visible = true      
      this.selectedRowKeys = selectedUsers?.map((x) => x.userId) || [] 
    },
    onChangeUsers(selectedRowKeys, row) {
      this.selectedRowKeys = selectedRowKeys
      this.selectedRow = row
    },
    handleOk() {
      this.$emit('change', this.selectedRow)
      this.visible = false
    },
    handleCancel() {
      this.visible = false
    },
  },
}
</script>