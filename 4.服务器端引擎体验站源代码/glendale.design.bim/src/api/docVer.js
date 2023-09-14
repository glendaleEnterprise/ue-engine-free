import request from '@/utils/request'
const api = {
  version:'/api/app/document-version',
  documentHandle: '/api/app/document-handle'
}


/**
 * 版本详情
*/
export async function getDocVer(id){
  return await request(
    {
      url: `${api.version}/${id}`,
      method: 'get',      
    }
  )
}

/**
 * 上传文件
 * @returns {Promise<AxiosResponse<T>>}
 */
 export async function uploadDocumentFile (docId,formData) {
  return await request(
    {
      url: `${api.documentHandle}/upload-file/?docId=${docId}`,
      method: 'post',
      data: formData
    }
  )
}

/**
 * 新版本模型上传
*/
export async function uploadVersion (param) {   
  return await request(
    {
      url: api.version,
      method: 'post',
      data: param
    }
  )
}

/**
 * 查询版本信息
 * @param {*} parameter 
 * @returns 
 */
 export async function getDocVerList(params){
  return await request({
    url: api.version,
    method: 'get',
    params: params
  })
}

///删除模型信息
export function DocVerDeleted(id) {
  return request({
    url: `${api.version}/${id}`,
    method: 'delete',
  })
}
//设置版本
export function SetDocVerCurrent(id){
  return request({
    url: `${api.version}/${id}/set-current`,
    method: 'post',
  })
}