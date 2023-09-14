<template>
  <a-modal title="操作" :width="600" :visible="visible" :confirmLoading="confirmLoading" @ok="handleOk" @cancel="handleCancel">
    <a-form :form="form">

      <!-- <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="组织机构">
        <a-cascader v-decorator="[ 'extraProperties.OrgId', {rules: [{ required: !role.isStatic, message: '组织机构不能为空' }]} ]" :options="organizationUnitList" placeholder="组织机构" :fieldNames="{ label: 'displayName', value: 'id', children: 'children' }" />
      </a-form-item> -->

      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="名称">
        <a-input placeholder="角色名称" v-decorator="['name', {rules: [{ required: true, message: '角色名称不能为空' }]}]" :maxLength="50" />
      </a-form-item>

      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="角色类别">
        <a-select v-decorator="['roleType',{ rules: [{ required: true, message: '请选择角色类别' }] },]" placeholder="请选择角色类别">
          <a-select-option value="公司级">公司级</a-select-option>
          <a-select-option value="项目级">项目级</a-select-option>
        </a-select>
      </a-form-item>

      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="描述">
        <a-input placeholder="角色描述" v-decorator="['describe']" :maxLength="20" />
      </a-form-item>

    </a-form>
  </a-modal>
</template>

<script>
import pick from 'lodash.pick'
// import { mapGetters } from 'vuex'
import { saveSystemRole } from '@/api/system'

export default {
  name: 'RoleModal',
  // computed: {
  //   ...mapGetters(['organizationUnits']),
  // },
  props: {
    organizationUnitId: {
      type: String,
      default: undefined
    },
  },
  data () {
    return {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 5 }
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 }
      },
      visible: false,
      confirmLoading: false,
      role: {},
      // organizationUnitList: [],
      form: this.$form.createForm(this),
    }
  },
  created () { },
  methods: {
    add () {
      this.edit({ id: undefined, name: undefined, extraProperties: undefined })
    },
    edit (record) {
      this.role = { ...record }
      this.role[`name`] = record.name;
      if(record.extraProperties){
        this.role[`roleType`] = record.extraProperties.RoleType;
        this.role[`describe`] = record.extraProperties.Describe;
      }
      this.visible = true
      // this.organizationUnitList = this.formatData(this.organizationUnits)
      this.$nextTick(() => {
        this.form.setFieldsValue(pick(this.role, 'name', 'extraProperties', 'roleType', 'describe'))
      })
    },
    close () {
      this.$emit('close')
      this.visible = false
    },
    handleOk () {
      const that = this
      // 触发表单验证
      this.form.validateFields((err, values) => {
        // 验证表单没错误
        if (!err) {
          that.confirmLoading = true
          const formObj = { ...that.role, ...values }
          // const [parentId] = [...values['extraProperties']['OrgId']].reverse()
          // formObj['extraProperties']['OrgId'] = parentId
          saveSystemRole(formObj).then(res => {
            that.$message.success('保存成功')
            that.$emit('ok')
            that.close()
          }).catch(() => {
          }).finally(() => {
            that.confirmLoading = false
          })
        }
      })
    },
    handleCancel () {
      this.close()
    },
    // formatData (items) {
    //   const that = this
    //   items.forEach(item => {
    //     if (item.children && item.children.length > 0) {
    //       that.formatData(item.children)
    //     } else {
    //       delete item.children
    //     }
    //   })
    //   return items
    // },
    // getTreeNodeIds (tree, func, path) {//获取Id数组
    //   if (!tree) return []
    //   for (const data of tree) {
    //     path.push(data.id)
    //     if (func(data)) return path
    //     if (data.children) {
    //       const findChildren = this.getTreeNodeIds(data.children, func, path)
    //       if (findChildren.length) return findChildren
    //     }
    //     path.pop()
    //   }
    //   return []
    // },
  }
}
</script>

<style scoped>
</style>
