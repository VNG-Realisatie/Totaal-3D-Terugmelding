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
  </div>
</template>

<style>

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
