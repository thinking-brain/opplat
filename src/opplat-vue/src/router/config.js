import {
  AuthLayout,
  DefaultLayout,
} from '@/components/layouts';
// import {
//   UserList
// } from "@/components/admin/UserList";

export const publicRoute = [
  {
    path: '*',
    component: () => import(/* webpackChunkName: "errors-404" */ '@/views/error/NotFound.vue'),
  },
  {
    path: '/auth',
    component: AuthLayout,
    meta: {
      title: 'Login',
    },
    redirect: '/auth/login',
    hidden: true,
    children: [{
      path: 'login',
      name: 'login',
      meta: {
        title: 'Login',
      },
      component: () => import(/* webpackChunkName: "login" */ '@/views/auth/Login.vue'),
    }],
  },

  {
    path: '/404',
    name: '404',
    meta: {
      title: 'Not Found',
    },
    component: () => import(/* webpackChunkName: "errors-404" */ '@/views/error/NotFound.vue'),
  },

  {
    path: '/500',
    name: '500',
    meta: {
      title: 'Server Error',
    },
    component: () => import(/* webpackChunkName: "errors-500" */ '@/views/error/Error.vue'),
  },
  {
    path: '/403',
    name: 'Denegado',
    meta: {
      title: 'Acceso denegado',
      hiddenInMenu: true,
    },
    component: () => import(/* webpackChunkName: "error-403" */ '@/views/error/Deny.vue'),
  },
];

export const protectedRoute = [
  {
    path: '/',
    component: DefaultLayout,
    meta: {
      title: 'Home',
      group: 'apps',
      requiresAuth: true,
      icon: '',
    },
    redirect: '/dashboard',
    children: [{
      path: '/dashboard',
      name: 'Dashboard',
      meta: {
        title: 'Home',
        requiresAuth: true,
        group: 'apps',
        icon: 'dashboard',
      },
      component: () => import(/* webpackChunkName: "dashboard" */ '@/components/HelloWorld.vue'),
    },
    ],
  },

  // usuarios
  {
    name: 'Administracion',
    path: '/admin',
    component: DefaultLayout,
    redirect: '/admin/usuarios',
    meta: {
      title: 'Admin',
      icon: 'view_compact',
      group: 'admin',
    },
    children: [{
      path: '/admin/usuarios',
      name: 'Usuarios',
      meta: {
        title: 'Usuarios',
        requiresAuth: true,
        roles: ['administrador'],
      },
      component: () => import(/* webpackChunkName: "table" */ '@/components/admin/UserList.vue'),
    }],
  },
  // nuevo usuario
  {
    name: 'nuevo',
    path: '/admin/usuarios',
    component: DefaultLayout,
    redirect: '/admin/usuarios/nuevo',
    meta: {
      title: 'Admin',
      icon: 'view_compact',
      group: 'admin',
    },
    children: [{
      path: '/admin/usuarios/nuevo',
      name: 'Nuevo Usuario',
      meta: {
        title: 'Nuevo Usuario',
        requiresAuth: true,
        roles: ['administrador'],
      },
      component: () => import(/* webpackChunkName: "table" */ '@/components/admin/NuevoUsuario.vue'),
    }],
  },
  // gestion de roles
  {
    name: 'gestionar-roles',
    path: '/admin/usuarios',
    component: DefaultLayout,
    redirect: '/admin/usuarios/gestionar-roles',
    meta: {
      title: 'Admin',
      icon: 'view_compact',
      group: 'admin',
    },
    children: [{
      path: '/admin/usuarios/gestionar-roles',
      name: 'Gestionar Roles',
      props: route => ({
        usuario: route.query.usuario,
      }),
      meta: {
        title: 'Gestionar Roles',
        requiresAuth: true,
        roles: ['administrador'],
      },
      component: () => import(/* webpackChunkName: "roles" */ '@/components/admin/GestionRoles.vue'),
    }],
  },
  // licencia
  {
    name: 'licencia',
    path: '/admin',
    component: DefaultLayout,
    redirect: '/admin/licencia',
    meta: {
      title: 'Licencia',
      icon: 'view_compact',
      group: 'admin',
    },
    children: [{
      path: '/admin/licencia',
      name: 'Licencia',
      meta: {
        title: 'Licencia',
        requiresAuth: true,
        roles: ['administrador'],
      },
      component: () => import(/* webpackChunkName: "roles" */ '@/components/admin/Licencia.vue'),
    }],
  },
  // configuracion
  {
    name: 'configuracion',
    path: '/admin',
    component: DefaultLayout,
    redirect: '/admin/configuracion',
    meta: {
      title: 'Admin',
      icon: 'view_compact',
      group: 'admin',
    },
    children: [{
      path: '/admin/configuracion',
      name: 'Configuracion',
      meta: {
        title: 'Configuracion',
        requiresAuth: true,
        roles: ['administrador'],
      },
      component: () => import(/* webpackChunkName: "roles" */ '@/components/admin/Configuracion.vue'),
    }],
  },
  // perfil de usuario
  {
    name: 'Account',
    path: '/account',
    component: DefaultLayout,
    redirect: '/account/perfil',
    meta: {
      title: 'PerfilDeUsuario',
      group: 'account',
    },
    children: [{
      path: '/account/perfil',
      name: 'PerfilDeUsuario',
      meta: {
        title: 'Perfil de Usuario',
        requiresAuth: true,
      },
      component: () => import(/* webpackChunkName: "table" */ '@/views/auth/PerfilDeUsuario.vue'),
    }],
  },
  // Reporte Ingresos y Gastos
  {
    name: 'Finanzas',
    path: '/finanzas',
    component: DefaultLayout,
    redirect: '/finanzas/ingresos_gastos',
    meta: {
      title: 'ReporteIngresosGastos',
      group: 'finanzas',
    },
    children: [{
      path: '/finanzas/ingresos_gastos',
      name: 'ReporteIngresosGastos',
      meta: {
        title: 'Reporte de Ingresos y Gastos',
        requiresAuth: true,
      },
      component: () => import(/* webpackChunkName: "table" */ '@/components/finanzas/reportes/IngresosGastos.vue'),
    }],
  },

];
