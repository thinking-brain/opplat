<template>
  <v-dialog v-model="dialog" persistent max-width="600px">
    <template v-slot:activator="{ on }">
      <v-btn depressed outlined icon fab dark color="secondary" small v-on="on">
        <v-icon>mdi-key</v-icon>
      </v-btn>
    </template>
    <v-card ref="form">
      <v-card-title>
        <span class="headline"
          >Resetear contraseña al usuario: {{ dataModel.username }}</span
        >
      </v-card-title>
      <v-card-text>
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
        ></v-text-field>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="error" @click="dialog = false" elevation="8"
          >Cerrar</v-btn
        >
        <v-btn color="primary" @click="submit" elevation="8">Guardar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script lang="ts">
import { Component, PropSync, Vue, Watch } from "vue-property-decorator";

@Component({
  components: {},
})
export default class ResetPassword extends Vue {
  @PropSync("usuario") public dataModel!: any;
  // Data
  dialog: boolean = false;
  show_password: boolean = false;
  errorMessages: string[] = [];
  errorMessagesPassword: string[] = [];
  contraseña = null;
  confirmarContraseña = null;
  rules = {
    required: (value) => !!value || "Obligatorio.",
    min: (v) => (!!v && v.length >= 8) || "Min 8 caracteres",
  };
  formHasErrors: boolean = false;
  errors = [];
  // Computed
  get form() {
    return {
      usuarioId: this.dataModel.userId,
      contraseña: this.contraseña,
      confirmarContraseña: this.confirmarContraseña,
    };
  }
  created() {}
  @Watch("nombres")
  async onPropertyChanged(value: any, oldValue: any) {
    this.errorMessages = [];
  }

  // Methods
  confirmPasswordCheck() {
    this.errorMessagesPassword =
      this.contraseña !== this.confirmarContraseña
        ? ["No coinciden la contraseña y la confirmacion."]
        : [];

    return true;
  }

  submit() {
    this.formHasErrors = false;
    // if (!this.formHasErrors) {
    //   const url = api.getUrl("api-account", "account/reset-password");
    //   this.axios
    //     .post(url, this.form)
    //     .then(() => {
    //       this.dialog = false;
    //       this.$toast.success("Contraseña reseteada correctamente.");
    //     })
    //     .catch((err) => {
    //       this.dialog = false;
    //       this.$toast.error(`Error reseteando la contraseña. ${err}`);
    //     });
    // }
  }
}
</script>
<style></style>
