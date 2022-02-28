<template>
  <div id="app">

    <div>
      <b-navbar toggleable="lg" >
        <b-navbar-brand href="#">            
          <router-link to="/"><img id="logo" src="images/amsterdam_logo.png"/></router-link>        
        </b-navbar-brand>

        <!-- <b-navbar-toggle target="nav-collapse"></b-navbar-toggle> -->

        <b-collapse id="nav-collapse" is-nav>
          <b-navbar-nav class="header">{{$route.name}}</b-navbar-nav>

          <!-- Right aligned nav items -->
          <b-navbar-nav class="ml-auto">
            <!-- <b-nav-item href="#">Mijn Amsterdam</b-nav-item>
            <router-link to="/">Login</router-link> -->

<b-dropdown id="dropdown-1" text="bart.burkhardt@gmail.com" class="m-md-2">
    <b-dropdown-item href="#/userfeedback">User Feedback</b-dropdown-item>
    <b-dropdown-item href="#/users">Users
    
    </b-dropdown-item>
    <b-dropdown-item>Third Action</b-dropdown-item>
    <b-dropdown-divider></b-dropdown-divider>
    <b-dropdown-item @click="logout()">Logout</b-dropdown-item>
    <!-- <b-dropdown-item disabled>Disabled action</b-dropdown-item> -->
  </b-dropdown>


          </b-navbar-nav>
        </b-collapse>
      </b-navbar>

    </div>

    <hr>
   
    <router-view/>



<div id="footer">

<b-container>
    <div class="title">Disclaimer projectwebsite Programma T3D</div>
      
        <div>Deze website verwijst naar twee technische proefopstellingen.</div>
        <div>Aan de inhoud kunnen geen rechten worden ontleend.</div>
        <div>Deze technische demo is enkel ontwikkeld om de haalbaarheid te toetsen.</div>
      
      <!-- <img src="images/footer.png" alt=""> -->
</b-container>

</div>
  

    

  </div>
</template>

<script>
export default {
  name: 'App',
  data: function () {
    return{
      name: "App"      
    }
  },
  created:function(){
  },
  mounted:function(){        
  },
  methods: {      

        logout(){

            alert("logged out");

            // var requestOptions = {
            //     method: "GET",
            //     headers: { 
            //         "Content-Type": "application/json",                
            //     }            
            // };

            // fetch(`http://localhost:7071/api/GetUsers`, requestOptions)
            // // fetch(`https://t3dapi.azurewebsites.net/api/getuserfeedback/${filename}`, requestOptions)
            // .then(response => response.json())
            // .then(data =>
            // {
            //     console.log(data);
            //     this.users  = data;
            
            // } );

        },
        activate(){
          alert(this.activateCode);
        }


  }
}

</script>

<style>

@font-face {
  font-family: "AvenirBold";
  src: local("AvenirBold"),
   url(./fonts/avenir/AvenirNextLTPro-Bold.ttf) format("truetype");
}

@font-face {
  font-family: "AvenirDemi";
  src: local("AvenirDemi"),
   url(./fonts/avenir/AvenirNextLTPro-Demi.ttf) format("truetype");
}

@font-face {
  font-family: "AvenirHeavy";
  src: local("AvenirHeavy"),
   url(./fonts/avenir/AvenirNextLTPro-Heavy.ttf) format("truetype");
}

@font-face {
  font-family: "AvenirIt";
  src: local("AvenirIt"),
   url(./fonts/avenir/AvenirNextLTPro-It.ttf) format("truetype");
}

@font-face {
  font-family: "AvenirLight";
  src: local("AvenirLight"),
   url(./fonts/avenir/AvenirNextLTPro-Light.ttf) format("truetype");
}

@font-face {
  font-family: "AvenirMedium";
  src: local("AvenirMedium"),
   url(./fonts/avenir/AvenirNextLTPro-Medium.ttf) format("truetype");
}

@font-face {
  font-family: "AvenirRegular";
  src: local("AvenirRegular"),
   url(./fonts/avenir/AvenirNextLTPro-Regular.ttf) format("truetype");
}

@font-face {
  font-family: "AvenirThin";
  src: local("AvenirThin"),
   url(./fonts/avenir/AvenirNextLTPro-Thin.ttf) format("truetype");
}

@font-face {
  font-family: "AvenirUltLt";
  src: local("AvenirUltLt"),
   url(./fonts/avenir/AvenirNextLTPro-UltLt.ttf) format("truetype");
}

.header{
  font-family: AvenirBold;
  font-size: 20px;
}

#footer{
  margin-top:100px;
  background: #777;
  height:200px;
  padding: 30px;
  color:#fff;
  text-align: left;
}

#logo{
  margin-top:-10px;
  width: 120px;
  padding-top: 20px;
}

hr {
  margin-top:2px;
  border: 0;  
  background-color: #eee;
  height: 1px;
}

#app {
  /* font-family: Avenir, Helvetica, Arial, sans-serif; */
  font-family: AvenirLight;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  /* color: #2c3e50; */
}

#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}

.title{
  font-family: AvenirBold;  
  padding-bottom: 20px;
  font-size: 18px !important;
}
</style>
