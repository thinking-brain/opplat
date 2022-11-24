import Vue from "vue";
import Router from "vue-router";
import Login from "@/views/auth/Login.vue";
import NotFound from "@/views/error/NotFound.vue";
import { AuthLayout, DefaultLayout } from "@/components/layouts";
Vue.use(Router);

async function guardRoute(to, from, next) {
  const token = localStorage.getItem("access_token");
  if (token) {
    next();
  } else {
    next({
      name: "login",
      query: { redirect: to.path },
    });
  }
}

const routerInstance = new Router({
  routes: [
    {
      path: "/auth",
      component: AuthLayout,
      meta: {
        title: "Login",
      },
      redirect: "/auth/login",
      children: [
        {
          path: "login",
          name: "login",
          meta: {
            title: "Login",
          },
          component: Login,
        },
      ],
    },
    {
      path: "/",
      name: "Layout",
      component: DefaultLayout,
      redirect: { name: "Home" },
      children: [
        {
          path: "/home",
          name: "Home",
          component: () => import("@/views/Home.vue"),
          meta: { needGuard: true, modelName: "Home" },
        },
        {
          path: "/users",
          name: "Usuarios",
          component: () => import("@/components/Admin/UserList.vue"),
          meta: { needGuard: true, modelName: "Users" },
        },
        {
          path: "/products",
          name: "Productos",
          component: () => import("@/views/Products.vue"),
          meta: { needGuard: true, modelName: "Products" },
        },
        {
          path: "/sell",
          name: "Vender",
          component: () => import("@/views/Sell.vue"),
          meta: { needGuard: true, modelName: "Sell" },
        },
      ],
    },
    { path: "*", component: NotFound },
  ],
});
routerInstance.beforeEach((to, from, next) => {
  if (to.meta?.needGuard) {
    guardRoute(to, from, next);
  } else {
    next();
  }
});
export default routerInstance;
