<template>
    <div class="container pt-3" id="main-block">
        <div id="admin">
            <Header :isAdmin="false"></Header>
            
            <div>Оборудование:</div>
            <div class="p-3">
                <ul class="list-group">
                    <li
                        class="list-group-item"
                        v-for="(reason, index) in reasons"
                        :key="index"
                    >
                        {{reason}}
                    </li>
                </ul>
            </div>
            <div class="row">
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
            if (this.newValue.lengtn < 1){
                alert("Введите  новое значение");
            }
            else{
                this.$http
                    .post(api.postAddNewValue, this.newValue)
                    .then(
                        function(response){
                            this.reasons.push(this.newValue);
                            this.newValue = '';
                        },
                        function(error){
                            alert("Ошибка!!!");
                        }
                    )
            }
        }
    },
    computed: {
        getDisabledBtn(){
            return this.newValue.length === 0;
        }
    }
}
</script>

<style>

</style>
