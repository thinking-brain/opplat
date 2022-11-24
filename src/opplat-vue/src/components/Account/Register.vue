<template>
  <v-card flat :loading="loading">
    <v-form ref="form" autocomplete="on" v-model="valid">
      <p class="display-1 text-center text--primary">Crear Usuario</p>
      <v-divider />
      <div v-if="errors">
        <p class="red--text">{{ errorMessage }}</p>
      </div>
      <v-text-field v-model="username" label="Usuario" clearable required>
      </v-text-field>
      <v-text-field
        append-icon="mdi-email"
        v-model="email"
        :rules="emailRules"
        label="Correo"
        required
      ></v-text-field>
      <vue-tel-input-vuetify
        :preferred-countries="['cu', 'gb', 'ua', 'us']"
        :valid-characters-only="true"
        select-label="Código"
        label="Número de Teléfono"
        placeholder=""
        @input="onInput"
        required
      ></vue-tel-input-vuetify>
      <v-text-field
        v-model="password"
        :rules="passwordRules"
        label="Contraseña"
        :append-icon="show_password ? 'mdi-eye' : 'mdi-eye-off'"
        :type="show_password ? 'text' : 'password'"
        @click:append="show_password = !show_password"
        clearable
        required
      ></v-text-field>
      <v-text-field
        v-model="confirmPassword"
        :rules="confirmRules"
        label="Confirmar Contraseña"
        :append-icon="show_confirm_password ? 'mdi-eye' : 'mdi-eye-off'"
        :type="show_confirm_password ? 'text' : 'password'"
        @click:append="show_confirm_password = !show_confirm_password"
        clearable
        required
      ></v-text-field>
    </v-form>
    <div class="text-center">
      <v-btn
        :disabled="!valid"
        color="secondary"
        @click="signUp()"
        elevation="2"
        class="text-center"
        block
      >
        Aceptar
      </v-btn>
    </div>
  </v-card>
</template>

<script lang="ts">
import AuthServices from "@/services/repos/AuthServices";
import { RouteOptions } from "@/services/repos/IRepository";
import { Method } from "axios";
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
import VueTelInputVuetify from "vue-tel-input-vuetify/lib/vue-tel-input-vuetify.vue";

@Component({ components: { VueTelInputVuetify } })
export default class Register extends Vue {
  @Prop({ required: true }) public sponsor_name!: string;

  //  Data
  private valid: boolean = true;
  private username: string = "";
  private email: string = "";
  private password: string = "";
  private phone: any = {
    number: "",
    valid: false,
    country: undefined,
  };
  private dialog: boolean = false;
  private loading: boolean = false;
  private show_password: boolean = false;
  private show_confirm_password: boolean = false;
  private errorMessages: any[] = [];
  private errorMessagesPassword: any[] = [];
  private confirmPassword: any = null;
  private errorMessage: string = "";
  private rules: any = {
    required: (value) => !!value || "Obligatorio.",
    min: (v) => (!!v && v.length >= 8) || "Min 8 caracteres",
  };
  private formHasErrors: boolean = false;
  private errors: any[] = [];
  private nombreRules: any = [(v) => !!v || "El nombre es requerido"];
  private emailRules: any = [
    (v) => !!v || "El email es requerido",
    (v) => /.+@.+\..+/.test(v) || "Debe tener una dirección válida",
  ];
  private passwordRules: any[] = [
    (v) => !!v || "Contraseña es requerida",
    (v) =>
      v.toString().length >= 8 || "Contraseña debe tener 8 o más caracteres.",
  ];
  // Hooks
  private get confirmRules() {
    const rules: any[] = [];
    const matchRule = (v) =>
      (!!v && v) === this.password || "Contraseñas no coinciden";
    rules.push(matchRule);
    return rules;
  }

  async created() {}
  onInput(formattedNumber, { number, valid, country }) {
    this.phone.number = number.international;
    this.phone.valid = valid;
    this.phone.country = country && country.name;
  }
  confirmPasswordCheck() {
    this.errorMessagesPassword =
      this.password !== this.confirmPassword
        ? ["No coinciden la contraseña y la confirmacion."]
        : [];

    return true;
  }
  async signUp() {
    if (this.confirmPasswordCheck()) {
      this.loading = true;
      const { username, password, phone, email, sponsor_name } = this;
      const phone_number = phone.number.replace(/ /g, "");
      var data = {
        username: username,
        password: password,
        phone_number: phone_number,
        email: email,
        sponsor_name: sponsor_name,
      };
      const options: RouteOptions = {
        data: data,
        url: "signup",
      };
      let token = localStorage.getItem("access_token") || "null";

      if (token != "null") {
        options.providedHeaders = { Authorization: `Bearer ${token}` };
      }
      const sendData = await AuthServices.insert(options);
      if (sendData) {
        this.loading = false;
        this.$emit("close");
        return this.$toast.success("Se creó correctamente");
      }
    }
  }
}
</script>

<style></style>
