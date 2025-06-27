<template>
  <div class="pop-up">
    <div class="pop-up-inner">
      <span class="pop-up-close" @click="$emit('close')">&times;</span>

      <h2>Agregar Nueva Audiencia</h2>
      <br>

      <div class="comment-container">
        <textarea v-model="numero" placeholder="Nombre de la Nueva Audiencia" rows="1" cols="1"></textarea>
        <br>
        <textarea v-model="tipo" placeholder="Tipo de audiencia" rows="1" cols="1"></textarea>
        <label for="fechaAudiencia" class="block mb-2 font-medium text-sm">Fecha de Audiencia *</label>
        <Calendar v-model="Fecha" dateFormat="yy-mm-dd" showIcon :minDate="hoy" class="w-full"
          placeholder="Seleccione una fecha" :panelStyle="{ zIndex: 99999 }" />

        <label for="fechaAudiencia" class="block mb-2 font-medium text-sm">Hora Audiencia *</label>
        <Calendar v-model="horaSeleccionada" showIcon timeOnly hourFormat="12" placeholder="Ej: 8:00am"
          :panelStyle="{ zIndex: 99999 }" />

        <div class="field col-12 md:col-4">
          <label for="tribunal" class="block mb-2 font-medium text-sm">Tribunal *</label>
          <Dropdown id="tribunal" v-model="id_Tribunal" :options="tribunales" optionLabel="nombre_Tribunal"
            optionValue="id_Tribunal" placeholder="Seleccione un tribunal" class="w-full" filter />
        </div>

      </div>

      <Notivue v-slot="item">
        <Notifications :item="item" />
      </Notivue>


      <Button label=" Subir Archivo" class="block mx-auto" icon="pi pi-calendar-plus" :loading="uploading" @click="guardar" />

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { push } from 'notivue';

const emit = defineEmits(['close', 'actualizar']);

const props = defineProps({
  id_Ltg: {
    type: Number,
    required: true
  }
});

const numero = ref('');
const tipo = ref('');
const Fecha = ref(null);
const horaSeleccionada = ref(null);
const id_Tribunal = ref(null);
const tribunales = ref([]);

const hoy = new Date(); // Para el minDate

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

    console.log('🟢 Datos de la última audiencia:', datos);

    numero.value = datos.NumeroAudiencia;
    tipo.value = datos.TipoAudiencia;

    const { fecha, hora } = separarFechaYHora(datos.FechaAudiencia);
    Fecha.value = fecha;
    horaSeleccionada.value = hora;

    id_Tribunal.value = datos.Id_Tribunal;
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
  } catch (error) {
    console.error('Error al actualizar audiencia:', error.response?.data || error.message);
    notif.reject('Error al actualizar la audiencia');
  }
}


onMounted(async () => {
  await Promise.all([
    cargarDatosDropdowns(),
    cargarUltimaAudiencia()
  ]);
});
</script>



<style scoped>
.ms-custom {
  margin-left: 4.3rem;
  /* o lo que necesites */
}

.pop-up {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
  /* Opcional para asegurarte de que esté al frente */
}

.pop-up-content {
  background: white;
  padding: 20px;
  width: 600px;
  height: 400px;
  border-radius: 8px;
}

.pop-up-close {
  position: absolute;
  top: 8px;
  right: 12px;
  font-size: 3rem;
  color: #333;
  cursor: pointer;
}

.pop-up-inner {
  background: white;
  color: black;
  padding: 30px;
  border-radius: 10px;
  width: 90%;
  max-width: 600px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
  position: relative;
}

.file-container,
.comment-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 15px;
}

textarea {
  width: 100%;
  max-width: 400px;
  padding: 10px;
  font-size: 1rem;
  border: 1px solid #ccc;
  border-radius: 5px;
  resize: none;
  box-sizing: border-box;
}

textarea::placeholder {
  color: #888;
}

::v-deep(.p-datepicker) {
  z-index: 99999 !important;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
