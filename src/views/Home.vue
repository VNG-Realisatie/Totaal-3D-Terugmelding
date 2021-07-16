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

          <!-- <div>{{bagcoordinates[0]}}</div>
          <div>{{bagcoordinates[1]}}</div> -->

        <div v-if="notfound" class="formlines notfound">
          <div class="bold">Helaas. Wij kunnen geen adres vinden bij deze combinatie van postcode en huisnummer.</div>
          <div>Probeer het opnieuw. Of neem contact op met de gemeente op telefoonnummer<a href="tel:14020">&#160;14020</a></div>
        </div>

        <div v-if="found_address" class="foundaddress formlines">
          <div class="foundaddress_header">Dit is het gekozen adres:</div>
          <div>{{street}} {{huisnummer}}</div>
          <div>{{postcode}} {{city}}</div>
        </div>

      <p class="gaverder">
          <b-button v-if="found_address" v-bind:href="bagurl" target="_blank" variant="danger">Ga verder</b-button>
      </p>
      
      <!-- <div v-for="id in bagids" v-bind:key="id">
        <div>Bagid:{{id}}</div>
        </div>       -->
  
      </div>

    </b-col>

    <b-col>
         <!-- <model-obj
            src="/3dmodels/bbox.obj"
            >
          </model-obj>         -->
        <!-- <img v-if="found_address" class="unity" v-bind:src="viewer_image" alt=""> -->
        <img  class="unity" v-bind:src="viewer_image" alt="">
    </b-col>
    
  </b-row>

  



</b-container>

</template>

<script>
// @ is an alias to /src
import HelloWorld from '@/components/HelloWorld.vue'
import { ModelObj } from 'vue-3d-model'

export default {
  name: 'Home',
  data: function () {
    return {
      viewer_base_url: "http://localhost:8081",
      postcode: "",
      huisnummer: "",
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
      mode_scale : { x: 10, y: 10, z: 10 }
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
        if( this.bagcoordinates.length == 3){
          let x = this.bagcoordinates[0];
          let y = this.bagcoordinates[1];
           //TODO get 3dmodel, for now show satellite photo of building
          let half = this.map_img_size/2;
          let bbox = `${x-half},${y-half},${x+this.map_img_size},${y+this.map_img_size}`;
//        let mapurl = `https://geodata.nationaalgeoregister.nl/luchtfoto/rgb/wms?styles=&layers=Actueel_ortho25&service=WMS&request=GetMap&format=image%2Fpng&version=1.1.0&bbox=${bbox}&width=${this.map_img_resolution}&height=${this.map_img_resolution}&srs=EPSG:28992`;
//        let mapurl = `https://geodata.nationaalgeoregister.nl/bag/wms/v1_1?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&FORMAT=image%2Fpng&TRANSPARENT=true&layers=pand&CRS=EPSG%3A28992&STYLES=&WIDTH=${this.map_img_resolution}&HEIGHT=${this.map_img_resolution}&BBOX=${bbox}`;
          let mapurl = `https://service.pdok.nl/kadaster/cp/wms/v1_0?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&FORMAT=image%2Fpng&TRANSPARENT=true&layers=CP.CadastralParcel&CRS=EPSG%3A28992&STYLES=&WIDTH=${this.map_img_resolution}&HEIGHT=${this.map_img_resolution}&BBOX=${bbox}`;

          return mapurl;
        }
        else{
          return this.viewer_default_image;
        }
      },
      set(newname){
        //v-bind needs a setter
      }
      
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

        this.street = "";
        this.city = "";
        this.notfound = false;
        
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

      let headers = { "X-Api-Key": "l772bb9814e5584919b36a91077cdacea7" }

      fetch(`https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressen?postcode=${postcode}&huisnummer=${huisnummer}&exacteMatch=true`, { headers })
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
        });
    }

    

 },

  components: {
    HelloWorld,
    ModelObj
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
