<template>
  <v-row justify="center" no-gutters>
    <!-- <v-tooltip top color="orange">
      <template v-slot:activator="{ on }">
        <v-btn
          v-on="on"
          @click="$emit('handleFileImport', item)"
          :disabled="!!!item.active"
          icon
          large
          color="orange"
        >
          <v-icon large>mdi-image-edit-outline</v-icon>
        </v-btn>
        <input
          :v-model="item.image"
          ref="uploader"
          class="d-none"
          type="file"
          @change="$emit('onFileChanged')"
        />
      </template>
      <span>Editar Imagen</span>
    </v-tooltip> -->
    <v-tooltip top color="primary">
      <template v-slot:activator="{ on }">
        <v-btn
          v-on="on"
          @click="$emit('editElement', item)"
          :disabled="!!!item.active"
          icon
          large
          color="primary"
        >
          <v-icon large>mdi-circle-edit-outline</v-icon>
        </v-btn>
      </template>
      <span>Editar</span>
    </v-tooltip>
    <v-tooltip top color="success">
      <template v-slot:activator="{ on }">
        <v-btn
          v-on="on"
          @click="$emit('historical', item)"
          dark
          icon
          large
          color="success"
        >
          <v-icon large>mdi-history</v-icon>
        </v-btn>
      </template>
      <span>Movimientos / Historial</span>
    </v-tooltip>
    <v-tooltip top color="#8f4a3f" v-if="!!item.active">
      <template v-slot:activator="{ on }">
        <v-btn
          v-on="on"
          @click="$emit('disableElement', item)"
          dark
          icon
          large
          color="#8f4a3f"
        >
          <v-icon large>mdi-delete-clock</v-icon>
        </v-btn>
      </template>
      <span>Deshabilitar</span>
    </v-tooltip>
    <v-tooltip top color="success" v-else>
      <template v-slot:activator="{ on }">
        <v-btn
          v-on="on"
          @click="$emit('activateElement', item)"
          dark
          icon
          large
          color="success"
        >
          <v-icon large>mdi-restore</v-icon>
        </v-btn>
      </template>
      <span>Habilitar</span>
    </v-tooltip>
    <v-tooltip top color="error">
      <template v-slot:activator="{ on }">
        <v-btn
          v-on="on"
          @click="$emit('confirmDelete', item)"
          dark
          icon
          large
          color="error"
        >
          <v-icon large>mdi-trash-can</v-icon>
        </v-btn>
      </template>
      <span>Eliminar</span>
    </v-tooltip>
  </v-row>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue } from "vue-property-decorator";

@Component
export default class Actions extends Vue {
  @PropSync("data") public item!: any;
}
</script>
