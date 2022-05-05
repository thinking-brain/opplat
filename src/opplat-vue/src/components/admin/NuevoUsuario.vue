<template>
  <v-dialog
    v-model="dialog"
    :fullscreen="$vuetify.breakpoint.xsOnly"
    transition="dialog-bottom-transition"
    persistent
    scrollable
    max-width="600px"
    min-height="800px"
    ><v-card>
      <v-container grid-list-xl fluid>
        <v-layout row wrap>
          <v-flex sm12>
            <h3>Nuevo Usuario</h3>
          </v-flex>
          <v-flex lg12>
            <v-card ref="form" flat>
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
                  :rules="[
                    () => !!username || 'Este campo es obligatorio.',
                    usernameCheck,
                  ]"
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
                  :rules="[
                    !!confirmarContraseña || 'Este campo es obligatorio.',
                    confirmPasswordCheck,
                  ]"
                  :type="show_password ? 'text' : 'password'"
                  name="confirmarContraseña"
                  counter
                  @click:append="show_password = !show_password"
                  ref="confirmarContraseña"
                  required
                  :error-messages="errorMessagesPassword"
                  v-on:keyup.enter="submit"
                ></v-text-field>
              </v-card-text>
              <v-divider class="mt-5"></v-divider>
              <v-card-actions>
                <v-btn color="error" @click="$emit('close')">Cancelar</v-btn>
                <v-spacer></v-spacer>
                <v-slide-x-reverse-transition>
                  <v-tooltip left v-if="formHasErrors">
                    <v-btn
                      icon
                      @click="resetForm"
                      slot="activator"
                      class="my-0"
                    >
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
      </v-container></v-card
    ></v-dialog
  >
</template>
<script lang="ts">
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
import AuthServices from "@/services/repos/AuthServices";
import { RouteOptions } from "@/services/repos/IRepository";

@Component({ components: {} })
export default class NuevoUsuario extends Vue {
  @Prop({ required: true }) public modal!: boolean;

  // Data
  show_password = false;
  errorMessages: string[] = [];
  errorMessagesPassword: string[] = [];
  nombres = null;
  apellidos = null;
  username = null;
  contraseña = null;
  confirmarContraseña = null;
  roles = [];
  selected_roles = [];
  rules = {
    required: (value) => !!value || "Obligatorio.",
    min: (v) => (!!v && v.length >= 8) || "Min 8 caracteres",
  };
  formHasErrors: boolean = false;
  errors = [];

  // Computed
  get dialog() {
    return this.modal;
  }

  get form() {
    return {
      nombres: this.nombres,
      apellidos: this.apellidos,
      username: this.username,
      contraseña: this.contraseña,
      confirmarContraseña: this.confirmarContraseña,
    };
  }
  public created() {}
  @Watch("nombres")
  async onPropertyChanged(value: any, oldValue: any) {
    this.errorMessages = [];
  }

  // Methods
  public usernameCheck() {
    this.errorMessages =
      this.username === "usuario" ? ["Este usuario existe"] : [];
    return true;
  }
  public confirmPasswordCheck() {
    this.errorMessagesPassword =
      this.contraseña !== this.confirmarContraseña
        ? ["No coinciden la contraseña y la confirmacion."]
        : [];
    return true;
  }

  public async submit() {
    this.formHasErrors = false;
    if (!this.formHasErrors) {
      const options: RouteOptions = {
        data: this.form,
        url: "account/add-usuario",
        providedHeaders: { "Content-Type": "multipart/form-data" },
      };
      const sendData = await AuthServices.insert(options);
      if (sendData) {
        this.$emit("close");
        return this.$toast.success("Usuario creado satisfactoriamente.");
      }
    }
  }
}
</script>
<style></style>
