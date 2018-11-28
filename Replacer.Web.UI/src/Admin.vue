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
                        v-for="(reason, index) in reasons"
                        :key="index"
                        class="mt-1 mb-1"
                    >
                        <div>
                            <span @click="showReason(reason)" class="pointer">
                                {{reason.TypeName}} ({{reason.Names.length}})
                            </span>
                        </div>
                        <transition name="fade" mode="out-in">
                            <div v-show="reason.IsShowNames">
                                <b-list-group>
                                    <b-list-group-item
                                        variant="light"
                                        class="m-1"
                                        v-for="(name, index) in reason.Names"
                                        :key="index"
                                    >
                                        {{name.Name}}
                                    </b-list-group-item>
                                </b-list-group>
                            </div>
                        </transition>
                    </b-list-group-item>
                </b-list-group>
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
            reasons: [],
            newValue: '',
        }
    },
    created() {
        this.getAllReasons();
    },
    methods: {
        getAllReasons() {
            this.$http
                .get(api.getAllReasons)
                .then(
                    function(response){
                        this.reasons = response.data;
                    },
                    function(error){
                        alert("Ошибка!!!");
                    }
                )
        },
        AddNewValue(){
            this.$http
                .post(api.postAddNewValue, JSON.stringify(this.newValue))
                .then(
                    function(response){
                        this.getAllReasons();
                        this.newValue = '';
                    },
                    function(error){
                        if (error !== undefined && error.data !== undefined)
                        {
                            error.data.Errors.forEach(element => {
                                alert(element);
                            });
                        }
                    }
                )
        },
        showReason(reason){
            reason.IsShowNames = !reason.IsShowNames;
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
