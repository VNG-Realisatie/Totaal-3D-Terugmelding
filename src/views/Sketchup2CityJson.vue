<template>

<b-container class="bv-example-row" >


    <b-row>   
        <b-col>      
            <b-form-group>
            
            <b-form-file   
              @input="onFile"          
              accept=".ifc,.skp"
              v-if="isUploading == false "
              v-model="file"
              :state="Boolean(file)"
              placeholder="Kies een bestand of sleep het hierin...."
              drop-placeholder="Zet het bestand hier neer...">
            </b-form-file>
            
            <b-progress          
              v-if="isUploading"
              variant="info" 
              striped 
              
              height="40px"
              :value="progressValue" 
              max="100" 
              show-progress 
              class="mb-3; topmargin20">
            </b-progress>

            <b-button 
              v-if="file != null && isUploading == false"
              class="topmargin20"
              @click="convertSketchup()"  
              variant="primary">Upload bestand</b-button>

        </b-form-group>

        

        <b-button v-if="isUploaded" @click="download()" variant="primary">Download</b-button> 


        <pre>{{ cityjson }}</pre>
        
        </b-col>

    </b-row>


</b-container>

</template>

<script>

import shared from '../shared'
import axios from 'axios';


export default {
  name: 'Home',
  data: function () {
    return {
      name:"Sketchup2CityJson",
      file:null,
      isUploading: false,
      progressValue: 0,
      cityjson: null,
      cityjsonString: "",
      thefile: null

    }
  },
  computed:{
  },
  watch: {
  },
  created:function(){    
  },
  mounted:function(){   
  },
 methods: {
      onFile(file)
      {
        var ext = file.name.split('.').pop().toLowerCase();
        if(ext != "ifc" && ext != "skp")
        {
          this.file = null;        
        }       
      },
      convertSketchup()
      {
        this.isUploading = true;

        var url = `${shared.backend_base}/sketchup2cityjson/`;
        
        var formdata=  new FormData();
        formdata.append("version", this.file, this.file.name );

        var requestOptions = {
        method: "PUT",
        onUploadProgress: uploadEvent =>{
            this.progressValue = (uploadEvent.loaded / uploadEvent.total) *100;          
            this.isUploaded = this.progressValue == 100;
        }
        };
        
        axios.post(url, formdata, requestOptions)                        
        .then(response =>
        {      
            this.isUploaded = true; 
            this.cityjson = response.data;
            this.cityjsonString =  JSON.stringify( response.data ); //axios returns data as JSON
        } );
    },
    download()
    {
      var blob = new Blob([ this.cityjsonString ], { "type" : "text/plain" });
      let link = document.createElement('a')
      link.href = window.URL.createObjectURL(blob)
      link.download = 'converted.json'
      link.click();
    }  
 },

  components: {
  }
}
</script>

<style scoped>
pre{
  text-align: left;
}
</style>

 
