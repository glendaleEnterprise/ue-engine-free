<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-modal title="设置" :width="320" :visible="visible" :confirm-loading="confirmLoading" @ok="SetUp" v-drag
      :destroyOnClose="true" :maskClosable="false" :mask="false" @cancel="handleCancel" cancel-text="取消" ok-text="确定"
      :getContainer="() => this.$refs.wrap">
      <a-form-model :model="form" :hideRequiredMark="true" :label-col="labelCol" :wrapper-col="wrapperCol">
        <a-tabs default-active-key="1">
          <a-tab-pane key="1" tab="基础配置" class="scroll-box set-box">
            <a-form-model-item label="GIS场景" v-if="projectMessage.functionalPermissions">
              <a-radio-group v-model="form.scene">
                <a-radio value="1"> 开启 </a-radio>
                <a-radio value="0"> 关闭 </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item label="地形" v-if="form.scene == '1' && projectMessage.functionalPermissions">
              <a-radio-group v-model="form.topography">
                <a-radio value="1"> 开启 </a-radio>
                <a-radio value="0"> 关闭 </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item label="导航立方">
              <a-radio-group v-model="form.navigationCube" @change="NavigationCubeChange">
                <a-radio value="1"> 开启 </a-radio>
                <a-radio value="0"> 关闭 </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item label="太阳光">
              <a-radio-group v-model="form.sun">
                <a-radio value="1"> 开启 </a-radio>
                <a-radio value="0"> 关闭 </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item label="光照强度">
              <a-input-number placeholder="请输入光照强度" v-model="form.lightIntensity" :min="0" />
            </a-form-model-item>
            <a-form-model-item label="环境光强度">
              <a-input-number placeholder="请输入环境光强度" v-model="form.ambientLight" :min="0" :max="form.lightIntensity" />
            </a-form-model-item>
            <!-- <a-form-model-item label="日照时间">
              <a-slider @change="ModelExplosionChange" :max="780" :min="0" v-model="form.sunshineTime" :step="1"
                :tip-formatter="formatter" />
            </a-form-model-item> -->
            <a-form-model-item label="天空盒" v-show="form.scene == '0'">
              <a-radio-group v-model="form.skyBox">
                <a-radio value="1"> 开启 </a-radio>
                <a-radio value="0"> 关闭 </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item label="云海拔比率" v-show="form.skyBox == '1'">
              <a-input-number placeholder="云海拔高度比率" v-model="form.cloudAltitudeRatio" :min="0" />
            </a-form-model-item>
          </a-tab-pane>
          <a-tab-pane key="2" tab="其他配置" class="scroll-box set-box">
            <a-form-model-item label="移动速度">
              <a-input-number placeholder="请输入移动速度" v-model="form.movementSpeed" :min="0" />
            </a-form-model-item>
            <a-form-model-item label="旋转速度">
              <a-input-number placeholder="请输入旋转速度" v-model="form.rotationSpeed" :min="0" />
            </a-form-model-item>
            <a-form-model-item label="缩放速度">
              <a-input-number placeholder="请输入缩放速度" v-model="form.zoomSpeed" :min="0" />
            </a-form-model-item>
          </a-tab-pane>
          <a-tab-pane key="3" tab="场景配置" class="scroll-box set-box">
            <a-form-model-item label="锐化">
              <a-slider v-model="form.sharpening" :min="1" :max="20" />
            </a-form-model-item>
            <a-form-model-item label="渲染质量">
              <a-slider v-model="form.renderQuality" :min="50" :max="200" />
            </a-form-model-item>
            <a-form-model-item label="修复共面闪烁" style="display: none;">
              <a-radio-group v-model="form.coplanarScintillation">
                <a-radio value="1"> 开启 </a-radio>
                <a-radio value="0"> 关闭 </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item label="模型显示调整">
              <a-radio-group v-model="form.adjustment" @change="ModelDisplayAdjustment">
                <a-radio value="1"> 开启 </a-radio>
                <a-radio value="0"> 关闭 </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item label="模型对比度" v-if="form.adjustment == '1'">
              <a-slider v-model="form.contrastRatio" :min="-1" :max="1" :step="0.1" @change="ContrastRatioChange" />
            </a-form-model-item>
            <a-form-model-item label="模型饱和度" v-if="form.adjustment == '1'">
              <a-slider v-model="form.saturationLevel" :min="-1" :max="1" :step="0.1" @change="SaturationLevelChange" />
            </a-form-model-item>
            <a-form-model-item label="模型曝光度" v-if="form.adjustment == '1'">
              <a-slider v-model="form.exposure" :min="-1" :max="1" :step="0.1" @change="ExposureChange" />
            </a-form-model-item>
            <a-form-model-item label="模型平均亮度" v-if="form.adjustment == '1'">
              <a-slider v-model="form.luminance" :min="0" :max="1" :step="0.1" @change="LuminanceChange" />
            </a-form-model-item>
            <a-form-model-item label="模型色彩曲线" v-if="form.adjustment == '1'">
              <a-slider v-model="form.colorCurve" :min="-1" :max="1" :step="0.01" @change="ColorCurveChange" />
            </a-form-model-item>
            <a-form-model-item label="帧率">
              <a-radio-group v-model="form.fpsSetting" @change="DisplayFrameRate">
                <a-radio :value="1"> 开启 </a-radio>
                <a-radio :value="0"> 关闭 </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item label="保证帧率">
              <a-input-number placeholder="请输入保证帧率" v-model="form.ensureFps" :min="0" />
            </a-form-model-item>
          </a-tab-pane>
        </a-tabs>
      </a-form-model>
    </a-modal>
  </div>
</template>

<script>
  import store from '@/store'
  import {
    setSceneConfig
  } from '@/api/document'
  import {
    combineSceneConfig
  } from '@/api/combine'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    props: {
      settingVisible: {
        type: Boolean,
        required: true,
      },
      projectMessage: {
        type: Object,
        default: undefined,
      },
    },
    data() {
      return {
        confirmLoading: false,
        visible: this.settingVisible,
        form: {
          scene: '0', //GIS场景
          topography: '0', //地形
          sun: '1', //太阳光
          automaticRotation: 0, //自动旋转
          skyBox: '1', //天空盒
          lightIntensity: 2, //光照强度
          ambientLight: 0.5, //环境光强度
          // sunshineTime: 420,   //日照时间
          navigationCube: '0', //导航立方
          movementSpeed: 1, //相机移动速度
          rotationSpeed: 2, // 相机旋转速度
          zoomSpeed: 2, //相机缩放速度
          sharpening: 3, //锐化强度
          renderQuality: 100, //渲染质量
          coplanarScintillation: '1', //修复共面闪烁
          adjustment: '0', //模型显示调整开启状态
          contrastRatio: 0, //模型对比度
          saturationLevel: 0, //模型饱和度
          exposure: 0, //模型曝光度
          luminance: 0.1, //模型平均亮度
          colorCurve: 0.1, //模型色彩曲线
          cloudAltitudeRatio: 0.6, //云层高度
          fpsSetting: 0, //fps显示
          ensureFps: 10, //保证帧率
        },
        // timeValue: '13:00',
        labelCol: {
          span: 8
        },
        wrapperCol: {
          span: 15
        },
        transitionForm: {},
        isMobile: false
      }
    },
    mounted() {
      const that = this;
      this.isMobile = _isMobile() ? true : false;
      Object.assign(that.form, store.state.bim.settingsItem)
      that.transitionForm = {
        ...that.form
      }
    },
    methods: {
      handleCancel() {
        this.visible = false;
        this.$emit('update:settingVisible', this.visible)
      },
      formatter(value) {
        var time = 6 + parseInt(value / 60) + ":" + (String(Math.round(value % 60)).length == 1 ? '0' + Math.round(
          value % 60) : Math.round(value % 60))
        return `${time}`;
      },
      // ModelExplosionChange(value) {
      //   const that = this;
      //   let time = [6 + parseInt(value / 60), (value % 60)];
      //   let minute = parseInt(time[1]) < 10 ? '0' + time[1] : JSON.stringify(time[1])
      //   api.Public.setTime({
      //     'Time': time[0] + ":" + minute
      //   })
      //   that.timeValue = time[0] + ":" + minute;
      //   that.transitionForm.sunshineTime = that.form.sunshineTime;
      // },
      NavigationCubeChange(e) {
        const that = this;
        if (that.form.navigationCube == '1') {
          api.Plugin.addCompass();
          api.Plugin.updateCompass({
            "Anchor": 1,
            "Offset": that.form.fpsSetting == 1 ? [-250, 85] : [-170, 85]
          });
        } else {
          api.Plugin.deleteCompass();
        }
        that.transitionForm.navigationCube = that.form.navigationCube
      },
      DisplayFrameRate() {
        const that = this;
        if (that.form.fpsSetting == 1) {
          let options = {
            state: true,
            anchor: 1,
            offset: [-100, 85],
          }
          api.Plugin.showFPS(options);
        } else {
          let options = {
            state: false,
          }
          api.Plugin.showFPS(options);
        }
        api.Plugin.updateCompass({
          "Anchor": 1,
          "Offset": that.form.fpsSetting == 1 ? [-250, 85] : [-170, 85]
        });
        that.transitionForm.fpsSetting = that.form.fpsSetting
      },
      ContrastRatioChange(value) {
        const that = this;
        api.Model.setContrast(value)
        that.transitionForm.contrastRatio = that.form.contrastRatio
      },
      SaturationLevelChange(value) {
        const that = this;
        api.Model.setSaturation(value)
        that.transitionForm.saturationLevel = that.form.saturationLevel
      },
      ExposureChange(value) {
        const that = this;
        api.Model.setLuminance(value)
        that.transitionForm.exposure = that.form.exposure
      },
      LuminanceChange(value) {
        const that = this;
        api.Model.setAverageBrightness(value)
        that.transitionForm.luminance = that.form.luminance
      },
      ColorCurveChange(value) {
        const that = this;
        api.Model.setColorCurve(value)
        that.transitionForm.colorCurve = that.form.colorCurve
      },
      ModelDisplayAdjustment() {
        const that = this;
        if (that.form.adjustment == '0') {
          api.Model.setContrast(0)
          api.Model.setSaturation(0)
          api.Model.setLuminance(0);
          api.Model.setAverageBrightness(0.1)
          api.Model.setColorCurve(0.1)
        } else {
          api.Model.setContrast(that.form.contrastRatio)
          api.Model.setSaturation(that.form.saturationLevel)
          api.Model.setLuminance(that.form.exposure);
          api.Model.setAverageBrightness(that.form.luminance)
          api.Model.setColorCurve(that.form.colorCurve)
        }
        that.transitionForm.adjustment = that.form.adjustment;
        that.transitionForm.contrastRatio = that.form.contrastRatio;
        that.transitionForm.saturationLevel = that.form.saturationLevel;
        that.transitionForm.exposure = that.form.exposure;
        that.transitionForm.luminance = that.form.luminance;
        that.transitionForm.colorCurve = that.form.colorCurve;
      },
      SetUp() {
        const that = this;
        if (that.objectValueAllEmpty(that.form)) {
          that.$message.warning('设置项参数不能为空，请填写！')
          return
        }
        // gis场景
        if (that.form.scene == '1') {
          api.Public.setGisState(true)
          that.form.skyBox = '1'
        } else {
          api.Public.setGisState(false)
        }
        // 地形
        if (that.form.topography == '1') {
          api.Public.setTerrainState(true)
        } else {
          api.Public.setTerrainState(false)
        }
        // 天空盒
        if (that.form.skyBox == '1') {
          api.Public.setSkyBoxState(1);
          api.Public.setCloud({
            altitude: that.form.cloudAltitudeRatio
          });
        } else {
          api.Public.setSkyBoxState(0);
        }
        //太阳光
        if (that.form.sun == '1') {
          api.Public.setSunlightState(true, that.form.lightIntensity)
        } else {
          api.Public.setSunlightState(false)
        }
        that.transitionForm.ambientLight != that.form.ambientLight ? api.Public.setSceneBrightness(that.form
          .ambientLight) : null //环境光强度
        api.Camera.setSpeed(that.form.movementSpeed, that.form.rotationSpeed, that.form.zoomSpeed); //相机速度
        that.transitionForm.sharpening != that.form.sharpening ? api.Public.setSharpen(that.form.sharpening) :
          null; //锐化强度
        that.transitionForm.renderQuality != that.form.renderQuality ? api.Public.renderQuality(that.form
          .renderQuality) : null; //渲染质量
        if (that.transitionForm.coplanarScintillation != that.form.coplanarScintillation) {
          that.projectMessage.modelList.forEach(item => {
            api.Public.enableFixCoincide(item.id, that.form.coplanarScintillation == '1' ? true : false)
          })
        }
        that.transitionForm.ensureFps != that.form.ensureFps ? api.Public.ensureFPS(that.form.ensureFps) : null; //保证帧率
        store.dispatch('SetSettingsItem', that.form)
        that.visible = false;
        that.$emit('update:settingVisible', that.visible)
        that.projectMessage.sceneConfig = that.form;
        that.transitionForm = that.form;
        if (that.projectMessage.sceneType == 0) {
          setSceneConfig(that.projectMessage.modelList[0].id, JSON.stringify(that.form)).then(res => {
            that.$message.success('设置成功')
          })
        } else {
          combineSceneConfig({
            id: that.projectMessage.id,
            SceneConfig: JSON.stringify(that.form),
          }).then(res => {
            that.$message.success('设置成功')
          })
        }
      },
      objectValueAllEmpty(object) {
        var isEmpty = false;
        Object.keys(object).forEach(function (x) {
          if (!object[x] && String(object[x]) !== '0') {
            isEmpty = true;
          }
        });
        return isEmpty;
      }
    },
    destroyed() {
      store.dispatch('SetSettingsItem', this.transitionForm)
    }
  }
</script>

<style lang="less" scoped>
  @import '../BimCss.less';

  /deep/.ant-modal {
    .set-box {
      height: 20vh;
      margin: 5px 0;
    }

    .ant-tabs-bar {
      margin: 0;
    }

    .ant-tabs-nav {
      .ant-tabs-tab {
        margin: 0;

      }
    }

    .ant-radio-group {
      width: 100%;

      .ant-radio-wrapper {
        color: #ffffff;
        margin-right: 0;
        width: 50%;

        .ant-radio-inner {
          background: #fff0;
        }
      }
    }
  }
</style>