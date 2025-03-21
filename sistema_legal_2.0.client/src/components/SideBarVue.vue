<template>
<div class="sidebar">
<h2>MENU</h2>

<a><router-link to="/login" class="no-underline" ><b>inicio</b></router-link></a>
<br><br/>
<!-- <a><router-link to="/lista-de-tarea" class="no-underline"><b>LISTA DE TAREA</b></router-link></a>
<br><br/>
<a><router-link to="/formulario" class="no-underline"><b>FORMULARIO</b></router-link></a> -->
</div>

</template>

 <script setup>

</script>


<style>
.sidebar{
  width: 200px;
  height: 1000px;
  background-color: gainsboro;
  padding: 20px;
}
H2{
  font-family: 'Franklin Gothic Medium';
  font-size: 20PX;
}



.no-underline {
  text-decoration: none;
  font-size: 12px;
  font-family: 'Franklin Gothic Medium', 'Arial Narrow', Arial, sans-serif;


}

.no-underline:hover{
  background-color: blanchedalmond;

}

</style>
