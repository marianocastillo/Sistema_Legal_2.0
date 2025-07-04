<template>
  <Dialog v-model:visible="visible" modal class="dialog-agregar-audiencia" :closable="false" :draggable="false"
    @hide="emit('close')">
    <template #header>
      <div class="custom-header">
        <span class="dialog-title">Editar Audiencia</span>
        <button class="close-btn" @click="visible = false">
          &times;
        </button>
      </div>
    </template>

    <div class="form-content">
      <div class="field">
          <label class="block mb-2 text-sm font-medium">Nombre de la Audiencia *</label>
        <InputText v-model="numero" class="w-full" placeholder="Nombre de la Nueva Audiencia" />
      </div>

      <div class="field">
        <label class="block mb-2 text-sm font-medium">Modalidad*</label>
         <Dropdown id="tipoAudiencia" v-model="tipo" :options="tiposAudiencia" optionLabel="label"
          optionValue="value" class="w-full" placeholder="Seleccione un tipo" />
      </div>

      <div class="field">
        <label class="block mb-2 font-medium text-sm">Fecha de Audiencia *</label>
        <Calendar v-model="Fecha" dateFormat="yy-mm-dd" showIcon :minDate="hoy" class="w-full"
          :placeholder="placeholderFecha" />
      </div>

      <div class="field">
        <label class="block mb-2 font-medium text-sm">Hora Audiencia *</label>
        <Calendar v-model="horaSeleccionada" showIcon timeOnly hourFormat="12" placeholder="09:00am" class="w-full" />
      </div>

      <div class="field">
        <label class="block mb-2 font-medium text-sm">Tribunal *</label>
        <Dropdown v-model="tribunalSeleccionado" :options="tribunales" optionLabel="nombre_Tribunal"
          optionValue="id_Tribunal" placeholder="Seleccione un tribunal" class="w-full" filter />
      </div>

      <div class="field">
        <label class="block mb-2 font-medium text-sm">Sala *</label>
        <Dropdown v-model="salaId" :options="salasFiltradas" :disabled="!tribunalSeleccionado" optionLabel="nombre"
          optionValue="idSala" placeholder="Seleccione una sala" class="w-full" filter />
      </div>

      <div class="mt-4 text-center">
        <Button label="Actualizar" icon="pi pi-calendar-plus" class="p-button" :loading="uploading" @click="guardar" />
      </div>

      <Notivue v-slot="item">
        <Notifications :item="item" />
      </Notivue>
    </div>
  </Dialog>
</template>

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
    notif.resolve('Audiencia actualizada correctamente');
  } catch (error) {
    console.error('Error al actualizar audiencia:', error);
    notif.reject('Error al actualizar la audiencia');
  }
}

onMounted(async () => {
  await cargarDatosDropdowns();
  await cargarUltimaAudiencia();
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


.custom-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  font-weight: bold;
  padding: 0 0 0 6.5rem;
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
