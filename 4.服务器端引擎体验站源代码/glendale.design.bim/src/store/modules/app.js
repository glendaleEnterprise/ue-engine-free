import storage from 'store'
import { CURR_PROJECTID } from '@/store/mutation-types'
import { getProject } from '../../api/project'

const app = {
  state: {
    currProjectId:  storage.get('projectId') ? storage.get('projectId') : undefined,
    currProject: storage.get('projectInfo') ? storage.get('projectInfo') : undefined,
  },
  mutations: {
    SET_PROJECTID: (state, projectId) => {

      state.currProjectId = projectId
      storage.set('projectId', projectId)
      state.currProject = undefined
    },
    SET_CURR_PROJECT: (state, project) => {
      state.currProject = project
      storage.set('projectInfo', project)
    },
  },
  actions: {
    /**设置当前项目Id */
    SetProject ({ commit, dispatch }, projectId) {
      storage.set(CURR_PROJECTID, projectId)
      commit('SET_PROJECTID', projectId)
    },
    /**设置当前项目 */
    GetProjectById ({ commit }, projectId) {
      return new Promise((resolve, reject) => {
        getProject(projectId)
          .then(res => {             
            commit('SET_CURR_PROJECT', res)
            resolve(res)
          }).catch(err => {
            reject(err)
          })
      })
    },

  }
}

export default app
