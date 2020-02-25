<template>
  <hot-table :data="data1" licenseKey="non-commercial-and-evaluation">
  </hot-table>
</template>

<script>
import { HotTable, HotColumn } from '@handsontable/vue';
import Handsontable from 'handsontable';

export default {
  props: {
    data: {
      type: Array,
    },
  },
  data() {
    return {
      data1: this.data,
    };
  },
  components: {
    HotTable,
  },
};
</script>
<style src="handsontable/dist/handsontable.full.css"></style>
