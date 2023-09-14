import qs from 'qs'
import request from '@/utils/request'
import { transformAbpListQuery, buildPagingQueryResult } from '@/utils/abpParamsTransform'
import requestModel from '@/utils/requestModel'
import store from '@/store'
import cloneDeep from 'lodash.clonedeep'

const api = {
  // 文档
  doc: `/api/app/document`,
  children: `/api/app/document/children`,
  //文档日志
  docLog: `/api/app/document-log`,
  docMerge: `/api/app/document-merge`,//合模
  documentHandle: '/api/app/document-handle'
}

/**移动文档文件 */
export function moveDoc({ moveId, documentIds }) {
  return request({
    url: `${api.doc}/move/${moveId}`,
    method: 'put',
    data: documentIds
  })
}
/**公开文档文件 */
export function setPublic({ isPublic, documentIds }) {
  return request({
    url: `${api.doc}/public?isPublic=${isPublic}`,
    method: 'put',
    data: documentIds
  })
}

/**删除文档 */
export function delDoc(id) {
  return request({
    url: `${api.doc}/${id}`,
    method: 'delete',
  })
}
/**
 * 删除多个文档
*/
export function mdeleteDoc(ids) {
  return request({
    url: `${api.doc}/mdelete`,
    method: 'post',
    data: ids
  })
}
/**
 * 查询一个对象
 */
export function getDocById(id) {
  return request({
    url: `${api.doc}/` + id,
    method: 'get'
  })
}

/**
 * 查询多个
*/
export function getArray(parameter) {
  return request({
    url: `${api.doc}/array`,
    method: 'get',
    params: { ids: parameter },
    paramsSerializer: function (parameter) {
      return qs.stringify(parameter, { indices: false })
    }
  })
}

/**查询文档列表 */
export function getDocList(parameter) {
  return request({
    url: api.doc,
    method: 'get',
    params: parameter,
    paramsSerializer: params => {
      return qs.stringify(params, { indices: false })
    }
  })
}

/**查询文档列表 */
export function getDocListAll(parameter) {
  return request({
    url: `${api.doc}/children`,
    method: 'get',
    params: parameter,
    paramsSerializer: params => {
      return qs.stringify(params, { indices: false })
    }
  })
}





/**
 * 上传文件
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function uploadDocumentFiles(projectId, parentId, formData) {
  return await request(
    {
      url: `${api.documentHandle}/${parentId}/upload-files?projectId=${projectId}`,
      method: 'post',
      data: formData
    }
  )
}
/**
 * 下载文件
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function downloadFile(documentId) {
  return await request(
    {
      url: `${api.documentHandle}/${documentId}/download-files`,
      method: 'post',
      responseType: 'arraybuffer'
    }
  )
}


/**
 * 获取目录用户
 * @param {*} documentId 
 * @returns 
 */
export function getDocUsers(documentId) {
  return request({
    url: `${api.doc}/${documentId}/users`,
    method: 'get',
  })
}

/**根据Id获取文档信息操作记录 */
export function getDocLog(id) {
  return request({
    url: `${api.docLog}/by-docid/${id}`,
    method: 'get',
  })
}

/**新增文档记录*/
export function createLog(docId, logType) {
  return request({
    url: `${api.docLog}/log/${docId}?logType=${logType}`,
    method: 'post',
  })
}

/**根据Id获取Word Html */
export function getWordHtml(fileName, page) {
  return request({
    url: `${api.doc}/word-html?blobName=${fileName}&page=${page}`,
    method: 'get',
  })
}
/**根据Id获取Excel Html */
export function getExcelHtml(fileName) {
  return request({
    url: `${api.doc}/excel-html?fileName=${fileName}`,
    method: 'get'
  })
}



/**
 * 获取视频地址
*/
export function getVideoPath(id) {
  return request({
    url: `${api.doc}/${id}/video-path`,
    method: 'get'
  })
}

/**移动文件 */
export function moveFile(id, parameter) {
  return request({
    url: `${api.doc}/move/${id}`,
    method: 'put',
    data: parameter
  })
}
/**
 * 获取文档合模
 * @param {*} documentId 
 * @returns 
 */
export function getDocMerges(documentId) {
  return request({
    url: `${api.doc}/${documentId}/merge`,
    method: 'get',
  })
}
/**获取模型统计和资料统计的数量*/
export function getNumOfEachFormat(id) {
  return request({
    url: `${api.doc}/${id}/num-of-each-format`,
    method: 'get'
  })
}


export function breakpointContinuation(params) {
  return request({
    url: `${api.documentHandle}/breakpoint-continuation/${params.parentId}`,
    method: 'post',
    data: params
  })
}

export function merge(params) {
  return request({
    url: `${api.documentHandle}/${params.id}/merge`,
    method: 'post',
    params: params
  })
}


/**
 * 获取文件大小
*/
export function getDownloadFileSize(fileName) {
  return request({
    url: `${api.documentHandle}/download-file-size`,
    method: 'get',
    params: { fileName: fileName }
  })
}

export function downloadFileSlice(params) {
  return request({
    url: `${api.documentHandle}/download-files`,
    method: 'post',
    params: params,
    responseType: 'arraybuffer',
  })
}

export function splitUploadFile(params) {
  return requestModel({
    url: `/api/app/model/SplitUploadFile`,
    method: 'post',
    data: params,
    headers: { 'Accept': 'application/octet-stream' }
  })
}

/**
 * BIM文件上传接口
 * @returns {Promise<AxiosResponse<T>>}
 */
export function uploadModelFile(params) {
  const formData = new FormData()
  formData.append('file', params.file, params.input.Name)
  formData.append('chunk', params.chunk)
  formData.append('chunks', params.chunks)
  let input = cloneDeep(store.state.modelInput)
  params.ConfigJson ? input.ConfigJson = { ...input.ConfigJson, ...params.ConfigJson } : ''
  input = { ...input, ...params.input }
  formData.append('input', JSON.stringify(input))
  return requestModel({
    url: `/api/app/model/SplitUploadFile`,
    method: 'post',
    data: formData,
    headers: { 'Content-Type': 'application/octet-stream' }
  })
}
/**
 * PAK文件上传接口
 * @returns {Promise<AxiosResponse<T>>}
 */
export function uploadPakFile(params) {
  const formData = new FormData()
  formData.append('file', params.file, params.input.Name)
  formData.append('chunk', params.chunk)
  formData.append('chunks', params.chunks)
  formData.append('LightweightName', params.input.LightweightName?params.input.LightweightName:'')
  formData.append('CallBackUrl', params.CallBackUrl)
  // let input = cloneDeep(store.state.modelInput)
  // params.ConfigJson ? input.ConfigJson = { ...input.ConfigJson, ...params.ConfigJson } : ''
  // input = { ...input, ...params.input }
  // formData.append('input', JSON.stringify(input))
  return requestModel({
    url: `/api/app/Pak/PakSplitUploadFile`,
    method: 'post',
    data: formData,
    headers: { 'Content-Type': 'application/octet-stream' }
  })
}
/**
 * CAD文件上传接口
 * @returns {Promise<AxiosResponse<T>>}
 */
export function uploadCADFile(params) {
  const formData = new FormData()
  formData.append('file', params.file, params.input.Name)
  formData.append('chunk', params.chunk)
  formData.append('chunks', params.chunks)
  let input = {}
  input = { ...input, ...params.input }
  formData.append('input', JSON.stringify(input))
  return requestModel({
    url: `/api/app/cad-drawing/SplitUploadFile`,
    method: 'post',
    data: formData
  })
}

/**
 * Osgb文件上传接口
 * @returns {Promise<AxiosResponse<T>>}
 */
export function uploadOsgbFile(params) {
  const formData = new FormData()
  formData.append('file', params.file, params.input.Name)  //此处name值一直保持一致，因此不能用params.fileName   
  formData.append('chunk', params.chunk)
  formData.append('chunks', params.chunks)
  let input = store.state.gisInput
  input = { ...input, ...params.input }
  formData.append('input', JSON.stringify(input))
  return requestModel({
    url: `/api/app/gismodel/OSGBSplitUploadFile`,
    method: 'post',
    data: formData
  })
}

/**
 * Osgb文件上传接口
 * file 要上传的文件
 * chunk 分段索引
 * modelname 接口返回的模型名称
 * blockSize 分块后每块的大小
 * UploadSend 方法执行中的回调，返回执行进度
 * UploadOver 方法执行完成后的回调
 */
export function uploadOsgbSplitFile(file, chunk, modelname, blockSize, UploadSend, UploadOver) {
  var chunkCont = Math.ceil(file.size / blockSize)
  var nextSize = Math.min((chunk + 1) * blockSize, file.size);//读取到结束位置            
  var fileData = file.slice(chunk * blockSize, nextSize);//截取 部分文件 块
  var filename = encodeURIComponent(file.name);

  const formData = new FormData()
  formData.append('file', fileData, filename);//将 部分文件 塞入FormData
  formData.append("chunk", chunk);
  formData.append("chunks", chunkCont);

  var index1 = filename.lastIndexOf(".");
  var index2 = filename.length;
  var postf = filename.substring(index1 + 1, index2).toLowerCase();//后缀名

  //模型文件上传接口传递的参数
  var params = {
    Name: file.name,
    LightweightName: modelname,
    Priority: '203',
  }

  let input = store.state.gisInput
  input = { ...input, ...params }

  formData.append('input', JSON.stringify(input))
  return requestModel({
    url: `/api/app/gismodel/OSGBSplitUploadFile`,
    method: 'post',
    data: formData
  }).then((retdata) => {
    if (chunk + 1 == chunkCont) {//如果上传完成，则跳出继续上传
      if (UploadOver) {
        UploadOver(retdata, file.name);
      }
    }
    else {
      if (UploadSend) {
        var sendint = (nextSize / file.size * 100).toFixed(2)
        UploadSend(parseFloat(sendint))
      }
      uploadOsgbSplitFile(file, ++chunk, retdata.datas.lightweightName, blockSize, UploadSend, UploadOver);//递归调用
    }
  })
}

/**
 * shp文件上传接口
*/
export function uploadShpFile(params) {
  const formData = new FormData()
  formData.append('file', params.file, params.input.Name)
  formData.append('chunk', params.chunk)
  formData.append('chunks', params.chunks)
  let input = store.state.gisInput
  input = { ...input, ...params.input }
  formData.append('input', JSON.stringify(input))
  return requestModel({
    url: `/api/app/gismodel/ShpSplitUploadFile`,
    method: 'post',
    data: formData
  })
}

/**
 * 点云文件上传
*/
export function uploadPointCloudFile(params) {
  const formData = new FormData()
  formData.append('file', params.file, params.input.Name)
  formData.append('chunk', params.chunk)
  formData.append('chunks', params.chunks)
  let input = store.state.gisInput
  input = { ...input, ...params.input }
  formData.append('input', JSON.stringify(input))
  return requestModel({
    url: `/api/app/gismodel/PointCloudSplitUploadFile`,
    method: 'post',
    data: formData
  })
}



/**
 * BIM下载
*/
export function downloadModel(params) {
  return requestModel({
    url: `/api/app/model/model-SourceFileurl?LightweightName=${params}`,
    method: 'get',
    // responseType: 'blob',
  })
}

/**
 * cad下载
*/
export function downloadCAD(params) {
  return requestModel({
    url: `/api/app/model/model-SourceFileurl?LightweightName=${params}`,
    method: 'get',
    responseType: 'blob',
  })
}

/**
 * Osgb下载, gis模型文件下载
*/
export function downloadOsgb(params) {
  return requestModel({
    url: `/api/app/gismodel/OSGB-SourceFileurl?LightweightName=${params}`,
    method: 'get',
    // responseType: 'blob',
  })
}


/**
 * 上传document
*/
export function createDocument(params) {
  return request({
    url: `${api.doc}`,
    method: 'post',
    data: params
  })
}

/**
 * 坐标校正
 * @returns {Promise<AxiosResponse<T>>}
 */
export function uploadCoordinate(params) {
  return requestModel({
    url: `/api/app/model/coordinate-correction-service?LightweightName=${params.LightweightName}&lon=${params.lon}&lat=${params.lat}&height=${params.height}`,
    method: 'post',
    headers: { 'Content-Type': 'application/json;charset=utf8', 'Token': store.state.bim.defaults.secretkey }
  })
}


/**存储默认视点 */
export function setSceneInformation(id, documentIds) {
  return request({
    url: `${api.doc}/${id}`,
    method: 'put',
    data: documentIds
  })
}

/**封面设置 */
export function modelProjectCover(id, BlobName) {
  return request({
    url: `${api.doc}/${id}/blob-name?BlobName=${BlobName}`,
    method: 'put',
  })
}

/**存储默认视点 */
export function setModelDefaultPerspective(id, documentIds) {
  return request({
    url: `${api.doc}/${id}/camera?Camera=${documentIds}`,
    method: 'put',
    data: documentIds
  })
}

/**存储模型设置等配置 */
export function setModelConfig(id, documentIds) {
  return request({
    url: `${api.doc}/${id}/model-config?ModelConfig=${documentIds}`,
    method: 'put',
    data: documentIds
  })
}


/**存储场景设置配置 */
export function setSceneConfig(id, documentIds) {
  return request({
    url: `${api.doc}/${id}/scene-config?SceneConfig=${documentIds}`,
    method: 'put',
    data: documentIds
  })
}

/**存储模型位置 */
export function setMatrix(id, documentIds) {
  return request({
    url: `${api.doc}/${id}/matrix?Matrix=${documentIds}`,
    method: 'put',
    data: documentIds
  })
}


 