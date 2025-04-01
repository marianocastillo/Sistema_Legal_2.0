import { createStore } from 'vuex'
import api from '@/utilities/api';
import { push } from "notivue";

export default createStore({
  state: {
    loading: false,
    user: {},
    views: [],
    isMobile: false,
    screenLocked: false,
  },
  mutations: {
    setLoading(state, loading) {
      state.loading = loading;
    },
    setScreenLocked(state, screenLocked) {
      state.screenLocked = screenLocked;
    },
    setUser(state, user) {
      state.user = user;
    },
    setViews(state, views) {
      state.views = views;
    },
    setIsMobile(state, isMobile) {
      state.isMobile = isMobile;
    },
  },
  actions: {
    fetchUser({ commit }) {
      api.get("/api/Account")
        .then(response => {
          commit('setUser', response.data.usuario);         
        })
        .catch(error => {
          push.error(error);
        });
    }
  },
  getters: {
    isLoading: state => state.loading,
    getUser: state => state.user,
    getViews: state => state.views,
    isMobile: state => state.isMobile,
    isScreenLocked: state => state.screenLocked,
  }
});