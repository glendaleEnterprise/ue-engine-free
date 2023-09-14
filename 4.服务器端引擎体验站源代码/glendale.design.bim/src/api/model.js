import request from '@/utils/request'
const api = { 
    modelProperty:`/api/app/model-property`,
    modelTree: `/api/app/model-tree`,
    modelType: `/api/app/model-type`,
    modelDrawing: `/api/app/model-drawing`,
    modelVersion: `/api/app/document-ver-than`,
    modelSpace: `/api/app/model-space`,
    modelPropertySpace: `/api/app/model-property-space`
}

/**
 * 获取构件属性
 * @returns {Promise<AxiosResponse<T>>}
 */
 export async function getFeatureProperty (params) {
  return await request(
    {
      url: `${api.modelProperty}/getproperty`,
      method: 'get',
      params: params
    }
  )
}

/**
 * 专业结构树
 * @returns {Promise<AxiosResponse<T>>}
 */
 export async function getModelTypeTree (params) {
  return await request(
    {
      url: api.modelType,
      method: 'get',
      params: params
    }
  )
}

/**
 * 获取构件id数组 
 */
 export async function getModelTypeTreeIdList (params) {
  return await request(
    { 
      url: `${api.modelType}/type-child-id-list/${params.glId}`,
      method: 'get',
      params: params
    }
  )
}

export function getTreeList (parameter) {
  return request({
    url: `${api.modelTree}/tree`,
    method: 'get',
    params: parameter
  })
}
export function getChildrenTreeList (parameter) {
  return request({
    url: `${api.modelTree}/children-tree`,
    method: 'get',
    params: parameter
  })
}
export function getPartTreeList (parameter,data) {
  return request({
    url: `${api.modelTree}/get-part-tree`,
    method: 'post',
    params: parameter,
    data: data
  })
}

/**
 * 获取楼层结构树
 * @returns {Promise<AxiosResponse<T>>}
 */
 export async function getModelFloorTree (params) {
  return await request(
    {
      url: `${api.modelTree}/list`,
      method: 'get',
      params: params,
    }
  )
}

/**
 * 获取楼层结构下的所有构件id
 * @returns {Promise<AxiosResponse<T>>}
 */
 export async function getModelFloorTreeId (params) {
  return await request(
    {
      url: `${api.modelTree}/child-id-list`,
      method: 'get',
      params: params,
    }
  )
}
// export function getTypeList (parameter) {
//   return request({
//     url: `${api.modelType}/tree`,
//     method: 'get',
//     params: parameter
//   })
// }
// export function getChildrenTypeList (parameter) {
//   return request({
//     url: `${api.modelType}/children-tree`,
//     method: 'get',
//     params: parameter
//   })
// }

export function getDrawingType (parameter) {
    return request({
      url: `${api.modelDrawing}/type`,
      method: 'get',
      params: parameter
    })
}

export function getDrawingData (parameter) {
    return request({
        url: `${api.modelDrawing}/drawing`,
        method: 'get',
        params: parameter
    })
}

export function getDrawingGuid (parameter) {
    return request({
        url: `${api.modelDrawing}/drawing-rvt-id`,
        method: 'get',
        params: parameter
    })
}

export function getModelVersionHave (parameter) {
  return request({
    url: `${api.modelVersion}/entity`,
    method: 'get',
    params: parameter
  })
}

export function setModelVersion (parameter) {
  return request({
    url: `${api.modelVersion}`,
    method: 'post',
    data: parameter
  })
}

export function getModelVersion (id) {
  return request({
    url: `${api.modelVersion}/${id}`,
    method: 'get',
  })
}

export function getModelVersionData (id,parameter) {
  return request({
    url: `${api.modelVersion}/${id}/start-than`,
    method: 'post',
    params: parameter,
    // data: {glid: parameter.glid}
  })
}

export function setModelVersionData (id,parameter,data) {
  return request({
    url: `${api.modelVersion}/${id}/metadata-save`,
    method: 'post',
    params:parameter,
    data: data
  })
}

/**
 * 空间结构树
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function getModelSpaceTree (params) {
  return await request(
    {
      url: api.modelSpace,
      method: 'get',
      params: params
    }
  )
}

/**
 * 获取构件id数组 
 */
 export async function getModelSpaceTreeIdList (params) {
  return await request(
    { 
      url: `${api.modelSpace}/type-child-id-list/${params.glId}`,
      method: 'get',
      params: params
    }
  )
}

/**
 * 获取构件id数组 
 */
export async function getModelPropertySpace (params) {
  return await request(
    { 
      url: `${api.modelPropertySpace}/getproperty`,
      method: 'get',
      params: params
    }
  )
}