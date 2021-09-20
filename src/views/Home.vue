<template>

<b-container class="bv-example-row" >

  <b-dropdown v-if="step==1" id="dropdown-1" text="Laad adres" class="m-md-2">
      <b-dropdown-item @click="laadAdres('3583JE', '79')">Stadhouderslaan 79 Utrecht</b-dropdown-item>
      <b-dropdown-item @click="laadAdres('3523RR', '15')">Hertestraat 15 Utrecht</b-dropdown-item>
      <b-dropdown-item @click="laadAdres('3524KX', '5')">Catalonië 5 Utrecht</b-dropdown-item>
      <b-dropdown-item @click="laadAdres('1015DT', '235')">Prinsengracht 235 Amsterdam</b-dropdown-item>
  </b-dropdown>



<div class="header">Uitbouw plaatsen</div>

  <b-row v-if="step==1">
    <b-col v-bind:class="{ entrycontainer: !isbeschermd, 'ismonument': isbeschermd }">

    <div class="formheader">Adresgegevens</div>
    <div class="formlines">Voer het adres in waar u de uitbouw wilt gaan plaatsen</div>

      <div class="entryform">
        <div class="formheader">Postcode</div>
        <b-form-input class="forminput noselect" v-model="postcode" @keypress="checkPostcode($event)" :state="postcodeState"></b-form-input>
      </div>

      <div class="entryform">
        <div class="formheader">Huisnummer + toevoeging</div>
        <b-form-input class="forminput" v-model="huisnummerinvoer" v-bind:disabled="invalid_postcode" :state="addressState"></b-form-input>
      </div>

      <div class="status">

        <div v-if="notfound" class="formlines notfound">
          <div class="bold">Helaas. Wij kunnen geen adres vinden bij deze combinatie van postcode en huisnummer.</div>
          <div>Probeer het opnieuw. Of neem contact op met de gemeente op telefoonnummer<a href="tel:14020">&#160;14020</a></div>
        </div>

        <div v-if="found_address" class="foundaddress formlines">
          <div class="foundaddress_header">Dit is het gekozen adres:</div>
          <div>{{street}} {{huisnummer}}{{huisletter}}</div>
          <div>{{postcode}} {{city}}</div>          


          <p></p>

          <div v-if="found_address">
            <div class="foundaddress_header">Over dit adres hebben we de volgende gegevens gevonden:</div>
            <ul>
              <li>Bouwjaar: {{bouwjaar}}</li>
              <li>Perceeloppervlakte: {{kadastraleGrootteWaarde}}m&#178;</li>
              <li v-if="ismonument">Het gebouw is een rijksmonument <a v-bind:href="monumentUrl">[pdf]</a></li>
              <li v-if="isbeschermd">Het gebouw ligt in een rijksbeschermd stads- of dorpsgezicht.</li>
            </ul>
          </div>

        </div>

      <p class="gaverder">
          <b-button v-if="found_address" @click="verder()" variant="danger">Ga verder</b-button>
      </p>
      
      </div>

    </b-col>

    <b-col>
       
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
        
          <img v-else  class="unity" src="images/3dnetherlands_viewer.PNG" alt="">
        
    </b-col>
    
  </b-row>

  <b-row v-if="step==2">
      <b-col v-bind:class="{ entrycontainer: !isbeschermd, 'ismonument': isbeschermd }">
      
          <div class="formheader">Heeft u een bestand van een 3D model van de uitbouw?</div>
          
          <div class="formlines topmargin10">Om de vergunning te kunnen beoordelen hebben we een 3D tekening van de uitbouw nodig.</div>

          <b-form-group class="alignleft topmargin20">
            
            <b-form-radio v-model="hasfile" name="some-radios" value="A">Ja, ik heb een bestand van mijn 3D ontwerp</b-form-radio>

              <div v-if="showfile" class="topmargin10 leftmargin20 file">Vansomeren_uitb...v.1.12.BIM</div>
              
              <b-button @click="browsefile()"  class="topmargin10 leftmargin20"  variant="primary">Browse bestand</b-button>


            <b-form-radio v-model="hasfile" class="topmargin10" name="some-radios" value="B">Nee, ik heb geen bestand van een 3D ontwerp</b-form-radio>
          </b-form-group>

        <p v-if="hasfile == 'B' || (hasfile == 'A' && showfile) " class="bekijk">
          <b-button  v-bind:href="bagurl" target="_blank" variant="danger">Bekijk de uitbouw in de 3D omgeving</b-button>
      </p>

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
      step: 1,
      viewer_base_url: "http://t3d.lab4242.nl/3d/",
      postcode: "",
      huisnummerinvoer: "",
      huisnummer: "",
      huisletter: "",
      bouwjaar:0,
      invalid_postcode: true,
      street:"",
      city: "",
      notfound:false,
      ismonument:false,
      monumentUrl: "",
      isbeschermd:false,
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
      kadastraleGrootteWaarde:0,
      hasfile: "",
      showfile:false      

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
        return this.huisnummer != "" && this.street != "" &&  this.notfound == false;
    },
    not_found_address: function(){
        return !this.invalid_postcode && this.huisnummer != "" && this.street == "";
    },
    bagurl: function(){
      return `${this.viewer_base_url}?position=${this.bagcoordinates[0]}_${this.bagcoordinates[1]}&id=${this.bagids[0]}`;
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
    huisnummerinvoer: function (val) {  
      
      this.huisnummerinvoer = val.toUpperCase().replace(/ /g, ""); 

        if(val == ""){
          this.bagcoordinates = [];
          this.street = "";
          this.bagids = [];
          this.notfound = false;
          this.ismonument = false;
          this.isbeschermd = false;
          return;
        }         
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
    laadAdresRedir: function(xy,id) {      
      window.location.href = `${this.viewer_base_url}?position=${xy}&id=${id}`;      
    },
    laadAdres: function(postcode,nummer) {      
      this.postcode = postcode;
      this.huisnummerinvoer = nummer;
    },
    getAddress: function(postcode, huisnummer){

      var regex = new RegExp('([0-9]+)|([a-zA-Z]+)','g');
      var splittedArray = huisnummer.match(regex);

      var num = splittedArray[0];
      var text = splittedArray[1];
           
      let headers = { 
                      "X-Api-Key": "l772bb9814e5584919b36a91077cdacea7",
                      "Accept-Crs": "epsg:28992" 
                    }

      let url = text == undefined ? `https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressenuitgebreid?postcode=${postcode}&huisnummer=${num}&exacteMatch=true` :
        `https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressenuitgebreid?postcode=${postcode}&huisnummer=${num}&huisletter=${text}&exacteMatch=true`

      fetch(url, { headers })
        .then(response => response.json())
        .then(data => {

          if(!data._embedded){            
            this.notfound = true;
            this.ismonument = false;
            this.isbeschermd = false;
            this.bagcoordinates = [];
            return;
          }

          this.notfound = false;

          let adres = data._embedded.adressen[0];
          this.street = adres.korteNaam;
          this.huisnummer = adres.huisnummer;
          this.huisletter = adres.huisletter;
          this.city = adres.woonplaatsNaam;
          this.bouwjaar = adres.oorspronkelijkBouwjaar[0];
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
          this.checkMonument(this.bagcoordinates[0], this.bagcoordinates[1]);
          this.checkBeschermd(this.bagcoordinates[0], this.bagcoordinates[1]);
        });
    },
    checkMonument: function(x,y){
        let bbox = `${x-0.5},${y-0.5},${x+0.5},${y+0.5}`;
        fetch(`https://services.rce.geovoorziening.nl/rce/wfs?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=rce:NationalListedMonuments&STARTINDEX=0&COUNT=1&SRSNAME=EPSG:28992&BBOX=${bbox}&outputFormat=json`)
        .then(response => response.json())
        .then(data => {
          this.ismonument = data.features.length > 0;
          if(this.ismonument){
            let feature = data.features[0];
            this.monumentUrl = feature.properties.KICH_URL;
            //console.log(feature.properties);
          }
        });
    },
    checkBeschermd: function(x,y){
        let bbox = `${x-0.5},${y-0.5},${x+0.5},${y+0.5}`;
        fetch(`https://services.rce.geovoorziening.nl/rce/wfs?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=rce:ArcheologicalMonuments&STARTINDEX=0&COUNT=1&SRSNAME=EPSG:28992&BBOX=${bbox}&outputFormat=json`)
        .then(response => response.json())
        .then(data => {
          this.isbeschermd = data.features.length > 0;
          // if(this.isbeschermd){
          //   let feature = data.features[0];
          //   //console.log(feature.properties);
          // }
        });
    
    },
    getPerceel:function(x,y){
        let bbox = `${x-0.5},${y-0.5},${x+0.5},${y+0.5}`;
        fetch(`https://geodata.nationaalgeoregister.nl/kadastralekaart/wfs/v4_0?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=kadastralekaartv4:perceel&STARTINDEX=0&COUNT=1&SRSNAME=urn:ogc:def:crs:EPSG::28992&BBOX=${bbox},urn:ogc:def:crs:EPSG::28992&outputFormat=json`)
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
    },
    verder(){
      this.step = 2;
    },
    browsefile(){
        this.showfile = true;        
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

a:visited, a:link {
  color: #fff;
}

.gaverder{
  position: absolute;
  bottom: -4px;
  right: 16px;
  right:22px;
}

.bekijk{
  position: absolute;
  bottom: 60px;
  right: 100px;
}

.foundaddress_header{
  margin-top:8px;
  margin-top:4px;
  font-weight: bold;
}

.foundaddress{
  background-color:rgb(0, 70, 153);;
  color:#fff;
  padding: 10px;
  margin-top: -10px;
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

.ismonument{
  background-color: #eee;
  height:566px;
}

.unity{
  height:520px;
}

.entryform{
  text-align: left;;
}

.alignleft{
  text-align: left;
}

.topmargin10{
  margin-top: 10px;
}

.topmargin20{
  margin-top: 20px;;
}

.leftmargin10{
  margin-left: 10px;
}
.leftmargin20{
  margin-left: 20px;
}

.file{
  border: 1px solid rgb(187, 187, 187);
  width: 224px;
  padding: 4px;
  background-color: rgb(221, 221, 221);

}

</style>
