<template>
  <v-container>
    <v-form v-model="valid" class="d-print-none">
      <v-row>
        <v-col cols="12" md="4">
          <v-select
            v-model="mes_form"
            item-text="nombre"
            return-object
            :items="meses"
            :rules="[v => !!v || 'Este campo es requerido']"
            label="MES"
            required
          ></v-select>
        </v-col>

        <v-col cols="12" md="4">
          <v-text-field v-model="year_form" :counter="4" :rules="nameRules" label="AÑO" required></v-text-field>
        </v-col>

        <v-col cols="12" md="4">
          <v-btn color="success" class="mr-4" @click="GenerarReporte">Generar Reporte</v-btn>
        </v-col>
      </v-row>
    </v-form>
    <v-card v-if="visible" color="basil">
      <v-card-title class="text-center justify-center py-6">
        <h6
          class="font-weight-bold display-1 basil--text"
        >Estado Financiero {{mes.nombre}} {{year}}</h6>
      </v-card-title>

      <v-tabs v-model="tab" background-color="transparent" color="basil" grow>
        <v-tab v-for="item in items" :key="item">{{ item }}</v-tab>
      </v-tabs>

      <v-tabs-items v-model="tab">
        <v-tab-item>
          <v-card flat color="basil">
            <v-card-text>5920</v-card-text>
          </v-card>
        </v-tab-item>
        <v-tab-item>
          <v-card flat color="basil">
            <EF_5920 :year="year" :mes="mes" />
          </v-card>
        </v-tab-item>
        <v-tab-item>
          <v-card flat color="basil">
            <v-card-text>5922</v-card-text>
          </v-card>
        </v-tab-item>
      </v-tabs-items>
    </v-card>
  </v-container>
</template>

<script>
import api from '@/api';
import EF_5920 from '@/components/finanzas/reportes/EF_5920';

export default {
  components: {
    EF_5920,
  },
  data() {
    return {
      data: null,
      valid: true,
      tab: null,
      items: ['5920', '5921', '5922', '5923', '5924', '5925', '5926'],
      mes_form: '',
      year_form: '',
      mes: '',
      year: '',
      nameRules: [
        v => !!v || 'Este campo es requerido',
        v => (v && v.length == 4) || 'El año debe tener 4 caracteres.',
      ],
      estado: [],
      errors: [],
      meses: [
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
      visible: false,
      lazy: false,
    };
  },
  methods: {
    GenerarReporte() {
      this.year = this.year_form;
      this.mes = this.mes_form;
      this.visible = true;
    },
  },
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
