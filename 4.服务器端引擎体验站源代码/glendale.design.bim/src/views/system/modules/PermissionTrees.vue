<template>
  <a-modal title="角色权限" :width="600" :visible="visible" @ok="onOk" @cancel="onCancel">
    <a-row>
      <a-col :span="6">
        <a-menu v-model="menuActive" mode="inline" theme="light">
          <a-menu-item v-for="permission in menuGroups" :key="permission.name">
            {{ permission.displayName }}
          </a-menu-item>
        </a-menu>
      </a-col>
      <a-col :span="18">
        <a-card :title="permission.displayName" size="small" v-for="permission in menuGroups" :key="permission.name" v-show="menuActive.some(name=>name===permission.name)" :bordered="false">
          <a-tree :treeData="transformPermissionTree(permission.permissions)" v-model="permission.checkedKeys" checkable :selectedKeys="permission.selectedKeys" :expandedKeys="permission.expandedKeys" @expand="onExpand" @select="onSelect" :replaceFields="{ children:'children',title:'displayName',key:'name' }" />
        </a-card>
      </a-col>
    </a-row>
  </a-modal>
</template>

<script>
import { getRoleOrgJoinList, getRolePermissionList, saveRolePermission } from '@/api/system'
export default {
  name: 'PermissionTrees',
  data () {
    return {
      visible: false,
      role: undefined,
      menuActive: [],
      menuGroups: [],
      permissions: [],
    }
  },
  methods: {
    async show (role) {
      this.visible = true
      this.role = role
      const res = await getRolePermissionList(role.name)
      const groups = res.groups.filter(x => x.name.startsWith('Design.'))
      groups.forEach((x, i) => {
        const { name, displayName, permissions } = x
        const checkedKeys = x.permissions.filter(x => x.isGranted).map(x => x.name)
        this.menuGroups.push({ name, displayName, permissions, checkedKeys, expandedKeys: [], selectedKeys: [] })
      })
      this.permissions = groups.flatMap(g => g.permissions.map(x => ({ name: x.name, isGranted: x.isGranted })))
      this.menuActive = [this.menuGroups[0].name]
    },
    onOk () {
      const checkedKeys = this.menuGroups.flatMap(m => m.checkedKeys)
      this.permissions.forEach(p => {
        p.isGranted = checkedKeys.some(x => x === p.name)
      })
      saveRolePermission(this.role.name, { permissions: this.permissions }).then(() => {
        this.$message.success('保存成功！')
        this.onCancel()
      })
    },
    onCancel () {
      this.visible = false
      this.role = undefined
      this.menuActive = []
      this.menuGroups = []
      this.permissions = []
    },
    transformPermissionTree (permissions) {
      const array = []
      permissions.filter(x => x.parentName === null).forEach(permission => {
        permission.children = permissions.filter(x => x.parentName === permission.name)
        array.push(permission)
      })
      return array
    },
    onExpand (expandedKeys, e) {
      // if (e.expanded) {
      //   if (expandedKeys.length > 0) {
      //     expandedKeys.splice(0, expandedKeys.length - 1)
      //   }
      // }
      this.menuGroups.find(x => x.name === this.menuActive[0]).expandedKeys = expandedKeys
    },
    onSelect (selectedKeys) {
      // this.menuGroups.find(x => x.name === this.menuActive[0]).expandedKeys = selectedKeys
      this.menuGroups.find(x => x.name === this.menuActive[0]).selectedKeys = selectedKeys
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
