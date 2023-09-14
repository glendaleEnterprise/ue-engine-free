<template>
  <a-modal title="用户选择" :width="1200" :centered="true" :visible="visible" @ok="handleOk" @cancel="hide" :zIndex='3000'>
    <DragControllerDiv _leftWidth='20'>
      <template v-slot:left>
        <organization-unit-trees v-model="organizationUnitId" :organizationUnits='organizationUnits' />
      </template>
      <template v-slot:right>
        <a-card size="small" :bordered="false">

            <a-input placeholder="手机号码" v-model="queryParams.filter" style="width: 150px" />
            <a-input placeholder="用户名" v-model="queryParams.name" style="width: 150px" />
            <a-button type="primary" @click="fetch({ current: 1 })">查询</a-button>
          <div style='padding: 10px 0 10px 0;'>
            <span slot="extra">{{ `已选择 ${selectedRowKeys.length} 人：` }}</span><a-tag v-for='item in selectedRows'>{{ item.name }}</a-tag>
          </div>

          <div style="height: 570px">
            <vue-scroll>
              <a-table
                :columns="columns"
                :row-key="(record) => record.id"
                :data-source="sourceData"
                bordered
                :pagination="pagination"
                size="middle"
                @change="handleTableChange"
                :row-selection="{
                  type: type,
                  selectedRowKeys: selectedRowKeys,
                  onChange: onSelectChange,
                  onSelect: onSelect,
                  onSelectAll: onSelectAll,
                }"
              >
                <template slot="serial" slot-scope="text, record, index">{{ index + 1 }}</template>
                <template slot="orgIds" slot-scope="text">{{ text.join('/') }}</template>
                <template slot="position" slot-scope="text" v-if="positions">{{
                    text ? positions.find((x) => x.value == text).name : ''
                  }}</template>
                <template slot="creationTime" slot-scope="text">
                  {{ text | dateFomart('YYYY-MM-DD') }}
                </template>
                <template slot="validityDate" slot-scope="text">
                  {{ text | dateFomart('YYYY-MM-DD') }}
                </template>
              </a-table>
            </vue-scroll>
          </div>
        </a-card>
      </template>

    </DragControllerDiv>

  </a-modal>
</template>

<script>
import OrganizationUnitTrees from './OrganizationUnitTrees'
import { getOrganizationUnitTrees, getSystemAppUserList } from '@/api/system'
import { getPositionList } from '@/api/dictionary'
import DragControllerDiv from '@/layouts/DragControllerDiv'
export default {
  name: 'SelectUserModal',
  computed: {
  },
  components: { OrganizationUnitTrees ,DragControllerDiv},
  props: {
    value: {
      type: Array,
      default: undefined,
    },
    type: {
      type: String,
      default: 'checkbox', //checkbox radio
    },
  },
  watch: {
    organizationUnitId(val) {
      this.fetch({ current: 1 })
    },
    value(val) {
      this.selectedRows = val
      this.selectedRowKeys=val.map((x) => x.id) || []
    },
  },
  data() {
    return {
      organizationUnits:[],
      visible: false,
      selectedRows: this.value,
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
        },
        {
          title: '职位',
          dataIndex: 'extraProperties.Position',
          scopedSlots: { customRender: 'position' },
          align: 'center',
          width: '120px',
        },
      ],
      sourceData: [],
      pagination: {
        current: 1,
        pageSize: 15,
      },
      organizationUnitId: undefined,
      queryParams: { filter: undefined,name: undefined, sorting: 'creationTime desc' },
      selectedRowKeys: this.value?.map((x) => x.id) || [],
      positions: [], //职位
      isModal: true,
    }
  },
  created() {
    const that = this
    getOrganizationUnitTrees().then(res => {
      this.organizationUnits=res
    })
    getPositionList().then((res) => {
      that.positions = res
      this.fetch({ current: 1 })
    })
  },
  methods: {
    fetch(params = {}) {
      const that = this
      this.pagination = Object.assign(this.pagination, params)
      this.queryParams.MaxResultCount = this.pagination.pageSize
      this.queryParams.SkipCount = (this.pagination.current - 1) * this.pagination.pageSize
      getSystemAppUserList(this.organizationUnitId, Object.assign({}, this.queryParams)).then((res) => {
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
    show() {
      this.visible = true
    },
    hide() {
      this.visible = false
    },
    handleOk() {
      this.$emit('input', this.selectedRows)
      this.$emit('ok', this.selectedRows)
    },
  },
}
</script>
<style scoped>
</style>
