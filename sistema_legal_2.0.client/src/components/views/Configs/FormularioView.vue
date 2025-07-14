<!-- <template>
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
</template> -->

<template>
  <div class="card p-6 shadow-2" style="max-height: 80vh; overflow-y: auto;">
    <div class="flex justify-content-between items-center mb-6 flex-wrap gap-2">
      <div class="flex items-center gap-3">
        <h2 class="text-2xl font-bold m-0">Formulario</h2>
        <span v-if="usuario.activo" class="tag status-sentencia">Activo</span>
        <span v-if="!usuario.activo && usuario.withData" class="tag status-casacion">Inactivo</span>
        <span v-if="FormMode == FormModes.Nuevo && !usuario.withData" class="tag bg-blue-100 text-blue-800">Nuevo</span>
      </div>

      <div class="flex gap-2 flex-wrap">
        <button v-if="usuario.activo" class="btn btn-sm text-white" style="background-color: #003870;"
          @click="usuario.activo = false" :disabled="!usuario.withData">
          <i class="fa-solid fa-user-lock me-1"></i> Inactivar
        </button>
        <button v-else class="btn btn-sm text-white" style="background-color: #003870;"
          @click="usuario.activo = true" :disabled="!usuario.withData">
          <i class="fa-solid fa-user-check me-1"></i> Activar
        </button>

        <button class="btn btn-sm text-white" style="background-color: #003870;" @click="Guardar()">
          <i class="fa-solid fa-save me-1"></i> Guardar
        </button>
        <button class="btn btn-sm border border-gray-300" @click="$router.back()">
          <i class="fa-solid fa-arrow-left me-1"></i> Volver
        </button>
      </div>
    </div>

    <div class="rounded-xl border border-gray-200 p-4 bg-white shadow">
      <h3 class="text-lg font-semibold mb-4">Datos del Usuario</h3>

      <div class="grid p-fluid">
  <div class="field col-12 md:col-3">
    <label class="form-label">Usuario <span class="text-danger" v-if="errors.NombreUsuario">*</span></label>
    <input
      type="text"
      class="form-control w-full"
      :disabled="FormMode == FormModes.Editar"
      v-model="usuario.nombreUsuario"
      placeholder="Digite el usuario de AD"
      @keyup.enter="LoadUsuarioByAD"
      @blur="LoadUsuarioByAD"
      :class="{ 'is-invalid': errors.NombreUsuario }"
    />
    <div class="invalid-feedback" v-if="errors.NombreUsuario">{{ errors.NombreUsuario }}</div>
  </div>

  <div class="field col-12 md:col-3">
    <label class="form-label">Nombres <span class="text-danger" v-if="errors.Nombres">*</span></label>
    <input
      type="text"
      class="form-control w-full"
      disabled
      v-model="usuario.nombres"
      placeholder="Nombres del usuario"
      :class="{ 'is-invalid': errors.Nombres }"
    />
    <div class="invalid-feedback" v-if="errors.Nombres">{{ errors.Nombres }}</div>
  </div>

  <div class="field col-12 md:col-3">
    <label class="form-label">Apellidos <span class="text-danger" v-if="errors.Apellidos">*</span></label>
    <input
      type="text"
      class="form-control w-full"
      disabled
      v-model="usuario.apellidos"
      placeholder="Apellidos del usuario"
      :class="{ 'is-invalid': errors.Apellidos }"
    />
    <div class="invalid-feedback" v-if="errors.Apellidos">{{ errors.Apellidos }}</div>
  </div>

  <div class="field col-12 md:col-3">
    <label class="form-label">Perfil <span class="text-danger" v-if="errors.IdPerfil">*</span></label>
    <select
      class="form-select w-full"
      v-model="usuario.idPerfil"
      :disabled="!usuario.withData"
      :class="{ 'is-invalid': errors.IdPerfil }"
    >
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
</template>


<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'


export default {
  name: 'FormularioView',
  data() {
    return {

      usuario: {
        idUsuario: 0,
        nombreUsuario: '',
        nombres: '',
        apellidos: '',
        idPerfil: 0,
        IdSupervisor: 0,
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
        console.log(this.usuario)
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
      const response = await api.get('/api/Perfiles/GetPerfiles');
      if (response.data) {
        this.perfiles = response.data;
      }
      else push.warning({ title: "Advertencia", message: response.data.message });
    },




    async Guardar() {
      const usuarioLogueado = JSON.parse(localStorage.getItem('usuario'))
      this.usuario.IdSupervisor = usuarioLogueado?.idUsuario || 0


      const response = await api[this.FormMode == this.FormModes.Editar ? 'put' : 'post']('/api/Usuarios', this.usuario);

      if (response.data.success) {
        push.success({ title: 'Operación exitosa', message: response.data.message });
        this.errors = {};

        this.usuario = {
          nombreUsuario: '',
          nombres: '',
          apellidos: '',
          idPerfil: 0,
          activo: false,
          IdSupervisor: usuarioLogueado?.idUsuario || 0
        };
        this.$router.push('/listadodeusuario');
      }
      else {
        if (response.data.errors) this.errors = response.data.errors;
        push.warning({ title: 'Advertencia', message: response.data.message });
      }
    },





    GetFormMode() {
      if (this.$route.name == "Nuevo Usuario") return this.FormModes.Nuevo;
      if (this.$route.name == "formulario") return this.FormModes.Editar;
    },
  }
}
</script>

<style scoped>
/* Botones */
.btn {
  padding: 0.4rem 0.9rem;
  border-radius: 6px;
  font-weight: 500;
  font-size: 0.875rem;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.text-white {
  color: white !important;
}

.shadow-2 {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
}

/* Tag/Badge */
.tag {
  padding: 0.35rem 0.8rem;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-align: center;
  display: inline-block;
}

.status-sentencia {
  background-color: #d4edda;
  color: #155724;
}

.status-casacion {
  background-color: #f8d7da;
  color: #721c24;
}

/* Input validation */
.is-invalid {
  border-color: #dc3545 !important;
}

.invalid-feedback {
  color: #dc3545;
  font-size: 0.8rem;
  margin-top: 0.25rem;
}

/* Responsive grid (Tailwind fallback if not available) */
@media (min-width: 768px) {
  .grid-cols-4 {
    grid-template-columns: repeat(4, minmax(0, 1fr));
  }
}
</style>

