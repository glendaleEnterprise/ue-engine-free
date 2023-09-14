<template>
  <div class="side-frame">
      <a-button icon="plus" style='position: absolute;margin-top: -20px' @click='CreateMenuClick()' ghost>添加批注</a-button>
    <br />
    <a-card size="small" :bordered="false" >
      <a-tabs size="small" :animated="false" tabPosition='top'>
        <a-tab-pane :key="1" tab="提交的批注">
          <div class="model-tree-box">
            <a-table :columns="launchColumns" :data-source="launchList" size="small" v-show='launchList.length' :rowKey="(row) => row.id" :pagination="pagination" @change="handleTableChange">
              <template slot="status" slot-scope="text">
                <span v-show='text==0'>待处理</span>
                <span v-show='text==1'>已处理</span>
                <span v-show='text==2'>已关闭</span>
              </template>
              <template slot="operation" slot-scope="text, record">
                  <a-space>
                    <a><a-icon type="eye" @click="info(record,1)" title="查看" /></a>

                    <a-popconfirm title="确定删除吗？" ok-text="确定" cancel-text="取消" @confirm="confirm">
                      <a><a-icon type="delete" @click="delNode(record.id,)" title="删除" /></a>
                    </a-popconfirm>

                    <a-popconfirm title="确定关闭吗？" ok-text="确定" cancel-text="取消" @confirm="getPostilStatus(record.id)" v-if='record.status===1'>
                      <a> <a-icon type="check-circle" title="关闭" /></a>
                    </a-popconfirm>
                  </a-space>
              </template>
            </a-table>
            <a-empty v-show='!launchList.length' description='暂无数据' />
          </div>
        </a-tab-pane>
        <a-tab-pane :key="2" tab="处理的批注">
          <div class="model-tree-box">
            <a-table :columns="launchColumns" :data-source="handleList" size="small" v-show='handleList.length'  :rowKey="(row) => row.id" :pagination="pagination1" @change="handleTableChange1">
              <template slot="status" slot-scope="text">
                <span v-show='text==0'>待处理</span>
                <span v-show='text==1'>已处理</span>
                <span v-show='text==2'>已关闭</span>
              </template>
              <template slot="operation" slot-scope="text, record">
                <a><a-icon type="eye" @click="info(record,2)" title="查看" /></a>
              </template>
            </a-table>
            <a-empty v-show='!handleList.length' description='暂无数据' />
          </div>
        </a-tab-pane>
      </a-tabs>
    </a-card>
    <mark-page :viewpointData="viewpointPictureData" :drawType='drawType' :changeStyle='changeStyle' :showDrawer.sync='showDrawer' :cameraData="cameraData" :projectMessage="projectMessage" v-if="markOpen" :markOpen.sync="markOpen" :base64image.sync="base64image" :viewData="viewData"  />
    <mark-page-show  :drawType='drawType' :projectMessage="projectMessage" v-if="markShowOpen"   :markPageShowData='markPageShowData' :base64image='base64image' @changeShow='changeShow'/>
    <canvas id="photo" style="display: none;"></canvas>
  </div>
</template>

<script>
import { getFileByBlobName } from '@/api/file'
import MarkPage from './MarkPage.vue'
import MarkPageShow from './MarkPageShow.vue'
import SaveView from './SaveView'
import { eventBus } from '@/utils/bus'
import html2canvas from 'html2canvas'
import { deletePostil, getPostilHandle, getPostilLaunch, getPostilStatus } from '@/api/postil'
import { mapGetters } from 'vuex'
import store from '@/store'
export default {
  components: { MarkPage, SaveView,MarkPageShow },
  name: 'ViewpointSettingNew',
  props: {
    // eslint-disable-next-line vue/require-default-prop
    modelApi: { type: Object, },
    // eslint-disable-next-line vue/require-default-prop
    projectMessage: { type: Object, },
    // eslint-disable-next-line vue/require-default-prop
    store: { type: Object, },
    user: { type: Object, },
    // eslint-disable-next-line vue/require-default-prop
    currProject: { type: Object, },
    // eslint-disable-next-line vue/require-default-prop
    pattern: null,//默认:都可以操作,'true':只能看,
  },
  data () {
    return {
      privilege: {//权限
        isDelete: true,//是否可以删除
        isCreate: true,//是否可以创建
        isShare: true,//是否可以分享
        isLeadingIn: true,//是否可以导入
      },
      markPageShowData:{},
      changeStyle: false,
      drawType: 'rect',
      viewData: [
        {
          viewName: this.projectMessage.docName,
          id: this.projectMessage.id,
          children: [],
          level: 0,
          slots: {
            icon: 'folder',
          },
          glid: 0,
        },
      ],
      launchColumns:[
        {
          title: '标题',
          dataIndex: 'title',
          key: 'title',
          scopedSlots: { customRender: 'title' },
          ellipsis:true
        },
        {
          title: '状态',
          dataIndex: 'status',
          key: 'status',
          scopedSlots: { customRender: 'status' },
          width: 60,
        },
        {
          title: '操作',
          dataIndex: 'operation',
          key: 'operation',
          scopedSlots: { customRender: 'operation' },
          width: 80,
        },
      ],
      treeList: [],
      replaceFields: {
        title: 'title',
        key: 'id',
        value: 'id',
      },
      checkedViewKeys: [],
      markOpen: false,
      markShowOpen: false,
      showDrawer: true,
      base64image: '',
      cameraData: '',
      viewpointPictureData: {
        viewPointImg: '',
        pictureCore: '',
        cameraData: this.cameraData,
        sceneType: this.projectMessage.sceneType,
        documentId: this.projectMessage.modelId,
        viewPosition: '',
      },
      createView: false,
      deleteItem: '',
      sharedFatherState: false,
      sharedMessage: {
        title: '视点分享',
        shareType: 1,
      },
      selectedKeys: [],
      expandedKeys: [],
      launchList: [],
      handleList: [],
      pagination: {
        current: 1,
        pageSize: 15,
      },
      pagination1: {
        current: 1,
        pageSize: 15,
      },
    }
  },
  async mounted () {
    const that = this
    eventBus.$off('getPostilLaunch')
    eventBus.$on('getPostilLaunch', (val) => {
      this.getPostilLaunch()
      this.getPostilHandle()
    })
    if (that.pattern) {
      that.privilege = {//权限
        isDelete: false,//是否可以删除
        isCreate: false,//是否可以创建
        isShare: false,//是否可以分享
        isLeadingIn: false,//是否可以导入
      }
    }
    this.getPostilLaunch()
    this.getPostilHandle()
  },
  computed: {

  },
  methods: {

    getFileByBlobName,
    getPostilLaunch(){
      const param={
        documentId:this.projectMessage.modelId,
        MaxResultCount : this.pagination.pageSize,
        SkipCount: (this.pagination.current - 1) * this.pagination.pageSize
      }
      getPostilLaunch(param).then(data=>{
        this.launchList=data.items
        const pagination = { ...this.pagination }
        pagination.total = data.totalCount
        this.pagination = pagination
      })
    },
    handleTableChange(pagination) {
      const that = this
      this.pagination = pagination
      this.pagination.skipCount = (pagination.current - 1) * pagination.pageSize
      this.getPostilLaunch()
    },
    getPostilHandle(){

      const param={
        documentId: this.projectMessage.modelList[0].id,
        handlerUserId: this.user.extraProperties.id,
        MaxResultCount : this.pagination1.pageSize,
        SkipCount: (this.pagination1.current - 1) * this.pagination1.pageSize
      }
      getPostilHandle(param).then(data=>{
        this.handleList=data.items
        const pagination = { ...this.pagination1 }
        pagination.total = data.totalCount
        this.pagination1 = pagination
      })
    },
    handleTableChange1(pagination) {
      const that = this
      this.pagination1 = pagination
      this.pagination1.skipCount = (pagination.current - 1) * pagination.pageSize
      this.getPostilHandle()
    },
    changeShow(){
      this.markShowOpen=false
    },
    filterList (tree) {
      var newArr = []
      for (var i = 0; i < tree.length; i++) {
        var item = tree[i]
        item.title = item.viewName
        if (item.viewType == 0) {
          item.slots = { icon: 'folder' }
          item.scopedSlots = { title: 'folder' }
        } else if (item.viewType == 1) {
          item.slots = { icon: 'file' }
          item.scopedSlots = { title: 'custom' }
          item.isLeaf = true
        }
        if (item.children) {
          item.children = this.filterList(item.children)
        }
        newArr.push(item)
      }
      return newArr
    },
    ss(){
      var that = this;
      that.$sapi.Public.saveScreenShot((imgdata) => {
        this.$sapi.Camera.getViewPort((data) => {
          that.MarkViewpoint(data, imgdata)
        })
      })
    },
    async info(info,type){
      if (info.viewPosition) {
        this.$sapi.Camera.setViewPort( JSON.parse(info.viewPosition))
        this.base64image=await getFileByBlobName(info.imgPath)
        info.type=type
        this.markPageShowData = info
        this.markShowOpen = info

      }
    },
    CreateMenuClick (e) {
      this.changeStyle = true
      this.drawType='rect'
      this.ss()
      this.changeShow()
    },
    MarkViewpoint (cameraData, base64image) {

      const that = this
      that.cameraData = cameraData
      if (that.cameraData != null) {
        this.$sapi.Camera.setViewPort(cameraData)
      }
      that.viewpointPictureData.viewPosition=cameraData
      that.base64image = base64image
      eventBus.$emit('changeShowDrawer', false)
      that.showDrawer = false
      that.markOpen = true
    },
    getPostilStatus(id){
      const param={
        status:2,
        id:id
      }
      getPostilStatus(param).then(data=>{
        this.$message.success('已关闭')
        this.getPostilLaunch()
      })
    },
    delNode (item) {
      this.deleteItem = item
    },
    confirm (e) {
      const that = this
      if (that.deleteItem) {
        deletePostil(that.deleteItem).then(res=>{

            that.$message.success('删除成功')
            // that.searchOption(that.deleteItem, that.viewData)
            that.getPostilLaunch()

        })
      }
    },
    searchOption (option, arr, type = 'delect', obj = {}) {
      //首先循环arr最外层数据
      for (let s = 0; s < arr.length; s++) {
        //如果匹配到了arr最外层中的我需要修改的数据
        if (arr[s].id === option.id) {
          //判断需要操作的类型
          if (type === 'delect') {
            //删除即删除即可
            arr.splice(s, 1)
          } else {
            //如果是修改，利用Object.assign()组合符合arr数据格式的对象，并进行修改
            this.$set(
              arr,
              s,
              Object.assign(obj, {
                key: obj.pid,
                title: obj.menuName,
                children: option.children,
                scopedSlots: { title: 'custom' },
              })
            )
          }
          break
        } else if (arr[s].children && arr[s].children.length > 0) {
          // 递归条件
          this.searchOption(option, arr[s].children, type, obj)
        } else {
          continue
        }
      }
    },
    Recursion (node) {
      node.scopedSlots = {}
      node.scopedSlots.title = 'custom'
      if (node.viewType == 1) {
        node.slots = {
          icon: 'file',
        }
        node.isLeaf = true
      } else {
        node.slots = {
          icon: 'folder',
        }
      }
      if (!node.children || node.children.length == 0) {
        // arr.push(node.id)
      } else {
        node.children.forEach((item) => this.Recursion(item))
      }
      return node
    },
  },
}
</script>

<style lang="less" scoped>
/deep/ .ant-tabs-nav {
  float: left;
}
.hid {
  white-space: nowrap;
  word-break: break-all; //英文换行，以字母为依据
  overflow: hidden; //隐藏超出的内容
  text-overflow: ellipsis; //超出的内容设置成省略号
  width: 220px;
}
/deep/.view-box {
  .ant-select-selection {
    background: none;
  }
  .ant-select {
    color: #fff;
  }
}
// /deep/.ant-notification-notice-description {
//   overflow-y: hidden;
// }
/deep/.ant-drawer-content,
.ant-drawer-header {
  background: #001529cc;
  .ant-drawer-header {
    background: #001529cc;
    border-bottom: 1px solid #5a738b;
  }
  .ant-drawer-title {
    color: #00ffc5;
  }
  .ant-drawer-close {
    color: #fff;
  }
}
/deep/.ant-tree li {
  height: 160px;
  padding: 0 10px;
}
/deep/.ant-tree li span.ant-tree-switcher.ant-tree-switcher-noop {
  display: none;
}
/deep/.ant-tree li .ant-tree-node-content-wrapper:hover {
  height: 160px;
  width: 250px;
}
/deep/.ant-tree li .ant-tree-node-content-wrapper.ant-tree-node-selected {
  height: 160px;
  width: 250px;
}
.model-tree-box {
  height: 70vh !important;
  /deep/.ant-tree-node-content-wrapper {
    width: 90%;
  }

  /deep/.ant-tree {
    color: #fff;
    height: 68vh;
    overflow: hidden;
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
    li {
      position: relative;
      .node-item {
        position: absolute;
        right: 5px;
        top:-2px;
        span {
          margin: 0 4px;
          color: #05a081;
        }
      }
      .ant-tree-node-content-wrapper.ant-tree-node-selected {
        background-color: #188fff65;
      }
      .ant-tree-node-content-wrapper:hover {
        background-color: #188fff65;
      }
    }
  }
  /deep/.ant-tree-iconEle {
    color: #fff;
  }
  /deep/.ant-table-thead > tr > th{
    color: #fff;
  }
  /deep/.ant-tree-title {
    display: inline-block;
    width: 93%;
    color: #fff;
    .node-box {
      display: flex;
      justify-content: space-between;
      .node-title {
        max-width: 50%;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
    }
  }
  /deep/.ant-divider {
    background-color: rgba(232, 232, 232, 0.5);
  }
  /deep/.ant-pagination-item a{
    color: #fff;
  }
  /deep/.ant-pagination-prev a{
    color: #FFFFFFA5
  }
  /deep/.ant-pagination-next a{
    color: #FFFFFFA5
  }
  /deep/.ant-pagination-item-active {
    background: #726f6f;
  }
}
</style>
