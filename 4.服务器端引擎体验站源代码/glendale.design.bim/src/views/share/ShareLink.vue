<template>
  <div class="shareBox">
    <a-modal v-model="verificationCode" class="form-box" title="授权码" :rules="rules" :maskClosable="false" :width="360"
      @ok="onSharedCodeSubmit">
      <a-form-model ref="ruleSharedLink" :model="SharedLink" class="mobile-cord" :rules="rules">
        <a-form-model-item ref="code" label="" prop="code" style="margin-bottom: 0">
          <a-input v-model="SharedLink.code" placeholder="请输入授权码" @pressEnter="onSharedCodeSubmit" ref="getFocus" />
        </a-form-model-item>
      </a-form-model>
    </a-modal>
    <view-model v-if="showView" :ids="ids" />
    <div v-show="excShow">
      <a-result status="500" :title="excMessge"> </a-result>
    </div>
  </div>
</template>
<script>
  import {
    ifIsAuthCode,
    getSharedCode,
    addPvm,
    checkSharedCode
  } from '@/api/share'
  import ViewModel from '@/views/bim/Index'
  export default {
    name: 'ShareLink',
    components: {
      ViewModel
    },
    data() {
      return {
        verificationCode: false,
        SharedLink: {
          code: '',
        },
        rules: {
          code: [{
              required: true,
              message: '授权码不能为空'
            },
            {
              len: 4,
              message: '授权码只能为4位数字'
            },
          ],
        },
        shareType: 0,
        shareData: {},
        floderData: {},
        floderArrData: [],
        excShow: false, //分享异常提示
        excMessge: '抱歉，分享数据异常',
        showView: false,
        ids: {}
      }
    },
    created() {},
    mounted() {
      const that = this
      const id = that.$route.params.id
      that.getSharedCode()
    },
    methods: {
      // 填入核验吗提交按钮
      onSharedCodeSubmit() {
        const that = this
        that.$refs.ruleSharedLink.validate((valid) => {
          if (valid) {
            const params = {
              id: that.$route.params.id,
              auth: that.SharedLink.code,
            }
            checkSharedCode(params)
              .then((res) => {
                that.shareData = res
                //成功
                // 根据id获取文件信息    
                that.showView = true
                that.ids = {
                  sceneType: res.sceneType,
                  id: res.modelId,
                  needCheck: false
                }
                // 关闭弹窗
                that.verificationCode = false
                addPvm(id).then((res) => {})
              })
              .catch((err) => {
                err == 'Error: Request failed with status code 403' ? that.$message.warning('验证码输入错误') : null
              })
          } else {
            return false
          }
        })
      },
      // 根据id和验证码获取分享链接对象
      async getSharedCode(state) {
        const that = this
        const params = {
          id: that.$route.params.id,
          auth: that.SharedLink.code,
        }
        await getSharedCode(params)
          .then((res) => {
            if (res.effectiveDay != 200) {
              let endtime = that.filterExpirationTime(res)
              let newDate = that.format(new Date(), '-');
              let time = that.getHour(newDate, endtime)
              if (time == 0) {
                that.excShow = true
              } else {
                that.IfIsAuthCode(res)
              }
            } else {
              that.IfIsAuthCode(res)
            }
          })
          .catch(() => {
            that.excShow = true
          })
      },
      IfIsAuthCode(res) {
        const that = this
        ifIsAuthCode(that.$route.params.id)
          .then((state) => {
            if (state == true) {
              // 打开授权码的框
              that.verificationCode = true
              return false
            } else {
              // 不需要授权码
              that.shareData = res
              //成功
              // 根据id获取文件信息    
              that.showView = true
              that.ids = {
                sceneType: res.sceneType,
                id: res.modelId,
                needCheck: true
              }
              // 关闭弹窗
              that.verificationCode = false
              addPvm(id).then((res) => {})
            }
          })
          .catch(() => {
            that.excShow = true
          })
      },
      handleEvent(data) {
        this.shareData = data
      },
      getHour(s1, s2) {
        var reDate = /\d{4}-\d{1,2}-\d{1,2}/;
        s1 = new Date((reDate.test(s1) ? s1 : '2023-1-1 ' + s1).replace(/-/g, '/'));
        s2 = new Date((reDate.test(s2) ? s2 : '2023-1-1 ' + s2).replace(/-/g, '/'));
        var ms = s2.getTime() - s1.getTime();
        if (ms < 0) return 0;
        return Math.floor(ms / 1000 / 60 / 60); //小时
      },
      filterExpirationTime(record) {
        var date = new Date(Date.parse(record.creationTime.replace(/-/g, "/")));
        var newDate = new Date(date.setDate(date.getDate() + record.effectiveDay)) //获取当前日期后加30天    2021/4/8
        var y = newDate.getFullYear();
        var m = newDate.getMonth() + 1;
        m = m < 10 ? ('0' + m) : m;
        var d = newDate.getDate();
        d = d < 10 ? ('0' + d) : d;
        var h = newDate.getHours();
        h = h < 10 ? ('0' + h) : h;
        var minute = newDate.getMinutes();
        minute = minute < 10 ? ('0' + minute) : minute;
        var second = newDate.getSeconds();
        second = second < 10 ? ('0' + second) : second;
        return y + '-' + m + '-' + d + ' ' + h + ':' + minute + ':' + second;
      },
      // 第一个参数为日期，第二个参数为年月日分割格式 '/'或'-'
      format(Date, str) {
        var obj = {
          Y: Date.getFullYear(),
          M: Date.getMonth() + 1,
          D: Date.getDate(),
          H: Date.getHours(),
          Mi: Date.getMinutes(),
          S: Date.getSeconds()
        }
        // 拼接时间 hh:mm:ss
        var time = ' ' + this.supplement(obj.H) + ':' + this.supplement(obj.Mi) + ':' + this.supplement(obj.S);
        // yyyy-mm-dd
        if (str.indexOf('-') > -1) {
          return obj.Y + '-' + this.supplement(obj.M) + '-' + this.supplement(obj.D) + time;
        }
        // yyyy/mm/dd
        if (str.indexOf('/') > -1) {
          return obj.Y + '/' + this.supplement(obj.M) + '/' + this.supplement(obj.D) + time;
        }
      },
      // 位数不足两位补全0
      supplement(nn) {
        return nn = nn < 10 ? '0' + nn : nn;
      }
    },
  }
</script>

<style lang="less" scoped>
  .shareBox {
    height: 100%;
  }
</style>