import apis from './api-mapper.json';

export default {
  getUrl(servicio, endpoint) {
    const servicios = apis;
    if (!servicios[servicio]) {
      vm.$snotify.error(`No existe el servicio '${servicio}' en la configuracion.`);
      return null;
    }
    return servicios[servicio] + endpoint;
  },
};
