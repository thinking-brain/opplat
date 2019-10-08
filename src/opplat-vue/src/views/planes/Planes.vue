<template>
  <v-container>
    <v-data-table
      :headers="headers"
      :items="desserts"
      :search="search"
      sort-by="ano"
      multi-sort
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
                        <v-text-field label="AÑO" required v-model="plan.ano"></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md4>
                        <v-select
                          v-model="plan.mes"
                          item-text="nombre"
                          return-object
                          :items="meses"
                          :rules="[v => !!v || 'requerido']"
                          label="MES"
                          required
                        ></v-select>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-file-input show-size label="SELECCIONAR FICHERO" v-model="planFile"></v-file-input>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="blue darken-1" text @click="dialog = false">Cerrar</v-btn>
                  <v-btn color="green darken-1" text @click="crearArea">Guardar</v-btn>
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
  </v-container>
</template>

<script>
export default {
  data: () => ({
    dialog: false,
    search: "",
    areas: null,
    centros_costo: [],
    plan: {
      mes: "",
      ano: ""
    },
    planFile: null,
    meses: [
      { id: 1, nombre: "ENERO" },
      { id: 2, nombre: "FEBRERO" },
      { id: 3, nombre: "MARZO" },
      { id: 4, nombre: "ABRIL" },
      { id: 5, nombre: "MAYO" },
      { id: 6, nombre: "JUNIO" },
      { id: 7, nombre: "JULIO" },
      { id: 8, nombre: "AGOSTO" },
      { id: 9, nombre: "SEPTIEMBRE" },
      { id: 10, nombre: "OCTUBRE" },
      { id: 11, nombre: "NOVIEMBRE" },
      { id: 12, nombre: "DICIEMBRE" }
    ],
    errors: [],
    headers: [
      {
        text: "Año",
        align: "left",
        sortable: true,
        value: "ano"
      },
      { text: "Mes", value: "mes" },
      { text: "Tipo", value: "tipo" },
      { text: "Actions", value: "action", sortable: false }
    ],
    desserts: [],
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

  methods: {
    initialize() {
      this.desserts = [
        {
          ano: "2017",
          mes: "abril",
          tipo: "Plan Ingresos"
        },
        {
          ano: "2019",
          mes: "junio",
          tipo: "Plan de Gastos"
        },
        {
          ano: "2019",
          mes: "julio",
          tipo: "Plan Ingresos"
        }
      ];
    },

    editItem(item) {
      this.editedIndex = this.desserts.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      const index = this.desserts.indexOf(item);
      confirm("¿Está seguro de eliminar este plan?") &&
        this.desserts.splice(index, 1);
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
        this.$emit("actualizarListado");
        this.dialog = false;
        this.area.nombre = "";
      }
    },
    crearArea: function() {
      this.axios
        .post("https://localhost:5001/api/areas", {
          nombre: this.area.nombre
        })
        .then(response => {
          this.getResponse(response);
        })
        .catch(response => {
          console.log("err");
          console.log(response.json());
        });
    }
  }
};
</script>
