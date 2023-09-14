import request from '@/utils/request'

/**
 * 保存 
 */
export function savePostil(params) {
    return request({
        url: `/api/app/postil`,
        method: 'post',
        data: params
    })
}
/**
 * 获取分页
*/
export function getPostilList(parameter) {
    return request({
        url: `/api/app/postil`,
        method: 'get',
        params: parameter
    })
}
/**
 * 分页-发起
*/
export function getPostilLaunch(parameter) {
    return request({
        url: `/api/app/postil/launch`,
        method: 'get',
        params: parameter
    })
}
/**
 * 分页-处理
*/
export function getPostilHandle(parameter) {
    return request({
        url: `/api/app/postil/handle`,
        method: 'get',
        params: parameter
    })
}

/**
 * 修改状态
 */
export function getPostilStatus(param) {
  return request({
    url: `/api/app/postil/${param.id}/status`,
    method: 'put',
    params: param
  })
}
/**
 * 保存处理结果
*/
export function saveTask(postilId, params) {
    return request({
        url: `/api/app/postil/task/${postilId}`,
        method: 'post',
        data: params
    })
}

/**
 * 删除批注
*/
export function deletePostil(id) {
    return request({
        url: `/api/app/postil/${id}`,
        method: 'delete',
    })
}

/**
 * 获取详情
*/
export function getPositil(id) {
    return request({
        url: `/api/app/postil/${id}`,
        method: 'get'
    })
}
