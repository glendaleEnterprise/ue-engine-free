<template>
  <a-modal title="操作" :width="600" :visible="visible" :confirmLoading="confirmLoading" @ok="handleOk" @cancel="close">
    <a-form :form="form">
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="父级" disabled>
        <a-cascader
          :disabled="isEdit"
          v-model="organizationUnit.parentId"
          :options="organizationUnitList"
          placeholder="父级"
          expand-trigger="click"
          @change="onChange"
          change-on-select
          :fieldNames="{ label: 'displayName', value: 'id', children: 'children' }"
        />
        <input type="hidden" v-decorator="['parentId']" />
      </a-form-item>

      <!-- <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="编码">
        <a-input placeholder="组织机构编码" v-decorator="['code', {rules: [{ required: true, message: '组织机构编码不能为空' }]}]" readOnly />
      </a-form-item> -->

      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="名称">
        <a-input
          placeholder="组织机构名称"
          v-decorator="['displayName', { rules: [{ required: true, message: '组织机构名称不能为空' }] }]"
        />
      </a-form-item>
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="备注">
        <a-textarea
          :auto-size="{ minRows: 2, maxRows: 5 }"
          placeholder="备注"
          v-decorator="['extraProperties.Describe', { rules: [{ max: 500, message: '最多输入500字符' }] }]"
        />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script>
import pick from 'lodash.pick'
import { saveOrganizationUnit, getNextChildCode } from '@/api/system'
import { mapGetters } from 'vuex'
export default {
  name: 'OrganizationUnitModal',
  computed: {
    ...mapGetters(['organizationUnits']),
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
      visible: false,
      confirmLoading: false,
      organizationUnit: {},
      organizationUnitList: [],
      form: this.$form.createForm(this),
      isEdit: false,
    }
  },
  created() {
    this.$store.dispatch('getOrganizationUnitTrees')    
  },
  methods: {
    async add() {
      const code = await getNextChildCode(null)
      this.edit({
        id: undefined,
        parentId: undefined,
        displayName: undefined,
        code: code,
        'extraProperties.Describe': undefined,
      })
    },
    edit(record) {
      if (typeof(record.id)==='undefined') {
        this.isEdit = false
      } else {
        this.isEdit = true
      }

      this.organizationUnit = { ...record }       
    //   this.organizationUnits.filter((p)=>{
    //    return p.code != '00001'
    // })
      this.organizationUnitList = this.formatData(this.organizationUnits.filter((p)=>{
        return p.code != '00001'
      }))
      this.visible = true
      const data = { ...this.organizationUnit }

      this.$nextTick(() => {
        this.form.setFieldsValue(pick(data, 'displayName', 'parentId', 'extraProperties.Describe')) //从对象中获取想要的属性
        this.form.setFieldsValue({ 'extraProperties.Describe': data['describe'] || undefined })
      })
      const parentIds = []
      this.getTreeNodeIds(this.organizationUnitList, (data) => data.id === record.parentId, parentIds)
      this.organizationUnit.parentId = parentIds
    },
    close() {
      this.visible = false
    },
    handleOk() {
      const that = this
      // 触发表单验证
      this.form.validateFields((err, values) => {
        // 验证表单没错误
        if (!err) {
          that.confirmLoading = true
          const { parentId, ...data } = { ...that.organizationUnit }
          const formObj = Object.assign(data, values)
          saveOrganizationUnit(formObj)
            .then((res) => {
              that.$message.success('保存成功')
              that.$emit('ok')
              that.close()
            })
            .catch(() => {})
            .finally(() => {
              that.confirmLoading = false
            })
        }
      })
    },
    async onChange(e) {
      const [parentId] = [...e].reverse() //获取下拉框数组最后一位
      // const code = await getNextChildCode(parentId)
      // this.form.setFieldsValue({ code: code, parentId: parentId })
    },
    formatData(items) {
      const that = this
      items.forEach((item) => {
        item.disabled = item.id == that.organizationUnit.id
        if (item.children && item.children.length > 0) {
          that.formatData(item.children)
        } else {
          delete item.children
        }
      })
      return items
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

<style scoped>
</style>
