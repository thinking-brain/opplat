<template>
  <div class="list-table">
    <v-container grid-list-xl fluid>
      <v-layout row wrap>
        <v-flex sm12>
          <h3>Listado de Usuarios</h3>
        </v-flex>
        <v-flex sm12 md4>
          <v-btn color="secondary" @click="dialogNewUser = true"
            >Nuevo Usuario</v-btn
          >
        </v-flex>
        <v-spacer></v-spacer>
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
                itemsPerPage: [10, 25, 50, { text: 'All', value: -1 }],
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
                      <RolesList
                        v-bind:roles="item.roles"
                        v-bind:usuario="item.username"
                      ></RolesList>
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
                      <ResetPassword :usuario="item"></ResetPassword>
                      <EditarUsuario :usuario="item"></EditarUsuario>
                      <CambiarEstadoUsuario
                        :usuario="item"
                      ></CambiarEstadoUsuario>
                    </td>
                  </tr>
                </tbody>
              </template>
            </v-data-table>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
    <NuevoUsuario :modal="dialogNewUser" @close="close" />
    <GestionRoles
      :usuario="usuario"
      :modal="dialogGestionRoll"
      @close="close"
    />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
import EditarUsuario from "@/components/Admin/EditarUsuario.vue";
import RolesList from "@/components/Admin/RolesList.vue";
import ResetPassword from "@/components/Admin/ResetPassword.vue";
import CambiarEstadoUsuario from "@/components/Admin/CambiarEstadoUsuario.vue";
import NuevoUsuario from "@/components/Admin/NuevoUsuario.vue";
import GestionRoles from "@/components/Admin/GestionRoles.vue";
import getLocalData from "@/util/getLocalData";

@Component({
  components: {
    EditarUsuario,
    RolesList,
    ResetPassword,
    CambiarEstadoUsuario,
    NuevoUsuario,
    GestionRoles,
  },
})
export default class UserList extends Vue {
  search = "";
  lista_usuarios = [];
  errors = [];
  dialog = false;
  dialogNewUser = false;
  dialogGestionRoll = false;
  usuario = {};
  userImportForm = {
    Host: "",
    Puerto: 389,
    UserName: "",
    Password: "",
    UnidadOrganizativa: "",
  };
  inputRules = [(v) => !!v || "Este campo es requerido"];
  portRules = [
    (v) => !!v || "Este campo es requerido",
    (v) => /^[0-9]+$/.test(v) || "Solo números",
  ];
  usuarios = {
    selected: [],
    headers: [
      {
        text: "Nombres",
        value: "nombres",
      },
      {
        text: "Apellidos",
        value: "apellidos",
      },
      {
        text: "Usuario",
        value: "username",
      },
      {
        text: "Roles",
        value: "roles",
      },
      {
        text: "Acciones",
        value: "",
      },
    ],
  };

  // items: this.lista_usuarios,
  created() {
    this.getUsuariosFromApi();
  }

  get users() {
    return this.lista_usuarios;
  }

  getUsuariosFromApi() {
    this.lista_usuarios = getLocalData.getUsers();
  }
  close() {
    this.dialog = false;
    this.dialogNewUser = false;
    this.dialogGestionRoll = false;
  }
  ImportUsers() {}
  getUsuarios(limit) {
    return limit ? this.lista_usuarios.slice(0, limit) : this.lista_usuarios;
  }
  gestionaRoles(usuario) {
    this.usuario = usuario;
    this.dialogGestionRoll = true;
  }
}
</script>

<style></style>
