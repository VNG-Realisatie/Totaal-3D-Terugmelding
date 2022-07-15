<template>

<b-container class="content">

  <div class="header">
    <h1 class="fronttext">Win 3D-modellen in om de digitial twin accuraat te krijgen</h1>

    <p class="fronttext">
    Team Amsterdam heeft een aantal toepassingen van het inwinnen van 3D modellen beproeft binnen het project T3D van de VNG. Deze projectpagina geeft toegang tot de drie verschillende proefopstellingen en linkt naar de website van de VNG waarop onderzoeksresultaten te vinden zijn.
    </p>
  </div>

  <b-row class="buttons">    
      <b-col class="linkcard">
        <h2>Terugmelden</h2>
        <div>Klopt er iets niet aan de 3D BAG? Geef dit aan en doe een mutatieverzoek.</div>
        <b-button href="#/terugmelden" class="button-link">Start Terugmelden</b-button>
      </b-col>

      <b-col class="linkcard">
        <h2>Vergunnings-checker</h2>
        <div>Benieuwd naar wat er vergunningsvrij aangebouwd mag worden?</div>
        <b-button href="/vergunningschecker#/vergunningschecker" class="button-link">Start Vergunningscheck</b-button>  
      </b-col>

      <b-col class="linkcard">
        <h2>Gemeenteblad</h2>
        <div>Bekijk het bouwplan van de buren op een 3D kaart en ervaar de impact.</div>
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
      <div><a href="https://github.com/VNG-Realisatie/Totaal-3D">Github Web</a></div>
      <div><a href="https://github.com/Amsterdam/3DAmsterdam/tree/develop_t3d">Github Unity</a></div>
      <div><a href="https://CloudCompetenceCenter@dev.azure.com/CloudCompetenceCenter/Totaal3Dimensionaal/_git/Totaal3Dimensionaal">Github Backend</a></div>


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

.fronttext{
color:#fff;
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
  font-size: 20px;  ;  
}

.header{
  background-image: url("/images/header_uitbouw_groot_donker_3d_model.png");
  background-repeat:no-repeat;
  background-size:cover;
  padding-top: 40px;
  padding-bottom: 10px;
  margin-right: -15px;
  margin-left: -15px;
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
  margin-right:10px;
  margin-left:10px;  
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
  margin-top:40px;
  background-color: #01489A;
}

.spacer{
margin-top:10px;
}

</style>

