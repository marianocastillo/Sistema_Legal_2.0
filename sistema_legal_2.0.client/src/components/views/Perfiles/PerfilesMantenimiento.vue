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

    const perfilModel = {
      idPerfil: this.perfilSeleccionado.idPerfil || 0,
      nombre: this.perfilSeleccionado.nombre,
      descripcion: this.perfilSeleccionado.descripcion,
      porDefecto: this.perfilSeleccionado.porDefecto || false,
      vistas: vistasAsignadas,
      usuarios: [],
      cantPermisos: vistasAsignadas.length
    };

    console.log("Modelo a enviar:", JSON.stringify(perfilModel, null, 2));

    const isNuevo = this.modoNuevo;
    const url = isNuevo ? '/api/Perfiles/SavePerfil' : '/api/Perfiles/UpdatePerfil';
    const method = isNuevo ? 'post' : 'put'; // Definimos la variable method

    const response = await axios({
      method: method, // Usamos la variable definida
      url: url,
      data: perfilModel,
      headers: {
        'Content-Type': 'application/json'
      }
    });

    console.log("Respuesta:", response.data);

    if (response.data.success) {
      this.$toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: response.data.message || 'Operación exitosa',
        life: 3000
      });
      this.mostrarDialogo = false;
      await this.cargarDatos();
    } else {
      this.$toast.add({
        severity: 'warn',
        summary: 'Advertencia',
        detail: response.data.message || 'Operación no completada',
        life: 3000
      });
    }
  } catch (error) {
    console.error("Error completo:", error);

    let errorMessage = 'Error en la conexión';
    if (error.response) {
      console.log("Datos del error:", error.response.data);
      errorMessage = error.response.data?.message ||
                    `Error ${error.response.status}`;
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
