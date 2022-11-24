<template>
  <v-dialog v-model="dialog" transition="dialog-bottom-transition" persistent>
    <v-img :src="dataModel.voucher">
      <v-toolbar flat dense
        ><v-spacer /><v-btn icon @click="closeModal()">
          <v-icon>mdi-close</v-icon>
        </v-btn></v-toolbar
      ></v-img
    >
  </v-dialog>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue } from "vue-property-decorator";

@Component
export default class BigPicture extends Vue {
  @Prop({ required: true }) public modal!: boolean;
  @PropSync("data") public dataModel;

  get dialog() {
    return this.modal;
  }
  public closeModal() {
    this.$emit("closePicture");
  }
}
</script>
