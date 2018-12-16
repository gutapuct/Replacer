<template>
    <div class="container pt-3" id="replacer">
        <Header btnName="Настройки" btnUrl="admin"></Header>
        <ModalWindow
            :toggleModal="toggleModal"
            :modalShow="modalShow"
            :modalErrors="modalErrors"
            :btnClick="goToMainPage"
            size="lg"
            :modalTitle="modalTitle"></ModalWindow>

        <transition name="fade" mode="out-in">
            <div class="loading" v-if="loadingShow">
                <img src="../Content/images/loading.gif" />
            </div>
        </transition>

        <div>
            <div id="hello">
                <h2>
                    <span>Добро пожаловать в <u><i>REPLACER</i></u></span>
                </h2>
            </div>
            <b-row class="pl-4 pt-2 pb-2"><strong>Путь к шаблону:</strong></b-row>
            <b-row>
                <b-file
                    accept=".doc, .docx"
                    v-model="fileTemplate"
                    :state="checkFormTemplate"
                    placeholder="Выберите файл..."></b-file>
            </b-row>
            <transition name="fade" mode="out-in">
                <div v-show="checkFormTemplate || checkFormValues">
                    <b-row class="pl-4 pt-5 pb-2">
                        <strong>Путь к значениям:</strong>
                    </b-row>
                    <b-row>
                        <b-file
                            accept=".xls, .xlsx"                        
                            v-model="fileValues"
                            :state="checkFormValues"
                            placeholder="Выберите файл..."></b-file>
                    </b-row>
                </div>
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
            modalTitle: "Ошибка"
        }
    },
    created() {
        this.createSignalRConnection();
    },
    methods: {
        createSignalRConnection(){
            const that = this;

            setTimeout(() => {
                const connection = window.$.hubConnection(api.getServerPath);
                const proxy = connection.createHubProxy(api.getHubName);

                proxy.on('sendProgress', function (max, current) {
                    console.log('max: ' + max + '; current: ' + current);
                });

                proxy.on('addError', function (message) {
                    console.log(message);
                });

                connection.start()
                    .done(function () { console.log('Connection successfully (ID=' + connection.id + ')'); })
                    .fail(function () { that.addErrorToModal('Сервер не отвечает. Проверьте службу.'); });
            }, 0);
        },
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
            this.btnDisabled = true;
            this.loadingShow = true
            let data = new FormData();
            data.append('fileTemplate', this.fileTemplate);
            data.append('fileValues', this.fileValues);

            this.$http
                .post(api.postStart, data)
                .then(
                    function(response){
                        this.btnDisabled = false;
                        this.modalTitle = "Импорт завершен";
                        this.loadingShow = false;
                        var results = response.data.Errors;
                        results.unshift("Создано актов: " + response.data.CountActs);
                        this.addErrorsToModal(results);
                    },
                    function(error){
                        this.modalTitle = "Ошибка";
                        this.loadingShow = false;
                        this.btnDisabled = false;
                        if (error && error.data && error.data.Errors)
                        {
                            this.addErrorsToModal(error.data.Errors);
                        } else {
                            this.addErrorToModal(error);
                        }
                    }
                )
        },
        goToMainPage(){
            document.location.reload();
        }
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

#hello{
    text-align: center;
    margin-bottom: 24px;
    margin-top: 32px;
}

#hello span{
    border: 5px solid rgb(118, 96, 245);
    border-radius: 15px;
    padding: 10px;
}
</style>
