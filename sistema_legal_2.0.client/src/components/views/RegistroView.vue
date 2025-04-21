<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-2">
      <h2 class="text-2xl font-semibold">Registro de Litigio</h2>
      <div style="width: 220px">
        <span class="p-float-label w-full block">
          <Calendar id="fechaExpediente" v-model="form.fechaExpediente" dateFormat="dd/mm/yy" class="w-full" disabled />
          <label for="fechaExpediente">Fecha del expediente</label>
        </span>
      </div>
    </div>

    <div class="formgrid grid">
      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <InputText id="noActo" v-model="form.noActo" class="w-full" />
          <label for="noActo">No. Acto Alguacil *</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <Calendar id="fechaActo" v-model="form.fechaActo" dateFormat="dd/mm/yy" class="w-full" />
          <label for="fechaActo">Fecha del acto *</label>
        </span>
      </div>

      <div class="field col-12">
        <span class="p-float-label">
          <Dropdown id="tipoDemanda" v-model="form.tipoDemanda" :options="tiposDemanda" optionLabel="label" class="w-full" />
          <label for="tipoDemanda">Tipo de Demanda</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <InputText id="cedulaDemandante" v-model="form.cedulaDemandante" class="w-full" />
          <label for="cedulaDemandante">Cédula del demandante</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <InputText id="demandante" v-model="form.demandante" class="w-full" />
          <label for="demandante">Demandante</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <InputText id="cedulaRepresentante" v-model="form.cedulaRepresentante" class="w-full" />
          <label for="cedulaRepresentante">Cédula del representante</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <InputText id="nombreRepresentante" v-model="form.nombreRepresentante" class="w-full" />
          <label for="nombreRepresentante">Nombre del representante</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <Dropdown id="tribunal" v-model="form.tribunal" :options="tribunales" optionLabel="label" class="w-full" />
          <label for="tribunal">Tribunal</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <InputText id="lugar" v-model="form.lugar" class="w-full" />
          <label for="lugar">Lugar</label>
        </span>
      </div>

      <div class="field col-12">
        <span class="p-float-label">
          <MultiSelect id="abogado" v-model="form.abogado" :options="abogados" optionLabel="label" class="w-full" />
          <label for="abogado">Abogado</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <Calendar id="fechaAudiencia" v-model="form.fechaAudiencia" dateFormat="dd/mm/yy" class="w-full" />
          <label for="fechaAudiencia">Fecha de audiencia</label>
        </span>
      </div>

      <div class="field col-12 md:col-6">
        <span class="p-float-label">
          <Dropdown id="estatus" v-model="form.estatus" :options="estatusList" optionLabel="label" class="w-full" />
          <label for="estatus">Estatus</label>
        </span>
      </div>

      <!-- Botones para cargar documentos -->
      <div class="field col-12">
        <label class="block mb-2">Adjuntar documentos del expediente</label>
        <div class="flex gap-3 flex-wrap">
          <FileUpload mode="basic" name="expediente" url="/api/upload" accept=".pdf,.doc,.docx" auto chooseLabel="Cargar expediente" />
          <FileUpload mode="basic" name="otros" url="/api/upload" accept=".pdf,.doc,.docx" auto chooseLabel="Otros documentos" />
        </div>
      </div>
    </div>

    <!-- Botón registrar centrado -->
    <div class="flex justify-content-center mt-4">
      <Button label="Registrar" icon="pi pi-check" class="p-button-success p-button-lg" @click="registrarLitigio" />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import InputText from 'primevue/inputtext'
import Calendar from 'primevue/calendar'
import Dropdown from 'primevue/dropdown'
import MultiSelect from 'primevue/multiselect'
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

const tribunales = [
  { label: 'Tribunal DN', value: 'dn' },
  { label: 'Tribunal San Cristóbal', value: 'sc' },
  { label: 'Tribunal SDE', value: 'sde' },
]

const abogados = [
  { label: 'Ana Sánchez', value: 'ana' },
  { label: 'Carlos Peña', value: 'carlos' },
  { label: 'Laura Gómez', value: 'laura' },
]

const estatusList = [
  { label: 'Pendiente', value: 'pendiente' },
  { label: 'En proceso', value: 'proceso' },
  { label: 'Finalizado', value: 'finalizado' },
]

const registrarLitigio = () => {
  console.log('Formulario enviado:', form.value)
  alert('¡Litigio registrado con éxito!')
}
</script>

<style scoped>
/* Opcional: botón más grande */
.p-button-lg {
  font-size: 1.1rem;
  padding: 0.8rem 2rem;
  border-radius: 8px;
}
</style>
