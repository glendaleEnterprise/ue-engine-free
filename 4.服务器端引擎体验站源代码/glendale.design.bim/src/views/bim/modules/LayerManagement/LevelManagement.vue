<template>
  <div class="scroll-box">
    <a-tree :defaultExpandAll="true" :checkedKeys="checkedKeys" checkable :tree-data="treeData" @check="onCheckTree"
      v-if="isResh">
      <template v-slot:title="nodeData">
        <span :class="{'treenode-disable':nodeData.addOver}">{{nodeData.title}}</span>
      </template>
    </a-tree>
  </div>
</template>
<script>
  import store from '@/store'
  export default {
    name: 'LevelManagement',
    data() {
      return {
        pakCurrent: [],
        treeData: [],
        checkedKeys: [],
        addPreparationList: [],
        addList: [],
        addNow: false,
        isResh: true,
      }
    },
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    mounted() {
      const that = this;
      that.treeData.push({
        title: that.projectMessage.modelList[0].modelName.split("-Windows")[0],
        key: that.projectMessage.modelId,
        children: []
      })
      store.state.bim.pakList.forEach(item => {
        that.treeData[0].children.push({
          title: item,
          key: item,
          disabled: false
        })
      })
      that.checkedKeys = store.state.bim.pakCurrent;
    },
    methods: {
      onCheckTree(checkedKeys, e) {
        const that = this;
        if (e.node.dataRef.key == that.projectMessage.modelId) {
          if (e.checked) {
            that.treeData[0].children.forEach(item => {
              if (that.checkedKeys.indexOf(item.key) == -1) {
                that.addPreparationList.push(item.key)
                item.disabled = true;
                item.addOver = true;
              } else {
                item.disabled = true;
              }
            })
            that.addNow ? null : that.addPak(that.addPreparationList)
            that.addPreparationList.length > 0 ? that.treeData[0].disabled = true : null;
          } else {
            that.checkedKeys.forEach(item => {
              if (item != that.projectMessage.modelId) {
                that.delPak(item)
              }
            })
          }
        } else {
          if (e.checked) {
            e.node.dataRef.addOver = true;
            that.treeData[0].children.forEach(item => {
              item.disabled = true;
            })
            if (that.addPreparationList.length > 0) {
              if (that.addPreparationList.indexOf(e.node.dataRef.key) == -1) {
                that.addPreparationList.push(e.node.dataRef.key)
              }
            } else {
              that.addPreparationList.push(e.node.dataRef.key)
              that.addNow ? null : that.addPak()
            }
          } else {
            that.delPak(e.node.dataRef.key)
          }
        }
        that.checkedKeys = checkedKeys;
      },
      addPak() {
        const that = this;
        if (that.addPreparationList.length == 0) {
          return;
        }
        that.addNow = true;
        var modelInfo = that.addPreparationList.shift();
        api.Pak.loadMap({
          MapName: modelInfo
        }, (res) => {
          that.addNow = false
          that.isResh = false;
          that.treeData[0].children.forEach(item => {
            if (item.key == res.MapName) {
              item.addOver = false;
            }
          })
          that.$nextTick(() => {
            that.isResh = true;
          });
          res.isSuccessed ? that.$message.success(`关卡${res.MapName}加载成功`) : that.$message.error(
            `关卡${res.MapName}加载失败`)
          if (that.addPreparationList.length > 0) {
            that.treeData[0].children.forEach(item => {
              item.disabled = true;
            })
            that.treeData[0].disabled = true;
            that.addPak()
          } else {
            that.treeData[0].children.forEach(item => {
              item.disabled = false;
            })
            that.treeData[0].disabled = false;
          }
        })
      },
      delPak(id) {
        api.Pak.unloadMap({
          MapName: id
        }, (res) => {
          res.isSuccessed ? this.$message.success(`关卡${res.MapName}卸载成功`) : this.$message.error(
            `关卡${res.MapName}卸载失败`)
        })
      }
    },
    destroyed() {
      store.dispatch('GetPakCurrent', this.checkedKeys);
    },
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

    // /deep/ ul:first-child {

    //   >li:first-child {

    //     >.ant-tree-checkbox {
    //       display: none;
    //     }
    //   }
    // }
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

  li.ant-tree-treenode-disabled>.ant-tree-node-content-wrapper {
    span {
      color: #ffffff;
    }

    .treenode-disable {
      color: rgba(0, 0, 0, 0.25);
    }
  }
</style>