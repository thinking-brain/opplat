<template>
  <v-container>
    <v-form v-model="valid" class="d-print-none">
      <v-row>
        <v-col cols="12" md="4">
          <v-text-field v-model="year" :counter="4" :rules="nameRules" label="AÑO" required></v-text-field>
        </v-col>
        <v-col cols="12" md="4">
          <v-btn color="success" class="mr-4" @click="GenerarReporte">Generar Reporte</v-btn>
        </v-col>
      </v-row>
    </v-form>
    <v-flex lg12>
      <RazonesFinancierasTabla ref="tabla"></RazonesFinancierasTabla>
    </v-flex>
  </v-container>
</template>
<script>
import RazonesFinancierasTabla from '@/components/finanzas/reportes/RazonesFinancierasTabla';
export default {
  components: { RazonesFinancierasTabla },
  data: () => ({
    data: null,
    valid: true,
    year: '',
    nameRules: [
      v => !!v || 'Este campo es requerido',
      v => (v && v.length <= 4) || 'El año debe tener 4 caracteres.',
    ],
    lazy: false,
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
      this.$refs.tabla.loadReporte(this.year)
    },
  },
};
</script>
