<template>
    <div class="container pt-3" id="main-block">
        <div id="admin">
            <Header btnName="Главная" btnUrl=""></Header>
            <ModalWindow :toggleModal="toggleModal" :modalShow="modalShow" :modalErrors="modalErrors"></ModalWindow>
            
            <router-link :to="'/import'" class="ml-3">
                <b-button variant="primary">Импорт</b-button>
            </router-link>

            <div class="row pt-3">
                <div class="col-md-10">
                    <b-form-input v-model="newValue"
                        type="text"
                        placeholder="Новое значение"
                    >
                    </b-form-input>
                </div>
                <div class="col-md-2 textAlignRight">
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
                            :changeNeedToMove="changeNeedToMove"
                            :getAllEquipments="getAllEquipments"
                            ></EquipmentLine>
                    </b-list-group-item>
                </b-list-group>
            </div>
        </div>
    </div>
</template>

<script>
import Header from './Header.vue'
import EquipmentLine from './EquipmentLine.vue'
import ModalWindow from './ModalWindow.vue'
import api from '../Content/scripts/Constants.js'

export default {
    components: { Header, EquipmentLine, ModalWindow },
    data() {
        return {
            equipments: [],
            newValue: '',
            modalShow: false,
            modalErrors: [],
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
        changeNeedToMove(id, change){
            var currentOrder = this.equipments.find(el => el.Id === id);
            
            if (currentOrder === undefined) return false;
            if (change === -1 && currentOrder.Order === 0) return false
            if (change === 1 && currentOrder.Order === this.equipments.length - 1) return false
            
            return true;
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
