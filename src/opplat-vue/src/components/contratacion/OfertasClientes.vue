<template>
  <v-container>
    <v-data-table
      :headers="headers"
      :items="ofertas"
      :search="search"
      dense
      class="elevation-1 pa-5"
    >
      <template v-slot:item.ofertVence="{ item }">
        <v-chip :color="getColor(item.ofertVence)" dark>{{ item.ofertVence }} días</v-chip>
      </template>
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-toolbar-title>{{textByfiltro}}</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>
          <v-btn
            color="primary"
            @click="newContrato()"
            class="ml-5"
            v-if="(roles.includes('administrador de contratos')||roles.includes('administrador'))"
          >Nueva Oferta</v-btn>
          <!-- Buscar -->
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
          <!-- /Buscar -->

          <v-spacer></v-spacer>

          <!-- Todas las Ofertas -->
          <v-badge :content="cantOfertas" :value="cantOfertas" color="primary" overlap class="mt-4">
            <template v-slot:badge>
              <span v-if="enTiempo > 0">{{ cantOfertas }}</span>
            </template>
            <v-tooltip top color="primary">
              <template v-slot:activator="{ on }">
                <v-icon
                  medium
                  v-on="on"
                  color="primary"
                  @click="getOfertasFromApi()"
                >mdi-file-document-box-multiple-outline</v-icon>
              </template>
              <span>Todas las Ofertas</span>
            </v-tooltip>
          </v-badge>
          <!-- /Todas las Ofertas -->

          <!-- Cantidad de Ofertas Ok -->
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
                  @click="filtro(ofertaTiempo)"
                >mdi-file-document-box-multiple-outline</v-icon>
              </template>
              <span>Ofertas en Tiempo</span>
            </v-tooltip>
          </v-badge>
          <!-- /Cantidad de Ofertas Ok -->

          <!-- Cantidad de Ofertas Casi Vencidos -->
          <v-badge
            :content="proxVencer"
            :value="proxVencer"
            color="orange"
            overlap
            class="mt-4 ml-4"
          >
            <template v-slot:badge>
              <span v-if="proxVencer > 0">{{ proxVencer}}</span>
            </template>
            <v-tooltip top color="orange">
              <template v-slot:activator="{ on }">
                <v-icon
                  medium
                  v-on="on"
                  color="orange"
                  @click="filtro(ofertasProxVencer)"
                >mdi-file-document-box-multiple-outline</v-icon>
              </template>
              <span>Ofertas Próximas a vencer</span>
            </v-tooltip>
          </v-badge>
          <!-- /Cantidad de Ofertas Casi Vencidos -->

          <!-- Cantidad de Ofertas proxVencer -->
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
                  @click="filtro(ofertasCasiVenc)"
                >mdi-file-document-box-multiple-outline</v-icon>
              </template>
              <span>Ofertas casi vencidas</span>
            </v-tooltip>
          </v-badge>
          <!-- /Cantidad de Ofertas proxVencer -->

          <!-- Cantidad de Ofertas Vencidas -->
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
                  @click="filtro(ofertasVenc)"
                >mdi-file-document-box-multiple-outline</v-icon>
              </template>
              <span>Ofertas Vencidas</span>
            </v-tooltip>
          </v-badge>
          <!-- /Cantidad de Ofertas Vencidas -->

          <!-- Aprobar oferta -->
          <v-dialog v-model="dialog4" persistent max-width="350px">
            <v-toolbar dark fadeOnScroll color="red">
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-card>
              <v-card-title class="headline text-center">Seguro que deseas aprobar la Oferta</v-card-title>
              <v-card-text class="text-center">{{oferta.nombre}}</v-card-text>
              <v-flex
                cols="2"
                class="px-12"
                v-if="(roles.includes('secretario comite de contratacion')||roles.includes('administrador'))"
              >
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
                      v-model="aprobarContrato.fechaDeFirmado"
                      label="Fecha de Firmado"
                      readonly
                      clearable
                      v-on="on"
                      required
                    ></v-text-field>
                  </template>
                  <v-date-picker v-model="aprobarContrato.fechaDeFirmado" @input="menu = false"></v-date-picker>
                </v-menu>
              </v-flex>
              <v-flex
                cols="2"
                class="px-12"
                v-if="(roles.includes('secretario comite de contratacion')||roles.includes('administrador'))"
              >
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
                      v-model="aprobarContrato.FechaDeVencimiento"
                      label="Fecha de Vencimiento"
                      readonly
                      clearable
                      v-on="on"
                      required
                    ></v-text-field>
                  </template>
                  <v-date-picker
                    v-model="aprobarContrato.FechaDeVencimiento"
                    @input="menu1 = false"
                  ></v-date-picker>
                </v-menu>
              </v-flex>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="red" dark @click="aprobarOferta()">Aceptar</v-btn>
                <v-btn color="primary" @click="close()">Cancelar</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
          <!-- /Aprobar oferta -->
          <!-- Delete oferta -->
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
              <v-card-title class="headline text-center">Seguro que deseas eliminar la Oferta</v-card-title>
              <v-card-text class="text-center">{{oferta.nombre}}</v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="red" dark @click="deleteItem(oferta)">Aceptar</v-btn>
                <v-btn color="primary" @click="close()">Cancelar</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
          <!-- /Delete oferta -->

          <!-- Subir Documento -->
          <v-row justify="center">
            <v-dialog v-model="dialog3" persistent max-width="400">
              <v-card>
                <v-toolbar dark fadeOnScroll color="blue darken-3">
                  <v-spacer></v-spacer>
                  <v-toolbar-items>
                    <v-btn icon dark @click=" close()">
                      <v-icon>mdi-close</v-icon>
                    </v-btn>
                  </v-toolbar-items>
                </v-toolbar>
                <v-flex cols="2" class="px-1 mt-10">
                  <v-file-input
                    v-model="file"
                    show-size
                    prepend-icon="mdi-note-multiple"
                    label="Seleccione el Documento"
                  ></v-file-input>
                </v-flex>
                <v-flex cols="2" class="px-1">
                  <v-alert v-if="message" border="left" color="red" dark>{{ message }}</v-alert>
                </v-flex>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="green darken-1" text @click="upload()">Aceptar</v-btn>
                  <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
                </v-card-actions>
              </v-card>
            </v-dialog>
          </v-row>
          <!-- /Subir Documento -->
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
                v-if="(roles.includes('administrador de contratos')||roles.includes('administrador'))"
              >
                <v-icon>v-icon notranslate mdi mdi-pen theme--dark</v-icon>
              </v-btn>
            </template>
            <span>Editar</span>
          </v-tooltip>
          <!-- Edit jurídico -->
            <EditJuridico v-if="roles.includes('juridico')" v-bind:oferta="item"></EditJuridico>
          <!-- /Edit jurídico -->
          <v-tooltip top color="black">
            <template v-slot:activator="{ on }">
              <v-btn
                class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small secondary--text"
                small
                v-on="on"
                @click="confirmUpload(item)"
                v-if="(roles.includes('administrador de contratos')||roles.includes('administrador'))"
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
          <v-tooltip top color="red darken-3">
            <template v-slot:activator="{ on }">
              <v-btn
                class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small red darken-3--text"
                small
                v-on="on"
                @click="download(item)"
                v-if="item.filePath ==null"
              >
                <v-icon>v-icon notranslate mdi mdi-download theme--dark</v-icon>
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
          <v-tooltip top color="pink">
            <template v-slot:activator="{ on }">
              <v-btn
                class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small pink--text"
                small
                v-on="on"
                @click="confirmDelete(item)"
                v-if="(roles.includes('administrador de contratos')||roles.includes('administrador'))"
              >
                <v-icon>v-icon notranslate mdi mdi-delete theme--dark</v-icon>
              </v-btn>
            </template>
            <span>Eliminar</span>
          </v-tooltip>
        </v-row>
      </template>
      <!-- /Actions -->
    </v-data-table>
  </v-container>
</template>
<script>
import api from "@/api";
import EditJuridico from "@/components/contratacion/EditJuridico.vue";

export default {
  components: {
    EditJuridico
  },
  data: () => ({
    dialog1: false,
    dialog2: false,
    dialog3: false,
    dialog4: false,
    dialog4: false,
    menu: false,
    menu1: false,
    search: "",
    editedIndex: -1,
    ofertas: [],
    cantOfertas: 0,
    oferta: {
      entidad: {},
      adminContrato: {},
      montos: [],
      username: ""
    },
    file: null,
    estados: [],
    tipos: [],
    enTiempo: 0,
    casiVenc: 0,
    proxVencer: 0,
    vencidos: 0,
    todasLasOfertas: false,
    vencimientoOfertas: [],
    monedas: [],
    ofertaTiempo: "ofertaTiempo",
    ofertasProxVencer: "ofertasProxVencer",
    ofertasCasiVenc: "ofertasCasiVenc",
    ofertasVenc: "ofertasVenc",
    tiempoVenOfertas: {},
    urlByfiltro: "",
    textByfiltro: "",
    show: false,
    tabs: null,
    errors: [],
    roles: [],
    username: {},
    aprobarContrato: {
      roles: [],
      contratoId: null,
      fechaDeFirmado: null,
      FechaDeVencimiento: null
    },
    headers: [
      { text: "Número", sortable: true, value: "numero" },
      { text: "Nombre", align: "left", sortable: true, value: "nombre" },
      { text: "Tipo", value: "tipoNombre" },
      { text: "Vence en", value: "ofertVence" },
      { text: "Estado", value: "estadoNombre" },
      { text: "Acciones", value: "action", sortable: false }
    ],
    message: ""
  }),
  computed: {},
  watch: {},

  created() {
    this.roles = this.$store.getters.roles;
    this.username = this.$store.getters.usuario;
    console.log("roles: " + this.roles);
    console.log("username: " + this.username);

    this.getOfertasFromApi();
    this.getTiempoVenOfertasFromApi();
    this.getMonedasFromApi();
    this.getVencimientoOferta();
  },

  methods: {
    getOfertasFromApi() {
      const url = api.getUrl(
        "contratacion",
        `Contratos?tipoTramite=oferta&cliente=true&username=${this.username}&roles=${this.roles}`
      );
      this.axios.get(url).then(
        response => {
          this.textByfiltro = "Ofertas de los Clientes";
          this.ofertas = response.data;
          this.cantOfertas = this.ofertas.length;
        },
        error => {
          console.log(error);
        }
      );
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
    },
    getMonedasFromApi() {
      const url = api.getUrl("contratacion", "Entidades/Monedas");
      this.axios.get(url).then(
        response => {
          this.monedas = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    newContrato() {
      const contrato = {
        cliente: true,
        adminContrato: {},
        dictaminadores: [],
        montos: [],
        especialistasExternos: [],
        username: null
      };
      contrato;
      this.$router.push({
        name: "Nuevo_Contrato",
        query: {
          contrato
        }
      });
    },
    editItem(item) {
      this.editedIndex = this.ofertas.indexOf(item);
      this.oferta = Object.assign({}, item);
      this.oferta.entidad = item.entidad[0];
      this.oferta.adminContrato = item.adminContrato.id;

      for (let index = 0; index < this.oferta.formasDePago.length; index++) {
        this.oferta.formasDePago[index] = item.formasDePago[index].id;
      }
      const contrato = this.oferta;
      this.$router.push({
        name: "Nuevo_Contrato",
        query: {
          contrato
        }
      });
    },
    getDetalles(item) {
      this.oferta = Object.assign({}, item);
      this.oferta.entidad = item.entidad[0];
      this.oferta.adminContrato = item.adminContrato;
      const contrato = this.oferta;
      this.$router.push({
        name: "Detalles_Contrato",
        query: {
          contrato
        }
      });
    },
    getUsuarioFromApi() {
      const username = this.$store.getters.usuario;
      const url = api.getUrl(
        "api-account",
        `account/perfil?usuario=${username}`
      );
      this.axios
        .get(url)
        .then(
          (
            p // console.log(p)
          ) => {
            this.usuario = p.data;
          }
        )
        .catch(e => {
          vm.$snotify.error(e.response.data.errors);
        });
    },
    confirmUpload(item) {
      this.oferta = Object.assign({}, item);
      this.dialog3 = true;
    },
    upload() {
      if (!this.file) {
        this.message = "Por favor seleccione un archivo!";
        return;
      }
      this.message = "";
      const formData = new FormData();
      formData.append("file", this.file);
      const url = api.getUrl("contratacion", "contratos/UploadFile");
      this.axios
        .post(`${url}/${this.oferta.id}`, formData, {
          headers: {
            "Content-Type": "multipart/form-data"
          }
        })
        .then(
          response => {
            this.getResponse(response);
            location.reload();
            this.close();
          },
          error => {
            vm.$snotify.error(error.response.data);
            console.log(error);
          }
        );
    },
    download(item) {
      const url = api.getUrl("contratacion", "contratos/DownloadFile");
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
    confirmDelete(item) {
      this.oferta = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(oferta) {
      const url = api.getUrl("contratacion", "Contratos");
      this.axios.delete(`${url}/${oferta.id}`).then(
        response => {
          this.getResponse(response);
          this.getOfertasFromApi();
          this.dialog2 = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    close() {
      this.dialog1 = false;
      this.dialog2 = false;
      this.dialog3 = false;
      this.dialog4 = false;
      this.aprobarContrato = {
        roles: null,
        contratoId: null,
        fechaDeFirmado: null
      };
      this.getOfertasFromApi();
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    getColor(ofertVence) {
      this.getVencimientoOferta();
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
    getVencimientoOferta() {
      const url = api.getUrl(
        "contratacion",
        `Contratos/VencimientoOferta?cliente=true&username=${this.username}&roles=${this.roles}`
      );
      this.axios.get(url).then(
        response => {
          this.vencimientoOfertas = response.data;
          this.vencidos = this.vencimientoOfertas[0];
          this.casiVenc = this.vencimientoOfertas[1];
          this.proxVencer = this.vencimientoOfertas[2];
          this.enTiempo = this.vencimientoOfertas[3];
        },
        error => {
          console.log(error);
        }
      );
    },
    filtro(filtro) {
      if (filtro == "ofertaTiempo") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          `Contratos?tipoTramite=oferta&filtro=ofertaTiempo&cliente=true&username=${this.username}&roles=${this.roles}`
        );
        this.textByfiltro = "Ofertas en Tiempo";
      }
      if (filtro == "ofertasProxVencer") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          `Contratos?tipoTramite=oferta&filtro=ofertasProxVencer&cliente=true&username=${this.username}&roles=${this.roles}`
        );
        this.textByfiltro = "Ofertas Próximas a Vencer";
      }
      if (filtro == "ofertasCasiVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          `Contratos?tipoTramite=oferta&filtro=ofertasCasiVenc&cliente=true&username=${this.username}&roles=${this.roles}`
        );
        this.textByfiltro = "Ofertas Casi Vencidas";
      }
      if (filtro == "ofertasVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          `Contratos?tipoTramite=oferta&filtro=ofertasVenc&cliente=true&username=${this.username}&roles=${this.roles}`
        );
        this.textByfiltro = "Ofertas Vencidas";
      }
      this.axios.get(this.urlByfiltro).then(
        response => {
          this.ofertas = response.data;
          vm.$snotify.success(this.textByfiltro);
          this.todasLasOfertas = true;
        },
        error => {
          console.log(error);
        }
      );
    },
    confirmAprobarOferta(item) {
      this.oferta = Object.assign({}, item);
      this.dialog4 = true;
    },
    aprobarOferta() {
      const url = api.getUrl("contratacion", "contratos/AprobContrato");
      this.aprobarContrato.contratoId = this.oferta.id;
      this.aprobarContrato.roles = this.roles;
      this.aprobarContrato.username = this.username;
      if (
        this.aprobarContrato.fechaDeFirmado == null &&
        this.aprobarContrato.FechaDeVencimiento == null
      ) {
        var fechaporDefecto = new Date("01/01/0001");
        this.aprobarContrato.fechaDeFirmado = fechaporDefecto;
        this.aprobarContrato.FechaDeVencimiento = fechaporDefecto;
      }
      this.axios
        .put(`${url}/${this.aprobarContrato.contratoId}`, this.aprobarContrato)
        .then(
          response => {
            this.getResponse(response);
            this.close();
          },
          error => {
            console.log(error);
            vm.$snotify.error(error.response.data);
          }
        );
    }
  }
};
</script>