<template>
  <v-data-table
    :headers="headers"
    :items="EspecialistasExternos"
    :search="search"
    class="elevation-1 pa-5"
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Listado de Especialistas</v-toolbar-title>
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
        <!-- Agregar y Editar Especialista -->
        <v-dialog v-model="dialog" persistent max-width="500">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on">Nuevo Especialista</v-btn>
          </template>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>{{ formTitle }}</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click=" close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-form ref="form" v-model="valid" lazy-validation>
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      label="Nombre"
                      v-model="EspecialistaExterno.nombre"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      label="Cargo"
                      v-model="EspecialistaExterno.cargo"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-form>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Agregar y Editar Especialista -->
        <!-- Delete Especialista -->
        <v-dialog v-model="dialog2" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="dialog2 = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas eliminar al Especialista Externo</v-card-title>
            <v-card-text class="text-center">{{EspecialistaExterno.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(EspecialistaExterno)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Especialista -->
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
          <v-icon small class="mr-2" v-on="on" @click="confirmDelete(item)">mdi-trash-can</v-icon>
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
    dialog2: false,
    search: "",
    editedIndex: -1,
    EspecialistasExternos: [],
    EspecialistaExterno: {},
    tabs: null,
    errors: [],
    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "nombre"
      },
      { text: "Cargo", value: "cargo" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nuevo Especialista"
        : "Editar Especialista";
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
    this.getEspecialistasExternosFromApi();
  },

  methods: {
    getEspecialistasExternosFromApi() {
      const url = api.getUrl("contratacion", "EspecialistasExternos");
      this.axios.get(url).then(
        response => {
          this.EspecialistasExternos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    editItem(item) {
      this.editedIndex = this.EspecialistasExternos.indexOf(item);
      this.EspecialistaExterno = Object.assign({}, item);
      this.dialog = true;
    },

    save(method) {
      const url = api.getUrl("contratacion", "EspecialistasExternos");
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          this.axios.post(url, this.EspecialistaExterno).then(
            response => {
              this.getResponse(response);
              this.getEspecialistasExternosFromApi();
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
        }
      }
      if (method === "PUT") {
        this.axios
          .put(
            `${url}/${this.EspecialistaExterno.id}`,
            this.EspecialistaExterno
          )
          .then(
            response => {
              this.getResponse(response);
              this.getEspecialistasExternosFromApi();
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
      }
    },
    confirmDelete(item) {
      this.EspecialistaExterno = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(EspecialistaExterno) {
      const url = api.getUrl("contratacion", "EspecialistasExternos");
      this.axios.delete(`${url}/${EspecialistaExterno.id}`).then(
        response => {
          this.getResponse(response);
          this.getEspecialistasExternosFromApi();
          this.dialog2 = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    close() {
      this.dialog = false;
      this.dialog2 = false;
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    }
  }
};
</script>