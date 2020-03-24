<template>
  <v-data-table
    :headers="headers"
    :items="Contratos"
    :search="search"
    item-key="id"
    class="elevation-1 pa-5"
    dense
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Contratos</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          label="Buscar"
          single-line
          hide-details
          clearable
        ></v-text-field>
        <v-spacer></v-spacer>
        <!-- Agregar y Editar Contrato -->
        <v-dialog v-model="dialog" persistent>
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on">Agregar Contrato</v-btn>
          </template>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>{{ formTitle }}</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click=" close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>

            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Agregar y Editar Contrato -->

        <!-- Detalles del Contrato -->
        <v-dialog v-model="dialog2" persistent transition="dialog-bottom-transition" flat>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles de la Contrato</v-flex>
              <v-spacer></v-spacer>
            </v-toolbar>
          </v-card>
        </v-dialog>
        <!-- Detalles  del Contrato -->

        <!-- Delete Trajador -->
        <v-dialog v-model="dialog4" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="dialog4 = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas eliminar la Contrato</v-card-title>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(trabajador)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Trajador -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="editItem(item)">mdi-pencil</v-icon>
        </template>
        <span>Editar</span>
      </v-tooltip>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon
            small
            class="mr-2"
            v-on="on"
            @click="getDetallesProfFromApi(item)"
          >mdi-account-plus</v-icon>
        </template>
        <span>Detalles</span>
      </v-tooltip>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="deleteItem(item)">mdi-delete</v-icon>
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
    search: "",
    editedIndex: -1,
    trabajadores: [],
    Contratos: [],
    date: new Date().toISOString().substr(0, 10),
    menu: false,
    menu1: false,
    menu2: false,
    modal: false,
    errors: [],
    headers: [
      {
        text: "Nombre de la Contrato",
        align: "left",
        sortable: true,
        value: "nombre"
      },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nuevo Contrato" : "Editar Contrato";
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
    this.getContratosFromApi();
    this.getTrabajadoresFromApi();
  },

  methods: {
    getContratosFromApi() {
      const url = api.getUrl("contratacion", "Contratos");
      this.axios.get(url).then(
        response => {
          this.Contratos = response.data;
          this.volver = false;
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
          this.volver = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    save(method) {
      this.axios.post(url, this.trabajador).then(
        response => {
          this.getResponse(response);
          this.getTrabajadoresBolsa();
          this.dialog = false;
          this.trabajador = {
            sexo: 0,
            colorDeOjos: 0,
            colorDePiel: 0,
            tallaDeCamisa: 0,
            nombre_Referencia: "",
            perfilOcupacionalId: -1
          };
        },
        error => {
          console.log(error);
        }
      );

      if (method === "PUT") {
        this.axios.put(`${url}/${this.trabajador.id}`, this.trabajador).then(
          response => {
            this.getResponse(response);
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
    }
  },
  editItem(item) {
    this.editedIndex = this.trabajadores.indexOf(item);
    this.trabajador = Object.assign({}, item);
    this.dialog = true;
  },
  confirmDelete(item) {
    this.trabajador = Object.assign({}, item);
    this.dialog4 = true;
  },
  deleteItem(trabajador) {
    const url = api.getUrl("recursos_humanos", "Trabajadores");
    this.axios.delete(`${url}/${trabajador.id}`).then(
      response => {
        this.getResponse(response);
      },
      error => {
        console.log(error);
      }
    );
  },
  getResponse(response) {
    if (response.status === 200 || response.status === 201) {
      vm.$snotify.success("Exito al realizar la operación");
      this.getTrabajadoresFromApi();
    }
  },
  close() {
    this.dialog = false;

    this.getTrabajadoresFromApi();
    setTimeout(() => {
      this.editedIndex = -1;
    }, 300);
  }
};
</script>