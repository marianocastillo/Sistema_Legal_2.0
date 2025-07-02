<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <div class="navbar">
    <div class="d-flex align-items-center">
      <!-- Puedes poner un logo o texto aquí si lo deseas -->
    </div>

    <div class="text-xl fw-bold" style="color: #003880;">
      Sistema Sileg 2.0
    </div>

    <div v-if="$store.state.user.idUsuario" class="d-flex align-items-center">
      <Button
        @click="profileClick"
        class="p-link flex align-items-center border-0"
        style="background: transparent; box-shadow: none;"
      >
        <Avatar
          :label="$store.state.user.nombres[0] + $store.state.user.apellidos[0]"
          class="me-2"
          shape="circle"
          style="background-color:#003880; color: #ffffff"
        />
        <div class="flex flex-column text-start">
          <span class="fw-bold" style="color: #003880;">
            {{ getFirstLastName($store.state.user.nombres, $store.state.user.apellidos) }}
          </span>
          <span class="text-xs text-muted">
            {{ $store.state.user.nombrePerfil }}
          </span>
        </div>
        <Menu ref="menu" :model="items" :popup="true" />
      </Button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useStore } from 'vuex';
import router from '../router/router';
import { push } from 'notivue';

const store = useStore();
const menu = ref();

const items = ref([
  {
    label: 'Manejo de usuarios',
    icon: 'pi pi-user-edit',
    command: () => {
      router.push('/Configuracion/Usuarios');
    }
  },
   {
    label: 'Manejo de Tribuanles',
    icon: 'pi pi-user-edit',
    command: () => {
      router.push('/Configuracion/Usuarios');
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
      push.success('Sesión cerrada con éxito');
      router.push('/Login');
    }
  }
]);

const profileClick = (event) => {
  menu.value.toggle(event);
};

const getFirstLastName = (Nombres, Apellidos) => {
  if (Nombres && Apellidos) {
    return `${Nombres.split(' ')[0]} ${Apellidos.split(' ')[0]}`;
  }
  return '';
};
</script>

<style>
.navbar {
  height: 64px;
  background-color: #fff;
  border-bottom: 1px solid #ddd;
  display: flex;
  font-family: Arial, sans-serif;
  align-items: center;
  justify-content: space-between;
  padding: 0 1rem;
  position: sticky;
  top: 0;
  z-index: 10;
}
</style>
