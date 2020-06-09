<template>
  <v-card :elevation="4">
    <v-row>
      <v-col cols="6">
        <VueApexCharts type="bar" :options="contratosOptions" :series="contratosProximosVencer" />
      </v-col>
        <v-col cols="6">
        <v-card flat class="mr-3 mt-12">
          <table>
            <thead>
              <tr>
                <th colspan="11" class="blue darken-2 white--text">
                  <h2>Contratos Próximos a Vencer en el Año {{new Date().getFullYear()}}</h2>
                </th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <th class="text-left">Este Mes</th>
                <th class="text-center">{{contratosProximosVencer[0].data[0]}}</th>
              </tr>
              <tr>
                <td class="text-left negrita">En 3 Meses</td>
                <td class="text-center">{{contratosProximosVencer[0].data[1]}}</td>
              </tr>
              <tr>
                <th class="text-left">En 6 Meses</th>
                <th class="text-center">{{contratosProximosVencer[0].data[2]}}</th>
              </tr>
              <tr>
                <td class="text-left negrita">Este Año</td>
                <td class="text-center">{{contratosProximosVencer[0].data[3]}}</td>
              </tr>
              <tr>
                <th class="text-left negrita">Próximo Año</th>
                <th class="text-center">{{contratosProximosVencer[0].data[4]}}</th>
              </tr>
            </tbody>
          </table>
        </v-card>
      </v-col>
    </v-row>
  </v-card>
</template>
<style scoped>
table {
  font-family: Arial, Helvetica, sans-serif;
  font-size: 13px !important;
  margin-right: 300px !important;
}
th {
  background-color: #bbdefb;
}
td {
  background-color: rgb(240, 244, 247);
}
.tr-th:hover {
  background-color: white !important;
}
.encabezado-tabla {
  padding-bottom: 20px;
}
table,
th,
td {
  border: 0.5px solid black;
  border-collapse: collapse;
  padding: 5px;
  text-align: center;
}
table {
  /* table-layout: fixed; */
  max-width: 100%;
  min-width: 100%;
  font-family: Arial, Helvetica, sans-serif;
  font-size: 20px;
}
</style>
<script>
import VueApexCharts from "vue-apexcharts";
import api from "@/api";

export default {
  components: {
    VueApexCharts
  },
  data() {
    return {
      contratosProximosVencer: [
        {
          name: "Contratos Próximos a Vencer",
          data: [0, 0, 0, 0,0]
        },
      ],
      contratosOptions: {
        title: {
          text: "Contratos Próximos a Vencer",
          align: "center",
          margin: 10,
          offsetX: 0,
          offsetY: 0,
          floating: false,
          style: {
            fontSize: "20px",
            color: "rgba(96, 89, 89, 0.87)"
          }
        },
        chart: {
          id: "vuechart",
          // mostrar el boton de exportar
          toolbar: {
            show: false
          },
          // definir color de fondo general
          background: ""
        },
        xaxis: {
          // mostrar tooltip para eje x
          tooltip: {
            enabled: false
          },
          // labels para mostrar en eje x
          categories: [
            "Este Mes",
            "En 3 Meses",
            "En 6 Meses",
            "Este Año",
            "Próximo Año",
          ]
        },
        yaxis: {
          // valor minimo para emezar en eje y
          min: 0,
          // valores redondeados
          forceNiceScale: true
        }
      },
      errors: []
    };
  },
  created() {
    const d = new Date();
    const year = d.getFullYear();
        this.getDashboardFromApi();
  },
 methods: {
    getDashboardFromApi() {
      const url = api.getUrl("contratacion", "contratos/Dashboard");
      this.axios.get(url).then(
        response => {
          this.contratosProximosVencer = [
            {
              name: "Contratos Próximos a Vencer",
              data: response.data.contratosProximosVencer
            },
          ];
        },
        error => {
          console.log(error);
        }
      );
    }
  }};
</script>
