<template>
  <v-container>
    <v-data-table :headers="headers" :items="trabajadores" :search="search" class="elevation-1">
      <template v-slot:top>
        <v-row>
          <v-col cols="6" sm="2" md="2">
            <v-layout>
              <v-dialog v-model="dialog" persistent max-width="800px">
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
                            :items="unidadesOrganizativas"
                            :filter="activeFilter"
                            cache-items
                            clearable
                            placeholder="Unidad Organizativa"
                            prepend-icon="mdi-database-search"
                            chips
                            allow-overflow
                          ></v-autocomplete>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-autocomplete
                            v-model="cargo"
                            item-text="nombre"
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
                            :items="sexos"
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
                            :items="coloresdePiel"
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
                            :items="nivelesEscolaridad"
                            :filter="activeFilter"
                            cache-items
                            clearable
                            placeholder="Nivel de Escolaridad"
                            prepend-icon="mdi-database-search"
                          ></v-autocomplete>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-autocomplete
                            v-model="estado"
                            item-text="nombre"
                            :items="estados"
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
                            cache-items
                            clearable
                            placeholder="Edad"
                            prepend-icon="mdi-database-search"
                          ></v-text-field>
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                  <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="green darken-1" text @click="getFiltrosFromApi">Aceptar</v-btn>
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
      <template v-slot:item.action="{ item }">
        <v-row justify="center">
          <v-dialog v-model="dialog1" hide-overlay transition="dialog-bottom-transition" flat>
            <template v-slot:activator="{ on }">
              <v-icon small class="mr-2" @click="getDetallesTrabFromApi(item)" v-on="on">mdi-account-plus</v-icon>
            </template>
            <v-card>
              <v-toolbar dark fadeOnScroll color="#1F7087">
                <v-flex xs12 sm10 md6 lg4>Detalles del Trabajador</v-flex>
                <v-spacer></v-spacer>
                <v-toolbar-items>
                  <v-btn icon dark @click="dialog1 = false">
                    <v-icon>mdi-close</v-icon>
                  </v-btn>
                </v-toolbar-items>
              </v-toolbar>

              <v-container fluid>
                <v-row dense>
                  <v-col cols="8">
                    <v-card flat v-for="item in trabajadorById" :key="item.ci">
                      <v-layout justify-center>
                        <v-layout class="pa-2">
                          <v-list-item two-line>
                            <v-list-item-content>
                              <v-list-item-title>
                                <strong>Unidad Organizativa:</strong>
                              </v-list-item-title>
                              <v-list-item-subtitle>{{item.unidadOrganizativa}}</v-list-item-subtitle>
                            </v-list-item-content>
                          </v-list-item>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-list-item two-line>
                            <v-list-item-content>
                              <v-list-item-title>
                                <strong>Cargo:</strong>
                              </v-list-item-title>
                              <v-list-item-subtitle>{{item.cargo}}</v-list-item-subtitle>
                            </v-list-item-content>
                          </v-list-item>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-list-item two-line>
                            <v-list-item-content>
                              <v-list-item-title>
                                <strong>Estado:</strong>
                              </v-list-item-title>
                              <v-list-item-subtitle>{{item.estadoTrabajador}}</v-list-item-subtitle>
                            </v-list-item-content>
                          </v-list-item>
                        </v-layout>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-list-item two-line>
                          <v-list-item-content>
                            <v-list-item-title>
                              <strong>Funciones o Tareas Principales del Cargo:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle
                              v-for="item in funciones"
                              :key="item.text"
                            >- {{item.text}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-list-item two-line>
                          <v-list-item-content>
                            <v-list-item-title>
                              <strong>Requisitos de Conocimiento para el Cargo:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle
                              v-for="item in requisitos"
                              :key="item.text"
                            >- {{item.text}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                    </v-card>
                  </v-col>

                  <v-col cols="4">
                    <v-card color="#1F7087" dark>
                      <v-list-item>
                        <v-layout column align-center xs12 sm10 md6 lg4>
                          <v-avatar size="120">
                            <img src="img/default-avatar.png" class="float-center pa-5" />
                          </v-avatar>
                          <v-layout class="pa-2">
                            <v-toolbar-title class="text-capitalize">{{item.nombre_Completo}}</v-toolbar-title>
                          </v-layout>
                        </v-layout>
                      </v-list-item>
                      <v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-list-item-title>Carnet de Identidad: {{item.ci}}</v-list-item-title>
                          </v-list-item-content>
                        </v-layout>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-toolbar-items class="text-capitalize">Direccion: {{item.direccion}}</v-toolbar-items>
                      </v-layout>
                      <v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-toolbar-items>Sexo: {{item.sexo}}</v-toolbar-items>
                          </v-list-item-content>
                        </v-layout>
                        <v-list-item-content>
                          <v-list-item-title>
                            <v-list-item-title>Correo: {{item.correo}}</v-list-item-title>
                          </v-list-item-title>
                        </v-list-item-content>
                      </v-layout>
                      <v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-list-item-title>Teléfono Fijo: {{item.telefonoFijo}}</v-list-item-title>
                          </v-list-item-content>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-list-item-title>Teléfono Movil: {{item.telefonoMovil}}</v-list-item-title>
                          </v-list-item-content>
                        </v-layout>
                      </v-layout>
                      <v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-toolbar-items>Nivel de Escolaridad: {{item.nivelEscolaridad}}</v-toolbar-items>
                          </v-list-item-content>
                        </v-layout>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-list-item-content>
                          <v-list-item-title>
                            <v-list-item-title>Talla de Pantalón: {{item.tallaPantalon}}</v-list-item-title>
                          </v-list-item-title>
                        </v-list-item-content>
                      </v-layout>
                      <v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-list-item-title>Color de Ojos: {{item.colorDeOjos}}</v-list-item-title>
                          </v-list-item-content>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-list-item-title>
                              <v-toolbar-items>Color de Piel: {{item.colorDePiel}}</v-toolbar-items>
                            </v-list-item-title>
                          </v-list-item-content>
                        </v-layout>
                      </v-layout>
                      <v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-list-item-title>Talla de Calzado: {{item.tallaCalzado}}</v-list-item-title>
                          </v-list-item-content>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-list-item-content>
                            <v-list-item-title>
                              <v-toolbar-items>Talla de Camisa: {{item.tallaDeCamisa}}</v-toolbar-items>
                            </v-list-item-title>
                          </v-list-item-content>
                        </v-layout>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-toolbar-items>Otras Caracteristicas: {{item.otrasCaracteristicas}}</v-toolbar-items>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-toolbar-items>Resumen: {{item.resumen}}</v-toolbar-items>
                      </v-layout>
                    </v-card>
                  </v-col>
                </v-row>
              </v-container>
            </v-card>
          </v-dialog>
        </v-row>
      </template>
    </v-data-table>
  </v-container>
</template>
        
<script>
import api from "@/api";
export default {
  data: () => ({
    dialog: false,
    dialog1: false,
    search: "",
    trabajador: "",
    unidadOrganizativa: "",
    edad: "",
    cargo: "",
    colordePiel: "",
    sexo: "",
    nivelEscolaridad: "",
    estado: "",
    defaultItem: {
      id: "",
      nombre: ""
    },
    trabajadores: [],
    trabajadorById: [],
    unidadesOrganizativas: [],
    cargos: [],
    sexos: [],
    coloresdePiel: [],
    nivelesEscolaridad: [],
    estados: [],
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
      // { text: "Municipio y Provincia", value: "municipioProv" },
      // { text: "Teléfono Fijo", value: "telefonoFijo" },
      // { text: "Teléfono Móvil", value: "telefonoMovil" },
      { text: "Sexo", value: "sexo" },
      { text: "NivelDeEscolaridad", value: "nivelDeEscolaridad" },
      // { text: "Unidad Organizativa", value: "unidadOrganizativa" },
      // { text: "Cargo", value: "cargo", sortable: false }
      { text: "Acciones", value: "action", sortable: false }
    ],
    funciones: [
      {
        text:
          "Participa en el establecimiento de las distintas fuentes de información"
      },
      {
        text: "Participa en la elaboración de los conceptos"
      },
      {
        text: "Participa en las acciones de capacitación"
      }
    ],
    requisitos: [
      {
        text: "Graduado de Nivel Medio Superior con entrenamiento en el puesto"
      }
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
    },
    dialog1(val) {
      val || this.close();
    }
  },

  created() {
    this.getTrabajadoresFromApi();
    this.getUnidadOrganizativaFromApi();
    this.getCargosFromApi();
    this.getSexoTrabFromApi();
    this.getColordePielTrabFromApi();
    this.getnivelEscolaridadTrabFromApi();
    this.getEstadosTrabFromApi();
  },

  methods: {
    getTrabajadoresFromApi() {
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
    getDetallesTrabFromApi(item) {
      this.dialog1 = true;
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      this.axios.get(url + "/" + item.id).then(
        response => {
          this.trabajadorById = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    closeDetalles() {
      this.dialog1 = false;
    },
    getUnidadOrganizativaFromApi() {
      const url = api.getUrl("recursos_humanos", "UnidadOrganizativa");
      this.axios.get(url).then(
        response => {
          this.unidadesOrganizativas = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getCargosFromApi() {
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
    getSexoTrabFromApi() {
      const url = api.getUrl("recursos_humanos", "CaracteristicasTrab/Sexo");
      this.axios.get(url).then(
        response => {
          this.sexos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getColordePielTrabFromApi() {
      const url = api.getUrl(
        "recursos_humanos",
        "CaracteristicasTrab/ColordePiel"
      );
      this.axios.get(url).then(
        response => {
          this.coloresdePiel = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getnivelEscolaridadTrabFromApi() {
      const url = api.getUrl(
        "recursos_humanos",
        "CaracteristicasTrab/nivelEscolaridad"
      );
      this.axios.get(url).then(
        response => {
          this.nivelesEscolaridad = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getEstadosTrabFromApi() {
      const url = api.getUrl("recursos_humanos", "CaracteristicasTrab/Estados");
      this.axios.get(url).then(
        response => {
          this.estados = response.data;
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
    },

    closeFiltro() {
      this.dialog = false;
    },

    getFiltrosFromApi() {
      const url = api.getUrl("recursos_humanos", "Trabajadores/Filtro");
      this.axios
        .get(url, {
          params: {
            unidadOrganizativa: this.unidadOrganizativa,
            cargo: this.cargo,
            sexo: this.sexo,
            estado: this.estado,
            nivelEscolaridad: this.nivelEscolaridad,
            edad: this.edad,
            colordePiel: this.colordePiel
          }
        })
        .then(
          response => {
            this.trabajadores = response.data;
            this.closeFiltro();
          },
          error => {
            console.log(error);
          }
        );
    }
  }
};
</script>