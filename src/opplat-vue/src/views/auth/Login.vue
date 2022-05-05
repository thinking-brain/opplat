<template>
  <v-card class="elevation-1 pa-3 login-card">
    <v-card-text>
      <div class="layout column align-center">
        <img :src="imgLogin" alt="Autenticacion " />
      </div>
      <v-form>
        <v-text-field
          append-icon="mdi-account"
          name="login"
          label="Usuario"
          type="text"
          v-model="username"
        ></v-text-field>
        <v-text-field
          append-icon="mdi-lock"
          name="password"
          label="Contraseña"
          id="password"
          type="password"
          v-model="password"
          v-on:keyup.enter="login"
        ></v-text-field>
      </v-form>
    </v-card-text>
    <div class="login-btn">
      <v-btn block color="primary" @click="login" :loading="loading"
        >Autenticar</v-btn
      >
    </div>
  </v-card>
</template>

<script lang="ts">
import store from "@/store";
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";

@Component({ components: {} })
export default class Login extends Vue {
  // Data
  loading: boolean = false;
  username: string = "";
  password: string = "";
  imgLogin: File = require("@/assets/images/user.jpg");
  created() {
    this.cargarLicencia();
  }
  // Methods
  public cargarLicencia() {
    // const url = api.getUrl('opplat-app', 'licencia');
    // this.axios
    //   .get(url)
    //   .then((response) => {
    //     const lic = response.data;
    //     this.$store
    //       .dispatch('agregar', lic)
    //       .then(() => {})
    //       .catch((err) => {});
    //   })
    //   .catch(() => {
    //    this.$toast.error(
    //       'No se pudo cargar la licencia. Contacte su administrador.',
    //     );
    //   });
  }
  public login() {
    this.loading = true;
    // handle login
    const { username } = this;
    const { password } = this;
    store
      .dispatch("login", { username, password })
      .then(() => {
        this.$router.push("/");
        this.loading = false;
      })
      .catch(() => {
        this.$toast.error(
          "Error conectando con el servidor de autenticacion. Contacte su administrador."
        );
        this.loading = false;
      });
  }
}
</script>
<style scoped lang="css"></style>
