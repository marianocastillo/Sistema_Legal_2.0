<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-2">
      <h2 class="text-2xl font-semibold">Modificar Registros</h2>
      <div style="width: 220px">
        <span class="p-float-label w-full">
          <Calendar id="fechaExpediente" v-model="form.fechaExpediente" dateFormat="dd/mm/yy" class="w-full" disabled />
          <!-- <label for="fechaExpediente">Fecha del expediente</label> -->
        </span>
      </div>
    </div>

    <div class="formgrid grid">
      <!-- No. Acto y Fecha del Acto -->
      <div class="field col-12 md:col-4">
        <span class="p-float-label">
          <InputText id="noActo" v-model="form.noActo" class="w-full" placeholder="No. Acto Alguacil *" />
        </span>
      </div>
      <div class="field col-12 md:col-4">
        <span class="p-float-label">
          <Calendar id="fechaActo" v-model="form.fechaActo" dateFormat="dd/mm/yy" class="w-full"
            placeholder="Fecha del acto " />
        </span>
      </div>

      <div class="field col-12 md:col-4">
        <span class="p-float-label">
          <InputText id="cedulaDemandante" v-model="form.cedulaDemandante" class="w-full"
            placeholder="Cédula del demandante" />
          <!-- <label for="cedulaDemandante">Cédula del demandante</label> -->
        </span>
      </div>

      <!-- Tipo de Demanda -->
      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <!-- <label for="tipoDemanda">Tipo de Demanda</label> -->
          <Dropdown id="tipoDemanda" v-model="form.tipoDemanda" :options="tiposDemanda" optionLabel="label"
            class="w-full w-full p-dropdown" placeholder="Tipo de Demanda" />

        </span>
      </div>

      <!-- Datos del Demandante -->

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <InputText id="demandante" v-model="form.demandante" class="w-full" placeholder="Demandante" />
          <!-- <label for="demandante">Demandante</label> -->
        </span>
      </div>
      <div class="field col-12 md:col-4">
        <span class="p-float-label">
          <!-- <label for="tipoDemanda">Tipo de Demanda</label> -->
          <Dropdown id="tipoDemandante" v-model="form.tiposDemandante" :options="tiposDemandante" optionLabel="label"
            class="w-full w-full p-dropdown" placeholder="Tipo de Demandante" />
        </span>
      </div>

      <!-- Representante -->
      <div class="field col-12 md:col-4">
        <span class="p-float-label">
          <InputText id="cedulaRepresentante" v-model="form.cedulaRepresentante" class="w-full"
            placeholder="Cédula del representante" />
          <!-- <label for="cedulaRepresentante">Cédula del representante</label> -->
        </span>
      </div>

      <div class="field col-12 md:col-4">
        <span class="p-float-label">
          <Dropdown id="tribunal" v-model="form.tribunal" :options="tribunales" optionLabel="label"
            class="w-full w-full p-dropdown" placeholder="Tribunal" />
          <!-- <label for="tribunal">Tribunal</label> -->
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <InputText id="nombreRepresentante" v-model="form.nombreRepresentante" class="w-full"
            placeholder="Nombre del representante" />
          <!-- <label for="nombreRepresentante">Nombre del representante</label> -->
        </span>
      </div>

      <!-- Tribunal -->

      <!-- Fecha Audiencia -->
      <div class="field col-12 md:col-3">
        <span class="p-float-label">
          <Calendar id="fechaAudiencia" v-model="form.fechaAudiencia" dateFormat="dd/mm/yy" class="w-full"
            placeholder="Fecha de audiencia" />
          <!-- <label for="fechaAudiencia">Fecha de audiencia</label> -->
        </span>
      </div>

      <!-- Estatus -->
      <div class="field col-12 md:col-3">
        <span class="p-dropdown">
          <Dropdown id="estatus" v-model="form.estatus" :options="estatusList" optionLabel="label"
            class="w-full p-dropdown mb-2" placeholder="Estatus" />
          <!-- <label class="block mb-5" for="estatus">Estatus</label> -->
        </span>
      </div>
      <br>

      <!-- Adjuntar documentos -->
      <div class="field col-4 align-left">

        <FileUpload mode="basic" name="otros" url="/api/upload" accept=".pdf,.doc,.docx" auto class="p-button custom-blue"
          chooseLabel="Evidencias Extra" />
      </div>

    </div>

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
