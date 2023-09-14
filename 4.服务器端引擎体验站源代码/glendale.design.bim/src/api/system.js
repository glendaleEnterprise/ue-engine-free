import request from '@/utils/request'
import store from '@/store'
import { transformAbpListQuery, buildPagingQueryResult } from '@/utils/abpParamsTransform'
 
const api = {
  user: `/api/identity/users`,
  systemuser: `/api/app/system-user`,
  role: `/api/identity/roles`,
  systemrole: `/api/app/system-role`,
  permission: `/api/permission-management/permissions`,
  organizationUnit: `/api/app/organization-unit`,   
  reset: `/api/app/user-profile`,
  isInitPwd:`/api/app/user-profile/is-init-pwd`,
  setting: `/api/app/system-setting`,
  roleOrgJoin: `/api/app/role-org-join`,
}

/**
 * 用户列表
 * @param {*} parameter 
 * @returns 
 */
export function getUserList (orgId, parameter) {   
  return request({
    url: orgId ? `${api.organizationUnit}/users/${orgId}` : api.user,
    method: 'get',
    params: parameter
  })
}
/**
 * 用户列表
 * @param {*} parameter 
 * @returns 
 */
export function getSystemAppUserList (orgId, parameter) {   
  return request({
    url: orgId ? `${api.organizationUnit}/users/${orgId}` : api.systemuser,
    method: 'get',
    params: parameter
  })
}
export function getSystemAppUserPageList (orgId, parameter) {   
  return request({
    url: orgId ? `${api.organizationUnit}/users-list/${orgId}` : `${api.systemuser}/users-list`,
    method: 'get',
    params: parameter
  })
}
export function getUser (parameter) {
  return request({
    url: `${api.user}`,
    method: 'get',
    params: parameter
  })
}

export function getUserInfo (id) {
  return request({
    url: `${api.user}/${id}`,
    method: 'get',
  })
}

export function getAllUsers(){
  return request({
    url:'/api/app/user-profile',
    method:'get'
  })

}
export function getRolesByUserId (userId) {
  return request({
    url: `${api.user}/${userId}/roles`,
    method: 'get',
  })
}

export function resetPassword (parameter) {
  const url = `${api.reset}/${parameter.id}/reset-password?password=` + parameter.password
  return request({
    url: url,
    method: 'post',
    data: null
  })
}

export function roleOrgJoinAdd (parameter) {
  return request({
    url: `${api.roleOrgJoin}/arry`,
    method: 'post',
    data: parameter
  })
}

export function getRoleOrgJoinList (parameter) {
  return request({
    url: `${api.roleOrgJoin}/list`,
    method: 'get',
    params:parameter
  })
}

export function saveUser (parameter) {
  let url = api.user
  if (parameter.id) url = `${url}/${parameter.id}`
  return request({
    url: url,
    method: parameter.id ? 'put' : 'post',
    data: parameter
  })
}
export function delUser (id) {
  return request({
    url: `${api.user}/${id}`,
    method: 'delete',
  })
}

export function getRoleList (orgId, parameter) {
  const queryParams = transformAbpListQuery(parameter)
  return request({
    url: orgId ? `${api.organizationUnit}/roles/${orgId}` : api.role,
    method: 'get',
    params: queryParams
  })
}
export function getSystemRoleList (parameter) {
  const queryParams = transformAbpListQuery(parameter)
  return request({
    url: `${api.systemrole}/list`,
    method: 'get',
    params: queryParams
  })
}
export function saveSystemRole (parameter) {
  let url = `${api.systemrole}/info`
  if (parameter.id) url = `${api.systemrole}/${parameter.id}/info`
  return request({
    url: url,
    method: parameter.id ? 'put' : 'post',
    data: parameter
  })
}
export function getRoleAll () {
  return request({
    url: `${api.role}/all`,
    method: 'get',
  })
}
export function saveRole (parameter) {
  let url = api.role
  if (parameter.id) url = `${url}/${parameter.id}`
  return request({
    url: url,
    method: parameter.id ? 'put' : 'post',
    data: parameter
  })
}
export function delRole (id) {
  return request({
    url: `${api.role}/${id}`,
    method: 'delete',
  })
}

export function getOrganizationUnitList (parameter) {
  const queryParams = transformAbpListQuery(parameter)
  return request({
    url: api.organizationUnit,
    method: 'get',
    params: queryParams
  }).then(data => {
    return buildPagingQueryResult(queryParams, data)
  })
}

export function getOrganizationUnitTrees () {
  return request({
    url: `${api.organizationUnit}/trees`,
    method: 'get',
  })
}

export function getOrganizationUnitByUserId (userId) {
  return request({
    url: `${api.organizationUnit}/by-userid/${userId}`,
    method: 'get',
  })
}

export function getNextChildCode (parentId) {
  let url = `${api.organizationUnit}/next-child-code`
  if (parentId) url = `${url}/${parentId}`
  return request({
    url: url,
    method: 'get',
  })
}

export function saveOrganizationUnit (parameter) {
  let url = api.organizationUnit
  if (parameter.id) url = `${url}/${parameter.id}`
  return request({
    url: url,
    method: parameter.id ? 'put' : 'post',
    data: parameter
  })
}
export function delOrganizationUnit (id) {
  return request({
    url: `${api.organizationUnit}/${id}`,
    method: 'delete',
  })
}
/**
 * 所有父节点集合
*/
export function getParents(id){
  return request({
    url: `${api.organizationUnit}/${id}/parent`,
    method:'get'
  })
}
export function getRolePermissionList (roleName) {
  return request({
    url: `${api.permission}?providerName=R&providerKey=${roleName}`,
    method: 'get',
  })
}

export function saveRolePermission (roleName, data) {
  const permissions = [{ 'name': 'AbpIdentity.Roles', 'isGranted': true },
  { 'name': 'AbpIdentity.Roles.Create', 'isGranted': true },
  { 'name': 'AbpIdentity.Roles.Update', 'isGranted': true },
  { 'name': 'AbpIdentity.Roles.Delete', 'isGranted': true },
  { 'name': 'AbpIdentity.Roles.ManagePermissions', 'isGranted': true },
  { 'name': 'AbpIdentity.Users', 'isGranted': true },
  { 'name': 'AbpIdentity.Users.Create', 'isGranted': true },
  { 'name': 'AbpIdentity.Users.Update', 'isGranted': true },
  { 'name': 'AbpIdentity.Users.Delete', 'isGranted': true },
  { 'name': 'AbpIdentity.Users.ManagePermissions', 'isGranted': true },
  { 'name': 'FeatureManagement.ManageHostFeatures', 'isGranted': true },
  { 'name': 'SettingManagement.Emailing', 'isGranted': true }]
  data.permissions = data.permissions.concat(permissions)
  return request({
    url: `${api.permission}?providerName=R&providerKey=${roleName}`,
    method: 'put',
    data: data
  })
}

 

/**
 *获取配置文件Settings对象
 */ 
export function getSettings () {
  return request({
    url: `${api.setting}`,
    method: 'get',
  })
}
//是否需要修改密码
export function isInitPwd(userName){
  return request({
    url:`${api.isInitPwd}?userName=${userName}`,
    method:'post'
  })
}





