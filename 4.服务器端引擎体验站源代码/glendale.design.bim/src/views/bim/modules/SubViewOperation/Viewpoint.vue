<template>
  <div ref="wrap">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateViewpoint" @keyup.prevent @keydown.enter.prevent>创建视点</a-button>
      </a-space>
      <a-list size="small" :data-source="viewpointList" :locale="{ emptyText: `暂无视点数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space @click="ZoomViewpoint(item)">
            <img :src="item.img" alt="">
          </a-space>
          <a-space @click="ZoomViewpoint(item)" class="viewpoint-title">{{ item.viewName }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="默认视点"
              v-if="item.status != 1 && userName != 'guest' && projectMessage.sceneClassification != 2 && projectMessage.functionalPermissions">
              <a-icon type="instagram" @click="DefaultViewpoint(item, index)" />
            </a-tooltip>
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelViewpoint(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="保存视点" :width="320" :visible="visible" :confirm-loading="confirmLoading" @ok="SaveViewpoint"
      @cancel="handleCancel" cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag
      :destroyOnClose="true" :maskClosable="false" :mask="false" :class="{ 'mobile-Model':isMobile  }">
      <a-form :form="form">
        <a-form-item label="视点名称" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <a-input placeholder="请输入视点名称" v-decorator="['title', { rules: [{ required: true, message: '请输入视点名称' }] }]" />
        </a-form-item>
        <a-form-item label="缩略图" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <img alt="example" style="width: 100%" :src="previewImage" />
        </a-form-item>
        <a-form-item label="默认视点" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }"
          v-show="userName != 'guest' && this.projectMessage.sceneClassification != 2 && projectMessage.functionalPermissions">
          <a-switch v-decorator="['viewpoint', { valuePropName: 'false', initialValue: 'false' }]" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>
<script>
  import {
    saveViewpoint,
    getViewpointList,
    deleteViewPoint,
    setViewPointState,
    setCombineViewPoint
  } from '@/api/viewpoint'
  import {
    setModelDefaultPerspective
  } from '@/api/document'
  import store from '@/store'
  import {
    uploadFile,
    getFileByBlobName
  } from '@/api/file'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    name: 'ViewpointList',
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        visible: false,
        confirmLoading: false,
        form: this.$form.createForm(this, {
          name: 'coordinated'
        }),
        viewpointList: [],
        pagination: {
          current: 1,
          pageSize: 1000
        },
        userName: store.state.user.info.userName,
        camera: '',
        previewImage: '',
        imgBlobName: '',
        isMobile: false
      }
    },
    mounted() {
      this.isMobile = _isMobile() ? true : false;
      this.camera = store.state.bim.defaultViewpoint;
      this.fetch()
    },
    methods: {
      fetch() {
        const that = this
        getViewpointList({
          MaxResultCount: that.pagination.pageSize,
          SkipCount: (that.pagination.current - 1) * that.pagination.pageSize,
          modelId: that.projectMessage.modelList[0].id,
          projectId: that.projectMessage.projectId,
        }).then((res) => {
          const pagination = {
            ...that.pagination
          }
          pagination.total = res.totalCount
          res.items.forEach((el) => {
            var url = el.blobName ? getFileByBlobName(el.blobName) : ""
            el.img = url
            delete el.children
          })
          that.viewpointList = res.items
          that.pagination = pagination
        })
      },
      CreateViewpoint() {
        const that = this;
        api.Public.saveScreenShot((res) => {
          const uploadImg = that.convertBase64UrlToBlob(res)
          uploadFile(uploadImg)
            .then((res) => {
              that.imgBlobName = res;
              var url = getFileByBlobName(res)
              that.previewImage = url;
              that.visible = true
            })
            .catch((err) => {
              that.$message.error('视点图片截取失败，请重新创建！')
            })
        })
      },
      convertBase64UrlToBlob(urlData) {
        const fd = new FormData()
        var bytes = window.atob(urlData.split(',')[1]) //去掉url的头，并转换为byte
        //处理异常,将ascii码小于0的转换为大于0
        var ab = new ArrayBuffer(bytes.length)
        var ia = new Uint8Array(ab)
        for (var i = 0; i < bytes.length; i++) {
          ia[i] = bytes.charCodeAt(i)
        }
        const blob = new Blob([ab], {
          type: 'image/png'
        })
        fd.append('file', blob, new Date().getTime() + '.png')
        return fd
      },
      ZoomViewpoint(item) {
        const that = this
        if (item.position != null) {
          api.Camera.setViewPort(JSON.parse(item.position))
        }
      },
      DelViewpoint(item, index) {
        const that = this
        deleteViewPoint(item.id).then((res) => {
          that.$message.success('视点删除成功')
          if (item.position == that.projectMessage.camera || item.position == JSON.stringify(that.projectMessage
              .camera)) {
            that.SetStateViewPoint(item.id, null, 0, 0)
          }
          that.viewpointList.splice(index, 1)
        })
      },
      SaveViewpoint() {
        const that = this
        that.form.validateFields((err, values) => {
          if (!err) {
            api.Camera.getViewPort((data) => {
              saveViewpoint({
                modelId: that.projectMessage.modelList[0].id,
                projectId: that.projectMessage.projectId,
                viewName: values.title,
                viewType: 0,
                position: JSON.stringify(data),
                remark: values.remark,
                sceneType: values.viewpoint == true ? 1 : 0,
                blobName: that.imgBlobName
              }).then((res) => {
                if (values.viewpoint == true) {
                  that.SetStateViewPoint(res.id, JSON.stringify(data), 1, 0)
                } else {
                  that.fetch()
                }
                that.viewpointList.push(res)
                that.confirmLoading = false
                that.$message.success('视点创建成功')
                that.form.resetFields() //清空表单
                that.visible = false
              })
            })
          }
        })
      },
      SetStateViewPoint(id, data, status, outStatus) {
        const that = this
        that.projectMessage.camera = JSON.parse(data)
        if (that.projectMessage.sceneType == 0) { //单模型
          // setSceneInformation(that.projectMessage.id, {
          //   "projectId": that.projectMessage.projectId,
          //   "projectFolderId": that.projectMessage.modelList[0].projectFolderId,
          //   "documentName": that.projectMessage.modelList[0].documentName,
          //   "documentType": that.projectMessage.documentType,
          //   "modelName": that.projectMessage.modelList[0].modelName,
          //   "modelFormat": that.projectMessage.modelList[0].modelFormat,
          //   "camera": data,
          //   "size": that.projectMessage.modelList[0].size,
          //   "sceneConfig": that.projectMessage.sceneConfig ? JSON.stringify(that.projectMessage.sceneConfig) : undefined,
          //   "matrix": that.projectMessage.modelList[0].matrix ? JSON.stringify(that.projectMessage.modelList[0].matrix) : undefined,
          // })
          setModelDefaultPerspective(that.projectMessage.id, data)
        } else { //合模
          let combineDetails = JSON.parse(JSON.stringify(that.projectMessage.modelList))
          combineDetails.forEach((item) => {
            item.matrix = JSON.stringify(item.matrix)
          })
          // setCombineViewPoint(that.projectMessage.id, {
          //   "folderId": that.projectMessage.folderId,
          //   "combineName": that.projectMessage.combineName,
          //   "projectId": that.projectMessage.projectId,
          //   "projectFolderId": that.projectMessage.modelList[0].projectFolderId,
          //   "documentName": that.projectMessage.modelList[0].documentName,
          //   "modelName": that.projectMessage.modelList[0].modelName,
          //   "modelFormat": that.projectMessage.modelList[0].modelFormat,
          //   "camera": data,
          //   "size": that.projectMessage.modelList[0].size,
          //   "sceneConfig": that.projectMessage.sceneConfig ? JSON.stringify(that.projectMessage.sceneConfig) : undefined,
          //   "combineDetails": combineDetails,
          // })
          setCombineViewPoint(that.projectMessage.id, data)
        }
        setViewPointState(id, {
          id: id,
          Status: status,
          OutStatus: outStatus
        }).then(res => {
          that.fetch()
        })
      },
      DefaultViewpoint(item, index) {
        const that = this;
        that.$confirm({
          cancelText: '取消',
          okText: '确定',
          title: `确定要设置视点“${item.viewName}” 为默认视点吗？`,
          onOk() {
            that.SetStateViewPoint(item.id, item.position, 1, 0)
            that.$message.success('设置成功！')
          },
        })
      },
      handleCancel() {
        this.visible = false;
      },
    },
  }
</script>
<style lang="less" scoped>
  .viewpoint-title {
    width: 45%;
    cursor: pointer;
  }

  .viewpoint-list {
    .ant-list-item {
      img {
        max-width: 50px;
        min-width: 50px;
        cursor: pointer;
      }
    }
  }


</style>