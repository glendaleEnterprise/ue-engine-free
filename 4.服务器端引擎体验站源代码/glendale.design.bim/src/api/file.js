import request from '@/utils/request'
import store from '@/store'

const api = {
  file: `/api/app/file`,   
  download:`/api/file-management/files`,
  downloadtype:`/api/file-management/files/GetModel`
  // preview: `/api/file-management/files`
}

/**
 * 上传文件-普通上传
 * @param {*} parameter 
 * @returns 
 */
export function uploadFile (parameter) {
  return request({
    url: api.file,
    method: 'post',
    data: parameter
  })
}

/**
 * 断点续传，自动合并
*/
export function uploadBreakpoint(params){
  const formData = new FormData()
  formData.append('file', params.file, params.fileName)
  return request({
    url:`${api.file}/breakpoint-continuation?fileName=${params.fileName}&hashName=${params.hashName}&chunks=${params.chunks}&chunk=${params.chunk}`,
    method:'post',
    data:formData
  })
}

/**
 * 断点续传，自动合并
 */
export function getBlobName(params){
  return request({
    url:`/api/app/uploads/${params}`,
    method:'post',
  })
}

// ${apiurl}/uploads/${blobName}
 

/**
 * 下载文件
*/
export function downloadFile(blobName){
  return request({
    url: `${api.download}/${blobName}`,
    method: 'get'     
  })
}
/**
 * 查看文件
 * @returns 
 */
 export function getFileByBlobName (blobName) {
  return `${store.state.baseUrl}${api.file}/?blobName=${blobName}`
}

export function getDocumentById (id) {
  return `${store.state.baseUrl}${api.fileManagement}/?blobName=${id}`
}

/**
 * 下载文件
*/
export function downloadFiletype(blobName, filetype){
  window.open(`${store.state.baseUrl}${api.downloadtype}/${blobName}?filetype=${filetype}`)
  // return request({
  //   url: `${api.downloadtype}/${blobName}?filetype=${filetype}`,
  //   method: 'get'     
  // })
}
// /**
//  * 文件浏览
//  * @param {*} blobName 
//  * @returns 
//  */
// export function previewFile (blobName) {
//   return request({
//     url: `${api.preview}/` + blobName,
//     method: 'get',
//   })
// }


