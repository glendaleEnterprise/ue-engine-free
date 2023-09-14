<!--可拖拽antdv 弹窗的封装-->
<template>
  <a-modal :class="[modalClass, simpleClass]" :visible="visible" v-bind="$props" :footer="null" :bodyStyle="{ padding: 0 }" @ok="handleOk" @cancel="handleCancel">
    <div class="ant-modal-body" :style="bodyStyle">
      <slot></slot>
    </div>
    <div v-if="footer" class="ant-modal-footer relative">
      <slot name="footer"></slot>
    </div>
    <div v-if="!title && title !== ''" slot="title">
      <slot name="title"></slot>
    </div>
  </a-modal>
</template>
<script>
import props from './props.js'
export default {
  name: 'DragModal',
  mixins: [props],
  props: {
    // 容器的类名
    modalClass: {
      type: String,
      default: () => {
        return 'modal-box'
      }
    },
    visible: {
      type: Boolean,
      default: () => {
        return false
      }
    },
    title: {
      type: String,
      default: () => {
        return undefined
      }
    },
    // width: {
    //   type: String,
    //   default: () => {
    //     return '70%'
    //   }
    // },
    footer: {
      type: Boolean,
      default: () => {
        return false
      }
    }
  },
  data () {
    return {
      mouseDownX: 0,
      mouseDownY: 0,
      deltaX: 0,
      deltaY: 0,
      sumX: 0,
      sumY: 0,
      header: null, // 标题头部
      contain: null,
      modalContent: null,
      onmousedown: false
    }
  },
  computed: {
    simpleClass () { return Math.random().toString(36).substring(2) }
  },
  watch: {
    visible () {
      this.$nextTick(() => {
        this.initialEvent(this.visible)
      })
    }
  },
  mounted () {
    this.$nextTick(() => {
      this.initialEvent(this.visible)
    })
  },
  beforeDestroy () {
    this.removeMove()
    window.removeEventListener('mouseup', this.removeUp, false) // 移除鼠标按下需监听
  },
  methods: {
    // 确定按钮回调
    handleOk (e) {
      this.resetNum()
      this.$emit('ok', e)
    },
    // 取消按钮回调
    handleCancel (e) {
      this.resetNum()
      this.$emit('cancel', e)
    },
    // 弹窗的初始位置初始化
    resetNum () {
      this.mouseDownX = 0
      this.mouseDownY = 0
      this.deltaX = 0
      this.deltaY = 0
      this.sumX = 0
      this.sumY = 0
    },
    // 移动事件
    handleMove (event) {
      const delta1X = event.pageX - this.mouseDownX
      const delta1Y = event.pageY - this.mouseDownY
      this.deltaX = delta1X
      this.deltaY = delta1Y
      this.modalContent.style.transform = `translate(${delta1X + this.sumX}px, ${delta1Y + this.sumY}px)`
    },
    // 弹窗初始化
    initialEvent (visible) {
      if (visible) {
        window.removeEventListener('mouseup', this.removeUp, false)
        this.contain = document.getElementsByClassName(this.simpleClass)[0]
        this.header = this.contain.getElementsByClassName('ant-modal-header')[0]
        this.modalContent = this.contain.getElementsByClassName('ant-modal-content')[0]
        this.modalContent.style.left = 0
        this.modalContent.style.transform = 'translate(0px,0px)'
        this.header.style.cursor = 'all-scroll'
        this.header.onmousedown = e => {
          this.onmousedown = true
          this.mouseDownX = e.pageX
          this.mouseDownY = e.pageY
          document.body.onselectstart = () => false
          window.addEventListener('mousemove', this.handleMove, false)
        }

        window.addEventListener('mouseup', this.removeUp, false)
      }
    },
    // 鼠标停止
    removeMove () {
      window.removeEventListener('mousemove', this.handleMove, false)
    },
    // 鼠标抬起事件
    removeUp (e) {
      document.body.onselectstart = () => true
      if (this.onmousedown && !(e.pageX === this.mouseDownX && e.pageY === this.mouseDownY)) {
        this.onmousedown = false
        this.sumX = this.sumX + this.deltaX
        this.sumY = this.sumY + this.deltaY
      }
      this.removeMove()
    }
  }
}
</script>
