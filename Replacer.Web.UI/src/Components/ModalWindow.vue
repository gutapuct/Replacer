<template>
    <div>
        <b-modal v-model="modalShow" :title="getModalTitle" hide-header-close no-close-on-esc no-close-on-backdrop centered hide-footer>
            <div class="d-block">
                <div
                    v-for="(error, index) in modalErrors"
                    :key="index"
                    class="pt-3"
                >
                    <span v-if="modalErrors.length > 1">
                        {{index + 1}}. 
                    </span>
                    <span>
                        {{error}}
                    </span> 
                </div>
            </div>
            <b-btn class="mt-3" variant="outline-danger" block @click="toggleModal">Закрыть</b-btn>
        </b-modal>
    </div>
</template>

<script>
export default {
    props:{
        modalShow: Boolean, //default = false
        modalErrors: Array,
        modalTitle: String,
        toggleModal: Function
    },
    computed: {
        getModalTitle(){
            return (this.modalTitle === undefined || this.modalTitle.length === 0) ? "Ошибка" : this.modalTitle;
        },
    },
}
</script>

<style>
</style>
