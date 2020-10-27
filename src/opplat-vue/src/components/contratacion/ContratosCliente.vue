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
              @click="newSuplemento(item)"
              slot="activator"
              v-if="(roles.includes('administrador de contratos')||roles.includes('administrador'))&&item.tipo!=12"
            >
              <v-icon>mdi-file-replace-outline</v-icon>
            </v-btn>
          </template>
          <span>Suplementar Contrato</span>
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
    editedIndex: -1,
    tabs: null,
    contratos: [],
    contrato: {
      esContrato: true
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
    roles: [],
    errors: [],
    headers: [
      { text: "Número", align: "left", sortable: true, value: "numero" },
      { text: "Nombre", sortable: true, value: "nombre" },
      { text: "Tipo", value: "tipoNombre" },
      { text: "Entidad", value: "entidad[0].nombre" },
      { text: "Sector", value: "entidad[0].sectorNombre" },
      { text: "Vence", value: "contVence" },
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
    this.getTiempoVenContratosFromApi();
  },

  methods: {
    getContratosFromApi() {
      const url = api.getUrl(
        "contratacion",
        `Contratos?tipoTramite=contrato&cliente=true&username=${this.username}&roles=${this.roles}`
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
      this.contrato.adminContrato = item.adminContrato;
      this.contrato.esContrato = true;
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
        contratoVence >= this.tiempoVenContratos.contratosProxVencerDesde &&
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
        `contratos/VencimientoContrato?cliente=true&username=${this.username}&roles=${this.roles}`
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
          `Contratos?tipoTramite=contrato&filtro=contratoTiempo&cliente=true&username=${this.username}&roles=${this.roles}`
        );
        this.textByfiltro = "Contratos en Tiempo";
      }
      if (filtro == "contratosProxVencer") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          `Contratos?tipoTramite=contrato&filtro=contratosProxVencer&cliente=true&username=${this.username}&roles=${this.roles}`
        );
        this.textByfiltro = "Contratos Próximos a Vencer";
      }
      if (filtro == "contratosCasiVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          `Contratos?tipoTramite=contrato&filtro=contratosCasiVenc&cliente=true&username=${this.username}&roles=${this.roles}`
        );
        this.textByfiltro = "Contratos Casi Vencidos";
      }
      if (filtro == "contratosVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          `Contratos?tipoTramite=contrato&filtro=contratosVenc&cliente=true&username=${this.username}&roles=${this.roles}`
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
    },
    newSuplemento(item) {
      var contrato = {};
      contrato.entidad = item.entidad[0];
      contrato.adminContrato = item.adminContrato.id;
      contrato.contratoId = item.id;
      contrato.formasDePago = [];
      contrato.especialistasExternos = [];
      contrato.departamentos = [];
      contrato.terminoDePago = item.terminoDePago;
      contrato.objetoDeContrato = item.objetoDeContrato;
      contrato.montos = item.montos;

      for (let index = 0; index < item.formasDePago.length; index++) {
        contrato.formasDePago[index] = item.formasDePago[index].id;
      }
      for (
        let index = 0;
        index < contrato.especialistasExternos.length;
        index++
      ) {
        contrato.especialistasExternos[index] =
          item.especialistasExternos[index].id;
      }
      for (let index = 0; index < item.departamentos.length; index++) {
        contrato.departamentos[index] = item.departamentos[index].id;
      }

      contrato.nombre = null;
      contrato.tipo = 12;
      contrato.cliente = true;
      contrato.esContrato = true;
      contrato.contratoPertenece = item.nombre;
      contrato.fechaDeVenOferta = null;
      contrato.fechaDeRecepcion = null;

      this.$router.push({
        name: "Nuevo_Contrato",
        query: {
          contrato
        }
      });
    }
  }
};
</script>