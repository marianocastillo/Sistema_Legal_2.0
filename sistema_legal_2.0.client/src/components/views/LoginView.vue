<template>
  <div class="login-outer">
    <div class="login-container">
      <!-- Logo -->
      <div class="welcome-section">
        <div class="logo">
          <img src="/src/assets/LogoBlancoLogin1.png" alt="Logo" />
        </div>
      </div>

      <!-- Login -->
      <div class="login-section">
        <h2>Sistema Sileg 2.0</h2>
        <p>Inicie sesión para continuar</p>

        <form @submit.prevent="LogIn" class="form-wrapper">
          <InputText
            v-model="credentials.userName"
            :class="{ 'input-error': invalid.userName }"
            placeholder="Usuario"
            class="custom-input"
          />

          <div class="password-wrapper">
            <Password
              v-model="credentials.password"
              :feedback="false"
              toggleMask
              placeholder="Contraseña"
              autocomplete="current-password"
              class="custom-input"
              :class="{ 'input-error': invalid.password }"
            />
          </div>

          <PrimeButton
            :label="loading ? 'Cargando...' : 'Iniciar sesión'"
            icon="pi pi-sign-in"
            class="login-btn"
            :disabled="loading"
            :loading="loading"
            type="submit"
          />
        </form>
      </div>
    </div>
  </div>

  <Notivue v-slot="item">
    <NotivueSwipe :item="item">
      <Notifications :item="item" />
    </NotivueSwipe>
  </Notivue>
</template>

<script>
import { push } from 'notivue'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import PrimeButton from 'primevue/button'
import api from '@/utilities/api.js'
import { Notivue, Notifications, NotivueSwipe } from 'notivue'


export default {
  name: 'LoginView',
  components: {
    InputText,
    Password,
    PrimeButton,
    Notivue,
    Notifications,
    NotivueSwipe
  },
  data() {
    return {
      credentials: { userName: '', password: '' },
      loading: false,
      invalid: { userName: false, password: false },
      mockUsers: [
        { userName: 'admin', password: '1234', nombreUsuario: 'Ana Sánchez', rol: 'Administrador' },
        { userName: 'juan', password: 'abcd', nombreUsuario: 'Juan Pérez', rol: 'Abogado' },
        { userName: 'carla', password: '5678', nombreUsuario: 'Carla Ruiz', rol: 'Cliente' }
      ]
    }
  },
  mounted() {
    localStorage.removeItem('sessionExpireTime')
    localStorage.removeItem('user')
    localStorage.removeItem('token')
  },
  methods: {
    async LogIn() {

      const useMockLogin = false;
      this.loading = true
      this.invalid.userName = !this.credentials.userName
      this.invalid.password = !this.credentials.password

      // console.log('VITE_USE_MOCK:', import.meta.env.VITE_USE_MOCK)

      if (this.invalid.userName || this.invalid.password) {
        push.warning('Por favor, rellene todos los campos')
        this.loading = false
        return
      }

      if (useMockLogin) {
        await this.handleMockLogin()
      } else {
        await this.handleRealLogin()
      }

      this.loading = false
    },

    async handleMockLogin() {
      await new Promise(resolve => setTimeout(resolve, 1000))
      const user = this.mockUsers.find(
        u => u.userName === this.credentials.userName && u.password === this.credentials.password
      )

      if (user) {
        const token = 'mockToken'
        localStorage.setItem('token', token)
        localStorage.setItem('usuario', JSON.stringify({ nombre: user.nombreUsuario, rol: user.rol}))
        localStorage.setItem('sessionExpireTime', new Date().getTime() + 30 * 60 * 1000)
        this.$store.commit('setUser', user)
        push.success(`Bienvenido ${user.nombreUsuario}`)
        this.$router.push('/drawer/home')
      } else {
        push.warning('Usuario o contraseña incorrectos')
      }
    },

    async handleRealLogin() {
    try {
       const response = await api.post('https://localhost:7177/api/Auth', this.credentials)
    if (response.data.success) {
      const token = response.data.token
      const { usuario } = response.data.data

      localStorage.setItem('token', token)
      localStorage.setItem('user', usuario.nombreUsuario)
      localStorage.setItem('idUsuario', usuario.idUsuario)
      localStorage.setItem('idPerfil', usuario.idPerfil)
      localStorage.setItem('sessionExpireTime', new Date().getTime() + 30 * 60 * 1000)
      localStorage.setItem('usuario', JSON.stringify({
        idUsuario: usuario.idUsuario,
        nombre: usuario.nombreUsuario,
        rol: nombrePerfil(usuario.idPerfil),
        perfil: usuario.idPerfil
      }))

      this.$store.commit('setUser', usuario)
      push.success(response.data.message)
      if(usuario.idPerfil == 4 ){
          this.$router.push('/drawer/inicio')
        } else{
          this.$router.push('/drawer/home')
        }

      // this.$router.push('/drawer/home')

      // setTimeout(() => window.location.reload(), 500)
    } else {
      push.warning(response.data.message)
    }
  } catch (error) {
    console.error('Error en login:', error)
    push.warning('Hubo un error al intentar iniciar sesión. Intenta nuevamente.')
  }
    }

  }
}



const nombrePerfil = (idPerfil) => {
  switch (idPerfil) {
    case 1:
      return 'Administrador';
    case 2:
      return 'Supervisor';
    case 3:
      return 'Digitador';
    case 4:
      return 'Abogado Litigante';
    default:
      return 'Perfil desconocido';
  }
};


</script>

<style scoped>
.login-outer {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100vh;
}

.login-container {
  display: flex;
  flex-direction: row;
  width: 70%;
  max-width: 1000px;
  background-color: white;
  border-radius: 10px 80px 10px 100px;
  overflow: hidden;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
  animation: fadeSlideUp 0.8s ease-out;
}

.welcome-section {
  width: 50%;
  background: linear-gradient(135deg, #013579 60%, #0963d8 100%);
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 50px;
  border-radius: 0 100px 0 100px;
}

.logo img {
  width: 270px;
  height: 250px;
}

.login-section {
  width: 50%;
  padding: 60px 60px 30px 60px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.login-section h2 {
  font-size: 2rem;
  font-weight: 800;
  margin-bottom: 30px;
  color: #013579;
  text-align: center;
}

.login-section p {
  margin-bottom: 20px;
  text-align: center;
  color: #555;
  font-size: 1.2rem;
}

.form-wrapper {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 300px;
}

/* ✅ InputText de PrimeVue */
:deep(.p-inputtext) {
  border-radius: 20px !important;
  background-color: rgba(0, 56, 128, 0.1) !important;
  border: 1px solid #ddd !important;
  font-size: 1.2rem !important;
  padding: 10px 16px !important;
  color: black !important;
  width: 100%;
  transition: all 0.3s ease-in-out;
  margin-bottom: 15px;
}

:deep(.p-inputtext:focus) {
  box-shadow: 0 0 0 3px rgba(1, 53, 121, 0.2) !important;
  outline: none !important;
}

/* ✅ Password de PrimeVue */
:deep(.p-password) {
  width: 100%;
  margin-bottom: 15px;
  position: relative;
}

:deep(.p-password-input) {
  border-radius: 20px !important;
  background-color: rgba(0, 56, 128, 0.1) !important;
  border: 1px solid #ddd !important;
  font-size: 1.2rem !important;
  padding: 10px 16px !important;
  color: black !important;
  width: 100%;
  transition: all 0.3s ease-in-out;
}


/* ✅ Centrado perfecto del icono del password (ojo) */
:deep(.p-password .p-password-toggle-mask) {
  position: absolute !important;
  top: 50% !important;
  right: 14px !important;
  transform: translateY(-50%) !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  height: 100%;
  cursor: pointer;
  z-index: 2;
}

:deep(.p-password .p-password-toggle-icon) {
  color: #aaa !important;
  font-size: 1rem !important;
}

:deep(.p-password .p-password-toggle-icon:hover) {
  color: #013579 !important;
}

:deep(.p-password-toggle-mask-icon) {
  top: 35% !important; /* Centrado visual */
  width: 20px !important;
  height: 18px !important;
}

.login-btn :deep(.p-button-label) {
  font-size: 1.1rem !important;
  font-weight: 600 !important;
  letter-spacing: 0.5px;
}

.login-btn {
  background: linear-gradient(135deg, #013579, #0056b3) !important;
  color: white !important;
  padding: 14px 24px !important;
  border-radius: 25px !important;
  border: none !important;
  font-size: 1.1rem !important;
  font-weight: 600 !important;
  letter-spacing: 0.5px !important;
  width: 100% !important;
  cursor: pointer !important;
  transition: all 0.3s ease !important;
  box-shadow: 0 4px 8px rgba(1, 53, 121, 0.2) !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  gap: 10px !important;
}

.login-btn :deep(.p-button-icon) {
  font-size: 1.2rem !important;
}

.login-btn:hover {
  background: linear-gradient(135deg, #21559b, #0056b3) !important;
  box-shadow: 0 6px 16px rgba(1, 53, 121, 0.3) !important;
  transform: translateY(-2px);
}




/* ✅ Responsive */
@media (max-width: 768px) {
  .login-container {
    flex-direction: column;
    width: 90%;
    height: auto;
    max-width: 500px;
    border-radius: 10px;
  }

  .welcome-section {
    width: 100%;
    border-radius: 10px 10px 0 0;
    padding: 20px;
  }

  .login-section {
    width: 100%;
    border-radius: 0 0 10px 10px;
    padding: 20px;
  }

  .logo img {
    width: 200px;
    height: auto;
    margin-bottom: 10px;
  }

  .form-wrapper {
    max-width: 100%;
  }
}

/* ✅ Animación de entrada */
@keyframes fadeSlideUp {
  0% {
    opacity: 0;
    transform: translateY(40px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
