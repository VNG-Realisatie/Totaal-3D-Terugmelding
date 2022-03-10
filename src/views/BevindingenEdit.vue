<template>

<b-container class="content">

  <div v-html="myHTML"  ></div>

  <wysiwyg v-model="myHTML" />


</b-container>
</template>
        

<script>
export default {
  name: 'BevindingenEdit',
  data: function () {
    return{
      name: "Bevindingen Edit",
      myHTML:""
    }
  },
  created:function(){
    
  },
  mounted:function(){        
  },
  methods: {        
  }
}

</script>


        
<style scoped>


</style>
    