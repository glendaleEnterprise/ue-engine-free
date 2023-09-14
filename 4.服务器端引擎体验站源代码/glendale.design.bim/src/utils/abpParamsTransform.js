//STable参数转换
// 默认列表查询条件
const baseListQuery = {
  page: 1,
  limit: 10
}

// 查询条件转化
export function transformAbpListQuery (query) {
  query.filter = query.filter === '' ? undefined : query.filter

  if (window.isNaN(query.pageSize)) {
    query.pageSize = baseListQuery.limit
  }
  if (window.isNaN(query.pageNo)) {
    query.pageNo = baseListQuery.page
  }

  const abpListQuery = {
    maxResultCount: query.pageSize,
    skipCount: (query.pageNo - 1) * query.pageSize,
    sorting: '',
    filter: '',
    ...query
  }

  if (typeof (query.sortField) !== 'undefined' && query.sortField !== null) {
    abpListQuery.sorting = query.sortOrder === 'ascend'
      ? query.sortField
      : `${query.sortField} Desc`
  }

  return abpListQuery
}
// 查询结果转化
export function transformAbpQueryResult (data, message, code = 0, headers = {}) {
  const responseBody = {}
  responseBody.result = data
  if (message !== undefined && message !== null) {
    responseBody.message = message
  }
  if (code !== undefined && code !== 0) {
    responseBody.code = code
    responseBody._status = code
  }
  if (headers !== null && typeof headers === 'object'
    && Object.keys(headers).length > 0) {
    responseBody._headers = headers
  }
  responseBody.timestamp = new Date().getTime()
  return responseBody
}

// 分页查询结果转化
export function buildPagingQueryResult (queryParam, data) {
  for (const item of data.items) {
    // Ant Design Vue 中要求每行数据中必须存在字段：key
    item.key = item.id
  }
  const pagedResult = {
    pageSize: queryParam.pageSize,
    pageNo: queryParam.pageNo,
    totalCount: data.totalCount,
    totalPage: data.totalCount / queryParam.pageSize,
    data: data.items,
    skipCount: queryParam.skipCount,
  }
  return transformAbpQueryResult(pagedResult)
}
