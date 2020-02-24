<template>
  <v-data-table :headers="headers" :items="trabajadores" :search="search" class="elevation-1 pa-5">
    <template v-slot:top>
      <!-- Filtro Trabajador -->
      <v-row>
        <v-col cols="12" sm="6" md="2">
          <div class="text-center">
            <div class="my-2">
              <v-btn color="primary" @click="dialog1=true">Filtrar por</v-btn>
            </div>
          </div>
        </v-col>
        <v-col cols="12" sm="6" md="2" v-if="volver===true">
          <div class="text-center">
            <div class="my-2">
              <v-btn color="primary" @click="getTrabajadoresFromApi">
                <v-icon>mdi-reply-all</v-icon>
              </v-btn>
            </div>
          </div>
        </v-col>
        <v-layout>
          <v-dialog v-model="dialog1" persistent max-width="800px">
            <v-card>
              <v-card-title>
                <span class="headline">Mostrar Trabajadores por:</span>
              </v-card-title>
              <v-card-text>
                <v-container grid-list-md>
                  <v-layout wrap>
                    <v-flex xs12 sm6 md6>
                      <v-autocomplete
                        v-model="unidadOrganizativa"
                        item-text="nombre"
                        :items="unidadesOrganizativas"
                        :filter="activeFilter"
                        cache-items
                        clearable
                        label="Unidad Organizativa"
                        prepend-icon="mdi-database-search"
                        chips
                        allow-overflow
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex xs12 sm6 md6>
                      <v-autocomplete
                        v-model="cargo"
                        item-text="nombre"
                        :items="cargos"
                        :filter="activeFilter"
                        cache-items
                        clearable
                        label="Cargo"
                        prepend-icon="mdi-database-search"
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex xs12 sm6 md6>
                      <v-select
                        v-model="sexo"
                        item-text="nombre"
                        :items="sexos"
                        :filter="activeFilter"
                        cache-items
                        clearable
                        label="Sexo"
                        prepend-icon="mdi-database-search"
                      ></v-select>
                    </v-flex>
                    <v-flex xs12 sm6 md6>
                      <v-select
                        v-model="colordePiel"
                        item-text="nombre"
                        :items="coloresdePiel"
                        :filter="activeFilter"
                        cache-items
                        clearable
                        label="Color de Piel"
                        prepend-icon="mdi-database-search"
                      ></v-select>
                    </v-flex>
                    <v-flex xs12 sm6 md6>
                      <v-select
                        v-model="nivelDeEscolaridad"
                        item-text="nombre"
                        :items="nivelesEscolaridad"
                        :filter="activeFilter"
                        cache-items
                        clearable
                        label="Nivel de Escolaridad"
                        prepend-icon="mdi-database-search"
                      ></v-select>
                    </v-flex>
                    <v-flex xs12 sm6 md6>
                      <v-select
                        v-model="estado"
                        item-text="nombre"
                        :items="estados"
                        :filter="activeFilter"
                        cache-items
                        clearable
                        label="Estado"
                        prepend-icon="mdi-database-search"
                      ></v-select>
                    </v-flex>
                    <v-layout>
                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="edadDesde"
                          clearable
                          label="Rango de Edad"
                          placeholder="Mayores de"
                          prepend-icon="mdi-database-search"
                        ></v-text-field>
                      </v-flex>
                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="edadHasta"
                          clearable
                          label="Rango de Edad"
                          placeholder="Menores de"
                          prepend-icon="mdi-database-search"
                        ></v-text-field>
                      </v-flex>
                    </v-layout>
                    <v-flex xs12 sm6 md6>
                      <v-autocomplete
                        v-model="perfil"
                        item-text="nombre"
                        :items="PerfilesOcupacionales"
                        :filter="activeFilter"
                        cache-items
                        clearable
                        label="Perfil Ocupacional"
                        prepend-icon="mdi-database-search"
                      ></v-autocomplete>
                    </v-flex>
                    <v-flex xs12 sm6 md6>
                      <v-autocomplete
                        v-model="municipio"
                        item-text="nombre"
                        :items="Municipios"
                        :filter="activeFilter"
                        label="Municipio"
                        required
                        clearable
                      ></v-autocomplete>
                    </v-flex>
                  </v-layout>
                </v-container>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="green darken-1" text @click="getFiltrosFromApi">Aceptar</v-btn>
                <v-btn color="blue darken-1" text @click="dialog1=false">Cancelar</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-layout>
      </v-row>
      <!-- /Filtro Trabajador -->

      <v-toolbar flat color="white">
        <v-toolbar-title>Listado de Trabajadores</v-toolbar-title>
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
        <!-- Agregar y Editar Trabajador -->
        <v-dialog v-model="dialog" persistent>
          <!-- <template v-slot:activator="{ on }">
            <v-btn color="primary" dark v-on="on">Agregar Trabajador</v-btn>
          </template>-->
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
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      label="Nombre"
                      v-model="trabajador.nombre"
                      :rules="NombreRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      label="Apellidos"
                      v-model="trabajador.apellidos"
                      :rules="ApellidosRules"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      label="Carnet de Identidad"
                      v-model="trabajador.ci"
                      :counter="11"
                      :rules="ciRules"
                      clearable
                      required
                    ></v-text-field>
                    <span asp-validation-for="CI" class="text-danger"></span>
                  </v-flex>
                  <!-- <v-flex xs3 class="px-3">
                    <v-file-input show-size label="Seleccionar Foto" v-model="trabajador.foto"></v-file-input>
                  </v-flex>-->
                  <v-flex xs4 class="px-3">
                    <v-text-field
                      label="Dirección"
                      v-model="trabajador.direccion"
                      placeholder="Calle e/ # Casa o Apto, Barrio o Finca"
                      clearable
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs2 class="px-3">
                    <v-autocomplete
                      v-model="trabajador.municipioId"
                      item-text="nombre"
                      item-value="id"
                      :items="Municipios"
                      :filter="activeFilter"
                      label="Municipio"
                      required
                      clearable
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-select
                      v-model="trabajador.nivelDeEscolaridad"
                      item-text="nombre"
                      item-value="id"
                      :items="nivelesEscolaridad"
                      label="Nivel de Escolaridad"
                      clearable
                    ></v-select>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-autocomplete
                      v-model="trabajador.perfilOcupacional"
                      item-text="nombre"
                      :items="PerfilesOcupacionales"
                      :filter="activeFilter"
                      cache-items
                      clearable
                      label="Perfil Ocupacional"
                    ></v-autocomplete>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      label="Teléfono Móvil"
                      v-model="trabajador.telefonoMovil"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field label="Teléfono Fijo" v-model="trabajador.telefonoFijo" clearable></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      label="Correo"
                      v-model="trabajador.correo"
                      :rules="emailRules"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-select
                      v-model="trabajador.colorDeOjos"
                      item-text="nombre"
                      item-value="id"
                      :items="coloresdeOjos"
                      label="Color de Ojos"
                      clearable
                    ></v-select>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-select
                      v-model="trabajador.colorDePiel"
                      item-text="nombre"
                      item-value="id"
                      :items="coloresdePiel"
                      label="Color de Piel"
                      clearable
                    ></v-select>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      v-model="trabajador.tallaCalzado"
                      label="Talla de Calzado"
                      clearable
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-select
                      v-model="trabajador.tallaDeCamisa"
                      item-text="nombre"
                      item-value="id"
                      :items="tallasDeCamisas"
                      label="Talla de Camisa"
                      clearable
                    ></v-select>
                  </v-flex>
                  <v-flex xs3 class="px-3">
                    <v-text-field
                      label="Talla de Pantalon"
                      v-model="trabajador.tallaPantalon"
                      clearable
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-flex xs5 class="px-3">
                  <v-text-field
                    label="Otras Características: "
                    v-model="trabajador.otrasCaracteristicas"
                    clearable
                  ></v-text-field>
                </v-flex>
              </v-container>
            </v-form>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green darken-1" text @click="save(method)">Aceptar</v-btn>
              <v-btn color="blue darken-1" text @click=" close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Agregar y Editar Trabajador -->

        <!-- Detalles del Trabajador -->
        <v-dialog v-model="dialog3" persistent transition="dialog-bottom-transition" flat>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Detalles del Trabajador</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="dialog3 = false">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>

            <v-container fluid>
              <v-row dense>
                <v-col cols="7">
                  <v-card flat>
                    <v-layout justify-center>
                      <v-layout class="pa-2">
                        <v-list-item two-line>
                          <v-list-item-content>
                            <v-list-item-title>
                              <strong>Unidad Organizativa:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle>{{trabajador.unidadOrganizativa}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-list-item two-line>
                          <v-list-item-content>
                            <v-list-item-title>
                              <strong>Cargo:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle>{{trabajador.cargo}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                      <v-layout class="pa-2">
                        <v-list-item two-line>
                          <v-list-item-content>
                            <v-list-item-title>
                              <strong>Estado:</strong>
                            </v-list-item-title>
                            <v-list-item-subtitle>{{trabajador.estadoTrabajadorName}}</v-list-item-subtitle>
                          </v-list-item-content>
                        </v-list-item>
                      </v-layout>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-list-item two-line>
                        <v-list-item-content>
                          <v-list-item-title>
                            <strong>Funciones o Tareas Principales del Cargo:</strong>
                          </v-list-item-title>
                          <v-list-item-subtitle
                            v-for="item in funciones"
                            :key="item.text"
                          >- {{item.text}}</v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-list-item two-line>
                        <v-list-item-content>
                          <v-list-item-title>
                            <strong>Requisitos de Conocimiento para el Cargo:</strong>
                          </v-list-item-title>
                          <v-list-item-subtitle
                            v-for="item in requisitos"
                            :key="item.text"
                          >- {{item.text}}</v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                    </v-layout>
                  </v-card>
                </v-col>

                <v-col cols="5">
                  <v-card color="blue darken-3" dark>
                    <v-list-item>
                      <v-layout column align-center xs12 sm10 md6 lg4>
                        <v-avatar size="120" v-if="trabajador.sexoName==='M'">
                          <img src="img/default-avatar-man.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="120" v-else-if="trabajador.sexoName==='F'">
                          <img src="img/default-avatar-woman.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="110" v-else>
                          <img src="img/default-avatar.png" class="float-center pa-5" dark />
                        </v-avatar>
                        <v-layout class="pa-2">
                          <v-toolbar-title class="text-capitalize">{{trabajador.nombre_Completo}}</v-toolbar-title>
                        </v-layout>
                      </v-layout>
                    </v-list-item>
                    <v-row dense>
                      <v-col cols="7">
                        <v-layout class="pa-2">
                          <v-text>Carnet de Identidad: {{trabajador.ci}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Nivel de Escolaridad: {{trabajador.nivelDeEscolaridadName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Perfil Ocupacional: {{trabajador.perfilOcupacional}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Color de Ojos: {{trabajador.colorDeOjosName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Fecha Nacimiento: {{trabajador.fecha_Nac}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Correo: {{trabajador.correo}}</v-text>
                        </v-layout>
                      </v-col>
                      <v-col cols="5">
                        <v-layout class="pa-2">
                          <v-text>Sexo: {{trabajador.sexoName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Teléfono Fijo: {{trabajador.telefonoFijo}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Teléfono Movil: {{trabajador.telefonoMovil}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Color de Piel: {{trabajador.colorDePielName}}</v-text>
                        </v-layout>
                        <v-layout class="pa-2">
                          <v-text>Edad: {{trabajador.edad}} Años</v-text>
                        </v-layout>
                      </v-col>
                    </v-row>
                    <v-layout class="pa-2">
                      <v-text>Direccion: {{trabajador.direccion}}</v-text>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-text>Otros Datos de Interes: {{trabajador.otrasCaracteristicas}}</v-text>
                    </v-layout>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- Detalles del Trabajador -->

        <!-- Movimientos -->
        <v-dialog v-model="dialog5" persistent transition="dialog-bottom-transition" flat>
          <v-card>
            <v-toolbar dark fadeOnScroll color="blue darken-3">
              <v-flex xs12 sm10 md6 lg4>Movimientos</v-flex>
              <v-spacer></v-spacer>
              <v-toolbar-items>
                <v-btn icon dark @click="close()">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
              </v-toolbar-items>
            </v-toolbar>

            <v-container fluid>
              <v-row dense>
                <v-col cols="7">
                  <v-card flat>
                    <v-toolbar flat>
                      <v-tabs slot="extension" v-model="tabs" centered>
                        <v-tab>Traslado</v-tab>
                        <v-tab>Baja</v-tab>
                      </v-tabs>
                    </v-toolbar>

                    <v-tabs-items v-model="tabs" py-6>
                      <v-tab-item>
                        <v-card flat>
                          <v-card-title class="headline mx-6">Hacer Traslado</v-card-title>
                          <v-card-text>
                            <form>
                              <v-container>
                                <v-row>
                                  <v-col cols="12" md="7">
                                    <v-autocomplete
                                      xs6
                                      class="pa-2"
                                      v-model="trabajador"
                                      item-text="nombre_Completo"
                                      item-value="id"
                                      :items="trabajadores"
                                      :filter="activeFilter"
                                      cache-items
                                      label="Nombre del Trabajador"
                                      disabled
                                    ></v-autocomplete>
                                  </v-col>
                                  <v-col cols="6" md="5">
                                    <v-autocomplete
                                      xs6
                                      class="pa-2"
                                      v-model="traslado.cargoDestinoId"
                                      item-text="nombre"
                                      item-value="id"
                                      :items="cargos"
                                      label="Cargo a Ocupar"
                                      clearable
                                      required
                                    ></v-autocomplete>
                                  </v-col>
                                </v-row>
                                <v-row>
                                  <v-col cols="6" md="6">
                                    <v-autocomplete
                                      class="pa-2"
                                      v-model="traslado.unidOrgDestinoId"
                                      item-text="nombre"
                                      item-value="id"
                                      :items="unidadesOrganizativas"
                                      label="Unidad Organizativa"
                                      clearable
                                      required
                                    ></v-autocomplete>
                                  </v-col>
                                  <v-col cols="6" md="6">
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
                                          v-model="traslado.fechaTraslado"
                                          label="Fecha"
                                          readonly
                                          v-on="on"
                                          class="pa-2"
                                        ></v-text-field>
                                      </template>
                                      <v-date-picker
                                        v-model="traslado.fechaTraslado"
                                        @input="menu1 = false"
                                      ></v-date-picker>
                                    </v-menu>
                                  </v-col>
                                </v-row>
                                <v-btn color="green darken-1" text @click="Traslado">Aceptar</v-btn>
                                <v-btn color="blue darken-1" text @click="clearTraslado">Cancelar</v-btn>
                              </v-container>
                            </form>
                          </v-card-text>
                        </v-card>
                      </v-tab-item>
                      <v-tab-item>
                        <v-card flat>
                          <v-card-title class="headline mx-6">Dar Baja</v-card-title>
                          <v-card-text>
                            <form>
                              <v-container>
                                <v-row>
                                  <v-col cols="12" md="8">
                                    <v-autocomplete
                                      xs6
                                      class="pa-2"
                                      v-model="trabajador"
                                      item-text="nombre_Completo"
                                      item-value="id"
                                      :items="trabajadores"
                                      :filter="activeFilter"
                                      cache-items
                                      label="Nombre del Trabajador"
                                      disabled
                                    ></v-autocomplete>
                                  </v-col>
                                  <v-col cols="6" md="4">
                                    <v-autocomplete
                                      xs6
                                      class="pa-2"
                                      v-model="baja.causaDeBaja"
                                      item-text="nombre"
                                      item-value="id"
                                      :items="CausasDeBajas"
                                      :filter="activeFilter"
                                      cache-items
                                      clearable
                                      label="Causa de Baja"
                                    ></v-autocomplete>
                                  </v-col>
                                </v-row>
                                <v-row>
                                  <v-col cols="12" md="4">
                                    <v-menu
                                      v-model="menu2"
                                      :close-on-content-click="false"
                                      :nudge-right="40"
                                      transition="scale-transition"
                                      offset-y
                                      full-width
                                      min-width="290px"
                                    >
                                      <template v-slot:activator="{ on }">
                                        <v-text-field
                                          v-model="baja.fechaBaja"
                                          label="Fecha"
                                          readonly
                                          v-on="on"
                                          class="pa-2"
                                        ></v-text-field>
                                      </template>
                                      <v-date-picker
                                        v-model="baja.fechaBaja"
                                        @input="menu2 = false"
                                      ></v-date-picker>
                                    </v-menu>
                                  </v-col>
                                </v-row>
                              </v-container>
                              <v-spacer></v-spacer>
                              <v-btn color="green darken-1" text @click="confirmBaja()">Aceptar</v-btn>
                              <v-btn color="blue darken-1" text @click="clearBaja">Cancelar</v-btn>
                            </form>
                          </v-card-text>
                        </v-card>
                        <v-dialog v-model="dialog6" persistent max-width="350px">
                          <v-toolbar dark fadeOnScroll color="red">
                            <v-spacer></v-spacer>
                            <v-toolbar-items>
                              <v-btn icon dark @click="close()">
                                <v-icon>mdi-close</v-icon>
                              </v-btn>
                            </v-toolbar-items>
                          </v-toolbar>
                          <v-card>
                            <v-card-title
                              class="headline text-center"
                            >Seguro que deseas darle de Baja al Trabajador</v-card-title>
                            <v-card-text class="text-center">{{trabajador.nombre_Completo}}</v-card-text>
                            <v-card-actions>
                              <v-spacer></v-spacer>
                              <v-btn color="red" dark @click="saveBaja()">Aceptar</v-btn>
                              <v-btn color="primary" @click="dialog6=false">Cancelar</v-btn>
                            </v-card-actions>
                          </v-card>
                        </v-dialog>
                      </v-tab-item>
                    </v-tabs-items>
                  </v-card>
                </v-col>

                <v-col cols="5">
                  <v-card color="blue darken-3" dark>
                    <v-list-item>
                      <v-layout column align-center xs12 sm10 md6 lg4>
                        <v-avatar size="120" v-if="trabajador.sexo==='M'">
                          <img src="img/default-avatar-man.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="120" v-else-if="trabajador.sexo==='F'">
                          <img src="img/default-avatar-woman.jpg" class="float-center pa-5" />
                        </v-avatar>
                        <v-avatar size="110" v-else>
                          <img src="img/default-avatar.png" class="float-center pa-5" dark />
                        </v-avatar>
                        <v-layout class="pa-2">
                          <v-toolbar-title class="text-capitalize">{{trabajador.nombre_Completo}}</v-toolbar-title>
                        </v-layout>
                      </v-layout>
                    </v-list-item>
                    <v-layout>
                      <v-layout class="pa-2">
                        <v-list-item-content>
                          <v-list-item-title>Carnet de Identidad: {{trabajador.ci}}</v-list-item-title>
                        </v-list-item-content>
                      </v-layout>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-toolbar-items class="text-capitalize">Direccion: {{trabajador.direccion}}</v-toolbar-items>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-toolbar-items
                        class="text-capitalize"
                      >Perfil Ocupacional: {{trabajador.perfil_Ocupacional}}</v-toolbar-items>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-toolbar-items
                        class="text-capitalize"
                      >Unidad Organizativa Actual: {{trabajador.unidadOrganizativa}}</v-toolbar-items>
                    </v-layout>
                    <v-layout class="pa-2">
                      <v-toolbar-items class="text-capitalize">Cargo Actual: {{trabajador.cargo}}</v-toolbar-items>
                    </v-layout>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
        <!-- /Movimientos -->

        <!-- Delete Trajador -->
        <v-dialog v-model="dialog4" persistent max-width="350px">
          <v-toolbar dark fadeOnScroll color="red">
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn icon dark @click="dialog4 = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-card>
            <v-card-title class="headline text-center">Seguro que deseas eliminar al Trabajador</v-card-title>
            <v-card-text class="text-center">{{trabajador.nombre_Completo}}</v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red" dark @click="deleteItem(trabajador)">Aceptar</v-btn>
              <v-btn color="primary" @click="close()">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- /Delete Trajador -->
      </v-toolbar>
    </template>
    <template v-slot:item.action="{ item }">
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="editItem(item)">mdi-pencil</v-icon>
        </template>
        <span>Editar</span>
      </v-tooltip>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon
            small
            class="mr-2"
            v-on="on"
            @click="getDetallesTrabFromApi(item)"
          >mdi-account-plus</v-icon>
        </template>
        <span>Detalles</span>
      </v-tooltip>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon small class="mr-2" v-on="on" @click="movimiento(item)">mdi-walk</v-icon>
        </template>
        <span>Movimiento</span>
      </v-tooltip>
    </template>
  </v-data-table>
</template>
<script>
import api from '@/api';

export default {
  data: () => ({
    dialog: false,
    dialog1: false,
    dialog3: false,
    dialog4: false,
    dialog5: false,
    dialog6: false,
    volver: false,
    search: '',
    editedIndex: -1,
    trabajadores: [],
    trabajador: {},
    unidadesOrganizativas: [],
    cargos: [],
    sexos: [],
    coloresdePiel: [],
    coloresdeOjos: [],
    tallasDeCamisas: [],
    nivelesEscolaridad: [],
    estados: [],
    PerfilesOcupacionales: [],
    Municipios: [],
    unidadOrganizativa: "",
    edad: "",
    cargo: "",
    nivelDeEscolaridad: "",
    edadDesde: "",
    edadHasta: "",
    otrasCaracteristicas: "",
    perfil: "",
    municipio: "",
    estado: "",
    date: new Date().toISOString().substr(0, 10),
    menu: false,
    menu1: false,
    menu2: false,
    modal: false,
    tabs: null,
    CausasDeBajas: [],
    traslado: {
      trabajadorId: '',
      fechaTraslado: '',
      cargoOrigenId: '',
      cargoDestinoId: '',
      unidOrgOrigenId: '',
      unidOrgDestinoId: '',
    },
    baja: {
      trabajadorId: '',
      fechaBaja: '',
      causaDeBaja: '',
    },
    errors: [],
    headers: [
      {
        text: 'Nombre y Apellidos',
        align: 'left',
        sortable: true,
        value: 'nombre_Completo',
      },
      { text: 'Carnet de Identidad', value: 'ci' },
      { text: 'Dirección', value: 'direccion' },
      { text: 'Sexo', value: 'sexoName' },
      { text: 'Cargo', value: 'cargo' },
      { text: 'Edad', value: 'edad' },
      { text: 'Estado', value: 'estadoTrabajadorName' },
      { text: 'Acciones', value: 'action', sortable: false },
    ],
    funciones: [
      {
        text:
          'Participa en el establecimiento de las distintas fuentes de información',
      },
      {
        text: 'Participa en la elaboración de los conceptos',
      },
      {
        text: 'Participa en las acciones de capacitación',
      },
    ],
    requisitos: [
      {
        text: 'Graduado de Nivel Medio Superior con entrenamiento en el puesto',
      },
    ],
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? 'Nuevo Trabajador' : 'Editar Trabajador';
    },
    method() {
      return this.editedIndex === -1 ? 'POST' : 'PUT';
    },
  },

  watch: {
    dialog(val) {
      val || this.close();
    },
  },

  created() {
    this.getTrabajadoresFromApi();
    this.getUnidadOrganizativaFromApi();
    this.getCargosFromApi();
    this.getSexoTrabFromApi();
    this.getColordePielTrabFromApi();
    this.getnivelEscolaridadTrabFromApi();
    this.getEstadosTrabFromApi();
    this.getColordeOjosTrabFromApi();
    this.gettallasDeCamisasFromApi();
    this.getCausaDeBajaFromApi();
    this.getPerfilesOcupacionalesFromApi();
    this.getMunicipiosFromApi();
  },

  methods: {
    getTrabajadoresFromApi() {
      const url = api.getUrl('recursos_humanos', 'Trabajadores');
      this.axios.get(url).then(
        (response) => {
          this.trabajadores = response.data;
          this.volver = false;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getDetallesTrabFromApi(item) {
      this.trabajador = Object.assign({}, item);
      this.dialog3 = true;
    },
    closeDetalles() {
      this.dialog3 = false;
    },
    getFiltrosFromApi() {
      const url = api.getUrl('recursos_humanos', 'Trabajadores/Filtro');
      this.axios
        .get(url, {
          params: {
            unidadOrganizativa: this.unidadOrganizativa,
            cargo: this.cargo,
            sexo: this.sexo,
            estado: this.estado,
            nivelDeEscolaridad: this.nivelDeEscolaridad,
            colordePiel: this.colordePiel,
            edadDesde: this.edadDesde,
            edadHasta: this.edadHasta,
            perfilOcupacional: this.perfil,
            municipio:this.municipio
          }
        })
        .then(
          (response) => {
            this.trabajadores = response.data;
            this.dialog1 = false;
            this.volver = true;
          },
          (error) => {
            console.log(error);
          },
        );
    },
    getUnidadOrganizativaFromApi() {
      const url = api.getUrl('recursos_humanos', 'UnidadOrganizativa');
      this.axios.get(url).then(
        (response) => {
          this.unidadesOrganizativas = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getCargosFromApi() {
      const url = api.getUrl('recursos_humanos', 'Cargos');
      this.axios.get(url).then(
        (response) => {
          this.cargos = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getSexoTrabFromApi() {
      const url = api.getUrl('recursos_humanos', 'CaracteristicasTrab/Sexo');
      this.axios.get(url).then(
        (response) => {
          this.sexos = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getColordePielTrabFromApi() {
      const url = api.getUrl(
        'recursos_humanos',
        'CaracteristicasTrab/ColordePiel',
      );
      this.axios.get(url).then(
        (response) => {
          this.coloresdePiel = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getColordeOjosTrabFromApi() {
      const url = api.getUrl(
        'recursos_humanos',
        'CaracteristicasTrab/ColordeOjos',
      );
      this.axios.get(url).then(
        (response) => {
          this.coloresdeOjos = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getnivelEscolaridadTrabFromApi() {
      const url = api.getUrl(
        'recursos_humanos',
        'CaracteristicasTrab/nivelEscolaridad',
      );
      this.axios.get(url).then(
        (response) => {
          this.nivelesEscolaridad = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    gettallasDeCamisasFromApi() {
      const url = api.getUrl(
        'recursos_humanos',
        'CaracteristicasTrab/TallaDeCamisa',
      );
      this.axios.get(url).then(
        (response) => {
          this.tallasDeCamisas = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getEstadosTrabFromApi() {
      const url = api.getUrl('recursos_humanos', 'CaracteristicasTrab/Estados');
      this.axios.get(url).then(
        (response) => {
          this.estados = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getCausaDeBajaFromApi() {
      const url = api.getUrl('recursos_humanos', 'Baja/CausaDeBaja');
      this.axios.get(url).then(
        (response) => {
          this.CausasDeBajas = response.data;
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getPerfilesOcupacionalesFromApi() {
      const url = api.getUrl("recursos_humanos", "PerfilesOcupacionales");
      this.axios.get(url).then(
        response => {
          this.PerfilesOcupacionales = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    getMunicipiosFromApi() {
      const url = api.getUrl("recursos_humanos", "Trabajadores/Municipios");
      this.axios.get(url).then(
        response => {
          this.Municipios = response.data;
        },
        error => {
          console.log(error);
        }
      );
    },
    save(method) {
      const url = api.getUrl('recursos_humanos', 'Trabajadores');
      if (method === 'POST') {
        this.axios.post(url, this.trabajador).then(
          (response) => {
            this.getResponse(response);
            this.dialog = false;
          },
          (error) => {
            console.log(error);
          },
        );
      }
      if (method === "PUT") {
        this.axios.put(`${url}/${this.trabajador.id}`, this.trabajador).then(
          response => {
            this.getResponse(response);
            this.dialog = false;
          },
          (error) => {
            console.log(error);
          },
        );
      }
    },
    Entrada() {
      const url = api.getUrl('recursos_humanos', 'Entradas');
      this.axios.post(url, this.entrada).then(
        (response) => {
          this.getResponse(response);
          this.entrada = {
            trabajadorId: '',
            fechaEntrada: '',
            unidadOrganizativaId: '',
            cargoId: '',
          };
          this.dialog5 = false;
          this.getTrabajadoresFromApi();
        },
        (error) => {
          console.log(error);
        },
      );
    },
    movimiento(item) {
      this.editedIndex = this.trabajadores.indexOf(item);
      this.trabajador = Object.assign({}, item);
      this.traslado.trabajadorId = this.trabajador.id;
      this.baja.trabajadorId = this.trabajador.id;
      this.dialog5 = true;
    },
    Traslado() {
      this.traslado.cargoOrigenId = this.trabajador.cargoId;
      this.traslado.unidOrgOrigenId = this.trabajador.unidadOrganizativaId;
      const url = api.getUrl('recursos_humanos', 'Traslados');
      if (this.traslado.cargoDestinoId == '') {
        vm.$snotify.error('El campo Cargo a Ocupar es obligatorio');
      }
      if (this.traslado.unidOrgDestinoId == '') {
        vm.$snotify.error('El campo Unidad Organizativa es obligatorio');
      }
      if (this.traslado.fechaEntrada == '') {
        vm.$snotify.error('El campo Fecha es obligatorio');
      } else {
        this.axios.post(url, this.traslado).then(
          (response) => {
            this.getResponse(response);
            this.traslado = {
              trabajadorId: '',
              fechaTraslado: '',
              cargoDestinoId: '',
            };
            this.dialog5 = false;
            this.getTrabajadoresFromApi();
          },
          (error) => {
            console.log(error);
          },
        );
      }
    },
    clearTraslado() {
      this.traslado = {
        fechaTraslado: '',
        cargoDestinoId: '',
      };
    },
    confirmBaja() {
      this.dialog6 = true;
      this.baja = {
        trabajadorId: this.trabajador.id,
        causaDeBaja: this.causaDeBaja,
        fechaBaja: this.fechaBaja,
      };
    },
    saveBaja() {
      const url = api.getUrl('recursos_humanos', 'Bajas');
      this.axios.post(url, this.baja).then(
        (response) => {
          this.getResponse(response);
          this.dialog6 = false;
          this.dialog5 = false;
          this.getTrabajadoresFromApi();
        },
        (error) => {
          console.log(error);
        },
      );
    },
    clearBaja() {
      this.baja = {
        causaDeBaja: '',
        fechaBaja: '',
      };
    },
    editItem(item) {
      this.editedIndex = this.trabajadores.indexOf(item);
      this.trabajador = Object.assign({}, item);
      this.dialog = true;
    },
    confirmDelete(item) {
      this.trabajador = Object.assign({}, item);
      this.dialog4 = true;
    },
    deleteItem(trabajador) {
      const url = api.getUrl("recursos_humanos", "Trabajadores");
      this.axios.delete(`${url}/${trabajador.id}`).then(
        response => {
          this.getResponse(response);
        },
        (error) => {
          console.log(error);
        },
      );
    },
    getResponse(response) {
      if (response.status === 200 || response.status === 201) {
        vm.$snotify.success('Exito al realizar la operación');
        this.getTrabajadoresFromApi();
        this.trabajador = [];
      }
    },
    close() {
      this.dialog = false;
      this.dialog1 = false;
      this.dialog3 = false;
      this.dialog4 = false;
      this.dialog5 = false;
      this.dialog6 = false;
      this.trabajador = {};
      this.getTrabajadoresFromApi();
      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },
  },
};
</script>
