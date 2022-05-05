import modulos from './menus/modulos.json';
import administracion from './menus/modulos/administracion.json';
import inventario from './menus/modulos/inventario.json';

export default {
    getModulos() {
        return modulos;
    },
    getMenusByModulo(item) {
        if (item == 'administracion') {
            return administracion
        }
        if (item == 'inventario') {
            return inventario
        }
        return [];
    },

};
