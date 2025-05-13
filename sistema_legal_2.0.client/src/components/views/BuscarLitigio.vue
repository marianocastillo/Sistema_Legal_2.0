<template>
  <div class="card p-4 shadow-2">
    <div class="flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
      <h2 class="text-xl font-bold">Buscar Litigio</h2>
      <router-link to="/drawer/home" class="btn text-white px-3 py-2" style="background-color: #003870;">
        <i class="fa-solid fa-home me-2"></i> Inicio
      </router-link>
    </div>

    <!-- Formulario de búsqueda con input y botón alineados -->
    <div class="flex flex-wrap gap-2 align-items-end mb-3">
      <div>
        <label for="busqueda" class="block font-medium text-sm mb-2">Cédula o No. Acto</label>
        <InputText
          id="busqueda"
          v-model="busqueda"
          class="w-full md:w-14rem"
          placeholder="Buscar por cédula o acto"
        />
      </div>
      <div>
        <Button
          label="Buscar"
          icon="pi pi-search"
          class="p-button-primary mt-2 md:mt-0"
          @click="buscarLitigio"
        />
      </div>
    </div>

    <!-- Resultado -->
    <div v-if="resultado">
      <div class="mt-4 border-1 border-round p-3">
        <h3 class="text-lg font-semibold mb-2">Resultado encontrado:</h3>
        <p><strong>No. Acto:</strong> {{ resultado.ltg_acto }}</p>
        <p><strong>Cédula Demandante:</strong> {{ resultado.ltg_Cedula_Demandante }}</p>
        <p><strong>Demandante:</strong> {{ resultado.ltg_Demandante }}</p>

        <Button
          label="Modificar"
          icon="pi pi-pencil"
          class="mt-3 p-button-warning"
          @click="irAModificar"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import { push } from 'notivue'

const busqueda = ref('')
const resultado = ref(null)
const router = useRouter()

const buscarLitigio = async () => {
  if (!busqueda.value.trim()) {
    push.warning('Por favor, ingrese un número de acto o cédula.')
    return
  }

  try {
    const response = await fetch(`/api/Litigio/Buscar?valor=${busqueda.value.trim()}`)
    const data = await response.json()

    if (data) {
      resultado.value = data
      push.success('Litigio encontrado exitosamente.')
    } else {
      resultado.value = null
      push.warning('No se encontró ningún litigio con esos datos.')
    }
  } catch (error) {
    console.error('Error al buscar litigio:', error)
    push.error('Ocurrió un error al realizar la búsqueda.')
  }
}

const irAModificar = () => {
  localStorage.setItem('litigioModificacion', JSON.stringify(resultado.value))
  router.push('/drawer/modificar')
}
</script>

<style scoped>
label {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  font-weight: 600;
  color: #003870;
  font-size: 1rem;
  letter-spacing: 0.5px;
}
</style>
