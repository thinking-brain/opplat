<template>
  <v-dialog v-model="dialogDetails" max-width="400px" persistent>
    <v-card color="blue darken-3" dark>
      <v-card-text class="mb-3">
        <v-btn icon dark absolute right @click="$emit('close')" class="mt-2">
          <v-icon>mdi-close</v-icon>
        </v-btn></v-card-text
      >
      <v-list-item>
        <v-layout column align-center xs12 sm10 md6 lg4>
          <v-avatar size="120" v-if="item.sexo === 0">
            <img src="img/default-avatar-man.jpg" class="float-center pa-5" />
          </v-avatar>
          <v-avatar size="120" v-else-if="item.sexo === 1">
            <img src="img/default-avatar-woman.jpg" class="float-center pa-5" />
          </v-avatar>
          <v-avatar size="110" v-else>
            <img src="img/default-avatar.png" class="float-center pa-5" dark />
          </v-avatar>
          <v-toolbar-title class="text-capitalize">{{
            item.nombre
          }}</v-toolbar-title>
        </v-layout>
      </v-list-item>
      <v-layout class="pa-2"> Carnet de Identidad: {{ item.ci }} </v-layout>
      <v-layout class="pa-2"> Teléfono: {{ item.telefono }} </v-layout>
      <v-layout class="pa-2"> Correo: {{ item.correo }} </v-layout>
      <v-layout class="pa-2"> Dirrección: {{ item.direccion }} </v-layout>
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
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    },
  },
  created() {},
  watch: {},
  methods: {
    save(item) {},
  },
};
</script>
<style></style>
