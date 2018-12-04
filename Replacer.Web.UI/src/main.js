import './Content/css/App.css'
import Vue from 'vue'
import VueRouter from 'vue-router'
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import VueResource from 'vue-resource'
import Replacer from './Components/Replacer.vue'
import Admin from './Components/Admin.vue'
import ImportDb from './Components/ImportDb.vue'

Vue.use(VueRouter)
Vue.use(BootstrapVue);
Vue.use(VueResource);

var router = new VueRouter({
  routes: [
    { path: '/', component: Replacer },
    { path: '/admin', name:"admin", component: Admin },
    { path: '/admin', name:"admin", component: Admin },
    { path: '/import', name: "import", component: ImportDb }
  ]
})

new Vue({
  el: '#app',
  router: router
})