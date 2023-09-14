<template>
  <div class="organizationunit_trees" :class="{tree_loading:!treeData}">
    <a-tree v-if="treeData" :treeData="treeData" :selectedKeys="selectedKeys" :expandedKeys="expandedKeys" :autoExpandParent="autoExpandParent" @expand="onExpand" @select="onSelect" :replaceFields="{ children: 'children', title: 'displayName',key:'id' }" />
    <a-spin v-else />
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
export default {
  name: 'OrganizationUnitTrees',
  computed: {
    // ...mapGetters(['organizationUnits']),
  },
  props: {
    value: {
      type: String,
      default: undefined
    },
    organizationUnits: {
      type: Array,
      default(){
        return []
      }
    },
    isDefault: {//是否设置默认值
      type: Boolean,
      default: false
    },
  },
  watch: {
    organizationUnits:{
      handler (val, oldVal) {
        if (val !== oldVal) {
          this.treeData = val
          if (this.isDefault) {
            const key = Array.of(val[0]?.id)
            this.expandedKeys = key
            this.selectedKeys = key
            this.$emit('input', key[0])
          }
        }
      },
      deep: true,
      immediate: true
    },
    // $route: {
    //   handler () {
    //     this.showModel = this.showModelArray.includes(this.$route.name)
    //   },
    //   deep: true,
    //   immediate: true
    // }
  },
  data () {
    return {
      expandedKeys: [],
      autoExpandParent: true,
      selectedKeys: [],
      treeData: undefined,
    }
  },
  created () {
    // console.log(1)
    // this.$store.dispatch('getOrganizationUnitTrees')
  },
  methods: {
    onExpand (expandedKeys, e) {
      if (e.expanded) {
        if (expandedKeys.length > 0) {
          expandedKeys.splice(0, expandedKeys.length - 1)
        }
      }
      this.expandedKeys = expandedKeys
      // this.autoExpandParent = false
    },
    onSelect (selectedKeys) {
      this.expandedKeys = selectedKeys
      this.selectedKeys = selectedKeys       
      this.$emit('input', selectedKeys[0])
    },
  }
}
</script>

<style lang="less" scoped>
.organizationunit_trees {
  background-color: #fff;
  padding: 10px;
  min-width: 120px;
  height: 650px;
  overflow: auto;
  overflow-x: hidden;
  margin-right: 10px;
  &::-webkit-scrollbar {
    //整体样式
    height: 5px;
    width: 5px;
  }
  &::-webkit-scrollbar-thumb {
    //滑动滑块条样式
    border-radius: 1px;
    box-shadow: inset 0 0 1px rgba(255, 255, 255, 0.2);
    background:#21ad8d;
  }
  &::-webkit-scrollbar-track {
    //轨道的样式
    box-shadow: 0;
    border-radius: 0;
    background: rgba(255, 255, 255, 0.3);
  }
}
.tree_loading {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
