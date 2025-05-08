<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="text-2xl font-semibold">Modificar Registros</h2>
      <router-link to="/drawer/home" class="btn text-white" style="background-color: #003870;">
        <i class="fa-solid fa-home me-2"></i> Inicio
      </router-link>
    </div>

    <div class="formgrid grid">

      <!-- DATOS DEL DEMANDANTE -->
      <fieldset class="col-12 border-1 border-round p-3 mb-3">
        <legend class="font-bold text-lg">Datos del Demandante</legend>
        <div class="grid">
          <div class="field col-12 md:col-4">
            <InputText v-model="form.cedulaDemandante" class="w-8" placeholder="Cédula del demandante" />
          </div>
          <div class="field col-12 md:col-4">
            <InputText v-model="form.demandante" class="w-full" placeholder="Nombre del demandante" />
          </div>
          <div class="field col-12 md:col-4">
            <Dropdown v-model="form.tiposDemandante" :options="tiposDemandante" optionLabel="label" placeholder="Tipo de Demandante" class="w-full" />
          </div>
        </div>
      </fieldset>

      <!-- INFORMACIÓN DEL LITIGIO -->
      <fieldset class="col-12 border-1 border-round p-3 mb-3">
        <legend class="font-bold text-lg">Información del Litigio</legend>
        <div class="grid">
          <div class="field col-12 md:col-4">
            <InputText v-model="form.noActo" class="w-7" placeholder="No. Acto Alguacil *" />
          </div>
          <div class="field col-12 md:col-4">
            <Calendar v-model="form.fechaActo" dateFormat="dd/mm/yy" class="w-6" placeholder="Fecha del acto" />
          </div>
          <div class="field col-12 md:col-4">
            <Dropdown v-model="form.tipoDemanda" :options="tiposDemanda" optionLabel="label" placeholder="Tipo de Demanda" class="w-8" />
          </div>
          <div class="field col-12 md:col-4">
            <Calendar v-model="form.fechaAudiencia" dateFormat="dd/mm/yy" class="w-7" placeholder="Fecha de audiencia" />
          </div>
          <div class="field col-12 md:col-4">
            <Dropdown v-model="form.tribunal" :options="tribunales" optionLabel="label" placeholder="Tribunal" class="w-6" />
          </div>
          <div class="field col-12 md:col-4">
            <Dropdown v-model="form.estatus" :options="estatusList" optionLabel="label" placeholder="Estatus" class="w-6" />
          </div>
        </div>
      </fieldset>

      <!-- DATOS DEL REPRESENTANTE -->
    <fieldset class="col-12 border-1 border-round p-3 mb-3">
      <legend class="font-bold text-lg">Datos del Representante</legend>

      <div class="grid">
        <div class="field col-12 md:col-6">
          <InputText v-model="form.cedulaRepresentante" placeholder="Cédula del representante"  class="w-6"/>
        </div>
        <div class="field col-12 md:col-6 ">
          <InputText v-model="form.nombreRepresentante" placeholder="Nombre del representante" class="w-6"/>
        </div>
      </div>
    </fieldset>

     <!-- ARCHIVOS -->
    <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <div class="grid">
          <div class="field col-12 md:col-6">
          <label class="font-bold">Otras Evidencias</label>
          <FileUpload
            name="Otros"
            customUpload
            @uploader="handleOtrosUpload"
            mode="basic"
            chooseLabel="Elegir archivo"
          />
        </div>
      </div>
    </fieldset>

    </div>

    <!-- BOTÓN -->
    <div class="flex justify-content-center mt-4">
      <Button label="Actualizar Litigio" icon="pi pi-check" class="p-button custom-blue" @click="registrarLitigio" />
    </div>
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
  cedulaRepresentante: '',
  nombreRepresentante: '',
  tribunal: null,
  lugar: '',
  abogado: [],
  fechaAudiencia: null,
  estatus: null,
})

onMounted(() => {
  form.value.fechaExpediente = new Date()
})

const tiposDemanda = [
  { label: 'Civil', value: 'civil' },
  { label: 'Penal', value: 'penal' },
  { label: 'Laboral', value: 'laboral' },
]
const tiposDemandante = [

  { label: 'Empleado', value: 'Empleado' },
  { label: 'Empresa', value: 'Empresa' },
  { label: 'Otros', value: 'Otros' },

]

const tribunales = [
  { label: 'Tribunal DN', value: 'dn' },
  { label: 'Tribunal San Cristóbal', value: 'sc' },
  { label: 'Tribunal SDE', value: 'sde' },
]


const estatusList = [
  { label: 'Option 1', value: '1' },
  { label: 'Option 2', value: '2' },
  { label: 'Option 3', value: '3' },
]

const registrarLitigio = () => {
  console.log('Formulario enviado:', form.value)
  alert('¡Litigio registrado con éxito!')
}




</script>

<style>
.p-inputtext,
.p-dropdown,
.p-calendar,
.p-multiselect,
.p-float-label {
  background-color: white !important;
  /* Fondo blanco */
  color: #000 !important;
  /* Texto negro */
  border-color: #ccc !important;
  /* Borde gris claro */
}

/* Asegura que las etiquetas también sean de color negro */
.p-float-label label {
  color: #000000 !important;
  /* Color de texto negro para las etiquetas */
}

/* Opcional: cambiar el color del borde al enfocarse en los campos */
.p-inputtext:focus,
.p-dropdown:focus,
.p-calendar:focus,
.p-dropdown-item,
.p-dropdown-panel,
.p-dropdown-item.p-highlight,
.p-dropdown-item:hover,
.p-multiselect:focus {
  border-color: #007bff !important;
  /* Color de borde azul al enfocarse */
  box-shadow: 0 0 0 0.2rem rgba(38, 143, 255, 0.25);
  /* Sombra de enfoque */
  background-color: white !important;
  color: #000 !important;

}



.custom-blue {
  background-color: #003870 !important;
  border-color: #dc3545 !important;
  color: white !important;
}
</style>
