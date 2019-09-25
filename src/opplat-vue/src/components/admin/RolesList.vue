<template>
  <v-layout row justify-center>
    <v-dialog v-model="dialog" persistent max-width="600px">
      <template v-slot:activator="{ on }">
        <v-btn color="primary" dark v-on="on">Ver Roles</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="headline">Listado de roles de usuario: {{usuario}}</span>
        </v-card-title>
        <v-card-text>
          <v-list class="pa-0">
            <template v-for="item in roles">
              <v-subheader v-if="item.header" :key="item.header">{{ item }}</v-subheader>
              <v-list-item v-else :key="item.title">
                <v-list-item-content>{{item}}</v-list-item-content>
              </v-list-item>
            </template>
          </v-list>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" @click="dialog = false">Cerrar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-layout>
</template>
<script>
export default {
  props: ['roles', 'usuario'],
  data: () => ({
    dialog: false,
  }),
};
</script>
<style></style>
