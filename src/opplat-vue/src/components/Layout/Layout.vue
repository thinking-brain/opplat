<template>
  <v-app id="inspire">
    <NavigationDrawer class="d-print-none" :drawer="drawer" />
    <AppBar />
    <v-content flat wraper>
      <!-- <div class="page-wrapper"> -->
      <router-view class="content" />
      <!-- </div> -->
      <!-- <vue-snotify></vue-snotify>s -->
      <!-- App Footer -->
      <v-footer
        height="auto"
        absolute
        class="white pa-3 app--footer d-print-none"
      >
        <span class="caption">ADD &copy; {{ new Date().getFullYear() }}</span>
        <v-spacer></v-spacer>
        <!-- <span v-if="licencia" class="caption mr-1"
          >Licenciado a : {{ licencia.subscriptor }} hasta:
          {{ licencia.vencimiento }}</span
        > -->
      </v-footer>
    </v-content>
  </v-app>
</template>
<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import AppBar from "@/components/AppBar.vue";
import NavigationDrawer from "@/components/NavigationDrawer.vue";
// import ReconnectingWebSocket, { Message } from "reconnecting-websocket";

@Component({
  components: {
    NavigationDrawer,
    AppBar,
  },
})
export default class Layout extends Vue {
  public name: string = "Layout";
  public isMobile: boolean = false;
  private drawer = true;
  private clipped = false;
  private licencia = false;

  public async created() {
    scrollTo(5, 5);
  }

  public async mounted() {}
  handleBackButton() {}
  onResize() {
    this.isMobile = window.innerWidth < 600;
  }
  // Methods
}
</script>
<style lang="scss" scoped>
@import "../../style.css";
</style>
