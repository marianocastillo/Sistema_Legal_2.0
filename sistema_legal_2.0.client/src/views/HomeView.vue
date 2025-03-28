<template>
  <div>
      <AsesorDashboard v-if="$store.state.user.idPerfil == PerfilesEnum.Asesor"/>
      <MainDashboard  v-else />
  </div>
</template>

<script>
  import PerfilesEnum from '@/enums/PerfilesEnum.js'

  export default {
      data() {
          return {

          }
      },
      computed: {
          PerfilesEnum() {
              return PerfilesEnum
          }
      },
      methods: {

      }
  }
</script>
