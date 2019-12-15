<template>
  <v-container>
    <v-form v-model="valid" class="d-print-none">
      <v-row>
        <v-col cols="12" md="4">
          <v-select
            v-model="mes"
            item-text="nombre"
            return-object
            :items="meses"
            :rules="[v => !!v || 'Item is required']"
            label="MES"
            required
          ></v-select>
        </v-col>

        <v-col cols="12" md="4">
          <v-text-field v-model="year" :counter="4" :rules="nameRules" label="AÑO" required></v-text-field>
        </v-col>

        <v-col cols="12" md="4">
          <v-btn color="success" class="mr-4" @click="GenerarReporte">Generar Reporte</v-btn>
        </v-col>
      </v-row>
    </v-form>
    <v-card color="basil">
      <v-card-title class="text-center justify-center py-6">
        <h6
          class="font-weight-bold display-1 basil--text"
        >Estado Financiero mes:{{mes.id}} año:{{year}}</h6>
      </v-card-title>

      <v-tabs v-model="tab" background-color="transparent" color="basil" grow>
        <v-tab v-for="item in items" :key="item">{{ item }}</v-tab>
      </v-tabs>

      <v-tabs-items v-model="tab">
        <v-tab-item v-for="item in data" :key="item">
          <v-card flat color="basil">
            <v-card>
              <v-simple-table>
                <thead>
                  <tr>
                    <th>razon</th>
                    <th>valor</th>
                  </tr>
                </thead>
                <tbody v-for="estado in data" :key="estado.concepto">
                  <tr>
                    <td class="text-left">{{estado.concepto}}</td>
                    <td class="text-left">{{estado.efe}}</td>
                  </tr>
                  <tr v-for="detalle in estado.detalles" :key="detalle.razon">
                    <td>{{detalle.razon}}</td>
                    <td>{{detalle.valor}}</td>
                  </tr>
                </tbody>
              </v-simple-table>
            </v-card>
            <!-- <v-card-text>{{ item }}</v-card-text> -->
          </v-card>
        </v-tab-item>
      </v-tabs-items>
    </v-card>
  </v-container>
</template>

<script>
import api from "@/api";
export default {
  data() {
    return {
      data: null,
      valid: true,
      tab: null,
      items: ["5920", "5921", "5922", "5923", "5924", "5925", "5926"],
      text:
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
      year: "",
      nameRules: [
        v => !!v || "Este campo es requerido",
        v => (v && v.length <= 4) || "El año debe tener 4 caracteres."
      ],
      mes: "",
      estado: [],
      errors: [],
      meses: [
        { id: 1, nombre: "ENERO" },
        { id: 2, nombre: "FEBRERO" },
        { id: 3, nombre: "MARZO" },
        { id: 4, nombre: "ABRIL" },
        { id: 5, nombre: "MAYO" },
        { id: 6, nombre: "JUNIO" },
        { id: 7, nombre: "JULIO" },
        { id: 8, nombre: "AGOSTO" },
        { id: 9, nombre: "SEPTIEMBRE" },
        { id: 10, nombre: "OCTUBRE" },
        { id: 11, nombre: "NOVIEMBRE" },
        { id: 12, nombre: "DICIEMBRE" }
      ],
      lazy: false,
      data: [
        {
          concepto: "Activo Circulante",
          efe: "5920",
          detalles: [
            {
              razon: "Efectivo en Caja (101-108)",
              valor: 72799.42
            },
            {
              razon: "Efectivo en Banco y en otras instituciones (109-119)",
              valor: -125637.42
            },
            {
              razon: "Inversiones a Corto Plazo o Temporales (120-129)",
              valor: 0
            },
            {
              razon: "Efectos por Cobrar a Corto Plazo (130-133)",
              valor: 0
            },
            {
              razon: "Menos:Efectos por Cobrar Descontados (365-368)",
              valor: 0
            },
            {
              razon: "Cuenta en Participación (134)",
              valor: 0
            },
            {
              razon: "Cuentas por Cobrar a Corto Plazo (135-139 y 154)",
              valor: -492414.05
            },
            {
              razon: "Menos: Provisión para Cuentas Incobrables (369)",
              valor: 0
            },
            {
              razon: "Pagos por Cuentas de Terceros (140)",
              valor: 0
            },
            {
              razon:
                "Participación de Reaseguradoras por Siniestros Pendientes (141)",
              valor: 0
            },
            {
              razon:
                "Préstamos y Otras Operaciones Crediticias a Cobrar a Corto Plazo (142)",
              valor: 0
            },
            {
              razon: "Suscriptores de Bonos (143)",
              valor: 0
            },
            {
              razon: "Pagos Anticipados a Suministradores (146-149)",
              valor: 27600
            },
            {
              razon: "Pagos Anticipados del Proceso Inversionista (150-153)",
              valor: 0
            },
            {
              razon: "Anticipos a Justificar (161-163)",
              valor: 0
            },
            {
              razon: "Adeudos del Presupuesto del Estado (164-166)",
              valor: -1386610.07
            },
            {
              razon: "Adeudos del Organo u Organismo (167-170)",
              valor: 0
            },
            {
              razon: "Ingresos acumulados por Cobrar (173-180)",
              valor: 0
            },
            {
              razon: "Dividendos y Participaciones o por Cobrar (181)",
              valor: 0
            },
            {
              razon:
                "Ingresos Acumulados por Cobrar - Reaseguros Aceptados (182)",
              valor: 0
            }
          ]
        },
        {
          concepto: "Total de Inventario",
          efe: "5920",
          detalles: [
            {
              razon: "Materias Primas y Materiales (183)",
              valor: -39986.22
            },
            {
              razon: "Combustibles y Lubricantes (184)",
              valor: 0
            },
            {
              razon: "Partes y Piezas de Respuesto (185)",
              valor: 0
            },
            {
              razon: "Envases y Embalajes (186)",
              valor: 0
            },
            {
              razon: "Utiles, Herramientas y Otros (187)",
              valor: 240
            },
            {
              razon: "Menos: Desgaste de Utiles y Herramientas (373)",
              valor: 0
            },
            {
              razon: "Producción Terminada (188)",
              valor: 0
            },
            {
              razon: "Mercancias para la Venta (189)",
              valor: 0
            },
            {
              razon: "Menos: Descuento comercial e Impuesto (370-372)",
              valor: 0
            },
            {
              razon: "Medicamentos (190)",
              valor: 0
            },
            {
              razon: "Base Material de Estudio (191)",
              valor: 0
            },
            {
              razon: "Menos: Desgaste de Base Material de Estudio (366)",
              valor: 0
            },
            {
              razon: "Vestuario y Lencería (192)",
              valor: 0
            },
            {
              razon: "Menos: Desgaste de Vestuario y Lenceria (367)",
              valor: 0
            },
            {
              razon: "Alimentos (193)",
              valor: 0
            },
            {
              razon: "Inventario de Mercancías de Importación (194)",
              valor: 0
            },
            {
              razon: "Inventario de Mercancias de Exportación (195)",
              valor: 0
            },
            {
              razon: "Producciones para Insumo o Autoconsumo (196)",
              valor: 0
            },
            {
              razon: "Otros Inventarios (205-207)",
              valor: 0
            },
            {
              razon: "Inventarios Ociosos (208)",
              valor: 0
            },
            {
              razon: "Inventarios de lento Movimiento (209)",
              valor: 0
            },
            {
              razon: "Producción en Proceso (700-724)",
              valor: 8591.73
            },
            {
              razon: "Produccion Propia para Insumo o Autoconsumo (725)",
              valor: 0
            },
            {
              razon: "Reparaciones Capitales con Medios Propios (726)",
              valor: 0
            },
            {
              razon: "Inversiones con Medios Propios (728)",
              valor: 0
            },
            {
              razon: "Créditos Documentarios (211)",
              valor: 0
            }
          ]
        },
        {
          concepto: "Activos a Largo Plazo",
          efe: "5920",
          detalles: [
            {
              razon: "Efectos por Cobrar a Largo Plazo (215-217)",
              valor: 0
            },
            {
              razon: "Cuentas por Cobrar a Largo Plazo (218-220)",
              valor: 0
            },
            {
              razon: "Préstamos Concedidos a Cobrar a Largo Plazo (221-224)",
              valor: 0
            },
            {
              razon: "Cuentas en Participación (418-420)",
              valor: 0
            }
          ]
        }
      ]
    };
  },
  methods: {
    GenerarReporte() {
      console.log(this.mes.nombre, this.year);
    }
  }
};
</script>
<style scoped>
/* Helper classes */
.basil {
  background-color: #fffbe6 !important;
}
.basil--text {
  color: #356859 !important;
  font-weight: bold;
  font-size: large;
}
</style>
