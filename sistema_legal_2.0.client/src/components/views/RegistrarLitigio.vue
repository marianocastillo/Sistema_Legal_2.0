
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

    <!-- DATOS DEL DEMANDANTE -->
    <fieldset class="col-12 border-1 border-round p-3 mb-3">
      <legend class="font-bold text-lg">Datos del Demandante</legend>

      <div class="grid">
        <div class="field col-12 md:col-4">
          <InputText v-model="form.cedulaDemandante" placeholder="Cédula del demandante" />
        </div>
        <div class="field col-12 md:col-4">
          <InputText v-model="form.demandante" placeholder="Nombre del demandante" />
        </div>


        <div class="field col-12 md:col-3">
          <Dropdown
            id="tipoDemandante"
            v-model="form.tipoDemandante"
            :options="tiposDemandante"
            optionLabel="label"
            optionValue="value"
            class="w-full p-dropdown"
            placeholder="Tipo de Demandante"
          />
        </div>

        <div class="field col-12 md:col-4" v-if="form.tipoDemandante === 'Otros'">
          <span class="p-float-label">
            <InputText
              id="otrosDemandante"
              v-model="form.otrosDemandante"
              class="w-full"
              placeholder="Especifique tipo de demandante"
            />

          </span>
        </div>

      </div>
    </fieldset>

    <!-- INFORMACIÓN DEL LITIGIO -->
    <fieldset class="col-12 border-1 border-round p-3 mb-3">
      <legend class="font-bold text-lg">Información del Litigio</legend>

      <div class="grid">
        <div class="field col-12 md:col-4">
          <InputText v-model="form.noActo" placeholder="No. Acto Alguacil *" />
        </div>
        <div class="field col-12 md:col-4">
          <Calendar v-model="form.fechaActo" dateFormat="yy-mm-dd" showIcon placeholder="Fecha del acto" class="w-7" />
        </div>
        <div class="field col-12 md:col-4">
          <Dropdown
          v-model="form.id_demanda"
            :options="tiposDemanda"
            optionLabel="nombre"
            optionValue="id_demanda"
            placeholder="Tipo de Demanda"
          />
        </div>

        <div class="field col-12 md:col-4">
          <Calendar v-model="form.fechaAudiencia" dateFormat="yy-mm-dd" showIcon placeholder="Fecha de audiencia" class="w-8"/>
        </div>
        <div class="field col-12 md:col-4">
          <Dropdown
            v-model="form.id_Tribunal"
            :options="tribunales"
            optionLabel="nombre_Tribunal"
            optionValue="id_Tribunal"
            placeholder="Tribunal"
            class="w-7"
          />
        </div>
        <div class="field col-12 md:col-4">
          <Dropdown
            v-model="form.ltg_estatus"
            :options="estatusLitigios"
            optionLabel="ltg_description"
            optionValue="ltg_estatus"
            placeholder="Estatus"
            class="w-7"
          />
        </div>
      </div>
    </fieldset>

    <!-- DATOS DEL REPRESENTANTE -->
    <fieldset class="col-12 border-1 border-round p-3 mb-3">
      <legend class="font-bold text-lg">Datos del Representante</legend>

      <div class="grid">
        <div class="field col-12 md:col-6">
          <InputText v-model="form.cedulaRepresentante" placeholder="Cédula del representante"  />
        </div>
        <div class="field col-12 md:col-6">
          <InputText v-model="form.nombreRepresentante" placeholder="Nombre del representante" class="w-7"/>
        </div>
      </div>
    </fieldset>

    <!-- ARCHIVOS -->
    <fieldset class="col-12 border-1 border-round p-3 mb-3">
      <legend class="font-bold text-lg">Documentos</legend>

      <div class="grid">
        <div class="field col-12 md:col-6">
          <label class="font-bold">Cargar Expediente</label>
          <FileUpload
            name="Archivo"
            customUpload
            @uploader="handleExpedienteUpload"
            mode="basic"
            chooseLabel="Elegir archivo"
          />
        </div>


      </div>
    </fieldset>
  </div>

  <!-- BOTÓN -->
  <div class="text-center mt-4">
    <Button type="submit" label="Registrar" icon="pi pi-check" class="p-button-primary" />
  </div>
</form>

  </div>
</template>


<script setup>
import { reactive, ref, onMounted } from 'vue'
import  InputText  from 'primevue/inputtext'
import  Calendar  from 'primevue/calendar'
import Dropdown  from 'primevue/dropdown'
import  FileUpload  from 'primevue/fileupload'
import  Button  from 'primevue/button'


const form = reactive({
  fechaExpediente: new Date().toISOString().split('T')[0],
  noActo: '',
  fechaActo: '',
  cedulaDemandante: '',
  tipoDemanda: '',
  demandante: '',
  tiposDemandante: '',
  cedulaRepresentante: '',
  id_Tribunal: null,
  nombreRepresentante: '',
  fechaAudiencia: '',
  ltg_estatus: null,
  id_usuario: 1,
  id_demanda: null,
  otrosDemandante: '',
})

const tiposDemanda = ref([])
const estatusLitigios = ref ([])
const tribunales = ref ([])
const otrosFile = ref(null)

const tiposDemandante = [
  { label: 'Empleado', value: 'Empleado' },
  { label: 'Empresa', value: 'Empresa' },
  { label: 'Otros', value: 'Otros' },
]

const cargarDatosDropdowns = async () => {
  try {
    const response = await fetch('/api/Litigio/datos-litigio')
    const data = await response.json()

    tiposDemanda.value = data.tiposDemanda // sin map
    tribunales.value = data.tribunales     // sin map
    estatusLitigios.value = data.estatusLitigios // sin map
  } catch (error) {
    console.error('Error al cargar los datos de los dropdowns:', error)
  }
}


onMounted(() => {
  form.fechaExpediente = new Date().toISOString().substring(0, 10)
  cargarDatosDropdowns()
})


const handleExpedienteUpload = (event) => {
  expedienteFile.value = event.files[0];
  if (expedienteFile.value) {
    console.log('Expediente seleccionado:', expedienteFile.value.name);
  }
}

const formatearFecha = (fecha) => {
  if (!fecha) return '';
  const d = new Date(fecha);
  const dia = String(d.getDate()).padStart(2, '0');
  const mes = String(d.getMonth() + 1).padStart(2, '0');
  const anio = d.getFullYear();
  return `${dia}-${mes}-${anio}`;
}



const registrarLitigio = async () => {

  const formData = new FormData()
  formData.append('ltg_acto', form.value.noActo)
  formData.append('ltg_Fecha_Acto', formatearFecha(form.value.fechaActo));
  formData.append('id_Tipo_Demanda', form.value.tipoDemanda)
  formData.append('ltg_Cedula_Demandante', form.value.cedulaDemandante)
  formData.append('ltg_Nacionalidad', '') // Modifica si tienes este dato
  formData.append('ltg_Demandante', form.value.demandante)
  formData.append('ltg_Tipo_Demandante', form.value.tipoDemandante === 'Otros' ? form.value.otrosDemandante : form.value.tipoDemandante)
  formData.append('ltg_Cedula_Representante', form.value.cedulaRepresentante)
  formData.append('ltg_Nombre_Representante', form.value.nombreRepresentante)
  formData.append('ltg_Fecha_Audiencia', formatearFecha(form.value.fechaAudiencia));
  formData.append('ltg_Fecha_Actualizacion', formatearFecha(new Date()));
  formData.append('id_Tribunal', form.value.tribunal)
  formData.append('id_Sentencia', '') // Modifica si corresponde
  formData.append('id_usuario', form.value.id_usuario)
  formData.append('id_Estatus', form.value.estatus)
  formData.append('NombreCarpeta', form.value.noActo) // Para la ruta
  formData.append('Archivo', expedienteFile.value)

  try {
    const response = await fetch('/api/Litigio/Subir_Litigio_Con_Archivo', {
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
