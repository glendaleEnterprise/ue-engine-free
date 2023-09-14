<template>
  <a-modal
    title="编辑用户"
    :width="550"
    :visible="visible"
    :confirmLoading="confirmLoading"
    @ok="handleOk"
    @cancel="handleCancel"
  >
    <a-form :form="form" class="form" :label-col="{ span: 5 }" :wrapper-col="{ span: 16 }">
      <a-form-item label="组织机构">
        <a-cascader
          placeholder="请选择"
          v-decorator="[
            'extraProperties.OrgIds',
            {
              rules: [{ required: user.userName !== 'admin', message: '组织机构不能为空' }],
            },
          ]"
          :options="organizationUnitList"
          change-on-select
          :fieldNames="{ label: 'displayName', value: 'id', children: 'children' }"
        />
      </a-form-item>
      <a-form-item label="职位">
        <a-select
          v-decorator="['extraProperties.Position']"
          show-search
          placeholder="职位"
          option-filter-prop="children"
          :style="{ width: '100%' }"
          :filter-option="filterOption"
          :allowClear="true"
        >
          <a-select-option :value="position.value" v-for="position in positions" :key="position.id">
            {{ position.name }}
          </a-select-option>
        </a-select>
      </a-form-item>
      <a-form-item v-show="false" label="账户">
        <a-input
          placeholder="账户"
          v-decorator="[
            'userName',
            {
              rules: [{ required: false, message: '不能为空' }],
            },
          ]"
        />
      </a-form-item>
      <a-form-item label="手机号">
        <a-input
          placeholder="手机号"
          v-decorator="[
            'phoneNumber',
            {
              rules: [
                { required: true, message: '手机号码不能为空' },
                { pattern: /^1[3456789]\d{9}$/, message: '输入手机号有误' },
              ],
            },
          ]"
        />
      </a-form-item>
      <a-form-item label="姓名">
        <a-input placeholder="姓名" v-decorator="['name', { rules: [{ required: true, message: '姓名不能为空' }] }]" />
      </a-form-item>
      <a-form-item label="登录密码" v-if="user.id == undefined">
        <a-input
          placeholder="初始密码:123456"
          v-decorator="['password', { rules: [{ required: true, message: '不能为空' }] }]"
          :disabled="true"
        />
      </a-form-item>
      <a-form-item label="角色">
        <a-checkbox-group
          :options="roles"
          v-decorator="[
            'roleNames',
            {
              rules: [{ required: true, message: '请选择角色' }],
            },
          ]"
        />
      </a-form-item>
      <a-form-item label="有效期">
        <a-date-picker v-decorator="['extraProperties.ValidityDate']" />
      </a-form-item>
      <a-form-item label="备注">
        <a-textarea
          placeholder="最多500字符"
          v-decorator="['extraProperties.Describe', { rules: [{ max: 500, message: '最多输入500字符' }] }]"
          :auto-size="{ minRows: 2, maxRows: 4 }"
        />
      </a-form-item>
    </a-form>
  </a-modal>
</template>
<script>
import pick from 'lodash.pick'
import { mapGetters } from 'vuex'
import { getPositionList } from '@/api/dictionary'
import {
  saveUser,
  getOrganizationUnitByUserId,
  getRolesByUserId,
  resetPassword,
  getParents,
  getRoleAll,
} from '@/api/system'

export default {
  name: 'LeaderModal',
  computed: {
    ...mapGetters(['organizationUnits']),
  },
  props: {
    organizationUnitId: {
      type: String,
      default: undefined,
    },
    isOutsideUser: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      visible: false,
      confirmLoading: false,
      positions: [],
      roles: [],
      user: {},
      organizationUnitList: [],
      form: this.$form.createForm(this),
    }
  },
  async created() {
    const that = this
    this.positions = await getPositionList()
    getRoleAll().then(({ items }) => {
      if (items.length > 0) {
        items.forEach((el) => {
          that.roles.push({ label: el.name, value: el.name })
        })
      }
    })
  },
  methods: {
    add() {
      const parentIds = []
      var uid = this.uuid()
      this.getTreeNodeIds(this.organizationUnits, (data) => data.id === this.organizationUnitId, parentIds)
      this.edit({
        id: undefined,
        email: '@x.x',
        extraProperties: { OrgIds: parentIds, Position: undefined },
        userName: uid, //生成唯一字符串
        name: undefined,
        phoneNumber: undefined,
        password: '123456',
        roleNames: undefined,
        lockoutEnabled: true,
      })
    },
    edit(record) {
      const that = this
      this.user = { ...record }
      this.visible = true
      this.organizationUnitList = this.formatData(this.organizationUnits)

      if (this.isOutsideUser) {
        that.organizationUnitList = this.organizationUnits.filter((p) => {
          return p.code === '00001'
        })
      } else {
        that.organizationUnitList = this.organizationUnits.filter((p) => {
          return p.code != '00001'
        })
      }

      this.$nextTick(() => {
        this.form.setFieldsValue(
          pick(
            this.user,
            'extraProperties.OrgIds',
            'extraProperties.Position',
            'extraProperties.Describe',
            'userName',
            'name',
            'phoneNumber',
            'password',
            'roleNames'
          )
        )
        this.form.setFieldsValue({ 'extraProperties.Position': this.user['extraProperties']['Position'] || undefined })
        this.form.setFieldsValue({ 'extraProperties.Describe': this.user['extraProperties']['Describe'] || undefined })
        if (this.user.id) {
          getOrganizationUnitByUserId(this.user.id).then((res) => {
            that.form.setFieldsValue({ 'extraProperties.OrgIds': res.items.map((x) => x.id) })
          })
          getRolesByUserId(this.user.id).then(({ items }) => {
            var role = []
            items.some(function (re) {
              role.push(re.name)
            })
            that.form.setFieldsValue({ roleNames: role })
          })
        } else {
          if (this.organizationUnitId === undefined) {
            //this.form.setFieldsValue({ 'extraProperties.OrgIds': [this.organizationUnitList[0].id] })
          } else {
            var orgs = []

            getParents(this.organizationUnitId).then((res) => {
              if (res != null && res.length > 0) {
                res.forEach((e) => {
                  orgs.push(e.id)
                })
              }
              orgs.push(that.organizationUnitId)
              that.form.setFieldsValue({ 'extraProperties.OrgIds': orgs })
            })
          }
        }
      })
    },
    reset(record) {
      const that = this
      this.user = { ...record }
      resetPassword({
        id: this.user.id,
        password: '123456',
      }).then((res) => {
        that.$message.success('密码重置成功！')
        that.$emit('ok')
      })
    },

    close() {
      this.$emit('close')
      this.visible = false
    },
    handleOk() {
      const that = this
      // 触发表单验证
      this.form.validateFields((err, values) => {
        // 验证表单没错误
        if (!err) {
          that.confirmLoading = true
          const { id, userName, name, surname, email, phoneNumber, lockoutEnabled, concurrencyStamp } = that.user
          const formObj = Object.assign(
            { id, userName, name, surname, email, phoneNumber, lockoutEnabled, concurrencyStamp },
            values
          )
          if (!formObj.id) formObj.email = formObj.userName + formObj.email
          formObj['extraProperties']['Position'] = formObj['extraProperties']['Position'] || null
          formObj.roleNames = formObj.roleNames
          saveUser(formObj)
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
    handleCancel() {
      this.close()
    },
    formatData(items) {
      const that = this
      items.forEach((item) => {
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
    filterOption(input, option) {
      return option.componentOptions.children[0].text.toLowerCase().indexOf(input.toLowerCase()) >= 0
    },
    uuid() {
      return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (Math.random() * 16) | 0,
          v = c == 'x' ? r : (r & 0x3) | 0x8
        return v.toString(16)
      })
    },
  },
}
</script>

<style lang="less" scoped>
</style>
