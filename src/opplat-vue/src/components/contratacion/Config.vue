<template>
  <div class="pt-2">
    <v-tabs v-model="tab" class="elevation-2">
      <v-tab class="caption py-3">Administradores</v-tab>
      <v-tab class="caption py-3">Dictaminadores</v-tab>
      <v-tab class="caption py-3">Departamentos</v-tab>
      <v-tab class="caption py-3">Proveedores</v-tab>
      <v-tab class="caption py-3">Especialistas Externos</v-tab>
      <v-tab class="caption py-3">Comité de Contratación</v-tab>
      <v-tab class="caption py-3">Tiempo de Vencimiento</v-tab>
      <v-tab-item>
        <AdminContratos></AdminContratos>
      </v-tab-item>
      <v-tab-item>
        <DictContratos></DictContratos>
      </v-tab-item>
      <v-tab-item>
        <Departamentos></Departamentos>
      </v-tab-item>
      <v-tab-item>
        <Entidades></Entidades>
      </v-tab-item>
      <v-tab-item>
        <EspExternos></EspExternos>
      </v-tab-item>
      <v-tab-item>
        <ComiteContratacion></ComiteContratacion>
      </v-tab-item>
      <v-tab-item>
        <TiempoVenContratos></TiempoVenContratos>
      </v-tab-item>
    </v-tabs>
  </div>
</template>
<script>
import api from "@/api";
import AdminContratos from "@/components/contratacion/AdminContratos.vue";
import DictContratos from "@/components/contratacion/DictContratos.vue";
import EspExternos from "@/components/contratacion/EspExternos.vue";
import FormasDePago from "@/components/contratacion/FormasDePago.vue";
import Entidades from "@/components/contratacion/Entidades.vue";
import Departamentos from "@/components/contratacion/Departamentos.vue";
import ComiteContratacion from "@/components/contratacion/ComiteContratacion.vue";
import TiempoVenContratos from "@/components/contratacion/TiempoVenContratos.vue";

export default {
  components: {
    AdminContratos,
    DictContratos,
    EspExternos,
    FormasDePago,
    Entidades,
    Departamentos,
    ComiteContratacion,
    TiempoVenContratos
  },
  data: () => ({
    tabs: null,
    tab: null,
    vertical: true,
    errors: []
  }),
  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {},

  methods: {}
};
</script>