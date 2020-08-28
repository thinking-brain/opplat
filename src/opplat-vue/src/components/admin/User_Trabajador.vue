<template>
  <v-layout>
    <template>
      <v-btn color="primary" @click="dialog=true">Nuevo</v-btn>
    </template>
    <v-dialog v-model="dialog" persistent max-width="720px">
      <v-card>
        <v-toolbar dark fadeOnScroll color="blue darken-3">
          <v-flex>Escoja el usuario perteneciente al trabajador</v-flex>
          <v-spacer></v-spacer>
          <v-toolbar-items>
            <v-btn icon dark @click=" close()">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <v-card-text class="pt-8">
          <v-form>
            <v-container grid-list-md text-xs-center>
              <v-layout row wrap>
                <v-flex sm12 md8 class="pt-3">
                  <v-autocomplete
                    v-model="userTrabajador.trabajadorId"
                    item-text="nombreCompleto"
                    item-value="id"
                    :items="trabajadores"
                    cache-items
                    label="Listado de trabajadores"
                  ></v-autocomplete>
                </v-flex>
                <v-flex sm12 md3 class="pt-3">
                  <v-autocomplete
                    v-model="userTrabajador.username"
                    item-text="username"
                    :items="lista_usuarios"
                    label="Listado de usuarios"
                  ></v-autocomplete>
                </v-flex>
                <v-flex sm12 md1 class="px-3 pt-5">
                  <v-tooltip top color="success">
                    <template v-slot:activator="{ on }">
                      <v-btn
                        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small success--text"
                        small
                        v-on="on"
                        @click="agregar()"
                        slot="activator"
                      >
                        <v-icon>mdi-plus</v-icon>
                      </v-btn>
                    </template>
                    <span>Agregar</span>
                  </v-tooltip>
                </v-flex>
              </v-layout>
            </v-container>
          </v-form>
          <v-flex>
            <v-chip-group multiple column active-class="primary--text">
              <v-chip
                v-for="item in list"
                :key="item.trabajadorId"
                outlined
                close
                @click:close="quitar(item)"
                sm12
                md6
                class="px-1"
              >
                {{ item.nombreCompleto }}
                usuario:
                {{ item.username }}
              </v-chip>
            </v-chip-group>
          </v-flex>
          <v-alert type="error" v-if="validate.show">{{validate.text}}</v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" text @click="save()">Aceptar</v-btn>
          <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-layout>
</template>
<script>
import api from "@/api";

export default {
  data: () => ({
    dialog: false,
    lista_usuarios: [],
    trabajadores: [],
    userTrabajador: {
      username: null,
      trabajadorId: null,
      nombreCompleto: null
    },
    list: [],
    userTrabajadorDto: [],
    messagesTrabajador: "",
    messagesUser: "",
    validate: {
      text: "",
      show: false
    },
    errors: []
  }),
  created() {
    this.getUsuariosFromApi();
    this.getTrabajadoresFromApi();
  },
  methods: {
    getUsuariosFromApi() {
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
      const url = api.getUrl(
        "recursos_humanos",
        "Trabajadores/Byusername?tieneUsername=false"
      );
      this.axios.get(url).then(
        response => {
          this.trabajadores = response.data;
          this.volver = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    save() {
      if (
        this.userTrabajador.username != null &&
        this.userTrabajador.trabajadorId
      ) {
        this.agregar();
      }
      this.userTrabajadorDto = this.list;
      const url = api.getUrl("recursos_humanos", "Trabajadores/AddUserName");
      this.axios.post(url, this.userTrabajadorDto).then(
        response => {
          this.getResponse(response);
          window.location.reload();
          this.dialog = false;
        },
        error => {
          this.validate.text = error.response.data;
          this.validate.show = true;
          vm.$snotify.error(error.response.data);
          console.log(error);
        }
      );
    },
    close() {
      this.dialog = false;
      this.userTrabajador = {
        username: null,
        trabajadorId: null,
        nombreCompleto: null
      };
    },
    agregar() {
      const verificarTrab = this.list.find(
        x => x.trabajadorId === this.userTrabajador.trabajadorId
      );
      const verificarUser = this.list.find(
        x => x.username === this.userTrabajador.username
      );
      if (verificarTrab != null) {
        vm.$snotify.error("Ya se agregó este trabajador");
      } else if (verificarUser != null) {
        vm.$snotify.error("Ya se agregó este usuario");
      } else {
        const t = this.trabajadores.find(
          x => x.id === this.userTrabajador.trabajadorId
        );
        this.userTrabajador.nombreCompleto = t.nombreCompleto;
        this.list.push(this.userTrabajador);
        this.userTrabajador = {
          username: null,
          trabajadorId: null,
          nombreCompleto: null
        };
      }
    },
    quitar(item) {
      this.list.splice(this.list.indexOf(item), 1);
      this.validate.text = "";
      this.validate.show = false;
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    }
  }
};
</script>
<style></style>
