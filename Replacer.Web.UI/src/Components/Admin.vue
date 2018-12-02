<template>
    <div class="container pt-3" id="main-block">
        <div id="admin">
            <Header btnName="Главная" btnUrl=""></Header>
            
            <b-button variant="primary">Импорт</b-button>

            <div class="row pt-3">
                <div class="col-md-11">
                    <b-form-input v-model="newValue"
                        type="text"
                        placeholder="Новое значение"
                    >
                    </b-form-input>
                </div>
                <div class="col-md-1">
                    <b-button class="btn btn-success" :disabled="getDisabledBtn" @click="addNewValue()">Добавить</b-button>
                </div>
            </div>
            <div class="p-3">
                <b-list-group>
                    <b-list-group-item
                        variant="info"
                        v-for="(equipment, index) in equipments"
                        :key="index"
                        class="mt-1 mb-1"
                    >
                        <EquipmentLine
                            :equipment="equipment"
                            :addErrorToModal="addErrorToModal"
                            :addErrorsToModal="addErrorsToModal"
                            ></EquipmentLine>
                    </b-list-group-item>
                </b-list-group>
            </div>

            <!-- Modal window -->
            <div>
                <b-modal v-model="modalShow" :title="modalTitle" hide-header-close no-close-on-esc no-close-on-backdrop centered hide-footer>
                    <div class="d-block">
                        <div
                            v-for="(error, index) in modalErrors"
                            :key="index"
                            class="pt-3"
                        >
                            <span v-if="modalErrors.length > 1">
                                {{index + 1}}. 
                            </span>
                            <span>
                                {{error}}
                            </span> 
                        </div>
                    </div>
                    <b-btn class="mt-3" variant="outline-danger" block @click="toggleModal">Закрыть</b-btn>
                </b-modal>
            </div>

        </div>
    </div>
</template>

<script>
import Header from './Header.vue'
import EquipmentLine from './EquipmentLine.vue'
import api from '../Content/scripts/Constants.js'

export default {
    components: { Header, EquipmentLine },
    data() {
        return {
            equipments: [],
            newValue: '',
            modalShow: false,
            modalErrors: [],
            modalTitle: "Ошибка",
        }
    },
    created() {
        this.getAllEquipments();
    },
    methods: {
        getAllEquipments() {
            this.$http
                .get(api.getAllEquipments)
                .then(
                    function(response){
                        this.equipments = response.data.Object;
                    },
                    function(error){
                        if (error.status === 0)
                            this.addErrorToModal("Сервер не отвечает. Проверьте службу.");
                        else
                            this.addErrorToModal(error.statusText);
                    }
                )
        },
        addNewValue(){
            this.$http
                .post(api.postAddNewEquipment, JSON.stringify(this.newValue))
                .then(
                    function(response){
                        this.getAllEquipments();
                        this.newValue = '';
                    },
                    function(error){
                        if (error !== undefined && error.data !== undefined && error.data.Errors !== undefined)
                        {
                            this.addErrorsToModal(error.data.Errors);
                        } else {
                            this.addErrorToModal(error);
                        }
                    }
                )
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
    },
    computed: {
        getDisabledBtn(){
            return this.newValue.length === 0;
        },
    }
}
</script>

<style>

</style>
