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
        <v-dialog v-model="dialog" persistent transition="dialog-bottom-transition" flat>
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark class="mb-2" v-on="on">Nueva Apertura</v-btn>
          </template>
          <v-card>
            <v-row justify="center pa-5">
              <v-card-title>
                <h2>
                  <strong>{{ formTitle }}</strong>
                </h2>
              </v-card-title>
            </v-row>
            <v-card-actions>
              <div class="flex-grow-1"></div>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>

        <!-- Cerrar Apertura -->
        <v-dialog v-model="dialog1" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="dialog1 = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <div class="pa-5">
              <v-card-title class="headline text-center">Estas Seguro de Cerrar la Aperura de Fecha </v-card-title>
              <v-card-text class="text-center"><strong>{{aperturaSocio.fecha}}</strong></v-card-text>
            </div>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(item)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Cerrar Apertura -->
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
          <v-icon small class="mr-2" v-on="on" @click="confCerrarApertura(item)">mdi-close-outline</v-icon>
        </template>
        <span>Cerrar Apertura</span>
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
    dialog1: false,
    search: "",
    editedIndex: -1,
    aperturaSocios: [],
    aperturaSocio: {},
    errors: [],
    headers: [
      { text: "Fecha", value: "fecha" },
      { text: "# Acuerdo Asamblea", value: "numeroAcuerdo" },
      { text: "Cantidad de Socios", value: "cantTrabajadores" },
      { text: "EstadoOrden", value: "estado" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nueva Apertura de Socios"
        : "Editar Apertura de Socios";
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
      this.dialog1 = false;
      setTimeout(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    },
    confCerrarApertura(item) {
      this.aperturaSocio = Object.assign({}, item);
      this.dialog1 = true;
    }
  }
};
</script>