<template>

<b-container class="bv-example-row" >

  <div v-if="hasSession">      
      <b-button class="btnHasSession" @click="NewSession()" variant="danger">Begin opnieuw</b-button>  
      <b-button class="btnHasSession" @click="OpenSession()" variant="primary">Ga verder met ontwerp uitbouw in 3D omgeving</b-button>
    </div>

  <div v-if="!hasSession">
    <b-dropdown v-if="step==1" id="dropdown-1" text="Laad adres" class="m-md-2">
        <b-dropdown-item @click="laadAdres('2522WV', '83')">Johan Gramstraat 83 's-Gravenhage'</b-dropdown-item>
        <b-dropdown-item @click="laadAdres('2522SC', '61A')">Oudemansstraat 61A 's-Gravenhage'</b-dropdown-item>
        <b-dropdown-item @click="laadAdres('2522PD', '13')">Trembleystraat 13 's-Gravenhage'</b-dropdown-item>
    </b-dropdown>
       
    <b-form-select style="margin-bottom:30px" v-if="step==2" v-model="selected_build" :options="build_options"></b-form-select>
    
    <div v-else class="header">Nieuwe melding</div>

      <div v-if="found_address" class="backupcityjson">
        <b-button v-b-tooltip.hover title="Download the CityJson van dit adres" class="backupbutton" @click="downloadCityJson()" >download cityjson</b-button>  
        <b-button v-b-tooltip.hover title="Backup de CityJson van dit adres" class="backupbutton" @click="backupCityJson()" >backup cityjson</b-button>  
        <b-button v-b-tooltip.hover title="Zet de CityJson weer terug van de gemaakte backup" class="backupbutton" @click="restoreCityJson()" >restore cityjson</b-button>
      </div>
    

    <b-row v-if="step==1">
      <b-col v-bind:class="{ entrycontainer: !isbeschermd, 'ismonument': isbeschermd }">

        <div class="entryform">
          <input id="zoek" v-model="zoektext" placeholder="Zoek adres..."  >
          <b-list-group v-if="selected_adres == null">
            <b-list-group-item :active="isactive(index)" class="listitem" v-for="(item,index) in zoekresultaten" v-bind:key="item.identificatie" button @click="selectAdres(index)"  >{{item.omschrijving}}</b-list-group-item>
          </b-list-group>

        </div>

<div></div>

        <div v-if="zoektext == ''" class="alignleft">Deze proefopstelling werkt alleen in één gebied in Den Haag. Demo-adressen zijn bovenin te vinden,
onder “Laad adres”.</div><br/>

<b-img v-if="zoektext == ''" src="images/proefopstelling.png" width=800 fluid alt="Responsive image"></b-img>
  
        <div class="status">

          <div v-if="found_address" class="foundaddress formlines">
            <div class="foundaddress_header">Dit is het gekozen adres:</div>            
            <div v-if="huisletter == undefined && huisnummertoevoeging == undefined">{{street}} {{huisnummer}}</div>
            <div v-if="huisletter != undefined">{{street}} {{huisnummer}}{{huisletter}}</div>
            <div v-if="huisnummertoevoeging != undefined">{{street}} {{huisnummer}}-{{huisnummertoevoeging}}</div>
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
        
        <b-col v-bind:class="{ 
                      entrycontainer: !isbeschermd, 
                      'ismonument': isbeschermd, 
                      'hasfile': hasfile == 'BimMode' && bim.file == null,
                      'hasfileSelected': hasfile == 'BimMode' && bim.file != null, 
                      'nofile': hasfile == 'DrawMode',
                      'no3dmodel': add3dmodel == 'nee'
                      }">
                      
            
          <div>{{add3dmodel}} </div>

            <div class="formheader">Wilt u bij uw terugmelding een 3D model toevoegen?</div>
            <b-form-group class="alignleft">
              <b-form-radio v-model="add3dmodel" name="radio-add3dmodel" value="nee">Nee, ik geef in de 3D kaart met een annotatie aan wat niet correct is.</b-form-radio>
              <b-form-radio v-model="add3dmodel" name="radio-add3dmodel" value="ja">Ja, ik voeg een 3D model toe van de juiste situatie.</b-form-radio>              
            </b-form-group>

<div v-if="add3dmodel == 'ja'">

           
            <div class="formheader">Over welk type bouwwerk wilt u een terugmelding doen?</div>
            <b-form-group class="alignleft ">
              <b-form-radio v-model="snapToWall" name="radio-snapToWall" value="noSnap">Een bijgebouw (los van het hoofdgebouw) toevoegen</b-form-radio>
              <b-form-radio v-model="snapToWall" name="radio-snapToWall" value="snap">Een aanbouw of uitbouw toevoegen (vast aan de gevel)</b-form-radio>              
            </b-form-group>

            <div class="formheader">Heeft u een bestand van een 3D model?</div>
            <b-form-group class="alignleft">
              <b-form-radio v-model="hasfile" name="radio-hasfile" value="DrawMode">Nee ik heb geen 3D bestand, ik wil deze intekenen</b-form-radio>
              <b-form-radio v-model="hasfile" name="radio-hasfile" value="BimMode">Ja ik kan een BIM of Sketchup bestand uploaden</b-form-radio>
            
            </b-form-group>

            <b-form-file   
              @input="onFile"          
              accept=".ifc,.skp"
              class="topmargin20"
              v-if="hasfile == 'BimMode' && bim.isUploading == false "
              v-model="bim.file"
              :state="Boolean(bim.file)"
              placeholder="Kies een bestand of sleep het hierin...."
              drop-placeholder="Zet het bestand hier neer...">
            </b-form-file>
            
            <b-progress          
              v-if="bim.isUploading"
              variant="info" 
              striped 
              
              height="40px"
              :value="bim.progressValue" 
              max="100" 
              show-progress 
              class="mb-3; topmargin20">
            </b-progress>

            <div v-if="bim.isUploaded">          
                <span>Conversie status: {{bim.conversionStatus}}</span>
                <BusyAnimation :isbusy="bim.conversionStatus !='DONE'"></BusyAnimation>          
            </div>

            <b-button 
              v-if="bim.file != null && bim.isUploading == false"
              class="topmargin20"
              @click="addBim()"  
              variant="primary">Upload bestand</b-button>


        <div v-if="isBimMode && bim.file == null" class="topmargin40" style="text-align:left">
          <div class="bold">3D-testmodellen (.IFC) van uitbouwen, vast aan het hoofdgebouw</div>
          <b-list-group>
            <b-list-group-item button @click="downloadModel('Uitbouw -  in lengte.ifc')">Uitbouw -  in lengte.ifc</b-list-group-item>
            <b-list-group-item button @click="downloadModel('Uitbouw - in breedte_dakterras.ifc')">Uitbouw - in breedte_dakterras.ifc</b-list-group-item>            
            <b-list-group-item button @click="downloadModel('Uitbouw - over twee verdiepingen.ifc')">Uitbouw - over twee verdiepingen.ifc</b-list-group-item>            
          </b-list-group>  
          
          <div class="bold topmargin20">3D-testmodellen (.IFC) van losstaande bouwwerken</div>
            <b-list-group>
              <b-list-group-item button @click="downloadModel('Bijgebouw - kas.ifc')">Bijgebouw - kas.ifc</b-list-group-item>
              <b-list-group-item button @click="downloadModel('Bijgebouw - carport met schuur.ifc')">Bijgebouw - carport met schuur.ifc</b-list-group-item>            
              <b-list-group-item button @click="downloadModel('Bijgebouw - kamer.ifc')">Bijgebouw - kamer.ifc</b-list-group-item>            
          </b-list-group>

        </div>

</div>

          <p v-if="hasfile == 'DrawMode' || (isBimMode && bim.isUploaded && bim.conversionStatus == 'DONE') || add3dmodel=='nee' " 
            class="bekijk">
            <b-button @click="SaveSession()" variant="danger">{{gaverderTekst}}</b-button>
          </p>
        
        </b-col>

    </b-row>
</div>

</b-container>

</template>

<script>

import Session from '@/assets/session.json';
import Months from '@/assets/months.json';
import shared from '../shared'
import { uuid } from 'vue-uuid'; // uuid object is also exported to things
                                   // outside Vue instance.

// @ is an alias to /src
import { ModelObj } from 'vue-3d-model'
import axios from 'axios';

import L from 'leaflet';
import { LMap, 
        LTileLayer, 
        LMarker,        
        LPolygon,
        LPolyline
        } from 'vue2-leaflet';

import rdToWgs84 from "rd-to-wgs84";
import BusyAnimation from '../components/BusyAnimation.vue';

export default {
  name: 'Home',
  data: function () {
    return {
      sessionId:null,
      step: 1,    
      postcode: "",
      huisnummerinvoer: "",
      huisnummer: "",
      huisletter: "",
      huisnummertoevoeging: "",
      bouwjaar:0,      
      street:"",
      city: "",      
      ismonument:false,
      monumentUrl: "",
      isbeschermd:false,
      bagcoordinates: [],
      map_img_resolution:600,
      map_img_size: 40,      
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
      query: "3829az 14",
      zoektext: "",
      zoekresultaten:[],
      lastadres: "",
      found_address: false,
      add3dmodel: false,
      bim:{
        file:null,
        progressValue:0,
        isUploading:false,
        isUploaded:false,        
        organisationId: "6194fc20c0da463026d4d8fe",
        projectId: "6194fc2ac0da463026d4d90e",  
        currentModelId:null,
        currentVersionId:null,
        conversionStatus:"",
        blobId:null
      },
      build_options: [      
        {value: "3d", text: 'laatste build'},
        {value: "separated", text: 'losgekoppelde build'}        
      ],
      selected_build: "3d",
      searchlist_index:-1,
      is_selecting:false,
      selected_adres:null,
      snapToWall:""
    }
  },
  beforeMount () {
  	window.addEventListener('keydown', this.handleKeydown, null);
  },
  beforeDestroy () {
  	window.removeEventListener('keydown', this.handleKeydown);
  },
  computed:{
    hasSession:function(){
      return this.sessionId != null;
    },
    gaverderTekst:function(){
      if(this.isBimMode) return "Bekijk de uitbouw in de 3D omgeving";
      else if( this.add3dmodel == "nee" ) return "Ga naar de 3D omgeving";
      else return "Start ontwerp uitbouw in 3D omgeving";
    },
    isBimMode:function(){
      return this.hasfile == 'BimMode';
    },
    // isSnapToWall:function(){
    //   return this.snapToWall == "snap";
    // },
    postcodeState:function(){
        if(this.postcode.length != 6) return null;        
        return true;
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
    zoektext: function(val, oldval){

        this.found_address = false;
        this.searchlist_index = -1;

        var selected = false;
        for(var i= 0; i < this.zoekresultaten.length ;  i++){

          if(this.zoekresultaten[i].omschrijving == val){
            var adres = this.zoekresultaten[i];              
            this.getAdres(adres.identificatie);            
            selected = true;
            break;
          }
        }

        if(val != "" && !selected){
          this.zoekAdres(val, false);
          this.is_selecting = false;
          this.selected_adres = null;
        }
        else{
          this.found_address = false;
          this.zoekresultaten = [];
        }
    }
  },
  created:function(){    
    this.viewer_image = this.viewer_default_image;
  },
  mounted:function(){
      if (localStorage.sessionId) {
        this.sessionId= localStorage.sessionId;        
      }
      
  },
 methods: {
    laadAdresRedir: function(xy,id) {      
      window.location.href = `${shared.frontend_base}?position=${xy}&id=${id}`;
    },
    laadAdres: function(postcode,nummer) {      
      this.zoekAdres(`${postcode} ${nummer}`, true);
    },
    zoekAdres:function(text, select){

      let headers = { 
                      "X-Api-Key": "l772bb9814e5584919b36a91077cdacea7",
                      "Accept-Crs": "epsg:28992" 
                    } 

      let url = `https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressen/zoek?zoek=${text}&page=1&pageSize=10`;

      fetch(url, { headers })
        .then(response => response.json())
        .then(data => {
          if(data._embedded == undefined){
            this.zoekresultaten = [];            
            return;
          }

            this.zoekresultaten = data._embedded.zoekresultaten;

            if(select && this.zoekresultaten.length == 1){
              this.selectAdres(0);             
            }

        });

    },
    getAdres:function(id){

      let headers = { 
                      "X-Api-Key": "l772bb9814e5584919b36a91077cdacea7",
                      "Accept-Crs": "epsg:28992" 
                    } 

      let url = `https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressen?zoekresultaatIdentificatie=${id}`;

      fetch(url, { headers })
        .then(response => response.json())
        .then(data => {          
          var adres = data._embedded.adressen[0];
          this.verblijfsobject_id = adres.adresseerbaarObjectIdentificatie;
          this.bagids = adres.pandIdentificaties;
          this.street = adres.korteNaam;
          this.huisnummer = adres.huisnummer;          
          this.postcode = adres.postcode;
          this.huisletter = adres.huisletter;
          this.huisnummertoevoeging = adres.huisnummertoevoeging;
          this.city = adres.woonplaatsNaam;
          //console.log(adres);

          this.getBagCoordinate(this.verblijfsobject_id);

          //voor bouwjaar
          this.getPand(this.bagids[0]);

          this.found_address = true;
        });

    }, 
    getPand:function(id){

      let headers = { 
                      "X-Api-Key": "l772bb9814e5584919b36a91077cdacea7",
                      "Accept-Crs": "epsg:28992" 
                    } 

      let url = `https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/panden/${id}`;

      fetch(url, { headers })
        .then(response => response.json())
        .then(data => {     
              this.bouwjaar = data.pand.oorspronkelijkBouwjaar;
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
    addBim(){
      this.bim.isUploading = true;

      var url = `${shared.backend_base}/uploadbim/${this.bim.file.name}`;
      
      var formdata=  new FormData();
      formdata.append("version", this.bim.file, this.bim.file.name );

      var requestOptions = {
      method: "PUT",
      onUploadProgress: uploadEvent =>{
          this.bim.progressValue = (uploadEvent.loaded / uploadEvent.total) *100;          
          this.bim.isUploaded = this.bim.progressValue == 100;
          console.log(  `Upload progress: ${this.bim.progressValue}` );
      }
      };
      
      axios.put(url, formdata, requestOptions)                        
      .then(response =>
      {      
        
        console.log(response);
           this.bim.isUploaded = true;
           
           if(response.data.blobId != null){
             this.bim.blobId = response.data.blobId;
             this.bim.conversionStatus = "DONE";
           }
           else{
            this.bim.currentModelId = response.data.modelId;                
            this.bim.currentVersionId = 1;
            this.checkVersion(this.bim.currentModelId, this.bim.currentVersionId);
           }

          
      } );
    },
    checkVersion(modelId, versionId){

      var url = `${shared.backend_base}/getbimversionstatus/${this.bim.currentModelId}`;

      var requestOptions = {
        method: "GET",        
        url:url
      };
      
      axios(requestOptions)                        
      .then(response =>
      {          
          var status = response.data.conversions.cityjson;
          this.bim.conversionStatus = status;
          
          if(status != "DONE"){
            setTimeout(() => {
              this.checkVersion(modelId, versionId);
            }, 1000);
          }

      } );
    },
    onFile(file){
      var ext = file.name.split('.').pop().toLowerCase();
     
      if(ext != "ifc" && ext != "skp"){
        this.bim.file = null;        
      }       
    },
    downloadModel(filename){
      
        const link = document.createElement('a');      
        link.href = `${shared.frontend_base}/bim_modellen/${filename}`;

        link.download = filename;        
        link.click();      
    },
    NewSession(){
        localStorage.removeItem("sessionId");        
        this.sessionId = null;
        
    },
    SaveSession(){

        var date = new Date();
        var month = Months[date.getMonth()];

        localStorage.sessionId = uuid.v1();
        this.sessionId = localStorage.sessionId;

        Session.HTMLInitSaveData.instance.SessionId = localStorage.sessionId;
        Session.HTMLInitSaveData.instance.Street = this.street;
        Session.HTMLInitSaveData.instance.City = this.city;
        Session.HTMLInitSaveData.instance.HouseNumber = this.huisnummer;
        Session.HTMLInitSaveData.instance.HouseNumberAddition = this.huisnummertoevoeging;
        Session.HTMLInitSaveData.instance.ZipCode = this.postcode;
        Session.HTMLInitSaveData.instance.HasFile = this.hasfile != "DrawMode";
        Session.HTMLInitSaveData.instance.RDPosition.x = this.bagcoordinates[0];
        Session.HTMLInitSaveData.instance.RDPosition.y = this.bagcoordinates[1];
        Session.HTMLInitSaveData.instance.BagId = this.bagids[0];
        Session.HTMLInitSaveData.instance.BlobId = this.bim.blobId;
        Session.HTMLInitSaveData.instance.ModelId = this.bim.currentModelId;
        Session.HTMLInitSaveData.instance.ModelVersionId = this.bim.currentVersionId;
        Session.HTMLInitSaveData.instance.Date = `${date.getDate()} ${month} ${date.getFullYear()}`;
        Session.HTMLInitSaveData.instance.IsMonument = this.ismonument;
        Session.HTMLInitSaveData.instance.IsBeschermd = this.isbeschermd;
        Session.HTMLInitSaveData.instance.SnapToWall = this.snapToWall == "snap";
        Session.HTMLInitSaveData.instance.Add3DModel = this.add3dmodel == "ja";

      //update session server
        this.UpdateSession();

        this.OpenSession();
    },
    OpenSession(){
      window.open(
        `${shared.frontend_base}/${this.selected_build}/?sessionId=${this.sessionId}`,
        '_blank'
      );
    },
    UpdateSession(){
      var url = `${shared.backend_base}/upload/${Session.HTMLInitSaveData.instance.SessionId}_html`;
      
      //console.log(url);
      //console.log(Session);

      var requestOptions = {
                method: "PUT",
                 body: JSON.stringify(Session)
            };            
            fetch(url, requestOptions)    
            .then(response => response.text())        
            .then(data =>
            {     
              console.log(`${url}:${data}`);
            });
    },
    isactive(index){
      return this.is_selecting && index == this.searchlist_index;
    },
    handleKeydown (e) {

    	switch (e.keyCode) {     
        case 13:
          this.selectAdres(this.searchlist_index);         
          break;
        case 38:
          if(this.searchlist_index == 0) break;     
          this.searchlist_index--;        
          break;         
         case 40: 
          if(this.searchlist_index == this.zoekresultaten.length-1) break;
          this.searchlist_index++;
          this.is_selecting = true;          
          break;
        case 33: 
          this.searchlist_index = 0;
          break;
        case 34: 
          this.searchlist_index = this.zoekresultaten.length-1;
          break;

      }
    },
    selectAdres(index){
          this.zoektext = this.zoekresultaten[index].omschrijving;  
          this.selected_adres = this.zoekresultaten[index];
    },
    downloadCityJson(){
      var url = `${shared.backend_base}/getbagcityjson/${this.bagids[0]}`;

      const link = document.createElement('a');      
      link.href = url;

      link.download = `${this.bagids[0]}.ifc`;
      link.click();  
    },
    backupCityJson(){

      fetch(`${shared.backend_base}/backupcityjson/${this.bagids[0]}`)
        .then(response => response.json())
        .then(data => {
          alert(data.Status);
        }).catch(err => {
          alert("error backup cityjson");
        });
    },
    restoreCityJson(){  
      fetch(`${shared.backend_base}/restorecityjson/${this.bagids[0]}`)
        .then(response => response.json())
        .then(data => {
          alert(data.Status);
        })    
        .catch( err => {
          alert("error restore cityjson");
        });
    }
    
 },

  components: {    
    ModelObj,
    LMap,
    LTileLayer,
    LMarker,
    LPolygon,
    LPolyline,
    BusyAnimation
  }
}
</script>

<style scoped>

.btnHasSession{
  margin-top: 60px;
  margin-bottom: 60px;
  margin-left: 20px;
  margin-right: 20px;

}

.noaccess{
  margin-top: 80px;
  color:red;
  font-size: 18px;;
}

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
  bottom: 20px;
  right: 40px;
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
  /* height:520px; */
}

.hasfile{
    height:790px;
}

.hasfileSelected{
    height:460px;
}

.nofile{
    height:400px;
}

.no3dmodel{
  height: 220px;
}

.ismonument{
  background-color: #eee;
  height:566px;
}

.unity{
  height:520px;
}

.entryform{
  text-align: left;
  margin-top:18px;
}

.alignleft{
  text-align: left;
}

.topmargin10{
  margin-top: 10px;
}

.topmargin20{
  margin-top: 20px;
}

.topmargin40{
  margin-top: 40px;;
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

.entryform input{
  border-radius: 10px;
  border-color: #eee;
  height:40px;
}

#zoek{
  width:400px;
  padding-left: 10px;
  border-width: 4px;
  border-radius: 10px;
  border-style:none;
  height: 40px;
  animation-name: shadow_out;
  animation-duration: 0.6s;
  margin-bottom: 20px;
}

#zoek:focus { 
    outline: none !important;
    animation-name: shadow_in;
    animation-duration: 0.4s;
    animation-fill-mode: forwards;      
 }

 .listitem{
   font-size: 14px;
   height: 42px;
  }

 @keyframes shadow_in {
  from {box-shadow: 0 0 0px 0px #B3D2F3;}
  to {box-shadow: 0 0 0px 2px #B3D2F3;}
}

 @keyframes shadow_out {
  from {box-shadow: 0 0 0px 2px #B3D2F3;}
  to {box-shadow: 0 0 0px 0px #B3D2F3;}
}

.backupcityjson{
  text-align: left;
  margin-top:-20px;
  margin-bottom:10px;
}

.backupbutton{
  margin-right: 10px;
}

 

</style>