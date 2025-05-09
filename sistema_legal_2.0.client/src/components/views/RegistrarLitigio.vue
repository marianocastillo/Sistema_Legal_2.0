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
              <InputText v-model="form.ltg_Cedula_Demandante" placeholder="Cédula del demandante" />
            </div>

            <div class="field col-12 md:col-4">
              <InputText v-model="form.ltg_Demandante" placeholder="Nombre del demandante" />
            </div>

            <div class="field col-12 md:col-4">
              <InputText v-model="form.ltg_Nacionalidad" placeholder="Nacionalidad" />
            </div>



            <div class="field col-12 md:col-3">
              <Dropdown id="tipoDemandante" v-model="form.ltg_Tipo_Demandante" :options="tiposDemandante"
                optionLabel="label" optionValue="value" class="w-full p-dropdown" placeholder="Tipo de Demandante" />
            </div>

            <div class="field col-12 md:col-4" v-if="form.ltg_Tipo_Demandante === 'Otros'">
              <span class="p-float-label">
                <InputText id="otrosDemandante" v-model="form.otrosDemandante" class="w-full"
                  placeholder="Especifique tipo de demandante" />

              </span>
            </div>

          </div>
        </fieldset>

        <!-- INFORMACIÓN DEL LITIGIO -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Información del Litigio</legend>

          <div class="grid">
            <div class="field col-12 md:col-4">
              <InputText v-model="form.ltg_acto" placeholder="No. Acto Alguacil *" />
            </div>
            <div class="field col-12 md:col-4">
              <Calendar v-model="form.ltg_Fecha_Acto" dateFormat="yy-mm-dd" showIcon placeholder="Fecha del acto"
                class="w-7" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.id_Tipo_Demanda" :options="tiposDemanda" optionLabel="nombre"
                optionValue="id_demanda" placeholder="Tipo de Demanda" />
            </div>

            <div class="field col-12 md:col-4">
              <Calendar v-model="form.ltg_Fecha_Audiencia" dateFormat="yy-mm-dd" showIcon
                placeholder="Fecha de audiencia" class="w-8" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.id_Tribunal" :options="tribunales" optionLabel="nombre_Tribunal"
                optionValue="id_Tribunal" placeholder="Tribunal" class="w-7" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.id_Estatus" :options="estatusLitigios" optionLabel="ltg_description"
                optionValue="ltg_estatus" placeholder="Estatus" class="w-7" />
            </div>
          </div>
        </fieldset>

        <!-- DATOS DEL REPRESENTANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Representante</legend>

          <div class="grid">
            <div class="field col-12 md:col-6">
              <InputText v-model="form.ltg_Cedula_Representante" placeholder="Cédula del representante" />
            </div>
            <div class="field col-12 md:col-6">
              <InputText v-model="form.ltg_Nombre_Representante" placeholder="Nombre del representante" class="w-7" />
            </div>
          </div>
        </fieldset>

        <!-- ARCHIVOS -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">

          <legend class="font-bold text-lg">Documentos</legend>

          <div class="grid">
            <div class="field col-12 md:col-6">
              <label class="font-bold" style="color: #003870;">Cargar Expediente</label>
              <FileUpload name="Archivo" customUpload @select="handleExpedienteUpload" mode="basic"
                chooseLabel="Elegir archivo" />
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
import InputText from 'primevue/inputtext'
import Calendar from 'primevue/calendar'
import Dropdown from 'primevue/dropdown'
import FileUpload from 'primevue/fileupload'
import Button from 'primevue/button'


const form = reactive({
  fechaExpediente: new Date().toISOString().split('T')[0],
  ltg_acto: '',
  ltg_Fecha_Acto: '',
  ltg_Cedula_Demandante: '',
  id_Tipo_Demanda: '',
  ltg_Demandante: '',
  ltg_Tipo_Demandante: '',
  ltg_Cedula_Representante: '',
  id_Tribunal: '',
  ltg_Nombre_Representante: '',
  ltg_Fecha_Audiencia: '',
  id_Estatus: '',
  id_usuario: 1,
  ltg_Nacionalidad: '',
  otrosDemandante: '',
  id_sentencia: 3,
})

const tiposDemanda = ref([])
const estatusLitigios = ref([])
const tribunales = ref([])
const otrosFile = ref(null)

const tiposDemandante = [
  { label: 'Empleado', value: 'Empleado' },
  { label: 'Empresa', value: 'Empresa' },
  { label: 'Otros', value: 'Otros' },
]

const expedienteFile = ref(null)

const handleExpedienteUpload = (event) => {
  expedienteFile.value = event.files[0];
  console.log("Archivo cargado:", expedienteFile.value); // Verifica el archivo cargado
}

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


const formatearFecha = (fecha) => {
  if (!fecha) return '';
  const d = new Date(fecha);
  const dia = String(d.getDate()).padStart(2, '0');
  const mes = String(d.getMonth() + 1).padStart(2, '0');
  const anio = d.getFullYear();
  return `${dia}-${mes}-${anio}`;
}



const registrarLitigio = async () => {

  if (!form.ltg_acto || !expedienteFile.value) {
    console.error("Faltan datos obligatorios como el acto o el archivo.");
    return;
  }

  const formData = new FormData()

  const parseOrNull = (value) => {
    const parsed = parseInt(value);
    return isNaN(parsed) ? null : parsed;
  };

  const formatearFechaISO = (fecha) => {
  if (!fecha) return '';
  const d = new Date(fecha);
  return d.toISOString().split('T')[0]; // "2025-05-26"
};

  formData.append('ltg_acto', form.ltg_acto.toString());
  formData.append("ltg_Fecha_Acto", formatearFechaISO(form.ltg_Fecha_Acto));
  formData.append('id_Tipo_Demanda', parseInt(form.id_Tipo_Demanda))
  formData.append('ltg_Cedula_Demandante', form.ltg_Cedula_Demandante)
  formData.append('ltg_Demandante', form.ltg_Demandante)
  formData.append('ltg_Tipo_Demandante', form.ltg_Tipo_Demandante === 'Otros' ? form.otrosDemandante : form.ltg_Tipo_Demandante)
  formData.append('ltg_Cedula_Representante', form.ltg_Cedula_Demandante)
  formData.append('ltg_Nombre_Representante', form.ltg_Nombre_Representante)
  formData.append("ltg_Fecha_Audiencia", formatearFechaISO(form.ltg_Fecha_Audiencia));
  formData.append('ltg_Fecha_Actualizacion', formatearFecha(new Date()));
  formData.append('id_Tribunal', parseInt(form.id_Tribunal))
  formData.append('ltg_Nacionalidad', form.ltg_Nacionalidad)
  formData.append('id_Sentencia', parseInt(form.id_sentencia));
  formData.append('id_usuario', parseInt(form.id_usuario))
  formData.append('id_Estatus', parseInt(form.id_Estatus))
  formData.append('NombreCarpeta', form.ltg_acto.toString());

  if (expedienteFile.value) {
    formData.append('Archivo', expedienteFile.value);
  }
  for (const [key, value] of formData.entries()) {
    console.log(`${key}:`, value);
  }

  try {
    const response = await fetch('/api/Litigio/Subir_Litigio_Con_Archivo', {
      method: 'POST',
      body: formData
    });




    const result = await response.json();
    console.log('Respuesta del backend:', result); // Cambié de `response.data` a `result`
  } catch (error) {
    console.error('Error al registrar el litigio:', error);
  }
};
</script>

<style scope>
legend {
  float: none !important;
  display: inline-block !important;
  padding: 0 5px 0 5px !important;
  margin-bottom: 0.5rem !important;
  line-height: inherit !important;
  font-size: calc(1.275rem + 0.3vw) !important;
  /* Elimina el ancho fijo */
  width: auto !important;
  color: #003870;
}

.border-1 {
  color: red;
}
</style>
