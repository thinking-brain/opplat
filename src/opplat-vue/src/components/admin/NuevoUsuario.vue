<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex sm12>
        <h3>Nuevo Usuario</h3>
      </v-flex>
      <v-flex lg12>
        <v-card ref="form">
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
            ></v-text-field>
            <v-text-field
              label="Usuario"
              :rules="[() => !!username || 'Este campo es obligatorio.', usernameCheck]"
              v-model="username"
              ref="username"
              required
              :error-messages="errorMessages"
            ></v-text-field>
            <v-text-field
              label="Contraseña"
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
              label="Confirmar Contraseña"
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
          <v-divider class="mt-5"></v-divider>
          <v-card-actions>
            <v-btn to="/admin/usuarios">Cancelar</v-btn>
            <v-spacer></v-spacer>
            <v-slide-x-reverse-transition>
              <v-tooltip left v-if="formHasErrors">
                <v-btn icon @click="resetForm" slot="activator" class="my-0">
                  <v-icon>refresh</v-icon>
                </v-btn>
                <span>Refrescar formulario</span>
              </v-tooltip>
            </v-slide-x-reverse-transition>
            <v-btn color="primary" @click="submit">Guardar</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>
<script>
import api from "@/api.js";
export default {
  data: () => ({
    dialog: false,
    show_password: false,
    errorMessages: [],
    errorMessagesPassword: [],
    nombres: null,
    apellidos: null,
    username: null,
    contraseña: null,
    confirmarContraseña: null,
    roles: [],
    selected_roles: [],
    rules: {
      required: value => !!value || "Obligatorio.",
      min: v => (!!v && v.length >= 8) || "Min 8 caracteres"
    },
    formHasErrors: false,
    errors: []
  }),
  computed: {
    form() {
      return {
        nombres: this.nombres,
        apellidos: this.apellidos,
        username: this.username,
        contraseña: this.contraseña,
        confirmarContraseña: this.confirmarContraseña
      };
    }
  },
  created() {},
  watch: {
    nombres() {
      this.errorMessages = [];
    }
  },

  methods: {
    usernameCheck() {
      this.errorMessages =
        this.username == "usuario" ? ["Este usuario existe"] : [];
      return true;
    },
    confirmPasswordCheck() {
      this.errorMessagesPassword =
        this.contraseña != this.confirmarContraseña
          ? ["No coinciden la contraseña y la confirmacion."]
          : [];

      return true;
    },
    resetForm() {
      this.errorMessages = [];
      this.formHasErrors = false;

      Object.keys(this.form).forEach(f => {
        this.$refs[f].reset();
      });
    },
    submit() {
      this.formHasErrors = false;
      // Object.keys(this.form).forEach(f => {
      //   if (!this.form[f]) this.formHasErrors = true;

      //   this.$refs[f].validate(true);
      // });
      if (!this.formHasErrors) {
        let url = api.getUrl("api-account", "account/add-usuario");
        this.axios
          .post(url, this.form)
          .then((
            p //console.log(p)
          ) => {
            this.$router.push({
              name: "gestionar-roles",
              query: {
                usuario: p.data
              }
            });
            vm.$snotify.success("Usuario creado satisfactorimente.");
          })
          .catch(e => {
            vm.$snotify.error(e.response.data.errors);
          });
      }
    }
  }
};
</script>
<style></style>
