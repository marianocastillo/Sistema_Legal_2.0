<template>
  <div class="container py-3">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h4 class="m-0">{{ isEdit ? 'Editar Tribunal' : 'Nuevo Tribunal' }}</h4>
      <div class="d-flex gap-2">
        <button class="btn btn-primary btn-sm" @click="guardarTribunal">
          <i class="fa-solid fa-save me-1"></i> Guardar
        </button>
        <button class="btn btn-outline-secondary btn-sm" @click="$router.back()">
          <i class="fa-solid fa-arrow-left me-1"></i> Volver
        </button>
      </div>
    </div>

    <div class="card">
      <div class="card-header fw-bold">Datos del Tribunal</div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-4">
            <label class="form-label">Nombre del Tribunal *</label>
            <input type="text" v-model="tribunal.nombre_Tribunal" class="form-control"
              :class="{ 'is-invalid': errors.nombre_Tribunal }" />
            <div class="invalid-feedback" v-if="errors.nombre_Tribunal">{{ errors.nombre_Tribunal }}</div>
          </div>

          <div class="col-md-4">
            <label class="form-label">Teléfono</label>
            <input type="text" v-model="tribunal.telefono" class="form-control"
              :class="{ 'is-invalid': errors.telefono }" />
            <div class="invalid-feedback" v-if="errors.telefono">{{ errors.telefono }}</div>
          </div>

          <div class="col-md-4">
            <label class="form-label">Distrito</label>
            <input type="text" v-model="tribunal.distrito" class="form-control"
              :class="{ 'is-invalid': errors.distrito }" />
            <div class="invalid-feedback" v-if="errors.distrito">{{ errors.distrito }}</div>
          </div>

          <div class="col-md-6">
            <label class="form-label">Dirección</label>
            <input type="text" v-model="tribunal.direccion" class="form-control" />
          </div>

          <div class="col-md-6">
            <label class="form-label">Mapa (URL)</label>
            <input type="text" v-model="tribunal.mapsUrl" class="form-control" />
          </div>

          <div class="col-md-4">
            <label class="form-label">Estado</label>
            <select v-model="tribunal.estatus" class="form-select">
              <option :value="true">Activo</option>
              <option :value="false">Inactivo</option>
            </select>
          </div>

          <div class="col-12">
            <label class="form-label">Descripción</label>
            <textarea v-model="tribunal.descripcion" class="form-control" rows="2"></textarea>
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
  name: 'FormularioTribunal',
  data() {
    return {
      tribunal: {
        id_Tribunal: 0,
        nombre_Tribunal: '',
        descripcion: '',
        telefono: '',
        estatus: true,
        direccion: '',
        distrito: '',
        mapsUrl: ''
      },
      errors: {},
      isEdit: false
    }
  },
  created() {

    const id_Tribunal = this.$route.params.id_Tribunal
    console.log('ID recibido:', id_Tribunal)
    if (id_Tribunal) {
      this.isEdit = true
      this.cargarTribunal(id_Tribunal)
    }
  },
  methods: {
    async cargarTribunal(id_Tribunal) {
      try {
        const response = await api.get(`/api/Tribunales/ObtenerTribunales/${id_Tribunal}`)
        console.log('Respuesta de tribunal:', response.data)
        this.tribunal = response.data
      } catch (error) {
        push.error({ title: 'Error', message: 'No se pudo cargar el tribunal.' })
      }
    },
    async guardarTribunal() {
      try {
        const metodo = this.isEdit ? 'put' : 'post';
        const url = this.isEdit
          ? `/api/Tribunales/ActualizarTribunal/${this.tribunal.id_Tribunal}`
          : `/api/Tribunales/CrearTribunal`;

        // Renombrar campos si Vue usa camelCase o underscore_case
        const payload = {
          Id_Tribunal: this.tribunal.id_Tribunal,
          Nombre_Tribunal: this.tribunal.nombre_Tribunal,
          Descripcion: this.tribunal.descripcion,
          Telefono: this.tribunal.telefono,
          Estatus: this.tribunal.estatus,
          Direccion: this.tribunal.direccion,
          Distrito: this.tribunal.distrito,
          MapsUrl: this.tribunal.mapsUrl
        };

        const response = await api[metodo](url, payload);

        if (response.data.mensaje) {
          push.success({ title: 'Éxito', message: response.data.mensaje });
          this.$router.back();
        } else {
          this.errors = response.data.errors || {};
          push.warning({ title: 'Advertencia', message: response.data.message || 'Ocurrió un error' });
        }
      } catch (error) {
        this.errors = error.response?.data?.errors || {};
        push.error({ title: 'Error', message: 'Ocurrió un problema al guardar.' });
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
