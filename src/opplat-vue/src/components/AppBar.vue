<template>
  <v-app-bar :clipped-left="$vuetify.breakpoint.lgAndUp" app color="blue darken-3" dark>
    <v-toolbar-title style="width: 300px" class="ml-0 pl-3">
      <v-app-bar-nav-icon @click.stop="changeDrawer"></v-app-bar-nav-icon>
      <span class="hidden-sm-and-down">Opplat</span>
    </v-toolbar-title>
    <!-- <v-text-field
      flat
      solo-inverted
      hide-details
      prepend-inner-icon="mdi-magnify"
      label="Search"
      class="hidden-sm-and-down"
    ></v-text-field>-->
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
            <v-list-item-title>Perfil de usuario</v-list-item-title>
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

<script>
import Util from '@/util';

export default {
  data: () => ({}),
  methods: {
    changeDrawer() {
      const visibility = !this.$store.getters.drawerVisibility;
      this.$store
        .dispatch('changeVisibility', visibility)
        .then(() => {})
        .catch((err) => {});
    },
    handleFullScreen() {
      Util.toggleFullScreen();
    },
    handleLogut() {
      this.$router.push('/auth/login');
    },
    handleSetting() {},
    handleProfile() {
      this.$router.push('/account/perfil');
    },
  },
};
</script>
