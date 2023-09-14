<template>
  <a-modal :title="modalTitle" :width="500" :visible="visible" :maskClosable="false" :confirmLoading="confirmLoading"
    @ok="handleOk" @cancel="handleCancel">
    <a-form-model layout="horizontal" :model="form" ref="refForm" :label-col="{ span: 4 }" :wrapper-col="{ span: 18 }"
      :rules="rules">
      <a-form-model-item label="所属目录" ref="parentId" prop="parentId">
        <a-cascader v-model="documentIds" :options="docTree" expand-trigger="hover" placeholder="所属目录" @change="onChange"
          change-on-select :fieldNames="{ label: 'title', value: 'key', children: 'children' }" />
        <a-input v-model="form.parentId" type="hidden" />
      </a-form-model-item>
    </a-form-model>
  </a-modal>
</template>
<script>
import { mapGetters } from 'vuex'
import { moveFile } from '@/api/document'

export default {
  props: {
    onOk: {
      type: Function,
      default: undefined,
    },
    moveFileVisible: {
      type: Boolean,
      default: false,
    },
    ids: {
      type: Array,
      default: undefined,
    },
    modalTitle: {
      type: String,
      default: '',
    },
  },
  computed: {
    ...mapGetters(['currProject', 'docTree']),
  },
  data() {
    return {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 5 },
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 },
      },
      visible: this.moveFileVisible,
      confirmLoading: false,
      documentIds: [],
      form: {
        id: undefined,
        projectId: '',
        parentId: '',
        docName: '',
        suffix: '',
        modelName: '',
        docSize: 0,
        docVer: '',
        docStatus: 0,
        sort: 0,
        remark: '',
        structureType: 0,
        syncID: '',
        isModel: true,
        openStatus: 1,
      },
      rules: {
        // parentId: [{ required: true, message: '请选择所属目录', trigger: 'blur' }],
      },
    }
  },
  created() {
    this.loadTree()
  },
  methods: {
    loadTree() {
      const parentIds = []
      this.getTreeNodeIds(this.docTree, (data) => data.key === null, parentIds)
      this.documentIds = parentIds
    },
    close() {
      this.$emit('update:moveFileVisible', false)
    },
    onChange(value) {
      const that = this
      that.form.parentId = value.length > 0 ? value[value.length - 1] : null
    },
    handleOk() {
      const that = this
      if (that.form.parentId) {
        that.$refs.refForm.validate((valid) => {
          if (valid) {
            that.moveFile()
          }
        })
      } else {
        that.$message.warning('请选择所属目录')
        return
      }

    },
    async moveFile() {
      const that = this
      that.confirmLoading = true
      await moveFile(that.form.parentId, this.ids)
        .then((res) => {
          that.close()
          that.$message.success('操作成功')
          that.$emit("update")
        })
        .catch((err) => {
          that.$message.warning('操作异常')
        })
        .finally(() => {
          that.confirmLoading = false
        })
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

<style scoped></style>
