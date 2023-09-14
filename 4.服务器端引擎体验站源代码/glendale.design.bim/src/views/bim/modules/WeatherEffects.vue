<template>
  <a-card>
    <a-tooltip v-for="item in currentList" :key="item.id" placement="left" :title="item.name"
      @click="FunctionChoose(item)">
      <div class="weather-btn" :class="{ active: item.id == isChildrenActive }">
        <img :src="require('../../../assets/img/bim/navBtn/' + item.icon + '.png')" />
      </div>
    </a-tooltip>
  </a-card>
</template>

<script>
export default {
  props: {
    typeEffects: {
      type: String
    }
  },
  data() {
    return {
      rainList: [
        {
          id: 'LightRain',
          icon: 'LightRain',
          name: '小雨'
        },
        {
          id: 'Rain',
          icon: 'Rain',
          name: '中雨'
        },
        {
          id: 'Thunderstorm',
          icon: 'Thunderstorm',
          name: '雷雨'
        },
        {
          id: 'SubClose',
          icon: 'SubClose',
          name: '关闭效果'
        },
      ],
      snowList: [
        {
          id: 'Sleet',
          icon: 'Sleet',
          name: '雨夹雪'
        },
        {
          id: 'LightSnow',
          icon: 'LightSnow',
          name: '小雪'
        },
        {
          id: 'Snow',
          icon: 'Snow',
          name: '中雪'
        },
        {
          id: 'Blizzard',
          icon: 'Blizzard',
          name: '暴风雪'
        },
        {
          id: 'SubClose',
          icon: 'SubClose',
          name: '关闭效果'
        },
      ],
      isChildrenActive: '',
      currentList: []
    }
  },
  mounted() {
    const that = this;
    that.currentList = that.typeEffects == 'Rain' ? that.rainList : that.snowList;
  },
  methods: {
    FunctionChoose(data) {
      const that = this;
      that.isChildrenActive = data.id
      if (data.id == 'SubClose') {
        api.Public.deleteWeather()
      } else {
        api.Public.setWeather(data.id);
      }
    }
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