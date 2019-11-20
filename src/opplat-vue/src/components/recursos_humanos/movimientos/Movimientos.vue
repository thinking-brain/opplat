<template>
  <v-container>
    <v-toolbar-title>Movimientos de Trabajadores</v-toolbar-title>

    <v-row justify="space-around">
      <v-switch v-model="chips" label="Entrada"></v-switch>
      <v-switch v-model="multiple" label="Traslado"></v-switch>
      <v-switch v-model="hideSelected" label="Baja"></v-switch>
      <v-switch v-model="clearable" label="Otros Movimientos"></v-switch>
    </v-row>
    <v-divider class="d-print-none"></v-divider>
  </v-container>
</template>

<script>
export default {
  data: () => ({})
};
</script>