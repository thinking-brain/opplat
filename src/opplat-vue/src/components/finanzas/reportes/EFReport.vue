<template>
  <v-container fluid>
    <v-form v-model="valid" class="d-print-none">
      <v-row justify="center">
        <v-col cols="12" md="2">
          <v-text-field v-model="year_form" :counter="4" :rules="nameRules" label="AÑO" required></v-text-field>
        </v-col>
        <v-col cols="12" md="2">
          <v-select
            v-model="selectedMonths"
            item-text="nombre"
            return-object
            :items="meses"
            :rules="[v => !!v || 'Este campo es requerido']"
            label="MES"
            required
          ></v-select>
          <!-- <v-select v-model="selectedMonths" :items="meses" item-text="nombre" label="MES" return-object multiple>
            <template v-slot:prepend-item>
              <v-list-item ripple @click="toggle">
                <v-list-item-action>
                  <v-icon :color="selectedMonths.length > 0 ? 'indigo darken-4' : ''">{{ icon }}</v-icon>
                </v-list-item-action>
                <v-list-item-content>
                  <v-list-item-title>Todos</v-list-item-title>
                </v-list-item-content>
              </v-list-item>
              <v-divider class="mt-2"></v-divider>
            </template>
            <template v-slot:append-item>
              <v-divider class="mb-2"></v-divider>
            </template>
          </v-select>-->
        </v-col>

        <v-col cols="12" md="2">
          <v-select :items="tipos_plan" label="TIPO"></v-select>
        </v-col>

        <v-col cols="12" md="3">
          <v-btn color="success" class="mr-4" @click="GenerarReporte">Generar Reporte</v-btn>
        </v-col>
      </v-row>
    </v-form>
    <v-card v-if="visible" color="basil" style="overflow:auto">
      <v-card-title class="text-center justify-center py-6">
        <h8 class="font-weight-bold display-1 basil--text">Estado Financiero {{mes.nombre}} {{year}}</h8>
      </v-card-title>
      <v-simple-table fixed-header="true" height="500">
        <template v-slot:default>
          <thead>
            <tr>
              <th rowspan="2" class="headcol"></th>
              <th colspan="2" class="text-center">{{selectedMonths.nombre}}</th>
            </tr>
            <tr>
              <th class="text-center">PLAN</th>
              <th class="text-center">REAL</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in data" :key="item.efe" :title="item.concepto">
              <td
                :class="item.encabezado ? 'font-weight-bold' : ''"
              >{{item.concepto}}</td>
              <td :class="item.encabezado ? 'font-weight-bold text-center' : 'text-center'">{{item.plan}}</td>
              <td :class="item.encabezado ? 'font-weight-bold text-center' : 'text-center'">{{item.real}}</td>
            </tr>
          </tbody>
        </template>
      </v-simple-table>
      <!-- <v-data-table :headers="header" :items="desserts" fixed-header height="400" hide-default-footer="true" disable-sort="true" item-key="name">
        <template slot="items" slot-scope="props">
          <tr>
            <td>{{ props.item.name }}</td>
            <td>{{ props.item.calories }}</td>
            <td>{{ props.item.fat }}</td>
            <td>{{ props.item.carbs }}</td>
            <td>{{ props.item.protein }}</td>
            <td>{{ props.item.nprotein }}</td>
            <td>{{ props.item.iron }}</td>
            <td>{{ props.item.niron }}</td>
          </tr>
        </template>
      </v-data-table>-->
    </v-card>
  </v-container>
</template>

<script>
import api from "@/api";
import EF_5920 from "@/components/finanzas/reportes/EF_5920";
export default {
  components: {
    EF_5920
  },
  data() {
    return {
      datatable: null,
      data: [
        {
          concepto: "Activo Circulante",
          efe: "5920",
          plan: 730332.29,
          real: -1411848.07,
          mes: 3,
          celda: "C5",
          encabezado: true
        },
        {
          concepto: "Efectivo en Caja  (101-108)",
          efe: "5920",
          plan: 140000,
          real: 72799.42,
          mes: 3,
          celda: "C6",
          encabezado: false
        },
        {
          concepto: "Efectivo en Banco y en otras instituciones (109-119)",
          efe: "5920",
          plan: 536000,
          real: -125637.42,
          mes: 3,
          celda: "C7",
          encabezado: false
        },
        {
          concepto: "Inversiones a Corto Plazo o Temporales (120-129)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C8",
          encabezado: false
        },
        {
          concepto: "Efectos por Cobrar a Corto Plazo (130-133)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C9",
          encabezado: false
        },
        {
          concepto: "Menos:Efectos por Cobrar Descontados (365)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C10",
          encabezado: false
        },
        {
          concepto: "Cuenta en Participación (134)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C11",
          encabezado: false
        },
        {
          concepto: "Cuentas por Cobrar a Corto Plazo (135-139 y 154)",
          efe: "5920",
          plan: 1117725,
          real: -492414.05,
          mes: 3,
          celda: "C12",
          encabezado: false
        },
        {
          concepto: "Menos: Provisión para Cuentas Incobrables (369)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C13",
          encabezado: false
        },
        {
          concepto: "Pagos por Cuentas de Terceros (140)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C14",
          encabezado: false
        },
        {
          concepto: "Participación de Reaseguradores por Siniestros Pend.(141)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C15",
          encabezado: false
        },
        {
          concepto:
            "Préstamos y Otras Operaciones Crediticias a Cobrar a Corto Plazo (142)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C16",
          encabezado: false
        },
        {
          concepto: "Suscriptores de Bonos (143)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C17",
          encabezado: false
        },
        {
          concepto: "Pagos Anticipados a Suministradores (146-149)",
          efe: "5920",
          plan: 50000,
          real: 27600,
          mes: 3,
          celda: "C18",
          encabezado: false
        },
        {
          concepto: "Pagos Anticipados del Proceso Inversionista (150-152)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C19",
          encabezado: false
        },
        {
          concepto: "Materiales Anticipados del Proceso Inversionista (153)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C20",
          encabezado: false
        },
        {
          concepto: "Anticipos a Justificar (161-163)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C21",
          encabezado: false
        },
        {
          concepto: "Adeudos del Presupuesto del Estado (164-166)",
          efe: "5920",
          plan: 4332.29,
          real: -1386610.07,
          mes: 3,
          celda: "C22",
          encabezado: false
        },
        {
          concepto: "Adeudos del Organo u Organismo (167-170)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C23",
          encabezado: false
        },
        {
          concepto: "Ingresos acumulados por Cobrar (173-180)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C24",
          encabezado: false
        },
        {
          concepto: "Dividendos y Participaciones por Cobrar (181)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C25",
          encabezado: false
        },
        {
          concepto:
            "Ingresos Acumulados por Cobrar - Reaseguros Aceptados (182)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C26",
          encabezado: false
        },
        {
          concepto: "Total de Inventario",
          efe: "5920",
          plan: 522891.61,
          real: -31154.49,
          mes: 3,
          celda: "C27",
          encabezado: true
        },
        {
          concepto: "Materias Primas y Materiales (183)",
          efe: "5920",
          plan: 264380.9,
          real: -39986.22,
          mes: 3,
          celda: "C28",
          encabezado: false
        },
        {
          concepto: "Combustibles y Lubricantes (184)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C29",
          encabezado: false
        },
        {
          concepto: "Partes y Piezas de Respuesto (185)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C30",
          encabezado: false
        },
        {
          concepto: "Envases y Embalajes (186)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C31",
          encabezado: false
        },
        {
          concepto: "Utiles, Herramientas y Otros (187)",
          efe: "5920",
          plan: 0,
          real: 240,
          mes: 3,
          celda: "C32",
          encabezado: false
        },
        {
          concepto: "Menos: Desgaste de Utiles y Herramientas (373)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C33",
          encabezado: false
        },
        {
          concepto: "Producción Terminada (188)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C34",
          encabezado: false
        },
        {
          concepto: "Mercancias para la Venta (189)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C35",
          encabezado: false
        },
        {
          concepto: "Menos: Descuento comercial e Impuesto (370-372)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C36",
          encabezado: false
        },
        {
          concepto: "Medicamentos (190)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C37",
          encabezado: false
        },
        {
          concepto: "Base Material de Estudio (191)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C38",
          encabezado: false
        },
        {
          concepto: "Menos: Desgaste de Base Material de Estudio (366)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C39",
          encabezado: false
        },
        {
          concepto: "Vestuario y Lencería (192)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C40",
          encabezado: false
        },
        {
          concepto: "Menos: Desgaste de Vestuario y Lenceria  (367)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C41",
          encabezado: false
        },
        {
          concepto: "Alimentos (193)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C42",
          encabezado: false
        },
        {
          concepto: "Inventario de Mercancías de Importación (194)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C43",
          encabezado: false
        },
        {
          concepto: "Inventario de Mercancias de Exportación (195)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C44",
          encabezado: false
        },
        {
          concepto: "Producciones para Insumo o Autoconsumo (196)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C45",
          encabezado: false
        },
        {
          concepto: "Otros Inventarios (205-207)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C46",
          encabezado: false
        },
        {
          concepto: "Inventarios Ociosos (208)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C47",
          encabezado: false
        },
        {
          concepto: "Inventarios de lento Movimiento (209)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C48",
          encabezado: false
        },
        {
          concepto: "Producción en Proceso (700-724)",
          efe: "5920",
          plan: 950.4,
          real: 8591.73,
          mes: 3,
          celda: "C49",
          encabezado: false
        },
        {
          concepto: "Producción Propia para Insumo (725)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C50",
          encabezado: false
        },
        {
          concepto: "Reparaciones Capitales con Medios Propios (726)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C51",
          encabezado: false
        },
        {
          concepto:
            "Inversiones con Medios Propios Activos Fijos Intangibles (727)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C52",
          encabezado: false
        },
        {
          concepto: "Inversiones con Medios Propios (728)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C53",
          encabezado: false
        },
        {
          concepto: "Créditos Documentarios (211)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C54",
          encabezado: false
        },
        {
          concepto: "Activos a Largo Plazo",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C55",
          encabezado: true
        },
        {
          concepto: "Efectos por Cobrar a Largo Plazo (215-217)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C56",
          encabezado: false
        },
        {
          concepto: "Cuentas por Cobrar a Largo Plazo (218-220)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C57",
          encabezado: false
        },
        {
          concepto: "Préstamos Concedidos a Cobrar a Largo Plazo (221-224)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C58",
          encabezado: false
        },
        {
          concepto: "Inversiones a Largo Plazo o Permanentes (225-234)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C59",
          encabezado: false
        },
        {
          concepto: "Activos Fijos",
          efe: "5920",
          plan: 10904263.21,
          real: 30992.24,
          mes: 3,
          celda: "C60",
          encabezado: false
        },
        {
          concepto: "Activos Fijos Tangibles (240-251)",
          efe: "5920",
          plan: 10987263.21,
          real: 104424,
          mes: 3,
          celda: "C61",
          encabezado: false
        },
        {
          concepto: "Menos:Depreciación de Activos Fijos Tangibles (375-388)",
          efe: "5920",
          plan: 0,
          real: 73431.76,
          mes: 3,
          celda: "C62",
          encabezado: false
        },
        {
          concepto: "Fondos Bibliotecarios (252)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C63",
          encabezado: false
        },
        {
          concepto: "Medios y Equipos para Alquilar  (253)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C64",
          encabezado: false
        },
        {
          concepto: "Menos: Desgaste de Medios y Equipos para Alquilar (389)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C65",
          encabezado: false
        },
        {
          concepto: "Monumentos y Obras de Arte (254)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C66",
          encabezado: false
        },
        {
          concepto: "Activos Fijos Intangibles (255 a 263)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C67",
          encabezado: false
        },
        {
          concepto: "Activos Fijos Intangibles en Proceso (264)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C68",
          encabezado: false
        },
        {
          concepto: "Menos:Amortización de Activos Fijos Intangibles (390-399)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C69",
          encabezado: false
        },
        {
          concepto: "Inversiones en Proceso (265-278)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C70",
          encabezado: false
        },
        {
          concepto: "Plan de Preparaci�n de Inversiones (279)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C71",
          encabezado: false
        },
        {
          concepto:
            "Equipos por Instalar y Materiales para el Proceso Inversionista (280-289)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C72",
          encabezado: false
        },
        {
          concepto: "Activos Diferidos",
          efe: "5920",
          plan: 1236614.65,
          real: 0,
          mes: 3,
          celda: "C73",
          encabezado: true
        },
        {
          concepto: "Gastos de Produccion y Servicios Diferidos (300-305)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C74",
          encabezado: false
        },
        {
          concepto: "Gastos Financieros Diferidos (306-307)",
          efe: "5920",
          plan: 913428.65,
          real: 0,
          mes: 3,
          celda: "C75",
          encabezado: false
        },
        {
          concepto: "Gastos Diferidos del Proceso Inversionista (310-311)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C76",
          encabezado: false
        },
        {
          concepto: "Gastos por Faltante y Perdidas Diferidas (312)",
          efe: "5920",
          plan: 323186,
          real: 0,
          mes: 3,
          celda: "C77",
          encabezado: false
        },
        {
          concepto: "Otros Activos",
          efe: "5920",
          plan: 18004,
          real: 16705.01,
          mes: 3,
          celda: "C78",
          encabezado: true
        },
        {
          concepto: "Pérdidas en Investigación (330-331)",
          efe: "5920",
          plan: 0,
          real: 10985.01,
          mes: 3,
          celda: "C79",
          encabezado: false
        },
        {
          concepto: "Faltante de Bienes en Investigación (332-333)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C80",
          encabezado: false
        },
        {
          concepto:
            "Cuentas por Cobrar Diversas-Operaciones Corrientes (334-341)",
          efe: "5920",
          plan: 18004,
          real: 5720,
          mes: 3,
          celda: "C81",
          encabezado: false
        },
        {
          concepto: "Cuentas por Cobrar- Compra de Monedas (342)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C82",
          encabezado: false
        },
        {
          concepto:
            "Cuentas por Cobrar Diversa del Proceso Inversionista (343-345)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C83",
          encabezado: false
        },
        {
          concepto: "Efectos por Cobrar en Litigio (346)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C84",
          encabezado: false
        },
        {
          concepto: "Cuentas por Cobrar en Litigio (347)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C85",
          encabezado: false
        },
        {
          concepto: "Efectos por Cobrar Protestados (348)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C86",
          encabezado: false
        },
        {
          concepto: "Cuentas por Cobrar en Proceso Judicial (349)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C87",
          encabezado: false
        },
        {
          concepto: "Depósitos y Fianzas (354-355)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C88",
          encabezado: false
        },
        {
          concepto:
            "Fondo de Amortización de Bonos - Efectivo en Valores (364)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C89",
          encabezado: false
        },
        {
          concepto: "Menos:Otras Provisiones Reguladoras de Activos (374)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C90",
          encabezado: false
        },
        {
          concepto: "TOTAL DEL ACTIVO",
          efe: "5920",
          plan: 12889214.15,
          real: -1364150.82,
          mes: 3,
          celda: "C91",
          encabezado: true
        },
        {
          concepto: "PASIVO",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C92",
          encabezado: true
        },
        {
          concepto: "Pasivo Circulante",
          efe: "5920",
          plan: 2752705.95,
          real: -1741797.99,
          mes: 3,
          celda: "C93",
          encabezado: true
        },
        {
          concepto: "Sobregiro Bancario (400)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C94",
          encabezado: false
        },
        {
          concepto: "Efectos por Pagar a Corto Plazo (401-404)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C95",
          encabezado: false
        },
        {
          concepto: "Cuentas por Pagar a Corto Plazo (405-415)",
          efe: "5920",
          plan: 406004,
          real: 94332.28,
          mes: 3,
          celda: "C96",
          encabezado: false
        },
        {
          concepto: "Cobros por Cuenta de Terceros (416)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C97",
          encabezado: false
        },
        {
          concepto: "Dividendos y Participaciones por Pagar (417)",
          efe: "5920",
          plan: 1654233,
          real: -1957.32,
          mes: 3,
          celda: "C98",
          encabezado: false
        },
        {
          concepto: "Cuentas en Participación (418-420)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C99",
          encabezado: false
        },
        {
          concepto: "Cuentas por Pagar-Activos Fijos Tangibles (421-424)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C100",
          encabezado: false
        },
        {
          concepto: "Cuentas por Pagar del Proceso Inversionista (425-429)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C101",
          encabezado: false
        },
        {
          concepto: "Cobros Anticipados (430-433)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C102",
          encabezado: false
        },
        {
          concepto: "Materiales Recibidos de Forma Anticipa (434)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C103",
          encabezado: false
        },
        {
          concepto: "Depósitos Recibidos (435-439)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C104",
          encabezado: false
        },
        {
          concepto: "Obligaciones con el Presupuesto del Estado (440-449)",
          efe: "5920",
          plan: 626254,
          real: -1696765.84,
          mes: 3,
          celda: "C105",
          encabezado: false
        },
        {
          concepto: "Obligaciones con el Organo u Organismo (450-453)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C106",
          encabezado: false
        },
        {
          concepto: "Nóminas por Pagar (455-459)",
          efe: "5920",
          plan: 65964,
          real: -13673.22,
          mes: 3,
          celda: "C107",
          encabezado: false
        },
        {
          concepto: "Retenciones por Pagar (460-469)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C108",
          encabezado: false
        },
        {
          concepto:
            "Préstamos Recibidos y Otras Operaciones Crediticias por Pagar (470-479)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C109",
          encabezado: false
        },
        {
          concepto: "Gastos Acumulados por Pagar (480-489)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C110",
          encabezado: false
        },
        {
          concepto: "Provisión para Vacaciones (491;492)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C111",
          encabezado: false
        },
        {
          concepto: "Otras Provosiones Operacionales (493-499)",
          efe: "5920",
          plan: 0,
          real: -30992.24,
          mes: 3,
          celda: "C112",
          encabezado: false
        },
        {
          concepto:
            "Provisión para Pagos de los Subsidios de Seguridad Social a Corto Plazo (500)",
          efe: "5920",
          plan: 250.95,
          real: -366.69,
          mes: 3,
          celda: "C113",
          encabezado: false
        },
        {
          concepto: "Fondo de Compensación para Desbalances Financieros (501)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C114",
          encabezado: false
        },
        {
          concepto: "Pasivos a Largo Plazo",
          efe: "5920",
          plan: 19890678.01,
          real: 92374.96,
          mes: 3,
          celda: "C115",
          encabezado: true
        },
        {
          concepto: "Efectos por Pagar a Largo Plazo (510-514)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C116",
          encabezado: false
        },
        {
          concepto: "Cuentas por Pagar a Largo Plazo (515-518)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C117",
          encabezado: false
        },
        {
          concepto: "Préstamos Recibidos por Pagar a Largo Plazo (520-524)",
          efe: "5920",
          plan: 0,
          real: 94332.28,
          mes: 3,
          celda: "C118",
          encabezado: false
        },
        {
          concepto: "Obligaciones a Largo Plazo (525-532)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C119",
          encabezado: false
        },
        {
          concepto: "Otras Provisiones a Largo Plazo (533-539)",
          efe: "5920",
          plan: 0,
          real: -1957.32,
          mes: 3,
          celda: "C120",
          encabezado: false
        },
        {
          concepto: "Bonos por Pagar (540)-(144)-(363)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C121",
          encabezado: false
        },
        {
          concepto: "Bonos Suscritos (541)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C122",
          encabezado: false
        },
        {
          concepto: "Pasivos Diferidos",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C123",
          encabezado: true
        },
        {
          concepto: "Ingresos Diferidos (545-548)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C124",
          encabezado: false
        },
        {
          concepto: "Ingresos Diferidos por Donaciones Recibidas (549)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C125",
          encabezado: false
        },
        {
          concepto: "Otros Pasivos",
          efe: "5920",
          plan: 0,
          real: 848.27,
          mes: 3,
          celda: "C126",
          encabezado: true
        },
        {
          concepto: "Sobrantes en Investigación (555-564)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C127",
          encabezado: false
        },
        {
          concepto: "Cuentas por Pagar Diversas (565-568)",
          efe: "5920",
          plan: 0,
          real: 848.27,
          mes: 3,
          celda: "C128",
          encabezado: false
        },
        {
          concepto: "Cuentas por Pagar- Compra de Moneda (569)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C129",
          encabezado: false
        },
        {
          concepto: "Ingresos de Períodos Futuros (570-574)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C130",
          encabezado: false
        },
        {
          concepto:
            "Obligaciones con el Presupuesto del Estado por Garantía Activada (575)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C131",
          encabezado: false
        },
        {
          concepto: "TOTAL PASIVO",
          efe: "5920",
          plan: 22643383.96,
          real: -1648574.76,
          mes: 3,
          celda: "C132",
          encabezado: true
        },
        {
          concepto: "PATRIMONIO NETO O CAPITAL CONTABLE",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C133",
          encabezado: true
        },
        {
          concepto: "Patrimonio y Fondo Común  (600-612) Sector Privado",
          efe: "5920",
          plan: 0,
          real: 190876.67,
          mes: 3,
          celda: "C134",
          encabezado: false
        },
        {
          concepto: "Inversión Estatal (600-612) Sector Público",
          efe: "5920",
          plan: 0,
          real: 190876.67,
          mes: 3,
          celda: "C135",
          encabezado: false
        },
        {
          concepto: "Capital Social Suscrito y Pagado",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C136",
          encabezado: true
        },
        {
          concepto: "Recursos Recibidos  (617-618) Sector Publico",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C137",
          encabezado: false
        },
        {
          concepto: "Donaciones Recibidas - Nacionales (620)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C138",
          encabezado: false
        },
        {
          concepto: "Donaciones Recibidas - Exterior (621)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C139",
          encabezado: false
        },
        {
          concepto: "Utilidad  Retenida (630-634)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C140",
          encabezado: false
        },
        {
          concepto: "Subvención por Pérdida  (635-639)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C141",
          encabezado: false
        },
        {
          concepto: "Reservas para Contingencias (645)",
          efe: "5920",
          plan: 3751.96635567345,
          real: -23545.78,
          mes: 3,
          celda: "C142",
          encabezado: false
        },
        {
          concepto: "Otras Reservas Patrimoniales (646-654)",
          efe: "5920",
          plan: 568570,
          real: -24194.88,
          mes: 3,
          celda: "C143",
          encabezado: false
        },
        {
          concepto: "Fondo de Contravalor para proyectos de Inversión (688)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C144",
          encabezado: false
        },
        {
          concepto: "Menos: Recursos Entregados (619) Sector Publico",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C145",
          encabezado: false
        },
        {
          concepto: "Donaciones Entregadas - Nacionales (626)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C146",
          encabezado: false
        },
        {
          concepto: "Donaciones Entregadas - Exterior (627)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C147",
          encabezado: false
        },
        {
          concepto: "Pago a Cuenta de Utilidades (693)",
          efe: "5920",
          plan: 1654233,
          real: 833396.62,
          mes: 3,
          celda: "C148",
          encabezado: false
        },
        {
          concepto: "Pago a Cuenta de Dividendos (691)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C149",
          encabezado: false
        },
        {
          concepto: "Pérdida (640-644)",
          efe: "5920",
          plan: 0,
          real: 500,
          mes: 3,
          celda: "C150",
          encabezado: false
        },
        {
          concepto:
            "Mas o Menos: Reevalucion de Activos Fijos Tangibles (613-615)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C151",
          encabezado: false
        },
        {
          concepto: "Otras Operaciones de Capital (616-619) Sector Privado",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C152",
          encabezado: false
        },
        {
          concepto: "Revaluación de Inventarios (697)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C153",
          encabezado: false
        },
        {
          concepto: "Ganancia o Pérdida no Realizada (698)",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C154",
          encabezado: false
        },
        {
          concepto: "Resultado del Período",
          efe: "5920",
          plan: 0,
          real: 0,
          mes: 3,
          celda: "C155",
          encabezado: true
        },
        {
          concepto: "TOTAL  PATRIMONIO NETO",
          efe: "5920",
          plan: -1081911.03364433,
          real: -499383.94,
          mes: 3,
          celda: "C156",
          encabezado: true
        },
        {
          concepto: "TOTAL DEL PASIVO Y PATRIMONIO NETO O CAPITAL CONTABLE",
          efe: "5920",
          plan: 35241960.7435105,
          real: -980049.4,
          mes: 3,
          celda: "C157",
          encabezado: true
        }
      ],
      valid: true,
      tab: null,
      items: ["5920", "5921", "5922", "5923", "5924", "5925", "5926"],
      selectedMonths: "",
      year_form: "",
      mes: "",
      year: "",
      tipos_plan: ["5920", "5921", "5924"],
      nameRules: [
        v => !!v || "Este campo es requerido",
        v => (v && v.length == 4) || "El año debe tener 4 caracteres."
      ],
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
      visible: false,
      lazy: false
    };
  },
  computed: {
    likesAllMonths() {
      return this.selectedMonths.length === this.meses.length;
    },
    likesSomeMonth() {
      return this.selectedMonths.length > 0 && !this.likesAllMonths;
    },
    icon() {
      if (this.likesAllMonths) return "mdi-close-box";
      if (this.likesSomeMonth) return "mdi-minus-box";
      return "mdi-checkbox-blank-outline";
    }
  },
  methods: {
    GenerarReporte() {
      this.year = this.year_form;
      this.mes = this.selectedMonths;
      this.visible = true;
    },
    toggle() {
      this.$nextTick(() => {
        if (this.likesAllMonths) {
          this.selectedMonths = [];
        } else {
          this.selectedMonths = this.meses.slice();
        }
      });
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
