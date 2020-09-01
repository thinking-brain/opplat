<template>
  <div class="list-table">
    <v-container grid-list-xl fluid>
      <v-layout row wrap>
        <v-flex sm12>
          <h3>Listado de Usuarios</h3>
        </v-flex>
        <v-flex sm12 md4>
          <v-btn to="/admin/usuarios/nuevo">Nuevo Usuario</v-btn>
        </v-flex>
        <v-spacer></v-spacer>
        <v-flex sm12 md3>
          <v-btn @click="dialog=true">Importar de AD o LDAP</v-btn>
        </v-flex>
        <v-flex lg12>
          <v-card>
            <v-card-title>
              Usuarios
              <div class="flex-grow-1"></div>
              <v-text-field
                v-model="search"
                append-icon="mdi-magnify"
                label="Buscar"
                single-line
                hide-details
              ></v-text-field>
            </v-card-title>
            <v-data-table
              :headers="usuarios.headers"
              :search="search"
              :items="lista_usuarios"
              :footer-props="{
                showFirstLastPage: true,
                itemsPerPage : [10, 25, 50, { text: 'All', value: -1 }],
              }"
              class="elevation-1"
              item-key="name"
              v-model="usuarios.selected"
            >
              <template v-slot:body="{ items }">
                <tbody>
                  <tr v-for="item in items" :key="item.name">
                    <!-- <td>
                      <v-checkbox primary hide-details v-model="selected"></v-checkbox>
                    </td>-->
                    <td>{{ item.nombres }}</td>
                    <td>{{ item.apellidos }}</td>
                    <td>{{ item.username }}</td>
                    <td>
                      <RolesList v-bind:roles="item.roles" v-bind:usuario="item.username"></RolesList>
                    </td>
                    <td>
                      <v-btn
                        depressed
                        outlined
                        icon
                        fab
                        dark
                        color="teal"
                        small
                        @click="gestionaRoles(item)"
                      >
                        <v-icon>mdi-format-list-bulleted</v-icon>
                      </v-btn>
                      <ResetPassword v-bind:usuario="item"></ResetPassword>
                      <EditarUsuario v-bind:usuario="item"></EditarUsuario>
                      <CambiarEstadoUsuario v-bind:usuario="item"></CambiarEstadoUsuario>
                    </td>
                  </tr>
                </tbody>
              </template>
            </v-data-table>
          </v-card>
        </v-flex>
      </v-layout>
      <v-layout>
        <v-dialog v-model="dialog" persistent max-width="600px">
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <span class="headline">Importar Usuarios de Active Directory o LDAP</span>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click=" close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-card-text>
              <v-container grid-list-md justify-end>
                <v-form ref="form">
                  <v-layout wrap>
                    <v-flex xs12 md7>
                      <v-text-field
                        label="SERVIDOR"
                        :rules="inputRules"
                        required
                        v-model="userImportForm.Host"
                      ></v-text-field>
                    </v-flex>
                    <v-flex xs12 md2>
                      <v-text-field
                        label="PUERTO"
                        required
                        :rules="portRules"
                        v-model="userImportForm.Puerto"
                      ></v-text-field>
                    </v-flex>
                    <v-flex xs12 md9>
                      <v-text-field
                        label="USUARIO"
                        required
                        :rules="inputRules"
                        v-model="userImportForm.UserName"
                      ></v-text-field>
                    </v-flex>
                    <v-flex xs12 md9>
                      <v-text-field
                        type="password"
                        label="CONTRASEÑA"
                        required
                        :rules="inputRules"
                        v-model="userImportForm.Password"
                      ></v-text-field>
                    </v-flex>
                    <v-flex xs12 md9>
                      <v-text-field
                        label="UNIDAD ORGANIZATIVA"
                        required
                        v-model="userImportForm.UnidadOrganizativa"
                      ></v-text-field>
                    </v-flex>
                  </v-layout>
                </v-form>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" text @click="close()">Cerrar</v-btn>
              <v-btn color="green darken-1" text @click="ImportUsers">Continuar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-layout>
    </v-container>
  </div>
</template>

<script>
import api from "@/api";
import RolesList from "@/components/admin/RolesList.vue";
import CambiarEstadoUsuario from "@/components/admin/CambiarEstadoUsuario.vue";
import ResetPassword from "@/components/admin/ResetPassword.vue";
import EditarUsuario from "@/components/admin/EditarUsuario.vue";

export default {
  components: {
    RolesList,
    CambiarEstadoUsuario,
    ResetPassword,
    EditarUsuario
  },
  data() {
    return {
      search: "",
      lista_usuarios: [],
      errors: [],
      dialog: false,
      userImportForm: {
        Host: "",
        Puerto: 389,
        UserName: "",
        Password: "",
        UnidadOrganizativa: ""
      },
      inputRules: [v => !!v || "Este campo es requerido"],
      portRules: [
        v => !!v || "Este campo es requerido",
        v => /^[0-9]+$/.test(v) || "Solo números"
      ],
      usuarios: {
        selected: [],
        headers: [
          {
            text: "Nombres",
            value: "nombres"
          },
          {
            text: "Apellidos",
            value: "apellidos"
          },
          {
            text: "Usuario",
            value: "username"
          },
          {
            text: "Roles",
            value: "roles"
          },
          {
            text: "Acciones",
            value: ""
          }
        ],
        items: this.lista_usuarios
      }
    };
  },
  created() {
    this.getUsuariosFromApi();
  },
  computed: {
    users() {
      return this.lista_usuarios;
    }
  },
  methods: {
    getUsuariosFromApi() {
      console.log("entro");
      const url = api.getUrl("api-account", "account/usuarios");
      this.axios
        .get(url)
        .then(response => {
          this.lista_usuarios = response.data;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    },
    close() {
      this.dialog = false;
    },
    ImportUsers() {
      if (this.$refs.form.validate()) {
        const url = api.getUrl("api-account", "account/importarusuarios");
        this.axios
          .post(url, {
            Host: this.userImportForm.Host,
            UserName: this.userImportForm.UserName,
            Puerto: this.userImportForm.Puerto,
            Password: this.userImportForm.Password,
            UnidadOrganizativa: this.userImportForm.UnidadOrganizativa
          })
          .then(response => {
            this.dialog = false;
            vm.$snotify.success(response.data.mensaje);
            this.getUsuariosFromApi();
          })
          .catch(e => {
            vm.$snotify.error(e.response.data.error);
          });
      }
    },
    getUsuarios(limit) {
      return limit ? this.lista_usuarios.slice(0, limit) : this.lista_usuarios;
    },
    gestionaRoles(usuario) {
      this.$router.push({
        name: "Nuevo_Contrato",
        query: {
          usuario
        }
      });
    }
  }
};
</script>

<style></style>
