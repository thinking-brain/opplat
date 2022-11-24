<template>
  <v-layout row justify-center>
    <v-dialog v-model="dialog" persistent max-width="600px">
      <template v-slot:activator="{ on }">
        <v-btn color="primary" dark v-on="on">Ver Roles</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="headline"
            >Listado de roles de usuario: {{ dataModel }}</span
          >
        </v-card-title>
        <v-card-text>
          <v-list class="pa-0">
            <template v-for="item in userRoles">
              <v-subheader v-if="item.header" :key="item.header">{{
                item
              }}</v-subheader>
              <v-list-item v-else :key="item.title">
                <v-list-item-content>{{ item }}</v-list-item-content>
              </v-list-item>
            </template>
          </v-list>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="error" elevation="8" @click="dialog = false"
            >Cerrar</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-layout>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue, Watch } from "vue-property-decorator";

@Component({ components: {} })
export default class RolesList extends Vue {
  @PropSync("usuario") public dataModel!: any;
  @PropSync("roles") public userRoles!: any;

  //  Data
  public dialog: boolean = false;
}
</script>
<style></style>
