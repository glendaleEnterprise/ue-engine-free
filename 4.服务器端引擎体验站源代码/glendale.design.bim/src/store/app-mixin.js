import { mapState } from 'vuex'

const baseMixin = {
  computed: {
    ...mapState({
      currProject: state => state.app.currProject,
    }),
  },
}

export {
  baseMixin
}
