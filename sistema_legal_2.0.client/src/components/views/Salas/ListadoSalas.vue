<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h4 class="text-dark">Listado de Salas</h4>
      <div class="d-flex gap-2">
        <router-link to="/EdicionSalas" class="btn text-white" style="background-color: #003870;">
          <i class="fas fa-plus me-2"></i> Nuevo
        </router-link>

        <input type="text" class="form-control-sm bg-white text-dark" placeholder="Buscar..." v-model="search" />
      </div>
    </div>

    <table class="table table-bordered table-hover table-sm">
      <thead class="table-light">
        <tr>
          <th @click="sort('nombre')">Sala</th>
          <th @click="sort('nombre_Tribunal')">Tribunal</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="sala in paginatedSalas" :key="sala.idSala">
          <td>{{ sala.nombre }}</td>
          <td>{{ sala.nombre_Tribunal }}</td>
          <td>
            <button class="btn btn-sm me-1" style="background-color: #003870;"
              @click="$router.push({ name: 'EdicionSalas', params: { idSala: sala.idSala } })">
              <i class="fas fa-edit" style="color: white;"></i>
            </button>

            <button class="btn btn-sm" style="background-color: #003870;" @click="confirmDelete(sala.idSala)">
              <i class="fas fa-trash-alt" style="color: white;"></i>
            </button>
          </td>
        </tr>
        <tr v-if="filteredSalas.length === 0">
          <td colspan="3" class="text-center">No se han encontrado salas.</td>
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
      salas: [],
      search: '',
      sortBy: 'nombre',
      sortDesc: false,
      page: 1,
      perPage: 10,
    }
  },
  computed: {
    filteredSalas() {
      if (!this.search) return this.sortedSalas
      const term = this.search.toLowerCase()
      return this.sortedSalas.filter(s =>
        Object.values(s).some(val => String(val).toLowerCase().includes(term))
      )
    },
    sortedSalas() {
      return [...this.salas].sort((a, b) => {
        const aVal = a[this.sortBy]?.toLowerCase?.() || ''
        const bVal = b[this.sortBy]?.toLowerCase?.() || ''
        return this.sortDesc
          ? bVal.localeCompare(aVal)
          : aVal.localeCompare(bVal)
      })
    },
    totalPages() {
      return Math.ceil(this.filteredSalas.length / this.perPage)
    },
    paginatedSalas() {
      const start = (this.page - 1) * this.perPage
      return this.filteredSalas.slice(start, start + this.perPage)
    }
  },
  mounted() {
    this.loadSalas()
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
    async loadSalas() {
      try {
        const response = await api.get('/api/Tribunales/Salas')
        this.salas = response.data
      } catch (err) {
        push.error({ title: 'Error', message: 'No se pudieron cargar las salas.' })
      }
    },
    async confirmDelete(id) {
      if (confirm('¿Estás seguro que deseas eliminar esta sala?')) {
        try {
          const response = await api.delete(`/api/Tribunales/EliminarSalas/${id}`);
          this.loadSalas();
          push.success({ title: 'Éxito', message: response.data.message });
        } catch (err) {
          push.error({ title: 'Error', message: err.response?.data?.message || 'No se pudo eliminar la sala.' });
        }
      }
    }
  }
}
</script>
