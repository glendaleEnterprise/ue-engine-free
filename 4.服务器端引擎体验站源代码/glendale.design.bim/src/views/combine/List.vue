<template>
  <a-row :gutter="24">
    <a-col :md="24" :lg="3">
      <folder-tree @ok="onOk" />
    </a-col>
    <a-col :md="24" :lg="21" style="padding-top: 35px;">
      <a-card size="small" :bordered="false" class="card-box">
        <a-space slot="title" v-action:Design.Business.Combine.Search>
          <a-input placeholder="关键字" v-model="queryParams.CombineName" style="width: 300px" />
          <a-button type="primary" @click="search">查询</a-button>
        </a-space>
        <a-space slot="extra">
          <a-button type="primary" @click="newMoldClosing" ghost v-action:Design.Business.Combine.Add>新建合模 </a-button>
          <a-button type="primary" @click="$refs.filePublic.open(selectedRowIds, 'clamping')" ghost
            v-action:Design.Business.Combine.Public>
            公开/私有
          </a-button>
          <a-button type="primary" @click="() => { isTable = !isTable }" class="cut-box">
            <a-icon v-if="isTable" type="menu" style="font-size: 20px" />
            <a-icon v-else type="appstore" style="font-size: 20px" />
          </a-button>
        </a-space>
        <a-table v-show="isTable" ref="table" class="documentTable" :columns="columns" :row-key="(record) => record.id"
          :data-source="sourceData" :row-selection="rowSelection" bordered :pagination="false" size="middle"
          @change="handleTableChange">
          <template slot="state" slot-scope="text, record">
            <div style="display: flex">
              <div style="width: 24px">
                <a-tooltip title="公开" v-if="record.isOpen" style="margin: 0 5px">
                  <a-icon type="eye" style="color: #21ad8d" />
                </a-tooltip>
              </div>
              <div style="width: 24px">
                <a-tooltip title="链接" v-show="record.hasLinkFile > 0" style="margin: 0 5px">
                  <a-icon type="link" style="color: #21ad8d" />
                </a-tooltip>
              </div>
            </div>
          </template>
          <template slot="serial" slot-scope="text, record, index">{{ index + 1 }}</template>
          <template slot="combineName" slot-scope="text, record">
            <a @click="previewFile(record)">{{ text }}</a>
          </template>
          <template slot="suffix" slot-scope="text, record">{{ record | totalType }}</template>
          <template slot="combineDetails" slot-scope="text">{{ text.combineDetails.length }}</template>
          <template slot="docSize" slot-scope="text, record">{{ record | aggregatedSize }}</template>
          <template slot="creationTime" slot-scope="text">{{ text | dateFomart('YYYY-MM-DD') }}</template>
          <template slot="lastModificationTime" slot-scope="text">{{ text | dateFomart('YYYY-MM-DD') }}</template>
          <template slot="action" slot-scope="text, record">
            <a-space>
              <a-tooltip title="分享" v-action:Design.Business.Combine.Share
                v-if='currentUser.userName != "guest" && currentUser.extraProperties.id == record.creatorId'>
                <a-button size="small" icon="share-alt" type="link" @click="shareNodeItem(record)" />
              </a-tooltip>
              <a-tooltip title="删除" v-action:Design.Business.Combine.Delete>
                <a-button size="small" icon="delete" type="link" @click="delInfo(record)" />
              </a-tooltip>
              <!-- <a-tooltip title="记录">
                <a-button size="small" icon="unordered-list" type="link" @click="showDrawer(record)" />
              </a-tooltip> -->
            </a-space>
          </template>
        </a-table>
        <div v-show="!isTable">
          <a-card :bordered="false" v-if="sourceData.length > 0">
            <a-card-grid style="width: 20%" v-for="(record, index) in sourceData" :key="index">
              <a-card :bodyStyle="{ padding: '10px 5px' }">
                <img slot="cover" alt="example" :src="record.img" @click="previewFile(record)"
                  style="cursor: pointer;" />
                <template slot="actions">
                  <a-tooltip title="分享" v-action:Design.Business.Combine.Share
                    v-if='currentUser.userName != "guest" && currentUser.extraProperties.id == record.creatorId'>
                    <a-button size="small" @click="shareNodeItem(record)" type="link" icon="share-alt" />
                  </a-tooltip>
                  <a-tooltip title="删除" v-action:Design.Business.Combine.Delete>
                    <a-button size="small" icon="delete" type="link" @click="delInfo(record)" />
                  </a-tooltip>
                </template>
                <a-card-meta>
                  <template slot="title">
                    <a @click="previewFile(record)">{{ record.combineName }}</a>
                  </template>
                  <template slot="description">
                    <div style="display: flex; justify-content: space-between;">
                      <div>
                        <a-icon type="user" title="合模者" /> {{ record.creationName }}</div>
                      <div>
                        <a-icon type="clock-circle" title="上传日期" /> {{ record.creationTime | dateFomart('YYYY-MM-DD')
                      }}
                      </div>
                    </div>
                  </template>
                </a-card-meta>
              </a-card>
            </a-card-grid>
          </a-card>
          <a-card v-else>
            <a-empty :image="simpleImage" />
          </a-card>
        </div>
        <div style="padding-top: 10px; text-align: right">
          <a-pagination size="small" :pageSize="pagination.pageSize" :show-total="pagination.showTotal"
            :total="pagination.total" @change="changePageSize" v-model="pagination.current" />
        </div>
      </a-card>
      <a-drawer maskClosable placement="right" @close="visDrawer = false" :title="DrawerLogTitle" :closable="false"
        width="320" :visible="visDrawer">
        <template>
          <a-timeline>
            <a-empty v-if="this.logList.length <= 0" />
            <a-timeline-item v-for="(item, i) in logList" :key="i">
              <a-icon slot="dot" type="clock-circle-o" style="font-size: 16px" />
              <p>
                {{ item.creationTime }}&nbsp;&nbsp;<span>[{{ item.creationName || 'System' }}]</span>
              </p>
              <p>{{ item.remark }}</p>
            </a-timeline-item>
          </a-timeline>
        </template>
      </a-drawer>
      <!-- 公开文件 -->
      <file-public title="模型公开/私有" ref="filePublic" @fetch="fetch" />
      <shared-model :sharedMessage="sharedMessage" :sharedFatherState.sync="sharedFatherState"
        :projectMessage="projectMessage" />
      <a-modal :visible="openData.visible" :footer="null" :mask="false" :maskClosable="false" :destroyOnClose="true"
        @cancel="openData.visible = false" :width="openData.width" :height="openData.height" :forceRender="true"
        :dialog-style="{ top: '0', left: '0', height: 'calc(100% - 80px)' }" :body-style="{ padding: '0px 0px 0 0px' }"
        :mask-style="{ left: '220px' }">
        <template slot="closeIcon">
          <a-icon type="close-circle" style="font-size: 18px" theme="filled" />
        </template>
        <view-bim v-if="openData.isSet == true" :doc="openData.Document" :openType="'clamping'" :openInGis="openInGis"
          modelType="combineModel" />
      </a-modal>
      <a-modal title="新建合模" :width="1200" :maskClosable="false" :visible="newMoldClosingVisible" @ok="selectClamping"
        @cancel="newMoldClosingHide" ok-text="确定" cancel-text="取消">
        <model-list @doSelect="doSelect" v-if="newMoldClosingVisible" :clampingList="clampingList" />
      </a-modal>
    </a-col>
  </a-row>
</template>

<script>
  import {
    Empty
  } from 'ant-design-vue'
  import {
    mapGetters
  } from 'vuex'
  import FolderTree from '../document/modules/FolderTree.vue'
  import FilePublic from './modules/FilePublic.vue'
  import SharedModel from '../bim/modules/SharedModel.vue'
  import ViewBIM from '../bim/Index.vue'
  import {
    getCombineList,
    deleted,
    getHasClampById,
    getCombineLogs
  } from '@/api/combine'
  import ClampPage from './Index'
  import ModelList from './modules/ModelList.vue'
  import storage from 'store'
  import {
    getFileByBlobName
  } from '@/api/file'
  export default {
    name: 'Model',
    components: {
      FolderTree,
      FilePublic,
      SharedModel,
      'view-bim': ViewBIM,
      ClampPage,
      ModelList,
    },
    beforeCreate() {
      this.simpleImage = Empty.PRESENTED_IMAGE_SIMPLE
    },
    computed: {
      ...mapGetters(['currProject']),
      rowSelection() {
        const that = this
        return {
          onChange: (selectedRowKeys, selectedRows) => {
            that.selectedRowIds = selectedRowKeys
          },
        }
      },
      ...mapGetters({
        currentUser: 'userInfo',
      }),
    },
    watch: {
      clampModelVisible(val) {
        if (!val) {
          this.fetch()
        }
      },
    },
    data() {
      return {
        folderId: undefined,
        DrawerLogTitle: '',
        visDrawer: false, //文件变更记录
        moveFileVisible: false,
        logList: [],
        queryParams: {
          docTypes: [2, 3],
          CombineName: undefined,
          sorting: 'creationTime desc',
        },
        columns: [{
            title: '状态',
            align: 'center',
            width: '65px',
            scopedSlots: {
              customRender: 'state'
            },
          },
          {
            title: '序号',
            align: 'center',
            width: '50px',
            scopedSlots: {
              customRender: 'serial'
            },
          },
          {
            title: '合模名称',
            dataIndex: 'combineName',
            scopedSlots: {
              customRender: 'combineName'
            },
            ellipsis: true,
          },
          {
            title: '模型数量',
            align: 'center',
            width: '100px',
            scopedSlots: {
              customRender: 'combineDetails'
            },
          },
          // {
          //   title: '文件类型',
          //   dataIndex: 'suffix',
          //   width: '200px',
          //   align: 'center',
          //   scopedSlots: { customRender: 'suffix' },
          // },
          // {
          //   title: '大小(MB)',
          //   dataIndex: 'docSize',
          //   align: 'center',
          //   width: '90px',
          //   scopedSlots: { customRender: 'docSize' },
          // },
          {
            title: '合模者',
            dataIndex: 'creationName',
            align: 'center',
            width: '100px',
          },
          {
            title: '合模日期',
            dataIndex: 'creationTime',
            align: 'center',
            width: '120px',
            scopedSlots: {
              customRender: 'creationTime'
            },
          },
          {
            title: '操作',
            width: '150px',
            dataIndex: 'action',
            align: 'center',
            scopedSlots: {
              customRender: 'action'
            },
          },
        ],
        selectedRowIds: [], //选中的数据行Id
        selectedRowKeys: this.value?.map((x) => x.id) || [],
        sourceData: [],
        pagination: {
          pageSize: 10,
          current: 1,
        },
        sharedFatherState: false,
        sharedMessage: {
          title: '分享',
          shareType: 5,
          name: '',
        },
        projectMessage: {
          projectId: '',
          id: '',
          sceneType: 0,
        },
        openData: {
          visible: false,
          isSet: undefined,
          width: 0,
          height: 0,
          Document: {
            suffix: '',
          },
        },
        newMoldClosingVisible: false, //新建合模弹窗状态
        checkedModelKeys: [],
        clampingList: [], //合模列表
        clampModelVisible: false,
        openInGis: false,
        clampingExists: [],
        folderIdClamping: '',
        isTable: false,
      }
    },
    mounted() {
      const that = this
      this.onResize()
      window.onresize = () => {
        this.onResize()
      }
      //是否公开浏览状态
      if (this.$store.state.user.info.userName === 'guest') {
        this.columns = [{
            title: '状态',
            width: '65px',
            align: 'center',
            scopedSlots: {
              customRender: 'state'
            },
          },
          {
            title: '序号',
            width: '50px',
            align: 'center',
            scopedSlots: {
              customRender: 'serial'
            },
          },
          {
            title: '文件名称',
            dataIndex: 'combineName',
            scopedSlots: {
              customRender: 'combineName'
            },
            ellipsis: true,
          },
          {
            title: '模型数量',
            align: 'center',
            width: '100px',
            scopedSlots: {
              customRender: 'combineDetails'
            },
          },
          // {
          //   title: '类型',
          //   dataIndex: 'suffix',
          //   width: '60px',
          //   scopedSlots: { customRender: 'suffix' },
          //   align: 'center',
          // },
          // {
          //   title: '大小(MB)',
          //   dataIndex: 'docSize',
          //   align: 'center',
          //   width: '90px',
          //   scopedSlots: { customRender: 'docSize' },
          // },
          {
            title: '合模者',
            dataIndex: 'creationName',
            align: 'center',
            width: '90px',
          },
          {
            title: '上传日期',
            dataIndex: 'creationTime',
            align: 'center',
            width: '120px',
            scopedSlots: {
              customRender: 'creationTime'
            },
          },
        ]
      }
      this.fetch()
    },
    methods: {
      fetch() {
        const that = this
        this.queryParams.projectId = this.currProject.id
        this.queryParams.folderId = this.folderId
        getCombineList({
          maxResultCount: this.pagination.pageSize,
          skipCount: (this.pagination.current - 1) * this.pagination.pageSize,
          ...that.queryParams,
        }).then((res) => {
          const pagination = {
            ...that.pagination
          }
          pagination.total = res.result.totalCount
          res.result.data.forEach(item => {
            if (item.blobName) {
              item.img = getFileByBlobName(item.blobName)
            } else {
              item.img = '/image/pro-bg.png'
            }
          })
          that.sourceData = res.result.data
          that.pagination = pagination
        })
      },
      handleTableChange(pagination) {
        const pager = {
          ...this.pagination
        }
        pager.current = pagination.current
        this.pagination = pager
        this.fetch()
      },
      delInfo(record) {
        const that = this
        that.$confirm({
          content: '删除后不能恢复，确定' + record.combineName + '删除吗？',
          okText: '确定',
          cancelText: '取消',
          onOk() {
            deleted(record.id).then((res) => {
              that.resetCurrenPage()
              that.fetch()
              that.$message.success('操作成功')
            })
          },
          onCancel() {},
        })
      },
      search() {
        this.pagination.current = 1;
        this.resetCurrenPage()
        this.fetch()
      },
      moveFile() {
        const that = this
        if (that.selectedRowIds.length == 0) {
          that.$message.warning('请选择需要移动的文件')
        } else {
          this.ids = that.selectedRowIds
          this.moveFileVisible = true
        }
      },
      //重新计算当前页码（如果删除第二页最后一条，则刷新到上一页）
      resetCurrenPage() {
        const totalPage = Math.ceil((this.pagination.total - 1) / this.pagination.pageSize) // 总页数
        this.pagination.current = this.pagination.current > totalPage ? totalPage : this.pagination.current
        this.pagination.current = this.pagination.current < 1 ? 1 : this.pagination.current
      },
      swapVera(info) {
        const that = this
        that.$refs.refSwapVer.showModal(info.id, info.combineName, true)
      },
      shareNodeItem(data) {
        const that = this
        that.sharedFatherState = true
        that.sharedMessage.shareType = 3
        that.sharedMessage.id = data.id
        that.sharedMessage.name = data.combineName
        that.projectMessage.id = data.id
        that.projectMessage.projectId = this.currProject.id
        that.projectMessage.sceneType = 1
      },
      showDrawer(data) {
        this.DrawerLogTitle = data.combineName
        this.visDrawer = true
        getCombineLogs(data.id).then((res) => {
          this.logList = res
        })
      },
      previewFile(info) {
        this.openData.Document = info
        var routerUrl = this.$router.resolve({
          path: '/combine/' + info.id,
        })
        const combine = {
          doc: this.openData.Document,
          openType: 'clamping',
          openInGis: this.openInGis,
          modelType: 'combineModel'
        }
        storage.set('combineInfo', JSON.stringify(combine))
        window.open(routerUrl.href, '_blank')
      },
      setModel(info, type) {
        this.openData.visible = true
        this.openData.isSet = true
        this.openData.Document = info
        this.openInGis = type
      },
      onResize() {
        this.openData.width = window.innerWidth
        this.openData.height = window.innerHeight - 152
      },
      newMoldClosing() {
        const that = this
        if (!this.folderIdClamping.length) {
          this.$message.info('请先创建模型目录')
          return
        }
        that.$confirm({
          title: '是否将合模结果保存在当前所选模型目录?',
          okText: '确认',
          cancelText: '取消',
          onOk() {
            that.clampingList = []
            that.newMoldClosingVisible = true
          },
        })
      },
      doSelect(records,state) {
        if (state) {
          this.clampingList.push(records)
        } else {
          let index = this.clampingList.findIndex((item) => {
            return item.id == records.id
          })
          this.clampingList.splice(index, 1)
        }
      },
      selectClamping() {
        const that = this
        if (that.clampingList.length > 0) {
          if (that.clampingExists.length > 0) {
            that.$confirm({
              title: '提示',
              content: '模型' + that.clampingExists.toString() + '已存在合模中，是否继续？',
              okText: '确定',
              cancelText: '取消',
              onOk: () => {
                var routerUrl = that.$router.resolve({
                  name: 'clamping'
                })
                const info = {
                  clampingList: that.clampingList,
                  projectMessage: that.projectMessage,
                  clampModelVisible: that.clampModelVisible,
                  folderIdClamping: that.folderIdClamping
                }
                storage.set('clampingInfo', JSON.stringify(info))
                window.open(routerUrl.href, '_blank')
                that.newMoldClosingVisible = false
                that.checkedModelKeys = []
              },
              onCancel() {},
            })
          } else {
            var routerUrl = that.$router.resolve({
              name: 'clamping'
            })
            const info = {
              clampingList: that.clampingList,
              projectMessage: that.projectMessage,
              clampModelVisible: that.clampModelVisible,
              folderIdClamping: that.folderIdClamping
            }
            storage.set('clampingInfo', JSON.stringify(info))
            window.open(routerUrl.href, '_blank')
            that.newMoldClosingVisible = false
            that.checkedModelKeys = []
          }
        } else {
          that.$message.warning('请先选择需要合模的模型！')
        }
      },
      newMoldClosingHide() {
        this.newMoldClosingVisible = false
        this.checkedModelKeys = []
      },
      findItem(arr, key, val) {
        for (let i = 0; i < arr.length; i++) {
          if (arr[i][key] == val) {
            return i
          }
        }
        return -1
      },
      changeClampingList(data, checked) {
        const that = this
        const index = that.findItem(that.clampingList, 'key', data.key)
        if (checked) {
          if (index == -1) {
            that.clampingList.push(data)
            getHasClampById(data.key).then((res) => {
              if (res.length > 0) {
                that.clampingExists.push(data.docName)
              }
            })
          }
        } else {
          if (index != -1) {
            that.clampingList.splice(index, 1)
            const sign = that.clampingExists.indexOf(data.docName)
            sign != -1 ? that.clampingExists.splice(sign, 1) : null
          }
        }
      },
      onOk(pid) {
        if (pid.length > 0) {
          this.folderId = pid
          if (typeof pid != 'string') {
            this.folderIdClamping = pid[0]
          } else {
            this.folderIdClamping = pid
          }
          this.pagination = {
            pageSize: 10,
            current: 1,
          };
          this.fetch()
        } else {
          this.folderIdClamping = ''
        }
      },
      changePageSize(current) {
        const that = this
        const pager = {
          ...this.pagination
        }
        pager.current = current
        this.pagination = pager
        this.fetch({
          ...this.queryParams,
        })
      },
    },
  }
</script>

<style lang="less" scoped>
  /deep/.ant-card-cover>img {
    height: 165px;
  }

  .cut-box {
    padding: 4px 4px 2px 4px;
  }

  .down-list {
    width: 100%;
    border-top: 1px solid #e8e8e8;
    border-left: 1px solid #e8e8e8;

    tr td,
    tr th {
      border-right: 1px solid #e8e8e8;
      border-bottom: 1px solid #e8e8e8;
      text-align: center;
      height: 40px;
    }

    tr th {
      background-color: #fafafa;
    }
  }

  /deep/.card-box {
    >.ant-card-body {
      padding-top: 20px;
    }

    >.ant-card-head {
      padding: 5px 12px;
    }
  }

  /deep/.ant-card {


    .ant-card-grid {
      padding: 15px;
    }


  }

  .model-title {
    width: 75%;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  /deep/.ant-table {
    >.ant-table-content>.ant-table-body>table {

      .ant-table-thead>tr>th,
      .ant-table-tbody>tr>td {
        padding: 15px 8px;
      }
    }
  }
</style>