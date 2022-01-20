<template>

<b-container>

<div class="content">

<h1>Toetsing waarde 3D modellen rondom de aanvraag van bouwvergunningen</h1>


<p>
Het programma Totaal Driedimensionaal (T3D) van de VNG heeft als doel een complete driedimensionale informatieketen te realiseren. Een complete informatieketen maakt het mogelijk om op een efficiënte en transparante wijze een 3D registratie te kunnen opzetten èn beheren. Daarbij wordt gebruik gemaakt van het benutten van (3D) informatie die in bestaande beleidsprocessen gebruikt worden. In eerste instantie bij en voor gemeenten, maar de verwachting is dat straks ook andere overheden hier gebruik van kunnen maken. Iedereen, ook burgers, hebben namelijk veel baat bij een goede 3D geo-registratie en alle toepassingen die daarmee gemaakt kunnen worden.
</p>

<img src="images/plaatje_uitbouw.png" >

<p>
Na de inhoudelijke verkenning in fase één lag de focus in fase twee op de techniek. Op deze website is het resultaat daarvan te zien. De technische demo ‘Vergunningschecker’, is een eerste werkend prototype van het spoor gebruik uit het programma T3D. In de laatste fase in 2022 zal het prototype verder worden verbeterd zodat het ingezet kan worden in de praktijk en als deeloplossing kan worden ingezet binnen het beheer van de (landelijke) 3D registratie.  De technische demo ‘Gemeenteblad’ geeft het beeld weer voor belanghebbenden die willen weten welke aanvragen er zijn ingediend en wat de status daarvan is. Geheel in 3D.
</p>

<p>
Tot slot zijn op deze site zijn de ingangen naar alle andere  technische demo’s binnen het programma T3D te vinden. Zie hiervoor de  verschillende links naar de prototypen en het eindrapport.
</p>

<p>
Het programma T3D is een samenwerking tussen  de Gemeenten Amsterdam, Rotterdam en Den Haag gezamenlijk een proefopstelling gemaak.
</p>

<p>
Amsterdam richt zich op het spoor gebruik en kijkt hoe vanuit Vergunningverlening 3D bouwaanvragen gebruikt kunnen worden als input voor de 3D registratie. Die wordt daarmee een stuk gedetailleerder maar vooral ook actueler.  
</p>

<b-button href="#/vergunningschecker">Vergunningschecker</b-button>

Ingang naar de technische demo waarin onderzocht is wat de mogelijkheden zijn van een dynamische vergunningschecker. Een gebruiker kan in een 3D model toetsen of het gewenste ontwerp van een uitbouw valt binnen de regelgeving van de DSO. In deze demo zijn vier regels (van de ruim 50) geautomatiseerd en te toetsen via een in te tekenen uitbouw of in te laden 3D tekening.



</div>




</b-container>

</template>

<script>

export default {
  name: 'Home',
  data: function () {
return{
  name: "vergunningschecker"
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

.content{
  text-align: left;
  font-family: AvenirBold;

}

</style>