<template>
  <v-container fluid>
    <!-- <v-row
      ><v-col cols="10" md="5" sm="10">
        <v-text-field label="Buscar" v-model="search"></v-text-field></v-col
      ><v-col cols="1" md="1" sm="2" class="mt-5"
        ><v-btn icon @click="searchMethod()"
          ><v-icon large>mdi-magnify</v-icon></v-btn
        ></v-col
      ></v-row
    > -->
    <div v-if="dataArrived" class="mx-auto">
      <v-data-table
        calculate-widths
        :options="options"
        :items="products"
        :headers="headers"
        dense
        :footer-props="{
          itemsPerPageText: 'Resultados por Página',
        }"
      >
        <template v-slot:top>
          <v-toolbar flat dense color="white" class="mt-4 mb-4">
            <v-toolbar-title>Productos</v-toolbar-title>
            <v-divider class="mx-2" inset vertical></v-divider>
            <v-spacer></v-spacer>
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              label="Buscar"
              single-line
              hide-details
              clearable
              dense
            ></v-text-field>
            <v-spacer></v-spacer>
            <template>
              <v-btn color="primary" @click="add">Agregar</v-btn>
            </template>
          </v-toolbar>
        </template>
        <template v-slot:[`item.name`]="{ item }">
          <h4 class="title_insight">
            {{ item.nombre }} ({{ item.cantidad
            }}{{ item.unidadDeMedida.siglas }})
          </h4>
          <br />
          Grupo: {{ item.grupo.descripcion }}<br />
          Precio de Compra: {{ item.precioDeVenta }}<br />
          Precio promedio: {{ item.precioUnitario }}<br />
          {{
            item.inventariable
              ? "Este producto es inventariable"
              : "Este producto no es inventariable"
          }}<br />
          StockMinimo: {{ item.stockMinimo }}
        </template>
        <template v-slot:[`item.image`]="{ value }">
          <v-img height="100" width="130" :src="value">
            <template v-slot:placeholder>
              <v-img aspect-ratio="1.1" :src="value"></v-img> </template
          ></v-img>
        </template>
        <template v-slot:[`item.existencias`]="{ item }">
          <v-chip color="blue darken-4" dark
            >{{ item.existenciaTotal }} {{ item.unidadDeMedida.siglas }}</v-chip
          ><br />
          <span v-for="(e, index) in item.existencias" :key="index">
            {{ e.lugar }}: {{ e.cantidad }} {{ item.unidadDeMedida.siglas
            }}<br />
          </span>
        </template>

        <!-- <template v-slot:footer>
          <v-divider></v-divider>
          <TableFooter
            :pagination="pagination"
            :options="options"
          ></TableFooter>
        </template> -->
        <template v-slot:[`item.image_url`]="{ value }">
          <v-img
            aspect-ratio="1"
            height="60"
            width="60"
            :src="base_url + value"
            contain
          ></v-img>
        </template>
        <template v-slot:[`item.actions`]="{ item }">
          <Actions
            :data="item"
            @handleFileImport="handleFileImport"
            @onFileChanged="onFileChanged"
            @editElement="editElement"
            @disableElement="disableElement"
            @activateElement="activateElement"
            @confirmDelete="confirmDelete"
            @historical="historical"
          />
        </template>
        <template v-slot:no-data>
          <v-row justify="center">
            <h2 class="heading">No hay Productos Disponibles</h2>
          </v-row>
        </template>
      </v-data-table>
    </div>
    <GettingData :dataArrived="dataArrived" />
    <AddEdit
      v-if="dialog"
      @saveData="editData"
      :title="title"
      :modal="dialog"
      :data="data"
      @closeModal="closeModal"
    >
    </AddEdit>
    <ConfirmDelete
      :modal="dialogConfirmDelete"
      :data="data"
      @deleteElement="deleteElement"
      @closeModal="closeModal"
    >
    </ConfirmDelete>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { Filters, Options, Pagination, Product, Group } from "@/types";
import { RouteOptions } from "@/services/repos/IRepository";
import ProductsService from "@/services/repos/ProductsService";
import AddEdit from "@/components/Product/AddEdit.vue";
import TableFooter from "@/components/Utils/TableFooter.vue";
import GettingData from "@/components/Utils/GettingData.vue";
import ConfirmDelete from "@/components/Utils/ConfirmDelete.vue";
import { Method } from "axios";
import Actions from "@/components/Utils/Actions.vue";
import getLocalData from "@/util/getLocalData";

@Component({
  name: "Home",
  components: { AddEdit, TableFooter, GettingData, ConfirmDelete, Actions },
})
export default class Home extends Vue {
  // Props

  // Data
  private options: Options = {
    page: 1,
    itemsPerPage: 5,
    sortBy: [],
    sortDesc: [],
    groupBy: [],
    groupDesc: [],
    multiSort: false,
    mustSort: false,
  };
  private pagination: Pagination = {
    limits: [5, 10, 15, 20],
    defaultLimit: 10,
    length: 0,
  };
  private headers: object[] = [
    { text: "Descripción", value: "name" },
    { text: "Existencia", value: "existencias" },
    { text: "Acciones", value: "actions", align: "center" },
  ];
  private products: Product[] = [];
  private dataArrived: boolean = false;
  private dialog: boolean = false;
  private dialogConfirmDelete: boolean = false;
  private token: string = "";
  private user_data: any = {};
  private data: object = {};
  private defaultData: Product = {
    id: 0,
    nombre: "",
    cantidad: "",
    grupo: { id: 0, descripcion: "" },
    precioDeVenta: 0,
    precioUnitario: 0,
    unidadDeMedida: { nombre: "", siglas: "" },
    esInventariable: false,
    stockMinimo: 0,
    proporcionDeMerma: 0,
    active: false,
  };
  private loading: boolean = false;
  private search: string = "";
  private title: string = "";
  private selectedFile: any = {};
  private isSelecting: boolean = false;
  private product: any = {};

  // Hooks
  public mounted(): void {
    this.user_data = localStorage.getItem("user_data") || "null";
    this.token = localStorage.getItem("access_token") || "null";
    this.onOptionsChange();
  }
  // Methods
  @Watch("options", { deep: true })
  private async onOptionsChange() {
    // const options: RouteOptions = {
    //   method: "GET" as Method,
    //   data: {
    //     offset: (this.options.page - 1) * this.options.itemsPerPage,
    //     limit: this.options.itemsPerPage,
    //   },
    // };
    // options.providedHeaders = { Authorization: `Bearer ${this.token}` };
    // const products = await ProductsService.list(options);
    // if (products) {
    //   this.products = products.results || [];
    //   this.dataArrived = true;
    //   this.pagination.length = Math.ceil(
    //     products.count / this.options.itemsPerPage
    //   );
    // }
    this.dataArrived = false;
    this.products = getLocalData.getProducts();
    this.dataArrived = true;

    setTimeout(() => {
      this.dataArrived = true;
    }, 1000);
  }

  private add() {
    this.dialog = true;
    this.title = "Agregar Producto";
    this.data = this.defaultData;
  }
  private editElement(product) {
    this.dialog = true;
    this.title = "Editar Producto";
    this.data = Object.assign({}, product);
  }
  private activateElement(data) {
    data.active = true;
    this.editData(data);
  }
  private disableElement(data) {
    data.active = false;
    this.editData(data);
  }
  private closeModal() {
    this.dialog = false;
    this.dialogConfirmDelete = false;
    this.data = this.defaultData;
  }

  private async editData(data) {
    let form = new FormData();
    form.append("name", data.name);
    form.append("active", data.active || false);
    const options: RouteOptions = {
      data: form,
      url: data.id ? `${data.id}` : "",
      providedHeaders: { Authorization: `Bearer ${this.token}` },
    };
    if (data.id) {
      let result = await ProductsService.update(options);
      if (result) {
        this.response(data);
      }
    } else {
      form.append("image", data.image);
      const product = await ProductsService.insert(options);
      if (product) {
        this.response(data);
      }
    }
  }
  public confirmDelete(data) {
    this.dialogConfirmDelete = true;
    this.data = data;
  }

  private async deleteElement(data) {
    const methodPOSTAsMethodWSMethodDataAsObject = {
      method: "DELETE" as Method,
      id: data.id,
      providedHeaders: { Authorization: `Bearer ${this.token}` },
    };
    const store = await ProductsService.delete(
      methodPOSTAsMethodWSMethodDataAsObject
    );
    this.dialogConfirmDelete = false;
    this.$toast.success("Se eliminó correctamente");
    this.onOptionsChange();
  }
  handleFileImport(item) {
    this.product = Object.assign({}, item);
    this.isSelecting = true;
    // Después de obtener el enfoque al cerrar FilePicker, vuelva el estado del botón a la normalidad
    window.addEventListener(
      "focus",
      () => {
        this.isSelecting = false;
      },
      { once: true }
    );
    // Haga clic en el gatillo en FileInput
    //
    if (this.$refs.uploader instanceof HTMLElement) {
      this.$refs.uploader.click();
    }
  }
  async onFileChanged(e) {
    this.selectedFile = e.target.files[0];
    var data = this.product;
    let form = new FormData();
    form.append("name", data.name);
    form.append("price", data.price || null);
    form.append("image", this.selectedFile);
    form.append("active", data.active || false);

    const options: RouteOptions = {
      data: form,
      url: data.id ? `${data.id}` : "",
      providedHeaders: { "Content-Type": "multipart/form-data" },
    };
    let result = await ProductsService.update(options);
    if (result) {
      this.response(data);
    }
  }
  public historical(item) {
    console.log(item);
  }
  public searchMethod() {
    this.axios
      .get(`${process.env.VUE_APP_SERVICE_URL}products/find/${this.search}`, {
        headers: {
          Authorization: `Bearer ${this.token}`,
        },
      })
      .then((response) => {
        this.products = response.data.results;
      });
  }
  public async response(data) {
    this.loading = false;
    this.closeModal();
    await this.onOptionsChange();
    return this.$toast.success(
      data.id ? "Se actualizó correctamente" : "Se creó correctamente"
    );
  }
}
</script>
<style scoped>
.margin-p {
  margin-left: 100px;
}
</style>
