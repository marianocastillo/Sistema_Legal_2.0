<script setup>
import { ref, onMounted, computed, watch, nextTick } from 'vue';

import axios from 'axios';
import Dialog from 'primevue/dialog';
import { push } from 'notivue';

const emit = defineEmits(['close', 'actualizar']);

const props = defineProps({
  id_Ltg: { type: Number, required: true }
});

const visible = ref(true);
const numero = ref('');
const tipo = ref('');
const Nombre = ref('');
const Fecha = ref(null);
const horaSeleccionada = ref(null);
const hoy = new Date();
const placeholderFecha = new Date(Date.now() + 86400000).toISOString().split('T')[0];
const horaPorDefecto = new Date(); horaPorDefecto.setHours(9, 0, 0, 0);

const salaId = ref(null);
const tribunalSeleccionado = ref(null);

const tribunales = ref([]);
const todasLasSalas = ref([]);
const rawUser = localStorage.getItem('usuario');
const user = rawUser ? JSON.parse(rawUser) : null;


const tiposAudiencia = [
  { label: 'Presencial', value: 'Presencial' },
  { label: 'Virtual', value: 'Virtual' },
]

// Filtrar salas según el tribunal seleccionado
const salasFiltradas = computed(() => {
  return tribunalSeleccionado.value
    ? todasLasSalas.value.filter(s => s.idTribunal === tribunalSeleccionado.value)
    : [];
});

watch(tribunalSeleccionado, () => {
  salaId.value = null; // limpiar sala si cambian tribunal
});

const cargarDatosDropdowns = async () => {
  try {
    const response = await axios.get('/api/Litigio/datos-litigio');
    tribunales.value = response.data.tribunales || [];
    todasLasSalas.value = response.data.salas || [];
  } catch (error) {
    console.error('Error al cargar datos litigio:', error);
    push.error('No se pudieron cargar los datos.');
  }
};
const cargarUltimaAudiencia = async () => {
  try {
    const { data } = await axios.get(`/api/Files/ultima-Audiencia/${props.id_Ltg}`);
    const audiencia = data.data;

    numero.value = audiencia.NumeroAudiencia || '';
    tipo.value = audiencia.TipoAudiencia || '';
    Fecha.value = new Date(audiencia.FechaAudiencia);
    horaSeleccionada.value = new Date(audiencia.FechaAudiencia);
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


function combinarFechaYHora(fecha, hora) {
  if (!fecha || !hora) return null;
  const resultado = new Date(fecha);
  resultado.setHours(hora.getHours());
  resultado.setMinutes(hora.getMinutes());
  resultado.setSeconds(0);
  resultado.setMilliseconds(0);
  return resultado;
}

async function guardar() {
  const fechaFinal = combinarFechaYHora(Fecha.value, horaSeleccionada.value);
  if (!fechaFinal) return push.warning('Debe ingresar una fecha y hora válida.');
  if (!salaId.value) return push.warning('Debe seleccionar una sala.');

  const body = {
    IdLitigio: props.id_Ltg,
    Numero: numero.value.trim(),
    Tipo: tipo.value.trim(),
    Fecha: fechaFinal,
    SalaId: salaId.value,
    id_usuario: user.idUsuario
  };

  const notif = push.promise('Actualizando audiencia...');
  try {
    await axios.put('/api/Files/actualizarAudiencias', body);
    emit('actualizar');
    emit('close');
    notif.resolve('Audiencia creada correctamente');
  } catch (error) {
    const msg = error.response?.data?.error || 'Error al crear la audiencia';
    notif.reject(msg);
  }
}

onMounted(async () => {
  await cargarDatosDropdowns();
  await cargarUltimaAudiencia();
});

</script>

<template>
  <Dialog :style="{ width: '40%' }" v-model:visible="visible" modal :closable="false" :draggable="false" @hide="emit('close')"
    class="dialog-editar-audiencia">
      <template #header>
      <div class="flex justify-content-between align-items-center m-2 flex-wrap gap-2 pt-3 w-100 custom-header">
        <!-- Columna izquierda: solo el título -->
        <div class="flex-grow">
          <h2 class="text-2xl font-bold m-0">Editar Audiencia</h2>
        </div>

        <!-- Columna derecha: botones + buscador -->
        <div class="flex items-center gap-2">
         <button class="close-btn" @click="visible = false">&times;</button>

        </div>
      </div>
    </template>
    <!-- Formulario vertical -->
    <div class="form-content">
      <div class="field">
        <label class="block mb-2 text-sm font-medium">Nombre de la Audiencia *</label>
        <InputText v-model="numero" class="w-full" placeholder="Nombre de la audiencia" />
      </div>

      <div class="field">
        <label class="block mb-2 text-sm font-medium">Modalidad *</label>
        <Dropdown v-model="tipo" :options="tiposAudiencia" optionLabel="label" optionValue="value" class="w-full"
          placeholder="Seleccione un tipo" />
      </div>

      <div class="field">
        <label class="block mb-2 text-sm font-medium">Fecha de Audiencia *</label>
        <Calendar v-model="Fecha" dateFormat="yy-mm-dd" showIcon :minDate="hoy" class="w-full"
          placeholder="Seleccione una fecha" />
      </div>

      <div class="field">
        <label class="block mb-2 text-sm font-medium">Hora Audiencia *</label>
        <Calendar v-model="horaSeleccionada" showIcon timeOnly hourFormat="12" placeholder="Ej: 8:00am"
          class="w-full" />
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
        <Button label="Actualizar" icon="pi pi-calendar-plus" class="p-button text-white" @click="guardar"
          :loading="uploading" style="background-color: #003870" />
      </div>
    </div>

    <!-- Notificaciones -->
    <Notivue v-slot="item">
      <Notifications :item="item" />
    </Notivue>
  </Dialog>
</template>

<style scoped>
.dialog-editar-audiencia {
  width: 500px;
  max-width: 95vw;
}

.form-content {
  padding: 1rem 1.5rem;
}

.field {
  margin-bottom: 1rem;
}

.custom-header {
  border-bottom: 1px solid #ddd;
}

.dialog-title {
  font-size: 1.2rem;
  color: #003870;
  font-weight: bold;
}


</style>
