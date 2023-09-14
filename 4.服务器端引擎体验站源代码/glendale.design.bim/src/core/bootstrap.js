import store from '@/store'
import storage from 'store'
import { ACCESS_TOKEN, CURR_PROJECTID } from '@/store/mutation-types'

export default function Initializer () {
  store.commit('SET_TOKEN', storage.get(ACCESS_TOKEN))
  store.dispatch('SetProject', storage.get(CURR_PROJECTID))
}
