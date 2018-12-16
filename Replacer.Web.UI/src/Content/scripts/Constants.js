const serverPath = "http://localhost:9009"
const server = serverPath + "/api/";

export default {
    getServerPath: serverPath,
    getHubName: "hubWebReporter",
    getAllEquipments: server + "equipments",
    postAddNewEquipment: server + "equipments",
    postChangeEquipmentNames: server + "equipments/names",
    getEquipmentById: server + "equipments/", 
    postChangeReasons: server + 'equipment/reasons/',
    postSaveTypeName: server + 'equipment/typename/',
    postDeleteEquipment: server + 'equipments/',
    postChangeOrder: server + "equipment/order/",
    postImportDb: server + "importdb",
    postStart: server + "start"
}