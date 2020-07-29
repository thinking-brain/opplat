<template>
  <v-row dense>
    <v-col>
      <v-card flat>
        <v-toolbar flat class="v-toolbar__content">
          <v-tabs slot="extension" v-model="tabs" centered class="v-toolbar__content">
            <v-tab class="caption py-3">Administradores</v-tab>
            <v-tab class="caption py-3">Dictaminadores</v-tab>
            <v-tab class="caption py-3">Departamentos</v-tab>
            <v-tab class="caption py-3">Proveedores</v-tab>
            <v-tab class="caption py-3">Especialistas Externos</v-tab>
            <v-tab class="caption py-3">Comité de Contratación</v-tab>
            <v-tab class="caption py-3">Tiempo de Vencimiento</v-tab>
          </v-tabs>
        </v-toolbar>
        <v-tabs-items v-model="tabs" py-6>
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <AdminContratos></AdminContratos>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <DictContratos></DictContratos>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <Departamentos></Departamentos>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <Entidades></Entidades>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <EspExternos></EspExternos>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <ComiteContratacion></ComiteContratacion>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <TiempoVenContratos></TiempoVenContratos>
              </v-card-text>
            </v-card>
          </v-tab-item>
        </v-tabs-items>
      </v-card>
    </v-col>
  </v-row>
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