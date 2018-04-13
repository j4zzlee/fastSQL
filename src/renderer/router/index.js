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
          path: '/attributes',
          name: 'attributes',
          component: () => import('@/components/Attributes/Index')
        }
      ]
    },
    {
      path: '*',
      redirect: '/'
    }
  ]
})
