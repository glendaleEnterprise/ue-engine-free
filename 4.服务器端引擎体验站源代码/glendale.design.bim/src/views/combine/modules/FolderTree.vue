<template>
  <a-card title="模型目录" size="small" :bordered="false" style="background: transparent">
    <div style="height:55vh">
      <vue-scroll>
        <a-tree
          v-model="selectedKeys"
          :treeData="myTreeData"
          :selectedKeys="selectedKeys"
          :expandedKeys="expandedKeys"
          :autoExpandParent="true"
          @expand="onExpand"
          @select="onSelectTree"
        ></a-tree>
      </vue-scroll>
    </div>
  </a-card>
</template>
<script>
import { mapGetters } from 'vuex'
import { getFolderTrees } from '@/api/project'
export default {
  name: 'DocumentTree',
  components: {},
  props: {
    value: {
      type: String,
      default: undefined,
    },
  },
  watch: {
    value(val) {
      this.selectedKeys = [val]
    },
  },
  computed: {
    ...mapGetters(['currProject']),
  },
  data() {
    return {
      myTreeData: [],
      expandedKeys: [],
      selectedKeys: [],
    }
  },
  created() {    
    this.getTree()
  },
  methods: {
    getTree() {
      const that = this
      getFolderTrees(that.currProject.id).then((res) => {
        if (res.length > 0) {
          that.selectedKeys = [res[0].key]
          that.myTreeData = res
          that.expandedKeys = that.selectedKeys
          that.$emit('ok', res[0].key)           
        }else{
          that.$emit('ok', [])          
        }
      })
    },
    onExpand(expandedKeys, e) {
      if (e.expanded) {
        if (expandedKeys.length > 0) {
          expandedKeys.splice(0, expandedKeys.length - 1)
        }
      }
      this.expandedKeys = expandedKeys
    },
    onSelectTree(selectedKeys, e) {       
      if (!e.selected) return
      const that = this
      that.selectedKeys = selectedKeys
      that.expandedKeys = selectedKeys
      this.$emit('ok', selectedKeys[0])
    },
  },
}
</script>

<style lang="less" scoped> 
/deep/.ant-card-head{
border-bottom: 1px solid #44444459;
} 
</style>
