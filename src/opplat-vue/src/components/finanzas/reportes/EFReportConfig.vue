<template>
  <v-container fluid>
    <v-row justify="center">
      <h3 class="mt-10">Configurar Plan:</h3>
      <v-col cols="12" md="2">
        <v-select
          class="mt-4"
          v-model="tipo_plan_selected"
          :items="tipos_plan"
          :rules="[v => !!v || 'Este campo es requerido']"
        ></v-select>
      </v-col>
    </v-row>

    <v-card>
      <v-treeview
        class="mx-10 my-5"
        v-model="selection"
        :items="items"
        selectable
        return-object
        open-all
      ></v-treeview>
      <v-row class="my-10" justify="center">
        <v-col cols="12" md="3" class="mb-10">
          <v-btn color="success" class="mr-4" @click="configurarReporte">Guardar Cambios</v-btn>
        </v-col>
      </v-row>
    </v-card>
  </v-container>
</template>

<script>
import api from "@/api";
export default {
  data: () => ({
    tipos_plan: ["5920", "5921", "5922", "5923", "5924", "5925", "5926"],
    tipo_plan_selected: "5920",
    selection: [],
    items: []
  }),
  methods: {
    configurarReporte() {
      const url = api.getUrl("finanzas", `EstadoFinanciero/configurarReporte`);
      const formData = new FormData();

      this.axios
        .post(url, {
          tipoPlan: this.tipo_plan_selected,
          items: this.selection
        })
        .then(response => {
          this.selection = response.data;
          vm.$snotify.success("Se han aplicado los cambios satisfactoriamente");
        })
        .catch(e => {
          this.errors.push(e);
          console.log(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    },
    cargarTipoReporte() {
      const url = api.getUrl(
        "finanzas",
        `EstadoFinanciero/configurarReporte/${this.tipo_plan_selected}`
      );
      this.axios
        .get(url)
        .then(response => {
          this.items = response.data.items;
          this.selection =
            response.data.selection != null ? response.data.selection : [];
        })
        .catch(e => {
          this.errors.push(e);
          console.log(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    }
  },
  created() {
    const url = api.getUrl(
      "finanzas",
      `EstadoFinanciero/configurarReporte/${this.tipo_plan_selected}`
    );
    this.axios
      .get(url)
      .then(response => {
        this.items = response.data.items;
        this.selection =
          response.data.selection != null ? response.data.selection : [];
      })
      .catch(e => {
        this.errors.push(e);
        console.log(e);
        vm.$snotify.error(
          "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
        );
      });
  },
  watch: {
    tipo_plan_selected() {
      console.log("se selecciono el plan" + this.tipo_plan_selected);
      this.cargarTipoReporte();
    }
  }
};
</script>

<style>
</style>
