<script setup>
import { ref, computed, onMounted } from 'vue'
import Dialog from 'primevue/dialog'
import ConfirmDialog from 'primevue/confirmdialog'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import Button from 'primevue/button'
import { useConfirm } from 'primevue/useconfirm'
import api from '@/utilities/api.js'
import { push } from 'notivue'
import DialogSalas from '../Salas/DialogSalas.vue'


const props = defineProps(['visible'])
const emit = defineEmits(['update:visible', 'close'])
const mostrarDialogoSalas = ref(false)
const tribunalSeleccionado = ref(null)
const errores = ref({})
const mostrarMensajeExito = ref(false)
const mostrarMensajeError = ref(false)
const mensajeError = ref('')
const mostrarFormulario = ref(false)
const tribunalEditando = ref({})
const confirm = useConfirm()
const tribunales = ref([])
const search = ref('')
const page = ref(1)
const perPage = 10
const guardando = ref(false)

const totalPages = computed(() => Math.ceil(filteredTribunales.value.length / perPage))

const paginatedTribunales = computed(() => {
  const start = (page.value - 1) * perPage
  return filteredTribunales.value.slice(start, start + perPage)
})

const estadoOptions = [
  { label: 'Activo', value: true },
  { label: 'Inactivo', value: false }
]
const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})
const camposFormulario = [
  { label: 'Nombre', model: 'nombre_Tribunal' },
  { label: 'Descripción', model: 'descripcion' },
  { label: 'Teléfono', model: 'telefono' },
  { label: 'Dirección', model: 'direccion' },
  { label: 'Provincia', model: 'distrito' }
]
const provincias = [
  { label: 'Azua', value: 'Azua' },
  { label: 'Bahoruco', value: 'Bahoruco' },
  { label: 'Barahona', value: 'Barahona' },
  { label: 'Dajabón', value: 'Dajabón' },
  { label: 'Distrito Nacional', value: 'Distrito Nacional' },
  { label: 'Duarte', value: 'Duarte' },
  { label: 'El Seibo', value: 'El Seibo' },
  { label: 'Elías Piña', value: 'Elías Piña' },
  { label: 'Espaillat', value: 'Espaillat' },
  { label: 'Hato Mayor', value: 'Hato Mayor' },
  { label: 'Hermanas Mirabal', value: 'Hermanas Mirabal' },
  { label: 'Independencia', value: 'Independencia' },
  { label: 'La Altagracia', value: 'La Altagracia' },
  { label: 'La Romana', value: 'La Romana' },
  { label: 'La Vega', value: 'La Vega' },
  { label: 'María Trinidad Sánchez', value: 'María Trinidad Sánchez' },
  { label: 'Monseñor Nouel', value: 'Monseñor Nouel' },
  { label: 'Monte Plata', value: 'Monte Plata' },
  { label: 'Montecristi', value: 'Montecristi' },
  { label: 'Pedernales', value: 'Pedernales' },
  { label: 'Peravia', value: 'Peravia' },
  { label: 'Puerto Plata', value: 'Puerto Plata' },
  { label: 'Samaná', value: 'Samaná' },
  { label: 'San Cristóbal', value: 'San Cristóbal' },
  { label: 'San José de Ocoa', value: 'San José de Ocoa' },
  { label: 'San Juan', value: 'San Juan' },
  { label: 'San Pedro de Macorís', value: 'San Pedro de Macorís' },
  { label: 'Sánchez Ramírez', value: 'Sánchez Ramírez' },
  { label: 'Santiago', value: 'Santiago' },
  { label: 'Santiago Rodríguez', value: 'Santiago Rodríguez' },
  { label: 'Santo Domingo', value: 'Santo Domingo' },
  { label: 'Valverde', value: 'Valverde' }
]

const filteredTribunales = computed(() => {
  if (!search.value) return tribunales.value
  const term = search.value.toLowerCase()
  return tribunales.value.filter(t =>
    Object.values(t).some(val => String(val).toLowerCase().includes(term))
  )
})




onMounted(() => cargarTribunales())

async function cargarTribunales() {
  try {
    const { data } = await api.get('/api/Tribunales/Tribunales')
    tribunales.value = data
  } catch (err) {
    push.error('No se pudieron cargar los tribunales')
  }
}

async function guardarTribunal() {
  errores.value = {} // limpiar errores
  const body = tribunalEditando.value

  // Validaciones manuales
  if (!body.nombre_Tribunal?.trim()) errores.value.nombre_Tribunal = 'Este campo es obligatorio'
  if (!body.descripcion?.trim()) errores.value.descripcion = 'Este campo es obligatorio'
  if (!body.direccion?.trim()) errores.value.direccion = 'Este campo es obligatorio'
  if (!body.distrito?.trim()) errores.value.distrito = 'Este campo es obligatorio'

  validarCampo('telefono')

  // Si hay errores en los inputs, salimos antes de mostrar loading
  if (Object.keys(errores.value).length) return

  // activamos loading DESPUÉS de validar
  guardando.value = true

  try {
    // Esperamos 2 segundos simulando "guardado"
    await new Promise(resolve => setTimeout(resolve, 1000))

    if (body.id_Tribunal) {
      await api.put(`/api/Tribunales/ActualizarTribunal/${body.id_Tribunal}`, body)
    } else {
      await api.post('/api/Tribunales/CrearTribunal', body)
    }

    await cargarTribunales()

    mostrarMensajeExito.value = true
    setTimeout(() => {
      cerrarFormularioexito()
    }, 10000)
  } catch (err) {
    mensajeError.value = 'Ocurrió un error al guardar el tribunal. Intenta nuevamente.'
    mostrarMensajeError.value = true
  } finally {
    guardando.value = false
  }
}


function abrirDialogoSalas(tribunal) {
  confirm.close(); // fuerza el cierre de confirmDialog si estaba activo
  tribunalSeleccionado.value = tribunal
  dialogVisible.value = false
  mostrarDialogoSalas.value = true
}


function abrirFormularioNuevo() {
  errores.value = {}
  tribunalEditando.value = {
    nombre_Tribunal: '', descripcion: '', telefono: '', direccion: '', distrito: '', estatus: true
  }
  mostrarFormulario.value = true
  dialogVisible.value = false
}

function abrirFormularioEditar(tribunal) {
  tribunalEditando.value = { ...tribunal }
  dialogVisible.value = false
  mostrarFormulario.value = true
}

function cerrarFormulario() {

  mostrarFormulario.value = false
  dialogVisible.value = true
}

function cerrarFormularioexito() {
  mostrarMensajeExito.value = false
  mostrarFormulario.value = false
  dialogVisible.value = true
}

function confirmarEliminacion(id) {
  console.log(id);
  confirm.require({
    message: '¿Seguro que deseas eliminar este tribunal?',
    header: 'Eliminar Tribunal',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Sí',
    rejectLabel: 'Cancelar',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await api.delete(`/api/Tribunales/EliminarTribunal/${id}`)
        await cargarTribunales()
        push.success('Tribunal eliminado correctamente')
      } catch (err) {
        push.error('No se pudo eliminar el tribunal, tiene casos agregados')
      }
    }
  })
}

function validarCampo(model) {
  const valor = tribunalEditando.value[model]

  if (!valor || !valor.trim()) {
    errores.value[model] = 'Este campo es obligatorio'
    return
  }

  // Validación específica para teléfono
  if (model === 'telefono') {
    if (!/^\d+$/.test(valor)) {
      errores.value.telefono = 'Solo se permiten números'
    } else if (valor.length < 10) {
      errores.value.telefono = 'Debe tener al menos 10 dígitos'
    } else {
      delete errores.value.telefono
    }
    return
  }

  // Limpiar error si el campo es válido
  delete errores.value[model]
}

function soloNumeros(event) {
  const key = event.key
  if (!/^\d$/.test(key)) {
    event.preventDefault()
  }
}
</script>

<template>

  <!-- dialogo principal del Mantenimiento de tribunales  -->
  <Dialog v-model:visible="dialogVisible" modal class="dialog-tribunales" :closable="false" :draggable="false"
    :style="{ width: '80%', height: '85vh' }">
    <template #header>
      <div class="flex justify-content-between align-items-center m-2 flex-wrap gap-2 pt-3 w-100">
        <!-- Columna izquierda: solo el título -->
        <div class="flex-grow">
          <h2 class="text-2xl font-bold m-0">Mantenimiento de Tribunales</h2>
        </div>

        <!-- Columna derecha: botones + buscador -->
        <div class="flex items-center gap-2 filtro-busqueda-bar">
          <button class="btn-litigio" @click="dialogVisible = false">
            <i class="pi pi-times me-2"></i> Cerrar
          </button>
          <button class="btn-litigio" @click="abrirFormularioNuevo">
            <i class="fas fa-plus me-2"></i> Nuevo Tribunal
          </button>
          <div class="search-container">
            <span class="p-input-icon-left search-input-wrapper">
              <i class="pi pi-search search-icon" />
              <input type="text" class="search-input" placeholder="Buscar..." v-model="search" />
            </span>
          </div>
        </div>
      </div>
    </template>
    <table class="table table-sm table-bordered table-hover mt-3">
      <thead class="table-light">
        <tr>
          <th>Nombre</th>
          <th>Descripción</th>
          <th>Teléfono</th>
          <th>Dirección</th>
          <th>Provincia</th>
          <th>Estado</th>
          <th class="text-center" :style="{ width: '120px' }">Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="tribunal in paginatedTribunales" :key="tribunal.id_Tribunal">
          <td>{{ tribunal.nombre_Tribunal }}</td>
          <td>{{ tribunal.descripcion }}</td>
          <td>{{ tribunal.telefono }}</td>
          <td>{{ tribunal.direccion }}</td>
          <td>{{ tribunal.distrito }}</td>
          <td>
            <span class="tag" :class="tribunal.estatus ? 'status-sentencia' : 'status-casacion'">
              {{ tribunal.estatus ? 'Activo' : 'Inactivo' }}
            </span>
          </td>
          <td class="text-end ">
            <div class="btn-group">
              <button class="btn btn-sm btn-hover" style="background-color: #003870;"
                @click="abrirFormularioEditar(tribunal)">
                <i class="pi pi-pencil white-icon"></i>
              </button>
              <button class="btn btn-sm btn-hover" style="background-color: #003870;"
                @click="confirmarEliminacion(tribunal.id_Tribunal)">
                <i class="pi pi-trash white-icon"></i>
              </button>
              <button class="btn btn-sm btn-hover" style="background-color: #003870;"
                @click="abrirDialogoSalas(tribunal)">
                <i class="pi pi-eye white-icon"></i>
              </button>
            </div>
          </td>
        </tr>
        <tr v-if="filteredTribunales.length === 0">
          <td colspan="7" class="text-center text-muted">No hay tribunales registrados.</td>
        </tr>
      </tbody>
    </table>
    <nav class="mt-4">
      <ul class="pagination justify-content-end">
        <li class="page-item" :class="{ disabled: page === 1 }">
          <button class="page-link" @click="page--">Anterior</button>
        </li>
        <li class="page-item" v-for="p in totalPages" :key="p" :class="{ active: page === p }">
          <button class="page-link" @click="page = p">{{ p }}</button>
        </li>
        <li class="page-item" :class="{ disabled: page === totalPages }">
          <button class="page-link" @click="page++">Siguiente</button>
        </li>
      </ul>
    </nav>



    <div v-if="filteredTribunales.length === 0" class="text-center text-muted mt-2">No hay tribunales registrados.</div>
  </Dialog>


  <!-- dialogo de formulario de agregar tribunal  -->
  <Dialog v-model:visible="mostrarFormulario" modal class="dialog-formulario-form" :closable="false">

    <!-- Header personalizado -->
    <template #header>
      <div class="flex justify-content-center align-items-center w-full">
        <h2 class="text-2xl font-bold m-0">Registro Tribunal</h2>
      </div>
    </template>


    <!-- Contenido del formulario -->
    <div class="p-fluid formgrid grid mt-2">
      <!-- Campos de texto comunes -->
      <div class="field col-12 md:col-6" v-for="campo in camposFormulario.filter(c => c.model !== 'distrito')"
        :key="campo.model">
        <label>{{ campo.label }} <span class="text-red-500">*</span></label>

        <!-- Validar solo números en teléfono -->
        <InputText v-if="campo.model === 'telefono'" v-model="tribunalEditando.telefono" @keypress="soloNumeros"
          @input="validarCampo('telefono')" placeholder="Ingrese teléfono" :class="{ 'input-error': errores.telefono }"
          maxlength="10" />

        <!-- Resto de campos normales -->
        <InputText v-else v-model="tribunalEditando[campo.model]" @input="validarCampo(campo.model)"
          :placeholder="`Ingrese ${campo.label.toLowerCase()}`" :class="{ 'input-error': errores[campo.model] }" />

        <small class="text-red-500" v-if="campo.model === 'telefono' ? errores.telefono : errores[campo.model]">
          {{ campo.model === 'telefono' ? errores.telefono : errores[campo.model] }}
        </small>
      </div>

      <!-- Dropdown para Provincia -->
      <div class="field col-12 md:col-6">
        <label>Provincia <span class="text-red-500">*</span></label>
        <Dropdown v-model="tribunalEditando.distrito" :options="provincias" optionLabel="label" optionValue="value"
          placeholder="Seleccione una provincia" class="w-full fix-dropdown-width" @change="validarCampo('distrito')"
          :class="{ 'input-error': errores.distrito }" filter />
        <small class="text-red-500" v-if="errores.distrito">
          {{ errores.distrito }}
        </small>
      </div>

      <!-- Dropdown para Estado (solo visible al editar) -->
      <div class="field col-12 md:col-6" v-if="tribunalEditando.id_Tribunal">
        <label>Estado</label>
        <Dropdown v-model="tribunalEditando.estatus" :options="estadoOptions" optionLabel="label" optionValue="value"
          class="w-full" />
      </div>
    </div>


    <template #footer>
      <div class="flex gap-2 w-full justify-content-center mt-4">
        <Button label="Cancelar" class="btn-litigio" @click="cerrarFormulario" />
        <Button label="Guardar" class="btn-litigio" :loading="guardando" :disabled="guardando"
          @click="guardarTribunal" />
      </div>
    </template>

  </Dialog>

  <!-- dialogo de mensaje de exito al agregar tribunal -->
  <Dialog v-model:visible="mostrarMensajeExito" modal :closable="false" class="w-96">
    <div class="text-center p-4">
      <i class="pi pi-check-circle text-green-500 text-4xl mb-3"></i>
      <p>El tribunal fue guardado exitosamente.</p>

      <div class="flex justify-content-center mt-4">
        <Button label="Aceptar" class="btn-litigio" @click="cerrarFormularioexito" />
      </div>
    </div>
  </Dialog>

  <!-- dialogo de mensaje de error si no se puede agregar tribunal -->
  <Dialog v-model:visible="mostrarMensajeError" modal :closable="false" class="w-96">
    <div class="text-center p-4">
      <i class="pi pi-times-circle text-red-500 text-4xl mb-3"></i>
      <p>{{ mensajeError }}</p>

      <div class="flex justify-content-center mt-4">
        <Button label="Cerrar" class="btn-litigio" @click="mostrarMensajeError = false" />
      </div>
    </div>
  </Dialog>



  <DialogSalas v-if="tribunalSeleccionado" v-model:visible="mostrarDialogoSalas"
    :tribunal-id="tribunalSeleccionado.id_Tribunal" :nombre-tribunal="tribunalSeleccionado.nombre_Tribunal" />

  <ConfirmDialog v-if="dialogVisible" :key="dialogVisible" />

</template>

<style scoped>
.fix-dropdown-width {
  min-width: 100%;
  max-width: 100%;
  box-sizing: border-box;
}


.tribunal-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  margin-bottom: 0.5rem;
  background-color: #fff;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tribunal-info {
  display: flex;
  flex-direction: column;
}

.p-dropdown.p-focus {
  border-color: #003870 !important;
  box-shadow: 0 0 0 0.2rem rgba(0, 56, 112, 0.25);
}


.search-container {
  display: flex;
  align-items: center;
  height: 38px;
}

.search-input-wrapper {
  position: relative;
}

.search-input-wrapper .search-icon {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #6c757d;
}

.search-input {
  padding-left: 2rem;
  padding-right: 0.75rem;
  height: 38px;
  border-radius: 6px;
  border: 1px solid #ced4da;
  font-size: 0.88rem;
  background-color: #fff;
  color: #212529;
}

.input-error {
  border: 1px solid #dc2626 !important;
  box-shadow: 0 0 0 0.15rem rgba(220, 38, 38, 0.25);
}

.white-icon {
  color: white !important;
}

.btn-group .btn {
  margin: 0 2px;
}

thead th {
  background-color: rgb(241, 242, 250);
  font-weight: 600;
  color: #2e3842;
  font-size: 0.95rem;
  border: none !important;
}

.tag {
  padding: 0.4rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.8rem;
  font-weight: 600;
  display: inline-block;
  text-align: center;
  min-width: 100px;
}

.status-sentencia {
  background-color: #d4edda;
  color: #155724;
}

.status-casacion {
  background-color: #f8d7da;
  color: #721c24;
}

.pagination .page-link {
  color: #003870;
  border: 1px solid #ccc;
  padding: 0.3rem 0.75rem;
  font-size: 0.875rem;
}

.pagination .page-item.active .page-link {
  background-color: #003870;
  border-color: #003870;
  color: white;
}

.pagination .page-item.disabled .page-link {
  color: #aaa;
  pointer-events: none;
}
</style>
