<template>
  <v-data-table
    :headers="headers"
    :items="dictaminadoresContratos"
    :search="search"
    class="elevation-1 pa-5"
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Dictaminadores de Contratos</v-toolbar-title>
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
        <!-- Agregar y Editar Dictaminador de Contratos -->
        <v-dialog v-model="dialog" persistent max-width="400">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on" class="mx-1">Nuevo Dictaminador</v-btn>
          </template>
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
            <v-form ref="form" v-model="valid" lazy-validation>
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs12 class="px-3">
                    <v-autocomplete
                      v-model="dictaminadorContrato.trabajadorId"
                      item-text="nombre_Completo"
                      item-value="id"
                      :items="trabajadores"
                      :filter="activeFilter"
                      :rules="trabajadorIdRules"
                      label="Dictaminador"
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
        <!-- /Agregar y Editar Dictaminador de Contratos -->

        <!-- Detalles del Dictaminador -->
        <v-dialog v-model="dialog3" persistent transition="dialog-bottom-transition" flat>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles del Dictaminador</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="dialog3 = false">
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
                              <strong>Unidad Organizativa:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle>{{dictaminadorContrato.unidadOrganizativa}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-list-item two-line>
                          <v-list-item-content>
                            <v-list-item-title>
                              <strong>Cargo:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle>{{dictaminadorContrato.cargo}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-list-item two-line>
                          <v-list-item-content>
                            <v-list-item-title>
                              <strong>Estado:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle>{{dictaminadorContrato.estadoTrabajadorName}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-list-item two-line>
                        <v-list-item-content>
                          <v-list-item-title>
                            <strong>Funciones o Tareas Principales del Cargo:</strong>
                          </v-list-item-title>
                          <v-list-item-subtitle
                            v-for="item in funciones"
                            :key="item.text"
                          >- {{item.text}}</v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-list-item two-line>
                        <v-list-item-content>
                          <v-list-item-title>
                            <strong>Requisitos de Conocimiento para el Cargo:</strong>
                          </v-list-item-title>
                          <v-list-item-subtitle
                            v-for="item in requisitos"
                            :key="item.text"
                          >- {{item.text}}</v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                    </v-layout>
                  </v-card>
                </v-col>

                <v-col cols="5">
                  <v-card color="blue darken-3" dark>
                    <v-list-item>
                      <v-layout column align-center xs12 sm10 md6 lg4>
                        <v-avatar size="120" v-if="dictaminadorContrato.sexoName==='M'">
                          <img src="img/default-avatar-man.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="120" v-else-if="dictaminadorContrato.sexoName==='F'">
                          <img src="img/default-avatar-woman.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="110" v-else>
                          <img src="img/default-avatar.png" class="float-center pa-5" dark />
                        </v-avatar>
                        <v-layout class="pa-2">
                          <v-toolbar-title
                            class="text-capitalize"
                          >{{dictaminadorContrato.nombre_Completo}}</v-toolbar-title>
                        </v-layout>
                      </v-layout>
                    </v-list-item>
                    <v-row dense>
                      <v-col cols="7">
                        <v-layout class="pa-2">
                          <v-text>Carnet de Identidad: {{dictaminadorContrato.ci}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Nivel de Escolaridad: {{dictaminadorContrato.nivelDeEscolaridadName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Perfil Ocupacional: {{dictaminadorContrato.perfilOcupacional}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Color de Ojos: {{dictaminadorContrato.colorDeOjosName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Fecha Nacimiento: {{dictaminadorContrato.fecha_Nac}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Correo: {{dictaminadorContrato.correo}}</v-text>
                        </v-layout>
                      </v-col>
                      <v-col cols="5">
                        <v-layout class="pa-2">
                          <v-text>Sexo: {{dictaminadorContrato.sexoName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Teléfono Fijo: {{dictaminadorContrato.telefonoFijo}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Teléfono Movil: {{dictaminadorContrato.telefonoMovil}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Color de Piel: {{dictaminadorContrato.colorDePielName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Edad: {{dictaminadorContrato.edad}} Años</v-text>
                        </v-layout>
                      </v-col>
                    </v-row>
                    <v-layout class="pa-2">
                      <v-text>Direccion: {{dictaminadorContrato.direccion}}</v-text>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-text>Otros Datos de Interes: {{dictaminadorContrato.otrasCaracteristicas}}</v-text>
                    </v-layout>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- Detalles del Dictaminador -->

        <!-- Delete Dictaminador de Contratos -->
        <v-dialog v-model="dialog2" persistent max-width="400px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="dialog2 = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title
              class="headline text-center"
            >Seguro de Eliminar a {{dictaminadorContrato.trabajadorId}} como Dictaminador de Contratros</v-card-title>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(dictaminadorContrato)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Dictaminador de Contratos -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="getDetallesDict(item)">mdi-account-plus</v-icon>
        </template>
        <span>Detalles</span>
      </v-tooltip>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="confirmDelete(item)">mdi-trash-can</v-icon>
        </template>
        <span>Eliminar</span>
      </v-tooltip>
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
    dictaminadoresContratos: [],
    dictaminadorContrato: {},
    trabajador: {},
    trabajadores: [],
    tabs: null,
    trabajadorIdRules: [v => !!v || "El Nombre del Trabajador es Requerido"],
    errors: [],
    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "nombreCompleto"
      },

      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nuevo Dictaminador"
        : "Editar Dictaminador";
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
    this.getDictContratosFromApi();
    this.getTrabajadoresFromApi();
  },

  methods: {
    getDictContratosFromApi() {
      const url = api.getUrl("contratacion", "DictContratos");
      this.axios.get(url).then(
        response => {
          this.dictaminadoresContratos = response.data;
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
    editItem(item) {
      this.editedIndex = this.dictaminadorContrato.indexOf(item);
      this.dictaminadorContrato = Object.assign({}, item);
      this.dialog = true;
    },
    getDetallesDict(item) {
      this.dictaminadorContrato = this.trabajadores.find(t => t.id === item.id);
      this.dialog3 = true;
    },
    save(method) {
      const url = api.getUrl("contratacion", "DictContratos");
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          this.snackbar = true;
        }
        if (this.dictaminadorContrato.trabajadorId == null) {
          vm.$snotify.error("Faltan campos por llenar que son obligatorios");
        } else {
          this.axios.post(url, this.dictaminadorContrato).then(
            response => {
              this.getResponse(response);
              this.getDictContratosFromApi();
              this.dictaminadorContrato = {};
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
        }
      }
      if (method === "PUT") {
        this.axios
          .put(
            `${url}/${this.dictaminadorContrato.id}`,
            this.dictaminadorContrato
          )
          .then(
            response => {
              this.getResponse(response);
              this.getDictContratosFromApi();
              this.dictaminadorContrato = {};
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
      }
    },
    confirmDelete(item) {
      this.dictaminadorContrato = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(dictaminadorContrato) {
      const url = api.getUrl("contratacion", "DictContratos");
      this.axios.delete(`${url}/${dictaminadorContrato.id}`).then(
        response => {
          this.getResponse(response);
          this.getDictContratosFromApi();
          this.dialog2 = false;
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
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    }
  }
};
</script>