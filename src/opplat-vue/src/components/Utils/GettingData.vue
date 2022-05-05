<template>
  <v-row
    class="fill-height"
    align-content="center"
    justify="center"
    v-if="!data_Arrived"
  >
    <v-col class="text-subtitle-1 text-center" cols="12">
      Cargando Datos
    </v-col>
    <v-col cols="6">
      <v-progress-linear
        color="deep-purple accent-4"
        indeterminate
        rounded
        height="6"
      ></v-progress-linear>
    </v-col>
  </v-row>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class AddEdit extends Vue {
  @Prop({ required: true }) public dataArrived!: boolean;

  get data_Arrived() {
    return this.dataArrived;
  }
}
</script>
