<template>
  <div>
    <div class="left-box" :class="{ 'mobile-box':isMobile  }">
      <a-drawer title="" placement="left" :closable="false" :visible="visible" :wrap-style="{ position: 'absolute' }"
        :get-container="false" :mask="false">
        <div class="left-nav">
          <div v-for="item in functionMenu" :key="item.id" @click="LeftMenuSelection(item.id)" class="left-tool"
            :class="{ active: item.id == isActive }" v-show="item.show">
            <img :src="require('../../../assets/img/bim/navBtn/' + item.icon + '.png')" />
            <span>{{ item.name }}</span>
          </div>
        </div>
      </a-drawer>
      <div class="up-down" @click="showDrawer" :class="{'showDrawer':visible}" v-if="isMobile">
        <div class="up-triangle all-triangle"></div>
        <a-icon type="menu-fold" v-if="visible" />
        <a-icon type="menu-unfold" v-else />
        <div class="down-triangle all-triangle"></div>
      </div>
    </div>
    <div class="footer-box">
      <div class="footer-btn">
        <div class="btn-tool" v-for="value in bottomMenu" :key="value.id"
          :class="{ active: value.id == isChildrenActive }"
          @click.stop="value.children == undefined ? FunctionChoose(value) : null" v-show="value.show">
          <a-tooltip v-if="value.children && value.children.length > 0" placement='top'
            :getPopupContainer="(triggerNode) => { return triggerNode.parentNode }"
            :overlayClassName="'example-overlay'">
            <template v-if="value.children" slot="title">
              <a-tooltip v-for="item in value.children" :key="item.id" placement="rightTop" :title="item.name"
                v-show="item.show" @click="FunctionChoose(item)">
                <div class="meature-btn" :class="{ active: item.id == isChildrenActive }">
                  <img :src="require('../../../assets/img/bim/navBtn/' + item.icon + '.png')" />
                </div>
              </a-tooltip>
            </template>
            <img :src="require('../../../assets/img/bim/navBtn/' + value.icon + '.png')" />
          </a-tooltip>
          <a-tooltip v-else :title="value.name">
            <img :src="require('../../../assets/img/bim/navBtn/' + value.icon + '.png')" />
          </a-tooltip>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
  import store from '@/store'
  import {
    MeasurementType
  } from './SubSceneOperation/Measurement'
  import Viewpoint from './SubViewOperation/Viewpoint.vue'
  import LabelManagement from './SubSceneOperation/LabelManagement.vue'
  import FirstRoaming from './SubSceneOperation/FirstRoaming.vue'
  import CustomViewpoint from './SubSceneOperation/CustomViewpoint.vue'
  import CaptureScene from './SubSceneOperation/CaptureScene'
  import ModelTypeTree from './ModelOperations/ModelTypeTree.vue'
  import ModelFloorTree from './ModelOperations/ModelFloorTree.vue'
  import ModelSpaceTree from './ModelOperations/ModelSpaceTree.vue'
  import ModelExplosion from './ModelOperations/ModelExplosion.vue'
  import FeatureProperties from './FeatureAction/FeatureProperties.vue'
  import FeatureVisible from './FeatureAction/FeatureVisible.vue'
  import SunshineTime from './SubEffect/SunshineTime'
  import PointSource from './SubEffect/PointSource.vue'
  import Spotlight from './SubEffect/Spotlight.vue'
  import GasFlowPage from './SubEffect/GasFlowPage.vue'
  import DiffusionPage from './SubEffect/DiffusionPage.vue'
  import RadarScanning from './SubEffect/RadarScanning.vue'
  import WeatherEffects from './WeatherEffects'
  import RegionRendering from './SubEffect/RegionRendering.vue'
  import DragCut from './ModelOperations/DragCut'
  import SplitFrame from './ModelOperations/SplitFrame'
  import SubGrid from './ModelOperations/SubGrid'
  import FloorLayering from './AnimationSettings/FloorLayering'
  import SubSetFeatureColor from './FeatureAction/SubSetFeatureColor'
  import RegionFeature from './FeatureAction/RegionFeature'
  import MemberRotation from './AnimationSettings/MemberRotation'
  import TrolleyMobile from './AnimationSettings/TrolleyMobile'
  import FlatteningFunction from './GISFunction/FlatteningFunction'
  import RegionClipping from './GISFunction/RegionClipping'
  import ModelUnitization from './GISFunction/ModelUnitization'
  import SelfLuminous from './SubEffect/SelfLuminous'
  import TrafficHeatingLine from './SubEffect/TrafficHeatingLine'
  import LightTrailing from './SubEffect/LightTrailing'
  import SubRotate from './SubEditOperation/SubRotate'
  import SubOffset from './SubEditOperation/SubOffset'
  import SubSetFeatureRotate from './SubEditOperation/SubSetFeatureRotate'
  import SubFeatureOffset from './SubEditOperation/SubFeatureOffset'
  import SelfLuminescentLine from './SubEffect/SelfLuminescentLine'
  import EscapeRoutes from './SubEffect/EscapeRoutes'
  import VideoFusion2D from './SubEffect/VideoFusion2D'
  import ElectronicFence from './SubEffect/ElectronicFence'
  import HeatMap from './SubEffect/HeatMap.vue'
  import LevelManagement from './LayerManagement/LevelManagement.vue'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    components: {
      Viewpoint,
      LabelManagement,
      FirstRoaming,
      CustomViewpoint,
      CaptureScene,
      ModelTypeTree,
      ModelFloorTree,
      ModelSpaceTree,
      FeatureProperties,
      FeatureVisible,
      PointSource,
      Spotlight,
      GasFlowPage,
      DiffusionPage,
      RadarScanning,
      WeatherEffects,
      RegionRendering,
      ModelExplosion,
      DragCut,
      SplitFrame,
      SubGrid,
      FloorLayering,
      SubSetFeatureColor,
      RegionFeature,
      MemberRotation,
      FlatteningFunction,
      RegionClipping,
      ModelUnitization,
      TrolleyMobile,
      SelfLuminous,
      TrafficHeatingLine,
      LightTrailing,
      SubRotate,
      SubOffset,
      SubSetFeatureRotate,
      SubFeatureOffset,
      SelfLuminescentLine,
      EscapeRoutes,
      VideoFusion2D,
      ElectronicFence,
      SunshineTime,
      HeatMap,
      LevelManagement
    },
    props: {
      projectMessage: {
        type: Object,
        default: undefined,
      },
      settingVisible: {
        type: Boolean
      },
      layerListVisible: {
        type: Boolean
      },
    },
    data() {
      return {
        functionMenu: [{
            id: 'ResetScene',
            icon: 'ResetScene',
            name: '重置场景',
            show: true
          },
          {
            id: 'LayerManagement',
            icon: 'LayerManagement',
            name: '图层列表',
            show: true
          },
          {
            id: 'LevelManagement',
            icon: 'LayerManagement',
            name: '关卡列表',
            show: this.projectMessage.documentType == 3 ? true : false
          },
          {
            id: 'ViewOperation',
            icon: 'ViewOperation',
            name: '视点管理',
            show: true,
            children: [{
                id: 'PerspectiveManagement',
                icon: 'PerspectiveManagement',
                name: '视角设置',
                show: true,
                children: [{
                    id: 'top',
                    icon: 'TopView',
                    name: '顶视角',
                    show: true,
                  },
                  {
                    id: 'bottom',
                    icon: 'BottomView',
                    name: '底视角',
                    show: true,
                  },
                  {
                    id: 'front',
                    icon: 'FrontView',
                    name: '前视角',
                    show: true,
                  },
                  {
                    id: 'back',
                    icon: 'RearView',
                    name: '后视角',
                    show: true,
                  },
                  {
                    id: 'left',
                    icon: 'LeftView',
                    name: '左视角',
                    show: true,
                  },
                  {
                    id: 'right',
                    icon: 'RightView',
                    name: '右视角',
                    show: true,
                  },
                ]
              },
              {
                id: 'SubCameraManage',
                icon: 'SubCameraManage',
                name: '视点设置',
                show: true,
              },
            ]
          },
          {
            id: 'SceneOperation',
            icon: 'SceneOperation',
            name: '场景操作',
            show: true,
            children: [{
                id: 'SubTagManager',
                icon: 'SubTagManager',
                name: '标签管理',
                show: true,
              },
              {
                id: 'SubImmersionRoaming',
                icon: 'SubImmersionRoaming',
                name: '漫游',
                show: true,
              },
              {
                id: 'CustomViewpoint',
                icon: 'CustomViewpoint',
                name: '自定义视点漫游',
                show: true,
              },
              {
                id: 'CaptureScene',
                icon: 'CaptureScene',
                name: '截取场景图片',
                show: true,
              },
              {
                id: 'MeasurementOperation',
                icon: 'MeasurementOperation',
                name: '测量操作',
                show: true,
                children: [{
                    id: 'DistanceMeasurement',
                    icon: 'SubDistance',
                    name: '测量距离',
                    show: true,
                  },
                  {
                    id: 'AreaMeasurement',
                    icon: 'SubArea',
                    name: '测量面积',
                    show: true,
                  },
                  {
                    id: 'AngleMeasurement',
                    icon: 'SubAngle',
                    name: '测量角度',
                    show: true,
                  },
                  {
                    id: 'ComponentAreaMeasurement',
                    icon: 'MeasurementArea',
                    name: '测量构件面积',
                    show: true,
                  },
                  {
                    id: 'ComponentVolumeMeasurement',
                    icon: 'MeasuringVolume',
                    name: '测量构件体积',
                    show: true,
                  },
                  {
                    id: 'SubClear',
                    icon: 'SubClear',
                    name: '清除测量',
                    show: true,
                  },
                ]
              },
            ]
          },
          {
            id: 'ModelAction',
            icon: 'ModelAction',
            name: '模型操作',
            show: true,
            children: [{
                id: 'SubStructure',
                icon: 'SubStructure',
                name: '专业结构',
                show: true,
              },
              {
                id: 'FloorTree',
                icon: 'FloorTree',
                name: '楼层结构',
                show: true,
              },
              {
                id: 'SpaceTree',
                icon: 'SpaceTree',
                name: '空间结构',
                show: false,
              },
              {
                id: 'ModelExplosion',
                icon: 'ModelExplosion',
                name: '模型爆炸',
                show: true,
              },
              {
                id: 'SubSectioning',
                icon: 'SubSectioning',
                name: '模型剖切',
                show: true,
                children: [{
                    id: 'DragCut',
                    icon: 'SubCut',
                    name: '拖动剖切',
                    show: true,
                  },
                  {
                    id: 'SplitFrame',
                    icon: 'SplitFrame',
                    name: '剖切框',
                    show: true,
                  }
                ]
              },
              {
                id: 'SubGrid',
                icon: 'SubGrid',
                name: '绘制轴网',
                show: true,
              },
              {
                id: 'ModelBoundingBox',
                icon: 'ModelBoundingBox',
                name: '模型包围盒',
                show: true,
              },
            ]
          },
          {
            id: 'FeatureAction',
            icon: 'FeatureAction',
            name: '构件操作',
            show: true,
            children: [{
                id: 'SubAttributes',
                icon: 'SubAttributes',
                name: '构件属性',
                show: true,
              },
              {
                id: 'SubHideOrShow',
                icon: 'SubHideOrShow',
                name: '构件隐藏',
                show: true,
              },
              {
                id: 'SubSetFeatureColor',
                icon: 'SubSetFeatureColor',
                name: '构件颜色',
                show: true,
              },
              {
                id: 'RegionFeature',
                icon: 'RegionFeature',
                name: '构件框选',
                show: true,
              },
            ]
          },
          {
            id: 'Effect',
            icon: 'Effect',
            name: '效果展示',
            show: true,
            children: [{
                id: 'SunshineTime',
                icon: 'SunshineTime',
                name: '日照时间',
                show: true,
              },
              {
                id: 'LightingEffects',
                icon: 'LightEffect',
                name: '灯光效果',
                show: true,
                children: [{
                    id: 'PointSource',
                    icon: 'PointSource',
                    name: '点光源',
                    show: true,
                  },
                  {
                    id: 'Spotlight',
                    icon: 'Spotlight',
                    name: '聚光灯',
                    show: false,
                  },
                  {
                    id: 'SelfLuminous',
                    icon: 'SelfLuminous',
                    name: '自发光',
                    show: true,
                  }
                ]
              },
              {
                id: 'GasFlow',
                icon: 'GasFlow',
                name: '烟雾扩散',
                show: true,
              },
              {
                id: 'Diffusion',
                icon: 'Diffusion',
                name: '扩散效果',
                show: true,
                children: [{
                    id: 'CircularDiffusion',
                    icon: 'CircularDiffusion',
                    name: '圆扩散',
                    show: true,
                  },
                  {
                    id: 'CustomDiffusion',
                    icon: 'CustomDiffusion',
                    name: '自定义扩散',
                    show: true,
                  }
                ]
              },
              {
                id: 'RadarScanning',
                icon: 'RadarScanning',
                name: '雷达扫描',
                show: true,
              },
              {
                id: 'ParticleEffect',
                icon: 'ParticleEffect',
                name: '天气效果',
                show: true,
                children: [{
                    id: 'Rain',
                    icon: 'Rain',
                    name: '雨天',
                    show: true,
                  },
                  {
                    id: 'Snow',
                    icon: 'Snow',
                    name: '雪天',
                    show: true,
                  }
                ]
              },
              {
                id: 'RegionRendering',
                icon: 'RegionRendering',
                name: '区域绘制',
                show: true,
              },
              {
                id: 'TrafficHeatingLine',
                icon: 'TrafficHeatingLine',
                name: '交通热力线',
                show: true,
              },
              {
                id: 'LightTrailing',
                icon: 'LightTrailing',
                name: '光线拖尾',
                show: true,
              },
              {
                id: 'SelfLuminescentLine',
                icon: 'SelfLuminescentLine',
                name: '自发光线条',
                show: true,
              },
              {
                id: 'EscapeRoutes',
                icon: 'EscapeRoutes',
                name: '逃生路线',
                show: true,
              },
              {
                id: 'VideoFusion',
                icon: 'VideoFusion',
                name: '视频融合',
                show: true,
                children: [{
                  id: 'VideoFusion2D',
                  icon: 'VideoFusion2D',
                  name: '2D视频融合',
                  show: true,
                }]
              },
              {
                id: 'ElectronicFence',
                icon: 'ElectronicFence',
                name: '电子围栏',
                show: true,
              },
              {
                id: 'HeatMap',
                icon: 'HeatMap',
                name: '热力图',
                show: true,
              },
            ]
          },
          {
            id: 'SubEditOperation',
            icon: 'EditOperation',
            name: '编辑操作',
            show: (this.projectMessage.sceneType == 0 && this.projectMessage.sceneClassification != 2) ? true : false,
            children: [{
                id: 'SubRotate',
                icon: 'SubRotate',
                name: '模型旋转',
                show: (this.projectMessage.sceneType == 0 && this.projectMessage.sceneClassification != 2) ? true :
                  false,
              },
              {
                id: 'SubOffset',
                icon: 'SubOffset',
                name: '模型偏移',
                show: (this.projectMessage.sceneType == 0 && this.projectMessage.sceneClassification != 2) ? true :
                  false,
              },
              {
                id: 'SubSetFeatureRotate',
                icon: 'SubSetFeatureRotate',
                name: '构件旋转',
                show: true,
              },
              {
                id: 'SubFeatureOffset',
                icon: 'SubFeatureOffset',
                name: '构件偏移',
                show: true,
              },
            ]
          },
          {
            id: 'GISFunction',
            icon: 'GISFunction',
            name: 'GIS功能',
            show: false,
            children: [{
                id: 'FlatteningFunction',
                icon: 'FlatteningFunction',
                name: '模型压平',
                show: true,
              },
              {
                id: 'RegionClipping',
                icon: 'RegionClipping',
                name: '模型裁剪',
                show: true,
              },
              {
                id: 'ModelUnitization',
                icon: 'ModelUnitization',
                name: '单体化',
                show: true,
              },
            ]
          },
          {
            id: 'AnimationSettings',
            icon: 'AnimationSettings',
            name: '动画效果',
            show: true,
            children: [{
                id: 'TrolleyMobile',
                icon: 'TrolleyMobile',
                name: '构件移动',
                show: true,
              },
              {
                id: 'FloorLayering',
                icon: 'FloorLayering',
                name: '楼层分层',
                show: true,
              },
              {
                id: 'MemberRotation',
                icon: 'MemberRotation',
                name: '构件自转',
                show: false,
              },
            ]
          },
          {
            id: 'SetData',
            icon: 'SetData',
            name: '设置',
            show: this.projectMessage.sceneClassification != 2 && this.projectMessage.functionalPermissions ? true :
              false,
          },
        ],
        bottomMenu: [],
        isActive: '',
        isChildrenActive: '',
        visible: true,
        isMobile: false
      }
    },
    created() {
      this.isMobile = _isMobile() ? true : false;
      for (let i = 0; i < this.projectMessage.modelList.length; i++) {
        let item = this.projectMessage.modelList[i];
        if (item.modelFormat == 'osgb') {
          this.functionMenu.forEach(item => {
            if (item.id == "GISFunction") {
              item.show = true
            }
          })
        } else if (item.modelFormat == 'UE工程模型') {
          this.functionMenu.forEach(item => {
            if (item.id == "GISFunction" || item.id == "AnimationSettings" || item.id == "SetData" || item.id ==
              "SubEditOperation" || item.id == "ModelAction" || item.id == "LayerManagement") {
              item.show = false
            } else if (item.id == "SceneOperation") {
              item.children[4].show = false;
            } else if (item.id == "FeatureAction") {
              item.children[0].show = false;
              item.children[3].show = false;
            } else if (item.id == "ViewOperation") {
              item.children[0].show = false;
            } else if (item.id == "Effect") {
              item.children[1].children[2].show = false;
            }
          })
        } else if (this.isMobile) {
          this.functionMenu.forEach(item => {
            if (item.id == "FeatureAction") {
              item.children[3].show = false;
            } else if (item.id == "Effect") {
              item.children[13].show = false;
            } else if (item.id == 'SubEditOperation') {
              item.children[2].show = false;
              item.children[3].show = false;
            } else if (item.id == 'AnimationSettings') {
              item.children[1].show = false;
            }
          })
        }
      }
      // let list = this.projectMessage.modelList.filter(item => item.modelFormat == 'osgb');
      // list.length != 0 ?
      //   this.functionMenu.forEach(item => {
      //     if (item.id == "GISFunction") {
      //       item.show = true
      //     }
      //   })
      //   : null·  3
    },
    methods: {
      LeftMenuSelection(id) {
        const that = this;
        if (that.isMobile) {
          that.visible = false;
        }
        that.bottomMenu = []
        that.isChildrenActive = '';
        store.dispatch('GetCurrentClick', undefined)
        that.ClearEvent();
        that.isActive = id;
        const index = that.functionMenu.findIndex(item => item.id == id);
        if (index != -1) {
          if (that.functionMenu[index].id == "SetData") {
            that.$emit('update:settingVisible', true)
          } else if (that.functionMenu[index].id == "LayerManagement") {
            that.$emit('update:layerListVisible', true)
          } else if (that.functionMenu[index].id == "LevelManagement") {
            that.NotificationPopup({
              title: '关卡管理',
              id: 'LevelManagement',
              description: < LevelManagement projectMessage = {
                that.projectMessage
              }
              />,
            })
          } else if (that.functionMenu[index].id == "ResetScene") {
            that.ResetScene()
          } else {
            that.functionMenu[index].children ? that.bottomMenu = that.functionMenu[index].children : that
              .bottomMenu = []
          }
        }
      },
      FunctionChoose(data) {
        const that = this;
        if (that.isMobile) {
          that.visible = false;
        }
        store.dispatch('GetCurrentClick', data.id)
        that.isChildrenActive = data.id
        that.ClearEvent();
        if (data.id.indexOf('Measurement') == -1) {
          MeasurementType({
            id: 'ClearResults'
          }, that)
        }
        switch (data.id) {
          case 'top':
          case 'bottom':
          case 'front':
          case 'back':
          case 'left':
          case 'right':
            api.Camera.setPerspectiveViewPort({
              type: data.id // 视角的类型,top:为顶视角,bottom:为底视角,front:为前视角,back:为后视角,left:为左视角,right:为右视角
            })
            break;
          case 'SubCameraManage':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < Viewpoint projectMessage = {
                that.projectMessage
              }
              />,
            })
            break
          case 'SubTagManager':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < LabelManagement projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SubImmersionRoaming':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < FirstRoaming projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'CustomViewpoint':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < CustomViewpoint projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "CaptureScene":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < CaptureScene projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'DistanceMeasurement':
          case 'AreaMeasurement':
          case 'AngleMeasurement':
            MeasurementType(data, that)
            break
          case 'ComponentAreaMeasurement':
          case 'ComponentVolumeMeasurement':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            MeasurementType(data, that)
            break
          case 'SubStructure':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < ModelTypeTree projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'FloorTree':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < ModelFloorTree projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SpaceTree':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < ModelSpaceTree projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "ModelExplosion":
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < ModelExplosion projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'DragCut':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < DragCut projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SplitFrame':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SplitFrame projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SubGrid':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SubGrid projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;

          case 'ModelBoundingBox':
            api.Model.getBounds(that.projectMessage.modelId, true, 'rgba(255,0,0,1)', 1, function (res) {})
            break;
          case "FlatteningFunction":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < FlatteningFunction projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "RegionClipping":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < RegionClipping projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "ModelUnitization":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < ModelUnitization projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'TrolleyMobile':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < TrolleyMobile projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'FloorLayering':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < FloorLayering projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "MemberRotation":
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < MemberRotation projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SubAttributes':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < FeatureProperties projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SubHideOrShow':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < FeatureVisible projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "SubSetFeatureColor":
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SubSetFeatureColor projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "RegionFeature":
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              tips: 'tips-notification',
              placement: "bottomRight",
              description: < RegionFeature projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "SunshineTime":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              width: '300px',
              description: < SunshineTime projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'PointSource':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < PointSource projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'Spotlight':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < Spotlight projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "SelfLuminous":
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SelfLuminous projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "GasFlow":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < GasFlowPage projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'CircularDiffusion':
          case 'CustomDiffusion':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < DiffusionPage projectMessage = {
                that.projectMessage
              }
              typeDiffusion = {
                that.isChildrenActive
              }
              />,
            })
            break;
          case 'RadarScanning':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < RadarScanning projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'Rain':
          case 'Snow':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              tips: 'operation-notification',
              width: '42px',
              placement: "topRight",
              description: < WeatherEffects projectMessage = {
                that.projectMessage
              }
              typeEffects = {
                that.isChildrenActive
              }
              />,
            })
            break;
          case 'RegionRendering':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              tips: 'operation-notification',
              width: '42px',
              placement: "topRight",
              description: < RegionRendering projectMessage = {
                that.projectMessage
              }
              typeEffects = {
                that.isChildrenActive
              }
              />,
            })
            break;
          case 'TrafficHeatingLine':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < TrafficHeatingLine projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'LightTrailing':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < LightTrailing projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SelfLuminescentLine':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SelfLuminescentLine projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "EscapeRoutes":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < EscapeRoutes projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "VideoFusion2D":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < VideoFusion2D projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "ElectronicFence":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < ElectronicFence projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "HeatMap":
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < HeatMap projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SubRotate':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SubRotate projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SubOffset':
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SubOffset projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case 'SubSetFeatureRotate':
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SubSetFeatureRotate projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
          case "SubFeatureOffset":
            if (that.projectMessage.documentType == 2) {
              that.$message.warning('该模型类型不支持此操作')
              break;
            }
            that.NotificationPopup({
              title: data.name,
              id: data.id,
              description: < SubFeatureOffset projectMessage = {
                that.projectMessage
              }
              />,
            })
            break;
        }
      },
      NotificationPopup(parameter) {
        const that = this;
        that.$notification.open({
          key: 'EngineKey',
          message: parameter.title,
          description: parameter.description,
          class: 'engine-notification ' + (parameter.tips ? parameter.tips : ''),
          duration: null,
          placement: parameter.placement ? parameter.placement : that.isMobile ? 'bottomLeft' : 'topRight',
          style: {
            width: parameter.width ? parameter.width : '280px',
            marginRight: `20px`,
          },
        })
      },
      ClearEvent() {
        const that = this
        api.Public.clearAllDrawObject()
        that.$notification.destroy();
        api.Feature.getByEvent(false) //关闭构件拾取事件
        api.Measurement.angle(false)
        api.Measurement.distance(false)
        api.Measurement.clearAllTrace()
        store.dispatch('GetObtainCoordinates', {
          clickStatus: false
        }) //关闭拾取坐标
        that.$emit('update:layerListVisible', false)
        that.$emit('update:settingVisible', false)
        api.Model.clearBoundsBox();
      },
      ResetScene() {
        const that = this;
        that.projectMessage.camera ? api.Camera.setViewPort(that.projectMessage.camera) : api.Model.location(that
          .projectMessage.modelId);
        //清除构件自转
        let featureRotate = store.state.bim.featureRotationList;
        if (featureRotate.length > 0) {
          featureRotate.forEach(item => {
            api.Feature.autoRotate(
              item.ID,
              item.Axis == 'x' ? 1 : 0,
              item.Axis == 'y' ? 1 : 0,
              item.Axis == 'z' ? 1 : 0,
              0
            );
            api.Feature.original(item.ID, item.ID.split("^")[0])
          })
          store.dispatch('GetFeatureRotationList', [])
        }
        //清除构件偏移
        let offsetHistoryList = store.state.bim.featureOffsetList;
        if (offsetHistoryList.length > 0) {
          offsetHistoryList.forEach(item => {
            api.Feature.original(item.id, item.id.split("^")[0])
          })
          store.dispatch('GetFeatureOffsetList', [])
        }
        //清除构件旋转
        let rotateHistoryList = store.state.bim.featureRotateList;
        if (rotateHistoryList.length > 0) {
          rotateHistoryList.forEach(item => {
            api.Feature.original(item.id, item.id.split("^")[0])
          })
          store.dispatch('GetFeatureRotateList', [])
        }
        //构件隐藏
        let componentList = store.state.bim.featureVisibleList
        componentList.forEach(item => {
          api.Feature.setVisible(item.id, true, item.id.split("^")[0])
        })
        store.dispatch('GetFeatureVisibleList', [])
        //构件颜色
        let componentColorList = store.state.bim.featureColorList
        componentColorList.forEach(item => {
          api.Feature.setColor(item.ID, "rgba(255,255,255,1)");
        })
        store.dispatch('GetFeatureColorList', [])
        //模型压平
        let flattenList = store.state.bim.flattenList;
        flattenList.forEach(item => {
          api.Model.removeFlatten(item.modelId, item.id);
        })
        store.dispatch('GetFlattenList', [])
        //模型裁剪
        let croppingList = store.state.bim.croppingList;
        croppingList.forEach(item => {
          api.Model.removeCutPart(item.modelId, item.id);
        })
        store.dispatch('GetCroppingList', [])
        //日照时间恢复
        api.Public.setTime({
          'Time': "13:00"
        })
        store.dispatch('GetSunshineTime', 420)
      },
      onClose() {
        this.visible = false;
      },
      showDrawer() {
        this.visible = !this.visible;
      },
    }
  }
</script>
<style lang="less" scoped>
  @import '../BimCss.less';


  // /deep/.mobile-box {
  //   .ant-drawer-content-wrapper {
  //     height: 80% !important;
  //     top: 10% !important;

  //     .left-nav {
  //       top: 0 !important;
  //       transform: translate(0, 0) !important;
  //     }

  //   }

  // }

  /deep/.left-box {
    position: fixed;
    left: 0;
    height: 100%;
    top: 0;

    .ant-drawer {
      color: aqua;

      .ant-drawer-content-wrapper {
        width: 60px !important;
        box-shadow: none;
        border-bottom-right-radius: 10px;
        border-top-right-radius: 10px;
        overflow: hidden;
        // background: rgba(50, 54, 65, 0.88);

        .ant-drawer-content {
          background: transparent;

          .ant-drawer-wrapper-body {
            overflow: auto;
            touch-action: auto;

            .ant-drawer-body {
              padding: 0;
              height: 100%;


              .left-nav {
                position: relative;

                left: 0;
                // transform: translate(0, -50%);
                width: 60px;
                color: #fff;
                border-top-right-radius: 5px;
                border-bottom-right-radius: 5px;
                top: 50%;
                transform: translate(0px, -50%);
                max-height: 100%;


                .left-tool {
                  padding: 10px 0;
                  display: flex;
                  flex-direction: column;
                  align-items: center;
                  justify-content: center;
                  font-size: 12px;
                  background: rgba(50, 54, 65, 0.88);
                  box-sizing: border-box;

                  cursor: pointer;

                  span {
                    margin-top: 10px;
                  }

                  &:hover {
                    background: rgba(5, 160, 129, 0.6);
                  }

                  &:first-child {
                    border-top-right-radius: 5px;
                  }

                  &:last-child {
                    border-bottom-right-radius: 5px;
                  }
                }
              }
            }
          }

        }

      }

    }

    .showDrawer {
      left: 60px !important;
    }

    .up-down {
      cursor: pointer;
      position: absolute;
      top: 50%;
      z-index: 1000;
      -webkit-transform: translate(0, -50%);
      transform: translate(0, -50%);
      left: 0px;
      background: rgba(50, 54, 65, 0.88);
      width: 30px;
      height: 80px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: left 0.3s;
      transition-timing-function: cubic-bezier(0.7, 0.3, 0.1, 1);

      .anticon {
        font-size: 22px;
        color: #fff;
      }

      .all-triangle {
        width: 0;
        height: 0;
        // border-left: 30px solid rgba(50, 54, 65, 0.88);
        // border-top: 30px solid transparent;
        // border-bottom: 30px solid transparent;
        position: absolute;
      }

      .up-triangle {
        top: -37%;
        border-bottom: 30px solid rgba(50, 54, 65, 0.88);
        border-right: 30px solid transparent;
      }

      .down-triangle {
        bottom: -37.1%;
        border-top: 30px solid rgba(50, 54, 65, 0.88);
        border-right: 30px solid transparent;
      }
    }
  }


  .active {
    background: rgba(5, 160, 129, 0.75) !important;
  }

  .footer-box {
    position: fixed;
    bottom: 35px;
    padding: 0;
    left: 50%;
    background-color: rgba(50, 54, 65, 0.92);
    border-radius: 3px;
    z-index: 1;
    max-width: 100%;
    transform: translate(-50%, 0);

  }

  .footer-btn {
    // position: relative;
    display: flex;
    overflow-x: auto;



    .btn-tool {
      text-align: center;
      margin: 0;
      padding: 8px 10px;
      cursor: pointer;

      img {
        width: 22px;
        height: 22px;
      }
    }

    .iconfont {
      font-size: 24px;
      color: #ffffff;
    }

    .iconfont:hover {
      color: rgba(5, 160, 129, 0.6);
    }

    .btn-tool:hover {
      background-color: rgba(5, 160, 129, 0.6);
      border-radius: 3px;

      >.iconfont {
        color: rgba(5, 160, 129, 0.6);
      }
    }

    /deep/.ant-tooltip {
      .ant-tooltip-inner {
        background-color: rgba(50, 54, 65, 1);
        border-radius: 3px;
        padding: 0px;

        .meature-btn {
          text-align: center;
          margin: 0px;
          padding: 8px 10px;
          cursor: pointer;
        }

        .meature-btn:hover {
          background-color: rgba(5, 160, 129, 0.6);
          border-radius: 3px;
        }
      }
    }
  }
</style>