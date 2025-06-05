<template>
  <div class="card p-4 shadow-2">
    <div class="d-flex justify-content-between align-items-center flex-wrap mb-3">
      <!-- Título -->
      <h2 class="h4 fw-bold mb-2 mb-md-0">Registro de Litigio</h2>

      <!-- Botones alineados a la derecha -->
      <div class="d-flex gap-2">
        <!-- Botón Registrar -->
        <Button type="submit" label="Registrar" icon="pi pi-check" class="p-button-sm" :disabled="enviando"
          @click="formRef?.requestSubmit()" style="background-color: #003870; border-color: #003870;" />

        <!-- Botón Inicio -->
        <router-link to="/home" class="btn btn-sm text-white d-flex align-items-center custom-home-btn">
          <i class="pi pi-home me-2"></i>
          Inicio
        </router-link>
      </div>
    </div>





    <form ref="formRef" @submit.prevent="registrarLitigio">
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

          </div>
        </fieldset>


        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Información de la audienica</legend>
          <div class="grid">


            <div class="field col-12 md:col-4">
              <Calendar v-model="form.ltg_Fecha_Audiencia" dateFormat="yy-mm-dd" showIcon
                placeholder="Fecha de audiencia" class="w-full" :minDate="hoy" />
            </div>

            <div class="field col-12 md:col-4">
              <input list="listaTribunales" id="tribunal" v-model="form.nombre_Tribunal"
                class="p-inputtext p-component w-full" placeholder="--Seleccione Tribunal--" />
              <datalist id="listaTribunales">
                <option v-for="tribunal in tribunales" :key="tribunal.id_Tribunal" :value="tribunal.nombre_Tribunal" />
              </datalist>
            </div>

            <div class="field col-12 md:col-4">
              <Dropdown id="tipoDemandante" v-model="form.Tipo_audiencia" :options="tiposAudiencia" optionLabel="label"
                optionValue="value" class="w-full" placeholder="--Seleccione Tipo de Audiencia--" />

            </div>
          </div>
        </fieldset>
        <!-- DATOS DEL DEMANDANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Demandante</legend>
          <div class="grid">
            <div class="field col-12 md:col-3">
              <Dropdown id="tipoDemandante" v-model="form.ltg_Tipo_Demandante" :options="tiposDemandante"
                optionLabel="label" optionValue="value" class="w-full"
                placeholder="--Seleccione Tipo de demandante--" />
            </div>

            <div class="field col-12 md:col-4" v-if="form.ltg_Tipo_Demandante === 'Otros'">
              <InputText id="otrosDemandante" v-model="form.otrosDemandante" class="w-full"
                placeholder="Especifique tipo de demandante" />
            </div>
            <div class="field col-12 md:col-2">
              <InputText v-model="form.ltg_Cedula_Demandante"
                @blur="() => buscarPersonaPorDocumento(form.ltg_Cedula_Demandante, 'ltg_Demandante', 'ltg_Nacionalidad')"
                class="w-full"
                :placeholder="form.ltg_Tipo_Demandante === 'Empresa' ? 'RNC de la empresa' : 'Cédula del demandante'" />


            </div>

            <div class="field col-12 md:col-4">
              <InputText v-model="form.ltg_Demandante"
                :placeholder="form.ltg_Tipo_Demandante === 'Empresa' ? 'Nombre de la empresa' : 'Nombre del demandante'"
                class="w-full" />
            </div>
            <div class="field col-12 md:col-3">
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

            <div class="field col-12 md:col-2">
              <InputText v-model="form.ltg_Cedula_Representante"
                @blur="() => buscarPersonaPorDocumento(form.ltg_Cedula_Representante, 'ltg_Nombre_Representante', 'ltg_Nacionalidad_Representante')"
                class="w-full" placeholder="Cedula del Representante" />

            </div>

            <div class="field col-12 md:col-5">
              <InputText v-model="form.ltg_Nombre_Representante" placeholder="Nombre del representante"
                class="w-full" />
            </div>
            <div class="field col-12 md:col-5">
              <InputText v-model="form.ltg_Nacionalidad_Representante" placeholder="Nacionalidad del representante"
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
const hoy = ref(new Date()) // Esto representa la fecha de hoy

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
  ltg_Nacionalidad_Representante: '',
  Tipo_audiencia: '',

})


watch(
  () => form.nombre_Tipo_Demanda,
  (nombre) => {
    const tipo = tiposDemanda.value.find(t => t.nombre === nombre);
    form.id_Tipo_Demanda = tipo ? tipo.id_demanda : null;
  }
);

watch(
  () => form.nombre_Tribunal,
  (nombre) => {
    const tribunal = tribunales.value.find(t => t.nombre_Tribunal === nombre)
    form.id_Tribunal = tribunal ? tribunal.id_Tribunal : null
  }
)

const formRef = ref(null);
const tiposDemanda = ref([])
const estatusLitigios = ref([])
const tribunales = ref([])
const expedienteFile = ref(null)


const tiposDemandante = [
  { label: 'Empleado', value: 'Empleado' },
  { label: 'Empresa', value: 'Empresa' },
]

const tiposAudiencia = [
  { label: 'Audiencia previa', value: 'Audiencia previa' },
  { label: 'Preliminar', value: 'Preliminar' },
  { label: 'Juicio', value: 'Juicio' },
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
    tribunales.value = data.tribunales
  } catch (error) {
    console.error('Error al cargar los datos de los dropdowns:', error)
  }
}

const buscarPersonaPorDocumento = async (documento, campoNombre, campoNacionalidad) => {
  if (!documento || documento.trim().length < 9) return

  try {
    const res = await fetch(`/api/Litigio/BuscarDocumento/${documento.trim()}`)
    const data = await res.json()

    form[campoNombre] = data.nombre
    form[campoNacionalidad] = data.nacionalidad
  } catch (error) {
    console.warn(`No se encontró el documento ${documento}`, error)
    form[campoNombre] = ''
    form[campoNacionalidad] = ''
  }
}
const enviando = ref(false)



onMounted(() => {
  cargarDatosDropdowns()
})

const formatearFechaISO = (fecha) => {
  if (!fecha) return ''
  const d = new Date(fecha)
  return d.toISOString().split('T')[0]
}


const registrarLitigio = async () => {

  if (enviando.value) return
  enviando.value = true

  if (!form.ltg_acto || !expedienteFile.value || !form.ltg_Cedula_Demandante || !form.ltg_Cedula_Representante || !form.ltg_Fecha_Acto || !form.ltg_Tipo_Demandante) {
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
  formData.append('NombreEvidencia',
    form.NombreEvidencia?.trim()
      ? form.NombreEvidencia
      : expedienteFile.value?.name?.split(".")[0] || "Evidencia"
  )

  formData.append('comentario',
    form.comentario?.trim()
      ? form.comentario
      : "Archivo subido sin nombre."
  )
  formData.append('Tipo_audiencia', form.Tipo_audiencia)
  formData.append('ltg_Nombre_Representante', form.ltg_Nombre_Representante)
  formData.append("ltg_Fecha_Audiencia", formatearFechaISO(form.ltg_Fecha_Audiencia))
  formData.append('ltg_Fecha_Actualizacion', formatearFechaISO(new Date()))
  if (form.id_Tribunal !== null && form.id_Tribunal !== '') {
    formData.append('id_Tribunal', parseInt(form.id_Tribunal));
  }
  formData.append('ltg_Nacionalidad', form.ltg_Nacionalidad)
  formData.append('ltg_Nacionalidad_Representante', form.ltg_Nacionalidad_Representante)
  formData.append('id_Sentencia', parseInt(form.id_sentencia))
  formData.append('id_usuario', parseInt(form.id_usuario))
  formData.append('id_Estatus', 1)
  formData.append('NombreCarpeta', form.ltg_acto)

  if (expedienteFile.value) {
    formData.append('Archivo', expedienteFile.value)
  }

  try {

    const notif = push.promise('Subiendo archivo...');

    // Esperar 2 segundos aunque se suba rápido
    await new Promise(resolve => setTimeout(resolve, 1000));

    const response = await fetch('/api/Litigio/Subir_Litigio_Con_Archivo', {
      method: 'POST',
      body: formData
    })

    const result = await response.json()

    if (!response.ok) {
      console.error('Error del backend:', result);
      notif.reject(result.mensaje || 'Error al guardar el litigio');
    } else {
      notif.resolve('El litigio ha sido cargado de forma exitosa');
      setTimeout(() => {
        router.push('/home');
      }, 1000);
    }

  } catch (error) {
    console.error('Error inesperado al registrar el litigio:', error)
    push.error('Error inesperado al registrar el litigio')
  } finally {
    enviando.value = false
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

.custom-home-btn {
  background-color: #003870;
  border-color: #003870;
  color: white;
  transition: background-color 0.3s ease, border-color 0.3s ease;
}

.custom-home-btn:hover {
  background-color: #004a99;
  border-color: #002f66;
  color: white;
}
</style>
