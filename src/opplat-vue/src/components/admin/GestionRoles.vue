<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex sm12>
        <h3>Gestionar roles para el usuario {{usuario.username}}</h3>
      </v-flex>
      <v-flex lg12>
        <v-card ref="form">
          <v-card-text v-if="roles">
            <v-container fluid v-for="rol in roles" v-bind:key="rol.Modulo">
              <v-subheader>{{rol.Modulo}}</v-subheader>
              <v-switch
                v-model="selected_roles"
                v-for="r in rol.Roles"
                v-bind:key="r"
                v-bind:label="r"
                v-bind:value="r"
              ></v-switch>
            </v-container>
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
  props: ["usuario"],
  data: () => ({
    dialog: false,
    errorMessages: [],
    roles: [],
    selected_roles: [],
    rules: {
      required: value => !!value || "Obligatorio."
    },
    formHasErrors: false,
    errors: []
  }),
  computed: {
    // selected_roles() {
    //   return ;
    // },
    form() {
      return {
        idUsuario: this.usuario.userId,
        roles: this.selected_roles
      };
    }
  },
  created() {
    this.selected_roles = this.usuario.roles;
    this.getRoles();
  },
  watch: {},
  methods: {
    getRoles: function() {
      let url = api.getUrl("api-account", "account/roles");
      this.axios
        .get(url)
        .then(response => {
          this.roles = response.data;
        })
        .catch(e => {
          this.errors.push(e);
        });
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
        let url = api.getUrl("api-account", "account/cambiar-roles");
        this.axios
          .post(url, this.form)
          .then(p => {
            vm.$snotify.success("Roles modificados correctamente.");
            this.$router.push("/admin/usuarios");
          })
          .catch(err => {
            vm.$snotify.error("Error modificando los roles. " + err);
          });
      }
    }
  }
};
</script>
<style></style>
