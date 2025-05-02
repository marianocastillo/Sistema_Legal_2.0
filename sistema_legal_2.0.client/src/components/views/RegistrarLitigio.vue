<template>
  <div class="card p-4 shadow-sm">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2 class="h4 fw-bold">Registro de Litigio</h2>
      <div style="width: 220px;">
        <input type="date" class="form-control" v-model="form.fechaExpediente" disabled />
      </div>
    </div>

    <form @submit.prevent="registrarLitigio">
      <div class="row g-3">
        <!-- No. Acto -->
        <div class="col-md-4">
          <input type="text" class="form-control" v-model="form.noActo" placeholder="No. Acto Alguacil *" />
        </div>

        <!-- Fecha Acto -->
        <div class="col-md-4">
          <input type="date" class="form-control" v-model="form.fechaActo" placeholder="Fecha del acto" />
        </div>

        <!-- Cédula Demandante -->
        <div class="col-md-4">
          <input type="text" class="form-control" v-model="form.cedulaDemandante" placeholder="Cédula del demandante" />
        </div>

        <!-- Tipo de Demanda -->
        <div class="col-md-6">
          <select class="form-select" v-model="form.tipoDemanda">
            <option disabled value="">Tipo de Demanda</option>
            <option v-for="item in tiposDemanda" :key="item.value" :value="item.value">{{ item.label }}</option>
          </select>
        </div>

        <!-- Demandante -->
        <div class="col-md-6">
          <input type="text" class="form-control" v-model="form.demandante" placeholder="Nombre demandante" />
        </div>

        <!-- Tipo de Demandante -->
        <div class="col-md-4">
          <select class="form-select" v-model="form.tiposDemandante">
            <option disabled value="">Tipo de Demandante</option>
            <option v-for="item in tiposDemandante" :key="item.value" :value="item.value">{{ item.label }}</option>
          </select>
        </div>

        <!-- Cédula Representante -->
        <div class="col-md-4">
          <input type="text" class="form-control" v-model="form.cedulaRepresentante"
            placeholder="Cédula del representante" />
        </div>

        <!-- Tribunal -->
        <div class="col-md-4">
          <select class="form-select" v-model="form.tribunal">
            <option disabled value="">Tribunal</option>
            <option v-for="item in tribunales" :key="item.value" :value="item.value">{{ item.label }}</option>
          </select>
        </div>

        <!-- Nombre Representante -->
        <div class="col-md-6">
          <input type="text" class="form-control" v-model="form.nombreRepresentante"
            placeholder="Nombre del representante" />
        </div>

        <!-- Fecha Audiencia -->
        <div class="col-md-3">
          <input type="date" class="form-control" v-model="form.fechaAudiencia" placeholder="Fecha de audiencia" />
        </div>

        <!-- Estatus -->
        <div class="col-md-3">
          <select class="form-select" v-model="form.estatus">
            <option disabled value="">Estatus</option>
            <option v-for="item in estatusList" :key="item.value" :value="item.value">{{ item.label }}</option>
          </select>
        </div>

        <!-- Documentos -->
        <div class="col-md-4 ">
          <label class="form-label">Adjuntar documentos del expediente</label>

          <!-- Expediente -->
          <div class="mb-2 btn">
            <label class="btn btn-primary w-100 custom-blue">
              <i class="bi bi-upload me-2"></i> Cargar expediente
              <input type="file" class="d-none" @change="handleExpedienteUpload" />
            </label>
          </div>

          <!-- Otros documentos -->
          <div class="mb-2 btn">
            <label class="btn btn-primary w-100 custom-blue">
              <i class="bi bi-paperclip me-2"></i> Otros documentos
              <input type="file" class="d-none"  @change="handleOtrosUpload" />
            </label>
          </div>


<!-- <div>
<file-upload :url="url" :thumb-url="thumbUrl" :headers="headers" @change="onFileChange"></file-upload>
</div> -->

        </div>
        <div class="text-center mt-4 btn-align-items-center">
        <button type="submit" class="btn p-button custom-blue ">
          <i class="bi bi-check-circle"></i> Registrar
        </button>
      </div>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const form = ref({
  fechaExpediente: new Date().toISOString().split('T')[0],
  noActo: '',
  fechaActo: '',
  cedulaDemandante: '',
  tipoDemanda: '',
  demandante: '',
  tiposDemandante: '',
  cedulaRepresentante: '',
  tribunal: '',
  nombreRepresentante: '',
  fechaAudiencia: '',
  estatus: '',
  id_usuario: 1 // Asegúrate de establecerlo correctamente
})

const expedienteFile = ref(null)
const otrosFile = ref(null)

const tiposDemandante = [
  { label: 'Empleado', value: 'Empleado' },
  { label: 'Empresa', value: 'Empresa' },
  { label: 'Otros', value: 'Otros' },
]

const tiposDemanda = ref([])
const tribunales = ref([])
const estatusList = ref([])

const cargarDatosDropdowns = async () => {
  try {
    const response = await fetch('/api/Litigio/datos-litigio')
    const data = await response.json()

    tiposDemanda.value = data.tiposDemanda.map(d => ({
      value: d.id_demanda,
      label: d.nombre
    }))

    tribunales.value = data.tribunales.map(t => ({
      value: t.id_Tribunal,
      label: t.nombre_Tribunal
    }))

    estatusList.value = data.estatusLitigios.map(e => ({
      value: e.ltg_estatus,
      label: e.ltg_description
    }))
  } catch (error) {
    console.error('Error al cargar los datos de los dropdowns:', error)
  }
}

onMounted(() => {
  form.value.fechaExpediente = new Date().toISOString().substring(0, 10)
  cargarDatosDropdowns()
})

const handleExpedienteUpload = (e) => {
  expedienteFile.value = e.target.files[0]
  if (expedienteFile.value) {
    console.log('Expediente seleccionado:', expedienteFile.value.name)
  }
}

const handleOtrosUpload = (e) => {
  otrosFile.value = e.target.files[0]
  if (otrosFile.value) {
    console.log('Otro documento seleccionado:', otrosFile.value.name)
  }
}

const registrarLitigio = async () => {
  if (!expedienteFile.value) {
    alert('Debe adjuntar un archivo de expediente.')
    return
  }

  const formData = new FormData()
  formData.append('ltg_acto', form.value.noActo)
  formData.append('ltg_Fecha_Acto', form.value.fechaActo)
  formData.append('id_Tipo_Demanda', form.value.tipoDemanda)
  formData.append('ltg_Cedula_Demandante', form.value.cedulaDemandante)
  formData.append('ltg_Nacionalidad', '') // Modifica si tienes este dato
  formData.append('ltg_Demandante', form.value.demandante)
  formData.append('ltg_Tipo_Demandante', form.value.tiposDemandante)
  formData.append('ltg_Cedula_Representante', form.value.cedulaRepresentante)
  formData.append('ltg_Nombre_Representante', form.value.nombreRepresentante)
  formData.append('ltg_Fecha_Audiencia', form.value.fechaAudiencia || '')
  formData.append('ltg_Fecha_Actualizacion', new Date().toISOString())
  formData.append('id_Tribunal', form.value.tribunal)
  formData.append('id_Sentencia', '') // Modifica si corresponde
  formData.append('id_usuario', form.value.id_usuario)
  formData.append('id_Estatus', form.value.estatus)
  formData.append('NombreCarpeta', form.value.noActo) // Para la ruta
  formData.append('Archivo', expedienteFile.value)

  try {
    const response = await fetch('https://localhost:7177/api/TuControlador/Subir_Litigio_Con_Archivo', {
      method: 'POST',
      body: formData
    })

    if (!response.ok) throw new Error('Error al registrar el litigio.')

    const result = await response.json()
    alert(result.mensaje) // Puedes cambiar esto por notivue
  } catch (error) {
    console.error('Error al registrar el litigio:', error)
    alert('Hubo un error al registrar el litigio.')
  }
}
</script>

<style scoped>
.card {
  background-color: #ffffff;
  border-radius: 0.75rem;
  border: 1px solid #ddd;
}





.custom-blue {
  background-color: #003870 !important;
  border-color: #dc3545 !important;
  color: white !important;
}
</style>
