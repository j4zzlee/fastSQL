import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      component: () => import('@/components/_Layout'),
      children: [
        {
          path: '',
          name: 'home',
          component: () => import('@/components/Index')
        },
        {
          path: '/global-connections',
          name: 'global-connections',
          component: () => import('@/components/Connections/Index')
        },
        {
          path: '/entities',
          name: 'entities',
          component: () => import('@/components/Entities/Index')
        },
        {
          path: '/entities/:id/edit',
          name: 'edit-entity',
          component: () => import('@/components/Entities/EditEntity')
        },
        {
          path: '/entities/:id/manage',
          name: 'manage-entity',
          component: () => import('@/components/Entities/ManageEntity')
        },
        {
          path: '/attributes',
          name: 'attributes',
          component: () => import('@/components/Attributes/Index')
        },
        {
          path: '/attributes/:id/edit',
          name: 'edit-attribute',
          component: () => import('@/components/Attributes/EditAttribute')
        },
        {
          path: '/attributes/:id/manage',
          name: 'manage-attribute',
          component: () => import('@/components/Attributes/ManageAttribute')
        }
      ]
    },
    {
      path: '*',
      redirect: '/'
    }
  ]
})
