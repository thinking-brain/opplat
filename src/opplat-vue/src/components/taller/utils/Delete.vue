<template>
  <v-menu v-model="menu" offset-x max-width="250" persistent>
    <template v-slot:activator="{ on }">
      <v-btn
        :class="
          item.activo
            ? 'v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small red--text'
            : 'v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small orange--text'
        "
        small
        @click="item.activo ? (menu = true) : restoreItem(item)"
        slot="activator"
      >
        <v-icon>{{ item.activo ? "mdi-delete" : "mdi-restore" }}</v-icon>
      </v-btn>
    </template>
    <v-card>
      <v-list>
        <v-list-item>
          <v-list-item-content class="headline text-center">
            <v-list-item-title>{{ text.title }}</v-list-item-title>
            <v-list-item-subtitle class="mt-3"
              >{{ dataItem }}
            </v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
      </v-list>
      <v-divider></v-divider
      ><v-card-actions class="mx-3">
        <v-btn color="error darken-1" text @click="deleteItem(item)"
          >Eliminar</v-btn
        >
        <v-spacer></v-spacer>
        <v-btn color="success darken-1" text>Cancelar</v-btn>
      </v-card-actions>
    </v-card>
  </v-menu>
</template>
<script>
import api from "@/api";

export default {
  props: {
    item: {
      type: Object,
      default: {},
    },
    text: {
      type: Object,
      default: {},
    },
    dataItem: {
      type: String,
      default: "",
    },
    urlObject: {
      type: String,
      default: "",
    },
  },

  data: () => ({
    menu: false,
    formHasErrors: false,
    errors: [],
  }),
  computed: {},
  created() {},
  watch: {},
  methods: {
    updateItem() {
      const url = api.getUrl("taller", `${this.urlObject}`);
      this.axios.put(`${url}/${this.item.id}`, this.item).then(
        (response) => {
          this.getResponse(response);
        },
        (error) => {
          console.log(error);
        }
      );
    },
    deleteItem() {
      this.item.activo = false;
      this.updateItem();
    },
    restoreItem() {
      this.item.activo = true;
      this.updateItem();
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
  },
};
</script>
<style></style>