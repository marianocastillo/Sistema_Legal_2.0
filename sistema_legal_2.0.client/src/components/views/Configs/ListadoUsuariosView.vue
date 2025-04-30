<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h4 class="text-dark">Listado de usuarios</h4>
      <div class="d-flex gap-2">
        <router-link to="/drawer/formulario" class="btn text-white" style="background-color: #003870;">
          <i class="fas fa-plus me-2"></i> Nuevo
        </router-link>

        <input type="text" class="form-control-sm bg-white text-dark" placeholder="Buscar..." v-model="search" />
        <router-link to="/drawer/home" class="btn text-white" style="background-color: #003870;">
          <i class="fa-solid fa-home me-2"></i> Inicio
        </router-link>
      </div>
    </div>

    <table class="table table-bordered table-hover table-sm">
      <thead class="table-light">
        <tr>
          <th @click="sort('nombreUsuario')">Nombre de Usuario</th>
          <th @click="sort('nombres')">Nombres</th>
          <th @click="sort('apellidos')">Apellidos</th>
          <th @click="sort('nombrePerfil')">Perfil</th>
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
            <span :class="['badge', usuario.activo ? 'bg-success' : 'bg-warning']">
              {{ usuario.activo ? 'Activo' : 'Inactivo' }}
            </span>
          </td>
          <td>
            <button class="btn btn-sm  me-2" style="background-color: #003870;"
              @click="$router.push({ name: 'formulario', params: { idUsuario: usuario.idUsuario } })">
              <i class="fas fa-edit" style="color: white;"></i>
            </button>
            <button class="btn btn-sm " style="background-color: #003870;" @click="ConfirmDelete(usuario.idUsuario)">
              <i class="fas fa-trash-alt" style="color: white;"></i>
            </button>
          </td>
        </tr>
        <tr v-if="filteredUsuarios.length === 0">
          <td colspan="7" class="text-center">No se han encontrado usuarios.</td>
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
      usuarios: [],
      search: '',
      sortBy: 'fechaCrea',
      sortDesc: true,
      page: 1,
      perPage: 10,
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
    }
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
    }
  }
}
</script>
