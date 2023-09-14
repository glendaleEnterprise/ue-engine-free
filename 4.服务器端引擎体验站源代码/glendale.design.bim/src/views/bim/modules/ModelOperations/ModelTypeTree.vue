<template>
  <div class="viewpoint-list scroll-box">
    <a-tree checkable class="model-tree" :tree-data="modelTrees" :load-data="onLoadData" :replace-fields="replaceFields"
      @check="onCheckTree" v-model="checkedKeys" :selectedKeys="selectedKeys" @select="onSelect">
      <span slot="title" slot-scope="text, record">
        {{text.name}}
        <a-icon type="loading" v-show="text.disableCheckbox ? text.disableCheckbox : false" />
      </span>
    </a-tree>
  </div>
</template>
<script>
  import {
    getModelTypeTree,
    getModelTypeTreeIdList
  } from '@/api/model'
  import store from '@/store'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    props: {
      projectMessage: {
        type: Object,
        default: undefined,
      },
    },
    data() {
      return {
        modelTrees: [],
        replaceFields: {
          title: 'name',
          key: 'glid',
          value: 'glid',
        },
        visibleList: [], //结构树显示构件列表
        checkedKeys: [], //用于清除选中状态
        selectedKeys: [],
        selectedFeature: undefined,
        requestedData: [], //已经请求过得不在请求
        firstClick: true,
        isMobile: false
      }
    },
    mounted() {
      const that = this
      that.isMobile = _isMobile() ? false : true;
      const date = that.projectMessage.modelList
      for (let i = 0; i < date.length; i++) {
        that.modelTrees.push({
          name: date[i].documentName,
          id: date[i].id,
          children: [],
          glid: date[i].id,
          key: date[i].id,
          modelName: date[i].modelName,
          level: 0,
          modelId: date[i].id,
        })
      }
      if (that.isMobile) {
        api.Public.event("RIGHT_CLICK", function (e) {
          //执行事件
          that.clearScene()
        });
        that.$notification.open({
          key: 'EngineKey',
          description: '右键恢复模型',
          class: 'original-notification tips-notification',
          duration: null,
          placement: 'bottomRight',
        })
      }
    },
    methods: {
      onLoadData(treeNode) {
        const that = this;
        return new Promise((resolve) => {
          const level = treeNode.dataRef.glid == treeNode.dataRef.modelId ? 0 : treeNode.dataRef.level + 1
          getModelTypeTree({
            PGlid: treeNode.dataRef.glid == treeNode.dataRef.modelId ? 0 : treeNode.dataRef.glid,
            ModelName: treeNode.dataRef.modelName,
            Level: level,
          }).then((res) => {
            if (res && res.length > 0) {
              for (let i = 0; i < res.length; i++) {
                if (res[i].externalId != '0') {
                  res[i].isLeaf = true;
                  res[i].name = res[i].externalId;
                }
                res[i].modelId = treeNode.dataRef.modelId;
                res[i].modelName = treeNode.dataRef.modelName;
                res[i].slots = {
                  title: res[i].name,
                }
                res[i].key = res[i].glid;
              }
              treeNode.dataRef.children = res;
            }
            this.modelTrees = [...this.modelTrees];
            resolve();
          }).catch((res) => {
            this.$message.info('数据请求失败')
            resolve()
          })
        })
      },
      async onCheckTree(checkedKeys, e) {
        const that = this
        if (that.firstClick) {
          that.allModelVisible(false);
          that.firstClick = false;
        }
        const checkNode = e.node.dataRef
        const checked = e.checked
        if (checkNode.glid == '0') {
          if (checked) {
            api.Model.setVisible(checkNode.modelId, true)
          } else {
            api.Model.setVisible(checkNode.modelId, false)
          }
          that.visibleList = []
        } else if (checkNode.externalId != '0') {
          if (checkNode.level !== 0) {
            const repeat = that.visibleList.indexOf(checkNode.externalId)
            repeat == -1 ? that.visibleList.push(checkNode.externalId) : that.visibleList.splice(repeat, 1)
            if (checked) {
              that.visibleList = that.MergeArray([checkNode.externalId], that.visibleList)
              api.Model.setVisible(checkNode.modelId, true)
              api.Feature.setVisible(that.visibleList.join('#'), true, checkNode.modelId, false)
            } else {
              that.visibleList = that.DelArray(that.visibleList, [checkNode.modelId])
              api.Feature.setVisible(checkNode.externalId, false, checkNode.modelId)
            }
          } else {
            if (checked) {
              api.Model.setVisible(checkNode.modelId, true)
              api.Feature.setVisible(that.visibleList.join('#'), true, checkNode.modelId, true)
            } else {
              api.Model.setVisible(checkNode.modelId, false)
              api.Feature.setVisible(that.visibleList.join('#'), false, checkNode.modelId, false)
            }
            that.visibleList = []
          }
        } else {
          e.node.disableCheckbox = true;
          const res = await getModelTypeTreeIdList({
            glId: e.node.dataRef.glid,
            modelName: e.node.dataRef.modelName
          })
          if (store.state.bim.currentClick == 'SubStructure') {
            e.node.disableCheckbox = false;
            if (checked) {
              that.visibleList = that.MergeArray(res, that.visibleList)
              api.Model.setVisible(checkNode.modelId, true)
              api.Feature.setVisible(that.visibleList.join('#'), true, checkNode.modelId, false)
            } else {
              that.visibleList = that.DelArray(that.visibleList, res)
              api.Feature.setVisible(res.join('#'), false, checkNode.modelId)
            }
          }
        }
      },
      treeAction(node, glid, fn) {
        node.some((item, index) => {
          if (item.glid === glid) {
            fn(node, item, index)
            return true;
          }
          if (item.children && item.children.length) {
            this.treeAction(item.children, glid, fn);
          }
        });
      },
      onSelect(keys, event) {
        const that = this;
        that.clearFeatureSelect();
        if (event.node.isLeaf && event.node.checked) {
          that.selectedKeys = keys;
          that.selectedFeature = event.node.dataRef.modelId + "^" + event.node.dataRef.externalId;
          api.Feature.zoomTo({
            featureId: that.selectedFeature,
            distanceRatio: 1,
            flyHeading: 90,
            flyPitch: -45
          });
          api.Feature.setColor(event.node.dataRef.externalId, "rgba(255,255,0,1)", event.node.dataRef.modelId);
        }
      },
      clearFeatureSelect() {
        const that = this;
        if (that.selectedKeys.length > 0 && that.visibleList.indexOf(that.selectedKeys[0]) != -1) {
          api.Feature.setColor(that.selectedFeature, "rgba(255,255,255,1)", that.selectedFeature.split("^")[0]);
          that.selectedFeature = undefined;
          that.selectedKeys = [];
        }
      },
      allModelVisible(state) {
        const that = this;
        let modelList = that.projectMessage.modelList;
        modelList.forEach(item => {
          api.Model.setVisible(item.id, state)
          api.Feature.setVisible(that.visibleList.join('#'), state, item.id, state)
        })
      },
      filterModelTrees(items) {
        items.forEach((item) => {
          if (item.children.length === 0) {
            if (item.equipments.length > 0 || item.parts.length > 0) {
              item.children = [...item.equipments, ...item.parts]
            } else {
              delete item.children
            }
            delete item.equipments
            delete item.parts
            item.disabled = item.status !== 3
          } else {
            delete item.equipments
            delete item.parts
            this.filterModelTrees(item.children)
          }
        })
        return items
      },
      MergeArray(arr1, arr2) {
        var _arr = new Array()
        for (var i = 0; i < arr1.length; i++) {
          _arr.push(arr1[i])
        }
        for (var i = 0; i < arr2.length; i++) {
          var flag = true
          for (var j = 0; j < arr1.length; j++) {
            if (arr2[i] == arr1[j]) {
              flag = false
              break
            }
          }
          if (flag) {
            _arr.push(arr2[i])
          }
        }
        return _arr
      },
      DelArray(array1, array2) {
        var result = []
        for (var i = 0; i < array1.length; i++) {
          var k = 0
          for (var j = 0; j < array2.length; j++) {
            if (array1[i] != array2[j]) {
              k++
              if (k == array2.length) {
                result.push(array1[i])
              }
            }
          }
        }
        return result
      },
      clearScene() {
        const that = this;
        that.clearFeatureSelect();
        that.firstClick = true;
        that.allModelVisible(true);
        that.projectMessage.camera ? api.Camera.setViewPort(that.projectMessage.camera) : api.Model.location(that
          .projectMessage.modelId);
        that.visibleList = [];
        that.checkedKeys = [];
      }
    },
    destroyed() {
      const that = this;
      that.clearScene();
    }
  }
</script>
<style lang="less" scoped>
  .scroll-box {
    margin-top: 15px;
    height: 48vh;
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

    /deep/.ant-tree-node-content-wrapper {
      color: #ffffff;
      max-width: 65%;
      display: inline-block;
      overflow: hidden;
      white-space: nowrap;
      text-overflow: ellipsis;
    }

    .ant-list-empty-text {
      padding: 5px;
      text-align: left;
    }

    /deep/ ul:first-child {

      >li:first-child {

        >.ant-tree-checkbox {
          display: none;
        }
      }
    }
  }

  /deep/.ant-notification-bottomRight {
    .original-notification {
      padding: 15px;

      .ant-notification-notice-message {
        display: none;
      }

      .ant-notification-notice-close {
        display: none;
      }
    }
  }
</style>