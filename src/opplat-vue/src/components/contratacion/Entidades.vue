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
        <v-dialog v-model="dialog" persistent max-width="1200">
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
            <v-form ref="form">
              <v-container grid-list-md text-xs-center>
                <p class="text-left title">Datos Generales:</p>
                <v-layout row wrap>
                  <v-flex cols="2" class="px-2">
                    <v-text-field
                      v-model="entidad.nombre"
                      label="Nombre"
                      :rules="NombreRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-2">
                    <v-text-field v-model="entidad.siglas" label="Siglas" clearable></v-text-field>
                  </v-flex>
                  <v-flex cols="2" col-md-6 class="px-2">
                    <v-text-field
                      v-model="entidad.direccion"
                      label="Direccion"
                      placeholder="Calle e/ #, Barrio o Finca, Municipio y Provincia"
                      :rules="DireccionRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex cols="2" class="px-2">
                    <v-text-field v-model="entidad.codigo" label="Código" clearable></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-2">
                    <v-text-field
                      v-model="entidad.nit"
                      label="NIT"
                      :error-messages="textNit"
                      :rules="NitRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="pr-1 pl-2">
                    <v-select
                      v-model="entidad.sector"
                      item-text="nombre"
                      item-value="id"
                      :items="sectores"
                      label="Sector"
                    ></v-select>
                  </v-flex>
                  <v-col cols="12" sm="12" class="px-1">
                    <v-textarea v-model="entidad.objetoSocial" label="Objeto Social" rows="1"></v-textarea>
                  </v-col>
                </v-layout>

                <!-- Telefonos Agregados -->
                <!-- <v-row justify="start">
                  <v-col cols="12" sm="12" class="px-3">
                    <v-data-table
                      :headers="headerstelefonos"
                      :items="telefonos"
                      hide-default-footer
                      fixed-header
                    ></v-data-table>
                  </v-col>
                </v-row>-->
                <!-- /Telefonos Agregados -->

                <!-- Agregar y Editar Número de Teléfono del Proveedor  -->
                <p class="text-left title px-1">Datos de Contacto:</p>
                <v-row class="px-1" justify="start">
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.correo"
                      label="Correo"
                      clearable
                      :rules="emailRules"
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field v-model="entidad.fax" label="Fax" clearable></v-text-field>
                  </v-flex>
                </v-row>
                <v-row class="px-1" justify="start" v-if="cantTelefonos>=2">
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.telefonos[0].numero"
                      label="Teléfono"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.telefonos[0].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.telefonos[1].numero"
                      label="Teléfono 2"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=2">
                    <v-text-field
                      v-model="entidad.telefonos[1].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs1 class="pt-2">
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
                </v-row>
                <v-row class="px-1" justify="start">
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=3">
                    <v-text-field
                      v-model="entidad.telefonos[2].numero"
                      label="Teléfono 3"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=3">
                    <v-text-field
                      v-model="entidad.telefonos[2].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=4">
                    <v-text-field
                      v-model="entidad.telefonos[3].numero"
                      label="Teléfono 4"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=4">
                    <v-text-field
                      v-model="entidad.telefonos[3].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs1 class="pt-2"></v-flex>
                </v-row>
                <v-row class="px-1" justify="start" v-if="cantTelefonos>=3">
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=5">
                    <v-text-field
                      v-model="entidad.telefonos[4].numero"
                      label="Teléfono 5"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=5">
                    <v-text-field
                      v-model="entidad.telefonos[4].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=6">
                    <v-text-field
                      v-model="entidad.telefonos[5].numero"
                      label="Teléfono 6"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1" v-if="cantTelefonos>=6">
                    <v-text-field
                      v-model="entidad.telefonos[5].extension"
                      label="Extensión"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs1 class="pt-2"></v-flex>
                </v-row>
                <!--/Agregar y Editar Número de Teléfono del Proveedor  -->
                <p class="text-left title">Datos Bancarios:</p>

                <!-- Agregar y Editar Cuenta Bancaria del Proveedor -->
                <v-row class="px-1" justify="start" v-if="cantCuentas>=1">
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[0].numeroCuenta"
                      :error-messages="modelstate['numeroCuenta']"
                      label="Número de cuenta 1"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[0].numeroSucursal"
                      label="Número Sucursal"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[0].nombreSucursal"
                      item-text="nombre"
                      item-value="id"
                      :items="nombreSuces"
                      label="Nombre Sucursal"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[0].moneda"
                      item-text="nombre"
                      item-value="id"
                      :items="monedas"
                      label="Moneda"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs1 class="pt-2">
                    <v-card tile flat>
                      <v-tooltip top color="success">
                        <template v-slot:activator="{ on }">
                          <v-icon medium v-on="on" @click="agregar(cuent)" color="success">mdi-plus</v-icon>
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
                </v-row>
                <v-row class="px-1" justify="start" v-if="cantCuentas>=2">
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[1].numeroCuenta"
                      label="Número de cuenta 2"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[1].numeroSucursal"
                      label="Número Sucursal"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[1].nombreSucursal"
                      item-text="nombre"
                      item-value="id"
                      :items="nombreSuces"
                      label="Nombre Sucursal"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[1].moneda"
                      item-text="nombre"
                      item-value="id"
                      :items="monedas"
                      label="Moneda"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs1 class="pt-2"></v-flex>
                </v-row>
                <v-row class="px-1" justify="start" v-if="cantCuentas>=3">
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[2].numeroCuenta"
                      label="Número de cuenta 3"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[2].numeroSucursal"
                      label="Número Sucursal"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[2].nombreSucursal"
                      item-text="nombre"
                      item-value="id"
                      :items="nombreSuces"
                      label="Nombre Sucursal"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[2].moneda"
                      item-text="nombre"
                      item-value="id"
                      :items="monedas"
                      label="Moneda"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs1 class="pt-2"></v-flex>
                </v-row>
                <v-row class="px-1" justify="start" v-if="cantCuentas>=4">
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[3].numeroCuenta"
                      label="Número de cuenta 4"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[3].numeroSucursal"
                      label="Número Sucursal"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[3].nombreSucursal"
                      item-text="nombre"
                      item-value="id"
                      :items="nombreSuces"
                      label="Nombre Sucursal"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[3].moneda"
                      item-text="nombre"
                      item-value="id"
                      :items="monedas"
                      label="Moneda"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs1 class="pt-2"></v-flex>
                </v-row>
                <v-row class="px-1" justify="start" v-if="cantCuentas>=5">
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[4].numeroCuenta"
                      label="Número de cuenta 5"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[4].numeroSucursal"
                      label="Número Sucursal"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[4].nombreSucursal"
                      item-text="nombre"
                      item-value="id"
                      :items="nombreSuces"
                      label="Nombre Sucursal"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[4].moneda"
                      item-text="nombre"
                      item-value="id"
                      :items="monedas"
                      label="Moneda"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs1 class="pt-2"></v-flex>
                </v-row>
                <v-row class="px-1" justify="start" v-if="cantCuentas>=6">
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[5].numeroCuenta"
                      label="Número de cuenta 6"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-text-field
                      v-model="entidad.cuentasBancarias[5].numeroSucursal"
                      label="Número Sucursal"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[5].nombreSucursal"
                      item-text="nombre"
                      item-value="id"
                      :items="nombreSuces"
                      label="Nombre Sucursal"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex cols="2" class="px-1">
                    <v-autocomplete
                      v-model="entidad.cuentasBancarias[5].moneda"
                      item-text="nombre"
                      item-value="id"
                      :items="monedas"
                      label="Moneda"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs1 class="pt-2"></v-flex>
                </v-row>
                <v-flex xs6 class="px-1 pt-3">
                  <v-alert
                    v-if="errorController!=null"
                    border="left"
                    color="red"
                    dark
                  >{{ errorController }}</v-alert>
                </v-flex>
                <!-- /Agregar y Editar Cuenta Bancaria del Proveedor -->
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
          max-width="1000"
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
                  <v-card flat>
                    <v-row no-gutters justify="start">
                      <v-col cols="12" md="6" class="pa-2">
                        <v-list-item-title>
                          <strong>Nombre del Proveedor</strong>
                        </v-list-item-title>
                        <v-list-item-subtitle>{{entidad.nombre}}</v-list-item-subtitle>
                      </v-col>
                      <v-col cols="12" md="3" class="pa-2">
                        <v-list-item-title>
                          <strong>Siglas</strong>
                        </v-list-item-title>
                        <v-list-item-subtitle>{{entidad.siglas}}</v-list-item-subtitle>
                      </v-col>
                      <v-col cols="12" md="3" class="pa-2">
                        <v-list-item-title>
                          <strong>Código</strong>
                        </v-list-item-title>
                        <v-list-item-subtitle>{{entidad.codigo}}</v-list-item-subtitle>
                      </v-col>
                      <v-col cols="12" md="9" class="pa-2">
                        <v-list-item-title>
                          <strong>NIT</strong>
                        </v-list-item-title>
                        <v-list-item-subtitle>{{entidad.nit}}</v-list-item-subtitle>
                      </v-col>
                      <v-col cols="12" md="3" class="pa-2">
                        <v-list-item-title>
                          <strong>Sector</strong>
                        </v-list-item-title>
                        <v-list-item-subtitle>{{entidad.sectorNombre}}</v-list-item-subtitle>
                      </v-col>
                      <v-col cols="12" md="12" class="pa-2">
                        <v-list-item-title>
                          <strong>Objeto Social</strong>
                        </v-list-item-title>
                        <v-list-item-subtitle>{{entidad.objetoSocial}}</v-list-item-subtitle>
                      </v-col>
                      <v-col cols="12" md="8" class="pa-2">
                        <v-list-item-title>
                          <strong>Dirección</strong>
                        </v-list-item-title>
                        <v-list-item-subtitle>{{entidad.direccion}}</v-list-item-subtitle>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col cols="sm" class="pa-2">
                        <v-data-table
                          :headers="headersCuentas"
                          :items="entidad.cuentasBancarias"
                          hide-default-footer
                          fixed-header
                        ></v-data-table>
                      </v-col>
                    </v-row>
                  </v-card>
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
                <!-- /Contactos de la Entidad -->
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
              <v-btn color="red" dark @click="deleteItem()">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Entidad -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small primary--text"
        small
        @click="editItem(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-pen theme--dark</v-icon>
      </v-btn>

      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small teal--text"
        small
        @click="getDetalles(item)"
      >
        <v-icon>mdi-format-list-bulleted</v-icon>
      </v-btn>

      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small pink--text"
        small
        @click="confirmDelete(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-delete theme--dark</v-icon>
      </v-btn>
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
      codigo: null,
      cuentasBancarias: [
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursal: 0,
          moneda: 0
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursal: 0,
          moneda: 0
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursal: 0,
          moneda: 0
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursal: 0,
          moneda: 0
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursal: 0,
          moneda: 0
        },
        {
          numeroCuenta: null,
          numeroSucursal: null,
          nombreSucursal: 0,
          moneda: 0
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
    cantTelefonos: 2,
    cuent: "cuentas",
    telef: "telefonos",
    tabs: null,
    emailRules: [v => /.+@.+\..+/.test(v) || "No tiene la estructura correcta"],
    NombreRules: [v => !!v || "El Nombre es Requerido"],
    DireccionRules: [v => !!v || "La Dirección es Requerida"],
    NitRules: [v => !!v || "El NIT es Requerido"],
    textNit: "",
    errors: [],
    errorController: null,
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
      { text: "Sector", value: "sectorNombre" },
      { text: "Acciones", value: "action", sortable: false }
    ],
    // headerstelefonos: [
    //   {
    //     text: "Número",
    //     align: "left",
    //     sortable: true,
    //     value: "numero"
    //   },
    //   { text: "Extensión ", value: "ext" },
    //   { text: "Acciones", value: "action", sortable: false }
    // ],
    headersCuentas: [
      {
        text: "Número de Cuenta",
        align: "left",
        sortable: true,
        value: "numeroCuenta"
      },
      { text: "Número Sucursal", value: "numeroSucursal" },
      { text: "Nombre Sucursal", value: "nombreSucursalString" },
      { text: "Moneda", value: "monedaString" }
    ],
    date: null,
    modelstate: {}
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
      this.cantTelefonos = this.entidad.cantTelefonos;
      this.cantCuentas = this.entidad.cantCuentas;

      // Asinar los 6 valores del arreglo en dependecia de lo que venga del controller
      if (this.cantCuentas == 1) {
        this.entidad.cuentasBancarias = [
          {
            numeroCuenta: item.cuentasBancarias[0].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[0].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[0].nombreSucursal,
            moneda: item.cuentasBancarias[0].moneda
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          }
        ];
      }
      if (this.cantCuentas == 2) {
        this.entidad.cuentasBancarias = [
          {
            numeroCuenta: item.cuentasBancarias[0].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[0].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[0].nombreSucursal,
            moneda: item.cuentasBancarias[0].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[1].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[1].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[1].nombreSucursal,
            moneda: item.cuentasBancarias[1].moneda
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          }
        ];
      }
      if (this.cantCuentas == 3) {
        this.entidad.cuentasBancarias = [
          {
            numeroCuenta: item.cuentasBancarias[0].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[0].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[0].nombreSucursal,
            moneda: item.cuentasBancarias[0].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[1].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[1].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[1].nombreSucursal,
            moneda: item.cuentasBancarias[1].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[2].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[2].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[2].nombreSucursal,
            moneda: item.cuentasBancarias[2].moneda
          },

          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          }
        ];
      }
      if (this.cantCuentas == 4) {
        this.entidad.cuentasBancarias = [
          {
            numeroCuenta: item.cuentasBancarias[0].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[0].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[0].nombreSucursal,
            moneda: item.cuentasBancarias[0].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[1].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[1].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[1].nombreSucursal,
            moneda: item.cuentasBancarias[1].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[2].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[2].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[2].nombreSucursal,
            moneda: item.cuentasBancarias[2].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[3].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[3].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[3].nombreSucursal,
            moneda: item.cuentasBancarias[3].moneda
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          }
        ];
      }
      if (this.cantCuentas == 5) {
        this.entidad.cuentasBancarias = [
          {
            numeroCuenta: item.cuentasBancarias[0].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[0].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[0].nombreSucursal,
            moneda: item.cuentasBancarias[0].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[1].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[1].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[1].nombreSucursal,
            moneda: item.cuentasBancarias[1].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[2].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[2].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[2].nombreSucursal,
            moneda: item.cuentasBancarias[2].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[3].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[3].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[3].nombreSucursal,
            moneda: item.cuentasBancarias[3].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[4].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[4].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[4].nombreSucursal,
            moneda: item.cuentasBancarias[4].moneda
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          }
        ];
      }
      if (this.cantCuentas == 6) {
        this.entidad.cuentasBancarias = [
          {
            numeroCuenta: item.cuentasBancarias[0].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[0].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[0].nombreSucursal,
            moneda: item.cuentasBancarias[0].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[1].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[1].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[1].nombreSucursal,
            moneda: item.cuentasBancarias[1].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[2].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[2].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[2].nombreSucursal,
            moneda: item.cuentasBancarias[2].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[3].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[3].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[3].nombreSucursal,
            moneda: item.cuentasBancarias[3].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[4].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[4].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[4].nombreSucursal,
            moneda: item.cuentasBancarias[4].moneda
          },
          {
            numeroCuenta: item.cuentasBancarias[5].numeroCuenta,
            numeroSucursal: item.cuentasBancarias[5].numeroSucursal,
            nombreSucursal: item.cuentasBancarias[5].nombreSucursal,
            moneda: item.cuentasBancarias[5].moneda
          }
        ];
      }
      if (this.cantCuentas == 0) {
        this.cantCuentas = 1;
        this.entidad.cuentasBancarias = [
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          }
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
      if (this.cantTelefonos == 3) {
        this.entidad.telefonos = [
          {
            numero: item.telefonos[0].numero,
            extension: item.telefonos[0].extension
          },
          {
            numero: item.telefonos[1].numero,
            extension: item.telefonos[1].extension
          },
          {
            numero: item.telefonos[2].numero,
            extension: item.telefonos[2].extension
          },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null }
        ];
      }
      if (this.cantTelefonos == 4) {
        this.entidad.telefonos = [
          {
            numero: item.telefonos[0].numero,
            extension: item.telefonos[0].extension
          },
          {
            numero: item.telefonos[1].numero,
            extension: item.telefonos[1].extension
          },
          {
            numero: item.telefonos[2].numero,
            extension: item.telefonos[2].extension
          },
          {
            numero: item.telefonos[3].numero,
            extension: item.telefonos[3].extension
          },
          { numero: null, extension: null },
          { numero: null, extension: null }
        ];
      }
      if (this.cantTelefonos == 5) {
        this.entidad.telefonos = [
          {
            numero: item.telefonos[0].numero,
            extension: item.telefonos[0].extension
          },
          {
            numero: item.telefonos[1].numero,
            extension: item.telefonos[1].extension
          },
          {
            numero: item.telefonos[2].numero,
            extension: item.telefonos[2].extension
          },
          {
            numero: item.telefonos[3].numero,
            extension: item.telefonos[3].extension
          },
          {
            numero: item.telefonos[4].numero,
            extension: item.telefonos[4].extension
          },
          { numero: null, extension: null }
        ];
      }
      if (this.cantTelefonos == 6) {
        this.entidad.telefonos = [
          {
            numero: item.telefonos[0].numero,
            extension: item.telefonos[0].extension
          },
          {
            numero: item.telefonos[1].numero,
            extension: item.telefonos[1].extension
          },
          {
            numero: item.telefonos[2].numero,
            extension: item.telefonos[2].extension
          },
          {
            numero: item.telefonos[3].numero,
            extension: item.telefonos[3].extension
          },
          {
            numero: item.telefonos[4].numero,
            extension: item.telefonos[4].extension
          },
          {
            numero: item.telefonos[5].numero,
            extension: item.telefonos[5].extension
          }
        ];
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
      }
      if (this.cantTelefonos == 0) {
        this.cantTelefonos = 2;
        this.entidad.telefonos = [
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null },
          { numero: null, extension: null }
        ];
      }
      // --Asinar los 6 valores del arreglo en dependecia de lo que venga del controller
      this.dialog = true;
    },
    save(method) {
      const url = api.getUrl("contratacion", "entidades");
      this.modelstate = {};
      if (method === "POST") {
        if (this.$refs.form.validate()) {
          const errors = [];
          if (
            this.entidad.cuentasBancarias[0].numeroCuenta == null ||
            this.entidad.cuentasBancarias[0].numeroSucursal == null ||
            this.entidad.cuentasBancarias[0].nombreSucursal == null ||
            this.entidad.cuentasBancarias[0].moneda == null
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #1 por Llenar");
          }
          if (
            (this.cantCuentas >= 2 &&
              this.entidad.cuentasBancarias[1].numeroCuenta == null) ||
            (this.cantCuentas >= 2 &&
              this.entidad.cuentasBancarias[1].numeroSucursal == null) ||
            (this.cantCuentas >= 2 &&
              this.entidad.cuentasBancarias[1].nombreSucursal == null) ||
            (this.cantCuentas >= 2 &&
              this.entidad.cuentasBancarias[1].moneda == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #2 por Llenar");
          }
          if (
            (this.cantCuentas >= 3 &&
              this.entidad.cuentasBancarias[2].numeroCuenta == null) ||
            (this.cantCuentas >= 3 &&
              this.entidad.cuentasBancarias[2].numeroSucursal == null) ||
            (this.cantCuentas >= 3 &&
              this.entidad.cuentasBancarias[2].nombreSucursal == null) ||
            (this.cantCuentas >= 3 &&
              this.entidad.cuentasBancarias[2].moneda == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #3 por Llenar");
          }
          if (
            (this.cantCuentas >= 4 &&
              this.entidad.cuentasBancarias[3].numeroCuenta == null) ||
            (this.cantCuentas >= 4 &&
              this.entidad.cuentasBancarias[3].numeroSucursal == null) ||
            (this.cantCuentas >= 4 &&
              this.entidad.cuentasBancarias[3].nombreSucursal == null) ||
            (this.cantCuentas >= 4 &&
              this.entidad.cuentasBancarias[3].moneda == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #4 por Llenar");
          }
          if (
            (this.cantCuentas >= 5 &&
              this.entidad.cuentasBancarias[4].numeroCuenta == null) ||
            (this.cantCuentas >= 5 &&
              this.entidad.cuentasBancarias[4].numeroSucursal == null) ||
            (this.cantCuentas >= 5 &&
              this.entidad.cuentasBancarias[4].nombreSucursal == null) ||
            (this.cantCuentas >= 5 &&
              this.entidad.cuentasBancarias[4].moneda == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #5 por Llenar");
          }
          if (
            (this.cantCuentas >= 6 &&
              this.entidad.cuentasBancarias[5].numeroCuenta == null) ||
            (this.cantCuentas >= 6 &&
              this.entidad.cuentasBancarias[5].numeroSucursal == null) ||
            (this.cantCuentas >= 6 &&
              this.entidad.cuentasBancarias[5].nombreSucursal == null) ||
            (this.cantCuentas >= 6 &&
              this.entidad.cuentasBancarias[5].moneda == null)
          ) {
            vm.$snotify.error("Faltan Datos de la Cuenta #6 por Llenar");
          } else {
            if (this.entidades.find(x => x.nit === this.entidad.nit) != null) {
              vm.$snotify.error("Ya hay un proveedor con este NIT");
              this.textNit = "Ya hay un proveedor con este NIT";
            } else {
              this.axios.post(url, this.entidad).then(
                response => {
                  this.getResponse(response);
                  this.resetDatos();
                  this.dialog = false;
                  this.errorController = null;
                },
                error => {
                  this.errorController = error.response.data;
                  vm.$snotify.error(error.response.data);
                  console.log(error);
                }
              );
            }
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
    getDetalles(item) {
      this.entidad = Object.assign({}, item);
      if (item.cuentasBancarias.length != 0) {
        this.entidad.cuentasBancarias = item.cuentasBancarias;
      } else
        this.entidad.cuentasBancarias = [
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          }
        ];
      this.cantCuentas = 1;
      if (item.telefonos[0] != null) {
        this.entidad.telefonos = item.telefonos;
      } else this.entidad.telefonos = [{ numero: null, extension: null }];
      this.cantTelefonos = 1;
      this.dialog3 = true;
    },
    confirmDelete(item) {
      this.entidad.id = item.id;
      this.entidad.nombre = item.nombre;
      this.dialog2 = true;
    },
    deleteItem() {
      const url = api.getUrl("contratacion", "entidades");
      this.axios.delete(`${url}/${this.entidad.id}`).then(
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
      this.entidad = {
        codigo: null,
        cuentasBancarias: [
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
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
      this.resetDatos();
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
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
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
          },
          {
            numeroCuenta: null,
            numeroSucursal: null,
            nombreSucursal: 0,
            moneda: 0
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
      this.textNit = "";
      this.errorController = null;
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