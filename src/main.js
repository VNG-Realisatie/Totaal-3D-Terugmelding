import Vue from 'vue'
import './plugins/bootstrap-vue'
import App from './App.vue'
import router from './router'

import 'leaflet/dist/leaflet.css';
import { LMap, LTileLayer, LMarker } from 'vue2-leaflet';

import 'leaflet/dist/leaflet.css';
Vue.component('l-map', LMap);
Vue.component('l-tile-layer', LTileLayer);
Vue.component('l-marker', LMarker);

Vue.config.productionTip = false;

new Vue({
  data() {    
    return {
      authenticated: false,
      user:"",
      authToken:""
    }
  },
  router,
  render: function (h) { return h(App) }
}).$mount('#app')