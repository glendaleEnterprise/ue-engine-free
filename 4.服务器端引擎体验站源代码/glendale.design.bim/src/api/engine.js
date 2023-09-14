import request from '@/utils/request'
const modelApi = {
  SetModelClosingData: `/api/app/combine`,  
  SharedRoaming: `/api/app/sharerecordlink/getroamingdata`,   
}





 
/**
 * 合模数据保存
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function setModelClosingData (params) {
  return await request(
    {
      url: modelApi.SetModelClosingData,
      method: 'post',
      data: params
    }
  )
}
/**
 * 合模数据查询
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function getModelClosingData (id) {
  return await request(
    {
      url: `${modelApi.SetModelClosingData}/${id}`,
      method: 'get',
    }
  )
}
 
/**
 * 分享漫游
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function sharedRoaming (params) {
  return await request(
    {
      url: modelApi.SharedRoaming,
      method: 'get',
      params: params
    }
  )
}



