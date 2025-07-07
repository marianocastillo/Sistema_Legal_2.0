<script setup>
import { ref, onMounted } from 'vue'
import { Qalendar } from 'qalendar'
import 'qalendar/dist/style.css'
import axios from 'axios'
import dayjs from 'dayjs'
import { useRouter } from 'vue-router'

const router = useRouter()
const eventos = ref([])

onMounted(async () => {
  try {
    const { data } = await axios.get('/api/Files/AudienciasHistorial')

    eventos.value = data.map(a => {
      const start = dayjs(a.fechaAudiencia).format('YYYY-MM-DD HH:mm')
      const end = dayjs(a.fechaAudiencia).add(2, 'hour').format('YYYY-MM-DD HH:mm')

      return {
        title: `${a.ltg_acto} ${a.numeroAudiencia}`,
        with: a.ltg_Nombre_Demandante,
        time: { start, end },
        description: `Tipo: (${a.tipoAudiencia})
Sala: ${a.nombreSala} | Tribunal: ${a.nombre_Tribunal} | ${a.tipoDemanda}`,
        eventType: a.tipoAudiencia?.toLowerCase().includes('virtual') ? 'virtual' : 'presencial',
        actions: [
          {
            icon: 'pi pi-eye',
            label: 'Ver Detalle',
            onClick: () => {
              router.push(`/litigio/detalle/${a.id_Ltg}`)
            }
          }
        ]
      }
    })
  } catch (error) {
    console.error('Error al cargar audiencias:', error)
  }
})
</script>

<template>
  <Qalendar
    :events="eventos"
    :config="{
      defaultMode: 'week',
      locale: 'es',
      week: { startsOn: 'monday' },
      isDark: false,
      eventTypes: [
        { name: 'virtual', color: '#10b981' },     // verde
        { name: 'presencial', color: '#3b82f6' }   // azul
      ]
    }"
  />
</template>


<style scoped>
.qalendar__event {
  position: relative;
  padding-right: 2rem !important; /* espacio para el ícono */
}

/* Botón flotante de "ver detalle" */
.qalendar__event .qalendar__event-action-button {
  position: absolute;
  top: 4px;
  right: 4px;
  background: white;
  border-radius: 50%;
  padding: 0.2rem;
  font-size: 0.8rem;
  cursor: pointer;
  z-index: 2;
  box-shadow: 0 1px 3px rgba(0,0,0,0.2);
  color: #333;
  transition: transform 0.1s ease-in-out;
}

.qalendar__event .qalendar__event-action-button:hover {
  transform: scale(1.1);
  background: #f3f4f6;
}

/* Ajustar tamaño de texto del evento */
.qalendar-event__title,
.qalendar-event__with,
.qalendar-event__description {
  font-size: 1rem !important;
  line-height: 1.4;
}
</style>

