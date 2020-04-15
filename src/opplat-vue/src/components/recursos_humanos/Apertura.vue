<template>
  <v-row justify="center pa-5">
    <template>
      <v-container>
        <!-- Stack the columns on mobile by making one full-width and the other half-width -->
        <v-row>
          <v-col cols="12" sm="6" md="2" offset-md="1">
            <v-menu v-model="menu" :close-on-content-click="false" full-width max-width="290">
              <template v-slot:activator="{ on }">
                <v-text-field
                  :value="aperturaSocio.fechaAsamblea"
                  clearable
                  label="Fecha"
                  readonly
                  v-on="on"
                ></v-text-field>
              </template>
              <v-date-picker v-model="aperturaSocio.fechaAsamblea" @change="menu = false"></v-date-picker>
            </v-menu>
          </v-col>
          <v-col cols="12" sm="6" md="3" offset-md="1">
            <v-text-field label="# Acuerdo Asamblea"></v-text-field>
          </v-col>
          <v-col cols="12" sm="6" md="3" offset-md="1">
            <v-text-field label="Cantidad de Socios a Aprobar"></v-text-field>
          </v-col>
          <v-col cols="12" sm="6" md="10" offset-md="1">
            <v-text-field label="Socios Escogidos Según las Características"></v-text-field>
          </v-col>
        </v-row>

        <!-- Columns start at 50% wide on mobile and bump up to 33.3% wide on desktop -->
        <v-row>
          <v-col cols="12" md="8" offset-md="1">
            <v-text>
              <strong>Seleccione las Caracerísticas de los Socios a Aprobar:</strong>
            </v-text>
          </v-col>
        </v-row>
        <template>
          <v-row>
            <v-row justify="space-around">
              <v-switch v-model="Sexo" class="ma-2" label="Sexo"></v-switch>
              <v-switch v-model="NivelEscolaridad" class="ma-2" label="Nivel de Escolaridad"></v-switch>
              <v-switch v-model="RangoEdad" class="ma-2" label="Rango de Edad"></v-switch>
              <v-switch v-model="PerfilOcupacional" class="ma-2" label="Perfil Ocupacional"></v-switch>
            </v-row>
          </v-row>
        </template>
        <template>
          <v-row>
            <v-row justify="space-around">
              <v-flex class="ma-2" v-if="Sexo">
                <v-select
                  v-model="sexo"
                  item-text="nombre"
                  :items="sexos"
                  :filter="activeFilter"
                  cache-items
                  clearable
                  label="Sexo"
                  prepend-icon="mdi-database-search"
                ></v-select>
              </v-flex>
              <v-flex class="ma-2" v-if="NivelEscolaridad">
                <v-select
                  v-model="nivelEscolaridad"
                  item-text="nombre"
                  :items="nivelesEscolaridad"
                  :filter="activeFilter"
                  cache-items
                  clearable
                  label="Nivel de Escolaridad"
                  prepend-icon="mdi-database-search"
                ></v-select>
              </v-flex>
              <v-flex class="ma-2" v-if="RangoEdad">
                <v-text-field
                  v-model="edadDesde"
                  clearable
                  label="Rango de Edad"
                  placeholder="Mayores de"
                  prepend-icon="mdi-database-search"
                ></v-text-field>
              </v-flex>
              <v-flex class="ma-2" v-if="RangoEdad">
                <v-text-field
                  v-model="edadHasta"
                  clearable
                  label="Rango de Edad"
                  placeholder="Menores de"
                  prepend-icon="mdi-database-search"
                ></v-text-field>
              </v-flex>
              <v-flex class="ma-2" v-if="PerfilOcupacional">
                <v-text-field
                  v-model="perfilOcupacional"
                  clearable
                  label="Perfil Ocupacional"
                  prepend-icon="mdi-database-search"
                ></v-text-field>
              </v-flex>
            </v-row>
          </v-row>
          <v-row v-if="Sexo||ColordePiel||NivelEscolaridad||RangoEdad||PerfilOcupacional">
            <v-btn color="green darken-1" class="ma-2" text @click="getFiltrosFromApi">Buscar</v-btn>
          </v-row>
          <template v-if="buscar">
            <v-data-table
              v-model="selected"
              :headers="headers"
              :items="trabajadores"
              :search="search"
              item-key="id"
              show-select
              class="elevation-1 pa-5"
              dense
            >
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
                  <v-spacer></v-spacer>
                </v-toolbar>
              </template>
            </v-data-table>
          </template>
        </template>
      </v-container>
    </template>
  </v-row>
</template>
<script>
import api from "@/api";

export default {
  data: () => ({
    dialog: false,
    menu: false,
    search: "",
    buscar: false,
    editedIndex: -1,
    aperturaSoc: [],
    aperturaSocio: {
      fechaAsamblea: ""
    },
    trabajadores: [],
    trabajador: {
      sexo: 0,
      colorDeOjos: 0,
      colorDePiel: 0,
      tallaDeCamisa: 0,
      nombre_Referencia: "",
      perfilOcupacionalId: 0,
      municipioId: 0
    },
    sexos:[],
    selected: [],
    Sexo: false,
    ColordePiel: false,
    NivelEscolaridad: false,
    RangoEdad: false,
    PerfilOcupacional: false,
    errors: [],
    ex7: "red",
    ex8: "primary",
    headers: [
      {
        text: "Nombre y Apellidos",
        align: "left",
        sortable: true,
        value: "nombre_Completo"
      },
      { text: "Carnet de Identidad", value: "ci" },
      { text: "Dirección", value: "direccion" },
      { text: "Perfil Ocupacional", value: "perfilOcupacional" },
      { text: "Referencia", value: "nombre_Referencia" },
      { text: "Tiempo en Bolsa", value: "tiempo_Bolsa" },
      { text: "Acciones", value: "action", sortable: false }
    ],
  }),

  computed: {
    formTitle() {},
    method() {}
  },

  created() {
    this.getAperturaSociosFromApi(); 
    this.getSexoTrabFromApi(); 
  },

  methods: {
    getAperturaSociosFromApi() {
      const url = api.getUrl("recursos_humanos", "AperturaSocios");
      this.axios.get(url).then(
        response => {
          this.aperturaSoc = response.data;
          this.volver = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    getFiltrosFromApi() {
      this.buscar = true;
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
            colordePiel: this.colordePiel,
            bolsa: true
          }
        })
        .then(
          response => {
            this.trabajadores = response.data;
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
    }
  }
};
</script>
