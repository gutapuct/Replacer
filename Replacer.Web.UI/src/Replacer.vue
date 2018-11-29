
<template>
    <div class="container pt-3" id="main-block">
        <Header isAdmin></Header>

        <div>
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
import Header from './Header.vue'
export default {
    name: 'main-block',
    components: { Header },
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
