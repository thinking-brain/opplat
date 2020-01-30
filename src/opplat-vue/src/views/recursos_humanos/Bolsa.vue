<template>
  <v-data-table :headers="headers" :items="trabajadores" :search="search" class="elevation-1 pa-5">
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Bolsa de Trabajadores</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          label="Buscar"
          single-line
          hide-details
          clearable
        ></v-text-field>
        <v-spacer></v-spacer>

        <!-- Agregar y Editar Trabajador -->
        <v-dialog v-model="dialog" persistent max-width="1000px">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on">Agregar Trabajador</v-btn>
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
                  <v-flex xs6 class="px-5">
                    <v-text-field
                      label="Nombre"
                      v-model="trabajador.nombre"
                      :rules="NombreRules"
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-5">
                    <v-text-field
                      label="Apellidos"
                      v-model="trabajador.apellidos"
                      :rules="ApellidosRules"
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-5">
                    <v-text-field
                      label="Carnet de Identidad"
                      v-model="trabajador.ci"
                      :counter="11"
                      :rules="ciRules"
                      required
                    ></v-text-field>
                    <span asp-validation-for="CI" class="text-danger"></span>
                  </v-flex>
                  <!-- <v-flex xs6 class="px-5">
                    <v-select
                      v-model="trabajador.sexo"
                      :items="sexos"
                      item-text="nombre"
                      item-value="id"
                      label="Sexo"
                    ></v-select>
                  </v-flex> -->
                  <v-flex xs12 class="pa-5">
                    <v-text-field label="Dirección" v-model="trabajador.direccion"></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs4 class="px-5">
                    <v-select
                      v-model="trabajador.nivelDeEscolaridad"
                      item-text="nombre"
                      item-value="id"
                      :items="nivelesEscolaridad"
                      label="Nivel de Escolaridad"
                    ></v-select>
                  </v-flex>
                  <v-flex xs4 class="px-5">
                    <v-text-field
                      label="Perfil Ocupacional"
                      v-model="trabajador.perfil_Ocupacional"
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs4 class="px-5">
                    <v-text-field label="Teléfono Móvil" v-model="trabajador.telefonoMovil"></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs4 class="px-5">
                    <v-text-field label="Teléfono Fijo" v-model="trabajador.telefonoFijo"></v-text-field>
                  </v-flex>
                  <v-flex xs4 class="px-5">
                    <v-text-field label="Correo" v-model="trabajador.correo" :rules="emailRules"></v-text-field>
                  </v-flex>
                  <v-flex xs4 class="px-5">
                    <v-select
                      v-model="trabajador.colorDeOjos"
                      item-text="nombre"
                      item-value="id"
                      :items="coloresdeOjos"
                      label="Color de Ojos"
                    ></v-select>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs4 class="px-5">
                    <v-select
                      v-model="trabajador.colorDePiel"
                      item-text="nombre"
                      item-value="id"
                      :items="coloresdePiel"
                      label="Color de Piel"
                    ></v-select>
                  </v-flex>
                  <v-flex xs4 class="px-5">
                    <v-text-field v-model="trabajador.tallaCalzado" label="Talla de Calzado"></v-text-field>
                  </v-flex>
                  <v-flex xs4 class="px-5">
                    <v-select
                      v-model="trabajador.tallaDeCamisa"
                      item-text="nombre"
                      item-value="id"
                      :items="tallasDeCamisas"
                      label="Talla de Camisa"
                    ></v-select>
                  </v-flex>
                  <v-flex xs4 class="px-5">
                    <v-text-field label="Talla de Pantalon" v-model="trabajador.tallaPantalon"></v-text-field>
                  </v-flex>
                  <v-flex xs8 class="px-5" v-if="editedIndex!=-1"></v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs4 class="px-5">
                    <v-switch v-model="Referencia" :label="`Tiene Referencia`"></v-switch>
                  </v-flex>
                  <v-flex xs4 class="px-5" v-if="Referencia">
                    <v-switch v-model="EsTrabEmpresa" :label="`Es Trabajador Suyo`"></v-switch>
                  </v-flex>
                  <v-flex xs4 class="px-5" v-if="Referencia && EsTrabEmpresa">
                    <v-autocomplete
                      v-model="trabajador.referencia"
                      item-text="nombre_Completo"
                      :items="trabajadoresReferencia"
                      :filter="activeFilter"
                      clearable
                      label="Nombre de la Refencia"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs4 class="px-5" v-if="Referencia && !EsTrabEmpresa">
                    <v-text-field
                      label="Nombre de la Refencia"
                      v-model="trabajador.referencia"
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-flex x12 class="px-5">
                  <v-textarea
                    solo
                    name="input-7-4"
                    label="Otras Características"
                    v-model="trabajador.otrasCaracteristicas"
                  ></v-textarea>
                </v-flex>
              </v-container>
            </v-form>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Agregar y Editar Trabajador -->

        <!-- Detalles del Trabajador -->
        <v-dialog v-model="dialog3" persistent transition="dialog-bottom-transition" flat>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles del Trabajador</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="dialog3 = false">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-container fluid>
              <v-row dense>
                <v-col cols="7">
                  <v-card flat>
                    <v-layout>
                      <v-text>
                        <strong>Su perfil Ocupacional es :</strong>
                        {{trabajador.perfil_Ocupacional}}
                      </v-text>
                    </v-layout>
                    <v-layout>
                      <v-text>
                        <strong>Ingresó a la Bolsa el :</strong>
                        {{trabajador.fecha_Entrada}}
                      </v-text>
                    </v-layout>
                    <v-layout v-if="trabajador.referencia!=null">
                      <v-text >
                        <strong>Es recomendado por :</strong>
                        {{trabajador.referencia}}
                      </v-text>
                    </v-layout>
                  </v-card>
                </v-col>
                <v-col cols="5">
                  <v-card color="blue darken-3" dark>
                    <v-list-item>
                      <v-layout column align-center xs12 sm10 md6 lg4>
                        <v-avatar size="120" v-if="trabajador.sexoName==='M'">
                          <img src="img/default-avatar-man.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="120" v-else-if="trabajador.sexoName==='F'">
                          <img src="img/default-avatar-woman.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="110" v-else>
                          <img src="img/default-avatar.png" class="float-center pa-5" dark />
                        </v-avatar>
                        <v-layout class="pa-2">
                          <v-toolbar-title class="text-capitalize">{{trabajador.nombre_Completo}}</v-toolbar-title>
                        </v-layout>
                      </v-layout>
                    </v-list-item>
                    <v-row dense>
                      <v-col cols="7">
                        <v-layout class="pa-2">
                          <v-text>Carnet de Identidad: {{trabajador.ci}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Nivel de Escolaridad: {{trabajador.nivelDeEscolaridadName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Correo: {{trabajador.correo}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Color de Ojos: {{trabajador.colorDeOjosName}}</v-text>
                        </v-layout>
                      </v-col>
                      <v-col cols="5">
                        <v-layout class="pa-2">
                          <v-text>Sexo: {{trabajador.sexoName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Teléfono Fijo: {{trabajador.telefonoFijo}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Teléfono Movil: {{trabajador.telefonoMovil}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Color de Piel: {{trabajador.colorDePielName}}</v-text>
                        </v-layout>
                      </v-col>
                    </v-row>
                    <v-layout class="pa-2">
                      <v-text>Direccion: {{trabajador.direccion}}</v-text>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-text>Otros Datos de Interes: {{trabajador.otrasCaracteristicas}}</v-text>
                    </v-layout>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- Detalles  del Trabajador -->

        <!-- Movimientos -->
        <v-dialog v-model="dialog5" persistent transition="dialog-bottom-transition" flat>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Movimientos</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>

            <v-container fluid>
              <v-row dense>
                <v-col cols="8">
                  <v-card flat>
                    <v-toolbar flat>
                      <v-tabs slot="extension" v-model="tabs" centered>
                        <v-tab>Entrada</v-tab>
                      </v-tabs>
                    </v-toolbar>
                    <v-tabs-items v-model="tabs" py-6>
                      <v-tab-item>
                        <v-card flat>
                          <v-card-title class="headline mx-6">Asígnele un Cargo al Trabajador</v-card-title>
                          <v-card-text>
                            <form>
                              <v-container>
                                <v-row>
                                  <v-col cols="12" md="8">
                                    <v-autocomplete
                                      xs6
                                      class="pa-2"
                                      v-model="trabajador"
                                      item-text="nombre_Completo"
                                      item-value="id"
                                      :items="trabajadores"
                                      :filter="activeFilter"
                                      label="Nombre del Trabajador"
                                      disabled
                                    ></v-autocomplete>
                                  </v-col>
                                  <v-col cols="6" md="4">
                                    <v-select
                                      xs6
                                      class="pa-2"
                                      v-model="entrada.cargoId"
                                      item-text="nombre"
                                      item-value="id"
                                      :items="cargos"
                                      label="Cargo"
                                      min-width="290px"
                                      required
                                    ></v-select>
                                  </v-col>
                                </v-row>
                                <v-row>
                                  <v-col cols="6" md="4">
                                    <v-select
                                      xs6
                                      class="pa-2"
                                      v-model="entrada.unidadOrganizativaId"
                                      item-text="nombre"
                                      item-value="id"
                                      :items="unidadesOrganizativas"
                                      label="Unidad Organizativa"
                                      min-width="290px"
                                      required
                                    ></v-select>
                                  </v-col>
                                  <v-col cols="6" md="4">
                                    <v-menu
                                      v-model="menu"
                                      :close-on-content-click="false"
                                      :nudge-right="40"
                                      transition="scale-transition"
                                      offset-y
                                      full-width
                                    >
                                      <template v-slot:activator="{ on }">
                                        <v-text-field
                                          v-model="entrada.fechaEntrada"
                                          label="Fecha"
                                          readonly
                                          v-on="on"
                                          class="pa-2"
                                        ></v-text-field>
                                      </template>
                                      <v-date-picker
                                        v-model="entrada.fechaEntrada"
                                        @input="menu = false"
                                      ></v-date-picker>
                                    </v-menu>
                                  </v-col>
                                </v-row>
                                <v-btn color="green darken-1" text @click="Entrada">Aceptar</v-btn>
                                <v-btn color="blue darken-1" text @click="clear">Cancelar</v-btn>
                              </v-container>
                            </form>
                          </v-card-text>
                        </v-card>
                      </v-tab-item>
                    </v-tabs-items>
                  </v-card>
                </v-col>

                <v-col cols="4">
                  <v-card color="blue darken-3" dark>
                    <v-list-item>
                      <v-layout column align-center xs12 sm10 md6 lg4>
                        <v-avatar size="120" v-if="trabajador.sexoName==='M'">
                          <img src="img/default-avatar-man.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="120" v-else-if="trabajador.sexoName==='F'">
                          <img src="img/default-avatar-woman.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="110" v-else>
                          <img src="img/default-avatar.png" class="float-center pa-5" dark />
                        </v-avatar>
                        <v-layout class="pa-2">
                          <v-toolbar-title class="text-capitalize">{{trabajador.nombre_Completo}}</v-toolbar-title>
                        </v-layout>
                      </v-layout>
                    </v-list-item>
                    <v-layout>
                      <v-layout class="pa-2">
                        <v-list-item-content>
                          <v-list-item-title>Carnet de Identidad: {{trabajador.ci}}</v-list-item-title>
                        </v-list-item-content>
                      </v-layout>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-toolbar-items class="text-capitalize">Direccion: {{trabajador.direccion}}</v-toolbar-items>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-toolbar-items
                        class="text-capitalize"
                      >Unidad Organizativa: {{trabajador.unidadOrganizativa}}</v-toolbar-items>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-toolbar-items class="text-capitalize">Cargo: {{trabajador.cargo}}</v-toolbar-items>
                    </v-layout>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- /Movimientos -->

        <!-- Delete Trajador -->
        <v-dialog v-model="dialog4" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="dialog4 = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas eliminar al Trabajador</v-card-title>
            <v-card-text class="text-center">{{trabajador.nombre_Completo}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(trabajador)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Trajador -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-icon small class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
      <v-icon small class="mr-2" @click="getDetallesTrabFromApi(item)">mdi-account-plus</v-icon>
      <v-icon small class="mr-2" @click="movimiento(item)">mdi-walk</v-icon>
      <!-- <v-icon small @click="confirmDelete(item)">mdi-delete</v-icon> -->
    </template>
  </v-data-table>
</template>
<script>
import api from "@/api";
export default {
  data: () => ({
    dialog: false,
    dialog1: false,
    dialog3: false,
    dialog4: false,
    dialog5: false,
    dialog6: false,
    volver: false,
    valid: true,
    search: "",
    editedIndex: -1,
    trabajadores: [],
    trabajadoresReferencia: [],
    trabajador: {
      sexo: 0,
      colorDeOjos: 0,
      colorDePiel: 0,
      tallaDeCamisa: 0,
      referencia: ""
    },
    unidadesOrganizativas: [],
    cargos: [],
    sexos: [],
    coloresdePiel: [],
    coloresdeOjos: [],
    tallasDeCamisas: [],
    nivelesEscolaridad: [],
    estados: [],
    unidadOrganizativa: "",
    edad: "",
    cargo: "",
    nivelEscolaridad: "",
    otrasCaracteristicas: "",
    estado: "",
    date: new Date().toISOString().substr(0, 10),
    menu: false,
    menu1: false,
    menu2: false,
    modal: false,
    tabs: null,
    Referencia: false,
    EsTrabEmpresa: false,
    entrada: {
      trabajadorId: "",
      fechaEntrada: "",
      unidadOrganizativaId: "",
      cargoId: ""
    },
    NombreRules: [v => !!v || "El Nombre es Requerido"],
    ApellidosRules: [v => !!v || "Los Apellidos son Requerido"],
    ciRules: [
      v => !!v || "El CI es Requerido",
      v => (v && v.length <= 11) || "El CI tiene 11 Caracteres"
    ],
    emailRules: [v => /.+@.+\..+/.test(v) || "No tiene la estructura correcta"],
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
      { text: "Perfil Ocupacional", value: "perfil_Ocupacional" },
      { text: "Referencia", value: "referencia" },
      { text: "Tiempo en Bolsa", value: "tiempo_Bolsa" },
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
    ]
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
    this.getTrabajadoresBolsa();
    this.getTrabajadoresFromApi();
    this.getUnidadOrganizativaFromApi();
    this.getCargosFromApi();
    this.getSexoTrabFromApi();
    this.getColordePielTrabFromApi();
    this.getnivelEscolaridadTrabFromApi();
    this.getEstadosTrabFromApi();
    this.getColordeOjosTrabFromApi();
    this.gettallasDeCamisasFromApi();
  },

  methods: {
    getTrabajadoresBolsa() {
      const url = api.getUrl("recursos_humanos", "Trabajadores/Bolsa");
      this.axios.get(url).then(
        response => {
          this.trabajadores = response.data;
          this.volver = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    getTrabajadoresFromApi() {
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      this.axios.get(url).then(
        response => {
          this.trabajadoresReferencia = response.data;
          this.volver = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    getDetallesTrabFromApi(item) {
      this.trabajador = Object.assign({}, item);
      this.dialog3 = true;
    },

    closeDetalles() {
      this.dialog3 = false;
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
            this.dialog1 = false;
            this.volver = true;
          },
          error => {
            console.log(error);
          }
        );
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
    getColordeOjosTrabFromApi() {
      const url = api.getUrl(
        "recursos_humanos",
        "CaracteristicasTrab/ColordeOjos"
      );
      this.axios.get(url).then(
        response => {
          this.coloresdeOjos = response.data;
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
    gettallasDeCamisasFromApi() {
      const url = api.getUrl(
        "recursos_humanos",
        "CaracteristicasTrab/TallaDeCamisa"
      );
      this.axios.get(url).then(
        response => {
          this.tallasDeCamisas = response.data;
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

    Entrada() {
      const url = api.getUrl("recursos_humanos", "Entradas");
      this.axios.post(url, this.entrada).then(
        response => {
          this.getResponse(response);
          this.entrada = {
            trabajadorId: "",
            fechaEntrada: "",
            unidadOrganizativaId: "",
            cargoId: ""
          };
          this.dialog5 = false;
          this.getTrabajadoresBolsa();
          this.getTrabajadoresFromApi();
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
    save(method) {
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          this.snackbar = true;
        }
        if (
          this.trabajador.ci == null ||
          this.trabajador.nombre == null ||
          this.trabajador.apellidos == null
        ) {
          vm.$snotify.error("Faltan campos por llenar que son obligatorios");
        } else {
          this.axios.post(url, this.trabajador).then(
            response => {
              this.getResponse(response);
              this.getTrabajadoresBolsa();
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
        }
      }
      if (method === "PUT") {
        this.axios.put(url + "/" + this.trabajador.id, this.trabajador).then(
          response => {
            this.getResponse(response);
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
    },

    confirmDelete(item) {
      this.trabajador = Object.assign({}, item);
      this.dialog4 = true;
    },
    deleteItem(trabajador) {
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      this.axios.delete(url + "/" + trabajador.id).then(
        response => {
          this.getResponse(response);
        },
        error => {
          console.log(error);
        }
      );
    },
    getResponse: function(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
        this.getTrabajadoresFromApi();
        this.getTrabajadoresBolsa();
        this.trabajador = [];
      }
    },
    close() {
      this.dialog = false;
      this.dialog1 = false;
      this.dialog3 = false;
      this.dialog4 = false;
      this.dialog5 = false;
      this.dialog6 = false;
      this.trabajador = {
        colordeOjos: 0,
        colordePiel: 0,
        tallaDeCamisa: 0,
        Nombre_Referencia: ""
      };
      this.getTrabajadoresFromApi();
      this.getTrabajadoresBolsa();
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },

    movimiento(item) {
      this.editedIndex = this.trabajadores.indexOf(item);
      this.trabajador = Object.assign({}, item);
      this.dialog5 = true;
    }
  }
};
</script>