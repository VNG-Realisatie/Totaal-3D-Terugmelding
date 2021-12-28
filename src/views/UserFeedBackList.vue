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

      <b-tr v-for="item in userfeedbackList" v-bind:key="item">
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

            fetch(`https://t3dapi.azurewebsites.net/api/getuserfeedbacklist`, requestOptions)
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

            fetch(`https://t3dapi.azurewebsites.net/api/userfeedback/${filename}`, requestOptions)
            //fetch(`http://localhost:7071/api/userfeedback/${filename}`, requestOptions)            
            .then(data =>
            {                
                this.getuserfeedback();            
            } );

        },
        getFeedbackUrl(feedbackFileName){            
            return `https://opslagt3d.z6.web.core.windows.net/3d/?sessionId=${feedbackFileName}&isuserfeedback=true`;
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
