<template>
  <v-card ref="form">
    <v-form>
      <v-card-text>
        <p class="text-left title">{{ title }}</p>
        <!-- <p
          v-if="contrato.dictamenes.length!=0"
        >Este Contrato ya ha sido dictaminado puede consultar en los detalles del mismo para mas información</p>-->
        <v-row>
          <v-layout row wrap class="px-3">
            <v-flex cols="2" md9 class="px-3">
              <v-textarea
                v-model="dictamen.observaciones"
                label="Observaciones"
                rows="1"
              ></v-textarea>
            </v-flex>
            <v-flex cols="2" md6 class="px-3">
              <v-textarea
                v-model="dictamen.consideraciones"
                label="Consideraciones"
                rows="2"
              ></v-textarea>
            </v-flex>
            <v-flex cols="2" md6 class="px-3">
              <v-textarea
                v-model="dictamen.recomendaciones"
                label="Recomendaciones"
                rows="2"
              ></v-textarea>
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
              <v-text-field
                v-model="dictamen.fundamentosDeDerecho"
                label="Fundamentos de Derecho"
              ></v-text-field>
            </v-flex>
            <v-flex cols="2" md6 class="px-1">
              <v-alert v-if="message" border="left" color="red" dark>{{
                message
              }}</v-alert>
            </v-flex>
          </v-layout>
        </v-row>
        <p class="text-left title">Datos a llenar del Contrato:</p>
        <v-row>
          <v-layout row wrap class="px-3">
            <v-flex cols="2" md3 class="px-3">
              <v-autocomplete
                v-model="contrato.estado"
                item-text="nombre"
                item-value="id"
                :items="estados"
                label="EstadoOrden del contrato"
              ></v-autocomplete>
            </v-flex>
          </v-layout>
        </v-row>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" dark text @click="save()"
            >Aceptar</v-btn
          >
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
    dictamen: {},
    file: null,
    roles: [],
    username: {},
    errorMessages: [],
    message: "",
    errors: [],
    estados: [],
    messagesNumDictamen: "",
    dictamenes: [],
    editarDictamen: false,
    title: "Datos a llenar del Nuevo Dictamen:",
  }),
  computed: {
    form() {},
  },
  created() {
    this.roles = this.$store.getters.roles;
    this.username = this.$store.getters.usuario;
    this.getEstadosParaAprobarFromApi();

    if (this.contrato.dictamenEdit == true) {
      this.dictamen = this.contrato.dictamen;
      this.file = this.dictamen.filePath;
      this.title = "Editar el dictamen del contrato: " + this.contrato.nombre;
      this.editarDictamen = true;
    }
  },
  watch: {},

  methods: {
    getEstadosParaAprobarFromApi() {
      const url = api.getUrl("contratacion", "contratos/EstadosParaAprobar");
      this.axios.get(url).then(
        (response) => {
          this.estados = response.data;
        },
        (error) => {
          console.log(error);
        }
      );
    },
    save() {
      if (this.file == null) {
        this.message = "Por favor seleccione un archivo!";
      } else {
        this.message = "";
        const formData = new FormData();
        formData.append("file", this.file);
        if (this.contrato.dictamenEdit == true) {
          const url = api.getUrl("contratacion", "Dictamenes");
          this.axios.put(`${url}/${this.dictamen.id}`, this.dictamen).then(
            (response) => {
              this.Dictaminar();
              this.uploadDictamen();
              this.close();
            },
            (error) => {
              vm.$snotify.error(error.response.data);
              console.log(error);
            }
          );
        } else {
          const url = api.getUrl(
            "contratacion",
            `Dictamenes/UploadFile?ContratoId=${this.contrato.id}&NumeroDeDictamen=${this.dictamen.numero}
        &Observaciones=${this.dictamen.observaciones}&FundamentosDeDerecho=${this.dictamen.fundamentosDeDerecho}
        &Consideraciones=${this.dictamen.consideraciones}&Recomendaciones=${this.dictamen.recomendaciones}&Username=${this.username}&id=${this.dictamen.id}`
          );
          this.axios
            .post(url, formData, {
              headers: {
                "Content-Type": "multipart/form-data",
              },
            })
            .then(
              (response) => {
                this.Dictaminar();
                this.close();
              },
              (error) => {
                vm.$snotify.error(error.response.data);
                console.log(error);
              }
            );
        }
      }
    },
    Dictaminar() {
      const url = api.getUrl("contratacion", "contratos/editNoAdminDto");
      if (
        this.contrato.fechaDeFirmado == null &&
        this.contrato.dictamenEdit == false
      ) {
        this.contrato.fechaDeFirmado = new Date("01/01/0001");
      }
      this.contrato.roles = this.roles;
      this.axios.put(`${url}/${this.contrato.id}`, this.contrato).then(
        (response) => {
        },
        (error) => {
          console.log(error);
          vm.$snotify.error(e.response.data.errors);
        }
      );
    },
    close() {
      if (this.contrato.sinFechaVen == true) {
        this.$router.push({
          name: "ContratosSinFecha",
        });
      } else {
        if (this.contrato.cliente == true && this.contrato.esContrato == true) {
          this.$router.push({
            name: "ContratosCliente",
          });
        } else if (
          this.contrato.cliente == false &&
          this.contrato.esContrato == true
        ) {
          this.$router.push({
            name: "ContratosPrestador",
          });
        } else if (this.contrato.cliente == true) {
          this.$router.push({
            name: "OfertasClientes",
          });
        } else
          this.$router.push({
            name: "OfertasPrestador",
          });
      }
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    uploadDictamen() {
      if (!this.file) {
        this.message = "Por favor seleccione un archivo!";
        return;
      }
      this.message = "";
      const formData = new FormData();
      formData.append("file", this.file);
      const url = api.getUrl(
        "contratacion",
        `Dictamenes/UploadDictamen?ContratoId=${this.contrato.id}&id=${this.dictamen.id}`
      );
      this.axios
        .put(`${url}`, formData, {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        })
        .then(
          (response) => {
            this.getResponse(response);          
          },
          (error) => {
            vm.$snotify.error(error.response.data);
            console.log(error);
          }
        );
    },
  },
};
</script>

<style></style>
