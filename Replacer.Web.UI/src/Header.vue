<template>
    <div id="headerBlock">
        <b-alert show variant="secondary">
            <div class="row">
                <div class="col-md-6">
                    <h2>Replacer</h2>
                </div>
                <div class="col-md-6 settings">
                    <router-link :to="this.getUrlByBtn">
                        <b-button variant="link" size="lg">
                            {{getBtnName}}
                        </b-button>
                    </router-link>
                </div>
            </div>
        </b-alert>    
    </div>
</template>

<script>
export default {
    name: 'headerBlock',
    props: {
        isAdmin: Boolean
    },
    methods:{
    },
    computed: {
        getBtnName(){
            return this.isAdmin ? 'Настройки' : 'Главная';
        },
        getUrlByBtn() {
            return this.isAdmin ? "/admin" : "/";
        }
    }
}
</script>

<style>
    .settings{
            text-align: right; 
        }
    .header h2{
        font-family: sans-serif;
        color: rgb(241, 144, 18);
        font-weight: bold;
    }
</style>
