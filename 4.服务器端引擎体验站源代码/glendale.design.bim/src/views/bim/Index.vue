<template>
  <a-layout id="EnginePage">
    <a-layout-content>
      <div id="cesiumContainer"></div>
      <menu-classification :projectMessage="projectMessage" :settingVisible.sync="settingVisible"
        :layerListVisible.sync="layerListVisible" ref="menu" v-if="loadingCompleted"></menu-classification>
      <setting-interface :settingVisible.sync="settingVisible" :projectMessage="projectMessage" v-if="settingVisible">
      </setting-interface>
      <layer-list :projectMessage="projectMessage" :layerListVisible.sync="layerListVisible" v-if="layerListVisible">
      </layer-list>
      <div class="loading-box" v-show="!loadingCompleted && validating && $store.state.showLogo">
        <img src="../../static/img/logoloading.gif" alt="">
      </div>
      <security-verification :securityVerification.sync="securityVerification" @SendMsg="InitValidation"
        v-if="securityVerification"></security-verification>
      <div class="tips-loading" v-show="loadTime">{{ loadTime }}</div>
      <div class="click-location" v-show="$store.state.bim.currentCoordinates.length == 3">
        <div>经度：{{ $store.state.bim.currentCoordinates[0] }}°</div>
        <div>纬度：{{ $store.state.bim.currentCoordinates[1] }}°</div>
        <div>高程：{{ $store.state.bim.currentCoordinates[2] }}M</div>
      </div>
    </a-layout-content>
  </a-layout>
</template>
<script>
  import SecurityVerification from '@/components/SecurityVerification.vue'
  import {
    mapGetters
  } from 'vuex'
  import {
    getArray,
    modelProjectCover
  } from '@/api/document'
  import {
    getClampById,
    combineProjectCover
  } from '@/api/combine'
  import MenuClassification from './modules/MenuClassification'
  import SettingInterface from './modules/SettingInterface'
  import LayerList from './modules/LayerManagement/LayerList.vue'
  import {
    genlabID,
    _isMobile
  } from '@/api/public'
  import {
    uploadFile
  } from '@/api/file'
  export default {
    name: 'BIM',
    components: {
      MenuClassification,
      SettingInterface,
      LayerList,
      SecurityVerification
    },
    props: {
      ids: {
        type: Object,
        default: undefined,
      },
    },
    computed: {
      ...mapGetters(['currProject']),
      ...mapGetters({
        currentUser: 'userInfo',
      }),
    },
    data() {
      return {
        projectMessage: {
          functionalPermissions: true, //公开项目功能权限控制
        },
        modelList: [],
        modelArr: [],
        camera: null,
        settingVisible: false, //设置框显示状态
        layerListVisible: false, //图层管理显示状态
        loadingCompleted: false,
        blobName: null,
        securityVerification: true,
        validating: false,
        startTime: undefined,
        loadTime: undefined,
      }
    },
    mounted() {
      const that = this
      if (that.$route.name == "sharelink" && !that.ids.needCheck) {
        that.securityVerification = false
        that.InitValidation()
      } else if (_isMobile()) {
        // let body = document.getElementsByTagName("body");
        // body[0].style.cssText =
        //   `width: 100vh;
        //   height: 100vw;
        //   transform-origin: 0 0;
        //   position: fixed;
        //   top: 0;
        //   left: 0;
        //   transform: rotateZ(90deg) translateY(-100%);`
        that.securityVerification = false
        that.InitValidation()
      } else if (that.$store.state.whetherVerify) {
        that.securityVerification = false
        that.InitValidation()
        that.loadingCompleted = true;
      }
      // window.event.returnValue = true

    },
    methods: {
      InitValidation() {
        const that = this
        that.validating = true;
        if (that.currProject && that.currProject.isPublic && that.currentUser.userName != that.currProject
          .creationAccount) {
          that.projectMessage.functionalPermissions = false;
        }
        if (that.currProject || that.$route.name == "sharelink") {
          const defaults = {
            ...that.$store.state.bim.defaults
          }
          api = new SAPI(defaults, () => {
            that.InitEngine()
            that.$store.dispatch('GetObtainCoordinates', {
              clickStatus: true
            })
          })
        } else {
          that.$router.replace({
            path: '/project'
          })
        }
      },
      async InitEngine() {
        const that = this
        that.modelList = []
        if (that.$route.name == "model") { //单模型
          that.projectMessage.sceneClassification = 0
          await getArray(that.$route.params.id).then((result) => {
            if (result != null) {
              that.InitScene(0, result)
            }
          })
          that.projectMessage.projectId = that.currProject.id
        } else if (that.$route.name == "combine") { //合模
          that.projectMessage.sceneClassification = 1
          await getClampById(that.$route.params.id).then((result) => {
            if (result != null) {
              that.InitScene(1, result)
            }
          })
          that.projectMessage.projectId = that.currProject.id
        } else if (that.$route.name == "sharelink") { //分享
          that.projectMessage.sceneClassification = 2
          if (that.ids.sceneType == 0) {
            await getArray(that.ids.id)
              .then((result) => {
                if (result != null) {
                  that.InitScene(0, result)
                  that.projectMessage.projectId = result[0].projectId
                }
              })
          } else {
            await getClampById(that.ids.id)
              .then((result) => {
                if (result != null) {
                  that.InitScene(1, result)
                  that.projectMessage.projectId = result.projectId
                }
              })
          }
        }
      },
      InitScene(type, result) {
        const that = this;
        let sceneConfig = undefined;
        let camera = null;
        if (type == 0) {
          result.forEach((res) => {
            let matrixData = res.matrix ? JSON.parse(res.matrix) : null;
            let list = that.$store.state.bim.modelEditList;
            if (matrixData) {
              matrixData.modelId = res.id;
            } else {
              matrixData = {
                modelId: res.id,
                rotate: [0, 0, 0],
                offset: [0, 0, 0]
              }
            }
            list.push(matrixData)
            that.$store.dispatch('GetModelEditList', list);
            that.blobName = res.blobName;
            //后期可删除
            if (res.modelConfig) { //模型配置
              res.modelConfig = JSON.parse(res.modelConfig)
            } else {
              if (res.modelFormat == 'osgb') {
                res.modelConfig = {
                  visibleDistance: 0.5
                }
              } else if (res.modelFormat == '点云' || res.modelFormat == 'las') {
                res.modelConfig = {
                  visibleDistance: 0.5
                }
              } else {
                res.modelConfig = {
                  visibleDistance: 2000
                }
              }
            }
            //完
            that.modelList.push({
              modelName: res.modelName,
              id: res.id,
              documentName: res.documentName,
              documentType: res.documentType,
              modelFormat: res.modelFormat,
              size: res.size,
              projectFolderId: res.projectFolderId,
              matrix: matrixData,
              isMain: that.modelList.length == 0 ? true : false,
              isShow: true,
              modelConfig: res.modelConfig
            })
            sceneConfig = JSON.parse(res.sceneConfig)
            that.projectMessage.id = res.id;
          })
          camera = result[0].camera ? JSON.parse(result[0].camera) : null
        } else {
          that.blobName = result.blobName
          result.combineDetails.forEach(res => {
            let matrixData = res.matrix ? JSON.parse(res.matrix) : null;
            let list = that.$store.state.bim.modelEditList;
            if (matrixData) {
              matrixData.modelId = res.documentId;
            } else {
              matrixData = {
                modelId: res.documentId,
                rotate: [0, 0, 0],
                offset: [0, 0, 0]
              }
            }
            list.push(matrixData)
            that.$store.dispatch('GetModelEditList', list);
            //后期可删除
            if (res.modelConfig) { //模型配置
              res.modelConfig = JSON.parse(res.modelConfig)
            } else {
              if (res.document.modelFormat == 'osgb') {
                res.modelConfig = {
                  visibleDistance: 0.5
                }
              } else if (res.document.modelFormat == '点云' || res.document.modelFormat == 'las') {
                res.modelConfig = {
                  visibleDistance: 0.5
                }
              } else {
                res.modelConfig = {
                  visibleDistance: 2000
                }
              }
            }
            //完
            that.modelList.push({
              modelName: res.document.modelName,
              id: res.documentId,
              documentName: res.document.documentName,
              documentType: res.document.documentType,
              modelFormat: res.document.modelFormat,
              size: res.document.size,
              projectFolderId: res.document.projectFolderId,
              matrix: matrixData,
              documentId: res.documentId,
              isMain: that.modelList.length == 0 ? true : false,
              isShow: true,
              modelConfig: res.modelConfig
            })
          })
          that.projectMessage.combineFlattens = result.combineFlattens;
          sceneConfig = JSON.parse(result.sceneConfig);
          camera = result.camera ? JSON.parse(result.camera) : null;
          that.projectMessage.folderId = result.folderId;
          that.projectMessage.combineName = result.combineName;
          that.projectMessage.id = result.id;
        }
        if (sceneConfig) {
          that.$store.dispatch('SetSettingsItem', sceneConfig)
          sceneConfig.scene == 1 ? api.Public.setGisState(true) : api.Public.setGisState(false,
            'rgba(135 ,206 ,250,1)');
          sceneConfig.topography == 1 ? api.Public.setTerrainState(true) : api.Public.setTerrainState(false);
          sceneConfig.sun == 1 ? api.Public.setSunlightState(true, sceneConfig.lightIntensity) : api.Public
            .setSunlightState(false);
          api.Public.setSceneBrightness(sceneConfig.ambientLight) //环境光强度
          // let time = [6 + parseInt(sceneConfig.sunshineTime / 60), (sceneConfig.sunshineTime % 60)];
          // let minute = parseInt(time[1]) < 10 ? '0' + time[1] : JSON.stringify(time[1])
          // api.Public.setTime({
          //   'Time': time[0] + ":" + minute
          // })
          api.Public.setTime({
            'Time': "13:00"
          })
          if (sceneConfig.skyBox == 0) {
            api.Public.setSkyBoxState(0)
          } else {
            api.Public.setSkyBoxState(1);
            if (sceneConfig.cloudAltitudeRatio) {
              api.Public.setCloud({
                altitude: sceneConfig.cloudAltitudeRatio
              });
            }
          }
          if (sceneConfig.navigationCube == 1) {
            api.Plugin.addCompass()
            api.Plugin.updateCompass({
              "Anchor": 1,
              "Offset": sceneConfig.fpsSetting == 1 ? [-250, 85] : [-170, 85]
            });
          } else {
            api.Plugin.deleteCompass()
          }
          if (sceneConfig.fpsSetting == 1) {
            let options = {
              state: true,
              anchor: 1,
              offset: [-100, 85],
            }
            api.Plugin.showFPS(options);
            if (sceneConfig.navigationCube == 1) {
              api.Plugin.updateCompass({
                "Anchor": 1,
                "Offset": [-250, 85]
              });
            }
          }
          api.Camera.setSpeed(sceneConfig.movementSpeed, sceneConfig.rotationSpeed, sceneConfig.zoomSpeed); //相机速度
          api.Public.setSharpen(sceneConfig.sharpening); //锐化强度
          api.Public.renderQuality(sceneConfig.renderQuality); //渲染质量
          api.Public.ensureFPS(sceneConfig.ensureFps ? sceneConfig.ensureFps : 10); //保证帧率
          if (sceneConfig.adjustment == 1) {
            api.Model.setContrast(sceneConfig.contrastRatio)
            api.Model.setSaturation(sceneConfig.saturationLevel)
            api.Model.setLuminance(sceneConfig.exposure);
            api.Model.setAverageBrightness(sceneConfig.luminance)
            api.Model.setColorCurve(sceneConfig.colorCurve)
          } else {
            api.Model.setContrast(0)
            api.Model.setSaturation(0)
            api.Model.setLuminance(0);
            api.Model.setAverageBrightness(0.1)
            api.Model.setColorCurve(0.1)
          }
        }
        if (camera) {
          that.$store.dispatch('GetDefaultViewpoint', camera)
          api.Camera.setViewPort(camera)
        }
        that.projectMessage.modelList = that.modelList
        that.projectMessage.modelId = that.modelList[0].id
        that.projectMessage.documentType = that.modelList[0].documentType
        that.projectMessage.sceneType = type;
        that.projectMessage.camera = camera;
        that.projectMessage.sceneConfig = sceneConfig;
        that.modelArr = [...that.modelList]
        that.startTime = new Date()
        that.AddModelNew(that.modelArr) //初始加载模型
      },
      AddModelNew(data) {
        const that = this
        if (data.length == 0) {
          if (that.blobName == null) {
            that.setProjectCover()
          }
          that.loadingCompleted = true;
          that.loadTime = "模型加载完成，用时" + that.getDateDiff(that.startTime, new Date(), "second") + "秒"
          setTimeout(() => {
            that.loadTime = undefined
          }, 2000)
          if (that.projectMessage.combineFlattens && that.projectMessage.combineFlattens.length > 0) {
            that.modelList.forEach(item => {
              if (item.documentType == 2) {
                that.projectMessage.combineFlattens.forEach(item1 => {
                  var opt = {
                    id: 'flatten_' + genlabID(6),
                    positions: JSON.parse(item1.flattenScope),
                    height: item1.flattenHeight,
                  }
                  api.Model.addFlatten(item.id, opt);
                })
              }
            })
          }
          return;
        }
        if (that.$store.state.specialType.length > 0 && that.$store.state.specialType.filter(item => item == that.$route
            .params.id).length > 0) {
          that.loadingCompleted = true
        }
        var modelInfo = data.shift();
        var url = `${that.$store.state.modelUrl}/Tools/output/model/${modelInfo.modelName}/root.glt`
        if (modelInfo.documentType == 2) {
          url = `${that.$store.state.modelUrl}/Tools/output/model/${modelInfo.modelName}/tileset.json`
        }
        let havePak = that.$store.state.pakAllList.length > 0 ?
          that.$store.state.pakAllList.filter(item => item.id == modelInfo.id) : []
        if (havePak.length > 0) {
          api.Pak.loadStreamPak({
            "PakPath": havePak[0].pakPath,
            "MountPoint": "../../../",
            "PackageName": havePak[0].packageName,
            "MapName": havePak[0].mapName,
          }, (res) => {
            that.AddModelNew(data)
          })
        } else if (modelInfo.documentType == '3') {
          api.Pak.loadStreamPak({
            "PakPath": "../../../../Servers/Matchmaker/public/" +
              modelInfo.modelName + '.pak',
            "PackageName": modelInfo.modelName.split("-Windows")[0],
          }, (res) => {
            console.log(res);
            if (res.isSuccessed) {
              res.LevelMaps.length > 0 ?
                api.Pak.loadMap({
                  MapName: res.LevelMaps[0]
                }, (res1) => {
                  that.$store.dispatch('GetPakCurrent', [res.LevelMaps[0]]);
                  that.AddModelNew(data)
                }) :
                that.$message.warning('未检测到关卡！')
              that.$store.dispatch('GetPakList', res.LevelMaps);

            } else {
              that.$message.error('挂载失败，请检查UE工程文件！')
            }
          })
        } else {
          console.log(url)
          api.Model.add(
            url,
            modelInfo.id,
            () => {
              modelInfo.matrix && modelInfo.matrix.offset ? api.Model.offset(modelInfo.matrix.offset[0], modelInfo
                .matrix.offset[1], modelInfo.matrix.offset[2], modelInfo.id) : null;
              modelInfo.matrix && modelInfo.matrix.rotate ? api.Model.rotate(modelInfo.matrix.rotate[0], modelInfo
                .matrix.rotate[1], modelInfo.matrix.rotate[2], modelInfo.id) : null;
            },
            () => {
              that.AddModelNew(data)
            }, {
              FlyTo: that.projectMessage.camera ? false : true, //场地模型设置true，其他模型均设置为false
              VisualRange: modelInfo.modelConfig.visibleDistance, //模型可视距离
              // EnableTransparency: true,//是否启用设置半透明，包括构件半透明
              // Offset: modelInfo.matrix && modelInfo.matrix.offset ? modelInfo.matrix.offset : [0, 0, 0], // 偏移
              // Rotate: modelInfo.matrix && modelInfo.matrix.rotate ? modelInfo.matrix.rotate : [0, 0, 0], // 旋转
            }
          )
        }
      },
      setSkyBoxState(e) {
        api.Public.setSkyBoxState(e ? 1 : 0)
      },
      setProjectCover() {
        const that = this;
        setTimeout(() => {
          api.Public.saveScreenShot((res) => {
            const uploadImg = that.convertBase64UrlToBlob(res)
            uploadFile(uploadImg)
              .then((res) => {
                that.projectMessage.sceneType == 0 ?
                  modelProjectCover(that.projectMessage.id, res) :
                  combineProjectCover(that.projectMessage.id, res)
              })
          })
        }, 1000)
      },
      convertBase64UrlToBlob(urlData) {
        const fd = new FormData()
        var bytes = window.atob(urlData.split(',')[1]) //去掉url的头，并转换为byte
        //处理异常,将ascii码小于0的转换为大于0
        var ab = new ArrayBuffer(bytes.length)
        var ia = new Uint8Array(ab)
        for (var i = 0; i < bytes.length; i++) {
          ia[i] = bytes.charCodeAt(i)
        }
        const blob = new Blob([ab], {
          type: 'image/png'
        })
        fd.append('file', blob, new Date().getTime() + '.png')
        return fd
      },
      getDateDiff(startTime, endTime, diffType) {
        //将xxxx-xx-xx的时间格式，转换为 xxxx/xx/xx的格式
        //将计算间隔类性字符转换为小写
        diffType = diffType.toLowerCase();
        var sTime = startTime; //开始时间
        var eTime = endTime; //结束时间
        //作为除数的数字
        var timeType = 1;
        switch (diffType) {
          case "second":
            timeType = 1000;
            break;
          case "minute":
            timeType = 1000 * 60;
            break;
          case "hour":
            timeType = 1000 * 3600;
            break;
          case "day":
            timeType = 1000 * 3600 * 24;
            break;
          default:
            break;
        }
        let time = parseInt((eTime.getTime() - sTime.getTime()) / parseInt(timeType))
        time == 0 ? time = 0.1 : null
        return time;
      }
    },
    destroyed() {
      this.$refs.menu ? this.$refs.menu.ClearEvent() : null
      this.$notification.destroy()
    },
  }
</script>

<style lang="less" scoped>
  @import './BimCss.less';
</style>
<style lang="less">
  .ant-notification-topRight {
    top: 15vh !important;
  }

  .ant-notification {
    width: auto !important;
    margin: 0;

    .engine-notification {
      background-color: rgba(50, 54, 65, 0.88);
      color: #fff;
      margin-bottom: 0;

      .ant-notification-notice-message {
        color: #fff;
      }

      .ant-notification-notice-close {
        color: #fff;
      }
    }
  }

  .ant-notification-bottomRight {
    right: 20px !important;
    bottom: 35px !important;

    .ant-notification-notice {
      background: rgba(50, 54, 65, 0.88);
      color: #ffffff;

      .ant-notification-notice-message {
        color: #ffffff;
      }

      .ant-notification-notice-close {
        color: #ffffff;
      }
    }
  }

  .ant-modal-confirm {
    .ant-modal-wrap {
      z-index: 1011;
    }
  }

  .ant-notification-bottomLeft {
    bottom: 0 !important;

    .engine-notification {
      width: 100vw !important;
      padding: 10px 24px;

      .scroll-box {
        max-height: 18vh !important
      }
    }
  }

  .mobile-Model {
    .ant-modal-wrap {
      top: 35%;

      .ant-modal-body {
        padding: 6px 15px;
      }

      .ant-modal-footer {
        padding: 10px 16px;
      }
    }

    .ant-form-item-label {
      width: 30% !important;
      text-align: right;
      padding-right: 10px;
      padding-bottom: 0;
    }

    .ant-form-item-control-wrapper {
      width: 67% !important;
    }

    .ant-col-24 {
      width: 100% !important;
    }
  }
</style>