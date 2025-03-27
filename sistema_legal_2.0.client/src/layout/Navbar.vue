<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <!-- Navbar -->
  <nav navbar class="main-header navbar navbar-expand navbar-white navbar-light p-2" >
    <!-- Left navbar links -->
    <ul class="navbar-nav">
      <li class="nav-item">
        <a class="nav-link" data-widget="pushmenu" href="#" role="button">
          <i class="fas fa-bars"></i>
        </a>
      </li>
      <li class="nav-item d-none d-sm-inline-block">
        <strong class="nav-link text-dark">{{ router.currentRoute.value.name }}</strong>
      </li>
    </ul>
    <!-- Right navbar links -->
    <ul class="navbar-nav ml-auto" v-if="$store.state.user.idUsuario">
      <li class="nav-item">
        <button @click="profileClick" class="w-full p-link flex align-items-center p-1 px-2 text-color hover:surface-200 border-round">
          <Avatar :label="$store.state.user.nombres[0] + $store.state.user.apellidos[0]" class="mr-2" shape="circle" style="background-color:#003880; color: #ffffff" />
          <div class="flex flex-column align">
            <span class="font-bold text-base">{{ getFirstLastName($store.state.user.nombres, $store.state.user.apellidos) }}</span>
            <span class="text-xs">{{ $store.state.user.nombrePerfil }}</span>
          </div>
        </button>
        <Menu ref="menu" :model="items" :popup="true" />
      </li>
    </ul>
    <ul class="navbar-nav ml-auto" v-else>
      <li class="nav-item">
        <button class="w-full p-link flex align-items-center p-1 px-3 text-color hover:surface-200 border-round gap-2">
          <Avatar label="N/A" shape="circle" style="background-color:#003880; color: #ffffff" />
          <div class="flex flex-column align">
            <span class="font-bold text-base">Anónimo</span>
            <span class="text-xs">Cargando...</span>
          </div>
        </button>
        <Menu ref="menu" :model="items" :popup="true" />
      </li>
    </ul>
  </nav>
  <!-- /.navbar -->
</template>


<script setup>
import { push } from 'notivue';
import { ref } from "vue";
import router from '@/router';
import { useStore } from "vuex";

const store = useStore();
const menu = ref();
const items = ref([
  {
    label: 'Bloquear pantalla',
    icon: 'fas fa-user-lock',
    command: () => {
      localStorage.removeItem("token");
      localStorage.removeItem('sessionExpireTime');
      store.commit('setScreenLocked', true);
    }
  },
  {
    label: 'Cerrar sesión',
    icon: 'pi pi-sign-out',
    command: () => {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      localStorage.removeItem('sessionExpireTime');
      store.commit('setUser', {});
      store.commit('setViews', []);
      push.success('Sesión cerrada con éxito');
      router.push('/Login');
    }
  }
]);

const profileClick = (event) => {
  menu.value.toggle(event);
};

const getFirstLastName = (Nombres, Apellidos) => {
  if (Nombres != null && Apellidos != null) {
    let firstName = Nombres.split(" ");
    firstName = firstName[0];
    let lastName = Apellidos.split(" ");
    lastName = lastName[0];
    return firstName + " " + lastName;
  }
}


</script>
<style>
.main-header {
  z-index: 400 !important;
}

.p-menu-list {
  margin: 0px;
  font-size: 0.95rem;
}
</style>
