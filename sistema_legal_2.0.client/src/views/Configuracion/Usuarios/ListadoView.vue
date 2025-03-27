<template>
  <DataTable v-model:filters="filters" size="small" sortField="fechaCrea" :sortOrder="-1" :value="usuarios"
      removableSort paginator :rows="10" :rowsPerPageOptions="[10, 20, 50]">
      <template #header>
          <div class="flex justify-content-between align-items-center my-2">
              <div class="text-lg">Listado</div>
              <div class="flex gap-2">
                  <Button icon="fas fa-plus" size="small" primary rounded class="border-round bg-primary" label="Nuevo" @click="$router.push({ name: 'Nuevo Usuario'})" />
                  <IconField iconPosition="left">
                      <InputIcon>
                          <i class="pi pi-search" />
                      </InputIcon>
                      <InputText size="small" v-model="filters['global'].value" variant="filled" placeholder="Buscar..." />
                  </IconField>
              </div>
          </div>
      </template>
      <template #empty> No se han encontrado usuarios. </template>
      <Column field="cedula" header="Cédula"></Column>
      <Column sortable field="nombreUsuario" header="Nombre de Usuario"></Column>
      <Column sortable field="nombres" header="Nombres"></Column>
      <Column sortable field="apellidos" header="Apellidos"></Column>
      <Column sortable field="nombrePerfil" header="Perfil"></Column>
      <Column sortable field="fechaCrea" header="Fecha de Creación">
          <template #body="{ data }">
              {{ new Date(data.fechaCrea).toLocaleDateString() }}
          </template>
      </Column>
      <Column sortable field="activo" header="Estado">
          <template #body="{ data }">
              <Tag v-if="data.activo" severity="success" value="Activo"></Tag>
              <Tag v-else severity="warning" value="Inactivo"></Tag>
          </template>
      </Column>
      <Column>
          <template #body="{ data }">
              <Button icon="fas fa-edit" severity="secondary" secondary class="border-none border-round	" text
                  @click="$router.push({ name: 'Editar Usuario', params: { idUsuario: data.idUsuario }})" />
          </template>
      </Column>
  </DataTable>
</template>

<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'
import { FilterMatchMode } from 'primevue/api';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';

export default {
  name: 'UsuariosView',
  components: { IconField, InputIcon },
  data() {
      return {
          filters: {
              global: { value: null, matchMode: FilterMatchMode.CONTAINS }
          },
          usuarios: [],
          perfiles: [],
          dialogVisible: false,
      }
  },

  mounted() {
      this.LoadData();
  },
  methods: {
      async LoadData() {
          await this.LoadUsuarios();
          await this.LoadPerfiles();
      },
      async LoadUsuarios() {
          const response = await api.get('/api/Usuarios');
          if (response.data) {
              this.usuarios = response.data;
          }
          else push.warning(response.data.message);
      },
      async LoadPerfiles() {
          const response = await api.get('/api/Perfiles');
          if (response.data) {
              this.perfiles = response.data;
          }
          else push.warning(response.data.message);
      },
  }
}
</script>

<style>
.p-input-icon {
  margin-top: -0.6rem !important;
}
</style>
