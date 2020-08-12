<template>
  <v-container grid-list-xl fluid>
    <p
      class="text-center text-uppercase headline font-weight-black"
    >OFERTA DE {{contrato.tipoNombre}} DE {{contrato.nombre}}.</p>
    <v-row>
      <!-- DATOS DE LA OFERTA -->
      <v-col>
        <v-card xs12 sm12 :elevation="2">
          <v-row class="mx-1">
            <v-col cols="12" md="12" class="pa-2 headline">
              <h3>Oferta</h3>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Vence en:</strong>
              {{contrato.ofertVence}} Días
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
              <strong>Objeto Social :</strong>
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
              <strong>Formas e Pago :</strong>
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
            <v-col cols="12" md="12" class="pa-2">
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
            <v-col cols="12" md="12" class="pa-2">
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
            <v-col cols="12" md="6" class="pa-2">
              <strong>Objeto Social :</strong>
              {{contrato.entidad.objetoSocial}}
            </v-col>
            <div cols="12" md="12" class="pa-2">
              <strong>Teléfonos :</strong>
              <div v-for="item in contrato.entidad.telefonos" :key="item.numero" class="pt-1">
                    <strong>Número:</strong>
                    {{item.numero}}<v-spacer></v-spacer>
                    <strong >Extensión:</strong>
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
export default {
  props: ["contrato"],
  data: () => ({
    enTiempo: 0,
    casiVenc: 0,
    proxVencer: 0,
    vencidos: 0,
    tiempoVenOfertas: {},
    ofertaTiempo: "ofertaTiempo",
    ofertasProxVencer: "ofertasProxVencer",
    ofertasCasiVenc: "ofertasCasiVenc",
    ofertasVenc: "ofertasVenc",
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
    ]
  }),
  computed: {},
  created() {},
  methods: {
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
    }
  }
};
</script>