<template>
    <div class="container">
        <div id="app">
            <b-alert show variant="secondary">
                <div class="row">
                    <div class="col-md-6">
                        <h2>Replacer</h2>
                    </div>
                    <div class="col-md-6 settings">
                        <router-link to="/admin">
                            <md-button
                                class="md-raised"
                                @click="GoToAdminPanel"
                            >
                                {{btnName}}
                            </md-button>
                        </router-link>
                    </div>
                </div>
            </b-alert>    
        </div>
    </div>
</template>

<script>
export default {
    name: 'app',
    data () {
        return {
            btnName: 'Настройки',
            mainVisibility: true,
        }
    },
    methods:{
        GoToAdminPanel(){
            this.btnName = "Главная";
            this.mainVisibility = false;
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
