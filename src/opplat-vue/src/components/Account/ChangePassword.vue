<template>
  <v-row justify="center">
    <v-dialog v-model="modal" max-width="290">
      <v-card>
        <v-card-title class="text-h5">
          Cambiar Contraseña
        </v-card-title>
        <v-card-text>
          <v-form ref="form" autocomplete="on" v-model="valid">
            <div v-if="errors">
              <p class="red--text">{{ errorMessage }}</p>
            </div>
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
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text @click="$emit('close')">
            Cancelar
          </v-btn>
          <v-btn color="green darken-1" text :disabled="!valid" @click="save()">
            Aceptar
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";
import { RouteOptions } from "@/services/repos/IRepository";
import { VIcon } from "vuetify/lib";
import { Method } from "axios";

@Component({ name: "ChangePassword", components: { VIcon } })
export default class ChangePassword extends Vue {
  // Props
  @PropSync("dialog") modal!: boolean;

  // Data
  private dialog: boolean = false;
  private loading: boolean = false;
  private valid: boolean = true;
  private confirmPassword: any = null;
  private show_password: boolean = false;
  private show_confirm_password: boolean = false;
  private errorMessages: any[] = [];
  private password: string = "";
  private errors: any[] = [];
  private errorMessage: string = "";
  private passwordRules: any[] = [
    (v) => !!v || "Contraseña es requerida",
    (v) =>
      v.toString().length >= 8 || "Contraseña debe tener 8 o más caracteres.",
  ];
  created() {
    scrollTo(0, 0);
  }
  // Hooks
  private get confirmRules() {
    const rules: any[] = [];
    const matchRule = (v) =>
      (!!v && v) === this.password || "Contraseñas no coinciden";
    rules.push(matchRule);
    return rules;
  }
  // Methods
  public async save() {
    this.$emit("changePassword", this.password);
  }
}
</script>
