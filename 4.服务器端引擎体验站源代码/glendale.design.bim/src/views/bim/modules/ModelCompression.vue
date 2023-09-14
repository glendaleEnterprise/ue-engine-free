<template>
  <div>
    <a-form layout="inline" :form="form" @submit="handleSubmit">
      <a-form-item label="压平高度">
        <a-input
          type="number"
          v-decorator="[
            'height',
            { rules: [{ required: true, message: '请输入压缩高度' }], initialValue: -10 },
          ]"
          style="width:80px;"
        />
      </a-form-item>
      <a-form-item>
        <a-button type="primary" html-type="submit">
          修改
        </a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script>
export default {
  name: 'ModelCompression',
  props: {
    mcList: {
      type: Array,
      default: () => {
        return []
      },
    },
    modelArrCopy: {
      type: Array,
      default: () => {
        return []
      },
    },
    flattenArray: {
      type: Array,
      default: () => {
        return []
      },
    },
  },
  data () {
    return {
      form: this.$form.createForm(this, { name: 'horizontal_login' }),
    }
  },
  methods: {
    getUuid () {
      return 'xxxx-4xxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 9 | 0
        const v = c === 'x' ? r : (r & 0x3 | 0x8)
        return v.toString(9)
      })
    },
    handleSubmit(e) {
      e.preventDefault()
      this.form.validateFields((err, values) => {
        if (!err) {
          this.flattenArray.forEach(item=>{
            const model = item.split('@')
            const opt = {
              id: item,
              positions: this.mcList,
              height: parseInt(values.height),
            }
            this.$sapi.Model.addFlatten(model[1], opt)
          })
        }
      })
    }
  }
}
</script>
<style scoped lang='less'>
</style>
