import request from '@/utils/request'
const api = {
  share: `/api/app/share-record`,
  SharedCode: `/api/app/share-record/getallasync`,
}
/**
 * 分享列表
 * @param {*} parameter 
 * @returns 
 */
export async function getList(params) {
  return await request(
    {
      url: api.share,
      method: 'get',
      params: params
    }
  )
}
export function saveShare(parameter) {
  let url = api.share
  if (parameter.id) url = `${url}/${parameter.id}`
  return request({
    url: url,
    method: parameter.id ? 'put' : 'post',
    data: parameter
  })
}
export function deleted(id) {
  return request({
    url: `${api.share}/${id}`,
    method: 'delete',
  })
}
/**
 * 获取分享对象
*/
export function getShare(id) {
  return request({
    url: `/api/app/share-record/${id}`,
    method: 'get'
  })
}
/**
 * 累加分享次数
*/
export function addPvm(id) {
  return request({
    url: `/api/app/share-record/${id}/pvm`,
    method: 'post'
  })
}
//---------------------------------

/**
* 分享保存
* @returns {Promise<AxiosResponse<T>>}
*/
export async function setSharedUrlSave(params) {
  return await request(
    {
      url: api.share,
      method: 'post',
      data: params
    }
  )
}

export async function ifIsAuthCode(id) {
  return await request({
    url: `${api.share}/${id}` + '/is-auth',
    method: 'post'
  })
}
/**
 * 分享信息获取
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function getSharedCode(params) {
  return await request(
    {
      url: api.share + `/${params.id}`,
      method: 'get',
      params: params
    }
  )
}

/**
 * 验证码验证
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function checkSharedCode(params) {
  return await request(
    {
      url: api.share + `/${params.id}/auth-verify`,
      method: 'post',
      params: params
    }
  )
}