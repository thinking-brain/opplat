<template>
  <v-dialog v-model="dialogDetails" max-width="470px" persistent dark>
    <v-card>
      <v-spacer></v-spacer>
      <div class="text-right">
        <v-btn icon @click="$emit('close')" class="mt-1 mr-1">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </div>
      <v-card-title class="text-center justify-center py-6">
        <p class="font-weight-bold">Detalles del Equipo</p>
      </v-card-title>

      <v-tabs v-model="tab" background-color="transparent" show-arrows>
        <v-tab v-for="i in items" :key="i">
          {{ i }}
        </v-tab>
      </v-tabs>
      <v-tabs-items v-model="tab">
        <v-tab-item>
          <v-card flat min-height="300px">
            <v-card-text>
              <p><strong>Número de Serie: </strong>{{ item.numeroSerie }}</p>
              <p><strong>Marca: </strong>{{ item.marca.nombre }}</p>
              <p><strong>Modelo: </strong>{{ item.modelo.nombre }}</p>
              <p>
                <strong>Fecha de Fabricación: </strong
                >{{ item.fechaFabricacion }}
              </p>
              <p><strong>Obsevaciones: </strong>{{ item.observaciones }}</p>
            </v-card-text>
          </v-card>
        </v-tab-item>
        <v-tab-item>
          <v-card flat min-height="300px">
            <v-card-text>
              <p><strong>Nombre: </strong>{{ item.cliente.nombre }}</p>
              <p><strong>CI: </strong>{{ item.cliente.ci }}</p>
              <p><strong>Dirección: </strong>{{ item.cliente.direccion }}</p>
              <p><strong>Teléfono: </strong>{{ item.cliente.telefono }}</p>
            </v-card-text>
          </v-card>
        </v-tab-item>
        <v-tab-item>
          <v-card flat min-height="300px">
            <v-card-text> </v-card-text>
          </v-card>
        </v-tab-item>
        <v-tab-item>
          <v-card flat min-height="300px">
            <v-card-text> </v-card-text>
          </v-card>
        </v-tab-item>
      </v-tabs-items>
    </v-card>
  </v-dialog>
</template>
<script>
import api from "@/api";

export default {
  props: {
    dialogDetails: {
      type: Boolean,
      default: false,
    },
    item: {
      type: Object,
      default: {},
    },
  },

  data: () => ({
    editedIndex: -1,
    formHasErrors: false,
    errors: [],
    tab: null,
    items: ["Equipo", "Dueño", "Histórico", "Documentos"],
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    },
  },
  created() {},
  watch: {},
  methods: {},
};
</script>
<style></style>
