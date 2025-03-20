<template>
    <div class="">
        <div class="">
            <div class="text-2xl font-bold" style="color: #003880;">Bienvenido a la plataforma de ejecución del gasto
            </div>
        </div>
        <Divider />
        <div class="">
            <div class="text-lg font-bold my-4" style="color: #003880;">Cargar archivo
                <Button @click="descargarPlantilla()" icon="fa-solid fa-file-excel" label="Descargar plantilla"
                    style="background-color: green; border-color: green; float:right;" />
            </div>
            <FileUpload name="demo[]" :customUpload="true" :fileLimit="1" ref="fileUploader" @clear="onClearFile"
                accept=".xls, .xlsx, application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                :maxFileSize="1000000" @select="onFileSelect">
                <template #empty>
                    <div class="p-2">
                        <div class="text-center">
                            <i class="pi pi-upload" style="font-size: 3rem"></i>
                            <p>Arrastre y suelte archivos aquí, o haga clic en el botón <b>Seleccionar</b> archivos.
                            </p>
                        </div>
                    </div>
                </template>
                <template #header="{ chooseCallback, clearCallback, files }">
                    <div class="flex flex-wrap justify-content-between align-items-center flex-1 gap-2">
                        <div class="flex gap-2">
                            <Button icon="fa-solid fa-file-excel" label="Seleccionar" @click="chooseCallback()" />
                            <Button icon="fa-solid fa-file-export" severity="secondary" label="Extraer datos"
                                @click="ExtraerXls" :disabled="!files || files.length === 0" />
                            <Button icon="pi pi-times" label="Limpiar" severity="danger" @click="clearCallback()"
                                :disabled="!files || files.length === 0" />
                        </div>
                    </div>
                </template>
                <template #content="{ files, messages }">
                    <div class="p-progressbar p-component p-progressbar-determinate">
                        <div class="p-progressbar-value p-progressbar-value-animate" style="display: flex" />
                    </div>
                    <div v-if="messages && messages.length > 0" class="mb-2">
                        <InlineMessage class="mb-2" v-for="msg of messages" :key="msg" severity="error">{{ msg }}
                        </InlineMessage>
                    </div>
                    <div v-if="files && files.length > 0">
                        <div class="p-fileupload-file p-2" v-for="file of files"
                            :key="file.name + file.type + file.size" data-pc-section="file">
                            <div class="p-2 d-flex justify-content-center align-items-center" style="width: 50px">
                                <img role="presentation" :src="XlsIcon" class="p-fileupload-file-thumbnail"
                                    width="35" />
                            </div>
                            <div class="p-fileupload-file-details">
                                <div class="p-fileupload-file-name">{{ file.name }}</div>
                                <span class="p-fileupload-file-size mr-2">{{
                                    FormatSize(file.size)
                                }}</span>
                                <Tag icon="pi pi-exclamation-triangle"
                                    class="p-badge p-component p-fileupload-file-badge" severity="warn"
                                    v-if="!archivoExtraido && !archivoSubido && !archivoSubidoConError">Pendiente para
                                    extraer data</Tag>
                                <Tag icon="pi pi-info-circle" class="p-badge p-component p-fileupload-file-badge"
                                    severity="info" v-if="archivoExtraido && !archivoSubido && !archivoSubidoConError">
                                    Data extraída</Tag>
                                <Tag icon="pi pi-check" class="p-badge p-component p-fileupload-file-badge"
                                    severity="success" v-if="archivoSubido && !archivoSubidoConError">Subido
                                    exitosamente</Tag>
                                <Tag icon="pi pi-times" class="p-badge p-component p-fileupload-file-badge"
                                    severity="danger" v-if="archivoSubidoConError">Archivo con anomalías, intentar de
                                    nuevo</Tag>
                            </div>
                        </div>
                    </div>
                </template>
            </FileUpload>
            <div v-if="archivoExtraido" class="my-2">
                <div class="flex-auto">
                    <label for="icondisplay" style="color: #003880;" class="font-bold block mb-2"> Fecha de ejecución
                        del
                        gasto </label>
                    <div class="flex">
                        <DatePicker v-model="fechaEjecucion" showIcon iconDisplay="input" inputId="icondisplay"
                            :maxDate="new Date()" />
                        <Button icon="fas fa-save" label="Guardar" class="ml-2" :disabled="fechaEjecucion == null"
                            @click="confirmSave($event)" />
                    </div>
                </div>
                <div class="card p-0 mt-2">
                    <div class="p-0">
                        <DataTable :value="registros" paginator :rows="10">
                            <Column field="referenciaConcepto" header="Cod. Ref CCP Concepto"></Column>
                            <Column field="concepto" header="Ref CCP Concepto"></Column>
                            <Column field="presupuestoVigenteAprobado" header="Pres. Vigente Aprobando">
                                <template #body="data">
                                    {{ formatNumber(data.data.presupuestoVigenteAprobado) }}
                                </template>
                            </Column>
                            <Column field="cantidadDocumento" header="Cantidad Documento">
                                <template #body="data">
                                    {{ formatNumber(data.data.cantidadDocumento) }}
                                </template>
                            </Column>
                            <Column field="libramientoAprobado" header="Libramiento Aprobado">
                                <template #body="data">
                                    {{ formatNumber(data.data.libramientoAprobado) }}
                                </template>
                            </Column>
                        </DataTable>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <ConfirmPopup />
</template>

<script>

import api from '@/utilities/api';
import { push } from 'notivue';
import XlsIcon from '@/assets/xls.png';


export default {
    data() {
        return {
            archivoExtraido: false,
            fechaEjecucion: null,
            registros: [],
            archivoSubido: false,
            archivoSubidoConError: false,
            selectedFiles: [],  // Lista de archivos seleccionados
        };
    },
    computed: {
        XlsIcon() {
            return XlsIcon;
        },
    },
    setup() {
        return {
            confirm: null,
        };
    },
    methods: {
        descargarPlantilla() {           
            window.open('/EjecuciondelGasto/Plantilla Ejecucion del gasto.XLSX', '_blank');
        },
        confirmSave(event) {
            this.$confirm.require({
                target: event.currentTarget,
                message: 'Estas seguro que deseas guardar?',
                icon: 'pi pi-exclamation-triangle',
                rejectProps: {
                    label: 'Cancelar',
                    severity: 'secondary',
                    outlined: true
                },
                acceptProps: {
                    label: 'Guardar'
                },
                accept: () => {
                    this.Guardar();
                },
                reject: () => {
                    this.$toast.add({ severity: 'error', summary: 'Rejected', detail: 'You have rejected', life: 3000 });
                }
            });
        },

        onFileSelect(event) {
            this.selectedFiles = event.files; // Actualiza los archivos seleccionados
        },
        onClearFile() {
            this.archivoExtraido = false;
            this.fechaEjecucion = null;
            this.registros = [];
            this.archivoSubido = false;
            this.archivoSubidoConError = false;
        },
        formatNumber(value) {
            if (typeof value === "number") {
                return value.toLocaleString("es-DO", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }
            return value;
        },
        async ExtraerXls() {
            if (!this.selectedFiles.length) return;

            const formData = new FormData();
            formData.append('file', this.selectedFiles[0]);

            try {
                const response = await api.post('/api/Files', formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                });

                if (response.data.success) {
                    this.registros = response.data.data;
                    this.archivoExtraido = true;
                    this.archivoSubidoConError = false;
                    this.archivoSubido = false;
                } else {
                    push.error(response.data.message);
                }
            } catch (error) {
                push.error(error.message);
            }
        },
        async Guardar() {
            if (!this.fechaEjecucion) {
                push.warning('Por favor, seleccione una fecha de ejecución del gasto.');
                return;
            }

            const ejecucion = {
                fechaEjecucion: this.fechaEjecucion,
                registros: this.registros,
            };

            try {
                const response = await api.post('/api/Files/Guardar', ejecucion);

                if (response.data.success) {
                    this.archivoSubido = true;
                    this.archivoSubidoConError = false;
                    this.archivoExtraido = true;
                    push.success(response.data.message);
                } else {
                    this.archivoSubidoConError = true;
                    this.archivoExtraido = false;
                    this.archivoSubido = false;
                    push.error(response.data.message);
                }
            } catch (error) {
                push.error(error.message);
            }
        },
        FormatSize(bytes) {
            const k = 1024;
            const dm = 3;
            const sizes = this.$primevue.config.locale.fileSizeTypes;

            if (bytes === 0) {
                return `0 ${sizes[0]}`;
            }

            const i = Math.floor(Math.log(bytes) / Math.log(k));
            const formattedSize = parseFloat((bytes / Math.pow(k, i)).toFixed(dm));

            return `${formattedSize} ${sizes[i]}`;
        },

    },
};
</script>

<style></style>