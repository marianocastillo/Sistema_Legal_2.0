<template>
  <div class="pop-up" >
    <div class="pop-up-inner" >
      <span class="pop-up-close" @click="$emit('close')">&times;</span>

      <h2>Agregar evidencia y comentario</h2>

      <!-- Usamos el componente FileUpload -->
      <div class="file-container">
        <FileUpload
          name="Archivo"
          customUpload
          @select="handleExpedienteUpload"
          mode="basic"
          chooseLabel="Elegir archivo"
          class="w-full md:w-20rem"
        />
      </div>

      <!-- Textarea para el comentario -->
      <div class="comment-container">
        <textarea
          v-model="Comentario"
          placeholder="Descripción del documento"
          rows="8"
          cols="50"
        ></textarea>
      </div>

      <Button class="p-button-sm p-button-text-dark" @click="guardar">Guardar</button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import FileUpload from 'primevue/fileupload';
import axios from 'axios'; // Importamos axios


const emit = defineEmits(['close', 'actualizar']);


const archivo = ref(null);
const Comentario = ref('');
const IdUsuario = 1;
const props = defineProps({
  id_Ltg: {
    type: Number,
    required: true
  }
});
function handleExpedienteUpload(event) {
  archivo.value = event.files[0];  // Guardamos el archivo en la referencia
}

async function guardar() {
  if (!archivo.value || !Comentario.value) {
    alert('Por favor, complete todos los campos.');
    return;
  }

  // Creamos el objeto FormData para enviar el archivo y el comentario
  const formData = new FormData();
  formData.append('Archivo', archivo.value);
  formData.append('Comentario', Comentario.value);
  formData.append('IdUsuario', IdUsuario);
   formData.append('IdLitigio', props.id_Ltg); // <- ¡Ahora dinámico!


  try {
  const response = await axios.post(`/api/Files/subir-evidencia-comentario`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });

   alert('Evidencia guardada correctamente');

    emit('actualizar'); // 🔄 Emitir para que el padre actualice la información
    emit('close');
  console.log('Archivo subido:', response.data);

} catch (error) {
  console.error('Error al subir el archivo:', error.response ? error.response.data : error.message);
  alert('Hubo un error al subir el archivo y el comentario.');
}
}
</script>

<style scoped>
.pop-up {
  width: 600px;
  height: 400px;
  background-color: rgba(0,0,0,0.5); /* fondo semitransparente */
  display: flex;
  justify-content: center;
  align-items: center;
}

.pop-up-content {
  background: white;
  padding: 20px;
  width: 600px; /* <-- Este valor controla el ancho */
  height: 400px; /* <-- Este valor controla la altura */
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
  max-width: 500px;
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

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
