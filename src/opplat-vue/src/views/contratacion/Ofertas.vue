<template>
  <v-data-table :headers="headers" :items="Ofertas" :search="search" class="elevation-1 pa-5">
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Listado de Ofertas</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
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
        <!-- Agregar y Editar Oferta -->
        <v-dialog v-model="dialog" persistent max-width="1100">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on">Nueva Oferta</v-btn>
          </template>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>{{ formTitle }}</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click=" close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-form ref="form" v-model="valid" lazy-validation>
              <v-container grid-list-md text-xs-center>
                <v-layout row wrap>
                  <v-flex xs4 class="px-3">
                    <v-text-field v-model="Oferta.Nombre" label="Nombre" clearable required></v-text-field>
                  </v-flex>
                  <v-flex xs4 class="px-3">
                    <v-autocomplete
                      v-model="Oferta.Tipo"
                      item-text="nombre"
                      item-value="id"
                      :items="tipos"
                      :filter="activeFilter"
                      cache-items
                      label="Tipo"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs4 class="px-3">
                    <v-autocomplete
                      v-model="Oferta.EntidadId"
                      item-text="nombre"
                      item-value="id"
                      :items="entidades"
                      :filter="activeFilter"
                      cache-items
                      label="Prestador"
                    >
                      <v-icon @click="dialog3=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                    </v-autocomplete>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs4 class="px-3">
                    <v-autocomplete
                      v-model="Oferta.FormasDePago"
                      item-text="nombre"
                      item-value="id"
                      :items="formasDePagos"
                      :filter="activeFilter"
                      cache-items
                      label="Formas de Pago"
                      multiple
                    >
                      <template v-slot:selection="{ item, index }">
                        <span v-if="index <= 2">{{ item.nombre }}</span>
                        <v-spacer></v-spacer>
                        <span
                          v-if="index > 2"
                          class="grey--text caption"
                        >( y {{ Oferta.formaDePago.length - 3 }} más)</span>
                      </template>
                    </v-autocomplete>
                  </v-flex>
                  <v-flex xs4 class="px-3">
                    <v-text-field
                      v-model="Oferta.MontoCup"
                      label="Monto CUP"
                      clearable
                      required
                      prefix="$"
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs4 class="px-3">
                    <v-text-field
                      v-model="Oferta.MontoCuc"
                      label="Monto CUC"
                      clearable
                      required
                      prefix="$"
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      v-model="Oferta.TerminoDePago"
                      label="Término de Pago en Meses"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-menu
                      v-model="menu"
                      :close-on-content-click="false"
                      :nudge-right="40"
                      transition="scale-transition"
                      offset-y
                      full-width
                      min-width="290px"
                    >
                      <template v-slot:activator="{ on }">
                        <v-text-field
                          v-model="Oferta.FechaDeRecepcion"
                          label="Fecha de Recepción "
                          readonly
                          clearable
                          v-on="on"
                        ></v-text-field>
                      </template>
                      <v-date-picker v-model="Oferta.FechaDeRecepcion" @input="menu = false"></v-date-picker>
                    </v-menu>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      v-model="Oferta.Vigencia"
                      label="Vigencia"
                      placeholder="Cantidad"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-select
                      v-model="Oferta.vigenciaDMA"
                      :items="items"
                      label="Vigencia en"
                      placeholder="Días, Meses o Años"
                      clearable
                    ></v-select>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs4 class="pa-3">
                    <v-text-field
                      v-model="Oferta.ObjetoDeContrato"
                      label="Objeto"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs4 class="pa-3">
                    <v-autocomplete
                      v-model="Oferta.TrabajadorId"
                      item-text="id"
                      item-value="id"
                      :items="adminContratos"
                      :filter="activeFilter"
                      cache-items
                      label="Administrador"
                    >
                      <v-icon @click="dialog4=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                    </v-autocomplete>
                  </v-flex>
                  <v-flex xs4 class="pa-3">
                    <v-autocomplete
                      v-model="Oferta.EspExternoId"
                      item-text="nombre"
                      item-value="id"
                      :items="especialistasExternos"
                      :filter="activeFilter"
                      cache-items
                      label="Especialista Externo"
                      placeholder="Dictaminador Externo"
                    >
                      <v-icon @click="dialog5=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                    </v-autocomplete>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <!-- <v-flex xs6 class="px-3">
                    <v-autocomplete
                      v-model="Oferta.estado"
                      item-text="nombre"
                      item-value="id"
                      :items="estados"
                      :filter="activeFilter"
                      cache-items
                      label="Estado"
                    ></v-autocomplete>
                  </v-flex>-->
                </v-layout>
              </v-container>
            </v-form>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Agregar y Editar Oferta -->

        <!-- Detalles de la Oferta -->
        <v-dialog
          v-model="dialog6"
          persistent
          transition="dialog-bottom-transition"
          flat
          max-width="1100"
        >
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="dialog6 = false">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-container fluid>
              <v-row dense no-gutters>
                <v-col cols="7">
                  <v-container>
                    <v-row>
                      <v-col cols="10" md="6">
                        <v-text-field v-model="Oferta.nombre" label="Nombre" outlined readonly></v-text-field>
                      </v-col>
                      <v-col cols="3" md="4">
                        <v-text-field
                          v-model="Oferta.objetoDeContrato"
                          label="Objeto"
                          outlined
                          readonly
                        ></v-text-field>
                      </v-col>
                      <v-col cols="3" md="2">
                        <v-text-field
                          v-model="Oferta.numero"
                          label="Número"
                          outlined
                          readonly
                          prefix="#"
                        ></v-text-field>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col cols="6" md="4">
                        <v-text-field v-model="Oferta.entidad" label="Entidad" outlined readonly></v-text-field>
                      </v-col>
                      <v-col cols="6" md="4">
                        <v-text-field
                          v-model="Oferta.montoCup"
                          label="Monto en CUP"
                          outlined
                          readonly
                          prefix="$"
                        ></v-text-field>
                      </v-col>
                      <v-col cols="6" md="4">
                        <v-text-field
                          v-model="Oferta.montoCuc"
                          label="Monto en CUC"
                          outlined
                          readonly
                          prefix="$"
                        ></v-text-field>
                      </v-col>
                    </v-row>

                    <v-row>
                      <v-col cols="6" md="6">
                        <v-text-field
                          v-model="Oferta.terminoDePago"
                          label="Término"
                          outlined
                          readonly
                        ></v-text-field>
                      </v-col>
                      <v-col cols="6" md="6">
                        <v-text-field v-model="Oferta.estado" label="Estado" outlined readonly></v-text-field>
                      </v-col>
                    </v-row>
                  </v-container>
                </v-col>

                <v-col cols="5">
                  <v-timeline>
                    <v-timeline-item :color="'blue'" :right="true" small>
                      <template v-slot:opposite>
                        <h5 :class="`subtitle-2 blue--text`" v-text="Oferta.fechaDeRecepcion"></h5>
                      </template>
                      <v-card class="elevation-2">
                        <v-card-text
                          :class="`subtitle-2 blue--text`"
                        >Fecha en que se Recibió la Oferta</v-card-text>
                      </v-card>
                    </v-timeline-item>
                    <v-timeline-item :color="'red'" :right="true" small>
                      <template v-slot:opposite>
                        <span :class="`subtitle-2 red--text`" v-text="Oferta.fechaDeVencimiento"></span>
                      </template>
                      <v-card class="elevation-2">
                        <v-card-text
                          :class="`subtitle-2 red--text`"
                        >Fecha en que se Vence la Validez de la Oferta</v-card-text>
                      </v-card>
                    </v-timeline-item>
                  </v-timeline>
                </v-col>
              </v-row>
              <v-row no-gutters class="px-3">
                <v-col>
                  <v-card class="pa-2" outlined tile>
                    <v-text>Aprobado por el Jurídico :</v-text>
                    <span v-if="Oferta.aprobJuridico">
                      <v-icon color="success">mdi-check-underline</v-icon>
                      <v-text :class="`success--text`">Sí</v-text>
                    </span>
                    <span v-else>
                      <v-icon color="red">mdi-close-outline</v-icon>
                      <v-text :class="`red--text`">No</v-text>
                    </span>
                  </v-card>
                </v-col>
                <v-col>
                  <v-card class="pa-2" outlined tile>
                    <v-text>Aprobado por el Económico :</v-text>
                    <span v-if="Oferta.aprobEconomico">
                      <v-icon color="success">mdi-check-underline</v-icon>
                      <v-text :class="`success--text`">Sí</v-text>
                    </span>
                    <span v-else>
                      <v-icon color="red">mdi-close-outline</v-icon>
                      <v-text :class="`red--text`">No</v-text>
                    </span>
                  </v-card>
                </v-col>
                <v-col>
                  <v-card class="pa-2" outlined tile>
                    <v-text>Aprobado por el Comité Contratación:</v-text>
                    <span v-if="Oferta.aprobComitContratacion">
                      <v-text :class="`success--text`">Sí</v-text>
                      <v-icon color="success">mdi-check-underline</v-icon>
                    </span>
                    <span v-else>
                      <v-icon color="red">mdi-close-outline</v-icon>
                      <v-text :class="`red--text`">No</v-text>
                    </span>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- /Detalles de la Oferta -->

        <!-- Delete Oferta -->
        <v-dialog v-model="dialog2" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="dialog2 = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas eliminar la Oferta</v-card-title>
            <v-card-text class="text-center">{{Oferta.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(Oferta)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Oferta -->

        <!-- Subir Documento -->
        <v-row justify="center">
          <v-dialog v-model="dialog7" persistent max-width="300">
            <v-card>
              <v-flex xs12 class="px-3">
                <v-file-input
                  v-model="file"
                  show-size
                  prepend-icon="mdi-note-multiple"
                  label="Seleccionar Documento"
                ></v-file-input>
              </v-flex>
              <v-card-actions>
                <v-spacer></v-spacer>
               <v-btn color="green darken-1" text @click="upload()">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-row>
        <!-- /Subir Documento -->

        <!-- Agregar Entidad-->
        <v-dialog v-model="dialog3" persistent max-width="1000">
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="closeAdd()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <Entidades></Entidades>
          </v-card>
        </v-dialog>
        <!-- /Agregar Entidad-->

        <!-- Agregar AdminContrato-->
        <v-dialog v-model="dialog4" persistent max-width="1000">
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="closeAdd()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <AdminContratos></AdminContratos>
          </v-card>
        </v-dialog>
        <!-- /Agregar AdminContrato-->

        <!-- Agregar EspExternos-->
        <v-dialog v-model="dialog5" persistent max-width="1000">
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="closeAdd()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <EspExternos></EspExternos>
          </v-card>
        </v-dialog>
        <!-- /Agregar EspExternos-->
      </v-toolbar>
    </template>
    <!-- Actions -->
    <template v-slot:item.action="{ item }">
      <v-tooltip top color="primary">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="editItem(item)">mdi-pencil</v-icon>
        </template>
        <span>Editar</span>
      </v-tooltip>
      <v-tooltip top color="primary">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="confirmUpload(item)">mdi-upload</v-icon>
        </template>
        <span>Guardar Documento</span>
      </v-tooltip>
      <v-tooltip top color="green">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="getDetalles(item)">mdi-file-document-box-plus</v-icon>
        </template>
        <span>Detalles</span>
      </v-tooltip>
      <v-tooltip top color="red">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="confirmDelete(item)">mdi-trash-can</v-icon>
        </template>
        <span>Eliminar</span>
      </v-tooltip>
    </template>
    <!-- /Actions -->
  </v-data-table>
</template>
<script>
import api from "@/api";
import AdminContratos from "@/components/contratacion/AdminContratos.vue";
import EspExternos from "@/components/contratacion/EspExternos.vue";
import FormasDePago from "@/components/contratacion/FormasDePago.vue";
import Entidades from "@/components/contratacion/Entidades.vue";

export default {
  components: {
    AdminContratos,
    EspExternos,
    FormasDePago,
    Entidades
  },
  data: () => ({
    dialog: false,
    dialog1: false,
    dialog2: false,
    dialog3: false,
    dialog4: false,
    dialog5: false,
    dialog6: false,
    dialog7: false,
    menu: false,
    menu1: false,
    search: "",
    editedIndex: -1,
    Ofertas: [],
    Oferta: {
      Nombre: "",
      Tipo: null,
      TrabajadorId: null,
      EntidadId: null,
      ObjetoDeContrato: "",
      MontoCup: null,
      MontoCuc: null,
      FechaDeRecepcion: null,
      Vigencia: null,
      FormasDePago: [],
      TerminoDePago: null,
      DictaminadoresId: [],
      EspExternoId: []
    },
    file:null,
    entidades: [],
    especialistasExternos: [],
    adminContratos: [],
    estados: [],
    tipos: [],
    formasDePagos: [],
    formaDePago: {},
    tabs: null,
    errors: [],
    items: ["Días", "Meses", "Años"],
    headers: [
      { text: "Número", sortable: true, value: "numero" },
      { text: "Nombre", align: "left", sortable: true, value: "nombre" },
      { text: "Tipo", value: "tipo" },
      { text: "Entidad", value: "entidad" },
      { text: "Monto Cup", value: "montoCup" },
      { text: "Monto Cuc", value: "montoCuc" },
      { text: "Fecha de Llegada", value: "fechaDeRecepcion" },
      { text: "Estado", value: "estado" },
      { text: "Acciones", value: "action", sortable: false }
    ]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nueva Oferta" : "Editar Oferta";
    },
    method() {
      return this.editedIndex === -1 ? "POST" : "PUT";
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {
    this.getContratosFromApi();
    this.getEstadosFromApi();
    this.getTiposFromApi();
    this.getEntidadesFromApi();
    this.getEspecialistasExternosFromApi();
    this.getAdminContratosFromApi();
    this.getFormasDePagosFromApi();
  },

  methods: {
    getContratosFromApi() {
      const url = api.getUrl("contratacion", "Contratos?tipoTramite=Oferta");
      this.axios.get(url).then(
        response => {
          this.Ofertas = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getEstadosFromApi() {
      const url = api.getUrl("contratacion", "contratos/Estados");
      this.axios.get(url).then(
        response => {
          this.estados = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getTiposFromApi() {
      const url = api.getUrl("contratacion", "contratos/Tipos");
      this.axios.get(url).then(
        response => {
          this.tipos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getEntidadesFromApi() {
      const url = api.getUrl("contratacion", "Entidades");
      this.axios.get(url).then(
        response => {
          this.entidades = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getEspecialistasExternosFromApi() {
      const url = api.getUrl("contratacion", "EspecialistasExternos");
      this.axios.get(url).then(
        response => {
          this.especialistasExternos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getFormasDePagosFromApi() {
      const url = api.getUrl("contratacion", "FormasDePagos");
      this.axios.get(url).then(
        response => {
          this.formasDePagos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getAdminContratosFromApi() {
      const url = api.getUrl("contratacion", "AdminContratos");
      this.axios.get(url).then(
        response => {
          this.adminContratos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getDetalles(item) {
      this.Oferta = Object.assign({}, item);
      this.dialog6 = true;
    },
    editItem(item) {
      this.editedIndex = this.Ofertas.indexOf(item);
      this.Oferta = Object.assign({}, item);
      this.dialog = true;
    },
    save(method) {
      if (method === "POST") {
        const url = api.getUrl("contratacion", "Contratos");
        this.axios.post(url, this.Oferta).then(
          response => {
            this.getResponse(response);
            this.getContratosFromApi();
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
      if (method === "PUT") {
        this.axios.put(`${url}/${this.Oferta.id}`, this.Oferta).then(
          response => {
            this.getResponse(response);
            this.getContratosFromApi();
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
    },
    confirmUpload(item) {
      this.Oferta = Object.assign({}, item);
      this.dialog7 = true;
    },
    upload() {
      const formData = new FormData();
      formData.append("file", this.file,);
      const url = api.getUrl("contratacion", "contratos/UploadFile");
      this.axios
        .post(url,formData, this.Oferta.id, {
          headers: {
            "Content-Type": "multipart/form-data"
          },
        })
        .then(
          response => {
            this.getResponse(response);
            this.getContratosFromApi();
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
    },
    confirmDelete(item) {
      this.Oferta = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(Oferta) {
      const url = api.getUrl("contratacion", "Contratos");
      this.axios.delete(`${url}/${Oferta.id}`).then(
        response => {
          this.getResponse(response);
          this.getContratosFromApi();
          this.dialog2 = false;
        },
        error => {
          console.log(error);
        }
      );
    },
    close() {
      this.dialog = false;
      this.dialog2 = false;
      this.dialog7 = false;
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    closeAdd() {
      this.getEntidadesFromApi();
      this.getEspecialistasExternosFromApi();
      this.getAdminContratosFromApi();
      this.dialog3 = false;
      this.dialog4 = false;
      this.dialog5 = false;
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    }
  }
};
</script>