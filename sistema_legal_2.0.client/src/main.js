import { createApp } from 'vue';
import PrimeVue from 'primevue/config';
import App from './App.vue';
import Aura from '@primeuix/themes/aura';
import router from './router';
import { createNotivue } from 'notivue'
import 'notivue/notification.css'
import 'notivue/animations.css'

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
