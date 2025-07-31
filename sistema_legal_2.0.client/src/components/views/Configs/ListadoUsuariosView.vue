


<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-between items-center mb-4 flex-wrap gap-2">
      <h2 class="text-2xl font-bold">Listado de Usuarios</h2>

      <div class="flex items-center gap-2 filtro-busqueda-bar">
        <router-link to="/ConfiguracionUsuarios" class="btn-filtro">
          <i class="fas fa-plus me-2"></i> Nuevo
        </router-link>

        <router-link to="/Seguimiento" class="btn-filtro">
          <i class="fa-solid fa-home me-2"></i> Inicio
        </router-link>

        <div class="search-container">
          <span class="p-input-icon-left search-input-wrapper">
            <i class="pi pi-search search-icon" />
            <input type="text" class="search-input" placeholder="Buscar..." v-model="search" />
          </span>
        </div>


      </div>
    </div>

    <table class="table table-bordered table-hover table-sm">
      <thead class="table-light">
        <tr>
          <th @click="sort('nombreUsuario')">Nombre de Usuario</th>
          <th @click="sort('nombres')">Nombres</th>
          <th @click="sort('apellidos')">Apellidos</th>
          <th @click="sort('idPerfil')">Perfil</th>
          <th @click="sort('fechaCrea')">Fecha de Creación</th>
          <th @click="sort('activo')">Estado</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="usuario in paginatedUsuarios" :key="usuario.idUsuario">
          <td>{{ usuario.nombreUsuario }}</td>
          <td>{{ usuario.nombres }}</td>
          <td>{{ usuario.apellidos }}</td>
          <td>{{ usuario.nombrePerfil }}</td>
          <td>{{ new Date(usuario.fechaCreacion).toLocaleDateString() }}</td>
          <td>
            <span class="tag" :class="usuario.activo ? 'status-sentencia' : 'status-casacion'">
              {{ usuario.activo ? 'Activo' : 'Inactivo' }}
            </span>
          </td>
          <td>
            <div class="btn-group">
              <button class="btn btn-sm btn-hover" style="background-color: #003870; border-color: #003870; margin-right: 0.3rem;"
               @click="$router.push({ path: `/ConfiguracionUsuarios/${usuario.idUsuario}` })">
                <i class="fas fa-edit white-icon"></i>
              </button>
              <button class="btn btn-sm btn-hover" style="background-color: #003870; border-color: #003870;"
                @click="ConfirmDelete(usuario.idUsuario)">
                <i class="fas fa-trash-alt white-icon"></i>
              </button>
            </div>
          </td>
        </tr>
        <tr v-if="filteredUsuarios.length === 0">
          <td colspan="7" class="text-center">No se han encontrado usuarios.</td>
        </tr>
      </tbody>
    </table>

    <!-- Paginación -->
    <nav class="mt-4">
      <ul class="pagination justify-content-end">
        <li class="page-item" :class="{ disabled: page === 1 }">
          <button class="page-link" @click="page--">Anterior</button>
        </li>
        <li class="page-item" v-for="p in totalPages" :key="p" :class="{ active: page === p }">
          <button class="page-link" @click="page = p">{{ p }}</button>
        </li>
        <li class="page-item" :class="{ disabled: page === totalPages }">
          <button class="page-link" @click="page++">Siguiente</button>
        </li>
      </ul>
    </nav>
  </div>



  <!-- Diálogo de confirmación -->
  <transition name="fade-dialog">
    <div v-if="showDialog" class="dialog-overlay" @click.self="showDialog = false">
      <div class="dialog-box">
        <h3 class="text-lg font-semibold mb-3">¿Eliminar usuario?</h3>
        <p class="text-sm text-gray-600 mb-5">Esta acción no se puede deshacer.</p>
        <div class="dialog-buttons">
          <button :disabled="eliminando" @click="EliminarUsuario"
            class="btn text-white px-4 py-2 rounded-md flex items-center justify-center"
            style="background-color: #003870;">
            <span v-if="!eliminando">Sí, eliminar</span>
            <span v-else>
              <i class="fas fa-spinner fa-spin me-2"></i>
              Eliminando...
            </span>
          </button>

          <button @click="showDialog = false" class="btn px-4 py-2 rounded-md border border-gray-300">
            Cancelar
          </button>
        </div>

      </div>
    </div>
  </transition>


</template>

<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'
import { ref } from 'vue'

export default {
  data() {
    return {
      usuarios: [],
      search: '',
      sortBy: 'fechaCrea',
      sortDesc: true,
      page: 1,
      perPage: 10,
      showDialog: false,
      usuarioEliminar: null,
      eliminando: false,
    }
  },
  computed: {
    filteredUsuarios() {
      if (!this.search) return this.sortedUsuarios
      const term = this.search.toLowerCase()
      return this.sortedUsuarios.filter(u =>
        Object.values(u).some(val => String(val).toLowerCase().includes(term))
      )
    },
    sortedUsuarios() {
      return [...this.usuarios].sort((a, b) => {
        const aVal = a[this.sortBy]
        const bVal = b[this.sortBy]
        if (aVal < bVal) return this.sortDesc ? 1 : -1
        if (aVal > bVal) return this.sortDesc ? -1 : 1
        return 0
      })
    },
    totalPages() {
      return Math.ceil(this.filteredUsuarios.length / this.perPage)
    },
    paginatedUsuarios() {
      const start = (this.page - 1) * this.perPage
      return this.filteredUsuarios.slice(start, start + this.perPage)
    },
  },
  mounted() {
    this.LoadUsuarios()
  },
  methods: {
    sort(field) {
      if (this.sortBy === field) {
        this.sortDesc = !this.sortDesc
      } else {
        this.sortBy = field
        this.sortDesc = false
      }
    },
    async LoadUsuarios() {
      const response = await api.get('/api/Usuarios/UsuariosConPerfil')
      if (response.data) {
        console.log(response.data) // ← Aquí
        this.usuarios = response.data
      } else {
        push.warning({ title: 'Advertencia', message: response.data.message })
      }
    },
    async ConfirmDelete(idUsuario) {
      if (confirm('¿Estás seguro que deseas eliminar este usuario?')) {
        const response = await api.delete(`/api/Usuarios/${idUsuario}`)
        if (response.data) {
          this.LoadUsuarios()
          push.success({ title: 'Éxito', message: response.data.message })
        } else {
          push.warning({ title: 'Advertencia', message: response.data.message })
        }
      }
    },
    ConfirmDelete(idUsuario) {
      this.usuarioAEliminar = idUsuario
      this.showDialog = true
    },
  async EliminarUsuario() {
  this.eliminando = true;

  try {
    const response = await api.delete(`/api/Usuarios/${this.usuarioAEliminar}`);

    if (response.data?.success) {
      await this.LoadUsuarios();
      push.success({ title: 'Éxito', message: response.data.message });
    } else {
      push.warning({ title: 'Advertencia', message: response.data.message || 'No se pudo eliminar el usuario.' });
    }

  } catch (error) {
    // Extraer mensaje si está en el cuerpo del error
    const errorMessage = error.response?.data?.message || 'No se pudo eliminar el usuario.';
    push.error({ title: 'Error', message: errorMessage });
  } finally {
    this.eliminando = false;
    this.showDialog = false;
  }
}
  }}
</script>

<style scoped>
.white-icon {
  color: white !important;
}

.btn-group .btn {
  margin: 0 2px;
}

.btn-filtro {
  padding: 0.35rem 1.2rem;
  height: 38px;
  border-radius: 6px;
  font-size: 0.88rem;
  font-weight: 500;
  background-color: #003870;
  border-color: #003870;
  color: #f0f0f0;
  transition: all 0.2s ease;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
  white-space: nowrap;
  text-decoration: none;
}

.btn-filtro:hover {
  background-color: #c00606;
  border-color: #c00606;
  box-shadow: 0 6px 16px rgba(121, 1, 51, 0.3) !important;
  transform: translateY(-1px);
  transition: background-color 0.2s;
}

.btn-filtro.active {
  background-color: #c00606;
  border-color: #c00606;
  box-shadow: 0 6px 16px rgba(121, 1, 51, 0.3) !important;
  transform: translateY(-1px);
  transition: background-color 0.2s;
}

/* === Encabezado de tabla === */
thead th {
  background-color: rgb(241, 242, 250);
  border: none !important;
  font-weight: 600;
  color: #2e3842;
  font-size: 0.95rem;
}

tbody td {
  border-right: 1px solid #ebebeb;
  border-top: none !important;
  border-bottom: none !important;
  border-left: none !important;
}


/* === Contenedor de botones + búsqueda === */
.filtro-busqueda-bar {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: nowrap;
  margin-left: auto;
}

/* === Input de búsqueda === */
.search-container {
  max-width: 250px;
  width: 100%;
  position: relative;
}

.search-input-wrapper {
  position: relative;
  display: block;
}

.search-input {
  width: 100%;
  padding: 0.5rem 2.5rem 0.5rem 2rem;
  font-size: 14px;
  border: 1px solid #ccc;
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: border-color 0.3s, box-shadow 0.3s;
  background-color: #fff;
  color: #000;
}

.search-input:focus {
  outline: none;
  border-color: #003870;
  box-shadow: 0 0 5px rgba(0, 56, 112, 0.3);
}

.search-icon {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #888;
  pointer-events: none;
  font-size: 1rem;
}

/* === Etiquetas de estado === */
.tag {
  padding: 0.4rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.8rem;
  font-weight: 600;
  display: inline-block;
  text-align: center;
  min-width: 100px;
  text-transform: capitalize;
}

.status-sentencia {
  background-color: #d4edda;
  color: #155724;
}

.status-casacion {
  background-color: #f8d7da;
  color: #721c24;
}

/* === Paginación === */
.pagination .page-link {
  color: #003870;
  border: 1px solid #ccc;
  padding: 0.3rem 0.75rem;
  font-size: 0.875rem;
}

.pagination .page-item.active .page-link {
  background-color: #003870;
  border-color: #003870;
  color: white;
}

.pagination .page-item.disabled .page-link {
  color: #aaa;
  pointer-events: none;
}

/* === estilos del diálogo === */
.dialog-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 99;
}

.dialog-box {
  background-color: white;
  border-radius: 1rem;
  padding: 2rem;
  max-width: 400px;
  width: 90%;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  text-align: center;
}

.dialog-buttons {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-top: 1rem;
}

/* === Animación del diálogo === */
.fade-dialog-enter-active,
.fade-dialog-leave-active {
  transition: opacity 0.3s ease, transform 0.3s ease;
}

.fade-dialog-enter-from,
.fade-dialog-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

.fade-dialog-enter-to,
.fade-dialog-leave-from {
  opacity: 1;
  transform: scale(1);
}



/* === Responsivo === */
@media (max-width: 768px) {
  .filtro-busqueda-bar {
    flex-direction: column;
    align-items: flex-end;
    width: 100%;
    margin-left: 0;
    gap: 0.5rem;
  }

  .search-container {
    width: 100%;
  }
}
</style>
