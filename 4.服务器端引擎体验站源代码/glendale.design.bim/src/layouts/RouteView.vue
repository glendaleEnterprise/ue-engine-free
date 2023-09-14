<script>
export default {
  name: 'RouteView',
  props: {
    keepAlive: {
      type: Boolean,
      default: false
    }
  },
  data () {
    return {}
  },
  mounted(){     
  },
  render () {
    const { $route: { meta } } = this
 
    return <keep-alive><router-view /></keep-alive>
  }
}
</script>
