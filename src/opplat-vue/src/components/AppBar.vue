<template>
  <v-app-bar
    :clipped-left="$vuetify.breakpoint.lgAndUp"
    app
    color="blue darken-3"
    class="d-print-none"
    dark
  >
    <v-toolbar-title style="width: 300px">
      <v-app-bar-nav-icon
        @click.stop="changeDrawer"
        v-model="drawer"
      ></v-app-bar-nav-icon>
      <span class="hidden-sm-and-down">Opplat</span>
    </v-toolbar-title>
    <v-spacer></v-spacer>
    <v-btn icon @click="handleFullScreen()">
      <v-icon>mdi-fullscreen</v-icon>
    </v-btn>
    <v-btn icon>
      <v-icon>mdi-bell</v-icon>
    </v-btn>
    <v-menu
      bottom
      origin="center center"
      :offset-x="true"
      :offset-y="true"
      transition="scale-transition"
    >
      <template v-slot:activator="{ on }">
        <v-btn icon v-on="on">
          <v-icon>mdi-account</v-icon>
        </v-btn>
      </template>
      <v-list class="pa-0">
        <v-list-item @click="handleProfile" ripple="ripple" rel="noopener">
          <v-list-item-action>
            <v-icon>mdi-account</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title
              >Perfil del usuario {{ username }}</v-list-item-title
            >
          </v-list-item-content>
        </v-list-item>
        <v-list-item @click="handleLogut" ripple="ripple" rel="noopener">
          <v-list-item-action>
            <v-icon>mdi-logout</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Cerrar sesion</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-menu>
  </v-app-bar>
</template>
<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import Util from "@/util";
import store from "@/store";

@Component({
  components: {},
})
export default class AppBar extends Vue {
  public username: string = "";
  public isMobile: boolean = false;
  public drawer: boolean = false;

  public async created() {
    // this.username = this.$store.getters.usuario;
    scrollTo(5, 5);
  }

  public async mounted() {}
  handleBackButton() {}
  onResize() {
    this.isMobile = window.innerWidth < 600;
  }
  // Methods
  public changeDrawer() {
    const visibility = !store.state.visibility;
    store
      .dispatch("changeVisibility", visibility)
      .then(() => {})
      .catch((err) => {});
  }
  public handleLogut() {
    this.$router.push("/auth/login");
  }
  public handleSetting() {}
  public handleFullScreen() {
    Util.toggleFullScreen();
  }
  public handleProfile() {
    this.$router.push("/account/perfil");
  }
}
</script>
<style lang="scss" scoped></style>
