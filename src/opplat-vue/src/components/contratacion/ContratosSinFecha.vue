<template>
  <v-data-table :headers="headers" :items="contratos" :search="search" class="elevation-1 pa-5">
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>{{textByfiltro}}</v-toolbar-title>
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
      </v-toolbar>
      <!-- Editar el Contrato -->
      <v-dialog v-model="dialog" persistent max-width="700">
        <v-card>
          <v-toolbar dark fadeOnScroll color="blue darken-3">
            <v-flex>Editar el contrato: {{nombre}}</v-flex>
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
                <v-flex cols="2" md3 class="px-1">
                  <v-text-field
                    v-model="contrato.numero"
                    :rules="numeroRules"
                    label="Número"
                    prefix="#"
                  ></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex cols="2" md6 class="px-1">
                  <v-menu
                    v-model="menu"
                    :close-on-content-click="false"
                    :nudge-right="40"
                    transition="scale-transition"
                    offset-y
                    min-width="290px"
                  >
                    <template v-slot:activator="{ on }">
                      <v-text-field
                        v-model="contrato.fechaDeFirmado"
                        label="Fecha de Firmado"
                        :rules="fechaFirmadoRules"
                        readonly
                        clearable
                        v-on="on"
                        required
                      ></v-text-field>
                    </template>
                    <v-date-picker v-model="contrato.fechaDeFirmado" @input="menu = false"></v-date-picker>
                  </v-menu>
                </v-flex>
                <v-flex cols="2" md6 class="px-1">
                  <v-menu
                    v-model="menu1"
                    :close-on-content-click="false"
                    :nudge-right="40"
                    transition="scale-transition"
                    offset-y
                    min-width="290px"
                  >
                    <template v-slot:activator="{ on }">
                      <v-text-field
                        v-model="contrato.fechaDeVencimiento"
                        label="Fecha de Vencimiento del Contrato"
                        :rules="fechaVencimientoRules"
                        readonly
                        clearable
                        v-on="on"
                        required
                      ></v-text-field>
                    </template>
                    <v-date-picker v-model="contrato.fechaDeVencimiento" @input="menu1 = false"></v-date-picker>
                  </v-menu>
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
      <!-- /Editar el Contrato -->
    </template>
    <!-- Actions -->
    <template v-slot:item.action="{ item }">
      <v-row>
        <v-tooltip top color="primary">
          <template v-slot:activator="{ on }">
            <v-btn
              class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small primary--text"
              small
              v-on="on"
              @click="editItem(item)"
              slot="activator"
            >
              <v-icon>v-icon notranslate mdi mdi-pen-plus theme--dark</v-icon>
            </v-btn>
          </template>
          <span>Editar</span>
        </v-tooltip>
        <v-tooltip top color="teal">
          <template v-slot:activator="{ on }">
            <v-btn
              class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small teal--text"
              small
              v-on="on"
              @click="getDetalles(item)"
            >
              <v-icon>mdi-format-list-bulleted</v-icon>
            </v-btn>
          </template>
          <span>Detalles</span>
        </v-tooltip>
        <v-tooltip top color="black">
          <template v-slot:activator="{ on }">
            <v-btn
              class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small secondary--text"
              small
              v-on="on"
              @click="download(item)"
            >
              <v-icon>v-icon notranslate mdi mdi-download theme--dark</v-icon>
            </v-btn>
          </template>
          <span>Descargar Documento</span>
        </v-tooltip>
      </v-row>
    </template>
    <!-- /Actions -->
  </v-data-table>
</template>
<script>
import api from "@/api";

export default {
  data: () => ({
    search: "",
    dialog: false,
    menu: false,
    menu1: false,
    editedIndex: -1,
    tabs: null,
    contratos: [],
    contrato: {
      esContrato: true
    },
    nombre: "",
    textByfiltro: "",
    show: false,
    tabs: null,
    roles: [],
    errors: [],
    numeroRules: [v => !!v || "El número es requerido"],
    fechaFirmadoRules: [v => !!v || "La fecha es requerida"],
    fechaVencimientoRules: [v => !!v || "La fecha es requerida"],

    headers: [
      { text: "Número", align: "left", sortable: true, value: "numero" },
      { text: "Nombre", sortable: true, value: "nombre" },
      { text: "Tipo", value: "tipoNombre" },
      { text: "Entidad", value: "entidad[0].nombre" },
      { text: "Sector", value: "entidad[0].sectorNombre" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {},
    method() {}
  },

  watch: {},

  created() {
    this.roles = this.$store.getters.roles;
    this.username = this.$store.getters.usuario;
    this.getContratosFromApi();
  },

  methods: {
    getContratosFromApi() {
      const url = api.getUrl(
        "contratacion",
        `Contratos?sinFechaVencimiento=true`
      );
      this.axios.get(url).then(
        response => {
          this.textByfiltro = "Contratos sin fecha de Vencimiento";
          this.contratos = response.data;
          this.cantContratos = this.contratos.length;
        },
        error => {
          console.log(error);
        }
      );
    },
    getDetalles(item) {
      this.contrato = Object.assign({}, item);
      this.contrato.entidad = item.entidad[0];
      this.contrato.adminContrato = item.adminContrato;
      this.contrato.esContrato = true;
      this.contrato.sinFechaVen = true;
      const contrato = this.contrato;
      this.$router.push({
        name: "Detalles_Contrato",
        query: {
          contrato
        }
      });
    },
    download(item) {
      const url = api.getUrl("contratacion", "contratos/DownloadFile");
      this.axios.get(`${url}/${item.id}`).then(
        response => {
          window.open(url + "/" + item.id);
        },
        error => {
          console.log(error);
        }
      );
    },
    editItem(item) {
      this.nombre = item.nombre;
      this.contrato.id = item.id;
      this.contrato.contratoId = item.id;
      this.contrato.fechaDeFirmado = null;
      this.contrato.username = this.username;
      this.dialog = true;
    },
    save(method) {
      if (this.$refs.form.validate()) {
        const url = api.getUrl("contratacion", "Contratos/editJuridico");
        this.axios
          .put(`${url}/${this.contrato.id}`, this.contrato)
          .then(
            response => {
              this.getResponse(response);
              this.getContratosFromApi();
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          )
          .catch(e => {
            vm.$snotify.error(e.response.data.errors);
          });
      }
    },
    close() {
      this.dialog = false;
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    }
  }
};
</script>