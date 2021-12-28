import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'
import BimServerTest from '../views/BimServerTest.vue'
import UserFeedBack from '../views/UserFeedBack.vue'
import UserFeedBackList from '../views/UserFeedBackList.vue'

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
  },
  {
    path: '/userfeedback',
    name: 'UserFeedBackList',
    component: UserFeedBackList
  },
  {
    path: '/userfeedback/:id',
    name: 'UserFeedBack',
    component: UserFeedBack
  }
]

const router = new VueRouter({
  routes
})

export default router
