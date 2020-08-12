<template>
  <div class="list-table">
    <v-container grid-list-xl fluid>
      <v-layout row wrap>
        <v-flex sm12>
          <h3>Usuarios VS Trabajadores</h3>
        </v-flex>
        <v-flex sm12 md3 class="px-6">
          <UserTrabajador></UserTrabajador>
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
              :items="trabajadores"
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
                    <td>{{ item.nombreCompleto }}</td>
                    <td>{{ item.user.username }}</td>
                    <td>
                      <v-tooltip top color="pink">
                        <template v-slot:activator="{ on }">
                          <v-btn
                            class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small pink--text"
                            small
                            v-on="on"
                            @click="confirmDelete(item)"
                          >
                            <v-icon>v-icon notranslate mdi mdi-delete theme--dark</v-icon>
                          </v-btn>
                        </template>
                        <span>Eliminar</span>
                      </v-tooltip>
                    </td>
                  </tr>
                </tbody>
              </template>
            </v-data-table>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
    <!-- Dejar el userId null -->
    <v-dialog v-model="dialog" persistent max-width="350px">
      <v-toolbar dark fadeOnScroll color="red">
        <v-spacer></v-spacer>
        <v-toolbar-items>
          <v-btn icon dark @click="dialog2 = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-toolbar-items>
      </v-toolbar>
      <v-card>
        <v-card-title class="headline text-center">Seguro quieres dejar con el usuarios null a</v-card-title>
        <v-card-text class="headline text-center">{{trabajador.nombre}}</v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="red" dark @click="userNull()">Aceptar</v-btn>
          <v-btn color="primary" @click="close()">Cancelar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <!-- /Dejar el userId null -->
  </div>
</template>

<script>
import api from "@/api";
import UserTrabajador from "@/components/admin/User_Trabajador.vue";

export default {
  components: {
    UserTrabajador
  },
  data() {
    return {
      search: "",
      dialog: false,
      lista_usuarios: [],
      trabajadores: [],
      trabajador: {},
      trab: [],
      errors: [],
      dialog: false,
      usuarios: {
        selected: [],
        headers: [
          {
            text: "Nombre del Trabajador",
            value: "nombreCompleto"
          },
          {
            text: "Usuario asignado al Trabajador",
            value: "user.username"
          },
          {
            text: "Acciones",
            value: ""
          }
        ],
        items: this.trabajadores
      }
    };
  },
  created() {
    this.getUsuariosFromApi();
    this.getTrabajadoresFromApi();
  },
  computed: {},
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
    getTrabajadoresFromApi() {
      this.trabajadores = [];
      const url = api.getUrl(
        "recursos_humanos",
        "Trabajadores/ByUserId?tieneUserId=true"
      );
      this.axios.get(url).then(
        response => {
          this.trab = response.data;
          for (let i = 0; i < this.trab.length; i++) {
            const u = this.lista_usuarios.find(
              x => x.userId === this.trab[i].user
            );
            this.trab[i].user = u;
            this.trabajadores.push(this.trab[i]);
          }
          this.volver = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    confirmDelete(item) {
      this.trabajador = Object.assign({}, item);
      this.dialog = true;
    },
    userNull() {
      const url = api.getUrl("recursos_humanos", "Trabajadores/NullUserId");
      this.axios.put(`${url}/${this.trabajador.id}`, this.trabajador).then(
        response => {
          this.getResponse(response);
          this.close();
        },
        error => {
          console.log(error);
        }
      );
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    close() {
      this.getTrabajadoresFromApi();
      this.dialog = false;
    }
  }
};
</script>

<style></style>
