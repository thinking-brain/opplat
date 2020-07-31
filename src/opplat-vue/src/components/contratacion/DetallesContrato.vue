<template>
  <v-container grid-list-xl fluid>
    <p
      class="text-center text-uppercase headline font-weight-black"
    >OFERTA DE {{contrato.tipoNombre}} DE {{contrato.nombre}}.</p>
    <v-row>
      <!-- DATOS DE LA OFERTA -->
      <v-col cols="6">
        <v-card :elevation="2">
          <v-row class="mx-0">
            <v-col cols="12" md="12" class="pa-2 headline">
              <h3>Oferta</h3>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Vence en:</strong>
              {{contrato.ofertVence}} Días
            </v-col>
          </v-row>
          <v-row class="mx-0">
            <v-col cols="12" md="6" class="pa-2">
              <strong>Número :</strong>
              <u class="pl-2">{{contrato.numero}}</u>
            </v-col>
            <v-col cols="12" md="6" class="pa-2">
              <strong>Estado :</strong>
              <u class="pl-2">{{contrato.estadoNombre}}</u>
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
              <strong>La Oferta Vence el:</strong>
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
            <v-col cols="12" md="6" class="pa-2">
              <strong>Monto del Contrato :</strong>
              <v-spacer></v-spacer>
              <span v-for="item in contrato.montos" :key="item.nombre">
                <v-spacer></v-spacer>-
                <u class="pl-2">${{item.cantidad}} en</u>
                <u class="pl-2">{{item.nombreString}}</u>
              </span>
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
      <v-col cols="6">
        <v-card :elevation="2">
          <v-row class="mx-0">
            <v-col cols="12" md="12" class="pa-2 headline">
              <h3>Entidad Proveedora</h3>
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
                <v-row>
                  <v-col cols="6">
                    <strong>Número:</strong>
                    {{item.numero}}
                  </v-col>
                  <v-col cols="6">
                    <strong>Extensión:</strong>
                    {{item.extension}}
                  </v-col>
                </v-row>
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
        </v-card>
      </v-col>
      <!-- /DATOS DE LA ENTIDAD PROVEEDORA -->
    </v-row>
    <div class="text-center">
      <v-btn color="blue darken-1" text @click=" close()">Volver al Listado</v-btn>
    </div>
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
      { text: "Moneda", value: "nombreString" }
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