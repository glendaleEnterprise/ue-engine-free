import request from '@/utils/request'
const api = {
  dic: `/api/app/dictionary`,
}

/**
 * 获取基础数据
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function getList (params) {
  return await request(
    {
      url: api.dic,
      method: 'get',
      params: params
    }
  )
}
export async function getPositionList () {
  return await request({
    url: api.dic + '/parent?parent=position',
    method: 'get',
  })
}
export async function getTagsList(){
  return await request({
    url: api.dic + '/parent?parent=tags',
    method: 'get',
  })
}

export async function getYearList(){
  return await request({
    url: api.dic + '/parent?parent=year',
    method: 'get',
  })
}

export async function getParent (p) {
  return await request({
    url: api.dic + `/parent?parent=${p}`,
    method: 'get',
  })
}

/**
 * 新增基础数据
 * @param name 名称
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function add (formData) {
  return await request(
    {
      url: api.dic,
      method: 'post',
      data: formData
    }
  )
}
/**
 * 编辑基础数据
 * @param name 名称
 */
export async function edit (formData) {
  const url = `${api.dic}/${formData.id}`
  return await request({ url: url, method: 'put', data: formData })
}
/**
 * 删除基础数据
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function deleted (id) {
  return await request({ url: `${api.dic}/${id}`, method: 'delete' })
}
 
export async function getTree () {
  return await request({ url: `${api.dic}/tree`, method: 'get' })
}

