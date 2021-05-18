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
              <v-autocomplete
                v-model="item.marca"
                :items="marcas"
                item-value="id"
                item-text="nombre"
                label="Marcas"
                clearable
              ></v-autocomplete>
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
  mounted() {
    this.getAllMarcasFromApi();
  },
  watch: {},
  methods: {
    getAllMarcasFromApi() {
      const url = api.getUrl("taller", "Marcas/getAll");
      this.axios.get(url).then((response) => {
        this.marcas = response.data;
      });
    },
    save(method) {
      const url = api.getUrl("taller", "Modelos");
      if (method === "POST") {
        this.item.activo = true;
        this.axios.post(url, this.item).then(
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
