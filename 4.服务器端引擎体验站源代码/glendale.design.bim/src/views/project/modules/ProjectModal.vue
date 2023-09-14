<template>
  <a-modal title="项目维护" :width="700" :visible="visible" :confirmLoading="confirmLoading" @ok="handleOk"
    @cancel="handleCancel" :maskClosable="false" :centered="true">
    <template slot="footer">
      <div v-if='tabKey == 2' style='"color": red;"height": 32px'>
        模型目录的修改将直接保存
      </div>
    </template>
    <a-form :form="form" :labelCol="{ span: 4 }" :wrapperCol="{ span: 17 }">
      <a-tabs v-model="tabKey" @change="onChangeTab" size="small">
        <a-tab-pane :key="1" :tab="'基本信息'" style="height: 420px; overflow: auto;">
          <a-form-item label="项目名称">
            <a-input placeholder="项目名称"
              v-decorator="['projectName', { rules: [{ required: true, message: '项目名称不能为空' }] }]" :maxLength="50" />
          </a-form-item>
          <a-form-item label="简介">
            <a-textarea placeholder="简介" v-decorator="['remark']" :maxLength="200"
              :auto-size="{ minRows: 2, maxRows: 4 }" />
          </a-form-item>
          <a-form-item label="封面图片" style="margin-bottom: 0">
            <a-upload accept=".jpg,.jpeg,.png,.gif" list-type="picture-card" :file-list="projectImages"
              :customRequest="customRequest" @change="handleChange">
              <div v-if="projectImages.length < 4">
                <a-icon type="plus" />
                <div class="ant-upload-text">上传图片</div>
              </div>
            </a-upload>
          </a-form-item>
          <a-form-item label="创建人"
            v-if="$store.getters.userInfo.userName == (this.manageInfo ? this.manageInfo.userName : '')">
            <div class='ant-input'>
              {{ this.manageInfo ? this.manageInfo.name : '' }}
            </div>
          </a-form-item>
          <a-form-item label="立项日期">
            <a-date-picker v-decorator="['beginDate']" valueFormat="YYYY-MM-DD HH:mm:ss" disabled />
          </a-form-item>
        </a-tab-pane>
        <a-tab-pane :key="2" :tab="'模型目录'" style="height: 420px">
          <FolderEdit :projectId='projectId' v-if='tabKey == 2' @fetch='fetch' />
        </a-tab-pane>
      </a-tabs>
    </a-form>
  </a-modal>
</template>

<script>
import pick from 'lodash.pick'
import { mapGetters } from 'vuex'
import FolderEdit from '@/views/project/FolderEdit'
import { uploadFile, getFileByBlobName } from '@/api/file'
import { getPositionList } from '@/api/dictionary'
import { saveProject, getProject, getFolderTrees } from '@/api/project'
import { delDoc } from '@/api/document'

export default {
  name: 'ProjectModal',
  components: {
    FolderEdit,
  },
  props: {
    organizationUnitId: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      manageInfo: [],
      visible: false,
      confirmLoading: false,
      tabKey: 1,
      positions: [],
      project: { projectImages: [] },
      projectImages: [],
      form: this.$form.createForm(this),
      dataFolder: [],
      // organizationUnitList: [],
      projectId: '',
      queryParams: {},
      pagination: {
        current: 1,
        total: 0,
        skipCount: 0,
        pageSize: 10, //每页中显示10条数据
      },
    }
  },
  async created() {
    this.$store.dispatch('getOrganizationUnitTrees')
    const that = this
    // getPositionList().then((res) => {
    //   that.positions = res
    // })
  },
  computed: {
    ...mapGetters(['organizationUnits']),
  },
  methods: {
    getFileByBlobName,
    add() {
      this.edit({
        id: undefined,
        projectName: undefined,
        remark: undefined,
        isPublic: false,
        projectImages: [],
        beginDate: undefined,
      })
    },
    getTreeNodeIds(tree, func, path) {
      //获取Id数组
      if (!tree) return []
      for (const data of tree) {
        path.push(data.id)
        if (func(data)) return path
        if (data.children) {
          const findChildren = this.getTreeNodeIds(data.children, func, path)
          if (findChildren.length) return findChildren
        }
        path.pop()
      }
      return []
    },
    // formatData(items) {
    //   const that = this;
    //   items.forEach((item) => {
    //     if (item.children && item.children.length > 0) {
    //       that.formatData(item.children)
    //     } else {
    //       delete item.children
    //     }
    //   })
    //   return items
    // },
    async edit(record) {
      const parentIds = []
      this.pagination.current = 1
      this.manageInfo = record.manageInfo
      // this.organizationUnitList = this.formatData(this.organizationUnits)
      const that = this
      that.tabKey = 1
      if (record.id) {
        that.projectId = record.id
        const project = await getProject(record.id)
        that.projectImages = project.projectImages.map((x) => ({
          uid: x.blobName,
          name: 'image',
          status: 'done',
          url: that.getFileByBlobName(x.blobName),
        }))
        that.project = { ...project }

        that.getFolderTree()
      } else {
        that.project = { ...record }
      }
      that.visible = true
      that.$nextTick(() => {
        that.form.setFieldsValue(
          pick(that.project, 'projectName', 'remark', 'isPublic', 'beginDate')
        )
        that.fetch()
      })
    },
    fetch(params = {}) {
      const that = this
      this.pagination = Object.assign(this.pagination, params)
      this.queryParams.MaxResultCount = this.pagination.pageSize
      this.queryParams.projectId = that.projectId
      this.queryParams.SkipCount = (this.pagination.current - 1) * this.pagination.pageSize
    },
    getFolderTree() {
      const that = this
      getFolderTrees(that.projectId).then((res) => {
        that.dataFolder = that.filterFolderData(res)
      })
    },
    filterFolderData(items) {
      items.forEach((item) => {
        if (item.children.length === 0) {
          delete item.children
        } else {
          this.filterFolderData(item.children)
        }
      })
      return items
    },
    delFolder(data) {
      const that = this
      that.$confirm({
        title: `确定要删除 “${data.title}” 吗？`,
        onOk() {
          delDoc(data.key).then(() => {
            that.$message.success('删除成功')
            that.$store.dispatch('GetDocTree', that.projectId) //更新状态
            that.getTree()
          })
        },
      })
    },
    okFolder() {
      this.getFolderTree()
      // 新增/修改 成功时，重载列表
    },
    close() {
      this.$emit('close')
      this.visible = false
    },
    handleTableFolderChange(pagination) {
      this.pagination = pagination
      this.pagination.skipCount = (pagination.current - 1) * pagination.pageSize
      this.getFolderTree()
    },
    handleOk() {
      const that = this
      // 触发表单验证
      that.form.validateFieldsAndScroll((err, values) => {
        // 验证表单没错误
        if (!err) {
          //更新图片
          if (that.projectImages.length > 0) {
            var imgs = that.projectImages.map((x) => ({
              blobName: x.uid,
              isConver: false,
            }))
            that.project.projectImages = imgs
          } else {
            that.project.projectImages = []
          }
          values.manageId = this.manageInfo.id
          that.submitForm(values)
        }
      })
    },
    submitForm(values) {
      const that = this
      that.confirmLoading = true
      const formObj = { ...that.project, ...values }
      saveProject(formObj)
        .then((res) => {
          that.$message.success('保存成功!')
          that.$emit('ok', 0)
          that.close()
        })
        .catch(() => { })
        .finally(() => {
          that.confirmLoading = false
        })
    },
    async customRequest({ file }) {
      const formData = new FormData()
      formData.append('file', file, file.name)
      const blobName = await uploadFile(formData)
      var url = getFileByBlobName(blobName)
      this.projectImages.splice(this.projectImages.length - 1, 1)
      this.projectImages.push({ uid: blobName, name: 'image', status: 'done', url: url })
    },
    handleChange({ fileList }) {
      this.projectImages = fileList
    },
    handleCancel() {
      this.$emit('ok', 0)
      this.close()
    },
    onChangeTab(key) {
      this.tabKey = key
    },
  },
}
</script>

<style lang="less" scoped>
/deep/.ant-upload-select-picture-card i {
  font-size: 32px;
  color: #999;
}

/deep/.ant-upload-select-picture-card .ant-upload-text {
  margin-top: 8px;
  color: #666;
}
</style>
