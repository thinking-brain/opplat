<template>
  <v-container grid-list-xl fluid>
    <v-card>
      <v-card-title>
        {{ title }}
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-btn color="primary" @click="addItem()">Nuevo</v-btn>
        <v-spacer></v-spacer>
        <div class="flex-grow-1"></div>
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          @click:append="searchMethod()"
          label="Buscar"
          single-line
          hide-details
          clearable
        ></v-text-field>
      </v-card-title>
      <v-data-table
        :headers="headers"
        :items="items"
        :options.sync="options"
        :server-items-length="totalItems"
        :loading="loading"
        loading-text="Buscando... Por favor espere"
        class="elevation-1"
        @getTecnicosFromApi="getTecnicosFromApi"
        :footer-props="{
          itemsPerPageText: 'Resultados por Página',
        }"
      >
        <template v-slot:no-data>
          <v-row justify="center">
            <h2 class="heading">No hay datos disponibles</h2>
          </v-row>
        </template>
        <template v-slot:item.action="{ item }">
          <v-tooltip top color="primary">
            <template v-slot:activator="{ on }">
              <v-btn
                class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small primary--text"
                small
                v-on="on"
                @click="editItem(item)"
                slot="activator"
              >
                <v-icon>v-icon notranslate mdi mdi-pen theme--dark</v-icon>
              </v-btn>
            </template>
            <span>Editar</span>
          </v-tooltip>
          <Delete
            v-bind:item="item"
            v-bind:text="text"
            v-bind:dataItem="item.nombre"
            :entity="entity"
            @close="$emit('close')"
          />
        </template>
      </v-data-table>
    </v-card>
    <AddEdit
      :dialogEdit="dialogEdit"
      :item="item"
      :editedIndex="editedIndex"
      @getTecnicosFromApi="getTecnicosFromApi"
      @close="close"
    />
  </v-container>
</template>

<script>
import api from "@/api";
import AddEdit from "@/components/taller/tecnicos/AddEdit.vue";
import Delete from "@/components/taller/utils/Delete.vue";

export default {
  components: { AddEdit, Delete },
  data() {
    return {
      search: "",
      dialogEdit: false,
      dialogDetails: false,
      loading: false,
      editedIndex: -1,
      items: [],
      errors: [],
      options: {},
      headers: [
        {
          text: "Nombre",
          align: "left",
          sortable: true,
          value: "nombre",
        },
        {
          text: "Cargo",
          align: "left",
          sortable: true,
          value: "cargo",
        },
        {
          text: "Taller",
          align: "left",
          sortable: true,
          value: "taller.nombre",
        },
        { text: "Acciones", align: "left", value: "action", sortable: false },
      ],
      item: {},
      text: { title: "Estas Seguro de Eliminar a " },
      cardTitle: " Listado de Técnicos",
      entity: "Tecnicos",
      totalItems: 0,
    };
  },
  computed: {
    title() {
      return this.cardTitle;
    },
  },
  watch: {
    options: {
      handler() {
        this.getTecnicosFromApi();
      },
      deep: true,
    },
    search: function () {
      if (this.search == null) {
        this.getTecnicosFromApi();
      }
    },
  },
  mounted() {
    this.getTecnicosFromApi();
  },
  methods: {
    getTecnicosFromApi() {
      this.loading = true;
      const url = api.getUrl(
        "taller",
        `${this.entity}?order=${this.options.order}&typeOrder=${this.options.sort}&page=${this.options.page}&itemsPerPage=${this.options.itemsPerPage}`
      );
      this.axios
        .get(url)
        .then((response) => {
          this.items = response.data.result;
          this.totalItems = response.data.totalItems;
          this.loading = false;
        })
        .catch((e) => {
          this.errors.push(e);
          vm.$snotify.error(error.response.data);
        });
    },
    searchMethod() {
      this.loading = true;
      const url = api.getUrl(
        "taller",
        `${this.entity}?&search=${
          this.search
        }&order=${"Id"}&typeOrder=${"ASC"}&page=${1}&itemsPerPage=${10}`
      );
      this.axios
        .get(url)
        .then((response) => {
          this.items = response.data.result;
          this.loading = false;
        })
        .catch((e) => {
          this.errors.push(e);
          vm.$snotify.error(error.response.data);
        });
    },
    addItem() {
      this.dialogEdit = true;
    },
    editItem(item) {
      this.editedIndex = item.id;
      this.item = item;
      this.dialogEdit = true;
    },
    close() {
      this.dialogEdit = false;
      this.dialogDetails = false;
      this.editedIndex = -1;
      this.item = {};
    },
  },
};
</script>

<style></style>
