<template>
  <a-modal
    title="个人信息"
    :width="600"
    :visible="visible"
    :confirmLoading="confirmLoading"
    @ok="handleOk"
    @cancel="hide"
  >
    <div class="basicInfo">
      <div class="r">
        <a-form v-model="form">
          <!-- <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="组织机构">
            <span
              v-if="organizationUnitsInfo != null && organizationUnitsInfo.length > 0"
            >{{ organizationUnitsInfo.join('/') }}</span
            >
            <span v-else>无</span>
          </a-form-item> -->

          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="手机号">
            <span>{{ form.phoneNumber }}</span>
          </a-form-item>

          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="用户名称">
            <a-input placeholder="用户名称" v-model="form.name"  v-decorator="['name', { rules: [{ required: true, message: '不能为空' }] }]"/>
          </a-form-item>

          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="备注">
            <a-textarea placeholder="备注" v-model="form.extraProperties.Describe" />
          </a-form-item>
        </a-form>
      </div>
    </div>
  </a-modal>
</template>

<script>
import { mapGetters } from 'vuex'
import { upInfo } from '@/api/login'
import { getOrganizationUnitByUserId, getRolesByUserId, getUser, getUserInfo, saveUser } from '@/api/system'
export default {
  name: 'BasicInfoMationModal',
  computed: {
    ...mapGetters(['organizationUnits']),
  },
  props: {
    organizationUnitId: {
      type: String,
      default: undefined,
    },
  },
  components: {    
  },
  data() {
    return {  
      form: this.$store.getters.userInfo,
      user: JSON.parse(JSON.stringify(this.$store.getters.userInfo)), //深拷贝 
      labelCol: {
        xs: { span: 24 },
        sm: { span: 5 },
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 },
      },
      visible: false,
      confirmLoading: false,
      organizationUnitList: [],
      organizationUnitsInfo:null
    }
  },
  methods: { 
    initialize() {
      //手机号
      this.form.phoneNumber = this.user.phoneNumber
      this.form.tenantId = this.user.tenantId
    },
    show(currentUser) {
      this.initialize()      
      this.visible = true
      this.organizationUnitsInfo=currentUser.extraProperties.organizationUnits
      getUserInfo(currentUser.extraProperties.id).then(( item ) => {
        this.form=item
      })
    },
    hide() {
      this.initialize()
      this.visible = false
    },
    verifyPhoneNumber(value) {
      if (value == '') {
        return false
      }
      return true
    },
    handleOk() {
      const that = this
      if (!that.verifyPhoneNumber(that.form.name)) {
        that.$message.error('用户名称输入有误!')
        return
      }
      // 触发表单验证
      that.confirmLoading = true
      const userData = {
        id: that.form.id,
        userName: that.form.userName,
        email: that.form.email,
        name: that.form.name,
        surname: null,
        phoneNumber: that.form.phoneNumber,
        tenantId: that.form.tenantId,
        extraProperties:{Describe: that.form.extraProperties.Describe}
      }
      saveUser(that.form)
        .then((res) => {
          that.$message.success('保存成功')
          that.$emit('ok')
          this.user.phoneNumber = this.form.phoneNumber
          that.hide()
        })        
        .finally(() => {
          that.confirmLoading = false
        })
    },
  },
}
</script>
<style scoped>
.basicInfo {
  overflow: hidden;
  overflow: 'hidden';
  position: 'relative';
  border: '0px solid #ebedf0';
  border-radius: '2px';
}
.basicInfo .l {
  width: 25%;
  padding-left: 20px;
  float: left;
}
.basicInfo .r {
  width: 70%;
  float: left;
  margin-left: 20px;
}
</style>
