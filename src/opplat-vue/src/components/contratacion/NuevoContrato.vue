<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex lg12>
        <h3>{{ formTitle }}</h3>
        <v-form ref="form">
          <v-container grid-list-md text-xs-center>
            <v-layout row wrap>
              <v-flex cols="2" md6 class="px-1">
                <v-text-field v-model="contrato.nombre" label="Nombre" clearable required></v-text-field>
              </v-flex>
              <v-flex cols="2" class="px-1" md1 v-if="editedIndex==-1">
                <v-autocomplete
                  v-model="contrato.tipo"
                  item-text="nombre"
                  item-value="id"
                  :items="tipos"
                  cache-items
                  label="tipo"
                ></v-autocomplete>
              </v-flex>
              <v-flex cols="2" md3 class="px-1" v-if="editedIndex!=-1">
                <v-autocomplete
                  v-model="contrato.tipo"
                  item-text="nombre"
                  item-value="id"
                  :items="tipos"
                  cache-items
                  label="tipo"
                ></v-autocomplete>
              </v-flex>
              <v-flex cols="2" class="px-1" v-if="editedIndex!=-1">
                <v-text-field v-model="contrato.numero" label="Número" prefix="#"></v-text-field>
              </v-flex>
            </v-layout>
            <v-layout row wrap>
              <v-flex cols="2" class="px-1">
                <v-autocomplete
                  v-model="contrato.adminContrato"
                  item-text="administrador.nombreCompleto"
                  item-value="administrador.id"
                  :items="adminContratos"
                  cache-items
                  label="Administrador"
                >
                  <v-icon @click="dialog4=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                </v-autocomplete>
              </v-flex>
              <v-flex cols="2" class="px-1">
                <v-autocomplete
                  v-model="contrato.entidad"
                  item-text="nombre"
                  item-value="id"
                  :items="entidades"
                  cache-items
                  label="Proveedor"
                >
                  <v-icon @click="dialog3=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                </v-autocomplete>
              </v-flex>
              <v-flex cols="2" class="px-1">
                <v-autocomplete
                  v-model="contrato.departamentos"
                  item-text="nombre"
                  item-value="id"
                  :items="departamentos"
                  cache-items
                  label="Departamentos que van a dictaminar la contrato"
                  multiple
                >
                  <template v-slot:selection="data">
                    <v-chip
                      v-bind="data.attrs"
                      :input-value="data.selected"
                      close
                      @click="data.select"
                      @click:close="removeDictaminadores(data.item)"
                      outlined
                    >{{ data.item.nombre }}</v-chip>
                  </template>
                  <template v-slot:item="data">
                    <template v-if="typeof data.item !== 'object'">
                      <v-list-item-content v-text="data.item"></v-list-item-content>
                    </template>
                    <template v-else>
                      <v-list-item-content>
                        <v-list-item-title v-html="data.item.nombre"></v-list-item-title>
                      </v-list-item-content>
                    </template>
                  </template>
                  <v-icon @click="dialog6=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                </v-autocomplete>
              </v-flex>
            </v-layout>
            <v-layout row wrap>
              <v-flex cols="2" class="px-1">
                <v-autocomplete
                  v-model="contrato.especialistasExternos"
                  item-text="nombreCompleto"
                  item-value="id"
                  :items="especialistasExternosAll"
                  cache-items
                  label="Especialistas Externos"
                  placeholder="Dictaminadores Externos"
                  multiple
                >
                  <v-icon @click="dialog5=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                </v-autocomplete>
              </v-flex>
            </v-layout>
            <v-layout row wrap>
              <v-flex cols="2" md3 class="px-1">
                <v-text-field
                  v-model="montoAndMoneda.cantidad"
                  label="Monto"
                  :error-messages="messagesCantidad"
                  clearable
                  prefix="$"
                ></v-text-field>
              </v-flex>
              <v-flex cols="2" md1 class="px-1">
                <v-select
                  v-model="montoAndMoneda.moneda"
                  item-text="nombre"
                  item-value="id"
                  :items="monedas"
                  :error-messages="messagesMoneda"
                  label="Moneda"
                ></v-select>
              </v-flex>
              <v-flex cols="2" md1 class="pt-7">
                <v-tooltip top color="success">
                  <template v-slot:activator="{ on }">
                    <v-icon medium v-on="on" @click="agregar()" color="success">mdi-plus</v-icon>
                  </template>
                  <span>Agregar</span>
                </v-tooltip>
              </v-flex>
              <v-flex cols="2" class="px-1">
                <v-chip-group multiple column active-class="primary--text">
                  <v-chip
                    v-for="item in montos"
                    :key="item.id"
                    outlined
                    close
                    @click:close="quitar(item)"
                  >
                    <v-icon left>mdi-cash-multiple</v-icon>
                    {{ item.cantidad }}
                    {{ item.nombreString }}
                  </v-chip>
                </v-chip-group>
              </v-flex>
            </v-layout>
            <v-layout row wrap>
              <v-flex cols="2" md="9" class="px-1">
                <v-autocomplete
                  v-model="contrato.formasDePago"
                  :items="formasDePagos"
                  label="Formas de Pago"
                  item-text="nombre"
                  item-value="id"
                  multiple
                >
                  <template v-slot:selection="data">
                    <v-chip
                      v-bind="data.attrs"
                      :input-value="data.selected"
                      close
                      @click="data.select"
                      @click:close="removeformasDePago(data.item)"
                      outlined
                    >{{ data.item.nombre }}</v-chip>
                  </template>
                  <template v-slot:item="data">
                    <template v-if="typeof data.item !== 'object'">
                      <v-list-item-content v-text="data.item"></v-list-item-content>
                    </template>
                    <template v-else>
                      <v-list-item-content>
                        <v-list-item-title v-html="data.item.nombre"></v-list-item-title>
                      </v-list-item-content>
                    </template>
                  </template>
                </v-autocomplete>
              </v-flex>
              <v-flex cols="2">
                <v-text-field
                  v-model="contrato.terminoDePago"
                  label="Término de Pago en Meses"
                  clearable
                  required
                ></v-text-field>
              </v-flex>
            </v-layout>
            <v-layout row wrap>
              <v-flex cols="2" class="px-1">
                <v-textarea v-model="contrato.objetoDeContrato" label="Objeto Social" rows="1"></v-textarea>
              </v-flex>
            </v-layout>
            <v-layout row wrap>
              <v-flex cols="2" class="px-1">
                <v-menu
                  v-model="menu"
                  :close-on-content-click="false"
                  :nudge-right="40"
                  transition="scale-transition"
                  offset-y
                  min-width="290px"
                >
                  <template v-slot:activator="{ on }">
                    <v-text-field
                      v-model="contrato.fechaDeRecepcion"
                      label="Fecha de Recepción"
                      readonly
                      clearable
                      v-on="on"
                      required
                    ></v-text-field>
                  </template>
                  <v-date-picker v-model="contrato.fechaDeRecepcion" @input="menu = false"></v-date-picker>
                </v-menu>
              </v-flex>
              <v-flex cols="2" class="px-1">
                <v-menu
                  v-model="menu1"
                  :close-on-content-click="false"
                  :nudge-right="40"
                  transition="scale-transition"
                  offset-y
                  min-width="290px"
                >
                  <template v-slot:activator="{ on }">
                    <v-text-field
                      v-model="contrato.fechaDeVenOferta"
                      label="Fecha de Vencimiento"
                      readonly
                      clearable
                      v-on="on"
                      required
                    ></v-text-field>
                  </template>
                  <v-date-picker v-model="contrato.fechaDeVenOferta" @input="menu1 = false"></v-date-picker>
                </v-menu>
              </v-flex>
              <!-- <v-flex cols="2" class="px-1">
                    <v-file-input v-model="contrato.file" show-size label="Seleccionar Documento"></v-file-input>
              </v-flex>-->
              <v-flex cols="2" class="px-1">
                <v-autocomplete
                  v-model="contrato.estado"
                  item-text="nombre"
                  item-value="id"
                  :items="estados"
                  cache-items
                  label="Estado"
                ></v-autocomplete>
              </v-flex>
            </v-layout>
          </v-container>
        </v-form>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
          <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
        </v-card-actions>
      </v-flex>
    </v-layout>
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
    <!-- Agregar Dictaminadores Contrato-->
    <v-dialog v-model="dialog6" persistent max-width="1000">
      <v-card>
        <v-toolbar dark fadeOnScroll color="blue darken-3">
          <v-spacer></v-spacer>
          <v-toolbar-items>
            <v-btn icon dark @click="closeAdd()">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <DictContratos></DictContratos>
      </v-card>
    </v-dialog>
    <!-- /Agregar Dictaminadores Contrato-->
  </v-container>
</template>
<script>
import api from "@/api";
import AdminContratos from "@/components/contratacion/AdminContratos.vue";
import EspExternos from "@/components/contratacion/EspExternos.vue";
import FormasDePago from "@/components/contratacion/FormasDePago.vue";
import Entidades from "@/components/contratacion/Entidades.vue";
import DictContratos from "@/components/contratacion/DictContratos.vue";

export default {
  props: ["contrato"],
  components: {
    AdminContratos,
    EspExternos,
    FormasDePago,
    Entidades,
    DictContratos
  },
  data: () => ({
    dialog3: false,
    dialog4: false,
    dialog5: false,
    dialog6: false,
    menu: false,
    menu1: false,
    search: "",
    editedIndex: -1,
    ofertas: [],
    cantOfertas: 0,
    file: null,
    entidades: [],
    entidadObjetc: {},
    especialistasExternosAll: [],
    dictaminadoresContratos: [],
    adminContratos: [],
    estados: [],
    tipos: [],
    formasDePagos: [],
    departamentos: [],
    monedas: [],
    trabajadores: [],
    montoAndMoneda: {
      cantidad: null,
      moneda: null,
      nombreString: null
    },
    montos: [],
    errors: [],
    roles: null,
    title: "",
    messagesMoneda: "",
    messagesCantidad: "",
    MonedaRules: [v => !!v || "Faltan datos por llenar"],
    MontoRules: [v => !!v || "Faltan datos por llenar"]
  }),
  computed: {
    formTitle() {
      if (this.contrato.id != null) {
        this.editedIndex = this.contrato.id;
      }
      return this.editedIndex === -1 ? "Nueva Oferta" : "Editar Oferta";
    },
    method() {
      return this.editedIndex === -1 ? "POST" : "PUT";
    }
    // monto() {
    //   if (this.contrato.entidad != 0) {
    //     this.entidadObjetc = this.entidades.find(
    //       x => x.id === this.contrato.entidad
    //     );
    //   }
    // }
  },
  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {
    this.getOfertasFromApi();
    this.getEstadosFromApi();
    this.getTiposFromApi();
    this.getEntidadesFromApi();
    this.getEspecialistasExternosFromApi();
    this.getAdminContratosFromApi();
    this.getDictContratosFromApi();
    this.getFormasDePagosFromApi();
    this.getTrabajadoresFromApi();
    this.getTiempoVenOfertasFromApi();
    this.getDepartamentosFromApi();
    this.getMonedasFromApi();
    this.roles = this.$store.getters.roles;
    this.montos = this.contrato.montos;
  },

  methods: {
    getOfertasFromApi() {
      const url = api.getUrl(
        "contratacion",
        "Contratos?tipoTramite=contrato&cliente=true"
      );
      this.axios.get(url).then(
        response => {
          this.textByfiltro = "Ofertas de los Clientes";
          this.ofertas = response.data;
          this.cantOfertas = this.ofertas.length;
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
          this.especialistasExternosAll = response.data;
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
    getDictContratosFromApi() {
      const url = api.getUrl("contratacion", "DictContratos");
      this.axios.get(url).then(
        response => {
          this.dictaminadoresContratos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getTrabajadoresFromApi() {
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      this.axios.get(url).then(
        response => {
          this.trabajadores = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getTiempoVenOfertasFromApi() {
      const url = api.getUrl("contratacion", "TiempoVenOfertas");
      this.axios.get(url).then(
        response => {
          this.tiempoVenOfertas = response.data[0];
        },
        error => {
          console.log(error);
        }
      );
    },
    getDepartamentosFromApi() {
      const url = api.getUrl("contratacion", "Departamentos");
      this.axios.get(url).then(
        response => {
          this.departamentos = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getMonedasFromApi() {
      const url = api.getUrl("contratacion", "Entidades/Monedas");
      this.axios.get(url).then(
        response => {
          this.monedas = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    save(method) {
      const url = api.getUrl("contratacion", "Contratos");
      if (method === "POST") {
        if (
          this.montoAndMoneda.cantidad != null &&
          this.montoAndMoneda.moneda != null
        ) {
          this.montos.push(this.montoAndMoneda);
          this.contrato.montos = this.montos;
          if (this.$refs.form.validate()) {
            this.axios.post(url, this.contrato).then(
              response => {
                this.getResponse(response);
                if (this.contrato.cliente == true) {
                  this.$router.push({
                    name: "OfertasClientes"
                  });
                } else
                  this.$router.push({
                    name: "OfertasPrestador"
                  });
              },
              error => {
                vm.$snotify.error(error.response.data);
                console.log(error);
              }
            );
          }
        } else if (this.montoAndMoneda.cantidad == null) {
          this.messagesCantidad = "Faltan datos por llenar";
        } else if (this.montoAndMoneda.moneda == null) {
          this.messagesMoneda = "Faltan datos por llenar";
        }
      }
      if (method === "PUT") {
        if (
          this.montoAndMoneda.cantidad == null &&
          this.montoAndMoneda.moneda == null
        ) {
          if (
            this.montoAndMoneda.cantidad != null &&
            this.montoAndMoneda.moneda != null
          ) {
            this.montos.push(this.montoAndMoneda);
          }
          this.contrato.montos = this.montos;
          const url = api.getUrl("contratacion", "Contratos");
          this.axios
            .put(`${url}/${this.contrato.id}`, this.contrato)
            .then(
              response => {
                this.getResponse(response);
                if (this.contrato.cliente == true) {
                  this.$router.push({
                    name: "OfertasClientes"
                  });
                } else
                  this.$router.push({
                    name: "OfertasPrestador"
                  });
              },
              error => {
                console.log(error);
              }
            )
            .catch(e => {
              vm.$snotify.error(e.response.data.errors);
            });
        } else if (this.montoAndMoneda.cantidad == null) {
          this.messagesCantidad = "Faltan datos por llenar";
        } else if (this.montoAndMoneda.moneda == null) {
          this.messagesMoneda = "Faltan datos por llenar";
        }
      }
    },
    close() {
      if (this.contrato.cliente == true) {
        this.$router.push({
          name: "OfertasClientes"
        });
      } else
        this.$router.push({
          name: "OfertasPrestador"
        });
    },
    closeAdd() {
      this.getEntidadesFromApi();
      this.getEspecialistasExternosFromApi();
      this.getAdminContratosFromApi();
      this.getDictContratosFromApi();
      this.getDepartamentosFromApi();
      this.dialog3 = false;
      this.dialog4 = false;
      this.dialog5 = false;
      this.dialog6 = false;
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    removeformasDePago(item) {
      const index = this.contrato.formasDePago.indexOf(item.id);
      if (index >= 0) this.contrato.formasDePago.splice(index, 1);
    },
    removeDictaminadores(item) {
      const index = this.contrato.dictaminadores.indexOf(item.id);
      if (index >= 0) this.contrato.dictaminadores.splice(index, 1);
    },
    agregar() {
      if (
        this.montoAndMoneda.cantidad != null &&
        this.montoAndMoneda.moneda != null
      ) {
        const m = this.monedas.find(x => x.id === this.montoAndMoneda.moneda);
        this.montoAndMoneda.nombreString = m.nombre;
        this.montos.push(this.montoAndMoneda);
        this.montoAndMoneda = {
          cantidad: null,
          moneda: null,
          nombreString: null
        };
        this.messagesMoneda = "";
        this.messagesCantidad = "";
      } else if (this.montoAndMoneda.cantidad == null) {
        this.messagesCantidad = "Faltan datos por llenar";
      } else if (this.montoAndMoneda.moneda == null) {
        this.messagesMoneda = "Faltan datos por llenar";
      }
    },
    quitar(item) {
      this.montos.splice(this.montos.indexOf(item), 1);
    },
    aprobarOferta(item) {}
  }
};
</script>