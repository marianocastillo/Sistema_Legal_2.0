import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import Components from 'unplugin-vue-components/vite'
import { PrimeVueResolver } from 'unplugin-vue-components/resolvers'
import vue from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';



/* global process */
const baseFolder =
process.env.APPDATA !== undefined && process.env.APPDATA !== ''
? `${process.env.APPDATA}/ASP.NET/https`
: `${process.env.HOME}/.aspnet/https`;

const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
const certificateName = certificateArg ? certificateArg.groups.value : "registrovisitas.client";

if (!certificateName) {
console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.')
process.exit(-1);
}
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);



if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
if (0 !== child_process.spawnSync('dotnet', [
'dev-certs',
'https',
'--export-path',
certFilePath,
'--format',
'Pem',
'--no-password',
], { stdio: 'inherit', }).status) {
throw new Error("Could not create certificate.");
}
}

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(), // Cambié 'plugin()' por 'vue()' que es el plugin oficial de Vue
    Components({
      resolvers: [
        PrimeVueResolver()
      ],
      types: [{
        from: 'notivue',
        names: ['push']
      }]
    }) // Corregí el cierre de paréntesis aquí
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))

    }
  },
  base: process.env.NODE_ENV === 'production' ? '/Sistema Legal/' : '/',
  server: {
    proxy: {
      '^/api': {
        target: 'https://localhost:7177/',
        changeOrigin: true,
        secure: false
      }
    },
    port: 5173,
    https: {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath)
    }
  },
  
  build: {
    rollupOptions: {
      output: {
        globals: {
          jquery: 'window.jQuery',
          '$': 'window.jQuery'
        }
      }
    },
    commonjsOptions: {
      transformMixedEsModules: true
    }
  }
})
