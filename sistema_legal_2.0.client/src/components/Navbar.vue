<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <div ng-if="" class="flex justify-content-between p-2 bg-white align-items-center navbar" >
    <div class="flex justify-content-between align-items-center">

      <!-- <strong class="">Sistema de Carga de Ejecución del Gasto</strong> -->
    </div>
    <div class="text-xl font-bold" style="color: #003880;">Sistema de Litigio</div>
      <!-- <div v-if="$store.state.user.idUsuario">
        <Button @click="profileClick" class="w-full p-link flex align-items-center p-1 px-2 text-color hover:surface-200 border-rounded" text severity="secondary">
          <Avatar :label="$store.state.user.nombres[0] + $store.state.user.apellidos[0]" class="mr-2" shape="circle" style="background-color:#003880; color: #ffffff" />
          <div class="flex flex-column align">
            <span class="font-bold" style="color: #003880;">{{ getFirstLastName($store.state.user.nombres, $store.state.user.apellidos) }}</span>
            <span class="text-xs">{{ $store.state.user.nombrePerfil }}</span>
          </div>

          <Menu ref="menu" :model="items" :popup="true" />
        </Button>
      </div> -->
  </div>
</template>


<script setup>
import { push } from 'notivue';
import { ref } from "vue";
import router from '../router/router';
import { useStore } from "vuex";

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

.logoNav {
  height: 40px;
}

.p-menu-list {
  margin: 0px;
}
</style>
