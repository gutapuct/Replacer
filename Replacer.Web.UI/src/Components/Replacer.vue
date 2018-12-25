<template>
    <div class="container pt-3" id="replacer">
        <Header btnName="Настройки" btnUrl="admin"></Header>
        <ModalWindow
            :toggleModal="toggleModal"
            :modalShow="modalShow"
            :modalErrors="modalErrors"
            size="lg"
            :modalTitle="modalTitle"></ModalWindow>

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
                <b-row class="pt-5">
                    <b-button v-show="showBtnStart" :disabled="checkBtnStart" type="submit" variant="success" @click="start()">Запустить</b-button>
                    <b-button v-show="showOneMoreBtn" variant="primary" @click="clickOneMoreBtn()" class="ml-3">Еще раз</b-button>
                    <img v-if="loadingShow" class="loading" src="../Content/images/loading.gif" />
                </b-row>
            </transition>

            <transition name="fade" mode="out-in">
                <div class="mt-5" v-if="showProgress">
                    <span>Создание актов: <strong>{{currentCreating}} / {{maxCreating}}</strong></span>
                    <b-progress :max="100" :value="getPercentCurrentCreating" class="mb-3" height="20px" :animated="getAnimatedCreating">
                        
                    </b-progress>
                    <span>Объединение актов: <strong >{{currentCombine}} / {{maxCombine}}</strong></span>
                    <b-progress :max="100" :value="getPercentCurrentCombine" class="mb-3" height="20px" :animated="getAnimatedCombine"></b-progress>
                </div>
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
            btnDisabled: false,
            loadingShow: false,
            modalTitle: "Ошибка",
            showOneMoreBtn: false,
            currentCreating: 0,
            maxCreating: 0,
            currentCombine: 0,
            maxCombine: 0,
            showProgress: false,
            connectionId: ""
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

                proxy.on('sendProgress', function (max, current, type) {
                    if (type === 0) { // Creating acts
                        that.maxCreating = max;
                        that.currentCreating = current;
                    } else if (type === 1) { // Cobine acts
                        that.maxCombine = max;
                        that.currentCombine = current;
                    }
                });

                proxy.on('addError', function (message) {
                    console.log(message);
                });

                connection.start()
                    .done(function () {
                        console.log('Connection successfully (ID=' + connection.id + ')');
                        that.connectionId = connection.id;
                    })
                    .fail(function () { that.addErrorToModal('Сервер не отвечает. Проверьте службу.'); });
            }, 0);
        },
        clickOneMoreBtn(){
            this.btnDisabled = false;
            this.showOneMoreBtn = false;
            this.currentCreating = 0;
            this.maxCreating = 0;
            this.currentCombine = 0;
            this.maxCombine = 0;
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
            this.loadingShow = true;
            this.showProgress = true;
            let data = new FormData();
            data.append('fileTemplate', this.fileTemplate);
            data.append('fileValues', this.fileValues);

            this.$http
                .post(api.postStart + this.connectionId, data)
                .then(
                    function(response){
                        this.showOneMoreBtn = true;
                        this.showProgress = false;
                        this.modalTitle = "Импорт завершен";
                        this.loadingShow = false;
                        var results = response.data.Errors;
                        results.unshift("Создано актов: " + response.data.CountActs);
                        this.addErrorsToModal(results);
                    },
                    function(error){
                        this.modalTitle = "Ошибка";
                        this.showProgress = false;
                        this.loadingShow = false;
                        this.showOneMoreBtn = false;
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
            return this.showBtnStart && this.btnDisabled
        },
        getAnimatedCreating(){
            return this.currentCreating !== this.maxCreating;
        },
        getAnimatedCombine(){
            return this.currentCombine !== this.maxCombine;
        },
        getPercentCurrentCreating(){
            return ((this.currentCreating / this.maxCreating) * 100)|0;
        },
        getPercentCurrentCombine(){
            return ((this.currentCombine / this.maxCombine) * 100)|0;
        },
    }
}
</script>

<style>
.custom-file-input:lang(en)~.custom-file-label::after {
    content: "Загрузить";
}

.loading{
    width: 5%;
    height: 5%;
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
