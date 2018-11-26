
<template>
    <div class="container pt-3" id="main-block">
        <App></App>
        <div class="header">
            <div class="row pb-5">Добро пожаловать в Replacer</div>
            <div class="row pt-5">Путь к шаблону:</div>
            <div class="row">
                <b-form-file v-model="pathToTemplate" :state="Boolean(pathToTemplate)" placeholder="Выберите файл..."></b-form-file>
            </div>
            <div class="row pt-5">Путь к значениям:</div>
            <div class="row">
                <b-form-file v-model="pathToValues" :state="Boolean(pathToValues)" placeholder="Выберите файл..."></b-form-file>
            </div>
            <div class="row pt-5">
                <b-button type="submit" variant="success">Запустить</b-button>
            </div>
        </div>
    </div>
</template>

<script>
import App from './App.vue'
export default {
    name: 'main-block',
    components: { App  },
    data () {
        return {
            pathToTemplate: {},
            pathToValues: {},
        }
    },
}
</script>

<style>
    .custom-file-input:lang(en)~.custom-file-label::after {
        content: "Загрузить";
    }
</style>
