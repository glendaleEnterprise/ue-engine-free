<template>
  <a-row :gutter="[16, 16]">
    <a-col :span="24">
      <div class="header">{{ project.projectName }}</div>
    </a-col>
    <a-col :span="14">
      <a-card :bordered="false" size="small">
        <div style="height: 110px">
          <vue-scroll>
            项目简介：{{ project.remark ? project.remark : '暂无内容' }}
          </vue-scroll>
        </div>
      </a-card>
      <a-card :bordered="false" size="small" style="margin-top: 16px">
        <div style="height: 154px" class="project-msg">
          <vue-scroll>
            <div>
              <a-space>项目全称：</a-space>
              <a-space>{{ project.projectName }}</a-space>
            </div>
            <div>
              <a-space>立项日期：</a-space>
              <a-space>{{ project.beginDate | dateFomart('YYYY年MM月DD日') }}</a-space>
            </div>
            <div>
              <a-space>公开属性：</a-space>
              <a-space>{{ project.isPublic ? '公开项目' : '私有项目' }}</a-space>
            </div>
            <div>
              <a-space>创建人：</a-space>
              <a-space>{{ project.creationName }}</a-space>
            </div>
          </vue-scroll>
        </div>
      </a-card>
    </a-col>
    <a-col :span="10">
      <a-card :bordered="false" size="small">
        <div style="height: 304px">
          <a-carousel arrows dots-class="slick-dots slick-thumb">
            <div slot="prevArrow" class="custom-slick-arrow" style="left: 10px; z-index: 1">
              <a-icon type="left-circle" />
            </div>
            <div slot="nextArrow" class="custom-slick-arrow" style="right: 10px">
              <a-icon type="right-circle" />
            </div>
            <a slot="customPaging" slot-scope="props">
              <img :src="getImgUrl(props.i)" />
            </a>
            <div v-for="(item, index) in projectImages" :key="index">
              <img :src="item.url" />
            </div>
          </a-carousel>
        </div>
      </a-card>
    </a-col>

    <a-col :span="24">
      <a-card :title="false" :bordered="false">
        <div class="chart_box">
          <div class="box">
            <div style="width: 35%">
              <div class="title">各格式模型数量</div>
              <div class="content">
                <div id="chart1" style="height: 240px; width: 100%"></div>
              </div>
            </div>
            <div style="width: 65%">
              <div class="title">上传模型数量</div>
              <div class="content">
                <div id="chart2" style="height: 240px; width: 100%"></div>
              </div>
            </div>
          </div>
        </div>
      </a-card>
    </a-col>
  </a-row>
</template>

<script>
import { getProject } from '../../api/project'
import { getFileByBlobName } from '@/api/file'
import { getNumOfEachFormat } from '../../api/document'
import provinces from '@/static/js/provinces.js'
import echart from 'echarts'

export default {
  name: 'ProjectHome',
  data() {
    return {
      projectId: this.$store.getters.projectId,
      project: {},
      provinces,
      graphData: {},
      projectImages: [
      ],
    }
  },
  created() { },
  mounted() {
    this.getProjectById()

    this.getNumOfEachFormat()
  },
  computed: {
    getProjectAddr() {
      if (this.project) {
        let prov = ''
        let city = ''
        let district = ''
        let ret = ''
        prov = this.provinces.find((t) => t.value == this.project.province)
        if (prov) {
          ret += prov.label
          city = prov.children.find((t) => t.value == this.project.city)
          if (city) {
            ret += city.label
            district = city.children.find((t) => t.value == this.project.district)
            if (district) {
              ret += district.label
            }
          }
        }
        return ret
      }
      return ''
    },
  },
  methods: {
    getFileByBlobName,
    getProjectById() {
      var that = this;
      const projectId = that.$store.getters.currProjectId
      getProject(projectId).then((res) => {
        that.project = res
        if (res.projectImages.length > 0) {
          res.projectImages.forEach((item) => {
            const result = that.getFileByBlobName(item.blobName)
            that.projectImages.push({ url: result })
          })
        }
      })
    },
    getNumOfEachFormat() {
      const that = this
      const projectId = that.$store.getters.currProjectId
      getNumOfEachFormat(projectId).then((res) => {
        if (Object.keys(res).length > 0) {
          that.graphData = res[1]
        }
        if (Object.keys(that.graphData).length > 0) {
          that.drawLine()
          that.drawBar()
        } else {
          that.drawLineDefault()
          that.drawBarDefault()
        }
      })
    },
    drawLine() {
      var myChart = echart.init(document.getElementById('chart1'))
      myChart.setOption({
        tooltip: {
          trigger: 'item',
        },
        legend: {
          type: 'scroll',
          orient: 'vertical',
          right: 40,
          top: 20,
          bottom: 0,
        },
        series: [
          {
            name: '模型格式',
            type: 'pie',
            radius: ['40%', '70%'],
            center: ['40%', '50%'],
            avoidLabelOverlap: false,
            label: {
              show: true,
              position: 'right',
              formatter: '{d}%',
            },
            labelLine: {
              show: true,
            },
            data: this.graphData.groupByFileTypeDto,
          },
        ],
      })
    },
    drawBar() {
      var userNameArr = []
      var userValueArr = []
      for (const item of this.graphData.groupByUserDto) {
        userNameArr.push(item.userName)
        userValueArr.push(item.uploadNum)
      }

      var myChart = this.$echarts.init(document.getElementById('chart2'))

      // const max = Math.max(...userValueArr)

      myChart.setOption({
        color: ['#1890ff'],
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'shadow',
          },
        },
        grid: {
          left: '0',
          right: '2%',
          bottom: '0',
          top: '8%',
          containLabel: true,
        },
        xAxis: [
          {
            type: 'category',
            data: userNameArr,
          },
        ],
        yAxis:
          // max < 5
          //   ? [
          //       {
          //         type: 'value',
          //         min: 0,
          //         max: max,
          //         splitNumber: max,
          //       },
          //     ]
          //   :
          [
            {
              type: 'value',
            },
          ],
        series: [
          {
            name: '数量',
            type: 'bar',
            barWidth: '25%',
            data: userValueArr,
          },
        ],
      })
    },

    drawLineDefault() {
      var myChart = echart.init(document.getElementById('chart1'))
      myChart.setOption({
        tooltip: {
          trigger: 'item',
        },
        legend: {
          type: 'scroll',
          orient: 'vertical',
          right: 10,
          top: 20,
          bottom: 20,
        },
        series: [
          {
            name: '模型格式',
            type: 'pie',
            radius: ['40%', '70%'],
            center: ['40%', '50%'],
            avoidLabelOverlap: false,
            label: {
              show: true,
              position: 'right',
              formatter: '{d}%',
            },
            labelLine: {
              show: true,
            },
            data: [{ name: '未知', value: 0 }],
          },
        ],
      })
    },
    drawBarDefault() {
      var myChart = this.$echarts.init(document.getElementById('chart2'))
      myChart.setOption({
        color: ['#1890ff'],
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'shadow',
          },
        },
        grid: {
          left: '0',
          right: '2%',
          bottom: '0',
          top: '8%',
          containLabel: true,
        },
        xAxis: [
          {
            type: 'category',
            data: ['未知'],
          },
        ],
        yAxis: [
          {
            type: 'value',
            min: 0,
            splitNumber: 5,
            max: 5,
          },
        ],
        series: [
          {
            name: '数量',
            type: 'bar',
            barWidth: '25%',
            data: [0],
          },
        ],
      })
    },
    getImgUrl(i) {
      return this.projectImages[i].url
    },
  },
}
</script> 
<style lang="less" scoped>
.header {
  text-align: center;
  border-bottom: 1px solid #888;
  font-size: 18px;
  height: 50px;
  line-height: 60px;
  font-weight: 400;
}

.chart_box {
  height: calc(100vh - 580px);

  .box {
    position: relative;
    height: 100%;
    margin-top: 10px;
    display: flex;
    justify-content: space-between;

    .title {
      text-align: center;
    }

    .content {}
  }
}

/deep/.ant-carousel {
  background-color: #777;

  .slick-dots {
    height: auto;
  }

  .slick-slide img {
    display: block;
    margin: auto;
    max-width: 100%;
    height: 300px;
  }

  .slick-thumb {
    bottom: 5px;
  }

  .slick-thumb li {
    width: 60px;
    height: 45px;
  }

  .slick-thumb li img {
    border: 2px solid #fff;
    width: 100%;
    height: 100%;
    filter: grayscale(100%);
  }

  .slick-thumb li.slick-active img {
    filter: grayscale(0%);
  }

  .custom-slick-arrow {
    width: 25px;
    height: 25px;
    font-size: 25px;
    color: #fff;
    background-color: rgba(31, 45, 61, 0.11);
    opacity: 0.3;
  }

  .custom-slick-arrow:before {
    display: none;
  }

  .custom-slick-arrow:hover {
    opacity: 0.5;
  }
}

.project-msg {
  /deep/.__view {
    >div {
      line-height: 36px;

      .ant-space:first-child {
        width: 5vw;
        text-align: right;

        .ant-space-item {
          width: 100%;
        }
      }
    }
  }
}
</style>