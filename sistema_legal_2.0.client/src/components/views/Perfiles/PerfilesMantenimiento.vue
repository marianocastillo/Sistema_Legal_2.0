<template>
  <div class="card">
    <h2>Gestión de Perfiles</h2>

    <Button label="Nuevo Perfil" icon="pi pi-plus" class="mb-3" @click="nuevoPerfil" />

    <DataTable :value="perfiles" dataKey="idPerfil">
      <Column field="idPerfil" header="ID" />
      <Column field="nombre" header="nombre" />
      <Column field="descripcion" header="Descripción" />
      <Column field="cantPermisos" header="Permisos" />
      <Column field="cantUsuarios" header="Usuarios" />
      <Column field="porDefecto" header="Por Defecto">
        <template #body="{ data }">
          <i v-if="data.porDefecto" class="pi pi-check text-green-500"></i>
        </template>
      </Column>
      <Column header="Acciones">
        <template #body="{ data }">
          <Button icon="pi pi-pencil" label="Editar" class="p-button-sm mr-2" @click="abrirEdicion(data)" />
          <Button icon="pi pi-trash" label="Eliminar" class="p-button-sm p-button-danger"
            @click="confirmarEliminacion(data.idPerfil)" />
        </template>
      </Column>
    </DataTable>

    <Dialog v-model:visible="mostrarDialogo" :header="modoNuevo ? 'Nuevo Perfil' : 'Editar Perfil'" :modal="true"
      :style="{ width: '50vw' }">
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
    </Dialog>
  </div>
</template>


<script>
import axios from 'axios';
import api from '@/utilities/api';

export default {
  data() {
    return {
      perfiles: [],
      vistas: [],
      vistasDisponibles: [[], []],
      mostrarDialogo: false,
      modoNuevo: false,
      perfilSeleccionado: {}
    };
  },
  methods: {
    async cargarDatos() {
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
.card {
  padding: 4rem;
}

.mt-3 {
  margin-top: 2rem;
}
</style>
