<template>
  <v-row>
    <!-- Formulario Ofertas -->
    <v-col cols="6" sm="6" md="6">
      <v-card :elevation="12">
        <v-form>
          <v-row align="center" justify="center" class="pt-3">
            <p class="text-center title font-italic">Tiempo de Vencimiento de las Ofertas</p>
            <v-col cols="6" sm="9" md="5">
              <v-text-field
                v-model="tiempoVenOferta.ofertasProxVencDesde"
                label="Ofertas Próximas a Vencer"
                outlined
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="6" sm="9" md="5">
              <v-text-field
                v-model="tiempoVenOferta.ofertasProxVencHasta"
                label="Ofertas Próximas a Vencer"
                outlined
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="6" sm="9" md="5">
              <v-text-field
                v-model="tiempoVenOferta.ofertasCasiVencDesde"
                label="Ofertas Casi Vencidas"
                outlined
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="6" sm="9" md="5">
              <v-text-field
                v-model="tiempoVenOferta.ofertasCasiVencHasta"
                label="Ofertas Casi Vencidas"
                outlined
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="6" sm="9" md="5">
              <v-text-field
                v-model="tiempoVenOferta.ofertaTiempo"
                label="Ofertas en Tiempo"
                outlined
                placeholder=" "
                prefix="Después de"
                suffix="Días"
              ></v-text-field>
            </v-col>
            <v-col cols="6" sm="9" md="5">
              <v-text-field
                v-model="tiempoVenOferta.ofertasVencidas"
                label="Ofertas Vencidas"
                outlined
                placeholder=" "
                prefix="Antes de"
                suffix="Días"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row justify="center">
            <v-col cols="6" sm="6" md="5">
              <v-btn
                color="success"
                block
                outlined
                class="elevation-7"
                @click="editTiempoVenOfertas"
              >Guardar Cambios</v-btn>
            </v-col>
            <v-col cols="6" sm="6" md="5">
              <v-btn
                color="cyan"
                block
                outlined
                class="elevation-7"
                @click="inicializarOfertas"
              >Por Defecto</v-btn>
            </v-col>
          </v-row>
        </v-form>
      </v-card>
    </v-col>
    <!-- /Formulario Ofertas -->

    <!-- Formulario Contratos -->
    <v-col cols="6" sm="6" md="6">
      <v-card :elevation="12">
        <v-form>
          <v-row align="center" justify="center" class="pt-3">
            <p class="text-center title font-italic">Tiempo de Vencimiento de los Contratos</p>
            <v-row class="px-12">
              <v-col cols="6" sm="9" md="6">
                <v-text-field
                  v-model="tiempoVenContrato.contratosProxVencerDesde"
                  label="Contratos Próximos a Vencer"
                  outlined
                  placeholder=" "
                  prefix="Después de"
                  suffix="Días"
                ></v-text-field>
              </v-col>
              <v-col cols="6" sm="9" md="6">
                <v-text-field
                  v-model="tiempoVenContrato.contratosProxVencerHasta"
                  label="Contratos Próximos a Vencer"
                  outlined
                  placeholder=" "
                  prefix="Antes de"
                  suffix="Días"
                ></v-text-field>
              </v-col>
            </v-row>
            <v-row class="px-12">
              <v-col cols="6" sm="9" md="6">
                <v-text-field
                  v-model="tiempoVenContrato.contratosCasiVencDesde"
                  label="Contratos Casi Vencidos"
                  outlined
                  placeholder=" "
                  prefix="Después de"
                  suffix="Días"
                ></v-text-field>
              </v-col>
              <v-col cols="6" sm="9" md="6">
                <v-text-field
                  v-model="tiempoVenContrato.contratosCasiVencHasta"
                  label="Contratos Casi Vencidos"
                  outlined
                  placeholder=" "
                  prefix="Antes de"
                  suffix="Días"
                ></v-text-field>
              </v-col>
            </v-row>
            <v-row class="px-12">
              <v-col cols="6" sm="9" md="6">
                <v-text-field
                  v-model="tiempoVenContrato.contratoTiempo"
                  label="Contratos en Tiempo"
                  outlined
                  placeholder=" "
                  prefix="Después de"
                  suffix="Días"
                ></v-text-field>
              </v-col>
              <v-col cols="6" sm="9" md="6">
                <v-text-field
                  v-model="tiempoVenContrato.contratosVencidos"
                  label="Contratos Vencidos"
                  outlined
                  placeholder=" "
                  prefix="Antes de"
                  suffix="Días"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-row>
          <v-row justify="center">
            <v-col cols="6" sm="6" md="5">
              <v-btn
                color="success"
                block
                outlined
                class="elevation-7"
                @click="editTiempoVenContratos"
              >Guardar Cambios</v-btn>
            </v-col>
            <v-col cols="6" sm="6" md="5">
              <v-btn
                color="cyan"
                block
                outlined
                class="elevation-7"
                @click="inicializarContratos"
              >Por defecto</v-btn>
            </v-col>
          </v-row>
        </v-form>
      </v-card>
    </v-col>
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