<template>
  <a-modal title="上传模型" :width="800" :maskClosable="false" :visible="visible" :confirmLoading="confirmLoading"
    @ok="handleOk" @cancel="hide" :ok-text="$t('public.OK')" :cancel-text="$t('public.cancel')">
    <a-form :form="form" :labelCol="{ span: 4 }" :wrapperCol="{ span: 18 }" class="scroll-box" :hideRequiredMark="true">
      <a-form-item label="所属目录">
        <a-cascader v-model="documentIds" :options="docTree" placeholder="请选择" @change="onChange" change-on-select
          expand-trigger="hover" :fieldNames="{ label: 'title', value: 'key', children: 'children' }" />
        <input type="hidden" v-decorator="['id', { rules: [{ required: true, message: '请选择所属目录' }] }]" />
      </a-form-item>
      <a-form-item label="上传类型" style="display: none;">
        <a-select v-model='uploadType'>
          <a-select-option value="0">
            单模型上传
          </a-select-option>
          <a-select-option value="1">
            同一建模原点批量上传
          </a-select-option>
        </a-select>
      </a-form-item>
      <a-form-item label="模型类型">
        <div>
          <a-cascader placeholder="请选择" :options="categoryList" @change="onChangeOptions"
            v-decorator="['options', { rules: [{ required: true, message: '请选择' }] }]">
          </a-cascader>
        </div>
      </a-form-item>
      <div v-if='showScene'>
        <a-form-item label="模型应用场景">
          <a-select v-model='scene'>
            <a-select-option value="a">
              纯BIM场景
            </a-select-option>
            <a-select-option value="b">
              GIS+BIM 融合场景
            </a-select-option>
          </a-select>
        </a-form-item>
        <div v-if='scene === "b"'>
          <a-form-item label="定位方式">
            <a-radio-group default-value="a" button-style="solid" v-model='loType'>
              <a-radio-button value="a">
                EPSG代号
              </a-radio-button>
              <a-radio-button value="b">
                经纬高（弧度制）
              </a-radio-button>
              <a-radio-button value="c">
                经纬高（角度制）
              </a-radio-button>
            </a-radio-group>
          </a-form-item>
          <a-row v-if='loType === "a"'>
            <a-col :span="12">
              <a-form-item label="EPSG代号" :labelCol="{ span: 8 }" :wrapperCol="{ span: 14 }">
                <a-input v-decorator="['srs']" />
              </a-form-item>
            </a-col>
            <a-col :span="10">
              <a-form-item label="项目基点" :labelCol="{ span: 8 }" :wrapperCol="{ span: 16 }">
                <a-input v-decorator="['srsOrigin']" />
              </a-form-item>
            </a-col>
          </a-row>
          <a-row v-if='loType === "b" || loType === "c"'>
            <a-col :span="12">
              <a-form-item label="经度" :labelCol="{ span: 8 }" :wrapperCol="{ span: 14 }">
                <a-input v-decorator="['longitude']" />
              </a-form-item>
            </a-col>
            <a-col :span="10">
              <a-form-item label="纬度" :labelCol="{ span: 8 }" :wrapperCol="{ span: 16 }">
                <a-input v-decorator="['latitude']" />
              </a-form-item>
            </a-col>
          </a-row>
          <a-row v-if='loType === "b" || loType === "c"'>
            <a-col :span="12">
              <a-form-item label="高度" :labelCol="{ span: 8 }" :wrapperCol="{ span: 14 }">
                <a-input v-decorator="['transHeight']" />
              </a-form-item>
            </a-col>
          </a-row>
        </div>
        <a-form-item label="LOD模式">
          <a-select v-model='isLod'>
            <a-select-option :value="0">
              不启用
            </a-select-option>
            <a-select-option :value="1">
              启用
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-row v-if="isLod == 1">
          <a-col :span="12">
            <a-form-item label="X向切块数" :labelCol="{ span: 8 }" :wrapperCol="{ span: 14 }">
              <a-input-number placeholder="请输入X向切块数" style="width: 100%;"
                v-decorator="['xCount', { rules: [{ required: true, message: '请输入X向切块数' }], initialValue: 5 }]"
                :min="1" />
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item label="Y向切块数" :labelCol="{ span: 8 }" :wrapperCol="{ span: 16 }">
              <a-input-number placeholder="请输入Y向切块数" style="width: 100%;"
                v-decorator="['yCount', { rules: [{ required: true, message: '请输入Y向切块数' }], initialValue: 5 }]"
                :min="1" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row v-if="isLod == 1">
          <a-col :span="12">
            <a-form-item label="Z向切块数" :labelCol="{ span: 8 }" :wrapperCol="{ span: 14 }">
              <a-input-number placeholder="请输入Z向切块数" style="width: 100%;"
                v-decorator="['level', { rules: [{ required: true, message: '请输入Z向切块数' }], initialValue: 1 }]"
                :min="1" />
            </a-form-item>
          </a-col>
        </a-row>
        <!-- <a-form-item label="数据压缩">
          <a-select v-model="draco" placeholder='请选择数据压缩状态'>
            <a-select-option :value="0">
              关闭
            </a-select-option>
            <a-select-option :value="1">
              开启
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="压缩级别" v-if="draco == 1">
          <a-input-number placeholder="请输入压缩级别"
            v-decorator="['compressionLevel', { rules: [{ required: true, message: '请输入压缩级别' }], initialValue: 10 }]"
            :min="1" :max="10" style="width: 100%;" />
        </a-form-item> -->
      </div>
      <a-form-item v-if="software === 'revit'" label="轻量化模式">
        <a-select v-decorator="[
          'style',
          { rules: [{ required: true, message: '请选择轻量化模式' }], initialValue: '1' }
        ]">
          <a-select-option value="1">
            真实模式
          </a-select-option>
          <a-select-option value="0">
            着色模式
          </a-select-option>
        </a-select>
      </a-form-item>
      <a-form-item v-if="software === 'revit' || software === 'bentley' || software === 'stp|step'" label="精细度">
        <a-select v-decorator="[
          'accuracy',
          { rules: [{ required: true, message: '请选择精细度' }], initialValue: '5' }
        ]">
          <a-select-option value="3">
            低精度
          </a-select-option>
          <a-select-option value="5">
            标准
          </a-select-option>
          <a-select-option value="7">
            高精度
          </a-select-option>
        </a-select>
      </a-form-item>
      <a-form-item label="选择文件">
        <a-input type="hidden" v-decorator="['files', { rules: [{ required: true, message: '请选择要上传的文件' }] }]" />
        <a-upload-dragger :file-list="fileList" :remove="handleRemove" :before-upload="beforeUpload" :multiple="true"
          :showUploadList="{ showRemoveIcon: showDelete }" :accept="accept">
          <p class="ant-upload-drag-icon">
            <a-icon type="inbox" />
          </p>
          <p class="ant-upload-text">
            单击或拖动文件到此区域进行上传
          </p>
        </a-upload-dragger>
        <div v-show="progressShow" style="width: 100%">
          <a-progress :percent="percent" />
        </div>
      </a-form-item>
      <!-- <a-form-item label="是否公开">
        <a-radio-group v-decorator="['isPublic']" @change="onChangeOpenStatus">
          <a-radio :value="true"> 公开 </a-radio>
          <a-radio :value="false"> 私有 </a-radio>
        </a-radio-group>
      </a-form-item> -->
      <a-form-item :wrapperCol="{ offset: 1, span: 21 }" label="">
        <div v-if="accept.length > 0" style="color: red">
          支持的格式：{{ accept.length > 0 ? accept.split(',').join(' ') : '' }}
        </div>
        <div style="color: #999; line-height: 24px">
          {{ description }}
        </div>
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script>
  import store from '@/store'
  import {
    mapGetters
  } from 'vuex'
  import {
    createDocument,
    uploadModelFile,
    uploadPakFile,
    uploadOsgbFile,
    uploadOsgbSplitFile,
    uploadShpFile,
    uploadPointCloudFile,
  } from '@/api/document'
  import {
    splitResult
  } from '@/utils/split'
  export default {
    name: 'UploadModal',
    computed: {
      ...mapGetters(['currProject', 'docTree']),
    },
    data() {
      return {
        uploadType: '1',
        scene: 'a',
        showScene: false,
        loType: 'a',
        visible: false,
        confirmLoading: false,
        formModel: {
          projectId: undefined,
          openStatus: 2,
        },
        form: this.$form.createForm(this),
        documentIds: [],
        fileList: [],
        docUserData: [],
        percent: 0, //上传进度
        upload: false, //正在上传
        percentCount: 0,
        progressShow: false,
        curentIndex: 0,
        category: '',
        software: '',
        accept: '', //支持的格式
        description: '', //支持的格式说明
        categoryList: [{
            value: '1',
            label: 'BIM/3D',
            children: [{
                value: 'revit',
                label: 'revit',
                children: [{
                    value: '.rfa,.rvt,.zip',
                    label: '单文件',
                    description: '单文件可直接上传。包含外部贴图的需将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格，将模型文件、贴图文件放到同名文件夹中，模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
                  },
                  {
                    value: '.zip',
                    label: '链接文件',
                    description: '多模型文件须为同一个基点。主文件模型名称必须为英文或英文数字混排，不能含有中文、特殊字符及空格，将主文件及链接文件(路径可以是多层级)放到同名文件夹中，要求模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
                  },
                ],
              },
              {
                value: '.fbx,.zip',
                label: 'fbx',
                description: '如上传压缩包将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格，将模型文件放到同名文件夹中，模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
              },
              {
                value: '.ifc,.zip',
                label: 'ifc',
                description: '如上传压缩包将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格，将模型文件放到同名文件夹中，模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
              },
              {
                value: '.stp,.step,.zip',
                label: 'stp|step',
                description: '如上传压缩包将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格，将模型文件放到同名文件夹中，模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
              },
              {
                value: '.glzip,',
                label: 'tekla',
                description: '此格式由Tekla轻量化插件导出，可联系GLENDALE技术人员获取该插件。',
              },
              {
                value: 'bentley',
                label: 'bentley',
                children: [{
                    value: '.dgn,.cel,.zip',
                    label: '单文件',
                    description: '单文件可直接上传。包含外部贴图的需将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格。将模型文件、贴图文件放到同名文件夹中，要求模型名称、文件夹名称、压缩包名称必须一致， 压缩为ZIP格式。',
                  },
                  {
                    value: '.zip',
                    label: '链接文件',
                    description: '多模型文件须为同一个基点。主文件模型名称必须为英文或英文数字混排，不能含有中文、特殊字符及空格，将主文件及链接文件(路径可以是多层级)放到同名文件夹中，要求模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
                  },
                ],
              },
              {
                value: '.nwd,.nwc,.zip',
                label: 'navisworks',
                description: '如上传压缩包将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格，将模型文件放到同名文件夹中，模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
              },
              {
                value: '.rvm,.zip',
                label: 'pdms',
                description: '如上传压缩包将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格，将模型文件放到同名文件夹中，模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
              },
              {
                value: ',.glzip',
                label: 'catia',
                description: '此格式由Catia轻量化插件导出，可联系GLENDALE技术人员获取该插件。',
              },
              {
                value: '.skp,.zip',
                label: 'sketchUp',
                description: '单文件可直接上传。包含外部贴图的需将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格。将模型文件、贴图文件放到同名文件夹中，要求模型名称、文件夹名称、压缩包名称必须一致， 压缩为ZIP格式。',
              },
              {
                value: '.glzip',
                label: 'glzip',
                description: '此格式由轻量化插件导出，可联系GLENDALE技术人员获取该插件。',
              },
            ],
          },
          {
            value: '2',
            label: 'GIS数据',
            children: [{
                value: '.zip',
                label: 'osgb',
                description: '倾斜摄影数据一般为单个项目文件夹，文件夹内包含Data目录与metadata.xml文件需将最外层项目文件夹名改为英文或英文数字混排，项目文件夹名、压缩包名称必须一致，压缩为ZIP格式。',
              },
              {
                value: '.shp,.zip',
                label: 'shp',
                description: '如上传压缩包将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格，将模型文件放到同名文件夹中，模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',

              },
              {
                value: '.las,.zip',
                label: '点云',
                description: '如上传压缩包将模型文件名改为英文或英文数字混排，不能含有中文、特殊字符及空格，将模型文件放到同名文件夹中，模型名称、文件夹名称、压缩包名称必须一致，压缩为ZIP格式。',
              },
            ],
          },
          {
            value: '3',
            label: 'UE工程模型',
            description: '将.pak、.sig文件放到同名文件夹中，以ZIP格式压缩文件夹，模型名称、文件夹名称、压缩包名称必须一致',
            accept: '.zip'
          }
        ],
        lightweightName: '',
        curentIndex2: 0,
        showDelete: true,
        allModelList: 0,
        allModelListCount: 0,
        requestList: [],
        successfullyLoaded: 0,
        isLod: 0,
        // docTree: []
        // draco: 0,
      }
    },
    methods: {
      //模型类型下拉
      onChangeOptions(e, selectedOptions) {
        if (selectedOptions && e.length > 0) {
          this.showScene = e[0] == 1
          this.fileList = []
          this.form.setFieldsValue({
            files: undefined
          })
          if (e[0] == '3') {
            this.accept = selectedOptions[0].accept
            this.category = e[0]
            this.software = selectedOptions[0].label
            this.description = selectedOptions[0].description
          } else {
            this.accept = [...e].reverse()[0]
            this.category = e[0]
            this.software = selectedOptions[1].label
            this.description = [...selectedOptions].reverse()[0].description
          }
        }
      },
      //模型上传到轻量化api
      async splitUploadFile(values, file, data) {
        var that = this;
        let requestList = {} // 请求集合
        const _paras = {}
        _paras.unitRatio = values.unitRatio
        if (that.software == 'revit') {
          _paras.style = values.style ? parseInt(values.style) : values.style
        }
        if (that.loType === 'a') {
          _paras.srsOrigin = values.srsOrigin
          _paras.srs = values.srs ? (values.srs.indexOf('EPSG') == -1 ? 'EPSG:' : '') + values.srs : ''
        } else if (that.loType === 'c') {
          _paras.longitude = (Math.PI / 180) * values.longitude
          _paras.latitude = (Math.PI / 180) * values.latitude
          _paras.transHeight = values.transHeight
        } else {
          _paras.longitude = values.longitude
          _paras.latitude = values.latitude
          _paras.transHeight = values.transHeight
        }
        // _paras.draco = that.draco;
        // _paras.compressionLevel = values.compressionLevel;
        _paras.isLod = that.isLod;
        _paras.xCount = values.xCount;
        _paras.yCount = values.yCount;
        _paras.level = values.level;
        _paras.accuracy = values.accuracy;
        data.chunkList.forEach((item, index) => {
          const fn = () => {
            const paras = {
              file: item.chunk,
              fileName: item.fileName,
              chunk: index,
              chunks: data.chunkListLength,
              CallBackUrl: store.state.baseUrl + "/api/app/document/pakcallback",
              input: {
                Name: file.name,
                LightweightName: file.lightweightName,
                Priority: '203',
              },
              ConfigJson: {
                ..._paras
              },
            }
            var promise = new Promise((resolve, reject) => {
              if (that.category == '1') {
                uploadModelFile(paras).then((res) => {
                  file.lightweightName = res.datas.lightweightName
                  if (res) {
                    that.percentCount--;
                    that.percent += that.allModelListCount // 改变进度
                    that.percent = +that.percent.toFixed(2)
                    if (that.percent > 99) {
                      that.percent = 99
                    }
                    resolve(res)
                  }
                })
              } else if (that.category === '2') {
                if (that.software == 'osgb') {
                  uploadOsgbFile(paras).then((res) => {
                    if (res) {
                      that.percentCount--;
                      that.percent += that.allModelListCount; // 改变进度
                      that.percent = +that.percent.toFixed(2)
                      if (that.percent > 99) {
                        that.percent = 99
                      }
                      resolve(res)
                    }
                  })
                } else if (that.software == 'shp') {
                  //动态值
                  paras.input.ConfigJson = {
                    srs: 'EPSG:32649',
                    height: 'Floor',
                  }
                  uploadShpFile(paras).then((res) => {
                    if (res) {
                      that.percentCount--;
                      that.percent += that.allModelListCount // 改变进度
                      that.percent = +that.percent.toFixed(2)
                      if (that.percent > 99) {
                        that.percent = 99
                      }
                      resolve(res)
                    }
                  })
                } else if (that.software == '点云') {
                  //动态值
                  paras.input.PointCloudConfigJson = {
                    format: 'las',
                    srs: 'EPSG:32649',
                    simplify: 0,
                    longitude: 115.79,
                    latitude: 40.5,
                    height: 0,
                  }
                  uploadPointCloudFile(paras).then((res) => {
                    if (res) {
                      that.percentCount--;
                      that.percent += that.allModelListCount // 改变进度
                      that.percent = +that.percent.toFixed(2)
                      if (that.percent > 99) {
                        that.percent = 99
                      }
                      resolve(res)
                    }
                  })
                }
              } else if (that.category === '3') {
                uploadPakFile(paras).then((res) => {
                  if (res) {
                    that.percentCount--;
                    that.percent += that.allModelListCount // 改变进度
                    that.percent = +that.percent.toFixed(2)
                    if (that.percent > 99) {
                      that.percent = 99
                    }
                    resolve(res)
                  }
                })
              }
            })
            return promise
          }
          if (index == 0) {
            file.curentIndex = 0
            requestList = {
              fn: [fn],
              file: file
            }
          } else {
            requestList.fn.push(fn)
          }
        })
        that.percentCount += data.chunkList.length;
        let difference = that.fileList.length - that.allModelList;
        that.allModelList++;
        let residualValue = 100 - that.percent;
        let proportion = residualValue / difference;
        that.allModelListCount = proportion / that.percentCount;
        const send = async (data) => {
          if (!that.upload) {
            return
          }
          if (data.file.curentIndex >= data.fn.length) {
            // 发送完毕
            that.complete(data.file)
            return
          }
          await data.fn[data.file.curentIndex]().then((a) => {
            if (a.code == 1) {
              data.file.lightweightName = a.datas.lightweightName
              data.file.curentIndex++
            } else {
              that.upload = false
              data.file.curentIndex++
              that.successfullyLoaded++
              that.$message.error(`上传失败！`)
            }
          })
          await send(data)
        }
        send(requestList) // 发送请求
      },
      complete(file) {
        var that = this;
        that.form.validateFields(async (err, values) => {
          const formModel = {
            ...that.formModel
          }
          const {
            id,
            isPublic,
            projectId
          } = Object.assign(formModel, values)
          let modelConfig = {
            visibleDistance: 2000,
            isLod: that.isLod,
          }
          if (that.software == "osgb") {
            modelConfig.visibleDistance = 0.5;
          } else if (that.software == "点云") {
            modelConfig.visibleDistance = 0.5;
          } else {
            modelConfig.visibleDistance = 2000;
          }
          if (that.isLod == 1) {
            modelConfig.visibleDistance = 100;
          }
          const params = {
            modelName: file.lightweightName,
            parentId: that.parentId,
            documentName: file.name,
            isPublic: isPublic,
            size: file.size,
            projectFolderId: id,
            projectId: projectId,
            documentType: that.category,
            modelFormat: that.software,
            modelConfig: JSON.stringify(modelConfig)
          }
          createDocument(params)
            .then((res) => {
              that.successfullyLoaded++
              if (res != null) {
                that.$message.success(`模型${res.documentName}上传成功`)
                that.$emit('fetch')
                if (that.fileList.length == that.successfullyLoaded) {
                  that.percent = 100
                  that.upload = false
                  that.confirmLoading = false
                  that.hide()
                }
              } else {
                that.$message.error(`模型${res.documentName}上传失败！`)
                if (that.fileList.length == that.successfullyLoaded) {
                  that.confirmLoading = false
                  that.hide()
                }
              }
            })
            .catch((err) => {
              that.successfullyLoaded++
              that.$message.error(`上传失败！`)
            })
        })
      },
      //上传界面初始化
      show(documentId) {
        this.isLod = 0;
        // this.draco = 0;
        this.successfullyLoaded = 0;
        this.allModelListCount = 0;
        this.allModelList = 0;
        this.showDelete = true
        this.confirmLoading = false
        this.curentIndex = 0
        this.scene = 'a'
        this.progressShow = false
        this.percentCount = 0
        this.percent = 0
        this.upload = false
        this.visible = true
        this.showScene = false
        this.software = ''
        this.formModel = {
          projectId: this.currProject.id,
          openStatus: 2,
          userId: []
        }
        this.fileList = []
        const parentIds = []
        this.category = ''
        this.accept = ''
        this.description = '';
        store.dispatch('GetDocTree', this.currProject.id)
        // this.getTreeNodeIds(this.docTree, (data) => data.key === documentId, parentIds)
        // console.log(this.docTree, 'this.docTreethis.docTree')
        this.documentIds = parentIds
        this.$nextTick(() => {
          if (this.documentIds.length > 0) {
            this.form.setFieldsValue({
              files: undefined,
              isPublic: false,
              options: undefined,
              id: documentId
            })
          } else {
            this.form.setFieldsValue({
              files: undefined,
              isPublic: false,
              options: undefined
            })
          }
        })
      },
      hide() {
        this.visible = false
      },
      handleOk() {
        const that = this
        // 触发表单验证
        that.showDelete = false
        that.form.validateFields(async (err, values) => {
          // 验证表单没错误
          if (!err) {
            that.confirmLoading = true
            that.progressShow = true
            that.upload = true
            const parentId = values.id
            if (that.fileList.length == 0) {
              that.$message.error('请选择文件')
              return
            }
            if (that.uploadType == '0') {
              if (that.fileList.length > 1) {
                that.$message.warning('此时为单模型上传默认上传')
              }
              that.UploadJudgment(parentId, that.fileList[0], values)
            } else {
              that.fileList.forEach(item => {
                that.UploadJudgment(parentId, item, values)
              })
            }
          }
        })
      },
      async UploadJudgment(id, file, values) {
        const that = this;
        that.parentId = id
        that.percent = 0
        if (that.category == 2 && that.software === 'osgb') {
          uploadOsgbSplitFile(file, 0, '', 1024 * 1024 * 10,
            (res) => { //进度
              that.percent = res
              if (that.percent > 99) {
                that.percent = 99
              }
            },
            (res) => { //结束
              file.lightweightName = res.datas.lightweightName
              that.complete(file)
            })
        } else {
          const res = await splitResult(file, 1024 * 1024 * 2)
          that.chunkList = res.chunkList
          const suffixName = that.accept.split(',').find((t) => t === '.' + res.suffix.toLowerCase())
          if (suffixName) {
            await that.splitUploadFile(values, file, res) //上传模型
          } else {
            that.$message.error('暂不支持此格式')
          }
        }
      },
      onChange(e) {
        const [id] = [...e].reverse() //获取下拉框数组最后一位
        this.form.setFieldsValue({
          id: id
        })
      },
      handleRemove(file) {
        const index = this.fileList.indexOf(file)
        const newFileList = this.fileList.slice()
        newFileList.splice(index, 1)
        this.fileList = newFileList
        this.form.setFieldsValue({
          files: this.fileList.map((x) => x.uid).join(',')
        })
      },
      beforeUpload(file) {
        if (this.uploadType == '0' && this.fileList.length >= 1) {
          this.$message.warning('一次只能上传一个文件！')
          return false
        }
        if (!this.category || !this.software) {
          var suffix = file.name.substring(file.name.lastIndexOf('.'), file.name.length).toLowerCase()
          let suffixStr = ''
          this.categoryList.forEach(item => {
            item.children.forEach(e => {
              if (e.children && e.children.length) {
                e.children.forEach(i => {
                  suffixStr = suffixStr + ',' + i.value
                })
              }
              suffixStr = suffixStr + ',' + e.value
            })
          })
          let options = []
          let label = []
          this.categoryList.forEach(item => {
            item.children.forEach(e => {
              if (e.children && e.children.length) {
                e.children.forEach(i => {
                  if (i.value.includes(suffix)) {
                    options = [item.value, e.value, i.value]
                    label = [item.label, e.label, i.label]
                  }
                })
              } else {
                if (e.value.includes(suffix)) {
                  options = [item.value, e.value]
                  label = [item.label, e.label]
                }
              }
            })
          })
          if (suffix === '.zip') {
            if (!this.accept.includes('.zip')) {
              this.$message.error('此格式有多项匹配,请手动选择')
              this.accept = ''
              this.form.setFieldsValue({
                options: undefined
              })
              return false
            }
          } else {
            this.form.setFieldsValue({
              options: options
            })
            this.accept = options[options.length - 1]
          }
          if (!suffixStr.includes(suffix)) {
            this.$message.error('暂不支持此格式')
            return false
          }
          this.category = options[0]
          this.software = label[1]
          this.showScene = options[0] == 1
        }
        if (file.name.split('.')[0].length <= 50) {
          this.fileList = [...this.fileList, file]
          this.form.setFieldsValue({
            files: this.fileList.map((x) => x.uid).join(',')
          })
        } else {
          this.$message.warning('文件名称不能超过50个字符')
        }
        return false
      },
      getTreeNodeIds(tree, func, path) {
        //获取Id数组
        if (!tree) return []
        for (const data of tree) {
          path.push(data.key)
          if (func(data)) return path
          if (data.children) {
            const findChildren = this.getTreeNodeIds(data.children, func, path)
            if (findChildren.length) return findChildren
          }
          path.pop()
        }
        return []
      },
      onChangeOpenStatus(e) {
        this.formModel.isPublic = e.target.value
      },
    },
  }
</script>

<style lang="less" scoped>
  /deep/.ant-upload-list {
    max-height: 215px;
    overflow-y: auto;

    &::-webkit-scrollbar {
      //整体样式
      height: 5px;
      width: 2px;
    }

    &::-webkit-scrollbar-thumb {
      //滑动滑块条样式
      border-radius: 1px;
      box-shadow: inset 0 0 1px #fafafa;
      background: #d9d9d9;
    }

    &::-webkit-scrollbar-track {
      //轨道的样式
      box-shadow: 0;
      border-radius: 0;
      background: #fafafa;
    }
  }

  /deep/.scroll-box {
    max-height: 60vh;
    overflow-y: auto;

    &::-webkit-scrollbar {
      //整体样式
      height: 5px;
      width: 2px;
    }

    &::-webkit-scrollbar-thumb {
      //滑动滑块条样式
      border-radius: 1px;
      box-shadow: inset 0 0 1px rgba(255, 255, 255, 0.2);
      background: #8c8c8c61;
    }

    &::-webkit-scrollbar-track {
      //轨道的样式
      box-shadow: 0;
      border-radius: 0;
      background: rgba(255, 255, 255, 0.3);
    }

    .ant-tree-node-content-wrapper {
      color: #ffffff;
    }

    .ant-list-empty-text {
      padding: 5px;
      text-align: left;
    }
  }
</style>