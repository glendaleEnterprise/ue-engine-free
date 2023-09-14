<template>
  <a-modal
    :title="title"
    :width="400"
    :visible="visible"
    :maskClosable="false"
    :confirmLoading="confirmLoading"
    @ok="handleOk"
    @cancel="close"
  >
    <a-form-model
      layout="horizontal"
      :model="form"
      ref="refForm"
      :label-col="{ span: 8 }"
      :wrapper-col="{ span: 14 }"
      :rules="rules"
    >
      <a-form-model-item label="公开权限" ref="openStatus" prop="openStatus">
        <a-radio-group v-model="form.isPublic" @change="onChangeOpenStatus">
          <a-radio :value="true"> 公开 </a-radio>
          <a-radio :value="false"> 私有 </a-radio>
        </a-radio-group>
      </a-form-model-item>
    </a-form-model>
  </a-modal>
</template>
<script>
import { mapGetters } from 'vuex'
import { setPublic } from '@/api/document'
import {openStatusClamping} from '@/api/combine'

export default {
  props: {
    title: {
      type: String,
      default: '',
    },
  },
  computed: {
    ...mapGetters(['currProject', 'docTree']),
  },
  data() {
    return {
      visible: false,
      confirmLoading: false,
      docUsers: [],
      form: {
        isPublic: true,
        documentIds: undefined,
      },
      rules: {
        // parentId: [{ required: true, message: '请选择所属目录', trigger: 'blur' }],
      },
      doctype: ''
    }
  },
  methods: {
    open(rowIds,type) {
      if (rowIds.length === 0) this.$message.warning('请选择要公开的文件!')
      else {
        this.visible = true
        this.form = {
          isPublic: true,
          documentIds: rowIds,
        }
        this.doctype = type
      }
    },
    handleOk() {
      const that = this
      that.$refs.refForm.validate((valid) => {
        if (valid) {
          const formData = { ...that.form }
          that.confirmLoading = true
          if(that.doctype == 'clamping'){
            openStatusClamping({
                id: formData.documentIds,
                isOpen: formData.openStatus == 1?true:false
            })
            .then((res) => {
                that.$message.success('设置成功！')
                that.$emit('fetch')
                that.close()
            })
            .finally(() => {
                that.confirmLoading = false
            })
          }else{             
            setPublic(formData)
            .then(() => {
              that.$message.success('设置成功！')
              that.$emit('fetch')
              that.close()
            })
            .finally(() => {
              that.confirmLoading = false
            })
          }
        }
      })
    },
    close() {
      this.visible = false
    },
    onChangeOpenStatus(e) {
      this.form.isPublic = e.target.value
    },
  },
}
</script>

<style scoped>
</style>
