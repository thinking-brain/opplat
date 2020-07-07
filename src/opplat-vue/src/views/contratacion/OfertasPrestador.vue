<template>
  <v-data-table :headers="headers" :items="ofertas" :search="search" dense class="elevation-1 pa-5">
    <template v-slot:item.ofertVence="{ item }">
      <v-chip :color="getColor(item.ofertVence)" dark>{{ item.ofertVence }} días</v-chip>
    </template>
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>{{textByfiltro}}</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <!-- Agregar y Editar oferta -->
        <v-dialog v-model="dialog" persistent max-width="1100">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on" class="ml-5">Nueva Oferta</v-btn>
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
                <v-layout row wrap>
                  <v-flex xs6 class="px-1">
                    <v-text-field v-model="oferta.nombre" label="Nombre" clearable required></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-1" v-if="editedIndex==-1">
                    <v-autocomplete
                      v-model="oferta.tipo"
                      item-text="nombre"
                      item-value="id"
                      :items="tipos"
                      cache-items
                      label="tipo"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs3 class="px-1" v-if="editedIndex!=-1">
                    <v-autocomplete
                      v-model="oferta.tipo"
                      item-text="nombre"
                      item-value="id"
                      :items="tipos"
                      cache-items
                      label="tipo"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs3 class="px-1" v-if="editedIndex!=-1">
                    <v-text-field v-model="oferta.numero" label="Número" prefix="#"></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-1">
                    <v-autocomplete
                      v-model="oferta.entidad"
                      item-text="nombre"
                      item-value="id"
                      :items="entidades"
                      cache-items
                      label="Proveedor"
                    >
                      <v-icon @click="dialog3=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                    </v-autocomplete>
                  </v-flex>
                  <v-flex xs3 class="px-1">
                    <v-text-field v-model="oferta.montoCup" label="Monto CUP" clearable prefix="$"></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-1">
                    <v-text-field v-model="oferta.montoCuc" label="Monto CUC" clearable prefix="$"></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-1">
                    <v-text-field v-model="oferta.montoUsd" label="Monto USD" clearable prefix="$"></v-text-field>
                  </v-flex>
                  <v-flex xs6 class="px-1">
                    <v-autocomplete
                      v-model="oferta.formasDePago"
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
                  <v-flex xs3>
                    <v-text-field
                      v-model="oferta.terminoDePago"
                      label="Término de Pago en Meses"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs2 class="px-1">
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
                          v-model="oferta.fechaDeRecepcion"
                          label="Fecha de Recepción"
                          readonly
                          clearable
                          v-on="on"
                          required
                        ></v-text-field>
                      </template>
                      <v-date-picker v-model="oferta.fechaDeRecepcion" @input="menu = false"></v-date-picker>
                    </v-menu>
                  </v-flex>
                  <v-flex xs2 class="px-1">
                    <v-menu
                      v-model="menu1"
                      :close-on-content-click="false"
                      :nudge-right="40"
                      transition="scale-transition"
                      offset-y
                      full-width
                      min-width="290px"
                    >
                      <template v-slot:activator="{ on }">
                        <v-text-field
                          v-model="oferta.fechaDeVenOferta"
                          label="Fecha de Vencimiento"
                          readonly
                          clearable
                          v-on="on"
                          required
                        ></v-text-field>
                      </template>
                      <v-date-picker v-model="oferta.fechaDeVenOferta" @input="menu1 = false"></v-date-picker>
                    </v-menu>
                  </v-flex>
                  <v-flex xs4 class="px-1">
                    <v-text-field v-model="oferta.objetoDeContrato" label="Objeto Social" clearable></v-text-field>
                  </v-flex>
                  <v-flex xs4 class="px-1">
                    <v-autocomplete
                      v-model="oferta.adminContrato"
                      item-text="administrador.nombreCompleto"
                      item-value="administrador.id"
                      :items="adminContratos"
                      cache-items
                      label="Administrador"
                    >
                      <v-icon @click="dialog4=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                    </v-autocomplete>
                  </v-flex>
                  <v-flex xs5 class="px-1">
                    <v-autocomplete
                      v-model="oferta.departamentos"
                      item-text="nombre"
                      item-value="id"
                      :items="departamentos"
                      cache-items
                      label="Departamentos que van a dictaminar la oferta"
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
                      <v-icon @click="dialog8=true" slot="append" color="blue darken-2">mdi-plus</v-icon>
                    </v-autocomplete>
                  </v-flex>
                  <v-flex xs4 class="px-1">
                    <v-autocomplete
                      v-model="oferta.especialistasExternos"
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
                  <!-- <v-flex xs4 class="px-1">
                    <v-file-input v-model="oferta.file" show-size label="Seleccionar Documento"></v-file-input>
                  </v-flex>-->
                  <v-flex x2 class="px-1">
                    <v-autocomplete
                      v-model="oferta.estado"
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
          </v-card>
        </v-dialog>
        <!-- /Agregar y Editar oferta -->
        <!-- Buscar -->
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
        <!-- /Buscar -->

        <v-spacer></v-spacer>

        <!-- Todas las Ofertas -->
        <v-badge :content="cantOfertas" :value="cantOfertas" color="primary" overlap class="mt-4">
          <template v-slot:badge>
            <span v-if="enTiempo > 0">{{ cantOfertas }}</span>
          </template>
          <v-tooltip top color="primary">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="primary"
                @click="getOfertasFromApi()"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Todas las Ofertas</span>
          </v-tooltip>
        </v-badge>
        <!-- /Todas las Ofertas -->

        <!-- Cantidad de Ofertas Ok -->
        <v-badge :content="enTiempo" :value="enTiempo" color="green" overlap class="mt-4 ml-4">
          <template v-slot:badge>
            <span v-if="enTiempo > 0">{{ enTiempo }}</span>
          </template>
          <v-tooltip top color="green">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="green"
                @click="filtro(ofertaTiempo)"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Ofertas en Tiempo</span>
          </v-tooltip>
        </v-badge>
        <!-- /Cantidad de Ofertas Ok -->

        <!-- Cantidad de Ofertas Casi Vencidos -->
        <v-badge :content="proxVencer" :value="proxVencer" color="orange" overlap class="mt-4 ml-4">
          <template v-slot:badge>
            <span v-if="proxVencer > 0">{{ proxVencer}}</span>
          </template>
          <v-tooltip top color="orange">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="orange"
                @click="filtro(ofertasProxVencer)"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Ofertas Próximas a vencer</span>
          </v-tooltip>
        </v-badge>
        <!-- /Cantidad de Ofertas Casi Vencidos -->

        <!-- Cantidad de Ofertas proxVencer -->
        <v-badge
          :content="casiVenc"
          :value="casiVenc"
          color="deep-orange"
          overlap
          class="mt-4 ml-4"
        >
          <template v-slot:badge>
            <span v-if="casiVenc > 0">{{ casiVenc }}</span>
          </template>
          <v-tooltip top color="deep-orange">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="deep-orange"
                @click="filtro(ofertasCasiVenc)"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Ofertas casi vencidas</span>
          </v-tooltip>
        </v-badge>
        <!-- /Cantidad de Ofertas proxVencer -->

        <!-- Cantidad de Ofertas Vencidas -->
        <v-badge :content="vencidos" :value="vencidos" color="red" overlap class="mt-4 ml-4">
          <template v-slot:badge>
            <span v-if="vencidos > 0">{{ vencidos }}</span>
          </template>
          <v-tooltip top color="red">
            <template v-slot:activator="{ on }">
              <v-icon
                medium
                v-on="on"
                color="red"
                @click="filtro(ofertasVenc)"
              >mdi-file-document-box-multiple-outline</v-icon>
            </template>
            <span>Ofertas Vencidas</span>
          </v-tooltip>
        </v-badge>
        <!-- /Cantidad de Ofertas Vencidas -->

        <!-- Detalles de la oferta -->
        <v-dialog
          v-model="dialog6"
          persistent
          transition="dialog-bottom-transition"
          flat
          max-width="1200"
        >
          <v-card>
            <v-toolbar dark fadeOnScroll :color="getColor(oferta.ofertVence)">
              <v-flex xs12 sm10 md6 lg4>Detalles</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>
            <v-container>
              <p
                class="text-center text-uppercase headline font-weight-black"
              >OFERTA DE {{oferta.tipoNombre}} DE {{oferta.nombre}}.</p>
              <v-row>
                <!-- DATOS DE LA OFERTA -->
                <v-col cols="6">
                  <v-card :elevation="2" flat>
                    <v-card-text>
                      <v-row>
                        <v-col cols="12" md="6" class="pa-2 headline">
                          <div>Oferta</div>
                        </v-col>
                        <v-col cols="12" md="6" :class="textOfertaVence.class">
                          <strong>{{textOfertaVence.text}}</strong>
                          {{oferta.ofertVence}} Días
                        </v-col>
                      </v-row>
                      <v-row>
                        <v-col cols="12" md="12" class="pa-2">
                          <strong>Número :</strong>
                          <u class="pl-2">{{oferta.numero}}</u>
                        </v-col>
                        <v-col cols="12" md="12" class="pa-2">
                          <strong>Administrador del Contrato :</strong>
                          <u class="pl-2">{{oferta.adminContrato.nombreCompleto}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Fecha de Recepción:</strong>
                          <u class="pl-2">{{oferta.fechaDeRece}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>La Oferta Vence el:</strong>
                          <u class="pl-2">{{oferta.fechaDeVenOfer}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Objeto Social :</strong>
                          <u class="pl-2">{{oferta.objetoDeContrato}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Término de Pago :</strong>
                          <u class="pl-1">{{oferta.terminoDePagoDet}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2" v-if="oferta.montoCup!=null">
                          <strong>Monto en CUP :</strong>
                          <u class="pl-2">{{oferta.montoCup}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2" v-if="oferta.montoCuc!=null">
                          <strong>Monto en CUC :</strong>
                          <u class="pl-2">{{oferta.montoCuc}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2" v-if="oferta.montoUsd!=null">
                          <strong>Monto en USD :</strong>
                          <u class="pl-2">{{oferta.montoUsd}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Estado :</strong>
                          <u class="pl-2">{{oferta.estadoNombre}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Formas e Pago :</strong>
                          <v-spacer></v-spacer>
                          <span v-for="item in oferta.formasDePago" :key="item.nombre">
                            <v-spacer></v-spacer>-
                            <u class="pl-2">{{item.nombre}}</u>
                          </span>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Especialistas Externos:</strong>
                          <span
                            v-for="item in oferta.especialistasExternos"
                            :key="item.nombreCompleto"
                            class="pl-2"
                          >
                            <v-spacer></v-spacer>-
                            <u class="pl-2">{{item.nombreCompleto}}</u>
                          </span>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Especialistas Internos:</strong>
                          <span
                            v-for="item in oferta.especialistasExternos"
                            :key="item.nombreCompleto"
                            class="pl-2"
                          >
                            <v-spacer></v-spacer>-
                            <u class="pl-2">{{item.nombreCompleto}}</u>
                          </span>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Aprobado por el Jurídico :</strong>
                          <span v-if="oferta.aprobJuridico">
                            <v-icon color="success">mdi-check-underline</v-icon>
                            <v-text :class="`success--text`">Sí</v-text>
                          </span>
                          <span v-else>
                            <v-icon color="red">mdi-close-outline</v-icon>
                            <v-text :class="`red--text`">No</v-text>
                          </span>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Aprobado por el Económico :</strong>
                          <span v-if="oferta.aprobEconomico">
                            <v-icon color="success">mdi-check-underline</v-icon>
                            <v-text :class="`success--text`">Sí</v-text>
                          </span>
                          <span v-else>
                            <v-icon color="red">mdi-close-outline</v-icon>
                            <v-text :class="`red--text`">No</v-text>
                          </span>
                        </v-col>
                        <v-col cols="12" md="12" class="pa-2">
                          <strong>Aprobado por el Comité Contratación:</strong>
                          <span v-if="oferta.aprobComitContratacion">
                            <v-text :class="`success--text`">Sí</v-text>
                            <v-icon color="success">mdi-check-underline</v-icon>
                          </span>
                          <span v-else>
                            <v-icon color="red">mdi-close-outline</v-icon>
                            <v-text :class="`red--text`">No</v-text>
                          </span>
                        </v-col>
                      </v-row>
                    </v-card-text>
                  </v-card>
                </v-col>
                <!-- /DATOS DE LA OFERTA -->

                <!-- DATOS DE LA ENTIDAD PROVEEDORA -->
                <v-col cols="6">
                  <v-card :elevation="2" flat>
                    <v-card-text>
                      <v-row>
                        <v-col cols="12" md="12" class="pa-2">
                          <div class="headline">Entidad Proveedora</div>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Nombre :</strong>
                          <u class="pl-2">{{oferta.entidad.nombre}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Dirección :</strong>
                          <u class="pl-2">{{oferta.entidad.direccion}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>NIT :</strong>
                          <u class="pl-2">{{oferta.entidad.nit}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Sector :</strong>
                          <u class="pl-2">{{oferta.entidad.sectorNombre}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Fax :</strong>
                          <u class="pl-2">{{oferta.entidad.fax}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Correo :</strong>
                          <u class="pl-2">{{oferta.entidad.correo}}</u>
                        </v-col>
                        <v-col cols="12" md="6" class="pa-2">
                          <strong>Objeto Social :</strong>
                          <u class="pl-2">{{oferta.entidad.objetoSocial}}</u>
                        </v-col>
                        <v-col cols="8" md="8" class="pa-2">
                          <strong>Teléfonos :</strong>
                          <v-spacer></v-spacer>
                          <span v-for="item in oferta.entidad.telefonos" :key="item.numero">
                            <strong class="pl-4">Número:</strong>
                            {{item.numero}}
                            <strong
                              class="pl-12"
                            >Extensión:</strong>
                            {{item.extension}}
                            <v-divider></v-divider>
                          </span>
                        </v-col>
                        <v-col cols="12" md="12" class="pa-2">
                          <strong>Cuentas Bancarias :</strong>
                          <v-data-table
                            :headers="headersCuentas"
                            :items="oferta.entidad.cuentasBancarias"
                            hide-default-footer
                            fixed-header
                            class="pt-3"
                          ></v-data-table>
                        </v-col>
                      </v-row>
                    </v-card-text>
                  </v-card>
                </v-col>
              </v-row>
              <!-- /DATOS DE LA ENTIDAD PROVEEDORA -->
            </v-container>
          </v-card>
        </v-dialog>
        <!-- /Detalles de la oferta -->

        <!-- Aprobar oferta -->
        <v-dialog v-model="dialog9" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="close()">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas aprobar la Oferta</v-card-title>
            <v-card-text class="text-center">{{oferta.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="aprobarOferta(oferta)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Aprobar oferta -->
        <!-- Delete oferta -->
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
            <v-card-text class="text-center">{{oferta.nombre}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(oferta)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete oferta -->

        <!-- Subir Documento -->
        <v-row justify="center">
          <v-dialog v-model="dialog7" persistent max-width="400">
            <v-card>
              <v-flex xs12 class="px-1">
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
        <!-- Agregar Dictaminadores Contrato-->
        <v-dialog v-model="dialog8" persistent max-width="1000">
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
      </v-toolbar>
    </template>
    <!-- Actions -->
    <template v-slot:item.action="{ item }">
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small primary--text"
        small
        @click="editItem(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-pen theme--dark</v-icon>
      </v-btn>
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small primary--text"
        small
        @click="confirmAprobarOferta(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-check-box-multiple-outline theme--dark</v-icon>
      </v-btn>
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small secondary--text"
        small
        @click="confirmUpload(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-upload theme--dark</v-icon>
      </v-btn>
      <v-btn
        class="v-btn v-btn--depressed v-btn--fab v-btn--flat v-btn--icon v-btn--outlined v-btn--round theme--dark v-size--small secondary--text"
        small
        @click="download(item)"
      >
        <v-icon>v-icon notranslate mdi mdi-download theme--dark</v-icon>
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
    <!-- /Actions -->
  </v-data-table>
</template>
<script>
import api from "@/api";
import AdminContratos from "@/components/contratacion/AdminContratos.vue";
import EspExternos from "@/components/contratacion/EspExternos.vue";
import FormasDePago from "@/components/contratacion/FormasDePago.vue";
import Entidades from "@/components/contratacion/Entidades.vue";
import DictContratos from "@/components/contratacion/DictContratos.vue";

export default {
  components: {
    AdminContratos,
    EspExternos,
    FormasDePago,
    Entidades,
    DictContratos
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
    dialog8: false,
    dialog9: false,
    menu: false,
    menu1: false,
    search: "",
    editedIndex: -1,
    ofertas: [],
    cantOfertas: 0,
    oferta: {
      entidad: {},
      adminContrato: {},
      dictaminadores: [],
      especialistasExternos: []
    },
    file: null,
    entidades: [],
    especialistasExternosAll: [],
    dictaminadoresContratos: [],
    adminContratos: [],
    estados: [],
    tipos: [],
    formasDePagos: [],
    enTiempo: 0,
    casiVenc: 0,
    proxVencer: 0,
    vencidos: 0,
    todasLasOfertas: false,
    vencimientoOfertas: [],
    ofertaTiempo: "ofertaTiempo",
    ofertasProxVencer: "ofertasProxVencer",
    ofertasCasiVenc: "ofertasCasiVenc",
    ofertasVenc: "ofertasVenc",
    tiempoVenOfertas: {},
    urlByfiltro: "",
    textByfiltro: "",
    show: false,
    tabs: null,
    textOfertaVence: {
      text: null,
      class: null
    },
    trabajadores: [],
    departamentos: [],
    errors: [],
    headers: [
      { text: "Número", sortable: true, value: "numero" },
      { text: "Nombre", align: "left", sortable: true, value: "nombre" },
      { text: "Tipo", value: "tipoNombre" },
      { text: "Entidad", value: "entidad.nombre" },
      { text: "Vence en", value: "ofertVence" },
      { text: "Estado", value: "estadoNombre" },
      { text: "Acciones", value: "action", sortable: false }
    ],
    headersCuentas: [
      {
        text: "Número de Cuenta",
        align: "left",
        sortable: true,
        value: "numeroCuenta"
      },
      { text: "Número Sucursal", value: "numeroSucursal" },
      { text: "Nombre Sucursal", value: "nombreSucursal" },
      { text: "Moneda", value: "moneda" }
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
  },

  methods: {
    getOfertasFromApi() {
      const url = api.getUrl("contratacion", "Contratos?tipoTramite=oferta");
      this.axios.get(url).then(
        response => {
          this.textByfiltro = "Ofertas Como Prestador";
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
    editItem(item) {
      this.editedIndex = this.ofertas.indexOf(item);
      this.oferta = Object.assign({}, item);
      this.oferta.entidad = item.entidad[0];
      this.oferta.adminContrato = item.adminContrato.id;

      for (let index = 0; index < this.oferta.formasDePago.length; index++) {
        this.oferta.formasDePago[index] = item.formasDePago[index].id;
      }

      this.dialog = true;
    },
    getDetalles(item) {
      this.oferta = Object.assign({}, item);
      this.oferta.entidad = item.entidad[0];
      this.dialog6 = true;
      if (this.oferta.ofertVence < this.tiempoVenOfertas.ofertasVencidas) {
        this.textOfertaVence.text = "La Oferta ya se Venció Tiene";
        this.textOfertaVence.class = "pa-2 pt-3 red--text";
      }
      if (this.oferta.ofertVence == this.tiempoVenOfertas.ofertasVencidas) {
        this.textOfertaVence.text = "La Oferta se Vence Hoy";
        this.textOfertaVence.class = "pa-2 pt-3 red--text";
      }
      if (
        this.oferta.ofertVence > this.tiempoVenOfertas.ofertasCasiVencDesde &&
        this.oferta.ofertVence <= this.tiempoVenOfertas.ofertasCasiVencHasta
      ) {
        this.textOfertaVence.text = "La Oferta está casi a vencida le quedan";
        this.textOfertaVence.class = "pa-2 pt-3 deep-orange--text";
      }
      if (
        this.oferta.ofertVence > this.tiempoVenOfertas.ofertasProxVencDesde &&
        this.oferta.ofertVence <= this.tiempoVenOfertas.ofertasProxVencHasta
      ) {
        this.textOfertaVence.text =
          "La Oferta está próxima a vencerce le quedan";
        this.textOfertaVence.class = "pa-2 pt-3 orange--text";
      }
      if (this.oferta.ofertVence > this.tiempoVenOfertas.ofertaTiempo) {
        this.textOfertaVence.text = "La Oferta Vence en";
        this.textOfertaVence.class = "pa-2 pt-3 green--text";
      }
    },
    save(method) {
      this.oferta.cliente = false;
      const url = api.getUrl("contratacion", "Contratos");
      if (method === "POST") {
        this.axios.post(url, this.oferta).then(
          response => {
            this.getResponse(response);
            this.getOfertasFromApi();
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
      if (method === "PUT") {
        this.oferta.cliente = false;
        this.axios.put(`${url}/${this.oferta.id}`, this.oferta).then(
          response => {
            this.getResponse(response);
            this.getOfertasFromApi();
            this.dialog = false;
          },
          error => {
            console.log(error);
          }
        );
      }
    },
    confirmUpload(item) {
      this.oferta = Object.assign({}, item);
      this.dialog7 = true;
    },
    upload() {
      const formData = new FormData();
      formData.append("file", this.file);
      const url = api.getUrl("contratacion", "contratos/UploadFile");
      this.axios
        .post(`${url}/${this.oferta.id}`, formData, {
          headers: {
            "Content-Type": "multipart/form-data"
          }
        })
        .then(
          response => {
            this.getResponse(response);
            this.getOfertasFromApi();
            this.dialog7 = false;
          },
          error => {
            console.log(error);
          }
        );
    },
    download(item) {
      const url = api.getUrl("contratacion", "contratos/DownloadFile");
      this.axios.get(`${url}/${item.id}`).then(
        response => {
          window.open(url + "/" + item.id);
        },
        error => {
          console.log(error);
        }
      );
    },
    confirmDelete(item) {
      this.oferta = Object.assign({}, item);
      this.dialog2 = true;
    },
    deleteItem(oferta) {
      const url = api.getUrl("contratacion", "Contratos");
      this.axios.delete(`${url}/${oferta.id}`).then(
        response => {
          this.getResponse(response);
          this.getOfertasFromApi();
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
      this.dialog6 = false;
      this.dialog7 = false;
      this.dialog9 = false;
      this.oferta = {
        entidad: {},
        adminContrato: {}
      };
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
    closeAdd() {
      this.getEntidadesFromApi();
      this.getEspecialistasExternosFromApi();
      this.getAdminContratosFromApi();
      this.getDictContratosFromApi();
      this.dialog3 = false;
      this.dialog4 = false;
      this.dialog5 = false;
      this.dialog8 = false;
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success("Exito al realizar la operación");
      }
    },
    getColor(ofertVence) {
      this.GetVencimientoOferta();
      if (ofertVence < this.tiempoVenOfertas.ofertasVencidas) {
        return "red";
      } else if (
        ofertVence >= this.tiempoVenOfertas.ofertasCasiVencDesde &&
        ofertVence <= this.tiempoVenOfertas.ofertasCasiVencHasta
      ) {
        return "deep-orange";
      } else if (
        ofertVence > this.tiempoVenOfertas.ofertasProxVencDesde &&
        ofertVence <= this.tiempoVenOfertas.ofertasProxVencHasta
      )
        return "orange";
      else {
        return "green";
      }
    },
    GetVencimientoOferta() {
      const url = api.getUrl(
        "contratacion",
        "contratos/VencimientoOferta?cliente=false"
      );
      this.axios.get(url).then(
        response => {
          this.vencimientoOfertas = response.data;
          this.vencidos = this.vencimientoOfertas[0];
          this.casiVenc = this.vencimientoOfertas[1];
          this.proxVencer = this.vencimientoOfertas[2];
          this.enTiempo = this.vencimientoOfertas[3];
        },
        error => {
          console.log(error);
        }
      );
    },
    removeformasDePago(item) {
      const index = this.oferta.formasDePago.indexOf(item.id);
      if (index >= 0) this.oferta.formasDePago.splice(index, 1);
    },
    removeDictaminadores(item) {
      const index = this.oferta.dictaminadores.indexOf(item.id);
      if (index >= 0) this.oferta.dictaminadores.splice(index, 1);
    },
    filtro(filtro) {
      if (filtro == "ofertaTiempo") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=oferta&filtro=ofertaTiempo&cliente=false"
        );
        this.textByfiltro = "Ofertas en Tiempo";
      }
      if (filtro == "ofertasProxVencer") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=oferta&filtro=ofertasProxVencer&cliente=false"
        );
        this.textByfiltro = "Ofertas Próximas a Vencer";
      }
      if (filtro == "ofertasCasiVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=oferta&filtro=ofertasCasiVenc&cliente=false"
        );
        this.textByfiltro = "Ofertas Casi Vencidas";
      }
      if (filtro == "ofertasVenc") {
        this.urlByfiltro = api.getUrl(
          "contratacion",
          "Contratos?tipoTramite=oferta&filtro=ofertasVenc&cliente=false"
        );
        this.textByfiltro = "Ofertas Vencidas";
      }
      this.axios.get(this.urlByfiltro).then(
        response => {
          this.ofertas = response.data;
          vm.$snotify.success(this.textByfiltro);
          this.todasLasOfertas = true;
        },
        error => {
          console.log(error);
        }
      );
    },
    confirmAprobarOferta(item) {
      this.oferta = Object.assign({}, item);
      this.dialog9 = true;
    },
    aprobarOferta(item) {}
  }
};
</script>