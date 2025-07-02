<template>
  <Dialog v-model:visible="visible" modal class="dialog-agregar-audiencia" header="Editar Audiencia" :closable="true"
    :draggable="false" :style="{ width: '500px' }">
    <div class="form-content">
      <div class="field">
        <InputText v-model="numero" class="w-full" placeholder="Nombre de la Nueva Audiencia" />
      </div>

      <div class="field">
        <InputText v-model="tipo" class="w-full" placeholder="Tipo de audiencia" />
      </div>

      <div class="field">
        <label class="block mb-2 font-medium text-sm">Fecha de Audiencia *</label>
        <Calendar v-model="Fecha" dateFormat="yy-mm-dd" showIcon :minDate="hoy" class="w-full"
          :placeholder="placeholderFecha" />
      </div>

      <div class="field">
        <label class="block mb-2 font-medium text-sm">Hora Audiencia *</label>
        <Calendar v-model="horaSeleccionada" showIcon timeOnly hourFormat="12" :placeholder="'09:00am'"
          class="w-full" />
      </div>


      <div class="field">
        <label class="block mb-2 font-medium text-sm">Tribunal *</label>
        <Dropdown v-model="id_Tribunal" :options="tribunales" optionLabel="nombre_Tribunal" optionValue="id_Tribunal"
          placeholder="Seleccione un tribunal" class="w-full" filter />
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
import { ref, onMounted } from 'vue';
import axios from 'axios';
import Dialog from 'primevue/dialog';
import { push } from 'notivue';

const emit = defineEmits(['close', 'actualizar']);

const props = defineProps({
  id_Ltg: {
    type: Number,
    required: true
  }
});

const hoy = new Date();
const manana = new Date();
manana.setDate(hoy.getDate() + 1);

const placeholderFecha = manana.toISOString().split('T')[0]; // "YYYY-MM-DD"

const horaPorDefecto = new Date();
horaPorDefecto.setDate(hoy.getDate() + 1);
horaPorDefecto.setHours(9, 0, 0, 0);

// Asignar valores por defecto si no se carga una audiencia existente
onMounted(async () => {
  await Promise.all([
    cargarDatosDropdowns(),
    cargarUltimaAudiencia()
  ]);

  if (!Fecha.value) Fecha.value = new Date(manana); // mañana
  if (!horaSeleccionada.value) horaSeleccionada.value = horaPorDefecto;
});


const numero = ref('');
const tipo = ref('');
const Fecha = ref(null);
const horaSeleccionada = ref(null);
const id_Tribunal = ref(null);
const visible = ref(true);
const tribunales = ref([]);



// 🔹 Cargar dropdown de tribunales
const cargarDatosDropdowns = async () => {
  try {
    const response = await fetch('/api/Litigio/datos-litigio');
    const data = await response.json();
    tribunales.value = data.tribunales;
  } catch (error) {
    console.error('Error al cargar los datos de los dropdowns:', error);
    push.error('No se pudieron cargar los tribunales.');
  }
};

// 🔹 Dividir fecha completa en fecha + hora
function separarFechaYHora(fechaCompletaStr) {
  const fecha = new Date(fechaCompletaStr);
  if (isNaN(fecha)) return { fecha: null, hora: null };
  return {
    fecha,
    hora: new Date(fecha.getFullYear(), fecha.getMonth(), fecha.getDate(), fecha.getHours(), fecha.getMinutes())
  };
}

// 🔹 Cargar última audiencia
const cargarUltimaAudiencia = async () => {
  try {
    const response = await axios.get(`/api/Files/ultima-Audiencia/${props.id_Ltg}`);
    const datos = response.data.data;

    numero.value = datos.NumeroAudiencia || '';
    tipo.value = datos.TipoAudiencia || '';
    id_Tribunal.value = datos.Id_Tribunal || null;

    const { fecha, hora } = separarFechaYHora(datos.FechaAudiencia);

    // Asignar valores si existen
    Fecha.value = fecha;
    horaSeleccionada.value = hora;

    // Si son nulos, asignar por defecto mañana a las 9:00am
    if (!Fecha.value) {
      const f = new Date();
      f.setDate(f.getDate() + 1);
      f.setHours(0, 0, 0, 0);
      Fecha.value = f;
    }

    if (!horaSeleccionada.value) {
      const h = new Date();
      h.setDate(h.getDate() + 1);
      h.setHours(9, 0, 0, 0);
      horaSeleccionada.value = h;
    }

  } catch (error) {
    console.error('Error al cargar última audiencia:', error);
    push.error('No se pudo cargar la última audiencia.');
  }
};

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

  if (!id_Tribunal.value) {
    push.warning("Debe seleccionar un tribunal.");
    return;
  }

  const body = {
    IdLitigio: props.id_Ltg,
    Numero: numero.value?.trim(),
    Tipo: tipo.value?.trim(),
    id_tribunal: id_Tribunal.value,
    fecha: fechaCompletaAudiencia

  };

  const notif = push.promise('Actualizando audiencia...');

  try {
    await new Promise(resolve => setTimeout(resolve, 300)); // simula carga opcional
    await axios.put('/api/Files/actualizarAudiencias', body);

    emit('actualizar');
    emit('close');
    notif.resolve('Audiencia actualizada correctamente');
    window.location.reload();

  } catch (error) {
    console.error('Error al actualizar audiencia:', error.response?.data || error.message);
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
</style>
