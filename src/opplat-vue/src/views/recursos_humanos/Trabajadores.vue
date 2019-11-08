<template>
  <div class="list-table">
    <v-container grid-list-xl fluid>
      <v-layout row wrap>
        <v-flex sm12>
          <h3>Listado de Trabajadores</h3>
        </v-flex>

        <v-flex lg12>
          <v-card>
            <v-card-title>
              Trabajadores
              <div class="flex-grow-1"></div>
              <ul>
                <li v-for="trabajador in trabajadores" :key="trabajador.id">{{ trabajador.nombre }}</li>
              </ul>
            </v-card-title>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
  </div>


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
import api from "@/api";

export default {
  data: () => ({
    trabajadores: []
  }),
  created() {
    const url = api.getUrl("recursos_humanos", "Trabajadores");
    this.axios.get(url).then(
      response => {
        this.trabajadores = response.data;
      },
      error => {
        console.log(error);
      }
    );
  }
};
</script>

<style></style>
