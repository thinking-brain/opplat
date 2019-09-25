<template>
  <div class="list-table">
    <v-container grid-list-xl fluid>
      <v-layout row wrap>
        <v-flex sm12>
          <h3>Listado de Usuarios</h3>
        </v-flex>
        <v-flex sm12>
          <v-btn to="/admin/usuarios/nuevo">Nuevo Usuario</v-btn>
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
                      <RolesList v-bind:roles="item.roles"
                        v-bind:usuario="item.username">
                      </RolesList>
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
    </v-container>
  </div>
</template>

<script>
import api from '@/api';
import RolesList from '@/components/admin/RolesList.vue';
import CambiarEstadoUsuario from '@/components/admin/CambiarEstadoUsuario.vue';
import ResetPassword from '@/components/admin/ResetPassword.vue';
import EditarUsuario from '@/components/admin/EditarUsuario.vue';

export default {
  components: {
    RolesList, CambiarEstadoUsuario, ResetPassword, EditarUsuario,
  },
  data() {
    return {
      search: '',
      lista_usuarios: [],
      errors: [],
      usuarios: {
        selected: [],
        headers: [
          {
            text: 'Nombres',
            value: 'nombres',
          },
          {
            text: 'Apellidos',
            value: 'apellidos',
          },
          {
            text: 'Usuario',
            value: 'username',
          },
          {
            text: 'Roles',
            value: 'roles',
          },
          {
            text: 'Acciones',
            value: '',
          },
        ],
        items: this.lista_usuarios,
      },
    };
  },
  created() {
    this.getUsuariosFromApi();
  },
  computed: {
    users() {
      return this.lista_usuarios;
    },
  },
  methods: {
    getUsuariosFromApi() {
      const url = api.getUrl('api-account', 'account/usuarios');
      this.axios
        .get(url)
        .then((response) => {
          this.lista_usuarios = response.data;
        })
        .catch((e) => {
          this.errors.push(e);
          vm.$snotify.error(
            'No nos podemos comunicar con el servicio de usuarios, contacte al administrador.',
          );
        });
    },
    getUsuarios(limit) {
      return limit ? this.lista_usuarios.slice(0, limit) : this.lista_usuarios;
    },
    gestionaRoles(usuario) {
      this.$router.push({
        name: 'gestionar-roles',
        query: {
          usuario,
        },
      });
    },
  },
};
</script>

<style></style>
