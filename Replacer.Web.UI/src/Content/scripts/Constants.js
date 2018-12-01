const server = "http://localhost:9009/api/";

export default {
    getAllEquipments: server + "equipments",
    postAddNewEquipment: server + "equipments",
    postChangeEquipmentNames: server + "equipments/names",
    getEquipmentById: server + "equipments/", 
}