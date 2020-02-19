<template>
  <v-container>
    <v-form v-model="valid" class="d-print-none">
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
          <v-btn
            color="success"
            class="mr-4"
            @click="GenerarReporte"
            :loading="loading"
          >Actualizar Datos</v-btn>
        </v-col>
      </v-row>
    </v-form>
  </v-container>
</template>
<script>
import api from '@/api';

export default {
  data: () => ({
    data: null,
    valid: true,
    loading: false,
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
    resetValidation() {
      this.$refs.form.resetValidation();
    },
    GenerarReporte() {
      this.loading = true;
      this.formHasErrors = false;
      if (!this.formHasErrors) {
        const url = api.getUrl('finanzas', `updatecache/${this.mes.id}/${this.year}`);
        this.axios
          .get(url)
          .then(() => {
            this.loading = false;
            vm.$snotify.success('Se actualizaron los datos correctamente.');
          })
          .catch((err) => {
            this.loading = false;
            vm.$snotify.error(`Error actualizando los datos contables. ${err}`);
          });
      }
    },
  },
};
</script>
