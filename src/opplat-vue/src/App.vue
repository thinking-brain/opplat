<template>
  <v-app>
    <router-view></router-view>
  </v-app>
</template>

<script lang="ts">
import Vue from "vue";

export default Vue.extend({
  name: "App",
  created() {
    document.addEventListener(
      "backbutton",
      (e) => {
        e.preventDefault();
        this.$router.back();
      },
      false
    );
  },
  data: () => ({
    //
  }),
});
</script>
<style></style>
