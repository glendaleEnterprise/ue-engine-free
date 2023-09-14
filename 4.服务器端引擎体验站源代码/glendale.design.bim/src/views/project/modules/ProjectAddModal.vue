<template>
  <a-modal title="创建项目" :width="700" :visible="visible" :confirmLoading="confirmLoading" @ok="handleOk"
    @cancel="handleCancel" :maskClosable="false">
    <a-form :form="form" :labelCol="{ span: 4 }" :wrapperCol="{ span: 17 }">
      <a-tabs v-model="tabKey" @change="onChangeTab" size="small">
        <a-tab-pane :key="1" :tab="'基本信息'">
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
        </a-tab-pane>
        <a-tab-pane :key="2" :tab="'模型目录'">
          <a-button @click="$refs.editFolder.add()" class='new-add'>新增</a-button>
          <a-table size="middle" class="documentTable" bordered :columns="columnsFolder" :data-source="dataFolder"
            :pagination="pagination" @change="handleTableFolderChange" :rowKey="(record) => record.id"
            :expandIconColumnIndex="1" :indentSize="20">
            <template slot="no" slot-scope="text, record, index">{{ pagination.skipCount + index + 1 }}</template>
            <template slot="action" slot-scope="text, record">
              <a-tooltip title="新增">
                <a-button @click="$refs.editFolder.add(record.id)" icon="plus-circle" type="link"></a-button>
              </a-tooltip>
              <a-divider type="vertical" />
              <a-tooltip title="编辑">
                <a-button @click="$refs.editFolder.edit(record)" icon="form" type="link"></a-button>
              </a-tooltip>
              <a-divider type="vertical" />
              <a-tooltip title="删除">
                <a-button @click="delFolder(record)" icon="delete" type="link"></a-button>
              </a-tooltip>
            </template>
          </a-table>
          <edit-folder :getTree="getFolderTree" ref="editFolder" :dataFolder='dataFolder' @ok="okFolder" :roles='roles' />
        </a-tab-pane>
      </a-tabs>
    </a-form>
  </a-modal>
</template>
<script>
import { dateFormat } from '@/api/public'
import pick from 'lodash.pick'
import EditFolder from './ProjectFolder'
import { uploadFile, getFileByBlobName } from '@/api/file'
import { saveProject, getProject, getFolderTrees, addFolderArr } from '@/api/project'
import { getSystemRoleList } from '@/api/system'
const columnsFolder = [
  {
    title: '序号',
    width: '70px',
    align: 'center',
    scopedSlots: { customRender: 'no' },
  },
  {
    title: '专业名称',
    dataIndex: 'folderName',
    key: 'folderName',
    width: '30%',
  },
  // {
  //   title: '负责人',
  //   dataIndex: 'projectFolderUsers',
  //   width: '250px',
  //   align: 'center',
  //   scopedSlots: { customRender: 'projectFolderUsers' },
  // },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    ellipsis: true,
  },
  {
    title: '操作',
    key: 'action',
    width: '150px',
    align: 'center',
    scopedSlots: { customRender: 'action' },
  },
]
export default {
  name: 'ProjectModal',
  components: {
    EditFolder,
  },
  props: {
    organizationUnitId: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      visible: false,
      columnsFolder,
      confirmLoading: false,
      tabKey: 1,
      project: { projectImages: [], projectUsers: [] },
      projectUsers: [],
      projectImages: [],
      form: this.$form.createForm(this),
      dataFolder: [],
      roles: [],
      projectId: '',
      pagination: {
        current: 0,
        total: 0,
        skipCount: 0,
        pageSize: 10, //每页中显示10条数据
      },
    }
  },
  async created() {
    this.$store.dispatch('getOrganizationUnitTrees')
    const that = this
    getSystemRoleList({ RoleType: '项目级' }).then((items) => {
      if (items.length > 0) {
        items.forEach((el) => {
          that.roles.push({ label: el.name, value: el.name })
        })
      }
    })
  },
  methods: {
    getFileByBlobName,
    add() {
      this.dataFolder = []
      this.edit({
        id: undefined,
        projectName: undefined,
        remark: undefined,
        isPublic: false,
        projectUsers: [],
        projectImages: [],
      })
    },
    async edit(record) {
      const that = this
      that.tabKey = 1
      if (record.id) {
        that.projectId = record.id;
        const project = await getProject(record.id)
        that.projectUsers = project.projectUsers.map((x) => ({
          id: x.userId,
          name: x.name,
          orgNames: x.orgNames,
          phoneNumber: x.phoneNumber,
          position: x.position,
          userName: x.userName,
          isManager: x.isManager
        }))
        that.projectImages = project.projectImages.map((x) => ({
          uid: x.blobName,
          name: 'image',
          status: 'done',
          url: that.getFileByBlobName(x.blobName),
        }))
        that.project = { ...project }

        that.getFolderTree()
      } else {
        //当前用户作为默认项目成员，且设置成项目经理
        var curUser = that.$store.getters.userInfo
        that.projectUsers = [
          {
            id: curUser.extraProperties.id,
            name: curUser.name,
            phoneNumber: curUser.phoneNumber,
            position: curUser.extraProperties.position,
            userName: curUser.userName,
            isManager: true
          },
        ]
        that.project = { ...record }
      }
      that.projectImages = []
      that.visible = true
      that.$nextTick(() => {
        that.form.setFieldsValue(
          pick(that.project, 'projectName', 'remark', 'isPublic')
        )
      })
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
        title: `确定要删除 “${data.folderName}” 吗？`,
        onOk() {
          that.dataFolder = that.filterFolderDel(that.dataFolder, data.id)
          that.$forceUpdate()
        },
      })
    },
    filterFolderDel(items, id) {
      items.forEach((item, index) => {
        if (item.id === id) {
          items.splice(index, 1)
        } else {
          if (item.children && item.children.length) {
            this.filterFolderDel(item.children, id)
          }
        }
      })
      return items
    },
    filterFolderEdit(items, data) {
      items.forEach((item, index) => {
        if (item.id === data.id) {
          items[index] = data
        } else {
          if (item.children && item.children.length) {
            this.filterFolderDel(item.children, data)
          }
        }
      })
      return items
    },
    filterParentId(items, data) {
      items.forEach((item) => {
        if (item.id === data.parentId) {
          if (!item.children || item.children.length === 0) {
            item.children = [data]
          } else {
            item.children.push(data)
          }
        } else {
          if (item.children && item.children.length) {
            this.filterParentId(item.children, data)
          }
        }

      })
      return items
    },
    okFolder(data, isAdd) {
      if (isAdd) {
        if (data.parentId) {
          this.dataFolder = this.filterParentId(this.dataFolder, data)
          this.$forceUpdate()
        } else {
          this.dataFolder.push(data)
        }
      } else {
        this.dataFolder = this.filterFolderEdit(this.dataFolder, data)
        this.$forceUpdate()
      }
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
          if (this.dataFolder.length == 0) {
            that.$confirm({
              title: `您还没有创建模型目录，确定要提交吗？`,
              onOk() {
                that.submitForm(values)
              },
              onCancel() {
                that.tabKey = 2
              },
            })
          } else {
            that.submitForm(values)
          }
        }
      })
    },
    submitForm(values) {
      const that = this
      that.confirmLoading = true
      const formObj = { ...that.project, ...values }
      formObj.beginDate = dateFormat(new Date())
      saveProject(formObj)
        .then((res) => {
          if (this.dataFolder.length) {
            addFolderArr(this.dataFolder, res.id).then(res => {
              that.$message.success('保存成功!')
              that.$emit('ok', 0)
              that.close()
            })
          } else {
            that.$message.success('保存成功!')
            that.$emit('ok', 0)
            that.close()
          }
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

.new-add {
  float: right;
  margin-bottom: 15px;
  z-index: 999;
}
</style>
