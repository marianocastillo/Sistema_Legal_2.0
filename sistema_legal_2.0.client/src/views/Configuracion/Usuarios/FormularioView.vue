<template>
  <div class="p-2" style="max-height: 80vh">
      <div class="flex justify-content-between align-items-center my-2">
          <div class="flex gap-2">
              <div class="text-lg text-bold">
                  Formulario
              </div>
              <Tag v-if="usuario.activo" severity="success" value="Activo"></Tag>
              <Tag v-if="!usuario.activo && usuario.withData" severity="warning" value="Inactivo"></Tag>
              <Tag v-if="FormMode == FormModes.Nuevo && !usuario.withData" severity="info" value="Nuevo"></Tag>
          </div>
          <div class="flex gap-2">
              <Button v-if="usuario.activo" size="small" icon="fa-solid fa-user-lock" label="Inactivar" class="border-round"
                  severity="secondary" @click="usuario.activo = !usuario.activo" :disabled="!usuario.withData"  />
              <Button v-else size="small" icon="fa-solid fa-user-check" label="Activar" class="border-round" severity="secondary"
                  @click="usuario.activo = !usuario.activo" :disabled="!usuario.withData"  />
              <Button size="small" icon="fa-solid fa-save" primary rounded class="border-round bg-primary" label="Guardar"
                  @click="Guardar()" />
              <Button size="small" icon="fa-solid fa-arrow-left" label="Volver" @click="$router.back()"
                  class="border-round bg-secondary" />
          </div>
      </div>
      <div class="grid my-2">
          <div class="col-12">
              <div class="border-solid border-primary-500 border-2 border-round surface-border px-3 py-2">
                  <Fieldset class="">
                      <template #legend>
                          <div class="flex align-items-center">
                              <span class="font-bold text-base p-0 my-1">Datos del Usuario</span>
                          </div>
                      </template>
                      <div>
                          <div class="mb-3 flex flex-wrap gap-2">
                              <div class="col-12 md:col-2 p-0">
                                  <label class="font-semibold mb-2 w-full">Cédula <span class="text-danger"
                                          v-if="errors.Cedula">*</span></label>
                                  <InputText size="small" disabled v-model="usuario.cedula" class="w-full"
                                      variant="filled" :invalid="errors.Cedula" placeholder="Cédula del usuario" />
                                  <small id="direccion-institucion-help" class="text-danger" v-if="errors.Cedula">{{errors.Cedula}}</small>
                              </div>
                              <div class="col-12 md:col-2 p-0">
                                  <label class="font-semibold mb-2 w-full">Usuario <span class="text-danger"
                                          v-if="errors.NombreUsuario">*</span></label>
                                  <InputText size="small" :disabled="FormMode == FormModes.Editar" v-model="usuario.nombreUsuario" class="w-full"
                                      variant="filled" :invalid="errors.NombreUsuario" placeholder="Digite el usuario de AD" @keyup.enter="LoadUsuarioByAD" @blur="LoadUsuarioByAD" />
                                  <small id="nombre-usuario-help" class="text-danger" v-if="errors.NombreUsuario">{{
                  errors.NombreUsuario
              }}</small>
                              </div>
                              <div class="col-12 md:col-2 p-0">
                                  <label class="font-semibold mb-2 w-full">Nombres <span class="text-danger"
                                          v-if="errors.Nombres">*</span></label>
                                  <InputText size="small" disabled v-model="usuario.nombres" class="w-full"
                                      variant="filled" :invalid="errors.Nombres" placeholder="Nombres del usuario" />
                                  <small id="nombres-help" class="text-danger" v-if="errors.Nombres">{{ errors.Nombres
                                      }}</small>
                              </div>
                              <div class="col-12 md:col-2 p-0">
                                  <label class="font-semibold mb-2 w-full">Apellidos <span class="text-danger"
                                          v-if="errors.Apellidos">*</span></label>
                                  <InputText size="small" disabled v-model="usuario.apellidos" class="w-full"
                                      variant="filled" :invalid="errors.Apellidos" placeholder="Apellidos del usuario"/>
                                  <small id="apellidos-help" class="text-danger" v-if="errors.Apellidos">{{
                  errors.Apellidos }}</small>
                              </div>
                              <div class="col-12 md:col-2 p-0">
                                  <label class="font-semibold mb-2 w-full">Perfil <span class="text-danger"
                                          v-if="errors.IdPerfil">*</span></label>
                                  <Dropdown size="small" v-model="usuario.idPerfil" class="w-full" :options="perfiles"
                                      optionLabel="nombre" optionValue="idPerfil" :disabled="!usuario.withData" placeholder="Seleccionar..."/>
                                  <small id="perfil-help" class="text-danger" v-if="errors.IdPerfil">{{
                  errors.IdPerfil }}</small>
                              </div>
                              <div class="col-12 md:col p-0"  >
                                  <div v-if="usuario.idPerfil == PerfilesEnum.Asesor || usuario.idPerfil == PerfilesEnum.Supervisor">
                                  <label class="font-semibold mb-2 w-full">Supervisor <span class="text-danger"
                                          v-if="errors.idSupervisor">*</span></label>
                                  <Dropdown id="supervisor" placeholder="Seleccione..." filter
                                      :invalid="errors.idSupervisor" class="w-full" v-model="usuario.idSupervisor"
                                      :options="supervisores" optionLabel="nombreUsuario" optionValue="idUsuario">
                                      <template #value="slotProps">
                                          <div v-if="slotProps.value" class="flex align-items-center">
                                              <div>{{ supervisores.filter(x => x.idUsuario ==
                  slotProps.value)[0].nombreUsuario }}</div>
                                          </div>
                                          <span v-else>
                                              {{ slotProps.placeholder }}
                                          </span>
                                      </template>
                                      <template #option="slotProps">
                                          <div class="flex align-items-center">
                                              <div>{{ slotProps.option.nombreUsuario }}</div>
                                          </div>
                                      </template>
                                      <template #empty>No se han encontrado supervisores</template>
                                  </Dropdown>
                                  <small id="supervisor-help" class="text-danger" v-if="errors.idSupervisor">{{
                  errors.idSupervisor }}</small>
                                  </div>
                              </div>
                          </div>
                          <div class="mb-3 flex flex-wrap gap-2">
                              <div class="col-12 md:col-3 p-0"  >
                                  <div v-if="usuario.idPerfil == PerfilesEnum.Asesor || usuario.idPerfil == PerfilesEnum.Supervisor">
                                      <div class="flex justify-content-start align-items-center gap-2 w-full mb-2">
                                          <label class="font-semibold mb-0">División <span class="text-danger"
                                              v-if="errors.idDivision">*</span></label>
                                          <ConfigurarDivisiones v-model:Divisiones="divisiones"
                                            :LoadDivisiones="LoadDivisiones"
                                            v-if="FormMode == FormModes.Nuevo || FormMode == FormModes.Editar" />
                                      </div>
                                      <Dropdown id="division" placeholder="Seleccione..."
                                          :invalid="errors.idDivision" class="w-full" v-model="usuario.idDivision"
                                          :options="divisiones" optionLabel="nombre" optionValue="idDivision">
                                          <template #value="slotProps">
                                              <div v-if="slotProps.value" class="flex align-items-center">
                                                  <div>{{ divisiones.filter(x => x.idDivision == slotProps.value)[0].nombre }}</div>
                                              </div>
                                              <span v-else>
                                                  {{ slotProps.placeholder }}
                                              </span>
                                          </template>
                                          <template #option="slotProps">
                                              <div class="flex align-items-center">
                                                  <div>{{ slotProps.option.nombre }}</div>
                                              </div>
                                          </template>
                                          <template #empty>No se han encontrado divisiones</template>
                                      </Dropdown>
                                      <small id="division-help" class="text-danger" v-if="errors.idDivision">{{
                      errors.idDivision }}</small>
                                  </div>
                              </div>
                          </div>
                      </div>
                  </Fieldset>
              </div>
              <div class="border-solid border-primary-500 border-2 border-round surface-border px-3 py-2 mt-2" v-if="usuario.idPerfil == PerfilesEnum.Asesor">
                  <Fieldset class="">
                      <template #legend>
                          <div class="flex align-items-center">
                              <span class="font-bold text-base p-0 my-1">Instituciones Asociadas</span>
                          </div>
                      </template>
                      <PickList id="InstitucionesAsociadas" class="p-picklist-sm mb-1"
                          v-model="institucionesAsociadasFiltrado" :showSourceControls="false" :showTargetControls="false"
                          @moveAllToTarget="moveToAsociadas($event)" @moveToTarget="moveToAsociadas($event)"
                          @moveAllToSource="moveToDisponibles($event)" @moveToSource="moveToDisponibles($event)"
                      >
                          <template #sourceheader>
                              <div>Disponibles</div>
                              <div class="w-full mt-2">
                                  <IconField iconPosition="left">
                                      <InputIcon>
                                          <i class="pi pi-search" />
                                      </InputIcon>
                                      <InputText class="w-full" v-model="filtroInstitucionesDisponibles" type="text"
                                          size="small" variant="filled" placeholder="Buscar..." />
                                  </IconField>
                              </div>
                          </template>
                          <template #targetheader>
                              <div>Asociadas</div>
                              <div class="w-full mt-2">
                                  <IconField iconPosition="left">
                                      <InputIcon>
                                          <i class="pi pi-search" />
                                      </InputIcon>
                                      <InputText class="w-full" v-model="filtroInstitucionesAsociadas" type="text"
                                          size="small" variant="filled" placeholder="Buscar..." />
                                  </IconField>
                              </div>
                          </template>
                          <template #item="slotProps">
                              <div class="flex flex-wrap align-items-center">
                                  <div class="flex-1 flex flex-column">
                                      <span class="picklistName">
                                          {{ slotProps.item.nombre}}
                                          <span class="picklistURL text-xs">{{ slotProps.item.estructura }}</span>
                                      </span>
                                  </div>
                              </div>
                          </template>
                      </PickList>
                  </Fieldset>
              </div>
          </div>
      </div>
  </div>
</template>

<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'
import PerfilesEnum from '@/enums/PerfilesEnum.js'
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';

export default {
  name: 'UsuariosView',
  components: { IconField, InputIcon },
  data() {
      return {
          usuario: {
              cedula: '',
              nombreUsuario: '',
              nombres: '',
              apellidos: '',
              idPerfil: 0,
              idSupervisor: 0,
              institucionesAsociadas: [],
              withData: false,
          },
          idUsuario: this.$route.params.idUsuario,
          errors: {},
          usuariosAsociados: [],
          perfiles: [],
          institucionesAsociadas: [],
          divisiones: [],
          supervisores: [],
          filtroInstitucionesAsociadas: '',
          filtroInstitucionesDisponibles: '',
          institucionesAsociadasFiltrado: [],
          FormModes: {
              Nuevo: 0,
              Editar: 1,
          },
          FormMode: 0,
          LastADSearch: '',
      }
  },
  watch: {
      filtroInstitucionesDisponibles: function (val) {
          if (val == '') this.institucionesAsociadasFiltrado[0] = this.institucionesAsociadas[0];
          else this.institucionesAsociadasFiltrado[0] = this.institucionesAsociadas[0].filter(x => x.nombre.toLowerCase().includes(val.toLowerCase()));
      },
      filtroInstitucionesAsociadas: function (val) {
          if (val == '') this.institucionesAsociadasFiltrado[1] = this.institucionesAsociadas[1];
          else this.institucionesAsociadasFiltrado[1] = this.institucionesAsociadas[1].filter(x => x.nombre.toLowerCase().includes(val.toLowerCase()));
      },
  },
  created() {
      this.FormMode = this.GetFormMode();
      this.LoadData();
  },
  computed: {
      PerfilesEnum() {
          return PerfilesEnum;
      }
  },
  methods: {
      async LoadData() {
          await this.LoadSupervisores();
          await this.LoadDivisiones();
          await this.LoadUsuario();
          await this.LoadPerfiles();
      },
      async LoadUsuario() {
          if (this.idUsuario) {
              const response = await api.get(`/api/Usuarios/${this.idUsuario}`);
              if (response.data) {
                  this.usuario = response.data;
                  this.usuario.withData = true;
                  this.LoadInstituciones();
              }
              else push.warning(response.data.message);
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
                  this.LoadInstituciones();
              }
              else push.warning("No se encontró el usuario en el Active Directory");
          }
      },
      async LoadPerfiles() {
          const response = await api.get('/api/Perfiles');
          if (response.data) {
              this.perfiles = response.data;
          }
          else push.warning(response.data.message);
      },
      async LoadSupervisores() {
          const response = await api.get('/api/Usuarios/GetSupervisores');
          if (response.data) {
              if (this.idUsuario) this.supervisores = response.data.filter(x => x.idUsuario != this.idUsuario);
              else this.supervisores = response.data;
          }
          else push.warning(response.data.message);
      },
      async LoadDivisiones() {
          const response = await api.get('/api/Divisiones');
          if (response.data) {
              this.divisiones = response.data;
          }
          else push.warning(response.data.message);
      },
      async LoadInstituciones() {
          const response = await api.get('/api/Instituciones');
          if (response.data) {
              this.institucionesAsociadas[0] = response.data;
              this.institucionesAsociadas[1] = [];
              await this.LoadInstitucionesAsociadas();
          }
          else push.warning(response.data.message);
      },
      async LoadInstitucionesAsociadas() {
          if (this.usuario.idUsuario) {
              const response = await api.get(`/api/Usuarios/GetInstitucionesAsociadas/${this.idUsuario}`);
              if (response.data) {
                  this.institucionesAsociadas[0] = this.institucionesAsociadas[0].filter(x => !response.data.some(y => y.estructura == x.estructura));
                  this.institucionesAsociadasFiltrado[0] = this.institucionesAsociadas[0];
                  this.institucionesAsociadas[1] = response.data;
                  this.institucionesAsociadasFiltrado[1] = this.institucionesAsociadas[1];
              }
              else push.warning(response.data.message);
          }
          else {
              this.institucionesAsociadasFiltrado[0] = this.institucionesAsociadas[0];
              this.institucionesAsociadasFiltrado[1] = this.institucionesAsociadas[1];
          }
      },
      async Guardar() {
          this.usuario.institucionesAsociadas = this.institucionesAsociadas[1];

          const response = await api[this.FormMode == this.FormModes.Editar ? 'put' : 'post']('/api/Usuarios', this.usuario);

          if (response.data.success) {
              push.success(response.data.message);
              this.errors = {};
              this.$router.push('/Configuracion/Usuarios');
          }
          else {
              this.errors = response.data.data;
              push.warning(response.data.message);
          }
      },
      moveToAsociadas(event) {
          const listadoInstituciones = event.items;
          this.institucionesAsociadas[1].push(...listadoInstituciones);
          this.institucionesAsociadas[0] = this.institucionesAsociadas[0].filter(x => !listadoInstituciones.some(y => y.estructura === x.estructura));

          if (this.filtroInstitucionesDisponibles === '') {
              this.institucionesAsociadasFiltrado[0] = this.institucionesAsociadas[0];
          } else {
              this.institucionesAsociadasFiltrado[0] = this.institucionesAsociadas[0].filter(x => x.nombre.toLowerCase().includes(this.filtroInstitucionesDisponibles.toLowerCase()));
          }

          if (this.filtroInstitucionesAsociadas === '') {
              this.institucionesAsociadasFiltrado[1] = this.institucionesAsociadas[1];
          } else {
              this.institucionesAsociadasFiltrado[1] = this.institucionesAsociadas[1].filter(x => x.nombre.toLowerCase().includes(this.filtroInstitucionesAsociadas.toLowerCase()));
          }
      },
      moveToDisponibles(event) {
          const listadoInstituciones = event.items;

          this.institucionesAsociadas[0].push(...listadoInstituciones);
          this.institucionesAsociadas[1] = this.institucionesAsociadas[1].filter(x => !listadoInstituciones.some(y => y.estructura === x.estructura));

          if (this.filtroInstitucionesDisponibles === '') {
              this.institucionesAsociadasFiltrado[0] = this.institucionesAsociadas[0];
          } else {
              this.institucionesAsociadasFiltrado[0] = this.institucionesAsociadas[0].filter(x => x.nombre.toLowerCase().includes(this.filtroInstitucionesDisponibles.toLowerCase()));
          }

          if (this.filtroInstitucionesAsociadas === '') {
              this.institucionesAsociadasFiltrado[1] = this.institucionesAsociadas[1];
          } else {
              this.institucionesAsociadasFiltrado[1] = this.institucionesAsociadas[1].filter(x => x.nombre.toLowerCase().includes(this.filtroInstitucionesAsociadas.toLowerCase()));
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
