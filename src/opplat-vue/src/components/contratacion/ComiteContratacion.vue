<template>
  <v-data-table
    :headers="headers"
    :items="comiteDeContratacion"
    :search="search"
    class="elevation-1 pa-5"
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Comité de Contratación</v-toolbar-title>
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
          <v-btn color="primary" @click="dialog=true">Nuevos Integrantes</v-btn>
        </template>
        <!-- Agregar y Editar Comité de Contratación -->
        <v-dialog v-model="dialog" persistent max-width="600">
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm12 md12>{{ formTitle }}</v-flex>
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
              >Seleccione los, o el Miembro del Comité de Contratación</p>
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs12 class="px-3">
                    <v-autocomplete
                      v-model="nuevoMiembro.comiteContratacion"
                      item-text="nombre_Completo"
                      item-value="id"
                      :items="trabajadores"
                      cache-items
                      multiple
                    >
                      <template v-slot:selection="data">
                        <v-chip
                          v-bind="data.attrs"
                          :input-value="data.selected"
                          close
                          @click="data.select"
                          @click:close="remove(data.item)"
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
        <!-- /Agregar y Editar Comité de Contratación -->
        <!-- Delete Comité de Contratación -->
        <v-dialog v-model="dialog2" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="close()">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas eliminar el Comité de Contratación</v-card-title>
            <v-card-text class="text-center">{{trabComiteContratacion.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(trabComiteContratacion)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Comité de Contratación -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
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
    dialog2: false,
    search: "",
    comiteDeContratacion: [],
    trabComiteContratacion: {},
    nuevoMiembro: { comiteContratacion: [] },
    trabajadores: [],
    editedIndex: -1,
    errors: [],
    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "trabComiteContratacion.nombreCompleto"
      },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nuevo Comité de Contratación"
        : "Editar Comité de Contratación";
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
    this.getComiteContratacionFromApi();
    this.getTrabajadoresFromApi();
  },
  methods: {
    getComiteContratacionFromApi() {
      const url = api.getUrl("contratacion", "ComiteContratacion");
      this.axios.get(url).then(
        response => {
          this.comiteDeContratacion = response.data;
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
    save(method) {
      const url = api.getUrl("contratacion", "ComiteContratacion");
      if (method === "POST") {
        this.axios.post(url, this.nuevoMiembro).then(
          response => {
            this.getResponse(response);
            this.nuevoMiembro = { comiteContratacion: [] };
            this.getComiteContratacionFromApi();
            this.dialog = false;
          },
          error => {
            vm.$snotify.error(error.response.data);
            console.log(error);
          }
        );
      }
      if (method === "PUT") {
        this.axios
          .put(
            `${url}/${this.trabComiteContratacion.id}`,
            this.trabComiteContratacion
          )
          .then(
            response => {
              this.getResponse(response);
              this.getComiteContratacionFromApi();
              this.trabComiteContratacion = {};
              this.dialog = false;
            },
            error => {
              console.log(error);
            }
          );
      }
    },
    confirmDelete(item) {
      this.trabComiteContratacion=item.trabComiteContratacion;
      this.dialog2 = true;
    },
    deleteItem() {
      const url = api.getUrl("contratacion", "ComiteContratacion");
      this.axios.delete(`${url}/${this.trabComiteContratacion.id}`).then(
        response => {
          this.getResponse(response);
          this.getComiteContratacionFromApi();
          this.dialog2 = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    close() {
      this.dialog = false;
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    }
  },
  remove(item) {
    const index = this.comiteContratacion.indexOf(item.id);
    if (index >= 0) this.comiteContratacion.splice(index, 1);
  }
};
</script>
