<template>

<b-container class="bv-example-row">

<div class="header">Uitbouw plaatsen</div>

  <b-row>

    <b-col class="entrycontainer">

    <div class="formheader">Adresgegevens</div>
    <div class="formlines">Voer het adres in waar u de uitbouw wilt gaan plaatsen</div>

      <div class="entryform">
        <div class="formheader">Postcode</div>
        <b-form-input class="forminput" v-model="postcode" ></b-form-input>   
      </div>

      <div class="entryform">
        <div class="formheader">Huisnummer + toevoeging</div>
        <b-form-input class="forminput" v-model="huisnummer" disabled></b-form-input>   
      </div>

    </b-col>

    <b-col>
      <div class="unity">Unity viewer here...</div>
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
      huisnummer: ""
    }
  },
  computed: {
  postcode: {
    // setter
    set: function (newValue) {

      this.postcode="";
      // var names = newValue.split(' ')
      // this.firstName = names[0]
      // this.lastName = names[names.length - 1]
    }
  }
},
  // watch: {
  //   postcode: function (newpostcode, oldpostcode) {
  //     //console.log(newpostcode);
  //     this.postcode = "bla";
      
  //     //this.answer = 'Waiting for you to stop typing...'
  //     //this.debouncedGetAnswer()
  //   }
  // },
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
  background:#bbb;
  height:400px;
}

.entryform{
  text-align: left;;
}
</style>
