import { createApp } from 'vue';
import store from "./store";
import PrimeVue from 'primevue/config';
import ToastService from 'primevue/toastservice'
import {dom, library} from '@fortawesome/fontawesome-svg-core';
import { fas } from '@fortawesome/free-solid-svg-icons'
import { fab } from '@fortawesome/free-brands-svg-icons';
import { far } from '@fortawesome/free-regular-svg-icons';
import App from './App.vue';
import Aura from '@primeuix/themes/aura';
import router from './router/router';
import { createNotivue } from 'notivue'
import 'notivue/notification.css'
import 'notivue/animations.css'
import { faEye, faUser, faLock, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';



import Column from 'primevue/column';
import ColumnGroup from 'primevue/columngroup';   // optional
import Row from 'primevue/row';
 // Importamos el router correctamente

import './assets/main.css';

//import 'notivue/notifications.css'
//import 'notivue/animations.css'
import 'primeflex/primeflex.css'
import 'primeicons/primeicons.css'
import Tooltip from 'primevue/tooltip';



library.add(faEye, faUser,  faLock, faEyeSlash  );

const app = createApp(App);
const notivue = createNotivue({
  position: 'top-right',
  limit: 4,
  enqueue: true,
  notifications: {
    global: {
      duration: 3000
    }
  }
})


library.add(fas, fab, far);
dom.watch()



app.use(notivue);
app.use(PrimeVue, { ripple: true, locale: {
  apply: 'Aplicar',
  clear: 'Limpiar',
},
theme: {
  preset: Aura
} });


app.directive('tooltip', Tooltip);

app.use(router);
app.use(store);
app.component('font-awesome-icon', FontAwesomeIcon);
app.mount('#app');
app.component('PrimeColumn', Column)
app.component('PrimeRow', Row)
app.component('ColumnGroup', ColumnGroup)
app.use(ToastService)
