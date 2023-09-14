import storage from 'store'
import { login, loginPhone, logout, getInfo } from '@/api/login'
import { ACCESS_TOKEN } from '@/store/mutation-types'
import {
  mainRouterMap
} from '@/config/router.config'
import cloneDeep from 'lodash.clonedeep'
import store from '@/store'
import { getProjectUserPermission } from '@/api/project'
const user = {
  state: {
    token: '',
    name: '',
    avatar: '',
    organizationUnits: [],
    roles: [],
    info: {},
    list: {},
    roleName: '',
    userId: storage.get('userId') ? storage.get('userId') : undefined,
    pw_status: storage.get('pw_status') ? storage.get('pw_status') : false,
    first: storage.get('login_first') ? storage.get('login_first') : true,
  },
  mutations: {
    SET_TOKEN: (state, token) => {
      state.token = token
    },
    SET_USERID: (state, id) => {
      storage.set('userId', id)
      state.userId = id
    },
    SET_PW_STATUS: (state, status) => {
      state.pw_status = status
    },
    SET_LOGIN_FIRST: (state, status) => {
      state.first = status
    },
    SET_NAME: (state, { name }) => {
      state.name = name
    },
    SET_AVATAR: (state, avatar) => {
      state.avatar = avatar
    },
    SET_ORGANIZATIONUNITS: (state, organizationUnits) => {
      state.organizationUnits = organizationUnits
    },
    SET_ROLES: (state, roles) => {
      state.roles = roles
    },
    SET_INFO: (state, info) => {
      state.info = info
    },
    SET_LIST: (state, list) => {
      state.list = list
    },
    SET_ROLENAME: (state, roleName) => {
      state.roleName = roleName
    }
  },
  actions: {
    change_pw_status({ commit }, info) {
      commit('SET_PW_STATUS', info)
      storage.set('pw_status', info)
    },
    change_login_first({ commit }, info) {
      commit('SET_LOGIN_FIRST', info)
      storage.set('login_first', info)
    },
    // 登录
    Login({ commit }, userInfo) {
      return new Promise((resolve, reject) => {
        let status = true
        if (userInfo.password === '123456' && userInfo.username !== '') status = false
        storage.set('pw_status', status)
        commit('SET_PW_STATUS', status)

        commit('SET_TOKEN', '')
        commit('SET_ROLES', [])
        storage.remove(ACCESS_TOKEN)
        login(userInfo).then(result => {
          storage.set(ACCESS_TOKEN, result.access_token, result.expires_in)
          commit('SET_TOKEN', result.access_token)
          resolve()
        }).catch(error => {
          reject(error)
        })
      })
    },
    //手机验证码登录
    LoginPhone({ commit }, userInfo) {
      return new Promise((resolve, reject) => {
        commit('SET_TOKEN', '')
        commit('SET_ROLES', [])
        storage.remove(ACCESS_TOKEN)
        loginPhone(userInfo).then(result => {
          storage.set(ACCESS_TOKEN, result.access_token, result.expires_in)
          commit('SET_TOKEN', result.access_token)
          resolve()
        }).catch(error => {
          reject(error)
        })
      })
    },
    // 登出
    Logout({ commit, state }) {
      return new Promise((resolve) => {
        logout(state.token).then(() => {
          commit('SET_TOKEN', '')
          commit('SET_ROLES', [])
          storage.remove(ACCESS_TOKEN)
          resolve()
        }).catch(() => {
          commit('SET_TOKEN', '')
          commit('SET_ROLES', [])
          storage.remove(ACCESS_TOKEN)
          resolve()
        })
      })
    },
    // 获取用户信息
    async GetInfo({ commit }) {
      let parentNameList = []
      if (store.getters.currProjectId) {
        const res = await getProjectUserPermission(store.getters.currProjectId)
        parentNameList = [...res.map(x => x.name), ...res.flatMap(r => r.children).map(x => x.name)]
      }
      return new Promise((resolve, reject) => {
        // 请求后端获取用户信息 /api/user/info
        getInfo().then(result => {
          const roleName = result.extraProperties.roles[0].entityDisplayName
          const organizationUnits = result.extraProperties.organizationUnits
          const roles = result.extraProperties.roles
          commit('SET_USERID', result.extraProperties.id)
          if (roles) {
            result.roles = { permissionList: ['dashboard'] }
            // const permissionList = Array.from(roles.flatMap(r => r.groups).flatMap(p => p.permissions).filter(x => x.isGranted).map(x => x.name))
            const permissionList = Array.from(roles.flatMap(r => r.groups).find(e => e.name === 'Design.Base').permissions.filter(x => x.isGranted).map(x => x.name))
            const _parentNameList = new Set(Array.from(roles.flatMap(r => r.groups).find(e => e.name === 'Design.Base').permissions.filter(x => x.isGranted).filter(x => x.parentName).map(x => x.parentName)))
            permissionList.push(...parentNameList, ..._parentNameList)
            result.roles.permissionList = result.roles.permissionList.concat([...new Set(permissionList)])
            commit('SET_ROLES', result.roles)
            commit('SET_INFO', result)
          } else {
            reject(new Error('getInfo: roles must be a non-null array !'))
          }
          if (organizationUnits) {
            commit('SET_ORGANIZATIONUNITS', organizationUnits)
          }
          if (roleName) {
            commit('SET_ROLENAME', roleName)
            commit('SET_ROUTERS', cloneDeep(mainRouterMap))
          }
          commit('SET_NAME', { name: result.name })
          resolve(result)
        }).catch(error => {
          reject(error)
        })

      })
    },
    GetInfoCreat({ commit }) {
      return new Promise((resolve, reject) => {
        // 请求后端获取用户信息 /api/user/info
        getInfo().then(result => {
          const roleName = result.extraProperties.roles[0].entityDisplayName
          const organizationUnits = result.extraProperties.organizationUnits
          const roles = result.extraProperties.roles
          commit('SET_USERID', result.extraProperties.id)
          if (roles) {
            result.roles = { permissionList: ['dashboard'] }
            const permissionList = Array.from(roles.flatMap(r => r.groups).flatMap(p => p.permissions).filter(x => x.isGranted).map(x => x.name))
            const parentNameList = new Set(Array.from(roles.flatMap(r => r.groups).filter(e => e.name === 'Design.Business').flatMap(p => p.permissions).map(x => x.name)))
            const _parentNameList = new Set(Array.from(roles.flatMap(r => r.groups).find(e => e.name === 'Design.Base').permissions.filter(x => x.isGranted).filter(x => x.parentName).map(x => x.parentName)))
            permissionList.push(...parentNameList, ..._parentNameList)
            result.roles.permissionList = result.roles.permissionList.concat([...new Set(permissionList)])
            commit('SET_ROLES', result.roles)
            commit('SET_INFO', result)
          } else {
            reject(new Error('getInfo: roles must be a non-null array !'))
          }
          if (organizationUnits) {
            commit('SET_ORGANIZATIONUNITS', organizationUnits)
          }
          if (roleName) {
            commit('SET_ROLENAME', roleName)
            commit('SET_ROUTERS', cloneDeep(mainRouterMap))
          }
          commit('SET_NAME', { name: result.name })
          resolve(result)
        }).catch(error => {
          reject(error)
        })

      })
    },
  }
}

export default user
