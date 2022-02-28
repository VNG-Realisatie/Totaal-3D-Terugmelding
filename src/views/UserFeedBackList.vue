<template>
<b-container class="bv-example-row" >

<div>
    <div>{{ $route.params.id  }}</div>

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

import Config from '@/assets/config.json';

export default {
  name: 'UserFeedBack',
  created:function(){    
    this.getuserfeedback();
    
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
                }            
            };

            fetch(`${Config.backend_url_base}/getuserfeedbacklist`, requestOptions)
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

            fetch(`${Config.backend_url_base}/userfeedback/${filename}`, requestOptions)           
            .then(data =>
            {                
                this.getuserfeedback();            
            } );

        },
        getFeedbackUrl(feedbackFileName){            
            return `${Config.frontend_url_base}/3d/?sessionId=${feedbackFileName}&isuserfeedback=true&iseditmode=false`;
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
