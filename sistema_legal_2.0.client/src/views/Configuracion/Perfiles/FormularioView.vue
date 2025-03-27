<template>
  <div class="p-2" style="max-height: 80vh">
      <div class="flex justify-content-between align-items-center my-2">
          <div class="text-lg text-bold">Formulario</div>
          <div class="flex gap-2">
              <Button size="small" icon="fa-solid fa-save" primary rounded class="border-round bg-primary" label="Guardar" @click="Guardar()" />
              <Button size="small" icon="fa-solid fa-arrow-left" label="Volver" @click="$router.back()" class="border-round bg-secondary" />
          </div>
      </div>
      <div class="grid my-2">
          <div class="col-12 lg:col-4">
              <div class="border-solid border-primary-500 border-2 border-round surface-border p-3">
                  <Fieldset class="">
                      <template #legend>
                          <div class="flex align-items-center">
                              <span class="font-bold text-base p-0 my-1">Datos del Perfil</span>
                          </div>
                      </template>
                      <div class="overflow-y-auto	" style="max-height: 62vh">
                          <div class="mb-2">
                              <label class="font-semibold mb-2 w-full">Nombre <span class="text-danger" v-if="errors.Nombre">*</span></label>
                              <InputText size="small" v-model="perfil.nombre" :disabled="perfil.noEditable" class="w-full" variant="filled" :invalid="errors.Nombre"/>
                              <small id="nombre-perfil-help" class="text-danger" v-if="errors.Nombre">{{ errors.Nombre }}</small>
                          </div>
                          <div class="mb-2">
                              <label class="font-semibold mb-2 w-full">Descripción <span class="text-danger" v-if="errors.Descripcion">*</span></label>
                              <Textarea size="small" autoResize v-model="perfil.descripcion" class="w-full" variant="filled" toggleMask :feedback="false" :invalid="errors.Descripcion" />
                              <small id="descripcion-perfil-help" class="text-danger" v-if="errors.Descripcion">{{ errors.Descripcion }}</small>
                          </div>
                          <div class="mb-2">
                              <label class="font-semibold mb-2 w-full">Por defecto <span class="text-danger" v-if="errors.porDefecto">*</span></label>
                              <InputSwitch v-model="perfil.porDefecto" />
                          </div>
                      </div>
                  </Fieldset>
              </div>
          </div>
          <div class="col-12 lg:col-6">
              <div class="border-solid border-primary-500 border-2 border-round surface-border p-3">
                  <Fieldset class="">
                      <template #legend>
                          <div class="flex align-items-center">
                              <span class="font-bold text-base p-0 my-1">Permisos</span>
                          </div>
                      </template>
                      <div class="overflow-y-auto" style="max-height: 62vh" v-if="perfil.vistas">
                          <div class="py-2" style="border-bottom: 1px solid #e2e8f0;" v-for="vista in perfil.vistas.filter(x => $store.state.views.filter((y) => y.modulo.idModulo == x.modulo.idModulo && (x.principal || !(!x.principal && x.idPadre == null)) && perfil.vistas.filter((z) => z.modulo.idModulo == y.modulo.idModulo).length <= 1).length == 1)" :key="vista.idVista">
                              <div class="flex">
                                  <span class="flex align-items-center gap-2 w-full">
                                      <i :class="`font-medium white-space-nowrap ${vista.modulo.icono}`" />
                                      <span class="font-medium white-space-nowrap">{{ vista.nombre }}</span>
                                      <InputSwitch v-model="vista.permiso" :disabled="vista.nombre == 'Inicio'" class="ml-auto"/>
                                  </span>
                              </div>
                          </div>
                          <Accordion :activeIndex="0" >
                              <AccordionTab v-for="modulo in getModulos(perfil.vistas.filter(x => x.idModulo != null))" :id="'modulo_' + modulo.nombre" :key="modulo.idModulo">
                                  <template #header>
                                      <span class="flex align-items-center gap-2 w-full">
                                          <i :class="`font-bold white-space-nowrap ${modulo.icono}`" />
                                          <span class="font-bold white-space-nowrap">{{ modulo.nombre }}</span>
                                          <Badge :value="`${perfil.vistas.filter(x => x.idModulo == modulo.idModulo && x.permiso && (x.principal || (!x.principal && x.idPadre == null))).length}/${perfil.vistas.filter(x => x.idModulo == modulo.idModulo && (x.principal || (!x.principal && x.idPadre == null))).length}`" class="ml-auto mr-2 p-badge-primary" />
                                      </span>
                                  </template>
                                  <div class="mt-2">
                                      <div class="pb-2" v-for="vista in perfil.vistas.filter(x => x.idModulo == modulo.idModulo && (x.principal || (!x.principal && x.idPadre == null)))" :key="vista.idVista" :id="'vista_' + vista.idVista">
                                          <div class="flex">
                                              <span class="flex align-items-center gap-2 w-full">
                                                  <i :class="`font-medium white-space-nowrap ${vista.icono}`" />
                                                  <span class="font-medium white-space-nowrap">{{ vista.nombre }}</span>
                                                  <InputSwitch v-model="vista.permiso" class="ml-auto"/>
                                              </span>
                                          </div>
                                      </div>
                                  </div>
                              </AccordionTab>
                          </Accordion>
                      </div>
                  </Fieldset>
              </div>
          </div>
          <div class="col-12 lg:col " >
              <div class="border-solid border-primary-500 border-2 border-round surface-border p-3 ">
                  <Fieldset>
                      <template #legend>
                          <div class="flex align-items-center">
                              <span class="font-bold text-base p-0 my-1">Usuarios asociados</span>
                              <div class="ml-auto cursor-pointer" @click="agregarUsuario = true">
                                  <i class="text-lg fas fa-gear " ></i>
                              </div>
                          </div>
                      </template>
                      <div class="overflow-y-auto" style="max-height: 62vh" v-if="usuariosAsociados[1]">
                          <div v-if="usuariosAsociados[1].length == 0" class="flex justify-content-center align-items-center h-40rem">
                              <span class="text-sm text-secondary">No hay usuarios asociados</span>
                          </div>
                          <div class="mb-2" v-for="usuario in usuariosAsociados[1]" :key="usuario.idUsuario">
                              <div class="font-normal mb-2 w-full">
                                  <Avatar :label="usuario.nombres[0] + usuario.apellidos[0]" class="mr-2" shape="circle" style="background-color:#003880; color: #ffffff" />{{ usuario.nombreUsuario }}
                              </div>
                          </div>
                      </div>
                  </Fieldset>
              </div>
          </div>
          <Dialog v-model:visible="agregarUsuario" :style="{ width: '50vw' }" header="Usuarios asociados" :modal="true" :closable="true" :resizable="false">
              <PickList id="UsuariosAsociados" class="p-picklist-sm" v-model="usuariosAsociadasFiltrado"
                  :showSourceControls="false" :showTargetControls="false" :dataKey="idUsuario"
                  @movelAllToTarget="moveToAsociadas($event)" @moveToTarget="moveToAsociadas($event)" @moveToSource="moveToDisponibles($event)" @moveAllToSource="moveToDisponibles($event)">
                  <template #sourceheader>
                      <div>Disponibles</div>
                      <div class="w-full mt-2">
                          <IconField iconPosition="left">
                              <InputIcon>
                                  <i class="pi pi-search" />
                              </InputIcon>
                              <InputText class="w-full" v-model="filtroUsuariosDisponibles" type="text"
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
                              <InputText class="w-full" v-model="filtroUsuariosAsociadas" type="text"
                                  size="small" variant="filled" placeholder="Buscar..." />
                          </IconField>
                      </div>
                  </template>
                  <template #item="slotProps">
                      <div class="flex flex-wrap align-items-center">
                          <div class="flex-1 flex flex-column">
                              <span class="picklistName">{{ slotProps.item.nombres + " " +  slotProps.item.apellidos}}</span>
                              <div class="flex align-items-center">
                                  <span class="picklistURL">{{ slotProps.item.perfil }}</span>
                              </div>
                          </div>
                      </div>
                  </template>
              </PickList>
          </Dialog>
      </div>
  </div>
</template>

<script>
import { push } from 'notivue'
import api from '@/utilities/api.js'
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';

export default {
  name: 'PerfilesView',
  components: { IconField, InputIcon },
  data() {
      return {
          perfil: {
              vistas: [],
              usuarios: [],
          },
          idPerfil: this.$route.params.idPerfil,
          checked: false,
          errors: {},
          agregarUsuario: false,
          usuariosAsociados: [],
          filtroUsuariosAsociadas: '',
          filtroUsuariosDisponibles: '',
          usuariosAsociadasFiltrado: [],
      }
  },
  watch: {
      filtroUsuariosDisponibles: function (val) {
          if (val == '') this.usuariosAsociadasFiltrado[0] = this.usuariosAsociados[0];
          else this.usuariosAsociadasFiltrado[0] = this.usuariosAsociados[0].filter(x => (x.nombres + ' ' + x.apellidos).toLowerCase().includes(val.toLowerCase()));
      },
      filtroUsuariosAsociadas: function (val) {
          if (val == '') this.usuariosAsociadasFiltrado[1] = this.usuariosAsociados[1];
          else this.usuariosAsociadasFiltrado[1] = this.usuariosAsociados[1].filter(x => (x.nombres + ' ' + x.apellidos).toLowerCase().includes(val.toLowerCase()));
      },
  },
  mounted() {
      this.LoadData();
  },
  methods: {
      getModulos(views) {
          return views.filter(x => views.filter((y) => y.modulo.idModulo == x.modulo.idModulo).length > 1).map(x => x.modulo).filter((value, index, self) => self.findIndex(t => t.idModulo === value.idModulo) === index);
      },
      async LoadData() {
          await this.LoadPerfil();
          await this.LoadPermisos();
          await this.LoadUsuariosAsociados();
          await this.LoadUsuarios();
      },
      async LoadPerfil() {
          if (this.idPerfil) {
              const response = await api.get(`/api/Perfiles/${this.idPerfil}`);
              if (response.data) {
                  this.perfil = { ...this.perfil, ...response.data };
              }
              else push.warning(response.data.message);
          }
      },
      async LoadPermisos() {
          const response = (this.idPerfil) ? await api.get(`/api/Perfiles/GetPermisos/${this.idPerfil}`) : await api.get('/api/Perfiles/GetPermisos');
          if (response.data) {
              this.perfil.vistas = response.data;
              this.perfil.vistas.forEach(vista => {
                  if (vista.nombre === 'Inicio') {
                      vista.permiso = true;
                  }
              });
          }
          else push.warning(response.data.message);
      },
      async LoadUsuariosAsociados() {
          if (this.idPerfil) {
              const response = await api.get(`/api/Perfiles/GetUsuarios/${this.idPerfil}`);
              if (response.data) {
                  this.usuariosAsociados[1] = response.data;
              }
              else push.warning(response.data.message);
          }
      },
      async LoadUsuarios() {
          const response = await api.get(`/api/Usuarios`);
          if (response.data) {
              if (this.usuariosAsociados[1] == null) {
                  this.usuariosAsociados[0] = response.data;
                  this.usuariosAsociadasFiltrado[0] = response.data;

                  this.usuariosAsociados[1] = [];
                  this.usuariosAsociadasFiltrado[1] = [];
              }
              else
              {
                  this.usuariosAsociados[0] = response.data.some(x => this.usuariosAsociados[1].some(y => y.idUsuario == x.idUsuario)) ? response.data.filter(x => !this.usuariosAsociados[1].some(y => y.idUsuario == x.idUsuario)) : response.data;
                  this.usuariosAsociadasFiltrado[0] = this.usuariosAsociados[0];

                  this.usuariosAsociadasFiltrado[1] = this.usuariosAsociados[1];
              }
          }
          else push.warning(response.data.message);
      },
      async Guardar() {
          this.perfil.usuarios = this.usuariosAsociados[1];

          const response = (this.idPerfil) ? await api.put('/api/Perfiles', this.perfil) : await api.post('/api/Perfiles', this.perfil);

          this.errors = {};

          if (response.data.success) {
              push.success(response.data.message);
              this.$router.push({ name: 'Perfiles' });
          }
          else {
              this.errors = response.data.data;
              push.warning(response.data.message);
          }
      },
      moveToAsociadas(event) {
          const listadoUsuarios = event.items;
          this.usuariosAsociados[1].push(...listadoUsuarios);
          this.usuariosAsociados[0] = this.usuariosAsociados[0].filter(x => !listadoUsuarios.some(y => y.idUsuario === x.idUsuario));

          if (this.filtroUsuariosDisponibles === '') {
              this.usuariosAsociadasFiltrado[0] = this.usuariosAsociados[0];
          } else {
              this.usuariosAsociadasFiltrado[0] = this.usuariosAsociados[0].filter(x => (x.nombres + ' ' + x.apellidos).toLowerCase().includes(this.filtroUsuariosDisponibles.toLowerCase()));
          }

          if (this.filtroUsuariosAsociadas === '') {
              this.usuariosAsociadasFiltrado[1] = this.usuariosAsociados[1];
          } else {
              this.usuariosAsociadasFiltrado[1] = this.usuariosAsociados[1].filter(x => (x.nombres + ' ' + x.apellidos).toLowerCase().includes(this.filtroUsuariosAsociadas.toLowerCase()));
          }
      },
      moveToDisponibles(event) {
          const listadoUsuarios = event.items;

          this.usuariosAsociados[0].push(...listadoUsuarios);
          this.usuariosAsociados[1] = this.usuariosAsociados[1].filter(x => !listadoUsuarios.some(y => y.idUsuario === x.idUsuario));

          if (this.filtroUsuariosDisponibles === '') {
              this.usuariosAsociadasFiltrado[0] = this.usuariosAsociados[0];
          } else {
              this.usuariosAsociadasFiltrado[0] = this.usuariosAsociados[0].filter(x => (x.nombres + ' ' + x.apellidos).toLowerCase().includes(this.filtroUsuariosDisponibles.toLowerCase()));
          }

          if (this.filtroUsuariosAsociadas === '') {
              this.usuariosAsociadasFiltrado[1] = this.usuariosAsociados[1];
          } else {
              this.usuariosAsociadasFiltrado[1] = this.usuariosAsociados[1].filter(x => (x.nombres + ' ' + x.apellidos).toLowerCase().includes(this.filtroUsuariosAsociadas.toLowerCase()));
          }
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
