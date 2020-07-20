<template>
  <v-data-table
    :headers="headers"
    :items="EspecialistasExternos"
    :search="search"
    class="elevation-1 pa-5"
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Especialistas Externos</v-toolbar-title>
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
          <v-btn color="primary" @click="newEspecialista()">Nuevo Especialista</v-btn>
        </template>
        <!-- Agregar y Editar Especialista -->
        <v-dialog v-model="dialog" persistent max-width="700">
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
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      label="Nombre"
                      v-model="especialistaExterno.nombre"
                      :rules="NombreRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      label="Apellidos"
                      v-model="especialistaExterno.apellidos"
                      :rules="ApellidosRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      label="Cargo"
                      v-model="especialistaExterno.cargo"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-3">
                    <v-autocomplete
                      v-model="especialistaExterno.entidadId"
                      item-text="nombre"
                      item-value="id"
                      :items="entidades"
                      :rules="EntidadRules"
                      cache-items
                      label="Entidad a la que Pertenece"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      label="Área"
                      v-model="especialistaExterno.area"
                      :rules="AreaRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      label="Departamento"
                      v-model="especialistaExterno.departamento"
                      :rules="DepartamentoRules"
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
            <v-card-title
              class="headline text-center"
            >Seguro que deseas eliminar al Especialista Externo</v-card-title>
            <v-card-text class="text-center">{{especialistaExterno.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(especialistaExterno)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Especialista -->
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
    dialog1: false,
    dialog2: false,
    search: "",
    editedIndex: -1,
    EspecialistasExternos: [],
    especialistaExterno: {},
    tabs: null,
    entidades: [],
    errors: [],
    NombreRules: [v => !!v || "El Nombre es Requerido"],
    ApellidosRules: [v => !!v || "Los Apellidos son Requeridos"],
    EntidadRules: [v => !!v || "El Proveedor es Requerido"],
    AreaRules: [v => !!v || "El Área es Requerida"],
    DepartamentoRules: [v => !!v || "El Departamento es Requerido"],
    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "nombreCompleto"
      },
      { text: "Entidad Proveedora a la Pertenece", value: "entidad.nombre" },
      { text: "Departamento", value: "departamento" },
      { text: "Área", value: "area" },
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
    this.getEntidadesFromApi();
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
    getEntidadesFromApi() {
      const url = api.getUrl("contratacion", "Entidades");
      this.axios.get(url).then(
        response => {
          this.entidades = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    editItem(item) {
      this.editedIndex = this.EspecialistasExternos.indexOf(item);
      this.especialistaExterno = Object.assign({}, item);
      this.dialog = true;
    },
    save(method) {
      const url = api.getUrl("contratacion", "EspecialistasExternos");
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          this.snackbar = true;
        }
        if (
          this.especialistaExterno.nombre == null ||
          this.especialistaExterno.apellidos == null ||
          this.especialistaExterno.entidadId == null
        ) {
          vm.$snotify.error("Faltan campos por llenar que son obligatorios");
        } else if (
          this.especialistaExterno.area == null &&
          this.especialistaExterno.departamento == null
        ) {
          vm.$snotify.error("Debe llenar Área o Departamento");
        } else {
          this.axios.post(url, this.especialistaExterno).then(
            response => {
              this.getResponse(response);
              this.getEspecialistasExternosFromApi();
              this.especialistaExterno = {};
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
            `${url}/${this.especialistaExterno.id}`,
            this.especialistaExterno
          )
          .then(
            response => {
              this.getResponse(response);
              this.getEspecialistasExternosFromApi();
              this.especialistaExterno = {};
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
      }
    },
    confirmDelete(item) {
      this.especialistaExterno = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(especialistaExterno) {
      const url = api.getUrl("contratacion", "EspecialistasExternos");
      this.axios.delete(`${url}/${especialistaExterno.id}`).then(
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
    newEspecialista() {
      this.dialog = true;
      this.getEntidadesFromApi();
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    }
  }
};
</script>