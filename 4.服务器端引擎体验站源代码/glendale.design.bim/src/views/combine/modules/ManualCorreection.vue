<template>
  <div class="side-frame floor-layering">
    <a-card :bordered="false">
      <div class="model-operation">
        <div>偏移</div>
        <div>
          <div class="operation-item">
            <div>东西(偏移值)：</div>
            <div>
              <a-icon type="double-left" @click="ModelOffset('X', 'minus')" />
              <a-space>
                <a-input v-model="xValue" placeholder="请输入模型沿东西方向偏移距离" @pressEnter="ModelOffset('X')"
                  @blur="ModelOffset('X')" />
                <span>m</span>
              </a-space>
              <a-icon type="double-right" @click="ModelOffset('X', 'plus')" />
            </div>
          </div>
          <div class="operation-item">
            <div>南北(偏移值)：</div>
            <div>
              <a-icon type="double-left" @click="ModelOffset('Y', 'minus')" />
              <a-space>
                <a-input v-model="yValue" placeholder="请输入模型沿南北方向偏移距离" @pressEnter="ModelOffset('Y')"
                  @blur="ModelOffset('Y')" />
                <span>m</span>
              </a-space>
              <a-icon type="double-right" @click="ModelOffset('Y', 'plus')" />
            </div>
          </div>
          <div class="operation-item">
            <div>高度(偏移值)：</div>
            <div>
              <a-icon type="double-left" @click="ModelOffset('Z', 'minus')" />
              <a-space>
                <a-input v-model="zValue" placeholder="请输入模型沿上下偏移距离" @pressEnter="ModelOffset('Z')"
                  @blur="ModelOffset('Z')" />
                <span>m</span>
              </a-space>
              <a-icon type="double-right" @click="ModelOffset('Z', 'plus')" />
            </div>
          </div>
        </div>
        <div>旋转</div>
        <div>
          <div class="operation-item">
            <div>旋转角度(X)：</div>
            <div>
              <a-icon type="double-left" @click="ModelRotate('x', 'minus')" />
              <a-space>
                <a-input v-model="rotateValueX" placeholder="请输入模型沿东西方向偏移距离" @pressEnter="ModelRotate('x')"
                  @blur="ModelRotate('x')" />
                <span>°</span>
              </a-space>
              <a-icon type="double-right" @click="ModelRotate('x', 'plus')" />
            </div>
          </div>
          <div class="operation-item">
            <div>旋转角度(Y)：</div>
            <div>
              <a-icon type="double-left" @click="ModelRotate('y', 'minus')" />
              <a-space>
                <a-input v-model="rotateValueY" placeholder="请输入模型沿东西方向偏移距离" @pressEnter="ModelRotate('y')"
                  @blur="ModelRotate('y')" />
                <span>°</span>
              </a-space>
              <a-icon type="double-right" @click="ModelRotate('y', 'plus')" />
            </div>
          </div>
          <div class="operation-item">
            <div>旋转角度(Z)：</div>
            <div>
              <a-icon type="double-left" @click="ModelRotate('z', 'minus')" />
              <a-space>
                <a-input v-model="rotateValueZ" placeholder="请输入模型沿东西方向偏移距离" @pressEnter="ModelRotate('z')"
                  @blur="ModelRotate('z')" />
                <span>°</span>
              </a-space>
              <a-icon type="double-right" @click="ModelRotate('z', 'plus')" />
            </div>
          </div>
        </div>
      </div>
      <div class="save-box" v-if="type == 'position'">
        <a-button type="primary" @click="CoordinateCorrection()">坐标确认</a-button>
      </div>
    </a-card>
  </div>
</template>

<script>
export default {
  props: {
    modelList: {
      type: Array,
    },
    type: {
      type: String,
    },
    movementPosition: {
      type: Array,
    },
    saveMatrix: {
      type: Function,
      default: undefined,
    },
    SaveManualValue: {
      type: Function
    },
    lastManualValue: {
      type: Number,
      default: 0
    },
    lastManualArray: {
      type: Array
    },
    changeRotate: {
      type: Function
    },
    changeArr: {
      type: Function
    },

  },
  data() {
    return {
      xValue: 0,
      yValue: 0,
      zValue: 0,
      rotateValueX: 0,
      rotateValueY: 0,
      rotateValueZ: 0,
      rotateLastValueX: 0,
      rotateLastValueY: 0,
      rotateLastValueZ: 0,
      lastRotateValue: 0,
      lastRotateArray: [],
      selectModelList: [],
      xLastValue: 0,
      yLastValue: 0,
      zLastValue: 0,
    }
  },
  watch: {
    modelList: {
      handler(val, oldVal) {
        if (val !== oldVal) {
          this.FilterList()
        }
      },
      deep: true,
    }
  },
  mounted() {
    const that = this;
    that.FilterList()
  },
  methods: {
    FilterList() {
      const that = this
      that.selectModelList = []
      if (that.type == 'position') {
        that.selectModelList = that.movementPosition
      } else {
        that.selectModelList = that.modelList
        const matrix = that.selectModelList[0].matrix

        that.rotateValueX = matrix.rotate[0]
        that.rotateValueY = matrix.rotate[1]
        that.rotateValueZ = matrix.rotate[2]

        that.rotateLastValueX = matrix.rotate[0]
        that.rotateLastValueY = matrix.rotate[1]
        that.rotateLastValueZ = matrix.rotate[2]

        that.xLastValue = matrix.offset[0]
        that.yLastValue = matrix.offset[1]
        that.zLastValue = matrix.offset[2]

        that.xValue = matrix.offset[0]
        that.yValue = matrix.offset[1]
        that.zValue = matrix.offset[2]
      }
    },
    ModelOffset(type, cut) {
      const that = this
      switch (type) {
        case 'X':
          if (cut == 'plus') {
            that.xValue++
          } else if (cut == 'minus') {
            that.xValue--
          }
          break
        case 'Y':
          if (cut == 'plus') {
            that.yValue++
          } else if (cut == 'minus') {
            that.yValue--
          }
          break
        case 'Z':
          if (cut == 'plus') {
            that.zValue++
          } else if (cut == 'minus') {
            that.zValue--
          }
          break
      }
      for (let i = 0; i < that.selectModelList.length; i++) {
        api.Model.offset(
          type == 'X' ? (that.xValue - that.xLastValue) * 1 : 0,
          type == 'Y' ? (that.yValue - that.yLastValue) * 1 : 0,
          type == 'Z' ? (that.zValue - that.zLastValue) * 1 : 0,
          that.selectModelList[i].key
        )
      }
      type == 'X' ? that.xLastValue = that.xValue * 1 : 0;
      type == 'Y' ? that.yLastValue = that.yValue * 1 : 0;
      type == 'Z' ? that.zLastValue = that.zValue * 1 : 0;
      that.changeArr([that.xValue, that.yValue, that.zValue], that.selectModelList[0].key)
    },
    ModelRotate(type, cut) {
      const that = this
      switch (type) {
        case 'x':
          if (cut == 'plus') {
            that.rotateValueX++
          } else if (cut == 'minus') {
            that.rotateValueX--
          }
          break
        case 'y':
          if (cut == 'plus') {
            that.rotateValueY++
          } else if (cut == 'minus') {
            that.rotateValueY--
          }
          break
        case 'z':
          if (cut == 'plus') {
            that.rotateValueZ++
          } else if (cut == 'minus') {
            that.rotateValueZ--
          }
          break
      }
      for (let i = 0; i < that.selectModelList.length; i++) {
        api.Model.rotate(
          type == 'x' ? (that.rotateValueX - that.rotateLastValueX) * 1 : 0,
          type == 'y' ? (that.rotateValueY - that.rotateLastValueY) * 1 : 0,
          type == 'z' ? (that.rotateValueZ - that.rotateLastValueZ) * 1 : 0,
          that.selectModelList[i].key
        )
      }
      type == 'x' ? that.rotateLastValueX = that.rotateValueX * 1 : 0;
      type == 'y' ? that.rotateLastValueY = that.rotateValueY * 1 : 0;
      type == 'z' ? that.rotateLastValueZ = that.rotateValueZ * 1 : 0;
      that.changeRotate([that.rotateValueX, that.rotateValueY, that.rotateValueZ], that.selectModelList[0].key)
    },
    CoordinateCorrection() {
      const that = this
      that.$confirm({
        title: '确定保存合模在GIS影像中的位置？',
        okText: '确认',
        cancelText: '取消',
        onOk() {
          that.saveMatrix()
        },
      })
    },

  },
  destroyed() {
    const that = this;
    that.$emit('SaveManualValue', {
      type: that.type,
      lastValue: that.lastRotateValue,
      lastArray: that.lastRotateArray
    })
  }
}
</script>
<style lang="less" scoped>
.model-operation {
  background: transparent;
  color: #ffffff;
  border: none;
  padding: 10px 0;

  >div:nth-child(2n + 1) {
    font-size: 16px;
  }
}

.operation-item {
  display: flex;
  align-items: center;
  margin: 10px 0;

  >div {
    &:first-child {
      width: 45%;
      text-align: right;
    }

    &:last-child {
      width: 55%;
      display: flex;
      align-items: center;
      justify-content: space-around;

      input {
        width: 50px;
        text-align: center;
        padding: 0;
      }

      .ant-space {
        width: 50%;
      }

      .iconfont {
        cursor: pointer;
      }
    }
  }
}

.save-box {
  text-align: right;
}
</style>