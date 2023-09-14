import request from '@/utils/request'
const api = {
  base: `/api/app/project`,
  folder:'/api/app/project-folder',
  folderArry:'/api/app/project-folder/arry',
  folderUser:'/api/app/project-folder-user',
  projectUserPermission:'/api/app/project/project-user-permission',
}

/**
 * 保存项目信息
 * @param {*} parameter 
 * @returns 
 */
export function saveProject (parameter) {
  const url = parameter.id ? `${api.base}/${parameter.id}` : api.base
  return request({
    url: url,
    method: parameter.id ? 'put' : 'post',
    data: parameter
  })
}

/**
 * 判断是否可以删除
*/
export function checkDelete(id){
  return request({
    url:`/api/app/project/${id}/check-delete`,
    method:'post'
  })
}
/**
 * 删除项目
 * @param {*} id 
 * @returns 
 */
export function delProject (id) {
  return request({
    url: `${api.base}/${id}`,
    method: 'delete',
  })
}

/** 根据项目id获取项目信息
 * @param {string} id 项目id
 * @returns 
 */
export function getProject (id) {
  return request({
    url: `${api.base}/` + id,
    method: 'get'
  })
}
/** 根据项目id获取项目信息
 * @param {string} id 项目id
 * @returns
 */
export function getProjectUserPermission (id) {
  return request({
    url: `${api.projectUserPermission}/` + id,
    method: 'get'
  })
}

/**
 * 获取项目列表

 * @param {*} keyword 
 * @param {*} skipCount 
 * @param {*} resCount 
 * @param {bool} isMain true:项目，false:子项目
 * @param {*} parentId 可选 父级id
 * @returns 
 */
export function getProjectList (parameter) {
  return request({
    url: api.base,
    method: 'get',
    params: parameter
  })
}
 


/**
 * 项目展板统计数据
 * @returns 
 */ 
export function getStatsCount(){
  return request({
    url:`/api/app/project/stats-count`,
    method:'get'
  })
}

/**
 * 判断人员是否已经参与项目
 * @param {人员Id} id 
 * @returns 
 */
export function hasJoin(id){
  return request({
    url:`/api/app/project/${id}/is-join`,
    method:'post'
  })
}


/**
 * 获取项目人员列表
*/
export function getProjectUsers(params){
  return request({
    url:`${api.base}/project-users`,
    method:'get',
    params: params
  })
}

/**
 * 获取项目人员列表
 */
export function getProjectFolderUsers(params){
  return request({
    url:`${api.folderUser}/project-user-list`,
    method:'get',
    params: params
  })
}


/******以下是项目目录*********/

export function getFolder(id){
  return request({
    url:`${api.folder}/${id}`,
    method:'get'
  })
}

export function getFolderTrees(projectId){
  return request({
    url:`${api.folder}/trees/${projectId}`,
    method:'get'    
  })
}

export function getFolderMyTrees(projectId){
  return request({
    url:`${api.folder}/my-trees/${projectId}`,
    method:'get'
  })
}

export function addFolder(parameter){
  return request({
    url:`${api.folder}`,
    method:'post',
    data: parameter
  })
}
export function folderArry(pid,parameter){
  return request({
    url:`${api.folderArry}/?ProjectID=${pid}`,
    method:'post',
    data: parameter
  })
}

export function addFolderArr(parameter,id){
  return request({
    url:`${api.folder}/arry?ProjectID=${id}`,
    method:'post',
    data: parameter
  })
}

export function delFolder(id) {
  return request({
    url: `${api.folder}/${id}`,
    method: 'delete',
  })
}

export function updateFolder(id,parameter){
  return request({
    url:`${api.folder}/${id}`,
    method:'put',
    data: parameter
  })
}
