<template>
  <v-container fluid>
    <p class="display-1 font-weight-light text-center text-uppercase mt-6 mb-2">
      <strong class="orange--text">Usuarios</strong>
    </p>
    <div v-if="dataArrived" class="mx-auto">
      <v-col cols="12">
        <v-data-table
          calculate-widths
          hide-default-footer
          :options="options"
          :items="users"
          :headers="headers"
          show-select
          dense
        >
          <template v-slot:top>
            <v-toolbar flat dense>
              <v-btn outlined text color="success" @click="add">Agregar</v-btn>
            </v-toolbar>
          </template>
          <template v-slot:[`item.profile_picture`]="{ item }">
            <v-img
              height="90"
              width="130"
              :src="media_url_auth + item.profile_picture"
              v-if="item.profile_picture"
            >
            </v-img>
          </template>
          <template v-slot:[`item.is_superuser`]="{ item }">
            <v-chip
              outlined
              v-html="item.is_superuser ? 'Administrador' : 'Usuario'"
              color="orange"
            ></v-chip>
            <v-chip
              outlined
              v-html="'Usuario Avanzado'"
              color="orange"
              v-if="item.is_staff"
            ></v-chip>
          </template>
          <template v-slot:[`item.actions`]="{ item }">
            <v-row justify="center" no-gutters>
              <v-btn
                @click="editUser(item)"
                :disabled="!!!item.is_active"
                class="mr-8"
                icon
                large
                color="info"
              >
                <v-icon large>mdi-circle-edit-outline</v-icon>
              </v-btn>

              <v-btn
                @click="deactivatedElement(item)"
                v-if="!!item.is_active"
                dark
                icon
                large
                color="#8f4a3f"
              >
                <v-icon large>mdi-delete-clock</v-icon>
              </v-btn>

              <v-btn
                @click="activateElement(item)"
                v-else
                dark
                icon
                large
                color="success"
              >
                <v-icon large>mdi-restore</v-icon>
              </v-btn>
              <v-tooltip top color="error">
                <template v-slot:activator="{ on }">
                  <v-btn
                    v-on="on"
                    @click="confirmDelete(item)"
                    dark
                    icon
                    large
                    color="error"
                  >
                    <v-icon large>mdi-trash-can</v-icon>
                  </v-btn>
                </template>
                <span>Eliminar</span>
              </v-tooltip>
            </v-row>
          </template>
          <template v-slot:footer>
            <v-divider></v-divider>
            <TableFooter
              :pagination="pagination"
              :options="options"
            ></TableFooter>
          </template>
          <template v-slot:no-data>
            <v-row justify="center">
              <h2 class="heading">No hay Usuarios Disponibles</h2>
            </v-row>
          </template>
          <template v-slot:[`item.with_rice`]="{ value }">
            {{ value ? "Sí" : "No" }}
          </template>
        </v-data-table>
      </v-col>
    </div>
    <v-row
      align="stretch"
      v-if="!dataArrived"
      justify="center"
      class="fill-height pt-12"
    >
      <v-col cols="12" class="fill-height text-center pt-12">
        <v-progress-circular
          class="fill-height"
          indeterminate
          color="light-blue"
          size="175"
          ><span class="gray--text lighten-2">Cargando<br />Datos</span>
        </v-progress-circular>
      </v-col>
    </v-row>
    <AddEdit
      v-if="dialog"
      @saveData="editData"
      :title="title"
      :modal="dialog"
      :data="data"
      @closeModal="closeModal"
      @loading="loading"
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
import { Filters, Options, Pagination } from "@/types";
import { RouteOptions } from "@/services/repos/IRepository";
import UserServices from "@/services/repos/UserServices";
import AddEdit from "@/components/Users/AddEdit.vue";
import TableFooter from "@/components/Utils/TableFooter.vue";
import { Method } from "axios";
import AuthServices from "@/services/repos/AuthServices";
import ConfirmDelete from "@/components/Utils/ConfirmDelete.vue";
@Component({
  name: "Home",
  components: { AddEdit, TableFooter, ConfirmDelete },
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
    { text: "Nombre", value: "username" },
    { text: "Correo", value: "email", align: "center" },
    { text: "Teléfono", value: "phone_number", align: "left" },
    { text: "picture", value: "profile_picture", align: "left" },
    { text: "Roles", value: "is_superuser", align: "left" },
    { text: "Acciones", value: "actions", align: "center" },
  ];
  public users: any[] = [];
  public dataArrived: boolean = false;
  private dialog: boolean = false;
  private token: string = "";
  private user_data: any = {};
  private title: string = "";
  private data: object = {};
  private loading: boolean = false;
  private search: string = "";
  private filters: Filters = {};
  private media_url_auth: string | undefined =
    process.env.VUE_APP_MEDIA_URL_AUTH;
  private dialogConfirmDelete: boolean = false;

  // Hooks
  public mounted(): void {
    this.user_data = localStorage.getItem("user_data") || "null";
    this.token = localStorage.getItem("access_token") || "null";
    this.onOptionsChange();
  }
  // Methods
  @Watch("options", { deep: true })
  private async onOptionsChange() {
    this.users = [];
    this.dataArrived = false;
    const options: RouteOptions = {
      method: "GET" as Method,
      data: {
        offset: (this.options.page - 1) * this.options.itemsPerPage,
        limit: this.options.itemsPerPage,
      },
    };
    if (this.token != "null") {
      options.providedHeaders = { Authorization: `Bearer ${this.token}` };
    }
    const users = await UserServices.list(options);

    if (users) {
      this.users = users.data.results || [];
      this.dataArrived = true;
      this.pagination.length = Math.ceil(
        users.data.total / this.options.itemsPerPage
      );
    }
    setTimeout(() => {
      this.dataArrived = true;
    }, 3000);
  }
  private add() {
    this.dialog = true;
    this.title = "Agregar Usuario";
    this.data = {};
  }
  private editUser(store) {
    this.dialog = true;
    this.title = "Editar Usuario";
    this.data = Object.assign({}, store);
  }
  private closeModal() {
    this.dialog = false;
    this.dialogConfirmDelete = false;
  }
  private activateElement(data) {
    data.is_active = 1;
    data.is_staff = data.is_staff ? 1 : 0;
    data.is_superuser = data.is_superuser ? 1 : 0;
    this.editData(data);
  }
  private deactivatedElement(data) {
    data.is_active = 0;
    data.is_staff = data.is_staff ? 1 : 0;
    data.is_superuser = data.is_superuser ? 1 : 0;
    this.editData(data);
  }
  public async editData(data) {
    let form = new FormData();
    form.append("is_superuser", data.is_superuser);
    form.append("is_staff", data.is_staff);
    form.append("is_active", data.is_active);
    const options: RouteOptions = {
      data: form,
      url: data.id ? `${data.id}` : "",
      providedHeaders: { Authorization: `Bearer ${this.token}` },
    };
    if (data.id) {
      const store = await UserServices.update(options);
      if (store) {
        this.dialog = false;
        this.$toast.success("Se actualizó correctamente");
        this.onOptionsChange();
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
    const user = await UserServices.delete(
      methodPOSTAsMethodWSMethodDataAsObject
    );
    this.dialogConfirmDelete = false;
    this.$toast.success("Se eliminó correctamente");
    this.onOptionsChange();
  }
}
</script>
<style scoped>
.margin-p {
  margin-left: 100px;
}
</style>
