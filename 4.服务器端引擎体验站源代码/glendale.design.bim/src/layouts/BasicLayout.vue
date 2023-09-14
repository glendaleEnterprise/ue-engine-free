<template>
  <a-layout id="components-layout">
    <!-- <change-password ref="changePassword" /> -->
    <a-layout-header class="header">
      <div class="logo" @click="ToOfficialWebsite" v-if="$store.state.showLogo">
        <img src="../assets/logo.png" :alt="$store.state.projectName" />
      </div>
      <div class="webname">{{$store.state.projectName}}</div>
      <div class="top-nav">
        <router-link :to="{ path: updateRouter(item.path) }" v-for="item in menus" :key="item.name" class="nav-box"
          active-class="nav-active">
          <div class="text">{{ item.meta.title }}</div>
        </router-link>
      </div>
      <div class="logout" v-if="currentUser && currentUser.name" v-show="currentUser.userName != 'guest' && $store.state.deploymentMethod != 'user'">
        <avatar-dropdown :currentUser="currentUser" @fatherMethod="fatherMethod" />
      </div>
      <div style="float: right" v-show="currentUser.userName == 'guest' && $store.state.deploymentMethod != 'user'"><a @click="onLogout">登录</a></div>
    </a-layout-header>
    <a-layout>
      <a-layout-sider @contextmenu.prevent="" width="220" theme="light" v-if="childMenus.length"
        :style="{ height: `${screenHeight}px` }">
        <a-menu v-model="activedMenus" mode="inline" theme="light" style="padding: 20px 10px 0 10px">
          <a-menu-item v-for="menu in childMenus" :key="menu.name">
            <router-link :to="{ path: menu.path }">
              <a-icon :type="menu.meta.icon" class="icon iconfont" :class="menu.meta.class"
                :style="{ fontSize: menu.meta.iconsize }" />
              {{ menu.meta.title }}
            </router-link>
          </a-menu-item>
        </a-menu>
      </a-layout-sider>
      <a-layout-content :style="{ padding: '12px', height: `${screenHeight}px` }">
        <router-view />
      </a-layout-content>
    </a-layout>
    <a-layout-footer class="footer" v-if="$store.state.showLogo">
      <span>版权所有：© 2023 glendale 上海葛兰岱尔网络科技有限公司.All Rights Reserved</span>
    </a-layout-footer>
    <login :loginVisible.sync="loginVisible" ref="logins"></login>
  </a-layout>
</template>
<script>
import { mapState, mapGetters } from 'vuex'
import AvatarDropdown from './../components/GlobalHeader/AvatarDropdown.vue'
import { getFileByBlobName } from '@/api/file'
import { createLog } from '@/api/logs'
// import ChangePassword from '@/components/GlobalHeader/modules/ChangePasswordModal'
import Login from '../views/user/Login'
export default {
  name: 'BasicLayout',
  components: {
    AvatarDropdown,
    // ChangePassword,
    Login
  },
  watch: {
    $route(val) {
      const that = this
      this.activedMenus = [val.meta.actived || val.name]
      this.setActivated(val)
    },
  },
  data() {
    return {
      menus: [],
      childMenus: [],
      activedMenus: [this.$route.name],
      screenHeight: window.innerHeight - 88,
      title: process.env.VUE_APP_TITLE,
      loginVisible: false
    }
  },
  computed: {
    ...mapState({
      // 动态主路由
      mainMenu: (state) => state.permission.addRouters,
    }),
    ...mapGetters(['currProject', 'pw_status']),
    ...mapGetters({
      currentUser: 'userInfo',
    }),
  },
  created() {
    this.setActivated(this.$route)
  },
  mounted() {
    this.$store.dispatch('change_login_first', false)
  },
  methods: {
    checkStatus() {
      if (!this.pw_status && this.currentUser.userName != 'guest' && this.$store.state.deploymentMethod != 'user') {
        this.$message.error('初次登录，请先修改密码！')
        this.$refs.changePassword.show()
        this.$store.dispatch('change_pw_status', true)
      }
    },
    updateRouter(path) {
      path.indexOf("/business") != -1 ? path = '/business' : null
      return path
    },
    getFileByBlobName,
    setActivated(route) {
      const that = this
      const routes = that.mainMenu.find((item) => item.path === '/')
      that.childMenus = []
      that.menus =
        routes.children
          ?.map((x) => {
            const { children, ...data } = x
            if (children?.some((x) => x.path.replace(':id', route.params.id) === route.fullPath)) {
              that.childMenus = children?.filter((x) => !x.hidden) || []
              x.path = '/' + data.name
            }
            return data
          })
          .filter((x) => !x.hidden) || []
    },
    handleLogout(e) {
      const that = this
      that.$confirm({
        title: '提示',
        content: '确定要退出登录吗？',
        onOk: () => {
          createLog({
            Name: '退出系统',
            ModuleName: '其他',
            Content: `退出系统，账号：${this.currentUser.userName}`,
          })
          return that.$store.dispatch('Logout').then(() => {
            that.$router.replace({ path: '/login' })
            that.$router.go(0)
          })
        },
        onCancel() { },
      })
    },
    closeMenu() {
      const that = this
      this.$store.dispatch('SetProject', '')
      var main = that.mainMenu.find((item) => item.path === '/')
      main.children.find((item) => item.path === '/business').hidden = true
      this.setActivated(this.$route)
    },
    //游客退出登录
    onLogout() {
      const that = this
      that.loginVisible = true
    },
    fatherMethod() {
      this.$refs.logins.demoLogin()
    },
    ToOfficialWebsite() {
      window.open('http://glendale.top/', "_blank")
    }
  },
}
</script>

<style lang="less" scoped>
#components-layout {
  .header {
    padding: 0 20px;
    background: #323641;

    .logo {
      margin-right: 20px;
      height: 30px;
      float: left;
      cursor: pointer;

      img {
        height: 30px;
        max-width: 160px;
      }
    }

    .webname {
      color: #fff;
      // width: 200px;
      height: 20px;
      margin: 22px 0;
      padding-left: 20px;
      float: left;
      border-left: 1px solid rgb(170, 161, 161);
      line-height: 20px;
      font-size: 16px;
      font-weight: 500;
      font-family: 'Microsoft Yahei';
      white-space: nowrap;
      margin-right: 20px;
    }

    .top-nav {
      float: left;
      display: flex;

      .nav-box {
        height: 40px;
        line-height: 40px;
        padding: 0 20px;
        border-radius: 3px 3px 0px 0px;
        margin: 25px 5px 0px 5px;
        color: #dfdfdf;
        font-weight: bold;
        font-size: 14px;
        position: relative;
      }

      .nav-active {
        color: #333;
        background-color: #fff;

        .text {
          height: 39px;
          border-bottom: 3px solid #07b391;
        }

        .close-m {
          position: absolute;
          right: -8px;
          top: -19px;

          .anticon {
            color: #07b391;
          }

          :hover {
            background-color: #dfdfdf;
            border-radius: 50%;
          }
        }
      }
    }

    .logout {
      display: flex;
      float: right;
      color: rgba(255, 255, 255, 0.8);

      .text {
        margin-right: 10px;
        padding-right: 10px;
        margin-top: 25px;
        height: 14px;
        line-height: 14px;
        border-right: 1px solid rgba(255, 255, 255, 0.5);
      }
    }
  }

  .basic_nav {
    height: 40px;
    line-height: 40px;
    padding: 0;
    text-indent: 15px;
    background: #fff;
    border-bottom: 1px solid #dfdfdf;
    font-weight: bold;
  }

  .footer {
    text-align: center;
    background: rgba(0, 0, 0, 0.3);
    height: 24px;
    line-height: 24px;
    font-size: 12px;
    padding: 0;
    margin: 0;

    span {
      margin: 0 10px;
    }
  }
}

// 二级导航菜单样式
.ant-menu:not(.ant-menu-horizontal) .ant-menu-item-active,
.ant-menu:not(.ant-menu-horizontal) .ant-menu-item-selected {
  border-radius: 5px;
  background-color: #dfdfdf;
  color: #333;
}

.ant-menu:not(.ant-menu-horizontal) .ant-menu-item-selected>a,
.ant-menu:not(.ant-menu-horizontal) .ant-menu-item-active>a:hover {
  color: #333;
  font-weight: bold;
}

.ant-menu-inline .ant-menu-item::after {
  border-right: 0px solid #05a081;
}

.ant-menu-inline>.ant-menu-item {
  height: 36px;
  line-height: 36px;
}

.ant-menu-inline {
  border-right-width: 0;
}

.close-m {
  position: absolute;
  right: -8px;
  top: 1px;

  .anticon {
    color: #07b391;
  }

  :hover {
    background-color: #dfdfdf;
    border-radius: 50%;
  }
}
</style>
