<template>

<b-container class="bv-example-row" >
    
<h1>BIM server test</h1>

<b-button class="text-left; marginside" @click="createModelAxios()" variant="primary">Create Model</b-button>
<b-button class="text-left" @click="refresh()" variant="primary">Refresh</b-button>

  <b-table-simple class="margintop100" hover small caption-top responsive>
    <b-thead head-variant="dark">
      <b-tr>
        <b-th>Id</b-th>
        <b-th>Name</b-th>
        <b-th>Hasversions</b-th>
        <b-th>Status</b-th>
        <b-th>Action</b-th>
        <b-th>Action</b-th>  
        <b-th>Action</b-th>        
      </b-tr>

      <b-tr v-for="model in models" v-bind:key="model._id">
        <b-td>{{model._id}}</b-td>
        <b-td>{{model.name}}</b-td>
        <b-td>{{hasVersions(model.versions) }}</b-td>
        <b-td>{{getStatus(model.versions) }}</b-td>        
        <b-td><b-button :disabled="file == null || model.versions.length>0" @click="addVersionAxios(model._id)"  variant="primary">Add version</b-button></b-td>        
        <b-td><b-button :disabled="isNotDone(model.versions)" @click="getCityJSON(model._id, model.versions)"  variant="primary">Get CityJSON</b-button></b-td>     
        <b-td><b-button @click="deleteModel(model._id)"  variant="danger">Delete</b-button></b-td>        
      </b-tr>
    </b-thead>
  </b-table-simple>

    <div class="margintop100">

    <!-- Plain mode -->
    <b-form-file v-model="file" class="mt-3" plain></b-form-file>
    <div class="mt-3">Selected file: {{ file ? file.name : '' }}</div>

    <h5>Progress label</h5>
    <b-progress :value="progressValue" max="100" show-progress class="mb-3"></b-progress>

  </div>
 

</b-container>

</template>

<script>

import axios from 'axios';

export default {
  name: 'BimServerTest',
  created:function(){
    if(this.authToken != undefined){
        this.getmodels();
    }
  },
  data: function () {
    return {
        organisationId: "6194fc20c0da463026d4d8fe",
        projectId: "6194fc2ac0da463026d4d90e",        
        authToken: this.$route.query.auth,
        models:[],        
        file:null,
        progressValue:0
    }
  },
   methods: {
        refresh(){
            this.getmodels();
        },
        isNotDone(versions){
            if(versions.length == 0) return true;
            else return versions[0].status != "DONE";
        },
        hasVersions(versions){
            return versions.length > 0;
        },
        getStatus(versions){
            if(versions.length == 0) return "-";
            else{
                return versions[0].status;
            }
        },
        deleteModel(id){
            var requestOptions = {
            method: "DELETE",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${this.authToken}`
                }            
            };
            fetch(`https://bim.clearly.app/api/organisations/${this.organisationId}/projects/${this.projectId}/models/${id}`, requestOptions)            
            .then(response =>
            {
                //console.log(data);
                this.getmodels();
            } );
        },
        getmodels(){
            var requestOptions = {
            method: "GET",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${this.authToken}`
                }            
            };
            fetch(`https://bim.clearly.app/api/organisations/${this.organisationId}/projects/${this.projectId}/models`, requestOptions)
            .then(response => response.json())
            .then(data =>
            {
                console.log(data);
                this.models = data;
            } );

        },
        createModel(){

            var count = this.models.length;
            var dn = Date.now();

            var requestOptions = {
            method: "POST",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${this.authToken}`
                },
            body: JSON.stringify({ name: "Model-" + dn.toString() })
            };
            fetch(`https://bim.clearly.app/api/organisations/${this.organisationId}/projects/${this.projectId}/models`, requestOptions)
            .then(response => response.json())
            .then(data =>
            {
                console.log(data);
                this.getmodels();
            } );
            
        },
        createModelAxios(){
            
            var dn = Date.now();

            var url = `https://bim.clearly.app/api/organisations/${this.organisationId}/projects/${this.projectId}/models`;
            var body = JSON.stringify({ name: "Model-" + dn.toString() });
            
            const options = {
                method: 'POST',
                headers: { 
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${this.authToken}`
                },
                data: body,
                url
            };
            
            axios(options)
            .then((response) => {
                console.log(response);
                 this.getmodels();
            });            
        },
        getVersions(id){

            var requestOptions = {
            method: "GET",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${this.authToken}`
                }            
            };
            fetch(`https://bim.clearly.app/api/organisations/${this.organisationId}/projects/${this.projectId}/models/${id}/versions`, requestOptions)            
            .then(response => response.json())
            .then(data =>
            {
                console.log(data);            
            } );
        },
        addVersion(id){
            console.log(this.file);
            var formdata=  new FormData();
            formdata.append("version", this.file, this.file.name );

            var requestOptions = {
            method: "POST",
            headers: {                 
             //   'Content-Type' : 'multipart/form-data',
                "Authorization": `Bearer ${this.authToken}`
                },
            body: formdata           
            };
            
            fetch(`https://bim.clearly.app/api/organisations/${this.organisationId}/projects/${this.projectId}/models/${id}/versions`, requestOptions)            
            .then(response => response.json())
            .then(data =>
            {
                //console.log(data);
                this.getmodels();       
            } );
        },
        addVersionAxios(id){
            //console.log(this.file);

            var url = `https://bim.clearly.app/api/organisations/${this.organisationId}/projects/${this.projectId}/models/${id}/versions`;

            var formdata=  new FormData();
            formdata.append("version", this.file, this.file.name );

            var requestOptions = {
            method: "POST",
            headers: {                        
                "Authorization": `Bearer ${this.authToken}`
            },
            onUploadProgress: uploadEvent =>{
                this.progressValue = (uploadEvent.loaded / uploadEvent.total) *100;
                console.log(  `Upload progress: ${this.progressValue}` );
            }
            };
            
            axios.post(url, formdata, requestOptions)                        
            .then(response =>
            {
                //console.log(response);
                this.getmodels();           
            } );
        },
        getCityJSON(id, versions){
            var latest = versions[versions.length-1];

            if(latest.status == "DONE"){
                this.getCityJSONCall(id, latest.version);
            }
            else {
                alert("version is not DONE, status:" + latest.status);
            }

        },
        getCityJSONCall(modelId, versionId){  
            var requestOptions = {
                method: "GET",
                headers: {                             
                    "Authorization": `Bearer ${this.authToken}`
                }        
            };
            fetch(`https://bim.clearly.app/api/organisations/${this.organisationId}/projects/${this.projectId}/models/${modelId}/versions/${versionId}/cityjson`, requestOptions)            
            .then(response => response.text())
            .then(data =>            
            {
                var blob = new Blob([data], {
                    type: 'text/plain'
                });

                const a = document.createElement('a')
                a.href = window.URL.createObjectURL(blob)
                a.download = 'cityjson.json'
                a.click();
            } );
        },
        browsefile(){
            console.log(this.file);

            const fileReader = new FileReader();
            fileReader.onload = (e) => 
            { 
                var file = e.target.result; 
                console.log(file);
            };
            fileReader.readAsDataURL(this.file);            
    },
    saveFile(){

        }
    }

}

</script>

<style>

.margintop100{
    margin-top: 40px;
}

.marginside{
    margin-left: 10px;
    margin-right: 10px;
}

</style>