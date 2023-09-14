import Vue from 'vue'
import moment from 'moment'
import 'moment/locale/zh-cn'
moment.locale('zh-cn')

Vue.filter('NumberFormat', function (value) {
  if (!value) {
    return '0'
  }
  const intPartFormat = value.toString().replace(/(\d)(?=(?:\d{3})+$)/g, '$1,') // 将整数部分逢三一断
  return intPartFormat
})



Vue.filter('dateFomart', function (value, pattern = 'YYYY-MM-DD HH:mm:ss') {
  if (value == '0001-01-01 00:00:00') return '日期未设置'
  if (!value) return ''
  return moment(value).format(pattern)
})


Vue.filter('tel', function (value) {
  return value.replace(value.substring(3, 7), '****')
})

Vue.filter('postilCategoryFomart', function (value, arr) {
  if (!value) return ''
  return arr.find(e => e.value == value).name
})

Vue.filter('userFomart', function (value, arr) {
  if (!value) return ''
  return arr.find(e => e.id == value)?.name
})

Vue.filter('userName', function (value) {
  if (!value) return ''
  let nameArr = []
  nameArr = value.map(e => e.name)
  return nameArr.join(' | ')
})

Vue.filter('projectFomart', function (value, arr) {
  if (!value) return ''
  return arr.find(e => e.id == value)?.projectName
})

Vue.filter('byteToMB', function (data, fixed = 2) {
  return data ? (data / 1024 / 1024).toFixed(fixed) : '--'
})

Vue.filter('byteToGB', function (data, fixed = 2) {
  let result = '--';
  var unit = ''
  if (data) {
    result = (data / 1024 / 1024).toFixed(fixed);
    unit = 'MB'
    if (result >= 1024) {
      result = (result / 1024).toFixed(fixed)
      unit = 'GB'
    }
  }
  return result + unit
})
Vue.filter('floatToVersion', function (data, fixed = 1) {
  return !data && String(data) !== '0' ? '--' : data.toFixed(fixed)
})
Vue.filter('aggregatedSize', function (data, fixed = 2) {
  const list = data.combineDetails
  let totleSize = 0
  for (let i = 0; i < list.length; i++) {
    totleSize += list[i].document.docSize
  }
  return (totleSize / 1024 / 1024).toFixed(fixed)
})

Vue.filter('totalType', function (data) {
  const list = data.combineDetails
  const type = []
  for (let i = 0; i < list.length; i++) {
    const index = type.indexOf(list[i].document.suffix)
    index == -1 ? type.push(list[i].document.suffix) : null
  }
  return type.toString()
})

Vue.filter('ellipsis', function (data) {
  if (!value) return ''
  if (value.length > len) {
    return value.slice(0, len) + '...'
  }
  return value
})