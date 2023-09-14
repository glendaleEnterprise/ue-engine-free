import Vue from 'vue'
import Vuex from 'vuex'

import app from './modules/app'
import user from './modules/user'
import system from './modules/system'
import project from './modules/project'
import bim from './modules/bim'
import permission from './modules/permission'
import getters from './getters'

Vue.use(Vuex)
const types = {
  CONFIG: 'CONFIG'
}
export default new Vuex.Store({
  modules: {
    app,
    user,
    system,
    project,
    bim,
    permission,
  },
  state: {
    baseUrl: undefined,
    modelUrl: undefined,
    modelFile: undefined,
    modelInput: undefined,
    gisInput: undefined,
    videoAddress: undefined,
    showLogo: undefined,
    pakAllList: [],
    specialType: [],
    publicProject: undefined,
    deploymentMethod: undefined,
    projectName: undefined,
    defaultUser: undefined,
    whetherVerify: undefined
  },
  mutations: {
    [types.CONFIG]: (state, data) => {
      state.baseUrl = data.baseUrl;
      state.modelUrl = data.modelUrl;
      state.modelFile = data.modelFile;
      state.modelInput = data.modelInput;
      state.gisInput = data.gisInput;
      bim.state.defaults.secretKey = data.secretKey;
      bim.state.defaults.serverIP = data.serverIP;
      bim.state.defaults.port = data.port;
      state.videoAddress = data.videoAddress;
      state.showLogo = data.showLogo;
      state.pakAllList = data.pakList;
      state.specialType = data.specialType;
      state.publicProject = data.publicProject;
      state.deploymentMethod = data.deploymentMethod;
      state.projectName = data.projectName;
      state.defaultUser = data.defaultUser;
      state.whetherVerify = data.whetherVerify;
    }
  },
  actions: {},
  getters
})
