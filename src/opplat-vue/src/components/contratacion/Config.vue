<template>
  <div class="pt-2">
    <v-tabs v-model="tab" class="elevation-2">
      <v-tab
        class="caption py-3"
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >Administradores</v-tab>
      <v-tab
        class="caption py-3"
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >Dictaminadores</v-tab>
      <v-tab
        class="caption py-3"
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >Departamentos</v-tab>
      <v-tab class="caption py-3">Proveedores</v-tab>
      <v-tab class="caption py-3">Especialistas Externos</v-tab>
      <v-tab
        class="caption py-3"
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >Comité de Contratación</v-tab>
      <v-tab
        class="caption py-3"
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >Tiempo de Vencimiento</v-tab>
      <v-tab-item
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >
        <AdminContratos></AdminContratos>
      </v-tab-item>
      <v-tab-item
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >
        <DictContratos></DictContratos>
      </v-tab-item>
      <v-tab-item
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >
        <Departamentos></Departamentos>
      </v-tab-item>
      <v-tab-item>
        <Entidades></Entidades>
      </v-tab-item>
      <v-tab-item>
        <EspExternos></EspExternos>
      </v-tab-item>
      <v-tab-item
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >
        <ComiteContratacion></ComiteContratacion>
      </v-tab-item>
      <v-tab-item
        v-if="(roles.includes('administrador contratacion')||roles.includes('administrador'))"
      >
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
    roles: null,
    username: null,
    errors: []
  }),
  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {
    this.roles = this.$store.getters.roles;
    this.username = this.$store.getters.usuario;
  },

  methods: {}
};
</script>