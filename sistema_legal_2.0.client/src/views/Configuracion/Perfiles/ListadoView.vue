<template>
  <DataTable v-model:filters="filters" size="small" :value="perfiles" removableSort paginator :rows="10" :rowsPerPageOptions="[10, 20, 50]">
      <template #header>
          <div class="flex justify-content-between align-items-center my-2">
              <div class="text-lg">Listado</div>
              <div class="flex gap-2">
                  <Button icon="fas fa-plus" size="small" primary rounded class="border-round bg-primary" label="Nuevo" @click="$router.push({ name: 'Nuevo Perfil'})" />
                  <IconField iconPosition="left">
                      <InputIcon>
                          <i class="pi pi-search" />
                      </InputIcon>
                      <InputText size="small" v-model="filters['global'].value" variant="filled" placeholder="Buscar..." />
                  </IconField>
              </div>
          </div>
      </template>
      <template #empty> No se han encontrado perfiles. </template>
      <Column sortable field="nombre" header="Nombre"></Column>
      <Column sortable field="descripcion" header="Descripcion" style="width: 50%"></Column>
      <Column sortable field="cantPermisos" header="Cantidad de Permisos">
          <template #body="{data}">
              <Badge :value="data.cantPermisos" class="p-badge-primary"></Badge>
          </template>
      </Column>
      <Column sortable field="porDefecto" header="Por defecto">
          <template #body="{data}">
              <div v-if="data.porDefecto" class="w-100 text-center">
                  <i class="fas fa-check" ></i>
              </div>
          </template>
      </Column>
      <Column>
          <template #body="{data}">
              <div class="d-flex justify-content-end align-items-center">
                  <Button icon="fas fa-edit" severity="secondary" secondary class="border-none border-round" text @click="$router.push({ name: 'Editar Perfil', params: { idPerfil: data.idPerfil }})" />
                  <Button icon="fas fa-trash-can" :disabled="data.noEditable" severity="danger" class="border-none border-round" text @click="ConfirmDelete($event, data)" />
              </div>
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
  name: 'PerfilesView',
  components: { IconField, InputIcon },
  data() {
      return {
          filters: {
              global: { value: null, matchMode: FilterMatchMode.CONTAINS }
          },
          perfiles: [],
          dialogVisible: false,
      }
  },
  mounted() {
      this.LoadPerfiles();
  },
  methods: {
      async LoadPerfiles() {
          const response = await api.get('/api/Perfiles');
          if (response.data) {
              this.perfiles = response.data;
          }
          else push.warning(response.data.message);
      },
      ConfirmDelete(event, data) {
          if (data.noEditable) {
              push.warning('No puedes eliminar este perfil');
              return;
          }

          this.$confirm.require({
              target: event.currentTarget,
              message: '¿Estás seguro que deseas proceder?',
              icon: 'pi pi-exclamation-triangle',
              rejectClass: 'p-button-secondary border-round p-button-outlined p-button-sm mx-2',
              acceptClass: 'p-button-danger p-button-sm border-round',
              rejectLabel: 'Cancel',
              acceptLabel: 'Eliminar',
              accept: async () => {
                  const response = await api.delete(`/api/Perfiles/${data.idPerfil}`);
                  if (response.data) {
                      this.LoadPerfiles();
                      push.success(response.data.message);
                  }
                  else push.warning(response.data.message);
              }
          });
      }
  }
}
</script>

<style>
.p-input-icon {
  margin-top: -0.6rem !important;
}
</style>
