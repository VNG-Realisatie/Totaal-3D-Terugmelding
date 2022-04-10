<template>
<b-container class="bv-example-row" >

<div v-if="$root.authenticated">
    <b-table-simple class="margintop100 alignLeft" hover small caption-top responsive>
        <b-thead head-variant="dark">
        <b-tr>
            <b-th>Sessie naam</b-th>
            <b-th>Detail </b-th>
            <b-th>Sessie</b-th>
            <b-th>Actie</b-th>
        </b-tr>

        <b-tr v-for="item in userfeedbackList" v-bind:key="item" >
            <b-td>{{item}}</b-td>
            <b-td>
                <b-link v-bind:href="getFeedbackDetailUrl(item)">Detail</b-link>
            </b-td>
            <b-td>
                <b-link v-bind:href="getFeedbackUrl(item)">Open sessie</b-link>
            </b-td>
            <b-td>
                <b-link @click="deleteUserfeedback(item)">Delete</b-link>
            </b-td>
            
        </b-tr>
        </b-thead>
    </b-table-simple>    
</div>

</b-container>

</template>

<script>

import shared from '../shared'

export default {
    
  name: 'UserFeedBack',
  mounted:function(){    
  },
  created:function(){
    
    if(this.$root.authenticated){
        this.getuserfeedback();             
    }

    this.$root.$on("authenticated", () => {
        this.getuserfeedback();
    })  

  },  
  data: function () {
    return {
        userfeedbackList: []
    }
  },
  methods: {
        getuserfeedback(){

            var requestOptions = {
                method: "GET",
                headers: { 
                    "Content-Type": "application/json",  
                    "Authorization": this.$root.authToken              
                }            
            };

            fetch(`${shared.backend_base}/getuserfeedbacklist`, requestOptions)
            .then(response => response.json())
            .then(data =>
            {                
                this.userfeedbackList = data.userfeedbacklist;
            
            } );

        },
        deleteUserfeedback(filename){
            var requestOptions = {
                method: "DELETE",
                headers: { 
                    "Content-Type": "application/json",                
                }            
            };

            fetch(`${shared.backend_base}/userfeedback/${filename}`, requestOptions)           
            .then(data =>
            {                
                this.getuserfeedback();            
            } );

        },
        getFeedbackUrl(feedbackFileName){            
            return `${shared.frontend_base}/3d/?sessionId=${feedbackFileName}&isuserfeedback=true&iseditmode=false`;
        },
        getFeedbackDetailUrl(feedbackFileName){
            return `#/userfeedback/${feedbackFileName}`;            
        }
    }

}

</script>
 

 <style>

 .alignLeft{
     text-align: left;
 }
 
 </style>
