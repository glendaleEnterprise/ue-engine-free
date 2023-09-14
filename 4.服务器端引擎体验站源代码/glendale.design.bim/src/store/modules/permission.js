import { constantRouterMap, mainRouterMap } from '@/config/router.config'
import cloneDeep from 'lodash.clonedeep'
import storage from 'store'
import getters from './../getters'
/**
 * 过滤账户是否拥有某一个权限，并将菜单从加载列表移除
 *
 * @param permission
 * @param route
 * @returns {boolean}
 */
function hasPermission(permission, route) {
  if (route.meta && route.meta.permission) {
    let flag = false
    for (let i = 0, len = permission.length; i < len; i++) {
      flag = route.meta.permission.includes(permission[i])
      if (flag) {
        return true
      }
    }
    return false
  }
  return true
}

/**
 * 单账户多角色时，使用该方法可过滤角色不存在的菜单
 *
 * @param roles
 * @param route
 * @returns {*}
 */
// eslint-disable-next-line
function hasRole(roles, route) {
  if (route.meta && route.meta.roles) {
    return route.meta.roles.includes(roles.id)
  } else {
    return true
  }
}

function filterAsyncRouter(routerMap, roles, project) {
  const accessedRouters = routerMap.filter(route => {
     
    if (!!project && route.project) {
      route.meta.title = project.projectName  //选中的项目名称赋值到顶部菜单
      route.hidden = false
    }     
    if (hasPermission(roles.permissionList, route, project)) {
      if (route.children && route.children.length) {
        route.children = filterAsyncRouter(route.children, roles, project)
      }
      return true
    }
    return false
  })
  return accessedRouters
}

const permission = {
  state: {
    routers: constantRouterMap,
    roleList: storage.get('roleList') ? storage.get('roleList'): null,
    addRouters:  storage.get('addRouters') ? storage.get('addRouters'): []
  },
  mutations: {
    SET_ROUTERS: (state, routers) => {
      state.addRouters = routers//constantRouterMap.concat(routers)
      storage.set('addRouters', routers)
      state.routers = [...constantRouterMap, routers]
    },
    SET_ROLELIST: (state, roleList) => {
      state.roleList = roleList
      storage.set('roleList', roleList)
    }
  },
  actions: {
    GenerateRoutes({
      commit,
      state
    }, data) {
      return new Promise(resolve => {        
        const roles = data.roles || state.roleList
        const project = data.project
        
        const accessedRouters = filterAsyncRouter(state.addRouters, roles, project)
        commit('SET_ROUTERS', accessedRouters)
        commit('SET_ROLELIST', roles)
        resolve()
      })
    }
  }
}
export default permission
