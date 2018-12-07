<template>
    <div class="container pt-3" id="replacer">
        <Header btnName="Настройки" btnUrl="admin"></Header>
        <ModalWindow :toggleModal="toggleModal" :modalShow="modalShow" :modalErrors="modalErrors"></ModalWindow>
        <transition name="fade" mode="out-in">
            <div class="loading" v-if="loadingShow">
                <img src="../Content/images/loading.gif" />
            </div>
        </transition>

        <div>
            <b-row class="pb-5">Добро пожаловать в Replacer</b-row>
            <b-row class="pt-5">Путь к шаблону:</b-row>
            <b-row>
                <b-file v-model="fileTemplate" :state="checkFormTemplate" placeholder="Выберите файл..."></b-file>
            </b-row>
            <transition name="fade" mode="out-in">
                <b-row class="pt-5" v-show="checkFormTemplate || checkFormValues">Путь к значениям:
                    <b-file v-model="fileValues" :state="checkFormValues" placeholder="Выберите файл..."></b-file>
                </b-row>
            </transition>
            <transition name="fade" mode="out-in">
                <b-row v-show="showBtnStart" class="pt-5">
                    <b-button :disabled="checkBtnStart" type="submit" variant="success" @click="start()">Запустить</b-button>
                </b-row>
            </transition>
        </div>
    </div>
</template>

<script>
import Header from './Header.vue'
import ModalWindow from './ModalWindow.vue'
import api from '../Content/scripts/Constants.js'

export default {
    name: 'replacer',
    components: { Header, ModalWindow },
    data () {
        return {
            fileTemplate: {},
            fileValues: {},
            modalShow: false,
            modalErrors: [],
            btnDisabled: true,
            loadingShow: false,
        }
    },
    methods: {
        toggleModal(){
            if (this.modalShow) //if open
                this.modalErrors = [];
            this.modalShow = !this.modalShow;
        },
        addErrorToModal(error){
            this.modalErrors.push(error);
            this.toggleModal();
        },
        addErrorsToModal(errors){
            errors.forEach(element => {
                this.modalErrors.push(element);
            });
            this.toggleModal();
        },
        start(){
            this.loadingShow = true
            let data = new FormData();
            data.append('fileTemplate', this.fileTemplate);
            data.append('fileValues', this.fileValues);

            this.$http
                .post(api.postStart, data)
                .then(
                    function(response){
                        console.log(response);
                        this.loadingShow = false;
                        this.addErrorToModal("Импорт завершен!");
                    },
                    function(error){
                        this.loadingShow = false;
                        if (error && error.data && error.data.Errors)
                        {
                            this.addErrorsToModal(error.data.Errors);
                        } else {
                            this.addErrorToModal(error);
                        }
                    }
                )
        },
    },
    computed: {
        checkFormTemplate(){
            return this.fileTemplate.name !== undefined
                   && this.fileTemplate.name
                   && (this.fileTemplate.name.endsWith('.doc') || this.fileTemplate.name.endsWith('docx'));
        },
        checkFormValues(){
            return this.fileValues.name !== undefined
                   && this.fileValues.name
                   && (this.fileValues.name.endsWith('.xls') || this.fileValues.name.endsWith('xlsx'));
        },
        showBtnStart(){
            return this.checkFormTemplate && this.checkFormValues;
        },
        checkBtnStart(){
            return this.showBtnStart && !this.btnDisabled
        },
    }
}
</script>

<style>
.custom-file-input:lang(en)~.custom-file-label::after {
    content: "Загрузить";
}

#replacer .loading img{
    width: 25%;
    position: absolute;
    margin: auto;
    left: 0;
    top: 0;
    bottom: 0;
    right: 0;
    z-index: 100;
}
</style>
