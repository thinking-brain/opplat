<template>
  <v-dialog v-model="dialog" persistent max-width="600px">
    <template v-slot:activator="{ on }">
      <v-btn color="deep-purple accent-4" text v-on="on">Cambiar Contraseña</v-btn>
    </template>
    <v-card ref="form">
      <v-card-title>
        <span class="headline">Cambiar la contraseña</span>
      </v-card-title>
      <v-card-text>
        <v-text-field
          label="Contraseña Actual"
          v-model="contraseñaActual"
          :append-icon="show_password ? 'mdi-eye' : 'mdi-eye-off'"
          :rules="[rules.required, rules.min]"
          :type="show_password ? 'text' : 'password'"
          name="contraseñaActual"
          hint="Al menos 8 caracteres"
          counter
          @click:append="show_password = !show_password"
        ></v-text-field>
        <v-text-field
          label="Contraseña Nueva"
          v-model="contraseña"
          :append-icon="show_password ? 'mdi-eye' : 'mdi-eye-off'"
          :rules="[rules.required, rules.min]"
          :type="show_password ? 'text' : 'password'"
          name="contraseña"
          hint="Al menos 8 caracteres"
          counter
          @click:append="show_password = !show_password"
        ></v-text-field>
        <v-text-field
          label="Confirmar Contraseña Nueva"
          v-model="confirmarContraseña"
          :append-icon="show_password ? 'mdi-eye' : 'mdi-eye-off'"
          :rules="[!!confirmarContraseña || 'Este campo es obligatorio.',confirmPasswordCheck]"
          :type="show_password ? 'text' : 'password'"
          name="confirmarContraseña"
          counter
          @click:append="show_password = !show_password"
          ref="confirmarContraseña"
          required
          :error-messages="errorMessagesPassword"
        ></v-text-field>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" @click="dialog = false">Cerrar</v-btn>
        <v-btn color="primary" @click="submit">Guardar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import api from '@/api.js';

export default {
  props: ['usuario'],
  data: () => ({
    dialog: false,
    show_password: false,
    errorMessages: [],
    errorMessagesPassword: [],
    contraseñaActual: null,
    contraseña: null,
    confirmarContraseña: null,
    rules: {
      required: value => !!value || 'Obligatorio.',
      min: v => (!!v && v.length >= 8) || 'Min 8 caracteres',
    },
    formHasErrors: false,
    errors: [],
  }),
  computed: {
    form() {
      return {
        usuarioId: this.usuario.userId,
        contraseñaActual: this.contraseñaActual,
        contraseña: this.contraseña,
        confirmarContraseña: this.confirmarContraseña,
      };
    },
  },
  created() {},
  watch: {
    nombres() {
      this.errorMessages = [];
    },
  },

  methods: {
    confirmPasswordCheck() {
      this.errorMessagesPassword = this.contraseña != this.confirmarContraseña
        ? ['No coinciden la contraseña y la confirmacion.']
        : [];

      return true;
    },
    resetForm() {
      this.errorMessages = [];
      this.formHasErrors = false;

      Object.keys(this.form).forEach((f) => {
        this.$refs[f].reset();
      });
    },
    submit() {
      this.formHasErrors = false;
      if (!this.formHasErrors) {
        const url = api.getUrl('api-account', 'account/change-password');
        this.axios
          .post(url, this.form)
          .then((p) => {
            this.dialog = false;
            vm.$snotify.success('Contraseña cambiada correctamente.');
          })
          .catch((err) => {
            this.dialog = false;
            vm.$snotify.error(`Error cambiando la contraseña. ${err.message}`);
          });
      }
    },
  },
};
</script>
<style></style>
