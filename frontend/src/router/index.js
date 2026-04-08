import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore.js'

const routes = [
  {
    path: '/',
    name: 'home',
    component: () => import('../views/HomeView.vue')
  },
  {
    path: '/calculator',
    name: 'calculator',
    component: () => import('../views/CalculatorView.vue')
  },
  {
    path: '/auctions',
    name: 'auctions',
    component: () => import('../views/AuctionsView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/auctions/:id',
    name: 'auction-detail',
    component: () => import('../views/AuctionDetailView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('../views/LoginView.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, _from, next) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    next({ name: 'login', query: { redirect: to.fullPath } })
  } else {
    next()
  }
})

export default router
