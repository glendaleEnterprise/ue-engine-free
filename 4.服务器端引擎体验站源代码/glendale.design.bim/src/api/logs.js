import request from '@/utils/request'

/**写日志 */
export async function createLog(param){
  return await request({
    url:`/api/app/logs`,
    method:'post',
    data:param
  })
}
//获取日志列表
export function GetLogsList(parameter) {
  return request({
    url: `/api/app/logs`,
    method: 'get',
    params: parameter
  })
}