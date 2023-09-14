<template>
  <a-row :gutter="24">
    <a-col :md="24" :lg="4">
      <div :style="{ height: treeHeight }">
        <vue-scroll>
          <organization-unit-trees v-model="organizationUnitId" />
        </vue-scroll>
      </div>
    </a-col>
    <a-col :md="24" :lg="20">
      <a-card size="small" :bordered="false">
        <a-space slot="title">
          <a-input placeholder="手机号码" v-model="queryParams.filter" style="width: 300px" />
          <a-button type="primary" @click="fetch({ current: 1 })">查询</a-button>
        </a-space>
        <div :style="{ height: tableHeight }">
          <vue-scroll>
            <a-table
              :columns="columns"
              :row-key="(record) => record.id"
              :data-source="sourceData"
              bordered
              :pagination="pagination"
              size="middle"
              @change="handleTableChange"
              :row-selection="
                isModalInner || showOperation
                  ? {
                      type: type,
                      selectedRowKeys: selectedRowKeys,
                      onChange: onSelectChange,
                      onSelect: onSelect,
                      onSelectAll: onSelectAll,
                    }
                  : null
              "
            >
              <template slot="serial" slot-scope="text, record, index">{{ index + 1 }}</template>
              <template slot="render_avatar" slot-scope="text">
                <a-avatar
                  size="small"
                  :src="text == null ? avatarImg : getFileByBlobName(text)"
                  class="antd-pro-global-header-index-avatar"
                />
              </template>
              <template slot="orgIds" slot-scope="text">{{ text.join('/') }}</template>
              <template slot="position" slot-scope="text">
                <a-tag>项目一</a-tag>
                <a-tag>项目二</a-tag>
                <a-tag>项目三</a-tag>
              </template>
              <template slot="creationTime" slot-scope="text">
                {{text|dateFomart('YYYY-MM-DD')}}
              </template>
              <template slot="validityDate" slot-scope="text">
                {{text|dateFomart('YYYY-MM-DD')}}
              </template>
              <template slot="action" slot-scope="text, record">
                <a>关联项目</a>
              </template>
            </a-table>
          </vue-scroll>
        </div>
        <leader-modal
          ref="modal"
          :organizationUnitId="organizationUnitId"
          :isOutsideUser="isOutsideUser"
          @ok="onOk"
        ></leader-modal>
      </a-card>
    </a-col>
  </a-row>
</template>

<script>
import { getFileByBlobName } from '@/api/file'
import avatarImg from '../../assets/avatar.png'
import { mapGetters } from 'vuex'
import OrganizationUnitTrees from './modules/OrganizationUnitTrees'
import { getPositionList } from '@/api/dictionary'
import { getUserList, delUser } from '@/api/system'
import { hasJoin } from '@/api/project'
import LeaderModal from './modules/LeaderModal'
export default {
  name: 'Leader',
  computed: {
    ...mapGetters(['organizationUnits', 'nickname', 'roleName']),
  },
  components: {
    OrganizationUnitTrees,
    LeaderModal
  },
  props: {
    isModal: {
      type: Boolean,
      default: false,
    },
    type: {
      type: String,
      default: 'checkbox', //checkbox radio
    },
    value: {
      type: Array,
      default: undefined,
    },
    isOutsideUser: {
      type: Boolean,
      default: false,
    },
  },
  watch: {
    organizationUnitId(val) {
      if (this.isOutsideUser) {
        const outUser = this.organizationUnits.filter((t) => {
          return t.code == '00001'
        })
        if (val == outUser[0].id) {
          this.isModalInner = false
          this.showOperation = true
        } else {
          this.isModalInner = true
          this.showOperation = false
        }
      } else {
        //admin登录后不能添加外部人员
        const outUser = this.organizationUnits.filter((t) => {
          return t.code == '00001'
        })
        if (val == outUser[0].id && this.roleName == 'admin') {
          this.isModalInner = true
        } else {
          this.isModalInner = false
        }
      }
      this.fetch({ current: 1 })
    },
    value(val) {
      this.selectedRowKeys = val?.map((x) => x.userId || x.id) || []
    },
  },
  data() {
    return {
      getFileByBlobName: getFileByBlobName,
      avatarImg: avatarImg,
      visible: false,
      positions: [], //职位
      organizationUnitId: undefined,
      queryParams: { filter: undefined, sorting: 'creationTime desc' },
      form: null,
      // 表头
      columns: [
        {
          title: '序号',
          width: '70px',
          align: 'center',
          scopedSlots: { customRender: 'serial' },
        },
        {
          title: '手机号码',
          dataIndex: 'phoneNumber',
          align: 'center',
          width: '150px',
        },
        {
          title: '姓名',
          dataIndex: 'name',
          align: 'center',
          width: '120px',
        },
        {
          title: '组织机构',
          dataIndex: 'extraProperties.OrgIds',
          scopedSlots: { customRender: 'orgIds' },
          align: 'center',
          ellipsis: true,
          width: '120px',
        },
        {
          title: '参与项目',
          dataIndex: 'extraProperties.Position',
          scopedSlots: { customRender: 'position' },
          align: 'center',
          ellipsis: true,
        },
        // {
        //   title: '职位',
        //   dataIndex: 'extraProperties.Position',
        //   scopedSlots: { customRender: 'position' },
        //   align: 'center',
        //   width: '120px',
        // },
        // {
        //   title: '注册日期',
        //   width: '120px',
        //   dataIndex: 'creationTime',
        //   scopedSlots: { customRender: 'creationTime' },
        //   align: 'center',
        // },
        // {
        //   title: '有效期',
        //   dataIndex: 'extraProperties.ValidityDate',
        //   scopedSlots: { customRender: 'validityDate' },
        //   align: 'center',
        //   width: '120px',
        // },
        // {
        //   title: '备注',
        //   dataIndex: 'extraProperties.Describe',
        //   align: 'center',
        // },
        {
          title: '操作',
          width: '180px',
          dataIndex: 'action',
          scopedSlots: { customRender: 'action' },
          align: 'center',
        },
      ],
      selectedRowKeys: this.value?.map((x) => x.id) || [],
      sourceData: [],
      pagination: {
        current: 1,
        pageSize: 15,
      },
      isModalInner: this.isModal,
      showOperation: false,
      treeHeight: 'calc(100vh - 140px)',
      tableHeight: 'calc(100vh - 210px)',
    }
  },
  async created() {
    if (this.isModal) {
      this.columns = [
        {
          title: '手机号码',
          dataIndex: 'phoneNumber',
          align: 'center',
          width: '150px',
        },
        {
          title: '姓名',
          dataIndex: 'name',
          align: 'center',
          width: '120px',
        },
        // {
        //   title: '组织机构',
        //   dataIndex: 'extraProperties.OrgIds',
        //   scopedSlots: { customRender: 'orgIds' },
        //   align: 'center',
        //   ellipsis: true,
        // },
        {
          title: '职位',
          dataIndex: 'extraProperties.Position',
          scopedSlots: { customRender: 'position' },
          width: '120px',
          align: 'center',
        },
        {
          title: '备注',
          dataIndex: 'extraProperties.Describe',
          align: 'center',
        },
      ]
      this.pagination.pageSize = 10
      ;(this.treeHeight = '565px'), (this.tableHeight = '500px')
      this.selectedRowKeys = this.value?.map((x) => x.id) || []
    }
    this.positions = await getPositionList()
    this.fetch({ current: 1 })
  },
  methods: {
    fetch(params = {}) {
      const that = this
      this.pagination = Object.assign(this.pagination, params)
      this.queryParams.MaxResultCount = this.pagination.pageSize
      this.queryParams.SkipCount = (this.pagination.current - 1) * this.pagination.pageSize
      getUserList(this.organizationUnitId, Object.assign({}, this.queryParams)).then((res) => {
        const pagination = { ...this.pagination }
        pagination.total = res.totalCount
        this.sourceData = res.items
        this.pagination = pagination
      })
    },
    handleTableChange(pagination) {
      const that = this
      this.pagination = pagination
      this.pagination.skipCount = (pagination.current - 1) * pagination.pageSize
      this.fetch({
        ...this.queryParams,
      })
    },
    onOk() {
      // 新增/修改 成功时，重载列表
      this.fetch({ current: 1 })
    },
    reset(record) {
      const that = this
      that.$confirm({
        title: `确定要重置用户 “${record.name}” 的密码为'123456'吗？`,
        onOk() {
          that.$refs.modal.reset(record)
        },
      })
    },
    deleteUser(record) {
      const that = this
      hasJoin(record.id).then((res) => {
        if (res) {
          that.$confirm({
            title: `确定要删除用户 “${record.name}” 吗？`,
            onOk() {
              delUser(record.id).then(() => {
                that.$message.success(that.$t('list.deleteSuccess'))
                that.resetCurrenPage()
                that.onOk()
              })
            },
          })
        } else {
          that.$warning({
            content: `用户“${record.name}”已经参与项目，在项目中移除后才能删除`,
            okText: that.$t('list.cancel'),
          })
        }
      })
    },
    onSelect(record, selected) {
      if (this.type === 'checkbox') {
        const rows = [...this.value]
        if (!selected) {
          rows.splice(
            rows.findIndex((x) => x.id == record.id),
            1
          )
          this.$emit('input', rows)
        } else {
          if (!this.value?.some((x) => x.id === record.id)) {
            const { id, name, phoneNumber, userName } = record
            rows.push({
              id,
              name,
              phoneNumber,
              userName,
              position: record['extraProperties']['Position'],
              orgNames: record['extraProperties']['OrgIds'],
            })
            this.$emit('input', rows)
          }
        }
      } else {
        const { id, name, phoneNumber, userName } = record
        this.$emit('input', [
          {
            id,
            name,
            phoneNumber,
            userName,
            position: record['extraProperties']['Position'],
            orgNames: record['extraProperties']['OrgIds'],
          },
        ])
      }
    },
    onSelectAll(selected, selectedRows, changeRows) {
      const rows = this.value || []
      if (selected) {
        this.$emit('input', [
          ...rows,
          ...changeRows.map((x) => ({
            id: x.id,
            name: x.name,
            phoneNumber: x.phoneNumber,
            position: x['extraProperties']['Position'],
            orgNames: x['extraProperties']['OrgIds'],
          })),
        ])
      } else {
        changeRows.forEach((row) => {
          rows.splice(
            rows.findIndex((x) => x.id == row.id),
            1
          )
          this.$emit('input', rows)
        })
      }
    },
    onSelectChange(selectedRowKeys) {
      this.selectedRowKeys = selectedRowKeys
    },
    resetCurrenPage() {
      const totalPage = Math.ceil((this.pagination.total - 1) / this.pagination.pageSize) // 总页数
      this.pagination.current = this.pagination.current > totalPage ? totalPage : this.pagination.current
      this.pagination.current = this.pagination.current < 1 ? 1 : this.pagination.current
    },
  },
}
</script>

<style scoped>
</style>
