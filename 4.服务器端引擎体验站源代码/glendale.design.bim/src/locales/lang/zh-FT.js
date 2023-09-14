import antd from 'ant-design-vue/es/locale-provider/zh_CN'
import momentCN from 'moment/locale/zh-cn'
import modelList from './zh-FT/modelList'
import publicList from './zh-FT/pubilc'

const components = {
  antLocale: antd,
  momentName: 'zh-cn',
  momentLocale: momentCN
}

export default {
  ...modelList,
  ...publicList
}
