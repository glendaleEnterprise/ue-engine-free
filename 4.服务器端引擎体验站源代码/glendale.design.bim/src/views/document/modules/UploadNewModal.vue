<template>
  <a-modal
    title="上传新版模型"
    :width="600"
    :maskClosable="false"
    :visible="visible"
    :confirmLoading="confirmLoading"
    @ok="handleOk"
    @cancel="hide"
  >
    <a-form :form="form" :labelCol="{ span: 4 }" :wrapperCol="{ span: 18 }">
      <a-form-item label="选择文件">
        <a-input
          type="hidden"
          v-decorator="['files', { rules: [{ required: true, message: '请选择要上传的文件' }] }]"
        />
        <a-upload
          :file-list="fileList"
          :accept="accept"
          :remove="handleRemove"
          :before-upload="beforeUpload"
          :multiple="false"
        >
          <a-button> <a-icon type="upload" /> 上传文件 </a-button>
        </a-upload>
        <div v-show="progressShow" style="width: 100%">
          <a-progress :percent="percent" />
        </div>
      </a-form-item>
      <a-form-item label="版本号设置">
        <a-radio-group v-decorator="['versionNo',{ rules: [{ required: true, message: '必须选择一项' }] }]">
          <a-radio :value="0"> 系统自动 </a-radio>
          <a-radio :value="-1"> 升级主版本号 </a-radio>
        </a-radio-group>
      </a-form-item>
      <a-form-item label="新版说明">
        <a-textarea
          v-decorator="['remark', { rules: [{ max: 100, message: '最多100个字符' }] }]"
          placeholder="最多100个字符"
          :auto-size="{ minRows: 4 }"
        />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script>
import { mapGetters } from 'vuex'
import { uploadModelFile, uploadOsgbFile, uploadOsgbSplitFile, uploadShpFile, uploadPointCloudFile } from '@/api/document'
import { uploadVersion } from '@/api/docVer'
import { splitResult } from '@/utils/split'
import { getPostilHandle } from '@/api/postil'
export default {
  name: 'UploadNewModal',
  data() {
    return {
      visible: false,
      confirmLoading: false,
      form: this.$form.createForm(this),
      documentId: [],
      document: null,
      fileList: [],
      percent: 0, //上传进度
      upload: false, //正在上传
      percentCount: 0,
      progressShow: false,
      curentIndex: 0,
      accept: '',
      lightweightName: '',
      columns: [
        {
          title: '标题',
          dataIndex: 'title',
        },
        {
          title: '优先级',
          dataIndex: 'priorityName',
          align: 'center',
          width: '100px',
        },
        {
          title: '问题批注',
          dataIndex: 'imgPath',
          align: 'center',
          width: '80px',
          scopedSlots: { customRender: 'renderImgPath' },
        },
      ],
    }
  },
  computed: {},
  methods: {
    //上传界面初始化
    show(record) {
      this.accept = record.documentName.substring(record.documentName.lastIndexOf('.'), record.documentName.length)
      this.documentId = record.id
      this.document = record
      this.curentIndex = 0
      this.progressShow = false
      this.percentCount = 0
      this.percent = 0
      this.upload = false
      this.visible = true
      this.fileList = []
      this.$nextTick(() => {
        this.form.setFieldsValue({ remark: '',versionNo:0, files: undefined })
      })
      getPostilHandle({ docId: record.id, status: 2, MaxResultCount: 1000, SkipCount: 0 }).then((res) => {
        this.data = res.items
      })
    },
    hide() {
      this.visible = false
    },
    handleOk() {
      const that = this
      // 触发表单验证
      that.form.validateFields(async (err, values) => {
        // 验证表单没错误
        if (!err) {
          that.confirmLoading = true
          that.progressShow = true
          that.upload = true
          const parentId = values.id
          const file = that.fileList[0]
          if (!file) {
            that.$message.error('请选择文件')
            return
          }
          that.percent = 0
          that.bigFileName = file.name
          that.parentId = parentId

          if (that.document.documentType == 2 && that.document.modelFormat === 'osgb') {
            uploadOsgbSplitFile(file, 0, '', 1024 * 1024 * 10,
            (res) => { //进度
              that.percent = res
              if (that.percent > 99) {
                that.percent = 99
              }
            },
            (res) => { //结束
              that.lightweightName = res.datas.lightweightName
              that.complete()
            })
          }
          else{
            const res = await splitResult(file, 1024 * 1024 * 2)
            that.chunkList = res.chunkList
            that.hash = res.hash
            that.suffix = res.suffix.toLowerCase()
            that.chunkListLength = res.chunkListLength
            if (that.accept === '.' + that.suffix) {
              await that.splitUploadFile() //上传模型
            } else {
              that.$message.error('请选择正确格式文件')
            }
          }
        }
      })
    },

    onChange(e) {
      const [id] = [...e].reverse() //获取下拉框数组最后一位
      this.form.setFieldsValue({ id: id })
    },
    handleRemove(file) {
      const index = this.fileList.indexOf(file)
      const newFileList = this.fileList.slice()
      newFileList.splice(index, 1)
      this.fileList = newFileList
    },
    beforeUpload(file) {
      if (this.fileList.length >= 1) {
        this.$message.warning('一次只能上传一个文件！')
        return false
      }
      if(this.document.documentName != file.name){
        this.$message.warning('注意：与源文件名称不一致')
      }
      var suffix = file.name.substring(file.name.lastIndexOf('.'), file.name.length)
      if (this.accept.indexOf(suffix) < 0) {
        this.$message.error('请选择正确格式文件')
        return false
      }
      if (file.name.split('.')[0].length <= 50) {
        this.fileList = [...this.fileList, file]
        this.form.setFieldsValue({ files: this.fileList.map((x) => x.uid).join(',') })
      } else {
        this.$message.warning('文件名称不能超过50个字符')
      }
      return false
    },
    //模型上传
    async splitUploadFile() {
      var that = this;
      const requestList = [] // 请求集合
      that.chunkList.forEach((item, index) => {
        const fn = () => {
          const paras = {
            file: item.chunk,
            fileName: item.fileName,
            chunk: index,
            chunks: that.chunkListLength,
            input: {
              Name: that.bigFileName,
              LightweightName: that.lightweightName,
              Priority: '203',
            },
          }

          var promise = new Promise((resolve, reject) => {
            if (that.document.documentType == 1) {
              uploadModelFile(paras).then((res) => {
                if (res) {
                  if (that.percentCount === 0) {
                    // 避免上传成功后会删除切片改变 chunkList 的长度影响到 percentCount 的值
                    that.percentCount = 100 / that.chunkList.length
                  }
                  that.percent += that.percentCount // 改变进度
                  that.percent = +that.percent.toFixed(2)
                  if (that.percent > 99) {
                    that.percent = 99
                  }
                  resolve(res)
                }
              })
            } else if (that.document.documentType == 2) {
              if (that.document.modelFormat === 'osgb') {
                uploadOsgbFile(paras).then((res) => {
                  if (res) {
                    if (that.percentCount === 0) {
                      // 避免上传成功后会删除切片改变 chunkList 的长度影响到 percentCount 的值
                      that.percentCount = 100 / that.chunkList.length
                    }
                    that.percent += that.percentCount // 改变进度
                    that.percent = +that.percent.toFixed(2)
                    if (that.percent > 99) {
                      that.percent = 99
                    }
                    resolve(res)
                  }
                })
              }else if (that.document.modelFormat === 'shp') {
                uploadShpFile(paras).then((res) => {
                  if (res) {
                    if (that.percentCount === 0) {
                      // 避免上传成功后会删除切片改变 chunkList 的长度影响到 percentCount 的值
                      that.percentCount = 100 / that.chunkList.length
                    }
                    that.percent += that.percentCount // 改变进度
                    that.percent = +that.percent.toFixed(2)
                    if (that.percent > 99) {
                      that.percent = 99
                    }
                    resolve(res)
                  }
                })
              }else if (that.document.modelFormat === '点云') {
                uploadPointCloudFile(paras).then((res) => {
                  if (res) {
                    if (that.percentCount === 0) {
                      // 避免上传成功后会删除切片改变 chunkList 的长度影响到 percentCount 的值
                      that.percentCount = 100 / that.chunkList.length
                    }
                    that.percent += that.percentCount // 改变进度
                    that.percent = +that.percent.toFixed(2)
                    if (that.percent > 99) {
                      that.percent = 99
                    }
                    resolve(res)
                  }
                })
              }
            }
          })
          return promise
        }
        requestList.push(fn)
      })
      const send = async () => {
        if (!that.upload) {
          return
        }
        if (that.curentIndex >= requestList.length) {
          // 发送完毕
          that.complete()
          return
        }
        await requestList[that.curentIndex]().then((a) => {
          if (a.code == 1) {
            that.lightweightName = a.datas.lightweightName
            that.curentIndex++
          } else {
            that.upload = false
            that.$message.error(`上传失败！`)
          }
        })
        await send()
      }
      send() // 发送请求
    },
    complete(){
      var that = this;
      that.form.validateFields(async (err, values) => {
          const params = {
            documentId: that.documentId,
            modelName: that.lightweightName,
            size: that.fileList[0].size,
            versionNo: values.versionNo,
            remark: values.remark,
            isCurrent: true,
          }

          uploadVersion(params)
            .then((res) => {
              if (res) {
                that.percent = 100
                that.upload = false
                that.$message.success('上传成功')
                that.$emit('fetch')
                that.confirmLoading = false
                that.hide()
              } else {
                that.$message.error(`上传失败！`)
                that.confirmLoading = false
                that.hide()
              }
            })
            .catch((err) => {
              that.$message.error(`上传失败！`)
            })
        })
    }
  },
}
</script>

<style scoped>
</style>
