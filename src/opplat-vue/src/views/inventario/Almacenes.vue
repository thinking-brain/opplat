<template>
  <v-container>
    <v-data-table :headers="headers" :items="almacenes" :search="search" class="elevation-1">
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-toolbar-title>Listado de Almacenes</v-toolbar-title>
          <v-divider class="mx-4" inset vertical></v-divider>
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            append-icon="mdi-magnify"
            label="Buscar"
            single-line
            hide-details
            loading="true"
          ></v-text-field>
          <v-spacer></v-spacer>
          <v-layout>
            <v-dialog v-model="dialog" persistent max-width="600px">
              <template v-slot:activator="{ on }">
                <v-btn color="primary" dark v-on="on">Agregar Almacen</v-btn>
              </template>
              <v-card>
                <v-card-title>
                  <span class="headline">{{ formTitle }}</span>
                </v-card-title>
                <v-card-text>
                  <v-container grid-list-md>
                    <v-layout wrap>
                      <v-flex xs12 sm4 md2>
                        <v-text-field label="NOMBRE" required v-model="almacen.nombre"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field label="CODIGO" v-model="almacen.codigo"></v-text-field>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="blue darken-1" text @click="close">Cerrar</v-btn>
                  <v-btn color="green darken-1" text @click="save(method)">Guardar</v-btn>
                </v-card-actions>
              </v-card>
            </v-dialog>
          </v-layout>
        </v-toolbar>
      </template>
      <template v-slot:item.action="{ item }">
        <v-icon small class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
        <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="initialize">Recargar</v-btn>
      </template>
    </v-data-table>
  </v-container>
</template>

<script>
import api from '@/api';

export default {
  data: () => ({
    dialog: false,
    search: '',
    almacen: {
      id: '',
      codigo: '',
      nombre: '',
    },
    defaultItem: {
      codigo: '',
      nombre: '',
    },
    almacenes: [],
    errors: [],
    headers: [
      {
        text: 'Código',
        align: 'left',
        sortable: true,
        value: 'codigo',
      },
      { text: 'Nombre', value: 'nombre' },
      { text: 'Actions', value: 'action', sortable: false },
    ],
    editedIndex: -1,
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? 'Nuevo Almacén' : 'Editar Almacén';
    },
    method() {
      return this.editedIndex === -1 ? 'POST' : 'PUT';
    },
  },

  watch: {
    dialog(val) {
      val || this.close();
    },
  },

  created() {
    this.initialize();
  },

  methods: {
    initialize() {
      const url = api.getUrl('inventario', 'Almacenes');
      this.axios.get(url).then(
        (response) => {
          this.almacenes = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },

    editItem(item) {
      this.editedIndex = this.almacenes.indexOf(item);
      this.almacen = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      const index = this.almacenes.indexOf(item);
      const url = api.getUrl('inventario', 'Almacenes');

      confirm('¿Está seguro de eliminar este almacen?')
        && this.axios.delete(`${url}/${item.id}`).then(
          (response) => {
            this.getResponse(response);
          },
          (error) => {
            console.log(error);
          },
        );
    },

    close() {
      this.dialog = false;
      setTimeout(() => {
        this.almacen = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    },

    save(method) {
      const url = api.getUrl('inventario', 'Almacenes');
      if (method === 'POST') {
        this.axios.post(url, this.almacen).then(
          (response) => {
            this.getResponse(response);
          },
          (error) => {
            console.log(error);
          },
        );
      }
      if (method === 'PUT') {
        this.axios.put(`${url}/${this.almacen.id}`, this.almacen).then(
          (response) => {
            this.getResponse(response);
          },
          (error) => {
            console.log(error);
          },
        );
      }
      this.close();
    },

    getResponse(response) {
      if (response.status == 200) {
        this.dialog = false;
        this.almacen.nombre = '';
        this.almacen.codigo = '';
        vm.$snotify.success('Exito al realizar la operación');
        this.initialize();
      }
    },
  },
};
</script>
