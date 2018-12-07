<template>
    <div id="ImportDb" class="container pt-3">
        <Header btnName="Назад" btnUrl="admin"></Header>
        <ModalWindow
            :toggleModal="toggleModal"
            :modalShow="modalShow"
            :modalErrors="modalErrors"
            :modalTitle="modalTitle"
            size="lg"
            :btnClick="goToAdminPage"></ModalWindow>
        <div class="loading" v-if="loadingShow">
            <img src="../Content/images/loading.gif" />
        </div>
        
        <b-row class="pl-4 pt-2 pb-2"><strong>Путь к импортируемому файлу:</strong></b-row>
        <b-row>
            <b-col>
                <b-file
                    accept=".xls, .xlsx"
                    v-model="file"
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
import ModalWindow from './ModalWindow.vue'
import api from '../Content/scripts/Constants.js'

export default {
    components: { Header, ModalWindow },
    data() {
        return{
            file: { },
            modalShow: false,
            modalErrors: [],
            modalQuestionShow: false,
            modalTitle: "",
            btnDisabled: false,
            loadingShow: false
        }
    },
    methods: {
        startImport(){
            if (this.checkFileType){
                this.loadingShow = true
                this.btnDisabled = true;
                let data = new FormData();
                data.append('file', this.file);

                this.$http
                    .post(api.postImportDb, data)
                    .then(
                        function(response){
                            this.loadingShow = false;
                            if (response && response.data && response.data.Object){
                                this.modalTitle = "Импорт завершен";
                                this.addErrorsToModal(response.data.Object);
                            }
                        },
                        function(error){
                            this.loadingShow = false;
                            this.btnDisabled = false;
                            this.modalTitle = "Ошибка";
                            if (error && error.data && error.data.Errors)
                            {
                                this.addErrorsToModal(error.data.Errors);
                            } else {
                                this.addErrorToModal(error);
                            }
                        }
                    )
            }
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
        goToAdminPage(){
            this.$router.push('/admin');
        }
    },
    computed: {
        checkFileType(){
            return this.file.name !== undefined
                   && !this.btnDisabled
                   && this.file.name.length > 0
                   && (this.file.name.endsWith('.xls') || this.file.name.endsWith('xlsx'));
        }
    },
}
</script>

<style>
.custom-file-input:lang(en)~.custom-file-label::after {
    content: "Загрузить";
}

#importDb .loading{
    height: 100px;
    position: relative;
}

#ImportDb .loading img{
    width: 25%;
    position: absolute;
    margin: auto;
    left: 0;
    top: 0;
    bottom: 0;
    right: 0;
}
</style>
