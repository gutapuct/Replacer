const server = "http://localhost:9009/api/";

export default {
    getAllEquipments: server + "equipments",
    postAddNewEquipment: server + "equipments",
    postChangeEquipmentNames: server + "equipments/names",
    getEquipmentById: server + "equipments/", 
    postChangeReasons: server + 'equipment/reasons/',
    postSaveTypeName: server + 'equipment/typename/',
    postDeleteEquipment: server + 'equipments/'
}