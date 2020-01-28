<template>
  <v-container>
    <v-form v-model="valid" class="d-print-none">
      <v-row>
        <v-col cols="12" md="4">
          <v-select
            v-model="selectedMonths"
            item-text="nombre"
            return-object
            :items="meses"
            :rules="[v => !!v || 'Este campo es requerido']"
            label="MES"
            required
          ></v-select>
          <!-- <v-select v-model="selectedMonths" :items="meses" item-text="nombre" label="MES" return-object multiple>
            <template v-slot:prepend-item>
              <v-list-item ripple @click="toggle">
                <v-list-item-action>
                  <v-icon :color="selectedMonths.length > 0 ? 'indigo darken-4' : ''">{{ icon }}</v-icon>
                </v-list-item-action>
                <v-list-item-content>
                  <v-list-item-title>Todos</v-list-item-title>
                </v-list-item-content>
              </v-list-item>
              <v-divider class="mt-2"></v-divider>
            </template>
            <template v-slot:append-item>
              <v-divider class="mb-2"></v-divider>
            </template>
          </v-select>-->
        </v-col>

        <v-col cols="12" md="4">
          <v-text-field v-model="year_form" :counter="4" :rules="nameRules" label="AÑO" required></v-text-field>
        </v-col>

        <v-col cols="12" md="4">
          <v-select :items="tipos_plan" label="TIPO"></v-select>
        </v-col>

        <v-col cols="12" md="4">
          <v-btn color="success" class="mr-4" @click="GenerarReporte">Generar Reporte</v-btn>
        </v-col>
      </v-row>
    </v-form>
    <v-card v-if="visible" color="basil">
      <v-card-title class="text-center justify-center py-6">
        <h6 class="font-weight-bold display-1 basil--text">Estado Financiero {{mes.nombre}} {{year}}</h6>
      </v-card-title>
      <v-data-table :headers="header" :items="desserts" fixed-header height="400" hide-default-footer="true" disable-sort="true" item-key="name">
        <template slot="items" slot-scope="props">
          <tr>
            <td fixed fixed-tabs class="fixed position-fixed">{{ props.item.name }}</td>
            <td>{{ props.item.calories }}</td>
            <td>{{ props.item.fat }}</td>
            <td>{{ props.item.carbs }}</td>
            <td>{{ props.item.protein }}</td>
            <td>{{ props.item.nprotein }}</td>
            <td>{{ props.item.iron }}</td>
            <td>{{ props.item.niron }}</td>
          </tr>
        </template>
      </v-data-table>
    </v-card>
  </v-container>
</template>

<script>
import api from "@/api";
import EF_5920 from "@/components/finanzas/reportes/EF_5920";
export default {
  components: {
    EF_5920
  },
  data() {
    return {
      data: null,
      header: [
        {text: "Dessert (100g serving)", value: "name", width: "200px", fixed: true},
        {text: "Calories", value: "calories", width: "200px", sortable: false},
        {text: "Fat (g)", value: "fat", width: "300px", sortable: false},
        {text: "Carbs (g)", value: "carbs", width: "300px", sortable: false},
        {text: "Protein (g)", value: "protein", width: "300px", sortable: false},
        {text: "New Protein (g)", value: "nprotein", width: "300px", sortable: false},
        {text: "Iron (%)", value: "iron", width: "300px", sortable: false},
        {text: "New Iron (%)", value: "niron", width: "300px", sortable: false}
      ],
      desserts: [
        {
          value: false,
          name: "Orange Juice",
          category: "Beverage",
          calories: 262,
          fat: 16.0,
          carbs: 23,
          protein: 6.0,
          nprotein: 6.0,
          iron: "7%",
          niron: "7%"
        },
        {
          value: false,
          name: "Larabar",
          category: "Snack",
          calories: 408,
          fat: 3.2,
          carbs: 87,
          protein: 6.5,
          nprotein: 6.5,
          iron: "45%",
          niron: "45%"
        },
        {
          value: false,
          name: "Orange Juice",
          category: "Beverage",
          calories: 262,
          fat: 16.0,
          carbs: 23,
          protein: 6.0,
          nprotein: 6.0,
          iron: "7%",
          niron: "7%"
        },
        {
          value: false,
          name: "Larabar",
          category: "Snack",
          calories: 408,
          fat: 3.2,
          carbs: 87,
          protein: 6.5,
          nprotein: 6.5,
          iron: "45%",
          niron: "45%"
        },{
          value: false,
          name: "Orange Juice",
          category: "Beverage",
          calories: 262,
          fat: 16.0,
          carbs: 23,
          protein: 6.0,
          nprotein: 6.0,
          iron: "7%",
          niron: "7%"
        },
        {
          value: false,
          name: "Larabar",
          category: "Snack",
          calories: 408,
          fat: 3.2,
          carbs: 87,
          protein: 6.5,
          nprotein: 6.5,
          iron: "45%",
          niron: "45%"
        },{
          value: false,
          name: "Orange Juice",
          category: "Beverage",
          calories: 262,
          fat: 16.0,
          carbs: 23,
          protein: 6.0,
          nprotein: 6.0,
          iron: "7%",
          niron: "7%"
        },
        {
          value: false,
          name: "Larabar",
          category: "Snack",
          calories: 408,
          fat: 3.2,
          carbs: 87,
          protein: 6.5,
          nprotein: 6.5,
          iron: "45%",
          niron: "45%"
        },{
          value: false,
          name: "Orange Juice",
          category: "Beverage",
          calories: 262,
          fat: 16.0,
          carbs: 23,
          protein: 6.0,
          nprotein: 6.0,
          iron: "7%",
          niron: "7%"
        },
        {
          value: false,
          name: "Larabar",
          category: "Snack",
          calories: 408,
          fat: 3.2,
          carbs: 87,
          protein: 6.5,
          nprotein: 6.5,
          iron: "45%",
          niron: "45%"
        },{
          value: false,
          name: "Orange Juice",
          category: "Beverage",
          calories: 262,
          fat: 16.0,
          carbs: 23,
          protein: 6.0,
          nprotein: 6.0,
          iron: "7%",
          niron: "7%"
        },
        {
          value: false,
          name: "Larabar",
          category: "Snack",
          calories: 408,
          fat: 3.2,
          carbs: 87,
          protein: 6.5,
          nprotein: 6.5,
          iron: "45%",
          niron: "45%"
        },{
          value: false,
          name: "Orange Juice",
          category: "Beverage",
          calories: 262,
          fat: 16.0,
          carbs: 23,
          protein: 6.0,
          nprotein: 6.0,
          iron: "7%",
          niron: "7%"
        },
        {
          value: false,
          name: "Larabar",
          category: "Snack",
          calories: 408,
          fat: 3.2,
          carbs: 87,
          protein: 6.5,
          nprotein: 6.5,
          iron: "45%",
          niron: "45%"
        },
        {
          value: false,
          name: "Donut",
          category: "Breakfast",
          calories: 452,
          fat: 25.0,
          carbs: 51,
          protein: 4.9,
          nprotein: 4.9,
          iron: "22%",
          niron: "22%"
        },

        {
          value: false,
          name: "Bagel",
          category: "Breakfast",
          calories: 999,
          fat: 28.0,
          carbs: 151,
          protein: 2.9,
          nprotein: 2.9,
          iron: "29%",
          niron: "29%"
        },
        {
          value: false,
          name: "KitKat",
          category: "Snack",
          calories: 518,
          fat: 26.0,
          carbs: 65,
          protein: 7,
          nprotein: 7,
          iron: "6%",
          niron: "6%"
        }
      ],
      valid: true,
      tab: null,
      items: ["5920", "5921", "5922", "5923", "5924", "5925", "5926"],
      selectedMonths: "",
      year_form: "",
      mes: "",
      year: "",
      tipos_plan: ["5920", "5921", "5924"],
      nameRules: [
        v => !!v || "Este campo es requerido",
        v => (v && v.length == 4) || "El año debe tener 4 caracteres."
      ],
      estado: [],
      errors: [],
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
      visible: false,
      lazy: false
    };
  },
  computed: {
    likesAllMonths() {
      return this.selectedMonths.length === this.meses.length;
    },
    likesSomeMonth() {
      return this.selectedMonths.length > 0 && !this.likesAllMonths;
    },
    icon() {
      if (this.likesAllMonths) return "mdi-close-box";
      if (this.likesSomeMonth) return "mdi-minus-box";
      return "mdi-checkbox-blank-outline";
    }
  },
  methods: {
    GenerarReporte() {
      this.year = this.year_form;
      this.mes = this.selectedMonths;
      this.visible = true;
    },
    toggle() {
      this.$nextTick(() => {
        if (this.likesAllMonths) {
          this.selectedMonths = [];
        } else {
          this.selectedMonths = this.meses.slice();
        }
      });
    }
  }
};
</script>
<style scoped>
/* Helper classes */
.basil {
  background-color: #fffbe6 !important;
}
.basil--text {
  color: #356859 !important;
  font-weight: bold;
  font-size: large;
}
</style>
