<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="text-2xl font-semibold">Detalle del Litigio</h2>
      <VaButton
        @click="$emit('close')"
        icon="close"
        color="danger"
        size="small"
        class="p-2"
      />
    </div>

    <!-- Encabezado del documento -->
    <div class="document-header mb-5 p-4 border-round-lg" style="background: #f8f9fa;">
      <div class="flex align-items-center justify-content-between">
        <div>
          <h3 class="text-xl m-0" style="color: #003870;">Expediente Judicial</h3>
          <p class="text-sm text-600 m-0">No. Acto: <strong>{{ litigio.ltg_acto }}</strong></p>
        </div>
        <div class="text-right">
          <p class="text-sm m-0">Fecha: {{ formatDate(litigio.ltg_Fecha_Acto) }}</p>
          <Tag :value="litigio.estatus_Descripcion"
               :severity="getStatusSeverity(litigio.estatus_Descripcion)"
               class="mt-1" />
        </div>
      </div>
    </div>

    <!-- Sección de información principal -->
    <div class="grid mb-5">
      <div class="col-12 md:col-6">
        <div class="surface-50 p-3 border-round-lg">
          <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Demandante</h4>
          <div class="grid">
            <div class="col-6 field">
              <label class="text-sm font-medium text-600">Nombre</label>
              <p class="m-0">{{ litigio.ltg_Demandante || 'N/A' }}</p>
            </div>
            <div class="col-6 field">
              <label class="text-sm font-medium text-600">Cédula</label>
              <p class="m-0">{{ litigio.ltg_Cedula_Demandante || 'N/A' }}</p>
            </div>
            <div class="col-6 field">
              <label class="text-sm font-medium text-600">Tipo</label>
              <p class="m-0">{{ litigio.ltg_Tipo_Demandante || 'N/A' }}</p>
            </div>
            <div class="col-6 field">
              <label class="text-sm font-medium text-600">Nacionalidad</label>
              <p class="m-0">{{ litigio.ltg_Nacionalidad || 'N/A' }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="col-12 md:col-6">
        <div class="surface-50 p-3 border-round-lg">
          <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Representante</h4>
          <div class="grid">
            <div class="col-6 field">
              <label class="text-sm font-medium text-600">Nombre</label>
              <p class="m-0">{{ litigio.ltg_Nombre_Representante || 'N/A' }}</p>
            </div>
            <div class="col-6 field">
              <label class="text-sm font-medium text-600">Cédula</label>
              <p class="m-0">{{ litigio.ltg_Cedula_Representante || 'N/A' }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Detalles del proceso -->
    <div class="surface-50 p-4 mb-5 border-round-lg">
      <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Detalles del Proceso</h4>
      <div class="grid">
        <div class="col-12 md:col-4 field">
          <label class="text-sm font-medium text-600">Tipo de Demanda</label>
          <p class="m-0">{{ litigio.tipoDemanda_Nombre || 'N/A' }}</p>
        </div>
        <div class="col-12 md:col-4 field">
          <label class="text-sm font-medium text-600">Tribunal</label>
          <p class="m-0">{{ litigio.nombre_Tribunal || 'N/A' }}</p>
        </div>
        <div class="col-12 md:col-4 field">
          <label class="text-sm font-medium text-600">Digitador</label>
          <p class="m-0">{{ litigio.nombreUsuario || 'N/A' }}</p>
        </div>
        <div class="col-12 md:col-4 field">
          <label class="text-sm font-medium text-600">Fecha Audiencia</label>
          <p class="m-0">{{ formatDate(litigio.ltg_Fecha_Audiencia) || 'N/A' }}</p>
        </div>
        <div class="col-12 md:col-4 field">
          <label class="text-sm font-medium text-600">Fecha Actualización</label>
          <p class="m-0">{{ formatDate(litigio.ltg_Fecha_Actualizacion) || 'N/A' }}</p>
        </div>
      </div>
    </div>

    <!-- Documentos y sentencia -->
    <div class="grid">
      <div class="col-12 md:col-8">
        <div class="surface-50 p-4 border-round-lg h-full">
          <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Sentencia</h4>
          <p class="m-0">{{ litigio.desc_Sentencia || 'No registrada' }}</p>
        </div>
      </div>
      <div class="col-12 md:col-4">
        <div class="surface-50 p-4 border-round-lg h-full">
          <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Documentos</h4>
          <div v-if="litigio.ruta_Nombre" class="flex align-items-center">
            <i class="pi pi-file-pdf mr-2" style="color: #e74c3c;"></i>
            <span>{{ litigio.ruta_Nombre }}</span>
          </div>
          <p v-else class="m-0 text-500">No hay documentos adjuntos</p>
        </div>
      </div>
    </div>

    <!-- Pie de documento -->
    <div class="flex justify-content-between mt-4 pt-3 border-top-1 surface-border">
      <small class="text-500">Sistema Judicial - {{ new Date().getFullYear() }}</small>
      <Button
        label="Imprimir"
        icon="pi pi-print"
        class="p-button-sm p-button-text"
        @click="printDocument"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps({
  litigioId: {  // Cambia a recibir el ID en lugar del objeto completo
    type: String || Number,
    required: true
  }
})

const litigio = ref(null)
const loading = ref(true)

onMounted(async () => {
  try {
    const response = await api.get(`/api/litigios/${props.litigioId}`)
    litigio.value = response.data
  } catch (error) {
    console.error('Error al cargar litigio:', error)
    // Puedes manejar el error mostrando un mensaje al usuario
  } finally {
    loading.value = false
  }
})

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString('es-ES', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const getStatusSeverity = (status) => {
  const statusMap = {
    'activo': 'success',
    'inactivo': 'warning',
    'cerrado': 'danger',
    'finalizado': 'info'
  }
  return statusMap[status?.toLowerCase()] || null
}

const printDocument = () => {
  window.print()
}
</script>

<style scoped>
.field {
  margin-bottom: 1rem;
}

.document-header {
  border-left: 4px solid #003870;
}

@media print {
  .card {
    box-shadow: none !important;
    border: none !important;
  }
}
</style>
