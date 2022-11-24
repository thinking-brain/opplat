<template>
  <v-dialog v-model="dialog" persistent max-width="600px">
    <template v-slot:activator="{ on }">
      <v-btn
        depressed
        outlined
        icon
        fab
        dark
        color="pink"
        small
        v-on="on"
        v-if="dataModel.activo"
      >
        <v-icon>mdi-delete</v-icon>
      </v-btn>
      <v-btn
        depressed
        outlined
        icon
        fab
        dark
        color="green"
        small
        v-on="on"
        v-else
      >
        <v-icon>mdi-check</v-icon>
      </v-btn>
    </template>
    <v-card v-if="dataModel.activo">
      <v-card-title>
        <span class="headline"
          >Esta seguro de desactivar el usuario: {{ nombre }}?</span
        >
      </v-card-title>
      <v-card-text></v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" @click="dialog = false">Cancelar</v-btn>
        <v-btn color="red" @click="cambiarEstado()">Desactivar</v-btn>
      </v-card-actions>
    </v-card>
    <v-card v-else>
      <v-card-title>
        <span class="headline"
          >Esta seguro de activar el usuario: {{ nombre }}?</span
        >
      </v-card-title>
      <v-card-text></v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="error" @click="dialog = false" elevation="8"
          >Cancelar</v-btn
        >
        <v-btn color="green" @click="cambiarEstado()" dark elevation="8"
          >Activar</v-btn
        >
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";
import AuthServices from "@/services/repos/AuthServices";
import { RouteOptions } from "@/services/repos/IRepository";
import { Method } from "axios";

@Component({ components: {} })
export default class CambiarEstadoUsuario extends Vue {
  @PropSync("usuario") public dataModel!: any;

  // Data
  public dialog: boolean = false;

  // Computed
  get nombre() {
    return `${this.dataModel.nombres} ${this.dataModel.apellidos}`;
  }

  // Method
  public async cambiarEstado() {
    const options: RouteOptions = {
      url: `account/cambiar-estado/?idUsuario=${this.dataModel.userId}`,
      providedHeaders: { "Content-Type": "multipart/form-data" },
    };
    let result = await AuthServices.list(options);
    if (result) {
      this.dataModel.activo = !this.dataModel.activo;
    }
    this.dialog = false;
  }
}
</script>
<style></style>
