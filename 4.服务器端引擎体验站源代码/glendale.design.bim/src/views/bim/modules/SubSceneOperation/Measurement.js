function getDistance(that) {//测量距离
  that.$message.destroy()
  that.$message.open({ content: '请点击拾取两个测量点', top: `50%`, duration: 2, maxCount: 1 })
  api.Measurement.distance(true, (json) => {
    if (json != undefined) {
      that.$notification.open({
        key: 'measurement',
        message: `距离`,
        description: '距离：' + json.distance.toFixed(2) + '米',
        placement: 'bottomRight',
        class: 'custom-notification',
        duration: null,
      })
    }
  })
}

function getArea(that) {//测量面积
  that.$message.destroy()
  that.$message.open({ content: '请点击选择三个及以上测量点,右键结束', top: `50%`, duration: 0, maxCount: 1 })
  api.Measurement.area(true)
  api.Public.event('RIGHT_CLICK', (res) => {
    api.Measurement.area(false, (json) => {
      if (json != undefined) {
        that.$notification.open({
          key: 'measurement',
          message: `面积`,
          description: '面积：' + json.area.toFixed(2) + '平方米',
          placement: 'bottomRight',
          class: 'custom-notification',
          duration: null,
        })
        api.Measurement.area(true)
      }
    })
  });
}

function getAngle(that) {//测量角度
  that.$message.destroy()
  that.$message.open({ content: '请点击选择3个测量点', top: `50%`, duration: 2, maxCount: 1 })
  api.Measurement.angle(true, (json) => {
    if (json != undefined) {
      that.$notification.open({
        key: 'measurement',
        message: `角度`,
        description: '夹角度数：' + json.angle.toFixed(2) + '度',
        placement: 'bottomRight',
        class: 'custom-notification',
        duration: null,
      })
    }
  })
}

var featureId = '';
function getFeatureArea(that) { //测量构件表面积
  that.$message.destroy()
  that.$message.open({ content: '请点击选择测量构件', top: `50%`, duration: 2, maxCount: 1 })
  api.Feature.getByEvent(true, (data) => {
    if (featureId) {
      api.Feature.setColor(featureId, 'rgba(255,255,255,1)', featureId.split('^')[0])
      featureId = ''
    }
    featureId = data
    api.Feature.setColor(data, "rgba(55,55,255,1)", data.split('^')[0])
    api.Measurement.featureArea(data, (json) => {
      if (json != undefined) {
        that.$notification.open({
          key: 'measurement',
          message: `构件表面积`,
          description: '表面积：' + parseFloat(json).toFixed(2) + '平方米',
          placement: 'bottomRight',
          class: 'custom-notification',
          duration: null,
        })
      }
    })
  })
}

function getVolume(that) {//测量构件体积
  that.$message.destroy()
  that.$message.open({ content: '请点击选择测量构件', top: `50%`, duration: 2, maxCount: 1 })
  api.Feature.getByEvent(true, (data) => {
    if (featureId) {
      api.Feature.setColor(featureId, 'rgba(255,255,255,1)', featureId.split('^')[0])
      featureId = ''
    }
    featureId = data
    api.Feature.setColor(data, "rgba(55,55,255,1)", data.split('^')[0])
    api.Measurement.featureVolume(data, (json) => {
      if (json != undefined) {
        that.$notification.open({
          key: 'measurement',
          message: `构件体积`,
          description: '体积：' + parseFloat(json).toFixed(2) + '立方米',
          placement: 'bottomRight',
          class: 'custom-notification',
          duration: null,
        })
      }
    })
  })
}

function clearTrace(that) {//清除测量
  that.$notification.close('measurement')
  that.$message.destroy()
  document.getElementById('EnginePage').style.cursor = 'default'
  api.Feature.getByEvent(false)  //关闭构件拾取事件
  api.Measurement.angle(false)
  api.Measurement.distance(false)
  api.Measurement.clearAllTrace()
  if (featureId) {
    api.Feature.setColor(featureId, 'rgba(255,255,255,1)', featureId.split('^')[0])
    featureId = ''
  }
}

function MeasurementType(type, that) {
  switch (type.id) {
    case 'DistanceMeasurement': //距离测量
      getDistance(that)
      break
    case 'AreaMeasurement': //面积测量
      getArea(that)
      break
    case 'AngleMeasurement': //角度测量
      getAngle(that)
      break
    case 'ComponentAreaMeasurement': //构件面积
      getFeatureArea(that)
      break
    case 'ComponentVolumeMeasurement': //构件体积
      getVolume(that)
      break
    case 'ClearResults': //清除测量
      clearTrace(that)
      break
  }
}

export {
  MeasurementType
}
