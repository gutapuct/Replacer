<template>
    <div>
        <b-modal v-model="modalShow" :title="getModalTitle" :size="getSize" hide-header-close no-close-on-esc no-close-on-backdrop centered hide-footer>
            <div class="d-block">
                <h3 v-if="shopH2">Список:</h3>
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
            <b-btn class="mt-3" variant="outline-danger" block @click="getBtnClickFunction">Закрыть</b-btn>
        </b-modal>
    </div>
</template>

<script>
export default {
    props:{
        modalShow: Boolean,
        modalErrors: Array,
        modalTitle: String,
        toggleModal: Function,
        size: String,
        btnClick: Function,
    },
    computed: {
        getModalTitle(){
            return (this.modalTitle) ? this.modalTitle : "Ошибка";
        },
        getSize(){
            return (this.size) ? this.size : "md"
        },
        shopH2(){
            return this.modalErrors.length > 1;
        },
        getBtnClickFunction(){
            return this.btnClick ? this.btnClick : this.toggleModal;
        }
    },
}
</script>

<style>
</style>
