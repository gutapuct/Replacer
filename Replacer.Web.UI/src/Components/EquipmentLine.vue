<template>
    <div id="EquipmentLine">
        <b-row>
            <b-col>
                <span @click="showEquipment(equipment)" class="pointer">
                    {{equipment.TypeName}} ({{equipment.Names.length}})
                </span>
            </b-col>
            <transition name="fade" mode="out-in">
                <b-col class="textAlignRight" v-show="equipment.IsShowNames">
                    <router-link to="/equipment">
                        <b-button variant="primary" size="sm" class="m-1">
                            Причины и рекоммендации
                        </b-button>
                    </router-link>
                </b-col>
            </transition>
        </b-row>
        <transition name="fade" mode="out-in">
            <div v-show="equipment.IsShowNames">
                <div v-for="(name, index) in equipment.Names" :key="index">
                    <input
                        type="text"
                        v-model="equipment.Names[index]"
                        class="m-1 form-control"
                        @change="(e) => SaveEquipmentName(equipment, e)" />
                </div>
                <b-button variant="success" @click="addName(equipment.Names)" class="mt-2 ml-3">Добавить</b-button>
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
        showEquipment(equipment){
            equipment.IsShowNames = !equipment.IsShowNames;
        },
        addName(names){
            if (names.filter(i => i.trim() === "").length > 0)
                this.addErrorToModal("Уже есть пустое поле для создания нового значения! Используйте его!");
            else
                names.push( "" );
        },
        SaveEquipmentName(equipment, e){
            if (e.target.value.length > 0)
            {
                e.target.disabled = "true";
            }
            this.$http
                .post(api.postChangeEquipmentNames, { TypeName: equipment.TypeName, Names: equipment.Names })
                .then(
                    function(response){
                        equipment.Names = [...equipment.Names.filter(el => el.length > 0)];
                        setTimeout(function () {
                            e.target.disabled = "";
                        }, 1000);
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
    }
}
</script>

<style>
    
</style>