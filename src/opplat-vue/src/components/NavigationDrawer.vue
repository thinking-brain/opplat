<template>
  <v-navigation-drawer v-model="drawer" :clipped="$vuetify.breakpoint.lgAndUp" app>
    <vue-perfect-scrollbar class="drawer-menu--scroll" :settings="scrollSettings">
      <v-list dense expand>
        <v-subheader>Personalizados</v-subheader>
        <template v-for="item in personalizados">
          <!--group with subitems-->
          <v-list-group
            v-if="item.items"
            :key="item.title"
            :group="item.group"
            :prepend-icon="item.icon"
            no-action="no-action"
          >
            <v-list-tile slot="activator" ripple="ripple">
              <v-list-tile-content>
                <v-list-tile-title>{{ item.title }}</v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <template v-for="subItem in item.items">
              <!--sub group-->
              <v-list-group
                v-if="subItem.items"
                :key="subItem.name"
                :group="subItem.group"
                sub-group="sub-group"
              >
                <v-list-tile slot="activator" ripple="ripple">
                  <v-list-tile-content>
                    <v-list-tile-title>{{ subItem.title }}</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-list-tile
                  v-for="grand in subItem.children"
                  :key="grand.name"
                  :to="genChildTarget(item, grand)"
                  :href="grand.href"
                  ripple="ripple"
                >
                  <v-list-tile-content>
                    <v-list-tile-title>{{ grand.title }}</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
              </v-list-group>
              <!--child item-->
              <v-list-tile
                v-else
                :key="subItem.name"
                :to="genChildTarget(item, subItem)"
                :href="subItem.href"
                :disabled="subItem.disabled"
                :target="subItem.target"
                ripple="ripple"
              >
                <v-list-tile-content>
                  <v-list-tile-title>
                    <span>{{ subItem.title }}</span>
                  </v-list-tile-title>
                </v-list-tile-content>
                <v-list-tile-action v-if="subItem.action">
                  <v-icon :class="[subItem.actionClass || 'success--text']">{{ subItem.action }}</v-icon>
                </v-list-tile-action>
              </v-list-tile>
            </template>
          </v-list-group>
          <v-divider v-else-if="item.divider" :key="item.name"></v-divider>
          <!--top-level link-->
          <v-list-tile
            v-else
            :to="!item.href ? { name: item.name } : null"
            :href="item.href"
            ripple="ripple"
            :disabled="item.disabled"
            :target="item.target"
            rel="noopener"
            :key="item.name"
          >
            <v-list-tile-action v-if="item.icon">
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>{{ item.title }}</v-list-tile-title>
            </v-list-tile-content>
            <v-list-tile-action v-if="item.subAction">
              <v-icon class="success--text">{{ item.subAction }}</v-icon>
            </v-list-tile-action>
          </v-list-tile>
        </template>
        <v-subheader>Modulo Actual</v-subheader>
        <template v-for="item in menus_modulo_actual">
          <!--group with subitems-->
          <v-list-group
            v-if="item.items.length > 0"
            :key="item.title"
            :group="item.group"
            :prepend-icon="item.icon"
            no-action="no-action"
          >
            <template v-slot:activator :ripple="true">
              <v-list-item-title>{{ item.title }}</v-list-item-title>
            </template>
            <v-list-item v-for="subItem in item.items" :key="subItem.name" link>
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
            :key="item.name"
          >
            <v-list-item-icon v-if="item.icon">
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>
            <v-list-item-title>{{ item.title }}</v-list-item-title>
          </v-list-item>
        </template>
        <v-subheader>Modulos</v-subheader>
        <template v-for="item in modulos">
          <v-list-item :key="item.title" ripple="ripple" @click="getMenusDeModulo(item.name)">
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

<script>
import api from "@/api.js";
import VuePerfectScrollbar from "vue-perfect-scrollbar";

export default {
  components: {
    VuePerfectScrollbar
  },
  props: {
    expanded: {
      type: Boolean,
      default: true
    },
    drawWidth: {
      type: [Number, String],
      default: "260"
    }
  },
  computed: {
    drawer: {
      get() {
        return this.$store.getters.drawerVisibility;
      },
      set(value) {
        let visibility = !this.$store.getters.drawerVisibility;
        this.$store
          .dispatch("changeVisibility", value)
          .then(() => {})
          .catch(err => {});
      }
    },
    computeGroupActive() {
      return true;
    },

    sideToolbarColor() {
      return this.$vuetify.options.extra.sideNav;
    }
  },
  data() {
    return {
      mini: false,
      modulos: [],
      personalizados: [],
      menus_modulo_actual: [],
      scrollSettings: {
        maxScrollbarLength: 160
      }
    };
  },
  created() {
    let url = api.getUrl("opplat-app", "menus");
    console.log(url);
    this.axios
      .get(url)
      .then(response => {
        this.modulos = response.data.modulos;
        this.personalizados = response.data.personalizados;
      })
      .catch(e => {
        this.errors.push(e);
        this.$snotify.error("Error cargando los menus. " + e);
      });
  },

  methods: {
    genChildTarget(item, subItem) {
      if (subItem.href) return;
      if (subItem.component) {
        return {
          name: subItem.component
        };
      }
      return { name: `${item.group}/${subItem.name}` };
    },
    getMenusDeModulo(modulo) {
      let url = api.getUrl("opplat-app", "menus/frommodulo?modulo=" + modulo);
      this.axios
        .get(url)
        .then(response => {
          this.menus_modulo_actual = response.data;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error("Error cargando los menus. " + e);
        });
    }
  }
};
</script>