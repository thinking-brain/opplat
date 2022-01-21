<template>
  <div>
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
                  v-model="marca"
                  :items="marcas"
                  item-value="id"
                  item-text="nombre"
                  label="Marcas"
                  clearable
                >
                  <v-icon @click="addMarca" slot="append" color="blue darken-2"
                    >mdi-plus</v-icon
                  ></v-autocomplete
                >
                <v-tooltip top color="success">
                  <template v-slot:activator="{ on }">
                    <v-btn
                      small
                      icon
                      color="success"
                      v-on="on"
                      @click="getAllMarcasFromApi(item)"
                      slot="activator"
                    >
                      <v-icon>mdi-reload</v-icon>
                    </v-btn>
                  </template>
                  <span>Volver a cargar Marcas</span>
                </v-tooltip>
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
    <AddEdit
      :dialogEdit="dialog_marca"
      :item="new_marca"
      :editedIndex="editedIndex"
      @close="close"
    />
  </div>
</template>
<script>
import api from "@/api";
import AddEdit from "@/components/taller/marcas/AddEdit.vue";

export default {
  components: { AddEdit },

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
    marca: {},
    new_marca: {},
    dialog_marca: false,
  }),
  computed: {
    formTitle() {
      this.marca = this.item.marca;
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
      this.item.marca =
        typeof this.marca === "object" ? this.marca.id : this.marca;
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
    addMarca() {
      this.dialog_marca = true;
    },
    close() {
      this.dialog_marca = false;
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
