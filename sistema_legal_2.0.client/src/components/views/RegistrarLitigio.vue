<template>
  <div class="card p-4 shadow-2">
    <div class="d-flex justify-content-between align-items-center flex-wrap mb-3">
      <h1> </h1>
      <!-- Título -->
      <h2 class="h4 fw-bold mb-2 mb-md-0">Registro de Litigio</h2>

      <!-- Botones alineados a la derecha -->
      <div class="d-flex gap-2">
        <!-- Botón Registrar -->
        <Button type="submit" label="Registrar" icon="pi pi-check" class="p-button-sm" :disabled="enviando"
          @click="formRef?.requestSubmit()" style="background-color: #003870; border-color: #003870;" />

        <!-- Botón Inicio -->
        <router-link :to="rutaInicio" class="btn btn-sm text-white d-flex align-items-center custom-home-btn">
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

            <!-- Número de Acto -->
            <div class="field col-12 md:col-4">
              <label for="acto" class="block mb-2 font-medium text-sm">No. Acto Alguacil *</label>
              <InputText id="acto" v-model="form.ltg_acto" class="w-full" placeholder="Ingrese el número de acto"
                maxlength="10" />
            </div>

            <!-- Fecha del Acto -->
            <div class="field col-12 md:col-4">
              <label for="fechaActo" class="block mb-2 font-medium text-sm">Fecha del Acto *</label>
              <Calendar id="fechaActo" v-model="form.ltg_Fecha_Acto" dateFormat="yy-mm-dd" showIcon class="w-full"
                placeholder="Seleccione la fecha" />
            </div>

            <!-- Tipo de Demanda -->
            <div class="field col-12 md:col-4">
              <label for="tipoDemanda" class="block mb-2 font-medium text-sm">Tipo de Demanda *</label>
              <Dropdown id="tipoDemanda" v-model="form.id_Tipo_Demanda" :options="tiposDemanda" optionLabel="nombre"
                optionValue="id_demanda" placeholder="Seleccione una opción" class="w-full" filter />
            </div>

          </div>
        </fieldset>


        <!-- Información de la Audiencia -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Información de la Audiencia</legend>
          <div class="grid">

            <!-- Fecha de Audiencia -->
            <div class="field col-12 md:col-3">
              <label for="fechaAudiencia" class="block mb-2 font-medium text-sm">Fecha de Audiencia *</label>
              <Calendar id="fechaAudiencia" v-model="form.ltg_Fecha_Audiencia" dateFormat="yy-mm-dd" showIcon
                :minDate="hoy" class="w-full" placeholder="Seleccione una fecha" />
            </div>
            <div class="field col-12 md:col-2">
              <label for="fechaAudiencia" class="block mb-2 font-medium text-sm">Hora Audiencia *</label>
              <Calendar v-model="horaSeleccionada" showIcon timeOnly hourFormat="12" placeholder="Ej: 8:00am" />
            </div>

            <!-- Tribunal -->
            <div class="field col-12 md:col-4">
              <label for="tribunal" class="block mb-2 font-medium text-sm">Tribunal *</label>
              <Dropdown id="tribunal" v-model="form.id_Tribunal" :options="tribunales" optionLabel="nombre_Tribunal"
                optionValue="id_Tribunal" placeholder="Seleccione un tribunal" class="w-full" filter />
            </div>

            <!-- Tipo de Audiencia -->
            <div class="field col-12 md:col-3">
              <label for="tipoAudiencia" class="block mb-2 font-medium text-sm">Tipo de Audiencia *</label>
              <Dropdown id="tipoAudiencia" v-model="form.Tipo_audiencia" :options="tiposAudiencia" optionLabel="label"
                optionValue="value" class="w-full" placeholder="Seleccione un tipo" />
            </div>

          </div>
        </fieldset>


        <!-- DATOS DEL DEMANDANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Demandante</legend>
          <div class="grid">

            <!-- Tipo de Demandante -->
            <div class="field col-12 md:col-2">
              <label for="tipoDemandante" class="block mb-2 font-medium text-sm">Tipo de Demandante *</label>
              <Dropdown id="tipoDemandante" v-model="form.ltg_Tipo_Demandante" :options="tiposDemandante"
                optionLabel="label" optionValue="value" class="w-full" placeholder="Seleccione una opción" />
            </div>

            <!-- Otro tipo de demandante -->
            <div class="field col-12 md:col-3" v-if="form.ltg_Tipo_Demandante === 'Otros'">
              <label for="otrosDemandante" class="block mb-2 font-medium text-sm">Especifique tipo de Demandante
                *</label>
              <InputText id="otrosDemandante" v-model="form.otrosDemandante" class="w-full"
                placeholder="Otro tipo de demandante" />
            </div>

            <!-- Cédula o RNC -->
            <div class="field col-12 md:col-3">
              <label for="cedulaDemandante" class="block mb-2 font-medium text-sm">
                {{ form.ltg_Tipo_Demandante === 'Empresa' ? 'RNC de la Empresa *' : 'Cédula del Demandante*' }}
              </label>
              <InputText id="cedulaDemandante" v-model="form.ltg_Cedula_Demandante"
                :maxlength="form.ltg_Tipo_Demandante === 'Empresa' ? 9 : 11"
                @input="form.ltg_Cedula_Demandante = form.ltg_Cedula_Demandante.replace(/\D/g, '')"
                @blur="() => buscarPersonaPorDocumento(form.ltg_Cedula_Demandante, 'ltg_Nombre_Demandante', 'ltg_Nacionalidad')"
                class="w-full"
                :placeholder="form.ltg_Tipo_Demandante === 'Empresa' ? 'Ej: 123456789' : 'Ej: 00112345678'"
                :disabled="!cedulaHabilitado" />
            </div>
            <!-- Nombre del Demandante -->
            <div class="field col-12 md:col-3">
              <label for="nombreDemandante" class="block mb-2 font-medium text-sm">
                {{ form.ltg_Tipo_Demandante === 'Empresa' ? 'Nombre de la empresa *' : 'Nombre del Demandante *' }}
              </label>
              <InputText id="nombreDemandante" v-model="form.ltg_Nombre_Demandante" class="w-full"
                :placeholder="form.ltg_Tipo_Demandante === 'Empresa' ? 'Nombre de la empresa' : 'Nombre completo'" :disabled="!nombreDemandante" />
            </div>

            <!-- Nacionalidad o país -->
            <div class="field col-12 md:col-3">
              <label for="nacionalidadDemandante" class="block mb-2 font-medium text-sm">
                {{ form.ltg_Tipo_Demandante === 'Empresa' ? 'País de Constitución *' : 'Nacionalidad *' }}
              </label>
              <InputText id="nacionalidadDemandante" v-model="form.ltg_Nacionalidad" class="w-full"
                placeholder="Ej: Dominicana" :disabled="!nacionalidadHabilitado" />
            </div>

          </div>
        </fieldset>


        <!-- DATOS DEL REPRESENTANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Representante</legend>
          <div class="grid">

            <div class="field col-12 md:col-2">
              <label for="cedulaRepresentante" class="block mb-2 font-medium text-sm">Cédula *</label>
              <InputText id="cedulaRepresentante" :maxlength="11" v-model="form.ltg_Cedula_Representante" @keydown="soloNumeros" @blur="() => buscarPersonaPorDocumento(
                form.ltg_Cedula_Representante,
                'ltg_Nombre_Representante',
                'ltg_Nacionalidad_Representante'
              )" class="w-full" placeholder="Ej: 00112345678" />
            </div>

            <div class="field col-12 md:col-4">
              <label for="nombreRepresentante" class="block mb-2 font-medium text-sm">Nombre del Representante *</label>
              <InputText id="nombreRepresentante" v-model="form.ltg_Nombre_Representante" class="w-full"
                placeholder="Nombre completo" :disabled="!nombreHabilitado" />
            </div>

            <div class="field col-12 md:col-5">
              <label for="nacionalidadRepresentante" class="block mb-2 font-medium text-sm">Nacionalidad del
                Representante *</label>
              <InputText id="nacionalidadRepresentante" v-model="form.ltg_Nacionalidad_Representante" class="w-full"
                placeholder="Ej: Dominicana"  :disabled="!nacionalidadHabilitado" />
            </div>

          </div>
        </fieldset>

        <!-- DATOS DOCUMENTACION -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Documentación</legend>
          <div class="grid">

            <!-- Nombre de la Evidencia y Carga de Archivo -->
            <div class="field col-12 md:col-4">
              <label for="nombreEvidencia" class="block mb-2 font-medium text-sm">Nombre de la Evidencia</label>
              <InputText id="nombreEvidencia" v-model="form.NombreEvidencia" placeholder="Ej: Acta de audiencia"
                class="w-full" />

              <label for="archivo" class="block mt-4 mb-2 font-medium text-sm">Archivo</label>
              <FileUpload id="archivo" name="Archivo" customUpload @select="handleExpedienteUpload" mode="basic"
                chooseLabel="Elegir archivo" class="w-full md:w-9rem text-sm" />
            </div>

            <!-- Comentario -->
            <div class="field col-12 md:col-8">
              <label for="comentario" class="block mb-2 font-medium text-sm">Comentario</label>
              <Textarea id="comentario" v-model="form.comentario"
                placeholder="Añadir comentario relacionado con el documento" class="w-full" rows="5" autoResize />
            </div>

          </div>
        </fieldset>

      </div>

    </form>
  </div>

         <!--Pantalla Litigio Registrado -->
  <Dialog v-model:visible="dialogVisible" modal class="dialog-exito-style" :closable="false" :draggable="false"
    header="Registro exitoso">
    <div class="d-flex align-items-start gap-3 p-3">
      <i class="pi pi-check-circle text-success" style="font-size: 1.8rem; flex-shrink: 0;"></i>
      <p class="m-0">El litigio fue registrado correctamente.</p>
    </div>
    <div class="text-end px-3 pb-3">
      <Button label="Aceptar" class="p-button-sm" :style="{ backgroundColor: '#003870', color: '#fff', border: 'none' }"
        @click="handleAceptarDialog" />
    </div>
  </Dialog>

</template>

<script setup>
import { reactive, ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { push } from 'notivue'
import InputText from 'primevue/inputtext'
import Calendar from 'primevue/calendar'
import Dropdown from 'primevue/dropdown'
import FileUpload from 'primevue/fileupload'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog';

const dialogVisible = ref(false);
const router = useRouter()
const hoy = ref(new Date()) // Esto representa la fecha de hoy
const formRef = ref(null);
const tiposDemanda = ref([])
const estatusLitigios = ref([])
const tribunales = ref([])
const expedienteFile = ref(null)
const enviando = ref(false)
const rutaInicio = ref('/Administrador/litigios');
const cedulaHabilitado = ref(false)
const nombreHabilitado = ref(false)
const nacionalidadHabilitado = ref(false)
const horaSeleccionada = ref(null)
const NombreEvidencia = ref(null)


onMounted(async () => {
  try {
    await cargarDatosDropdowns();
  } catch (error) {
    console.error('Error en onMounted al cargar datos de dropdowns:', error);
    push.error('No se pudo cargar la información necesaria. Verifique su conexión.');
  }
});


const form = reactive({
  ltg_acto: '',
  ltg_Fecha_Acto: '',
  id_Tipo_Demanda: null,
  ltg_Cedula_Demandante: '',
  ltg_Nombre_Demandante: '',
  ltg_Tipo_Demandante: '',
  otrosDemandante: '',
  ltg_Nacionalidad: '',
  ltg_Cedula_Representante: '',
  ltg_Nombre_Representante: '',
  ltg_Nacionalidad_Representante: '',
  ltg_Fecha_Audiencia: '',
  id_Tribunal: '',
  Tipo_audiencia: '',
  comentario: '',
  NombreEvidencia: '',
  id_sentencia: 3,
  id_Estatus: 1,
  id_usuario: null
});


const handleAceptarDialog = () => {
  dialogVisible.value = false;
  push.success('El litigio ha sido cargado de forma exitosa');

  const usuario = JSON.parse(localStorage.getItem('usuario'));
  const perfil = parseInt(usuario?.perfil);

  const rutasPorPerfil = {
    1: '/Administrador/litigios',
    2: '/Administrador/litigios',
    3: '/registrar',
    4: '/abogado/inicio'
  };

  const ruta = rutasPorPerfil[perfil] || '/Administrador/litigios';

  if (perfil === 3) {
    // Si es digitador, limpiar el formulario
    Object.keys(form).forEach(k => form[k] = '');
    expedienteFile.value = null;
  }

  router.push(ruta);
};

function soloNumeros(event) {
  const tecla = event.key;
  // Permite solo números y teclas útiles (borrar, tab, flechas, etc.)
  if (!/^\d$/.test(tecla) &&
    tecla !== 'Backspace' &&
    tecla !== 'Tab' &&
    tecla !== 'ArrowLeft' &&
    tecla !== 'ArrowRight' &&
    tecla !== 'Delete') {
    event.preventDefault();
  }
}

watch(() => form.ltg_Tipo_Demandante, (nuevoValor) => {
  if (nuevoValor) {
    cedulaHabilitado.value = true
    nombreHabilitado.value = false
    nacionalidadHabilitado.value = false

    // Limpia campos si se cambia el tipo
    form.ltg_Cedula_Demandante = ''
    form.ltg_Nombre_Demandante = ''
    form.ltg_Nacionalidad = ''
  } else {
    cedulaHabilitado.value = false
  }
})



function validarCedulaODocumento(doc, tipo) {
  if (!doc) return false;
  return tipo === 'Empresa' ? /^\d{9}$/.test(doc) : /^\d{11}$/.test(doc);
}

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
  if (!documento || documento.trim().length < 9) {
    form[campoNombre] = ''
    form[campoNacionalidad] = ''
    nombreHabilitado.value = false
    nacionalidadHabilitado.value = false
    return
  }

  try {
    const res = await fetch(`/api/Litigio/BuscarDocumento/${documento.trim()}`)
    const data = await res.json()

    form[campoNombre] = data.nombre
    form[campoNacionalidad] = data.nacionalidad

    // Habilitar los campos si se recibió información válida
    // nombreHabilitado.value = true
    // nacionalidadHabilitado.value = true
  } catch (error) {
    console.warn(`No se encontró el documento ${documento}`, error)

    form[campoNombre] = ''
    form[campoNacionalidad] = ''

    // Deshabilitar los campos si falló la búsqueda
    nombreHabilitado.value = false
    nacionalidadHabilitado.value = false
  }
}


const formatearFechaISO = (fecha) => {
  if (!fecha) return ''
  const d = new Date(fecha)
  return d.toISOString().split('T')[0]
}
function validarFormulario() {
  if (!form.ltg_acto?.trim()) return "El número de acto es obligatorio.";
  if (!form.ltg_Fecha_Acto) return "La fecha del acto es obligatoria.";
  if (!form.id_Tipo_Demanda) return "Seleccione un tipo de demanda.";
  if (!form.ltg_Tipo_Demandante) return "Seleccione el tipo de demandante.";

  if (form.ltg_Tipo_Demandante === "Otros" && !form.otrosDemandante?.trim()) {
    return "Debe especificar el tipo de demandante.";
  }

  if (!validarCedulaODocumento(form.ltg_Cedula_Demandante, form.ltg_Tipo_Demandante)) {
    return "La cédula/RNC del demandante es inválida.";
  }

  if (!form.ltg_Nombre_Demandante?.trim()) {
    return "El nombre del demandante es obligatorio.";
  }

  if (!/^\d{11}$/.test(form.ltg_Cedula_Representante)) {
    return "La cédula del representante es inválida.";
  }

  if (!form.ltg_Nombre_Representante?.trim()) {
    return "El nombre del representante es obligatorio.";
  }

  if (!expedienteFile.value) return "Debe subir un archivo.";

  return null; // todo válido
}

const combinarFechaYHora = (fecha, hora) => {
  if (!(fecha instanceof Date) || isNaN(fecha.getTime())) return null;
  if (!(hora instanceof Date) || isNaN(hora.getTime())) return null;

  const fechaObj = new Date(fecha);
  fechaObj.setHours(hora.getHours());
  fechaObj.setMinutes(hora.getMinutes());
  fechaObj.setSeconds(0);
  fechaObj.setMilliseconds(0);

  return fechaObj;
};


const registrarLitigio = async () => {
  if (enviando.value) return;
  enviando.value = true;

  const error = validarFormulario();

  if (error) {
    push.warning(error);
    enviando.value = false;
    return;
  }



  const usuarioLogueado = JSON.parse(localStorage.getItem('usuario'));
  form.id_usuario = usuarioLogueado.idUsuario;

 const formData = new FormData();



formData.append('ltg_acto', form.ltg_acto);
formData.append('ltg_Fecha_Acto', formatearFechaISO(form.ltg_Fecha_Acto));
formData.append('ltg_Cedula_Demandante', form.ltg_Cedula_Demandante);
formData.append('ltg_Nombre_Demandante', form.ltg_Nombre_Demandante);
formData.append('ltg_Tipo_Demandante', form.ltg_Tipo_Demandante);
formData.append('ltg_Nacionalidad', form.ltg_Nacionalidad);
formData.append('ltg_Cedula_Representante', form.ltg_Cedula_Representante);
formData.append('ltg_Nombre_Representante', form.ltg_Nombre_Representante);
formData.append('ltg_Nacionalidad_Representante', form.ltg_Nacionalidad_Representante);
formData.append('id_Tipo_Demanda', parseInt(form.id_Tipo_Demanda));
formData.append('id_Sentencia', parseInt(form.id_sentencia));
formData.append('id_usuario', parseInt(form.id_usuario));
formData.append('id_Estatus', 1);
formData.append('Tipo_audiencia', form.Tipo_audiencia || 'Documentos del acto');
formData.append('NombreEvidencia', form.NombreEvidencia || 'Evidencia');
formData.append('comentario', form.comentario || 'Sin comentario');
formData.append('Nombre_Archivo', expedienteFile.value?.name || '');
formData.append('Ruta_Archivo', expedienteFile.value?.name || ''); // si tu backend lo genera, puedes omitirlo

const fechaCompletaAudiencia = combinarFechaYHora(form.ltg_Fecha_Audiencia, horaSeleccionada.value);

// Validar primero
if (!(fechaCompletaAudiencia instanceof Date) || isNaN(fechaCompletaAudiencia.getTime())) {
  push.warning("Debe seleccionar una hora válida para la audiencia.");
  enviando.value = false;
  return;
}

// Solo después de validar, usarla
formData.append('Fecha', fechaCompletaAudiencia.toISOString());
formData.append('Id_tribunal', parseInt(form.id_Tribunal));
formData.append('Tipo', form.Tipo_audiencia);


formData.append('Archivo', expedienteFile.value); // obligatorio si usas archivos


console.log(' Enviando FormData:');
for (let [key, value] of formData.entries()) {
  console.log(`${key}:`, value);
}

// Opcional: construir texto para mostrar en alert en caso de error
let contenidoForm = '';
for (let [key, value] of formData.entries()) {
  contenidoForm += `${key}: ${value instanceof File ? value.name : value}\n`;
}



  try {

    for (let [key, value] of formData.entries()) {
  console.log(`${key}:`, value);
}



    const notif = push.promise('Subiendo archivo...');

    await new Promise(resolve => setTimeout(resolve, 1000)); // opcional



    const response = await fetch('/api/Litigio/Subir_Litigio_Con_Archivo', {
      method: 'POST',
      body: formData
    });

    const result = await response.json();

    if (!response.ok) {
      console.error('Error del backend:', result);
      notif.reject(result.mensaje || 'Error al guardar el litigio');
    } else {
      // ✅ Mostrar el diálogo para todos los perfiles
      notif.resolve('El archivo se subió correctamente');
      dialogVisible.value = true;
    }
  } catch (error) {
    console.error('Error inesperado al registrar el litigio:', error);
    push.error('Error inesperado al registrar el litigio');
  } finally {
    enviando.value = false;
  }

};

</script>

<style scoped>
legend {
  float: none !important;
  display: inline-block !important;
  padding: 0 5px !important;
  margin-bottom: 0.5rem !important;
  line-height: inherit !important;
  font-size: calc(0.9rem + 0.3vw) !important;
  width: auto !important;
  color: #003870;
}

fieldset {
  background-color: #f9f9f9;
  border-radius: 0.5rem;
  padding: 1.5rem;
  margin-bottom: 2rem;
  border: 1px solid #ccc;
}

input:focus,
textarea:focus,
.p-inputtext:focus,
.p-dropdown:focus {
  border-color: #003870;
  box-shadow: 0 0 0 2px rgba(0, 56, 112, 0.2);
  outline: none;
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
  background-color: #c00606;
  border-color: #c00606;
  box-shadow: 0 6px 16px rgba(121, 1, 51, 0.3) !important;
  transform: translateY(-1px);
  transition: background-color 0.2s;
}

.p-button-sm {
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
}

::v-deep(.p-button:hover) {
  background-color: #c00606 !important;
  border-color: #c00606 !important;
  box-shadow: 0 6px 16px rgba(121, 1, 51, 0.3) !important;
  transform: translateY(-1px);
  transition: background-color 0.2s;
}

.p-button-sm:hover {
  background-color: #c00606 !important;
  border-color: #c00606 !important;
  box-shadow: 0 6px 16px rgba(121, 1, 51, 0.3) !important;
  transform: translateY(-1px);
  transition: background-color 0.2s;
}

fieldset label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 600;
  font-size: 0.9rem;
  color: #003870;
}

.text-muted {
  color: #6c757d;
  font-size: 0.8rem;
}

::v-deep(.p-button-icon-only) {
  width: 0.8rem !important;
}

@media (max-width: 768px) {
  .field {
    margin-bottom: 1rem;
  }
}
</style>
