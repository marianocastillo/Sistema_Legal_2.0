<template>
  <div class="container-fluid">
    <div class="row flex-nowrap">
      <!-- Sidebar -->
      <div class="col-auto col-md-3 col-xl-2 px-sm-2 px-0 sidebar">
        <div class="d-flex flex-column align-items-start px-3 pt-2 text-white min-vh-100">
          <span class="fs-5 d-none d-sm-inline">Sistema de Litigio</span>
          <br>
          <ul class="nav nav-pills flex-column mb-sm-auto mb-0 w-100" id="menu">
            <li class="nav-item">
              <router-link to="home" class="nav-link text-white w-100">
                <i class="fs-5 bi-house"></i> <span class="ms-2 d-none d-sm-inline">Home</span>
              </router-link>
            </li>
            <li>
              <router-link to="registro" class="nav-link text-white w-100">
                <i class="fs-5 bi-card-checklist"></i> <span class="ms-2 d-none d-sm-inline">Registrar</span>
              </router-link>
            </li>
            <li>
              <router-link to="edit" class="nav-link text-white w-100">
                <i class="fs-5 bi-pencil-square"></i> <span class="ms-2 d-none d-sm-inline">Modificar</span>
              </router-link>
            </li>
          </ul>
        </div>
      </div>

      <!-- Contenido principal -->
      <div class="col py-3">
        <router-view />
      </div>
    </div>
  </div>

  <!-- Notificaciones -->
  <Notivue v-slot="item">
    <NotivueSwipe :item="item">
      <Notifications :item="item" :theme="pastelTheme" />
    </NotivueSwipe>
  </Notivue>
</template>

<script>

import { Notivue, Notifications, NotivueSwipe, pastelTheme } from 'notivue';

export default {
  name: 'App',
  components: {
    Notivue,
    Notifications,
    NotivueSwipe,

  },
  data() {
    return {
      pastelTheme
    };
  }
};
</script>

<style>
* {
  font-family: 'Poppins', sans-serif;
}

.sidebar {
  background-color: rgb(26, 26, 230);
  min-height: 100vh;
  overflow: auto;
}

/* Ajustes para mejorar la apariencia */
.nav-link {
  display: flex;
  align-items: center;
  padding: 10px 15px;
  border-radius: 5px;
  transition: background 0.3s;
}

.nav-link:hover {
  background: rgba(255, 255, 255, 0.2);
}

/* Scrollbar personalizado */
::-webkit-scrollbar {
  width: 10px;
}

::-webkit-scrollbar-thumb {
  background-color: #133db3;
  border-radius: 10px;
}

html {
  font-size: 14px;
}
</style>
