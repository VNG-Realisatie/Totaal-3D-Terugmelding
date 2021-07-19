<template>

<b-container class="bv-example-row" >

<div class="header">Uitbouw plaatsen</div>

  <b-row>

    <b-col class="entrycontainer">

    <div class="formheader">Adresgegevens</div>
    <div class="formlines">Voer het adres in waar u de uitbouw wilt gaan plaatsen</div>

      <div class="entryform">
        <div class="formheader">Postcode</div>
        <b-form-input class="forminput noselect" v-model="postcode" @keypress="checkPostcode($event)" :state="postcodeState"></b-form-input>
      </div>

      <div class="entryform">
        <div class="formheader">Huisnummer + toevoeging</div>
        <b-form-input class="forminput" v-model="huisnummer" v-bind:disabled="invalid_postcode" :state="addressState"></b-form-input>
      </div>

      <div class="status">

        <div v-if="notfound" class="formlines notfound">
          <div class="bold">Helaas. Wij kunnen geen adres vinden bij deze combinatie van postcode en huisnummer.</div>
          <div>Probeer het opnieuw. Of neem contact op met de gemeente op telefoonnummer<a href="tel:14020">&#160;14020</a></div>
        </div>

        <div v-if="found_address" class="foundaddress formlines">
          <div class="foundaddress_header">Dit is het gekozen adres:</div>
          <div>{{street}} {{huisnummer}}</div>
          <div>{{postcode}} {{city}}</div>          
          <div>Bouwjaar: {{bouwjaar}}</div>
          <div>Perceeloppervlakte: {{kadastraleGrootteWaarde}}m&#178;</div>
        </div>

      <p class="gaverder">
          <b-button v-if="found_address" v-bind:href="bagurl" target="_blank" variant="danger">Ga verder</b-button>
      </p>
      
      </div>

    </b-col>

    <b-col>
         <!-- <model-obj src="/3dmodels/bbox.obj"></model-obj> -->
        
          <l-map v-if="found_address"
                  style="height: 100%; width: 100%"
                  :zoom="zoom"
                  :center="center"      
                  @update:zoom="zoomUpdated"
                  @update:center="centerUpdated"
                  @update:bounds="boundsUpdated">
                <l-tile-layer :url="url" :options="{ maxZoom: 19 }"></l-tile-layer>

                <l-polygon
                  :lat-lngs="polygon_wgs84"
                  color="#28A745"
                />

          </l-map>
        
          <img v-else  class="unity" v-bind:src="viewer_image" alt="">
        
    </b-col>
    
  </b-row>



</b-container>

</template>

<script>
// @ is an alias to /src
import { ModelObj } from 'vue-3d-model'

import L from 'leaflet';
import { LMap, 
        LTileLayer, 
        LMarker,        
        LPolygon,
        LPolyline
        } from 'vue2-leaflet';

import rdToWgs84 from "rd-to-wgs84";

export default {
  name: 'Home',
  data: function () {
    return {
      viewer_base_url: "http://localhost:8081",
      postcode: "",
      huisnummer: "",
      bouwjaar:0,
      invalid_postcode: true,
      viewer_default_image: "images/3dnetherlands_viewer.PNG",
      street:"",
      city: "",
      notfound:false,
      bagcoordinates: [],
      map_img_resolution:600,
      map_img_size: 40,
      postcode_regex: /^[1-9][0-9][0-9][0-9]?(?!sa|sd|ss)[a-z][a-z]$/i,
      verblijfsobject_id: "",
      bagids:[],
      model_pos: { x: 157769, y: 467204, z: 0 },
      model_scale: { x: 0.1, y: 0.1, z: 0.1 },
      model_rotation: { x: 0.5, y: 0, z: 0 },
      mode_scale : { x: 10, y: 10, z: 10 },
      url: 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
      zoom: 19,
      bounds: null,
      polygon_rd: [],
      kadastraleGrootteWaarde:0

    }
  },
  computed:{
    postcodeState:function(){
        if(this.postcode.length != 6) return null;        
        return true;
    },
    addressState: function (){
      if(!this.found_address) return null;
      return true;
    },   
    found_address: function(){
        return this.huisnummer != "" && this.street != "";
    },
    bagurl: function(){
      return `${this.viewer_base_url}?position=${this.bagcoordinates[0]}_${this.bagcoordinates[1]}`;
    },
    viewer_image: {
      get(){        
//         if( this.bagcoordinates.length == 3){
//           let x = this.bagcoordinates[0];
//           let y = this.bagcoordinates[1];           
//           let half = this.map_img_size/2;
//           let bbox = `${x-half},${y-half},${x+this.map_img_size},${y+this.map_img_size}`;
// //        let mapurl = `https://geodata.nationaalgeoregister.nl/luchtfoto/rgb/wms?styles=&layers=Actueel_ortho25&service=WMS&request=GetMap&format=image%2Fpng&version=1.1.0&bbox=${bbox}&width=${this.map_img_resolution}&height=${this.map_img_resolution}&srs=EPSG:28992`;
// //        let mapurl = `https://geodata.nationaalgeoregister.nl/bag/wms/v1_1?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&FORMAT=image%2Fpng&TRANSPARENT=true&layers=pand&CRS=EPSG%3A28992&STYLES=&WIDTH=${this.map_img_resolution}&HEIGHT=${this.map_img_resolution}&BBOX=${bbox}`;
//           let mapurl = `https://service.pdok.nl/kadaster/cp/wms/v1_0?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&FORMAT=image%2Fpng&TRANSPARENT=true&layers=CP.CadastralParcel&CRS=EPSG%3A28992&STYLES=&WIDTH=${this.map_img_resolution}&HEIGHT=${this.map_img_resolution}&BBOX=${bbox}`;

//           return mapurl;
//         }
//         else{
          return this.viewer_default_image;
       // }
      },
      set(newname){
        //v-bind needs a setter
      }
      
    },
    center: {
      get(){    
      if(this.bagcoordinates.length == 0 ){
        return rdToWgs84(155000, 463000);  
      } 
      let x = this.bagcoordinates[0];
      let y = this.bagcoordinates[1];
      var wgs84 = rdToWgs84(x, y);
      return [wgs84.lat, wgs84.lon];
      },    
      set(newname){
        //v-bind needs a setter
      }
    },
    polygon_wgs84: function(){

      if(this.polygon_rd.length == 0) return;

      let newarr = [];

      for(let i=0; i <this.polygon_rd.length; i++ ){
        let element =this.polygon_rd[i];
        let x = element[0];
        let y = element[1];
        let wgs84 = rdToWgs84(x,y);       
        newarr.push( [wgs84.lat, wgs84.lon]  ); 
      }      
      return newarr;

    }
  },
  watch: {
    postcode: function (val, oldval) {

      this.postcode = val.toUpperCase().replace(/ /g, ""); 
      
      if(this.postcode.length > 6){
        this.postcode = null;
        return;
      }

      this.invalid_postcode = !this.postcode_regex.test(val);
      
      if(this.invalid_postcode){
        this.bagcoordinates = [];
        this.street = "";
        this.notfound = false;  
        this.bagids =[];
      }

    },
      huisnummer: function (val) {                
        if(val == ""){
          this.bagcoordinates = [];
          this.street = "";
          this.bagids = [];
          return;
        } 

        // this.street = "";
        // this.city = "";
        // this.notfound = false;
        
        this.getAddress(this.postcode, val);
    }
  },
  created:function(){
    this.viewer_image = this.viewer_default_image;
  },
 methods: {
    checkPostcode: function(evt) {
      evt = (evt) ? evt : window.event;
      //var charCode = (evt.which) ? evt.which : evt.keyCode;
      if(evt.srcElement.selectionEnd - evt.srcElement.selectionStart > 0){
        this.postcode = "";         
      }

      let isvalid = false;
      let newPostcode = this.postcode + evt.key;
      let count = newPostcode.length;

      if(count == 1){        
        isvalid = /^[1-9]$/i.test(newPostcode);
      }
      else if(count == 2){      
        isvalid = /^[1-9][0-9]$/i.test(newPostcode);
      }
      else if(count == 3){      
        isvalid = /^[1-9][0-9][0-9]$/i.test(newPostcode);
      }
      else if(count == 4){
        isvalid = /^[1-9][0-9][0-9][0-9]$/i.test(newPostcode);
      }
      else if(count == 5){
        isvalid = /^[1-9][0-9][0-9][0-9][a-z]$/i.test(newPostcode);
      }
      else if(count == 6){
        isvalid = this.postcode_regex.test(newPostcode);
      }
      
      if(isvalid == false){
         evt.preventDefault();
      }
      else {
        return true;
      }

    },
    getAddress: function(postcode, huisnummer){

      let headers = { 
                      "X-Api-Key": "l772bb9814e5584919b36a91077cdacea7",
                      "Accept-Crs": "epsg:28992" 
                    }

      //https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressenuitgebreid?postcode=3829AZ&huisnummer=14&exacteMatch=true

      fetch(`https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressenuitgebreid?postcode=${postcode}&huisnummer=${huisnummer}&exacteMatch=true`, { headers })
        .then(response => response.json())
        .then(data => {

          if(!data._embedded){            
            this.notfound = true;
            this.bagcoordinates = [];
            return;
          }

          let adres = data._embedded.adressen[0];
          this.street = adres.korteNaam;
          this.city = adres.woonplaatsNaam;
          this.bouwjaar = adres.oorspronkelijkBouwjaar[0];

      console.log(adres);

          this.verblijfsobject_id = adres.adresseerbaarObjectIdentificatie;
          this.bagids = adres.pandIdentificaties;          
          this.getBagCoordinate(this.verblijfsobject_id);
        });

    },
    getBagCoordinate: function(bagid){
      
      let headers = { "X-Api-Key": "l772bb9814e5584919b36a91077cdacea7", "Accept-Crs": "epsg:28992" }

      fetch(`https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/verblijfsobjecten/${bagid}`, { headers })
        .then(response => response.json())
        .then(data => {

          if(!data.verblijfsobject){            
            return;
          }
          this.bagcoordinates = data.verblijfsobject.geometrie.punt.coordinates;
          this.getPerceel(this.bagcoordinates[0], this.bagcoordinates[1]);

        });
    },
    getPerceel:function(x,y){
        let bbox = `${x-0.5},${y-0.5},${x+0.5},${y-0.5}`;
        fetch(`https://geodata.nationaalgeoregister.nl/kadastralekaart/wfs/v4_0?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=kadastralekaartv4:perceel&STARTINDEX=0&COUNT=1000&SRSNAME=urn:ogc:def:crs:EPSG::28992&BBOX=${bbox},urn:ogc:def:crs:EPSG::28992&outputFormat=json`)
        .then(response => response.json())
        .then(data => {
          let feature = data.features[0];
          this.kadastraleGrootteWaarde = feature.properties.kadastraleGrootteWaarde;
          //TODO support multiple polygons
          this.polygon_rd = feature.geometry.coordinates[0]; 
        });
    },
    zoomUpdated (zoom) {
      this.zoom = zoom;
    },
    centerUpdated (center) {
      this.center = center;
    },
    boundsUpdated (bounds) {
      this.bounds = bounds;
    }
   

 },

  components: {    
    ModelObj,
    LMap,
    LTileLayer,
    LMarker,
    LPolygon,
    LPolyline
  }
}
</script>

<style scoped>

.gaverder{

  position: absolute;
  bottom:10px;
  right:22px;
  /* text-align: left; */
}

.foundaddress_header{
  margin-top:8px;
  margin-top:4px;
  font-weight: bold;
}

.foundaddress{
  background-color:rgb(0, 70, 153);;
  color:#fff;
  padding: 12px;
}

.bold{
  font-weight: bold;
}

.status{

margin-top:26px;  
}

.notfound{
  border: #f00 2px solid;
  padding: 8px;  
}

.formlines{
  text-align: left;
}

.forminput{
  width: 120px;;
}

.formheader{
  font-weight: bold;
  margin-top:20px;
  margin-bottom:4px;
  text-align: left;
}

.header{
  text-align: left;
  font-size: 30px;
  font-weight: bold;
  margin-bottom: 20px;
}

.entrycontainer{
  background-color: #eee;
  height:520px;

}

.unity{
  height:520px;
}

.entryform{
  text-align: left;;
}
</style>
