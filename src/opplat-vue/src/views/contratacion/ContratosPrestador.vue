<template>
  <v-data-table :headers="headers" :items="contratos" :search="search" class="elevation-1 pa-5">
    <template v-slot:item.contVence="{ item }">
      <v-chip :color="getColor(item.contVence)" dark>{{ item.contVence }} días</v-chip>
    </template>
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

        <!-- Todos los Contratos -->
        <v-badge
          :content="cantContratos"
          :value="cantContratos"
          color="primary"
          overlap
          class="mt-4"
        >
          <template v-slot:badge>
            <span v-if="enTiempo > 0">{{ cantContratos }}</span>
          </template>
          <v-tooltip top color="primary">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="primary"
                @click="getContratosFromApi()"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Todos los Contratos</span>
          </v-tooltip>
        </v-badge>
        <!-- /Todos los Contratos -->

        <!-- Cantidad de Contratos Ok -->
        <v-badge :content="enTiempo" :value="enTiempo" color="green" overlap class="mt-4 ml-4">
          <template v-slot:badge>
            <span v-if="enTiempo > 0">{{ enTiempo }}</span>
          </template>
          <v-tooltip top color="green">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="green"
                @click="filtro(contratoTiempo)"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Contratos en Tiempo</span>
          </v-tooltip>
        </v-badge>
        <!-- /Cantidad de Contratos Ok -->

        <!-- Cantidad de Contratos Casi Vencidos -->
        <v-badge :content="proxVencer" :value="proxVencer" color="orange" overlap class="mt-4 ml-4">
          <template v-slot:badge>
            <span v-if="proxVencer > 0">{{ proxVencer}}</span>
          </template>
          <v-tooltip top color="orange">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="orange"
                @click="filtro(contratosProxVencer)"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Contratos Próximos a vencer</span>
          </v-tooltip>
        </v-badge>
        <!-- /Cantidad de Contratos Casi Vencidos -->

        <!-- Cantidad de Contratos proxVencer -->
        <v-badge
          :content="casiVenc"
          :value="casiVenc"
          color="deep-orange"
          overlap
          class="mt-4 ml-4"
        >
          <template v-slot:badge>
            <span v-if="casiVenc > 0">{{ casiVenc }}</span>
          </template>
          <v-tooltip top color="deep-orange">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="deep-orange"
                @click="filtro(contratosCasiVenc)"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Contratos casi vencidas</span>
          </v-tooltip>
        </v-badge>
        <!-- /Cantidad de Contratos proxVencer -->

        <!-- Cantidad de Contratos Vencidos -->
        <v-badge :content="vencidos" :value="vencidos" color="red" overlap class="mt-4 ml-4">
          <template v-slot:badge>
            <span v-if="vencidos > 0">{{ vencidos }}</span>
          </template>
          <v-tooltip top color="red">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="red"
                @click="filtro(contratosVenc)"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Contratos Vencidos</span>
          </v-tooltip>
        </v-badge>
        <!-- /Cantidad de Contratos Vencidos -->

        <!-- Detalles del Contrato -->
        <v-dialog
          v-model="dialog6"
          persistent
          transition="dialog-bottom-transition"
          flat
          max-width="1100"
        >
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles del Contrato</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>

            <v-container fluid>
              <v-row dense no-gutters>
                <v-col cols="7">
                  <v-container>
                    <v-row>
                      <v-col cols="10" md="6">
                        <v-text-field v-model="contrato.nombre" label="Nombre" outlined readonly></v-text-field>
                      </v-col>
                      <v-col cols="3" md="2">
                        <v-text-field
                          v-model="contrato.numero"
                          label="Número"
                          outlined
                          readonly
                          prefix="#"
                        ></v-text-field>
                      </v-col>
                      <v-col cols="3" md="4">
                        <v-text-field
                          v-model="contrato.objetoDeContrato"
                          label="Objeto de Contrato"
                          outlined
                          readonly
                        ></v-text-field>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col cols="6" md="4">
                        <v-text-field v-model="contrato.entidad" label="Entidad" outlined readonly></v-text-field>
                      </v-col>
                      <v-col cols="6" md="4">
                        <v-text-field
                          v-model="contrato.montoCup"
                          label="Monto en CUP"
                          outlined
                          readonly
                          prefix="$"
                        ></v-text-field>
                      </v-col>
                      <v-col cols="6" md="4">
                        <v-text-field
                          v-model="contrato.montoCuc"
                          label="Monto en CUC"
                          outlined
                          readonly
                          prefix="$"
                        ></v-text-field>
                      </v-col>
                    </v-row>

                    <v-row>
                      <v-col cols="6" md="6">
                        <v-text-field
                          v-model="contrato.terminoDePago"
                          label="Término de Pago"
                          outlined
                          readonly
                        ></v-text-field>
                      </v-col>
                      <v-col cols="6" md="6">
                        <v-text-field v-model="contrato.estado" label="Estado" outlined readonly></v-text-field>
                      </v-col>
                    </v-row>
                  </v-container>
                </v-col>

                <v-col cols="5">
                  <v-timeline>
                    <v-timeline-item :color="'blue'" :right="true" small>
                      <template v-slot:opposite>
                        <h5 :class="`subtitle-2 blue--text`" v-text="contrato.fechaDeRecepcion"></h5>
                      </template>
                      <v-card class="elevation-2">
                        <v-card-text
                          :class="`subtitle-2 blue--text`"
                        >Fecha en que se Recibió el Contrato</v-card-text>
                      </v-card>
                    </v-timeline-item>
                    <v-timeline-item :color="'success'" :right="true" small>
                      <template v-slot:opposite>
                        <span :class="`subtitle-2 success--text`" v-text="contrato.fechaDeFirmado"></span>
                      </template>
                      <v-card class="elevation-2">
                        <v-card-text
                          :class="`subtitle-2 success--text`"
                        >Fecha en que se Firmó el Contrato</v-card-text>
                      </v-card>
                    </v-timeline-item>
                  </v-timeline>
                </v-col>
              </v-row>
              <v-row no-gutters class="px-3">
                <v-col>
                  <v-card class="pa-2" outlined tile>
                    <v-text>Aprobado por el Jurídico :</v-text>
                    <span v-if="contrato.aprobJuridico">
                      <v-icon color="success">mdi-check-underline</v-icon>
                      <v-text :class="`success--text`">Sí</v-text>
                    </span>
                    <span v-else>
                      <v-icon color="red">mdi-close-outline</v-icon>
                      <v-text :class="`red--text`">No</v-text>
                    </span>
                  </v-card>
                </v-col>
                <v-col>
                  <v-card class="pa-2" outlined tile>
                    <v-text>Aprobado por el Económico :</v-text>
                    <span v-if="contrato.aprobEconomico">
                      <v-icon color="success">mdi-check-underline</v-icon>
                      <v-text :class="`success--text`">Sí</v-text>
                    </span>
                    <span v-else>
                      <v-icon color="red">mdi-close-outline</v-icon>
                      <v-text :class="`red--text`">No</v-text>
                    </span>
                  </v-card>
                </v-col>
                <v-col>
                  <v-card class="pa-2" outlined tile>
                    <v-text>Aprobado por el Comité Contratación:</v-text>
                    <span v-if="contrato.aprobComitContratacion">
                      <v-icon color="success">mdi-check-underline</v-icon>
                      <v-text :class="`success--text`">Sí</v-text>
                    </span>
                    <span v-else>
                      <v-icon color="red">mdi-close-outline</v-icon>
                      <v-text :class="`red--text`">No</v-text>
                    </span>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- /Detalles del Contrato -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon
            medium
            class="mr-2"
            v-on="on"
            @click="getDetalles(item)"
          >mdi-file-document-box-plus</v-icon>
        </template>
        <span>Detalles</span>
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
    dialog6: false,
    menu: false,
    search: "",
    editedIndex: -1,
    tabs: null,
    contratos: [],
    contrato: {},
    cantContratos: 0,
    enTiempo: 0,
    casiVenc: 0,
    proxVencer: 0,
    vencidos: 0,
    todosLosContratos: false,
    vencimientoContratos: [],
    contratoTiempo: "contratoTiempo",
    contratosProxVencer: "contratosProxVencer",
    contratosCasiVenc: "contratosCasiVenc",
    contratosVenc: "contratosVenc",
    tiempoVenContratos: {},
    urlByfiltro: "",
    textByfiltro: "",
    show: false,
    tabs: null,
    textContratoVence: {
      text: null,
      class: null
    },
    errors: [],
    headers: [
      { text: "Número", align: "left", sortable: true, value: "numero" },
      { text: "Nombre", sortable: true, value: "nombre" },
      { text: "Tipo", value: "tipoNombre" },
      { text: "Entidad", value: "entidad.nombre" },
      { text: "Vence", value: "contVence" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nueva Contrato" : "Editar Contrato";
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
    this.getContratosFromApi();
  },

  methods: {
    getContratosFromApi() {
      const url = api.getUrl("contratacion", "Contratos?tipoTramite=contrato&cliente=false");
      this.axios.get(url).then(
        response => {
          this.textByfiltro = "Contratos como Prestador";
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
      this.dialog6 = true;
    },
    getTiempoVenContratosFromApi() {
      const url = api.getUrl("contratacion", "TiempoVenContratos");
      this.axios.get(url).then(
        response => {
          this.tiempoVenContratos = response.data[0];
        },
        error => {
          console.log(error);
        }
      );
    },
    editItem(item) {
      this.editedIndex = this.contratos.indexOf(item);
      this.contrato = Object.assign({}, item);
      this.dialog = true;
    },
    close() {
      this.dialog = false;
      this.dialog2 = false;
      this.dialog6 = false;
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    save(method) {
      const url = api.getUrl("contratacion", "Contratos");
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          this.axios.post(url, this.contrato).then(
            response => {
              this.getResponse(response);
              this.getContratosFromApi();
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
        }
      }
      if (method === "PUT") {
        this.axios.put(`${url}/${this.Contrato.id}`, this.Contrato).then(
          response => {
            this.getResponse(response);
            this.getContratosFromApi();
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
    },
    confirmDelete(item) {
      this.contrato = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(Contrato) {
      const url = api.getUrl("contratacion", "Contratos");
      this.axios.delete(`${url}/${Contrato.id}`).then(
        response => {
          this.getResponse(response);
          this.getContratosFromApi();
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
      this.dialog6 = false;
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    getColor(contratoVence) {
      this.GetVencimientoContrato();
      if (contratoVence < this.tiempoVenContratos.contratosVencidos) {
        return "red";
      } else if (
        contratoVence >= this.tiempoVenContratos.contratosCasiVencDesde &&
        contratoVence <= this.tiempoVenContratos.contratosCasiVencHasta
      ) {
        return "deep-orange";
      } else if (
        contratoVence > this.tiempoVenContratos.contratosProxVencerDesde &&
        contratoVence <= this.tiempoVenContratos.contratosProxVencerHasta
      )
        return "orange";
      else {
        return "green";
      }
    },
    GetVencimientoContrato() {
      const url = api.getUrl("contratacion", "contratos/VencimientoContrato?cliente=false");
      this.axios.get(url).then(
        response => {
          this.vencimientoContratos = response.data;
          this.vencidos = this.vencimientoContratos[0];
          this.casiVenc = this.vencimientoContratos[1];
          this.proxVencer = this.vencimientoContratos[2];
          this.enTiempo = this.vencimientoContratos[3];
        },
        error => {
          console.log(error);
        }
      );
    },
    filtro(filtro) {
      if (filtro == "contratoTiempo") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=contrato&filtro=contratoTiempo&cliente=false"
        );
        this.textByfiltro = "Contratos en Tiempo";
      }
      if (filtro == "contratosProxVencer") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=contrato&filtro=contratosProxVencer&cliente=false"
        );
        this.textByfiltro = "Contratos Próximos a Vencer";
      }
      if (filtro == "contratosCasiVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=contrato&filtro=contratosCasiVenc&cliente=false"
        );
        this.textByfiltro = "Contratos Casi Vencidos";
      }
      if (filtro == "contratosVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=contrato&filtro=contratosVenc&cliente=false"
        );
        this.textByfiltro = "Contratos Vencidos";
      }
      this.axios.get(this.urlByfiltro).then(
        response => {
          this.contratos = response.data;
          vm.$snotify.success(this.textByfiltro);
          this.todosLosContratos = true;
        },
        error => {
          console.log(error);
        }
      );
    }
  }
};
</script>