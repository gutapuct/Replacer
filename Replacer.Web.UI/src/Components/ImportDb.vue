<template>
    <div id="ImportDb" class="container pt-3">
        <Header btnName="Назад" btnUrl="admin"></Header>

        <b-row class="pl-4 pt-2 pb-2"><strong>Путь к импортируемому файлу:</strong></b-row>
        <b-row>
            <b-col>
                <b-file
                    class="pointer"
                    accept=".xls, .xlsx"
                    v-model="pathToReason"
                    :state="checkFileType"
                    placeholder="Выберите файл..."></b-file>
            </b-col>
        </b-row>
        <b-row>
            <b-button :disabled="!checkFileType" variant="primary" size="lg" class="ml-4 mt-4" @click="startImport()">
                Импортировать
            </b-button>
        </b-row>
    </div>
</template>

<script>
import Header from './Header.vue'

export default {
    components: { Header },
    data() {
        return{
            pathToReason: { name: "" },
        }
    },
    methods: {
        startImport(){
            
        }
    },
    computed: {
        checkFileType(){
            console.log(this.pathToReason);
            return this.pathToReason.name !== undefined
                   && this.pathToReason.name.length > 0
                   && (this.pathToReason.name.endsWith('.xls') || this.pathToReason.name.endsWith('xlsx'));
        }
    },
}
</script>

<style>
.custom-file-input:lang(en)~.custom-file-label::after {
    content: "Загрузить";
}
</style>
