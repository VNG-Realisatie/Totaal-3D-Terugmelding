<template>

<b-container class="content">

    <h1>Floating point precision</h1>
  
<p>
Bij het maken van onze test omgeving zijn we begonnen met een 3D wereld waarbij we het middenpunt de Dom toren in Utrecht hadden gebruikt. 
We zijn heel agile begonnen met een test adres in Utrecht, Stadhouderslaan 79. 
Na presentatie van de sprint resultaten in de tweewekelijks sprint review kregen we vaak als feedback de vraag of we ook andere steden beschikbaar zouden kunnen stellen. 
We hebben toen in de sprint rond de jaarwisseling opgenomen dat we alle gebouwen van Nederland zouden genereren.
Dit is inmiddels voltooid en nu kan elk adres ingevoerd worden.
</p>

<p>
Dit resulteerde in veel nieuwe feedback van meerdere adressen waar niet alles lekker in loopt. 
Een van de problemen is dat bij adressen ver weg van het middenpunt de rendering wat gaat bibberen. 
Dit heeft te maken met <b>floating point precision</b>
</p>

<p>
Een float is een getal met precisie achter de komma. Unity gebruikt floats voor de positie van 3D objecten. Elke meter is een eenheid. 
Dus een Unity positie 10.1 staat dan op 10 meter en 10 centimer. Een float is verdeeld in eenheden (meter) en de precisie (centimers en milimeters)
Als je dus een positie heb met een groot aantal eenheden heb je minder precisie. Unity zal bij berekingen dan gaan afronden in de precisie die nog over is en dit resulteerd dan
in een bibberend beeld vooral als je gaat animeren.
</p>

<p>
Na ongeveer 5000 meter gaat dit problemen geven.
In onderstaand voorbeeld zie je een adres in Hengelo. 
Zoals je kan zien gaat de positie ver over de limiet en gaat er van alles mis bij in dit geval het roteren van de camera.
Unity geeft het het zelfs al aan dat een 3D object te ver weg van middenpunt staat, en dit moet dus opgelost worden.
</p>

<p>
<b-img src="images/floating point precision unity.png" fluid alt="Responsive image"></b-img>
</p>
 
<p>
    <b-img src="images/floating point precision.gif" fluid alt="Responsive image"></b-img>
</p>

<h2>Oplossing</h2>

<p>
Een oplossing is op het middepunt punt te verplaatsen naar het middenpunt van het opgevraagde adres    
</p>

  


</b-container>
</template>
        

<script>
export default {
  name: 'Gemeenteblad',
  data: function () {
    return{
      name: "Bevindingen"
    }
  },
  created:function(){
    
  },
  mounted:function(){        
  },
  methods: {        
  }
}

</script>


        
<style scoped>

.content{
  text-align: left;
  margin-left: 200px;
  margin-right: 200px;
}

/* h1{  
  font-family: AvenirBold;
  font-size: 56px;
} */


</style>
    