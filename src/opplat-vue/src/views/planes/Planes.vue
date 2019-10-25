<template>
  <v-container>
    <v-data-table
      :headers="headers"
      :items="planes"
      :search="search"
      sort-by="ano"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-toolbar-title>Listado de Planes</v-toolbar-title>
          <v-divider class="mx-4" inset vertical></v-divider>
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            append-icon="mdi-magnify"
            label="Buscar"
            single-line
            hide-details
          ></v-text-field>
          <v-spacer></v-spacer>
          <v-layout>
            <v-dialog v-model="dialog" persistent max-width="600px">
              <template v-slot:activator="{ on }">
                <v-btn color="primary" dark v-on="on">Agregar Plan</v-btn>
              </template>
              <v-card>
                <v-card-title>
                  <span class="headline">Agregar Plan</span>
                </v-card-title>
                <v-card-text>
                  <v-container grid-list-md>
                    <v-layout wrap>
                      <v-flex xs12 sm4 md2>
                        <v-text-field label="AÑO" required v-model="plan.year"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-file-input show-size label="SELECCIONAR FICHERO" v-model="plan.file"></v-file-input>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="blue darken-1" text @click="dialog = false">Cerrar</v-btn>
                  <v-btn color="green darken-1" text @click="guargarPlan">Guardar</v-btn>
                </v-card-actions>
              </v-card>
            </v-dialog>
          </v-layout>
        </v-toolbar>
      </template>
      <template v-slot:item.action="{ item }">
        <v-icon small class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
        <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="initialize">Reset</v-btn>
      </template>
    </v-data-table>

    <v-dialog
      v-model="dialog2"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
      scrollable
    >
      <v-card tile>
        <v-toolbar flat dark color="primary">
          <v-btn icon dark @click="dialog2 = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>Editar Plan</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-toolbar-items>
            <v-btn dark text @click="dialog2 = false">Guardar</v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <v-card tile>
          <Handsontable :data="data" />
        </v-card>

        <div style="flex: 1 1 auto;"></div>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import api from "@/api";
import Handsontable from "@/components/Handsontable.vue";
export default {
  components: {
    Handsontable
  },
  data: () => ({
    dialog: false,
    dialog2: false,
    search: "",
    plan: {
      year: "",
      file: null
    },
    planes: [],
    errors: [],
    data: [
      ["", "Tesla", "Nissan", "Toyota", "Honda", "Mazda", "Ford"],
      ["2017", 10, 11, 12, 13, 15, 16],
      ["2018", 10, 11, 12, 13, 15, 16],
      ["2019", 10, 11, 12, 13, 15, 16],
      ["2020", 10, 11, 12, 13, 15, 16],
      ["2021", 10, 11, 12, 13, 15, 16]
    ],
    headers: [
      {
        text: "Año",
        align: "left",
        sortable: true,
        value: "year"
      },
      { text: "Nombre", value: "nombre" },
      { text: "Actions", value: "action", sortable: false }
    ],
    editedIndex: -1,
    editedItem: {
      name: "",
      calories: 0,
      fat: 0,
      carbs: 0,
      protein: 0
    },
    defaultItem: {
      name: "",
      calories: 0,
      fat: 0,
      carbs: 0,
      protein: 0
    }
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {
    this.initialize();
  },
  mounthed() {
    this.initPopper();
  },
  methods: {
    initPopper() {
      const chart = document.querySelector(".vtc");
      const ref = chart.querySelector(".active-line");
      const tooltip = this.$refs.tooltip;
      this.popper = new Popper(ref, tooltip, {
        placement: "right",
        modifiers: {
          offset: { offset: "0,10" },
          preventOverflow: {
            boundariesElement: chart
          }
        }
      });
    },
    onMouseMove(params) {
      this.popperIsActive = !!params;
      this.popper.scheduleUpdate();
      this.tooltipData = params || null;
    },
    initialize() {
      const url = api.getUrl("contabilidad", "PlanGI");
      this.axios.get(url).then(
        response => {
          this.planes = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },

    editItem(item) {
      // const url = api.getUrl("contabilidad", "PlanGI");
      // this.axios.get(url).then(
      //   response => {
      //     this.data = response.data;
      //   },
      //   error => {
      //     console.log(error);
      //   }
      // );

      this.dialog2 = true;
    },

    deleteItem(item) {
      const index = this.planes.indexOf(item);
      confirm("¿Está seguro de eliminar este plan?") &&
        this.planes.splice(index, 1);
    },

    close() {
      this.dialog = false;
      setTimeout(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    },

    save() {
      if (this.editedIndex > -1) {
        Object.assign(this.desserts[this.editedIndex], this.editedItem);
      } else {
        this.desserts.push(this.editedItem);
      }
      this.close();
    },
    getResponse: function(response) {
      if (response.status == 200) {
        this.dialog = false;
        this.plan.year = "";
        this.plan.file = null;
        vm.$snotify.success("Exito al realizar la operación");
        this.initialize();
      }
    },
    guargarPlan: function() {
      if (this.plan.file != null) {
        var formData = new FormData();
        formData.append("File", this.plan.file);
        formData.append("year", this.plan.year);
        const url = api.getUrl("contabilidad", "PlanGI/UploadPlanGI");
        this.axios
          .post(url, formData, {
            headers: {
              "Content-Type": "multipart/form-data"
            }
          })
          .then(
            response => {
              this.getResponse(response);
            },
            error => {
              console.log(error);
            }
          );
      }
    }
  }
};
</script>
