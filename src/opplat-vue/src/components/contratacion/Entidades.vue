<template>
  <v-data-table :headers="headers" :items="entidades" :search="search" class="elevation-1 pa-5">
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Proveedores</v-toolbar-title>
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
        <!-- Agregar y Editar Entidad -->
        <v-dialog v-model="dialog" persistent max-width="1000">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on">Nuevo Proveedor</v-btn>
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
                  <v-flex xs3 class="px-2">
                    <v-text-field
                      v-model="entidad.nombre"
                      label="Nombre"
                      :rules="NombreRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex 6 class="px-2">
                    <v-text-field
                      v-model="entidad.direccion"
                      label="Direccion"
                      placeholder="Calle e/ #, Barrio o Finca, Municipio y Provincia"
                      :rules="DireccionRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-2">
                    <v-text-field
                      v-model="entidad.nit"
                      label="NIT"
                      :rules="NitRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex xs3 class="px-2">
                    <v-autocomplete
                      v-model="entidad.sectorId"
                      item-text="nombre"
                      item-value="id"
                      :items="sectores"
                      :filter="activeFilter"
                      label="Sector"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs3 class="px-2">
                    <v-text-field v-model="entidad.fax" label="Fax" clearable></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-2">
                    <v-text-field
                      v-model="entidad.correo"
                      label="Correo"
                      clearable
                      :rules="emailRules"
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <!-- Agregar Número de Teléfono del Proveedor  -->
                <v-row class="pl-3" justify="start">
                  <v-flex cols="2" class="pt-2">
                    <v-card tile flat>
                      <v-tooltip top color="success">
                        <template v-slot:activator="{ on }">
                          <v-icon medium v-on="on" @click="agregar(telef)" color="success">mdi-plus</v-icon>
                        </template>
                        <span>Agregar Número de Teléfono</span>
                      </v-tooltip>
                    </v-card>
                    <v-card tile flat>
                      <v-tooltip top color="red darken-1">
                        <template v-slot:activator="{ on }">
                          <v-icon medium v-on="on" @click="quitar(telef)" color="red">mdi-minus</v-icon>
                        </template>
                        <span>Quitar Número de Teléfono</span>
                      </v-tooltip>
                    </v-card>
                  </v-flex>
                  <v-flex cols="2" class="pr-3" v-if="cantTelefonos>=1">
                    <v-text-field
                      v-model="entidad.telefonos[0].numero"
                      label="Teléfono 1"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-3" v-if="cantTelefonos>=1">
                    <v-text-field
                      v-model="entidad.telefonos[0].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-3" v-if="cantTelefonos>=2">
                    <v-text-field
                      v-model="entidad.telefonos[1].numero"
                      label="Teléfono 2"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-6" v-if="cantTelefonos>=2">
                    <v-text-field
                      v-model="entidad.telefonos[1].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pl-12 pr-3" v-if="cantTelefonos>=3">
                    <v-text-field
                      v-model="entidad.telefonos[2].numero"
                      label="Teléfono 3"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-3" v-if="cantTelefonos>=3">
                    <v-text-field
                      v-model="entidad.telefonos[2].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-3" v-if="cantTelefonos>=4">
                    <v-text-field
                      v-model="entidad.telefonos[3].numero"
                      label="Teléfono 4"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-6" v-if="cantTelefonos>=4">
                    <v-text-field
                      v-model="entidad.telefonos[3].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pl-12 pr-3" v-if="cantTelefonos>=5">
                    <v-text-field
                      v-model="entidad.telefonos[4].numero"
                      label="Teléfono 5"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-3" v-if="cantTelefonos>=5">
                    <v-text-field
                      v-model="entidad.telefonos[4].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-3" v-if="cantTelefonos>=6">
                    <v-text-field
                      v-model="entidad.telefonos[5].numero"
                      label="Teléfono 6"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-6" v-if="cantTelefonos>=6">
                    <v-text-field
                      v-model="entidad.telefonos[5].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                </v-row>
                <!--/Agregar Número de Teléfono del Proveedor  -->

                <!-- Agregar Cuenta Bancaria del Proveedor -->
                <v-flex>
                  <v-row class="pl-3 pr-3">
                    <v-flex cols="2" class="pt-2">
                      <v-card tile flat>
                        <v-tooltip top color="success">
                          <template v-slot:activator="{ on }">
                            <v-icon
                              medium
                              v-on="on"
                              @click="agregar(cuent)"
                              color="success"
                            >mdi-plus</v-icon>
                          </template>
                          <span>Agregar Cuenta</span>
                        </v-tooltip>
                      </v-card>
                      <v-card tile flat>
                        <v-tooltip top color="red darken-1">
                          <template v-slot:activator="{ on }">
                            <v-icon medium v-on="on" @click="quitar(cuent)" color="red">mdi-minus</v-icon>
                          </template>
                          <span>Quitar Cuenta</span>
                        </v-tooltip>
                      </v-card>
                    </v-flex>
                    <v-flex cols="2" class="pr-3" v-if="cantCuentas>=1">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[0].numeroCuenta"
                        label="Número de cuenta 1"
                        clearable
                        required
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="2" class="pr-3" v-if="cantCuentas>=1">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[0].numeroSucursal"
                        label="Número Sucursal"
                        clearable
                        required
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="2" class="pr-3" v-if="cantCuentas>=1">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[0].nombreSucursalId"
                        item-text="nombre"
                        item-value="id"
                        :items="nombreSuces"
                        :filter="activeFilter"
                        label="Nombre Sucursal"
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex cols="2" class="pr-3" v-if="cantCuentas>=1">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[0].monedaId"
                        item-text="nombre"
                        item-value="id"
                        :items="monedas"
                        :filter="activeFilter"
                        label="Moneda"
                      ></v-autocomplete>
                    </v-flex>
                  </v-row>
                  <v-row class="pl-9 pr-3" justify="space-around" no-gutters v-if="cantCuentas>=2">
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[1].numeroCuenta"
                        label="Número de cuenta 2"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[1].numeroSucursal"
                        label="Número Sucursal"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[1].nombreSucursalId"
                        item-text="nombre"
                        item-value="id"
                        :items="nombreSuces"
                        :filter="activeFilter"
                        label="Nombre Sucursal"
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[1].monedaId"
                        item-text="nombre"
                        item-value="id"
                        :items="monedas"
                        :filter="activeFilter"
                        label="Moneda"
                      ></v-autocomplete>
                    </v-flex>
                  </v-row>
                  <v-row class="pl-9 pr-3" justify="space-around" no-gutters v-if="cantCuentas>=3">
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[2].numeroCuenta"
                        label="Número de cuenta 3"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[2].numeroSucursal"
                        label="Número Sucursal"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[2].nombreSucursalId"
                        item-text="nombre"
                        item-value="id"
                        :items="nombreSuces"
                        :filter="activeFilter"
                        label="Nombre Sucursal"
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[2].monedaId"
                        item-text="nombre"
                        item-value="id"
                        :items="monedas"
                        :filter="activeFilter"
                        label="Moneda"
                      ></v-autocomplete>
                    </v-flex>
                  </v-row>
                  <v-row class="pl-9 pr-3" justify="space-around" no-gutters v-if="cantCuentas>=4">
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[3].numeroCuenta"
                        label="Número de cuenta 4"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[3].numeroSucursal"
                        label="Número Sucursal"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[3].nombreSucursalId"
                        item-text="nombre"
                        item-value="id"
                        :items="nombreSuces"
                        :filter="activeFilter"
                        label="Nombre Sucursal"
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[3].monedaId"
                        item-text="nombre"
                        item-value="id"
                        :items="monedas"
                        :filter="activeFilter"
                        label="Moneda"
                      ></v-autocomplete>
                    </v-flex>
                  </v-row>
                  <v-row class="pl-9 pr-3" justify="space-around" no-gutters v-if="cantCuentas>=5">
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[4].numeroCuenta"
                        label="Número de cuenta 5"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[4].numeroSucursal"
                        label="Número Sucursal"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[4].nombreSucursalId"
                        item-text="nombre"
                        item-value="id"
                        :items="nombreSuces"
                        :filter="activeFilter"
                        label="Nombre Sucursal"
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[4].monedaId"
                        item-text="nombre"
                        item-value="id"
                        :items="monedas"
                        :filter="activeFilter"
                        label="Moneda"
                      ></v-autocomplete>
                    </v-flex>
                  </v-row>
                  <v-row class="pl-9 pr-3" justify="space-around" no-gutters v-if="cantCuentas>=6">
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[5].numeroCuenta"
                        label="Número de cuenta 6"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-text-field
                        v-model="entidad.cuentasBancarias[5].numeroSucursal"
                        label="Número Sucursal"
                        clearable
                      ></v-text-field>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[5].nombreSucursalId"
                        item-text="nombre"
                        item-value="id"
                        :items="nombreSuces"
                        :filter="activeFilter"
                        label="Nombre Sucursal"
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex cols="3" class="px-2">
                      <v-autocomplete
                        v-model="entidad.cuentasBancarias[5].monedaId"
                        item-text="nombre"
                        item-value="id"
                        :items="monedas"
                        :filter="activeFilter"
                        label="Moneda"
                      ></v-autocomplete>
                    </v-flex>
                  </v-row>
                </v-flex>
                <!-- /Agregar Cuenta Bancaria del Proveedor -->
              </v-container>
            </v-form>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Agregar y Editar Entidad -->

        <!-- Detalles de la Entidad -->
        <v-dialog
          v-model="dialog3"
          persistent
          transition="dialog-bottom-transition"
          flat
          max-width="1100"
        >
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles del Proveedor</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-container fluid>
              <v-row dense>
                <v-col cols="8">
                  <v-row>
                    <v-col cols="6" class="px-1">
                      <v-text-field
                        v-model="entidad.nombre"
                        label="Nombre del Proveedor"
                        outlined
                        readonly
                      ></v-text-field>
                    </v-col>
                    <v-col cols="6" class="px-1">
                      <v-text-field v-model="entidad.nit" label="NIT" outlined readonly></v-text-field>
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col cols="9" class="px-1">
                      <v-text-field v-model="entidad.direccion" label="Dirección" outlined readonly></v-text-field>
                    </v-col>
                    <v-col cols="3" class="px-1">
                      <v-text-field v-model="entidad.sector" label="Sector" outlined readonly></v-text-field>
                    </v-col>
                  </v-row>
                  <v-row
                    class="mt-1"
                    v-for="item in entidad.cuentasBancarias"
                    :key="item.numeroCuenta"
                  >
                    <v-flex md4 class="px-1">
                      <v-text-field
                        v-model="item.numeroCuenta"
                        label="Número de cuenta"
                        outlined
                        readonly
                      ></v-text-field>
                    </v-flex>
                    <v-flex md3 class="px-1">
                      <v-text-field
                        v-model="item.numeroSucursal"
                        label="Número Sucursal"
                        outlined
                        readonly
                      ></v-text-field>
                    </v-flex>
                    <v-flex md3 class="px-1">
                      <v-text-field
                        v-model="item.nombreSucursal"
                        label="Nombre Sucursal"
                        outlined
                        readonly
                      ></v-text-field>
                    </v-flex>
                    <v-flex md2 class="px-1">
                      <v-text-field v-model="item.moneda" label="Moneda" outlined readonly></v-text-field>
                    </v-flex>
                  </v-row>
                </v-col>
                <!-- Contactos de la Entidad -->
                <v-col cols="4" class="pt-4 pl-3">
                  <v-card max-width="400" color="#385F73" :elevation="8" dark flat>
                    <v-card-text class="white--text">
                      <div class="headline mb-2">Contactos</div>
                      <div class="mt-1" v-if="entidad.correo!=''">Correo: {{entidad.correo}}</div>
                      <div class="mt-1" v-if="entidad.fax!=null">Fax: {{entidad.fax}}</div>
                      <div v-if="cantTelefonos!=0">
                        <div
                          v-for="item in entidad.telefonos"
                          :key="item.numero"
                          class="mt-1"
                        >Telefóno: {{item.numero}} Ext: {{item.extension}}</div>
                      </div>
                    </v-card-text>
                  </v-card>
                </v-col>
                <!-- Contactos de la Entidad -->
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- /Detalles de la Entidad -->

        <!-- Delete Entidad -->
        <v-dialog v-model="dialog2" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="close()">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas eliminar la Entidad</v-card-title>
            <v-card-text class="text-center">{{entidad.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(entidad)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Entidad -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-tooltip top color="success">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="editItem(item)">mdi-pencil</v-icon>
        </template>
        <span>Editar</span>
      </v-tooltip>
      <v-tooltip top color="green darken-4">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="getDetalles(item)">mdi-file-document-box-plus</v-icon>
        </template>
        <span>Detalles</span>
      </v-tooltip>
      <v-tooltip top color="red darken-1">
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="confirmDelete(item)">mdi-trash-can</v-icon>
        </template>
        <span>Eliminar</span>
      </v-tooltip>
    </template>
  </v-data-table>
</template>
<script>
import api from "@/api";

export default {
  data: () => ({
    justify: ["start"],
    dialog: false,
    dialog1: false,
    dialog2: false,
    dialog3: false,
    search: "",
    editedIndex: -1,
    entidades: [],
    nombreSuces: [],
    monedas: [],
    sectores: [],
    entidad: {
      nombre: null,
      direccion: null,
      nit: null,
      correo: null,
      sector: null,
      cuentasBancarias: [
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursalId: null,
          monedaId: null
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursalId: null,
          monedaId: null
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursalId: null,
          monedaId: null
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursalId: null,
          monedaId: null
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursalId: null,
          monedaId: null
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursalId: null,
          monedaId: null
        }
      ],
      telefonos: [
        { numero: null, extension: null },
        { numero: null, extension: null },
        { numero: null, extension: null },
        { numero: null, extension: null },
        { numero: null, extension: null },
        { numero: null, extension: null }
      ]
    },
    cantCuentas: 1,
    cuent: "cuentas",
    cantTelefonos: 1,
    telef: "telefonos",
    tabs: null,
    emailRules: [v => /.+@.+\..+/.test(v) || "No tiene la estructura correcta"],
    NombreRules: [v => !!v || "El Nombre es Requerido"],
    DireccionRules: [v => !!v || "La Dirección es Requerida"],
    NitRules: [v => !!v || "El NIT es Requerido"],
    errors: [],
    headers: [
      {
        text: "Nombre",
        align: "left",
        sortable: true,
        value: "nombre"
      },
      { text: "Dirección", value: "direccion" },
      { text: "NIT", value: "nit" },
      { text: "Correo", value: "correo" },
      { text: "Sector", value: "sector" },
      { text: "Acciones", value: "action", sortable: false }
    ],
    date: null
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nuevo Proveedor" : "Editar Proveedor";
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
    this.getEntidadesFromApi();
    this.getnombreSucesFromApi();
    this.getMonedasFromApi();
    this.getSectoresFromApi();
  },

  methods: {
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
    getnombreSucesFromApi() {
      const url = api.getUrl("contratacion", "Entidades/Sucursales");
      this.axios.get(url).then(
        response => {
          this.nombreSuces = response.data;
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
    getSectoresFromApi() {
      const url = api.getUrl("contratacion", "Entidades/Sectores");
      this.axios.get(url).then(
        response => {
          this.sectores = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    editItem(item) {
      this.editedIndex = this.entidades.indexOf(item);
      this.entidad = Object.assign({}, item);
      this.cantTelefonos = item.cantTelefonos;
      this.cantCuentas = item.cantCuentasBancarias;
      this.dialog = true;
      if (this.cantTelefonos == 0) {
        this.cantTelefonos = 1;
        this.entidad.telefonos = [
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null }
        ];
      }
      if (this.cantTelefonos == 1) {
        this.entidad.telefonos = [
          {
            numero: item.telefonos[0].numero,
            extension: item.telefonos[0].extension
          },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null }
        ];
      }
      if (this.cantTelefonos == 2) {
        this.entidad.telefonos = [
          {
            numero: item.telefonos[0].numero,
            extension: item.telefonos[0].extension
          },
          {
            numero: item.telefonos[1].numero,
            extension: item.telefonos[1].extension
          },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null }
        ];
      }
    },
    save(method) {
      const url = api.getUrl("contratacion", "entidades");
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          if (
            this.entidad.cuentasBancarias[0].numeroCuenta == null ||
            this.entidad.cuentasBancarias[0].numeroSucursal == null ||
            this.entidad.cuentasBancarias[0].nombreSucursalId == null ||
            this.entidad.cuentasBancarias[0].monedaId == null
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #1 por Llenar");
          }
          if (
            (this.cantCuentas >= 2 &&
              this.entidad.cuentasBancarias[1].numeroCuenta == null) ||
            (this.cantCuentas >= 2 &&
              this.entidad.cuentasBancarias[1].numeroSucursal == null) ||
            (this.cantCuentas >= 2 &&
              this.entidad.cuentasBancarias[1].nombreSucursalId == null) ||
            (this.cantCuentas >= 2 &&
              this.entidad.cuentasBancarias[1].monedaId == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #2 por Llenar");
          }
          if (
            (this.cantCuentas >= 3 &&
              this.entidad.cuentasBancarias[2].numeroCuenta == null) ||
            (this.cantCuentas >= 3 &&
              this.entidad.cuentasBancarias[2].numeroSucursal == null) ||
            (this.cantCuentas >= 3 &&
              this.entidad.cuentasBancarias[2].nombreSucursalId == null) ||
            (this.cantCuentas >= 3 &&
              this.entidad.cuentasBancarias[2].monedaId == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #3 por Llenar");
          }
          if (
            (this.cantCuentas >= 4 &&
              this.entidad.cuentasBancarias[3].numeroCuenta == null) ||
            (this.cantCuentas >= 4 &&
              this.entidad.cuentasBancarias[3].numeroSucursal == null) ||
            (this.cantCuentas >= 4 &&
              this.entidad.cuentasBancarias[3].nombreSucursalId == null) ||
            (this.cantCuentas >= 4 &&
              this.entidad.cuentasBancarias[3].monedaId == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #4 por Llenar");
          }
          if (
            (this.cantCuentas >= 5 &&
              this.entidad.cuentasBancarias[4].numeroCuenta == null) ||
            (this.cantCuentas >= 5 &&
              this.entidad.cuentasBancarias[4].numeroSucursal == null) ||
            (this.cantCuentas >= 5 &&
              this.entidad.cuentasBancarias[4].nombreSucursalId == null) ||
            (this.cantCuentas >= 5 &&
              this.entidad.cuentasBancarias[4].monedaId == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #5 por Llenar");
          }
          if (
            (this.cantCuentas >= 6 &&
              this.entidad.cuentasBancarias[5].numeroCuenta == null) ||
            (this.cantCuentas >= 6 &&
              this.entidad.cuentasBancarias[5].numeroSucursal == null) ||
            (this.cantCuentas >= 6 &&
              this.entidad.cuentasBancarias[5].nombreSucursalId == null) ||
            (this.cantCuentas >= 6 &&
              this.entidad.cuentasBancarias[5].monedaId == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #6 por Llenar");
          } else {
            this.axios.post(url, this.entidad).then(
              response => {
                this.getResponse(response);
                this.resetDatos();
                this.dialog = false;
              },
              error => {
                console.log(error);
              }
            );
          }
        } else {
          vm.$snotify.error("Faltan campos por llenar que son obligatorios");
        }
      }
      if (method === "PUT") {
        this.axios.put(`${url}/${this.entidad.id}`, this.entidad).then(
          response => {
            this.getResponse(response);
            this.resetDatos();
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
    },
    resetDatos() {
      this.getEntidadesFromApi();
      this.cantTelefonos = 1;
      this.cantCuentas = 1;
      this.entidad = {
        nombre: null,
        direccion: null,
        nit: null,
        correo: null,
        sector: null,
        cuentasBancarias: [
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursalId: null,
            monedaId: null
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursalId: null,
            monedaId: null
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursalId: null,
            monedaId: null
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursalId: null,
            monedaId: null
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursalId: null,
            monedaId: null
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursalId: null,
            monedaId: null
          }
        ],
        telefonos: [
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null }
        ]
      };
    },
    getDetalles(item) {
      this.entidad = Object.assign({}, item);
      this.cantTelefonos = item.cantTelefonos;
      this.dialog3 = true;
    },
    confirmDelete(item) {
      this.entidad = Object.assign({}, item);
      this.cantTelefonos = item.cantTelefonos;
      this.dialog2 = true;
    },
    deleteItem(entidad) {
      const url = api.getUrl("contratacion", "entidades");
      this.axios.delete(`${url}/${entidad.id}`).then(
        response => {
          this.getResponse(response);
          this.getEntidadesFromApi();
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
      this.dialog3 = false;
      this.resetDatos();
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    agregar(string) {
      if (string == "telefonos") {
        if (this.cantTelefonos < 6) {
          this.cantTelefonos += 1;
        } else {
          vm.$snotify.error("No se Pueden Agregar más de 6 Teléfonos");
        }
      }
      if (string == "cuentas") {
        if (this.cantCuentas < 6) {
          this.cantCuentas += 1;
        } else {
          vm.$snotify.error("No se Pueden Agregar más de 6 Cuentas");
        }
      }
    },
    quitar(string) {
      if (string == "telefonos") {
        if (this.cantTelefonos > 0) {
          this.cantTelefonos -= 1;
        } else {
          vm.$snotify.error("No se Pueden Quitar más Teléfonos");
        }
      }
      if (string == "cuentas") {
        if (this.cantCuentas > 0) {
          this.cantCuentas -= 1;
        } else {
          vm.$snotify.error("No se Pueden Quitar más Teléfonos");
        }
      }
    }
  }
};
</script>