<template>
  <v-container fluid>
    <v-row justify="space-between">
      <v-col cols="12" sm="2">
        <v-select
          v-model="options.itemsPerPage"
          :items="pagination.limits"
        ></v-select>
      </v-col>
      <v-col cols="12" sm="6">
        <v-pagination
          class="py-3"
          color="info"
          v-model="options.page"
          :length="pagination.length"
          :next-icon="'mdi-arrow-right'"
          :prev-icon="'mdi-arrow-left'"
          :total-visible="pagination.totalVisible || 7"
        ></v-pagination>
      </v-col>
    </v-row>
  </v-container>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

interface Pagination {
  limits: number[];
  defaultLimit?: number;
  length: number;
  totalVisible?: number;
}

interface Options {
  page: number;
  itemsPerPage: number;
  sortBy: string[];
  sortDesc: boolean[];
  groupBy: string[];
  groupDesc: boolean[];
  multiSort: boolean;
  mustSort: boolean;
}

@Component({ name: "TableFooter" })
export default class TableFooter extends Vue {
  @Prop() public pagination!: Pagination;
  @Prop() public options!: Options;

  private selectedLimit: number =
    this.pagination.defaultLimit || this.pagination.limits[0];
}
</script>
