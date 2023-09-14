import request from '@/utils/request'
const api = {  
  roaming: `/api/app/roaming`,
  viewPort: `/api/app/roaming/view-port`,
}

/**
 * 存储漫游轨迹
 * @returns {Promise<AxiosResponse<T>>}
 */
 export async function setRoamingTrack (params) {
    return await request(
      {
        url: api.roaming,
        method: 'post',
        data: params
      }
    )
  }
/**
 * 存自定义储漫游
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function setViewPort (params) {
  return await request(
    {
      url: api.viewPort,
      method: 'post',
      data: params
    }
  )
}
  /**
   * 获取漫游轨迹
   * @returns {Promise<AxiosResponse<T>>}
   */
  export async function getRoamingTrack (params) {
    return await request(
      {
        url: api.roaming,
        method: 'get',
        params: params
      }
    )
  }
  
  /**
   * 修改漫游名称和备注
  */
  export async function updateRoamingTrack(params){
    return await request(
      {
        url: `${api.roaming}/${params.id}`,
        method: 'put',
        data: params
      }
    )
  }
  
  /**
   * 删除漫游轨迹
   * @returns {Promise<AxiosResponse<T>>}
   */
  export async function deleteRoamingTrack (id) {
    return await request(
      {
        url: `${api.roaming}/${id}`,
        method: 'delete',
      }
    )
  } 