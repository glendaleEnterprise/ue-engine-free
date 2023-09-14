<template>
  <div id="userLayout" :class="['user-layout-wrapper', isMobile && 'mobile']">
    <div class="container">
      <router-view />
    </div>
  </div>
</template>

<script>
import { deviceMixin } from '@/store/device-mixin'
import { mapGetters } from 'vuex'
import { getFileByBlobName } from '@/api/file'

export default {
  name: 'UserLayout',
  mixins: [deviceMixin],
  components: {},
  data() {
    return {
      h: 900,
      w: 600,     
    }
  },   
  created() {
  },
  mounted() {
    const that = this
    this.onResize()
    window.onresize = () => {
      this.onResize()
    }
  },
  methods: {
    getFileByBlobName,
    onResize() {
      this.h = window.innerHeight
      this.w = window.innerWidth
    },
  },
}
</script>

<style lang="less" scoped>
#userLayout.user-layout-wrapper {
  .header-logo {
    position: absolute;
    top: 44px;
    left: 10vw;
    z-index: 5;
    height: 40px;
    .logo {
      height: 40px;
      vertical-align: top;
    }
    .title {
      font-size: 22px;
      color: #fff;
      margin-left: 20px;
      border-left: 1px solid #fff;
      padding-left: 20px;
      line-height: 40px;
    }
  }
  &.mobile {
    .container {
      .main {
        max-width: 368px;
        width: 98%;
      }
    }
  }

  .container {
    position: fixed;
    left: 0;
    right: 0;
    top: 0;
    height: 100vh;

    a {
      text-decoration: none;
    }
  }

  .footer {
    position: fixed;
    height: 58px;
    width: 100%;
    bottom: 0;
    padding: 10px 0;
    color: #fff;
    background-color: rgba(0, 0, 0, 0.5);
    .box {
      width: 100%;
      display: flex;
      justify-content: center;
      .one {
        width: calc(100vw - 55px);
        display: flex;
        justify-content: center;
        .content {
          border-bottom: 1px solid rgba(255, 255, 255, 0.4);
          height: 26px;
          line-height: 24px;
          margin-top: 7px;
          overflow: hidden;
          span {
            font-weight: lighter;
            font-size: 12px;
            font-family: Arial;
            margin: 0 10px;
          }
          span:first-of-type {
            margin-left: 0;
          }
          span:last-of-type {
            margin-right: 0;
          }
        }
      }
      .four {
        width: 55px;
        display: flex;
        font-size: 0;
        img {
          height: 40px;
        }
      }
    }
  }
}
.ant-carousel {
  font-size: 0;
}
</style>
