<template>
  <v-container grid-list-xl fluid>
    <p
      class="text-center text-uppercase headline font-weight-black"
    >{{Title}} DE {{contrato.tipoNombre}} DE {{contrato.nombre}}.</p>
    <v-row>
      <!-- DATOS DE LA OFERTA -->
      <v-col>
        <v-card xs12 sm12 flat>
          <v-row class="mx-1">
            <v-col cols="12" md="12" class="pa-2 headline">
              <h3>Datos {{subTitle}}</h3>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Vence en:</strong>
              {{contrato.ofertVence}} Días
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Tipo:</strong>
              {{contrato.tipoNombre}}
            </v-col>
          </v-row>
          <v-row class="mx-1">
            <v-col cols="12" md="6" class="pa-2">
              <strong>Número :</strong>
              {{contrato.numero}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Estado :</strong>
              {{contrato.estadoNombre}}
            </v-col>
            <v-col cols="12" md="12" class="pa-2">
              <strong>Administrador del Contrato :</strong>
              {{contrato.adminContrato.nombreCompleto}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Fecha de Recepción:</strong>
              {{contrato.fechaDeRece}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>La Oferta Vence el:</strong>
              {{contrato.fechaDeVenOfer}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Objeto del Contrato :</strong>
              {{contrato.objetoDeContrato}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Término de Pago :</strong>
              {{contrato.terminoDePagoDet}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Monto del Contrato :</strong>
              <v-spacer></v-spacer>
              <span v-for="item in contrato.montos" :key="item.nombre">
                <v-spacer></v-spacer>
                -
                ${{item.cantidad}} en
                {{item.nombreString}}
              </span>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Formas de Pago :</strong>
              <v-spacer></v-spacer>
              <span v-for="item in contrato.formasDePago" :key="item.nombre">
                <v-spacer></v-spacer>
                -
                {{item.nombre}}
              </span>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Especialistas Externos:</strong>
              <span
                v-for="item in contrato.especialistasExternos"
                :key="item.nombreCompleto"
                class="pl-2"
              >
                <v-spacer></v-spacer>
                -
                {{item.nombreCompleto}}
              </span>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Especialistas Internos:</strong>
              <span
                v-for="item in contrato.especialistasExternos"
                :key="item.nombreCompleto"
                class="pl-2"
              >
                <v-spacer></v-spacer>
                -
                {{item.nombreCompleto}}
              </span>
            </v-col>
            <v-col cols="12" md="12" class="pa-2">
              <hr />
              <strong>Estado {{subTitle}}:</strong>
            </v-col>
            <v-col cols="12" md="6">
              <strong>Jurídico:</strong>
              <u>{{contrato.estadoJuridicoNombre}}</u>
            </v-col>
            <v-col cols="12" md="6">
              <strong>Económico:</strong>
              <u>{{contrato.estadoEconomicoNombre}}</u>
            </v-col>
            <v-col cols="12" md="12">
              <strong>Comité Contratación:</strong>
              <u>{{contrato.estadoComitContratacionNombre}}</u>
            </v-col>
          </v-row>
        </v-card>
      </v-col>
      <!-- /DATOS DE LA OFERTA -->
      <!-- DATOS DE LA ENTIDAD PROVEEDORA -->
      <v-col>
        <v-card xs12 sm12 :elevation="2">
          <v-row class="mx-1">
            <v-col cols="12" md="12" class="pa-2 headline">
              <h3>Entidad Proveedora</h3>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Nombre :</strong>
              {{contrato.entidad.nombre}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Dirección :</strong>
              {{contrato.entidad.direccion}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>NIT :</strong>
              {{contrato.entidad.nit}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Sector :</strong>
              {{contrato.entidad.sectorNombre}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Fax :</strong>
              {{contrato.entidad.fax}}
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Correo :</strong>
              {{contrato.entidad.correo}}
            </v-col>
            <v-col cols="12" md="12" class="pa-2">
              <strong>Objeto Social :</strong>
              {{contrato.entidad.objetoSocial}}
            </v-col>
            <div cols="12" md="12" class="pa-2">
              <strong>Teléfonos :</strong>
              <div
                v-for="item in contrato.entidad.telefonos"
                :key="item.numero"
                cols="12"
                md="12"
                class="pt-1"
              >
                <strong>Número:</strong>
                {{item.numero}}
                <strong class="ml-2">Ext:</strong>
                {{item.extension}}
              </div>
            </div>
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
        </v-card>
      </v-col>
      <!-- /DATOS DE LA ENTIDAD PROVEEDORA -->
    </v-row>
    <v-tabs v-model="tab" class="elevation-2">
      <v-tab class="caption py-3">Dictámenes</v-tab>
      <v-tab class="caption py-3">Suplementos</v-tab>
      <v-tab-item>
        <!-- Dictamenes -->
        <v-row class="mx-1">
          <v-col cols="12" md="12" class="pa-2">
            <v-data-table
              :headers="headersDictamen"
              :items="contrato.dictamenes"
              hide-default-footer
              fixed-header
              class="pt-3"
              dense
              :search="search"
            >
              <template v-slot:top>
                <v-toolbar flat color="white">
                  <v-divider class="mx-1" inset vertical></v-divider>
                  <!-- Buscar -->
                  <v-text-field
                    v-model="search"
                    append-icon="mdi-magnify"
                    label="Buscar"
                    single-line
                    hide-details
                    clearable
                    dense
                    class="px-3"
                  ></v-text-field>
                  <!-- /Buscar -->
                  <v-spacer></v-spacer>
                  <v-spacer></v-spacer>
                </v-toolbar>
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
                        v-if="roles.includes('juridico')||roles.includes('economico')||roles.includes('secretario comite de contratacion')||roles.includes('dictaminador')"
                      >
                        <v-icon>v-icon notranslate mdi mdi-pen theme--dark</v-icon>
                      </v-btn>
                    </template>
                    <span>Editar</span>
                  </v-tooltip>
                  <v-tooltip top color="black">
                    <template v-slot:activator="{ on }">
                      <v-btn
                        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small secondary--text"
                        small
                        v-on="on"
                        @click="confirmUpload(item)"
                        v-if="roles.includes('juridico')||roles.includes('economico')||roles.includes('secretario comite de contratacion')||roles.includes('dictaminador')"
                      >
                        <v-icon>v-icon notranslate mdi mdi-upload theme--dark</v-icon>
                      </v-btn>
                    </template>
                    <span>Guardar Documento</span>
                  </v-tooltip>
                  <v-tooltip top color="black">
                    <template v-slot:activator="{ on }">
                      <v-btn
                        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small secondary--text"
                        small
                        v-on="on"
                        @click="download(item)"
                        v-if="item.filePath !=null"
                      >
                        <v-icon>v-icon notranslate mdi mdi-download theme--dark</v-icon>
                      </v-btn>
                    </template>
                    <span>Descargar Documento</span>
                  </v-tooltip>
                  <v-tooltip top color="warning">
                    <template v-slot:activator="{ on }">
                      <v-btn
                        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small warning darken-3--text"
                        small
                        v-on="on"
                        @click="download(item)"
                        v-if="item.filePath ==null"
                      >
                        <v-icon>mdi-file-question-outline</v-icon>
                      </v-btn>
                    </template>
                    <span>No tiene un documento guardado</span>
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
                  <!-- <v-tooltip top color="pink">
                <template v-slot:activator="{ on }">
                  <v-btn
                    class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small pink--text"
                    small
                    v-on="on"
                    @click="confirmDelete(item)"
                  >
                    <v-icon>v-icon notranslate mdi mdi-delete theme--dark</v-icon>
                  </v-btn>
                </template>
                <span>Eliminar</span>
                  </v-tooltip>-->
                </v-row>
              </template>
              <!-- /Actions -->
            </v-data-table>
            <!-- Editar Dictamen-->
            <v-dialog v-model="dialog" persistent max-width="900">
              <v-card>
                <v-toolbar dark fadeOnScroll color="blue darken-3">
                  <v-spacer></v-spacer>
                  <v-toolbar-items>
                    <v-btn icon dark @click="dialog=false">
                      <v-icon>mdi-close</v-icon>
                    </v-btn>
                  </v-toolbar-items>
                </v-toolbar>
                <v-card-text>
                  <p class="text-left title">Datos a llenar del Dictamen:</p>
                  <v-row>
                    <v-layout row wrap class="px-3">
                      <v-flex cols="2" md3 class="px-3 pt-3">
                        <v-text-field
                          v-model="dictamen.id"
                          label="Número de Dictamen"
                          :error-messages="messagesNumDictamen"
                          prefix="#"
                          readonly
                        ></v-text-field>
                      </v-flex>
                      <v-flex cols="2" md9 class="px-3">
                        <v-textarea v-model="dictamen.observaciones" label="Observaciones" rows="1"></v-textarea>
                      </v-flex>
                      <v-flex cols="2" md6 class="px-3">
                        <v-textarea
                          v-model="dictamen.consideraciones"
                          label="Consideraciones"
                          rows="2"
                        ></v-textarea>
                      </v-flex>
                      <v-flex cols="2" md6 class="px-3">
                        <v-textarea
                          v-model="dictamen.recomendaciones"
                          label="Recomendaciones"
                          rows="2"
                        ></v-textarea>
                      </v-flex>
                      <v-flex cols="2" xs12 md6 class="px-3">
                        <v-file-input
                          v-model="file"
                          show-size
                          prepend-icon="mdi-note-multiple"
                          label="Seleccione el dictamen del contrato"
                        ></v-file-input>
                      </v-flex>
                      <v-flex cols="2" md6 class="px-3">
                        <v-text-field
                          v-model="dictamen.fundamentosDeDerecho"
                          label="Fundamentos de Derecho"
                        ></v-text-field>
                      </v-flex>
                      <v-flex cols="2" md6 class="px-1">
                        <v-alert v-if="message" border="left" color="red" dark>{{ message }}</v-alert>
                      </v-flex>
                    </v-layout>
                  </v-row>
                  <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="green darken-1" dark text @click="save()">Aceptar</v-btn>
                    <v-btn text @click="dialog=false">Cancelar</v-btn>
                  </v-card-actions>
                </v-card-text>
              </v-card>
            </v-dialog>
            <!-- /Editar Dictamen-->

            <!-- Detalles del dictamen -->
            <v-dialog v-model="dialog1" persistent max-width="900">
              <v-card>
                <v-toolbar dark fadeOnScroll color="blue darken-3">
                  <v-spacer></v-spacer>
                  <v-toolbar-items>
                    <v-btn icon dark @click="dialog1=false">
                      <v-icon>mdi-close</v-icon>
                    </v-btn>
                  </v-toolbar-items>
                </v-toolbar>
                <v-card-text>
                  <p class="text-center title mt-2">Detalles del Dictamen:</p>
                  <v-row class="mx-1">
                    <v-col cols="12" md="6" class="pa-2">
                      <strong>Número :</strong>
                      {{dictamen.id}}
                    </v-col>
                    <v-col cols="12" md="6" class="pa-2">
                      <strong>Pertenece al contrato :</strong>
                      {{contrato.nombre}}
                    </v-col>
                    <v-col cols="12" md="6" class="pa-2">
                      <strong>Observaciones :</strong>
                      {{dictamen.observaciones}}
                    </v-col>
                    <v-col cols="12" md="6" class="pa-2">
                      <strong>Consideraciones :</strong>
                      {{dictamen.consideraciones}}
                    </v-col>
                    <v-col cols="12" md="6" class="pa-2">
                      <strong>Recomendaciones</strong>
                      {{dictamen.recomendaciones}}
                    </v-col>
                    <v-col cols="12" md="6" class="pa-2">
                      <strong>FundamentosDeDerecho</strong>
                      {{dictamen.fundamentosDeDerecho}}
                    </v-col>
                    <v-col cols="12" md="6" class="pa-2">
                      <strong>Fecha del Dictamen :</strong>
                      {{dictamen.fecha}}
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </v-dialog>
            <!-- /Detalles del dictamen -->
          </v-col>
        </v-row>
        <!-- /Dictamenes -->
      </v-tab-item>
      <v-tab-item>
        <!-- Suplementos -->
        <v-row class="mx-1">
          <v-col cols="12" md="12" class="pa-2">
            <v-data-table
              :headers="headerSuplementos"
              :items="suplementos"
              hide-default-footer
              fixed-header
              class="pt-3"
              :search="search"
              flat
            >
              <template v-slot:item.ofertVence="{ item }">
                <v-chip :color="getColor(item.ofertVence)" dark>{{ item.ofertVence }} días</v-chip>
              </template>
              <template v-slot:top>
                <v-toolbar flat color="white">
                  <v-divider class="mx-1" inset vertical></v-divider>
                  <!-- Buscar -->
                  <v-text-field
                    v-model="search"
                    append-icon="mdi-magnify"
                    label="Buscar"
                    single-line
                    hide-details
                    clearable
                    dense
                    class="px-3"
                  ></v-text-field>
                  <!-- /Buscar -->
                  <v-spacer></v-spacer>
                  <v-spacer></v-spacer>
                </v-toolbar>
              </template>
            </v-data-table>
          </v-col>
        </v-row>
        <!-- /Suplementos -->
      </v-tab-item>
    </v-tabs>
    <v-row>
      <v-col>
        <div class="text-center">
          <v-btn color="blue darken-1" text @click=" close()">Volver al Listado</v-btn>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>
<script>
import api from "@/api";

export default {
  props: ["contrato"],
  components: {},
  data: () => ({
    dialog: false,
    dialog1: false,
    search: "",
    enTiempo: 0,
    casiVenc: 0,
    proxVencer: 0,
    vencidos: 0,
    tiempoVenOfertas: {},
    ofertaTiempo: "ofertaTiempo",
    ofertasProxVencer: "ofertasProxVencer",
    ofertasCasiVenc: "ofertasCasiVenc",
    ofertasVenc: "ofertasVenc",
    dictamen: {},
    file: "",
    suplementos: [],
    tiempoVenOfertas: [],
    tabs: null,
    tab: null,
    textOfertaVence: {
      text: null,
      class: null
    },
    headersCuentas: [
      {
        text: "Número de Cuenta",
        align: "left",
        sortable: true,
        value: "numeroCuenta"
      },
      { text: "Número Sucursal", value: "numeroSucursal" },
      { text: "Nombre Sucursal", value: "nombreSucursalString" },
      { text: "Moneda", value: "monedaString" }
    ],
    headersDictamen: [
      {
        text: "Número",
        align: "left",
        sortable: true,
        value: "id"
      },
      { text: "Observaciones", value: "observaciones" },
      { text: "Dictaminó", value: "dictaminador.nombreCompleto" },
      { text: "Fecha", value: "fecha" },
      { text: "Acciones", value: "action", sortable: false }
    ],
    headerSuplementos: [
      { text: "Identificador", sortable: true, value: "id" },
      { text: "Nombre", sortable: true, value: "nombre" },
      { text: "Motivo Suplemento", value: "motivoSuplemento" },
      { text: "Fecha de Recepción", value: "fechaDeRece" },
      { text: "Vence", value: "ofertVence" }
      // { text: "Acciones", value: "action", sortable: false }
    ],
    message: "",
    messagesNumDictamen: "",
    roles: [],
    username: ""
  }),
  computed: {
    Title() {
      return this.contrato.esContrato === true ? "Contrato" : "Oferta";
    },
    subTitle() {
      return this.contrato.esContrato === true
        ? "del Contrato"
        : "de la Oferta";
    }
  },
  created() {
    this.getSuplementosFromApi();
    this.getTiempoVenOfertasFromApi();
    this.roles = this.$store.getters.roles;
    this.username = this.$store.getters.usuario;
  },
  methods: {
    editItem(item) {
      this.dictamen = item;
      this.dialog = true;
    },
    save() {
      const formData = new FormData();
      formData.append("file", this.file);
      const url = api.getUrl(
        "contratacion",
        `Dictamenes?ContratoId=${this.contrato.id}&NumeroDeDictamen=${this.dictamen.numero}
        &Observaciones=${this.dictamen.observaciones}&FundamentosDeDerecho=${this.dictamen.fundamentosDeDerecho}
        &Consideraciones=${this.dictamen.consideraciones}&Recomendaciones=${this.dictamen.recomendaciones}&Username=${this.username}&Id=${this.dictamen.id}`
      );
      this.axios
        .put(url, formData, {
          headers: {
            "Content-Type": "multipart/form-data"
          }
        })
        .then(
          response => {
            this.getResponse(response);
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
    },
    getDetalles(item) {
      this.dialog1 = true;
      this.dictamen = item;
    },
    download(item) {
      const url = api.getUrl("contratacion", "dictamenes/DownloadFile");
      this.axios.get(`${url}/${item.id}`).then(
        response => {
          window.open(url + "/" + item.id);
        },
        error => {
          vm.$snotify.error(error.response.data);
          console.log(error);
        }
      );
    },
    close() {
      if (
        this.contrato.cliente == true &&
        this.contrato.aprobComitContratacion == true &&
        this.contrato.estadoNombre == "Aprobado"
      ) {
        this.$router.push({
          name: "ContratosCliente"
        });
      } else if (
        this.contrato.cliente == false &&
        this.contrato.aprobComitContratacion == true &&
        this.contrato.estadoNombre == "Aprobado"
      ) {
        this.$router.push({
          name: "ContratosPrestador"
        });
      } else if (this.contrato.cliente == true) {
        this.$router.push({
          name: "OfertasClientes"
        });
      } else
        this.$router.push({
          name: "OfertasPrestador"
        });
    },
    getSuplementosFromApi() {
      var username = this.$store.getters.usuario;
      const url = api.getUrl(
        "contratacion",
        `Contratos?cliente=true&contratoId=${this.contrato.id}`
      );
      this.axios.get(url).then(
        response => {
          this.suplementos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    getColor(ofertVence) {
      if (ofertVence < this.tiempoVenOfertas.ofertasVencidas) {
        return "red";
      } else if (
        ofertVence >= this.tiempoVenOfertas.ofertasCasiVencDesde &&
        ofertVence <= this.tiempoVenOfertas.ofertasCasiVencHasta
      ) {
        return "deep-orange";
      } else if (
        ofertVence >= this.tiempoVenOfertas.ofertasProxVencDesde &&
        ofertVence <= this.tiempoVenOfertas.ofertasProxVencHasta
      )
        return "orange";
      else {
        return "green";
      }
    },
    getTiempoVenOfertasFromApi() {
      const url = api.getUrl("contratacion", "TiempoVenOfertas");
      this.axios.get(url).then(
        response => {
          this.tiempoVenOfertas = response.data[0];
        },
        error => {
          console.log(error);
        }
      );
    }
  }
};
</script>