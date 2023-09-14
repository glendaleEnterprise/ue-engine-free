<template>
  <a-config-provider :locale="locale">
    <div id="app">
      <router-view />
    </div>
  </a-config-provider>
</template>

<script>
  import request from '@/utils/request'
  import qs from 'qs'
  import zhCN from 'ant-design-vue/es/locale/zh_CN'
  // import zhtw from 'ant-design-vue/lib/locale-provider/zh_TW.js'
  // import { i18nRender } from '@/locales'
  // 以下3句作用于日期控件显示中文
  // import moment from 'moment'
  // import 'moment/locale/zh-tw'
  // moment.locale('zh-tw')
  export default {
    data() {
      return {
        locale: zhCN,
      }
    },
    //  computed: {
    //   locale () {
    //     // 只是为了切换语言时，更新标题
    //     const { title } = this.$route.meta
    //     title && (setDocumentTitle(`${i18nRender(title)} - ${domTitle}`))

    //     // 原先的en-US改成zh-CN即可
    //     return this.$i18n.getLocaleMessage('zh-CN').antLocale
    //   }
    // }
   
  }
</script>