<template>
  <div class="container py-3">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h4 class="m-0">{{ isEdit ? 'Editar Sala' : 'Nueva Sala' }}</h4>
      <div class="d-flex gap-2">
        <button class="btn btn-primary btn-sm" @click="guardarSala">
          <i class="fa-solid fa-save me-1"></i> Guardar
        </button>
        <button class="btn btn-outline-secondary btn-sm" @click="$router.back()">
          <i class="fa-solid fa-arrow-left me-1"></i> Volver
        </button>
      </div>
    </div>

    <div class="card">
      <div class="card-header fw-bold">Datos de la Sala</div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-6">
            <label class="form-label">Nombre de la Sala *</label>
            <input type="text" v-model="sala.nombre" class="form-control" :class="{ 'is-invalid': errors.nombre }" />
            <div class="invalid-feedback" v-if="errors.nombre">{{ errors.nombre }}</div>
          </div>

          <div class="col-md-6">
            <label class="form-label">Tribunal *</label>
            <select v-model="sala.idTribunal" class="form-select" :class="{ 'is-invalid': errors.idTribunal }">
              <option disabled value="">Seleccione un tribunal</option>
              <option v-for="tribunal in tribunales" :key="tribunal.id_Tribunal" :value="tribunal.id_Tribunal">
                {{ tribunal.nombre_Tribunal }}
              </option>
            </select>
            <div class="invalid-feedback" v-if="errors.idTribunal">{{ errors.idTribunal }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'

export default {
  name: 'FormularioSala',
  data() {
    return {
      sala: {
        idSala: 0,
        nombre: '',
        idTribunal: ''
      },
      tribunales: [],
      errors: {},
      isEdit: false
    }
  },
  async created() {
    await this.cargarTribunales()

    const idSala = this.$route.params.idSala
    if (idSala) {
      this.isEdit = true
      this.cargarSala(idSala)
    }
  },
  methods: {
    async cargarTribunales() {
      try {
        const response = await api.get('/api/Tribunales/Tribunales')
        this.tribunales = response.data
      } catch (error) {
        push.error({ title: 'Error', message: 'No se pudieron cargar los tribunales.' })
      }
    },
    async cargarSala(id) {
      try {
        const response = await api.get(`/api/Tribunales/Salaspor/${id}`)
        this.sala = {
          idSala: response.data.idSala,
          nombre: response.data.nombre.trim(),
          idTribunal: response.data.idTribunal
        }
      } catch (error) {
        push.error({ title: 'Error', message: 'No se pudo cargar la sala.' })
      }
    },
    async guardarSala() {
      try {
        const metodo = this.isEdit ? 'put' : 'post'
        const url = this.isEdit
          ? `/api/Tribunales/Actualizarsalas/${this.sala.idSala}`
          : `/api/Tribunales/CrearSalas`

        const payload = {
          idSala: this.sala.idSala,
          nombre: this.sala.nombre.trim(),
          idTribunal: parseInt(this.sala.idTribunal)
        }

        const response = await api[metodo](url, payload)

        if (response.data.mensaje) {
          push.success({ title: 'Éxito', message: response.data.mensaje })
          this.$router.back()
        } else {
          this.errors = response.data.errors || {}
          push.warning({ title: 'Advertencia', message: response.data.message || 'Ocurrió un error' })
        }
      } catch (error) {
        this.errors = error.response?.data?.errors || {}
        push.error({ title: 'Error', message: 'Ocurrió un problema al guardar.' })
      }
    }
  }
}
</script>

<style scoped>
.form-label {
  font-weight: 500;
}
</style>
