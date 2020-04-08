<template>
  <v-card>
    <v-container fluid>
      <v-row dense>
        <v-col>
          <v-card flat>
            <v-toolbar flat>
              <v-tabs slot="extension" v-model="tabs" centered>
                <v-tab>Administradores</v-tab>
                <v-tab>Especialistas Externos</v-tab>
                <v-tab>Proveedores</v-tab>
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
                    <EspExternos></EspExternos>
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
            </v-tabs-items>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-card>
</template>
 
<script>
import api from "@/api";
import AdminContratos from "@/components/contratacion/AdminContratos.vue";
import EspExternos from "@/components/contratacion/EspExternos.vue";
import FormasDePago from "@/components/contratacion/FormasDePago.vue";
import Entidades from "@/components/contratacion/Entidades.vue";

export default {
  components: {
    AdminContratos,
    EspExternos,
    FormasDePago,
    Entidades
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