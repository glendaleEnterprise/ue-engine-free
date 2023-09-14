<template>
  <a-card size="small" :bordered="false">
    <a-space slot="title" v-action:Design.Business.Share.Search>
      <a-input placeholder="请输入名称" style="width: 300px" v-model="queryParams.keyword" />
      <a-button type="primary" @click="search">查询</a-button>
    </a-space>

    <a-table size="middle" bordered :columns="columns" :data-source="data" :pagination="pagination"
      @change="handleTableChange" :rowKey="(record) => record.id" :loading="loading"
      :locale="{ emptyText: $t('public.noData') }">
      <template slot="no" slot-scope="text, record, index"> {{ index + 1 }} </template>
      <span slot="render_shareLink" slot-scope="text">
        <a-tooltip placement="topLeft" :title="text">
          <a @click="skipList(text)">{{ text }}</a>
        </a-tooltip>
      </span>
      <span slot="render_auth" slot-scope="text">
        <span>{{ text }}</span>
      </span>
      <span slot="shareCount" slot-scope="text">
        <span>{{ text === 0 ? '无限' : text }}</span>
      </span>
      <span slot="render_sceneType" slot-scope="text">
        <span>{{ text | filterTypo }}</span>
      </span>
      <span slot="render_shareState" slot-scope="text, record">
        <span v-if="record.shareState === 1 && record.effectiveDay != 200">{{ record | filterExpirationTime }}</span>
        <span v-else>{{ text | filterStatu }}</span>
      </span>
      <template slot="action" slot-scope="text, record">
        <a-space>
          <a v-action:Design.Business.Share.Info @click="shareInfo(record)">
            <a-tooltip title="链接分享查看">
              <a-icon type="eye" />
            </a-tooltip>
          </a>
          <a v-action:Design.Business.Share.Delete @click="deleteSpace(record)">
            <a-tooltip title="删除">
              <a-icon type="delete" />
            </a-tooltip>
          </a>
        </a-space>
      </template>
    </a-table>
    <share-result :shareInfo="editShare" v-if="editShareVisible" :visible.sync="editShareVisible" @changeShare="edit" />
  </a-card>
</template>

<script>
const columns = [
  {
    title: '序号',
    width: '70px',
    align: 'center',
    scopedSlots: { customRender: 'no' },
  },
  {
    title: '名称',
    dataIndex: 'shareTile',
    key: 'shareTile',
    ellipsis: true,
  },
  {
    title: '分享类型',
    dataIndex: 'sceneType',
    align: 'center',
    width: '100px',
    scopedSlots: { customRender: 'render_sceneType' },
  },
  {
    title: '分享链接',
    dataIndex: 'shareLink',
    key: 'shareLink',
    ellipsis: true,
    scopedSlots: { customRender: 'render_shareLink' }, //用來過濾
  },
  {
    title: '授权码',
    dataIndex: 'auth',
    align: 'center',
    key: 'auth',
    width: '100px',
    scopedSlots: { customRender: 'render_auth' },
  },
  // {
  //   title: '是否校验',
  //   align: 'center',
  //   dataIndex: 'isVerify',
  //   scopedSlots: { customRender: 'render_isVerify' }
  // // },
  // {
  //   title: '分享次数',
  //   align: 'center',
  //   dataIndex: 'shareCount',
  //   key: 'shareCount',
  //   width: '100px',
  //   scopedSlots: { customRender: 'shareCount' },
  // },
  // {
  //   title: '查看次数',
  //   align: 'center',
  //   dataIndex: 'pvmCount',
  //   key: 'pvmCount',
  //   width: '100px',
  // },
  {
    title: '期限',
    align: 'center',
    dataIndex: 'shareState',
    width: '170px',
    scopedSlots: { customRender: 'render_shareState' },
  },
  {
    title: '创建时间',
    align: 'center',
    dataIndex: 'creationTime',
    key: 'creationTime',
    width: '170px',
  },
  {
    title: '操作',
    align: 'center',
    key: 'action',
    width: '100px',
    scopedSlots: { customRender: 'action' },
  },
]
// import logoUrl from '../../assets/logo-head.png'
import vueQr from 'vue-qr'
import { mapGetters } from 'vuex'
import ShareResult from './modules/SharedResult.vue'
import { getList, deleted } from '@/api/share'
export default {
  name: 'Share',
  computed: {
    ...mapGetters(['currProject']),
  },
  components: {
    ShareResult,
    vueQr,
  },
  data() {
    return {
      copySign: '\n' + process.env.VUE_APP_COPY_SIGN,
      custorColumns: [],
      columns,
      ShareId: undefined,
      loading: false,
      valueParams: undefined,
      editShareVisible: false,
      level: 1,
      queryParams: {
        parent: undefined,
        keyword: undefined,
        projectId: undefined,
        value: undefined,
      },
      data: [],
      pagination: {
        current: 1,
        pageSize: 10, //每页中显示10条数据
      },
      editShare: {},
    }
  },

  async created() {
    const paths = this.$route.path.split('/')
    this.valueParams = paths[paths.length - 1]
    if (this.valueParams != 'share') {
      this.queryParams.parent = this.valueParams
    }
    await this.fetch()
  },

  filters: {
    //设备状态过滤方法
    filterStatu(val) {
      switch (val) {
        case 0:
          return '关闭'
        case 1:
          return '无限'
        case 200:
          return '永久'
        case -1:
          return '超期'
        default:
          return '未知状态'
      }
    },
    filterTypo(val) {
      switch (val) {
        case 0:
          return '单模型'
        case 1:
          return '合模'
        default:
          return '未知状态'
      }
    },
    filterVerify(val) {
      if (val) return '校验'
      else return '不校验'
    },
    filterLink(url) {
      return 'http://' + window.location.host + '/#/' + url
    },
    filterExpirationTime(record) {
      var date = new Date(Date.parse(record.creationTime.replace(/-/g, "/")));
      var newDate = new Date(date.setDate(date.getDate() + record.effectiveDay)) //获取当前日期后加30天    2021/4/8
      var y = newDate.getFullYear();
      var m = newDate.getMonth() + 1;
      m = m < 10 ? ('0' + m) : m;
      var d = newDate.getDate();
      d = d < 10 ? ('0' + d) : d;
      var h = newDate.getHours();
      h = h < 10 ? ('0' + h) : h;
      var minute = newDate.getMinutes();
      minute = minute < 10 ? ('0' + minute) : minute;
      var second = newDate.getSeconds();
      second = second < 10 ? ('0' + second) : second;
      return y + '-' + m + '-' + d + ' ' + h + ':' + minute + ':' + second;
    }
  },
  methods: {
    fetch() {
      this.loading = true
      this.queryParams.projectId = this.currProject.id
      getList({
        MaxResultCount: this.pagination.pageSize,
        SkipCount: (this.pagination.current - 1) * this.pagination.pageSize,
        ...this.queryParams,
      }).then((res) => {
        res.items.forEach((item) => {
          item.shareLink = this.alterLink(item.shareLink)
          if (!item.isVerify) {
            item.auth = '----'
          }
        })
        const pagination = { ...this.pagination }
        pagination.total = res.totalCount
        this.loading = false
        this.data = res.items
        this.pagination = pagination
      })
    },
    alterLink(url) {
      return 'http://' + window.location.host + '/#/' + url
    },
    handleTableChange(pagination) {
      const pager = { ...this.pagination }
      pager.current = pagination.current
      this.pagination = pager
      this.fetch({
        MaxResultCount: pagination.pageSize,
        SkipCount: (this.pagination.current - 1) * this.pagination.pageSize,
        ...this.queryParams,
      })
    },
    paneChange(key) {
      this.level = parseInt(key)
      this.getColumns()
      this.getShareRecord()
    },
    async search() {
      this.pagination.current = 1
      this.fetch()
    },

    shareInfo(data) {
      this.editShare = data
      this.editShareVisible = true
    },
    skipList(url) {
      window.open(url, '_blank')
    },
    edit(visible) {
      this.editShareVisible = visible
    },
    onCopy(e) {
      this.$message.success('内容已复制到剪切板！')
    },
    // 复制失败时的回调函数
    onError(e) {
      this.$message.error('抱歉，复制失败！')
    },
    deleteSpace(data) {
      const that = this
      that.$confirm({
        okText: '确认',
        cancelText: that.$t('list.cancel'),
        title: `确定要删除 “${data.shareTile}” 吗？`,
        onOk() {
          deleted(data.id).then(() => {
            that.$message.success(that.$t('list.deleteSuccess'))
            that.resetCurrenPage()
            that.fetch()
          })
        },
      })
    },
    resetCurrenPage() {
      const totalPage = Math.ceil((this.pagination.total - 1) / this.pagination.pageSize) // 总页数
      this.pagination.current = this.pagination.current > totalPage ? totalPage : this.pagination.current
      this.pagination.current = this.pagination.current < 1 ? 1 : this.pagination.current
    }
  }
}
</script>

<style lang="less" scoped>
.qrcode {
  overflow: hidden;
}

.qrcode .q_left {
  float: left;
}

.qrcode .q_right {
  float: left;
  margin-left: 20px;
  line-height: 100px;
  text-align: center;
  font-size: 24px;
}

.qrcode .q_copybtn {
  position: absolute;
  top: 40px;
  right: 20px;
  cursor: pointer;
}

.qrcode .q_icon {
  position: absolute;
  width: 25px;
  height: 25px;
  top: 83px;
  left: 65px;
  background-color: #fff;
  border: solid 1px rgb(47, 151, 251);
}

.qrcode .q_icon img {
  width: 100%;
  height: 100%;
  padding: 2px;
}
</style>
