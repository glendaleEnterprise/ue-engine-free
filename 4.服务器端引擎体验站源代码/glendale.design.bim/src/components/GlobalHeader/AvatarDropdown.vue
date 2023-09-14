<template>
  <a-dropdown v-if="currentUser && currentUser.name" placement="bottomRight">
    <span class="ant-pro-account-avatar">
      <span style="cursor: pointer">您好，{{ currentUser.name }} <a-icon type="down" style="font-size:12px" /></span>
    </span>
    <template v-slot:overlay>
      <a-menu class="ant-pro-drop-down menu" :selected-keys="[]">
        <a-menu-item v-if="menu" key="basicInfoMation" @click="$refs.basicInfoMation.show(currentUser)">
          <a-icon type="solution" />个人信息
          <basic-info-mation ref="basicInfoMation" />
        </a-menu-item>
        <a-menu-divider v-if="menu && currentUser.userName != 'guest'" />
        <a-menu-item v-if="menu && currentUser.userName != 'guest'" key="settings" @click="$refs.changePassword.show()">
          <a-icon type="setting" />修改密码
          <change-password ref="changePassword" />
        </a-menu-item>
        <a-menu-divider v-if="menu" />
        <a-menu-item key="logout" @click="handleLogout"> <a-icon type="logout" />退出 </a-menu-item>
      </a-menu>
    </template>
  </a-dropdown>
  <span v-else>
    <a-spin size="small" :style="{ marginLeft: 8, marginRight: 8 }" />
  </span>
</template>

<script>
import ChangePassword from './modules/ChangePasswordModal'
import BasicInfoMation from './modules/BasicInformationModal'
export default {
  name: 'AvatarDropdown',
  components: {
    ChangePassword,
    BasicInfoMation,
  },
  props: {
    currentUser: {
      type: Object,
      default: () => null,
    },
    menu: {
      type: Boolean,
      default: true,
    },
  },
  methods: {
    handleLogout(e) {
      this.$confirm({
        title: '提示',
        content: '确定要退出登录吗？',
        okText: '确认',
        cancelText: '取消',
        onOk: () => {
          return this.$store.dispatch('Logout').then(() => {
            window.location.reload()
            // this.$emit('fatherMethod');   //调用login中的公开项目方法
          })
        },
      });
    },
  },
}
</script>

<style lang="less" scoped>
.ant-pro-drop-down {
  /deep/ .action {
    margin-right: 8px;
  }

  /deep/ .ant-dropdown-menu-item {
    min-width: 160px;
  }
}
</style>
