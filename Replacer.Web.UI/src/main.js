import Vue from 'vue'
import VueRouter from 'vue-router'
import Replacer from './Replacer.vue'
import Admin from './Admin.vue'
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import VueResource from 'vue-resource'

Vue.use(VueRouter)
Vue.use(BootstrapVue);
Vue.use(VueResource);

var router = new VueRouter({
  routes: [
    { path: '/', component: Replacer },
    { path: '/admin', component: Admin }
  ]
})

new Vue({
  el: '#app',
  router: router
})