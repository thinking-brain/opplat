<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex lg12>
        <v-card ref="form">
          <v-card-text>
            <v-form ref="form" v-model="valid" :lazy-validation="lazy">
              <v-select
                v-model="mes"
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
      v => (v && v.length <= 4) || "Name must be less than 10 characters"
    ],
    mes: "",
    items: [
      "ENERO",
      "FEBRERO",
      "MARZO",
      "ABRIL",
      "MAYO",
      "JUNIO",
      "JULIO",
      "AGOSTO",
      "SEPTIEMBRE",
      "OCTUBRE",
      "NOVIEMBRE",
      "DICIEMBRE"
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