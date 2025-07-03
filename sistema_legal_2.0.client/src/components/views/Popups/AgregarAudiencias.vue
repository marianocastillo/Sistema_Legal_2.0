<template>
  <Dialog v-model:visible="visible" modal header="Agregar Nueva Audiencia" class="dialog-agregar-audiencia"
    :closable="true" :draggable="false">
    <div class="form-content">
      <div class="field">
        <InputText v-model="numero" class="w-full" placeholder="Nombre de la Nueva Audiencia" />
      </div>

      <div class="field">
        <InputText v-model="tipo" class="w-full" placeholder="Tipo de audiencia" />
      </div>

      <div class="field">
        <label class="block mb-2 text-sm font-medium">Fecha de Audiencia *</label>
        <Calendar v-model="Fecha" dateFormat="yy-mm-dd" showIcon :minDate="hoy" class="w-full"
          placeholder="Seleccione una fecha" />
      </div>

      <div class="field">
        <label class="block mb-2 text-sm font-medium">Hora Audiencia *</label>
        <Calendar v-model="horaSeleccionada" showIcon timeOnly hourFormat="12"
          placeholder="Ej: 8:00am" class="w-full" />
      </div>

      <div class="field">
        <label class="block mb-2 text-sm font-medium">Tribunal *</label>
        <Dropdown v-model="tribunalSeleccionado" :options="tribunales" optionLabel="nombre_Tribunal"
          optionValue="id_Tribunal" placeholder="Seleccione un tribunal" class="w-full" filter />
      </div>

      <div class="field">
        <label class="block mb-2 text-sm font-medium">Sala *</label>
        <Dropdown v-model="salaId" :options="salasFiltradas" :disabled="!tribunalSeleccionado"
          optionLabel="nombre" optionValue="idSala" placeholder="Seleccione una sala" class="w-full" filter />
      </div>

      <div class="mt-4 text-end">
        <Button label="Guardar" icon="pi pi-check" class="p-button-sm" @click="guardar" />
      </div>
    </div>
  </Dialog>
</template>

<script setup>
import { push } from 'notivue'
import axios from 'axios';
import { ref, onMounted, computed, watch } from 'vue'

const emit = defineEmits(['close', 'actualizar']);
const props = defineProps({
  id_Ltg: { type: Number, required: true }
});

const visible = ref(true);
const numero = ref('');
const tipo = ref('');
const Fecha = ref(null);
const horaSeleccionada = ref(null);
const hoy = new Date();

const tribunalSeleccionado = ref(null);
const salaId = ref(null);
const tribunales = ref([]);
const todasLasSalas = ref([]);

// Computed: filtrar salas del tribunal seleccionado
const salasFiltradas = computed(() => {
  return tribunalSeleccionado.value
    ? todasLasSalas.value.filter(s => s.idTribunal === tribunalSeleccionado.value)
    : [];
});

// Limpiar sala si cambia el tribunal
watch(tribunalSeleccionado, () => {
  salaId.value = null;
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
  const fechaCompleta = combinarFechaYHora(Fecha.value, horaSeleccionada.value);
  if (!fechaCompleta) return push.warning("Debe seleccionar fecha y hora.");
  if (!salaId.value) return push.warning("Debe seleccionar una sala.");

  const body = {
    idLitigio: props.id_Ltg,
    Numero: numero.value.trim(),
    Tipo: tipo.value.trim(),
    Fecha: fechaCompleta,
    SalaId: salaId.value
  };

  const notif = push.promise('Agregando audiencia...');
  try {
    await axios.post('/api/Files/crearAudiencias', body);
    emit('actualizar');
    emit('close');
    notif.resolve('Audiencia creada correctamente');
  } catch (error) {
    console.error('Error al crear audiencia:', error);
    notif.reject('Error al crear la audiencia');
  }
}

onMounted(async () => {
  await cargarDatosDropdowns();
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
</style>
