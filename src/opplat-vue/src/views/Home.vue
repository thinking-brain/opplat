<template>
  <h1 class="m-0">Dashboard</h1>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import VueApexCharts from "vue-apexcharts";
import { RouteOptions } from "@/services/repos/IRepository";
import { Method } from "axios";

@Component({
  name: "Home",
  components: { apexchart: VueApexCharts },
})
export default class Home extends Vue {
  // Props

  // Data

  // Hooks
  public mounted(): void {}
  // Methods
}
</script>
