const bim = {
  state: {
    defaults: {//模型默认设置
      serverIP: ``, //服务ip地址
      port: 8000, //HTTP端口       
      container: `cesiumContainer`,   //[必须]容器id
      secretKey: ``,   //token
      openearth: false,//[可选]开启场景
      tintColor: "rgba(255,0,0,1)",//[可选]osgb单体化颜色
      // bgcolor: '',//[可选]bim模式场景背景色       
    },
    labelVisibleNow: [],   //当前显示的标签
    featureVisibleList: [],   //构件显示隐藏
    featureColorList: [],   //构件颜色
    featureRotationList: [],   //构件自动旋转
    pointSourceList: [],   //点光源
    spotlightList: [],   //点光源
    gasFlowList: [],   //烟雾扩散
    diffusionList: {
      circleType: [],
      customType: []
    },   //扩散
    radarScannList: [],   //雷达扫描
    settingsItem: {
      scene: '0',   //GIS场景
      topography: '0',   //地形
      sun: '1',     //太阳光
      automaticRotation: 0,   //自动旋转
      skyBox: '1',    //天空盒
      lightIntensity: 2,   //光照强度
      ambientLight: 0.5,   //环境光强度
      // sunshineTime: 420,   //日照时间
      navigationCube: '0',  //导航立方
      movementSpeed: 1,   //相机移动速度
      rotationSpeed: 2,    // 相机旋转速度
      zoomSpeed: 2,    //相机缩放速度
      sharpening: 3,     //锐化强度
      renderQuality: 100,    //渲染质量
      coplanarScintillation: '1',     //修复共面闪烁
      adjustment: '0',     //模型显示调整开启状态
      contrastRatio: 0,    //模型对比度
      saturationLevel: 0,    //模型饱和度
      exposure: 0,     //模型曝光度
      luminance: 0.1,   //模型平均亮度
      colorCurve: 0.1,    //模型色彩曲线
      cloudAltitudeRatio: 0.6,   //云层高度
      fpsSetting: 0,    //fps显示
      ensureFps: 10,  //保证帧率
    },
    featureTrolleyMobileList: [],   //构件移动
    featureSelfLuminousList: [],   //构件自发光
    trafficLinesList: [],   //交通热力线
    lightTrailingList: [],   //光线拖尾
    modelEditList: [],   //模型旋转偏移记录
    featureRotateList: [],     //构件旋转
    featureOffsetList: [],     //构件偏移
    selfLuminescentLineList: [],   //自发光线条
    escapeRoutesList: [],    //逃生路线
    videoFusion2DList: [],    //2D视频融合
    electronicFenceList: [],    //电子围栏
    defaultViewpoint: {},     //场景默认视点
    flattenList: [],     //模型压平
    croppingList: [],    //模型裁剪
    unitizationList: [],
    sunshineTime: 420,
    currentCoordinates: [], //当前坐标
    heatMapHistoryList: [],  //热力图
    currentClick: undefined,
    pakList: [],   //关卡
    pakCurrent: [],   //关卡
  },
  mutations: {
    LabelVisibleNow: (state, data) => {
      state.labelVisibleNow = data
    },
    FeatureVisibleList: (state, data) => {
      state.featureVisibleList = data
    },
    FeatureColorList: (state, data) => {
      state.featureColorList = data
    },
    FeatureRotationList: (state, data) => {
      state.featureRotationList = data
    },
    PointSourceList: (state, data) => {
      state.pointSourceList = data
    },
    SpotlightList: (state, data) => {
      state.spotlightList = data
    },
    GasFlowList: (state, data) => {
      state.gasFlowList = data
    },
    DiffusionList: (state, data) => {
      if (data.type == 1) {
        state.diffusionList.circleType = data.list
      } else {
        state.diffusionList.customType = data.list
      }
    },
    RadarScannList: (state, data) => {
      state.radarScannList = data
    },
    SettingsItemSave: (state, data) => {
      Object.assign(state.settingsItem, data)
      console.log(state.settingsItem)
    },
    FeatureTrolleyMobileList: (state, data) => {
      state.featureTrolleyMobileList = data
    },
    FeatureSelfLuminousList: (state, data) => {
      state.featureSelfLuminousList = data
    },
    TrafficLinesList: (state, data) => {
      state.trafficLinesList = data
    },
    LightTrailingList: (state, data) => {
      state.lightTrailingList = data
    },
    ModelEditList: (state, data) => {
      state.modelEditList = data
    },
    FeatureRotateList: (state, data) => {
      state.featureRotateList = data
    },
    FeatureOffsetList: (state, data) => {
      state.featureOffsetList = data
    },
    SelfLuminescentLineList: (state, data) => {
      state.selfLuminescentLineList = data
    },
    EscapeRoutesList: (state, data) => {
      state.escapeRoutesList = data
    },
    VideoFusion2DList: (state, data) => {
      state.videoFusion2DList = data
    },
    ElectronicFenceList: (state, data) => {
      state.electronicFenceList = data
    },
    DefaultViewpoint: (state, data) => {
      state.defaultViewpoint = data
    },
    FlattenList: (state, data) => {
      state.flattenList = data
    },
    CroppingList: (state, data) => {
      state.croppingList = data
    },
    UnitizationList: (state, data) => {
      state.unitizationList = data
    },
    SunshineTime: (state, data) => {
      state.sunshineTime = data
    },
    ObtainCoordinates: (state, data) => {
      state.currentCoordinates = data
    },
    HeatMapHistoryList: (state, data) => {
      state.heatMapHistoryList = data
    },
    CurrentClick: (state, data) => {
      state.currentClick = data
    },
    PakList: (state, data) => {
      state.pakList = data
    },
    PakCurrent: (state, data) => {
      state.pakCurrent = data
    },
    
  },
  actions: {
    //修改当前显示的标签列表
    GetLabelVisibleNow({ commit }, data) {
      commit('LabelVisibleNow', data)
    },
    GetFeatureVisibleList({ commit }, data) {
      commit('FeatureVisibleList', data)
    },
    GetFeatureColorList({ commit }, data) {
      commit('FeatureColorList', data)
    },
    GetFeatureRotationList({ commit }, data) {
      commit('FeatureRotationList', data)
    },
    GetPointSourceList({ commit }, data) {
      commit('PointSourceList', data)
    },
    GetSpotlightList({ commit }, data) {
      commit('SpotlightList', data)
    },
    GetGasFlowList({ commit }, data) {
      commit('GasFlowList', data)
    },
    GetDiffusionList({ commit }, data) {
      commit('DiffusionList', data)
    },
    GetRadarScannList({ commit }, data) {
      commit('RadarScannList', data)
    },
    SetSettingsItem({ commit }, data) {
      commit('SettingsItemSave', data)
    },
    GetFeatureTrolleyMobileList({ commit }, data) {
      commit('FeatureTrolleyMobileList', data)
    },
    GetFeatureSelfLuminousList({ commit }, data) {
      commit('FeatureSelfLuminousList', data)
    },
    GetTrafficLinesList({ commit }, data) {
      commit('TrafficLinesList', data)
    },
    GetLightTrailingList({ commit }, data) {
      commit('LightTrailingList', data)
    },
    GetModelEditList({ commit }, data) {
      commit('ModelEditList', data)
    },
    GetFeatureRotateList({ commit }, data) {
      commit('FeatureRotateList', data)
    },
    GetFeatureOffsetList({ commit }, data) {
      commit('FeatureOffsetList', data)
    },
    GetSelfLuminescentLineList({ commit }, data) {
      commit('SelfLuminescentLineList', data)
    },
    GetEscapeRoutesList({ commit }, data) {
      commit('EscapeRoutesList', data)
    },
    GetVideoFusion2DList({ commit }, data) {
      commit('VideoFusion2DList', data)
    },
    GetElectronicFenceList({ commit }, data) {
      commit('ElectronicFenceList', data)
    },
    GetDefaultViewpoint({ commit }, data) {
      commit('DefaultViewpoint', data)
    },
    GetFlattenList({ commit }, data) {
      commit('FlattenList', data)
    },
    GetCroppingList({ commit }, data) {
      commit('CroppingList', data)
    },
    GetUnitizationList({ commit }, data) {
      commit('UnitizationList', data)
    },
    GetSunshineTime({ commit }, data) {
      commit('SunshineTime', data)
    },
    GetObtainCoordinates({ commit }, { clickStatus, callback }) {
      api.Public.pickupCoordinate(false)
      setTimeout(() => {
        api.Public.pickupCoordinate(true, function (data) {
          api.Public.convertWorldToCartographicLocation(data, (position) => {
            commit('ObtainCoordinates', position)
            if (clickStatus && callback) {
              callback(data)
            }
          })
        });
      })
    },
    GetHeatMapHistoryList({ commit }, data) {
      commit('HeatMapHistoryList', data)
    },
    GetCurrentClick({ commit }, data) {
      commit('CurrentClick', data)
    },
    GetPakList({ commit }, data) {
      commit('PakList', data)
    },
    GetPakCurrent({ commit }, data) {
      commit('PakCurrent', data)
    },
    
  }
}

export default bim 
