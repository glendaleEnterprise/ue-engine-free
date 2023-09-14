<template>
  <a-card size="small" :bordered="false">
    <a-button v-action:Design.Business.Folder.Add @click="$refs.editFolder.add()" slot="extra">新增</a-button>
    <a-table size="middle" class="documentTable" bordered :columns="columns" :data-source="data" :pagination="pagination"
      @change="handleTableChange" :rowKey="(record) => record.key" :expandIconColumnIndex="1" :indentSize="20">
      <template slot="no" slot-scope="text, record, index">{{ (pagination.current - 1) * pagination.pageSize + index + 1
      }}</template>
      <template slot="action" slot-scope="text, record">
        <a-space>
          <a-tooltip title="新增">
            <a-button v-action:Design.Business.Folder.Add @click="$refs.editFolder.add(record.key)" icon="plus-circle"
              type="link"></a-button>
          </a-tooltip>
          <a-tooltip title="编辑">
            <a-button v-action:Design.Business.Folder.Update @click="$refs.editFolder.edit(record.key)" icon="form"
              type="link"></a-button>
          </a-tooltip>
          <a-tooltip title="删除">
            <a-button v-action:Design.Business.Folder.Delete @click="delFolder(record)" icon="delete"
              type="link"></a-button>
          </a-tooltip>
        </a-space>
      </template>
    </a-table>
    <edit-folder :getTree="getTree" ref="editFolder" @ok="ok" />
  </a-card>
</template>
<script>
import { mapGetters } from 'vuex'
import EditFolder from './modules/EditFolder.vue'
import { delFolder, getFolderTrees } from '@/api/project'
export default {
  name: 'Folder',
  components: {
    'edit-folder': EditFolder,
  },
  computed: {
    ...mapGetters(['currProject']),
  },
  data() {
    return {
      columns: [
        {
          title: '序号',
          width: '90px',
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
          title: '备注',
          dataIndex: 'remark',
          key: 'remark',
          ellipsis: true,
        },
        {
          title: '操作',
          key: 'action',
          width: '180px',
          align: 'center',
          scopedSlots: { customRender: 'action' },
        },
      ],
      level: 1,
      queryParams: {
        parent: undefined,
        name: undefined,
        value: undefined,
      },
      data: [],
      pagination: {
        current: 1,
        total: 0,
        skipCount: 0,
        pageSize: 10, //每页中显示10条数据
        showTotal: (total) => `共 ${total} 条`, //分页中显示总的数据
      },
    }
  },
  async mounted() {
    const that = this
    that.$store.dispatch('GetDocTree', that.currProject.id)
    await that.getTree()
  },

  methods: {
    getTree() {
      const that = this
      getFolderTrees(that.currProject.id).then((res) => {
        that.data = that.filterData(res)
        that.pagination.total = that.data.length
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
    ok(state) {
      const that = this
      if (state) {
        that.pagination.total++
        if (that.pagination.total > that.pagination.current * that.pagination.pageSize && (that.pagination.current * that.pagination.pageSize) % that.pagination.total != 0) {
          that.pagination.current++
        }
      }
      that.getTree()
      // 新增/修改 成功时，重载列表
    },
    delFolder(data) {
      const that = this
      that.$confirm({
        title: `当前目录下所有模型文件将同步删除，确定要删除 “${data.title}” 吗？`,
        onOk() {
          delFolder(data.key).then(() => {
            that.$message.success('删除成功')
            that.$store.dispatch('GetDocTree', that.currProject.id) //更新状态
            that.pagination.total--
            if (that.pagination.total < that.pagination.current * that.pagination.pageSize && (that.pagination.current * that.pagination.pageSize) % that.pagination.total == 0 && that.pagination.current != 1) {
              that.pagination.current--
            }
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
</style>
