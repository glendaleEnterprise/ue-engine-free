<template>
  <div ref="wrap"  :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-space>
        <a-button ghost @click="CreateHeatMap" @keyup.prevent @keydown.enter.prevent>新建热力图</a-button>
      </a-space>
      <a-list size="small" :data-source="heatMapHistoryList" :locale="{ emptyText: `暂无数据` }"
        class="viewpoint-list scroll-box">
        <a-list-item slot="renderItem" slot-scope="item,index">
          <a-space class="viewpoint-title">{{ item.title }}</a-space>
          <a-space slot="actions">
            <a-tooltip title="删除">
              <a-icon type="delete" @click="DeleteHeatMap(item, index)" />
            </a-tooltip>
          </a-space>
        </a-list-item>
      </a-list>
    </a-card>
    <a-modal title="热力图设置" :width="320" :visible="visible" @ok="SaveHeatMap" @cancel="handleCancel" cancel-text="取消"
      ok-text="确定" :getContainer="() => this.$refs.wrap" v-drag :destroyOnClose="false" :maskClosable="false"
      :mask="false">
      <a-form :form="form" :hideRequiredMark="true">
        <a-form-item label="热力图名称" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-input placeholder="请输入热力图名称" v-decorator="['title', { rules: [{ required: true, message: '请输入热力图名称' }] }]" />
        </a-form-item>
        <a-form-item label="温度区间" :label-col="{ span: 8 }" :wrapper-col="{ span: 15 }">
          <a-space>
            <a-input-number placeholder="请输入最低温度"
              v-decorator="['minimumTemperature', { rules: [{ required: true, message: '请输入最低温度' }], initialValue: 0 }]"
              :formatter="(value) => `${value}°`" :parser="(value) => value.replace('°', '')" />
            -
            <a-input-number placeholder="请输入最高温度"
              v-decorator="['highestTemperature', { rules: [{ required: true, message: '请输入最高温度' }], initialValue: 40 }]"
              :formatter="(value) => `${value}°`" :parser="(value) => value.replace('°', '')" />
          </a-space>
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>
<script>
import store from '@/store'
import { colorHextoRGB, genlabID,
    _isMobile } from '@/api/public'
export default {
  name: 'PointSourse',
  props: {
    projectMessage: {
      type: Object,
      default: null,
    },
  },
  data() {
    return {
      visible: false,
      form: this.$form.createForm(this, { name: 'coordinated' }),
      heatMapHistoryList: [],   //热力线列表
      color: 'rgba(255,255,0,1)',
      colorRgba: 'rgba(255,255,0,1)',
      heatMapList: [],   //轨迹列表
      minLon: undefined,
      maxLon: undefined,
      minLat: undefined,
      maxLat: undefined,
      isMobile: false
    }
  },
  mounted() {
    const that = this;
    this.isMobile = _isMobile() ? true : false;
    that.heatMapHistoryList = store.state.bim.heatMapHistoryList;
  },
  methods: {
    CreateHeatMap() {
      const that = this;
      that.form.resetFields() //清空表单
      that.heatMapList = []
      that.minLon = undefined;
      that.maxLon = undefined;
      that.maxLat = undefined;
      that.minLat = undefined;
      api.Public.clearAllDrawObject()
      that.$message.info('请在场景中点击四个点，组成热力图范围')
      store.dispatch('GetObtainCoordinates', {
        clickStatus: true, callback: (data) => {
          data[2] = data[2] + 1
          api.Public.convertWorldToCartographicLocation(data, (position) => {
            that.heatMapList.length < 4 ? that.heatMapList.push([position[1], position[0], position[2]]) : null
            that.minLon = that.minLon != undefined ? Math.min(that.minLon, position[0]) : position[0]
            that.maxLon = that.maxLon != undefined ? Math.max(that.maxLon, position[0]) : position[0]
            that.maxLat = that.maxLat != undefined ? Math.max(that.maxLat, position[1]) : position[1]
            that.minLat = that.minLat != undefined ? Math.min(that.minLat, position[1]) : position[1]
            if (that.heatMapList.length == 4) {
              that.visible = true
              store.dispatch('GetObtainCoordinates', {
                clickStatus: false
              })
            }
          })
          api.Public.drawPoint({
            "position": data,
            "size": "5",
            "persistent": "1",
            "color": "rgba(255,255,0,1)"
          });
        }
      })
    },
    changePosition(position) {
      var p = new Promise(function (resolve, reject) { //做一些异步操作
        api.Public.convertCartographicToWorldLocation(position, (ss) => {
          api.Public.drawPoint({
            "position": ss,
            "size": "5",
            "persistent": "1",
            "color": "rgba(255,0,255,1)"
          });
          resolve()
        })
      });
      return p;
    },
    SaveHeatMap() {
      const that = this
      api.Public.clearAllDrawObject()
      if (that.heatMapList.length != 4) {
        that.$message.warning('请点击!')
        return
      }
      that.form.validateFields(async (err, values) => {
        if (!err) {
          let teatMapData = {
            "data": {
              "M": [
              ]
            },
            "min": values.minimumTemperature,// "s" 数据中最小值
            "max": values.highestTemperature,//"s"数据中最大值
            "point1": that.heatMapList[0],//热力图范围 左上
            "point2": that.heatMapList[1],//热力图范围 左下
            "point3": that.heatMapList[2],//热力图范围 右下
            "point4": that.heatMapList[3]//热力图范围 右上
          }

          for (let i = 0; i < 200; i++) {
            let randomLat = Math.random() * (that.maxLat - that.minLat) + that.minLat
            let randomLon = Math.random() * (that.maxLon - that.minLon) + that.minLon
            // await that.changePosition([randomLon, randomLat, that.heatMapList[0][2] + 0.1])
            teatMapData.data.M.push({
              "v": [randomLat, randomLon, that.heatMapList[0][2] + 0.01],
              "s": Math.random() * (values.highestTemperature - values.minimumTemperature) + values.minimumTemperature
            })
          }
          var options = {
            id: "heatMap" + genlabID(6),
            data: teatMapData,
            isFlyTo: true,
          };
          that.heatMapHistoryList.push({
            title: values.title,
            id: options.id
          })
          api.Plugin.loadHeatMap(options);
          that.visible = false;
        }
      })
    },
    DeleteHeatMap(item, index) {
      const that = this
      that.$confirm({
        cancelText: '取消',
        okText: '确定',
        title: `确定要删除热力图 “${item.title}” 吗？`,
        onOk() {
          api.Plugin.removeHeatMap(item.id);
          that.heatMapHistoryList.splice(index, 1)
          that.$message.success('删除成功！')
        },
      })
    },
    handleCancel() {
      this.visible = false
    },
    ColorPickerChange() {
      const that = this;
      that.colorRgba = colorHextoRGB(that.color)
    }
  },
  destroyed() {
    const that = this
    store.dispatch('GetObtainCoordinates', {
      clickStatus: false
    });   //终止拾取坐标事件 
    store.dispatch('GetHeatMapHistoryList', that.heatMapHistoryList)
  }
}
</script>
<style lang="less" scoped>
.viewpoint-title {
  width: 70%;
  cursor: pointer;
}

.motionTrail-list {
  height: 12vh !important;
  margin: 0 13px;
}
</style>