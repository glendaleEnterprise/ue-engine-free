<template>
  <div>
    <a-button @click="$refs.editFolder.add()" style='float: right; margin-bottom: 15px;z-index: 10;'>新增</a-button>
    <a-table size="middle" class="documentTable" bordered :columns="columns" :data-source="data" :pagination="pagination"
      @change="handleTableChange" :rowKey="(record) => record.key" :expandIconColumnIndex="1" :indentSize="20"
      :scroll="{ y: 276 }">
      <template slot="no" slot-scope="text, record, index">{{ pagination.skipCount + index + 1 }}</template>
      <template slot="projectFolderUsers" slot-scope="text">
        <a-tag v-for='(item, i) in text' :key='i'>{{ item.name }}</a-tag>
      </template>
      <template slot="action" slot-scope="text, record">
        <a-space>
          <a-tooltip title="新增">
            <a-button @click="$refs.editFolder.add(record.key, data)" icon="plus-circle" type="link"></a-button>
          </a-tooltip>
          <a-divider type="vertical" />
          <a-tooltip title="编辑">
            <a-button @click="$refs.editFolder.edit(record.key, data)" icon="form" type="link"></a-button>
          </a-tooltip>
          <a-divider type="vertical" />
          <a-tooltip title="删除">
            <a-button @click="delFolder(record)" icon="delete" type="link"></a-button>
          </a-tooltip>
        </a-space>
      </template>
    </a-table>
    <edit-folder :getTree="getTree" ref="editFolder" @ok="ok" :roles='roles' :projectId='projectId' />
  </div>
</template>
<script>
import EditFolder from './modules/EditFolderEdit.vue'
import { delFolder, getFolderTrees } from '@/api/project'
import { getSystemRoleList } from '@/api/system'
export default {
  name: 'FolderEdit',
  components: {
    'edit-folder': EditFolder,
  },
  props: {
    projectId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      columns: [
        {
          title: '序号',
          width: '70px',
          align: 'center',
          scopedSlots: { customRender: 'no' },
        },
        {
          title: '专业名称',
          dataIndex: 'title',
          key: 'title',
          width: '30%',
        },
        {
          title: '负责人',
          dataIndex: 'projectFolderUsers',
          width: '250px',
          align: 'center',
          scopedSlots: { customRender: 'projectFolderUsers' },
        },
        {
          title: '备注',
          dataIndex: 'remark',
          key: 'remark',
          ellipsis: true,
        },
        {
          title: '操作',
          key: 'action',
          width: '150px',
          align: 'center',
          scopedSlots: { customRender: 'action' },
        },
      ],
      level: 1,
      roles: [],
      queryParams: {
        parent: undefined,
        name: undefined,
        value: undefined,
      },
      data: [],
      pagination: {
        current: 0,
        total: 0,
        skipCount: 0,
        pageSize: 6, //每页中显示10条数据
        showTotal: (total) => `共 ${total} 条`, //分页中显示总的数据
      },
    }
  },
  async created() {
    await this.getTree()
    getSystemRoleList({ RoleType: '项目级' }).then((items) => {
      if (items.length > 0) {
        items.forEach((el) => {
          this.roles.push({ label: el.name, value: el.name })
        })
      }
    })
  },
  mounted() {
    const that = this
    // console.log(this.projectId)
    // this.$store.dispatch('GetDocTree', this.projectId)
  },

  methods: {
    getTree() {
      const that = this
      getFolderTrees(that.projectId).then((res) => {
        this.data = this.filterData(res)
        this.$emit('fetch')
      })
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
    handleTableChange(pagination) {
      this.pagination = pagination
      this.pagination.skipCount = (pagination.current - 1) * pagination.pageSize
      this.getTree()
    },
    ok() {
      this.getTree()
      // 新增/修改 成功时，重载列表
    },
    delFolder(data) {
      const that = this
      that.$confirm({
        title: `当前目录下所有模型文件将同步删除，确定要删除 “${data.title}” 吗？`,
        onOk() {
          delFolder(data.key).then(() => {
            that.$message.success('删除成功')
            that.$store.dispatch('GetDocTree', that.projectId) //更新状态
            that.getTree()
          })
        },
      })
    },
  },
}
</script>

<style lang="less" scoped>
.documentTable .ant-btn-icon-only {
  width: auto;
}

/deep/.ant-table-middle>.ant-table-content>.ant-table-body>table>.ant-table-tbody>tr>td {
  padding: 6.5px 8px;
}

/deep/.ant-table-middle>.ant-table-content>.ant-table-scroll>.ant-table-body>table>.ant-table-tbody>tr>td {
  padding: 6.5px 8px;
}
</style>