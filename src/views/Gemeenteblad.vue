<template>

<b-container class="content">

  <div class="aanvragen" v-for="item in pagedItems" v-bind:key="item.$_session_id">

    <img src="images/icoon_externe_link.png" class="linkimg" alt="">

    <span @click="opensession(item.$_session_id)">Aanvraag omgevingsvergunning {{item.$_street}} {{item.$_huisnummer}}, {{item.$_postcode}} {{item.$_city}} </span>    
    <div class="datum">{{item.$_date}}</div>
  </div>

<b-pagination
      v-model="currentPage"
      :total-rows="filtered.length"
      :per-page="perPage"      
      prev-text="Vorige"
      next-text="Volgende"      
    ></b-pagination>

</b-container>
</template>
        

<script>
export default {
  name: 'Gemeenteblad',
  computed:{
    pagedItems:function(){

      var start = (this.currentPage-1) * this.perPage;
      var end = start + this.perPage;
      var itemsLen = this.filtered.length;

      if(end > itemsLen) end = itemsLen;

      return this.filtered.slice(start,end);
    },
    filtered: function () {
      var submitted = [];

      for (let i = 0; i < this.aanvragen.length; i++) {
        if (this.aanvragen[i].$_has_submitted) {
          submitted.push(this.aanvragen[i]);
         }
      }
      return submitted;
    }
  },
  data: function () {
    return{
      name: "gemeenteblad",
      aanvragen:[],      
      perPage: 10,
      currentPage: 1
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

            fetch(`https://t3dbackend.azurewebsites.net/api/getsessionlist`, requestOptions)
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

            fetch(`https://t3dbackend.azurewebsites.net/api/download/${id}_html`, requestOptions)    
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
    