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
          v-model="dialog3"
          persistent
          transition="dialog-bottom-transition"
          flat
          max-width="1250"
        >
          <v-card>
            <v-toolbar dark fadeOnScroll :color="getColor(contrato.contVence)">
                <h2>Detalles del Contrato de {{contrato.tipoNombre}} de {{contrato.nombre}}.</h2>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-container>
              <v-row>
                <!-- DATOS DE El contrato -->
                <v-col cols="6">
                  <v-card :elevation="2" flat>
                    <v-card-text>
                      <v-row>
                        <v-col cols="6" md="6" class="pa-2">
                          <h2>Contrato número {{contrato.numero}}</h2>
                        </v-col>
                        <v-col cols="12" md="12" class="pa-2">
                          <strong>Administrador del Contrato :</strong>
                          <u class="pl-2">{{contrato.adminContrato.nombreCompleto}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Fecha de Recepción:</strong>
                          <u class="pl-2">{{contrato.fechaDeRece}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>El contrato Vence el:</strong>
                          <u class="pl-2">{{contrato.fechaDeVenOfer}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Objeto Social :</strong>
                          <u class="pl-2">{{contrato.objetoDeContrato}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Término de Pago :</strong>
                          <u class="pl-1">{{contrato.terminoDePagoDet}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2" v-if="contrato.montoCup!=null">
                          <strong>Monto en CUP :</strong>
                          <u class="pl-2">{{contrato.montoCup}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2" v-if="contrato.montoCuc!=null">
                          <strong>Monto en CUC :</strong>
                          <u class="pl-2">{{contrato.montoCuc}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2" v-if="contrato.montoUsd!=null">
                          <strong>Monto en USD :</strong>
                          <u class="pl-2">{{contrato.montoUsd}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Estado :</strong>
                          <u class="pl-2">{{contrato.estadoNombre}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Formas e Pago :</strong>
                          <v-spacer></v-spacer>
                          <span v-for="item in contrato.formasDePago" :key="item.nombre">
                            <v-spacer></v-spacer>-
                            <u class="pl-2">{{item.nombre}}</u>
                          </span>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Especialistas Externos:</strong>
                          <span
                            v-for="item in contrato.especialistasExternos"
                            :key="item.nombreCompleto"
                            class="pl-2"
                          >
                            <v-spacer></v-spacer>-
                            <u class="pl-2">{{item.nombreCompleto}}</u>
                          </span>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Especialistas Internos:</strong>
                          <span
                            v-for="item in contrato.especialistasExternos"
                            :key="item.nombreCompleto"
                            class="pl-2"
                          >
                            <v-spacer></v-spacer>-
                            <u class="pl-2">{{item.nombreCompleto}}</u>
                          </span>
                        </v-col>
                        <v-col cols="6" md="6" class="pa-2">
                          <strong>Aprobado por el Jurídico :</strong>
                          <span v-if="contrato.aprobJuridico">
                            <v-icon color="success">mdi-check-underline</v-icon>
                            <v-text :class="`success--text`">Sí</v-text>
                          </span>
                          <span v-else>
                            <v-icon color="red">mdi-close-outline</v-icon>
                            <v-text :class="`red--text`">No</v-text>
                          </span>
                        </v-col>
                        <v-col cols="6" md="6" class="pa-2">
                          <strong>Aprobado por el Económico :</strong>
                          <span v-if="contrato.aprobEconomico">
                            <v-icon color="success">mdi-check-underline</v-icon>
                            <v-text :class="`success--text`">Sí</v-text>
                          </span>
                          <span v-else>
                            <v-icon color="red">mdi-close-outline</v-icon>
                            <v-text :class="`red--text`">No</v-text>
                          </span>
                        </v-col>
                        <v-col cols="6" md="6" class="pa-2">
                          <strong>Aprobado por el Comité Contratación:</strong>
                          <span v-if="contrato.aprobComitContratacion">
                            <v-text :class="`success--text`">Sí</v-text>
                            <v-icon color="success">mdi-check-underline</v-icon>
                          </span>
                          <span v-else>
                            <v-icon color="red">mdi-close-outline</v-icon>
                            <v-text :class="`red--text`">No</v-text>
                          </span>
                        </v-col>
                      </v-row>
                    </v-card-text>
                  </v-card>
                </v-col>
                <!-- /DATOS DE El contrato -->

                <!-- DATOS DE LA ENTIDAD PROVEEDORA -->
                <v-col cols="6">
                  <v-card :elevation="2" flat>
                    <v-card-text>
                      <v-row>
                        <v-col cols="12" md="12" class="pa-2">
                          <h2>Entidad Proveedora</h2>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Nombre :</strong>
                          <u class="pl-2">{{contrato.entidad.nombre}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Dirección :</strong>
                          <u class="pl-2">{{contrato.entidad.direccion}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>NIT :</strong>
                          <u class="pl-2">{{contrato.entidad.nit}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Sector :</strong>
                          <u class="pl-2">{{contrato.entidad.sectorNombre}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Fax :</strong>
                          <u class="pl-2">{{contrato.entidad.fax}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Correo :</strong>
                          <u class="pl-2">{{contrato.entidad.correo}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Objeto Social :</strong>
                          <u class="pl-2">{{contrato.entidad.objetoSocial}}</u>
                        </v-col>
                        <v-col cols="8" md="8" class="pa-2">
                          <strong>Teléfonos :</strong>
                          <v-spacer></v-spacer>
                          <span v-for="item in contrato.entidad.telefonos" :key="item.numero">
                            <strong class="pl-4">Número:</strong>
                            {{item.numero}}
                            <strong
                              class="pl-12"
                            >Extensión:</strong>
                            {{item.extension}}
                            <v-divider></v-divider>
                          </span>
                        </v-col>
                        <v-col cols="12" md="12" class="pa-2">
                          <strong>Cuentas Bancarias :</strong>
                          <v-data-table
                            :headers="headersCuentas"
                            :items="contrato.entidad.cuentasBancarias"
                            hide-default-footer
                            fixed-header
                            class="pt-3"
                          ></v-data-table>
                        </v-col>
                      </v-row>
                    </v-card-text>
                  </v-card>
                </v-col>
              </v-row>
              <!-- /DATOS DE LA ENTIDAD PROVEEDORA -->
            </v-container>
          </v-card>
        </v-dialog>
        <!-- /Detalles del Contrato -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small teal--text"
        small
        @click="getDetalles(item)"
      >
        <v-icon>mdi-format-list-bulleted</v-icon>
      </v-btn>
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
    menu: false,
    search: "",
    editedIndex: -1,
    tabs: null,
    contratos: [],
    contrato: {
      entidad: {},
      adminContrato: {},
      dictaminadores: [],
      especialistasExternos: []
    },
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
    ],
    headersCuentas: [
      {
        text: "Número de Cuenta",
        align: "left",
        sortable: true,
        value: "numeroCuenta"
      },
      { text: "Número Sucursal", value: "numeroSucursal" },
      { text: "Nombre Sucursal", value: "nombreSucursal" },
      { text: "Moneda", value: "moneda" }
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
      const url = api.getUrl(
        "contratacion",
        "Contratos?tipoTramite=contrato&cliente=true"
      );
      this.axios.get(url).then(
        response => {
          this.textByfiltro = "Contratos como Cliente";
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
      this.dialog3 = true;
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
      this.dialog3 = false;
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    save(method) {
      this.Contrato.cliente = true;
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
        this.Contrato.cliente = true;
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
      this.dialog3 = false;
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
      const url = api.getUrl(
        "contratacion",
        "contratos/VencimientoContrato?cliente=true"
      );
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
          "Contratos?tipoTramite=contrato&filtro=contratoTiempo&cliente=true"
        );
        this.textByfiltro = "Contratos en Tiempo";
      }
      if (filtro == "contratosProxVencer") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=contrato&filtro=contratosProxVencer&cliente=true"
        );
        this.textByfiltro = "Contratos Próximos a Vencer";
      }
      if (filtro == "contratosCasiVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=contrato&filtro=contratosCasiVenc&cliente=true"
        );
        this.textByfiltro = "Contratos Casi Vencidos";
      }
      if (filtro == "contratosVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=contrato&filtro=contratosVenc&cliente=true"
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