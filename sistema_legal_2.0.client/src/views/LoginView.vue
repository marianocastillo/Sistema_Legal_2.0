<template>

  <body>
    <div class="login-container ">
      <div class="welcome-section">
        <div class="logo">
          <img src="/src/assets/LogoBlancoLogin1.png" alt="Logo">
        </div>
      </div>
      <div class="login-section">
        <h2>Sistema Legal</h2>
        <p>Inicie sesión para continuar </p>
        <form>
          <div class="input-container">
            <span class="input-icon">
              <font-awesome-icon icon="user" />
            </span>
            <input type="text" placeholder="Usuario" v-model="credentials.UserName" />
          </div>

          <div class="password-container">
            <input :type="showPassword ? 'text' : 'password'" placeholder="Contraseña">

            <span class="password-icon" v-if="showPassword" @click="togglePasswordVisibility()">
              <i class='fas fa-eye'></i>
            </span>
            <span class="password-icon" v-else @click="togglePasswordVisibility()">
              <i class='fas fa-eye-slash'></i>
            </span>
          </div>
          <button type="submit" class="login-btn">Iniciar sesión</button>
        </form>

      </div>
    </div>
  </body>

</template>



<script>
import { push } from 'notivue';
import api from '@/utilities/api.js'


export default {
  name: 'LoginVue',
  data() {
    return {
      credentials: {
        UserName: "",
        Password: "",
      },
      invalid: {
        UserName: false,
        Password: false,
      },
      showPassword: false,
    };
  },

  mounted() {
        localStorage.removeItem('sessionExpireTime');
        localStorage.removeItem('user');
        localStorage.removeItem('token');
    },

  methods: {
    togglePasswordVisibility() {
      this.showPassword = !this.showPassword;
    },

    async LogIn(event) {
            event.preventDefault();
            this.invalid.UserName = this.invalid.Password = false;

            if (this.credentials.UserName === "" || this.credentials.Password === "") {
                (this.credentials.UserName === "" ? this.invalid.UserName = true : this.invalid.UserName = false);
                (this.credentials.Password === "" ? this.invalid.Password = true : this.invalid.Password = false);
                push.warning("Por favor, rellene todos los campos")
            }
            else {
                const response = await api.post('/api/Account', this.credentials);
                if (response.data.success) {
                    const token = response.data.token;
                    localStorage.setItem("token", token);
                    localStorage.setItem("user", response.data.data.usuario.nombreUsuario);

                    push.success(response.data.message);

                    const { usuario } = response.data.data;

                    this.$store.commit('setUser', usuario);

                    const sessionExpireTime = new Date().getTime() + (30 * 60 * 1000);
                    localStorage.setItem('sessionExpireTime', sessionExpireTime);

                    this.$router.push('/home');
                }
                else push.warning(response.data.message);
            }
        },
  },
};
</script>









<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: 'Poppins', sans-serif;
}

body {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;


}

input[type="text"]:focus,
input[type="password"]:focus {
  border-color: #013579;
  /* Cambia el color del borde al hacer focus */
  outline: none;
  /* Sin borde de enfoque del navegador */

}


.login-container {
  display: flex;
  flex-direction: row;
  width: 90%;
  height: 50%;
  max-width: 1000px;
  background-color: white;
  border-radius: 10px 80px 10px 100px;
  overflow: hidden;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
}

.welcome-section {
  width: 50%;
  background: linear-gradient(135deg, #013579 60%, #0963d8 100%, #1668d3 100%);
  color: white;
  padding: 40px;
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 0px 100px 0px 100px;

}

.welcome-section .logo h1 {
  font-size: 2rem;
  margin-bottom: 20px;
}

.logo img {
  width: 270px;
  height: 250px;
  margin-bottom: 20px;
}

.welcome-section h2 {
  font-size: 1.8rem;
  margin-bottom: 10px;
}

.welcome-section p {
  margin-bottom: 30px;
  text-align: center;
  font-size: 1rem;
}

.sign-in-btn {
  background-color: transparent;
  border: 2px solid white;
  color: white;
  padding: 10px 20px;
  font-size: 1rem;
  border-radius: 20px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.sign-in-btn:hover {
  background-color: white;
  color: #013579;
}

.login-section {
  width: 50%;
  padding: 40px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: white;
  border: 10px #013579;
  border-radius: 0px 100px 0px 0px;
  position: relative;


}

.login-section h2 {
  font-size: 1.8rem;
  margin-bottom: 15px;
  color: #013579;
  text-align: center;
}

.login-section p {
  margin-bottom: 30px;
  text-align: center;
  color: #555;
  font-size: 1rem;
}

.login-section form {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 300px;
}

.login-section input {
  margin-bottom: 15px;
  padding: 10px;
  border-radius: 20px;
  border: 1px solid #ddd;
  font-size: 1rem;
  width: 100%;
  background-color: rgba(0, 56, 128, 0.1);
  color: black;
}

.forgot-password {
  align-self: flex-end;
  margin-bottom: 20px;
  color: #013579;
  text-decoration: none;
}

.login-btn {
  background-color: #013579;
  color: white;
  padding: 10px;
  border-radius: 20px;
  border: none;
  cursor: pointer;
  font-size: 1rem;
  width: 100%;
  transition: background-color 0.3s ease;
}

.login-btn:hover {
  background-color: #2378e7;
}

.login-section a {
  color: #013579;
  text-decoration: none;
}

/* Here start the media query for responsive page */

/* Media query para dispositivos móviles (pantallas pequeñas) */
@media (max-width: 768px) {
  .login-container {
    flex-direction: column;
    /* Cambiar la dirección de flex a columna */
    width: 90%;
    /* Reducir el ancho */
    height: auto;
    /* Ajustar la altura automáticamente */
    max-width: 500px;
    /* Ancho máximo para pantallas pequeñas */
    border-radius: 10px;
    /* Ajustar el radio del borde */
  }

  .welcome-section {
    width: 100%;
    /* Hacer que ocupe todo el ancho */
    border-radius: 10px 10px 0 0;
    /* Ajustar el borde */
    padding: 20px;
    /* Reducir el padding */
  }

  .login-section {
    width: 100%;
    /* Hacer que ocupe todo el ancho */
    border-radius: 0 0 10px 10px;
    /* Ajustar el borde */
    padding: 20px;
    /* Reducir el padding */
  }

  .logo img {
    width: 200px;
    /* Reducir el tamaño del logo */
    height: auto;
    /* Mantener la proporción */
    margin-bottom: 10px;
    /* Reducir el margen inferior */
  }

  .login-section form {
    max-width: 100%;
    /* Ajustar el ancho del formulario */
  }
}

/* Media query para pantallas medianas (tabletas) */
@media (min-width: 769px) and (max-width: 1024px) {
  .login-container {
    width: 80%;
    /* Reducir el ancho */
    max-width: 800px;
    /* Ancho máximo para pantallas medianas */
    height: 400px;
  }

  .welcome-section,
  .login-section {
    padding: 30px;
    /* Ajustar el padding */
  }

  .logo img {
    width: 240px;
    /* Ajustar el tamaño del logo */
    height: auto;
    /* Mantener la proporción */
  }
}

/* Media query para pantallas grandes (escritorios y más grandes) */
@media (min-width: 1025px) {
  .login-container {
    width: 70%;
    /* Ancho mayor para pantallas grandes */
    max-width: 1000px;
    /* Limitar el ancho máximo */
    height: 400px;
  }

  .welcome-section,
  .login-section {
    padding: 40px;
    /* Mantener el padding */
  }

  .logo img {
    width: 270px;
    /* Mantener el tamaño del logo */
    height: 250px;
  }
}

.password-container {
  position: relative;
  /* Asegura que el icono esté dentro del input */
}

.password-container input {
  padding-right: 40px;
  /* Añade espacio para el icono dentro del input */
}

.password-icon {
  position: absolute;
  top: 40%;
  right: 10px;
  /* Alinea el icono a la derecha dentro del input */
  transform: translateY(-50%);
  cursor: pointer;
  color: #aaa;
  /* Color del icono */
}

.password-icon:hover {
  color: #013579;
  /* Cambia de color al hacer hover */
}











/* Estilos de los inputs */
.input-container {
  position: relative;
  margin-bottom: 20px;
}

input {
  width: 100%;
  padding: 10px 40px;
  font-size: 16px;
  border: 1px solid #ccc;
  border-radius: 5px;
}

/* Ícono de usuario/candado */
.input-icon {
  position: absolute;
  right: 10px;
  top: 40%;
  transform: translateY(-50%);
  color: gray;
}

/* Ícono de ojo en la contraseña */
.password-icon {
  position: absolute;
  right: 10px;
  top: 40%;
  transform: translateY(-50%);
  cursor: pointer;
  color: gray;
}
</style>
