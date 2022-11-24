<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex sm12>
        <h3>Configuraciones</h3>
      </v-flex>
      <v-flex lg12>
        <v-card v-if="licencia">
          <v-card-text>Gestione la configuracion de su sistema.</v-card-text>
          <v-divider class="mt-5"></v-divider>
          <v-card-actions>
            <v-btn color="primary" flat @click="submit">Agregar Licencia</v-btn>
          </v-card-actions>
        </v-card>
        <v-card ref="form" v-if="!licencia">
          <v-card-text>
            <v-text-field
              required
              label="Fichero de licencia"
              prepend-icon="attach_file"
              type="file"
              v-model="licencia_file"
              :rules="[() => !!licencia_file || 'Este campo es obligatorio.']"
            ></v-text-field>
          </v-card-text>
          <v-divider class="mt-5"></v-divider>
          <v-card-actions>
            <v-btn flat to="/admin/usuarios">Cancelar</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="primary" flat @click="submit">Guardar</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";

@Component({ components: {} })
export default class Configuracion extends Vue {
  dialog: boolean = false;
  show_password: boolean = false;
  errorMessages = [];
  errorMessagesPassword = [];
  licencia = null;
  licencia_file = null;
  rules = {
    required: (value) => !!value || "Obligatorio.",
  };
  formHasErrors: boolean = false;
  errors = [];

  get form() {
    return {
      licencia: this.licencia_file,
    };
  }
  created() {}
  @Watch("nombres")
  async onPropertyChanged(value: any, oldValue: any) {
    this.errorMessages = [];
  }

  public submit() {
    this.formHasErrors = false;
    // Object.keys(this.form).forEach(f => {
    //   if (!this.form[f]) this.formHasErrors = true;

    //   this.$refs[f].validate(true);
    // });
    if (!this.formHasErrors) {
      this.axios
        .post("http://localhost:5200/config/licencia", this.form)
        .then((p) => {
          this.licencia = p.data;
          this.$toast.success("Licencia agregada satisfactoriamente.");
        })
        .catch((e) => {
          this.$toast.error(e.response.data.errors);
        });
    }
  }
}
</script>
<style></style>
