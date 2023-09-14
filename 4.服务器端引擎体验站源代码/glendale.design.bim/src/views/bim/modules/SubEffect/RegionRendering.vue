<template>
  <a-card>
    <a-tooltip v-for="item in currentList" :key="item.id" placement="left" :title="item.name"
      @click="FunctionChoose(item)">
      <div class="weather-btn" :class="{ active: item.id == isChildrenActive }">
        <img :src="require('../../../../assets/img/bim/navBtn/' + item.icon + '.png')" />
      </div>
    </a-tooltip>
  </a-card>
</template>

<script>
  import store from '@/store'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    props: {
      typeEffects: {
        type: String
      }
    },
    data() {
      return {
        currentList: [{
            id: 'DrawPoints',
            icon: 'DrawPoints',
            name: '绘制点'
          },
          {
            id: 'DrawLine',
            icon: 'DrawLine',
            name: '绘制线'
          },
          // {
          //   id: 'DrawFaces',
          //   icon: 'DrawFaces',
          //   name: '绘制面'
          // },
          {
            id: 'SubClose',
            icon: 'SubClose',
            name: '清除绘制'
          },
        ],
        isChildrenActive: '',
        isMobile: false
      }
    },
    methods: {
      FunctionChoose(type) {
        const that = this;
        that.isChildrenActive = type.id
        let posArr = []
        let polygonArr = []
        if (type.id == 'SubClose') {
          api.Public.clearAllDrawObject()
        } else {
          store.dispatch('GetObtainCoordinates', {
            clickStatus: true,
            callback: (data) => {
              api.Public.drawPoint({
                "position": data,
                "size": "5",
                "persistent": "0",
                "time": "3",
                "color": "rgba(255,255,0,1)"
              });
              if (type.id == 'DrawPoints') {
                api.Public.drawPoint({
                  "position": data,
                  "size": "5",
                  "persistent": "1",
                  "time": "500",
                  "color": "rgba(255,0,0,1)"
                });
              }
              if (type.id == 'DrawLine') {
                data[2] = data[2] + 3
                posArr.push(data)
                if (posArr.length == 2) {
                  api.Public.drawLine({
                    "startPosition": posArr[0],
                    "endPosition": posArr[1],
                    "persistent": "1",
                    "time": "500",
                    "color": "rgba(255,0,0,1)"
                  });
                  posArr = [];
                }
              }
              if (type.id == 'DrawFaces') {
                that.$message.success('点击拾取三个及以上点，右键结束取点')
                polygonArr.push(data)
                if (polygonArr.length >= 3) {
                  api.Public.event("RIGHT_CLICK", function (e) {
                    //执行事件
                    api.Public.drawPolygon({
                      "positions": polygonArr,
                      "color": "rgba(255,0,0,1)"
                    });
                    polygonArr = []
                  })
                }
              }
            }
          })
        }
      }
    },
    destroyed() {
      api.Public.clearAllDrawObject()
      // store.dispatch('GetObtainCoordinates', {
      //   clickStatus: true
      // })//关闭拾取坐标
    }
  }
</script>
<style lang="less">
  .operation-notification {
    padding: 0;
    top: calc(35vh - 120px);

    .ant-notification-notice-message {
      display: none;
    }

    .ant-notification-notice-close {
      display: none;
    }
  }
</style>
<style lang="less" scoped>
  .ant-card {
    background-color: transparent;
    border: none;
    border-radius: 5px;

    /deep/.ant-card-body {
      padding: 0;

      .weather-btn {
        width: 100%;
        line-height: 42px;
        text-align: center;
        cursor: pointer;
      }

      .active {
        background: rgba(5, 160, 129, 0.55);
        border-radius: 5px;
      }
    }
  }
</style>