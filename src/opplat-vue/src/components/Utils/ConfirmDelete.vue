<template>
  <v-row justify="center">
    <v-dialog v-model="dialog" persistent max-width="290" class="rounded-pill">
      <v-card>
        <h3
          class="text-subtitle-2 text-center text-align-justify pa-6"
          style="color: red"
        >
          ¿Estás Seguro?<br />
          Si elimina este elemento no lo podrá recuperar
        </h3>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            elevation="5"
            color="error"
            @click="deleteElement()"
            :loading="loading"
          >
            Aceptar
          </v-btn>
          <v-btn text outlined elevation="5" @click="cancel()">
            Cancelar
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue } from "vue-property-decorator";

@Component({
  name: "ConfirmDelete",
  components: {},
})
export default class ConfirmDelete extends Vue {
  // Props
  @Prop({ required: true }) public modal!: boolean;
  @PropSync("data") public dataModel;
  private loading: boolean = false;

  // Data

  // Hooks
  get dialog() {
    return this.modal;
  }
  public mounted(): void {}
  // Methods
  private deleteElement() {
    this.loading = this.modal;
    this.$emit("deleteElement", this.dataModel);
  }
  private cancel() {
    this.$emit("closeModal");
  }
}
</script>
