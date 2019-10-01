<template>
  <v-container>
    <v-form v-model="valid">
      <v-row>
        <v-col cols="12" md="4">
          <v-select
            v-model="mes"
            item-text="nombre"
            return-object
            :items="items"
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
    <v-flex lg12>
      <IngresosGastosTabla ref="tabla"></IngresosGastosTabla>
    </v-flex>
  </v-container>
</template>
<script>
import IngresosGastosTabla from '@/components/finanzas/reportes/IngresosGastosTabla';

export default {
  components: { IngresosGastosTabla },  
  data: () => ({
    data: null,
    valid: true,
    year: '',
    nameRules: [
      v => !!v || 'Este campo es requerido',
      v => (v && v.length <= 4) || 'El año debe tener 4 caracteres.',
    ],
    mes: '',
    items: [
      { id: 1, nombre: 'ENERO' },
      { id: 2, nombre: 'FEBRERO' },
      { id: 3, nombre: 'MARZO' },
      { id: 4, nombre: 'ABRIL' },
      { id: 5, nombre: 'MAYO' },
      { id: 6, nombre: 'JUNIO' },
      { id: 7, nombre: 'JULIO' },
      { id: 8, nombre: 'AGOSTO' },
      { id: 9, nombre: 'SEPTIEMBRE' },
      { id: 10, nombre: 'OCTUBRE' },
      { id: 11, nombre: 'NOVIEMBRE' },
      { id: 12, nombre: 'DICIEMBRE' },
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
      const data = this.mes;      
      this.$refs.tabla.loadReporte(data,this.year)
      
    },
  },
};
</script>
