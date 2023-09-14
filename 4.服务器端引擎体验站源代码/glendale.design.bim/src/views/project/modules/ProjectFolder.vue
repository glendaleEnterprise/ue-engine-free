<template>
  <a-modal :title="modalTitle" :width="600" :visible="visible" :maskClosable="false" :confirmLoading="confirmLoading"
    @ok="handleOk" @cancel="handleCancel" ok-text="确定" cancel-text="取消">
    <a-form-model layout="horizontal" :model="form" ref="refForm" :label-col="{ span: 4 }" :wrapper-col="{ span: 19 }"
      :rules="rules">
      <a-form-model-item label="所属目录" ref="parentId" prop="parentId">
        <a-cascader :disabled="!isAdd" v-model="documentIds" :options="dataFolder" expand-trigger="hover"
          placeholder="根目录" @change="onChange" change-on-select
          :fieldNames="{ label: 'folderName', value: 'id', children: 'children' }" />
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
import { mapGetters } from 'vuex'
export default {
  name: 'ProjectFloder',
  computed: {
    ...mapGetters(['currProject', 'docTree']),
  },
  props: {
    dataFolder: {
      type: Array,
      default() {
        return []
      }
    },
  },
  data() {
    return {
      projectUsers: [],
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
        projectFolderUsers: [],
      },
      rules: {
        folderName: [{ required: true, message: '请输入名称', trigger: 'blur' }],
      },
      isAdd: false, //是否是新增操作
    }
  },
  methods: {
    add(pid) {
      this.modalTitle = '添加目录'
      this.isAdd = true
      this.visible = true
      const parentIds = []
      this.getTreeNodeIds(this.dataFolder, (data) => data.id === pid, parentIds)
      this.documentIds = parentIds
      this.form = {
        id: this.getUuid(),
        projectId: undefined,
        parentId: pid,
        folderName: undefined,
        remark: undefined,
      }
      this.projectUsers = []
    },
    async edit(res) {
      this.selectedRowKeys = []
      this.isAdd = false
      const that = this
      this.modalTitle = '编辑目录'
      this.visible = true
      this.form = res
      const parentIds = []
      this.getTreeNodeIds(this.dataFolder, (data) => data.id === res.parentId, parentIds)
      this.documentIds = parentIds
      this.projectUsers = res.projectFolderUsers
    },
    close() {
      this.visible = false
    },
    onChange(value) {
      const that = this
      that.form.parentId = value.length > 0 ? value[value.length - 1] : null
    },
    getUuid() {
      return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0
        const v = c === 'x' ? r : (r & 0x3 | 0x8)
        return v.toString(16)
      })
    },
    handleOk() {
      const that = this
      that.$refs.refForm.validate((valid) => {
        if (valid) {
          //检查是否添加人员
          that.confirmLoading = true
          that.form.projectFolderUsers = that.projectUsers.map((x) => ({
            userId: x.id,
            id: x.id,
            name: x.name,
            orgNames: x.orgNames,
            phoneNumber: x.phoneNumber,
            position: x.position,
            userName: x.userName,
            isAdmin: false,
          }))
          that.confirmLoading = false
          that.close()
          that.$emit('ok', that.form, this.isAdd)

        }
      })
    },
    handleCancel() {
      this.close()
    },
    getTreeNodeIds(tree, func, path) {
      //获取Id数组
      if (!tree) return []
      for (const data of tree) {
        path.push(data.id)
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
