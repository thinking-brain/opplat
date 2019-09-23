<template>
  <div>
    <v-btn color="blue darken-1" text @click="exportar">Exportar</v-btn>
    <v-btn color="blue darken-1" text @click="imprimir">Imprimir</v-btn>
    <div id="areatoPrint">
      <table id="table1">
        <thead>
          <tr>
            <th colspan="11" class="text-center encabezado1">
              <h2>CNA La Concordia</h2>
            </th>
          </tr>
          <tr>
            <th colspan="11" class="text-center encabezado">
              <h2>Estado del cumplimiento del plan de ingresos y gastos 2019 hasta junio</h2>
            </th>
          </tr>
          <tr>
            <th class="text-center"></th>
            <th class="text-center">junio plan</th>
            <th class="text-center">Junio real</th>
            <th class="text-center">% de cumplimiento</th>
            <th class="text-center">% en relacion a ingresos</th>
            <th class="text-center">% de los gastos en función del total</th>
            <th class="text-center">Hasta junio plan</th>
            <th class="text-center">Hasta junio real</th>
            <th class="text-center">% de cumplimiento</th>
            <th class="text-center">% en ralacion a ingresos</th>
            <th class="text-center">% de los gastos en función del total</th>
          </tr>
        </thead>
        <tbody style="overflow-y:auto;">
          <tr
            v-for="item in ingresos"
            :key="item.grupo"
            v-bind:class="[(item.grupo === 'Ingresos')? 'negrita':'']"
          >
            <td v-bind:class="[(item.grupo === 'Ingresos')? 'text-right':'colum0']">{{ item.grupo }}</td>
            <td>{{ item.planMes }}</td>
            <td>{{ item.realMes }}</td>
            <td>{{ item.porcCumplimiento }}</td>
            <td>{{item.porcRelacionIngresos}}</td>
            <td>{{item.porcGastosFuncionTotal}}</td>
            <td>{{item.planAcumulado}}</td>
            <td>{{item.realAcumulado}}</td>
            <td>{{item.porcCumpAcumulado}}</td>
            <td>{{item.porcIngresosFuncionTotal}}</td>
            <td>{{item.porcGastosFuncionTotalAcumulado}}</td>
          </tr>

          <tr
            v-for="item in egresos"
            :key="item.grupo"
            v-bind:class="[(item.grupo === 'Egresos')? 'negrita':'']"
          >
            <td v-bind:class="[(item.grupo === 'Egresos')? 'text-right':'colum0']">{{ item.grupo }}</td>
            <td>{{ item.planMes }}</td>
            <td>{{ item.realMes }}</td>
            <td>{{ item.porcCumplimiento }}</td>
            <td>{{item.porcRelacionIngresos}}</td>
            <td>{{item.porcGastosFuncionTotal}}</td>
            <td>{{item.planAcumulado}}</td>
            <td>{{item.realAcumulado}}</td>
            <td>{{item.porcCumpAcumulado}}</td>
            <td>{{item.porcIngresosFuncionTotal}}</td>
            <td>{{item.porcGastosFuncionTotalAcumulado}}</td>
          </tr>
          <tr
            v-for="item in utilidades"
            :key="item.grupo"
            v-bind:class="[(item.grupo === 'Utilidad')? 'negrita':'']"
          >
            <td v-bind:class="[(item.grupo === 'Utilidad')? 'text-right':'colum0']">{{ item.grupo }}</td>
            <td>{{ item.planMes }}</td>
            <td>{{ item.realMes }}</td>
            <td>{{ item.porcCumplimiento }}</td>
            <td>{{item.porcRelacionIngresos}}</td>
            <td>{{item.porcGastosFuncionTotal}}</td>
            <td>{{item.planAcumulado}}</td>
            <td>{{item.realAcumulado}}</td>
            <td>{{item.porcCumpAcumulado}}</td>
            <td>{{item.porcIngresosFuncionTotal}}</td>
            <td>{{item.porcGastosFuncionTotalAcumulado}}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
<style scoped>
@media print {
  * {
    display: none;
  }
  #areatoPrint {
    display: block;
  }
  @page {
    size: landscape;
  }
  .tr-th:hover {
    background-color: white !important;
  }
  .encabezado-tabla {
    padding-bottom: 30px;
  }
  table,
  th,
  td {
    border: 1px solid black;
    border-collapse: collapse;
    padding: 5px;
  }
  .encabezado {
    padding-bottom: 20px;
    border-top: none;
  }
  .encabezado1 {
    border-bottom: none;
  }
}
.tr-th:hover {
  background-color: white !important;
}
.encabezado-tabla {
  padding-bottom: 30px;
}
table,
th,
td {
  border: 1px solid black;
  border-collapse: collapse;
  padding: 5px;
  text-align: center;
}
table {
  table-layout: fixed;
  max-width: 100%;
  font-family: Arial, Helvetica, sans-serif;
  font-size: 14px;
}
#areatoPrint {
  overflow-x: auto;
}
td {
  word-wrap: normal;
}
.encabezado {
  padding-bottom: 20px;
  border-top: none;
}
.encabezado1 {
  border-bottom: none;
}
.negrita {
  font-weight: bold;
}
th {
  font-weight: normal;
}
.colum0 {
  text-align: left;
  width: 22%;
}
</style>
<script>
import api from "@/api.js";
import printJS from "print-js";
export default {
  props: ["mes", "year"],
  data() {
    return {
      ingresos: [],
      egresos: [],
      utilidades: []
    };
  },
  created() {
    this.getIngresosFromApi();
    this.getEgresosFromApi();
    this.getUtilidadesFromApi();
  },
  methods: {
    getIngresosFromApi: function() {
      let url = api.getUrl(
        "finanzas",
        "PlanGI/Ingresos/" + this.year + "/" + this.mes
      );
      this.axios
        .get(url)
        .then(response => {
          this.ingresos = response.data;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    },
     getEgresosFromApi: function() {
      let url = api.getUrl(
        "finanzas",
        "PlanGI/Egresos/" + this.year + "/" + this.mes
      );
      this.axios
        .get(url)
        .then(response => {
          this.egresos = response.data;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    },
     getUtilidadesFromApi: function() {
      let url = api.getUrl(
        "finanzas",
        "PlanGI/Utilidades/" + this.year + "/" + this.mes
      );
      this.axios
        .get(url)
        .then(response => {
          this.utilidades = response.data;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    },
    exportar: function() {
      var TableExport = require("tableexport");
      // dom id, filename, type: json, txt, csv, xml, doc, xsl, image, pdf
      TableExport(document.getElementById("table1"));
    },
    imprimir: function() {
      require("print-js");
      printJS("areatoPrint", "html");
      // var printContents = document.getElementById("areatoPrint").innerHTML;
      // var originalContents = document.body.innerHTML;

      // document.body.innerHTML = printContents;

      // window.print();

      // document.body.innerHTML = originalContents;
    }
  }
};
</script>