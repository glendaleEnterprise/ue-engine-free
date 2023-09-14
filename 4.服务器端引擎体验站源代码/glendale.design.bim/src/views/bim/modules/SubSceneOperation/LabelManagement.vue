<template>
  <div ref="wrap">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateLabel" @keyup.prevent @keydown.enter.prevent>创建标签</a-button>
      </a-space>
      <a-list size="small" :data-source="labelList" :locale="{ emptyText: `暂无标签数据` }" class="label-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-switch @change="LabelVisible($event, item)" v-model="item.state" />
          <a-space class="label-title">{{ item.labelName }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DelLabel(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="创建标签" :width="320" :visible="visible" :confirm-loading="confirmLoading" @ok="SaveLabel"
      @cancel="handleCancel" cancel-text="取消" ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag
      :destroyOnClose="true" :maskClosable="false" :mask="false" :class="{ 'mobile-Model':isMobile  }">
      <a-form :form="formFolder" :hideRequiredMark="true">
        <a-form-item label="标签背景" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <a-select
            v-decorator="['labelStyle', { rules: [{ required: true, message: '请选择标签背景样式！' }], initialValue: '2' },]"
            placeholder="请选择标签背景样式！">
            <a-select-option value="1"> 气泡标签 </a-select-option>
            <a-select-option value="2"> 展板标签 </a-select-option>
            <a-select-option value="0"> 图标标签 </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item :label="formFolder.getFieldsValue().labelStyle * 1 != 0 ? '标签内容' : '标签名称'" :label-col="{ span: 7 }"
          :wrapper-col="{ span: 16 }">
          <a-input v-decorator="['title', { rules: [{ required: true, message: '请输入' }] }]" />
        </a-form-item>
        <a-form-item label="标签位置" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }">
          <a-select v-decorator="['location', { rules: [{ required: true, message: '请选择标签位置！' }], initialValue: '2' },]"
            placeholder="请选择标签位置！">
            <a-select-option value="1">左上</a-select-option>
            <a-select-option value="2">中上</a-select-option>
            <a-select-option value="3">右上</a-select-option>
            <a-select-option value="4">左中</a-select-option>
            <a-select-option value="5">正中</a-select-option>
            <a-select-option value="6">右中</a-select-option>
            <a-select-option value="7">左下</a-select-option>
            <a-select-option value="8">中下</a-select-option>
            <a-select-option value="9">右下</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="文字大小" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }"
          v-show="formFolder.getFieldsValue().labelStyle * 1 != 0">
          <a-input-number placeholder="请输入标签文字大小"
            v-decorator="['fontsize', { rules: [{ required: true, message: '请输入标签文字大小' }], initialValue: 14 }]"
            :min="0" />
        </a-form-item>
        <a-form-item label="标签颜色" :label-col="{ span: 7 }" :wrapper-col="{ span: 16 }" class="color-picker"
          v-show="formFolder.getFieldsValue().labelStyle * 1 != 0">
          <a-input v-model="color" disabled />
          <colorPicker v-model="color"></colorPicker>
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>
<script>
  import {
    getListLabel,
    deleteLabel,
    saveLabel
  } from '@/api/label'
  import store from '@/store'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    name: 'LabelManagement',
    data() {
      return {
        pagination: {
          current: 1,
          pageSize: 10
        },
        labelList: [], //标签列表
        visible: false, //创建框
        color: '#ffffff', //标签颜色
        confirmLoading: false,
        formFolder: this.$form.createForm(this, {
          name: 'coordinated2'
        }),
        labelPosition: [],
        isMobile: false
      }
    },
    props: {
      projectMessage: {
        type: Object,
        default: undefined,
      },
    },
    mounted() {
      const that = this;
      that.isMobile = _isMobile() ? true : false
      that.pagination.pageSize = Math.ceil(document.getElementsByClassName("label-list")[0].offsetHeight / 40) + 1
      that.formFolder.resetFields(); //清空表单
      that.fetch()
    },
    methods: {
      fetch() {
        const that = this
        getListLabel({
          MaxResultCount: that.pagination.pageSize,
          SkipCount: (that.pagination.current - 1) * that.pagination.pageSize,
          modelId: that.projectMessage.sceneType == 0 ? that.projectMessage.modelList[0].id : that
            .projectMessage.id,
        }).then((res) => {
          const pagination = {
            ...that.pagination
          }
          pagination.total = res.totalCount
          let list = store.state.bim.labelVisibleNow;
          res.items.forEach(item => {
            list.indexOf(item.id) == -1 ? item.state = false : item.state = true;
          })
          that.labelList = that.labelList.concat(res.items)
          that.pagination = pagination
          if (res.totalCount > (that.pagination.current * that.pagination.pageSize)) {
            that.updated();
          }
        })
      },
      updated() {
        const that = this;
        that.$nextTick(() => {
          const el = document.querySelector('.scroll-box');
          const offsetHeight = el.offsetHeight;
          el.onscroll = () => {
            const scrollTop = el.scrollTop;
            const scrollHeight = el.scrollHeight;
            if ((offsetHeight + scrollTop) - scrollHeight >= -1) {
              // 需要执行的代码
              if (that.pagination.total > that.pagination.current * that.pagination.pageSize) {
                that.pagination.current++
                that.fetch()
              }
            }
          };
        });
      },
      CreateLabel() {
        const that = this;
        that.$message.info('请点击选择标签位置')
        store.dispatch('GetObtainCoordinates', {
          clickStatus: true,
          callback: (data) => {
            that.labelPosition = data;
            that.visible = true;
            store.dispatch('GetObtainCoordinates', {
              clickStatus: false
            })
          }
        })
      },
      DelLabel(data) {
        const that = this;
        deleteLabel(data.id).then((res) => {
          let list = store.state.bim.labelVisibleNow;
          let index = list.indexOf(data.id);
          if (index != -1) {
            api.Label.removeBalloon(list[index])
            list.splice(index, 1)
            store.dispatch('GetLabelVisibleNow', list)
          }
          that.$message.success('删除成功')
          that.pagination.current = 1;
          that.labelList = []
          that.fetch()
        })

        // index != -1 ? list.splice(index, 1) : null
        // store.dispatch('GetLabelVisibleNow', list)
      },
      SaveLabel() {
        const that = this
        that.formFolder.validateFields((err, values) => {
          if (!err) {
            values.color = that.color;
            values.position = that.labelPosition
            const params = {
              labelType: 0,
              labelName: values.title,
              blobName: values.location + '&' + values.fontsize + '&' + values.labelStyle + "&" + values.color,
              projectId: this.projectMessage.projectId,
              modelId: that.projectMessage.sceneType == 0 ? this.projectMessage.modelList[0].id : this
                .projectMessage.id,
              position: JSON.stringify(values.position),
              sceneType: that.projectMessage.sceneType == 0 ? 0 : 1
            }
            saveLabel(params).then((res) => {
              if (res) {
                values.id = res.id
                let list = store.state.bim.labelVisibleNow;
                list.push(res.id)
                store.dispatch('GetLabelVisibleNow', list)
                that.AddLabel(res)
                that.$message.success('保存成功')
                that.pagination.current = 1;
                that.labelList = []
                that.fetch()
              } else {
                that.$message.error(`保存失败！`)
              }
            }).finally(() => {
              that.handleCancel()
            })
          }
        })
      },
      AddLabel(data) {
        const that = this;
        let parameter = data.blobName.split('&');
        let image = '';
        let top;
        let haveTxt = ""
        switch (parseInt(parameter[2])) {
          case 0:
            image = 'http://' + window.location.host + '/static/img/0.png';
            break;
          case 1:
            image = 'http://' + window.location.host + '/static/img/1.png';
            top =
              'top:15%;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;width: 100%;'
            haveTxt = data.labelName
            break;
          case 2:
            image = 'http://' + window.location.host + '/static/img/2.png';
            top = 'top:30%'
            haveTxt = data.labelName
            break;
        }
        let html = ` <style>
                    .tag-box{
                        position: relative
                    }
                    .tag-txt{
                        position: absolute;
                        ${top};
                        left: 50%;
                        transform: translate(-50%, -50%);
                        text-align: center;
                        color: #ffffff;
                    }
                    </style>
                    <div class="tag-box">
                        <img src="${image}" alt="">
                        <span class="tag-txt" style="color:${parameter[3] ? parameter[3] : '#ffffff'};font-size:${parameter[1] ? parameter[1] : 14}px">${haveTxt}</span>
                    </div>`
        html = html.replace(/\"/g, "'")
        html = html.replace(/[\n]/g, "");
        let lableOption = {
          "Title": data.labelName,
          "ID": data.id,
          "Position": JSON.parse(data.position),
          "Pivot": parameter[0],
          "Html": html,
          onClick: (res) => {}
        }
        api.Label.addBalloon(lableOption);
      },
      handleCancel() {
        const that = this
        that.visible = false;
        that.formFolder.resetFields() //清空表单
      },
      LabelVisible(state, data) {
        const that = this;
        let list = store.state.bim.labelVisibleNow;
        if (state) {
          list.push(data.id)
          store.dispatch('GetLabelVisibleNow', list)
          that.AddLabel(data)
        } else {
          let index = list.indexOf(data.id);
          list.splice(index, 1)
          store.dispatch('GetLabelVisibleNow', list)
          api.Label.removeBalloon(data.id);
        }
      },
    },
  }
</script>
<style lang='less' scoped>
  .label-title {
    width: 50%;
    cursor: pointer;
  }
</style>