<template>

<b-container class="content">



<h1>Bekijk het bouwplan van de buren op een 3D kaart</h1>


<p>
Stel dat bekendmakingen van vergunningsaanvragen in 3D te zien zijn. Je ziet dat de buren een omgevingsvergunning hebben aangevraagd voor een aanbouw. Je wilt dan graag weten welk effect deze geplande uitbouw op jouw leefomgeving heeft en welke schaduwwerking daarvan uit gaat.
</p>

<img src="images/plaatje_uitbouw.png" >

<p>
In het programma Totaal Driedimensionaal (T3D) van de VNG is dit één van de vraagstukken die onderzocht is. Het doel is om uiteindelijk een complete driedimensionale informatieketen te realiseren waarin het mogelijk is om op een efficiënte en transparante wijze een 3D registratie te kunnen opzetten èn beheren. Op deze website is een demonstratie te zien van twee onderdelen van het onderzoek:
een vergunningschecker en een bekendmaking in een gemeenteblad in 3D. 
</p>

<p>
Deze technische demo is een eerste werkend prototype uit het programma T3D van de samenwerkende Gemeenten Amsterdam, Rotterdam en Den Haag. Amsterdam richt zich op het spoor gebruik en kijkt hoe vanuit Vergunningverlening 3D bouwaanvragen gebruikt kunnen worden als input voor de 3D registratie. 
</p>

<b-row class="buttons">
    <b-col class="linkcard">
      <h2>Demo dynamische vergunningschecker</h2>
      <div>Een gebruiker kan in een 3D model toetsen of het gewenste ontwerp van een uitbouw valt binnen de regelgeving van de omgevingswet. In deze demo zijn vier regels geautomatiseerd en te toetsen via een in te tekenen uitbouw of in te laden 3D tekening.</div>
      <b-button href="#/vergunningschecker" class="button-link">Start Vergunningschecker</b-button>  
    </b-col>

    <b-col class="linkcard">
      <h2>Demo 3D Gemeenteblad</h2>
      <div>Ingetekende modellen in de ‘demo vergunningschecker’ kunnen ter inzage gepresenteerd worden aan omwonenden en na realisatie van de bouw worden toegevoegd in de landelijke 3D registratie conform het CityJSON datamodel.</div>
      <b-button href="#/gemeenteblad" class="button-link">Open gemeenteblad</b-button>
    </b-col>
</b-row>



  <b-row class="team">
    <b-col class="linkcard">

       <h2>Ontwikkel Team</h2>

        <b-row class="teamrow" v-for="item in theteam" v-bind:key="item.name">          
          <b-col class="avatar_img col-sm-2">
            <img v-bind:src="item.image" alt="" class="avatar">
          </b-col>
          <b-col>
            <strong>{{item.name}}</strong>
            <div>{{item.title}}</div>
          </b-col>
        </b-row>
    </b-col>

    <b-col  class="linkcard">
      <h2>Contact</h2>
      <strong>Alex Peters</strong>
      <div>Projectleider T3D</div>
      <div class="email">alex.peters@amsterdam.nl</div>
    </b-col>
  </b-row>




</b-container>

</template>

<script>

import team from '@/assets/team.json'

export default {
  name: 'Home',
  data: function () {
    return{
      name: "vergunningschecker",
      name1: "vergunningschecker2",
      theteam:team      
    }

  },
  computed:{
  },
  watch: {
  },
  created:function(){    
  },
  mounted:function(){
  },
 methods: {   
 },

  components: {    
  }
}
</script>

<style scoped>

h1{
  margin-left: 100px;
  margin-right: 100px;
  font-family: AvenirBold;
  font-size: 56px;
}

h2{
  font-family: AvenirBold;
   font-size: 24px;
}

p{
  margin-left: 100px;
  margin-right: 100px;
  margin-top: 40px;
  margin-bottom: 40px;
}

.content{
  text-align: left;
  font-family: AvenirMedium;
  font-size: 20px;
}

.buttons{
 background-color: #ff9100;
 padding: 40px;
}

.linkcard{
  background-color: #fff;
  font-size: 16px;
  width: 50%;
  padding: 20px;
  margin-right:20px;
  margin-left:20px;  
}

.avatar{
   border-radius: 50%;
   width: 44px;
}

.team{
  font-size: 16px;
  font-family: AvenirRegular;
}

.teamrow{
  margin-top:10px;
  margin-bottom:10px;  
}

.email{
  color: darkcyan;
}

.button-link{
  margin-top:40px;
  background-color: #01489A;
}


</style>

