<template>
    <div class="container pt-3" id="equipment">
        <Header btnName="Назад" btnUrl="admin"></Header>
        
        <b-alert show class="textAlignRight pr-5">
            <span class="typeName">{{equipment.TypeName}}</span>
            <img src="../Content/images/Edit.png" width="24" class="pointer ml-3 mr-2" /> 
            <img src="../Content/images/Delete.png" width="24" class="pointer" /> 
        </b-alert>
        <div class="p-3">
            <b-list-group>
                <b-list-group-item
                    variant="info"
                    v-for="(reason, index) in equipment.Reasons"
                    :key="index"
                >
                    <ReasonLine :reason="reason" :index="index"></ReasonLine>
                </b-list-group-item>
            </b-list-group>
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
        }
    }
}
</script>

<style>
    .typeName{
        font-size: 24px;
        font-weight: bold;
    }
</style>

