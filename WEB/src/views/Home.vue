<template>

<b-container class="content">

  <div class="header">
    <h1 class="fronttext">Win 3D-modellen in om de digital twin accuraat te krijgen</h1>

    <p class="fronttext">
      Amsterdam heeft binnen het programma T3D (van VNG) in een aantal experimenten beproefd wat de randvoorwaarden en eisen van 3D data zijn om deze niet alleen te kunnen gebruiken in verschillende toepassingen maar ook te kunnen inwinnen en registreren. 
Deze projectpagina geeft toegang naar de ontwikkelde proefopstellingen en de documentatie daarvan. 
</p>
    <div class="main-image-container"><img src="/images/header_uitbouw_groot_donker_3d_model.png" class="main-image"></div>
  </div>

  <b-row class="buttons">    
      <b-col class="linkcard" style="max-width: 70%; width: 70%;">
        <h2>Terugmeldtool</h2>
        <div>
          Deze technische demo is een beproeving van een keten van verschillende features ontwikkeld in Unity, welke mogelijk ingezet kunnen worden voor een 3D-terugmeldtool. Het concept van de ontwikkelde features is m.b.v. een prototype gevalideerd met ambtenaren van Gemeente Amsterdam wie betrokken zijn bij 2D terugmeldingen en basisregistraties.
Het eindresultaat is een haalbaarheidsstudie, met de nadruk op de techniek en de output: een CityJSON bestand welke óók geschikt is bevonden om te kunnen inwinnen en te registreren.
        </div>
        <!-- <b-button href="#/terugmelden" class="button-link">Start Terugmelden</b-button> -->
        <b-button @click="OpenSession()" class="button-link">Start Terugmelden</b-button>
      </b-col>


  </b-row>
  <b-row style="background-color: #F5F5F5;">
    <b-col class="linkcard">
        <h2>Vergunningschecker</h2>
        <div>
          Het doel van de proefopstelling was tweeledig. Het eerste doel was de (technische) haalbaarheid te onderzoeken van een te ontwikkelen vergunningschecker en een tekentool in een Unity viewer (spoor gebruik).
Het tweede doel was het onderzoeken of een 3D model mogelijk te verkrijgen uit een vergunnings- proces, gebruikt kan worden om daarmee de landelijke 3D-database van de proefopstelling bij te werken (inwinnen). De gebruiker kan in deze demo een aan- of uitbouw tegen vijf checks uit de omgevingswet valideren en zien of deze vergunningsvrij gebouwd mag worden. De bevindingen en conclusies zijn te lezen in het verslag op de <a href="https://alkem.io/t3d/dashboard">projectwebsite</a>.
</div>
      </b-col>

      <b-col class="linkcard">
        <h2>Gemeenteblad</h2>
        <div>Elke gemeente publiceert door hen verleende vergunningen, zodat burgers en belanghebbenden in de gelegenheid gesteld worden bezwaar te maken. In de bekendmaking is te lezen op welk adres er welke (bouw)vergunningen zijn afgegeven. Mochten bewoners inzage willen hebben in bijvoorbeeld bouwtekeningen, dan kunnen zij hiervoor een afspraak moeten maken om deze in te zien bij het Gemeenteloket.
De proefopstelling had als doel te valideren wat gebruikers vinden van een bekendmaking voorzien van een 3D-model. De bevindingen en conclusies zijn te lezen in het verslag op de <a href="https://alkem.io/t3d/dashboard">projectwebsite</a>. 
</div>

      </b-col>
  </b-row>
  <b-row style="background-color: #F5F5F5;">
    <b-col class="linkcard" style="padding-top: 0;">        <b-button href="/vergunningschecker#/vergunningschecker" class="button-link">Start Vergunningschecker</b-button>  </b-col>
    <b-col class="linkcard" style="padding-top: 0;">        <b-button href="#/gemeenteblad" class="button-link">Open gemeenteblad</b-button></b-col>
  </b-row>
  <b-row>
    <b-col class="linkcard">
        <h2>Binnenkanten</h2>
        <div>Amsterdam heeft ook gewerkt aan het verbeteren van een algoritme dat automatisch een 3D puntenwolk van de binnenkant van een pand omzet naar een 3D model, welke omgezet kan worden naar een CityJSON bestand. 
Deze binnenzijde kan dan middels de terugmeldtool, ontwikkeld in Amsterdam, worden gepositioneerd op de juiste positie binnen de buiten-geometrie van het pand. Het registreren van binnen-geometrie is in CityJSON 1.0 nog niet mogelijk.
Dit datamodel staat niet toe dat een interne toevoeging aan de geometrie van een kamer (BuildingRoom) in de hiërarchie een afgeleide van het hoofdgebouw (Building) kan zijn. Om het in deze keten tóch mogelijk te maken om óók de binnenkant te kunnen registreren in CityJSON 1.0, is er daarom voor gekozen de kamers als BuildingParts (externe toevoeging) te vermelden in het informatiemodel. De bevindingen en conclusies zijn te lezen in het verslag op de <a href="https://alkem.io/t3d/dashboard">projectwebsite</a>.
</div>
      </b-col>

      <b-col class="linkcard">
        <div class="main-image-container"><img src="/images/binnenkanten.png" class="main-image"></div>      </b-col>
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

    <b-col  class="linkcard links">
      
      <h2>Contact</h2>
      <strong>Alex Peters</strong>
      <div>Projectleider T3D</div>
      <div class="email">alex.peters@amsterdam.nl</div>

      <div class="spacer"></div>
      <div><strong>Project website VNG</strong></div>
      <a href="https://hub.alkem.io/digileefomgeving/challenges/t3d/dashboard">Alkemio</a>

      <div class="spacer"></div>
      <div><strong>3D Amsterdam</strong></div>
      <a href="https://3d.amsterdam.nl/">3d.amsterdam.nl</a>

      
      <div class="spacer"></div>
      <div><strong>Github</strong></div>
      <div><a href="https://github.com/VNG-Realisatie/Totaal-3D/tree/main/UNITY">Github Unity</a></div>      
      <div><a href="https://CloudCompetenceCenter@dev.azure.com/CloudCompetenceCenter/Totaal3Dimensionaal/_git/Totaal3Dimensionaal">Github Backend</a></div>
      
    <div class="spacer"></div>
      <div><strong>Testmodellen</strong></div>
      <div><a download :href="download('ifc_modellen.zip')" >ifc_modellen.zip</a></div>
      <div><a download :href="download('Simpel.zip')" >Eenvoudig SketchUP model</a></div>
      <div><a download :href="download('DenHaag_stadskwekerij.city.json.zip')" >Binnenkant Kwekerijweg 8 Den Haag</a></div>      
    </b-col>
  </b-row>

</b-container>

</template>

<script>

import team from '@/assets/team.json'
import shared from '../shared'

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
  download(file){
      return `${shared.frontend_base}/bim_modellen/${file}`;
  }, 
  OpenSession(){
      window.open(
        `${shared.frontend_base}/3d/`,
        '_blank'
      );
    }

 },

  components: {    
  }
}
</script>

<style scoped>

h1{
  margin-left: 50px;
  margin-right: 50px;
  font-family: AvenirBold;
  font-size: 56px;
  
}

.fronttext{
color:#fff;
}

h2{
  font-family: AvenirBold;
   font-size: 24px;
}

p{
  margin-left: 50px;
  margin-right: 50px;
  margin-top: 40px;
  margin-bottom: 40px;
}

.content{
  text-align: left;
  font-family: AvenirMedium;
  font-size: 20px;  ;  
}

.header{
  /* background-image: url("/images/header_uitbouw_groot_donker_3d_model.png"); */
  background-repeat:no-repeat;
  background-size:cover;
  padding-top: 40px;
  padding-bottom: 40px;
  margin-right: -15px;
  margin-left: -15px;
  background-color: #00A03C;
}

.main-image-container{
  margin-left: 50px;
  margin-right: 50px;
  /* margin-bottom: 30px; */
  overflow: hidden;
}
.main-image{
  max-width: 100%;
  max-height: 100%;
}

.buttons{
 /* background-color: #ff9100; */
 padding: 40px; 
}

.linkcard{
  /* background-color: #fff; */
  font-size: 16px;
  width: 50%;
  max-width: 50%;
  padding: 20px;
  margin-left: auto;
  margin-right: auto;
  /* margin-right:10px; */
  /* margin-left:10px;   */
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

.email, .links a, .links a:visited{
  color: rgb(118, 142, 158);
  /* color: darkcyan; */
}


.button-link{
  margin-top:20px;
  margin-bottom: 20px;
  background-color: #01489A;
}

.spacer{
margin-top:10px;
}

</style>

