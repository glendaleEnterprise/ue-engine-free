const path = require('path')
const webpack = require('webpack')
const GitRevisionPlugin = require('git-revision-webpack-plugin')
const GitRevision = new GitRevisionPlugin()
const buildDate = JSON.stringify(new Date().toLocaleString())
const CopyWebpackPlugin = require('copy-webpack-plugin') //引入插件
// const createThemeColorReplacerPlugin = require('./config/plugin.config')

function resolve(dir) {
  return path.join(__dirname, dir)
}

// check Git
function getGitHash() {
  try {
    return GitRevision.version()
  } catch (e) {}
  return 'unknown'
}

const isProd = process.env.NODE_ENV === 'production'

const assetsCDN = {
  // webpack build externals
  externals: {
    vue: 'Vue',
    'vue-router': 'VueRouter',
    vuex: 'Vuex',
    axios: 'axios'
  },
  css: [],
  // https://unpkg.com/browse/vue@2.6.10/
  js: [
    '//cdn.jsdelivr.net/npm/vue@2.6.14/dist/vue.min.js',
    '//cdn.jsdelivr.net/npm/vue-router@3.5.2/dist/vue-router.min.js',
    '//cdn.jsdelivr.net/npm/vuex@3.1.1/dist/vuex.min.js',
    '//cdn.jsdelivr.net/npm/axios@0.21.1/dist/axios.min.js'
  ]
}

// vue.config.js
const vueConfig = {
  configureWebpack: {
    // webpack plugins
    plugins: [
      // Ignore all locale files of moment.js
      new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/),
      new webpack.DefinePlugin({
        APP_VERSION: `"${require('./package.json').version}"`,
        GIT_HASH: JSON.stringify(getGitHash()),
        BUILD_DATE: buildDate
      }),
      new CopyWebpackPlugin([{
        from: resolve('src/static'),
        to: 'static'
      }]),
    ],
    //打包文件大小配置
    performance: {
      'hints': false, // 取消打包文件过大的警告
      'maxEntrypointSize': 10000000,
      'maxAssetSize': 30000000
    },
    externals: {
      'wxLogin': 'WwLogin'
    },
    resolve: {
      alias: {
        '@': resolve('src')
      }
    }
    // if prod, add externals
    // externals: isProd ? assetsCDN.externals : {}
  },

  chainWebpack: config => {
    config.resolve.alias.set('@$', resolve('src'))

    const svgRule = config.module.rule('svg')
    svgRule.uses.clear()
    svgRule
      .oneOf('inline')
      .resourceQuery(/inline/)
      .use('vue-svg-icon-loader')
      .loader('vue-svg-icon-loader')
      .end()
      .end()
      .oneOf('external')
      .use('file-loader')
      .loader('file-loader')
      .options({
        name: 'assets/[name].[hash:8].[ext]'
      })
    config.plugin('html').tap(args => {
      args[0].title =process.env.VUE_APP_TITLE
      return args
    })
    // if prod is on
    // assets require on cdn
    // if (isProd) {
    //   config.plugin('html').tap(args => {
    //     args[0].cdn = assetsCDN
    //     return args
    //   })
    // }
  },

  css: {
    loaderOptions: {
      less: {
        modifyVars: {
          // less vars，customize ant design theme
          // 'font-family': 'Microsoft YaHei',//字体
          'primary-color': '#05a081', // 全局主色
          'link-color': '#05a081', // 链接色
          'link-hover-color': '#05a081',
          'link-active-color': '#05a081',
          // 'text-color': '#fff',// 主文本色
          'border-radius-base': '2px', // 组件/浮层圆角           
        },
        // DO NOT REMOVE THIS LINE
        javascriptEnabled: true
      },
      // css: {},
      // postcss: {
      //   plugins: [
      //     require('postcss-px2rem')({
      //       // 以设计稿750为例， 750 / 10 = 75
      //       remUnit: 54
      //     }),
      //   ]
      // }
    },
    extract: process.env.NODE_ENV === 'production' ? {
      ignoreOrder: true,
   } : false,
  },

  devServer: {
    overlay: {
      warning: false,
      errors: false
    },
    // development server port 8100
    port: 8200, //8100,
    // If you want to turn on the proxy, please remove the mockjs /src/main.jsL11
    proxy: {
      '/models': {
        target: 'http://192.168.1.65:8023',
        ws: false,
        changeOrigin: true,
        pathRewrite: {
          '^/models': ''
        }
      },
    }
  },
  publicPath: './',
  // disable source map in production
  productionSourceMap: false,
  lintOnSave: false,
  // babel-loader no-ignore node_modules/*
  transpileDependencies: []
}

// preview.pro.loacg.com only do not use in your production;
if (process.env.VUE_APP_PREVIEW === 'true') {
  console.log('VUE_APP_PREVIEW', true)
  // add `ThemeColorReplacer` plugin to webpack plugins
  // vueConfig.configureWebpack.plugins.push(createThemeColorReplacerPlugin())
  vueConfig.configureWebpack.devtool = 'source-map'
}

module.exports = vueConfig