<template>
  <a-row :gutter="24">
    <a-col :md="21" :lg="3">
      <folder-tree @ok="onOk" />
    </a-col>
    <a-col :md="21" :lg="21">
      <div style='padding-left: 12px'>已选模型<a>&nbsp;&nbsp;{{ clampingList.length }}&nbsp;</a>个</div>
      <a-card size="small" :bordered="false">
        <a-table ref="table" class="documentTable" :columns="columns" :row-key="(record) => record.id"
          :data-source="sourceData" :row-selection="rowSelection" bordered :pagination="pagination" @change="onChange"
          @onSelectAll="onSelectAll" size="middle">
          <template slot="serial" slot-scope="text, record, index">{{ index + 1 }}</template>
          <template slot="docSize" slot-scope="text">{{ text | byteToMB }}</template>
          <template slot="creationTime" slot-scope="text">{{ text | dateFomart('YYYY-MM-DD') }}</template>
          <template slot="lastModificationTime" slot-scope="text">{{ text | dateFomart('YYYY-MM-DD') }}</template>
          <template slot="isPublic" slot-scope="text">
            <a-tag v-if="text == true" color="red">公开</a-tag>
            <a-tag v-else color="green">私有</a-tag>
          </template>
        </a-table>
      </a-card>
    </a-col>
  </a-row>
</template>
<script>
  import {
    mapGetters
  } from 'vuex'
  import FolderTree from './FolderTree.vue'
  import {
    getDocList
  } from '@/api/document'
  export default {
    name: 'ModelList',
    components: {
      FolderTree,
    },
    props: {
      clampingList: {
        type: Array,
      }
    },
    computed: {
      ...mapGetters(['currProject']),
      rowSelection() {
        return {
          getCheckboxProps: record => ({
            props: {
              disabled: record.status != 2, // Column configuration not to be checked
            },
          }),
          selectedRowKeys: this.selectedRowIds,
          onChange: this.onSelectChange,
          onSelect: this.onSelect,
          onSelectAll: this.onSelectAll
        };
      },
    },
    data() {
      return {
        queryParams: {
          documentType: [1, 2],
          keyword: undefined,
          sorting: 'creationTime desc',
        },
        columns: [{
            title: '序号',
            align: 'center',
            width: '50px',
            scopedSlots: {
              customRender: 'serial'
            },
          },
          {
            title: '模型名称',
            dataIndex: 'documentName',
            ellipsis: true,
          },
          {
            title: '类型',
            dataIndex: 'modelFormat',
            width: '90px',
            align: 'center',
          },
          {
            title: '大小(MB)',
            dataIndex: 'size',
            align: 'center',
            width: '90px',
            scopedSlots: {
              customRender: 'docSize'
            },
          },
          {
            title: '轻量化状态',
            dataIndex: 'statusName',
            width: '90px',
            align: 'center',
          },
          {
            title: '上传者',
            dataIndex: 'creationName',
            align: 'center',
            width: '100px',
          },
          {
            title: '上传日期',
            dataIndex: 'creationTime',
            align: 'center',
            width: '110px',
            scopedSlots: {
              customRender: 'creationTime'
            },
          },
          {
            title: '更新日期',
            dataIndex: 'lastModificationTime',
            align: 'center',
            width: '110px',
            scopedSlots: {
              customRender: 'lastModificationTime'
            },
          },
          // {
          //   title: '是否公开',
          //   dataIndex: 'isPublic',
          //   width: '90px',
          //   align: 'center',
          //   scopedSlots: { customRender: 'isPublic' },
          // },         
        ],
        selectedRowIds: [], //选中的数据行Id
        selectedRows: [],
        selectedRowKeys: this.value?.map((x) => x.id) || [],
        sourceData: [],
        pagination: {
          pageSize: 10,
          current: 1,
        },
      }
    },
    methods: {
      onChange(pagination) {
        this.pagination = pagination
        this.pagination.skipCount = (pagination.current - 1) * pagination.pageSize
        this.fetch()
      },
      fetch() {
        const that = this
        this.queryParams.projectId = this.currProject.id
        this.queryParams.projectFolderId = this.folderId
        this.queryParams.MaxResultCount = this.pagination.pageSize
        this.queryParams.SkipCount = (this.pagination.current - 1) * this.pagination.pageSize
        getDocList(Object.assign({}, this.queryParams)).then((res) => {
          const pagination = {
            ...this.pagination
          }
          pagination.total = res.totalCount
          this.sourceData = res.items
          this.pagination = pagination
        })
      },
      onSelectChange(selectedRowKeys, selectedRows) {
        this.selectedRowIds = selectedRowKeys
        this.selectedRows = selectedRows
        // this.$emit('doSelect', this.selectedRows)
      },
      onSelect(selectedRowKeys, selectedRows) {
        console.log(selectedRowKeys, selectedRows)
        // if (selectedRows) {
        //   if (selectedRowKeys.status !== 2) this.$message.info('轻量化未成功!无法加载!')
        // } else {
        //   const _selectedRows = this.selectedRows.filter(e => e.id != selectedRowKeys.id)
        //   this.selectedRows = _selectedRows
        // }
        this.$emit('doSelect', selectedRowKeys, selectedRows)
      },
      onSelectAll(selected, selectedRows, changeRows) {
        console.log(selected, selectedRows, changeRows)
        changeRows.forEach(item => {
          this.$emit('doSelect', item, selected)
        })
      },
      onOk(pid) {
        if (pid.length > 0) {
          this.folderId = pid
          this.pagination = {
            pageSize: 10,
            current: 1,
          }
          this.fetch()
        } else {}
      },
    }
  }
</script>