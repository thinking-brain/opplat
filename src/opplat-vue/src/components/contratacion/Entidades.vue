<template>
  <v-data-table :headers="headers" :items="entidades" :search="search" class="elevation-1 pa-5">
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Proveedores</v-toolbar-title>
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
        <!-- Agregar y Editar Entidad -->
        <v-dialog v-model="dialog" persistent max-width="1000">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on">Nuevo Proveedor</v-btn>
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
                  <v-flex xs3 class="px-3">
                    <v-text-field v-model="entidad.nombre" label="Nombre" clearable required></v-text-field>
                  </v-flex>
                  <v-flex 6 class="px-3">
                    <v-text-field
                      v-model="entidad.direccion"
                      label="Direccion"
                      placeholder="Calle e/ #, Barrio o Finca, Municipio y Provincia"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field v-model="entidad.nit" label="NIT" clearable required></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs6 class="px-3">
                    <v-text-field v-model="entidad.fax" label="Fax" clearable required></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      v-model="entidad.correo"
                      label="Correo"
                      clearable
                      required
                      :rules="emailRules"
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <!-- Agregar Número de Teléfono del Proveedor -->
                <v-layout row wrap>
                  <v-flex xs12>
                    <v-expansion-panels flat focusable>
                      <v-expansion-panel>
                        <v-expansion-panel-header v-slot="{ open }">
                          <v-row no-gutters>
                            <v-col cols="3">Número de Teléfono</v-col>
                            <v-col cols="9" class="text--secondary">
                              <v-fade-transition leave-absolute>
                                <span v-if="open">Llene los Datos Pertinentes</span>
                                <v-row v-else no-gutters style="width: 100%">
                                  <v-col cols="6">Teléfonos Agregados: {{  }}</v-col>
                                </v-row>
                              </v-fade-transition>
                            </v-col>
                          </v-row>
                        </v-expansion-panel-header>
                        <v-expansion-panel-content>
                          <v-col cols="6">
                            <v-row class="mb-6 pt-4" no-gutters>
                              <v-col lg="1">
                                <v-card class="pa-2" tile flat>
                                  <v-tooltip top color="success">
                                    <template v-slot:activator="{ on }">
                                      <v-icon
                                        medium
                                        v-on="on"
                                        @click="agregar(telef)"
                                        color="success"
                                      >mdi-plus</v-icon>
                                    </template>
                                    <span>Agregar Número de Teléfono</span>
                                  </v-tooltip>
                                </v-card>
                              </v-col>
                              <v-col md="auto">
                                <v-card class="pa-2" tile flat>{{telefonos}}</v-card>
                              </v-col>
                              <v-col lg="2">
                                <v-card class="pa-2" tile flat>
                                  <v-tooltip top color="red darken-1">
                                    <template v-slot:activator="{ on }">
                                      <v-icon
                                        medium
                                        v-on="on"
                                        @click="quitar(telef)"
                                        color="red"
                                      >mdi-minus</v-icon>
                                    </template>
                                    <span>Eliminar</span>
                                  </v-tooltip>
                                </v-card>
                              </v-col>
                            </v-row>
                            <v-row justify="space-around" no-gutters v-if="telefonos>=1">
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.telefono"
                                  label="Teléfono"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.extension"
                                  label="Extensión"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                              <v-col cols="6"></v-col>
                            </v-row>
                            <v-row justify="space-around" no-gutters v-if="telefonos>=2">
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.telefono"
                                  label="Teléfono"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.extension"
                                  label="Extensión"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                            </v-row>
                            <v-row justify="space-around" no-gutters v-if="telefonos>=3">
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.telefono"
                                  label="Teléfono"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.extension"
                                  label="Extensión"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                            </v-row>
                            <v-row justify="space-around" no-gutters v-if="telefonos>=4">
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.telefono"
                                  label="Teléfono"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.extension"
                                  label="Extensión"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                            </v-row>
                            <v-row justify="space-around" no-gutters v-if="telefonos>=5">
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.telefono"
                                  label="Teléfono"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.extension"
                                  label="Extensión"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                            </v-row>
                            <v-row justify="space-around" no-gutters v-if="telefonos>=6">
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.telefono"
                                  label="Teléfono"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                              <v-flex cols="3">
                                <v-text-field
                                  v-model="entidad.extension"
                                  label="Extensión"
                                  clearable
                                  required
                                ></v-text-field>
                              </v-flex>
                            </v-row>
                          </v-col>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-expansion-panels>
                  </v-flex>
                </v-layout>
                <!-- Agregar Número de Teléfono -->

                <!-- Agregar Cuenta Bancaria del Proveedor -->
                <v-layout row wrap>
                  <v-flex xs12>
                    <v-expansion-panels flat focusable>
                      <v-expansion-panel>
                        <v-expansion-panel-header v-slot="{ open }">
                          <v-row no-gutters>
                            <v-col cols="3">Cuenta Bancaria</v-col>
                            <v-col cols="9" class="text--secondary">
                              <v-fade-transition leave-absolute>
                                <span v-if="open">Llene los Datos Pertinentes</span>
                                 <v-row v-else no-gutters style="width: 100%">
                                  <v-col cols="6">Cuentas Agregados: {{  }}</v-col>
                                </v-row>
                              </v-fade-transition>
                            </v-col>
                          </v-row>
                        </v-expansion-panel-header>
                        <v-expansion-panel-content>
                          <v-col cols="4">
                            <v-row class="mb-6 pt-4" no-gutters>
                              <v-card class="pa-2" tile flat>
                                <v-tooltip top color="success">
                                  <template v-slot:activator="{ on }">
                                    <v-icon
                                      medium
                                      v-on="on"
                                      @click="agregar(cuent)"
                                      color="success"
                                    >mdi-plus</v-icon>
                                  </template>
                                  <span>Agregar Cuenta Bancaria</span>
                                </v-tooltip>
                              </v-card>
                              <v-col md="auto">
                                <v-card class="pa-2" tile flat>{{cuentas}}</v-card>
                              </v-col>
                              <v-card class="pa-2" tile flat>
                                <v-tooltip top color="red darken-1">
                                  <template v-slot:activator="{ on }">
                                    <v-icon
                                      medium
                                      v-on="on"
                                      @click="quitar(cuent)"
                                      color="red"
                                    >mdi-minus</v-icon>
                                  </template>
                                  <span>Eliminar</span>
                                </v-tooltip>
                              </v-card>
                            </v-row>
                          </v-col>
                          <v-row justify="space-around" no-gutters v-if="cuentas>=1">
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número de cuenta 1"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Nombre Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Moneda"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                          </v-row>
                          <v-row justify="space-around" no-gutters v-if="cuentas>=2">
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número de cuenta 2"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Nombre Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Moneda"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                          </v-row>
                          <v-row justify="space-around" no-gutters v-if="cuentas>=3">
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número de cuenta 3"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Nombre Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Moneda"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                          </v-row>
                          <v-row justify="space-around" no-gutters v-if="cuentas>=4">
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número de cuenta 4"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Nombre Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Moneda"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                          </v-row>
                          <v-row justify="space-around" no-gutters v-if="cuentas>=5">
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número de cuenta 5"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Nombre Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Moneda"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                          </v-row>
                          <v-row justify="space-around" no-gutters v-if="cuentas>=6">
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número de cuenta 6"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Número Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Nombre Sucursal"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                            <v-flex cols="3" class="px-2">
                              <v-text-field
                                v-model="entidad.nombreCta"
                                label="Moneda"
                                clearable
                                required
                              ></v-text-field>
                            </v-flex>
                          </v-row>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-expansion-panels>
                  </v-flex>
                </v-layout>
                <!-- Agregar Cuenta Bancaria del Proveedor -->
              </v-container>
            </v-form>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Agregar y Editar Entidad -->

        <!-- Detalles de la Entidad -->
        <v-dialog
          v-model="dialog3"
          persistent
          transition="dialog-bottom-transition"
          flat
          max-width="900"
        >
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles del Proveedor</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="dialog3 = false">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-container fluid>
              <v-row dense no-gutters>
                <v-container>
                  <v-row>
                    <v-col cols="10" md="6">
                      <v-text-field
                        v-model="entidad.nombre"
                        label="Nombre del Proveedor"
                        outlined
                        readonly
                      ></v-text-field>
                    </v-col>
                    <v-col cols="3" md="4">
                      <v-text-field v-model="entidad.direccion" label="Dirección" outlined readonly></v-text-field>
                    </v-col>
                    <v-col cols="3" md="2">
                      <v-text-field v-model="entidad.nit" label="NIT" outlined readonly></v-text-field>
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col cols="6" md="4">
                      <v-text-field
                        v-model="entidad.ctaBancariaCuc"
                        label="Cuenta Bancaria CUC"
                        outlined
                        readonly
                      ></v-text-field>
                    </v-col>
                    <v-col cols="6" md="4">
                      <v-text-field
                        v-model="entidad.ctaBancariaMn"
                        label="Cuenta Bancaria MN"
                        outlined
                        readonly
                      ></v-text-field>
                    </v-col>
                    <v-col cols="6" md="4">
                      <v-text-field
                        v-model="entidad.nombreCtaCuc"
                        label="Nombre de la Cuenta en CUC"
                        outlined
                        readonly
                        prefix="$"
                      ></v-text-field>
                    </v-col>
                  </v-row>

                  <v-row>
                    <v-col cols="6" md="6">
                      <v-text-field
                        v-model="entidad.nombreCta"
                        label="Nombre de la Cuenta en MN"
                        outlined
                        readonly
                      ></v-text-field>
                    </v-col>
                    <v-col cols="6" md="6">
                      <v-text-field v-model="entidad.telefono" label="Teléfono" outlined readonly></v-text-field>
                    </v-col>
                  </v-row>
                </v-container>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- /Detalles de la Entidad -->

        <!-- Delete Entidad -->
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
            <v-card-title class="headline text-center">Seguro que deseas eliminar la Entidad</v-card-title>
            <v-card-text class="text-center">{{entidad.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(entidad)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete entidad -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-tooltip top color="success">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="editItem(item)">mdi-pencil</v-icon>
        </template>
        <span>Editar</span>
      </v-tooltip>
      <v-tooltip top color="green darken-4">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="getDetalles(item)">mdi-file-document-box-plus</v-icon>
        </template>
        <span>Detalles</span>
      </v-tooltip>
      <v-tooltip top color="red darken-1">
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
    dialog3: false,
    search: "",
    editedIndex: -1,
    entidades: [],
    entidad: {},
    cuentas: 1,
    cuent: "cuentas",
    telefonos: 1,
    telef: "telefonos",
    tabs: null,
    emailRules: [v => /.+@.+\..+/.test(v) || "No tiene la estructura correcta"],
    errors: [],
    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "nombre"
      },
      { text: "Dirección", value: "direccion" },
      { text: "NIT", value: "nit" },
      { text: "Cuenta Ban Cuc", value: "ctaBancariaCuc" },
      { text: "Cuenta Ban Mn", value: "ctaBancariaMn" },
      { text: "Acciones", value: "action", sortable: false }
    ],
    date: null,
    trip: {
      name: "",
      location: null,
      start: null,
      end: null
    },
    locations: [
      "Australia",
      "Barbados",
      "Chile",
      "Denmark",
      "Equador",
      "France"
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nuevo Proveedor" : "Editar Proveedor";
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
    this.getEntidadesFromApi();
  },

  methods: {
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
      this.editedIndex = this.entidades.indexOf(item);
      this.entidad = Object.assign({}, item);
      this.dialog = true;
    },

    save(method) {
      const url = api.getUrl("contratacion", "entidades");
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          this.axios.post(url, this.entidad).then(
            response => {
              this.getResponse(response);
              this.getEntidadesFromApi();
              this.entidad = {};
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
        }
      }
      if (method === "PUT") {
        this.axios.put(`${url}/${this.entidad.id}`, this.entidad).then(
          response => {
            this.getResponse(response);
            this.getEntidadesFromApi();
            this.entidad = {};
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
    },
    getDetalles(item) {
      this.entidad = Object.assign({}, item);
      this.dialog3 = true;
    },
    confirmDelete(item) {
      this.entidad = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(entidad) {
      const url = api.getUrl("contratacion", "entidades");
      this.axios.delete(`${url}/${entidad.id}`).then(
        response => {
          this.getResponse(response);
          this.getEntidadesFromApi();
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
    },
    agregar(string) {
      if (string == "telefonos") {
        this.telefonos = this.telefonos + 1;
      }
      if (string == "cuentas") {
        this.cuentas = this.cuentas + 1;
      }
    },
    quitar(string) {
      if (string == "telefonos") {
        this.telefonos = this.telefonos - 1;
      }
      if (string == "cuentas") {
        this.cuentas = this.cuentas - 1;
      }
    }
  }
};
</script>
