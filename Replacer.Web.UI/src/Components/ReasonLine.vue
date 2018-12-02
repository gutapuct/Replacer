<template>
    <div id="Reason">
        <b-row>
            <b-col cols="0" class="ml-2 pt-1">
                <span>{{index + 1}}.</span>
            </b-col>
            <b-col cols="6">
                <input
                    type="text"
                    v-model="reason.NameReason"
                    class="form-control"
                    @change="(e) => saveReasons(e)" />
            </b-col>
            <b-col cols="5">
                <input
                    type="text"
                    v-model="reason.NameRecommendation"
                    class="form-control"
                    @change="(e) => saveReasons(e)" />
            </b-col>
            <b-col cols="0" class="ml-3 pt-1">
                <img
                    src="../Content/images/Delete2.png"
                    class="imgIcon24 pointer"
                    @click="deleteReason(index)" />
            </b-col>
        </b-row>
    </div>
</template>

<script>
import api from '../Content/scripts/Constants.js'

export default {
    props: {
        reason: Object,
        index: Number,
        saveReasons: Function,
        deleteReason: Function
    },
    methods: {
        
    }
}
</script>

<style>

</style>