<template>
  <v-card :elevation="4">
    <v-row>
      <v-col cols="6">
        <VueApexCharts type="line" :options="ofertasOptions" :series="ofertas" />
      </v-col>
      <v-col cols="6">
        <v-card flat class="mr-3">
          <table>
            <thead>
              <tr>
                <th colspan="11" class="blue darken-2 white--text">
                  <h2>Circulación de Ofertas del Año {{new Date().getFullYear()}}</h2>
                </th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <th class="text-left">Total de ofertas procesadas hasta la fecha</th>
                <th class="text-center">{{ofertasTabla.ofertasProcesadas}}</th>
              </tr>
              <tr>
                <td class="text-left negrita">Ofertas en Proceso</td>
                <td class="text-center">{{ofertasTabla.ofertasEnProceso}}</td>
              </tr>
              <tr>
                <th class="text-left">Total de ofertas vencidas hasta la fecha</th>
                <th class="text-center">{{ofertasTabla.ofertasVenHastaFecha}}</th>
              </tr>
              <tr>
                <td class="text-left negrita">Ofertas vencidas este Mes</td>
                <td class="text-center">{{ofertasTabla.ofertasVenEsteMes}}</td>
              </tr>
              <tr>
                <th class="text-left">Tiempo promedio de circulación de ofertas hasta la fecha</th>
                <th class="text-center">{{ofertasTabla.promCircuOferta}} Días</th>
              </tr>
              <tr>
                <td class="text-left">Tiempo promedio de circulación de ofertas este mes</td>
                <td class="text-center">{{ofertasTabla.promCircuOfertaMes}} Días</td>
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
      ofertas: [
        {
          name: "Ofertas vencidas",
          data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        },
        {
          name: "Ofertas en proceso",
          data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        }
      ],
      ofertasTabla: [],
      ofertasOptions: {
        title: {
          text: "Circulación de Ofertas",
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
            show: true,
            offsetX:0,
            offsetY:0,
            tools:{
              download:false,
              selection:true,
              zoom:true,
              zoomin:true,
              zoomout:true,
              pan:true,
              reset:true,
              customIcons:[]
            },
            autoSelected:'zoom'
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
            "Enero",
            "Febrero",
            "Marzo",
            "Abril",
            "Mayo",
            "Junio",
            "Julio",
            "Agosto",
            "Septiembre",
            "Octubre",
            "Noviembre",
            "Diciembre"
          ]
        },
        yaxis: {
          // valor minimo para emezar en eje y
          min: 0,
          // valores redondeados
          forceNiceScale: true
        }
      },
      ofertas: [],
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
          this.ofertas = [
            {
              name: "Ofertas Vencidas",
              data: response.data.ofertasVencidas
            },
            {
              name: "Ofertas en Proceso",
              data: response.data.ofertasProceso
            }
          ];
          this.ofertasTabla = response.data;
        },
        error => {
          console.log(error);
        }
      );
    }
  }
};
</script>
