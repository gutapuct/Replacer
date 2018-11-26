import Vue from 'vue'
import VueRouter from 'vue-router'
import { MdButton } from 'vue-material/dist/components'
import 'vue-material/dist/vue-material.min.css'
import 'vue-material/dist/theme/default.css'
import Replacer from './Replacer.vue'
import App from './App.vue'
import Admin from './Admin.vue'
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

Vue.use(VueRouter)
Vue.use(MdButton)
Vue.use(BootstrapVue);

var router = new VueRouter({
  routes: [
    { path: '/app', component: App },
    { path: '/replacer', component: Replacer },
    { path: '/admin', component: Admin }
  ]
})

new Vue({
  el: '#app',
  router: router
})