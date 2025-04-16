<template>
    <DataTable v-model:filters="filters" tableStyle="" size="small" sortField="fechaCrea" :sortOrder="-1" :value="usuarios"
        removableSort paginator :rows="10" :rowsPerPageOptions="[10, 20, 50]">
        <template #header>
            <div class="flex justify-content-between align-items-center my-2 mx-2">
                <div class="text-xl font-bold" style="color: #003876;">Listado de usuarios</div>
                <div class="flex gap-2">
                    <Button icon="fas fa-plus" label="Nuevo" @click="$router.push({ name: 'Nuevo Usuario'})" />
                    <IconField iconPosition="left">
                        <InputIcon>
                            <i class="pi pi-search" />
                        </InputIcon>
                        <InputText v-model="filters['global'].value" variant="filled" placeholder="Buscar..." />
                    </IconField>
                    <Button  icon="fa-solid fa-home" label="Inicio" @click="$router.push({name: 'Inicio'})" severity="secondary" />
                </div>
            </div>
        </template>
        <template #empty> No se han encontrado usuarios. </template>     
        <Column sortable style="padding: 0px 20px 0px 20px; color: #003876; width: 15%;" body-style="color: dark-gray" field="nombreUsuario" header="Nombre de Usuario"></Column>
        <Column sortable style="color: #003876; width: 20%;" body-style="color: dark-gray" field="nombres" header="Nombres"></Column>
        <Column sortable style="color: #003876; width: 20%;" body-style="color: dark-gray" field="apellidos" header="Apellidos"></Column>
        <Column sortable style="color: #003876;" body-style="color: dark-gray" field="nombrePerfil" header="Perfil"></Column>
        <Column sortable style="color: #003876;" body-style="color: dark-gray" field="fechaCrea" header="Fecha de Creación">
            <template #body="{ data }">
                {{ new Date(data.fechaCrea).toLocaleDateString() }}
            </template>
        </Column>
        <Column style="color: #003876;" sortable field="activo" header="Estado">
            <template #body="{ data }">
                <Tag v-if="data.activo" severity="success" value="Activo"></Tag>
                <Tag v-else severity="warn" value="Inactivo"></Tag>
            </template>
        </Column>
        <Column header="Acciones">
            <template #body="{ data }">
                <Button style="color: #003876;"  icon="fas fa-edit" severity="secondary" text
                    @click="$router.push({ name: 'Editar Usuario', params: { idUsuario: data.idUsuario }})" />
                    <Button  icon="fas fa-trash-alt" severity="danger" text
                    @click="ConfirmDelete($event, data.idUsuario)" />
            </template>
        </Column>
        <ConfirmPopup></ConfirmPopup>
    </DataTable>
</template>

<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'
import { FilterMatchMode } from '@primevue/core/api';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';
import ConfirmPopup from 'primevue/confirmpopup';

export default {
    name: 'UsuariosView',
    components: { IconField, InputIcon, ConfirmPopup },
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
        async LoadUsuarios() 
        {
            const response = await api.get('/api/Usuarios');
            if (response.data) {
                this.usuarios = response.data;               
            }
            else push.warning({ title: "Advertencia", message: response.data.message });
        },
        async LoadPerfiles() {
            const response = await api.get('/api/Perfiles');
            if (response.data) {
                this.perfiles = response.data;               
            }
            else push.warning({ title: "Advertencia", message: response.data.message });
        },
        ConfirmDelete(event, idUsuario) {
            this.$confirm.require({
                target: event.currentTarget,
                message: '¿Estás seguro que deseas proceder?',
                icon: 'pi pi-exclamation-triangle',
                rejectClass: 'p-button-secondary border-rounded p-button-outlined p-button-sm mx-2',
                acceptClass: 'p-button-danger p-button-sm border-rounded',
                rejectLabel: 'Cancelar',
                acceptLabel: 'Eliminar',
                accept: async () => {
                    const response = await api.delete(`/api/Usuarios/${idUsuario}`);
                    if (response.data) {
                        this.LoadUsuarios();
                        push.success({ title: 'Operación exitosa', message: response.data.message});
                    }
                    else push.warning({ title: 'Advertencia', message: response.data.message});
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
