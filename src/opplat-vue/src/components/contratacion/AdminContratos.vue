<template>
  <v-data-table
    :headers="headers"
    :items="adminContratos"
    :search="search"
    class="elevation-1 pa-5"
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Administradores de Contratos</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          label="Buscar"
          single-line
          hide-details
          clearable
          dense
        ></v-text-field>
        <v-spacer></v-spacer>
        <template>
          <v-btn color="primary" dark @click="dialog=true" class="mx-1">Nuevo Administrador</v-btn>
        </template>
        <!-- Agregar Administrador de Contratos -->
        <v-dialog v-model="dialog" persistent max-width="800">
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex>{{ formTitle }}</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click=" close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-form>
              <p
                class="text-center title font-italic mt-12"
              >Seleccione los Administradores de Contratos</p>
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs8 class="px-3">
                    <v-autocomplete
                      v-model="newAdminContratos.administradores"
                      item-text="nombre_Completo"
                      item-value="id"
                      :items="trabajadores"
                      cache-items
                      label="Administradores"
                      multiple
                    >
                      <template v-slot:selection="data">
                        <v-chip
                          v-bind="data.attrs"
                          :input-value="data.selected"
                          close
                          @click="data.select"
                          @click:close="removeAdministradoresContratos(data.item)"
                          outlined
                        >{{ data.item.nombre_Completo }}</v-chip>
                      </template>
                      <template v-slot:item="data">
                        <template v-if="typeof data.item !== 'object'">
                          <v-list-item-content p="data.item"></v-list-item-content>
                        </template>
                        <template v-else>
                          <v-list-item-content>
                            <v-list-item-title v-html="data.item.nombre_Completo"></v-list-item-title>
                          </v-list-item-content>
                        </template>
                      </template>
                    </v-autocomplete>
                  </v-flex>
                  <v-flex xs4 class="px-3">
                    <v-autocomplete
                      v-model="newAdminContratos.departamentoId"
                      item-text="nombre"
                      item-value="id"
                      :items="departamentos"
                      cache-items
                      label="Departamento"
                    ></v-autocomplete>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-form>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Agregar Administrador de Contratos -->
        <!-- Editar Administrador de Contratos -->
        <v-dialog v-model="dialog1" persistent max-width="800">
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex>{{ formTitle }}</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click=" close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-form>
              <p
                class="text-center title font-italic mt-12"
              >Seleccione los Administradores de Contratos</p>
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs8 class="px-3">
                    <p-field
                      v-model="adminContrato.nombreCompleto"
                      item-value="id"
                      label="Administradores"
                      readonly
                    ></p-field>
                  </v-flex>
                  <v-flex xs4 class="px-3">
                    <v-autocomplete
                      v-model="adminContrato.departamentoId"
                      item-text="nombre"
                      item-value="id"
                      :items="departamentos"
                      cache-items
                      label="Departamento"
                    ></v-autocomplete>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-form>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Editar Administrador de Contratos -->

        <!-- Detalles del Administrador -->
        <v-dialog v-model="dialog2" persistent transition="dialog-bottom-transition" flat>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles del Administrador</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>

            <v-container fluid>
              <v-row dense>
                <v-col cols="7">
                  <v-card flat>
                    <v-layout justify-center>
                      <v-layout class="pa-2">
                        <v-list-item two-line>
                          <v-list-item-content>
                            <v-list-item-title>
                              <strong>Departamento:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle>{{departamento.nombre}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                    </v-layout>
                  </v-card>
                </v-col>

                <v-col cols="5">
                  <v-card color="blue darken-3" dark>
                    <v-list-item>
                      <v-layout column align-center xs12 sm10 md6 lg4>
                        <v-avatar size="120" v-if="adminContrato.sexoName==='M'">
                          <img src="img/default-avatar-man.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="120" v-else-if="adminContrato.sexoName==='F'">
                          <img src="img/default-avatar-woman.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="110" v-else>
                          <img src="img/default-avatar.png" class="float-center pa-5" dark />
                        </v-avatar>
                        <v-layout class="pa-2">
                          <v-toolbar-title class="text-capitalize">{{adminContrato.nombre_Completo}}</v-toolbar-title>
                        </v-layout>
                      </v-layout>
                    </v-list-item>
                    <v-row dense>
                      <v-col cols="7">
                        <v-layout class="pa-2">
                          <p>Carnet de Identidad: {{adminContrato.ci}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Nivel de Escolaridad: {{adminContrato.nivelDeEscolaridadName}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Perfil Ocupacional: {{adminContrato.perfilOcupacional}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Color de Ojos: {{adminContrato.colorDeOjosName}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Fecha Nacimiento: {{adminContrato.fecha_Nac}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Correo: {{adminContrato.correo}}</p>
                        </v-layout>
                      </v-col>
                      <v-col cols="5">
                        <v-layout class="pa-2">
                          <p>Sexo: {{adminContrato.sexoName}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Teléfono Fijo: {{adminContrato.telefonoFijo}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Teléfono Movil: {{adminContrato.telefonoMovil}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Color de Piel: {{adminContrato.colorDePielName}}</p>
                        </v-layout>
                        <v-layout class="pa-2">
                          <p>Edad: {{adminContrato.edad}} Años</p>
                        </v-layout>
                      </v-col>
                    </v-row>
                    <v-layout class="pa-2">
                      <p>Direccion: {{adminContrato.direccion}}</p>
                    </v-layout>
                    <v-layout class="pa-2">
                      <p>Otros Datos de Interes: {{adminContrato.otrasCaracteristicas}}</p>
                    </v-layout>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- Detalles del Administrador -->

        <!-- Delete Administrador de Contratos -->
        <v-dialog v-model="dialog3" persistent max-width="600px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="close()">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <p class="text-center pt-5">
              Seguro de Eliminar a
              <strong>
                <b>
                  <u>{{adminContrato.nombreCompleto}}</u>
                </b>
              </strong> como Administrador de Contratos
            </p>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(adminContrato)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Administrador de Contratos -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small primary--text"
        small
        @click="editItem(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-pen theme--dark</v-icon>
      </v-btn>

      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small teal--text"
        small
        @click="getDetalles(item)"
      >
        <v-icon>mdi-format-list-bulleted</v-icon>
      </v-btn>

      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small pink--text"
        small
        @click="confirmDelete(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-delete theme--dark</v-icon>
      </v-btn>
    </template>
  </v-data-table>
</template>
<script>
import api from "@/api";

export default {
  data: () => ({
    dialog: false,
    dialog1: false,
    dialog2: false,
    dialog3: false,
    search: "",
    editedIndex: -1,
    newAdminContratos: {
      administradores: [],
      departamentoId: {}
    },
    adminContratos: [],
    adminContrato: {},
    departamentos: [],
    departamento: {},
    trabajadores: [],
    tabs: null,
    errors: [],
    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "administrador.nombreCompleto"
      },
      { text: "Departamento", value: "departamento.nombre" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nuevo Administrador"
        : "Editar Administrador";
    },
    method() {
      return this.editedIndex === -1 ? "POST" : "PUT";
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {
    this.getAdminContratosFromApi();
    this.getTrabajadoresFromApi();
    this.getDepartamentosFromApi();
  },

  methods: {
    getAdminContratosFromApi() {
      const url = api.getUrl("contratacion", "AdminContratos");
      this.axios.get(url).then(
        response => {
          this.adminContratos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getTrabajadoresFromApi() {
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      this.axios.get(url).then(
        response => {
          this.trabajadores = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getDepartamentosFromApi() {
      const url = api.getUrl("contratacion", "Departamentos");
      this.axios.get(url).then(
        response => {
          this.departamentos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    editItem(item) {
      this.editedIndex = this.adminContratos.indexOf(item);
      this.adminContrato = item.administrador;
      this.adminContrato.departamentoId = item.departamento.id;
      this.dialog1 = true;
    },
    getDetalles(item) {
      const url = api.getUrl("contratacion", "AdminContratos");
      this.axios.get(`${url}/${item.administrador.id}`).then(
        response => {
          this.adminContrato = response.data;
        },
        error => {
          console.log(error);
        }
      );
      this.departamento = item.departamento;
      this.dialog2 = true;
    },
    save(method) {
      const url = api.getUrl("contratacion", "AdminContratos");
      if (method === "POST") {
        this.axios.post(url, this.newAdminContratos).then(
          response => {
            this.getResponse(response);
            this.getAdminContratosFromApi();
            this.newAdminContratos = {
              administradores: [],
              departamentoId: {}
            };
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
      if (method === "PUT") {
        this.newAdminContratos.departamentoId = this.adminContrato.departamentoId;
        this.newAdminContratos.administradores[0] = this.adminContrato.id;
        this.axios
          .put(`${url}/${this.adminContrato.id}`, this.newAdminContratos)
          .then(
            response => {
              this.getResponse(response);
              this.getAdminContratosFromApi();
              this.newAdminContratos = {
                administradores: [],
                departamentoId: {}
              };
              this.adminContrato = {};
              this.dialog1 = false;
            },
            error => {
              console.log(error);
            }
          );
      }
    },
    confirmDelete(item) {
      this.adminContrato = item.administrador;
      this.dialog3 = true;
    },
    deleteItem() {
      const url = api.getUrl("contratacion", "AdminContratos");
      this.axios.delete(`${url}/${this.adminContrato.id}`).then(
        response => {
          this.getResponse(response);
          this.getAdminContratosFromApi();
          this.adminContrato = {};
          this.dialog3 = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    close() {
      this.dialog = false;
      this.dialog1 = false;
      this.dialog2 = false;
      this.dialog3 = false;
      this.newAdminContratos = {
        administradores: [],
        departamentoId: {}
      };
      this.adminContrato = {};
      this.administradores = [];
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    removeAdministradoresContratos(item) {
      const index = this.administradores.indexOf(item.id);
      if (index >= 0) this.administradores.splice(index, 1);
    }
  }
};
</script>