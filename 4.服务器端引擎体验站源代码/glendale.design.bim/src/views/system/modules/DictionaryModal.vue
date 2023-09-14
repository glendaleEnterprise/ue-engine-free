<template>
  <a-modal
    v-model="thisVisible"
    :title="`${form.id ? '编辑' : '新增'}`"
    :maskClosable="false"
    :width="600"
    :confirmLoading="loading"
    @ok="onSubmit"
    @cancel="handleCancel"
  >
    <a-form-model ref="ruleForm" :model="form" :rules="rules" :label-col="labelCol" :wrapper-col="wrapperCol">
      <a-form-model-item label="父级" v-if="actionType == 1" ref="parentId" prop="parentId" required>
        <!-- <a-tree-select
          placeholder="根级"
          v-model="form.parentId"
          allowClear
          treeDataSimpleMode
          :tree-data="trees"
          :replaceFields="{ children: 'children', title: 'name', key: 'id', value: 'id' }"
          @change="selectChange"
        /> -->
        <a-select v-model="form.parentId" @change="selectChange">
          <a-select-option :value="item.id"
                           v-for="(item, index) in trees"
                           :key="index"
          >{{ item.name }}
          </a-select-option>
        </a-select>
      </a-form-model-item>
      <a-form-model-item label="名称" ref="name" prop="name" required>
        <a-input placeholder="请输入名称" v-model="form.name" />
      </a-form-model-item>
      <a-form-model-item label="值" ref="value" prop="value" required>
        <a-input placeholder="请输入值" v-model="form.value" />
      </a-form-model-item>
      <a-form-model-item label="排序">
        <a-input-number placeholder="请输入排序" v-model="form.orderIndex" style="width: 100%" />
      </a-form-model-item>
    </a-form-model>
  </a-modal>
</template>

<script>
import { add, edit, getTree } from '@/api/dictionary'
export default {
  name: 'BasicModel',
  props: {
    visible: {
      type: Boolean,
      default: false,
    },
    basic: {
      type: Object,
      default: undefined,
    },
    actionType: {
      type: Number,
      default: 0,
    },
  },
  data() {
    return {
      loading: false,
      thisVisible: this.visible,
      form: {
        id: undefined,
        parentId: undefined,
        value: undefined,
        name: undefined,
        orderIndex: undefined,
      },
      rules: {
        parentId: [{ required: true, message: '请选择', whitespace: true }],
        name: [{ required: true, message: '请输入名称', whitespace: true }],
        value: [{ required: true, message: '请输入值', whitespace: true }],
      },
      usageData: [],
      departmentData: [],
      floorData: [],
      modelData: [],
      trees: [],
      labelCol: { span: 6 },
      wrapperCol: { span: 14 },
    }
  },
  async created() {},
  mounted() {
    const basic = { ...this.basic }
    Object.assign(this.form, basic)
    this.getTrees(basic)
  },
  methods: {
    getTrees(basic) {
      var dd = this.actionType
      getTree().then((res) => {
        if (res != null && res.length > 0) {
          res.forEach((element) => {
            delete element.children
          })
        }
        this.trees = res.filter((p) => {
          return p.id != basic.id
        })
      })
    },
    onSubmit(e) {
      const that = this
      e.preventDefault()
      that.$refs.ruleForm.validate((valid) => {
        if (valid) {
          that.loading = true
          const formObj = { ...that.form }
          if (formObj.id) {
            edit(formObj)
              .then(that.afterForm)
              .catch(() => {
                that.loading = false
              })
          } else {
            add(formObj)
              .then(that.afterForm)
              .catch(() => {
                that.loading = false
              })
          }
        }
      })
    },
    afterForm() {
      this.loading = false
      this.$message.success('保存成功！')
      this.$emit('getBasics')
      this.$emit('update:visible', false)
    },
    handleCancel() {
      this.$emit('update:visible', false)
    },
    selectChange(value, label, extra) {
      this.form.parentId = value
    },
  },
}
</script>

<style lang="less" scoped>
</style>
