import Vue from 'vue';
import Router from 'vue-router';
import NProgress from 'nprogress';
import {
  publicRoute,
  protectedRoute,
} from './config';
import store from '../store/index';
import 'nprogress/nprogress.css';

const routes = publicRoute.concat(protectedRoute);

Vue.use(Router);
const router = new Router({
  mode: 'hash',
  linkActiveClass: 'active',
  routes,
});
// router gards
router.beforeEach((to, from, next) => {
  NProgress.start();
  // console.log(to);
  if (!to.matched.some(record => record.meta.requiresAuth)) {
    // console.log("sin autenticacion");
    next();
    return;
  }
  if (!store.getters.isLoggedIn) {
    // console.log("no logueado");
    next('/auth/login');
    return;
  }
  if ((to.name != 'Licencia' || to.path != '/admin/licencia') && (to.name != 'login' || to.path != '/auth/login')) {
    if (!store.getters.hasLicence) {
      // console.log("sin licencia");
      next({ name: 'Licencia' });
      return;
    }
  }
  // auth route is authenticated
  // redirect to login page if not logged in and trying to access a restricted page
  // todo: redireccionar cuando se autentique para la pagina de donde vino la peticion

  if (to.meta.roles) {
    const roles_count = to.meta.roles.filter(value => store.getters.roles.includes(value));
    if (roles_count.length > 0) {
      next();
      return;
    }
    // console.log("sin roles");
    next('/403');
    return;
  }
  next();
});

router.afterEach((to, from) => {
  NProgress.done();
});

export default router;
