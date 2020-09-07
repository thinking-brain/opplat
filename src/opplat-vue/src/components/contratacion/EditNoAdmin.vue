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
        <span>Editar contrato: {{contrato.nombre}}</span>
      </v-card-title>
      <v-card-text>
        <v-row>
          <v-layout row wrap>
            <v-flex cols="2" md3 class="px-3">
              <v-text-field v-model="contrato.numero" label="Número" prefix="#"></v-text-field>
            </v-flex>
            <v-flex cols="2" md9 class="px-3">
              <v-file-input
                v-model="contrato.file"
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
                    v-model="contrato.fechaDeFirmado"
                    label="Fecha de Firmado"
                    readonly
                    clearable
                    v-on="on"
                    required
                  ></v-text-field>
                </template>
                <v-date-picker v-model="contrato.fechaDeFirmado" @input="menu = false"></v-date-picker>
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
                    v-model="contrato.fechaDeVencimiento"
                    label="Fecha de Vencimiento"
                    readonly
                    clearable
                    v-on="on"
                    required
                  ></v-text-field>
                </template>
                <v-date-picker v-model="contrato.fechaDeVencimiento" @input="menu1 = false"></v-date-picker>
              </v-menu>
            </v-flex>
          </v-layout>
          <v-row justify="center">
            <v-flex cols="2" class="px-3">
              <v-autocomplete
                v-model="contrato.estado"
                item-text="nombre"
                item-value="id"
                :items="estados"
                label="Estado del contrato"
              ></v-autocomplete>
            </v-flex>
          </v-row>
        </v-row>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" dark text @click="save()">Aceptar</v-btn>
          <v-btn text @click="dialog=false">Cancelar</v-btn>
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
    contrato: {
      roles: [],
      contratoId: 0,
      fechaDeFirmado: null,
      estado: null,
      dictamen: null,
      fechaDeVencimiento: null,
      file: null
    },
    roles: [],
    username: {},
    errorMessages: [],
    errors: [],
    estados: []
  }),
  computed: {
    form() {}
  },
  created() {
    this.roles = this.$store.getters.roles;
    this.username = this.$store.getters.usuario;
    this.contrato.id = this.oferta.id;
    this.getEstadosParaAprobarFromApi();
    this.getEstadoByRool();
  },
  watch: {},

  methods: {
    getEstadosParaAprobarFromApi() {
      const url = api.getUrl("contratacion", "contratos/EstadosParaAprobar");
      this.axios.get(url).then(
        response => {
          this.estados = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getEstadoByRool() {
      const url = api.getUrl(
        "contratacion",
        `contratos/EstadoByRool?id=${this.contrato.id}&roles=${this.roles}`
      );
      this.axios.get(url).then(
        response => {
          this.contrato.estado = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    save() {
      const url = api.getUrl("contratacion", "contratos/EditNoAdmin");
      this.contrato.contratoId = this.oferta.id;
      this.contrato.roles = this.roles;
      this.contrato.username = this.username;

      if (this.contrato.file != null && this.contrato.contratoId != 0) {
        this.axios.put(url, this.contrato).then(
          response => {
            if (response.status === 200 || response.status === 201) {
              vm.$snotify.success("Exito al realizar la operación");
            }
            this.dialog = false;
          },
          error => {
            vm.$snotify.error(error.response.data);
            console.log(error);
          }
        );
      }
    }
  }
};
</script>
<style></style>
