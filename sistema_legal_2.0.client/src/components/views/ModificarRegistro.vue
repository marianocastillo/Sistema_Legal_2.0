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
              <Dropdown v-model="form.tiposDemandante" :options="tiposDemandante" optionLabel="label"
              optionValue="value" class="w-full" placeholder="Tipo de Demandante" />
            </div>
            <div class="field col-12 md:col-4">
              <InputText
                v-model="form.cedulaDemandante"
                class="w-full"
                :class="{ 'p-invalid': cedulaInvalida }"
                :maxlength="form.tiposDemandante === 'Empresa' ? 9 : 11"
                @input="handleCedulaInput"
                :placeholder="form.tiposDemandante === 'Empresa' ? 'RNC de la empresa' : 'Cédula del demandante'"
              />
              <small v-if="cedulaInvalida" class="p-error">
                {{ form.tiposDemandante === 'Empresa' ? 'Debe tener 9 dígitos numéricos (RNC).' : 'Debe tener 11 dígitos numéricos (cédula).' }}
              </small>
            </div>
            <div class="field col-12 md:col-4">
              <InputText v-model="form.demandante" class="w-full"
                :placeholder="form.tiposDemandante === 'Empresa' ? 'Nombre de la empresa' : 'Nombre del demandante'" />
            </div>
            <div class="field col-12 md:col-4">
              <InputText v-model="form.Nacionalidad"
                :placeholder="form.tiposDemandante === 'Empresa' ? 'País de constitución' : 'Nacionalidad'"
                class="w-full" />
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
              <Calendar v-model="form.fechaActo" dateFormat="yy-mm-dd" showIcon placeholder="Fecha del acto"
                class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.tipoDemanda" :options="tiposDemanda" optionLabel="nombre" optionValue="id_demanda"
                placeholder="Tipo de Demanda" class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Calendar v-model="form.fechaAudiencia" dateFormat="yy-mm-dd" showIcon placeholder="Fecha de audiencia"
                class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.tribunal" :options="tribunales" optionLabel="nombre_Tribunal"
                optionValue="id_Tribunal" placeholder="--Seleccione Tribunal--" class="w-full" />
            </div>
            <div class="field col-12 md:col-4">
              <Dropdown v-model="form.estatus" :options="estatusList" optionLabel="ltg_description"
                optionValue="ltg_estatus" placeholder="--Seleccione Estatus--" class="w-full" />
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

const form = ref({
  id_Ltg: null,
  noActo: '',
  fechaActo: '',
  tipoDemanda: null,
  cedulaDemandante: '',
  Nacionalidad: '',
  demandante: '',
  tiposDemandante: null,
  otrosDemandante: '',
  cedulaRepresentante: '',
  nombreRepresentante: '',
  fechaAudiencia: '',
  tribunal: null,
  estatus: null,
  id_usuario: null,
  id_Sentencia: null
})

// ✅ Estado visual de error para cédula o RNC
const cedulaInvalida = ref(false)

const tiposDemanda = ref([])
const tiposDemandante = [
  { label: 'Empleado', value: 'Empleado' },
  { label: 'Empresa', value: 'Empresa' },
  { label: 'Otros', value: 'Otros' }
]
const tribunales = ref([])
const estatusList = ref([])

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
      fechaActo: data.ltg_Fecha_Acto?.substring(0, 10),
      tipoDemanda: data.id_Tipo_Demanda ?? data.tipoDemanda_Id ?? null,
      cedulaDemandante: data.ltg_Cedula_Demandante,
      Nacionalidad: data.ltg_Nacionalidad,
      demandante: data.ltg_Demandante,
      tiposDemandante: data.ltg_Tipo_Demandante,
      otrosDemandante: data.ltg_Tipo_Demandante === 'Otros' ? data.otrosDemandante || '' : '',
      cedulaRepresentante: data.ltg_Cedula_Representante,
      nombreRepresentante: data.ltg_Nombre_Representante,
      fechaAudiencia: data.ltg_Fecha_Audiencia?.substring(0, 10),
      tribunal: data.id_Tribunal,
      estatus: data.id_Estatus ?? data.ltg_estatus ?? null,
      id_usuario: data.id_usuario ?? data.idUsuario ?? null,
      id_Sentencia: data.id_Sentencia ?? null
    }
  }
})


// ✅ Validación dinámica para cédula o RNC según el tipo
const validarIdentificacion = (valor, tipo) => {
  if (!valor) return false
  const regex = tipo === 'Empresa' ? /^\d{9}$/ : /^\d{11}$/
  return regex.test(valor)
}

// ✅ Manejo de input: solo números y longitud según tipo
const handleCedulaInput = (event) => {
  const soloNumeros = event.target.value.replace(/\D/g, '')
  const tipo = form.value.tiposDemandante
  const maxLength = tipo === 'Empresa' ? 9 : 11
  form.value.cedulaDemandante = soloNumeros.slice(0, maxLength)
  cedulaInvalida.value = !validarIdentificacion(form.value.cedulaDemandante, tipo)
}

const registrarLitigio = async () => {
  const tipo = form.value.tiposDemandante
  if (!validarIdentificacion(form.value.cedulaDemandante, tipo)) {
    cedulaInvalida.value = true
    const tipoTexto = tipo === 'Empresa' ? 'RNC (9 dígitos)' : 'Cédula (11 dígitos)'
    push.error(`Debe ingresar un ${tipoTexto} válido.`)
    return
  } else {
    cedulaInvalida.value = false
  }

  const payload = {
    id_Ltg: form.value.id_Ltg,
    ltg_acto: form.value.noActo,
    ltg_Fecha_Acto: form.value.fechaActo,
    id_Tipo_Demanda: form.value.tipoDemanda,
    ltg_Cedula_Demandante: form.value.cedulaDemandante,
    ltg_Nacionalidad: form.value.Nacionalidad,
    ltg_Demandante: form.value.demandante,
    ltg_Tipo_Demandante: tipo === 'Otros' ? form.value.otrosDemandante : tipo,
    ltg_Cedula_Representante: form.value.cedulaRepresentante,
    ltg_Nombre_Representante: form.value.nombreRepresentante,
    ltg_Fecha_Audiencia: form.value.fechaAudiencia,
    ltg_Fecha_Actualizacion: new Date().toISOString().substring(0, 10),
    id_Tribunal: form.value.tribunal,
    id_Sentencia: form.value.id_Sentencia,
    id_usuario: form.value.id_usuario,
    id_Estatus: form.value.estatus
  }

  try {
    const response = await fetch('/api/Litigio/EditarLitigio', {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    })

    // eslint-disable-next-line no-unused-vars
    const data = await response.json()

    if (response.ok) {
      push.success('Litigio actualizado exitosamente.')
      localStorage.removeItem('litigioModificacion')
      router.push('/drawer/home')
    } else {
      push.error('Error al actualizar el litigio.')
    }
  } catch (err) {
    console.error('Error al actualizar:', err)
    push.error('Error de red o campos incompletos.')
  }
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
