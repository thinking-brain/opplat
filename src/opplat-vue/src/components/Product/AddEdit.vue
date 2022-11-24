<template>
  <v-dialog
    v-model="dialog"
    :fullscreen="$vuetify.breakpoint.xsOnly"
    transition="dialog-bottom-transition"
    persistent
    scrollable
    max-width="600px"
    min-height="800px"
  >
    <v-card>
      <v-card-title>
        <span class="headline">{{ title }}</span>
      </v-card-title>
      <v-card-text style="height: 100%">
        <v-form ref="form" v-model="valid" class="ml-6">
          <v-row>
            <!-- <v-col cols="12" v-if="!dataModel.id">
              <v-file-input
                v-model="dataModel.image"
                accept="image/png, image/jpeg, image/bmp"
                placeholder="Imagen"
              ></v-file-input>
            </v-col> -->
            <v-col cols="12">
              <v-text-field
                label="Nombre"
                :rules="[(v) => !!(v && v.length) || 'Requerido']"
                v-model="dataModel.nombre"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" sm="6">
              <v-autocomplete
                label="Grupo"
                :items="groups"
                v-model="dataModel.grupo"
                item-text="descripcion"
                item-value="id"
                required
              ></v-autocomplete>
            </v-col>
            <v-col cols="12" md="6" sm="6">
              <v-text-field
                label="Unidad de Medida"
                v-model="dataModel.unidadDeMedida.siglas"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" sm="6">
              <v-text-field
                label="Presentación"
                type="number"
                v-model="dataModel.cantidad"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" sm="6">
              <v-text-field
                label="Precio de Compra"
                type="number"
                prefix="$"
                v-model="dataModel.precioUnitario"
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" sm="6">
              <v-switch
                v-model="dataModel.esInventariable"
                inset
                :label="
                  dataModel.esInventariable
                    ? 'Es Inventariable'
                    : 'No es Inventariable'
                "
              ></v-switch>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
      <v-card-actions class="justify-space-between px-4">
        <v-btn color="error" text @click="$emit('closeModal')">Cerrar</v-btn>
        <v-btn
          color="success"
          text
          :loading="loading"
          :disabled="!valid"
          @click="save"
          >Guardar</v-btn
        >
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue } from "vue-property-decorator";
import { RouteOptions } from "@/services/repos/IRepository";
import { Product } from "@/types";
import getLocalData from "@/util/getLocalData";

@Component
export default class AddEdit extends Vue {
  @Prop({ required: true }) public modal!: boolean;
  private loading: boolean = false;
  @PropSync("data") public dataModel!: Product;
  @Prop({ required: true }) public title!: string;
  private valid: boolean = false;
  private disableSaveBtn: boolean = false;
  private ready: boolean = false;
  private currency: any = { nombre: "USDT", valor: "usd" };
  private unidades: string[] = ["U", "Lit", "g", "kg", "Lib", "oz"];
  private descriptionRules: any = [
    (v) =>
      v.length <= 254 ||
      "Asegúrese que la descripción tenga menos de 254 caracteres",
  ];
  private groups: any[] = [];

  // Computed
  get dialog() {
    return this.modal;
  }

  created() {}
  async mounted() {
    this.groups = getLocalData.getGroups();

    const options: RouteOptions = {
      data: { is_active: 1 },
    };
  }
  private save() {
    if (this.valid) {
      this.loading = this.modal;
      this.$emit("saveData", this.dataModel);
    } else {
      // @ts-ignore
      this.$refs.form.validate();
    }
  }
  public setPicture(image) {
    if (image) {
      this.dataModel.picture = image;
    }
  }
}
</script>
