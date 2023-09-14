<template>
  <a-spin :spinning="spinning">
    <a-layout id="ClampPage" style="height:100vh">
      <a-layout-content>
        <div id="viewer2d"></div>
        <div id="cesiumContainer"></div>
        <a-card :bordered="false" class="left-tree" style='left: 24px;'>
          <a-layout-sider class="model-tree-box scroll-box">
            <a-tree :defaultExpandAll="true" :checkable="true" class="model-tree" :blockNode="true"
              :tree-data="modelTrees" v-model="checkedKeys" @check="onCheckTree" :autoExpandParent="true">
              <template v-slot:title="modelTrees">
                <a-tooltip :title="modelTrees.title">
                  <span @click="LocationModel(modelTrees.isLeaf ? modelTrees : null)">{{ modelTrees.title }}</span>
                </a-tooltip>
                <a-button-group style="float: right" v-if="modelTrees.isLeaf">
                  <a-icon type="drag" @click="ManualCorreection(modelTrees)" /> 
                  <a-icon type="delete" @click="deleteNode(modelTrees)" />
                </a-button-group>
              </template>
            </a-tree>
          </a-layout-sider>
        </a-card>
      </a-layout-content>
      <div class="footer-btn">
        <div class="btn-tool" @click="settingVisible = true">
          <a-tooltip title="场景设置">
            <img src="../../assets/img/bim/navBtn/Set.png" />
          </a-tooltip>
        </div>
        <div class="btn-tool" v-if="isFlag" @click="CreatFlatten">
          <a-tooltip placement="rightTop" title="模型压平">
            <img src="../../assets/img/bim/navBtn/DrawSeal_1.png" />
          </a-tooltip>
        </div>
      </div>
      <clamping-page :projectMessage="projectMessage" :movementPosition="movementPosition"
        :folderIdClamping="folderIdClamping" @closePage="closePage" :mapValue="mapValue"
        :historyFlattenList="historyFlattenList" @GetSaveMessage="GetSaveMessage"></clamping-page>
      <setting-interface :settingVisible.sync="settingVisible" :projectMessage="projectMessage"
        v-if="settingVisible"></setting-interface>
    </a-layout>
  </a-spin>
</template>

<script>
import { eventBus } from '@/utils/bus'
import ManualCorreection from './modules/ManualCorreection'
import ClampingPage from './modules/ClampingPage'
import storage from 'store'
import ModelCompression from './modules/ModelCompression.vue'
import SettingInterface from './modules/SettingInterface'
export default {
  name: 'Clamp',
  components: {
    ManualCorreection,
    ClampingPage,
    ModelCompression,
    SettingInterface
  },
  data() {
    return {
      modelList: [],
      modelTrees: [
        {
          title: '合模列表',
          key: 'clamp',
          children: [
            {
              title: 'GIS模型',
              key: 'gis',
              children: [],
            },
            {
              title: 'BIM模型',
              key: 'bim',
              children: [],
            },
          ],
        },
      ],
      checkedKeys: [],
      expandedKeys: [],
      gisModelList: [],
      bimModelList: [],
      movementPosition: [],
      historyFlattenList: [],
      openImageSelection: false,
      mapValue: {
        state: false,
        type: '',
      },
      lastManualValue: 0,
      index: 0,
      time: 0,
      lastManualArray: [],
      // offset: [0, 0, 0],
      // rotate: [0, 0, 0],
      modelArr: [],
      spinning: false,
      clampingList: [],
      projectMessage: {
        projectId: '',
        id: '',
        sceneType: 0,
      },
      isFlag: false,
      clampModelVisible: false,
      folderIdClamping: '',
      settingVisible: false
    }
  },
  mounted() {
    const clamping = storage.get('clampingInfo')
    const { clampingList, projectMessage, clampModelVisible, folderIdClamping } = clamping ? JSON.parse(clamping) : ''
    this.clampingList = clampingList
    this.projectMessage = projectMessage
    this.clampModelVisible = clampModelVisible
    this.folderIdClamping = folderIdClamping
    // 判断传过来的模型有没有osgb格式的，有就显示模型压平操作按钮
    if (this.clampingList.length > 0) {
      this.clampingList.some(model => {
        if (model.modelFormat == 'osgb') {
          this.isFlag = true
          return true
        }
        return false
      })
    }
    this.InitEngine()
    // 获取压平列表
    eventBus.$on('giveFlattenList', res => {
      this.historyFlattenList = res
    })
  },
  methods: {
    InitEngine() {
      const defaults = { ...this.$store.state.bim.defaults }
      api = new SAPI(defaults, () => {
        console.log("初始化成功")
        this.clampInit()
      })
    },
    clampInit() {
      const that = this
      that.modelList = []
      const gisList = []
      const bimList = []
      this.clampingList.forEach((item) => {
        item.children = []
        if (item.documentType == 2) {
          item.isLeaf = true
          item.title = item.documentName
          item.key = item.id
          // item.matrix = item.matrix ? JSON.parse(item.matrix) : { rotate: [0, 0, 0], offset: [0, 0, 0] }
          item.matrix = { rotate: [0, 0, 0], offset: [0, 0, 0] }
          gisList.push(item)
        } else if (item.documentType == 1) {
          item.isLeaf = true
          item.title = item.documentName
          item.key = item.id
          // item.matrix = item.matrix ? JSON.parse(item.matrix) : { rotate: [0, 0, 0], offset: [0, 0, 0] }
          item.matrix = { rotate: [0, 0, 0], offset: [0, 0, 0] }
          bimList.push(item)
        }
      })
      that.movementPosition = JSON.parse(JSON.stringify(this.clampingList))
      that.modelTrees = [
        {
          title: '合模列表',
          key: 'clamp',
          isLeaf: false,
          children: [],
        },
      ]
      gisList.length > 0
        ? that.modelTrees[0].children.push({
          title: 'GIS模型',
          key: 'gis',
          isLeaf: false,
          children: gisList,
        })
        : null
      bimList.length > 0
        ? that.modelTrees[0].children.push({
          title: 'BIM模型',
          key: 'bim',
          isLeaf: false,
          children: bimList,
        })
        : null
    },
    AddModelNew() {
      const that = this
      if (!that.modelArr[that.index]) {
        that.spinning = false
        that.index = 0
        that.modelArr = []
        return
      }
      const data = that.modelArr[that.index]
      var url = `${that.$store.state.modelUrl}/Tools/output/model/${data.modelName}/root.glt`
      if (data.documentType == 2) {
        url = `${that.$store.getters.modelUrl}/tools/output/model/${data.modelName}/tileset.json`
      }
      api.Model.add(
        url,
        data.id,
        () => {
          // data.matrix && data.matrix.offset ? api.Model.offset(data.matrix.offset[0], data.matrix.offset[1], data.matrix.offset[2], data.id) : null;
          // data.matrix && data.matrix.rotate ? api.Model.rotate(data.matrix.rotate[0], data.matrix.rotate[1], data.matrix.rotate[2], data.id) : null;
        },
        () => {
          data.isvisible = true
          if (data.documentType == 2) {
            that.gisModelList.push(data)
          } else if (data.documentType == 1) {
            that.bimModelList.push(data)
          }
          that.index += 1
          setTimeout(function () {
            that.AddModelNew()
          }, 500)

        },
        {
          FlyTo: true, //场地模型设置true，其他模型均设置为false
        }
      )
    },
    onExpand(expandedKeys, e) {
      this.expandedKeys = expandedKeys
    },
    onCheckTree(checkedKeys, e) {
      const checkNode = e.node.dataRef
      const checked = e.checked
      const that = this
      that.time = 0
      if (!checkNode.isLeaf) {
        if (checkNode.children.length > 0) {
          that.FilterModelNodes(checkNode.children, checked)
        } else {
          that.FilterModelNodes(checkNode.children, checked)
        }
      } else {
        that.changeClampingList(checkNode, checked)

      }
    },
    FilterModelNodes(data, checked) {
      const that = this
      for (let i = 0; i < data.length; i++) {
        if (data[i].isLeaf) {
          that.changeClampingList(data[i], checked)
        }
        if (data[i].children) {
          that.FilterModelNodes(data[i].children, checked)
        }
      }
    },
    changeClampingList(data, checked) {
      const that = this
      let index = -1
      if (data.documentType == 2) {
        index = that.findItem(that.gisModelList, 'key', data.key)
      } else {
        index = that.findItem(that.bimModelList, 'key', data.key)
      }
      if (checked) {
        if (index == -1) {
          that.modelArr.push(data)
          that.projectMessage.modelList ? that.projectMessage.modelList.push(data) : that.projectMessage.modelList = [data];
          clearTimeout(that.time)
          that.time = setTimeout(function () {
            that.spinning = true
            that.AddModelNew()
          }, 1000)
        } else {
          if (data.documentType == 2) {
            that.gisModelList[index].isvisible = true
          } else if (data.documentType == 1) {
            that.bimModelList[index].isvisible = true
          }
          api.Model.setVisible(data.key, true)
        }
      } else {
        if (index != -1) {
          if (data.documentType == 2) {
            that.gisModelList[index].isvisible = false
          } else if (data.documentType == 1) {
            that.bimModelList[index].isvisible = false
          }
          api.Model.setVisible(data.key, false)
        }
      }
    },
    findItem(arr, key, val) {
      for (let i = 0; i < arr.length; i++) {
        if (arr[i][key] == val) {
          return i
        }
      }
      return -1
    },
    deleteNode(e) {
      const that = this
      const treeData = that.modelTrees
      that.modelTrees = that.deleteParentNode(treeData, e.key)
      const delIndex = that.findItem(that.movementPosition, 'key', e.key)
      delIndex != -1 ? that.movementPosition.splice(delIndex, 1) : null
      let index = -1
      if (e.documentType == 2) {
        index = that.findItem(that.gisModelList, 'key', e.key)
        index != -1 ? that.gisModelList.splice(index, 1) : null
      } else if (e.documentType == 1) {
        index = that.findItem(that.bimModelList, 'key', e.key)
        index != -1 ? that.bimModelList.splice(index, 1) : null
      }
      index != -1 ? api.Model.remove(e.key) : null
    },
    LocationModel(e) {   //树定位模型
      if (e && api.m_model.get(e.key)) {
        api.Model.location(e.key);
      }
    },
    has(obj, key) {
      if (obj.key !== key) {
        return obj
      }
    },
    deleteParentNode(data, key) {
      const ret = []
      data.forEach((ele) => {
        if (this.has(ele, key)) {
          if (ele.children) {
            ele.children = this.deleteParentNode(ele.children, key)
          }
          ret.push(ele)
        }
      })
      return ret
    },
    ManualCorreection(val) {
      //手工校正
      const that = this
      let findModel = {}
      if (val.documentType == 2) {
        findModel = that.gisModelList.find(e => e.id == val.id)
      } else if (val.documentType == 1) {
        findModel = that.bimModelList.find(e => e.id == val.id)
      }
      if (findModel && findModel.isvisible) {
        that.ClearEvent()
        setTimeout(() => {
          const modelList = [val.dataRef]
          that.$notification.open({
            key: 'EngineKey',
            message: `手工校正`,
            description: (
              <manual-correection
                projectMessage={that.projectMessage}
                modelList={modelList}
                type={'correcting'}
                SaveManualValue={that.SaveManualCorreection}
                lastManualValue={that.lastManualValue}
                lastManualArray={that.lastManualArray}
                changeArr={that.changeArr}
                changeRotate={that.changeRotate}
              />
            ),
            class: 'combine-notification right-combine-notification',
            top: '10vh',
            right: '110px',
            duration: null,
            placement: 'topRight',
            onClose: () => { },
          })
        })
      } else {
        that.$message.warning('请先加载对应模型')
      }
    },
    SaveManualCorreection(val) {
      const that = this
      if (val.type == 'position') {
        that.lastManualValue = val.lastValue
      } else {
        that.lastManualArray = val.lastArray
      }
    },
    changeArr(val, id) {
      // this.offset = val
      // const m = { rotate: this.rotate, offset: val }
      this.clampingList.find(e => e.key == id).matrix.offset = val
      this.movementPosition.find(e => e.key == id).matrix.offset = val
    },
    changeRotate(val, id) {
      // this.rotate = val
      // const m = { rotate: val, offset: this.offset }
      this.clampingList.find(e => e.key == id).matrix.rotate = val
      this.movementPosition.find(e => e.key == id).matrix.rotate = val
    },
    GetSaveMessage(res) {
      const that = this
      if (res.type == 'flatten') {
        that.historyFlattenList = res.data
      }
      if (!that.openImageSelection) {
        that.SaveModelMatrix('bim')
      }
      if (res.type == 'map') {
        that.mapValue = that.$refs.map.mapSelect
      }
    },
    SaveModelMatrix(type) {
      const that = this
      for (let i = 0; i < that.movementPosition.length; i++) {
        if (api.m_model.get(that.movementPosition[i].key)) {
          const matrix = []
          if (type == 'bim') {
            that.movementPosition[i].matrix = that.movementPosition[i].matrix
          } else {
            that.movementPosition[i].matrixGis = that.movementPosition[i].matrix
            that.$message.success('保存成功')
          }
        }
      }
    },
    ClearEvent() {
      const that = this
      that.$notification.close('EngineKey')
    },
    closePage() {
      this.$emit('update:clampModelVisible', false)
    },
    CreatFlatten() {   //创建压平
      this.ClearEvent()
      const _this = this
      this.$notification.open({
        key: 'EngineKey',
        message: `模型压平`,
        description: <ModelCompression modelArrCopy={_this.clampingList} />,
        class: 'combine-notification',
        duration: null,
        placement: 'topRight',
        onClose: () => {
          _this.ClearEvent()
          api.Public.clearHandler('RIGHT_CLICK')
        },
      })
    },
    ClearEvent() {
      const that = this;
      that.$notification.close('EngineKey')
      that.$notification.close('measurement')
      api.Feature.getByEvent(false)  //关闭构件拾取事件
      api.Measurement.angle(false)
      api.Measurement.distance(false)
      api.Measurement.clearAllTrace()
      api.Public.pickupCoordinate(false)
    },
  },
  destroyed() {
    this.ClearEvent()
    this.$notification.destroy()
  },
}

</script>

<style lang="less">
.ant-notification-topRight {
  top: 15vh !important;
}
</style>
<style lang="less" scoped>
@import './style.less';
/deep/.scroll-box {
  margin-top: 15px;
  height: 76vh;
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

  .ant-tree-node-content-wrapper {
    color: #ffffff;
  }

  .ant-list-empty-text {
    padding: 5px;
    text-align: left;
  }
}

/deep/ .ant-tooltip {
  position: fixed !important;
  bottom: 46px !important;
  top: unset !important;
}

/deep/ .ant-tree-title {
  color: #fff;
}

/deep/.ant-tree-switcher {
  position: relative;
  color: #fff;
}

.toolsBox {
  position: absolute;
  left: 0;
  right: 0;
  top: -100px;
}

.footer {
  width: 100vw;
  height: 40px;
  position: absolute;
  overflow: auto;
  bottom: 10px;
  display: flex;
  justify-content: center;
}

.model {
  position: absolute;
  top: -100px;
  right: 0;
  //bottom: 0;
  left: 0;

  .modelContent {
    z-index: 2;
    padding: 10px;
    width: 300px;
    background: #0d1a29cc;
    box-shadow: 10px -10px 40px #14263c78;
    //height: 500px;
    top: 80vh;
    position: relative;
    background-clip: padding-box;
    margin: 0 auto;
    border: 0;
    border-radius: 2px;
    pointer-events: auto;

    .textLine {
      display: flex;
      flex-wrap: wrap;
      justify-content: space-between;

      /deep/ .ant-input {
        background: transparent;
        color: #eee;
      }

      /deep/ .ant-input-group-addon {
        background: transparent;
        color: #eee;
      }

      /deep/ .ant-slider {
        margin: 5px 5px;
      }

      div {
        width: 50%;
        margin-bottom: 10px;
        font-weight: 900;
        display: flex;
        color: #FFF;

        span {
          color: #FFFFFF99;
          display: block;
          /* font-size: 14px; */
          font-weight: 900;
          width: 100px;
          text-align: right;
        }
      }
    }

    .text-qr {
      text-align: right;
    }

    .close {
      float: right;
      /* background: aliceblue; */
      border-radius: 16px;
      width: 20px;
      height: 20px;
      font-size: 24px;
      margin: -20px;
      line-height: 20px;
      color: aliceblue;
    }
  }
}
</style>
<style lang="less">
.ant-notification {
  width: 300px;

  .combine-notification {
    background-color: rgba(50, 54, 65, 0.88);
    color: #fff;
    margin-bottom: 0;

    .ant-notification-notice-message {
      color: #fff;
    }

    .ant-notification-notice-close {
      color: #fff;
    }

    .ant-card,
    .ant-input-number,
    .ant-input {
      background: transparent;
      color: #fff;
    }

    .ant-form-item {

      //margin-bottom: 6px;
      .ant-form-item-label>label {
        color: #fff;
      }
    }

    .ant-tabs-nav-wrap {
      text-align: center;

      .ant-tabs-tab {
        color: #fff;
      }

      .ant-tabs-tab-active.ant-tabs-tab {
        color: #05a081;
      }
    }

    .ant-tabs {
      color: #ffffff;
    }

    .ant-list-header {
      color: #ffffff;
    }

    .ant-list-item {
      color: #fff;
      border-bottom: 1px dashed rgba(185, 185, 185, 0.5);

      a {
        color: #fff;
      }

      a.active {
        color: #e4d904;
      }

      span {
        color: #b6c9da;
      }
    }

    .ant-list-item:last-child {
      border-bottom: none;
    }

    .ant-list-item.edit {
      padding-top: 10px;
      padding-bottom: 10px;
    }

    .ant-list-empty-text {
      color: #fff;
    }

    .ant-list-footer {
      padding-bottom: 0;
    }

    /*表格*/
    .ant-table {
      .ant-table-tbody>tr:hover:not(.ant-table-expanded-row):not(.ant-table-row-selected)>td {
        background: #285681;
      }

      .ant-table-thead>tr>th {
        padding: 8px;
      }

      .ant-table-tbody>tr>td {
        padding: 6px;
        color: #fff;
      }
    }

    /**专业结构、楼层结构tree*/
    .model-tree.ant-tree {
      color: #fff;

      li .ant-tree-node-content-wrapper {
        color: #fff;
      }

      .ant-tree-checkbox-inner {
        background: transparent;
      }

      >li {
        >.ant-tree-checkbox {
          //display: none;
        }
      }
    }

    .ant-card-body {
      padding: 10px;
    }
  }
}

.footer-btn {
  position: absolute;
  bottom: 10px;
  padding: 0;
  left: 50%;
  background-color: rgba(50, 54, 65, 0.92);
  border-radius: 3px;
  z-index: 1;
  display: flex;
  transform: translate(-50%, 0);

  .btn-tool {
    text-align: center;
    margin: 0;
    padding: 8px 10px;
    cursor: pointer;

    img {
      width: 22px;
      height: 22px;
    }
  }

  .btn-tool:hover {
    background-color: rgba(5, 160, 129, 0.6);
    border-radius: 3px;

    >.iconfont {
      color: rgba(5, 160, 129, 0.6);
    }
  }
}
</style>
