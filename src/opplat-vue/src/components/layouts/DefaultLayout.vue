<template>
  <v-app id="inspire">
    <NavigationDrawer class="d-print-none" :drawer="drawer" />
    <AppBar />
    <v-content flat wraper>
      <!-- <div class="page-wrapper"> -->
      <router-view class="content" />
      <!-- </div> -->
      <!-- <vue-snotify></vue-snotify> -->
      <!-- App Footer -->
      <v-footer
        height="auto"
        absolute
        class="white pa-3 app--footer d-print-none"
      >
        <span class="caption">ADD &copy; {{ new Date().getFullYear() }}</span>
        <v-spacer></v-spacer>
        <span v-if="licencia" class="caption mr-1"
          >Licenciado a : {{ licencia.subscriptor }} hasta:
          {{ licencia.vencimiento }}</span
        >
      </v-footer>
    </v-content>
  </v-app>
</template>

<script lang="ts">
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";
import AppBar from "@/components/AppBar.vue";
import NavigationDrawer from "@/components/NavigationDrawer.vue";
import store from "@/store";

@Component({ components: { AppBar, NavigationDrawer } })
export default class DefaultLayout extends Vue {
  // Computed
  get licencia() {
    return store.getters.licencia;
  }
  // Data
  dialog: boolean = false;
  drawer = null;
  items = [
    { icon: "contacts", text: "Contacts" },
    { icon: "history", text: "Frequently contacted" },
    { icon: "content_copy", text: "Duplicates" },
    {
      icon: "keyboard_arrow_up",
      "icon-alt": "keyboard_arrow_down",
      text: "Labels",
      model: true,
      children: [{ icon: "add", text: "Create label" }],
    },
    {
      icon: "keyboard_arrow_up",
      "icon-alt": "keyboard_arrow_down",
      text: "More",
      model: false,
      children: [
        { text: "Import" },
        { text: "Export" },
        { text: "Print" },
        { text: "Undo changes" },
        { text: "Other contacts" },
      ],
    },
    { icon: "settings", text: "Settings" },
    { icon: "chat_bubble", text: "Send feedback" },
    { icon: "help", text: "Help" },
    { icon: "phonelink", text: "App downloads" },
    { icon: "keyboard", text: "Go to the old version" },
  ];
}
</script>

<style scoped>
.page-wrapper {
  min-height: calc(100vh - 64px - 50px - 1px);
}
.content {
  padding-bottom: 50px;
}

@media print {
  .v-content {
    padding: 0 !important;
  }
}
</style>
