<template>
  <div class="pop-up">
    <div class="pop-up-inner">
      <span class="pop-up-close" @click="$emit('close')">&times;</span>

      <h2>Agregar evidencia y comentario</h2>
      <br>
      <div class=" ms-3 file-container">
        <FileUpload name="Archivo" customUpload @select="handleExpedienteUpload" mode="basic"
          chooseLabel="Elegir archivo" class="w-full md:w-19rem" style="background-color: #003870;" />
      </div>

      <div class="comment-container">
        <textarea v-model="NombreEvidencia" placeholder="Nombre de la evidencia" rows="1" cols="1"></textarea>
        <br>
        <textarea v-model="Comentario" placeholder="Escribe tu comentario aquí..." rows="8" cols="50"
          maxlength="2000" class="texAreaComentario"></textarea>
        <p>{{ Comentario.length }}/2000 caracteres</p>
      </div>

      <Notivue v-slot="item">
        <Notifications :item="item" />
      </Notivue>


      <Button label=" Subir Archivo" class="block mx-auto" icon="pi pi-upload" :loading="uploading" @click="guardar" />

    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import FileUpload from 'primevue/fileupload';
import { push } from 'notivue'
import axios from 'axios';


const uploading = ref(false);
const emit = defineEmits(['close', 'actualizar']);


const archivo = ref(null);
const Comentario = ref('');
const NombreEvidencia = ref('');
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
    push.error('Debes seleccionar un archivo');
    return;
  }

  const nombreAuto = NombreEvidencia.value?.trim() || getNombreSinExtension(archivo.value.name);
  const comentarioAuto = Comentario.value?.trim() || 'Documento subido sin descripción.';

  const formData = new FormData();
  formData.append('Archivo', archivo.value);
  formData.append('Comentario', comentarioAuto);
  formData.append('IdUsuario', IdUsuario);
  formData.append('NombreEvidencia', nombreAuto);
  formData.append('IdLitigio', props.id_Ltg);

  // Notificación tipo promesa
  const notif = push.promise('Subiendo archivo...');

  try {

    await new Promise(resolve => setTimeout(resolve, 500));
    const response = await axios.post(`/api/Files/subir-evidencia`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    // console.log('Archivo subido:', response.data);

    emit('actualizar');
    emit('close');

    notif.resolve('Archivo subido correctamente');
    // window.location.reload();
  } catch (error) {
    console.error('Error al subir el archivo:', error.response?.data || error.message);
    notif.reject('Error al subir el archivo');
  }
}


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

.texAreaComentario{
  white-space: pre-wrap;
   word-wrap: break-word;
   resize: vertical;
   width: 100%;
   padding: 10px;
   font-size: 16px;
   border: 1px solid #ccc;
   border-radius: 5px;
}

.file-container,
.comment-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 15px;
  width: 100%;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
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
