<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex lg12>
        <v-card v-if="usuario" class="mx-auto my-12">
          <v-card-title>Perfil de usuario</v-card-title>
          <v-card-text>
            <div class="my-4 subtitle-1 black--text">Nombre y Apellidos</div>
            <div>{{ usuario.nombres }} {{ usuario.apellidos }}</div>
          </v-card-text>
          <v-card-text>
            <div class="my-4 subtitle-1 black--text">Nombre de usuario</div>
            <div>{{ usuario.username }}</div>
          </v-card-text>
          <v-card-actions>
            <CambiarPassword v-bind:usuario="usuario"></CambiarPassword>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import api from "@/api";
import CambiarPassword from "@/views/auth/CambiarPassword.vue";

export default {
  components: { CambiarPassword },
  data: () => ({
    dialog: false,
    usuario: null,
  }),
  computed: {},
  created() {
    const username = this.$store.getters.usuario;
    const url = api.getUrl("api-account", `account/perfil?usuario=${username}`);
    this.axios
      .get(url)
      .then(
        (
          p // console.log(p)
        ) => {
          this.usuario = p.data;
        }
      )
      .catch((e) => {
        this.$toast.error(e.response.data.errors);
      });
  },
  watch: {
    nombres() {
      this.errorMessages = [];
    },
  },

  methods: {},
};
</script>
<style></style>
