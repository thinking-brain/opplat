<template>
  <v-dialog v-model="dialog" persistent max-width="600px">
    <template v-slot:activator="{ on }">
      <v-btn depressed outlined icon fab dark color="pink" small v-on="on" v-if="usuario.activo">
        <v-icon>mdi-delete</v-icon>
      </v-btn>
      <v-btn depressed outlined icon fab dark color="green" small v-on="on" v-else>
        <v-icon>mdi-check</v-icon>
      </v-btn>
    </template>
    <v-card v-if="usuario.activo">
      <v-card-title>
        <span class="headline">Esta seguro de desactivar el usuario: {{nombre}}?</span>
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
        <span class="headline">Esta seguro de activar el usuario: {{nombre}}?</span>
      </v-card-title>
      <v-card-text></v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" @click="dialog = false">Cancelar</v-btn>
        <v-btn color="green" @click="cambiarEstado()">Activar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import api from '@/api';

export default {
  props: ['usuario'],
  data: () => ({
    dialog: false,
  }),
  computed: {
    nombre() {
      return `${this.usuario.nombres} ${this.usuario.apellidos}`;
    },
  },
  methods: {
    cambiarEstado() {
      const url = api.getUrl(
        'api-account',
        `account/cambiar-estado/?idUsuario=${this.usuario.userId}`,
      );
      this.axios
        .get(url)
        .then(() => {
          this.usuario.activo = !this.usuario.activo;
        })
        .catch(() => {
          // this.errors.push(e);
          // console.log(e);
        });
      this.dialog = false;
    },
  },
};
</script>
<style></style>
