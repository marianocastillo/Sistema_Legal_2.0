<template>
  <div class="card p-4 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="text-xl font-bold">Registro de Litigio</h2>
      <router-link to="/drawer/home" class="btn text-white me-8" style="background-color: #003870;">
          <i class="fa-solid fa-home me-2"></i> Inicio
        </router-link>
    </div>

    <form @submit.prevent="registrarLitigio">
      <div class="grid formgrid p-fluid">
        <!-- No. Acto -->
        <div class="field col-12 md:col-4">
          <InputText v-model="form.noActo" placeholder="No. Acto Alguacil *" />
        </div>

        <!-- Fecha Acto -->
        <div class="field col-12 md:col-4">
          <Calendar v-model="form.fechaActo" dateFormat="yy-mm-dd" showIcon placeholder="Fecha del acto" />
        </div>

        <!-- Cédula Demandante -->
        <div class="field col-12 md:col-4">
          <InputText v-model="form.cedulaDemandante" placeholder="Cédula del demandante" />
        </div>

        <!-- Tipo de Demanda -->
        <div class="field col-12 md:col-6">
          <Dropdown
            v-model="form.tipoDemanda"
            :options="tiposDemanda"
            optionLabel="label"
            optionValue="value"
            placeholder="Tipo de Demanda"
          />
        </div>

        <!-- Demandante -->
        <div class="field col-12 md:col-6">
          <InputText v-model="form.demandante" placeholder="Nombre demandante" />
        </div>

        <!-- Tipo de Demandante -->
        <div class="field col-12 md:col-4">
          <Dropdown
            v-model="form.tiposDemandante"
            :options="tiposDemandante"
            optionLabel="label"
            optionValue="value"
            placeholder="Tipo de Demandante"
          />
        </div>

        <!-- Cédula Representante -->
        <div class="field col-12 md:col-4">
          <InputText v-model="form.cedulaRepresentante" placeholder="Cédula del representante" />
        </div>

        <!-- Tribunal -->
        <div class="field col-12 md:col-4">
          <Dropdown
            v-model="form.tribunal"
            :options="tribunales"
            optionLabel="label"
            optionValue="value"
            placeholder="Tribunal"
          />
        </div>

        <!-- Nombre Representante -->
        <div class="field col-12 md:col-6">
          <InputText v-model="form.nombreRepresentante" placeholder="Nombre del representante" />
        </div>

        <!-- Fecha Audiencia -->
        <div class="field col-12 md:col-3">
          <Calendar v-model="form.fechaAudiencia" dateFormat="yy-mm-dd" showIcon placeholder="Fecha de audiencia" />
        </div>

        <!-- Estatus -->
        <div class="field col-12 md:col-3">
          <Dropdown
            v-model="form.estatus"
            :options="estatusList"
            optionLabel="label"
            optionValue="value"
            placeholder="Estatus"
          />
        </div>

        <!-- Subidas -->
        <div class="field col-12 md:col-6">
          <label class="font-bold">Cargar Expediente</label>
          <FileUpload
            name="Archivo"
            customUpload
            @uploader="handleExpedienteUpload"
            mode="basic"
            chooseLabel="Elegir archivo"
          />

          <label class="font-bold mt-3">Otras Evidencias</label>
          <FileUpload
            name="Otros"
            customUpload
            @uploader="handleOtrosUpload"
            mode="basic"
            chooseLabel="Elegir archivo"
          />
        </div>
      </div>

      <div class="text-center mt-4">
        <Button type="submit" label="Registrar" icon="pi pi-check" class="p-button-primary" />
      </div>
    </form>
  </div>
</template>


<script setup>
import { ref, onMounted } from 'vue'
import  InputText  from 'primevue/inputtext'
import  Calendar  from 'primevue/calendar'
import Dropdown  from 'primevue/dropdown'
import  FileUpload  from 'primevue/fileupload'
import  Button  from 'primevue/button'

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
const url = ref('');
const thumbUrl = ref('');
const headers = ref([]);


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

</style>
