import request from '@/utils/request'
const api = {
  viewpoint: `/api/app/view-point`,
  doc: `/api/app/document`,
  combine: `/api/app/combine`
}

/**
 * 获取视点列表分页
*/
export async function getViewpointList(params) {
  return await request({
    url: api.viewpoint,
    method: 'get',
    params: params
  })
}


/**
 * 保存视点数据
 */
export async function saveViewpoint(params) {
  return await request(
    {
      url: api.viewpoint,
      method: 'post',
      data: params
    }
  )
}

/**
 * 删除视点 
 */
export async function deleteViewPoint(id) {
  return await request(
    {
      url: `${api.viewpoint}/${id}`,
      method: 'delete',
    }
  )
}


/**修改视点列表状态 */
export async function setViewPointState(id, documentIds) {
  return await request({
    url: `${api.viewpoint}/${id}/status?Status=${documentIds.Status}&OutStatus=${documentIds.OutStatus}`,
    method: 'put',
  })
}


/**合模修改默认视点 */
export function setCombineViewPoint(id, documentIds) {
  return request({
    url: `${api.combine}/${id}/camera?Camera=${documentIds}`,
    method: 'put',
    data: documentIds
  })
}


/**合模修改默认视点 */
export function setCombineSetting(id, documentIds) {
  return request({
    url: `${api.combine}/${id}`,
    method: 'put',
    data: documentIds
  })
}