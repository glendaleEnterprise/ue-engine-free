const getters = {
  lang: state => state.app.lang,
  token: state => state.user.token,
  avatar: state => state.user.avatar,
  nickname: state => state.user.name,
  organizationUnits: state => state.user.organizationUnits,
  roles: state => state.user.roles,
  pw_status: state => state.user.pw_status,
  first: state => state.user.first,
  userInfo: state => state.user.info,
  userId: state => state.user.userId,
  organizationUnits: state => state.system.organizationUnits,
  addRouters: state => state.permission.addRouters,
  routers: state => state.permission.routers,
  currProjectId: state => state.app.currProjectId,
  currProject: state => state.app.currProject,
  docTree: state => state.project.docTree,
  baseUrl: state => state.baseUrl,
  modelUrl: state => state.modelUrl,
  bimSecretkey:state=>state.bimSecretkey,
  roleName:state=>state.user.roleName,
}

export default getters
