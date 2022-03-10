<template>

<b-container class="content">

<h1>Floating point precision</h1>
<h2>17 februari 2022, Bart Burkhardt</h2>
  
<p>
Bij het maken van onze testomgeving zijn we begonnen met een 3D wereld waarbij we de Dom toren in Utrecht als middenpunt hadden gebruikt. 
We zijn begonnen te ontwikkelen met een test adres in Utrecht, Stadhouderslaan 79. 
Na presentatie van de sprint resultaten in de tweewekelijks sprint review kregen we vaak als feedback de vraag of we ook andere steden beschikbaar zouden kunnen stellen. 
We hebben toen in de sprint rond de jaarwisseling opgenomen dat we alle gebouwen van Nederland zouden genereren.
Dit is inmiddels voltooid en nu kan elk adres ingevoerd worden.
</p>

<p>
Dit resulteerde in veel nieuwe feedback van meerdere adressen waar niet alles lekker in loopt. 
Een van de problemen is dat bij adressen die verder van het middenpunt zijn de rendering wat gaat bibberen. 
Dit heeft te maken met <b>floating point precision</b>
</p>

<p>
Een float is een getal met een precisie achter de komma. Unity gebruikt floats voor de positie van 3D objecten. In onze 3D omgeving is elke meter is een eenheid dus het getal voor de komma.
Dus een object met positie 10.1 staat dan op 10 meter en 10 centimeter. Een float is verdeeld in eenheden (meter) en de precisie (centimers en milimeters)
Als je dus een positie heb met een groot aantal eenheden heb je minder precisie. De applicatie zal bij berekingen dan gaan afronden in de precisie die nog over is en dit resulteert dan
in een bibberend beeld vooral als je de camera of objecten gaat verplaatsen.
</p>

<p>
Na ongeveer 5000 meter gaat dit problemen geven.
In onderstaand voorbeeld zie je een adres in Hengelo. 
Zoals je kan zien gaat de positie ver over de limiet en ontstaat er een bibberend beeld.
Unity, onze ontwikkeltool, geeft het het zelfs al aan dat een 3D object te ver weg van middenpunt staat, en dit moet dus opgelost worden.
</p>

<p>
<b-img src="images/floating point precision unity.png" fluid alt="Responsive image"></b-img>
</p>
 
<p>
    <b-img src="images/floating point precision.gif" width=800 fluid alt="Responsive image"></b-img>
</p>

<h2>Oplossing</h2>

<p>
Een oplossing is om het middenpunt te verplaatsen naar de positie van het opgevraagde adres.
</p>

<p>Dit bleek in code een vrij eenvoudige oplossing te zijn. 
Dit komt omdat we gebruik maken van the Netherlands3D library. In deze library zitten functies om coordinaten te converteren. </p>

<p>Bij het converteren naar de Unity positie wordt er gebruik gemaakt van een instelbaare configuratie. <br>
In dit geval was het voldoende om bij het starten van de applicatie als eerste deze configuratie te doen. </p>
<p>

<p><b>Het resultaat is hieronder zichtbaar, geen floating point precision problemen meer en een stabiel beeld!</b></p>

    <b-img src="images/floating point precision fixed.gif" width=800 fluid alt="Responsive image"></b-img>
 


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

h2{
    font-size: 12px;    
    margin-bottom: 20px;
}


</style>
    