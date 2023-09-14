<template>
  <div class="viewpoint-list scroll-box">
    <a-tree checkable class="model-tree" :tree-data="modelTrees" :load-data="onLoadData" :replace-fields="replaceFields"
      @check="onCheckTree" v-model="checkedKeys" :selectedKeys="selectedKeys" />
  </div>
</template>
<script>
  import store from '@/store'
  import {
    getModelSpaceTree,
    getModelSpaceTreeIdList,
    getModelPropertySpace
  } from '@/api/model'
  export default {
    props: {
      projectMessage: {
        type: Object,
        default: undefined,
      },
      addModelNewChilds: {
        type: Function
      }
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
        spaceState: new Map()
      }
    },
    mounted() {
      const that = this
      const date = that.projectMessage.modelList
      for (let i = 0; i < date.length; i++) {
        // this.checkedKeys.push(date[i].id)
        that.modelTrees.push({
          name: date[i].documentName,
          id: date[i].id,
          children: [],
          glid: date[i].id,
          modelName: date[i].modelName,
          level: 0,
          modelId: date[i].id,
        })
        api.Model.setColor(date[i].id, 255, 255, 255, 0.1);
        fetch(`${store.state.modelUrl}/Tools/output/model/${date[i].modelName}space/root.glt`).then(res => {
          if (res.status == 200) {
            let item = JSON.parse(JSON.stringify(date[i]))
            item.modelName = item.modelName + 'space';
            item.id = item.id + 'space';
            that.AddModelNew([item])
          }
        })
      }
    },
    methods: {
      AddModelNew(data) {
        const that = this
        if (data.length == 0) {
          return;
        }
        var modelInfo = data.shift();
        var url = `${store.state.modelUrl}/Tools/output/model/${modelInfo.modelName}/root.glt`
        if (modelInfo.documentType == 2) {
          url = `${store.state.modelUrl}/Tools/output/model/${modelInfo.modelName}/tileset.json`
        }
        api.Model.add(
          url,
          modelInfo.id,
          () => {
            modelInfo.matrix && modelInfo.matrix.offset ? api.Model.offset(modelInfo.matrix.offset[0], modelInfo
              .matrix.offset[1], modelInfo.matrix.offset[2], modelInfo.id) : null;
            modelInfo.matrix && modelInfo.matrix.rotate ? api.Model.rotate(modelInfo.matrix.rotate[0], modelInfo
              .matrix.rotate[1], modelInfo.matrix.rotate[2], modelInfo.id) : null;
          },
          (res) => {
            api.Model.setColor(res.ModelTag, 255, 255, 255, 0);
            that.AddModelNew(data)
          }, {
            FlyTo: that.projectMessage.camera ? false : true, //场地模型设置true，其他模型均设置为false
            VisualRange: modelInfo.modelConfig.visibleDistance, //模型可视距离
            EnableTransparency: true, //是否启用设置半透明，包括构件半透明
          }
        )
      },
      onLoadData(treeNode) {
        return new Promise((resolve) => {
          const level = treeNode.dataRef.glid == treeNode.dataRef.modelId ? 0 : treeNode.dataRef.level + 1
          getModelSpaceTree({
            PGlid: treeNode.dataRef.glid == treeNode.dataRef.modelId ? 0 : treeNode.dataRef.glid,
            ModelName: treeNode.dataRef.modelName,
            Level: level,
          }).then((res) => {
            if (res && res.length > 0) {
              for (let i = 0; i < res.length; i++) {
                if (res[i].externalId != '0') {
                  res[i].isLeaf = true
                  // res[i].name = res[i].externalId
                }
                res[i].modelId = treeNode.dataRef.modelId
              }
              treeNode.dataRef.children = res
            }
            this.modelTrees = [...this.modelTrees]
          })
          resolve()
        })
      },
      async onCheckTree(checkedKeys, e) {
        const checkNode = e.node.dataRef
        const checked = e.checked
        const that = this
        if (checkNode.glid == '0') {
          if (checked) {
            api.Model.setVisible(checkNode.modelId, true)
          } else {
            api.Model.setVisible(checkNode.modelId, false)
          }
        } else if (checkNode.externalId != '0') {
          if (checkNode.level !== 0) {
            const repeat = that.visibleList.indexOf(checkNode.externalId)
            repeat == -1 ? that.visibleList.push(checkNode.externalId) : that.visibleList.splice(repeat, 1)
            if (checked) {
              that.getModelPropertySpace(checkNode.externalId, checkNode.modelName, checkNode.modelId)
              api.Feature.setColor(checkNode.externalId, "rgba(255,255,255,0.5)", checkNode.modelId + 'space');
            } else {
              api.Feature.setColor(checkNode.externalId, "rgba(255,255,255,0)", checkNode.modelId + 'space');
              api.Feature.setColor(that.spaceState.get(checkNode.externalId), "rgba(255,255,255,0.03)", checkNode
                .modelId);
              that.spaceState.delete(checkNode.externalId)
            }
          }
        } else {
          const res = await getModelSpaceTreeIdList({
            glId: e.node.dataRef.glid,
            modelName: e.node.dataRef.modelName
          })
          for (let i = 0; i < res.length; i++) {
            if (checked) {
              that.getModelPropertySpace(res[i], checkNode.modelName, checkNode.modelId)
              api.Feature.setColor(res[i], "rgba(255,255,255,0.5)", checkNode.modelId + 'space');
            } else {
              api.Feature.setColor(res[i], "rgba(255,255,255,0)", checkNode.modelId + 'space');
              api.Feature.setColor(that.spaceState.get(res[i]), "rgba(255,255,255,0.03)", checkNode
                .modelId);
              that.spaceState.delete(res[i])
            }
          }

        }
      },
      async getModelPropertySpace(glid, modelName, modelId) {
        const res = await getModelPropertySpace({
          externalId: glid,
          modelName: modelName
        })
        let data = res.find(item => item.propertySetName == "构件id集合")
        if (!this.spaceState.get(glid)) {
          this.spaceState.set(glid, data.value.split(",").join("#"));
        }
        api.Feature.setColor(data.value.split(",").join("#"), "rgba(255,255,255,1)", modelId);
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
    },
    destroyed() {
      const that = this;
      let modelList = that.projectMessage.modelList;
      modelList.forEach(item => {
        api.Model.setColor(item.id, 255, 255, 255, 1);
        api.Model.remove(item.id + "space");
      })
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

    .ant-tree-node-content-wrapper {
      color: #ffffff;
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
</style>