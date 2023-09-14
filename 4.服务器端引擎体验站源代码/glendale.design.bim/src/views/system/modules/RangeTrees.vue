<template>
  <a-modal title="数据范围设置" :width="600" :visible="visible" @ok="onOk" @cancel="onCancel">
    <a-row>
      <a-col>
        <a-card size="small" :bordered="false">
          <a-tree :treeData="organizationUnitList" v-model='checkedKeys' @check="onCheck" checkable  :replaceFields="{ children:'children',title:'displayName',key:'id' }" />
        </a-card>
      </a-col>
    </a-row>
  </a-modal>
</template>

<script>
import { getRoleOrgJoinList, getRolePermissionList, roleOrgJoinAdd, saveRolePermission } from '@/api/system'
import { mapGetters } from 'vuex'
export default {
  name: 'PermissionTrees',
  data () {
    return {
      visible: false,
      role: undefined,
      organizationUnitList: [],
      checkedKeys: [],
    }
  },
  computed: {
    ...mapGetters(['organizationUnits']),
  },
  created() {
    this.$store.dispatch('getOrganizationUnitTrees')
  },
  methods: {
    async show (role) {
      this.checkedKeys=[]
      const org = await getRoleOrgJoinList({RoleId:role.id})
      this.checkedKeys=org.map(item=>item.orgId)
      this.organizationUnitList = this.formatData(this.organizationUnits)
      this.visible = true
      this.role = role
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
    onOk () {
      const param=this.checkedKeys.map(item=>{
        return {
          roleId:this.role.id,
          orgId: item
        }
      })
      roleOrgJoinAdd(param).then(res=>{
        if(res){
          this.$message.success('保存成功！')
          this.onCancel()
        }
      })
    },
    onCheck(checkedKeys, info){
    },
    onCancel () {
      this.visible = false
      this.role = undefined
    },
  }
}
</script>

<style lang="less" scoped>
.tree_loading {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
