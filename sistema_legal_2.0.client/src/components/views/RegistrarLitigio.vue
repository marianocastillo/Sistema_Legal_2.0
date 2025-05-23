<template>
  <div class="card p-4 shadow-2">
    <div class="flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
      <h2 class="text-xl font-bold">Registro de Litigio</h2>
      <router-link to="/drawer/home" class="btn text-white px-3 py-2" style="background-color: #003870;">
        <i class="fa-solid fa-home me-2"></i> Inicio
      </router-link>
    </div>

    <form @submit.prevent="registrarLitigio">
      <div class="grid formgrid p-fluid">


        <!-- INFORMACIÓN DEL LITIGIO -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Información del Litigio</legend>
          <div class="grid">
            <div class="field col-12 md:col-4">
              <InputText v-model="form.ltg_acto" placeholder="No. Acto Alguacil *" class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Calendar v-model="form.ltg_Fecha_Acto" dateFormat="yy-mm-dd" showIcon placeholder="Fecha del acto"
                class="w-full" />
            </div>

            <div class="field col-12 md:col-4">
              <input list="tiposDemanda" id="tipoDemanda" v-model="form.nombre_Tipo_Demanda"
                class="p-inputtext p-component w-full" placeholder="--Seleccione el Tipo de Demanda--" />
              <datalist id="tiposDemanda">
                <option v-for="tipo in tiposDemanda" :key="tipo.id_demanda" :value="tipo.nombre" />
              </datalist>
            </div>

            <!-- <div class="field col-12 md:col-4">
              <Dropdown v-model="form.id_Tipo_Demanda" :options="tiposDemanda" optionLabel="nombre"
              optionValue="id_demanda" placeholder="--Seleccione el Tipo de Demanda--" class="w-full" />
            </div> -->

            <div class="field col-12 md:col-4">
              <Calendar v-model="form.ltg_Fecha_Audiencia" dateFormat="yy-mm-dd" showIcon
                placeholder="Fecha de audiencia" class="w-full" />
            </div>

            <div class="field col-12 md:col-4">
              <input list="listaTribunales" id="tribunal" v-model="form.nombre_Tribunal"
                class="p-inputtext p-component w-full" placeholder="--Seleccione Tribunal--" />
              <datalist id="listaTribunales">
                <option v-for="tribunal in tribunales" :key="tribunal.id_Tribunal" :value="tribunal.nombre_Tribunal" />
              </datalist>
            </div>

            <!-- <div class="field col-12 md:col-4">
              <Dropdown v-model="form.id_Tribunal" :options="tribunales" optionLabel="nombre_Tribunal"
                optionValue="id_Tribunal" placeholder="--Seleccione Tribunal--" class="w-full" />
            </div> -->



            <!-- <div class="field col-12 md:col-4">
              <Dropdown v-model="form.id_Estatus" :options="estatusLitigios" optionLabel="ltg_description"
              optionValue="ltg_estatus" placeholder="--Seleccione el Estatus--" class="w-full" />
            </div> -->
          </div>
        </fieldset>
        <!-- DATOS DEL DEMANDANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Demandante</legend>
          <div class="grid">
            <div class="field col-12 md:col-4">
              <Dropdown id="tipoDemandante" v-model="form.ltg_Tipo_Demandante" :options="tiposDemandante"
                optionLabel="label" optionValue="value" class="w-full"
                placeholder="--Seleccione Tipo de demandante--" />
            </div>

            <div class="field col-12 md:col-4" v-if="form.ltg_Tipo_Demandante === 'Otros'">
              <InputText id="otrosDemandante" v-model="form.otrosDemandante" class="w-full"
                placeholder="Especifique tipo de demandante" />
            </div>
            <div class="field col-12 md:col-3">
              <InputText v-model="form.ltg_Cedula_Demandante" :placeholder="form.ltg_Tipo_Demandante === 'Empresa'
                ? 'RNC de la empresa'
                : (form.ltg_Tipo_Demandante === 'Otros'
                  ? 'Identificación del demandante'
                  : 'Cédula del demandante')" class="w-full" @blur="obtenerDatosPorDocumento" />


            </div>

            <div class="field col-12 md:col-5">
              <InputText v-model="form.ltg_Demandante"
                :placeholder="form.ltg_Tipo_Demandante === 'Empresa' ? 'Nombre de la empresa' : 'Nombre del demandante'"
                class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <InputText v-model="form.ltg_Nacionalidad"
                :placeholder="form.ltg_Tipo_Demandante === 'Empresa' ? 'País de constitución' : 'Nacionalidad'"
                class="w-full" />
            </div>


          </div>
        </fieldset>

        <!-- DATOS DEL REPRESENTANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Representante</legend>
          <div class="grid">
            <div class="field col-12 md:col-3">
              <InputText v-model="form.ltg_Cedula_Representante" placeholder="Cédula del representante"
                class="w-full" />
            </div>
            <div class="field col-12 md:col-9">
              <InputText v-model="form.ltg_Nombre_Representante" placeholder="Nombre del representante"
                class="w-full" />
            </div>
          </div>
        </fieldset>

        <!-- ARCHIVOS -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Documentación</legend>
          <div class="grid">

            <div class="field col-12 md:col-4">
              <InputText v-model="form.NombreEvidencia" placeholder="Nombrar Evidencia" class="w-full" />
              <br> <br>
              <FileUpload name="Archivo" customUpload @select="handleExpedienteUpload" mode="basic"
                chooseLabel="Elegir archivo" class="w-full md:w-20rem" style="background-color: #003870;" />
            </div>
            <div class="field col-12 md:col-8">
              <Textarea v-model="form.comentario" placeholder="Añadir comentario" class="w-full" rows="5" autoResize />
            </div>
          </div>
        </fieldset>
      </div>

      <!-- BOTÓN -->
      <div class="text-center mt-4">
        <Button type="submit" label="Registrar" icon="pi pi-check" class="p-button-primary"
          style="background-color: #003870;" />
      </div>
    </form>
  </div>
</template>

<script setup>
import { watch } from 'vue';
import { reactive, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { push } from 'notivue'
import InputText from 'primevue/inputtext'
import Calendar from 'primevue/calendar'
import Dropdown from 'primevue/dropdown'
import FileUpload from 'primevue/fileupload'
import Button from 'primevue/button'

const router = useRouter()


const form = reactive({
  fechaExpediente: new Date().toISOString().split('T')[0],
  ltg_acto: '',
  ltg_Fecha_Acto: '',
  ltg_Cedula_Demandante: '',
  nombre_Tipo_Demanda: '',
  id_Tipo_Demanda: null,
  ltg_Demandante: '',
  ltg_Tipo_Demandante: '',
  ltg_Cedula_Representante: '',
  id_Tribunal: '',
  ltg_Nombre_Representante: '',
  ltg_Fecha_Audiencia: '',
  id_Estatus: '',
  ltg_Nacionalidad: '',
  otrosDemandante: '',
  id_sentencia: 3,
  comentario: '',
  NombreEvidencia: '',
})


watch(
  () => form.nombre_Tipo_Demanda,
  (nombre) => {
    const tipo = tiposDemanda.value.find(t => t.nombre === nombre);
    form.id_Tipo_Demanda = tipo ? tipo.id_demanda : null;
  }
);


const tiposDemanda = ref([])
const estatusLitigios = ref([])
const tribunales = ref([])
const expedienteFile = ref(null)


const tiposDemandante = [
  { label: 'Empleado', value: 'Empleado' },
  { label: 'Empresa', value: 'Empresa' },
  { label: 'Otros', value: 'Otros' },
]

const handleExpedienteUpload = (event) => {
  expedienteFile.value = event.files[0]
  console.log("Archivo cargado:", expedienteFile.value)
}

const cargarDatosDropdowns = async () => {
  try {
    const response = await fetch('/api/Litigio/datos-litigio')
    const data = await response.json()
    tiposDemanda.value = data.tiposDemanda
    estatusLitigios.value = data.estatusLitigios
    tribunales.value = [
      { id_Tribunal: null, nombre_Tribunal: '--Seleccione Tribunal--' },
      ...data.tribunales
    ]
  } catch (error) {
    console.error('Error al cargar los datos de los dropdowns:', error)
  }
}

const obtenerDatosPorDocumento = async () => {
  const documento = form.ltg_Cedula_Demandante?.trim()
  if (!documento || documento.length < 9) return

  try {
    const res = await fetch(`/api/Litigio/BuscarDocumento/${documento}`)
    const data = await res.json()

    console.log("Respuesta del backend:", data)

    form.ltg_Demandante = data.nombre
    form.ltg_Nacionalidad = data.nacionalidad
  } catch (error) {
    console.warn('No se encontró el documento:', error)
    form.ltg_Demandante = ''
    form.ltg_Nacionalidad = ''
  }

  console.log("Nombre:", form.ltg_Demandante, "Nacionalidad:", form.ltg_Nacionalidad)
}



onMounted(() => {
  cargarDatosDropdowns()
})

const formatearFechaISO = (fecha) => {
  if (!fecha) return ''
  const d = new Date(fecha)
  return d.toISOString().split('T')[0]
}


const registrarLitigio = async () => {



  if (!form.ltg_acto || !expedienteFile.value) {
    push.warning('Favor de llenar los campos con datos validos')
    console.error("Faltan datos obligatorios como el acto o el archivo.")
    return
  }

  const usuarioLogueado = JSON.parse(localStorage.getItem('usuario'));
  form.id_usuario = usuarioLogueado.idUsuario;


  const formData = new FormData()

  formData.append('ltg_acto', form.ltg_acto)
  formData.append("ltg_Fecha_Acto", formatearFechaISO(form.ltg_Fecha_Acto))
  formData.append('id_Tipo_Demanda', parseInt(form.id_Tipo_Demanda))
  formData.append('ltg_Cedula_Demandante', form.ltg_Cedula_Demandante)
  formData.append('ltg_Demandante', form.ltg_Demandante)
  formData.append('ltg_Tipo_Demandante', form.ltg_Tipo_Demandante === 'Otros' ? form.otrosDemandante : form.ltg_Tipo_Demandante)
  formData.append('ltg_Cedula_Representante', form.ltg_Cedula_Demandante)
  formData.append('comentario', form.comentario)
  formData.append('NombreEvidencia', form.NombreEvidencia)
  formData.append('ltg_Nombre_Representante', form.ltg_Nombre_Representante)
  formData.append("ltg_Fecha_Audiencia", formatearFechaISO(form.ltg_Fecha_Audiencia))
  formData.append('ltg_Fecha_Actualizacion', formatearFechaISO(new Date()))
  if (form.id_Tribunal !== null && form.id_Tribunal !== '') {
    formData.append('id_Tribunal', parseInt(form.id_Tribunal));
  }
  formData.append('ltg_Nacionalidad', form.ltg_Nacionalidad)
  formData.append('id_Sentencia', parseInt(form.id_sentencia))
  formData.append('id_usuario', parseInt(form.id_usuario))
  formData.append('id_Estatus', 1)
  formData.append('NombreCarpeta', form.ltg_acto)

  if (expedienteFile.value) {
    formData.append('Archivo', expedienteFile.value)
  }

  try {
    const response = await fetch('/api/Litigio/Subir_Litigio_Con_Archivo', {
      method: 'POST',
      body: formData
    })
    const result = await response.json()
    push.success('El litigio a sido cargado de forma exitosa')
    console.log('Respuesta del backend:', result)

    setTimeout(() => {
      router.push('/drawer/home')
    }, 1000);



  } catch (error) {
    console.error('Error al registrar el litigio:', error)
  }
}
</script>

<style scoped>
legend {
  float: none !important;
  display: inline-block !important;
  padding: 0 5px !important;
  margin-bottom: 0.5rem !important;
  line-height: inherit !important;
  font-size: calc(1.275rem + 0.3vw) !important;
  width: auto !important;
  color: #003870;
}

.border-1 {
  border: 1px solid #ccc;
}
</style>
