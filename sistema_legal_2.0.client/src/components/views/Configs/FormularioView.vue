<template>
  <div class="container py-3" style="max-height: 80vh;">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <div class="d-flex gap-2 align-items-center">
        <h4 class="m-0">Formulario</h4>
        <span v-if="usuario.activo" class="badge bg-success">Activo</span>
        <span v-if="!usuario.activo && usuario.withData" class="badge bg-warning text-dark">Inactivo</span>
        <span v-if="FormMode == FormModes.Nuevo && !usuario.withData" class="badge bg-info text-dark">Nuevo</span>
      </div>
      <div class="d-flex gap-2">
        <button v-if="usuario.activo" class="btn btn-secondary btn-sm" @click="usuario.activo = false"
          :disabled="!usuario.withData">
          <i class="fa-solid fa-user-lock me-1"></i>Inactivar
        </button>
        <button v-else class="btn btn-secondary btn-sm" @click="usuario.activo = true" :disabled="!usuario.withData">
          <i class="fa-solid fa-user-check me-1"></i>Activar
        </button>
        <button class="btn btn-primary btn-sm" @click="Guardar()">
          <i class="fa-solid fa-save me-1"></i>Guardar
        </button>
        <button class="btn btn-outline-secondary btn-sm" @click="$router.back()">
          <i class="fa-solid fa-arrow-left me-1"></i>Volver
        </button>
      </div>
    </div>

    <div class="card">
      <div class="card-header fw-bold">Datos del Usuario</div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-3">
            <label class="form-label">Usuario <span class="text-danger" v-if="errors.NombreUsuario">*</span></label>
            <input type="text" class="form-control" :disabled="FormMode == FormModes.Editar"
              v-model="usuario.nombreUsuario" placeholder="Digite el usuario de AD" @keyup.enter="LoadUsuarioByAD"
              @blur="LoadUsuarioByAD" :class="{ 'is-invalid': errors.NombreUsuario }" />
            <div class="invalid-feedback" v-if="errors.NombreUsuario">{{ errors.NombreUsuario }}</div>
          </div>
          <div class="col-md-3">
            <label class="form-label">Nombres <span class="text-danger" v-if="errors.Nombres">*</span></label>
            <input type="text" class="form-control" disabled v-model="usuario.nombres" placeholder="Nombres del usuario"
              :class="{ 'is-invalid': errors.Nombres }" />
            <div class="invalid-feedback" v-if="errors.Nombres">{{ errors.Nombres }}</div>
          </div>
          <div class="col-md-3">
            <label class="form-label">Apellidos <span class="text-danger" v-if="errors.Apellidos">*</span></label>
            <input type="text" class="form-control" disabled v-model="usuario.apellidos"
              placeholder="Apellidos del usuario" :class="{ 'is-invalid': errors.Apellidos }" />
            <div class="invalid-feedback" v-if="errors.Apellidos">{{ errors.Apellidos }}</div>
          </div>
          <div class="col-md-3">
            <label class="form-label">Perfil <span class="text-danger" v-if="errors.IdPerfil">*</span></label>
            <select class="form-select" v-model="usuario.idPerfil" :disabled="!usuario.withData"
              :class="{ 'is-invalid': errors.IdPerfil }">
              <option value="">Seleccionar...</option>
              <option v-for="perfil in perfiles" :key="perfil.idPerfil" :value="perfil.idPerfil">
                {{ perfil.nombre }}
              </option>
            </select>
            <div class="invalid-feedback" v-if="errors.IdPerfil">{{ errors.IdPerfil }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'


export default {
  name: 'FormularioView',
  data() {
    return {
      usuario: {
        Cedula: '',
  NombreUsuario: '',
  Nombres: '',
  Apellidos: '',
  IdPerfil: 0,
  idSupervisor: null,
  Activo: true,
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
    const idUsuario = this.$route.params.idUsuario;
    console.log(idUsuario);
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
      console.log("Datos enviados:", this.usuario);

      const payload = { ...this.usuario };
      delete payload.withData;
      const response = await api[this.FormMode == this.FormModes.Editar ? 'put' : 'post']('/api/Usuarios', this.usuario);

      if (response.data.success) {
        push.success({ title: 'Operación exitosa', message: response.data.message });
        this.errors = {};
        this.$router.push('/drawer/listadodeusuario');
      }
      else {
        if (response.data.errors) this.errors = response.data.errors;
        push.warning({ title: 'Advertencia', message: response.data.message });
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
