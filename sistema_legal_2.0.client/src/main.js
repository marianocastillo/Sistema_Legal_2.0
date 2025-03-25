import { createApp } from 'vue';
import PrimeVue from 'primevue/config';

import {dom, library} from '@fortawesome/fontawesome-svg-core';
import { fas } from '@fortawesome/free-solid-svg-icons'
import { fab } from '@fortawesome/free-brands-svg-icons';
import { far } from '@fortawesome/free-regular-svg-icons';

import App from './App.vue';
import Aura from '@primeuix/themes/aura';
import router from './router';
import { createNotivue } from 'notivue'
import 'notivue/notification.css'
import 'notivue/animations.css'

import { faEye, faUser, faLock, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';

library.add(faEye, faUser,  faLock, faEyeSlash);

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
app.use(router);

app.mount('#app');
app.component('font-awesome-icon', FontAwesomeIcon);
