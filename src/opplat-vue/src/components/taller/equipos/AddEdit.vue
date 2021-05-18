<template>
  <v-dialog v-model="dialogEdit" max-width="650px">
    <v-card>
      <v-card-title>
        <span class="headline">{{ formTitle }}</span>
      </v-card-title>
      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="12" sm="6" md="6">
              <v-text-field
                v-model="item.numeroSerie"
                label="Número de Serie"
                clearable
              ></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" md="6">
              <v-autocomplete
                auto-select-first
                v-model="item.marca"
                :items="marcas"
                item-value="id"
                item-text="nombre"
                label="Marca"
                clearable
              ></v-autocomplete>
            </v-col>
            <v-col cols="12" sm="6" md="6">
              <v-autocomplete
                auto-select-first
                v-model="item.modelo"
                :items="modelos"
                item-value="id"
                item-text="nombre"
                label="Modelo"
                clearable
              ></v-autocomplete>
            </v-col>
            <v-col cols="12" sm="6" md="6">
              <v-autocomplete
                auto-select-first
                v-model="item.tipoEquipo"
                :items="tiposEquipos"
                item-value="id"
                item-text="nombre"
                label="Tipo de Equipo"
                clearable
              ></v-autocomplete>
            </v-col>
            <v-col cols="12" sm="6" md="6">
              <v-autocomplete
                auto-select-first
                v-model="item.cliente"
                :items="clientes"
                item-value="id"
                item-text="nombre"
                label="Dueño del Equipo"
                clearable
              ></v-autocomplete>
            </v-col>
            <v-col cols="12" sm="6" md="6">
              <v-autocomplete
                auto-select-first
                v-model="item.situacionEquipo"
                :items="situacionEquipo"
                item-value="id"
                item-text="nombre"
                label="Situación del Equipo"
                clearable
              ></v-autocomplete>
            </v-col>
            <v-col cols="12" sm="6" md="6">
              <v-menu
                v-model="menu"
                :close-on-content-click="false"
                :nudge-right="40"
                transition="scale-transition"
                offset-y
                min-width="auto"
              >
                <template v-slot:activator="{ on, attrs }">
                  <v-text-field
                    v-model="item.fechaFabricacion"
                    label="Fecha de Fabricacion"
                    prepend-icon="mdi-calendar"
                    readonly
                    v-bind="attrs"
                    v-on="on"
                  ></v-text-field>
                </template>
                <v-date-picker
                  v-model="item.fechaFabricacion"
                  @input="menu = false"
                ></v-date-picker>
              </v-menu>
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model="item.obsevaciones"
                label="Obsevaciones"
              ></v-text-field>
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="$emit('close')"> Cancelar </v-btn>
        <v-btn color="blue darken-1" text @click="save(method)">
          Aceptar
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import api from "@/api";

export default {
  props: {
    dialogEdit: {
      type: Boolean,
      default: false,
    },
    item: {
      type: Object,
      default: {},
    },
    editedIndex: {
      type: Number,
      default: -1,
    },
  },

  data: () => ({
    marcas: [],
    modelos: [],
    tiposEquipos: [],
    clientes: [],
    situacionEquipo: [],
    menu: false,
    errors: [],
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nuevo" : "Editar";
    },
    method() {
      return this.editedIndex === -1 ? "POST" : "PUT";
    },
  },
  created() {
    this.getAllMarcasFromApi();
    this.getAllModelosFromApi();
    this.getAllTiposEquiposFromApi();
    this.getAllTiposClientesFromApi();
    this.getAllSituacionEquipoFromApi();
  },

  watch: {},
  methods: {
    getAllMarcasFromApi() {
      const url = api.getUrl("taller", "Marcas/getAll");
      this.axios.get(url).then((response) => {
        this.marcas = response.data;
      });
    },
    getAllModelosFromApi() {
      const url = api.getUrl("taller", "Modelos/getAll");
      this.axios.get(url).then((response) => {
        this.modelos = response.data;
      });
    },
    getAllTiposEquiposFromApi() {
      const url = api.getUrl("taller", "TiposEquipos/getAll");
      this.axios.get(url).then((response) => {
        this.tiposEquipos = response.data;
      });
    },
    getAllTiposClientesFromApi() {
      const url = api.getUrl("taller", "Clientes/getAll");
      this.axios.get(url).then((response) => {
        this.clientes = response.data;
      });
    },
    getAllSituacionEquipoFromApi() {
      const url = api.getUrl("taller", "Equipos/SituacionEquipo");
      this.axios.get(url).then((response) => {
        this.situacionEquipo = response.data;
      });
    },
    save(method) {
      const url = api.getUrl("taller", "Equipos");
      if (method === "POST") {
        this.item.activo = true;
        this.axios.post(url, this.item).then(
          (response) => {
            this.getResponse(response);
            this.$emit("getEquiposFromApi");
            this.$emit("close");
          },
          (error) => {
            console.log(error);
          }
        );
      }
      if (method === "PUT") {
        this.axios.put(`${url}/${this.item.id}`, this.item).then(
          (response) => {
            this.getResponse(response);
            this.$emit("getEquiposFromApi");
            this.$emit("close");
          },
          (error) => {
            console.log(error);
          }
        );
      }
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
  },
};
</script>
<style></style>
