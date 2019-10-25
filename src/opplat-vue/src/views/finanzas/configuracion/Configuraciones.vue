<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex sm12>
        <h3>Configuraciones de finanzas</h3>
      </v-flex>
      <v-flex lg12>
        <v-card ref="form">
          <v-card-text>
            <v-text-field
              label="Porciento Contingencia"
              v-model="porcientoContingencia"
              required
              type="number"
              min="2"
              max="10"
              ref="porcientoContingencia"
              :rules="[() => !!porcientoContingencia || 'Este campo es obligatorio.']"
            ></v-text-field>
            <v-text-field
              label="Nombre Jefe"
              v-model="nombreJefe"
              required
              ref="nombreJefe"
              :rules="[() => !!nombreJefe || 'Este campo es obligatorio.']"
            ></v-text-field>
            <v-text-field
              label="Cargo Jefe"
              v-model="cargoJefe"
              required
              ref="cargoJefe"
              :rules="[() => !!cargoJefe || 'Este campo es obligatorio.']"
            ></v-text-field>
            <v-text-field
              label="Nombre Economico"
              v-model="nombreEconomico"
              required
              ref="nombreEconomico"
              :rules="[() => !!nombreEconomico || 'Este campo es obligatorio.']"
            ></v-text-field>
            <v-text-field
              label="Cargo Economico"
              v-model="cargoEconomico"
              required
              ref="cargoEconomico"
              :rules="[() => !!cargoEconomico || 'Este campo es obligatorio.']"
            ></v-text-field>
          </v-card-text>
          <v-divider class="mt-5"></v-divider>
          <v-card-actions>
            <v-btn color="primary" @click="submit">Guardar</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>
<script>
import api from '@/api';

export default {
  data: () => ({    
    porcientoContingencia: null,
    nombreJefe: null,
    cargoJefe: null,
    nombreEconomico: null,
    cargoEconomico: null,    
    rules: {
      required: value => !!value || 'Obligatorio.',      
    },
    formHasErrors: false,
    errors: [],
  }),
  computed: {
    
  },
  created() {
    const url = api.getUrl('finanzas', 'configuraciones');
        this.axios
          .get(url)
          .then((resp) => {
            const configs = resp.data;
            this.porcientoContingencia = configs.find(config => config.nombre === 'PorcientoContingencia').valor;
            this.nombreJefe = configs.find(config => config.nombre === 'NombreJefe').valor;
            this.cargoJefe = configs.find(config => config.nombre === 'CargoJefe').valor;
            this.nombreEconomico = configs.find(config => config.nombre === 'NombreEconomico').valor;
            this.cargoEconomico = configs.find(config => config.nombre === 'CargoEconomico').valor;
          })
          .catch((e) => {
            vm.$snotify.error(e.response.data.errors);
          });    
  },
  watch: {    
  },

  methods: {   
    submit() {      
      if (!this.formHasErrors) {
        let errorCount = 0;        
        const url = api.getUrl('finanzas', 'configuraciones');
        let form = {
          Nombre : "PorcientoContingencia",
          Valor: this.porcientoContingencia
        };
        this.axios
          .put(url+ '/PorcientoContingencia', form)
          .then((
            p, // console.log(p)
          ) => {            
          })
          .catch((e) => {
            errorCount ++;
            this.errors.push(e.response.data.errors);
          });
        
        form = {
          Nombre : "NombreJefe",
          Valor: this.nombreJefe
        };
        this.axios
          .put(url+ '/NombreJefe', form)
          .then((
            p, // console.log(p)
          ) => {            
          })
          .catch((e) => {
            errorCount ++;
            this.errors.push(e.response.data.errors);
          });
        
        form = {
          Nombre : "CargoJefe",
          Valor: this.cargoJefe
        };
        this.axios
          .put(url+ '/CargoJefe', form)
          .then((
            p, // console.log(p)
          ) => {            
          })
          .catch((e) => {
            errorCount ++;
            this.errors.push(e.response.data.errors);
          });
        form = {
          Nombre : "NombreEconomico",
          Valor: this.nombreEconomico
        };
        this.axios
          .put(url+ '/NombreEconomico', form)
          .then((
            p, // console.log(p)
          ) => {            
          })
          .catch((e) => {
            errorCount ++;
            this.errors.push(e.response.data.errors);
          });
        form = {
          Nombre : "CargoEconomico",
          Valor: this.cargoEconomico
        };
        this.axios
          .put(url+ '/CargoEconomico', form)
          .then((
            p, // console.log(p)
          ) => {            
          })
          .catch((e) => {
            errorCount ++;
            this.errors.push(e.response.data.errors);
          });       

        if(errorCount > 0){
          vm.$snotify.error(this.errors);
        } else{
          vm.$snotify.success('Configuraciones guardadas correctamente, ya puede salir.');
        }
      }
    },
  },
};
</script>
<style></style>
