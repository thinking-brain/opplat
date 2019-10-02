<template>
  <v-container>
    <v-form>
      <v-row>
        <v-file-input show-size label="Seleccionar fichero" v-model="planFile"></v-file-input>
        <v-col cols="12" md="4">
          <v-btn color="success" class="mr-4" @click="GenerarPlan">Generar Plan</v-btn>
        </v-col>
      </v-row>
    </v-form>
  </v-container>
</template>
      </v-row>
    </v-form>
  </v-container>
</template>
<script>
import api from "@/api";
export default {
  data: () => ({
    planFile: null
  }),
  methods: {
    GenerarPlan() {
      if (this.planFile != null) {
        var formData = new FormData();
        formData.append("File", this.planFile);
        const url = api.getUrl("contabilidad", "PlanIG/UploadPlanGI");
        this.axios.post(url, formData, {
          headers: {
            "Content-Type": "multipart/form-data"
          }
        }).then(
          response => {
            console.log(response);
          },
          error => {
            console.log(error);
          }
        );
      }
    }
  }
};
</script>
