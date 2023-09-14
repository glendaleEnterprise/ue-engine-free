<template>
  <a-modal v-model="createViewState" :title="viewpointPictureData.title" @cancel="resetViewForm" @ok="onViewSubmit" ok-text="确定" cancel-text="取消" :zIndex=1049>
    <a-form-model ref="ruleViewForm" :model="viewForm" :rules="rules" :label-col="labelCol" :wrapper-col="wrapperCol">
      <a-form-model-item ref="name" label="标注标题" prop="title"  :label-col="{ span: 5 }" :wrapper-col="{ span: 19 }">
        <a-input v-model="viewForm.title" />
      </a-form-model-item>
      <a-form-model-item  label="问题类型"  prop="postilCategory"   :label-col="{ span: 5 }" :wrapper-col="{ span: 19 }">
        <a-select placeholder="问题类型" v-model="viewForm.postilCategory" >
          <a-select-option v-for='(item ,i) in postilQuestionType' :key='i'  :value='item.value'>
            {{ item.name }}
          </a-select-option>
        </a-select>
      </a-form-model-item>
      <a-form-model-item ref="advise" label="处理建议" prop='advise' :label-col="{ span: 5 }" :wrapper-col="{ span: 19 }">
        <a-input v-model="viewForm.advise" />
      </a-form-model-item>
      <a-form-model-item ref="handlerUserId" label="指派给" prop='handlerUserId' :label-col="{ span: 5 }" :wrapper-col="{ span: 19 }">
        <div class='ant-input' @click='visible=true' style='cursor: pointer'>
          {{viewForm.handlerUserName}}
        </div>
      </a-form-model-item>

      <a-form-model-item ref="picture" prop="picture" label="批注截图" :label-col="{ span: 5 }" :wrapper-col="{ span: 19 }">
        <div class="thumbnail">
          <img :src="viewpointPictureData.viewPointImg" alt="" style="width: 370px">
        </div>
      </a-form-model-item>
    </a-form-model>
    <UserList ref='UserList' :projectId='projectMessage.projectId' v-model="selectedRows" @handlerUser='handlerUser'/>
    <select-user-modal ref="selectUser" @ok="onChangeUser" v-model="projectUsers" type='radio' />
    <a-modal title="用户选择" :width="1200" :centered="true" :visible="visible" @ok="handlerUser" @cancel="visible=false" :zIndex='3000'>
      <div style="height: 570px">
        <vue-scroll>
          <a-table
            :columns="columns"
            :row-key="(record) => record.userId"
            :data-source="userData"
            bordered
            size="middle"
            :row-selection="{
                  type: 'radio',
                  selectedRowKeys: selectedRowKeys,
                  onSelect: onSelect,
                }"
          >
            <template slot="serial" slot-scope="text, record, index">{{ index + 1 }}</template>
            <template slot="orgIds" slot-scope="text">{{ text.join('/') }}</template>
            <template slot="creationTime" slot-scope="text">
              {{ text | dateFomart('YYYY-MM-DD') }}
            </template>
            <template slot="validityDate" slot-scope="text">
              {{ text | dateFomart('YYYY-MM-DD') }}
            </template>
          </a-table>
        </vue-scroll>
      </div>
    </a-modal>
  </a-modal>
</template>

<script>
import { eventBus } from '@/utils/bus'
import { getParent } from '@/api/dictionary'
import { getProject, getProjectFolderUsers, getProjectUsers } from '@/api/project'
import UserList from '@/views/user/UserList'
import { savePostil } from '@/api/postil'
import SelectUserModal from '@/views/system/modules/SelectUserModal.vue'
export default {
  components:{
    UserList,
    SelectUserModal
  },
  props: {
    createView: {
      type: Boolean,
    },
    projectMessage: {
      type: Object,
    },
    viewpointPictureData: {
      type: Object,
    },
    markOpenChild: {
      type: Boolean,
    },
    viewDataChild: {
      type: Array,
    },
    viewData: {
      type: Array,
    },
  },
  data() {
    return {
      selectedRowKeys:[],
      selectedName:null,
      columns: [
        {
          title: '序号',
          width: '70px',
          align: 'center',
          scopedSlots: { customRender: 'serial' },
        },
        {
          title: '手机号码',
          dataIndex: 'phoneNumber',
          align: 'center',
          width: '150px',
        },
        {
          title: '姓名',
          dataIndex: 'name',
          align: 'center',
          width: '120px',
        },
        {
          title: '职位',
          dataIndex: 'extraProperties.Position',
          scopedSlots: { customRender: 'position' },
          align: 'center',
          width: '120px',
        },
      ],
      createViewState: this.createView , //显示保存界面,
      viewForm: {
        title: '',
        advise:'',
        postilCategory: '',
        handlerUserId: '',
        handlerUserName:''
      },
      projectUsers:[],
      userData:[],
      selectedRows:[],
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      rules: {
        title: [{ required: true, message: '请填写视点名称', trigger: 'change' }],
        postilCategory: [{ required: true, message: '请选择问题类型', trigger: 'change' }],
        advise: [{ required: true, message: '请填写处理建议', trigger: 'change' }],
        handlerUserName: [{ required: true, message: '请选择人员', trigger: 'change' }],
      },
      viewNodeData: [
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
      replaceFields: {
        title: 'viewName',
        key: 'id',
        value: 'id',
      },
      treeNodeNameId: undefined,
      postilQuestionType: [],
      middle_: [],
      viewDataChildNode: this.viewDataChild,
      visible:false,
    }
  },
  watch: {

    createView(newVal) {
      this.createViewState = newVal
    },
    createViewState(newVal, oldVal) {
      this.viewForm = {
        name: '',
        treeNodeName: '',
      }
      this.$emit('update:createView', newVal)
    },
    viewData(newVal) {
      this.viewDataChildNode = newVal
    },
    viewDataChildNode(newVal, oldVal) {
      this.$emit('update:viewData', [])
    },
  },
  async mounted() {
    this.getParent()
    this.getProjectUsers()
  },
  methods: {
    handlerUser(v){
      this.viewForm.handlerUserId=this.selectedRowKeys[0]
      this.viewForm.handlerUserName=this.selectedName
      this.$forceUpdate()
      this.visible=false
    },
    onSelect(record, selected) {
      // this.selectedRowKeys=
      if(selected){
        this.selectedRowKeys=[record.userId]
        this.selectedName=record.name
      }else {
        this.selectedRowKeys=[]
        this.selectedName=null
      }
    },
    async getProjectUsers(){
      // const project = await getProject(record.id)
      getProjectFolderUsers({ SkipCount: 0,projectId: this.projectMessage.projectId, maxResultCount: 20 }).then(data=>{
        this.userData=data
      })
    },
    getParent() {
      getParent('postilQuestionType').then(data=>{
        this.postilQuestionType=data
      })
    },
    filterMenu(tree) {
      var newArr = []
      for (var i = 0; i < tree.length; i++) {
        var item = tree[i]
        if (item.viewType == 1) {
          if (item.children) {
            item.children = this.filterMenu(item.children)
          }
          newArr.push(item)
        }
      }
      return newArr
    },
    Recursion(node) {
      node.scopedSlots = {}
      node.scopedSlots.title = 'custom'
      if (node.viewType == 1) {
        //node.splice(i,1);   //视点本身

        // delete node;
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
    handleCancel() {
      this.$emit('update:visible', false)
    },
    TreeNodeNameChange(value, label, extra) {
      const that = this
      that.treeNodeNameId = extra.triggerValue
    },
    onViewSubmit() {
      const that = this
      that.$refs.ruleViewForm.validate((valid) => {
        if (valid) {
          const parame = {
            sceneType: that.viewpointPictureData.sceneType,
            documentId: that.viewpointPictureData.documentId,
            title: that.viewForm.title,
            postilCategory: that.viewForm.postilCategory,
            describe: that.viewForm.describe,
            advise: that.viewForm.advise,
            imgPath:that.viewpointPictureData.pictureCore,
            viewPosition: that.viewpointPictureData.viewPosition,
            handlerUserId: that.viewForm.handlerUserId,
            projectId: that.projectMessage.projectId,
          }

          savePostil(parame).then((res) => {
            // that.$emit('update:viewData', []);
            that.viewDataChildNode = []
            // let sss = this.getAllNodeId(that.treeNodeNameId,res);
            that.createViewState = false
            that.$emit('update:markOpenChild', false)
            eventBus.$emit('changeShowDrawer',true)
            eventBus.$emit('getPostilLaunch')
            that.$emit('changeShowDrawer')
          })
        } else {
          return false
        }
      })
    },
    // 递归获取所有节点id
    getAllNodeId(expandedKeys, res) {
      if (this.viewDataChild) {
        for (let i = 0; i < this.viewDataChild.length; i++) {
          if (expandedKeys == this.viewDataChild[i].id) {
            res.children = []
            ;(res.slots = {
              icon: 'file',
            }),
              this.viewDataChild[i].children.push(res)
            return
          }
          if (this.viewDataChild[i].children) {
            expandedKeys = this.getAllNodeId(expandedKeys, this.viewDataChild[i].children)
          }
        }
      }
    },
    changeOpenAllModuleFolderVisible() {

      const expandedKeys = this.getAllNodeId(this.expandedKeys, this.moduleDataList)

    },

    resetViewForm() {
      const that = this
      that.createViewState = false
    },
    onChangeUser(v) {
      this.viewForm.handlerUserId=v[0].id
      this.viewForm.handlerUserName=v[0].name
      this.projectUsers = v.map((x) => ({
        id: x.id,
        name: x.name,
        orgNames: x.orgNames,
        phoneNumber: x.phoneNumber,
        position: x.position,
        userName: x.userName,
        isAdmin: false,
      }))
      this.$refs.selectUser.hide()
    },
  },
}
</script>
<style lang="less" scoped>
.form-box {
  .thumbnail {
    width: 100%;
    height: 150px;
    text-align: center;
    vertical-align: middle;
    img {
      max-width: 100%;
      max-height: 100%;
    }
  }
}
.save-view {
  /deep/.ant-modal-mask,
  /deep/.ant-modal-wrap {
    z-index: 1010;
  }
}
/deep/.ant-modal-body {
  padding: 40px;}
</style>
