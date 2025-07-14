<template>
  <div class="card">
    <h2>Gestión de Perfiles</h2>

    <Button label="Nuevo Perfil" icon="pi pi-plus" class="mb-3" @click="nuevoPerfil" />

    <DataTable :value="perfiles" dataKey="idPerfil">
      <Column field="idPerfil" header="ID" />
      <Column field="nombre" header="Nombre" />
      <Column field="descripcion" header="Descripción" />
      <Column field="cantPermisos" header="Permisos" />
      <Column field="porDefecto" header="Por Defecto">
        <template #body="{ data }">
          <i v-if="data.porDefecto" class="pi pi-check text-green-500"></i>
        </template>
      </Column>
      <Column header="Acciones">
        <template #body="{ data }">
          <Button icon="pi pi-pencil" label="Editar" class="p-button-sm" @click="abrirEdicion(data)" />
        </template>
      </Column>
    </DataTable>

    <Dialog v-model:visible="mostrarDialogo" :header="modoNuevo ? 'Nuevo Perfil' : 'Editar Perfil'" :modal="true" :style="{ width: '50vw' }">
      <div class="p-fluid">
        <label>Nombre</label>
        <InputText v-model="perfilSeleccionado.nombre" />

        <label>Descripción</label>
        <InputText v-model="perfilSeleccionado.descripcion" />

        <label>Por defecto</label>
        <InputSwitch v-model="perfilSeleccionado.porDefecto" />

        <label class="mt-3">Vistas</label>
        <br><br>
        <PickList
          v-model="vistasDisponibles"
          :sourceHeader="'Disponibles'"
          :targetHeader="'Asignadas'"
          :dataKey="'idVista'"
        >
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
      const [resPerfiles, resVistas] = await Promise.all([
        fetch('/api/Perfiles/GetPerfiles'),
        fetch('/api/Perfiles/GetVistas')
      ]);
      this.perfiles = await resPerfiles.json();
      this.vistas = await resVistas.json();
    },

    async abrirEdicion(perfil) {
      this.modoNuevo = false;

      const res = await fetch(`/api/Perfiles/${perfil.idPerfil}`);
      const perfilCompleto = await res.json();
      this.perfilSeleccionado = { ...perfilCompleto };

      const vistasPerfil = await fetch(`/api/Perfiles/GetPermisos/${perfil.idPerfil}`).then(r => r.json());
      const vistasAsignadas = vistasPerfil.filter(v => v.permiso);
      const vistasNoAsignadas = vistasPerfil.filter(v => !v.permiso);
      this.vistasDisponibles = [vistasNoAsignadas, vistasAsignadas];

      this.mostrarDialogo = true;
    },

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

       console.log("Vistas disponibles (raw):", this.vistasDisponibles);

    const vistasAsignadas = this.vistasDisponibles[1].map(v => ({
      idVista: v.idVista,
      nombre: v.nombre,
      descripcion: v.descripcion,
      url: v.url,
      idModulo: v.idModulo,
      permiso: true,
      principal: v.principal,
      iconClass: v.iconClass,
      orden: v.orden
    }));
 console.log("Vistas asignadas procesadas:", vistasAsignadas);
    // Preparar el objeto perfil según lo que espera la API
    const perfilModel = {
      idPerfil: this.perfilSeleccionado.idPerfil || 0, // Usar || en lugar de ??
      nombre: this.perfilSeleccionado.nombre,
      descripcion: this.perfilSeleccionado.descripcion,
      porDefecto: this.perfilSeleccionado.porDefecto || false,
      vistas: vistasAsignadas,
      usuarios: [], // Siempre enviar array vacío si no hay usuarios
      cantPermisos: vistasAsignadas.length
    };
 console.log("Modelo a enviar a la API:", JSON.stringify(perfilModel, null, 2));
    const isNuevo = this.modoNuevo;
    const url = isNuevo ? '/api/Perfiles/SavePerfil' : '/api/Perfiles/UpdatePerfil';
console.log(`Enviando petición ${method.toUpperCase()} a: ${url}`);
    const response = await axios({
      method: isNuevo ? 'post' : 'put',
      url: url,
      data: perfilModel,
      headers: {
        'Content-Type': 'application/json'
      }
    });
console.log("Respuesta de la API:", response);
    if (response.data.success) {
      // Asumo que tienes un sistema de notificaciones (toast)
      this.$toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: response.data.message || 'Operación realizada con éxito',
        life: 3000
      });
      this.mostrarDialogo = false;
      await this.cargarDatos(); // Recargar los datos
    } else {
      this.$toast.add({
        severity: 'warn',
        summary: 'Advertencia',
        detail: response.data.message || 'La operación no se completó',
        life: 3000
      });
    }
  } catch (error) {
    // 6. Mostrar error completo
    console.error("Error completo:", error);
    console.log("Configuración de la petición:", error.config);

    if (error.response) {
      console.log("Datos de respuesta del error:", error.response.data);
      console.log("Estado del error:", error.response.status);
      console.log("Cabeceras del error:", error.response.headers);
    }

    let errorMessage = 'Error al conectar con el servidor';
    if (error.response) {
      errorMessage = error.response.data?.message ||
                     `Error ${error.response.status}: ${error.response.statusText}`;
    }

    this.$toast.add({
      severity: 'error',
      summary: 'Error',
      detail: errorMessage,
      life: 5000
    });
  }
}


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
