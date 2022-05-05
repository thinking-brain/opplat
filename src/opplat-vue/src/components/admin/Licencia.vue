<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex sm12>
        <h3>Licencia</h3>
      </v-flex>
      <v-flex lg12>
        <v-card v-if="licencia">
          <v-card-text>
            Sistema licenciado a nombre de : {{ licencia.subscriptor }} hasta :
            {{ licencia.vencimiento }}
          </v-card-text>
          <v-divider class="mt-5"></v-divider>
          <v-card-actions>
            <v-btn color="secundary" @click="eliminar">Eliminar Licencia</v-btn>
          </v-card-actions>
        </v-card>
        <v-card ref="form" v-if="!licencia">
          <v-card-text>
            <v-file-input
              label="Fichero de licencia"
              v-model="imageFile"
              prepend-icon="mdi-paperclip"
            ></v-file-input>
          </v-card-text>
          <v-divider class="mt-5"></v-divider>
          <v-card-actions>
            <v-btn to="/admin/usuarios">Cancelar</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="primary" @click="submit">Guardar</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>
<script lang="ts">
import { RouteOptions } from "@/services/repos/IRepository";
import store from "@/store";
import { Method } from "axios";
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";
import AdminService from "@/services/repos/AdminService";

@Component({ components: {} })
export default class CambiarEstadoUsuario extends Vue {
  // Data
  dialog: boolean = false;
  show_password: boolean = false;
  errorMessages = [];
  errorMessagesPassword = [];
  licencia_file = null;
  rules = {
    required: (value) => !!value || "Obligatorio.",
  };
  formHasErrors: boolean = false;
  errors = [];
  imageFile = "";

  // Computed
  get form() {
    return {
      licence: this.imageFile,
    };
  }
  get licencia() {
    return store.getters.licencia;
  }
  set licencia(value) {
    this.licencia = value;
  }
  created() {}

  @Watch("nombres")
  async onPropertyChanged(value: any, oldValue: any) {
    this.errorMessages = [];
  }

  // Methods
  public eliminar() {
    store
      .dispatch("quitar")
      .then(() => {
        this.licencia = "";
        this.$toast.success("Licencia eliminada correctamente.");
      })
      .catch(() => {
        this.$toast.error(
          "Error eliminando licencia. Contacte su administrador."
        );
      });
  }
  public async submit() {
    this.formHasErrors = false;
    if (!this.formHasErrors) {
      const form = new FormData();
      form.append("licence", this.imageFile);

      const options: RouteOptions = {
        data: form,
        url: "licencia",
        providedHeaders: { "Content-Type": "multipart/form-data" },
      };
      let lic = await AdminService.insert(options);
      if (lic.data) {
        this.$toast.success("Licencia agregada satisfactoriamente.");
        store
          .dispatch("agregar", lic.data)
          .then(() => {})
          .catch(() => {});
      }
    }
  }
}
</script>
<style></style>
