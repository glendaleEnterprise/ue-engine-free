import { getFolderTrees, getFolderMyTrees } from '@/api/project'
function formatData(items) {
  items.forEach(item => {
    if (item.children && item.children.length > 0) {
      formatData(item.children)
    } else {
      delete item.children
    }
  })
  return items
}

const project = {
  state: {
    docTree: [],//目录树
  },
  mutations: {
    DOC_TREE: (state, tree) => {
      state.docTree = tree
    }
  },
  actions: {
    // 获取子项目
    GetDocTree({ commit }, projectId) {
      return new Promise((resolve) => {
        getFolderTrees(projectId).then(result => {
          const tree = formatData(result)
          commit('DOC_TREE', tree)
          resolve(tree)
        })
      })
    },
    GetNewDocTree({ commit }, result) {
      const tree = formatData(result)
      commit('DOC_TREE', tree)
    }
  }
}

export default project
