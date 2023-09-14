import router, { resetRouter } from './router'
import store from './store'
import storage from 'store'
import NProgress from 'nprogress' // progress bar
import '@/components/NProgress/nprogress.less' // progress bar custom style
import notification from 'ant-design-vue/es/notification'
import {
  ACCESS_TOKEN
} from '@/store/mutation-types'

NProgress.configure({
  showSpinner: false
}) // NProgress Configuration

const allowList = ['login', 'sharelink', '404', 'expired'] // no redirect allowList
const loginRoutePath = '/login'
const defaultRoutePath = '/'
let isLogin = true
router.beforeEach((to, from, next) => {
  NProgress.start() // start progress bar
  //to.meta && typeof to.meta.title !== 'undefined'
  /* has token */
  const token = storage.get(ACCESS_TOKEN)
  if (token) {
    if (to.path === loginRoutePath) {
      const project = store.getters.currProject
      // 根据用户权限信息生成可访问的路由表
      store.dispatch('GenerateRoutes', { token, ...res, project }).then(() => {
        // 动态添加可访问路由表
        store.getters.addRouters.forEach(r => {
          router.addRoute(r)
          next({ path: defaultRoutePath })
        })
        // 请求带有 redirect 重定向时，登录自动重定向到该地址
        next({ path: to.path })
      })

      NProgress.done()
    } else {
      // check login user.roles is null
      if (store.getters.roles.length === 0) {
        // request login userInfo
        if (store.getters.currProjectId) {
          store.dispatch('GetProjectById', store.getters.currProjectId).then(project => {
            if (project.manageId === store.getters.userId) {
              store
                .dispatch('GetInfoCreat')
                .then(res => {
                  const project = store.getters.currProject
                  // 根据用户权限信息生成可访问的路由表
                  store.dispatch('GenerateRoutes', { token, ...res, project }).then(() => {
                    // 动态添加可访问路由表
                    // VueRouter@3.5.0+ New API
                    //resetRouter() // 重置路由 防止退出重新登录或者 token 过期后页面未刷新，导致的路由重复添加
                    store.getters.addRouters.forEach(r => {
                      router.addRoute(r)
                    })
                    // 请求带有 redirect 重定向时，登录自动重定向到该地址
                    // const redirect = decodeURIComponent(from.query.redirect || to.path)
                    // if (to.path === redirect) {
                    //   next({ ...to, replace: true }) // set the replace: true so the navigation will not leave a history record
                    // } else {
                    //   next({ path: redirect }) // 跳转到目的路由
                    // }
                    next({ path: to.path })
                  })
                })
                .catch((e) => {
                  // notification.error({
                  //   message: '错误',
                  //   description: '请求用户信息失败，请重试。'
                  // })
                  // 失败时，获取用户信息失败时，调用登出，来清空历史保留信息
                  store.dispatch('Logout').then(() => {
                    next({ path: loginRoutePath })
                  })
                })
            } else {
              store
                .dispatch('GetInfo')
                .then(res => {
                  const project = store.getters.currProject
                  // 根据用户权限信息生成可访问的路由表
                  store.dispatch('GenerateRoutes', { token, ...res, project }).then(() => {
                    // 动态添加可访问路由表
                    // VueRouter@3.5.0+ New API
                    //resetRouter() // 重置路由 防止退出重新登录或者 token 过期后页面未刷新，导致的路由重复添加
                    store.getters.addRouters.forEach(r => {
                      router.addRoute(r)
                    })
                    // 请求带有 redirect 重定向时，登录自动重定向到该地址
                    // const redirect = decodeURIComponent(from.query.redirect || to.path)
                    // if (to.path === redirect) {
                    //   next({ ...to, replace: true }) // set the replace: true so the navigation will not leave a history record
                    // } else {
                    //   next({ path: redirect }) // 跳转到目的路由
                    // }
                    next({ path: to.path })
                  })
                })
                .catch((e) => {
                  // notification.error({
                  //   message: '错误',
                  //   description: '请求用户信息失败，请重试。'
                  // })
                  // 失败时，获取用户信息失败时，调用登出，来清空历史保留信息
                  store.dispatch('Logout').then(() => {
                    next({ path: loginRoutePath })
                  })
                })
            }
          })
        } else {
          // next({ path: to.path })
          store
            .dispatch('GetInfo')
            .then(res => {
              const project = store.getters.currProject
              // 根据用户权限信息生成可访问的路由表
              store.dispatch('GenerateRoutes', { token, ...res, project }).then(() => {
                // 动态添加可访问路由表
                // VueRouter@3.5.0+ New API
                //resetRouter() // 重置路由 防止退出重新登录或者 token 过期后页面未刷新，导致的路由重复添加
                store.getters.addRouters.forEach(r => {
                  router.addRoute(r)
                })
                // 请求带有 redirect 重定向时，登录自动重定向到该地址
                // const redirect = decodeURIComponent(from.query.redirect || to.path)
                // if (to.path === redirect) {
                //   next({ ...to, replace: true }) // set the replace: true so the navigation will not leave a history record
                // } else {
                //   next({ path: redirect }) // 跳转到目的路由
                // }
                next({ path: to.path })
              })
            })
            .catch((e) => {
              // notification.error({
              //   message: '错误',
              //   description: '请求用户信息失败，请重试。'
              // })
              // 失败时，获取用户信息失败时，调用登出，来清空历史保留信息
              store.dispatch('Logout').then(() => {
                next({ path: loginRoutePath })
              })
            })
        }

      } else if (!store.getters.currProject) {
        // if (store.getters.currProjectId) {
        //   store.dispatch('GetProjectById', store.getters.currProjectId).then(project => {
        //     store.dispatch('GenerateRoutes', {
        //       roles: null,
        //       project
        //     }).then(() => {

        //       next()
        //       // if (this.$router.options.routes[1] !== undefined) {
        //       //   location.reload();
        //       // }
        //     })
        //   })
        // } else {
        //   next()
        // }
        if (isLogin && store.getters.roleName == "guest" && store.state.publicProject) {
          let id = store.state.publicProject
          store.dispatch('SetProject', id).then(() => {
            store.dispatch('GetInfo').then(res => {
              const pp = { token, ...res, id }
              store.dispatch('GenerateRoutes', pp).then(() => {
                store.getters.addRouters.forEach(r => {
                  router.addRoute(r)
                })
                const children = store.getters.addRouters[0].children.find(item => item.name === 'business').children
                if (children.length) {
                  let list = children.filter(item => item.path == "/business/model")
                  const path = list.length > 0 ? list[0].path : children[0].path
                  next({ path: path })
                  // router.replace({ path: path })
                }
              })
            })
          })
          isLogin = false
        } else {
          if (store.getters.currProjectId) {
            store.dispatch('GetProjectById', store.getters.currProjectId).then(project => {
              store.dispatch('GenerateRoutes', {
                roles: null,
                project
              }).then(() => {

                next()
                // if (this.$router.options.routes[1] !== undefined) {
                //   location.reload();
                // }
              })
            })
          } else {

            next()
          }
        }

      } else {
        if (to.fullPath == '/business') {
          const children = store.getters.addRouters[0].children.find(item => item.name === 'business').children
          let list = children.filter(item => item.path == "/business/model")
          const path = list.length > 0 ? list[0].path : children[0].path
          next({ path: path })
        } else {
          if (from.path == '/' && to.redirectedFrom == '/' && store.state.publicProject) {
            let id = store.state.publicProject
            store.dispatch('SetProject', id).then(() => {
              store.dispatch('GetInfo').then(res => {
                const pp = { token, ...res, id }
                store.dispatch('GenerateRoutes', pp).then(() => {
                  store.getters.addRouters.forEach(r => {
                    router.addRoute(r)
                  })
                  const children = store.getters.addRouters[0].children.find(item => item.name === 'business').children
                  if (children.length) {
                    let list = children.filter(item => item.path == "/business/model")
                    const path = list.length > 0 ? list[0].path : children[0].path
                    next({ path: path })
                  }
                })
              })
            })
            // next({ path: '/business/model' })
          } else {
            next()
          }
        }
      }
    }
  } else {
    if (allowList.includes(to.name)) {
      // 在免登录名单，直接进入
      next()
    } else {
      next({ path: loginRoutePath })
      NProgress.done() // if current page is login will not trigger afterEach hook, so manually handle it
    }
  }
})

router.afterEach(() => {
  NProgress.done() // finish progress bar
})
