<template>
    <div id="EquipmentLine">
        <b-row>
            <b-col>
                <span @click="showEquipment()" class="pointer">
                    {{equipment.TypeName}} ({{equipment.Names.length}})
                </span>
            </b-col>
            <transition name="fade" mode="out-in">
                <b-col class="textAlignRight" v-show="equipment.IsShowNames">
                    <router-link :to="{ name: 'equipment', params: {id: equipment.Id } }">
                        <b-button variant="primary" size="sm" class="m-1">
                            Причины и рекоммендации
                        </b-button>
                    </router-link>
                </b-col>
            </transition>
            <b-col cols="1" class="textAlignRight m-1">
                <img src="../Content/images/Up.png" class="imgIcon24 mr-1" />
                <img src="../Content/images/Down.png" class="imgIcon24" />
            </b-col>
        </b-row>
        <transition name="fade" mode="out-in">
            <div v-show="equipment.IsShowNames">
                <b-row v-for="(name, index) in equipment.Names" :key="index">
                    <b-col cols="11">
                        <input
                            type="text"
                            v-model="equipment.Names[index]"
                            class="m-1 form-control"
                            @change="(e) => saveEquipmentName(e)" />
                    </b-col>
                    <b-col cols="1">
                        <img src=../Content/images/Delete2.png class="imgIcon24 pointer mt-2" @click="deleteName(index)" />
                    </b-col>
                </b-row>
                <img src="../Content/images/Add.png" @click="addName()" class="imgIcon48 pointer mt-2 ml-3" /> 
            </div>
        </transition>
    </div>
</template>

<script>
import api from '../Content/scripts/Constants.js'

export default {
    name: "EquipmentLine",
    props: {
        equipment: Object,
        addErrorToModal: Function,
        addErrorsToModal: Function,
    },
    methods: {
        showEquipment(){
            this.equipment.IsShowNames = !this.equipment.IsShowNames;
        },
        addName(){
            if (this.equipment.Names.filter(i => i.trim() === "").length > 0)
                this.addErrorToModal("Уже есть пустое поле для создания нового значения! Используйте его!");
            else
                this.equipment.Names.push( "" );
        },
        saveEquipmentName(e){
            if (e !== undefined && e.target.value.length > 0)
            {
                e.target.disabled = "true";
            }
            this.$http
                .post(api.postChangeEquipmentNames, { TypeName: this.equipment.TypeName, Names: this.equipment.Names })
                .then(
                    function(response){
                        this.equipment.Names = [...this.equipment.Names.filter(el => el.length > 0)];
                        
                        if (e !== undefined)
                        {
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
        deleteName(index){
            this.equipment.Names.splice(index, 1);
            this.saveEquipmentName();
        },
    },
}
</script>

<style>
    
</style>