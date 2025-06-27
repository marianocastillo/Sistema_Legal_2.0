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
import { push } from 'notivue'
import axios from 'axios';
import { ref, onMounted } from 'vue'

const emit = defineEmits(['close', 'actualizar']);
const Fecha = ref('');
const horaSeleccionada = ref(null); // ✅ Esta es la que faltaba
const tribunales = ref([]);
const numero = ref('');
const tipo = ref('');
const props = defineProps({
  id_Ltg: {
    type: Number,
    required: true
  }
});

const form = ref({
  id_Tribunal: 2
});



const cargarDatosDropdowns = async () => {
  try {
    const response = await fetch('/api/Litigio/datos-litigio')
    const data = await response.json()
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

  console.log('Audiencia creada:', response.data);

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
