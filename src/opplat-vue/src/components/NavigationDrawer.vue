<template>
  <v-navigation-drawer
    class="d-print-none"
    v-model="drawer"
    :clipped="$vuetify.breakpoint.lgAndUp"
    app
  >
    <vue-perfect-scrollbar
      class="drawer-menu--scroll"
      :settings="scrollSettings"
    >
      <v-list dense expand>
        <v-list-item link to="/dashboard">
          <v-list-item-icon>
            <v-icon>mdi-desktop-mac-dashboard</v-icon>
          </v-list-item-icon>
          <v-list-item-title>Cuadro de Mando</v-list-item-title>
        </v-list-item>
        <v-subheader>
          Modulo Actual
          <span class="text-uppercase pl-4">{{ nombreModuloActual }}</span>
        </v-subheader>
        <template v-for="item in menus_modulo_actual">
          <!--group with subitems-->
          <v-list-group
            v-if="item.items && item.items.length > 0"
            :key="item.title"
            :group="item.group"
            :prepend-icon="item.icon"
            no-action="no-action"
          >
            <template v-slot:activator :ripple="true">
              <v-list-item-title>{{ item.title }}</v-list-item-title>
            </template>
            <v-list-item
              v-for="subItem in item.items"
              :key="subItem.name"
              link
              :to="!subItem.href ? { name: subItem.name } : null"
              :href="subItem.href"
              ripple="ripple"
              :disabled="subItem.disabled"
              :target="subItem.target"
              rel="noopener"
            >
              <v-list-item-title>{{ subItem.title }}</v-list-item-title>
              <v-list-item-icon>
                <v-icon>{{ subItem.icon }}</v-icon>
              </v-list-item-icon>
            </v-list-item>
          </v-list-group>
          <v-divider v-else-if="item.divider" :key="item.name"></v-divider>
          <!--top-level link-->
          <v-list-item
            v-else
            :to="!item.href ? { name: item.name } : null"
            :href="item.href"
            ripple="ripple"
            :disabled="item.disabled"
            :target="item.target"
            rel="noopener"
            :key="item.title"
          >
            <v-list-item-icon v-if="item.icon">
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>
            <v-list-item-title>
              {{ item.title }}
              <v-span v-if="item.cant > 0">{{ item.cant }}</v-span>
            </v-list-item-title>
          </v-list-item>
        </template>
        <v-subheader>Modulos</v-subheader>
        <template v-for="item in modulos">
          <v-list-item :key="item.title" @click="getMenusDeModulo(item.name)">
            <v-list-item-icon>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>
            <v-list-item-title>{{ item.title }}</v-list-item-title>
          </v-list-item>
        </template>
      </v-list>
    </vue-perfect-scrollbar>
  </v-navigation-drawer>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import VuePerfectScrollbar from "vue-perfect-scrollbar";
import menus from "../util/menus";
import store from "@/store";

@Component({
  components: { VuePerfectScrollbar },
})
export default class NavigationDrawer extends Vue {
  // Props

  // Data
  public mini: boolean = false;
  public modulos: any[] = menus.getModulos();
  public personalizados: any[] = [];
  public menus_modulo_actual: any[] = [];
  public nombreModuloActual = null;
  public scrollSettings: any = {
    maxScrollbarLength: 160,
  };

  // Computed
  get drawer() {
    return store.state.visibility;
  }
  set drawer(value) {
    store
      .dispatch("changeVisibility", value)
      .then(() => {})
      .catch(() => {});
  }
  get computeGroupActive() {
    return true;
  }
  public async created() {}
  // Methods
  async getMenusDeModulo(item) {
    this.nombreModuloActual = item;
    this.menus_modulo_actual = await menus.getMenusByModulo(item);
    console.log(this.menus_modulo_actual);
  }
}
</script>
<style scoped></style>
