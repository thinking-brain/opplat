<template>
  <v-container>
    <p class="display-1 text-center text--primary mt-4">Venta</p>
    <v-form ref="form" v-model="valid" class="mx-1">
      <v-row>
        <v-col cols="12" md="8" sm="8">
          <v-autocomplete
            label="Dependiente"
            :items="groups"
            v-model="dataModel.grupo"
            item-text="descripcion"
            item-value="id"
            required
          ></v-autocomplete>
        </v-col>
        <v-col cols="12" md="4" sm="4">
          <v-autocomplete
            label="Posición"
            :items="groups"
            v-model="dataModel.grupo"
            item-text="descripcion"
            item-value="id"
            required
          ></v-autocomplete>
        </v-col>
        <v-col cols="12" md="3" sm="3">
          <v-text-field
            label="Comanda"
            v-model="dataModel.unidadDeMedida"
            required
          ></v-text-field>
        </v-col>
        <v-col cols="12" md="9" sm="9">
          <v-textarea
            label="Observaciones"
            v-model="dataModel.precioUnitario"
            rows="1"
          ></v-textarea>
        </v-col>
        <v-divider></v-divider>
        <v-col cols="12" md="4" sm="4">
          <v-autocomplete
            label="Menu"
            :items="groups"
            v-model="dataModel.grupo"
            item-text="descripcion"
            item-value="id"
            required
          ></v-autocomplete>
        </v-col>
        <v-col cols="12" md="4" sm="4">
          <v-text-field
            label="Cantidad"
            v-model="dataModel.unidadDeMedida"
            required
          ></v-text-field>
        </v-col>
        <v-col cols="12" md="3" sm="3">
          <v-text-field
            label="Importe Total"
            v-model="dataModel.unidadDeMedida"
            required
          ></v-text-field>
        </v-col>
        <v-col cols="12" md="1" sm="1" class="mt-3">
          <v-btn outlined fab color="teal" small>
            <v-icon>mdi-plus-thick</v-icon>
          </v-btn></v-col
        >
      </v-row>
    </v-form>
    <v-card-actions class="justify-space-between px-4">
      <v-btn color="primary" :loading="loading" :disabled="!valid" @click="save"
        >Registrar Venta</v-btn
      >
    </v-card-actions>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component({
  name: "Sell",
  components: {},
})
export default class Sell extends Vue {
  // Props

  // Data
  private dataModel = {};
  // Hooks
  public mounted(): void {}
  // Methods
}
</script>
