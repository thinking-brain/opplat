<template>
  <v-dialog v-model="dialogEdit" max-width="350px">
    <v-card>
      <v-card-title>
        <span class="headline">{{ formTitle }}</span>
      </v-card-title>
      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="12">
              <v-text-field
                v-model="item.nombre"
                label="Nombre"
                clearable
              ></v-text-field>
            </v-col>
            <v-col cols="12">
              <v-text-field
                v-model="item.cargo"
                label="Cargo"
                clearable
              ></v-text-field>
            </v-col>
            <v-col cols="12">
              <v-autocomplete
                v-model="taller"
                :items="talleres"
                item-value="id"
                item-text="nombre"
                label="Taller"
                clearable
              >
                <v-icon
                  @click="dialog4 = true"
                  slot="append"
                  color="blue darken-2"
                  >mdi-plus</v-icon
                >
              </v-autocomplete>
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
    talleres: [],
    errors: [],
    taller: { nombre: "" },
  }),
  computed: {
    formTitle() {
      this.taller = this.item.taller;
      return this.editedIndex === -1 ? "Nuevo" : "Editar";
    },
    method() {
      return this.editedIndex === -1 ? "POST" : "PUT";
    },
  },
  mounted() {
    this.getAllTalleresFromApi();
  },
  watch: {},
  methods: {
    getAllTalleresFromApi() {
      const url = api.getUrl("taller", "Talleres");
      this.axios.get(url).then((response) => {
        this.talleres = response.data;
      });
    },
    save(method) {
      this.item.taller =
        typeof this.taller === "object" ? this.taller.id : this.taller;
      const url = api.getUrl("taller", "Tecnicos");
      if (method === "POST") {
        this.item.activo = true;
        this.axios.post(url, this.item).then(
          (response) => {
            this.getResponse(response);
            this.$emit("getTecnicosFromApi");
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
            this.$emit("getModelosFromApi");
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
