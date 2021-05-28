<template>
  <v-dialog v-model="dialogEdit" persistent scrollable max-width="900px">
    <v-card>
      <v-toolbar elevation="2">
        <span class="headline">{{ formTitle }}</span>
      </v-toolbar>
      <v-card-text style="height: 1200px" class="mt-3">
        <v-row dense>
          <v-col cols="12" sm="4" md="4">
            <v-menu
              v-model="menu"
              :close-on-content-click="false"
              :nudge-right="40"
              transition="scale-transition"
              offset-y
              min-width="290px"
            >
              <template v-slot:activator="{ on, attrs }">
                <v-text-field
                  v-model="item.fechaIngreso"
                  label="Fecha de Ingreso al Taller"
                  prepend-icon="mdi-calendar"
                  readonly
                  v-on="on"
                ></v-text-field>
              </template>
              <v-date-picker
                v-model="item.fechaIngreso"
                locale="esp-es"
                @input="menu = false"
              ></v-date-picker>
            </v-menu>
          </v-col>
          <v-col cols="12" sm="4" md="4"> </v-col>
          <v-col cols="12" sm="4" md="4">
            <v-autocomplete
              v-model="item.tecnicoRxEquipo"
              label="Técnico que recibe el equipo"
              :items="tecnicos"
              item-value="id"
              item-text="nombre"
              clearable
            ></v-autocomplete>
          </v-col>
        </v-row>
        <v-card @click="dialogEquipos = true" tile ripple>
          <v-col cols="5">
            <v-text-field label="Equipo" v-model="equipoDetail.marca.nombre">
            </v-text-field>
          </v-col>
          <v-row dense>
            <v-col cols="7">
                <v-row>
                  <v-col cols="6">
                    <h3 class="ml-2">Datos del Equipo</h3>
                    <v-layout class="pa-2">
                      Número de Serie: {{ equipoDetail.numeroSerie }}
                    </v-layout>
                    <v-layout class="pa-2">
                      Marca: {{ equipoDetail.marca.nombre }}
                    </v-layout>
                    <v-layout class="pa-2">
                      Modelo: {{ equipoDetail.modelo.nombre }}
                    </v-layout>
                    <v-layout class="pa-2">
                      Tipo de Equipo: {{ equipoDetail.tipoEquipo.nombre }}
                    </v-layout>
                  </v-col>
                  <v-col cols="6" class="mt-6">
                    <v-layout class="pa-2">
                      Fecha de Fabricación: {{ equipoDetail.fechaFabricacion }}
                    </v-layout>
                    <v-layout class="pa-2">
                      Observaciones: {{ equipoDetail.observaciones }}
                    </v-layout>
                    <v-layout class="pa-2">
                      Estado del Equipo: {{ equipoDetail.estadoEquipo }}
                    </v-layout>
                  </v-col>
                </v-row>
            </v-col>
            <v-col cols="5">
                <h3 class="ml-2">Datos del Cliente</h3>
                <v-layout class="pa-2">
                  Nombre: {{ equipoDetail.cliente.nombre }}
                </v-layout>
                <v-layout class="pa-2">
                  Carnet de Identidad: {{ equipoDetail.cliente.ci }}
                </v-layout>
                <v-layout class="pa-2">
                  Teléfono: {{ equipoDetail.cliente.telefono }}
                </v-layout>
                <v-layout class="pa-2">
                  Correo: {{ equipoDetail.cliente.correo }}
                </v-layout>
                <v-layout class="pa-2">
                  Dirrección: {{ equipoDetail.cliente.direccion }}
                </v-layout>
            </v-col>
          </v-row>
        </v-card>
        <v-row>
          <v-col cols="12">
            <v-text-field
              v-model="item.defecto"
              label="Falla del Equipo"
              placeholder="Describa la falla o el motivo por el cual el equipo ingresa al taller"
              clearable
            ></v-text-field>
          </v-col>
          <v-col cols="12" sm="4" md="4">
            <v-menu
              v-model="menu1"
              :close-on-content-click="false"
              :nudge-right="40"
              transition="scale-transition"
              offset-y
              min-width="290px"
            >
              <template v-slot:activator="{ on, attrs }">
                <v-text-field
                  v-model="item.fechaPrometido"
                  label="Fecha de Prometido"
                  prepend-icon="mdi-calendar"
                  readonly
                  v-on="on"
                ></v-text-field>
              </template>
              <v-date-picker
                v-model="item.fechaPrometido"
                locale="esp-es"
                @input="menu1 = false"
              ></v-date-picker>
            </v-menu>
          </v-col>
          <v-col cols="12" sm="4" md="4">
            <v-autocomplete
              v-model="item.tecnico"
              label="Asignar Orden"
              :items="tecnicos"
              item-value="id"
              item-text="nombre"
              clearable
            ></v-autocomplete>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="$emit('close')"> Cancelar </v-btn>
        <v-btn color="blue darken-1" text @click="save(method)">
          Aceptar
        </v-btn>
      </v-card-actions>
    </v-card>
    <v-dialog v-model="dialogEquipos" max-width="900px">
      <v-card> <Equipos :isDialog="isDialog" @close="close" /> </v-card>
    </v-dialog>
  </v-dialog>
</template>
<script>
import api from "@/api";
import Equipos from "@/views/taller/Equipos.vue";
export default {
  components: { Equipos },
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
    errors: [],
    tecnicos: [],
    clientes: [],
    equipos: [],
    equipo: null,
    cliente: 0,
    equipoDetail: {
      numeroSerie: null,
      fechaFabricacion: "",
      tipoEquipo: {
        nombre: "",
        codigo: "",
      },
      cliente: {
        id: null,
        nombre: "",
        telefono: "",
        correo: "",
        direccion: "",
        ci: "",
      },
      observaciones: "",
      marca: {
        nombre: "",
      },
      modelo: {
        nombre: "",
      },
      estadoEquipo: 0,
    },
    menu: false,
    menu1: false,
    dialogEquipos: false,
    isDialog: true,
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nueva Orden de Reparación"
        : "Editar Orden de Reparación";
    },
    method() {
      return this.editedIndex === -1 ? "POST" : "PUT";
    },
  },
  created() {
    var url = api.getUrl("taller", "Tecnicos/getAll");
    this.axios.get(url).then((response) => {
      this.tecnicos = response.data;
    });
    url = api.getUrl("taller", "Equipos/getAll");
    this.axios.get(url).then((response) => {
      this.equipos = response.data;
    });
    url = api.getUrl("taller", "Clientes/getAll");
    this.axios.get(url).then((response) => {
      this.clientes = response.data;
    });
  },
  watch: {
    cliente() {
      this.getClienteById();
    },
  },
  methods: {
    save(method) {
      const url = api.getUrl("taller", "OrdenesReparacion");
      if (method === "POST") {
        this.item.activo = true;
        this.axios.post(url, this.item).then(
          (response) => {
            this.getResponse(response);
            this.$emit("getOrdenesReparacionFromApi");
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
            this.$emit("getOrdenesReparacionFromApi");
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
    close(data) {
      this.dialogEquipos = false;
      this.equipoDetail = data[0];
    },
  },
};
</script>
<style></style>
