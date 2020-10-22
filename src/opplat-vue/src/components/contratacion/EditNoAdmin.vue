<template>
  <v-card ref="form">
    <v-row justify="center">
      <v-card-title>
        <span>Editar: {{contrato.nombre}}</span>
      </v-card-title>
    </v-row>
    <v-form>
      <v-card-text>
        <p class="text-left title">Datos a llenar del Nuevo Dictamen:</p>
        <p
          v-if="contrato.dictamenes!=null"
        >Este Contrato ya ha sido dictaminado puede consultar en los detalles del mismo para mas información</p>
        <v-row>
          <v-layout row wrap class="px-3">
            <v-flex cols="2" md9 class="px-3">
              <v-textarea v-model="dictamen.observaciones" label="Observaciones" rows="1"></v-textarea>
            </v-flex>
            <v-flex cols="2" md6 class="px-3">
              <v-textarea v-model="dictamen.consideraciones" label="Consideraciones" rows="2"></v-textarea>
            </v-flex>
            <v-flex cols="2" md6 class="px-3">
              <v-textarea v-model="dictamen.recomendaciones" label="Recomendaciones" rows="2"></v-textarea>
            </v-flex>
            <v-flex cols="2" xs12 md6 class="px-3">
              <v-file-input
                v-model="file"
                show-size
                prepend-icon="mdi-note-multiple"
                label="Seleccione el dictamen del contrato"
              ></v-file-input>
            </v-flex>
            <v-flex cols="2" md6 class="px-3">
              <v-text-field v-model="dictamen.fundamentosDeDerecho" label="Fundamentos de Derecho"></v-text-field>
            </v-flex>
            <v-flex cols="2" md6 class="px-1">
              <v-alert v-if="message" border="left" color="red" dark>{{ message }}</v-alert>
            </v-flex>
          </v-layout>
        </v-row>
        <p class="text-left title">Datos a llenar del Contrato:</p>
        <v-row>
          <v-layout row wrap class="px-3">
            <v-flex cols="2" md2 class="px-3" v-if="roles.includes('juridico')">
              <v-text-field v-model="contrato.numero" label="Número" prefix="#"></v-text-field>
            </v-flex>
            <v-flex cols="2" md3 class="px-3">
              <v-autocomplete
                v-model="contrato.estado"
                item-text="nombre"
                item-value="id"
                :items="estados"
                label="Estado del contrato"
              ></v-autocomplete>
            </v-flex>
            <v-flex cols="2" md3 class="px-3" v-if="roles.includes('juridico')">
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
                    class="px-3"
                  ></v-text-field>
                </template>
                <v-date-picker v-model="contrato.fechaDeFirmado" @input="menu = false"></v-date-picker>
              </v-menu>
            </v-flex>
            <v-flex cols="2" md3 class="px-3" v-if="roles.includes('juridico')">
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
                    v-model="contrato.fechaVenContrato"
                    label="Fecha de Vencimiento"
                    readonly
                    clearable
                    v-on="on"
                    required
                    class="px-3"
                  ></v-text-field>
                </template>
                <v-date-picker v-model="contrato.fechaVenContrato" @input="menu1 = false"></v-date-picker>
              </v-menu>
            </v-flex>
          </v-layout>
        </v-row>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" dark text @click="save()">Aceptar</v-btn>
          <v-btn text @click="close()">Cancelar</v-btn>
        </v-card-actions>
      </v-card-text>
    </v-form>
  </v-card>
</template>
<script>
import api from "@/api";

export default {
  props: ["contrato"],
  data: () => ({
    dialog: false,
    menu: false,
    menu1: false,
    dictamen: {
      numero: null
    },
    file: null,
    roles: [],
    username: {},
    errorMessages: [],
    message: "",
    errors: [],
    estados: [],
    messagesNumDictamen: "",
    dictamenes: []
  }),
  computed: {
    form() {}
  },
  created() {
    this.roles = this.$store.getters.roles;
    this.username = this.$store.getters.usuario;
    this.contrato.id = this.contrato.id;
    this.getEstadosParaAprobarFromApi();
    this.getEstadoByRool();
    this.getDictamenesByContratoId();
  },
  watch: {},

  methods: {
    getDictamenesByContratoId() {
      const url = api.getUrl("contratacion", "dictamenes/getByContratoId");
      this.axios.get(`${url}/${this.contrato.id}`).then(
        response => {
          this.dictamenes = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
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
      if (!this.file) {
        this.message = "Por favor seleccione un archivo!";
        return;
      } else if (this.dictamen.numero == null) {
        this.messagesNumDictamen = "El número de dictamen es requerido";
      } else {
        this.message = "";
        const formData = new FormData();
        formData.append("file", this.file);
        const url = api.getUrl(
          "contratacion",
          `Dictamenes/UploadFile?ContratoId=${this.contrato.id}&NumeroDeDictamen=${this.dictamen.numero}
        &Observaciones=${this.dictamen.observaciones}&FundamentosDeDerecho=${this.dictamen.fundamentosDeDerecho}
        &Consideraciones=${this.dictamen.consideraciones}&Recomendaciones=${this.dictamen.recomendaciones}&Username=${this.username}`
        );
        this.axios
          .post(url, formData, {
            headers: {
              "Content-Type": "multipart/form-data"
            }
          })
          .then(
            response => {
              this.editNoAdmin();
            },
            error => {
              vm.$snotify.error(error.response.data);
              console.log(error);
            }
          );
      }
    },
    editNoAdmin() {
      const url = api.getUrl("contratacion", "contratos/editNoAdminDto");
      if (this.contrato.fechaDeFirmado == null) {
        this.contrato.fechaDeFirmado = new Date("01/01/0001");
      }
      this.axios.put(`${url}/${this.contrato.id}`, this.contrato).then(
        response => {
          this.getResponse(response);
          if (this.contrato.cliente == true) {
            this.$router.push({
              name: "OfertasClientes"
            });
          } else
            this.$router.push({
              name: "OfertasPrestador"
            });
        },
        error => {
          console.log(error);
          vm.$snotify.error(e.response.data.errors);
        }
      );
    },
    close() {
      if (this.contrato.cliente == true) {
        this.$router.push({
          name: "OfertasClientes"
        });
      } else
        this.$router.push({
          name: "OfertasPrestador"
        });
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    }
  }
};
</script>
<style></style>
