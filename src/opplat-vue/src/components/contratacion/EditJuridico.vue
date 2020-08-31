<template>
  <v-dialog v-model="dialog" persistent max-width="600px">
    <template v-slot:activator="{ on }">
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small primary--text"
        small
        v-on="on"
        @click="dialog=true"
        slot="activator"
      >
        <v-icon>v-icon notranslate mdi mdi-pen theme--dark</v-icon>
      </v-btn>
    </template>
    <v-card ref="form">
      <v-card-title>
        <span>Editar oferta: {{oferta.nombre}}</span>
      </v-card-title>
      <v-card-text>
        <v-row>
          <v-layout row wrap>
            <v-flex cols="2" md3 class="px-3">
              <v-text-field v-model="oferta.numero" label="Número" prefix="#"></v-text-field>
            </v-flex>
            <v-flex cols="2" md9 class="px-3">
              <v-file-input
                v-model="file"
                show-size
                prepend-icon="mdi-note-multiple"
                label="Seleccione el dictamen del contrato"
              ></v-file-input>
            </v-flex>
          </v-layout>
          <v-layout row wrap>
            <v-flex cols="2" class="px-3">
              <v-menu
                v-model="menu"
                :close-on-content-click="false"
                :nudge-right="40"
                transition="scale-transition"
                offset-y
                min-width="290px"
              >
                <template v-slot:activator="{ on }">
                  <v-text-field
                    v-model="aprobarContrato.fechaDeFirmado"
                    label="Fecha de Firmado"
                    readonly
                    clearable
                    v-on="on"
                    required
                  ></v-text-field>
                </template>
                <v-date-picker v-model="aprobarContrato.fechaDeFirmado" @input="menu = false"></v-date-picker>
              </v-menu>
            </v-flex>
            <v-flex cols="2" class="px-3">
              <v-menu
                v-model="menu1"
                :close-on-content-click="false"
                :nudge-right="40"
                transition="scale-transition"
                offset-y
                min-width="290px"
              >
                <template v-slot:activator="{ on }">
                  <v-text-field
                    v-model="aprobarContrato.FechaDeVencimiento"
                    label="Fecha de Vencimiento"
                    readonly
                    clearable
                    v-on="on"
                    required
                  ></v-text-field>
                </template>
                <v-date-picker v-model="aprobarContrato.FechaDeVencimiento" @input="menu1 = false"></v-date-picker>
              </v-menu>
            </v-flex>
          </v-layout>
          <v-row justify="center">
            <v-radio-group v-model="row" row>
              <v-radio label="Aprobar" value="radio-1"></v-radio>
              <v-radio label="No Aprobar" value="radio-2"></v-radio>
              <v-radio label="En Revisión" value="radio-3"></v-radio>
            </v-radio-group>
          </v-row>
        </v-row>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" dark text @click="aprobarOferta()">Aceptar</v-btn>
          <v-btn text @click="close()">Cancelar</v-btn>
        </v-card-actions>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
<script>
import api from "@/api";

export default {
  props: ["oferta"],
  data: () => ({
    dialog: false,
    menu: false,
    menu1: false,
    aprobarContrato: {
      roles: [],
      contratoId: null,
      fechaDeFirmado: null,
      FechaDeVencimiento: null
    },
    roles: [],
    username: {},
    errorMessages: [],
    errors: []
  }),
  computed: {
    form() {}
  },
  created() {
    this.roles = this.$store.getters.roles;
    this.username = this.$store.getters.usuario;
  },
  watch: {},

  methods: {
    close() {
      this.dialog = false;
    }
  }
};
</script>
<style></style>
