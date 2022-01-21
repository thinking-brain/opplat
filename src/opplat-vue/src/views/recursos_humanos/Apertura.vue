<template>
  <v-row justify="center px-4">
    <template>
      <v-container>
        <v-row>
          <v-container>
            <v-row justify="center">
              <v-card-title>
                <h2>
                  <strong>Nueva Apertura de Socios</strong>
                </h2>
              </v-card-title>
            </v-row>
          </v-container>
          <v-col cols="12" sm="6" md="4" class="px-12">
            <v-menu v-model="menu" :close-on-content-click="false" full-width max-width="290">
              <template v-slot:activator="{ on }">
                <v-text-field
                  :value="aperturaSocio.Fecha"
                  clearable
                  label="Fecha"
                  readonly
                  v-on="on"
                ></v-text-field>
              </template>
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
            <v-text>
              <strong>Seleccione las Caracerísticas de los Socios a Aprobar:</strong>
            </v-text>
          </v-col>
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
              v-if="aperturaSocio.Fecha!=null"
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
    Municipios: [],
    edadDesde: "",
    edadHasta: "",
    NivelesEscolaridad: [],
    nivelDeEscolaridad: "",
    PerfilesOcupacionales: [],
    perfilOcupacional: "",
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
    this.getaperturaSociosFromApi();
    this.getSexoTrabFromApi();
    this.getPerfilesOcupacionalesFromApi();
    this.getMunicipiosFromApi();
    this.getnivelEscolaridadTrabFromApi();
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
            sexo: this.sexo,
            nivelEscolaridad: this.nivelEscolaridad,
            edadDesde: this.edadDesde,
            edadHasta: this.edadHasta,
            perfilOcupacional: this.perfilOcupacional,
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
      for (let i = 0; index < this.selected.length; i++) {
        const ListaTrabId = array[i];
      }
      this.aperturaSocio.ListaTrab = ListaTrabId;
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
    }
  }
};
</script>
