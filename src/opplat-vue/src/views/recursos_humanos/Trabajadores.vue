<template>
  <v-container>
    <v-data-table :headers="headers" :items="trabajadores" :search="search" class="elevation-1">
      <template v-slot:top>
        <v-row>
          <v-col cols="6" sm="2" md="2">
            <v-layout>
              <v-dialog v-model="dialog" persistent max-width="600px">
                <template v-slot:activator="{ on }">
                  <div class="mx-2 pl-2">
                    <v-btn color="primary" dark v-on="on">Filtrar por</v-btn>
                  </div>
                </template>
                <v-card>
                  <v-card-title>
                    <span class="headline">Mostrar Trabajadores por:</span>
                  </v-card-title>
                  <v-card-text>
                    <v-container grid-list-md>
                      <v-layout wrap>
                        <v-flex xs12 sm6 md6>
                          <v-autocomplete
                            v-model="unidadOrganizativa"
                            item-text="nombre"
                            return-object
                            :items="unidadOrganizativa"
                            :filter="activeFilter"
                            cache-items
                            clearable
                            placeholder="Unidad Organizativa"
                            prepend-icon="mdi-database-search"
                          ></v-autocomplete>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-autocomplete
                            v-model="cargo"
                            item-text="nombre"
                            return-object
                            :items="cargos"
                            :filter="activeFilter"
                            cache-items
                            clearable
                            placeholder="Cargo"
                            prepend-icon="mdi-database-search"
                          ></v-autocomplete>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-autocomplete
                            v-model="sexo"
                            item-text="nombre"
                            return-object
                            :items="sexo"
                            :filter="activeFilter"
                            cache-items
                            clearable
                            placeholder="Sexo"
                            prepend-icon="mdi-database-search"
                          ></v-autocomplete>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-autocomplete
                            v-model="colordePiel"
                            item-text="nombre"
                            return-object
                            :items="colordePiel"
                            :filter="activeFilter"
                            cache-items
                            clearable
                            placeholder="Color de Piel"
                            prepend-icon="mdi-database-search"
                          ></v-autocomplete>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-autocomplete
                            v-model="nivelEscolaridad"
                            item-text="nombre"
                            return-object
                            :items="nivelEscolaridad"
                            :filter="activeFilter"
                            cache-items
                            clearable
                            placeholder="Nivel de Escolaridad"
                            prepend-icon="mdi-database-search"
                          ></v-autocomplete>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-autocomplete
                            v-model="nivelEscolaridad"
                            item-text="nombre"
                            return-object
                            :items="nivelEscolaridad"
                            :filter="activeFilter"
                            cache-items
                            clearable
                            placeholder="Estado"
                            prepend-icon="mdi-database-search"
                          ></v-autocomplete>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            v-model="edad"
                            item-text="Edad"
                            return-object
                            cache-items
                            clearable
                            placeholder="Edad"
                          ></v-text-field>
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                  <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
                    <v-btn color="blue darken-1" text @click="close">Cancelar</v-btn>
                  </v-card-actions>
                </v-card>
              </v-dialog>
            </v-layout>
          </v-col>
        </v-row>
        <v-divider class="d-print-none"></v-divider>
        <v-toolbar flat color="white">
          <v-toolbar-title>Listado de Trabajadores</v-toolbar-title>
          <v-divider class="mx-2" inset vertical></v-divider>
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            append-icon="mdi-magnify"
            label="Buscar"
            single-line
            hide-details
          ></v-text-field>
          <v-spacer></v-spacer>
          <!-- <v-layout>
            <v-dialog v-model="dialog" persistent max-width="600px">
              <template v-slot:activator="{ on }">
                <v-btn color="primary" dark v-on="on">Agregar Trabajador</v-btn>
              </template>
              <v-card>
                <v-card-title>
                  <span class="headline">{{ formTitle }}</span>
                </v-card-title>
                <v-card-text>
                  <v-container grid-list-md>
                    <v-layout wrap>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="Nombre" required v-model="trabajador.nombre"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="Apellidos" required v-model="trabajador.nombre"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="CI" v-model="trabajador.ci"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="Teléfono Fijo" v-model="trabajador.cargo"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="Teléfono Móvil" v-model="trabajador.cargo"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="Sexo" v-model="trabajador.cargo"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="Dirección" v-model="trabajador.cargo"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="Municipio" v-model="trabajador.cargo"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="NIvel de Escolaridad" v-model="trabajador.cargo"></v-text-field>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="blue darken-1" text @click="close">Cerrar</v-btn>
                  <v-btn color="green darken-1" text @click="save(method)">Guardar</v-btn>
                </v-card-actions>
              </v-card>
            </v-dialog>
          </v-layout>-->
        </v-toolbar>
      </template>
      <!-- <template v-slot:item.action="{ item }">
        <v-icon small class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
        <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
      </template>-->
    </v-data-table>
  </v-container>
</template>

<script>
import api from "@/api";
export default {
  data: () => ({
    dialog: false,
    search: "",
    trabajador: {
      id: "",
      nombre: ""
    },
    defaultItem: {
      id: "",
      nombre: ""
    },
    trabajadores: [],
    unidadOrganizativa: [],
    cargos: [],
    sexo: [],
    colordePiel: [],
    nivelEscolaridad: [],
    errors: [],
    headers: [
      {
        text: "Nombre y Apellidos",
        align: "left",
        sortable: true,
        value: "nombre_Completo"
      },
      { text: "Carnet de Identidad", value: "ci" },
      { text: "Dirección", value: "direccion" },
      { text: "Municipio y Provincia", value: "municipioProv" },
      { text: "Teléfono Fijo", value: "telefonoFijo" },
      { text: "Teléfono Móvil", value: "telefonoMovil" },
      { text: "Unidad Organizativa", value: "unidadOrganizativa" },
      { text: "Cargo", value: "cargo", sortable: false }
      // { text: "Actions", value: "action", sortable: false }
    ],
    editedIndex: -1
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nuevo Trabajador" : "Editar Trabajador";
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
    this.initialize();
    this.initialize1();
    this.initialize2();
    this.initialize3();
    this.initialize4();
    this.initialize5();
  },

  methods: {
    initialize() {
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      this.axios.get(url).then(
        response => {
          this.trabajadores = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    initialize1() {
      const url = api.getUrl("recursos_humanos", "UnidadOrganizativa");
      this.axios.get(url).then(
        response => {
          this.unidadOrganizativa = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    initialize2() {
      const url = api.getUrl("recursos_humanos", "Cargos");
      this.axios.get(url).then(
        response => {
          this.cargos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    initialize3() {
      const url = api.getUrl("recursos_humanos", "CaracteristicasTrab/Sexo");
      this.axios.get(url).then(
        response => {
          this.sexo = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    initialize4() {
      const url = api.getUrl(
        "recursos_humanos",
        "CaracteristicasTrab/ColordePiel"
      );
      this.axios.get(url).then(
        response => {
          this.colordePiel = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    initialize5() {
      const url = api.getUrl(
        "recursos_humanos",
        "CaracteristicasTrab/nivelEscolaridad"
      );
      this.axios.get(url).then(
        response => {
          this.nivelEscolaridad = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },

    editItem(item) {
      this.editedIndex = this.trabajadores.indexOf(item);
      this.trabajador = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      const index = this.trabajadores.indexOf(item);
      const url = api.getUrl("recursos_humanos", "Trabajadores");

      confirm("¿Está seguro de eliminar este trabajador?") &&
        this.axios.delete(url + "/" + item.id).then(
          response => {
            this.getResponse(response);
          },
          error => {
            console.log(error);
          }
        );
    },

    close() {
      this.dialog = false;
      setTimeout(() => {
        this.trabajador = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    },

    save(method) {
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      if (method === "POST") {
        this.axios.post(url, this.trabajador).then(
          response => {
            this.getResponse(response);
          },
          error => {
            console.log(error);
          }
        );
      }
      if (method === "PUT") {
        this.axios.put(url + "/" + this.trabajador.id, this.trabajador).then(
          response => {
            this.getResponse(response);
          },
          error => {
            console.log(error);
          }
        );
      }
      this.close();
    },
    getResponse: function(response) {
      if (response.status === 200) {
        this.dialog = false;
        this.trabajador.nombre = "";
        this.trabajador.codigo = "";
        vm.$snotify.success("Exito al realizar la operación");
        this.initialize();
      }
    }
  }
};
</script>
