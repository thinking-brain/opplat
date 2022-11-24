<template>
  <v-dialog v-model="dialog" persistent max-width="600px">
    <template v-slot:activator="{ on }">
      <v-btn depressed outlined icon fab dark color="primary" small v-on="on">
        <v-icon>mdi-pen</v-icon>
      </v-btn>
    </template>
    <v-card ref="form">
      <v-card-title>
        <span class="headline">Editar usuario: {{ usuario.username }}</span>
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
<script lang="ts">
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";

@Component({ components: {} })
export default class EditarUsuario extends Vue {
  @PropSync("usuario") public dataModel!: any;

  // Data
  dialog: boolean = false;
  errorMessages = [];
  nombres = null;
  apellidos = null;
  rules = {
    required: (value) => !!value || "Obligatorio.",
  };
  formHasErrors: boolean = false;
  errors = [];

  // Computed
  get form() {
    return {
      id: this.dataModel.userId,
      nombres: this.nombres,
      apellidos: this.apellidos,
    };
  }
  created() {
    this.nombres = this.dataModel.nombres;
    this.apellidos = this.dataModel.apellidos;
  }
  @Watch("nombres")
  async onPropertyChanged(value: any, oldValue: any) {
    this.errorMessages = [];
  }
  // Methods
  public submit() {
    this.formHasErrors = false;
    // if (!this.formHasErrors) {
    // const url = api.getUrl("api-account", "account/editar-usuario");
    // this.axios
    //   .post(url, this.form)
    //   .then(() => {
    //     this.dataModel.nombres = this.nombres;
    //     this.dataModel.apellidos = this.apellidos;
    //     this.dialog = false;
    //               this.$toast.success("Usuario editado correctamente.");
    //     this.dialog = false;
    //   })
    //   .catch((err) => {
    //               this.$toast.success(`Error editando el usuario. ${err}`);
    //     this.dialog = false;
    //   });
    // }
  }
}
</script>
<style></style>
