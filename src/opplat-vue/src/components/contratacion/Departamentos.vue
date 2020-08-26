<template>
  <v-data-table :headers="headers" :items="departamentos" :search="search" class="elevation-1 pa-5">
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Departamentos</v-toolbar-title>
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
        <template>
          <v-btn color="primary" @click="newDepartamento()">Nuevo Departamento</v-btn>
        </template>
        <!-- Agregar y Editar Departamento -->
        <v-dialog v-model="dialog" persistent max-width="450">
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
            <v-form ref="form">
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs12 class="px-3">
                    <v-text-field label="Nombre" v-model="departamento.nombre" clearable required></v-text-field>
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
        <!-- /Agregar y Editar Departamento -->
        <!-- Delete Departamento -->
        <v-dialog v-model="dialog2" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="close()">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas eliminar el Departamento</v-card-title>
            <v-card-text class="text-center">{{departamento.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(departamento)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Departamento -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small primary--text"
        small
        @click="editItem(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-pen theme--dark</v-icon>
      </v-btn>
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small pink--text"
        small
        @click="confirmDelete(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-delete theme--dark</v-icon>
      </v-btn>
   
    </template>
  </v-data-table>
</template>
<script>
import api from "@/api";

export default {
  data: () => ({
    dialog: false,
    dialog2: false,
    departamentos: [],
    departamento: {},
    editedIndex: -1,
    errors: [],

    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "nombre"
      },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nuevo Departamento"
        : "Editar Departamento";
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
    this.getDepartamentosFromApi();
  },
  methods: {
    getDepartamentosFromApi() {
      const url = api.getUrl("contratacion", "Departamentos");
      this.axios.get(url).then(
        response => {
          this.departamentos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    newDepartamento() {
      this.dialog = true;
    },
    editItem(item) {
      this.editedIndex = this.departamentos.indexOf(item);
      this.departamento = Object.assign({}, item);
      this.dialog = true;
    },
    save(method) {
      const url = api.getUrl("contratacion", "Departamentos");
      if (method === "POST") {
        this.axios.post(url, this.departamento).then(
          response => {
            this.getResponse(response);
            this.getDepartamentosFromApi();
            this.departamento = [];
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
      if (method === "PUT") {
        this.axios
          .put(`${url}/${this.departamento.id}`, this.departamento)
          .then(
            response => {
              this.getResponse(response);
              this.getDepartamentosFromApi();
              this.departamento = {};
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
      }
    },
    confirmDelete(item) {
      this.departamento = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(departamento) {
      const url = api.getUrl("contratacion", "Departamentos");
      this.axios.delete(`${url}/${departamento.id}`).then(
        response => {
          this.getResponse(response);
          this.getDepartamentosFromApi();
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
      this.departamento = {};
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
