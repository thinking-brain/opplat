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
        <template>
          <v-btn color="primary" @click="dialog=true">Nuevo Dictaminador</v-btn>
        </template>
        <!-- Agregar Dictaminador de Contratos -->
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
            <v-form ref="form">
              <p
                class="text-center title font-italic mt-12"
              >Seleccione los Dictaminadores de Contratos</p>
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs8 class="px-3">
                    <v-autocomplete
                      v-model="dictContratos.dictaminadores"
                      item-text="nombreCompleto"
                      item-value="id"
                      :items="trabajadores"
                      cache-items
                      label="Dictaminadores"
                      multiple
                    >
                      <template v-slot:selection="data">
                        <v-chip
                          v-bind="data.attrs"
                          :input-value="data.selected"
                          close
                          @click="data.select"
                          @click:close="removeDictaminadoresContratos(data.item)"
                          outlined
                        >{{ data.item.nombreCompleto }}</v-chip>
                      </template>
                      <template v-slot:item="data">
                        <template v-if="typeof data.item !== 'object'">
                          <v-list-item-content v-text="data.item"></v-list-item-content>
                        </template>
                        <template v-else>
                          <v-list-item-content>
                            <v-list-item-title v-html="data.item.nombreCompleto"></v-list-item-title>
                          </v-list-item-content>
                        </template>
                      </template>
                    </v-autocomplete>
                  </v-flex>
                  <v-flex xs4 class="px-3">
                    <v-autocomplete
                      v-model="dictContratos.departamentoId"
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
        <!-- /Agregar Dictaminador de Contratos -->
        <!-- Editar Dictaminador de Contratos -->
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
            <v-form ref="form">
              <p
                class="text-center title font-italic mt-12"
              >Seleccione los Dictaminadores de Contratos</p>
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs6 class="px-3">
                    <v-text-field
                      v-model="dictaminador.nombreCompleto"
                      item-value="id"
                      label="Dictaminador"
                      readonly
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-3">
                    <v-autocomplete
                      v-model="dictaminador.departamentoId"
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
        <!-- /Editar Dictaminador de Contratos -->
        <!-- Detalles del Dictaminador -->
        <v-dialog
          v-model="dialog2"
          persistent
          transition="dialog-bottom-transition"
          flat
          max-width="1100"
        >
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles del Dictaminador</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-container fluid>
              <v-row dense>
                <v-col cols="6">
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
                <v-col cols="6">
                  <v-card color="blue darken-3" dark>
                    <v-list-item>
                      <v-layout column align-center xs12 sm10 md6 lg4>
                        <v-avatar size="120" v-if="dictaminador.sexoName==='M'">
                          <img src="img/default-avatar-man.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="120" v-else-if="dictaminador.sexoName==='F'">
                          <img src="img/default-avatar-woman.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="110" v-else>
                          <img src="img/default-avatar.png" class="float-center pa-5" dark />
                        </v-avatar>
                        <v-layout class="pa-2">
                          <v-toolbar-title class="text-capitalize">{{dictaminador.nombreCompleto}}</v-toolbar-title>
                        </v-layout>
                      </v-layout>
                    </v-list-item>
                    <v-row dense>
                      <v-col cols="7">
                        <v-layout class="pa-2">
                          <v-text>Carnet de Identidad: {{dictaminador.ci}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Nivel de Escolaridad: {{dictaminador.nivelDeEscolaridadName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Perfil Ocupacional: {{dictaminador.perfilOcupacional}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Color de Ojos: {{dictaminador.colorDeOjosName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Fecha Nacimiento: {{dictaminador.fecha_Nac}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Correo: {{dictaminador.correo}}</v-text>
                        </v-layout>
                      </v-col>
                      <v-col cols="5">
                        <v-layout class="pa-2">
                          <v-text>Sexo: {{dictaminador.sexoName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Teléfono Fijo: {{dictaminador.telefonoFijo}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Teléfono Movil: {{dictaminador.telefonoMovil}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Color de Piel: {{dictaminador.colorDePielName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Edad: {{dictaminador.edad}} Años</v-text>
                        </v-layout>
                      </v-col>
                    </v-row>
                    <v-layout class="pa-2">
                      <v-text>Direccion: {{dictaminador.direccion}}</v-text>
                    </v-layout>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- Detalles del Dictaminador -->

        <!-- Delete Dictaminador de Contratos -->
        <v-dialog v-model="dialog3" persistent max-width="500px">
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
                  <u>{{dictaminador.nombre}}</u>
                </b>
              </strong> como Dictaminador de Contratos
            </p>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(dictaminador)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Dictaminador de Contratos -->
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
    dictContratos: {
      dictaminadores: [],
      departamentoId: {}
    },
    dictaminadoresContratos: [],
    dictaminador: {},
    departamento: {},
    trabajador: {},
    trabajadores: [],
    departamentos: [],
    tabs: null,
    errors: [],
    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "dictaminador.nombreCompleto"
      },

      { text: "Departamento", value: "departamento.nombre" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nuevos Dictaminadores"
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
    this.getDepartamentosFromApi();
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
      const url = api.getUrl(
        "contratacion",
        "DictContratos/TrabNoDictaminadores"
      );
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
      this.editedIndex = 1;
      this.dictaminador = item.dictaminador;
      this.dictaminador.departamentoId = item.departamento.id;
      this.dialog1 = true;
    },
    getDetalles(item) {
      const url = api.getUrl("contratacion", "DictContratos");
      this.axios.get(`${url}/${item.dictaminador.id}`).then(
        response => {
          this.dictaminador = response.data;
        },
        error => {
          console.log(error);
        }
      );
      this.departamento = item.departamento;
      this.dialog2 = true;
    },
    save(method) {
      const url = api.getUrl("contratacion", "DictContratos");
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          this.snackbar = true;
        }
        if (this.dictContratos == null) {
          vm.$snotify.error("Faltan campos por llenar que son obligatorios");
        } else {
          this.axios.post(url, this.dictContratos).then(
            response => {
              this.getResponse(response);
              this.getDictContratosFromApi();
              this.dictContratos = {};
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
        }
      }
      if (method === "PUT") {
        this.dictContratos.departamentoId = this.dictaminador.departamentoId;
        this.dictContratos.dictaminadores[0] = this.dictaminador.id;
        this.axios
          .put(`${url}/${this.dictaminador.id}`, this.dictContratos)
          .then(
            response => {
              this.getDictContratosFromApi();
              this.dictContratos = {
                dictaminadores: [],
                departamentoId: {}
              };
              this.dictaminador = {};
              this.dialog1 = false;
              this.getResponse(response);
            },
            error => {
              console.log(error);
            }
          );
      }
    },
    confirmDelete(item) {
      this.dictaminador = item.dictaminador;
      this.dialog3 = true;
    },
    deleteItem() {
      const url = api.getUrl("contratacion", "DictContratos");
      this.axios.delete(`${url}/${this.dictaminador.id}`).then(
        response => {
          this.getResponse(response);
          this.getDictContratosFromApi();
          this.dictaminador={};
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
      this.dictContratos = {
        dictaminadores: [],
        departamentoId: {}
      };
      this.dictaminador = {};
      this.departamento = {};
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    removeDictaminadoresContratos(item) {
      const index = this.dictContratos.indexOf(item.id);
      if (index >= 0) this.dictContratos.splice(index, 1);
    }
  }
};
</script>