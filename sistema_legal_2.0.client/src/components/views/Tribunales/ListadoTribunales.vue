<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h4 class="text-dark">Listado de Tribunales</h4>
      <div class="d-flex gap-2">
        <router-link to="/EdicionTribunales" class="btn text-white" style="background-color: #003870;">
          <i class="fas fa-plus me-2"></i> Nuevo
        </router-link>

        <input type="text" class="form-control-sm bg-white text-dark" placeholder="Buscar..." v-model="search" />
      </div>
    </div>

    <table class="table table-bordered table-hover table-sm">
      <thead class="table-light">
        <tr>
          <th @click="sort('nombre_Tribunal')">Nombre</th>
          <th @click="sort('descripcion')">Descripción</th>
          <th @click="sort('telefono')">Teléfono</th>
          <th @click="sort('direccion')">Dirección</th>
          <th @click="sort('distrito')">Provincia </th>
          <th @click="sort('estatus')">Estado</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="tribunal in paginatedTribunales" :key="tribunal.id_Tribunal">
          <td>{{ tribunal.nombre_Tribunal }}</td>
          <td>{{ tribunal.descripcion }}</td>
          <td>{{ tribunal.telefono }}</td>
          <td>{{ tribunal.direccion }}</td>
          <td>{{ tribunal.distrito }}</td>
          <td>
            <span :class="['badge', tribunal.estatus ? 'bg-success' : 'bg-danger']">
              {{ tribunal.estatus ? 'Activo' : 'Inactivo' }}
            </span>
          </td>
          <td>
            <button class="btn btn-sm me-1" style="background-color: #003870;"
              @click="$router.push({ name: 'EdicionTribunales', params: { id_Tribunal: tribunal.id_Tribunal } })">
              <i class="fas fa-edit" style="color: white;"></i>
            </button>

            <button class="btn btn-sm" style="background-color: #003870;" @click="confirmDelete(tribunal.id_Tribunal)">
              <i class="fas fa-trash-alt" style="color: white;"></i>
            </button>
          </td>
        </tr>
        <tr v-if="filteredTribunales.length === 0">
          <td colspan="7" class="text-center">No se han encontrado tribunales.</td>
        </tr>
      </tbody>
    </table>

    <!-- Paginación -->
    <nav>
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
</template>

<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'

export default {
  data() {
    return {
      tribunales: [],
      search: '',
      sortBy: 'nombre_Tribunal',
      sortDesc: false,
      page: 1,
      perPage: 10,
    }
  },
  computed: {
    filteredTribunales() {
      if (!this.search) return this.sortedTribunales
      const term = this.search.toLowerCase()
      return this.sortedTribunales.filter(t =>
        Object.values(t).some(val => String(val).toLowerCase().includes(term))
      )
    },
    sortedTribunales() {
      return [...this.tribunales].sort((a, b) => {
        const aVal = a[this.sortBy]
        const bVal = b[this.sortBy]
        if (aVal < bVal) return this.sortDesc ? 1 : -1
        if (aVal > bVal) return this.sortDesc ? -1 : 1
        return 0
      })
    },
    totalPages() {
      return Math.ceil(this.filteredTribunales.length / this.perPage)
    },
    paginatedTribunales() {
      const start = (this.page - 1) * this.perPage
      return this.filteredTribunales.slice(start, start + this.perPage)
    }
  },
  mounted() {
    this.loadTribunales()
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
    async loadTribunales() {
      try {
        const response = await api.get('/api/Tribunales')
        this.tribunales = response.data
      } catch (err) {
        push.error({ title: 'Error', message: 'No se pudieron cargar los tribunales.' })
      }
    },
    async confirmDelete(id) {
      if (confirm('¿Estás seguro que deseas eliminar este tribunal?')) {
        try {
          const response = await api.delete(`/api/Tribunales/EliminarTribunal/${id}`);
          this.loadTribunales();
          push.success({ title: 'Éxito', message: response.data.message });
        } catch (err) {
          push.error({ title: 'Error', message: err.response?.data?.message || 'No se pudo eliminar el tribunal.' });
        }
      }
    }

  }
}
</script>
