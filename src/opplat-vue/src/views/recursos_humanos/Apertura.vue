<template>
<<<<<<< HEAD
  <v-row justify="center pa-5">
    <template>
      <v-container>
        <!-- Stack the columns on mobile by making one full-width and the other half-width -->
        <v-row>
          <v-container>
            <v-row justify="center pa-8">
=======
  <v-row justify="center px-4">
    <template>
      <v-container>
        <v-row>
          <v-container>
            <v-row justify="center">
>>>>>>> cf1e645ce9778f428d321854f37b566c23d25e24
              <v-card-title>
                <h2>
                  <strong>Nueva Apertura de Socios</strong>
                </h2>
              </v-card-title>
            </v-row>
          </v-container>
<<<<<<< HEAD
          <v-col cols="12" sm="6" md="2" offset-md="1">
            <v-menu v-model="menu" :close-on-content-click="false" full-width max-width="290">
              <template v-slot:activator="{ on }">
                <v-text-field
                  :value="aperturaSocio.fechaAsamblea"
=======
          <v-col cols="12" sm="6" md="4" class="px-12">
            <v-menu v-model="menu" :close-on-content-click="false" full-width max-width="290">
              <template v-slot:activator="{ on }">
                <v-text-field
                  :value="aperturaSocio.Fecha"
>>>>>>> cf1e645ce9778f428d321854f37b566c23d25e24
                  clearable
                  label="Fecha"
                  readonly
                  v-on="on"
                ></v-text-field>
              </template>
<<<<<<< HEAD
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
            <v-text>Socios Escogidos Según las Características :</v-text>
          </v-col>
          <v-col cols="12" sm="6" md="10" offset-md="1">
            <v-text v-for="item in selected" :key="item.nombre">{{item.nombre_Completo}}</v-text>
          </v-col>
        </v-row>

        <!-- Columns start at 50% wide on mobile and bump up to 33.3% wide on desktop -->
        <v-row>
          <v-col cols="12" md="8" offset-md="1">
=======
              <v-date-picker v-model="aperturaSocio.Fecha" @change="menu = false"></v-date-picker>
            </v-menu>
          </v-col>
          <v-col cols="12" sm="6" md="4" class="px-12">
            <v-text-field v-model="aperturaSocio.NumeroAcuerdo" label="# Acuerdo Asamblea"></v-text-field>
          </v-col>
          <v-col cols="12" sm="6" md="4" class="px-10">
            <v-text-field
              v-model="aperturaSocio.CantTrabajadores"
              label="Cantidad de Socios a Aprobar"
            ></v-text-field>
          </v-col>
          <v-col cols="12" sm="10" md="12" class="px-12">
            <v-text>
              <strong>Socios Escogidos Según las Características :</strong>
            </v-text>
            <v-chip-group multiple column active-class="primary--text">
              <v-chip
                v-for="item in selected"
                :key="item"
                outlined
                close
                @click="select"
                @click:close="remove(item)"
              >
                <v-icon left>mdi-account-check-outline</v-icon>
                {{ item.nombre_Completo }}
              </v-chip>
            </v-chip-group>
          </v-col>
        </v-row>

        <!-- Buscar Socios -->
        <div>
          <v-col cols="12" sm="10" md="8" offset-md="3">
>>>>>>> cf1e645ce9778f428d321854f37b566c23d25e24
            <v-text>
              <strong>Seleccione las Caracerísticas de los Socios a Aprobar:</strong>
            </v-text>
          </v-col>
<<<<<<< HEAD
        </v-row>
        <template>
          <v-row>
            <v-row justify="space-around">
              <v-switch v-model="Sexo" cols="12" sm="6" md="2" label="Sexo"></v-switch>
              <v-switch v-model="NivelEscolaridad" class="ma-2" label="Nivel de Escolaridad"></v-switch>
              <v-switch v-model="RangoEdad" class="ma-2" label="Rango de Edad"></v-switch>
              <v-switch v-model="PerfilOcupacional" class="ma-2" label="Perfil Ocupacional"></v-switch>
            </v-row>
          </v-row>
        </template>
        <template>
          <v-row>
            <v-row>
              <v-flex cols="12" sm="6" md="2" offset-md="1" v-if="Sexo">
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
              <v-flex cols="12" sm="6" md="2" offset-md="1" v-if="NivelEscolaridad">
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
              <v-flex cols="12" sm="6" md="2" offset-md="1" v-if="RangoEdad">
                <v-text-field
                  v-model="edadDesde"
                  clearable
                  label="Rango de Edad"
                  placeholder="Mayores de"
                  prepend-icon="mdi-database-search"
                ></v-text-field>
              </v-flex>
              <v-flex cols="12" sm="6" md="2" offset-md="1" v-if="RangoEdad">
                <v-text-field
                  v-model="edadHasta"
                  clearable
                  label="Rango de Edad"
                  placeholder="Menores de"
                  prepend-icon="mdi-database-search"
                ></v-text-field>
              </v-flex>
              <v-flex cols="12" sm="6" md="2" offset-md="1" v-if="PerfilOcupacional">
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
                  <v-toolbar-title>Trabajadores Según las Caracteristicas Antes Seleccionadas</v-toolbar-title>
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
=======
          <v-row>
            <v-col cols="3">
              <v-checkbox v-model="Sexo" label="Sexo" class="mt-0 px-5"></v-checkbox>
              <v-checkbox v-model="NivelEscolaridad" label="Nivel de Escolaridad" class="mt-0 px-5"></v-checkbox>
              <v-checkbox v-model="RangoEdad" label="Rango de Edad" class="mt-0 px-5"></v-checkbox>
              <v-checkbox v-model="PerfilOcupacional" label="Perfil Ocupacional" class="mt-0 px-5"></v-checkbox>
            </v-col>
            <v-col cols="9">
              <v-row>
                <v-flex cols="12" sm="6" md="2" offset-md="1" v-if="Sexo">
                  <v-select
                    v-model="aperturaSocio.CaracteristicasSocio.sexo"
                    item-text="nombre"
                    item-value="id"
                    :items="sexos"
                    :filter="activeFilter"
                    cache-items
                    clearable
                    label="Sexo"
                    prepend-icon="mdi-database-search"
                    class="mt-0 px-5"
                  ></v-select>
                </v-flex>
                <v-flex cols="12" sm="6" md="3" v-if="NivelEscolaridad">
                  <v-select
                    v-model="aperturaSocio.CaracteristicasSocio.nivelEscolaridad"
                    item-text="nombre"
                    item-value="id"
                    :items="NivelesEscolaridad"
                    :filter="activeFilter"
                    cache-items
                    clearable
                    label="Nivel de Escolaridad"
                    prepend-icon="mdi-database-search"
                    class="mt-0 px-5"
                  ></v-select>
                </v-flex>
                <v-flex cols="12" sm="6" md="3" v-if="RangoEdad">
                  <v-text-field
                    v-model="aperturaSocio.CaracteristicasSocio.edadDesde"
                    clearable
                    label="Rango de Edad"
                    placeholder="Mayores de"
                    prepend-icon="mdi-database-search"
                    class="mt-0 px-5"
                  ></v-text-field>
                </v-flex>
                <v-flex cols="12" sm="6" md="3" v-if="RangoEdad">
                  <v-text-field
                    v-model="aperturaSocio.CaracteristicasSocio.edadHasta"
                    clearable
                    label="Rango de Edad"
                    placeholder="Menores de"
                    prepend-icon="mdi-database-search"
                    class="mt-0 px-5"
                  ></v-text-field>
                </v-flex>
                <v-flex cols="12" sm="6" md="3" v-if="PerfilOcupacional">
                  <v-autocomplete
                    v-model="aperturaSocio.CaracteristicasSocio.perfilOcupacional"
                    item-text="nombre"
                    item-value="id"
                    :items="PerfilesOcupacionales"
                    :filter="activeFilter"
                    cache-items
                    clearable
                    label="Perfil Ocupacional"
                    prepend-icon="mdi-database-search"
                  ></v-autocomplete>
                </v-flex>
              </v-row>
              <v-flex v-if="Sexo||ColordePiel||NivelEscolaridad||RangoEdad||PerfilOcupacional">
                <v-btn color="green darken-1" class="ma-2" text @click="getFiltrosFromApi">Buscar</v-btn>
              </v-flex>
            </v-col>
          </v-row>
        </div>
        <!-- /Buscar Socios -->
        <template>
          <div class="text-center">
            <v-btn
              class="ma-2"
              small
              color="indigo"
              dark
              @click="guardarApertura()"
            >Guardar Apertura</v-btn>
            <v-btn
              color="error"
              small
              class="ma-2"
              @click="cerrarApertura()"
              v-if="aperturaSocio.fechaAsamblea!='' && aperturaSocio.numAcuerdo||numAcuerdo"
            >Cerrar Apertura</v-btn>
          </div>
        </template>
      </v-container>
    </template>

    <!-- Trabajadores Segun las Caract-->
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
            <v-toolbar-title>Trabajadores Según las Caracteristicas Antes Seleccionadas</v-toolbar-title>
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
        <template v-slot:item.action="{ item }">
          <v-tooltip top>
            <template v-slot:activator="{ on }">
              <v-icon
                small
                class="mr-2"
                v-on="on"
                @click="getDetallesTrabFromApi(item)"
              >mdi-account-plus</v-icon>
            </template>
            <span>Detalles</span>
          </v-tooltip>
        </template>
      </v-data-table>
      <!-- /Trabajadores Segun las Caract-->
    </template>
>>>>>>> cf1e645ce9778f428d321854f37b566c23d25e24
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
    aperturasSocios: [],
    aperturaSocio: {
      ListaTrab: [],
      CaracteristicasSocio: {
        sexo: 0,
        municipioId: 0,
        nivelDeEscolaridad: 0,
        perfilOcupacional: 0,
        edadDesde: "",
        edadHasta: ""
      }
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
    sexos: [],
    selected: [],
<<<<<<< HEAD
=======
    Municipios: [],
    edadDesde: "",
    edadHasta: "",
    NivelesEscolaridad: [],
    nivelDeEscolaridad: "",
    PerfilesOcupacionales: [],
    perfilOcupacional: "",
>>>>>>> cf1e645ce9778f428d321854f37b566c23d25e24
    Sexo: false,
    ColordePiel: false,
    NivelEscolaridad: false,
    RangoEdad: false,
    PerfilOcupacional: false,
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
      { text: "Perfil Ocupacional", value: "perfilOcupacional" },
      { text: "Referencia", value: "nombre_Referencia" },
      { text: "Tiempo en Bolsa", value: "tiempo_Bolsa" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {},
    method() {}
  },

  created() {
<<<<<<< HEAD
    this.getAperturaSociosFromApi();
    this.getSexoTrabFromApi();
=======
    this.getaperturaSociosFromApi();
    this.getSexoTrabFromApi();
    this.getPerfilesOcupacionalesFromApi();
    this.getMunicipiosFromApi();
    this.getnivelEscolaridadTrabFromApi();
>>>>>>> cf1e645ce9778f428d321854f37b566c23d25e24
  },

  methods: {
    getaperturaSociosFromApi() {
      const url = api.getUrl("recursos_humanos", "aperturaSocios");
      this.axios.get(url).then(
        response => {
          this.aperturasSocios = response.data;
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
<<<<<<< HEAD
            unidadOrganizativa: this.unidadOrganizativa,
            cargo: this.cargo,
            sexo: this.sexo,
            estado: this.estado,
            nivelEscolaridad: this.nivelEscolaridad,
            edad: this.edad,
            colordePiel: this.colordePiel,
=======
            sexo: this.sexo,
            nivelEscolaridad: this.nivelEscolaridad,
            edadDesde: this.edadDesde,
            edadHasta: this.edadHasta,
            perfilOcupacional: this.perfilOcupacional,
>>>>>>> cf1e645ce9778f428d321854f37b566c23d25e24
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
<<<<<<< HEAD
=======
    },
    remove(item) {
      this.selected.splice(this.selected.indexOf(item), 1);
      this.selected = [...this.selected];
    },
    getPerfilesOcupacionalesFromApi() {
      this.aperturaSocio.selected = this.selected;
      const url = api.getUrl("recursos_humanos", "PerfilesOcupacionales");
      this.axios.get(url).then(
        response => {
          this.PerfilesOcupacionales = response.data;
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
          this.NivelesEscolaridad = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getPerfilesOcupacionalesFromApi() {
      const url = api.getUrl("recursos_humanos", "PerfilesOcupacionales");
      this.axios.get(url).then(
        response => {
          this.PerfilesOcupacionales = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    guardarApertura() {
      this.aperturaSocio = {
        ListaTrab: this.selected
      };
      const url = api.getUrl("recursos_humanos", "aperturaSocios");
      this.axios.post(url, this.aperturaSocio).then(
        response => {
          this.getResponse(response);
        },
        error => {
          console.log(error);
        }
      );
    },
    getMunicipiosFromApi() {
      const url = api.getUrl("recursos_humanos", "Trabajadores/Municipios");
      this.axios.get(url).then(
        response => {
          this.Municipios = response.data;
        },
        error => {
          console.log(error);
        }
      );
>>>>>>> cf1e645ce9778f428d321854f37b566c23d25e24
    }
  }
};
</script>
