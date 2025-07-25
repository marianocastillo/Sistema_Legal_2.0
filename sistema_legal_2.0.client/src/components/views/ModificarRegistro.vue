<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
      <h2 class="text-2xl font-semibold" style="color: #003870;">Modificar Litigio</h2>
      <router-link :to="rutaInicio" class="btn btn-sm text-white d-flex align-items-center custom-home-btn">
        <i class="pi pi-home me-2"></i>
        Inicio
      </router-link>
    </div>

    <!-- Formulario de registro  -->
    <form @submit.prevent="registrarLitigio">
      <div class="grid formgrid p-fluid">

        <!-- INFORMACIÓN DEL LITIGIO -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Información del Litigio</legend>
          <div class="grid">

            <div class="field col-12 md:col-4">
              <label class="block mb-2">No. Acto Alguacil *</label>
              <InputText v-model="form.noActo" class="w-full" placeholder="No. Acto Alguacil" />
            </div>

            <div class="field col-12 md:col-4">
              <label class="block mb-2">Fecha del Acto</label>
              <Calendar v-model="form.fechaActo" dateFormat="yy-mm-dd" showIcon placeholder="Fecha del acto"
                class="w-full" />
            </div>

            <div class="field col-12 md:col-4">
              <label class="block mb-2">Tipo de Demanda</label>
              <Dropdown v-model="form.tipoDemanda" :options="tiposDemanda" optionLabel="nombre" optionValue="id_demanda"
                placeholder="Tipo de Demanda" class="w-full" />
            </div>

            <!-- <div class="field col-12 md:col-4">
              <label class="block mb-2">Fecha de Audiencia</label>
              <Calendar v-model="form.fechaAudiencia" dateFormat="yy-mm-dd" showIcon placeholder="Fecha de audiencia"
                class="w-full" :minDate="hoy" />
            </div>

            <div class="field col-12 md:col-4">
              <label class="block mb-2">Tribunal</label>
              <Dropdown v-model="form.tribunal" :options="tribunales" optionLabel="nombre_Tribunal"
                optionValue="id_Tribunal" placeholder="--Seleccione Tribunal--" class="w-full" />
            </div> -->


            <!-- modificado por el momento -->
            <!-- <div class="field col-12 md:col-3">
              <label class="block mb-2">Estatus</label>
              <Dropdown v-model="form.estatus" :options="estatusList" optionLabel="ltg_description"
                optionValue="ltg_estatus" placeholder="--Seleccione Estatus--" class="w-full" />
            </div> -->

          </div>
        </fieldset>
        <!-- DATOS DEL DEMANDANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Demandante</legend>
          <div class="grid">
            <div class="field col-12 md:col-3">
              <label class="block mb-2">Tipo de Demandante</label>
              <Dropdown v-model="form.tiposDemandante" :options="tiposDemandante" optionLabel="label"
                optionValue="value" class="w-full" placeholder="Tipo de Demandante" @change="onTipoDemandanteChange" />


            </div>

            <div class="field col-12 md:col-3">





              <label for="cedulaDemandante" class="block mb-2" v-html="form.tiposDemandante === 'Persona Jurídica' ? 'RNC de la Empresa <span class=\'text-red-500\'>*</span>'
                : 'Cédula del Demandante <span class=\'text-red-500\'>*</span>'">
              </label>
              <InputText id="cedulaDemandante" v-model="form.cedulaDemandante" :class="{ 'p-invalid': cedulaInvalida }"
                :maxlength="form.tiposDemandante === 'Persona Jurídica' ? 9 : 11" @input="handleCedulaInput"
                @blur="() => buscarPersonaPorDocumento(form.cedulaDemandante, 'ltg_Nombre_Demandante', 'Nacionalidad')"
                class="w-full"
                :placeholder="form.tiposDemandante === 'Persona Jurídica' ? 'Ej: 123456789' : 'Ej: 00112345678'" />
              <small v-if="cedulaInvalida && !documentoNoEncontrado" class="p-error">
                {{ form.tiposDemandante === 'Persona Jurídica' ? 'Debe tener 9 dígitos numéricos (RNC).' : `Debe tener
                11 dígitos numéricos(cédula).` }}
              </small>
              <small v-else-if="documentoNoEncontrado" class="p-error">
                {{ form.tiposDemandante === 'Persona Jurídica' ? 'RNC no encontrado.' : 'Cédula no encontrada.' }}
              </small>




              <!-- <label class="block mb-2">Cédula/RNC</label>
              <InputText v-model="form.cedulaDemandante" class="w-full" :class="{ 'p-invalid': cedulaInvalida }"
              :maxlength="form.tiposDemandante === 'Persona Jurídica' ? 9 : 11" @input="handleCedulaInput"
               :placeholder="form.tiposDemandante === 'Persona Jurídica' ? 'RNC de la empresa' : 'Cédula del demandante'" />
              -->

            </div>

            <div class="field col-12 md:col-3">
              <label class="block mb-2">Nombre</label>
              <InputText id="nombreDemandante" v-model="form.ltg_Nombre_Demandante" class="w-full"
                :disabled="!nombreDemandante"
                :placeholder="form.tiposDemandante === 'Persona Jurídica' ? 'Nombre de la empresa' : 'Nombre del demandante'" />
            </div>

            <div class="field col-12 md:col-3">
              <label class="block mb-2">Nacionalidad</label>
              <InputText id="nacionalidadDemandante" v-model="form.Nacionalidad" class="w-full"
                :disabled="!nacionalidadDemandante"
                :placeholder="form.tiposDemandante === 'Persona Jurídica' ? 'País de constitución' : 'Nacionalidad'" />
            </div>

            <div class="field col-12 md:col-4" v-if="form.tiposDemandante === 'Otros'">
              <label class="block mb-2">Especifique tipo de demandante</label>
              <InputText v-model="form.otrosDemandante" class="w-full" placeholder="Especifique tipo de demandante" />
            </div>
          </div>
        </fieldset>

        <!-- DATOS DEL REPRESENTANTE -->
        <fieldset class="col-12 border-1 border-round p-3 mb-3">
          <legend class="font-bold text-lg">Datos del Representante</legend>
          <div class="grid">

            <div class="field col-12 md:col-4">
              <label class="block mb-2">Cédula</label>
              <InputText v-model="form.cedulaRepresentante" class="w-full"
                :class="{ 'p-invalid': documentoRepNoEncontrado }" placeholder="Cédula del representante" maxlength="11"
                @input="handleCedulaRepresentanteInput" @blur="buscarRepresentantePorCedula" />
              <small v-if="documentoRepNoEncontrado" class="p-error">
                Cédula no encontrada o inválida.
              </small>


            </div>

            <div class="field col-12 md:col-4">
              <label class="block mb-2">Nombre</label>
              <InputText v-model="form.nombreRepresentante" class="w-full" placeholder="Nombre del representante"
                :disabled="true" />
            </div>

            <div class="field col-12 md:col-4">
              <label class="block mb-2">Nacionalidad</label>
              <InputText v-model="form.ltg_Nacionalidad_Representante" class="w-full"
                placeholder="Nacionalidad del representante" :disabled="true" />
            </div>

          </div>
        </fieldset>

        <!-- ARCHIVOS -->

      </div>

      <!-- BOTÓN -->
      <div class="text-center mt-4">
        <Button type="submit" label="Actualizar Litigio" icon="pi pi-check" class="p-button custom-blue" />
      </div>
    </form>
  </div>
</template>

<script setup>
import { push } from 'notivue'
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import InputText from 'primevue/inputtext'
import Calendar from 'primevue/calendar'
import Dropdown from 'primevue/dropdown'
import Button from 'primevue/button'


const router = useRouter()
const hoy = ref(new Date())
const rutaInicio = ref('');
const cedulaInvalida = ref(false)
const tiposDemanda = ref([])
const tribunales = ref([])
const estatusList = ref([])
const nombreHabilitado = ref(true)
const nacionalidadHabilitado = ref(true)
const documentoNoEncontrado = ref(false)
const documentoRepNoEncontrado = ref(false)

const form = ref({
  id_Ltg: null,
  noActo: '',
  fechaActo: '',
  tipoDemanda: null,
  cedulaDemandante: '',
  Nacionalidad: '',
  ltg_Nombre_Demandante: '',
  tiposDemandante: null,
  otrosDemandante: '',
  cedulaRepresentante: '',
  nombreRepresentante: '',
  ltg_Nacionalidad_Representante: '',
  fechaAudiencia: '',
  tribunal: null,
  estatus: null,
  id_Sentencia: null,
})

const usuarioLogueado = JSON.parse(localStorage.getItem('usuario'));
form.id_usuario = usuarioLogueado.idUsuario;

const onTipoDemandanteChange = () => {
  documentoNoEncontrado.value = false;
  cedulaInvalida.value = false;

  // Limpiar campos cuando se cambia el tipo de demandante
  form.value.cedulaDemandante = '';
  form.value.ltg_Nombre_Demandante = '';
  form.value.Nacionalidad = '';

  // También desactiva los campos nombre/nacionalidad si están controlados por lógica
  nombreHabilitado.value = false;
  nacionalidadHabilitado.value = false;
};


const tiposDemandante = [
  { label: 'Persona Física', value: 'Persona Física' },
  { label: 'Persona Jurídica', value: 'Persona Jurídica' },
  { label: 'Ex Empleado', value: 'Ex Empleado' }

]

const cargarDatosDropdowns = async () => {
  try {
    const response = await fetch('/api/Litigio/datos-litigio')
    const data = await response.json()
    tiposDemanda.value = data.tiposDemanda
    estatusList.value = data.estatusLitigios
    tribunales.value = [
      { id_Tribunal: null, nombre_Tribunal: '--Quitar Tribunal--' },
      ...data.tribunales
    ]
  } catch (error) {
    console.error('Error al cargar los datos de los dropdowns:', error)
  }
}

const buscarRepresentantePorCedula = async () => {
  const cedula = form.value.cedulaRepresentante?.trim() || '';
  documentoRepNoEncontrado.value = false;

  // Validar si la cédula está vacía
  if (!cedula) {
    documentoRepNoEncontrado.value = true;
    form.value.nombreRepresentante = '';
    form.value.ltg_Nacionalidad_Representante = '';
    return;
  }

  // Validar que tenga exactamente 11 dígitos numéricos
  if (!/^\d{11}$/.test(cedula)) {
    documentoRepNoEncontrado.value = true;
    form.value.nombreRepresentante = '';
    form.value.ltg_Nacionalidad_Representante = '';
    return;
  }

  try {
    const res = await fetch(`/api/Litigio/BuscarDocumento/${cedula}`);

    if (!res.ok) throw new Error(`HTTP error: ${res.status}`);

    const data = await res.json();

    if (!data?.nombre || !data?.nacionalidad) {
      documentoRepNoEncontrado.value = true;
      form.value.nombreRepresentante = '';
      form.value.ltg_Nacionalidad_Representante = '';
      return;
    }

    form.value.nombreRepresentante = data.nombre;
    form.value.ltg_Nacionalidad_Representante = data.nacionalidad;

  } catch (err) {
    console.warn(`❌ Error al buscar representante con cédula ${cedula}:`, err);
    documentoRepNoEncontrado.value = true;
    form.value.nombreRepresentante = '';
    form.value.ltg_Nacionalidad_Representante = '';
  }
};


onMounted(async () => {
  await cargarDatosDropdowns()

  console.log('form:', form.value)
  console.log('tiposDemanda:', tiposDemanda.value)
  console.log('tribunales:', tribunales.value)
  console.log('estatusList:', estatusList.value)

  const almacenado = localStorage.getItem('litigioModificacion')
  console.log("Contenido en localStorage:", almacenado)

  if (almacenado) {
    const data = JSON.parse(almacenado)
    console.log('Litigio cargado:', data)

    form.value = {
      id_Ltg: data.id_Ltg,
      noActo: data.ltg_acto,
      fechaActo: data.ltg_Fecha_Acto ? new Date(data.ltg_Fecha_Acto) : null,
      tipoDemanda: data.id_Tipo_Demanda ?? data.tipoDemanda_Id ?? null,
      cedulaDemandante: data.ltg_Cedula_Demandante,
      Nacionalidad: data.ltg_Nacionalidad,
      ltg_Nombre_Demandante: data.ltg_Nombre_Demandante,
      tiposDemandante: data.ltg_Tipo_Demandante,
      otrosDemandante: data.ltg_Tipo_Demandante === 'Otros' ? data.otrosDemandante || '' : '',
      cedulaRepresentante: data.ltg_Cedula_Representante,
      nombreRepresentante: data.ltg_Nombre_Representante,
      ltg_Nacionalidad_Representante: data.ltg_Nacionalidad_Representante,
      fechaAudiencia: data.ltg_Fecha_Audiencia ? new Date(data.ltg_Fecha_Audiencia) : null,
      tribunal: data.id_Tribunal,
      estatus: data.id_Estatus ?? data.ltg_estatus ?? null,
      id_Sentencia: data.id_Sentencia ?? null,
      id_usuario: data.id_usuario ?? null
    }

    const usuarioActual = localStorage.getItem('usuario');
    if (usuarioActual) {
      const usuarioParseado = JSON.parse(usuarioActual);
      form.value.id_usuario = usuarioParseado?.idUsuario || null;
    } else {
      console.warn('No se encontró usuario en localStorage');
    }
  }

  // ✅ Agrega esta parte para establecer la ruta dinámica
  const rawUser = localStorage.getItem('usuario');
  const user = rawUser ? JSON.parse(rawUser) : null;

  if (user) {
    const perfil = parseInt(user.perfil);
    const rutasPorPerfil = {
      1: '/Seguimiento',
      2: '/GestionLitigios',
      3: '/LitigiosRegistrados',
      4: '/abogado/inicio'
    };

    rutaInicio.value = rutasPorPerfil[perfil] || '/Administrador/litigios';
  }
});


//Validación dinámica para cédula o RNC según el tipo
const validarIdentificacion = (valor, tipo) => {
  if (!valor) return false
  const regex = tipo === 'Persona Jurídica' ? /^\d{9}$/ : /^\d{11}$/
  return regex.test(valor)
}

const buscarPersonaPorDocumento = async (documento, campoNombre, campoNacionalidad) => {
  const doc = documento?.trim() || '';
  documentoNoEncontrado.value = false;
  const tipo = form.value.tiposDemandante;

  // Validar longitud mínima
  if (!doc || doc.length < 9) {
    form.value[campoNombre] = '';
    form.value[campoNacionalidad] = '';
    nombreHabilitado.value = false;
    nacionalidadHabilitado.value = false;
    documentoNoEncontrado.value = true;
    return;
  }

  // Validar si el tipo de documento no coincide con el tipo de demandante
  const isCedula = doc.length === 11;
  const isRNC = doc.length === 9;

  if ((tipo === 'Persona Física' || tipo === 'Ex Empleado') && !isCedula) {
    documentoNoEncontrado.value = true;
    form.value[campoNombre] = '';
    form.value[campoNacionalidad] = '';
    nombreHabilitado.value = false;
    nacionalidadHabilitado.value = false;
    return;
  }

  if (tipo === 'Persona Jurídica' && !isRNC) {
    documentoNoEncontrado.value = true;
    form.value[campoNombre] = '';
    form.value[campoNacionalidad] = '';
    nombreHabilitado.value = false;
    nacionalidadHabilitado.value = false;
    return;
  }

  try {
    const res = await fetch(`/api/Litigio/BuscarDocumento/${doc}`);
    if (!res.ok) throw new Error(`Respuesta HTTP no válida: ${res.status}`);

    const data = await res.json();

    if (!data?.nombre || !data?.nacionalidad) {
      documentoNoEncontrado.value = true;
      form.value[campoNombre] = '';
      form.value[campoNacionalidad] = '';
      nombreHabilitado.value = false;
      nacionalidadHabilitado.value = false;
      return;
    }

    form.value[campoNombre] = data.nombre;
    form.value[campoNacionalidad] = data.nacionalidad;
    nombreHabilitado.value = true;
    nacionalidadHabilitado.value = true;

  } catch (error) {
    console.warn(`❌ Error al buscar el documento ${doc}:`, error);
    form.value[campoNombre] = '';
    form.value[campoNacionalidad] = '';
    nombreHabilitado.value = false;
    nacionalidadHabilitado.value = false;
    documentoNoEncontrado.value = true;
  }
};



//Manejo de input: solo números y longitud según tipo
const handleCedulaInput = (event) => {
  documentoNoEncontrado.value = false
  const soloNumeros = event.target.value.replace(/\D/g, '')
  const tipo = form.value.tiposDemandante
  const maxLength = tipo === 'Persona Jurídica' ? 9 : 11
  form.value.cedulaDemandante = soloNumeros.slice(0, maxLength)
  cedulaInvalida.value = !validarIdentificacion(form.value.cedulaDemandante, tipo)
}


const handleCedulaRepresentanteInput = (event) => {
  documentoRepNoEncontrado.value = false;
  const soloNumeros = event.target.value.replace(/\D/g, '');
  form.value.cedulaRepresentante = soloNumeros.slice(0, 11);
};

const registrarLitigio = async () => {
  const tipo = form.value.tiposDemandante

  // 1. Validación de cédula/RNC
  if (!validarIdentificacion(form.value.cedulaDemandante, tipo)) {
    cedulaInvalida.value = true
    const tipoTexto = tipo === 'Persona Jurídica' ? 'RNC (9 dígitos)' : 'Cédula (11 dígitos)'
    push.error(`Debe ingresar un ${tipoTexto} válido.`)
    return
  } else {
    cedulaInvalida.value = false
  }

  // 2. Validación si no se encontró el documento
  if (documentoNoEncontrado.value) {
    push.error('La cédula o RNC ingresado no fue encontrado en los registros.')
    return
  }

  // Validación cédula representante
  if (!/^\d{11}$/.test(form.value.cedulaRepresentante)) {
    documentoRepNoEncontrado.value = true;
    push.error('Debe ingresar una cédula válida del representante (11 dígitos).');
    return;
  }

  if (documentoRepNoEncontrado.value) {
    push.error('La cédula del representante no fue encontrada en los registros.');
    return;
  }

  // 3. Armar el payload solo si pasó todas las validaciones
  const payload = {
    id_Ltg: form.value.id_Ltg,
    ltg_acto: form.value.noActo,
    ltg_Fecha_Acto: form.value.fechaActo,
    id_Tipo_Demanda: form.value.tipoDemanda,
    ltg_Cedula_Demandante: form.value.cedulaDemandante,
    ltg_Nacionalidad: form.value.Nacionalidad,
    ltg_Nombre_Demandante: form.value.ltg_Nombre_Demandante,
    ltg_Tipo_Demandante: tipo === 'Otros' ? form.value.otrosDemandante : tipo,
    ltg_Cedula_Representante: form.value.cedulaRepresentante,
    ltg_Nombre_Representante: form.value.nombreRepresentante,
    ltg_Nacionalidad_Representante: form.value.ltg_Nacionalidad_Representante,
    ltg_Fecha_Audiencia: form.value.fechaAudiencia
      ? form.value.fechaAudiencia.toISOString().split('T')[0]
      : null,
    ltg_Fecha_Actualizacion: new Date().toISOString().substring(0, 10),
    id_Tribunal: form.value.tribunal,
    id_Sentencia: form.value.id_Sentencia,
    id_usuario: form.value.id_usuario,
    id_Estatus: form.value.estatus,
  }

  try {
    const response = await fetch('/api/Litigio/EditarLitigio', {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    });

    let data;
    try {
      data = await response.clone().json();
    } catch (jsonErr) {
      const text = await response.text();
      console.error("❌ Respuesta no es JSON:", text);
      data = { error: text };
    }

    console.log("👉 Datos que devuelve el backend:", data);

    if (response.ok) {
      push.success('Litigio actualizado exitosamente.');
      localStorage.removeItem('litigioModificacion');

      const usuarioActual = JSON.parse(localStorage.getItem('usuario'));
      const perfil = usuarioActual?.perfil;

      const rutasPorPerfil = {
        1: '/Seguimiento',
        2: '/GestionLitigios',
        3: '/LitigiosRegistrados',
        4: '/abogado/inicio'
      };

      const ruta = rutasPorPerfil[perfil] || '/Seguimiento';
      router.push(ruta);
    } else {
      push.error(data?.message || 'Error al actualizar el litigio.');
    }
  } catch (err) {
    console.error('Error al actualizar:', err);
    push.error('Error de red o campos incompletos.');
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

.border-1 {
  border: 1px solid #ccc;
}

.custom-blue {
  background-color: #003870 !important;
  border-color: #003870 !important;
  color: white !important;
}

.field label {
  font-weight: 600;
  color: #003870;
}

.custom-home-btn {
  font-size: 1rem;
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
