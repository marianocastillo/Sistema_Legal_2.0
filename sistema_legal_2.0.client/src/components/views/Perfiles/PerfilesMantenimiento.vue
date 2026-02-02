<template>
  <div class="card p-6 shadow-2">
    <!-- 🌀 Loading spinner -->
    <div v-if="loading" class="loading-container">
      <ProgressSpinner strokeWidth="4" />
    </div v-else>

    <div class="flex justify-between items-center mb-4 flex-wrap gap-2">
      <h2 class="text-2xl font-bold">Gestión de Perfiles</h2>

      <!-- <div class="filtro-busqueda-bar">
        <Button icon="pi pi-plus" label="Nuevo Perfil" class="btn-litigio" @click="nuevoPerfil" />
      </div> -->
    </div>

    <table class="table table-bordered table-hover table-sm">
      <thead class="table-light">
        <tr>
          <th>ID</th>
          <th>Nombre</th>
          <th>Descripción</th>
          <th>Permisos</th>
          <th>Usuarios</th>
          <th>Por Defecto</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="perfil in perfiles" :key="perfil.idPerfil">
          <td>{{ perfil.idPerfil }}</td>
          <td>{{ perfil.nombre }}</td>
          <td>{{ perfil.descripcion }}</td>
          <td>{{ perfil.cantPermisos }}</td>
          <td>{{ perfil.cantUsuarios }}</td>
          <td>
            <i v-if="perfil.porDefecto" class="pi pi-check text-green-500"></i>
          </td>
          <td>
            <div class="btn-group">
              <button class="btn btn-sm btn-hover"
                style="background-color: #003870; border-color: #003870; margin-right: 0.3rem;"
                v-tooltip="'Editar Perfil'" @click="abrirEdicion(perfil)">
                <i class="pi pi-pencil white-icon"></i>
              </button>
              <button class="btn btn-sm btn-hover" style="background-color: #003870; border-color: #003870;"
                v-tooltip="'Eliminar Perfil'" @click="confirmarEliminacion(perfil.idPerfil)">
                <i class="pi pi-trash white-icon"></i>
              </button>
            </div>
          </td>
        </tr>
        <tr v-if="perfiles.length === 0">
          <td colspan="7" class="text-center">No se han encontrado perfiles.</td>
        </tr>
      </tbody>
    </table>


    <!-- <Dialog v-model:visible="mostrarDialogo" :header="modoNuevo ? 'Nuevo Perfil' : 'Editar Perfil'" :modal="true"
      :style="{ width: '30vw', height: '30%' }">
      <div class="p-fluid">
        <label>Nombre</label>
        <InputText v-model="perfilSeleccionado.nombre" />

        <label>Descripción</label>
        <InputText v-model="perfilSeleccionado.descripcion" />

        <label>Por defecto</label>
        <InputSwitch v-model="perfilSeleccionado.porDefecto" />

        <label class="mt-3">Vistas</label>
        <br><br>
        <PickList v-model="vistasDisponibles" :sourceHeader="'Disponibles'" :targetHeader="'Asignadas'"
          :dataKey="'idVista'">
          <template #item="{ item }">
            <div>{{ item.nombre }}</div>
          </template>
        </PickList>
      </div>

      <template #footer>
        <Button label="Guardar" icon="pi pi-save" @click="guardarPerfil" />
      </template>
    </Dialog> -->
  </div>

</template>


<script>
import axios from 'axios';
import api from '@/utilities/api';
import { ref, onMounted } from 'vue';
const loading = ref(true);
const datos = ref(null);
import ProgressSpinner from 'primevue/progressspinner';



export default {
  data() {
    return {
      perfiles: [],
      vistas: [],
      vistasDisponibles: [[], []],
      mostrarDialogo: false,
      modoNuevo: false,
      perfilSeleccionado: {},
      loading: true
    };
  },
  components: {
    ProgressSpinner
  },

  methods: {
    async cargarDatos() {
      this.loading = true; // 🟡 comienza carga

      try {
        const [resPerfiles, resVistas] = await Promise.all([
          api.get('/api/Perfiles/GetPerfiles'),
          api.get('/api/Perfiles/GetVistas')
        ]);

        const perfiles = resPerfiles.data;

        // Cargar usuarios por perfil
        const conteos = await Promise.all(
          perfiles.map(async perfil => {
            const res = await api.get(`/api/Perfiles/GetUsuariosPerfiles/${perfil.idPerfil}`);
            return { idPerfil: perfil.idPerfil, cantUsuarios: res.data.length };
          })
        );

        const conteoMap = Object.fromEntries(conteos.map(c => [c.idPerfil, c.cantUsuarios]));

        // Asignar cantidad de usuarios al perfil
        this.perfiles = perfiles.map(p => ({
          ...p,
          cantUsuarios: conteoMap[p.idPerfil] || 0
        }));

        this.vistas = resVistas.data;
      } catch (error) {
        // ya se maneja por interceptor
      } finally {
        this.loading = false;
      }
    }
    ,

    nuevoPerfil() {
      this.perfilSeleccionado = {
        nombre: '',
        descripcion: '',
        porDefecto: false
      };
      this.vistasDisponibles = [this.vistas, []];
      this.mostrarDialogo = true;
      this.modoNuevo = true;
    },

    async guardarPerfil() {
      try {
        const vistasAsignadas = this.vistasDisponibles[1].map(v => ({
          idVista: v.idVista,
          permiso: true
        }));

        const perfilModel = {
          idPerfil: this.perfilSeleccionado.idPerfil || 0,
          nombre: this.perfilSeleccionado.nombre,
          descripcion: this.perfilSeleccionado.descripcion,
          porDefecto: this.perfilSeleccionado.porDefecto || false,
          vistas: vistasAsignadas,
          usuarios: [],
          cantPermisos: vistasAsignadas.length
        };

        const isNuevo = this.modoNuevo;
        const url = isNuevo ? '/api/Perfiles/SavePerfil' : '/api/Perfiles/UpdatePerfil';
        const method = isNuevo ? 'post' : 'put';

        const response = await axios({
          method,
          url,
          data: perfilModel,
          headers: {
            'Content-Type': 'application/json'
          }
        });

        if (response.data.success) {
          this.$toast.add({
            severity: 'success',
            summary: 'Éxito',
            detail: response.data.message || 'Perfil guardado correctamente',
            life: 3000
          });
          this.mostrarDialogo = false;
          await this.cargarDatos();
        } else {
          this.$toast.add({
            severity: 'warn',
            summary: 'Advertencia',
            detail: response.data.message || 'No se pudo guardar el perfil',
            life: 3000
          });
        }
      } catch (error) {
        console.error("Error completo:", error);

        let errorMessage = 'Error en la conexión';
        if (error.response) {
          console.log("Datos del error:", error.response.data);
          errorMessage = error.response.data?.message || `Error ${error.response.status}`;
        }

        this.$toast.add({
          severity: 'error',
          summary: 'Error',
          detail: errorMessage,
          life: 5000
        });
      }
    },
    async abrirEdicion(perfil) {
      this.modoNuevo = false;
      const res = await fetch(`/api/Perfiles/${perfil.idPerfil}`);
      const perfilCompleto = await res.json();

      const vistasPerfil = await fetch(`/api/Perfiles/GetPermisos/${perfil.idPerfil}`).then(r => r.json());
      this.perfilSeleccionado = { ...perfilCompleto };

      const vistasAsignadas = vistasPerfil.filter(v => v.permiso);
      const vistasNoAsignadas = vistasPerfil.filter(v => !v.permiso);

      this.vistasDisponibles = [vistasNoAsignadas, vistasAsignadas];
      this.mostrarDialogo = true;
    },
    async confirmarEliminacion(idPerfil) {
      try {
        // Consultar cuántos usuarios tiene el perfil
        const res = await axios.get(`/api/Perfiles/GetUsuariosPerfiles/${idPerfil}`);
        const cantidad = res.data?.length || 0;

        if (cantidad > 0) {
          this.$toast.add({
            severity: 'warn',
            summary: 'No permitido',
            detail: `Este perfil tiene ${cantidad} usuario(s) asignado(s) y no puede eliminarse.`,
            life: 4000
          });
          return;
        }

        // Confirmación de eliminación
        this.$confirm.require({
          message: '¿Estás seguro de eliminar este perfil?',
          header: 'Confirmar Eliminación',
          icon: 'pi pi-exclamation-triangle',
          acceptClass: 'p-button-danger',
          acceptLabel: 'Sí',
          rejectLabel: 'No',
          accept: async () => {
            try {
              const delRes = await axios.delete(`/api/Perfiles/DeletePerfil/${idPerfil}`);
              if (delRes.data.success) {
                this.$toast.add({
                  severity: 'success',
                  summary: 'Eliminado',
                  detail: delRes.data.message || 'Perfil eliminado correctamente',
                  life: 3000
                });
                await this.cargarDatos();
              } else {
                this.$toast.add({
                  severity: 'warn',
                  summary: 'Advertencia',
                  detail: delRes.data.message || 'No se pudo eliminar el perfil',
                  life: 3000
                });
              }
            } catch (error) {
              console.error("Error al eliminar perfil:", error);
              this.$toast.add({
                severity: 'error',
                summary: 'Error',
                detail: 'Ocurrió un error al eliminar el perfil',
                life: 3000
              });
            }
          }
        });
      } catch (error) {
        console.error("Error al consultar usuarios del perfil:", error);
        this.$toast.add({
          severity: 'error',
          summary: 'Error',
          detail: 'No se pudo verificar si el perfil tiene usuarios',
          life: 3000
        });
      }
    },

  },



  mounted() {
    this.cargarDatos();
  }
};

</script>

<style scoped>
.filtro-busqueda-bar {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: nowrap;
  margin-left: auto;
}

.loading-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 200px;
}

.mt-3 {
  margin-top: 2rem;
}

/* === Tabla HTML === */
.table thead th {
  background-color: rgb(241, 242, 250);
  font-weight: 600;
  color: #2e3842;
  font-size: 0.95rem;
  border: none !important;
}

.table tbody td {
  border-right: 1px solid #ebebeb;
  border-top: none !important;
  border-bottom: none !important;
  border-left: none !important;
}

/* === Botones de acción === */
.btn-group .btn {
  margin: 0 2px;
}

.white-icon {
  color: white !important;
}

/* === Responsivo (opcional, como en el otro) === */
@media (max-width: 768px) {
  .filtro-busqueda-bar {
    flex-direction: column;
    align-items: flex-end;
    width: 100%;
    margin-left: 0;
    gap: 0.5rem;
  }
}
</style>

