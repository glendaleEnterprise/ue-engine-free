<template>
  <div>
    <a-card size="small" :bordered="false" style="margin-bottom: 12px">
      <div style="margin-bottom: 10px">
        按年份：
        <a-checkable-tag name="year" v-for="tag in tagYear" :key="tag.value" :checked="selectedTagYear == tag.value"
          @change="() => handleChange(tag, 'year')">
          {{ tag.name !== '全部' ? tag.name + '年' : tag.name }}
        </a-checkable-tag>
      </div>
    </a-card>
    <a-card size="small" :bordered="false">
      <a-space slot="extra">
        <a-input-search v-action:Design.Base.Projects.Search placeholder="关键字" v-model="queryParams.keyword"
          @search="search" />
        <a-button v-action:Design.Base.Projects.Add type="primary" @click="$refs.refProjectAdd.add()"
          v-if='currentUser.userName != "guest"'>创建项目</a-button>
      </a-space>
      <a-row :gutter="24">
        <a-col :xl="24" :lg="24" :md="24" :sm="24" :xs="24">
          <div v-if="data.length > 0" class="project-list">
            <div class="parent">
              <div class="child" v-for="item in data" :key="item.id">
                <a-card hoverable style="cursor: default;">
                  <img slot="cover" :src="item.coverImage != '' ? getFileByBlobName(item.coverImage) : defProjectImg"
                    @click="gotoBusiness(item.id, item)" style="cursor: pointer;" />
                  <a-tag v-if="item.isPublic" class="meta-open" color="#87d068">公开</a-tag>
                  <a-tag v-else class="meta-open">私有</a-tag>
                  <a-card-meta style="cursor: default;">
                    <div slot='title' :title='item.projectName'
                      style='cursor:pointer;overflow: hidden;color: rgba(0, 0, 0, 0.85);font-weight: 500;font-size: 16px;white-space: nowrap;text-overflow: ellipsis;'
                      @click="gotoBusiness(item.id, item)">{{ item.projectName }}</div>
                    <div style='flex-wrap: wrap;display: flex;height: 24px;' slot="description">
                      <span style='min-width: 85px;display: block;'><a-icon type="user" title="创建者" />{{
                        item.manageInfo ? item.manageInfo.name : '未设置' }}</span>
                      <span style="display: block;">
                        <a-icon type="clock-circle" title="立项时间" />{{
                          item.beginDate | dateFomart('YYYY年MM月DD日')
                        }}</span>
                    </div>
                    <a-space slot="avatar">
                      <a-button v-action:Design.Base.Projects.Update type="link" style="padding: 0"
                        @click.stop="$refs.refProject.edit(item)" title="编辑"
                        v-if='currentUser.userName != "guest" && currentUser.userName == item.manageInfo.userName'>
                        <a-icon type="setting" :style="{ fontSize: '18px', color: '#00000073' }" />
                      </a-button>
                      <a-button v-action:Design.Base.Projects.Delete type="link" style="padding: 0"
                        @click.stop="delProject(item.id)" title="删除"
                        v-if='currentUser.userName != "guest" && currentUser.userName == item.manageInfo.userName'>
                        <a-icon type="delete" :style="{ fontSize: '18px', color: '#00000073' }" />
                      </a-button>
                    </a-space>
                  </a-card-meta>
                </a-card>
              </div>
            </div>
            <div style="text-align:right">
              <a-pagination v-model="pagination.current" :pageSize="pagination.pageSize" :total="pagination.total"
                @change="handleTableChange" size="small" />
            </div>
          </div>
          <a-empty v-else description="暂无数据" />
        </a-col>
      </a-row>
      <project-modal ref="refProject" @ok='fetch' />
      <project-add-modal ref="refProjectAdd" @ok='fetch' />
    </a-card>
  </div>
</template>
 
<script>
import ProjectModal from './modules/ProjectModal.vue'
import ProjectAddModal from './modules/ProjectAddModal.vue'
import { getProjectList, delProject } from '../../api/project'
import { getFileByBlobName } from '@/api/file'
import { getYearList } from '@/api/dictionary'
import { mapState, mapGetters } from 'vuex'
import store from '@/store'
import router from '@/router'
export default {
  name: 'ProjectIndex',
  components: {
    ProjectModal,
    ProjectAddModal
  },
  computed: {
    ...mapState({
      // 动态主路由
      mainMenu: (state) => state.permission.addRouters,
    }),
    ...mapGetters({
      currentUser: 'userInfo',
    }),
    ...mapGetters(['token']),
  },
  data() {
    return {
      defProjectImg: process.env.VUE_APP_PROJECT_IMG,
      tagYear: [
        {
          value: 0,
          name: '全部',
        },
      ],
      selectedTagYear: 0,
      selectedTagChildType: '',
      loading: false,
      queryParams: {
        keyword: undefined,
      },
      data: [],
      pagination: {
        current: 1,
        total: 0,
        skipCount: 0,
        pageSize: 10, //每页中显示10条数据
        showTotal: (total) => `共 ${total} 条`, //分页中显示总的数据
      },
    }
  },
  created() {
    this.fetch()
    getYearList().then((res) => {
      this.tagYear = [...this.tagYear, ...res]
    })
  },
  methods: {
    getFileByBlobName,
    handleChange(tag, obj) {
      if (obj === 'year') this.selectedTagYear = tag.value
      this.fetch()
    },
    fetch() {
      const that = this
      this.loading = true
      const params = {
        keyword: this.queryParams.keyword,
        year: this.selectedTagYear,
        skipCount: this.pagination.skipCount,
        maxResultCount: this.pagination.pageSize,
      }

      getProjectList(params).then((res) => {
        that.data = res.items
        that.pagination.total = res.totalCount
        that.loading = false
      })
    },
    handleTableChange(page) {
      this.pagination.skipCount = (page - 1) * this.pagination.pageSize
      this.fetch()
    },
    async search() {
      this.pagination.current = 0
      this.pagination.skipCount = 0
      this.fetch()
    },
    delProject(id) {
      const that = this
      this.$confirm({
        title: '系统提示',
        content: '确定要删除该项目吗？',
        okType: 'danger',
        onOk() {
          delProject(id).then(() => {
            that.$message.info('删除成功')
            that.search()
          })
        },
      });
    },
    /**选择项目进入业务系统 */
    gotoBusiness(id, item) {
      const that = this
      that.$store.dispatch('SetProject', id).then(() => {
        if (that.currentUser.extraProperties.id === item.manageId) {
          that.$store.dispatch('GetInfoCreat').then(res => {
            that.GenerateRoutes(res, id, that.token)
          })
        } else {
          that.$store.dispatch('GetInfo').then(res => {
            that.GenerateRoutes(res, id, that.token)
          })
        }
      })
    },
    GenerateRoutes(res, id, token) {
      var that = this
      const pp = { token, ...res, id }
      store.dispatch('GenerateRoutes', pp).then(() => {
        store.getters.addRouters.forEach(r => {
          router.addRoute(r)
        })
        const children = store.getters.addRouters[0].children.find(item => item.name === 'business').children
        if (children.length) {
          let list = children.filter(item => item.path == "/business/model")
          const path = list.length > 0 ? list[0].path : children[0].path
          var main = that.mainMenu.find((item) => item.path === '/')
          const business = main.children.find((item) => item.path === '/business')
          business.path = children[0].path
          business.redirect = children[0].path

          this.$store.dispatch('SetProject', id)
          this.$router.replace({ path: path })
        } else {
          this.$message.error('没有项目权限!')
        }
      })
    }
  },
}
</script>
<style lang="less" scoped>
.project-list {
  /deep/.ant-card-meta-avatar {
    float: right;
    padding-right: 0;
    padding-top: 16px;
  }

  /deep/.ant-card-cover>img {
    height: 200px;
  }

  /deep/.ant-card-body {
    padding: 12px;
  }

  .card-description {
    color: rgba(0, 0, 0, 0.45);
    height: 44px;
    line-height: 22px;
    overflow: hidden;
  }

  .parent {
    overflow: hidden;
    margin-right: -20px;
  }

  .child {
    float: left;
    margin-top: 10px;
    margin-bottom: 10px;
    width: calc(20% - 20px);
    margin-right: 20px;
  }

  .meta-open {
    position: absolute;
    top: 10px;
    left: 10px;
  }

  /deep/.anticon {
    margin-right: 3px;
  }
}
</style>
