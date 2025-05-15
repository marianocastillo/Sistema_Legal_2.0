<!-- eslint-disable vue/multi-word-component-names -->
<template>
    <!-- <div class="cupula-logo-container d-flex justify-content-center align-items-center">
        <div class="cupula-logo"></div>
    </div> -->
    <div class="w-100">
           <!-- <h3>Documentación Importante del Sistema</h3> -->
       </div>
    <div class="d-flex justify-content-start align-items-start">

        <Card style="width: 25em !important" class="p-0 mx-4 my-2" id="Criterios" @click="MostrarContenido('Aceptacion')">
            <template #header>
                <div class="imagen w-100 m-0 rounded">

                </div>

            </template>
            <template #title> Criterios de Aceptación </template>

            <template #content>
                <p class="m-0">

                   Para conocer los detalles de los criterios de aceptación, <span style="color: rgb(0, 56,118);"><strong>haga clic aquí</strong></span>
                </p>
            </template>
        </Card>
        <Card style="width: 25em !important" class="p-0 mx-4 my-2" id="Documentos" @click="MostrarContenido('Documents')">
            <template #header>
                <div class="imagen2 w-100 m-0 rounded">

                </div>

            </template>
            <template #title>Documentos Obligatorios </template>

            <template #content>
                <p class="m-0">
                    Para conocer los documentos obligatorios para la solicitud <span style="color: rgb(0, 56,118);"><strong>haga clic aquí</strong></span>
                </p>
            </template>
        </Card>

        <Card style="width: 25em !important" class="p-0 mx-4 my-2" id="rechazo" @click="MostrarContenido('rechazos')">
            <template #header>
                <div class="imagen3 w-100 m-0 rounded">

                </div>

            </template>
            <template #title>Motivos de Rechazo</template>
            <div class="card">
        <ProgressBar mode="indeterminate" style="height: 6px"></ProgressBar>
    </div>
            <template #content>
                <p class="m-0">
                    Para conocer los detalles de los motivos de rechazos <span style="color: rgb(0, 56,118);"><strong>haga clic aquí</strong></span>

                </p>
            </template>
        </Card>

    </div>

    <Dialog v-model:visible="visible" modal :header="modalLabel" :style="{ width: '50vw' }">
        <div v-if="Criterio">
             <ol class="text-lg">
                <li v-for="elemento in CriterioAprobacion" :key="elemento.id">
                    {{ elemento.criterio }}
                </li>
             </ol>
        </div>

        <div v-if="Rechazos">
           <ol>
               <li  v-for="index in motivosRetorno" :key="index.id"> {{ index.descripcion }}</li>
           </ol>
        </div>
        <div v-if="DocObligatorios">
             <ol>
                <li>
                    Oficio de la Máxima Autoridad quien solicita el Registro de la Firma.
                </li>
                <li>
                   Tarjeta con la rúbrica de las Firmas a registrar de los servidores públicos.
                </li>
                <li>
                    Copia por delante y por detrás de las cédulas de los servidores públicos a registrar la firma
                </li>
                <li>
                    Acción de personal de la designación departamental en un ministerio o direcciones
                     generales correspondiente a un servidor público objeto de que su firma sea registrada./
                     Decreto de los funcionarios designados por el Poder Ejecutivo
                </li>
             </ol>
        </div>
    </Dialog>

    <Dialog v-model:visible="changePassDialog" class="changePassDialog" :closable="false" modal>
        <template #header>
            <p style="font-size: 18px;" class="m-0"><b>{{ modalLabel }}<!--Cambio de contraseña--></b></p>
        </template>
        <div>
            <Message severity="warn" class="alertDelete" :closable="false">{{warningMessage}}<!--Se requiere actualizar su contraseña para continuar.--> </Message>
            <div class="password-input-wrapper">
                <Password v-model="password" :feedback="false" :toggleMask="true" placeholder="Contraseña"
                    class="w-100 mb-3" />
                    <small class="p-error" v-if="Errors['NuevaContraseña']">{{Errors["NuevaContraseña"][0]}}</small>
            </div>
            <div class="password-input-wrapper">
                <Password v-model="confirmPassword" :feedback="false" :toggleMask="true"
                    placeholder="Confirmar la contraseña" class="w-100 mb-3" />
            </div>
            <div class="d-flex justify-content-end pt-3 pb-1">
                <Button @click="changePassword" icon="pi pi-save" severity="secondary" style="background-color: #00224d;"
                    label="Cambiar contraseña" size="small"></Button>
            </div>
        </div>
        <template #footer>
        </template>
    </Dialog>
</template>

<script>
import api from "@/utilities/api";



export default {
    setup() {
        const toast = useToast();
        return { toast }
    },
    mounted() {
        const userData = this.$store.getters.getUserData;
        console.log(userData);
        this.userInfo = userData;
        if (userData.cambioClave) this.changePassDialog = true;
        this.getMotivoCambioEstatus();
        this.getCriteriosAprobacion();
        this.calcularDiasExpiracion();
    },
    data() {
        return {
            changePassDialog: false,
            userInfo: {},
            password: "",
            confirmPassword: "",
            motivosRetorno: [],
            CriterioAprobacion: [],
            visible: false,
            Criterio: false,
            Rechazos: false,
            DocObligatorios: false,
            modalLabel:"Cambio de contraseña",
            warningMessage:"Se requiere actualizar su contraseña para continuar.",
            Errors: {}
        };
    },
    methods: {
        async changePassword() {

            if (this.userInfo != null) {
                const CambiarContraseña = {
                    idUsuario: this.userInfo.idUsuario,
                    NuevaContraseña: this.password,
                    ConfirmarContraseña: this.confirmPassword
                }
                const response = await api.put("api/Account/CambiarContraseña", CambiarContraseña)
                if (response.data.success) {
                    this.toast.success(response.data.message, toastProperties);
                console.log(this.data)

                    this.changePassDialog = false;
                }
                else {
                    this.Errors = response.data.errors;
                    console.log(this.Errors)
                    this.toast.warning(response.data.message, toastProperties);
                }
            }
        },
        async getMotivoCambioEstatus() {

            const response = await api.get(`api/SolicitudFirma/GetMotivosCambioEstatus`);
            if (response.data != null) {
                response.data.forEach((element) => {
                    this.objMotivo = {
                        activo: element.activo,
                        descripcion: element.descripcion,
                        nombre: element.nombre,
                        idMotivo: element.idMotivo
                    }
                    this.motivosRetorno.push(this.objMotivo);
                })


            }
            else {
                this.toast.error("Error al cargar los detalles de la solicitud", toastProperties);
            }

        },
        async getCriteriosAprobacion() {

            const response = await api.get(`api/Usuarios/CargarCriterios`);
            if (response.data != null) {
                this.CriterioAprobacion = response.data


            }
            else {
                this.toast.error("Error al cargar los detalles de la solicitud", toastProperties);
            }

        },
        MostrarContenido(opcion) {
            this.visible = true;

            if (opcion == "Aceptacion") {
                this.Criterio = true
                this.Rechazos = false
                this.DocObligatorios = false
                this.modalLabel='CRITERIOS DE ACEPTACIÓN'
            }
            if (opcion == "Documents") {
                this.Criterio = false
                this.Rechazos = false
                this.DocObligatorios = true
                this.modalLabel='DOCUMENTOS OBLIGATORIOS'
            }
            if (opcion == "rechazos") {
                this.Criterio = false
                this.Rechazos = true
                this.DocObligatorios = false
                this.modalLabel = 'MOTIVOS DE RECHAZOS'
            }
        },
        calcularDiasExpiracion() {
        if (this.userInfo && this.userInfo.ultCambioClave) {
            const fechaCambioClave = new Date(this.userInfo.ultCambioClave);
            const hoy = new Date();
            const diferenciaTiempo = hoy - fechaCambioClave;
            const diasTranscurridos = Math.floor(diferenciaTiempo / (1000 * 60 * 60 * 24));

            const diasRestantes = 90 - diasTranscurridos; // Suponiendo que expira en 90 días

            if (diasRestantes <= 0) {
                this.modalLabel = "Contraseña Expirada";
                this.warningMessage = `Tu contraseña ha expirado. Para continuar, es necesario actualizarla.`;
                this.changePassDialog = true;
                return 0; // Evita valores negativos
            }
            // else if (diasRestantes <= 5) {
            //     this.warningMessage = `Tu contraseña expirará en ${diasRestantes} días. Se recomienda cambiarla pronto.`;
            // } else {
            //     this.warningMessage = `Tu contraseña sigue vigente.`;
            // }

            return diasRestantes;
        }
        return null;
        }



    },
}

</script>


<style>
.p-datatable {
    font-size: small !important;
}

.AccordionContent {
    font-size: 16px;
    width: 260px;
}

.AccordionButton {
    padding: 2rem !important;
    width: 100%;
    height: 100%;
}

.cupula-logo-container {
    min-height: 88vh;
    min-width: 50vw;

}

.cupula-logo {
    background-image: url('../assets/cupula-half.png');
    background-repeat: no-repeat;
    background-position: right;
    background-size: contain;
    height:450px;
    width: 100%;
    margin-right: 0%;
    z-index: -1;
    position: relative;
}

.login-body {
    overflow: hidden;
}

.p-password-input {
    width: 100%;
}

.p-password .p-icon:hover {
    cursor: pointer;
}

.imagen {
    background-image: url("../assets/Criterios\ de\ Aprobacion.png");
    background-repeat: no-repeat;
    background-position: center;
    background-size: cover;
    width: 100%;
    height: 170px;
}

.imagen2 {
    background-image: url("../assets/Documentos\ Obligatorios.png");
    background-repeat: no-repeat;
    background-position: center;
    background-size: cover;
    width: 100%;
    height: 170px;
}

.imagen3 {
    background-image: url("../assets/Motivos\ de\ Rechazos.png");
    background-repeat: no-repeat;
    background-position: center;
    background-size: cover;
    width: 100%;
    height: 170px;

}

#Criterios:hover {
    cursor: pointer;
    transition: ease-in-out 0.3s;
    transform: translate(2px, 2px);
}

#Documentos:hover {
    cursor: pointer;
    transition: ease-in-out 0.3s;
    transform: translate(2px, 2px);
}

#rechazo:hover {
    cursor: pointer;
    transition: ease-in-out 0.3s;
    transform: translate(2px, 2px);
}
</style>
