<template>
 <Dialog v-model:visible="visible" modal header="Agregar Nueva Audiencia" class="dialog-agregar-audiencia" :closable="true" :draggable="false">
  <div class="form-content">
    <div class="field">
      <InputText v-model="numero" class="w-full" placeholder="Nombre de la Nueva Audiencia" />
    </div>

    <div class="field">
      <label for="tipoAudiencia" class="block mb-2 font-medium text-sm">Tipo de Audiencia *</label>
       <Dropdown id="tipoAudiencia" v-model="tipo" :options="tiposAudiencia" optionLabel="label"
                optionValue="value" class="w-full" placeholder="Seleccione un tipo" />
    </div>

    <div class="field">
      <label class="block mb-2 text-sm font-medium">Fecha de Audiencia *</label>
      <Calendar v-model="Fecha" dateFormat="yy-mm-dd" showIcon placeholder="Seleccione una fecha" :minDate="hoy" class="w-full" />
    </div>

    <div class="field">
      <label class="block mb-2 text-sm font-medium">Hora Audiencia *</label>
      <Calendar v-model="horaSeleccionada" showIcon timeOnly hourFormat="12" placeholder="Ej: 8:00am" class="w-full" />
    </div>

    <div class="field">
      <label class="block mb-2 text-sm font-medium">Tribunal *</label>
      <Dropdown v-model="form.id_Tribunal" :options="tribunales" optionLabel="nombre_Tribunal" optionValue="id_Tribunal" class="w-full" placeholder="Seleccione un tribunal" filter />
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
import { ref, onMounted } from 'vue'
import Dialog from 'primevue/dialog'

const emit = defineEmits(['close', 'actualizar']);
const Fecha = ref('');
const visible = ref(true)
const horaSeleccionada = ref(null);
const tribunales = ref([]);
const numero = ref('');
const tipo = ref('');

const props = defineProps({
  id_Ltg: {
    type: Number,
    required: true
  }
});

const tiposAudiencia = [
  { label: 'Presencial ', value: 'Presencial' },
  { label: 'Virtual', value: 'Virtual' },
]

const form = ref({
  id_Tribunal: ''
});

const cargarDatosDropdowns = async () => {
  try {
    const response = await fetch('/api/Litigio/datos-litigio')
    const data = await response.json()
    // console.log(data);
    tribunales.value = data.tribunales
  } catch (error) {
    console.error('Error al cargar los datos de los dropdowns:', error)
  }
}

function combinarFechaYHora(fecha, hora) {
  if (!fecha || !hora) return null;

  const fechaObj = new Date(fecha);
  const horaObj = new Date(hora);

  // Combina la fecha y la hora
  fechaObj.setHours(horaObj.getHours());
  fechaObj.setMinutes(horaObj.getMinutes());
  fechaObj.setSeconds(0);
  fechaObj.setMilliseconds(0);

  return fechaObj;
}

async function guardar() {
  const fechaCompletaAudiencia = combinarFechaYHora(Fecha.value, horaSeleccionada.value);

  if (!(fechaCompletaAudiencia instanceof Date) || isNaN(fechaCompletaAudiencia.getTime())) {
    push.warning("Debe seleccionar una hora válida para la audiencia.");
    return;
  }

  if (!form.value.id_Tribunal) {
    push.warning("Debe seleccionar un tribunal.");
    return;
  }

  const body = {
    idLitigio: props.id_Ltg,
    idTribunal: form.value.id_Tribunal,
    numero: numero.value?.trim(),
    tipo: tipo.value?.trim(),
    fecha: fechaCompletaAudiencia.toISOString()
  };


  const notif = push.promise('Agregando Audiencia...');

  try {
    await new Promise(resolve => setTimeout(resolve, 300));

    const response = await axios.post('/api/Files/crearAudiencias', body);

    // console.log('Audiencia creada:', response.data);

    emit('actualizar');
    emit('close');
    notif.resolve('Audiencia creada correctamente');
  } catch (error) {
    console.error('Error al crear audiencia:', error.response?.data || error.message);
    notif.reject('Error al crear la audiencia');
  }
}

onMounted(async () => {
  try {
    await cargarDatosDropdowns();
  } catch (error) {
    console.error('Error en onMounted al cargar datos de dropdowns:', error);
    push.error('No se pudo cargar la información necesaria. Verifique su conexión.');
  }
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
