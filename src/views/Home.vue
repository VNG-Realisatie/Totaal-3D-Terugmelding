<template>

<b-container class="bv-example-row">

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

      <div v-if="found_address" class="formlines">
        <div>Hovevierstraat 3</div>
        <div>BAG Id: 43248572923428</div>
        <div>Bouwjaar: 1975</div>
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
      found_address:false,
      viewer_default_image: "images/3dnetherlands_viewer.PNG",
      viewer_image: ""
    }
  },
  watch: {
    postcode: function (val) {
      //TODO add regex
      this.invalid_postcode = val.length != 6;
    },
    huisnummer: function (val) {
      //TODO check building using BAG API
      this.found_address = val == 3;
      if(this.found_address){
        this.viewer_image = "images/hovenierstraat3.png";
      }
      else{
        this.viewer_image = this.viewer_default_image;
      }
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
    }
 },

  components: {
    HelloWorld
  }
}
</script>

<style scoped>

.formlines{
    margin-top:14px;
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
}

.unity{
  height:400px;
}

.entryform{
  text-align: left;;
}
</style>
