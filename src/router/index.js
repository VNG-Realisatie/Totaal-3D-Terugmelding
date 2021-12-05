import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'
import BimServerTest from '../views/BimServerTest.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/bim',
    name: 'BimServerTest',
    component: BimServerTest
  }
]

const router = new VueRouter({
  routes
})

export default router
