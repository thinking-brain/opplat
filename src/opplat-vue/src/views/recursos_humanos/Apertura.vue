<template>
  <v-data-table :headers="headers" :items="aperturaSocios" sort-by="fecha" class="elevation-1 pa-5">
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Listado de Apertura de Socios</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
         <v-spacer></v-spacer>
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          label="Buscar"
          single-line
          hide-details
          clearable
          dense
        ></v-text-field>
        <v-spacer></v-spacer>
        <div class="flex-grow-1"></div>
        <v-dialog v-model="dialog" max-width="1000px">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark class="mb-2" v-on="on">Nueva Apertura</v-btn>
          </template>
          <v-card>
            <v-card-title>
              <span class="headline">{{ formTitle }}</span>
            </v-card-title>

            <v-card-text>
              <v-container>
                <v-row>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field v-model="aperturaSocio.fecha" label="Fecha"></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field v-model="aperturaSocio.cantTrabajadores" label="Cant Trabajadores"></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field v-model="aperturaSocio.perfil_Ocupacional" label="Perfil Ocupacional"></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field v-model="aperturaSocio.numeroAcuerdo" label="Número Acuerdo"></v-text-field>
                  </v-col>
                </v-row>
              </v-container>
            </v-card-text>

            <v-card-actions>
              <div class="flex-grow-1"></div>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
       <v-tooltip top>
      <template v-slot:activator="{ on }">
        <v-icon small class="mr-2" v-on="on" @click="editItem(item)">mdi-pencil</v-icon>
      </template>
      <span>Editar</span>
    </v-tooltip>
       <v-tooltip top>
      <template v-slot:activator="{ on }">
      <v-icon small class="mr-2" v-on="on" @click="deleteItem(item)">mdi-delete</v-icon>
      </template>
      <span>Eliminar</span>
    </v-tooltip>
    </template>
  </v-data-table>
</template>
<script>
import api from "@/api";
export default {
  data: () => ({
    dialog: false,
    search: "",
    editedIndex: -1,
    aperturaSocios: [],
    aperturaSocio: {},
    errors: [],
    headers: [
      { text: "Fecha", value: "fecha" },
      { text: "Cantidad de Trabajadores", value: "cantTrabajadores" },
      { text: "Perfil Ocupacional", value: "perfil_Ocupacional" },
      { text: "Número de Acuerdo", value: "numeroAcuerdo" },
      { text: "Cerrada", value: "F63E" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nueva Apertura" : "Editar Apertura";
    },
    method() {
      return this.editedIndex === -1 ? "POST" : "PUT";
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {
    this.getAperturaSociosFromApi();
  },

  methods: {
    getAperturaSociosFromApi() {
      const url = api.getUrl("recursos_humanos", "AperturaSocios");
      this.axios.get(url).then(
        response => {
          this.aperturaSocios = response.data;
          this.volver = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    editItem(item) {
      this.editedIndex = this.aperturaSocios.indexOf(item);
      this.aperturaSocio = Object.assign({}, item);
      this.dialog = true;
    },
    close() {
      this.dialog = false;
      setTimeout(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    }
  }
};
</script>