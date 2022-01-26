<template>

<b-container class="content">

  <div class="aanvragen" v-for="item in aanvragen" v-bind:key="item.session_id">

    <img src="images/icoon_externe_link.png" class="linkimg" alt="">

    <span @click="opensession(item.session_id)">Aanvraag omgevingsvergunning {{item.street}} {{item.huisnummer}}, {{item.postcode}} {{item.city}} </span>
    <!-- <span @click="getsession(item.session_id)">Aanvraag omgevingsvergunning {{item.street}} {{item.huisnummer}}, {{item.postcode}} {{item.city}} </span> -->
    <div class="datum">{{item.date}}</div>
  </div>

</b-container>
</template>
        

<script>
export default {
  name: 'Gemeenteblad',
  data: function () {
    return{
      name: "gemeenteblad",
      aanvragen:[]
    }
  },
  created:function(){
    this.getsessionlist();
  },
  mounted:function(){    
  },
  methods: {
        getsessionlist(){
            var requestOptions = {
                method: "GET",
                headers: { 
                    "Content-Type": "application/json",                
                }            
            };

            fetch(`https://t3dapi.azurewebsites.net/api/getsessionlist`, requestOptions)
            //fetch(`http://localhost:7071/api/getsessionlist`, requestOptions)
            .then(response => response.json())
            .then(data =>
            {               
                this.aanvragen = data;
                console.log(this.aanvragen);
                                
            } );

        },
        getsession(idstring){
            var requestOptions = {
                method: "GET"       
            };

            var id = idstring.replace('.json', '');

            fetch(`https://t3dapi.azurewebsites.net/api/download/${id}_html`, requestOptions)    
            //fetch(`http://localhost:7071/api/download/${id}`, requestOptions)    
            .then(response => response.json())        
            .then(data =>
            {     
              console.log(data);                          
            } );

        },
        opensession(sessionid){
          window.location=`https://opslagt3d.z6.web.core.windows.net/3d/?sessionId=${sessionid}`;
        }
  }
}

</script>


        
<style scoped>

.content{
  text-align: left;
  margin-left: 200px;
  margin-right: 200px;
}

h1{  
  font-family: AvenirBold;
  font-size: 56px;
}

.aanvragen{  
  cursor: pointer;
  border-top: 1px #eee solid;
  width: 600px;
  font-weight: bold;
  padding-top:10px;
  padding-bottom:0px;
}

.datum{
  font-size: 12px;
  color: #bbb;
  margin-bottom: 10px;
}

.linkimg{
  margin-top:-4px;
  width: 24px;
}


</style>
    