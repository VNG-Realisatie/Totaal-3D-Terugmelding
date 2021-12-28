<template>


<b-container class="bv-example-row" >



  <b-table-simple class="marginTop20" hover small caption-top responsive>
    <b-thead head-variant="dark">

      <b-tr>
        <b-td style="width:33%"></b-td>
        
        <b-td style="width:33%">
            <div class="userfeedback">
                <b-link v-bind:href="getFeedbackUrl($route.params.id)">Open sessie </b-link>          
                <!-- <div class="title">{{ $route.params.id  }}</div> -->
                <pre>{{userfeedback}}</pre>   
            </div>
        </b-td>

        <b-td style="width:33%"></b-td>        
      </b-tr>
    </b-thead>
  </b-table-simple>


</b-container>


</template>

<script>

export default {
  name: 'UserFeedBack',
  created:function(){    
    this.getuserfeedback(this.$route.params.id);
    
  },
  data: function () {
    return {
        userfeedback: null      
    }
  },
   methods: {

        getuserfeedback(filename){
            var requestOptions = {
                method: "GET",
                headers: { 
                    "Content-Type": "application/json",                
                }            
            };

            fetch(`https://t3dapi.azurewebsites.net/api/getuserfeedback/${filename}`, requestOptions)
            .then(response => response.json())
            .then(data =>
            {
                console.log(data);
                this.userfeedback = data;
            
            } );

        },
        getFeedbackUrl(feedbackFileName){            
            return `https://opslagt3d.z6.web.core.windows.net/3d/?sessionId=${feedbackFileName}&isuserfeedback=true`;
        }
    }

}

</script>

<style>

.marginTop20{
    margin-top: 20px;
}

.userfeedback{
    text-align: left;
}

.title{
    font-size: 14px;    
}

</style>
 
