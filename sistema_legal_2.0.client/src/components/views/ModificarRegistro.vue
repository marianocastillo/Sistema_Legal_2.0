<template>
  <div class="card p-4 shadow-2">
    <div class="flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
      <h2 class="text-xl font-bold">Modificar Litigio</h2>
      <router-link to="/drawer/home" class="btn text-white px-3 py-2" style="background-color: #003870;">
        <i class="fa-solid fa-home me-2"></i> Inicio
      </router-link>
    </div>

    <form @submit.prevent="registrarLitigio">
      <div class="grid formgrid p-fluid">

        <!-- DATOS DEL DEMANDANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Demandante</legend>
          <div class="grid">
            <div class="field col-12 md:col-4">
              <InputText v-model="form.cedulaDemandante" class="w-full"
                :placeholder="form.tiposDemandante === 'Empresa' ? 'RNC de la empresa' : 'Cédula del demandante'" />
            </div>
            <div class="field col-12 md:col-4">
              <InputText v-model="form.demandante" class="w-full"
                :placeholder="form.tiposDemandante === 'Empresa' ? 'Nombre de la empresa' : 'Nombre del demandante'" />
            </div>
            <div class="field col-12 md:col-4">
              <InputText v-model="form.Nacionalidad"
                :placeholder="form.tiposDemandante === 'Empresa' ? 'País de constitución' : 'Nacionalidad'"
                class="w-full"
              />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.tiposDemandante" :options="tiposDemandante" optionLabel="label" optionValue="value"
                class="w-full" placeholder="Tipo de Demandante" />
            </div>
            <div class="field col-12 md:col-4" v-if="form.tiposDemandante === 'Otros'">
              <InputText v-model="form.otrosDemandante" class="w-full" placeholder="Especifique tipo de demandante" />
            </div>
          </div>
        </fieldset>

        <!-- INFORMACIÓN DEL LITIGIO -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Información del Litigio</legend>
          <div class="grid">
            <div class="field col-12 md:col-4">
              <InputText v-model="form.noActo" class="w-full" placeholder="No. Acto Alguacil *" />
            </div>
            <div class="field col-12 md:col-4">
              <Calendar v-model="form.fechaActo" dateFormat="yy-mm-dd" showIcon placeholder="Fecha del acto" class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.tipoDemanda" :options="tiposDemanda" optionLabel="label" optionValue="value"
                placeholder="Tipo de Demanda" class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Calendar v-model="form.fechaAudiencia" dateFormat="yy-mm-dd" showIcon placeholder="Fecha de audiencia"
                class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.tribunal" :options="tribunales" optionLabel="label" optionValue="value"
                placeholder="Tribunal" class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.estatus" :options="estatusList" optionLabel="label" optionValue="value"
                placeholder="Estatus" class="w-full" />
            </div>
          </div>
        </fieldset>

        <!-- DATOS DEL REPRESENTANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Representante</legend>
          <div class="grid">
            <div class="field col-12 md:col-6">
              <InputText v-model="form.cedulaRepresentante" placeholder="Cédula del representante" class="w-full" />
            </div>
            <div class="field col-12 md:col-6">
              <InputText v-model="form.nombreRepresentante" placeholder="Nombre del representante" class="w-full" />
            </div>
          </div>
        </fieldset>

        <!-- ARCHIVOS -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Otras Evidencias</legend>
          <div class="grid">
            <div class="field col-12 md:col-6">
              <label class="font-bold" style="color: #003870;">Cargar Archivo</label>
              <FileUpload name="Otros" customUpload @uploader="handleOtrosUpload" mode="basic"
                chooseLabel="Elegir archivo" class="w-full md:w-20rem" />
            </div>
          </div>
        </fieldset>
      </div>

      <!-- BOTÓN -->
      <div class="text-center mt-4">
        <Button type="submit" label="Actualizar Litigio" icon="pi pi-check" class="p-button custom-blue" />
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import InputText from 'primevue/inputtext'
import Calendar from 'primevue/calendar'
import Dropdown from 'primevue/dropdown'
import FileUpload from 'primevue/fileupload'
import Button from 'primevue/button'

const form = ref({
  noActo: '',
  fechaActo: null,
  fechaExpediente: null,
  tipoDemanda: null,
  cedulaDemandante: '',
  demandante: '',
  tiposDemandante: null,
  otrosDemandante: '',
  cedulaRepresentante: '',
  nombreRepresentante: '',
  Nacionalidad: '',
  tribunal: null,
  lugar: '',
  abogado: [],
  fechaAudiencia: null,
  estatus: null
})

onMounted(() => {
  form.value.fechaExpediente = new Date()
})

const tiposDemanda = [
  { label: 'Civil', value: 'civil' },
  { label: 'Penal', value: 'penal' },
  { label: 'Laboral', value: 'laboral' }
]

const tiposDemandante = [
  { label: 'Empleado', value: 'Empleado' },
  { label: 'Empresa', value: 'Empresa' },
  { label: 'Otros', value: 'Otros' }
]

const tribunales = [
  { label: 'Tribunal DN', value: 'dn' },
  { label: 'Tribunal San Cristóbal', value: 'sc' },
  { label: 'Tribunal SDE', value: 'sde' }
]

const estatusList = [
  { label: 'Option 1', value: '1' },
  { label: 'Option 2', value: '2' },
  { label: 'Option 3', value: '3' }
]

const registrarLitigio = () => {
  console.log('Formulario enviado:', form.value)
  alert('Litigio actualizado con éxito!')
}

const handleOtrosUpload = (event) => {
  console.log('Archivo cargado:', event.files[0]?.name || 'Sin archivo')
}
</script>

<style scoped>
legend {
  float: none !important;
  display: inline-block !important;
  padding: 0 5px 0 5px !important;
  margin-bottom: 0.5rem !important;
  line-height: inherit !important;
  font-size: calc(1.275rem + 0.3vw) !important;
  width: auto !important;
  color: #003870;
}

.border-1 {
  border: 1px solid #ccc;
}

.custom-blue {
  background-color: #003870 !important;
  border-color: #003870 !important;
  color: white !important;
}
</style>
