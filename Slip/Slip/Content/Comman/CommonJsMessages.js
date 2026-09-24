
function SetNotify(getval) {
    //if (getval == "Success") {
    //    toaster_message('success', 'fail', 'check notes and re-submit.')
    //}
    //else if (getval = "AddressAdd") {
    //    toaster_message('success', 'fail', 'address successulfakjsdfhjksa.')
    //}
    //if (getval == "error") {
    //    toaster_message('error', 'fail', 'check notes and re-submit.')
    //}
    //if (getval == "Warning") {
    //    toaster_message('error', 'Warning', 'Please fill proper role details.');
    //}
    var msg = title = type = '';
    switch (getval) {
        case "RoughInwardUpdate":
            msg = "Attribute group has been save successfully.";
            title = "Save";
            type = 'success';
            break;
        case "RoughInwardInvoiceNo":
            msg = "Please Enter The Invoice no.";
            title = "Warning";
            type = 'info';
            break;
        case "SelectCmp":
            msg = "Please Select Company";
            title = "Warning";
            type = 'info';
            break;
        case "Error":
            msg = "Something went Wrong please Try Again";
            title = "Warning";
            type = 'info';
            break;
        case "AddDetails":
            msg = "Please Select or Fill Proper Details";
            title = "Warning";
            type = 'warning';//error
            break;
        case "SelectRow":
            msg = "Please Select the RoughSales details Record";
            title = "Warning";
            type = 'warning';//error
            break;
        case "TransferType":
            msg = "Please, Select Company & Transfer Type";
            title = "Warning";
            type = 'info';//error
            break;
        case "SelectCompany":
            msg = "Please, Select Company Name";
            title = "Warning";
            type = 'info';//error
            break;
        case "RoughMixing":
            msg = "Mixing has been save successfully.";
            title = "Save";
            type = 'success';
            break;
        case "LotCreation":
            msg = "Lot Created successfully.";
            title = "Save";
            type = 'success';
            break;
        case "PacketCreation":
            msg = "Packets Created successfully.";
            title = "Save";
            type = 'success';
            break;
        case "UnableToAddPacket":
            msg = "Not Enough Pcs Or Cts To Create Packets...";
            title = "Warning";
            type = 'info';//error
            break;
        case "CreateMixing":
            msg = "Please, Fill Mixing Grid First..";
            title = "Warning";
            type = 'warning';//error
            break;
        case "RecordExists":
            msg = "Record already exists.";
            title = "Warning";
            type = 'info';//error
            break;
        case "savesuccess":
            msg = "Record has been successfully save.";
            title = "Save";
            type = 'success';//error
            break;
        case "updatesuccess":
            msg = "Record has been updated successfully.";
            title = "Save";
            type = 'success';//error
            break;
        case "StockDistribution":
            msg = "Stock Distributed Successfully..";
            title = "Save";
            type = 'success';//error
            break;
        case "CertificateResultFinal":
            msg = "Certificate Result Finalized Successfully..";
            title = "Save";
            type = 'success';//error
            break;
        case "MFGMemoInward":
            msg = "MFG Stock Upload Successfully..";
            title = "Save";
            type = 'success';//error
            break;
        case "fileuploadsuccess":
            msg = "File upload has been uploaded successfully.";
            title = "Upload";
            type = 'success';//error
            break;
        case "filedeletesuccess":
            msg = "File has been deleted successfully.";
            title = "Delete";
            type = 'success';//error
            break;
        case "ProcessSuccess":
            msg = "Process Has been successfully.";
            title = "";
            type = 'success';//error
            break;
        case "MFGStockNotMatch":
            msg = "Your MFG stock Data Not Mached...";
            title = "";
            type = 'info';//error
            break;
        case "CertificationIssueSaveSucess":
            msg = "Certification Issue Save successfully.";
            title = "Save";
            type = 'success';//error
            break;
        case "datanotfound":
            msg = "Opps... data not found.";
            title = "";
            type = 'warning';//error
            break;
        case "FileExists":
            msg = "File already exists.";
            title = "Warning";
            type = 'info';//error
            break;
        default:
            break;
    }

    toaster_message(type, title, msg)
}
function validationMessage(Msg) {
    msg = Msg;
    title = "";
    type = 'info';//error
    toaster_message(type, title, msg)
}

function Noty(Type, Text) {
    noty({
        text: Text,
        type: Type, //success , error,warning,info
        timeout: 4000,
        theme: 'relax',
        layout: 'topCenter',
        killer: true
    });
}