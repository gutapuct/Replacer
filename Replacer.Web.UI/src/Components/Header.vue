<template>
    <div id="headerBlock">
        <b-alert show variant="secondary pt-3 pb-3">
            <div class="row">
                <div class="col-md-6">
                    <h2>Replacer</h2>
                </div>
                <div class="col-md-6 settings">
                    <router-link :to="'/' + this.btnUrl">
                        <b-button variant="link" size="lg">
                            {{btnName}}
                        </b-button>
                    </router-link>
                </div>
            </div>
        </b-alert>    
    </div>
</template>

<script>
export default {
    props: {
        btnName: String,
        btnUrl: String
    },
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
