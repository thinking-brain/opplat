<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex sm12>
        <h3>Licencia</h3>
      </v-flex>
      <v-flex lg12>
        <v-card v-if="licencia">
          <v-card-text>Sistema licenciado a nombre de : {{licencia.subscriptor}}
             hasta : {{licencia.vencimiento}}</v-card-text>
          <v-divider class="mt-5"></v-divider>
          <v-card-actions>
            <v-btn color="secundary" @click="eliminar">Eliminar Licencia</v-btn>
          </v-card-actions>
        </v-card>
        <v-card ref="form" v-if="!licencia">
          <v-card-text>
            <v-file-input
              label="Fichero de licencia"
              v-model="imageFile"
              prepend-icon="mdi-paperclip"
            ></v-file-input>
          </v-card-text>
          <v-divider class="mt-5"></v-divider>
          <v-card-actions>
            <v-btn to="/admin/usuarios">Cancelar</v-btn>
            <v-spacer></v-spacer>
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
    dialog: false,
    show_password: false,
    errorMessages: [],
    errorMessagesPassword: [],
    licencia_file: null,
    rules: {
      required: value => !!value || 'Obligatorio.',
    },
    formHasErrors: false,
    errors: [],
    imageFile: null,
  }),
  computed: {
    form() {
      return {
        licence: this.imageFile,
      };
    },
    licencia() {
      return vm.$store.getters.licencia;
    },
  },
  created() {},
  watch: {
    nombres() {
      this.errorMessages = [];
    },
  },

  methods: {
    eliminar() {
      this.$store
        .dispatch('quitar')
        .then(() => {
          this.licencia = null;
          vm.$snotify.success('Licencia eliminada correctamente.');
        })
        .catch(() => {
          vm.$snotify.error(
            'Error eliminando licencia. Contacte su administrador.',
          );
        });
    },
    submit() {
      this.formHasErrors = false;
      // Object.keys(this.form).forEach(f => {
      //   if (!this.form[f]) this.formHasErrors = true;

      //   this.$refs[f].validate(true);
      // });
      if (!this.formHasErrors) {
        const formData = new FormData();
        formData.append('licence', this.imageFile);
        const url = api.getUrl('opplat-app', 'licencia');
        this.axios
          .post(url, formData, {
            headers: {
              'Content-Type': 'multipart/form-data',
            },
          })
          .then((response) => {
            const lic = response.data;
            this.$store
              .dispatch('agregar', lic)
              .then(() => {})
              .catch(() => {});
            // this.$store
            //   .dispatch("cargar")
            //   .then(() => {
            //     this.licencia = vm.$store.getters.licencia;
            //   })
            //   .catch(err => {
            //     vm.$snotify.error(
            //       "No se pudo cargar la licencia. Contacte su administrador."
            //     );
            //   });
            vm.$snotify.success('Licencia agregada correctamente.');
          })
          .catch(() => {
            // console.log('no debe');
            vm.$snotify.error(
              'Error agregando licencia. Contacte su administrador.',
            );
          });
      }
    },
  },
};
</script>
<style></style>
