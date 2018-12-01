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
                    class="form-control" />
            </b-col>
            <b-col cols="5">
                <input
                    type="text"
                    v-model="reason.NameRecommendation"
                    class="form-control" />
            </b-col>
            <b-col cols="0" class="ml-3 pt-1">
                <img src="../Content/images/Delete.png" width="16" class="pointer" />
            </b-col>
        </b-row>
    </div>
</template>

<script>
export default {
    props: {
        reason: Array,
        index: Number,
    }
}
</script>

<style>

</style>