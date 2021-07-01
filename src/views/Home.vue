<template>

<b-container class="bv-example-row" >

<div class="header">Uitbouw plaatsen</div>

  <b-row>

    <b-col class="entrycontainer">

    <div class="formheader">Adresgegevens</div>
    <div class="formlines">Voer het adres in waar u de uitbouw wilt gaan plaatsen</div>

      <div class="entryform">
        <div class="formheader">Postcode</div>

        <!-- <input v-model="postcode" @keypress="isNumber($event)"> -->
        <b-form-input class="forminput" v-model="postcode" ></b-form-input>   
      </div>

      <div class="entryform">
        <div class="formheader">Huisnummer + toevoeging</div>
        <b-form-input class="forminput" v-model="huisnummer" v-bind:disabled="invalid_postcode"></b-form-input>   
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
          <div class="foundaddress_header">Over dit adres hebben we de volgende gegevens gevonden:</div>

          <ul>  
            <li>Het gebouw is een rijksmonument.</li>
            <li>Het gebouw ligt in een rijksbeschermd stads- of dorpsgezicht.</li>
          </ul>

        </div>
      </div>

    </b-col>

    <b-col>     
        <img class="unity" v-bind:src="viewer_image" alt="">
    </b-col>
    
  </b-row>



</b-container>

</template>

<script>
// @ is an alias to /src
import HelloWorld from '@/components/HelloWorld.vue'

export default {
  name: 'Home',
  data: function () {
    return {
      postcode: "",
      huisnummer: "",
      invalid_postcode: true,
      viewer_default_image: "images/3dnetherlands_viewer.PNG",
      street:"",
      city: "",
      notfound:false,
      bagcoordinates: [],
      map_img_resolution:600,
      map_img_size: 40
    }
  },
  computed:{
    // notfound: function(){
      
    //  return this.street == "" && this.huisnummer != "";
    // },
    found_address: function(){
        return this.huisnummer != "" && this.street != "";
    },
    viewer_image: {
      get(){
        // if(!this.invalid_postcode && this.found_address){          
        //   return "images/hovenierstraat3.png";
        // }
        if(this.bagcoordinates.length == 3){

          let x = this.bagcoordinates[0];
          let y = this.bagcoordinates[1];
           //TODO get 3dmodel, for now show satellite photo of building
          let half = this.map_img_size/2;
          let bbox = `${x-half},${y-half},${x+this.map_img_size},${y+this.map_img_size}`;
          let mapurl = `https://geodata.nationaalgeoregister.nl/luchtfoto/rgb/wms?styles=&layers=Actueel_ortho25&service=WMS&request=GetMap&format=image%2Fpng&version=1.1.0&bbox=${bbox}&width=${this.map_img_resolution}&height=${this.map_img_resolution}&srs=EPSG:28992`;
          console.log(mapurl);
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
    postcode: function (val) {
      //TODO add regex
      this.invalid_postcode = val.length != 6;
    },
      huisnummer: function (val) {        
        if(val == "") return;

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
    isNumber: function(evt) {
      evt = (evt) ? evt : window.event;
      var charCode = (evt.which) ? evt.which : evt.keyCode;
      if ((charCode > 31 && (charCode < 48 || charCode > 57)) && charCode !== 46) 
      {
        evt.preventDefault();
      }
      else 
      {
        return true;
      }
    },
    getAddress: function(postcode, huisnummer){

      let headers = { "X-Api-Key": "l772bb9814e5584919b36a91077cdacea7" }

      fetch(`https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressen?postcode=${postcode}&huisnummer=${huisnummer}&exacteMatch=true`, { headers })
        .then(response => response.json())
        .then(data => {

          if(!data._embedded){
            //alert("niks gevonden");
            this.notfound = true;
            return;
          }

          let adres = data._embedded.adressen[0];
          this.street = adres.korteNaam;
          this.city = adres.woonplaatsNaam;

          let bagid = adres.adresseerbaarObjectIdentificatie;
          this.getBagCoordinate(bagid);
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
//          console.log(this.bagcoordinates);

        });


    }

 },

  components: {
    HelloWorld
  }
}
</script>

<style scoped>

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
