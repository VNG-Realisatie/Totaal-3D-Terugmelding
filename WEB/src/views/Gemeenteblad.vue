<template>

<b-container class="content">

  <div class="aanvragen" v-for="item in pagedItems" v-bind:key="item.SessionId">

    <img src="images/icoon_externe_link.png" class="linkimg" alt="">

    <span @click="opensession(item)">Aanvraag omgevingsvergunning {{item.Street}} {{item.HouseNumber}}, {{item.Zipcode}} {{item.City}} </span>    
    <div class="datum">{{item.Date}}</div>
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

import shared from '../shared'

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

        let savedata = this.aanvragen[i].HTMLInitSaveData;
        if(savedata != null && savedata.instance != null && savedata.instance.HasSubmitted){
          submitted.push(savedata.instance);
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

            fetch(`${shared.backend_base}/getsessionlist`, requestOptions)
            .then(response => response.json())
            .then(data =>
            {               
                this.aanvragen = data;                                       
            } );
        },
        getsession(idstring){
            var requestOptions = {
                method: "GET"       
            };

            var id = idstring.replace('.json', '');

            fetch(`${shared.backend_base}/api/download/${id}_html`, requestOptions) 
            .then(response => response.json())        
            .then(data =>
            {     
              console.log(data);                          
            } );

        },
        opensession(item){

          if(item.simpleAddressJson == undefined){
            window.location=`${shared.frontend_base}/vergunningschecker_3d/?sessionId=${item.SessionId}`;
          }
          else{
            window.location=`${shared.frontend_base}/3d/?sessionId=${item.SessionId.replace("_html", "")}`;
          }
          
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
    