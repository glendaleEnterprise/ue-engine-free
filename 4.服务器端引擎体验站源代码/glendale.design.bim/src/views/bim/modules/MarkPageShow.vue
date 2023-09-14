<template>
  <div id="MarkPage" :class="openType == 'shared'?'shared-mark':'origin-mark'">
    <div class="canvasDraw" :style="{width:initData.canvasWidth+'px',height:initData.canvasHeight+'px',left:canvasLeft+'px',top:canvasTop+'px'}">
      <div class="drawFont" data-type="hide" v-show="drawFont" :style="{left:drawFontLeft+'px',top:drawFontTop+'px',height: fontBoxSize + 8+'px'}">
        <span class="intoFont"></span>
        <input type="text" class="intoFontInput" id="markFront" @blur="intoFontInputBlur" @input="IntoFontInput" v-model="intoFontInput">
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

    </div>

    <div class="markMessage">
      处理意见
      <a-form-model   ref="ruleForm" :model="form" :rules="rules">
        <a-form-model-item  prop="handlerRemark">
          <a-textarea
            v-model="form.handlerRemark"
            placeholder="内容"
            :auto-size="{ minRows: 3, maxRows: 5 }"
            v-if='form.type ===2 && form.status ===0'
          />
          <div class='remarkText' v-else>
            {{form.handlerRemark}}
          </div>
        </a-form-model-item>
        <a-form-model-item>
          <div class='footer'>
            <a-button @click='subMessage' v-if='form.type ===2 && form.status ===0'  :disabled="form.handlerRemark === '' ">
              提交处理
            </a-button>
            <a-button @click='subMessage' v-if='form.type ===1 && form.status ===1' :disabled="form.handlerRemark === '' ">
              处理关闭
            </a-button>
          </div>
        </a-form-model-item>
      </a-form-model>


    </div>

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
        <a-button type="primary" @click="SortClickNew('cancel',-1)" class="btn-tool">
          <img src="../../../assets/img/bim/Close.png" title="关闭" />
        </a-button>
      </a-space>
    </div>
<!--    <save-view ref="SaveV" @changeShowDrawer='changeShowDrawer($event)' :viewDataChild="viewDataChild" :viewpointPictureData="viewpointPictureData" :createView.sync="createView"  ></save-view>-->
  </div>
</template>
<script>
import { uploadFile } from '@/api/file'
import { eventBus } from '@/utils/bus'
import { getPostilStatus } from '@/api/postil'
const SortSelect = [
  // {
  //   name: 'downLoad',
  //   nameNum: '1',
  //   title: '保存',
  //   img: require('../../../assets/img/bim/SaveBtn.png'),
  //   img1: require('../../../assets/img/bim/SaveBtn_1.png'),
  // },
]
export default {
  name: 'MarkPageShow',
  components: {  },
  data () {
    return {
      rules: {
        handlerRemark: [
          { required: true, message: '请填写', trigger: 'blur' },
          // { min: 3, max: 5, message: 'Length should be 3 to 5', trigger: 'blur' },
        ],
      },
      form:this.markPageShowData,
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
      isMove: false,
      drawFont: false,
      isDrawing: false,
      drawFontTop: 0,
      drawFontLeft: 0,
      intoFontInput: '',
      fontBoxSize: 20,
      SortSelect,
      cursor: 'auto',
      editSelect: true,
      createView: false,
      viewpointPictureData: this.viewpointData,
      selectColor: '#ff1e02',
      editBtn: false,
      activeIndex: 0,
      drawSelectIndex: 0,
      viewDataChild: this.viewData,
      visibleDrawer: false,
    }
  },
  props: {
    base64image: { type: String, },
    changeStyle: { type: Boolean, default: false, },
    markPageShowData: { type: Object, },
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
      this.activeIndex = 1111
    }

  },
  watch: {
    base64image (newVal, oldVal) {
      this.initMark()
    },

    markPageShowData (newVal, oldVal) {
      this.form=newVal
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
    getPostilStatus(id){
      const param={
        status:2,
        id:id
      }
      getPostilStatus(param).then(data=>{
        this.$message.success('已关闭')
        eventBus.$emit('getPostilLaunch')
      })
    },
    subMessage(){

      this.$refs.ruleForm.validate(valid => {
        if (valid) {
         const param={
           id:this.form.id,
           status:1,
           handlerRemark:this.form.handlerRemark
         }
          getPostilStatus(param).then(data=>{
            this.$message.success('已提交')
            this.form=data
            eventBus.$emit('getPostilLaunch')
          })
        }
      })
    },
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
    changeShowDrawer () {
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
    SortClickNew (name) {
      const that = this
      switch (name) {
        case 'fillDraw':
          that.initData.isFill = !that.initData.isFill
          break
        case 'cancel':
          that.$emit('changeShow', false)
          that.changeShowDrawer()
          break
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

    transform: translate(-50%, 0);
  }
}
.origin-mark {
  z-index: -1;
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
      cursor: pointer;
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
.markMessage{
  right: 10px;
  padding: 5px;
  z-index: 3;
  position: fixed;
  top: 10px;
  width: 400px;
  //height: 100px;
  background: #959697;
  .footer{
    padding: 5px;
    display: flex;
    justify-content: center;
  }
  .remarkText{
    border: 1px solid #fff;
    min-height: 70px;
    color: #fff;
    padding: 4px;
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
