<template>
  <v-container>
    <v-divider class="d-print-none"></v-divider>
    <v-row>
      <v-data-table :headers="headers" :items="trabajadores" :search="search" class="elevation-1">
        <template v-slot:top>
          <v-toolbar flat color="white">
            <v-toolbar-title>Listado de Trabajadores por </v-toolbar-title>
            <v-divider class="mx-4" inset vertical></v-divider>
            <v-spacer></v-spacer>
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              label="Buscar"
              single-line
              hide-details
            ></v-text-field>
            <v-spacer></v-spacer>
            <v-layout>
              <!-- <v-dialog v-model="dialog" persistent max-width="600px">
                <template v-slot:activator="{ on }">
                  <v-btn color="primary" dark v-on="on">Agregar Trabajador</v-btn>
                </template>
                <v-card>
                  <v-card-title>
                    <span class="headline">{{ formTitle }}</span>
                  </v-card-title>
                  <v-card-text>
                    <v-container grid-list-md>
                      <v-layout wrap>
                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            label="Nombre Completo"
                            required
                            v-model="trabajador.nombre"
                          ></v-text-field>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-text-field label="CI" v-model="trabajador.ci"></v-text-field>
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-text-field label="Cargo" v-model="trabajador.cargo"></v-text-field>
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
              </v-dialog>-->
            </v-layout>
          </v-toolbar>
        </template>
        <template v-slot:item.action="{ item }">
          <v-icon small class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
          <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
        </template>
      </v-data-table>
    </v-row>
  </v-container>
</template>

<script>
import api from "@/api";

export default {
  data() {
    return {
      unidadOrganizativa: { id: 0, nombre: "Ninguno" },
      trabajadores: [],
      errors: [],
       headers: [
      {
        text: "Nombre y Apellidos",
        align: "left",
        sortable: true,
        value: "nombre_Completo"
      },
      { text: "Carnet de Identidad", value: "ci" },
      { text: "Dirección", value: "direccion" },
      { text: "Municipio y Provincia", value: "municipioProv" },
      { text: "Teléfono Fijo", value: "telefonoFijo" },
      { text: "Teléfono Móvil", value: "telefonoMovil" },
      { text: "Cargo", value: "cargo", sortable: false }
      // { text: "Actions", value: "action", sortable: false }
    ],
    };
  },

  methods: {
    loadReporte(unidadOrganizativa) {
      this.unidadOrganizativa = unidadOrganizativa;
      this.trabajadores = [];
      this.getUnidadOrganizativaFromApi();
    },

    //Trabajadores por UnidadOrganizativa
    initialize() {
      const url = api.getUrl("recursos_humanos", "UnidadOrganizativa");
      this.axios
        .get(url + "/" + 1)
        .then(response => {
          this.trabajadores = response.data;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    }
  }
};
</script>
