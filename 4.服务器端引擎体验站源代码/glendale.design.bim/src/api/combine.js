import request from '@/utils/request'
import { transformAbpListQuery, buildPagingQueryResult } from '@/utils/abpParamsTransform'

const api = {
  combine: `/api/app/combine`,//合摸
  flatten: `/api/app/combine-flatten`,//合摸压平
}

//保存合模
export function saveModelClosing(parameter) {
  let url = api.combine;
  return request({
    url: url,
    method: 'post',
    data: parameter
  });
}

//getCombineList
/**查询合摸列表 */
export function getCombineList(parameter) {
  const queryParams = transformAbpListQuery(parameter)//使用stable展示列表需要转换一下数据格式
  return request({
    url: api.combine,
    method: 'get',
    params: queryParams
  }).then(data => {
    return buildPagingQueryResult(parameter, data)
  })
}

//getCombineList
/**查询文档列表 */
export function getDocumentList(id) {
  return request({
    url: api.combine / GetDocument,
    method: 'get',
    params: { id: id }
  }).then(data => {
    return data
  })
}

/**
 * 查询合模树结构-并且带
 * @param {*} projectId 项目Id
 * @param {*} combineName 合模名称
 * @returns 
 */
export function getCombine(projectId, combineName) {
  return request({
    url: `${api.combine}/getalllistasync`,
    method: 'get',
    params: { projectId: projectId, combineName: combineName }
  })
}

export function deleted(id) {
  return request({
    url: `${api.combine}/${id}`,
    method: 'delete',
  })
}

export function getClampById(id) {
  return request({
    url: `${api.combine}/${id}`,
    method: 'get',
  })
}

export function getHasClampById(docId) {
  return request({
    url: `${api.combine}/has-combine/${docId}`,
    method: 'get',
  })
}

//公开
export function openStatusClamping(parameter) {
  return request({
    url: `${api.combine}/set-open`,
    method: 'post',
    params: { isOpen: parameter.isOpen },
    data: parameter.id,
  });
}

/**
 * 获取合模日志
*/
export function getCombineLogs(combineId) {
  return request({
    url: `${api.combineLog}/by-combine-id/${combineId}`,
    method: 'get'
  })
}

/**
 * 分页查询合模压平列表
 * @param {*} CombineId 合模ID 
 * @param {*} MaxResultCount 一页多少条数据
 * @param {*} SkipCount
 * @returns 
 */
export function getCombineFlatten(parameter) {
  return request({
    url: `${api.flatten}`,
    method: 'get',
    params: parameter,
  })
}

/**
 * 添加合模压平
 * @param {*} combineId 合模id 
 * @param {*} flattenName 压平名称 
 * @param {*} flattenHeight 压平高度 
 * @param {*} flattenScope 压平范围 
 * @returns 
 */
export function addCombineFlatten(parameter) {
  return request({
    url: `${api.flatten}`,
    method: 'post',
    data: parameter,
  })
}

/**
 * 根据压平id删除压平
 * @param {*} id 压平id 
 * @returns 
 */
export function deleteCombineFlatten(id) {
  return request({
    url: `${api.flatten}/${id}`,
    method: 'delete',
  })

}
/**合模封面设置 */
export function combineProjectCover(id, BlobName) {
  return request({
    url: `${api.combine}/${id}/blob-name?BlobName=${BlobName}`,
    method: 'put',
  })
}

/**存储可视距离等配置 */
export function combineModelConfig(params) {
  return request({
    url: `${api.combine}/${params.id}/model-config/${params.DocumentId}?ModelConfig=${params.ModelConfig}`,
    method: 'put',
    data: params
  })
}

/**存储场景设置等配置 */
export function combineSceneConfig(params) {
  return request({
    url: `${api.combine}/${params.id}/scene-config?SceneConfig=${params.SceneConfig}`,
    method: 'put',
    data: params
  })
}