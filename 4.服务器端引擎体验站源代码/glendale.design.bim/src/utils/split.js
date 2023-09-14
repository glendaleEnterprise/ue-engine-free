const SparkMD5 = require('spark-md5')


export async function splitResult(file, chunkSize) {
  // 将文件按固定大小（2M）进行切片，注意此处同时声明了多个常量
  //  const chunkSize = 2097152,
  //文件分片
  //const chunkSize = 10 * 1024 * 1024 //分片大小10M
  const chunks = Math.ceil(file.size / chunkSize)//分片总数量
  let index = 0
  const chunkList= []
  const suffix = /\.([0-9A-z]+)$/.exec(file.name)[1]
  // 将分片添加到Promise中
  while (index < chunks) {
    const item = {
      chunk: file.slice(index * chunkSize, (index + 1) * chunkSize),
      fileName: `${ file.name}_${index}.${suffix}`, // 文件名规则按照 hash_1.jpg 命名
    }
    chunkList.push(item)
    // const chunkFile = file.slice(index * chunkSize, (index + 1) * chunkSize) //file对象的slice方法用于切出文件的一部分
    // const formData = new FormData()
    // formData.append('file', chunkFile, file.name)
    // formData.append('chunk', index)// 当前是第几片
    // formData.append('chunks', chunks)// 总片数
    // formData.append('input', JSON.stringify(input))
    index++
  }
  return {
    chunkList: chunkList,
    chunkListLength: chunks,
    suffix: suffix
  }
  //     const
  //       chunkList = [], // 保存所有切片的数组
  //       chunkListLength = Math.ceil(file.size / chunkSize), // 计算总共多个切片
  //       suffix = /\.([0-9A-z]+)$/.exec(file.name)[1] // 文件后缀名
  //     let buffer
  //     await fileToBuffer(file)
  //       .then((data) => {
  //         buffer = data.slice(0, 20000)
  //       })
  //       .catch((err) => {
  //         console.log('上传失败！')      
  //       })

  //     // 根据文件内容生成 hash 值
  //     const spark = new SparkMD5.ArrayBuffer()
  //     spark.append(buffer)
  //     const hash = spark.end()

  //     // 生成切片，这里后端要求传递的参数为字节数据块（chunk）和每个数据块的文件名（fileName）
  //     let curChunk = 0 // 切片时的初始位置
  //     for (let i = 0; i < chunkListLength; i++) {
  //       const item = {
  //         chunk: file.slice(curChunk, curChunk + chunkSize),
  //         fileName: `${hash}_${i}.${suffix}`, // 文件名规则按照 hash_1.jpg 命名
  //       }
  //       curChunk += chunkSize
  //       chunkList.push(item)
  //     }
  //     return {
  //       chunkList: chunkList,
  //       chunkListLength: chunkListLength,
  //       hash: hash,
  //       suffix: suffix
  //     }

}

function fileToBuffer(file) {
  return new Promise((resolve, reject) => {
    const fr = new FileReader()
    fr.onload = (e) => {
      resolve(e.target.result)
    }
    fr.readAsArrayBuffer(file)
    fr.onerror = () => {
      reject(new Error('转换文件格式发生错误'))
    }
  })
}