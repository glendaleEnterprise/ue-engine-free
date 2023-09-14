<template>
  <div ref="wrap">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-form-model class="roaming-set" ref="ruleForm" :form="form" :labelCol="{ span: 8 }" :hideRequiredMark="true"
        :wrapperCol="{ span: 16 }" @cancel="handleCancel">
        <a-form-model-item label="模型切换" prop="moveRate" v-show="projectMessage.modelList.length > 1">
          <a-select v-model="form.id" @change="ModelChange">
            <a-select-option v-for='(item, i) in gridList' :key='i' :value='item.id'>
              {{ item.documentName.split(".")[0] }}
            </a-select-option>
          </a-select>
        </a-form-model-item>
        <a-form-model-item label="轴网层级" prop="lookFactor">
          <a-select v-model="form.type" placeholder="请选择轴网层级">
            <a-select-option v-for='(item, i) in gridLevelList' :key='i' :value='item.LevelName'>
              {{ item.LevelName }}
            </a-select-option>
          </a-select>
        </a-form-model-item>
        <a-form-model-item :wrapper-col="{ span: 18, offset: 2 }" style="margin-top: 20px;">
          <a-space :size="20">
            <a-button ghost @click="DrawGrid" @keyup.prevent @keydown.enter.prevent>开始绘制</a-button>
            <a-button ghost @click="ClearDraw" @keyup.prevent @keydown.enter.prevent>清除绘制</a-button>
          </a-space>
        </a-form-model-item>
      </a-form-model>
    </a-card>
  </div>
</template>
<script>
  import store from '@/store'
  export default {
    name: 'SubGrid',
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        gridList: [],
        form: {
          id: '',
          type: '',
        },
        gridLevelList: [], //轴网层级
      }
    },
    mounted() {
      const that = this;
      that.hasGrid();
    },
    methods: {
      hasGrid() {
        const that = this;
        if (that.projectMessage.modelList.length == 1) {
          if (that.projectMessage.modelList[0].modelFormat != "revit" && that.projectMessage.modelList[0].modelFormat !=
            "glzip") {
            that.$message.warning('该模型暂无轴网数据!')
            that.$notification.destroy();
          } else {
            that.getGridData((res) => {
              if (!res) {
                that.$message.warning('该模型暂无轴网数据!')
                that.$notification.destroy();
              } else {
                that.form.id = res[0].id;
                that.form.type = res[0].gridArr[0].LevelName;
                that.gridLevelList = that.gridList[0].gridArr;
              }
            })
          }
        } else {
          that.getGridData((res) => {
            if (!res) {
              that.$message.warning('当前场景模型暂无轴网数据!')
              that.$notification.destroy();
            } else {
              that.form.id = res[0].id;
              that.form.type = res[0].gridArr[0].LevelName;
              that.gridLevelList = that.gridList[0].gridArr;
            }
          })
        }
      },
      judgeGrid() {

      },
      getGridData(callback) {
        const that = this;
        let modelList = JSON.parse(JSON.stringify(that.projectMessage.modelList));
        let count = 0;
        let allCount = modelList.length;
        for (let i = 0; i < modelList.length; i++) {
          if (modelList[i].modelFormat != "revit" && modelList[i].modelFormat != "glzip") {
            allCount--;
          } else {
            fetch(`${store.state.modelUrl}/Tools/output/model/${modelList[i].modelName}/GridJson.json`).then(res => res
              .json().catch(res => {
                count++
                if (count == allCount && that.gridList.length == 0) {
                  callback(false)
                }
              })).then(res => {
              count++;
              if (res.length > 0) {
                modelList[i].gridArr = res;
                that.gridList.push(modelList[i])
                if (count == allCount) {
                  callback(that.gridList)
                }
              } else {
                if (count == allCount && that.gridList.length == 0) {
                  callback(false)
                }
              }
            }).catch(res => {
              count++
              if (count == allCount && that.gridList.length == 0) {
                callback(false)
              }
            })
          }
        }
      },
      DrawGrid() {
        const that = this;
        let list = that.gridList.filter(item => item.id == that.form.id)
        api.Model.clearGrid();
        api.Model.drawGrid({
          "jsonData": list[0].gridArr,
          "level": that.form.type,
          "showLable": true,
          "color": "rgba(255,255,0,1)",
          "width": 3
        });
      },
      handleCancel() {
        this.$notification.destroy();
      },
      ClearDraw() {
        api.Model.clearGrid();
      },
      ModelChange() {
        const that = this;
        api.Model.clearGrid();
        let list = that.gridList.filter(item => item.id == that.form.id)
        that.gridLevelList = list[0].gridArr
      }
    },
    destroyed() {
      const that = this
      api.Model.clearGrid();
    }
  }
</script>