<template>
    <span class="dot">{{dots}}</span>    
</template>

<script>
export default {
  name: 'BusyAnimation',
  props: {
    isbusy: Boolean
  },  
  data: function () {
    return {
      steps: 1
      }
  },
  computed:{
    dots:function(){

        if(this.isbusy == false) return '';

        if(this.steps == 1) return '.';
        else if(this.steps == 2) return '..';
        else if(this.steps == 3) return '...';

    }
  },
  created:function(){      
      this.tick();
  },
  methods: {
    tick: function(evt) {

        if(this.$props.isbusy){
            setTimeout(() => {
                this.tick();    
            }, 1000);

            this.steps++;
            if(this.steps == 4) this.steps = 1;
        }
        else{
            this.steps = 0;
        }

    }
   }

}
</script>

<style scoped>
.dot{    
    font-size: 40px;
}
</style>