<template>
  <div class="text-center">
    <v-dialog v-model="dialog" width="500">
      <v-card>
        <v-card-title class="text-h5 grey lighten-2">
          <span class="headline">{{ title }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" class="ml-6">
            <v-row>
              <v-col cols="12" md="6" sm="6">
                <v-text-field
                  label="Nombre"
                  :rules="[(v) => !!(v && v.length) || 'Requerido']"
                  v-model="dataModel.username"
                  :readonly="dataModel.id ? true : false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" md="6" sm="6">
                <v-text-field
                  label="Correo"
                  v-model="dataModel.email"
                  :readonly="dataModel.id ? true : false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" md="6" sm="6">
                <v-text-field
                  label="Teléfono"
                  v-model="dataModel.phone_number"
                  :readonly="dataModel.id ? true : false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" md="6" sm="6" v-if="!dataModel.id">
                <v-text-field
                  v-model="dataModel.password"
                  :rules="passwordRules"
                  label="Contraseña"
                  :append-icon="show_password ? 'mdi-eye' : 'mdi-eye-off'"
                  :type="show_password ? 'text' : 'password'"
                  @click:append="show_password = !show_password"
                  clearable
                  required
                >
                </v-text-field>
              </v-col>
              <v-col cols="12" md="6" sm="6" v-if="!dataModel.id">
                <v-text-field
                  v-model="confirmPassword"
                  :rules="confirmRules"
                  label="Confirmar Contraseña"
                  :append-icon="
                    show_confirm_password ? 'mdi-eye' : 'mdi-eye-off'
                  "
                  :type="show_confirm_password ? 'text' : 'password'"
                  @click:append="show_confirm_password = !show_confirm_password"
                  clearable
                  required
                >
                </v-text-field
              ></v-col>
            </v-row>
            <v-row>
              <v-col cols="12" md="6" sm="6">
                <v-switch
                  v-model="dataModel.is_superuser"
                  color="blue"
                  inset
                  :label="`Administrador`"
                ></v-switch>
              </v-col>
              <v-col cols="12" md="6" sm="6">
                <v-switch
                  v-model="dataModel.is_staff"
                  color="blue"
                  inset
                  :label="`Usuario Avanzado`"
                ></v-switch>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="success" @click="openModal()">
            Cambiar Contraseña
          </v-btn>
          <v-btn color="success" @click="save()"> Guardar </v-btn>
          <v-btn color="error" @click="$emit('closeModal')">Cerrar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <ChangePassword
      v-if="dataModel.id"
      @changePassword="changePassword"
      @close="close"
      :dialog="dialogPass"
    />
  </div>
</template>
<script lang="ts">
import { Component, Prop, PropSync, Vue } from "vue-property-decorator";
import { RouteOptions } from "@/services/repos/IRepository";
import { Method } from "axios";
import ChangePassword from "@/components/Account/ChangePassword.vue";

interface IProducts {
  [propName: string]: any;
}

@Component({
  components: { ChangePassword },
})
export default class AddEdit extends Vue {
  @Prop({ required: true }) public modal!: boolean;
  private loading: boolean = false;
  @PropSync("data") public dataModel!: IProducts;
  @Prop({ required: true }) public title!: string;
  private valid: boolean = false;
  private disableSaveBtn: boolean = false;
  private ready: boolean = false;
  private confirmPassword: string = "";
  private show_password: boolean = false;
  private show_confirm_password: boolean = false;
  public passwordRules: any[] = [(v) => !!v || "La contraseña es requerida"];
  private dialogPass: boolean = false;
  private user_data: any = {};
  private token: string = "";

  // Hooks
  private get confirmRules() {
    const rules: any[] = [];
    const matchRule = (v) =>
      (!!v && v) === this.dataModel.password || "Contraseñas no coinciden";
    rules.push(matchRule);
    return rules;
  }
  get dialog() {
    return this.modal;
  }

  created() {}
  async mounted() {
    this.user_data = JSON.parse(localStorage.getItem("user_data") || "[]");
    this.token = localStorage.getItem("access_token") || "null";
  }
  private save() {
    if (this.valid) {
      this.dataModel.is_superuser = this.dataModel.is_superuser == true ? 1 : 0;
      this.dataModel.is_staff = this.dataModel.is_staff == true ? 1 : 0;
      this.dataModel.is_active = this.dataModel.is_active == true ? 1 : 0;
      this.loading = this.modal;
      this.$emit("saveData", this.dataModel);
    } else {
      // @ts-ignore
      this.$refs.form.validate();
    }
  }
  public setPicture(image) {
    if (image) {
      this.dataModel.image = image;
    }
  }
  private openModal() {
    this.dialogPass = true;
  }
  public changePassword(password) {
    var data = this.user_data;
    let form = new FormData();
    form.append("password", password);
    if (data.id) {
      this.axios
        .post(`${process.env.VUE_APP_AUTH_URL}users/${data.id}/`, form, {
          headers: {
            Authorization: `Bearer ${this.token}`,
          },
        })
        .then((response) => {
          this.close();
          return this.$toast.success(
            data.id ? "Se actualizó correctamente" : "Se creó correctamente"
          );
        });
    }
  }
  private close() {
    this.dialogPass = false;
  }
}
</script>
