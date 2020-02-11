<template>
  <v-card flat>
    <v-row justify="center pa-5">
      <v-text>
        <h2>
          <strong>Nueva Apertura de Socios</strong>
        </h2>
      </v-text>
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
            <v-col cols="12" sm="6" md="12">
              <v-text-field label="Socios Escogidos Según las Características"></v-text-field>
            </v-col>
          </v-row>

          <!-- Columns start at 50% wide on mobile and bump up to 33.3% wide on desktop -->
          <v-row>
            <v-col cols="12" md="8">
              <v-text>
                <strong>Seleccione las Caracerísticas de los Socios a Aprobar:</strong>
              </v-text>
            </v-col>
          </v-row>
          <template>
            <v-row>
              <v-row justify="space-around">
                <v-switch v-model="Sexo" class="ma-2" label="Sexo"></v-switch>
                <v-switch v-model="ColordePiel" class="ma-2" label="Color de Piel"></v-switch>
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
                <v-flex class="ma-2" v-if="ColordePiel">
                  <v-select
                    v-model="colordePiel"
                    item-text="nombre"
                    :items="coloresdePiel"
                    :filter="activeFilter"
                    cache-items
                    clearable
                    label="Color de Piel"
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
                    v-model="edadHasta"
                    clearable
                    label="Rango de Edad"
                    placeholder="Menores de"
                    prepend-icon="mdi-database-search"
                  ></v-text-field>
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
              <v-btn color="green darken-1" class="ma-2" text @click="getFiltrosFromApi">Aceptar</v-btn>
            </v-row>
          </template>
        </v-container>
      </template>
    </v-row>
  </v-card>
</template>
<script>
import api from "@/api";
export default {
  data: () => ({
    dialog: false,
    menu: false,
    search: "",
    editedIndex: -1,
    aperturaSocios: [],
    aperturaSocio: {
      fechaAsamblea: ""
    },
    Sexo: false,
    ColordePiel: false,
    NivelEscolaridad: false,
    RangoEdad: false,
    PerfilOcupacional: false,
    errors: []
  }),

  computed: {
    formTitle() {},
    method() {}
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
    }
  }
};
</script>