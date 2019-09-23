<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex lg12>
        <v-card ref="form">
          <v-card-text>
            <v-form ref="form" v-model="valid" :lazy-validation="lazy">
              <v-select
                v-model="mes"
                item-text="nombre"
                item-value="id"
                :items="items"
                :rules="[v => !!v || 'Item is required']"
                label="MES"
                required
              ></v-select>

              <v-text-field v-model="year" :counter="4" :rules="nameRules" label="AÑO" required></v-text-field>

              <v-btn color="success" class="mr-4" @click="GenerarReporte">Generar Reporte</v-btn>
            </v-form>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex lg12 v-if="data">
        <IngresosGastosTabla v-bind="data"></IngresosGastosTabla>
      </v-flex>
    </v-layout>
  </v-container>
</template>
<script>
import IngresosGastosTabla from "@/components/finanzas/reportes/IngresosGastosTabla";
export default {
  components: { IngresosGastosTabla },
  data: () => ({
    data: null,
    valid: true,
    year: "",
    nameRules: [
      v => !!v || "Este campo es requerido",
      v => (v && v.length <= 4) || "El año debe tener 4 caracteres."
    ],
    mes: "",
    items: [
      {id: 1, nombre: "ENERO"},
      {id: 2, nombre: "FEBRERO"},
      {id: 3, nombre: "MARZO"},
      {id: 4, nombre: "ABRIL"},
      {id: 5, nombre: "MAYO"},
      {id: 6, nombre: "JUNIO"},
      {id: 7, nombre: "JULIO"},
      {id: 8, nombre: "AGOSTO"},
      {id: 9, nombre: "SEPTIEMBRE"},
      {id: 10, nombre: "OCTUBRE"},
      {id: 11, nombre: "NOVIEMBRE"},
      {id: 12, nombre: "DICIEMBRE"},
    ],
    checkbox: false,
    lazy: false
  }),

  methods: {
    validate() {
      if (this.$refs.form.validate()) {
        this.snackbar = true;
      }
    },
    reset() {
      this.$refs.form.reset();
    },
    resetValidation() {
      this.$refs.form.resetValidation();
    },
    GenerarReporte() {
      this.data = {
        mes: this.mes,
        year: this.year
      };
    }
  }
};
</script>