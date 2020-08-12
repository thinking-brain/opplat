<template>
    <v-row>
    <!-- Formulario Ofertas -->
      <v-card xs12 md6 flat class="flex xs12 sm6 md6 px-5 pt-10" width="600px">
        <v-form>
          <p class="text-center title font-italic">Tiempo de Vencimiento de las Ofertas</p>
          <v-row class="px-10">
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenOferta.ofertasProxVencDesde"
                label="Ofertas Próximas a Vencer"
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenOferta.ofertasProxVencHasta"
                label="Ofertas Próximas a Vencer"
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenOferta.ofertasCasiVencDesde"
                label="Ofertas Casi Vencidas"
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenOferta.ofertasCasiVencHasta"
                label="Ofertas Casi Vencidas"
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenOferta.ofertaTiempo"
                label="Ofertas en Tiempo"
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenOferta.ofertasVencidas"
                label="Ofertas Vencidas"
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
          </v-row>
          <div class="text-center">
            <v-btn
              class="ma-2"
              color="success"
              outlined
              text
              @click="editTiempoVenOfertas"
            >Guardar Cambios</v-btn>
            <v-btn class="ma-2" color="cyan" outlined text @click="inicializarOfertas">Por Defecto</v-btn>
          </div>
        </v-form>
      </v-card>
    <!-- /Formulario Ofertas -->
    <!-- Formulario Contratos -->
      <v-card xs12 md6 flat class="flex xs12 sm6 md6 px-5 pt-10" width="600px">
        <v-form>
          <p class="text-center title font-italic">Tiempo de Vencimiento de los Contratos</p>
          <v-row class="px-10">
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenContrato.contratosProxVencerDesde"
                label="Próximos a Vencer"
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenContrato.contratosProxVencerHasta"
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row class="px-10">
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenContrato.contratosCasiVencDesde"
                label="Casi Vencidos"
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenContrato.contratosCasiVencHasta"
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row class="px-10">
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenContrato.contratoTiempo"
                label="En Tiempo"
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <v-text-field
                v-model="tiempoVenContrato.contratosVencidos"
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
          </v-row>
          <div class="text-center">
            <v-btn
              class="ma-2"
              color="success"
              outlined
              text
              @click="editTiempoVenContratos"
            >Guardar Cambios</v-btn>
            <v-btn class="ma-2" color="cyan" outlined text @click="inicializarContratos">Por Defecto</v-btn>
          </div>
        </v-form>
      </v-card>
    <!-- /Formulario Contratos -->
    </v-row>
</template>
<script>
import api from "@/api";
export default {
  data: () => ({
    tiempoVenOfertas: [],
    tiempoVenOferta: {},
    tiempoVenContratos: [],
    tiempoVenContrato: {}
  }),
  created() {
    this.getTiempoVenOfertasFromApi();
    this.getTiempoVenContratosFromApi();
  },
  methods: {
    getTiempoVenOfertasFromApi() {
      const url = api.getUrl("contratacion", "TiempoVenOfertas");
      this.axios.get(url).then(
        response => {
          this.tiempoVenOfertas = response.data;
          this.tiempoVenOferta = {
            id: this.tiempoVenOfertas[0].id,
            ofertaTiempo: this.tiempoVenOfertas[0].ofertaTiempo,
            ofertasProxVencDesde: this.tiempoVenOfertas[0].ofertasProxVencDesde,
            ofertasProxVencHasta: this.tiempoVenOfertas[0].ofertasProxVencHasta,
            ofertasCasiVencDesde: this.tiempoVenOfertas[0].ofertasCasiVencDesde,
            ofertasCasiVencHasta: this.tiempoVenOfertas[0].ofertasCasiVencHasta,
            ofertasVencidas: this.tiempoVenOfertas[0].ofertasVencidas
          };
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
          this.tiempoVenContratos = response.data;
          this.tiempoVenContrato = {
            id: this.tiempoVenContratos[0].id,
            contratoTiempo: this.tiempoVenContratos[0].contratoTiempo,
            contratosProxVencerDesde: this.tiempoVenContratos[0]
              .contratosProxVencerDesde,
            contratosProxVencerHasta: this.tiempoVenContratos[0]
              .contratosProxVencerHasta,
            contratosCasiVencDesde: this.tiempoVenContratos[0]
              .contratosCasiVencDesde,
            contratosCasiVencHasta: this.tiempoVenContratos[0]
              .contratosCasiVencHasta,
            contratosVencidos: this.tiempoVenContratos[0].contratosVencidos
          };
        },
        error => {
          console.log(error);
        }
      );
    },
    inicializarOfertas() {
      const url = api.getUrl("contratacion", "TiempoVenOfertas");
      this.tiempoVenOferta = {
        ofertaTiempo: 23,
        ofertasProxVencDesde: 7,
        ofertasProxVencHasta: 23,
        ofertasCasiVencDesde: 0,
        ofertasCasiVencHasta: 6,
        ofertasVencidas: 0
      };
      this.axios.post(url, this.tiempoVenOferta).then(
        response => {
          this.getResponse(response);
          this.getTiempoVenOfertasFromApi();
        },
        error => {
          console.log(error);
        }
      );
    },
    inicializarContratos() {
      const url = api.getUrl("contratacion", "TiempoVenContratos");
      this.tiempoVenContrato = {
        contratoTiempo: 23,
        contratosProxVencerDesde: 7,
        contratosProxVencerHasta: 23,
        contratosCasiVencDesde: 0,
        contratosCasiVencHasta: 6,
        contratosVencidos: 0
      };
      this.axios.post(url, this.tiempoVenContrato).then(
        response => {
          this.getResponse(response);
          this.getTiempoVenContratosFromApi();
        },
        error => {
          console.log(error);
        }
      );
    },
    editTiempoVenOfertas() {
      const url = api.getUrl("contratacion", "TiempoVenOfertas");
      this.axios
        .put(`${url}/${this.tiempoVenOferta.id}`, this.tiempoVenOferta)
        .then(
          response => {
            this.getResponse(response);
            this.getTiempoVenOfertasFromApi();
          },
          error => {
            console.log(error);
          }
        );
    },
    editTiempoVenContratos() {
      const url = api.getUrl("contratacion", "TiempoVenContratos");
      this.axios
        .put(`${url}/${this.tiempoVenContrato.id}`, this.tiempoVenContrato)
        .then(
          response => {
            this.getResponse(response);
            this.getTiempoVenContratosFromApi();
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
    }
  }
};
</script>