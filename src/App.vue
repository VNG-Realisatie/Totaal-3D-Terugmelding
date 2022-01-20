<template>
  <div id="app">

    <div>
      <b-navbar toggleable="lg" >
        <b-navbar-brand href="#">   
        <img id="logo" alt="logo" src="images/amsterdam_logo.png">    
        </b-navbar-brand>

        <!-- <b-navbar-toggle target="nav-collapse"></b-navbar-toggle> -->

        <b-collapse id="nav-collapse" is-nav>
          <b-navbar-nav>

          </b-navbar-nav>

          <!-- Right aligned nav items -->
          <b-navbar-nav class="ml-auto">

            <b-nav-item href="#">Mijn Amsterdam</b-nav-item>
            <b-nav-item href="#">English site</b-nav-item>

            <!-- <router-link to="/">Mijn Amsterdam</router-link> |
            <router-link to="/about">English site</router-link> -->

          </b-navbar-nav>
        </b-collapse>
      </b-navbar>
    </div>

    <hr>
   
    <router-view/>

    <div id="footer">
      <img src="images/footer.png" alt="">
    </div>

  </div>
</template>

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

#footer{
  margin-top:100px;
  background: #777;
    height:330px;
padding: 30px;
}

#logo{
  margin-top:-10px;
}

hr {
  margin-top:-30px;
  border: 0;  
  background-color: #eee;
  height: 40px;
}


#app {
  /* font-family: Avenir, Helvetica, Arial, sans-serif; */
  font-family: Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
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
</style>
