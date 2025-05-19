<template>
  <div class="pop-up" >
    <div class="pop-up-inner" >
      <span class="pop-up-close" @click="$emit('close')">&times;</span>

      <h2>Agregar evidencia y comentario</h2>
<br>
      <div class=" ms-3 file-container">
        <FileUpload
          name="Archivo"
          customUpload
          @select="handleExpedienteUpload"
          mode="basic"
          chooseLabel="Elegir archivo"
          class="w-full md:w-19rem"
        />
      </div>

      <div class="comment-container">
  <textarea
          v-model="NombreEvidencia"
          placeholder="Nombre de la evidencia"
          rows="1"
          cols="1"
        ></textarea>
        <br>
        <textarea
          v-model="Comentario"
          placeholder="Descripción del documento"
          rows="8"
          cols="50"
        ></textarea>
      </div>

      <Button class="p-button-sm p-button-text-dark ms-3 " @click="guardar">Guardar</button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import FileUpload from 'primevue/fileupload';
import axios from 'axios';

const emit = defineEmits(['close', 'actualizar']);


const archivo = ref(null);
const Comentario = ref('');
const NombreEvidencia = ref ('');
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

function getNombreSinExtension(nombre) {
  return nombre.replace(/\.[^/.]+$/, '');
}

async function guardar() {
  if (!archivo.value) {
    alert('Por favor, complete todos los campos.');
    return;
  }
const nombreAuto = NombreEvidencia.value?.trim() || getNombreSinExtension(archivo.value.name);

  // Si no se llenó el comentario, usar un texto genérico
  const comentarioAuto = Comentario.value?.trim() || 'Documento subido sin descripción.';



  const formData = new FormData();
  formData.append('Archivo', archivo.value);
  formData.append('Comentario', comentarioAuto);
  formData.append('IdUsuario', IdUsuario);
  formData.append('Nombre',nombreAuto)

   formData.append('IdLitigio', props.id_Ltg);

for (let [key, value] of formData.entries()) {
  console.log(`${key}: ${value}`);
}

  try {
  const response = await axios.post(`/api/Files/subir-evidencia-comentario`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });

   alert('Evidencia guardada correctamente');

    emit('actualizar');
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
  background-color: rgba(0,0,0,0.5);
  display: flex;
  justify-content: center;
  align-items: center;
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

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
