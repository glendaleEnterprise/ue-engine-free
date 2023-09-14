<template>
  <a-modal :title="modalTitle" :width="600" :visible="visible" :maskClosable="false" :confirmLoading="confirmLoading"
    @ok="handleOk" @cancel="handleCancel" ok-text="确定" cancel-text="取消">
    <a-form-model layout="horizontal" :model="form" ref="refForm" :label-col="{ span: 4 }" :wrapper-col="{ span: 19 }"
      :rules="rules">
      <a-form-model-item label="所属目录" ref="parentId" prop="parentId">
        <a-cascader :disabled="!isAdd" v-model="documentIds" :options="dataFolder" expand-trigger="hover"
          placeholder="根目录" @change="onChange" change-on-select
          :fieldNames="{ label: 'title', value: 'key', children: 'children' }" />
        <a-input v-model="form.parentId" type="hidden" />
      </a-form-model-item>
      <a-form-model-item label="目录名称" ref="folderName" prop="folderName">
        <a-input v-model="form.folderName" :maxLength="50" :allowClear="true" placeholder="目录名称"> </a-input>
      </a-form-model-item>
      <a-form-model-item label="备注" ref="remark" prop="remark">
        <a-textarea v-model="form.remark" style="height: 50px" placeholder="备注" />
      </a-form-model-item>
    </a-form-model>
  </a-modal>
</template>
<script>
import { getFolder, getFolderTrees, addFolder, updateFolder } from '@/api/project'
export default {
  name: 'DocEditFloder',
  props: {
    projectId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      modalTitle: '',
      visible: false,
      confirmLoading: false,
      documentIds: [],
      form: {
        id: undefined,
        projectId: undefined,
        parentId: undefined,
        folderName: undefined,
        remark: undefined,
      },
      rules: {
        folderName: [{ required: true, message: '请输入名称', trigger: 'blur' }],
      },
      isAdd: false, //是否是新增操作
      dataFolder: [],
    }
  },
  methods: {
    add(pid, tree) {
      this.modalTitle = '添加目录'
      this.isAdd = true
      this.visible = true
      const parentIds = []
      this.getTreeNodeIds(tree, (data) => data.key === pid, parentIds)
      this.documentIds = parentIds
      this.form = {
        id: undefined,
        projectId: this.projectId,
        parentId: pid,
        folderName: undefined,
        remark: undefined,
      }
      this.getFolderTree()
    },
    async edit(id, tree) {
      this.isAdd = false
      const that = this
      if (id) {
        getFolder(id).then((res) => {
          that.form = res
        })
        this.modalTitle = '编辑目录'
        this.visible = true
        const res = await getFolder(id)
        this.form = res
        const parentIds = []
        this.getTreeNodeIds(tree, (data) => data.key === res.parentId, parentIds)
        this.documentIds = parentIds
      } else {
        this.$message.warning('请选择要编辑的目录!')
      }
      that.getFolderTree()
    },
    getFolderTree() {
      const that = this
      getFolderTrees(that.projectId).then((res) => {
        that.dataFolder = that.filterFolderData(res)
      })
    },
    filterFolderData(items) {
      items.forEach((item) => {
        if (item.children.length === 0) {
          delete item.children
        } else {
          this.filterFolderData(item.children)
        }
      })
      return items
    },
    close() {
      this.visible = false
    },
    onChange(value) {
      const that = this
      that.form.parentId = value.length > 0 ? value[value.length - 1] : null
    },
    handleOk() {
      const that = this
      that.$refs.refForm.validate((valid) => {
        if (valid) {
          //检查是否添加人员
          that.confirmLoading = true
          if (typeof that.form.id === 'undefined') {
            addFolder(that.form)
              .then(that.afterForm)
              .catch(() => {
                that.confirmLoading = false
              })
          } else {
            updateFolder(that.form.id, that.form)
              .then(that.afterForm)
              .catch(() => {
                that.confirmLoading = false
              })
          }
        }
      })
    },
    afterForm() {
      this.confirmLoading = false
      this.$message.success('保存成功！')
      this.$store.dispatch('GetDocTree', this.projectId)
      this.$refs.refForm.resetFields()
      this.$emit('ok')
      this.close()
    },
    handleCancel() {
      this.close()
    },
    getTreeNodeIds(tree, func, path) {
      //获取Id数组
      if (!tree) return []
      for (const data of tree) {
        path.push(data.key)
        if (func(data)) return path
        if (data.children) {
          const findChildren = this.getTreeNodeIds(data.children, func, path)
          if (findChildren.length) return findChildren
        }
        path.pop()
      }
      return []
    },
  },
}
</script>
<style lang="less" scoped>
/deep/.clickRow {
  background-color: #dfdfdf;
}

/deep/.ant-table-middle>.ant-table-content>.ant-table-scroll>.ant-table-body>table>.ant-table-tbody>tr>td {
  padding: 8px;
}
</style>
