<template>
  <!-- Control Sidebar -->
  <aside class="control-sidebar control-sidebar-dark" />
  <!-- /.control-sidebar -->

  <aside class="main-sidebar sidebar-light-primary elevation-4">
    <!-- Brand Logo -->
    <router-link :to="{ name: 'Inicio' }" class="brand-link" style="padding: 1.12rem 0.5rem !important">
      <img
        src="@/assets/contraloria azul.png"
        alt="CGRD"
        class="brand-image select-none mr-2"
        style="opacity: 0.8; margin-left: 0.4rem"
      />
      <span class="brand-text font-weight-bold px-2 text-lg"
        >Gestión de Visitas</span
      >
    </router-link>

    <!-- Sidebar -->
    <div class="sidebar">
      <!-- Sidebar Menu -->
      <nav class="mt-2">
        <ul
          class="nav nav-pills nav-sidebar flex-column"
          data-widget="treeview"
          role="menu"
          data-accordion="true"
          v-if="$store.state.views"
        >
          <li
            class="nav-item mb-2"
            v-for="vista in $store.state.views.filter(
              (x) =>
                $store.state.views.filter(
                  (y) => y.modulo.idModulo == x.modulo.idModulo && y.principal && x.principal
                ).length == 1
            )"
            :key="vista.url"
          >
            <router-link
              :to="{ name: vista.nombre }"
              :class="{
                'nav-link': true,
                active:
                  $route.name === vista.nombre || vistaActiva(vista.idVista),
              }"
            >
              <i :class="'nav-icon ' + vista.modulo.icono"></i>
              <p>
                {{ vista.modulo.nombre }}
              </p>
            </router-link>
          </li>
          <li
            :class="{
              'nav-item tree-view-modulos mb-2': true,
              'menu-is-opening menu-open': userViews
                .filter((x) => x.idModulo == modulo.idModulo)
                .some((vista) => $route.name === vista.nombre),
            }"
            v-for="modulo in getModulos(
              userViews.filter((x) => x.idModulo != null && (userViews.filter((y) => y.idModulo == x.idModulo && y.principal).length > 1))
            )"
            :id="'modulo_' + modulo.nombre"
            :key="modulo.idModulo"
          >
            <a
              href="#"
              :class="{
                'nav-link': true,
                'sidebar-menu-father-item-active': moduloActivo(
                  modulo.idModulo
                ),
              }"
            >
              <i :class="'nav-icon ' + modulo.icono"></i>
              <p>
                {{ modulo.nombre }}
                <i class="right fas fa-angle-left"></i>
              </p>
            </a>
            <ul class="nav nav-treeview">
              <li
                class="nav-item"
                v-for="vista in userViews.filter(
                  (x) => x.idModulo == modulo.idModulo && x.principal
                )"
                :key="vista.url"
                :id="'vista_' + vista.idVista"
              >
                <router-link
                  :to="{ name: vista.nombre }"
                  :class="{
                    'nav-link': true,
                    active:
                      ($route.name === vista.nombre ||
                        vistaActiva(vista.idVista)) &&
                      !modalVisible,
                  }"
                >
                  <i :class="'nav-icon ' + vista.icono"></i>
                  <p>
                    {{ vista.nombre }}
                  </p>
                </router-link>
              </li>
            </ul>
          </li>

          <li class="nav-item">
            <a href="#" class="nav-link" @click="manualUsuario()">
              <i class="nav-icon fas fa-book-reader"></i>
              <p>Manual de usuario</p>
            </a>
          </li>
        </ul>
      </nav>
    </div>
    <!-- /.sidebar -->
  </aside>
</template>

<!-- <script>
// import $ from "jquery";
import * as AdminLte from "admin-lte";
import { push } from "notivue";

export default {
  data() {
    return {
      modalVisible: false,
    };
  },
  computed: {
    userViews() {
      return this.$store.state.views;
    },
  },
  methods: {
    getModulos(views) {
      return views
        .filter(
          (x) =>
            views.filter((y) => y.modulo.idModulo == x.modulo.idModulo).length >
            1
        )
        .map((x) => x.modulo)
        .filter(
          (value, index, self) =>
            self.findIndex((t) => t.idModulo === value.idModulo) === index
        );
    },
    vistaActiva(idVista) {
      const vistaActual = this.$store.state.views.filter(
        (x) => x.nombre == this.$route.name
      )[0];

      const vistaSeleccionada = this.$store.state.views.filter(
        (x) => x.idVista == idVista
      )[0];

      let esModuloPrincipal = false;

      if (vistaActual) {
        esModuloPrincipal =
          this.$store.state.views.filter(
            (x) => x.idModulo == vistaActual.idModulo && x.principal
          ).length == 1;
      }

      if (!vistaActual) return false;
      return idVista == vistaActual.idPadre || (vistaSeleccionada.idModulo == vistaActual.idModulo && esModuloPrincipal);
    },
    moduloActivo(idModulo) {
      const vistaActual = this.$store.state.views.filter(
        (x) => x.nombre == this.$route.name
      )[0];
      if (!vistaActual) return false;
      return idModulo == vistaActual.modulo.idModulo;
    },
    manualUsuario() {
      push.warning("Manual de usuario no habilitado");
    },
  },
  created() {
    $('[data-widget="treeview"]').each(function () {
      AdminLte.Treeview._jQueryInterface.call($(this), "init");
      AdminLte.Layout._jQueryInterface.call($("body"));
      AdminLte.PushMenu._jQueryInterface.call($('[data-widget="pushmenu"]'));
    });
  },
};
</script> -->

<style>
.sidebar .nav-link p {
  transition: margin-left 0.3s linear, opacity 0.3s ease, visibility 0.3s ease;
}

.nav-sidebar .nav-link p {
  white-space: nowrap !important;
}

.brand-link:hover .brand-text {
  color: #003880;
}

.nav-link.active {
  background-color: #003880 !important;
  color: #ffffff !important;
}
</style>
