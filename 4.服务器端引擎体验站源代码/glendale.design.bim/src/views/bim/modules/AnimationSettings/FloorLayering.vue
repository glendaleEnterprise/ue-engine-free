<template>
  <div class="floor-layer" :class="{ 'mobile-Model':isMobile  }">
    <a-form :form="form" :hideRequiredMark="true" v-bind="layout">
      <a-form-item v-for="item in modelList" :label="item.documentName.split('.')[0]" :key="item.id">
        <a-input-number placeholder="请输入分层距离" v-model="item.floorLayer" :formatter="(value) => `${value}米`"
          :parser="(value) => value.replace('米', '')" />
      </a-form-item>
    </a-form>
    <a-space class="options-btn">
      <a-button ghost @click="StartLayering()" @keyup.prevent @keydown.enter.prevent>开始分层</a-button>
      <a-button ghost @click="ClearLayering()" @keyup.prevent @keydown.enter.prevent>清除分层</a-button>
    </a-space>
  </div>
</template>

<script>
  import store from '@/store'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        form: this.$form.createForm(this, {
          name: 'coordinated'
        }),
        modelList: [],
        layout: {
          labelCol: {
            span: 8
          },
          wrapperCol: {
            span: 16
          },
        },
        isMobile: false,
      }
    },
    mounted() {
      const that = this;
      this.isMobile = _isMobile() ? true : false;
      that.projectMessage.modelList.forEach(element => {
        if (element.documentType == 1) {
          that.modelList.push({
            floorLayer: 1,
            floorJson: undefined,
            modelName: element.modelName,
            id: element.id,
            documentName: element.documentName.split('.')[0]
          });
        }
      });
    },
    methods: {
      StartLayering() {
        const that = this;
        that.form.validateFields((err, values) => {
          if (!err) {
            that.modelList.forEach(data => {
              if (data.floorJson == undefined) {
                fetch(
                    `${store.state.modelUrl}/Tools/output/model/${data.modelName}/${data.modelName}floor.json`)
                  .then(res => res.json()).then(jsonData => {
                    data.floorJson = JSON.stringify(jsonData)
                    api.Model.floorLayering(data.id, jsonData, data.floorLayer ? data.floorLayer * 100 : 0)
                  })
              } else {
                api.Model.floorLayering(data.id, JSON.parse(data.floorJson), data.floorLayer ? data.floorLayer *
                  100 : 0)
              }
            })
          }
        })

      },
      ClearLayering() {
        const that = this;
        that.modelList.forEach(data => {
          if (data.floorJson != undefined) {
            api.Model.floorLayering(data.id, JSON.parse(data.floorJson), 0)
          }
        })
      },
    },
    destroyed() {
      this.ClearLayering()
    }
  }
</script>

<style lang="less" scoped>
  .ant-input-number {
    width: 100%;
  }

  .ant-form .ant-form-item {
    margin: 8px 0 !important;
  }

  .options-btn {
    width: 100%;
    justify-content: space-around;
    margin: 10px 0;
  }

  .floor-layer {
    /deep/.ant-form-item-required {
      overflow: hidden;
      white-space: nowrap;
      text-overflow: ellipsis;
      width: 100%;
      display: inline-block;
    }
  }
</style>