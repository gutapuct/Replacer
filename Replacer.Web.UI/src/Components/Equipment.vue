<template>
    <div class="container pt-3" id="equipment">
        <Header btnName="Назад" btnUrl="admin"></Header>
        
        <b-alert show class="typeName pr-5">
            <span v-if="!inputForTypeName">{{equipment.TypeName}}
                <img
                    src="../Content/images/Edit.png"
                    class="imgIcon24 pointer ml-3 mr-2"
                    @click="openChangeTypeName()" /> 
                <img src="../Content/images/Delete.png" class="imgIcon24 pointer" />
            </span>
            <span v-else>
                <div>
                    <b-form-input
                        v-show="inputForTypeName"
                        :state="checkTypeName"
                        type="text"
                        v-model="equipment.TypeName"
                        placeholder="Введите название...">
                    </b-form-input>
                </div>
                <div class="textAlignLeft">
                    <b-button
                        variant="success ml-3 mt-3"
                        size="sm"
                        @click="openChangeTypeName()"
                        :disabled="!checkTypeName"
                    >
                        Сохранить
                    </b-button>
                </div>
            </span>
        </b-alert>
        <div class="p-3">
            <b-list-group>
                <b-list-group-item
                    variant="info"
                    v-for="(reason, index) in equipment.Reasons"
                    :key="index"
                >
                    <ReasonLine
                        :reason="reason"
                        :index="index"
                        :saveReasons="saveReasons"
                        :deleteReason="deleteReason">
                    </ReasonLine>
                </b-list-group-item>
            </b-list-group>
        </div>
        <div>
            <img
                src="../Content/images/Add.png"
                class="imgIcon48 pointer mt-2 ml-3"
                @click="addReason()" /> 
        </div>
    </div>
</template>

<script>
import Header from './Header.vue'
import api from '../Content/scripts/Constants.js'
import ReasonLine from './ReasonLine.vue'

export default {
    components: { Header, ReasonLine },
    data() {
        return {
            equipment: {},
            inputForTypeName: false,
        }
    },
    created() {
        this.getEquipmentById(this.$route.params.id);
    },
    methods: {
        getEquipmentById(id){
            console.log(api.getEquipmentById + id);
            this.$http
                .get(api.getEquipmentById + id)
                .then(
                    function(response){
                        this.equipment = response.data.Object;
                    },
                    function(error){
                        if (error.status === 0)
                            this.addErrorToModal("Сервер не отвечает. Проверьте службу.");
                        else
                            this.addErrorToModal(error.statusText);
                    }
                )
        },
        addReason(){
            if (this.equipment.Reasons.some(el => el.NameReason.trim().length === 0))
                alert("Добавь уже имеющееся");
            else
                this.equipment.Reasons.push({ NameReason: "", NameRecommendation: "" });
        },
        saveReasons(e){
            if (e !== undefined){
                e.target.disabled = "true";
            }
            this.$http
                .post(api.postChangeReasons + this.equipment.Id, this.equipment.Reasons)
                .then(
                    function(response){
                        this.equipment.Reasons = this.equipment.Reasons.filter(el => 
                            el.NameReason.trim().length > 0 || el.NameRecommendation.trim().length > 0);
                        
                        if (e !== undefined){
                            setTimeout(function () {
                                e.target.disabled = "";
                            }, 1000);
                        }
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
        deleteReason(index){
            this.equipment.Reasons.splice(index, 1);
            this.saveReasons();
        },
        openChangeTypeName(){
            if (!this.inputForTypeName)
                this.inputForTypeName = !this.inputForTypeName;
            else
                this.saveTypeName();
        },
        saveTypeName(){
            this.$http
                .post(api.postSaveTypeName + this.equipment.Id + '/' + this.equipment.TypeName)
                .then(
                    function(response){
                        this.inputForTypeName = false;
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
    },
    computed: {
        checkTypeName(){
            return this.equipment.TypeName.trim().length > 0;
        }
    }
}
</script>

<style>
    .typeName{
        font-size: 24px;
        font-weight: bold;
        text-align: right;
    }
</style>
