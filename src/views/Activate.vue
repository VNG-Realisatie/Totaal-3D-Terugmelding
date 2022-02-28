<template>

<b-container class="content">

<p>Activeer je T3D beheer pagina, voer de code die je gekregen hebt</p>

<p>
   <b-input v-model="activateCode"></b-input>
</p>

<p>
    <b-button @click="activate">Activeer</b-button>
</p>

    






</b-container>

</template>
        

<script>
export default {
  name: 'AuthTest',
  data: function () {
    return{
      name: "Azure AD Test pagina",
      users:[],
      activateCode:""
    }
  },
  created:function(){

    this.getuserfeedback();
    
  },
  mounted:function(){        
  },
  methods: {      

        getuserfeedback(filename){
            var requestOptions = {
                method: "GET",
                headers: { 
                    "Content-Type": "application/json",                
                }            
            };

            fetch(`http://localhost:7071/api/GetUsers`, requestOptions)
            // fetch(`https://t3dapi.azurewebsites.net/api/getuserfeedback/${filename}`, requestOptions)
            .then(response => response.json())
            .then(data =>
            {
                console.log(data);
                this.users  = data;
            
            } );

        },
        activate(){
          alert(this.activateCode);
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
    