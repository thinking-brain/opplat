<template>
  <v-container fluid>
    <v-form v-model="valid" class="d-print-none" ref="form">
      <v-row justify="center">
        <v-col cols="12" md="2">
          <v-text-field v-model="year_form" :counter="4" :rules="yearRules" label="AÑO" required></v-text-field>
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
          <v-select
            v-model="tipo_plan_selected"
            :items="tipos_plan"
            :rules="[v => !!v || 'Este campo es requerido']"
            label="TIPO"
          ></v-select>
        </v-col>

        <v-col cols="12" md="3">
          <v-btn color="success" class="mr-4" @click="GenerarReporte">Generar Reporte</v-btn>
        </v-col>
      </v-row>
    </v-form>
    <v-row justify="center">
      <v-col md="11">
        <v-card v-if="visible" color="basil" style="overflow:auto">
          <v-card-title class="text-center justify-center py-6">
            <h6
              class="font-weight-bold display-1 basil--text"
            >Estado Financiero {{mes.nombre}} {{year}}</h6>
          </v-card-title>
          <v-simple-table :fixed-header="true" height="700">
            <template v-slot:default>
              <thead>
                <tr>
                  <th rowspan="2" class="headcol"></th>
                  <th colspan="2" class="text-center">{{mes.nombre}}</th>
                </tr>
                <tr>
                  <th class="text-center">PLAN</th>
                  <th class="text-center">REAL</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in data" :key="item.efe" :title="item.concepto">
                  <td :class="item.encabezado ? 'font-weight-bold' : ''">{{item.concepto}}</td>
                  <td
                    :class="item.encabezado ? 'font-weight-bold text-center' : 'text-center'"
                  >{{item.plan}}</td>
                  <td
                    :class="item.encabezado ? 'font-weight-bold text-center' : 'text-center'"
                  >{{item.real}}</td>
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
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import api from "@/api";
export default {
  data() {
    return {
      datatable: null,
      data: [],
      valid: true,
      tab: null,
      tipos_plan: ["5920", "5921", "5922", "5923", "5924", "5925", "5926"],
      tipo_plan_selected: "",
      selectedMonths: "",
      year_form: "",
      mes: "",
      year: "",
      yearRules: [
        v => !!v || "Este campo es requerido",
        v => /^[0-9]+$/.test(v) || "Solo números",
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
      if (this.$refs.form.validate()) {
        const url = api.getUrl(
          "finanzas",
          `EstadoFinanciero/estadoFinanciero${this.tipo_plan_selected}Report/${this.year}/${this.mes.id}`
        );
        this.axios
          .get(url)
          .then(response => {
            this.data = response.data;
          })
          .catch(e => {
            this.errors.push(e);
            vm.$snotify.error(
              "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
            );
          });
        this.visible = true;
      }
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
