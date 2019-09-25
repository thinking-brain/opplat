<template>
  <v-dialog v-model="dialog" persistent max-width="600px">
    <template v-slot:activator="{ on }">
      <v-btn depressed outlined icon fab dark color="primary" small v-on="on">
        <v-icon>mdi-pen</v-icon>
      </v-btn>
    </template>
    <v-card ref="form">
      <v-card-title>
        <span class="headline">Editar usuario: {{usuario.username}}</span>
      </v-card-title>
      <v-card-text>
        <v-text-field
          label="Nombres"
          v-model="nombres"
          required
          ref="nombres"
          :rules="[() => !!nombres || 'Este campo es obligatorio.']"
        ></v-text-field>
        <v-text-field
          label="Apellidos"
          v-model="apellidos"
          required
          ref="apellidos"
          :rules="[() => !!apellidos || 'Este campo es obligatorio.']"
          v-on:keyup.enter="submit"
        ></v-text-field>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="error" @click="dialog = false">Cerrar</v-btn>
        <v-btn color="primary" @click="submit">Guardar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import api from "@/api.js";
export default {
  props: ["usuario"],
  data: () => ({
    dialog: false,
    errorMessages: [],
    nombres: null,
    apellidos: null,
    rules: {
      required: value => !!value || "Obligatorio."
    },
    formHasErrors: false,
    errors: []
  }),
  computed: {
    form() {
      return {
        id: this.usuario.userId,
        nombres: this.nombres,
        apellidos: this.apellidos
      };
    }
  },
  created() {
    this.nombres = this.usuario.nombres;
    this.apellidos = this.usuario.apellidos;
  },
  watch: {
    nombres() {
      this.errorMessages = [];
    }
  },

  methods: {
    resetForm() {
      this.errorMessages = [];
      this.formHasErrors = false;

      Object.keys(this.form).forEach(f => {
        this.$refs[f].reset();
      });
    },
    submit() {
      this.formHasErrors = false;
      if (!this.formHasErrors) {
        let url = api.getUrl("api-account", "account/editar-usuario");
        this.axios
          .post(url, this.form)
          .then(p => {
            this.usuario.nombres = this.nombres;
            this.usuario.apellidos = this.apellidos;
            this.dialog = false;
            vm.$snotify.success("Usuario editado correctamente.");
            this.dialog = false;
          })
          .catch(err => {
            vm.$snotify.error("Error editando el usuario. " + err);
            this.dialog = false;
          });
      }
    }
  }
};
</script>
<style></style>
