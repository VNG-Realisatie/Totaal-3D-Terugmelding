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

import Config from '@/assets/config.json';

export default {
  name: 'UserFeedBack',
  mounted:function(){   
  },
  created:function(){    

    if(this.$root.authenticated){
        this.getuserfeedback(this.$route.params.id);   
    }

    this.$root.$on("authenticated", () => {
        this.getuserfeedback(this.$route.params.id);   
    })
  },
  data: function () {
    return {
        userfeedback: null,

    }
  },
   methods: {

        getuserfeedback(filename){

            var requestOptions = {
                method: "GET",
                headers: { 
                  "Content-Type": "application/json",
                  "Authorization": this.$root.authToken
                }            
            };

            fetch(`${Config.backend_url_base}/getuserfeedback/${filename}`, requestOptions)            
            .then(response => response.json())
            .then(data =>
            {
                console.log(data);
                this.userfeedback = data;
            
            } );

        },
        getFeedbackUrl(feedbackFileName){            
            return `${Config.frontend_url_base}/3d/?sessionId=${feedbackFileName}&isuserfeedback=true&iseditmode=false`;
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
 
