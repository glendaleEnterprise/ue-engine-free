<template>
  <div id="MarkPage" :class="openType == 'shared'?'shared-mark':'origin-mark'">
    <div class="canvasDraw" :style="{width:initData.canvasWidth+'px',height:initData.canvasHeight+'px',left:canvasLeft+'px',top:canvasTop+'px'}">
      <div class="drawFont" data-type="hide" v-show="drawFont" :style="{left:drawFontLeft+'px',top:drawFontTop+'px',height: fontBoxSize + 8+'px'}">
        <span class="intoFont"></span>
        <input type="text" :style='{color: selectColor,fontSize: fontBoxSize}' class="intoFontInput" id="markFront" @blur="intoFontInputBlur" @input="IntoFontInput" v-model="intoFontInput">
      </div>
      <canvas id="canvas" :width="initData.canvasWidth" :height="initData.canvasHeight" :style="{cursor:cursor}"></canvas>
      <canvas id="canvasImg" :width="initData.canvasWidth" :height="initData.canvasHeight"></canvas>
      <img src="" alt="" class="downImg">
    </div>
    <div class="markBtnBox" :style="{left:canvasLeft+'px',transform:'translate(0,calc(-50% + '+canvasTop/2 +'px))'}" v-show='this.changeStyle'>
      <div class="shrink-box" :class="[editSelect?'shrink-openshrink-open':'shrink-hide']">
        <span class="shrink-line"></span>
        <span @click="ShrinkChange" class="shrink-change">
          <img v-if="editSelect" src="../../../assets/img/bim/shrink.png" alt="" />
          <img v-else src="../../../assets/img/bim/extend.png" alt="" />
        </span>
      </div>
      <div class="markBtn" :class="[editSelect?'mark-btn-show':'mark-btn-hide']">
        <div class="tool_item" v-for="(value,index) in DrawSelect" :key="index" @click="DrawClick(value.name,value.nameNum,index)" :class="[drawSelectIndex == index? 'draw-select':'draw-noselect']">
          <div>
            <img :src="drawSelectIndex == index?value.img:value.img1" :title="value.title" />
          </div>
          <div>{{ value.title }}</div>
          <div class="color-panel" v-if="value.name == 'strokeColorBox'">
            <colorPicker v-model="selectColor" id="colorPick" @change="ChangeColor"></colorPicker>
          </div>
        </div>
      </div>
    </div>
    <!-- <div class="tool_item" v-for="value,index in DrawSelect" :key="index" @click="DrawClick(value.name,value.nameNum,index)" :class="[drawSelectIndex == index? 'draw-select':'draw-noselect']">
          <div>
            <img :src="drawSelectIndex == index?value.img:value.img1" :title="value.title" />
          </div>
          <div>{{value.title}}</div>
        </div> -->

    <!-- <div class="markBtn" :style="{left:canvasLeft+'px',transform:'translate(0,calc(-50% + '+canvasTop/2 +'px))'}">
      <a-drawer placement="left" :closable="false" :visible="visibleDrawer" :get-container="false" :wrap-style="{ position: 'fixed' }">
        <div class="mark-btn-show">
          <div class="tool_item" v-for="value,index in DrawSelect" :key="index" @click="DrawClick(value.name,value.nameNum,index)" :class="[drawSelectIndex == index? 'draw-select':'draw-noselect']">
            <div>
              <img :src="drawSelectIndex == index?value.img:value.img1" :title="value.title" />
            </div>
            <div>{{value.title}}</div>
          </div>
        </div>
      </a-drawer>
    </div> -->
    <div class="sort-btn hidden-xs">
      <a-space :size="5">
        <a-button v-for="(value,index) in SortSelect" :key="index" type="primary" @click="SortClick(value.name,index)" class="btn-tool" :class="[activeIndex === index ?'edit-btn-select':'edit-btn-noselect']" :style="{background: !changeStyle?'#fff':''}" :disabled='!changeStyle'>
          <div v-if="value.name == 'strokeColorBox'">
            <colorPicker v-model="selectColor" id="colorPick" @change="ChangeColor"></colorPicker>
            <img :src="value.img" :title="value.title" />
          </div>
          <div v-else-if="value.name == 'downLoad' && openType == 'shared'">
            <img :src="activeIndex === index?(index == 4 && initData.isFill)? value.img1: value.img1:(index == 4 && initData.isFill)? value.img1: value.img" :title="value.title" />
          </div>
          <img v-else :src="activeIndex === index?(index == 4 && initData.isFill)? value.img1: value.img1:(index == 4 && initData.isFill)? value.img1: value.img" :title="value.title" />
        </a-button>
        <!--        <a-button type="primary" @click="SortClickNew('fillDraw',-1)" class="btn-tool" :class="[initData.isFill ?'edit-btn-select':'edit-btn-noselect']" :disabled='!changeStyle'  :style="{background: !changeStyle?'#fff':''}">-->
        <!--          <img :src="initData.isFill? require('../../../assets/img/bim/FillBtn_1.png'): require('../../../assets/img/bim/FillBtn.png')" title="填充" />-->
        <!--        </a-button>-->
        <a-button type="primary" @click="SortClickNew('cancel',-1)" class="btn-tool">
          <img src="../../../assets/img/bim/Close.png" title="关闭" />
        </a-button>
      </a-space>
    </div>
    <save-view ref="SaveV" @changeShowDrawer='changeShowDrawer($event)' :viewDataChild="viewDataChild" :viewpointPictureData="viewpointPictureData" :createView.sync="createView" :projectMessage="projectMessage" :markOpenChild.sync="markOpenChild"></save-view>
  </div>
</template>
<script>
import { uploadFile } from '@/api/file'
import SaveView from './SaveView'
import { eventBus } from '@/utils/bus'
const DrawSelect = [
  {
    name: 'rect',
    nameNum: '1',
    title: '矩形',
    img: require('../../../assets/img/bim/DrawRectangle.png'),
    img1: require('../../../assets/img/bim/DrawRectangle_1.png'),
  },
  // {
  //   name: 'delta',
  //   nameNum: '1',
  //   title: '三角',
  //   img: require('../../../assets/img/bim/DrawTriangle.png'),
  //   img1: require('../../../assets/img/bim/DrawTriangle_1.png'),
  // },
  // {
  //   name: 'circle',
  //   nameNum: '1',
  //   title: '圆形',
  //   img: require('../../../assets/img/bim/DrawCircle.png'),
  //   img1: require('../../../assets/img/bim/DrawCircle_1.png'),
  // },
  {
    name: 'ellipse',
    nameNum: '1',
    title: '圆形',
    img: require('../../../assets/img/bim/DrawCircle.png'),
    img1: require('../../../assets/img/bim/DrawCircle_1.png'),
  },
  // {
  //   name: 'line',
  //   nameNum: '1',
  //   title: '直线',
  //   img: require('../../../assets/img/bim/DrawLine.png'),
  //   img1: require('../../../assets/img/bim/DrawLine_1.png'),
  // },
  //   {
  //     name: 'polygon',
  //     nameNum: '1',
  //     title: '多边形',
  //     img: require('../../../assets/img/bim/DrawPolygon.png'),
  //     img1: require('../../../assets/img/bim/DrawPolygon_1.png'),
  //   },
  {
    name: 'arrow',
    nameNum: '1',
    title: '箭头',
    img: require('../../../assets/img/bim/mark/arrow.png'),
    img1: require('../../../assets/img/bim/mark/arrow_1.png'),
  },
  {
    name: 'font',
    nameNum: '1',
    title: '文字',
    img: require('../../../assets/img/bim/DrawText.png'),
    img1: require('../../../assets/img/bim/DrawText_1.png'),
  },
  //   {
  //     name: 'signet',
  //     nameNum: '1',
  //     title: '印章',
  //     img: require('../../../assets/img/bim/DrawSeal.png'),
  //     img1: require('../../../assets/img/bim/DrawSeal_1.png'),
  //   },
  {
    name: 'pen',
    nameNum: '1',
    title: '铅笔',
    img: require('../../../assets/img/bim/DrawPencil.png'),
    img1: require('../../../assets/img/bim/DrawPencil_1.png'),
  },
  {
    name: 'strokeColorBox',
    nameNum: '1',
    title: '颜色',
    img: require('../../../assets/img/bim/mark/ColorBtn.png'),
    img1: require('../../../assets/img/bim/mark/ColorBtn_1.png'),
  },
  {
    name: 'recall',
    nameNum: '1',
    title: '撤销',
    img: require('../../../assets/img/bim/mark/ColorBtn.png'),
    img1: require('../../../assets/img/bim/mark/ColorBtn_1.png'),
  },
  {
    name: 'downLoad',
    nameNum: '1',
    title: '保存',
    img: require('../../../assets/img/bim/SaveBtn.png'),
    img1: require('../../../assets/img/bim/SaveBtn_1.png'),
  },
  //   {
  //     name: 'eraser',
  //     nameNum: '1',
  //     title: '橡皮',
  //     img: require('../../../assets/img/bim/DrawRubber.png'),
  //     img1: require('../../../assets/img/bim/DrawRubber_1.png'),
  //   },
  // {
  //   name: 'cloud',
  //   nameNum: '1',
  //   title: '云',
  //   img: require('../../../assets/img/bim/Clouds.png'),
  //   img1: require('../../../assets/img/bim/Clouds_1.png'),
  // },
]
const SortSelect = [
  // {
  //   name: 'draw',
  //   nameNum: '1',
  //   title: '绘制',
  //   img: require('../../../assets/img/bim/EditorBtn.png'),
  //   img1: require('../../../assets/img/bim/EditorBtn_1.png'),
  // },
  // {
  //   name: 'move',
  //   nameNum: '1',
  //   title: '平移',
  //   img: require('../../../assets/img/bim/TranslationBtn.png'),
  //   img1: require('../../../assets/img/bim/TranslationBtn_1.png'),
  // },
  // {
  //   name: 'remote',
  //   nameNum: '1',
  //   title: '删除',
  //   img: require('../../../assets/img/bim/DeleteBtn.png'),
  //   img1: require('../../../assets/img/bim/DeleteBtn_1.png'),
  // },
  // {
  //   name: 'strokeColorBox',
  //   nameNum: '1',
  //   title: '颜色',
  //   img: require('../../../assets/img/bim/EditorBtn.png'),
  //   img1: require('../../../assets/img/bim/EditorBtn_1.png'),
  // },
  // {
  //   name: "fillDraw",
  //   nameNum: "1",
  //   title: '填充',
  //   img: require("../../../assets/img/bim/FillBtn.png"),
  //   img1: require("../../../assets/img/bim/FillBtn_1.png"),
  // },
  // {
  //   name: 'downLoad',
  //   nameNum: '1',
  //   title: '保存',
  //   img: require('../../../assets/img/bim/SaveBtn.png'),
  //   img1: require('../../../assets/img/bim/SaveBtn_1.png'),
  // },
  // {
  //   name: "cancel",
  //   nameNum: "1",
  //   title: '取消',
  //   img: require("../../../assets/img/bim/Close.png"),
  //   img1: require("../../../assets/img/bim/Close.png"),
  // }
]
export default {
  name: 'MarkPage',
  components: { SaveView },
  data () {
    return {
      canvasLeft: 0,
      canvasTop: 0,
      canvas_front: null,
      canvas_bgImg: null,
      ctx_base: undefined,
      ctx_bg: undefined,
      initData: {
        drawHistoryArrData: [], // 存放所有绘制图形的数据
        context2d: this.ctx_base, // canvas绘图2d环境
        canvasWidth: 0,
        canvasHeight: 0,
        relPosX: 0, // 鼠标在绘制图形中按下相对该图形左面的距离
        relPosY: 0, // 鼠标在绘制图形中按下相对该图形上面的距离
        relPosToX: 0, // 鼠标在绘制图形中按下相对该图形左面的距离
        relPosToY: 0,
        initLeft: 0,
        initTop: 0,
        chooseIndex: 0, // 选中图形在drawHistoryArrData数据中的index
        drawOrMove: 'draw', // 当前模式是拖拽还是绘制
        isMove: false, // 是否可以拖拽
        drawType: this.drawType, // 绘制图形的类型
        drawTypeNum: '1', // 用于区分同一图形不同形状
        size: 2, // 绘制的粗细
        fontSize: this.fontBoxSize,
        context: '',
        color: this.selectColor, // 绘制颜色
        isFill: false, // 是否填充
        background: this.selectColor,
        msgArr: [], // 画笔信息
      },
      DrawSelect,
      isMove: false,
      drawFont: false,
      isDrawing: false,
      drawFontTop: 0,
      drawFontLeft: 0,
      intoFontInput: '',
      fontBoxSize: 20,
      SortSelect,
      cursor: 'crosshair',
      editSelect: true,
      createView: false,
      viewpointPictureData: this.viewpointData,
      selectColor: '#ff1e02',
      editBtn: false,
      activeIndex: 0,
      drawSelectIndex: 0,
      markOpenChild: this.markOpen,
      viewDataChild: this.viewData,
      visibleDrawer: false,
      drawList:[],
    }
  },
  props: {
    base64image: { type: String, },
    changeStyle: { type: Boolean, default: false, },
    markOpen: { type: Boolean, },
    projectMessage: { type: Object, },
    viewData: { type: Array, },
    openType: { type: String, },
    drawType: { type: String, default: 'rect', },
    viewpointData: {
      type: Object,
    },
  },
  created () {
    if (this.openType == 'shared') {
      this.SortSelect.splice(4, 1)
    }
    if (!this.changeStyle) {
      // this.DrawSelect=[]
      // this.SortSelect=[]
      // this.DrawClick ('', '', '')
      // this.SortClickNew('cancel',-1)
      this.activeIndex = 1111
    }
  },
  watch: {
    base64image (newVal, oldVal) {
      this.initMark()
    },
    markOpen (newVal) {
      this.markOpenChild = newVal
    },
    markOpenChild (newVal, oldVal) {
      this.$emit('update:markOpen', newVal)
    },
    viewDataChild (newVal, oldVal) {
      this.$emit('update:viewData', newVal)
    },
    viewData (newVal) {
      this.viewDataChild = newVal
    },
  },
  mounted () {
    const that = this
    const element = document.getElementById('cesiumContainer').getBoundingClientRect()
    that.canvas_front = document.getElementById('canvas')
    that.ctx_base = that.canvas_front.getContext('2d')
    that.canvas_bgImg = document.getElementById('canvasImg')
    that.ctx_bg = that.canvas_bgImg.getContext('2d')
    that.initData.canvasWidth = element.width
    that.initData.canvasHeight = element.height
    that.canvasLeft = element.left
    that.canvasTop = element.top
    this.initMark()
  },
  methods: {
    initMark () {
      const that = this
      that.ctx_bg.fillStyle = 'rgba(255, 255, 255, 0)'
      that.ctx_base.fillStyle = 'rgba(255, 255, 255, 0)'
      that.ctx_base.fillRect(0, 0, that.initData.canvasWidth, that.initData.canvasHeight)
      that.ctx_bg.fillRect(0, 0, that.initData.canvasWidth, that.initData.canvasHeight)
      that.ctx_base.restore()
      that.ctx_bg.restore()
      that.ctx_base.beginPath()
      that.ctx_bg.beginPath()

      that.ctx_base.clearRect(0, 0, that.initData.canvasWidth, that.initData.canvasHeight)
      that.ctx_bg.clearRect(0, 0, that.initData.canvasWidth, that.initData.canvasHeight)

      that.initData.context2d = that.ctx_base
      this.initData.color = this.selectColor
      this.initData.background = this.selectColor
      const img = new Image()
      img.src = that.base64image
      img.onload = function() {
        that.ctx_bg.drawImage(this, 0, 0, that.initData.canvasWidth, that.initData.canvasHeight)
      }

      that.canvas_front.addEventListener('mousedown', function(e) {
        const fatherNode = document.getElementById('colorPick');
        fatherNode.getElementsByClassName('box')[0].classList.remove('open')
        if (e.type == 'touchstart') {
          e.preventDefault()
        }
        if (e.button == '0' || e.type == 'touchstart') {
          // 判断是左键按下
          that.mouseDown(e)
          that.isMove = true
          that.canvas_front.addEventListener(
            'mousemove',
            function(e) {
              if (that.isMove) {
                if (e.type == 'touchmove') {
                  e.preventDefault()
                }
                that.mouseMove(e)
              }
            },
            true
          )
        }
      })
      that.canvas_front.addEventListener('touchstart', function(e) {
        if (e.type == 'touchstart') {
          e.preventDefault()
        }
        if (e.button == '0' || e.type == 'touchstart') {
          // 判断是左键按下
          that.mouseDown(e)
          that.isMove = true
          that.canvas_front.addEventListener(
            'touchmove',
            function(e) {
              if (that.isMove) {
                if (e.type == 'touchmove') {
                  e.preventDefault()
                }
                that.mouseMove(e)
              }
            },
            true
          )
        }
      })
      that.canvas_front.addEventListener('mouseup', function(e) {
        that.mouseUp()
      })
      that.canvas_front.addEventListener('touchend', function(e) {
        that.mouseUp()
      })
      that.$emit('update:base64image', '')
    },
    drawArr (arr) {
      this.drawList=arr
      for (var j in arr) {
        this.drawTypeArr(arr, j)
      }
    },
    DrawClick (name, nameNum, index) {
      const that = this
      const fatherNode = document.getElementById('colorPick');
      fatherNode.getElementsByClassName('box')[0].classList.remove('open')
      if (name == 'strokeColorBox') {
        fatherNode.getElementsByClassName('box')[0].classList.add('open')
      } else if(name == 'recall'){
        this.initData.drawHistoryArrData.pop()
        that.initData.context2d.clearRect(0, 0, that.initData.canvasWidth, that.initData.canvasHeight)
        that.drawArr(this.initData.drawHistoryArrData)
        that.initData.drawType = name
        that.initData.drayTypeNum = nameNum
        that.drawSelectIndex = index
      }  else{
        that.initData.drawType = name
        that.initData.drayTypeNum = nameNum
        that.drawSelectIndex = index
      }
      if(name == 'downLoad'){
        this.SortClick (name)
      }

    },
    drawTypeArr: function(arr, j) {
      const that = this
      var drawTypeFn = {
        // 绘制方法
        rect: function() {
          // 矩形
          that.initData.context2d.beginPath()
          that.initData.context2d.lineWidth = arr[j].size
          that.initData.context2d.strokeStyle = arr[j].color
          that.initData.context2d.rect(arr[j].x, arr[j].y, arr[j].w, arr[j].h)
          if (arr[j].fill) {
            that.initData.context2d.fillStyle = arr[j].fill
            that.initData.context2d.fill()
          }
          that.initData.context2d.stroke()
        },
        line: function() {
          // 线
          that.initData.context2d.beginPath()
          that.initData.context2d.moveTo(arr[j].x, arr[j].y) //设置起点状态
          that.initData.context2d.lineTo(arr[j].toX, arr[j].toY) //设置末端状态
          that.initData.context2d.lineWidth = arr[j].size //设置线宽状态
          that.initData.context2d.strokeStyle = arr[j].color //设置线的颜色状态
          that.initData.context2d.stroke()
        },
        circle: function() {
          // 圆
          that.initData.context2d.beginPath()
          that.initData.context2d.lineWidth = arr[j].size //设置线宽状态
          that.initData.context2d.strokeStyle = arr[j].color
          that.initData.context2d.arc(arr[j].x, arr[j].y, arr[j].r, 0, 2 * Math.PI)
          if (arr[j].fill) {
            that.initData.context2d.fillStyle = arr[j].fill
            that.initData.context2d.fill()
          }
          that.initData.context2d.stroke()
        },
        delta: function() {
          // 三角
          var w = arr[j].toX - arr[j].x
          var h = arr[j].toY - arr[j].y
          var harfDis = Math.tan(30 / 2) * h
          if (arr[j].toX - arr[j].x > 0) {
            harfDis = harfDis
          } else {
            harfDis = -harfDis
          }
          that.initData.context2d.beginPath()
          that.initData.context2d.moveTo(arr[j].x, arr[j].y) //设置起点状态
          that.initData.context2d.lineTo(arr[j].toX, arr[j].toY)
          that.initData.context2d.lineTo(arr[j].toX + 2 * harfDis, arr[j].toY)
          that.initData.context2d.lineTo(arr[j].x, arr[j].y) //设置末端状态
          that.initData.context2d.lineWidth = arr[j].size //设置线宽状态
          that.initData.context2d.strokeStyle = arr[j].color //设置线的颜色状态
          if (arr[j].fill) {
            that.initData.context2d.fillStyle = arr[j].fill
            that.initData.context2d.fill()
          }
          that.initData.context2d.stroke()
        },
        ellipse: function() {
          that.initData.context2d.beginPath()
          that.initData.context2d.lineWidth = arr[j].size //设置线宽状态
          that.initData.context2d.strokeStyle = arr[j].color
          that.initData.context2d.ellipse(
            arr[j].x + (arr[j].toX - arr[j].x) / 2, arr[j].y + (arr[j].toY - arr[j].y) / 2,
            Math.abs(arr[j].toX - arr[j].x) / 2,
            Math.abs(arr[j].toY - arr[j].y) / 2,
            0,
            0,
            Math.PI * 2
          )
          if (arr[j].fill) {
            that.initData.context2d.fillStyle = arr[j].fill
            that.initData.context2d.fill()
          }
          that.initData.context2d.stroke()
        },
        arrow: function() {
          // 箭头
          var vertex = [];
          var x1 = arr[j].x, y1 = arr[j].y, x2 = arr[j].toX, y2 = arr[j].toY;
          var el = 50, al = 15;
          //计算箭头底边两个点（开始点，结束点，两边角度，箭头角度）
          vertex[0] = x1, vertex[1] = y1, vertex[6] = x2, vertex[7] = y2;
          //计算起点坐标与X轴之间的夹角角度值
          var angle = Math.atan2(y2 - y1, x2 - x1) / Math.PI * 180;
          var x = x2 - x1, y = y2 - y1, length = Math.sqrt(Math.pow(x, 2) + Math.pow(y, 2));
          if (length < 250) {
            el /= 2, al / 2;
          } else if (length < 500) {
            el *= length / 500, al *= length / 500;
          }
          vertex[8] = x2 - el * Math.cos(Math.PI / 180 * (angle + al));
          vertex[9] = y2 - el * Math.sin(Math.PI / 180 * (angle + al));
          vertex[4] = x2 - el * Math.cos(Math.PI / 180 * (angle - al));
          vertex[5] = y2 - el * Math.sin(Math.PI / 180 * (angle - al));
          //获取另外两个顶点坐标
          x = (vertex[4] + vertex[8]) / 2, y = (vertex[5] + vertex[9]) / 2;
          vertex[2] = (vertex[4] + x) / 2;
          vertex[3] = (vertex[5] + y) / 2;
          vertex[10] = (vertex[8] + x) / 2;
          vertex[11] = (vertex[9] + y) / 2;
          //计算完成,开始绘制
          that.initData.context2d.beginPath();
          that.initData.context2d.moveTo(vertex[0], vertex[1]);
          that.initData.context2d.lineTo(vertex[2], vertex[3]);
          that.initData.context2d.lineTo(vertex[4], vertex[5]);
          that.initData.context2d.lineTo(vertex[6], vertex[7]);
          that.initData.context2d.lineTo(vertex[8], vertex[9]);
          that.initData.context2d.lineTo(vertex[10], vertex[11]);
          that.initData.context2d.fillStyle = arr[j].color
          that.initData.context2d.fill()
          that.initData.context2d.lineWidth = arr[j].size //设置线宽状态
          that.initData.context2d.strokeStyle = arr[j].color //设置线的颜色状态
          that.initData.context2d.stroke()
        },
        polygon: function() {
          //六边形
          var y1 = (arr[j].toY - arr[j].y) / 2
          var tan = Math.tan((90 - arr[j].reg / 2) * ((2 * Math.PI) / 360))
          var x1 = y1 * tan
          that.initData.context2d.beginPath()
          that.initData.context2d.moveTo(arr[j].x, arr[j].y) //设置起点状态
          that.initData.context2d.lineTo(arr[j].toX, arr[j].y)
          that.initData.context2d.lineTo(arr[j].toX + x1, arr[j].y + y1)
          that.initData.context2d.lineTo(arr[j].toX, arr[j].toY) //设置末端状态
          that.initData.context2d.lineTo(arr[j].x, arr[j].toY)
          that.initData.context2d.lineTo(arr[j].x - x1, arr[j].y + y1)
          that.initData.context2d.lineTo(arr[j].x, arr[j].y)
          that.initData.context2d.lineWidth = arr[j].size //设置线宽状态
          that.initData.context2d.strokeStyle = arr[j].color //设置线的颜色状态
          if (arr[j].fill) {
            that.initData.context2d.fillStyle = arr[j].fill
            that.initData.context2d.fill()
          }
          that.initData.context2d.stroke()
        },
        font: function() {
          // that.isMove = false;
          that.initData.context2d.font = (arr[j].fontSize - 4) + 'px  Verdana'
          that.initData.context2d.textAlign = 'left'
          that.initData.context2d.textBaseline = 'top'
          that.initData.context2d.fillStyle = arr[j].color
          that.initData.context2d.fillText(arr[j].context, arr[j].x, arr[j].y)
          that.drawFont = false
          document.getElementsByClassName('intoFont')[0].innerHTML = ''
          that.intoFontInput = ''
        },
        signet: function() {
          var img = new Image()
          img.src =
            'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAfQAAAHGCAMAAAC1uokeAAAC+lBMVEUAAAD1rhv1uh31uh31rhv1rhv1rhv1rhv1rhv1sRv1rhv1uR31rhv1rhv1rhv1rhv1rhv1rhv1wR/1rhv1rhv1rhv1rhv1rhv1rhv0yyH1rhv1rhv1rhv1rhv1rhv1rhv1rhv1rhv1rhv1rhv1rhvz3CX1rhv1rhv1rhv1rhv1rhv1rhv1rhv1rhvz3CX1rhv1rhvz3CX1rhv1rhvz3CX1rhv1rhv1rhv1rhv1rhvz3CX1rhvz3CX1rhvz3CXz3CXz3CXz3CXz3CXz3CX1rhv1rhv1rhv1rhv1rhv1rhv1rhv1rhv1rhv1rhvz3CXz3CXz3CX1rhv1rhv1rhv1rhvz3CX1rhvz3CX1rhvz3CXz3CX1rhv1rhv1rhvz3CXz3CXz3CXz3CXz3CXz3CXz3CXz3CXz3CXz3CXz3CX1rhvz3CXz3CXz3CXz3CXz3CXz3CXz3CXz3CXz3CX1rhvz3CXz3CX1rhvz3CXz3CX1rhvz3CXz3CXz3CXz3CX1rhv///8IBAQAAAHx1gDx1wXz2hjz5Cf//yvy2hPy2yT1qBrz3Cfz2xzy2Q7z3CLy2AkFAQPz3Sn1phn//ynz4Sb1rBry2CT1pBnz2x7z3iX/6Sf03zXz2iT1rRv/7Sf/8ynz2yDz4Cbz4yf44CX//Cn13iX//fT/+Sn2qhr01iP1vR72tBwTDgn03zr/9Snz3yX//vj+/O/9+t/89cb68af04EL2uR354iX00iL9+dnz3S/1wh/79L7787j/8Cfz4yb8983464f1yiH68rH25Fj0xyA5Mg7+++X25mT84yX1sBvw1QD+++n36HD35mn25V314Uf14kz0ziFCOhAlHwwcFwr9+NP46oH//vz67pr57IzgzCbBsCP68KH46Xj141Hl0SagkR6Zix0yKw2AcBlNRBPRvyTHtiOFeBluYxYNCge0pCGuniBcURRWTRP57ZJ1axfr1Cemlh+UhRz56SrXxCRoWxbw0wCOgBz05SfbxyXq2ylgVRS8qCL1oxnHc083AAAAfXRSTlMA8gUKHBP5NOgOpQEj18m+UfYIViwpGJyPEIo5L9pO7OTFWj3+182ggGcg3dFqJUpGOCbU0MGY4LtjFalcQ/Ln497JwHx0cbD8h4R3eW5LMey0rZSNLO6HYR37uF1AnKGQYlZE9qx9cVCzGZaCdmiwP6cheLtstrm3ZbS1tsFWbK8AADARSURBVHja7N0JV9NAEAfwf1t6QCk9KAUsYKHlEItWQQERFZCjgoCAyKkoyENRVDyfz8xn94DngdBmk9002+zvE1CmaXZ2ZmehWOl+NjtQlR9MN0JxiMoaOubrWIxUQhEs2L1Zu9G20dYy3NTtQUl4++lvvvUVDxRxPEsB+mM5FO7s2kgnYKlYlk7yVfmhiLJOp8hcnMzXXoVVJuk0HZehCDFNZ0stTMRhgRk6Q3MOCn85KuLi0lUIlqaztVZD4SwRoOLyEModokKWoPC1TnrchUhLVFhKrei4Gid9aiFOUs+XTuGmLkP6uCogzDUqbsANhZMF0msBolwgPVIVULhIk34tEKMxQLpE1Yudj3rSrwNinCO9hqGY5wlQyR/1ugzppjZqOPATi3WIUEUMklDMihCTOPhLEguX2p0zbYWY3AV/D4hJKAbFnEFiEgZ3w8RoEoo5E8TE5QVvYWI1DcWUaWKTBmctxE6l6+bMEJsNcNZKx2ywX+AU54jNJvi6TEbMQLEu6JkK8DVARvhU3mZh0EPga5iM6YRiWdAXAPDP0dVajoH1QR8HV34yqgqKkCKb+CpXFRlWB8WoTmIR9YKnOipItUoyEBf0a+BqkYxLqeYpi4KeB08VUTqh9LV9R2go4T/6Lv2isjar5YmBLwGOYikyI6VKrJakbK3gqYfM2YRiQdDz4KmGzHkPxYKgt4Cjy2TSCyjiS6vRBDh6QSaFUA68uZXBwSZYapoxS7fFxsyRjPwHXrzjNRn6KVwNC90n/boseK84qRu6opOhqsHTYIk23hvP09lscIxWvGBDyfo920i385WAbfK1n3ogsc3zdEJzAhZpYVkt2yhfk70rtoH+txyHNTZLU0vfJA7uQVaeVjrNVBqW8DM1q9ihkP5HFyTVnSrt+cyKDOl0MQhuKnxODnqTi84UgQW80VJUtcaJh/uQkt9HVNqou/tJpxXw00E8DEJG1a7Sn8CvIZ2qwU2OuJBygKi33wbTNl6QPh12W8ZJ2ht5jYrx+SFaPenTYLdlHLm8kM+iLeYujJM+EXBzn7hohXzaSI+UF2K1Wf9YtRIXi5BOhYt06XBDqDjpMgRukuTYddwQ6bQOoRpduitaNuh2/0svpBMh3a5DqLDFRws8AeJiBdLptc0R/AaL/4Q2xy7jLtinWyBCxQU84GaSfnBk20wzscg0QZzK83qqqnZL0vOQThux6fdCnHoqJgsANmqZkXPUUJgYDUCcTUun8IaJgykJd2Cb7HUc+4WF6cNV4qEN8nlvr88Zp4JCbgC2qqRL2ScVJna+OgjTQwUEKiCmyOSseTNeFxmQDUKYPJ0p7AFH1c7M0I1vPtdDnBk6XX8tuLpApoXkvGp5kIzZgDj+Zjppqrc+B86GqICyvrJpmoxxdUOglnUfHUtV1aZzSS+4C6bof8642GGJjtkpWwdQmevp6uqaqI0HIUj3lNmYS5igH2kgo8YhtybHxtzMVKUmSK3OsTE3s4S9Bqklok6NOdJETv2B7yfjAt2QmOeomOnE8fbrZFh/AlLrJOOGILMuMipbCbkNkwkRSCxJBg0EIbtPZFwWMss6qMZyQtyBJ3SPTDilOargZ2d3UeZJuN5lYncB5aGTjJuAxN4Tq6nyuTM9TIY1y7yoCYaITUruJPVfYZsc7gh6Xq5emvvlcPVloxuC5YhJuLyu7TAe9TB48Fy5ufv4c9/I/I7228787fbRD++2dg/2rhzGIMQ9YnAOZWaodDMYbuxtje5oRcz3fdl//WzuBjibtMnZntK4V5qsbW53VGOw1v5l62B7FdwEa0ifi+X0Ov8tYv1e7JWtEc2Qka+Pbj50gwd3h6Ov5unuJSOaYcylR3c0U+bHtnhE3t1KRfmk3m8urMGyGTuxp6MaF/PfXs3eEL1PMSl7gaWgZMCSS4Uv7d/SOFrr27+5ChNWqJCw5C1CxS2Kv35we0wToP2xicAnBugsC2UfcgDxkNig32zXhGnf2muEMZtZOkV/l+TdErr1+MQF/eC2Jtba2Os5GJKsitI/QksSTpgwzFMvKOgH85oV7jzei8EAz+X8g1AgGnUFeh+ca5O6C86I6gEBV07enNcss/PmaSMMiXkT3vLaYtcv2cH5WrzZEc1iYwc3oLDJhTi2TM31aaUw9rQRCpPLvZyGkdx4rpXK2ps9p/5YGzVcw2Nv5pVWUvP7c1BY+B+YvSFt9rZWcn1PZe7lK4HuxSgVEEdBqx80W9jZP4TCINbTbLQ5dFezjw9XoLBID00ZuI7syohmK32zUFhUXqhhfM7djzXbad+GwqT7bi/9LVuHAmZvaXakws6semIgSkd6Iygg9lyzq1H1bmfmGZ4Zqsl2+lHIsx3Nxj5cgsJb7I1mc0/U9qwBEr7N/7ULhR/3O00Kt1X+xs3beU0WY6sQKdFT35mvdcJ7ZF+TySMIE5+kX6aGyr2zcu6OJpfbbyFEZRX9MVnW3ZWvNPm8i4G/SLSMprQUdNinyejWLHirp5MG5BwMX9RHTVbPY+DJ20H/W/aj/DSOafK6tQ1+6gLlc6tTYc/WNKntg5fkVJmO0pWouqLXyCVwMZwp9wl0x7bl2Y8p4AAc5JwytGZLKw/vYFo644xRRZ5RrVyMHMIc/xQVcRfl4FC2PbiC9mBG9TIV1QP5PZShivqdvPMOiqOK4/iTFFOxRA1qjMYe0dhi7D1qbLHG3nvvdZz3zgfschcS4MpeDo+Du4BHC2cFFAVFEwU7ARuoSYgxJiaaOvYZ926XK2/73e7KHZ+/nM0MZvLl996vP5MKriNyM35bOqf5kG6QSYZHDX+6Nd2zNDMyzM7DzNzT4LXKY9K7/HJSRt3ng5w+w+CN0tkgjclKzwKLItcdncyEL1LPLJC+pH8aToq52kcCdkAaOBGkK1fDzOUUoJGDh8fjlbfBTOZWjdv3hseDRzMyLlhL5G6ggf2RVnJAOpI5ydfUVT9mF6SZdHwLJlOKLLqc8NOQdiam39s398BhwCmGPo9wAUgz9hwCa2RMYK6h71IfA9KLm+HwQE2WJmsfNCxe78zsaC2O02bo+DACyYFptXx2mBzuYc7cM+lHqTOstD4EV8kYxtmKbzgOjxdL58LhxE1Alr2GyePzB8FhhWySZkckSsZ1UwyldYCmcLtOaRmSfdNnvO2xDM+5Czn9JCDFESgFjgNpw3AJ0eN4WjKM2RfJkDnd0LfDYcgDQJxD0fC40jO+uKblWs9FQjKwU+4UOCw5c1fR+VSE0DBIzWQNs3BNPlo/EaXAvmnzfOvdcLhyT6olVZKLQJowjJLuJGeIHPDno+TJTZvXhNJxeZRe3K+v8z4VpAknaRpissLFZW1tbb5OmBk8oninZ+S4gxZDt3ZW0RXz/9y2bUUZ7SjzWmHac5ZYy0zG909oMHSmjPb1/7a8Z93zzzcM/L3ZT3cyMO0RVF7GXoWSYnQa7ZlSbejMYsfWLT04RsPvwcb0V/00QQ7+MpQMJ6dNsKbB0Jk2+pdeVurnBynBeBlVlf6q36/HpZ6bVm+yqy2pMlVtGzAu4fSOyt4TyABbn0euhtxB88mu/5LYEWON66AfqzJGZxx/9bCSk+DljWVp783dmWrfzGQ9S+i7T7/i3uyJ+4zZN3efY7OvH5dz0TWnHsP9fLM3vzJVK3px7FiPmnwJXkt7oW5Yqzt94XCwGpoKGbbtoWmeaacJQDfGXyxa7dnh2HuvnH7AHuY2STGNVA+OSr5uYKBh8G7H6/oqoD5YYZmDLtvU12d6OPgcILgIqWafG4F+nCh7sczJPe/QnXX4DbsVqsHaGfxxUGTcs2qFz7dpS8Pgh99oqAdWbxtduvnvgQaWgQ0r/Y5qE1W/Pdm59KtuAPoxRdV8xX7nzT41tUBB3Y5nK/07LuE1/9lHt3V2sgINYO5Lb2kZTB2mmS5d3RAXDq6nF5umutDUpyA1zJmtp7N1pQbHMfvKqaOMbZixVvStw7zmm2kfE/7kdfzF2XoJ/sUBU4apoFeu4z0F/hJZSXuhWQiTsdeoSbrqGZkfprXTfk72BQeMN3C9DP3zoOa/RO2PodfyH3+mYaqw8eBGjBPjAvyTg4Fmcaf2sst5I4GOHJlc9f6QqVmaW2ChKnybOKMOW1/0prUGf8Lc12W+ZpgKEvEgXtZmYlFnnsZhtuwpIAkM6L/dYa9RhmRgHauiNt0cu2fLmF5O9IYVPpgSTNVf3I8iMj+bHbrc6vMri+ZDCeRKrLORJBNPBbpyIUqBA6/UEsbvqi4xUx3cyKk70OiLU6HZt4y3/zWpBW1MxdYBnCh3CZf4qVgMdaBySWlhKZSDz8CTTJI6VPcHQ0hzltHn6t4OWVbagzlxHQyMF335oCdXBVPA2klvxPGKR+Bu9UaoA4te+BUWSNm67JjTHtPE/oUfBzozHaXM8apT/09DVbT9+XxEg9WJ7vTiih950Vel5L576S1x9zkbrG1f2f8L92u2loZ6kPfd23kLoQIzgRhTc8kr9NDIWfr/+3BJ9m/Mg+pojHhsuOcPn1UoOgvekoro1mBfCY5pXrK6lHYE6X6M9TvfC1+wfJqneMDPBeKyXz8nZuTXT1J0lg1aSqlM7gQ9V0MG/8E4bM80Aw2w9Gp6Q5zmPdvoNmhlP/6IeRdRD/Les5QWKx3wj0r2jU49dNxlT50368KjDKmYE1eIsTPSWZK7nv0BinL6nRTVHoIsFWFLxz1by6ABolur2J8ei9K2OiI5d6tjJS5hf/L6KqgH+e9b3sovgvLccRL4PzgU6ce5yW2Y8bB6+5nupa11dXWtS1sgFYDQF87H4d9pq4ToKR3v1XFeHB5oG+zJ8K1gUwPh/ynUg6KiVyzff6Rk6g8B81CehDdmn9nDUICTgi1LW5uQzW632+xuu62rtTbQ3lk2wBpjX4Wk6CtTEL1xW9TQ8boVDibhh+MNVVAX8n61WJbkK1zrT4D/geOQrpwDZLn2OkgQoGpb62tYuV3liKPc5q6pq6WqVmK8KgihuOgY/xOESUOv5jTncrxM4ne8rKwZ6kHxFxbLqwsKoDwzgOmciHRmkqbuCc8fTF2N21aOEnG5ba2hYN+KRo9VkLP5O/XkTGfZsqjoG+PzfVW/4JKwJ1cG9aCo6A2L5as8hQP+QWA2WblIZ0bvrqGo6gwsrXG7EAEnO6oN+gTV7WgZBvduSl6ZijUl0UL9miprfHJgHebSM7qQ943F8vUSBQ/+LGA25yLdmQakmUFo7m/6twZJYLO1UFCIY23qBZeIlx4Bb6CriSMgkn6HSVC5UOC/f2axsB78IiiH+ef7sUgCY1z4BwnN691ICinVqzbjklRLq/Rv0S6s9YnlFXo7TjonN7+AVL1g/scWi2KK5m5gLvsjA7hqlLrVE6H2JjnNw6oz7SJHM8bJOe+kM8hfEsLSHt6ejPu+4MNPBNd3/nus6B/DBeT3/9V/3wkZwWx1KVhqqRvJY6/3hyAHUWbD61JImzVzfpxYSa0xnInFGxuroWYKP3/59XxC3bzvLSzf5EE5rrsPmMkUZAjHZ6k53alamwsp4G6lhFnz/nBJbFUKsXQ4BTB4pQcT5S3b1IuTjNmKFr30BpmAW/C6JcwX+bKmfgpQZIgm4+KZpMJ39zjr7UiJGpfXCQm8wb61q38Kam9bJWvyXA1P5BDAPfFdl56Q3x8KqailFrzDxmfkty8tLK8Wy/pyNwMzORgZwyEqfHeqw40UEZo617VMN6bUoE6v5hsmSrZVkH7i3zh603ucgUC7H0Iv4w1XBpxQloXhC5yMz/Jfs4T5VDZYPxOYyG7IIHKVMzMhT5cNKVODvH6oO2WLB3CEVQ4vGc2txnx2JkDBlo7Wuqb6Lpb6prqOWiflUYrK38tbSF7qYd4okM3LXQIUGQJ9cUrsBsR4JsHQ7UgN7qUU1B2mwrm9oWTd8n6HoMudXhUWvc9HOVta6102Nj1sq3G5amrY/3I1dYfkZM97l5X3h/z5wktdPi9nbtA2DhnFVMXxZH+TDanB1uSE+sP4aGbbisYqwjHgE7GspVd0N9nchKPpstvru52UdCn1k3AqpjjB1Iusr1jCfGwthNLcCUxjfC4yiguVqqrO2ppypAZXlxHnO7RWl7X5xIZZqtazoq/r67Dbxf5+NndTC+WXrp+zvJ9g0wuL37ZE+Fba1E291CcciIwiB4hwExGjq6K8psUJTcTX14DxhibJX0m7rdUfkCyqRUy9kjjzI7xcKmfq84BZ7IwMYzIQ4cx4S6+zI1WU27oD0ESsFf3LVnfUu5AULne9lxLPziz5OmLqCbd63ocWRVNX7qQY0sWWQU4GQi6BMTywvgapwXzRPcHOOkJzArurllddmGlneTt/YYIn97UKU78fmMVkZBjjFLaCOmuRC6nAfNFD7UpnEFEJIsrnLJ8XwxgF8AMLx7uSpm5mefU8ZBizFBqlnC0q/ThW9JZ2aCJUqxspYrPVUuIpuTDf5cV7cgvesnB8UCkVqyu2R6ZBPo5lLyAgK36aqb3bplJ0F6r1Q2Mh8oRq/mI2F3Gvc/q+zYVnpQWEJ8fxvYypHw1MYh9kGEcAAUfDeNFb1Ipuq/d4oGkEamvU+Rr2rpATkuSxOVfOlSMaKTi+LCiC4hiQntl1yqRzcsZdPu58YuhxDDKMo0QrbNpF55LvxkMWgZRx17V7RPKwfPotYdDlJQvPZ1KmrrMnN3L/nGmxAZYTxoM4dkBGceBhCpsInLUuF1LGND+OzB4oU+7uoASif8uJ+1q+sNBG+PUE+nlyY3e+guyGutwc0Y9ViNJhyNul6hhVPt2t1VxiTQ/8ENmQWmzlgqpv/vecuO8ULiK6Z3heXwAlOH1XXdJt554wWmwYwRTRJyvOLTpVpd6V6y3W5opNW0nVjfHcSYQXT/6HfEj+JunJRRM0UIp5qddMDz1WxbjZVUg3lGfabk3qILV1wZCs5ovp3/HPbdJTpsxW2tHMqDV04viRx+VinGTyXcyi8z61qHDl5qa48PH8XLmuxdgCwP2QQYwZAQTcAmESl7rw4hRWQuWaoZkK/+otPrmVokS1Vz3cKUQm3zk+zI/PyX1uifJDPpTgwVTGjmdPRPLcS7Q/G8CVKp5gC9SpMHV3k1P2Rmcc/RjLTDJafY3LMf6NrobKkFeOMrb6EP+3I1Ou38cf4wVvvsx9lW2RvCVpz216NlLmGmKKTX8mSCyaIYI2RVO3ER1yQk2dvRj3/uWT7HVmx2FKMO53WNVoXlvuQlpwuWqdxBqCl3lt8xOys6z7rpCVS7oP+sjL56g7fPcEHCcjY5glWm3R7jW57C2U4tCC5IwpN2/Or5Zo1uV0J7Fz57swz/5ePlFSj/KZ1Pk+E2jnmL3Up1pyiJVlOrPDHqq2C/lDXfL/yC730j8UqqB/lrCibyFH2In1Y5EBVxWm3t6qWfS6ANEEzbXJsKOqlfH+3XeRj0RenuDSPYE2svbPRho48BhiYFVfnlW5O67d65K7Q1nNKeXpJHJvhHBkTfUIlLPOhrRhawqJltksr1iLBDkb/k8qpfz3Gdq89SuIgFtth/KpyAjGqd1FAKnaGmnbsrFnp0epp7VHfsS0ihtwVTe84PE0aRa9HoaIMptooP6JJcYPxVAMTSWX8ayRa4ebJh61C9KfbCDKTCiqOpKqabld3RRUINiPCdGF6wf5RULVZVAUsq9DG64uxp8geuGrg6IXEjkb5fzMbaoz6+drSrCQMVU20p2DR2h5p4eCdW6heUX6D70UVMLxOw5f2P2N0qsHDBYd1TpFRX/xhXjRi9+3xHi1sBKK8iRQxY3XI22Qj/DOQnpzHhDnEql7NNBdb7cLO407nAGoiONnzO1ylelx5EVv7jTgeOdFJwvqLC8tKSR636N8vUQi/36Xmnrp9FTK4UfwW0j1Zc50IMFcKAXl724qD2+ciRCeKShv6vBQHjXLYzdg4o0HYTczP5uqZiDRr9mRI4/3hcWk6Hz4/qIlxid5yWZnRu6VWrVkGvdTRiMdOf7Cscm8mu2hnLUddU1diNUcdRHTQ7KiN26MiD7g61TYJoT/roJGhGw19d6QtKWTKTme10RFV55inHI5ShVub/gJSC/2nbV38msinVTA461taWmp9YYU5wTJ7TOSCbfFFcsHRV/rsBqQnOHrvsqil35gifFGkXjQ9rB85i15/4scQbkY6cKYyUeNB7I8qnijhpws/pCCjZOzhvx+KLEqm7VxDY78ORfKG5CGRfY6JyREF3PkihZy4bt8Uf1sIM0NuUgPdtLtfM/NOUCnl3qUEY6dRUx9vdgLHF5+sYz6Za/OJo2mzqePyDudDNkK4CsWpUtdpncm6wi9+tpGj9Qj/T5n2oW7ATXMhAbg4x/2wb2BICMswPU9T+wY0ft8j7Vyke3OHyckZ4oWfWlRvtRnSkh+jkbvTbnP4SiUPGMO2X+U+sccDKA6+CPmVQ2SqjNVFdyKEW6BuxUqQo7MK8POVoYEje98GrZA7HNCMxXJmQZITnJlSr3vo4+bveN4oJr77oAGYHVsjsZkWx3V1oSGmq2xnZB4TaPqpnekAXtdO4RkcobskBGK/iJ39pMcBIScqHNz0/XJF12OzZk6FmhiBjSEzrbo7pjen+iqZshR3emg/+nFca/xLoaq8Gi71e18plio7lsLFkqLzrfFkwhF320i0pkxI5Jqn5k4+YaRQDOXQENgHOtxdN3nhjWNdFVFW0XQQZet/5H/zj/bZ1U962BTm4rl+/fEq2zvJfQ6F1XGvPdYWzzJ6SNM2PF3DB8NGCM4Od1iDIvpDbFDHC9bu37Nn9v6V23v4R5VjBp6sw5TTSS87y5eT/8uQfQCL/tZ0ZO7dE/jNUc7a3nXYXR2zqSRIGluh8bAVATijvEwsRe3efDzfwYZLV3Q/6qdt0LQD6F458y7eZAQXdGTIzvfD0BGMF3tTz/+5MdvHAtS4hFoEIxjGytrFFJxfp27tkllVbZOGDqRZP8kUfTSjy080QFHEvJO3zUX6Qo59nCFjN73XrDzYSB15kKjYOh+tmVKEoy30Nq2DPoD6lS31/ONumLltM+KIZF7T+ALYU6OFD0H6Qs5hTL+OLFc+rTJ5xw5EujEKdAorF56TQMukdR8JbfCX5vqNqRIjdiEevEPg+XTQigcYZTtjiTi9N2R7pAbQiZddvxo/u4eM/G4cedPP3VCFtCT26BhWL0O/3IsKjvGDf1JPIfubK9XVN3l7vhDYr0Uywfc+U1MO8T4Xig6kZE7BBnDZSCePXbbcccdp0zYIyp2uogOIdNIr2wgZOeu9+1/OCChuT45mvJ/WynRRXJEMyzZOcPzq9B9j+be9a55kyUXZYb+8R6GaaYDa3t5Hy7mz21cQ7cxMLkpdXnVy9nh9JCY6O8So8pEj5xMH3Rile1iZBAnAPO4BxqK1eujmzdv7MVRGpb/3kcnvSg6wNhkm7NZzf1ST3Lx2RdyPyxPLHdDEK2nq+pzSAvRDQjZSNnLHPT8n1au/W379t/Wrurf1Ei3pbAbnOq2S2XmuObsBM3Jlc+ko5bP/S7INkcmdM6MMmyI/GSglaGXnCEWfzY6HHRVkHY4gmVEtl276lK27nKXt/xBBGtEvYWrqAh/F+ImlgXZmYQeuaOQUYwD0qSdpceoroZ6QLWUi4XrbAtnHaTknmHjyy3CKrt8x1RCN+w5yChyQLIMmYKLkVCwTrATuMZta2ohe/iESZh384iMPPe7oDDbdDeIMgsZxbnAPObB9MNJtUS2f0eELy932dz2rroWcvO3WDz+RTH5u0CKvkhE9NvM2Oo4CZjHYzAdGdzzz2KrQfWt3V6qHcqQ/5no4b3gCwsBN+pG8Ajxtrkh7AjM46Q7YFoSoDy17APfrUs7WhgPRYWgLHlfkdsmyJ0zPC+yaVoB80CUw5FB7DAKmMeI02Ga4nEGqPb28Gv+HqhE3ndkYwy5XcoS65AmueMkE0TPBmZyEMx8eC/9lcoC8hVGZdETi2zZyCBmATOZCTOfoqJXyNOd3xipLHpi6v0pZBA3ADN5GmY+hUteIraGEeOLsqLfbMIi/gMnADO5GWY+nPP+ZeEiQb1VWfTESeULkDFMA6ZyNcx8OOf93TzhZ5IPoDBku9WEx3VmA1N5EGY+kW3vL72wgDwAuPk2peTM0Sb0zagbQUuTgvqQYFHhO2LtzUWVrHunnIa97lrAoWYYIQ06KLjG94znP/bu7SeKKwwA+McucltRCyqlIGwrKFTEegGBoqLYWluxYIsUrdfWS63ai72350wyO8k2szuTeduEPuwTbyRye4BAAqEJJMv1RdEE+RN85LlsbZtymJmdmZ3dPTPj74WEN/Jx5ty+830cmpQr/irOM+s8Ii9ckpMW+QbEi4p3TTQJDhKJUvIZFFFj669Wf0l80nsuJFmeZY/kNBOXiZbZSkcz0bvXMCLcle+pZNUbNqeczvjmiNeqRFs29XdNjQnvd73HBcl2A9mdb0auM49vmSHIFgAnGzt4dmCz1YMWLzfquvieMSO+sEwvH4Js1cgmchjmY5PVQPJdQ3a3GvRBHy+7jyPN+BDhJ4Xe9ta8SXfOns33bImVW9IT5KsS3AWS+7CVD+NeuHMB2Vyoh3iMKn+XTlaZe+EsrJP5noXPZZxyoy4FAhIihIP9DKPhQO5CJ6y3swyb5Xw6pMQ3yIFE2a/7qEg+cDkBcopMi/olSI2byIGItbviju0qyDrgtV4OLFGXwHkC3CNGxqwPEW6DPNc5i+7QLZ0FbQiZP0GaJ25fo1O6kvxXcLw+AV1eruRMuGHXkELRDMrctTg+JZBCl5HTBLsealvH3QU16TnYuKytkEr3kcPwMpt02YQq9DWoS3/V6Ee+zQXGOPcRo37kI1ZN53FNsSOT92kN1i8jgdP5huKSyoraty9urqvb3FZwpmQDyDqOHIVnZxk5vesO7q6AFuktF0uxHt6T2ZAI7qL6j7/IJb89GZv3bnBoGvT/hIJjjLYp/Tpo5Nm25YOCiwfPl+FYSusqisB8nuL62k2KU01ZzkYHXrStHejEfk2xHPDRQtDHvbGo5OczH+8+tXlXrvd82nsZGWXRoZaRkZa1Z1ddzqvvlmxzgelcO898kYVjqHU5L0/uf8JiPyNrXkRr3YgrFO4N6enpW4uLi99MTy/MgwQpfqctDWuRVey4lCmFga5eeqQR6FaYX+A1fs5/FTmH4kBfkvm6Uyy9pS4jrnSNz5Bj8OwcQ1CqC3sDqJX+YBfWL9fjsDzofwUUlu7M9Loz2OtAp8LK17ExZ5x5EiutaZmumv18zA00unSuDBuV5V5bLdQh/OFJRt4UGfQvgT7ulsPmvZ9ytyJniDxntH7d9wNtsk9n4PjUOvBQjvcNMozGtfsJoM3J+BMyqxyYPhMgtmtqly33gS7FXhw/r8txly585Bkjj3zrRt8mfSc2wyvbHfa6CUm+rkVGwTj51u0y0KXKpKA77fydU/64L/aQ2XEfAlXysUlBd1ptMfl8GbLNxwu/AV3exqbwupxVfEZiFxhF8yKP/oe60ziXNzHVSF1NyM4kMTTNKBklW7ccArpsxeaodNRLF55jRxhFC+RL5mtAl32JKjvcaefnqwJxFKfeo6kpD+jyCTZD2VZnpcqxs4yyeXKg3wLKVGIzvOGoVGhiERfrfq2VsoMZgJ+xCSpAzlfIniS25yGjaLGLXLrfA9rk4/jtTnm/rmSSWL6XUTZFHsYdywba7MRxawMFPyEbklhhklE2F0FrUDijAxwwt8yJ7XPl1GPePR9BhOMuoI7Lm8AeMb8iu5HYP1Ri3ieR33b69uh/24zjssvlpPpiPDs8zSga8fsktAaNyRPxt5R43eWgqkO8sDLfzSh6wgYlRDoLNCrBcahzUiVwPrAyxSibXeHIbzu1ye7x9PA+5aRSsZKffcoomuyJCDxapx3odA4bVQAxZdvm3YPkEycYRaN+lkfr3QVKvYENet9BBYJ5ITI8xCgal5vO6dyuvZBdig3ZC5o0IxuQ/Cuzi4yS3oVIgEcybgO1arER+5xUgoYNDjCK+kMskvUN0CsT61e6zTll/yVuZbBP7dMuSkhOawNQ7BTWa1M2aOaxejVB1qeyap+eX5H/tNNehOCSWct2G962Sf7IoMoKbsLPIlnUbtGN7toqHdPXhRdY/xNG0cPZSFBCcmj/uK8qxjpkZYJeJ5Al8cgXmZpWWcFJrMAjWdRlPcs4jTVrywPd2i2ZJMkHI4P9jKLFZxFRQvLoe8ckw5OLNWpxSg8niYtIA4yyEYkN8EhJObXHMvrXcnu2OqQ0NC+wwaeLKov2KVZlmNNYgkDOSazBq2CUx1K15aKT+ZzK1rx7nFOZzRF9r9GNr+C9RWBc9VFkFTwSWZXJfHHsscByElJkiQldY4Pf15zR2yUa8p4JRsHQwEyXyPp5pIzWbBlZ7iqsYtM2iNMtZAnh8Nwj2T350OjzqR6OZf0CUtfaCdahEvXSfDDCitVDua6BvrGxvsnp7qjpyb5HIxNLj2eWuwLiasBDKLbPwFLasKyMB6CXdZ+88MgvBrnfpeGuqGEJcUHRx7KiPyAgDayziPvPB3i9tAo3xGSnu3VBEEIBjvNHcRwXEgSkQwdYzkayTuiOvS4wjdtSGzcD6L5DV3apDv8nq7YITNVg91JjP4JFpdcX1Bysqsl5N9MDZuu0d1WSE1Y4fY3pZdT1KKez0nPq2Tjqx+/AS/IarJ4+9TLmBrgtmlNhasxdO1s+evviuYIzmeAQdqxWoCvm+3LS8L9yW+grVbHWy87LCg65QbNKL3HiWbARnOAespdmD2j1lhev94MjvvL2avPyDWjlPoXlbToC9tduo63bVdAq8zxWVFUCtpdtm63bfbPqc+7YAnZ3xx5j/fOvdV1iOj3stoj6oTvxxZyU+xbYW+qjLggoPlfML8NbtRNsLcVRD7JBkWV9fk4xK0ZA6hpBsyNYs9ftvYFLYdQ51tf1dGz0+czcoBQO+nyr0RfFoP8fQVFc/Y1/WFILe/m3oNn2UqzDRVsf16Tm+kXws6w0M7LIvNA7NDLwZHxmdm55oafrbz0Ly3Oz4yOjs0IYKeoAHX7A+uzeDvaV9BQqgRPZYNfMRDcj5+E/mKihqXAIKWnaDzrUY91O09YQwkSe71CyCAG/jxX55fH+h0xso4MREfFIwVXQw52F9UurBPu6ghIuzAWjeczhnqmnI72MBr1PetRCXt4OulRgQw7beP92F5mLjDfLBofnp549GelbZDQZmwlHgsohR/cSVraNVHMA7KoRmU8QAv7V4e3zSwszT/q7Gc16lxZEluORoh/vJLNTTq1tE+/2H0UmCQc4fzTYrE8MrA7vxwP9vYwO3ROzv7M+tXfIrWeTXF89w7ZTe6FZi/ie+YWF5T9nZ54OjD4ihreWiEssyyE1HaDfRhyfHbY9rLmMTBAajo7tacaAyaWpmBFHzZ1gQD2OV45dU6quITOEOIHvmp+dWNQ1xEfGFwIxI46azoIhBThuGT+DPVUfR2YQQlyQjQxPaF23jTz9c1hkxQCK4cI9MGgTNkGVtdfx2QcyV73pTuiOXVyZinkC83Bo4PGyJLI+TkAxXXaDQe4sbIrXwJoK91V8kZuG/1a6qXaLm0ydu4BMwgusWkmwyf6lx39GC0uIRMAVfFcNhm3EJvEWgeWkt9RlkFNVTjHxX9GMzCIFV+aHert7u1+M6t7u6b6hsf6JpeePp5YH/whG71RDSJvy/TS0K19VC5biyq/BsmqIajb3kXnEIL96ZTa4ar5nuIsPBTh/9ALdJwa5MNKs6XbKu1z+J8tCg337a2lY0UfEbWszMk2Ii96NR63+5AKBUFhAOh1rpKC1qQVndterWNVhYrBf/xzRovV7iFsRNtVBSyRYbEnT24A9j5KO+0TIDdqGTVYPtMtuwxq8Q7Z+oSArngi5Yell2GQ5QLctGViTM0BoPIpSqukaACVNjNfbQXMylSfHeL8IVwdKnfLbYJ5N2HSv0JsnnZmFtTsCpIZUTe3NX4OZcnACfAp0+gDrUboB1qn+BSXflWow1wOcCO8ChfLqsD7nQEb1Vyipjt0sBLMdwQnxAKiTWYr1yodUh72ZuDxNfedqS23d9hpZk3pAVudllAytHd9CYmzGiUHZam43NqISFBTebEUJ9tNtDyTKSWwacpTQY8MubEguKPJcP4ESp/xWAyii7iCWysVcpuE5rARUtH95FCXC8Y52SLAdODF2AS3ewIbtBlWuxmbTI351PyTexzgxDgIlKrBxeyCWznuHkGkO3f0QkiITJ0Yd0GE3jkcxxFZ904y4H/3lfifEQvv6/X2ggSfOvy4fNKm+13wBGXfhRMdneZBMW3BC5AMFsg/i+JwErRoarzQZGuHNHWcbQAsLDPU3IfW2e3GcToMe7bdu6An854cuf79f7win+SiWhsX7gTSc+KCTqv9q726bmgaCAABvaEOT0pYGSlvEUlsrhb6IFktH69uoCFb4ACiioDPq6IzjOH5zRj/sb/dtdBQtTXqbvbskzw9gJlyuSXb3dl89+XLh2ajtfeXjk+uXbZCoj78FqV6uYyDXop907fL1D+/f3L1w6+Yfy//s5u0Ldz6+ePvy9WMFjvt2akhOfs8CijXHDRAyMf/04uPLP1y8+HReqTk6y0itJ/365oxg3Lu+mUojsX2QLF5GAmUFfod9s4o/BCfJZjeRQh2CLImkMiBZBUmsgjbi2fuD5ZWFKXAvbyClz+CVkvdwCTRhPmzhT+WBLdCSQoQVA6keIo17oAV7Hf8wOetxWHkw/lkLSKOmQlRxlIkbD4yxgwupSaRyYIJUa0hjD5SXLxr4r2Vw6zNS2QapBkhE/VkWy8LZrr1gBGZSW0ijBYqzMziEZYJLZjkQB1z2kcgOEJnIx22gN5PGoYrgVjYIJTMxC2kYJlBI3etajuOUkscxoNXHU8zxbpIcjEW5RPE+EOjs4m9O0QRCWaosZwuFPYQxKPiNTnLznsG/1B4CnSOqL6gEipoGyfaQxiYIy7fwpDUTiDRqZEnhNoo5iINkLVQl7J4z8F9HQGSHMP1R1zz/bDpIogKiElu+Fowe4QgLXDtlA0So1Drp0K/CnSUgYRqUBWsdR+uukc8Vya/ZTX/jfM9pL2FR61h1VpHAzCYOs8n1kfIcPNjW9cX9u0OkULZBzFUcrsMUdywydP7vggqOkcIyiDEtgT9OlQlP+3/m5R0ogWSnT8ZpgzInF4NAF0dbAC9iafSqAGqoIoEiiGlM+l41ukRfyxK39EqhE9f1pkBMEU/1iKlQpOvrUYFJ6TEZ0uP2BRBjHvhfb7nmx1Mq56BrGROU0UBxcyBme/TDVlgaXaiC51XXsKUQwFRTfiPrkQuyzNTJeR28SlnoxrT0FMvfpqXnVFcZAvtTPXQhDZ6ZLRypp9wZkDPSN3qfoWPDjOXb/VsfteRVUE4VBZ0FMfFJHOmQqb3rCoxh1sHhKougoIbsjX6f42Ckafh4MbFHQ+7b0kDVWZtLghudI1bWZFr0EozHni2ka/gXa3MjB8oayN3oOZ5nyJbvL6X5S+0z9XfT093d5P5Gdk6lXr/UJ9nO8hRmtkFMx3UtSEhkhIJxorosPygpfcpaeLRxfB3gWY00iElp1NmNhV0WSK+JWkF3OjyL7qg8MI3UIxyTE2cru6+CkE4AG6gQhUf4mxjHLJ7miglN+kOMJD0Ua9kg6hJTQWFCp2JVHvNlHMcO4+1mxXgWvaf297X0F/g11lZmOaZaES2mm8tLsFZBWHyL6QhNLkA9VMjMySnbn+Wqsm7oODXLdzvo1Xne1ie7IKKjX8kqh76MGu4MVxujRk2rsSpcJtLohRFnDhA4eZ4ZuQkIk5TBPjAyy1eK1wtae1si59lHzgz4xlm9c51NCJlFdO0GUNhFD+6zXNsChM0sulSQ0MKuyBGJqED4rKIrlgkUzqIXmxwFQlchhBIOulCVcTa+whDmb0qfnyRFvoQj9YFGEb1Ig6BKlFgdP0pTzgONXd4WJ+Y5HMFR7MgZo0OmmNXMOfSiJrwinQM8VVrhQnXf5SssI+QaNeYiTGhYOJRVOA/hdugwTBPLojcJEBabxv8oZworYd7kv8wU8f+MhrTKDZKd2C45+EPNKTeXjuqDlUudGYj8ZK5Th+JOKsiZ9GencnPfNPJBHhE6NnNwQPqFftKn4I531NlU9pOP42ZK0aIryjxOWv7Uj80b0aKra+rsynq3t1XbiwOlnG6Tp8NnIj8PtFa9n4qO6O4Yf4oq0kNkA70xFGq5GGE6QLcGEe2t6zWpMkIhid6cgYj2+nrNIY7IWPQGRLSX1HHkTYS1000bIvq7il5YUSI0COKOt1YnkSC4H32kh1Ad3WraEAmIJLpzLrz16AG0gW5Uon0eKLmlqAtMCC2u4ak2o5/2IFqoGziEsR9VTgTVxGpxaQtPKhUWw3lyODzMG+1iv9vq9XrnSplPhe3FUCdYvgKoLp6WFjy2ewAAAABJRU5ErkJggg=='
          img.onload = function() {
            that.initData.context2d.drawImage(img, arr[j].x - 50, arr[j].y - 50, 100, 100)
          }
        },
        pen: function() {
          var lineWidth = arr[j].size
          var radius = lineWidth / 2
          var lineColor = arr[j].color
          that.initData.context2d.beginPath()
          arr[j].msgArr.forEach((val) => {
            that.initData.context2d.lineWidth = lineWidth
            that.initData.context2d.lineTo(val.x, val.y)
            that.initData.context2d.strokeStyle = lineColor
            that.initData.context2d.stroke()
            that.initData.context2d.beginPath()
            that.initData.context2d.arc(val.x, val.y, radius, 0, 360, false)
            that.initData.context2d.fillStyle = lineColor
            that.initData.context2d.fill()
            that.initData.context2d.beginPath()
            that.initData.context2d.moveTo(val.x, val.y)
            that.initData.context2d.stroke()
          })
        },
        eraser: function() {
          arr[j].msgArr.forEach((val) => {
            that.initData.context2d.clearRect(val.x, val.y, arr[j].size, arr[j].size)
          })
        },
        cloud: function() {
          that.initData.context2d.beginPath()
          that.initData.context2d.moveTo(arr[j].x, arr[j].y)
          that.initData.context2d.bezierCurveTo(
            (arr[j].x - 20),
            (arr[j].y + 10),
            (arr[j].x - 20),
            (arr[j].y + 35),
            (arr[j].x + 30),
            (arr[j].y + 35)
          )
          that.initData.context2d.bezierCurveTo(
            (arr[j].x + 40),
            (arr[j].y + 50),
            (arr[j].x + 75),
            (arr[j].y + 50),
            (arr[j].x + 85),
            (arr[j].y + 35)
          )

          that.initData.context2d.bezierCurveTo(
            (arr[j].x + 125),
            (arr[j].y + 35),
            (arr[j].x + 125),
            (arr[j].y + 20),
            (arr[j].x + 110),
            (arr[j].y + 10)
          )
          that.initData.context2d.bezierCurveTo(
            (arr[j].x + 130),
            (arr[j].y - 2),
            (arr[j].x + 100),
            (arr[j].y - 25),
            (arr[j].x + 85),
            (arr[j].y - 15)
          )
          that.initData.context2d.bezierCurveTo(
            (arr[j].x + 75),
            (arr[j].y - 32),
            (arr[j].x + 40),
            (arr[j].y - 30),
            (arr[j].x + 40),
            (arr[j].y - 14)
          )
          that.initData.context2d.bezierCurveTo(
            (arr[j].x + 14),
            (arr[j].y - 32),
            (arr[j].x - 10),
            (arr[j].y - 30),
            (arr[j].x),
            (arr[j].y)
          )
          that.initData.context2d.closePath()
          //填充渐变
          var grdCenterX = 260
          var grdCenterY = 80
          var grd = that.initData.context2d.createRadialGradient(grdCenterX, grdCenterY, 10, grdCenterX, grdCenterY, 200)
          grd.addColorStop(0, '#ffffff00')
          grd.addColorStop(1, '#ffffff00')
          that.initData.context2d.fillStyle = grd
          that.initData.context2d.fill()
          if (arr[j].fill) {
            that.initData.context2d.fillStyle = arr[j].fill
            that.initData.context2d.fill()
          }
          that.initData.context2d.lineWidth = 1
          that.initData.context2d.strokeStyle = arr[j].color
          that.initData.context2d.stroke()
          // var maxWidth = that.initData.context2d.canvas.width
          // //如果超过边界从头开始绘制
          // arr[j].x = arr[j].x % maxWidth
          // //云朵高度为宽度的60%
          // // var ch = arr[j].toX * 0.6
          // //开始绘制云朵
          //  that.initData.context2d.beginPath()
          //  that.initData.context2d.fillStyle = 'white'
          // //创建渐变
          // var grd =  that.initData.context2d.createLinearGradient(0, 0, 0, arr[j].y)
          // grd.addColorStop(0, 'rgba(255,0,0,1)')
          // grd.addColorStop(1, 'rgba(255,0,0,1)')
          //  that.initData.context2d.fillStyle = grd
          //  that.initData.context2d.fill()
          // //在不同位置创建5个圆拼接成云朵现状
          //  that.initData.context2d.arc(arr[j].x, arr[j].y, arr[j].toX * 0.19, 0, 360, false)
          //  that.initData.context2d.arc(arr[j].toX - arr[j].x * 0.08, arr[j].toY - arr[j].y * 0.3, arr[j].toX * 0.11, 0, 360, false)
          //  that.initData.context2d.arc(arr[j].toX - arr[j].x * 0.3, arr[j].toY - arr[j].y * 0.25, arr[j].toX * 0.25, 0, 360, false)
          //  that.initData.context2d.arc(arr[j].toX - arr[j].x * 0.6, arr[j].y, arr[j].toX * 0.21, 0, 360, false)
          //  that.initData.context2d.arc(arr[j].toX - arr[j].x * 0.3, arr[j].toY - arr[j].y * 0.1, arr[j].toX * 0.28, 0, 360, false)
          //  that.initData.context2d.closePath()
          //  that.initData.context2d.fill()

          // that.initData.context2d.beginPath()
          // that.initData.context2d.moveTo(arr[j].x, arr[j].y)

          // that.initData.context2d.arc(arr[j].x, arr[j].y, arr[j].toX * 0.19, 0, 360, false)
          //  that.initData.context2d.arc(arr[j].toX - arr[j].x * 0.08, arr[j].toY - arr[j].y * 0.3, arr[j].toX * 0.11, 0, 360, false)
          //  that.initData.context2d.arc(arr[j].toX - arr[j].x * 0.3, arr[j].toY - arr[j].y * 0.25, arr[j].toX * 0.25, 0, 360, false)
          //  that.initData.context2d.arc(arr[j].toX - arr[j].x * 0.6, arr[j].y, arr[j].toX * 0.21, 0, 360, false)
          //  that.initData.context2d.arc(arr[j].toX - arr[j].x * 0.3, arr[j].toY - arr[j].y * 0.1, arr[j].toX * 0.28, 0, 360, false)

          // that.initData.context2d.bezierCurveTo(
          //   Math.abs(arr[j].toX - arr[j].x  * 0.08),
          //   Math.abs(arr[j].toY - arr[j].y * 0.3),
          //   Math.abs(arr[j].toX - arr[j].x * 0.11),
          //   Math.abs(arr[j].toY - arr[j].y * 0.3),
          //   Math.abs(arr[j].toX - arr[j].x * 0.3),
          //   Math.abs(arr[j].toY - arr[j].y * 0.08)
          //   // arr[j].x - 40,
          //   // arr[j].y + 20,
          //   // arr[j].x - 40,
          //   // arr[j].y + 70,
          //   // arr[j].x + 60,
          //   // arr[j].y + 70
          // )
          // that.initData.context2d.bezierCurveTo(
          //   Math.abs(arr[j].toX),
          //   Math.abs(arr[j].y),
          //   Math.abs(arr[j].toX*0.19),
          //   Math.abs(arr[j].toY*0.15),
          //   Math.abs(arr[j].toX - arr[j].x*0.17),
          //   Math.abs(arr[j].toY - arr[j].y*0.17)

          //   // arr[j].x + 80,
          //   // arr[j].y + 100,
          //   // arr[j].x + 150,
          //   // arr[j].y + 100,
          //   // arr[j].x + 170,
          //   // arr[j].y + 70
          // )
          // that.initData.context2d.bezierCurveTo(
          //   Math.abs(arr[j].toX - arr[j].x*0.08),
          //   Math.abs(arr[j].toY - arr[j].y *0.2),
          //   Math.abs(arr[j].toX - arr[j].x*0.11),
          //   Math.abs(arr[j].toY - arr[j].y *0.4),
          //   Math.abs(arr[j].toX - arr[j].x*0.22),
          //   Math.abs(arr[j].toY - arr[j].y*0.2)

          //   // arr[j].x + 250,
          //   // arr[j].y + 70,
          //   // arr[j].x + 250,
          //   // arr[j].y + 40,
          //   // arr[j].x + 220,
          //   // arr[j].y + 20
          // )
          // that.initData.context2d.bezierCurveTo(
          //   Math.abs(arr[j].toX - arr[j].x*0.26),
          //   Math.abs(arr[j].toY - arr[j].y *0.4),
          //   Math.abs(arr[j].toX - arr[j].x*0.2),
          //   Math.abs(arr[j].toY - arr[j].y *0.5),
          //   Math.abs(arr[j].toX - arr[j].x*0.17),
          //   Math.abs(arr[j].toY - arr[j].y*0.3)

          //   // arr[j].x + 260,
          //   // arr[j].y - 40,
          //   // arr[j].x + 200,
          //   // arr[j].y - 50,
          //   // arr[j].x + 170,
          //   // arr[j].y - 30
          // )
          // that.initData.context2d.bezierCurveTo(
          //   Math.abs(arr[j].toX - arr[j].x*0.15),
          //   Math.abs(arr[j].toY - arr[j].y*0.75),
          //   Math.abs(arr[j].toX - arr[j].x*0.8),
          //   Math.abs(arr[j].toY - arr[j].y*0.6),
          //   Math.abs(arr[j].toX - arr[j].x+0.8),
          //   Math.abs(arr[j].toY - arr[j].y*0.3)

          //   // arr[j].x + 150,
          //   // arr[j].y - 75,
          //   // arr[j].x + 80,
          //   // arr[j].y - 60,
          //   // arr[j].x + 80,
          //   // arr[j].y - 30
          // )
          // that.initData.context2d.bezierCurveTo(
          //   Math.abs(arr[j].toX - arr[j].x*0.3),
          //   Math.abs(arr[j].toY - arr[j].y*0.75),
          //   Math.abs(arr[j].toX - arr[j].x*0.2),
          //   Math.abs(arr[j].toY - arr[j].y*0.6),
          //   Math.abs(arr[j].toX - arr[j].x),
          //   Math.abs(arr[j].toY - arr[j].y)

          //   // arr[j].x + 30,
          //   // arr[j].y - 75,
          //   // arr[j].x - 20,
          //   // arr[j].y - 60,
          //   // arr[j].x,
          //   // arr[j].y
          // )
          // that.initData.context2d.closePath()

          // var grdCenterX = 260
          // var grdCenterY = 80
          // var grd = that.initData.context2d.createRadialGradient(
          //   grdCenterX,
          //   grdCenterY,
          //   10,
          //   grdCenterX,
          //   grdCenterY,
          //   200
          // )
          // grd.addColorStop(0, '#8ED6FF')
          // grd.addColorStop(1, '#004CB3')
          // that.initData.context2d.fillStyle = grd
          // that.initData.context2d.fill()

          // that.initData.context2d.lineWidth = 1
          // that.initData.context2d.strokeStyle = '#0000ff'
          // that.initData.context2d.stroke()
        },
      }
      switch (arr[j].drawType) {
        case 'rect':
          drawTypeFn.rect()
          break
        case 'line':
          drawTypeFn.line()
          break
        case 'circle':
          drawTypeFn.circle()
          break
        case 'delta':
          drawTypeFn.delta()
          break
        case 'ellipse':
          drawTypeFn.ellipse()
          break
        case 'arrow':
          drawTypeFn.arrow()
          break
        case 'polygon':
          drawTypeFn.polygon()
          break
        case 'font':
          drawTypeFn.font()
          break
        case 'signet':
          drawTypeFn.signet()
          break
        case 'pen':
          drawTypeFn.pen()
          break
        case 'eraser':
          drawTypeFn.eraser()
          break
        case 'cloud':
          drawTypeFn.cloud()
          break
      }
      // that.isMove = false;
    },
    mouseMove (e) {
      const that = this
      if (that.isMove) {
        that.initData.context2d.clearRect(0, 0, that.initData.canvasWidth, that.initData.canvasHeight)
        var moveX = e.offsetX ? e.offsetX : e.targetTouches[0].pageX - that.canvas_front.offsetLeft
        var moveY = e.offsetY ? e.offsetY : e.targetTouches[0].pageY - that.canvas_front.offsetTop
        var moveWidth = moveX - that.initData.initLeft
        var moveHeight = moveY - that.initData.initTop
        if (that.initData.isMove) {
          switch (that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType) {
            case 'rect':
              that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                drawType: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType,
                drawTypeNum: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawTypeNum,
                fill: that.initData.drawHistoryArrData[that.initData.chooseIndex].fill,
                size: that.initData.drawHistoryArrData[that.initData.chooseIndex].size,
                color: that.initData.drawHistoryArrData[that.initData.chooseIndex].color,
                x: moveWidth + that.initData.initLeft - that.initData.relPosX,
                y: moveHeight + that.initData.initTop - that.initData.relPosY,
                w: that.initData.drawHistoryArrData[that.initData.chooseIndex].w,
                h: that.initData.drawHistoryArrData[that.initData.chooseIndex].h,
              }
              break
            case 'line':
              that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                drawType: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType,
                drawTypeNum: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawTypeNum,
                fill: that.initData.drawHistoryArrData[that.initData.chooseIndex].fill,
                size: that.initData.drawHistoryArrData[that.initData.chooseIndex].size,
                color: that.initData.drawHistoryArrData[that.initData.chooseIndex].color,
                x: that.initData.drawHistoryArrData[that.initData.chooseIndex].x + moveX,
                y: that.initData.drawHistoryArrData[that.initData.chooseIndex].y + moveY,
                toX: moveX,
                toY: moveY,
              }
              break
            case 'circle':
              that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                drawType: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType,
                drawTypeNum: that.initData.drawTypeNum,
                fill: that.initData.drawHistoryArrData[that.initData.chooseIndex].fill,
                size: that.initData.size,
                color: that.initData.color,
                x: moveWidth + that.initData.initLeft - that.initData.relPosX,
                y: moveHeight + that.initData.initTop - that.initData.relPosY,
                r: that.initData.drawHistoryArrData[that.initData.chooseIndex].r,
              }
              break
            case 'delta':
              that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                drawType: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType,
                drawTypeNum: that.initData.drawTypeNum,
                fill: that.initData.drawHistoryArrData[that.initData.chooseIndex].fill,
                size: that.initData.size,
                color: that.initData.color,
                x: moveWidth + that.initData.initLeft - that.initData.relPosX,
                y: moveHeight + that.initData.initTop - that.initData.relPosY,
                toX: moveWidth + that.initData.initLeft - that.initData.relPosToX,
                toY: moveHeight + that.initData.initTop - that.initData.relPosToY,
              }
              break
            case 'ellipse':
              that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                drawType: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType,
                drawTypeNum: that.initData.drawTypeNum,
                fill: that.initData.drawHistoryArrData[that.initData.chooseIndex].fill,
                size: that.initData.size,
                color: that.initData.color,
                x: moveWidth + that.initData.initLeft - that.initData.relPosX,
                y: moveHeight + that.initData.initTop - that.initData.relPosY,
                toX: moveWidth + that.initData.initLeft - that.initData.relPosToX,
                toY: moveHeight + that.initData.initTop - that.initData.relPosToY,
              }
              break
            case 'arrow':
              that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                drawType: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType,
                drawTypeNum: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawTypeNum,
                fill: that.initData.drawHistoryArrData[that.initData.chooseIndex].fill,
                size: that.initData.drawHistoryArrData[that.initData.chooseIndex].size,
                color: that.initData.drawHistoryArrData[that.initData.chooseIndex].color,
                x: that.initData.drawHistoryArrData[that.initData.chooseIndex].x + moveX,
                y: that.initData.drawHistoryArrData[that.initData.chooseIndex].y + moveY,
                toX: moveX,
                toY: moveY,
              }
              break
            case 'polygon': // 六边形
              that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                drawType: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType,
                drawTypeNum: that.initData.drawTypeNum,
                fill: that.initData.drawHistoryArrData[that.initData.chooseIndex].fill,
                size: that.initData.size,
                color: that.initData.color,
                x: moveWidth + that.initData.initLeft - that.initData.relPosX,
                y: moveHeight + that.initData.initTop - that.initData.relPosY,
                reg: that.initData.drawHistoryArrData[that.initData.chooseIndex].reg,
                toX: moveWidth + that.initData.initLeft - that.initData.relPosToX,
                toY: moveHeight + that.initData.initTop - that.initData.relPosToY,
              }
              break
            case 'cloud':
              that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                drawType: that.initData.drawHistoryArrData[that.initData.chooseIndex].drawType,
                drawTypeNum: that.initData.drawTypeNum,
                fill: that.initData.drawHistoryArrData[that.initData.chooseIndex].fill,
                size: that.initData.size,
                color: that.initData.color,
                x: moveWidth + that.initData.initLeft - that.initData.relPosX,
                y: moveHeight + that.initData.initTop - that.initData.relPosY,
                reg: that.initData.drawHistoryArrData[that.initData.chooseIndex].reg,
                toX: moveWidth + that.initData.initLeft - that.initData.relPosToX,
                toY: moveHeight + that.initData.initTop - that.initData.relPosToY,
              }
              break
          }
        } else {
          if (that.initData.chooseIndex != -1) {
            switch (that.initData.drawType) {
              case 'rect': // 矩形
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  w: moveWidth,
                  h: moveHeight,
                }
                break
              case 'line': // 线
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  toX: moveX,
                  toY: moveY,
                }
                break
              case 'circle': //圆
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  r: Math.sqrt(moveWidth * moveWidth + moveHeight * moveHeight),
                }
                break
              case 'delta': // 三角
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  toX: moveX,
                  toY: moveY,
                }
                break
              case 'ellipse': // 椭圆
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  toX: moveX,
                  toY: moveY,
                }
                break
              case 'arrow':
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  toX: moveX,
                  toY: moveY,
                }
                break
              case 'polygon': // 六边形
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  reg: 120,
                  toX: moveX,
                  toY: moveY,
                }
                break
              case 'pen':
                that.initData.msgArr.push({
                  x: moveX,
                  y: moveY,
                })
                var msg = that.initData.msgArr.concat()
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  toX: moveX,
                  toY: moveY,
                  msgArr: msg,
                }
                break
              case 'eraser':
                that.initData.msgArr.push({
                  x: moveX,
                  y: moveY,
                })
                var msg = that.initData.msgArr.concat()
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  toX: moveX,
                  toY: moveY,
                  msgArr: msg,
                }
                break
              case 'cloud':
                that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
                  drawType: that.initData.drawType,
                  drawTypeNum: that.initData.drawTypeNum,
                  fill: that.initData.isFill ? that.initData.background : '',
                  size: that.initData.size,
                  color: that.initData.color,
                  x: that.initData.initLeft,
                  y: that.initData.initTop,
                  toX: moveX,
                  toY: moveY,
                }
                break
            }
          }
        }
        that.drawArr(that.initData.drawHistoryArrData)
      }
    },
    mouseDown (e) {
      const that = this
      if (!that.isDrawing) {
        that.initData.initLeft = e.offsetX ? e.offsetX : e.targetTouches[0].pageX //获取鼠标起始位置
        that.initData.initTop = e.offsetY ? e.offsetY : e.targetTouches[0].pageY
        if (that.initData.drawType == 'font') {
          that.isDrawing = true
        }
      }
      that.initData.msgArr = []
      if (that.initData.drawHistoryArrData.length > 0) {
        function getChooseIndex () {
          for (var i in that.initData.drawHistoryArrData) {
            that.drawArr([that.initData.drawHistoryArrData[i]])
            if (that.initData.drawOrMove == 'move') {
              if (that.initData.context2d.isPointInPath(that.initData.initLeft, that.initData.initTop)) {
                that.initData.isMove = true
                that.drawArr(that.initData.drawHistoryArrData)
                that.initData.relPosX = that.initData.initLeft - that.initData.drawHistoryArrData[i].x
                that.initData.relPosY = that.initData.initTop - that.initData.drawHistoryArrData[i].y
                that.initData.relPosToX =
                  that.initData.initLeft - that.initData.drawHistoryArrData[i].toX
                that.initData.relPosToY =
                  that.initData.initTop - that.initData.drawHistoryArrData[i].toY
                return i
              }
            }
          }
          if (that.initData.drawOrMove == 'move') {
            return -1
          } else {
            return that.initData.drawHistoryArrData.length
          }
        }
        that.initData.chooseIndex = getChooseIndex()
      } else {
        if (that.initData.drawOrMove == 'move') {
          that.initData.chooseIndex = -1
        }
      }
      if (!that.drawFont) {
        if (that.initData.drawType == 'font') {
          that.drawFontTop = that.initData.initTop
          that.drawFontLeft = that.initData.initLeft
          that.drawFont = true
          setTimeout(() => {
            document.getElementById('markFront').focus();
          }, 200)
        }
      }
      if (that.initData.drawType == 'signet') {
        that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
          drawType: that.initData.drawType,
          drawTypeNum: that.initData.drawTypeNum,
          fill: that.initData.isFill ? that.initData.background : '',
          size: that.initData.size,
          color: that.initData.color,
          x: that.initData.initLeft,
          y: that.initData.initTop,
        }
        that.drawArr(that.initData.drawHistoryArrData)
      }
    },
    mouseUp () {
      const that = this
      that.isMove = false
      that.initData.msgArr = []
      that.initData.isMove = false
      that.initData.relPosX = 0
      that.initData.relPosY = 0
      that.canvas_front.removeEventListener(
        'mousemove',
        function(e) {
          if (e.type == 'touchmove') {
            e.preventDefault()
          }
          that.mouseMove(e)
        },
        true
      )
    },
    intoFontInputBlur () {


      const that = this

      that.isDrawing = false

      var x = that.initData.initLeft
      var y = that.initData.initTop
      that.initData.drawHistoryArrData[that.initData.chooseIndex] = {
        drawType: that.initData.drawType,
        drawTypeNum: that.initData.drawTypeNum,
        fill: that.initData.isFill ? that.initData.background : '',
        size: that.initData.size,
        fontSize: that.fontBoxSize,
        context: that.initData.context,
        color: that.initData.color,
        x: x,
        y: y,
        w: document.getElementsByClassName('intoFontInput')[0].clientWidth,
        h: document.getElementsByClassName('intoFontInput')[0].clientHeight,
      }
      that.drawArr(that.initData.drawHistoryArrData)
      that.initData.context =''
    },
    IntoFontInput () {
      const that = this
      document.getElementsByClassName('intoFont')[0].innerHTML = that.intoFontInput
      that.initData.context = that.intoFontInput
    },
    SortClick (name, index) {
      const that = this
      // that.activeIndex = index
      const fatherNode = document.getElementById('colorPick');
      fatherNode.getElementsByClassName('box')[0].classList.remove('open')
      switch (name) {
        case 'move':
          that.visibleDrawer = !this.visibleDrawer
          this.editSelect = false
          this.editBtn = true
          that.initData.drawOrMove = name
          that.cursor = 'move'
          break
        case 'draw':
          this.editSelect = !this.editSelect
          that.initData.drawOrMove = name
          that.cursor = 'crosshair'
          break
        case 'remote':
          this.editSelect = false
          that.initData.context2d.clearRect(0, 0, that.initData.canvasWidth, that.initData.canvasHeight)
          that.initData.drawHistoryArrData = []
          that.initData.drawHistoryArrData.length = 0
          break
        case 'strokeColorBox':
          fatherNode.getElementsByClassName('box')[0].classList.add('open')
          break
        case 'recall':
          this.drawList.splice(1,-1)
          this.drawArr( this.drawList)
          break
        case 'downLoad':
          that.CreateViewpoint()
          // ChangeColor
          // const element = document.getElementById('canvas')
          // const url = element.toDataURL()
          // const canvasImgBase = that.canvas_bgImg.getContext('2d')
          // const img = new Image()
          // img.src = url
          // img.onload = function() {
          //   canvasImgBase.drawImage(this, 0, 0, element.width, element.height)
          //   const urlSave = that.canvas_bgImg.toDataURL()
          //   that.$emit('saveEventName', {
          //     img: urlSave,
          //     viewPosition: that.viewpointPictureData.cameraData,
          //   })
          // }
          break
      }
    },
    SortClickNew (name) {
      const that = this
      switch (name) {
        case 'fillDraw':
          that.initData.isFill = !that.initData.isFill
          break
        case 'cancel':
          that.markOpenChild = false
          eventBus.$emit('changeShowDrawer', true)
          that.changeShowDrawer()
          break
      }
    },
    changeShowDrawer () {
      this.$emit('GetViewpointList')
      this.$emit('update:showDrawer', true)
    },
    CreateViewpoint () {
      //创建视点弹窗
      const that = this
      const element = document.getElementById('canvas')
      const url = element.toDataURL()
      const canvasImgBase = that.canvas_bgImg.getContext('2d')
      const img = new Image()
      img.src = url
      img.onload = function() {
        canvasImgBase.drawImage(this, 0, 0, element.width, element.height)
        const urlSave = that.canvas_bgImg.toDataURL()
        that.viewpointPictureData.viewPointImg = urlSave

        that.$sapi.Camera.getViewPort((data) => {
          that.viewpointPictureData.viewPosition=JSON.stringify(data)
        })
        const uploadImg = that.convertBase64UrlToBlob(urlSave)

        uploadFile(uploadImg)
          .then((res) => {
            that.viewpointPictureData.pictureCore = res
            that.createView = true //显示保存界面
          })
          .catch((err) => {
            that.$message.open({ content: '图片生成失败，请重新创建！', top: `50%`, duration: 2, maxCount: 1 })
            that.createView = false
          })
      }
    },
    handleCancel () {


      this.$emit('update:visible', false)
    },
    convertBase64UrlToBlob (urlData) {
      const fd = new FormData()
      var bytes = window.atob(urlData.split(',')[1]) //去掉url的头，并转换为byte
      //处理异常,将ascii码小于0的转换为大于0
      var ab = new ArrayBuffer(bytes.length)
      var ia = new Uint8Array(ab)
      for (var i = 0; i < bytes.length; i++) {
        ia[i] = bytes.charCodeAt(i)
      }
      const blob = new Blob([ab], { type: 'image/png' })
      fd.append('file', blob, new Date().getTime() + '.png')
      return fd
    },
    ChangeColor () {
      this.initData.color = this.selectColor
      this.initData.background = this.selectColor
    },
    ShrinkChange () {
      this.editSelect = !this.editSelect
    },
  },
}
</script>
<style lang="less" scoped>
#MarkPage {
  position: fixed;
  .canvasDraw {
    position: fixed;
    .drawFont {
      position: absolute;
      left: 50px;
      top: 50px;
      min-width: 100px;
      min-height: 20px;
      display: flex;
      justify-content: center;
      border: 2px dashed orange;
      z-index: 10;
      box-sizing: border-box;
      .intoFont {
        font-size: 14px;
        opacity: 0;
      }
      input {
        position: absolute;
        left: 0;
        top: 0;
        background: transparent;
        border: none;
        outline: none;
        min-width: 10px;
        width: 110%;
        box-sizing: border-box;
        padding: 2px;
        font-size: 16px;
        color: #333333;
      }
    }
    canvas {
      position: absolute;
      left: 0;
      right: 0;
      width: 100%;
      height: 100%;
      background: #ffffff;
      cursor: crosshair;
    }
    #canvasImg {
      z-index: 3;
    }
    #canvas {
      z-index: 5;
      background: rgba(255, 255, 255, 0);
    }
  }
}
.shared-mark {
  z-index: 1010;
  .sort-btn {
    left: calc(50% - 173px);
    transform: translate(-50%, 0);
  }
}
.origin-mark {
  z-index: 9;
}
.markBtnBox {
  position: fixed;
  top: 50%;
  .markBtn {
    // height: 61vh;
    overflow-y: auto;
    transition: width 0.1s;
    background-color: #001529cc;
    z-index: -10;
    padding: 15px;
    border-top-right-radius: 4px;
    border-bottom-right-radius: 4px;
    border: 1px solid #eeeeee;
    width: 0;
    display: flex;
    flex-direction: column;
    .tool_item {
      display: flex;
      align-items: center;
      cursor: pointer;
      div {
        display: inline-block;
        white-space: nowrap;
      }
      div:first-child {
        width: 24px;
        height: 30px;
        position: relative;
        img {
          width: 16px;
          height: 16px;
          position: absolute;
          top: 50%;
          left: 50%;
          transform: translate(-50%, -50%);
        }
      }
      &:not(:last-child) {
        margin-bottom: 10px;
        border-bottom: 1px dashed #dddddd;
        padding-bottom: 5px;
        cursor: pointer;
      }
      .color-panel {
        position: absolute;
        left: 130px;
      }
    }
    .draw-select {
      color: #05a081;
      border-color: #05a081 !important;
    }
    .draw-noselect {
      color: #ffffff;
      border-color: #ffffff;
    }
  }
  .mark-btn-show {
    z-index: 3;
    display: flex;
    width: 80px;
    padding: 10px;
    top: 50%;
    .tool_item {
      display: flex;
    }
  }
  .mark-btn-hide {
    z-index: -1;
    width: 0;
    padding: 0;
    top: 50%;
    .tool_item {
      display: none;
    }
  }
  .shrink-box {
    transition: width 0.1s;
    position: absolute;
    top: 45%;
    left: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: -1;
    .shrink-line {
      height: 1px;
      width: 80px;
      display: inline-block;
      background: rgba(5, 160, 129, 0);
      margin-right: 5px;
    }
    .shrink-change {
      cursor: pointer;
    }
  }
  .shrink-open {
    width: 110px;
  }
  .shrink-hide {
    width: 25px;
  }
}
.sort-btn {
  position: fixed;
  bottom: 53px;
  left: calc(50% - 173px);
  transform: translate(calc(-50% + 173px), 0);
  background-color: transparent;
  button {
    background-color: #001529cc;
    background-color: #001529cc;
    border-radius: 5px;
  }
  .edit-btn-select {
    background-color: #05a081;
  }
  .edit-btn-noselect {
    background-color: #001529cc;
  }
}
#colorPick {
  position: absolute;
  /deep/.colorBtn {
    background: transparent !important;
    position: absolute;
    top: 0;
    left: 0px;
    width: 26px;
    height: 26px;
  }
  /deep/.box {
    bottom: 6px !important;
    left: -16px !important;
  }
}
@media screen and (max-width: 370px) {
  .hidden-xs {
    max-width: 100vw;
    overflow-x: auto;
  }
  .hidden-xs::-webkit-scrollbar {
    width: 0 !important;
  }
  .hidden-xs {
    -ms-overflow-style: none;
  }
  .hidden-xs {
    overflow: -moz-scrollbars-none;
  }
}
</style>
