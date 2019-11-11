<template>
  <v-container>
    <v-toolbar-title>Trabajadores por Área</v-toolbar-title>
    <v-divider class="mx-4" inset vertical></v-divider>
    <v-form v-model="valid" class="d-print-none">
      <v-row>
        <v-col cols="12" md="4">
          <v-autocomplete
            v-model="unidadOrganizativa"
            item-text="nombre"
            return-object
            :items="unidades_organizativas"
            :rules="[v => !!v || 'Item is required']"
            label="Unidades Organizativas"
            required
          ></v-autocomplete>
        </v-col>
        <v-col cols="12" md="4">
          <v-btn
            color="success"
            class="mr-4"
            @click="GenerarReporte(unidadOrganizativa)"
          >Generar Reporte</v-btn>
        </v-col>
      </v-row>
    </v-form>
    <v-flex lg12>
      <v-divider class="d-print-none"></v-divider>
      <v-row>
        <v-data-table :headers="headers" :items="trabajadores" :search="search" class="elevation-1">
          <template v-slot:top>
            <v-toolbar flat color="white">
              <v-toolbar-title>Trabajadores del Área {{unidadOrganizativa.nombre}}</v-toolbar-title>
              <v-divider class="mx-4" inset vertical></v-divider>
              <v-spacer></v-spacer>
              <v-text-field
                v-model="search"
                append-icon="mdi-magnify"
                label="Buscar"
                single-line
                hide-details
              ></v-text-field>
              <v-spacer></v-spacer>
            </v-toolbar>
          </template>
          <template v-slot:item.action="{ item }">
            <v-icon small class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
            <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
          </template>
        </v-data-table>
      </v-row>
    </v-flex>
  </v-container>
</template>
<script>
import api from "@/api";
import UnidadOrganizativaTabla from "@/components/recursos_humanos/reportes/UnidadOrganizativaTabla";

export default {
  components: { UnidadOrganizativaTabla },
  data: () => ({
    data: null,
    unidades_organizativas: [],
    unidadOrganizativa: { id: 0, nombre: "" },
    trabajadores: [],
    errors: [],
    headers: [
      {
        text: "Nombre y Apellidos",
        align: "left",
        sortable: true,
        value: "nombre"
      },
      { text: "Cargo", value: "cargo", sortable: false }
      // { text: "Actions", value: "action", sortable: false }
    ]
  }),

  created() {
    this.initialize();
  },

  methods: {
    initialize() {
      const url = api.getUrl("recursos_humanos", "UnidadOrganizativa");
      this.axios.get(url).then(response => {
        this.unidades_organizativas = response.data;
      });
      this.axios.get(url + "/" + unidadOrganizativa.id).then(response => {
        this.trabajadores = response.data;
      });
    }
  },

  GenerarReporte() {
    const data = this.unidadOrganizativa.id;
    this.$refs.tabla.loadReporte(data);
  }
};
</script>
