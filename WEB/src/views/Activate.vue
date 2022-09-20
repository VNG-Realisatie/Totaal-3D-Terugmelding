<template>

<b-container class="content">

  <div v-if="!isActivated">
      <p>Activeer je T3D beheer pagina met de code die je gekregen hebt</p>

      <p>
        <b-input v-model="activateCode"></b-input>
      </p>

      <p>
          <b-button @click="activate()">Activeer</b-button>
      </p>
  </div>

  <div v-if="isActivated">
     <img src="https://cataas.com/cat/says/Je bent nu geactiveerd!?width=300" >
  </div>


</b-container>

</template>
        

<script>

import shared from '../shared'

export default {
  name: 'AuthTest',
  data: function () {
    return{
      name: "Activate",
      users:[],
      activateCode:"",
      isActivated:false
    }
  },
  created:function(){
    
  },
  mounted:function(){        
  },
  methods: {      
        activate(){

            var requestOptions = {
                method: "POST",
                headers: { 
                    "Content-Type": "application/json",                
                },
                body: JSON.stringify({  "authToken" : this.activateCode.trim()  })     
            };

            fetch(`${shared.backend_base}/activate`, requestOptions)            
            .then(response => {
                  if(response.status ==200){
                    response.json().then(data =>
                    {  
                        this.isActivated = true;
                        localStorage.authToken = this.activateCode;
                        localStorage.user = data.user;                                                
                        this.$root.$emit('authenticated');
                    });
                  };
            });

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




</style>
    