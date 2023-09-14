import { getOrganizationUnitTrees } from '@/api/system'

const system = {
  state: {
    organizationUnits: { totalCount: 0, items: [] },
  },
  mutations: {
    ORGANIZATION_UNITS: (state, organizationUnits) => {
      state.organizationUnits = organizationUnits
      
    },
  },
  actions: {
    // 获取组织机构列表
    getOrganizationUnitTrees ({ commit }) {
      return new Promise((resolve) => {
        // getOrganizationUnitTrees().then(result => {
        //   commit('ORGANIZATION_UNITS', result)
        //   resolve(result)
        // })
      })
    },
  }
}

export default system
