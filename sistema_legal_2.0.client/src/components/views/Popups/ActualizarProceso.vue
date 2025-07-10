<template>
  <div class="custom-header">
  </div>
  <div class="form-content">
    <div class="field">
      <br>
      <label class="block mb-2 text-sm font-medium">Título personalizado</label>
      <InputText v-model="numero" class="w-full" placeholder="" />
    </div>

    <div class="field">
      <label class="block mb-2 text-sm font-medium">Estado</label>
      <Dropdown id="tipoAudiencia" v-model="tipo" :options="tiposAudiencia" optionLabel="label" optionValue="value"
        class="w-full" placeholder="Seleccione un tipo" />
    </div>
    <div class="field" v-if="tipo === 'Cierre del Caso'">
      <label class="block mb-2 text-sm font-medium">Resultado del caso</label>
      <div class="flex items-center gap-4">
        <label class="inline-flex items-center gap-2">
          <input type="radio" name="resultadoCaso" v-model="cierre" :value="true" />
          <span>A favor</span>
        </label>
        <label class="inline-flex items-center gap-2">
          <input type="radio" name="resultadoCaso" v-model="cierre" :value="false" />
          <span>En contra</span>
        </label>
      </div>
    </div>
    <div v-if="tipo === 'Cierre del Caso'" class="field">
      <label class="block mb-2 text-sm font-medium">Evidencia del cierre *</label>
      <FileUpload name="Archivo" customUpload @select="handleExpedienteUpload" mode="basic" chooseLabel="Elegir archivo"
        class="w-full" style="background-color: #003870;" />
    </div>

    <div v-if="tipo === 'Cierre del Caso'" class="field">
      <label class="block mb-2 text-sm font-medium">Nombre de la evidencia</label>
      <InputText v-model="nombreEvidencia" class="w-full" placeholder="Ej: Resolución final" />
    </div>

    <div v-if="tipo === 'Cierre del Caso'" class="field">
      <label class="block mb-2 text-sm font-medium">Comentario</label>
      <textarea v-model="comentario" rows="4" maxlength="2000" class="texAreaComentario w-full"></textarea>
      <small>{{ comentario.length }}/2000 caracteres</small>
    </div>


    <div class="field">
      <label class="block mb-2 text-sm font-medium">Fecha de la declaracion *</label>
      <Calendar v-model="Fecha" dateFormat="yy-mm-dd" showIcon :minDate="hoy" class="w-full"
        placeholder="Seleccione una fecha" />
    </div>

    <div class="field">
      <label class="block mb-2 text-sm font-medium">Hora de la declaracion *</label>
      <Calendar v-model="horaSeleccionada" showIcon timeOnly hourFormat="12" placeholder="Ej: 8:00am" class="w-full" />
    </div>

    <div class="field">
      <label class="block mb-2 text-sm font-medium">Tribunal *</label>
      <Dropdown v-model="tribunalSeleccionado" :options="tribunales" optionLabel="nombre_Tribunal"
        optionValue="id_Tribunal" placeholder="Seleccione un tribunal" class="w-full" filter />
    </div>

    <div class="field">
      <label class="block mb-2 text-sm font-medium">Sala *</label>
      <Dropdown v-model="salaId" :options="salasFiltradas" :disabled="!tribunalSeleccionado" optionLabel="nombre"
        optionValue="idSala" placeholder="Seleccione una sala" class="w-full" filter />
    </div>

    <div class="mt-4 text-center">
      <Button v-if="tipo === 'Cierre del Caso'" label="Guardar Cierre" icon="pi pi-lock"
        class="p-button p-button-danger" @click="guardarCierre" />
      <Button v-else label="Guardar" icon="pi pi-save" class="p-button p-button-success" @click="guardarSimple" />
    </div>
  </div>

</template>

<script setup>
import { push } from 'notivue'
import axios from 'axios';
import { ref, onMounted, computed, watch, nextTick } from 'vue'

const emit = defineEmits(['close', 'actualizar']);
const props = defineProps({
  id_Ltg: { type: Number, required: true }
});

const visible = ref(false);
const numero = ref('');
const tipo = ref('');
const Fecha = ref('');
const horaSeleccionada = ref(null);
const hoy = new Date();
const cierre = ref(null);
const tribunalSeleccionado = ref(null);
const salaId = ref(null);
const tribunales = ref([]);
const todasLasSalas = ref([]);
const rawUser = localStorage.getItem('usuario');
const user = rawUser ? JSON.parse(rawUser) : null;
const archivo = ref(null);
const nombreEvidencia = ref('');
const comentario = ref('');

const tiposAudiencia = [
  { label: 'Sentencia', value: 'Sentencia' },
  { label: 'Recurso de Casación', value: 'Recurso de Casación' },
  { label: 'Sentencia Definitiva', value: 'Sentencia Definitiva' },
  { label: 'Cierre del Caso', value: 'Cierre del Caso' },
]

// Computed: filtrar salas del tribunal seleccionado
const salasFiltradas = computed(() => {
  return tribunalSeleccionado.value
    ? todasLasSalas.value.filter(s => s.idTribunal === tribunalSeleccionado.value)
    : [];
});

function handleExpedienteUpload(event) {
  archivo.value = event.files[0];
}

function combinarFechaYHora(fecha, hora) {
  if (!fecha || !hora) return null;
  const resultado = new Date(fecha);
  resultado.setHours(hora.getHours());
  resultado.setMinutes(hora.getMinutes());
  resultado.setSeconds(0);
  resultado.setMilliseconds(0);
  return resultado;
}

// Limpiar sala si cambia el tribunal
watch(tribunalSeleccionado, () => {
  salaId.value = null;
});

function resetFormulario() {
  numero.value = '';
  tipo.value = '';
  Fecha.value = null;
  horaSeleccionada.value = null;
  tribunalSeleccionado.value = null;
  salaId.value = null;
}

watch(() => props.id_Ltg, () => {
  resetFormulario();
  visible.value = true;
});

// Cargar datos
const cargarDatosDropdowns = async () => {
  try {
    const response = await axios.get('/api/Litigio/datos-litigio');
    tribunales.value = response.data.tribunales || [];
    todasLasSalas.value = response.data.salas || [];
  } catch (error) {
    console.error('Error al cargar datos:', error);
    push.error('No se pudieron cargar los datos.');
  }
};

const cargarUltimaAudiencia = async () => {
  try {
    const { data } = await axios.get(`/api/Files/ultima-Audiencia/${props.id_Ltg}`);
    const audiencia = data.data;

    Fecha.value = new Date(audiencia.FechaAudiencia);
    const fechaOriginal = new Date(audiencia.FechaAudiencia);
    fechaOriginal.setHours(fechaOriginal.getHours() + 2);
    horaSeleccionada.value = fechaOriginal;
    console.log(audiencia)
    tribunalSeleccionado.value = audiencia.IdSala
      ? todasLasSalas.value.find(s => s.idSala === audiencia.IdSala)?.idTribunal || null
      : null;
    await nextTick();
    salaId.value = audiencia.IdSala || null;
  } catch (error) {
    console.error('Error al cargar audiencia:', error);
    push.error('No se pudo cargar la audiencia.');
  }
};

async function guardarSimple() {
  const fechaCompleta = combinarFechaYHora(Fecha.value, horaSeleccionada.value);
  if (!fechaCompleta) return push.warning("Debe seleccionar fecha y hora.");
  if (!salaId.value) return push.warning("Debe seleccionar una sala.");
  if (!tipo.value) return push.warning("Debe seleccionar el nuevo estado del litigio");
  if (!numero.value) return push.warning("Debe colocar un título para identificar el estado");
  if (!user || !user.idUsuario) return push.warning("No se encontró el usuario en localStorage.");

  const bodySimple = {
    idLitigio: Number(props.id_Ltg),
    numero: numero.value.trim(),
    tipo: tipo.value.trim(),
    fecha: fechaCompleta,
    salaId: Number(salaId.value),
    id_usuario: user.idUsuario
  };

  const notif = push.promise('Agregando audiencia...');
  try {
    await axios.post('/api/Files/crearAudiencias', bodySimple);
    notif.resolve('Audiencia creada correctamente');
    emit('actualizar');
    emit('close');
  } catch (error) {
    console.error('Error:', error);
    const msg = error.response?.data?.error || 'Error al crear la audiencia';
    push.error(msg);
  }
}

async function guardarCierre() {
  const fechaCompleta = combinarFechaYHora(Fecha.value, horaSeleccionada.value);
  if (!fechaCompleta) return push.warning("Debe seleccionar fecha y hora.");
  if (!salaId.value) return push.warning("Debe seleccionar una sala.");
  if (!tipo.value) return push.warning("Debe seleccionar el nuevo estado del litigio");
  if (!numero.value) return push.warning("Debe colocar un título para identificar el estado");
  if (!user || !user.idUsuario) return push.warning("No se encontró el usuario en localStorage.");
  if (cierre.value === null) return push.warning("Debe seleccionar el resultado del caso.");
  if (!archivo.value) return push.warning("Debe seleccionar un archivo de evidencia.");

  const confirmar = confirm("¿Está seguro que desea cerrar este caso? Esta acción es irreversible.");
  if (!confirmar) return;

  const body = {
    idLitigio: Number(props.id_Ltg),
    numero: numero.value.trim(),
    tipo: tipo.value.trim(),
    fecha: fechaCompleta,
    salaId: Number(salaId.value),
    id_usuario: user.idUsuario,
    cierre: cierre.value
  };

  const notif = push.promise('Agregando audiencia...');
  try {
    const { data } = await axios.post('/api/Files/crearAudiencias', body);
    const idAudiencia = data.idAudiencia;
    notif.resolve('Audiencia creada correctamente');

    const formData = new FormData();
    formData.append('Archivo', archivo.value);
    formData.append('Comentario', comentario.value?.trim() || 'Documento subido sin descripción.');
    formData.append('IdUsuario', user.idUsuario);
    formData.append('NombreEvidencia', nombreEvidencia.value?.trim() || archivo.value.name.split('.')[0]);
    formData.append('IdLitigio', props.id_Ltg);
    formData.append('IdAudiencia', idAudiencia);

    const fileNotif = push.promise('Subiendo evidencia...');
    await axios.post('/api/Files/subir-evidencia', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    fileNotif.resolve('Evidencia subida correctamente');

    emit('actualizar');
    emit('close');
  } catch (error) {
    console.error('Error:', error);
    const msg = error.response?.data?.error || 'Error al crear la audiencia';
    push.error(msg);
  }
}


onMounted(async () => {
  await cargarDatosDropdowns();
  await cargarUltimaAudiencia();

  visible.value = true;
});
</script>


<style scoped>
.dialog-agregar-audiencia {
  width: 500px;
  max-width: 90vw;
}

.field {
  margin-bottom: 1rem;
}

.texAreaComentario {
  white-space: pre-wrap;
  word-wrap: break-word;
  resize: vertical;
  padding: 10px;
  font-size: 1rem;
  border: 1px solid #ccc;
  border-radius: 5px;
}

.custom-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  font-weight: bold;
  padding: 0 0 0 4rem;
  border-bottom: 1px solid rgb(221, 216, 216);
}

.dialog-title {
  font-size: 1.2rem;
  color: #003870;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.8rem;
  cursor: pointer;
  color: #888;
  transition: color 0.2s;
}

.close-btn:hover {
  color: #e53935;
}
</style>
