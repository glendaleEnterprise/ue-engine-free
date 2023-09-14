<template>
  <div class="box-DragControllerDiv" ref="box">
    <div class="left" :style='{width:leftWidth}'>
      <!--左侧div内容-->
      <slot name='left'></slot>
    </div>
    <div class="resize" title="收缩侧边栏">
      ⋮
    </div>
    <div class="mid" :style='{width:rightWidth}'>
      <slot name='right'></slot>
      <!--右侧div内容-->
    </div>
  </div>
</template>

<script>
export default {
  name: 'DragControllerDiv',
  data(){
    return{
      rightWidth:'68%',
      leftWidth:'calc(32% - 10px)'
    }
  },
  props:{
    _leftWidth:{
      type:String,
      default:'32'
    }
  },
  mounted() {
    this.rightWidth=(100-this._leftWidth)+'%'
    this.leftWidth='calc('+this._leftWidth+'% - 10px)'
    this.dragControllerDiv()
  },
  destroyed() {
    this.rightWidth=(100-this._leftWidth)+'%'
    this.leftWidth='calc('+this._leftWidth+'% - 10px)'
  },
  methods: {
    dragControllerDiv() {
      var resize = document.getElementsByClassName('resize');
      var left = document.getElementsByClassName('left');
      var mid = document.getElementsByClassName('mid');
      var box = document.getElementsByClassName('box-DragControllerDiv');
      for (let i = 0; i < resize.length; i++) {
        // 鼠标按下事件
        resize[i].onmousedown = function (e) {
          //颜色改变提醒
          resize[i].style.background = '#818181';
          var startX = e.clientX;
          resize[i].left = resize[i].offsetLeft;
          // 鼠标拖动事件
          document.onmousemove = function (e) {
            var endX = e.clientX;
            var moveLen = resize[i].left + (endX - startX); // （endx-startx）=移动的距离。resize[i].left+移动的距离=左边区域最后的宽度
            var maxT = box[i].clientWidth - resize[i].offsetWidth; // 容器宽度 - 左边区域的宽度 = 右边区域的宽度

            if (moveLen < 32) moveLen = 32; // 左边区域的最小宽度为32px
            if (moveLen > maxT - 150) moveLen = maxT - 150; //右边区域最小宽度为150px

            resize[i].style.left = moveLen; // 设置左侧区域的宽度

            for (let j = 0; j < left.length; j++) {
              left[j].style.width = moveLen + 'px';
              mid[j].style.width = (box[i].clientWidth - moveLen - 10) + 'px';
            }
          };
          // 鼠标松开事件
          document.onmouseup = function (evt) {
            //颜色恢复
            resize[i].style.background = '#d6d6d6';
            document.onmousemove = null;
            document.onmouseup = null;
            resize[i].releaseCapture && resize[i].releaseCapture(); //当你不在需要继续获得鼠标消息就要应该调用ReleaseCapture()释放掉
          };
          resize[i].setCapture && resize[i].setCapture(); //该函数在属于当前线程的指定窗口里设置鼠标捕获
          return false;
        };
      }
    },
  }
}
</script>

<style lang='less'>
/* 拖拽相关样式 */
/*包围div样式*/
.box-DragControllerDiv {
  width: 100%;
  height: 100%;
  margin: 1% 0px;
  overflow: hidden;
  user-select: none;
  /*左侧div样式*/
  .left {
    width: calc(32% - 10px); /*左侧初始化宽度*/
    height: 100%;
    background: #FFFFFF;
    float: left;
  }
  /*右侧div'样式*/
  .mid {
    float: left;
    width: 68%; /*右侧初始化宽度*/
    height: 100%;
    background: #fff;
    box-shadow: -1px 4px 5px 3px rgba(0, 0, 0, 0.11);
  }
  /*拖拽区div样式*/
  .resize {
    cursor: col-resize;
    float: left;
    position: relative;
    background-color: #d6d6d6;
    border-radius: 5px;
    margin-top: 20%;
    width: 8px;
    height: 50px;
    background-size: cover;
    background-position: center;
    font-size: 24px;
    color: white;

    /*拖拽区鼠标悬停样式*/
    &:hover {
      color: #444444;
    }
  }
}

</style>