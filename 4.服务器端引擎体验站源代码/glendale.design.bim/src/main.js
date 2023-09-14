// with polyfills
import 'core-js/stable'
import 'regenerator-runtime/runtime'

import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import {
  VueAxios
} from './utils/request'
import ProLayout, {
  PageHeaderWrapper
} from '@ant-design-vue/pro-layout'
import './assets/iconFont/iconfont.css'
// model实现可拖拽
import draggable from './utils/draggable'
Vue.use(draggable)
import bootstrap from './core/bootstrap'
import './core/lazy_use' // use lazy load components
import './permission' // permission control
import './utils/filter' // global filter 
import './global.less' // global style
import {
  Slider,
  Empty
} from 'ant-design-vue'
import MineEcharts from 'echarts'
import 'lib-flexible'
import vuescroll from 'vuescroll'
import SlideVerify from 'vue-monoplasty-slide-verify';
Vue.use(SlideVerify);
// import {i18n} from './locales'
// import VueI18n from 'vue-i18n'
// Vue.use(VueI18n)
Vue.prototype.$echarts = MineEcharts
import i18n from './locales/index'
import './utils/drag'
Vue.component(Slider.name, Slider)
Vue.component(Empty.name, Empty)
// mount axios to `Vue.$http` and `this.$http`
Vue.use(VueAxios)
// use pro-layout components
Vue.component('pro-layout', ProLayout)
Vue.component('page-container', PageHeaderWrapper)
Vue.component('page-header-wrapper', PageHeaderWrapper)
// Vue.component('vue-scroll', vuescroll)
Vue.use(vuescroll, {
  ops: {
    bar: {
      hoverStyle: true,
      onlyShowBarOnScroll: false,
      background: '#000',
      opacity: 0.3,
    }
  }, // 在这里设置全局默认配置   
})
// import Vconsole from 'vconsole'
// const vConsole = new Vconsole()
// Vue.use(vConsole)
fetch('/static/config.json').then(res => res.json()).then(res => {
  store.commit('CONFIG', res)
  window.vm = new Vue({
    router,
    store,
    i18n,
    created: bootstrap,
    render: h => h(App)
  }).$mount('#app')
})