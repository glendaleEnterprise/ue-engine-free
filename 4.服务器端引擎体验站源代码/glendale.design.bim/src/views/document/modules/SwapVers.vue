<template>
  <a-modal :title="RowSelection ? '版本对比' : modalTitle" @cancel="close" :footer="null" :width="modalWidth"
    :visible="visible" :maskClosable="false" class="model-custom-style">
    <a-card size="small" :bordered="false">
      <a-space slot="title">
        <a-input placeholder="请输入模型轻量化名称" style="width: 300px" v-model="queryParams.keyword" />
        <a-button type="primary" @click="search">查询</a-button>
      </a-space>
      <!-- <a-space slot="title" v-show="hasVersion">
        <a-button type="primary" @click="versionComparison"  v-action:Design.Business.Documents.VersionThan>版本对比</a-button>
      </a-space> -->
      <a-table size="middle" bordered :columns="columns" :data-source="data" :pagination="pagination"
        @change="handleTableChange" :rowKey="(record) => record.id" :loading="loading" class="table-header-style"
        :row-selection="RowSelection
            ? {
              selectedRowKeys: selectedVersionKeys,
              onChange: onSelectChange,
              columnTitle: '选择',
              hideDefaultSelections: true,
            }
            : null
          ">
        <template slot="no" slot-scope="text, record, index"> {{ pagination.skipCount + index + 1 }} </template>
        <template slot="versionNo" slot-scope="text"> V{{ text | floatToVersion }} </template>
        <span slot="docSize" slot-scope="text">{{ text | byteToMB }}</span>
        <span slot="render_status" slot-scope="text">
          <span>{{ text | filterDocStatus }}</span>
        </span>
        <template slot="render_isCurrent" slot-scope="text">
          <span>{{ text | filterIsCurrent }}</span>
        </template>
        <template slot="action" slot-scope="text, record">
          <a-tooltip title="设置当前版本"
            :disabled="record.isCurrent || (record.docType == 2 && record.docStatus != 3 && record.docStatus != 4)">
            <a-popconfirm @confirm="setCurrent(record)" title="确定设置为当前版本?" :ok-text="$t('public.OK')"
              :cancel-text="$t('public.cancel')" :disabled="record.isCurrent">
              <a-button icon="pushpin" type="link" :disabled="record.isCurrent" />
            </a-popconfirm>
          </a-tooltip>
          <a-divider type="vertical" />
          <a-tooltip title="删除" v-action:Design.Business.Documents.VersionThan>
            <a-popconfirm @confirm="deleteSpace(record)" title="删除该文件?" :ok-text="$t('public.OK')"
              :cancel-text="$t('public.cancel')" :disabled="record.isCurrent">
              <a-button icon="delete" type="link" :disabled="record.isCurrent" />
            </a-popconfirm>
          </a-tooltip>
        </template>
      </a-table>
      <div v-show="RowSelection" class="create-btn">
        <a-button type="primary" @click="addVersionComparison">{{ $t('public.OK') }}</a-button>
        <a-button type="primary" @click="cancleVersionComparison">{{ $t('public.cancel') }}</a-button>
      </div>
      <edit-share :editShare="editShare" v-cloak v-if="editShareVisible" :visible.sync="editShareVisible"
        @changeEngine="edit" />
    </a-card>
  </a-modal>
</template>
<script>
  var columns = []
  const columnsModel = [{
      title: '序号',
      width: '50px',
      align: 'center',
      scopedSlots: {
        customRender: 'no'
      },
    },
    {
      title: '模型轻量化名称',
      dataIndex: 'modelName',
      align: 'center',
      width: '160px',
    },
    {
      title: '版本号',
      dataIndex: 'versionNo',
      align: 'center',
      width: '150px',
      scopedSlots: {
        customRender: 'versionNo'
      },
    },
    {
      title: '文件大小(MB)',
      dataIndex: 'size',
      align: 'center',
      width: '120px',
      scopedSlots: {
        customRender: 'docSize'
      },
    },
    // {
    //   title: '类型',
    //   dataIndex: 'modelFormat',
    //   align: 'center',
    //   width: '80px',
    // },
    {
      title: '轻量化状态',
      align: 'center',
      dataIndex: 'status',
      scopedSlots: {
        customRender: 'render_status'
      },
      width: '120px',
    },
    {
      title: '创建时间',
      align: 'center',
      dataIndex: 'creationTime',
      key: 'creationTime',
      width: '150px',
    },
    {
      title: '设置当前版本',
      align: 'center',
      dataIndex: 'isCurrent',
      width: '120px',
      scopedSlots: {
        customRender: 'render_isCurrent'
      },
    },
    {
      title: '操作',
      align: 'center',
      key: 'action',
      width: '120px',
      scopedSlots: {
        customRender: 'action'
      },
    },
  ]
  import {
    mapGetters
  } from 'vuex'
  import {
    getDocVerList,
    SetDocVerCurrent,
    DocVerDeleted,
    uploadDocumentFile
  } from '@/api/docVer'
  import {
    setModelVersion,
    getModelVersionHave
  } from '@/api/model'
  export default {
    name: 'SwapVer',
    computed: {
      ...mapGetters(['currProject']),
    },
    data() {
      return {
        form: {
          docId: undefined,
          docName: undefined
        },
        visible: false,
        modalTitle: '',
        columns: [],
        ShareId: undefined,
        loading: false,
        valueParams: undefined,
        editShareVisible: false,
        level: 1,
        queryParams: {
          DocId: undefined,
        },
        data: [],
        pagination: {
          current: 0,
          total: 0,
          skipCount: 0,
          pageSize: 5, //每页中显示10条数据
          showTotal: (total) => `共 ${total} 条`, //分页中显示总的数据
        },
        editShare: {},
        fileList: [],
        modalWidth: '1400',
        RowSelection: false, //版本对比
        selectedVersionKeys: [],
        selectedVersionItem: [],
        hasVersion: false, //是否存从模型列表而来
      }
    },
    filters: {
      //轻量化状态
      filterDocStatus(val) {
        switch (val) {
          case 0:
            return '待轻量化'
          case 1:
            return '轻量化中'
          case 2:
            return '轻量化成功'
          case 3:
            return '轻量化失败'
          default:
            return '未知状态'
        }
      },
      filterIsCurrent(val) {
        if (val) return '是'
        else return '否'
      },
    },
    mounted() {
      window.onresize = () => {
        this.modalWidth = window.innerWidth * 0.7
      }
      this.modalWidth = window.innerWidth * 0.7
    },
    methods: {
      // async uploadFiles(file) {
      //   const formData = new FormData()
      //   formData.append('file', file, file.name)
      //   await uploadDocumentFile(this.form.docId, formData)
      //   this.search()
      // },
      async search() {
        this.pagination.current = 1
        this.resetCurrenPage()
        this.getDocVer()
      },
      // handleRemove(file) {
      //   const index = this.fileList.indexOf(file)
      //   const newFileList = this.fileList.slice()
      //   newFileList.splice(index, 1)
      //   this.fileList = newFileList
      // },
      // beforeUpload(file) {
      //   if (file.name.split('.')[0].length <= 50) {
      //     this.fileList = [...this.fileList, file]
      //     this.form.setFieldsValue({ files: this.fileList.map((x) => x.uid).join(',') })
      //   } else {
      //     this.$message.warning('文件名称不能超过50个字符')
      //   }
      //   return false
      // },
      async getDocVer() {
        this.loading = true
        const par = {
          DocumentId: this.form.docId,
          keyword: this.queryParams.keyword,
          skipCount: (this.pagination.current - 1) * this.pagination.pageSize,
          maxResultCount: this.pagination.pageSize,
        }
        const res = await getDocVerList(par)
        this.data = res.items
        this.pagination.total = res.totalCount
        this.loading = false
      },
      showModal(id, name, isModel) {
        const that = this
        that.RowSelection = false
        that.selectedVersionKeys = []
        that.selectedVersionItem = []
        that.form.docId = id
        that.form.docName = name
        if (isModel) {
          that.columns = columnsModel
          that.hasVersion = true
        } else {
          that.hasVersion = false
        }
        that.search()
        that.visible = true
        that.modalTitle = '切换版本-' + name
      },
      close() {
        this.queryParams.keyword = '' //清空
        this.visible = false
      },
      handleTableChange(pagination) {
        this.pagination = pagination
        this.pagination.skipCount = (pagination.current - 1) * pagination.pageSize
        this.getDocVer()
      },
      setCurrent(data, opt) {
        const that = this
        SetDocVerCurrent(data.id).then(() => {
          that.$message.success('设置成功！')
          that.getDocVer()
          that.$emit('fetch')
        })
      },
      //重新计算当前页码（如果删除第二页最后一条，则刷新到上一页）
      resetCurrenPage() {
        const totalPage = Math.ceil((this.pagination.total - 1) / this.pagination.pageSize) // 总页数
        this.pagination.current = this.pagination.current > totalPage ? totalPage : this.pagination.current
        this.pagination.current = this.pagination.current < 1 ? 1 : this.pagination.current
      },
      // pvwData(data) {
      //   this.$message.success('暂无开发！')
      // },
      deleteSpace(data) {
        const that = this
        DocVerDeleted(data.id).then(() => {
          that.$message.success(that.$t('list.deleteSuccess'))
          that.resetCurrenPage()
          that.getDocVer()
        })
      },
      versionComparison() {
        this.RowSelection = true
      },
      onSelectChange(selectedRowKeys, selectedRows) {
        if (selectedRowKeys.length > 2) {
          this.$message.warning('请选择两个版本进行对比！')
          return false
        }
        this.selectedVersionKeys = selectedRowKeys
        this.selectedVersionItem = selectedRows
      },
      addVersionComparison() {
        if (this.selectedVersionKeys.length != 2) {
          this.$message.warning('请选择两个版本进行对比！')
          return false
        }
        getModelVersionHave({
          sourceId: this.selectedVersionItem[1].id,
          destinationId: this.selectedVersionItem[0].id,
        }).then((res) => {
          if (res) {
            this.$emit('openVersion', res.id)
            this.close()
          } else {
            setModelVersion({
              title: '',
              sourceDocVerId: this.selectedVersionItem[1].id,
              destinationDocVerId: this.selectedVersionItem[0].id,
              remark: '',
            }).then((res) => {
              this.$emit('openVersion', res.id)
              this.close()
            })
          }
        })
      },
      cancleVersionComparison() {
        this.RowSelection = false
        this.selectedVersionKeys = []
        this.selectedVersionItem = []
      },
    },
  }
</script>

<style lang="less" scoped>
  .model-custom-style {
    /deep/.ant-modal-body {
      padding: 10px 24px 24px;
    }

    .table-header-style {
      tr {
        th {
          text-align: center;
        }
      }
    }

    /deep/.ant-card-head-title {
      display: flex;
      justify-content: space-between;
    }

    .create-btn {
      text-align: right;

      button {
        margin: 5px;
      }
    }
  }
</style>