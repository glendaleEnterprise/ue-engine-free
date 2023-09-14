<template>
  <a-row :gutter="24">
    <div v-if="isMobile" class="mobile-box"></div>
    <a-col v-if="!versionChange" :md="24" :lg="3">
      <folder-tree @ok="onOk" />
    </a-col>
    <a-col v-if="!versionChange" :md="24" :lg="21" style="padding-top: 35px;">
      <a-card size="small" :bordered="false" class="card-box" v-if="isResh">
        <a-space slot="title" v-action:Design.Business.Documents.Search>
          <a-input placeholder="关键字" v-model="queryParams.keyword" style="width: 300px" />
          <a-button v-action:Design.Business.Documents.Search type="primary" @click="search">查询</a-button>
        </a-space>
        <a-space slot="extra">
          <a-button v-action:Design.Business.Documents.Upload type="primary" @click="$refs.modal.show(folderId)" ghost>
            模型上传
          </a-button>
          <a-button v-action:Design.Business.Documents.Move type="primary" @click="moveFile" ghost> 模型移动 </a-button>
          <!-- <a-button v-action:Design.Business.Documents.Public type="primary"
            @click="$refs.filePublic.open(selectedRowIds)" ghost> 公开/私有 </a-button> -->
          <a-button v-action:Design.Business.Documents.Delete type="danger" ghost @click="onDeletes">删除</a-button>
          <a-button type="primary" @click="() => { isTable = !isTable }" class="cut-box">
            <a-icon v-if="isTable" type="menu" style="font-size: 20px" />
            <a-icon v-else type="appstore" style="font-size: 20px" />
          </a-button>
        </a-space>
        <a-table v-if="isTable" ref="table" class="documentTable" :columns="columns" :row-key="(record) => record.id"
          :data-source="sourceData"
          :row-selection="currentUser.userName == 'guest' ? null : { selectedRowKeys: selectedRowIds, onChange: onSelectChange }"
          bordered :pagination="false" size="middle">
          <template slot="serial" slot-scope="text, record, index">{{ index + 1 }}</template>
          <template slot="docName" slot-scope="text, record">
            <a v-if="record.status === 2" @click="previewFile(record)">{{ text }}</a>
            <span v-else>{{ text }}</span>
          </template>
          <template slot="docSize" slot-scope="text">{{ text | byteToGB }}</template>
          <template slot="creationTime" slot-scope="text">{{ text | dateFomart('YYYY-MM-DD') }}</template>
          <template slot="lastModificationTime" slot-scope="text, record">
            <span v-if="text">{{ text | dateFomart('YYYY-MM-DD') }}</span>
            <span v-else>{{ record.creationTime | dateFomart('YYYY-MM-DD') }}</span>
          </template>
          <template slot="isPublic" slot-scope="text">
            <a-tag v-if="text == true" color="red">公开</a-tag>
            <a-tag v-else color="green">私有</a-tag>
          </template>
          <template slot="statusName" slot-scope="text,record">
            <span v-if="record.documentType != 3">{{  text  }}</span>
            <span v-else>{{ record.status === 2 ? '上传成功':'上传中'  }}</span>
          </template>
          <template slot="action" slot-scope="text, record">
            <a-space v-if="record.documentType != 3">
              <a-tooltip title="上传新版本" v-action:Design.Business.Documents.UploadNewVersion>
                <a-button v-action:Design.Business.Documents.UploadNewVersion size="small" icon="cloud-upload"
                  type="link" @click="$refs.newModal.show(record)" :disabled="record.status != 2" />
              </a-tooltip>
              <a-tooltip title="切换版本" v-action:Design.Business.Documents.Version>
                <a-button v-action:Design.Business.Documents.Version size="small" @click="swapVera(record)" icon="swap"
                  type="link" :disabled="record.status != 2" />
              </a-tooltip>
              <a-tooltip title="分享" v-action:Design.Business.Documents.Share
                v-if='currentUser.userName != "guest" && currentUser.extraProperties.id == record.creatorId'>
                <a-button v-action:Design.Business.Documents.Share size="small" icon="share-alt" type="link"
                  @click="shareNodeItem(record)" :disabled="record.status != 2" />
              </a-tooltip>
              <a-tooltip title="下载" v-action:Design.Business.Documents.Download>
                <a-button v-action:Design.Business.Documents.Download size="small" icon="download"
                  @click="onDownload(record)" type="link" />
              </a-tooltip>
              <a-tooltip title="记录">
                <a-button size="small" icon="unordered-list" type="link" @click="showDrawer(record)" />
              </a-tooltip>
            </a-space>
          </template>
        </a-table>
        <div v-else>
          <a-card :bordered="false" v-if="sourceData.length > 0" :class="isMobile?'mobile-terminal':''">
            <div class="logo" v-if="isMobile">
              {{$store.state.projectName}}
            </div>
            <a-checkbox-group @change="ModelListSelect" class="scroll-box">
              <a-card-grid style="width: 20%" v-for="(record, index) in  sourceData " :key="index">
                <a-card :bodyStyle="{ padding: '10px 5px' }">
                  <template>
                    <img v-if="record.status === 2" @click="previewFile(record)" slot="cover" alt="example"
                      :src="record.img" style="cursor: pointer;" />
                    <img v-else slot="cover" alt="example" :src="record.img" />
                  </template>
                  <template slot="actions" v-if="record.documentType != 3">
                    <a-tooltip title="上传新版本" v-action:Design.Business.Documents.UploadNewVersion>
                      <a-button v-action:Design.Business.Documents.UploadNewVersion size="small" icon="cloud-upload"
                        type="link" @click="$refs.newModal.show(record)" :disabled="record.status != 2" />
                    </a-tooltip>
                    <a-tooltip title="切换版本" v-action:Design.Business.Documents.Version>
                      <a-button v-action:Design.Business.Documents.Version size="small" @click="swapVera(record)"
                        icon="swap" type="link" :disabled="record.status != 2" />
                    </a-tooltip>
                    <a-tooltip title="分享" v-action:Design.Business.Documents.Share
                      v-if='currentUser.userName != "guest" && currentUser.extraProperties.id == record.creatorId'>
                      <a-button v-action:Design.Business.Documents.Share size="small" type="link"
                        @click="shareNodeItem(record)" icon="share-alt" :disabled="record.status != 2" />
                    </a-tooltip>
                    <a-tooltip title="下载" v-action:Design.Business.Documents.Download>
                      <a-button v-action:Design.Business.Documents.Download size="small" icon="download"
                        @click="onDownload(record)" type="link" />
                    </a-tooltip>
                    <a-tooltip title="记录">
                      <a-button size="small" type="link" icon="unordered-list" @click="showDrawer(record)" />
                    </a-tooltip>
                  </template>
                  <template slot="actions" v-else>
                    <div style="height: 24px;"></div>
                  </template>
                  <a-card-meta>
                    <template slot="title">
                      <div style="display: flex;justify-content: space-between;align-items: center;">
                        <a-checkbox :value="record.id" class="mask-card" :id="record.id"
                          v-if="currentUser.userName != 'guest'"></a-checkbox>
                        <a-tooltip :title="record.documentName" placement="bottom">
                          <a class="model-title" v-if="record.status === 2" @click="previewFile(record)">{{
                            record.documentName }}</a>
                          <span class="model-title" v-else>{{ record.documentName }}</span>
                        </a-tooltip>
                        <span style="color: rgba(0, 0, 0, 0.45);font-weight: 400;font-size: 14px;">{{ record.size |
                          byteToGB
                        }}</span>
                      </div>
                    </template>
                    <template slot="description">
                      <div style="display: flex;justify-content: space-between;">
                        <div>
                          <a-icon type="user" :title="record.creationName" /> {{ record.creationName }}</div>
                        <div>
                          <a-icon type="clock-circle" /> {{ record.creationTime |
                          dateFomart('YYYY-MM-DD') }}</div>
                      </div>
                    </template>
                  </a-card-meta>
                  <template>
                    <div style="position: absolute; top: 0; left: 6px; padding-top: 10px">
                      <a-tag color="#87d068" v-if="record.status === 2">
                        {{ record.documentType != 3 ? record.statusName:'上传成功' }} </a-tag>
                      <a-tag v-else> {{  record.documentType != 3 ? record.statusName : '上传中' }} </a-tag>
                      <!-- <br /> -->
                      <!-- {{ record.size | byteToMB }}MB -->
                    </div>
                    <div div style=" position: absolute; top: 0; right: 0; padding-top: 10px">
                      <a-tag color="cyan">{{ record.modelFormat }} </a-tag>
                    </div>
                  </template>
                </a-card>
              </a-card-grid>
            </a-checkbox-group>
          </a-card>
          <a-card v-else>
            <a-empty :image="simpleImage" />
          </a-card>
        </div>
        <div style="padding-top: 20px; text-align: right;padding-bottom: 10px">
          <a-pagination size="small" :pageSize="pagination.pageSize" :show-total="pagination.showTotal"
            :total="pagination.total" @change="changePageSize" />
        </div>
      </a-card>
      <a-drawer maskClosable placement="right" @close="visDrawer = false" :title="DrawerLogTitle" :closable="false"
        width="320" :visible="visDrawer">
        <template>
          <a-timeline>
            <a-empty v-if="this.logList.length <= 0" />
            <a-timeline-item v-for="( item, i ) in  logList " :key="i">
              <a-icon slot="dot" type="clock-circle-o" style="font-size: 16px" />
              <p>
                {{ item.creationTime }}&nbsp;&nbsp;<span>[{{ item.creatorName || 'System' }}]</span>
              </p>
              <p>{{ item.remark }}</p>
            </a-timeline-item>
          </a-timeline>
        </template>
      </a-drawer>
      <!-- 上传模型 -->
      <upload-modal ref="modal" @fetch="fetch" />
      <!-- 上传新版本 -->
      <upload-new-modal ref="newModal" @fetch="fetch" />
      <!-- 移动文件 -->
      <move-file-modal v-if="moveFileVisible" :moveFileVisible.sync="moveFileVisible" :ids="ids" @fetch="fetch"
        @update="update" modalTitle="模型移动" />
      <!-- 公开文件 -->
      <file-public title="模型公开/私有" ref="filePublic" @fetch="fetch" />
      <!-- 切换模型版本 -->
      <swap-ver @fetch="fetch" ref="refSwapVer" @openVersion="OpenVersionComparsion" />
      <shared-model :sharedMessage="sharedMessage" :sharedFatherState.sync="sharedFatherState"
        :projectMessage="projectMessage" />
      <a-modal title="下载版本" :width="600" :visible="downloadData.visible" :forceRender="true" :destroyOnClose="true"
        :maskClosable="false" :footer="null" :bodyStyle="{ padding: '10px 10px 15px 10px' }" @cancel="() => {
          downloadData.visible = false
        }
          ">
        <table class="down-list">
          <tr>
            <!-- <th>名称</th> -->
            <th>版本号</th>
            <th>创建时间</th>
            <th>下载</th>
          </tr>
          <tr v-for="( record, index ) in  downloadData.documentVersion " :key="index">
            <!-- <td>{{ record.documentName }}</td> -->
            <td>
              <div v-if="record.isCurrent" style="color: green; font-weight: bolder" title="当前选中版本">
                V{{ record.versionNo.toFixed(1) }}
              </div>
              <span v-else>V{{ record.versionNo.toFixed(1) }}</span>
            </td>
            <td>{{ record.creationTime | dateFomart('YYYY-MM-DD') }}</td>
            <td>
              <a-tooltip title="下载">
                <a-button size="small" @click="downloadUp(record, downloadData.info)" type="link" icon="download" />
              </a-tooltip>
            </td>
          </tr>
        </table>
      </a-modal>
    </a-col>
    <version-comparison v-if="versionChange" :versionId="versionId" @close="versionChange = false" />
  </a-row>
</template>

<script>
  import {
    Empty
  } from 'ant-design-vue'
  import {
    mapGetters
  } from 'vuex'
  import FolderTree from './modules/FolderTree.vue'
  import FilePublic from './modules/FilePublic.vue'
  import MoveFileModal from './modules/MoveFileModal.vue'
  import SwapVer from './modules/SwapVers.vue'
  import UploadModal from './modules/UploadModal.vue'
  import UploadNewModal from './modules/UploadNewModal.vue'
  import SharedModel from '../bim/modules/SharedModel.vue'
  import ViewBIM from '../bim/Index.vue'
  import VersionComparison from './modules/VersionComparison'
  import {
    getFileByBlobName
  } from '@/api/file'
  import {
    getDocList,
    mdeleteDoc,
    getDocLog,
    downloadModel,
    downloadOsgb,
    createLog,
  } from '@/api/document'
  import download from 'downloadjs'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    name: 'Model',
    components: {
      FolderTree,
      FilePublic,
      SwapVer,
      SharedModel,
      UploadModal,
      UploadNewModal,
      MoveFileModal,
      'view-bim': ViewBIM,
      VersionComparison,
    },
    beforeCreate() {
      this.simpleImage = Empty.PRESENTED_IMAGE_SIMPLE
    },
    computed: {
      ...mapGetters(['currProject']),
      ...mapGetters({
        currentUser: 'userInfo',
      }),
    },
    data() {
      return {
        folderId: undefined,
        DrawerLogTitle: '',
        visDrawer: false, //文件变更记录
        moveFileVisible: false,
        logList: [],
        queryParams: {
          documentType: [1, 2, 3],
          keyword: undefined,
          sorting: 'creationTime desc',
        },
        columns: [{
            title: '序号',
            align: 'center',
            width: '50px',
            scopedSlots: {
              customRender: 'serial'
            },
          },
          {
            title: '模型名称',
            dataIndex: 'documentName',
            scopedSlots: {
              customRender: 'docName'
            },
            ellipsis: true,
          },
          {
            title: '类型',
            dataIndex: 'modelFormat',
            width: '90px',
            align: 'center',
          },
          {
            title: '大小',
            dataIndex: 'size',
            align: 'center',
            width: '90px',
            scopedSlots: {
              customRender: 'docSize'
            },
          },
          {
            title: '轻量化说明',
            dataIndex: 'statusName',
            align: 'center',
            width: '200px',
            scopedSlots: {
              customRender: 'statusName'
            },
          },
          {
            title: '上传者',
            dataIndex: 'creationName',
            align: 'center',
            width: '100px',
          },
          {
            title: '更新日期',
            dataIndex: 'lastModificationTime',
            align: 'center',
            width: '110px',
            scopedSlots: {
              customRender: 'lastModificationTime'
            },
          },
          {
            title: '操作',
            width: '180px',
            dataIndex: 'action',
            align: 'center',
            scopedSlots: {
              customRender: 'action'
            },
          },
        ],
        selectedRowIds: [], //选中的数据行Id
        selectedRowKeys: this.value ? this.value.map((x) => x.id) : [],
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
        downloadData: {
          visible: false,
          info: {},
          documentVer: [], //版本列表
        },
        versionChange: false,
        versionId: '',
        isTable: false,
        isResh: true,
        reshFid: undefined,
        isMobile: false,
      }
    },
    mounted() {
      const that = this;
      that.isMobile = _isMobile();
      if (that.isMobile) {
        let body = document.getElementsByTagName("body");
        body[0].setAttribute("style", "")
      }
    },
    methods: {
      fetch() {
        const that = this
        this.queryParams.projectId = this.currProject.id
        this.queryParams.projectFolderId = this.folderId
        this.queryParams.MaxResultCount = this.pagination.pageSize
        this.queryParams.SkipCount = (this.pagination.current - 1) * this.pagination.pageSize
        getDocList(Object.assign({}, this.queryParams)).then((res) => {
          const pagination = {
            ...this.pagination
          }
          pagination.total = res.totalCount
          res.items.forEach(item => {
            if (item.blobName) {
              item.img = getFileByBlobName(item.blobName)
            } else {
              item.img = '/image/pro-bg.png'
            }
          })
          this.sourceData = res.items
          this.pagination = pagination
        })
      },
      onSelectChange(selectedRowKeys, selectedRows) {
        this.selectedRowIds = selectedRowKeys
      },
      onDeletes() {
        const that = this
        if (that.selectedRowIds.length == 0) {
          that.$message.warning('请选择要删除的模型')
          return
        }
        that.$confirm({
          content: '删除后不能恢复，多个版本以及参与合模场景将一并删除，确定删除吗？',
          onOk() {
            mdeleteDoc(that.selectedRowIds).then((res) => {
              that.pagination.total -= that.selectedRowIds.length;
              that.selectedRowIds = []
              that.resetCurrenPage()
              that.fetch()
              that.$message.success('操作成功')
            })
          },
          onCancel() {},
        })
      },
      ModelListSelect(data) {
        this.selectedRowIds = data
      },
      onOk(pid) {
        this.reshFid = pid
        if (pid.length > 0) {
          this.folderId = pid
          this.pagination = {
            pageSize: 10,
            current: 1,
          };
          this.fetch()
        } else {}
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
        that.$refs.refSwapVer.showModal(info.id, info.documentName, true)
      },
      shareNodeItem(data) {
        const that = this
        that.sharedFatherState = true
        that.sharedMessage.shareType = 4
        that.sharedMessage.id = data.id
        that.sharedMessage.name = data.docName
        that.projectMessage.id = data.id
        that.projectMessage.projectId = this.currProject.id
        that.projectMessage.sceneType = 0
      },
      showDrawer(data) {
        this.DrawerLogTitle = data.docName
        this.visDrawer = true
        getDocLog(data.id).then((res) => {
          this.logList = res
        })
      },
      previewFile(info) {
        var routerUrl = this.$router.resolve({
          path: '/model/' + info.id,
        })
        this.isMobile ? window.open(routerUrl.href, '_self') : window.open(routerUrl.href, '_blank')
      },

      //点击下载
      onDownload(record) {
        if (record.documentVersion.length > 1) {
          this.downloadData.visible = true
          this.downloadData.info = record
          this.downloadData.documentVersion = record.documentVersion
        } else {
          this.download(record)
        }
      },
      downloadUp(v, w) {
        v.documentName = w.documentName
        this.download(v)
      },
      async download(para) {
        this.$message.loading('即将下载，请稍候...')
        if (para.documentType == 1) {
          await downloadModel(para.modelName)
            .then((res) => {
              if (res.code == 1) {
                download(res.datas)
              }
            })
            .catch((error) => {
              this.$message.error('下载失败')
            })
        } else if (para.documentType == 2) {
          await downloadOsgb(para.modelName)
            .then((res) => {
              if (res.code == 1) {
                download(res.datas)
              }
            })
            .catch((error) => {
              this.$message.error('下载失败')
            })
        }
        var docId = para.documentId || para.id
        createLog(docId, 2) //新增下载记录
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
      OpenVersionComparsion(val) {
        this.versionChange = true
        this.versionId = val
      },
      update() {
        this.isResh = false;
        this.reshFid ? this.onOk(this.reshFid) : null
        this.selectedRowIds = []
        this.$nextTick(() => {
          this.isResh = true;
        })
      },
      closePage() {
        window.close()
      }
    },
    beforeRouteLeave(to, from, next) {
      if (_isMobile()) {
        if (to.name != 'model') {
          this.closePage()
        } else {
          next()
        }
      } else {
        next()
      }
    }
  }
</script>

<style lang="less" scoped>
  .documentTable .ant-btn-icon-only {
    width: auto;
  }

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

      .ant-card-body {
        padding: 0;
      }

    }

    >.ant-card-head {
      padding: 5px 12px;
    }
  }

  /deep/.ant-card {

    .ant-checkbox-group {
      width: 100%;

      .ant-card-grid {
        padding: 15px;
      }
    }
  }

  .model-title {
    width: 70%;
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

  /deep/.mask-card {
    margin-right: 7px;

    .ant-checkbox-inner {
      border-color: #9f9f9f;
    }
  }

  .mobile-box {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    z-index: 800;
    background-color: #ffffff;
  }

  /deep/.mobile-terminal {
    .logo {
      padding: 0 20px;
      background: #323641;
      height: 64px;
      line-height: 64px;
      color: #fff;
      font-size: 16px;
      font-weight: 500;
    }

    >.ant-card-body {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      // overflow-y: auto;
      background: #fff;
      z-index: 850;

      .ant-checkbox-group {
        margin-top: 0;

        .ant-card-grid {
          padding: 30px 50px;
          width: 100% !important;

          .ant-card-cover>img {
            height: 180px;
          }
        }
      }
    }

    .scroll-box {
      margin-top: 15px;
      height: 100vh;
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
        background: #ffffff;
      }

      &::-webkit-scrollbar-track {
        //轨道的样式
        box-shadow: 0;
        border-radius: 0;
        background: rgba(255, 255, 255, 0.3);
      }

    }
  }
</style>