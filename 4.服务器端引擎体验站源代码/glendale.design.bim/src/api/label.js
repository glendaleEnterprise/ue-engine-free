import request from '@/utils/request'
const api = {
  label:'/api/app/label'
} 
export async function saveLabel(params) {
  return await request({
    url: `${api.label}`,
    method: 'post',
    data: params
  })
}
export async function deleteLabel(id) {
  return await request({
    url: `${api.label}/${id}`,
    method: 'delete'
  })
}
export async function getListLabel(params) {
  return await request({
    url: `${api.label}`,
    method: 'get',
    params: params
  })
}
 