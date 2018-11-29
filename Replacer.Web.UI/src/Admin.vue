<template>
    <div class="container pt-3" id="main-block">
        <div id="admin">
            <Header :isAdmin="false"></Header>
            
            <div>Оборудование:</div>
            <div class="row pt-3">
                <div class="col-md-11">
                    <b-form-input v-model="newValue"
                        type="text"
                        placeholder="Новое значение"
                    >
                    </b-form-input>
                </div>
                <div class="col-md-1">
                    <b-button class="btn-success" :disabled="getDisabledBtn" @click="AddNewValue()">Добавить</b-button>
                </div>
            </div>
            <div class="p-3">
                <b-list-group>
                    <b-list-group-item
                        variant="info"
                        v-for="(reason, index) in equipments"
                        :key="index"
                        class="mt-1 mb-1"
                    >
                        <div>
                            <span @click="showEquipment(reason)" class="pointer">
                                {{reason.TypeName}} ({{reason.Names.length}})
                            </span>
                        </div>
                        <transition name="fade" mode="out-in">
                            <div v-show="reason.IsShowNames">
                                <div v-for="(name, index) in reason.Names" :key="index">
                                    <input
                                        type="text"
                                        v-model="reason.Names[index]"
                                        class="m-1 form-control"
                                        @blur="SaveEquipmentName(reason)" />
                                </div>
                                <b-button variant="success" @click="addName(reason.Names)">+</b-button>
                            </div>
                        </transition>
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
import api from './Constants.js'

export default {
    components: { Header },
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
        AddNewValue(){
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
        showEquipment(equipment){
            equipment.IsShowNames = !equipment.IsShowNames;
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
        SaveEquipmentName(equipment){
            this.$http
                .post(api.postChangeEquipmentNames, { TypeName: equipment.TypeName, Names: equipment.Names })
                .then(
                    function(response){},
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
        addName(names){
            if (names.filter(i => i.trim() === "").length > 0)
                this.addErrorToModal("Уже есть пустое поле для создания нового значения! Используйте его!");
            else
                names.push( "" );
        }
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
