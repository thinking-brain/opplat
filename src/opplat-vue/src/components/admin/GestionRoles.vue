<template>
  <v-dialog
    v-model="dialog"
    transition="dialog-bottom-transition"
    persistent
    max-width="800px"
    ><v-card>
      <v-container grid-list-xl fluid>
        <v-layout row wrap>
          <v-flex sm12>
            <h3 class="ml-5">
              Gestionar roles para el usuario {{ dataModel.username }}
            </h3>
          </v-flex>
          <v-flex lg12 ref="form">
            <v-card-text v-if="roles">
              <v-row>
                <v-col
                  cols="12"
                  sm="6"
                  v-for="rol in roles"
                  v-bind:key="rol.Modulo"
                >
                  <v-subheader>{{ rol.Modulo }}</v-subheader>
                  <v-switch
                    v-model="selected_roles"
                    v-for="r in rol.Roles"
                    v-bind:key="r"
                    v-bind:label="r"
                    v-bind:value="r"
                  ></v-switch></v-col
              ></v-row>
            </v-card-text>
            <v-divider class="mt-4"></v-divider>
            <v-card-actions>
              <v-btn color="error" @click="$emit('close')">Cancelar</v-btn>
              <v-spacer></v-spacer>
              <v-btn color="primary" @click="submit">Guardar</v-btn>
            </v-card-actions>
          </v-flex>
        </v-layout>
      </v-container></v-card
    ></v-dialog
  >
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";
import getLocalData from "@/util/getLocalData";

@Component({ components: {} })
export default class GestionRoles extends Vue {
  @PropSync("usuario") public dataModel!: any;
  @Prop({ required: true }) public modal!: boolean;

  // Data
  errorMessages = [];
  roles = [];
  selected_roles = [];
  rules = {
    required: (value) => !!value || "Obligatorio.",
  };
  formHasErrors = false;
  errors = [];

  // Computed
  get dialog() {
    return this.modal;
  }
  get form() {
    return {
      idUsuario: this.dataModel.userId,
      roles: this.selected_roles,
    };
  }
  created() {
    this.selected_roles = this.dataModel.roles;
    this.getRoles();
  }

  public getRoles() {
    this.roles = getLocalData.getRoles();
  }

  public submit() {
    this.formHasErrors = false;
    Object.keys(this.form).forEach((f) => {
      if (!this.form[f]) this.formHasErrors = true;
      // @ts-ignore
      this.$refs.form.validate();
    });
    // if (!this.formHasErrors) {
    //   const url = api.getUrl("api-account", "account/cambiar-roles");
    //   this.axios
    //     .post(url, this.form)
    //     .then(() => {
    //      this.$toast.success("Roles modificados correctamente.");
    //      this.$router.push("/administracion/usuarios");
    //     })
    //     .catch((err) => {
    //     this.$toast.error(`Error modificando los roles. ${err}`);
    //     });
    // }
  }
}
</script>
<style></style>
