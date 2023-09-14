<template>
  <a-layout id="VersionPage">
    <div class="version-top">
      <div>
        <span>图层选择：</span>
        <span class="version-text">{{ oldVersion }}</span>
        <span class="version-tree">
          <a-tree-select
            style="width: 100%"
            :dropdown-style="{ maxHeight: '400px', overflow: 'auto' }"
            placeholder="请选择楼层"
            :tree-data="treeDataOld"
            :replaceFields="{
              children: 'children',
              label: 'name',
              value: 'glid',
              key: 'glid',
            }"
            @change="LayerOldSelection"
            v-model="layerOldSelectionValue"
          >
            <!-- <template #title="{ value: val, title }">
              <b v-if="val === 'parent 1-1'" style="color: #08c">sss</b>
              <template v-else>{{ title }}</template>
            </template> -->
          </a-tree-select>
        </span>
        <span class="version-text">{{ newVersion }}</span>
        <span class="version-tree">
          <a-tree-select
            style="width: 100%"
            :dropdown-style="{ maxHeight: '400px', overflow: 'auto' }"
            placeholder="请选择楼层"
            :tree-data="treeDataNew"
            :replaceFields="{
              children: 'children',
              label: 'name',
              value: 'glid',
              key: 'glid',
            }"
            @change="LayerNewSelection"
            v-model="layerNewSelectionValue"
          >
            <!-- <template #title="{ value: val, title }">
              <b v-if="val === 'parent 1-1'" style="color: #08c">sss</b>
              <template v-else>{{ title }}</template>
            </template> -->
          </a-tree-select>
        </span>
        <a-button type="primary" @click="StartComparison" :loading="openComparison">开始对比</a-button>
      </div>
      <!-- <div>
        <a-switch v-model="modelLinkage" checked-children="模型联动" un-checked-children="模型联动" />
      </div> -->
      <div>
        <a-button icon='fullscreen' type='link' title='开启全屏' @click='fullScreen' v-show='!fullScreenState'></a-button>
        <a-button icon='fullscreen-exit' type='link' title='关闭全屏' @click='exitFullScreen' v-show='fullScreenState'></a-button>
        <a-button icon='close-circle' type='link' title='关闭' @click='close'></a-button>
      </div>
    </div>
    <a-layout-content>
      <div class="version-list">
        <a-radio-group v-model="versionType" button-style="solid" @change="VersionStateChange">
          <a-radio-button value="new">版本新增</a-radio-button>
          <a-radio-button value="revise">版本修改</a-radio-button>
          <a-radio-button value="delete">版本去除</a-radio-button>
        </a-radio-group>
        <div class="version-table">
          <a-table
            v-if="versionType == 'new'"
            size="middle"
            :bordered="false"
            :columns="columns"
            :data-source="newlyAdded"
            :scroll="{ y: 'calc(100vh - 320px)' }"
            :pagination="false"
            :rowSelection="{
              selectedRowKeys: selectedVersionNewlyKeys,
              onChange: handleVersionNewlyChange,
              columnTitle: '选择',
              hideSelectAll: true,
            }"
            :locale="{ emptyText: versionResultNew }"
          >
            <span slot="newlySerial" slot-scope="text, record, index">{{ index + 1 }}</span>
            <span slot="versionName" slot-scope="text, record" @click="ComponentIdClick(record)">{{ record.id }}</span>
          </a-table>
          <a-table
            v-else-if="versionType == 'revise'"
            size="middle"
            :bordered="false"
            :columns="columns"
            :data-source="modifyAdded"
            :scroll="{ y: 'calc(100vh - 320px)' }"
            :pagination="false"
            :rowSelection="{
              selectedRowKeys: selectedVersionModifyKeys,
              onChange: handleVersionModifyChange,
              columnTitle: '选择',
              hideSelectAll: true,
            }"
            :locale="{ emptyText: versionResultModify }"
          >
            <span slot="newlySerial" slot-scope="text, record, index">{{ index + 1 }}</span>
            <span slot="versionName" slot-scope="text, record" @click="ComponentIdClick(record)">{{ record.id }}</span>
          </a-table>
          <a-table
            v-else-if="versionType == 'delete'"
            size="middle"
            :bordered="false"
            :columns="columns"
            :data-source="deleteAdded"
            :scroll="{ y: 'calc(100vh - 320px)' }"
            :pagination="false"
            :rowSelection="{
              selectedRowKeys: selectedVersionDeleteKeys,
              onChange: handleVersionDeleteChange,
              columnTitle: '选择',
              hideSelectAll: true,
            }"
            :locale="{ emptyText: versionResultDelete }"
          >
            <span slot="newlySerial" slot-scope="text, record, index">{{ index + 1 }}</span>
            <span slot="versionName" slot-scope="text, record" @click="ComponentIdClick(record)">{{ record.id }}</span>
          </a-table>
        </div>
        <div class="save-btn">
          <a-button type="primary" @click="SaveProblem">保存问题</a-button>
        </div>
      </div>
      <div id="cesiumContainer_1">
        <div class="page-version">版本：{{ oldVersion }}</div>
      </div>
      <div id="cesiumContainer_2">
        <div class="page-version">版本：{{ newVersion }}</div>
      </div>
    </a-layout-content>
  </a-layout>
</template>

<script>
import Vue from 'vue'
import { mapGetters } from 'vuex'
import { getPartTreeList, getModelVersion, getModelVersionData, setModelVersionData } from '@/api/model'
export default {
  name: 'Version',
  computed: {
    ...mapGetters(['currProject']),
  },
  props: {
    versionId: {
      type: String,
    },
  },
  data() {
    return {
      sapi_1:'',
      sapi_2:'',
      projectMessage: {},
      versionType: 'new',
      newlyAdded: [],
      modifyAdded: [],
      deleteAdded: [],
      pagination: {
        current: 0,
        total: 0,
        skipCount: 0,
        pageSize: 10, //每页中显示10条数据
        showTotal: (total) => `共 ${total} 条`, //分页中显示总的数据
      },
      columns: [
        {
          title: '序号',
          align: 'center',
          scopedSlots: { customRender: 'newlySerial' },
        },
        {
          title: '构件ID',
          dataIndex: 'name',
          key: 'name',
          align: 'center',
          scopedSlots: { customRender: 'versionName' },
        },
      ],
      selectedVersionNewlyKeys: [],
      selectedVersionNewlyLists: [],
      selectedVersionModifyKeys: [],
      selectedVersionModifyLists: [],
      selectedVersionDeleteKeys: [],
      selectedVersionDeleteLists: [],
      treeDataNew: [],
      treeDataOld: [],
      oldVersion: '',
      newVersion: '',
      modelLinkage: true,
      versionList: [],
      layerNewSelectionValue: '',
      layerOldSelectionValue: '',
      listHeight: 'calc(100vh - 320px)',
      lastFeatureId: '',
      openComparison: false,
      fullScreenState: false,
      versionResultNew: this.$t('public.noData'),
      versionResultDelete: this.$t('public.noData'),
      versionResultModify: this.$t('public.noData'),
    }
  },

  async mounted() {


    const that = this
    window.onresize=function() {
      if (!that.checkFull()) {
        that.fullScreenState=false
      }
    }
    await getModelVersion(that.versionId).then((res) => {
      that.versionList = [res.sourceDocVer, res.destinationDocVer]
      that.GetTreeFloor(that.versionList[0].modelName, ['0'], 'old')
      that.GetTreeFloor(that.versionList[1].modelName, ['0'], 'new')
    })
    that.$nextTick(() => {
      const defaults = { ...this.$store.state.bim.defaults }
      defaults.container = 'cesiumContainer_1'
      defaults.bgcolor = '#c6c9d1'
      this.sapi_1 = new SAPI(defaults, () => {
        defaults.container = 'cesiumContainer_2'
        this.sapi_2 = new SAPI(defaults, () => {
          for (let i = 0; i < that.versionList.length; i++) {
            i == 0 ? (that.oldVersion = that.versionList[i].versionNo) : (that.newVersion = that.versionList[i].versionNo)
            that.AddModel(
              {
                name: that.versionList[i].modelName,
                id: that.versionList[i].id,
              },
              i == 0 ? 'old' : 'new'
            ) //初始加载模型
          }
        })
      })

    })
  },
  methods: {
    checkFull() {

      var fullScreen = parent.document.webkitIsFullScreen || parent.document.fullscreen || parent.document.mozFullScreen || parent.document.msFullscreenElement;

      if (fullScreen === undefined) {
        fullScreen = false;
      }

      return fullScreen;
    },
    GetTreeFloor(modelName, data, type) {
      getPartTreeList({ modelName: modelName }, data).then((res) => {
        if (data[0] == '0') {
          var gilds = []
          for (let i = 0; i < res.length; i++) {
            gilds.push(res[i].glid)
          }
          this.GetTreeFloor(modelName, gilds, type)
        } else {
          type == 'old' ? (this.treeDataOld = res) : (this.treeDataNew = res)
        }
      })
    },
    AddModel(data, type) {
      //加载模型
      const that = this
      const url = `${that.$store.state.modelFile}/Tools/output/model/${data.name}/tileset.json`
      // if (data.docType === 4) {
      //   url = `${that.$store.getters.modelUrl}/tools/output/model/${data.name}/tileset.json`
      // }
      if (type == 'old') {
        this.sapi_1.Model.add(
          url,
          data.id,
          () => {
            console.log('模型加载成功了')
          },
          {
            FlyTo: true, //场地模型设置true，其他模型均设置为false
          }
        )
      } else {
        this.sapi_2.Model.add(
          url,
          data.id,
          () => {
            console.log('模型加载成功了')
          },
          {
            FlyTo: true, //场地模型设置true，其他模型均设置为false
          }
        )
      }
    },
    handleVersionNewlyChange(selectedRowKeys, selectedRows) {
      this.selectedVersionNewlyKeys = selectedRowKeys
      this.selectedVersionNewlyLists = selectedRows
    },
    handleVersionModifyChange(selectedRowKeys, selectedRows) {
      this.selectedVersionModifyKeys = selectedRowKeys
      this.selectedVersionModifyLists = selectedRows
    },
    handleVersionDeleteChange(selectedRowKeys, selectedRows) {
      this.selectedVersionDeleteKeys = selectedRowKeys
      this.selectedVersionDeleteLists = selectedRows
    },
    VersionStateChange(e) {
      this.clearPage()
    },
    ComponentIdClick(val) {
      var that = this
      that.lastFeatureId = val.id
      // let index_1 = that.sapi_1.Feature.zoomTo(
      //   that.versionList[0].id + '^' + val.id,
      //   that.versionList[0].id,
      //   true, 1, null, 0, 45 )
      // let index_2 = that.sapi_2.Feature.zoomTo(
      //   that.versionList[1].id + '^' + val.id,
      //   that.versionList[1].id,
      //   true, 1, null, 0, 45 )
      that.sapi_1.Feature.zoomTo({
        featureId: that.versionList[0].id + '^' + val.id,
        distanceRatio: 1,
        flyHeading:90,
        flyPitch:-45})
      // that.sapi_1.Feature.setVisible(val.id,true,that.versionList[0].id,false);
      that.sapi_2.Feature.zoomTo({
        featureId: that.versionList[1].id + '^' + val.id,
        distanceRatio: 1,
        flyHeading:90,
        flyPitch:-45})
      // that.sapi_2.Feature.setVisible(val.id,true,that.versionList[1].id,false);


      // setTimeout(() => {
      //   if (that.modelLinkage) {
      //     if (index_1 == -1 && index_2 != -1) {
      //       let visualAngle = that.sapi_2.Camera.getViewPort()
      //       that.sapi_1.Camera.setViewPort(visualAngle.position, visualAngle.heading, visualAngle.pitch)
      //     } else if (index_2 == -1 && index_1 != -1) {
      //       let visualAngle = that.sapi_1.Camera.getViewPort()
      //       that.sapi_2.Camera.setViewPort(visualAngle.position, visualAngle.heading, visualAngle.pitch)
      //     }
      //   }
      // }, 500)
    },
    LayerOldSelection(value, label, extra) {
      this.layerNewSelectionValue = ''
      if (extra.triggerNode.dataRef.level == 1) {
        let item = this.getChidlren(
          extra.triggerNode.$parent._props.dataRef.name,
          this.treeDataNew,
          extra.triggerNode.dataRef.name
        )
        if (item) {
          this.layerNewSelectionValue = item.glid
        }
      } else {
        for (let i = 0; i < this.treeDataNew.length; i++) {
          if (this.treeDataNew[i].name == extra.triggerNode.dataRef.name) {
            this.layerNewSelectionValue = this.treeDataNew[i].glid
          }
        }
      }
    },
    LayerNewSelection(value, label, extra) {
      this.layerOldSelectionValue = ''
      if (extra.triggerNode.dataRef.level == 1) {
        let item = this.getChidlren(
          extra.triggerNode.$parent._props.dataRef.name,
          this.treeDataOld,
          extra.triggerNode.dataRef.name
        )
        if (item) {
          this.layerOldSelectionValue = item.glid
        }
      } else {
        for (let i = 0; i < this.treeDataOld.length; i++) {
          if (this.treeDataOld[i].name == extra.triggerNode.dataRef.name) {
            this.layerOldSelectionValue = this.treeDataOld[i].glid
          }
        }
      }
    },
    StartComparison() {
      var that = this
      if (that.layerNewSelectionValue == '' && that.layerOldSelectionValue == '') {
        that.$message.warning('请先选择楼层数据再进行对比！')
        return false
      }
      that.versionResultNew = that.$t('public.noData')
      that.versionResultDelete = that.$t('public.noData')
      that.versionResultModify = that.$t('public.noData')
      that.openComparison = true
      that.sapi_1.Feature.setVisible(that.versionList[0].id + '^' + that.lastFeatureId, true)
      // window.$api_1.Public.clearBOX23line()
      that.sapi_2.Feature.setVisible(that.versionList[1].id + '^' + that.lastFeatureId, true)
      // window.$api_2.Public.clearBOX23line()
      that.lastFeatureId = ''
      that.versionType = 'new'
      that.clearPage()
      getModelVersionData(that.versionId, {
        glid: that.layerOldSelectionValue,
        newglid: that.layerNewSelectionValue,
      }).then((res) => {
        that.openComparison = false
        that.PartModel('old', that.versionList[0], res.sourceMetadata.externalIdList.join('#'))
        that.PartModel('new', that.versionList[1], res.destinationMetadata.externalIdList.join('#'))
        for (let i = 0; i < res.addMetadata.externalIdList.length; i++) {
          res.addMetadata.externalIdList[i] = { id: res.addMetadata.externalIdList[i] }
        }
        that.newlyAdded = res.addMetadata.externalIdList
        that.newlyAdded.length == 0
          ? (that.versionResultNew = '此次对比暂无新增内容')
          : (that.versionResultNew = that.$t('public.noData'))
        for (let i = 0; i < res.updateMetadata.externalIdList.length; i++) {
          res.updateMetadata.externalIdList[i] = { id: res.updateMetadata.externalIdList[i] }
        }
        that.modifyAdded = res.updateMetadata.externalIdList
        that.modifyAdded.length == 0
          ? (that.versionResultModify = '此次对比暂无修改内容')
          : (that.versionResultModify = that.$t('public.noData'))
        for (let i = 0; i < res.deleteMetadata.externalIdList.length; i++) {
          res.deleteMetadata.externalIdList[i] = { id: res.deleteMetadata.externalIdList[i] }
        }
        that.deleteAdded = res.deleteMetadata.externalIdList
        that.deleteAdded.length == 0
          ? (that.versionResultDelete = '此次对比暂无去除内容')
          : (that.versionResultDelete = that.$t('public.noData'))
      })
    },
    PartModel(type, item, data) {
      const that = this
      // var url = `${that.$store.getters.modelUrl}/tools/output/model/${item.name}/root.glt`
      // if (data.docType === 4) {
      //   url = `${that.$store.getters.modelUrl}/tools/output/model/${item.name}/tileset.json`
      // }
      if (type == 'old') {
        // that.sapi_1.Feature.setVisible(data,true,item.id,false);
        // that.sapi_1.Model.addPart(url, item.id, data)
        if (that.modelLinkage) {
          that.sapi_1.Model.location(item.id)
        }
      } else {
        // that.sapi_2.Feature.setVisible(data,true,item.id,false);
        // that.sapi_2.Model.addPart(url, item.id, data)
        if (that.modelLinkage) {
          that.sapi_2.Model.location(item.id)
        }
      }
    },
    getChidlren(parentname, data, name) {
      var hasFound = false, // 表示是否有找到id值
        result = null
      var fn = function (data) {
        if (Array.isArray(data) && !hasFound) {
          // 判断是否是数组并且没有的情况下，
          data.forEach((item) => {
            if (item.name === parentname) {
              // 数据循环每个子项，并且判断子项下边是否有id值
              for (let i = 0; i < item.children.length; i++) {
                if (item.children[i].name == name) {
                  result = item.children[i] // 返回的结果等于每一项
                  hasFound = true // 并且找到id值
                }
              }
            } else if (item.children) {
              fn(item.children) // 递归调用下边的子项
            }
          })
        }
      }
      fn(data) // 调用一下
      return result
    },
    SaveProblem() {
      const that = this
      that.$confirm({
        title: '确认修改?',
        content:
          '提交后' +
          (that.versionType == 'new' ? '版本新增' : that.versionType == 'revise' ? '版本修改' : '版本去除') +
          '内容将发生改变',
        onOk() {
          let type = that.versionType == 'new' ? 1 : that.versionType == 'revise' ? 2 : 3
          let list =
            that.versionType == 'new'
              ? that.selectedVersionNewlyLists
              : that.versionType == 'revise'
              ? that.selectedVersionModifyLists
              : that.selectedVersionDeleteLists
          let externalIdList = []
          list.forEach((item) => {
            externalIdList.push(item.id)
          })
          setModelVersionData(
            that.versionId,
            {
              method: type,
            },
            {
              guid: that.layerOldSelectionValue + '#' + that.layerNewSelectionValue,
              externalIdList: externalIdList,
            }
          ).then((res) => {
            if (res) {
              that.versionType == 'new'
                ? (that.newlyAdded = that.selectedVersionNewlyLists)
                : that.versionType == 'revise'
                ? (that.modifyAdded = that.selectedVersionModifyLists)
                : (that.deleteAdded = that.selectedVersionDeleteLists)
              that.selectedVersionNewlyKeys = []
              that.selectedVersionModifyKeys = []
              that.selectedVersionDeleteKeys = []
            }
          })
        },
        onCancel() {},
        class: 'test',
      })
    },
    close(){
      this.$emit('close')
    },
    fullScreen() {
      // documentElement 属性以一个元素对象返回一个文档的文档元素
      this.fullScreenState=true
      const el = document.getElementById('VersionPage')
      el.requestFullscreen||el.mozRequestFullScreen||el.webkitRequestFullscreen||el.msRequestFullScreen?
        el.requestFullscreen()||el.mozRequestFullScreen()|| el.webkitRequestFullscreen()||el.msRequestFullscreen():null
    },
    exitFullScreen() {
      this.fullScreenState=false
      let el = document // 此时获取的元素是 document
      let cfs = el.cancelFullScreen
        || el.webkitCancelFullScreen
        || el.mozCancelFullScreen
        || el.exitFullScreen
      if (typeof cfs != "undefined" && cfs) {
        cfs.call(el)
        return
      }
      if (typeof window.ActiveXObject != "undefined") {
        let wscript = new ActiveXObject("WScript.Shell")
        if (wscript != null) {
          wscript.SendKeys("{F11}")
        }
      }
    },
    clearPage() {
      this.selectedVersionNewlyKeys = []
      this.selectedVersionModifyKeys = []
      this.selectedVersionDeleteKeys = []
      this.selectedVersionNewlyLists = []
      this.selectedVersionModifyLists = []
      this.selectedVersionModifyLists = []
    },
  },
}
</script>

<style lang="less" scoped>
#VersionPage {
  position: relative;
  height: calc(100vh - 130px);
  > .ant-layout-content {
    display: flex;
    #cesiumContainer_1 {
      border-right: 1px dashed #ffffff;
      position: relative;
    }
    #cesiumContainer_2 {
      position: relative;
    }
    .page-version {
      position: absolute;
      top: 6px;
      left: 10px;
      z-index: 99;
      color: rgba(0, 0, 0, 0.65);
    }
    .version-list {
      width: 220px;
      height: 100%;
      .ant-radio-group {
        width: 100%;
        text-align: center;
        margin: 10px 0;
        .ant-radio-button-wrapper {
          padding: 0 6px;
          height: 28px;
          line-height: 26px;
        }
        .ant-radio-button-wrapper:first-child {
          border-top-left-radius: 15px;
          border-bottom-left-radius: 15px;
        }
        .ant-radio-button-wrapper:last-child {
          border-top-right-radius: 15px;
          border-bottom-right-radius: 15px;
        }
      }
      .save-btn {
        text-align: center;
        margin: 10px;
      }
      .version-table {
        /deep/.ant-table-middle > .ant-table-content > .ant-table-body > table > .ant-table-thead > tr > th {
          padding: 8px;
        }
        /deep/.ant-table-row {
          cursor: pointer;
        }
        /deep/.ant-table-placeholder {
          padding: 30px 16px;
        }
      }
    }
    > div:not(:first-child) {
      width: calc((100% - 220px) / 2);
    }
  }
  .version-top {
    display: flex;
    justify-content: space-between;
    padding: 5px 10px;
    align-items: center;
    border-bottom: 1px solid #bbbdbf;
    > div:first-child {
      display: flex;
      align-items: center;
      .version-text {
        margin: 0 6px;
      }
      .version-tree {
        width: 250px;
      }
      button {
        margin: 0 10px;
      }
    }
  }
  // 滚动条背景颜色
  /deep/.ant-table-body::-webkit-scrollbar,
  .info::-webkit-scrollbar {
    height: 9px;
    width: 3px;
    background-color: #f0f2f5;
  }

  // 滚动条颜色
  /deep/.ant-table-body::-webkit-scrollbar-thumb,
  .info::-webkit-scrollbar-thumb {
    border-radius: 9px;
    background-color: #05a081;
  }
}
</style>
