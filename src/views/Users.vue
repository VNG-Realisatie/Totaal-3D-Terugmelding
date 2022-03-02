<template>

<b-container class="content">

  <div v-if="$root.authenticated">
      <b-table-simple class="margintop100" hover small caption-top responsive>
          <b-thead head-variant="dark">
            <b-tr>
              <b-th>Email</b-th>
              <b-th>Invite Id</b-th>
              <b-th>IsActive</b-th>
              <b-th></b-th>        
            </b-tr>

            <b-tr v-for="user in users" v-bind:key="user.RowKey" >
              <b-td>{{user.PartitionKey}}</b-td>
              <b-td>{{user.RowKey}}</b-td>
              <b-td>{{user.IsActive}}</b-td>
              <b-td><a href="#" @click="copyText(user)">Kopieer invite tekst</a></b-td>
              
            </b-tr>
          </b-thead>
        </b-table-simple>
  </div>

  
  
</b-container>

</template>
        

<script>

import Config from '@/assets/config.json';

export default {
  name: 'AuthTest',
  data: function () {
    return{
      name: "T3D Users",
      users:[],
      selectedUser:"",
      selectedInviteCode:""
      
    }
  },
  created:function(){    

    if(this.$root.authenticated){      
        this.getusers();              
    }

    this.$root.$on("authenticated", () => {      
        this.getusers();  
    })  
        
  },
  mounted:function(){        
  },
  methods: {      

        getusers(){

          var requestOptions = {
                method: "GET",
                headers: { 
                    "Content-Type": "application/json",
                    "Authorization": this.$root.authToken
                }            
            };

            fetch(`${Config.backend_url_base}/getusers`, requestOptions)                        
            .then(response => {
              if(response.status ==200){
                response.json().then(data =>
                {  
                  this.users = data;
                });
             
              }              
            } );

        },
        ShowActivate(user){          
          this.selectedUser = user.PartitionKey;
          this.selectedInviteCode = user.RowKey;
        },
        async copyText(user){

          try {
          
          var content = `<div>Beste ${user.PartitionKey}</div>
          <p>
            <div>Hierbij je activatie code</div>          
          </p>

          <p>
            <b>${user.RowKey}</b>
          </p>

          <div>Ga naar de volgende site en voer de code in</div>
          <p>
            <a href="https://t3dstorage.z6.web.core.windows.net/#/activate">https://t3dstorage.z6.web.core.windows.net/#/activate</a>
          </p>

          <img src="https://cataas.com/cat/says/welcome%20to%20T3D?width=300" >
          `; 
                    
          const blob = new Blob([content], { type: "text/html" });
          const richTextInput = new ClipboardItem({ "text/html": blob });
          await navigator.clipboard.write([richTextInput]);

            //await navigator.clipboard.writeText(this.$refs.panel.innerHTML);            
          } catch($e) {
            alert('Er ging iets mis bij het kopieren naar klembord');
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





</style>
    