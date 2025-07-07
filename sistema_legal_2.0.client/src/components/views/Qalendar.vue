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
  // Definir la función global para el botón HTML
  window.__goToDetalle = (id) => {
    router.push(`/litigio/detalle/${id}`)
  }

  try {
    const { data } = await axios.get('/api/Files/AudienciasHistorial')

    eventos.value = data.map(a => {
      const start = dayjs(a.fechaAudiencia).format('YYYY-MM-DD HH:mm')
      const end = dayjs(a.fechaAudiencia).add(2, 'hour').format('YYYY-MM-DD HH:mm')

      return {
        title: `${a.ltg_acto} ${a.numeroAudiencia}`,
        with: a.ltg_Nombre_Demandante,
        time: { start, end },
        description: `
  Tipo: (${a.tipoAudiencia})<br />
  Sala: ${a.nombreSala} | Tribunal: ${a.nombre_Tribunal} | ${a.tipoDemanda}<br /><br />
  <button class="ver-detalle-btn" onclick="window.__goToDetalle(${a.id_Ltg})">🔍 Ver Detalle</button>
`,
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
      eventDialog: true,
      showEventActions: true,
      eventTypes: [
        { name: 'virtual', color: '#10b981' },
        { name: 'presencial', color: '#3b82f6' }
      ]
    }"
  />
</template>

<style scoped>
.qalendar__event {
  position: relative;
  padding-right: 2rem !important;
}

/* Estilo del botón dentro del diálogo */
.ver-detalle-btn {
  background-color: #003870;
  color: white;
  border: none;
  padding: 6px 12px;
  font-size: 0.85rem;
  border-radius: 5px;
  cursor: pointer;
  margin-top: 8px;
}

.ver-detalle-btn:hover {
  background-color: #0050aa;
}

/* Ícono flotante del evento en el calendario */
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
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
  color: #333;
  transition: transform 0.1s ease-in-out;
}

.qalendar__event .qalendar__event-action-button:hover {
  transform: scale(1.1);
  background: #f3f4f6;
}

/* Texto del evento */
.qalendar-event__title,
.qalendar-event__with,
.qalendar-event__description {
  font-size: 1rem !important;
  line-height: 1.4;
}

:deep(.ver-detalle-btn) {
  background-color: #003870;
  color: white;
  border: none;
  padding: 6px 12px;
  font-size: 0.9rem;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s ease;
  margin-top: 8px;
  display: inline-block;
}

:deep(.ver-detalle-btn:hover) {
  background-color: #0050aa;
}

</style>
