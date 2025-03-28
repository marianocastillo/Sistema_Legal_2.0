<template>

  <body class="sidebar-mini layout-fixed">
    <div class="wrapper">
      <!-- NAVBAR -->
      <Navbar v-if="$route.name !== 'Login'"></Navbar>

      <!-- CONTENT WRAPPER -->
      <div class="content-wrapper surface-100 bg-contraloria relative" v-if="$route.name !== 'Login'">
        <section class="content p-4" :style="{ maxHeight: '83vh', minHeight: '100%' }">
          <div class="container-fluid bg-white border-round" style="max-height: 83vh; overflow-y: auto;"
            v-if="$route.name !== 'Inicio'">
            <router-view :key="$route.fullPath" />
          </div>
          <div v-else class="container-fluid" style="max-height: 83vh; overflow-y: auto;">
            <router-view :key="$route.fullPath" />
          </div>
        </section>
      </div>
      <section class="content h-screen" v-else>
        <router-view :key="$route.fullPath" />
      </section>

      <!-- FOOTER -->
      <FooterComponent v-if="$route.name !== 'Login'"></FooterComponent>

      <!-- CONTROL SIDEBAR -->
      <div v-if="$route.name !== 'Login'">
        <ControlSidebar />
      </div>
      <BlockScreen v-if="$route.name !== 'Login'" />
      <PageLoader />
      <ConfirmPopup></ConfirmPopup>
    </div>
  </body>
  <Notivue v-slot="item">
    <NotivueSwipe :item="item">
      <Notifications :item="item" :theme="pastelTheme" />
    </NotivueSwipe>
  </Notivue>
</template>

<script>
import { Notivue, Notifications, NotivueSwipe, pastelTheme } from 'notivue'
import Navbar from "./layout/Navbar.vue";
import FooterComponent from "./layout/Footer.vue";
import ControlSidebar from "./layout/ControlSidebar.vue"
// import PageLoader from "@/components/PageLoader.vue"
// import BlockScreen from "@/components/BlockScreen.vue"
import ConfirmPopup from 'primevue/confirmpopup';
// import { FilterService } from 'primevue/api';
// import { FilterService } from 'primevue/utils';

export default {
  name: 'App',
  components: {
    Navbar,
    ControlSidebar,
    FooterComponent,
    Notivue,
    Notifications,
    NotivueSwipe,
    PageLoader,
    BlockScreen,
    ConfirmPopup
  },
  data() {
    return {
      pastelTheme
    }
  },
  mounted() {
    const createCheckedFilter = (callback) => (value, filter) => {
      if (filter === undefined || filter === null) {
        return true;
      }

      if (value === undefined || value === null) {
        return false;
      }

      return callback(value, filter);
    };

    FilterService.register('dateAfter', createCheckedFilter((value, filter) => value.getTime() >= filter.setHours(0, 0, 0, 0)));
    FilterService.register('dateBefore', createCheckedFilter((value, filter) => value.getTime() <= filter.setHours(23, 59, 59, 999)));
    FilterService.register('dateIs', createCheckedFilter((value, filter) => new Date(value).setHours(0, 0, 0, 0) === filter.setHours(0, 0, 0, 0)));
    FilterService.register('dateIsNot', createCheckedFilter((value, filter) => new Date(value).setHours(0, 0, 0, 0) !== filter.setHours(0, 0, 0, 0)));
  }
}
</script>


<style>
.bg-contraloria {
  background-image: url('https://www.presidencia.gob.do/themes/custom/presidency/images/cupula-trans.png') !important;
  background-position: right;
  background-repeat: no-repeat;
  background-size: contain;
}

.p-component {
  font-family: 'Poppins', sans-serif !important;
}
</style>
