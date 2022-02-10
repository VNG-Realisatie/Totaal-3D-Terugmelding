<template>
  <b-container class="content">
    <div
      class="aanvragen"
      v-for="item in filtered"
      v-bind:key="item.$_session_id"
    >
      <img src="images/icoon_externe_link.png" class="linkimg" alt="" />

      <span @click="opensession(item.$_session_id)"
        >Aanvraag omgevingsvergunning {{ item.$_street }}
        {{ item.$_huisnummer }}, {{ item.$_postcode }} {{ item.$_city }}
      </span>
      <div class="datum">{{ item.$_date }}</div>
    </div>
  </b-container>
</template>
        

<script>
export default {
  name: "Gemeenteblad",
  data: function () {
    return {
      name: "gemeenteblad",
      aanvragen: [],
    };
  },
  created: function () {
    this.getsessionlist();
  },
  mounted: function () {},
  methods: {
    getsessionlist() {
      var requestOptions = {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      };

      fetch(
        `https://t3dapi.azurewebsites.net/api/getsessionlist`,
        requestOptions
      )
        //fetch(`http://localhost:7071/api/getsessionlist`, requestOptions)
        .then((response) => response.json())
        .then((data) => {
          //only show requests that have been submitted
          // for (let i = 0; i < data.length; i++) {
          //   console.log(data[i].$_session_id);
          //   if(data[i].$_has_submitted){
          //     console.log(i);
          //     this.aanvragen[this.aanvragen.length] = data[i];
          //   }
          // }

          // console.log(this.aanvragen);
          this.aanvragen = data;
          console.log(this.aanvragen);
        });
    },
    getsession(idstring) {
      var requestOptions = {
        method: "GET",
      };

      var id = idstring.replace(".json", "");

      fetch(
        `https://t3dapi.azurewebsites.net/api/download/${id}_html`,
        requestOptions
      )
        //fetch(`http://localhost:7071/api/download/${id}`, requestOptions)
        .then((response) => response.json())
        .then((data) => {
          console.log(data);
        });
    },
    opensession(sessionid) {
      window.location = `https://opslagt3d.z6.web.core.windows.net/3d/?sessionId=${sessionid}`;
    },
  },
  computed: {
    filtered: function () {
      var submitted = [];

      for (let i = 0; i < this.aanvragen.length; i++) {
        if (this.aanvragen[i].$_has_submitted) {
          console.log(i);
          submitted.push(this.aanvragen[i]);
        }
      }
      return submitted;
    },
  },
};
</script>


        
<style scoped>
.content {
  text-align: left;
  margin-left: 200px;
  margin-right: 200px;
}

h1 {
  font-family: AvenirBold;
  font-size: 56px;
}

.aanvragen {
  cursor: pointer;
  border-top: 1px #eee solid;
  width: 600px;
  font-weight: bold;
  padding-top: 10px;
  padding-bottom: 0px;
}

.datum {
  font-size: 12px;
  color: #bbb;
  margin-bottom: 10px;
}

.linkimg {
  margin-top: -4px;
  width: 24px;
}
</style>
    