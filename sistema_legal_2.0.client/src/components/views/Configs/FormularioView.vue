<template>
    <div class="p-2" style="max-height: 80vh">
        <div class="flex justify-content-between align-items-center my-2">
            <div class="flex gap-2">
                <div class="text-xl font-bold">
                    Formulario
                </div>
                <Tag v-if="usuario.activo" severity="success" value="Activo"></Tag>
                <Tag v-if="!usuario.activo && usuario.withData" severity="warning" value="Inactivo"></Tag>
                <Tag v-if="FormMode == FormModes.Nuevo && !usuario.withData" severity="info" value="Nuevo"></Tag>
            </div>
            <div class="flex gap-2">
                <Button v-if="usuario.activo" icon="fa-solid fa-user-lock" label="Inactivar"
                    severity="secondary" @click="usuario.activo = !usuario.activo" :disabled="!usuario.withData"  />
                <Button v-else  icon="fa-solid fa-user-check" label="Activar" severity="secondary"
                    @click="usuario.activo = !usuario.activo" :disabled="!usuario.withData"  />
                <Button  icon="fa-solid fa-save" primary label="Guardar"
                    @click="Guardar()" />
                <Button  icon="fa-solid fa-arrow-left" label="Volver" @click="$router.back()" severity="secondary" />
            </div>
        </div>
        <div class="grid my-2">
            <div class="col-12">
                    <Fieldset class="">
                        <template #legend>
                            <div class="flex align-items-center">
                                <span class="font-bold text-base p-0 my-1">Datos del Usuario</span>
                            </div>
                        </template>
                        <div>
                            <div class="mb-3 flex flex-wrap gap-2">
                               <!-- <div class="col-12 md:col-2 p-0">
                                    <label class="font-semibold mb-2 w-full block">Cédula <span class="text-danger"
                                            v-if="errors.Cedula">*</span></label>
                                    <InputText  disabled v-model="usuario.cedula" class="w-full"
                                        variant="filled" :invalid="errors.Cedula" placeholder="Cédula del usuario" />
                                    <small id="direccion-institucion-help" class="text-danger" v-if="errors.Cedula">{{errors.Cedula}}</small>
                                </div>-->
                                <div class="col-12 md:col-3 p-0">
                                    <label class="font-semibold mb-2 w-full block">Usuario <span class="text-danger"
                                            v-if="errors.NombreUsuario">*</span></label>
                                    <InputText  :disabled="FormMode == FormModes.Editar" v-model="usuario.nombreUsuario" class="w-full"
                                        variant="filled" :invalid="errors.NombreUsuario" placeholder="Digite el usuario de AD" @keyup.enter="LoadUsuarioByAD" @blur="LoadUsuarioByAD" />
                                    <small id="nombre-usuario-help" class="text-danger" v-if="errors.NombreUsuario">{{
                    errors.NombreUsuario
                }}</small>
                                </div>
                                <div class="col-12 md:col-3 p-0">
                                    <label class="font-semibold mb-2 w-full block">Nombres <span class="text-danger"
                                            v-if="errors.Nombres">*</span></label>
                                    <InputText  disabled v-model="usuario.nombres" class="w-full"
                                        variant="filled" :invalid="errors.Nombres" placeholder="Nombres del usuario" />
                                    <small id="nombres-help" class="text-danger" v-if="errors.Nombres">{{ errors.Nombres
                                        }}</small>
                                </div>
                                <div class="col-12 md:col-3 p-0">
                                    <label class="font-semibold mb-2 w-full block">Apellidos <span class="text-danger"
                                            v-if="errors.Apellidos">*</span></label>
                                    <InputText  disabled v-model="usuario.apellidos" class="w-full"
                                        variant="filled" :invalid="errors.Apellidos" placeholder="Apellidos del usuario"/>
                                    <small id="apellidos-help" class="text-danger" v-if="errors.Apellidos">{{
                    errors.Apellidos }}</small>
                                </div>
                                <div class="col-12 md:col-3 p-0">
                                    <label class="font-semibold mb-2 w-full block">Perfil <span class="text-danger"
                                            v-if="errors.IdPerfil">*</span></label>
                                    <Select  v-model="usuario.idPerfil" class="w-full" :options="perfiles"
                                        optionLabel="nombre" optionValue="idPerfil" :disabled="!usuario.withData" placeholder="Seleccionar..."/>
                                    <small id="perfil-help" class="text-danger" v-if="errors.IdPerfil">{{
                    errors.IdPerfil }}</small>
                                </div>
                            </div>
                        </div>
                    </Fieldset>
            </div>
        </div>
    </div>
</template>

<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'

export default {
    name: 'UsuariosView',
    data() {
        return {
            usuario: {
                cedula: '',
                nombreUsuario: '',
                nombres: '',
                apellidos: '',
                idPerfil: 0,
                withData: false,
            },
            idUsuario: this.$route.params.idUsuario,
            errors: {},
            usuariosAsociados: [],
            perfiles: [],
            FormModes: {
                Nuevo: 0,
                Editar: 1,
            },
            FormMode: 0,
            LastADSearch: '',
        }
    },
    created() {
        this.FormMode = this.GetFormMode();
        this.LoadData();
    },
    methods: {
        async LoadData() {
            await this.LoadUsuario();
            await this.LoadPerfiles();
        },
        async LoadUsuario() {
            if (this.idUsuario) {
                const response = await api.get(`/api/Usuarios/${this.idUsuario}`);                
              
                if (response.data) {
                    this.usuario = response.data;
                    this.usuario.withData = true;
                }
                else push.warning({ title: "Advertencia", message: response.data.message });
            }
        },
        async LoadUsuarioByAD() {
            if (this.usuario.nombreUsuario) {
                if (this.LastADSearch === this.usuario.nombreUsuario) return;
                this.LastADSearch = this.usuario.nombreUsuario;
                const response = await api.get(`/api/Usuarios/GetADUser/${this.usuario.nombreUsuario}`);

                if (response.data) {
                    this.usuario = response.data;
                    this.usuario.withData = true;
                }
                else push.warning({ title: "Advertencia", message: "No se encontró el usuario en el Active Directory" });
            }
        },
        async LoadPerfiles() {
            const response = await api.get('/api/Perfiles');
            if (response.data) {
                this.perfiles = response.data;
            }
            else push.warning({ title: "Advertencia", message: response.data.message });
        },
        async Guardar() {
            const response = await api[this.FormMode == this.FormModes.Editar ? 'put' : 'post']('/api/Usuarios', this.usuario);

            if (response.data.success) {
                push.success({ title: 'Operación exitosa', message: response.data.message});
                this.errors = {};
                this.$router.push('/Configuracion/Usuarios');
            }
            else {
                if (response.data.errors) this.errors = response.data.errors;
                push.warning({ title: 'Advertencia', message: response.data.message});
            }
        },
        GetFormMode() {
            if (this.$route.name == "Nuevo Usuario") return this.FormModes.Nuevo;
            if (this.$route.name == "Editar Usuario") return this.FormModes.Editar;
        },
    }
}
</script>

<style>
.p-input-icon {
    margin-top: -0.6rem !important;
}

.p-accordion .p-accordion-header .p-accordion-header-link {
    padding: 0.5rem 0rem !important;
    color: #334155;
}
</style>