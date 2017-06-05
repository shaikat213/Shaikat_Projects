$(document).ready(function () {
    $(function () {
        $("#jqGrid").jqGrid({
            url: "/Auth/Submodule/GetSubModule",
            datatype: 'json',
            mtype: 'Get',
            colNames: ['Id', 'Name', 'Display Name', 'Description', 'Serial','ModuleId','Module Name'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: false },
                { key: false, name: 'Name', label: 'Name', index: 'Name', width: 140,editable: true, editrules: { custom_func: validateText, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'DisplayName', index: 'DisplayName', width: 140, editable: true, editrules: { custom_func: validateText, custom: true, required: true }, formoptions: { colpos: 2, rowpos: 1,label:"Display Name"}, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'Description', label: 'Description', index: 'Description', width: 140, editable: true, editrules: { custom_func: validateText, custom: true, required: true }, formoptions: { colpos: 1, rowpos: 2 }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'Sl',index: 'Sl', width: 140, editable: true, editrules: { custom_func: validatePositive, custom: true, required: true }, formoptions: { colpos: 2, rowpos: 2 }, align: 'right', searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                //{ key: false, name: 'Gender', index: 'Gender', editable: true, edittype: 'select', editoptions: { value: { 'M': 'Male', 'F': 'Female', 'N': 'None' } } },
                //{ key: false, name: 'ClassName', index: 'ClassName', editable: true, edittype: 'select', editoptions: { value: { '1': '1st Class', '2': '2nd Class', '3': '3rd Class', '4': '4th Class', '5': '5th Class' } } },
                {
                    key: false, hidden: true, name: 'ModuleId', width: 140, index: 'ModuleId', editable: true, edittype: "select", editoptions: { dataUrl: '/Auth/Submodule/GetCorrespondingModules', cacheUrlData: true }, editrules: { edithidden: true, required: true },
                    formoptions: { colpos: 1, rowpos: 3, label: "Module"}
                },//edithidden can allowus to edit hidden col
                { key: false, name: 'ModuleName', label: 'Module Name', index: 'ModuleName', editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" }

            ],
            ondblClickRow: function (rowid) {
                jQuery("#jqGrid").jqGrid('editGridRow', rowid);
            },
            pager: jQuery('#jqControls'),
            rowNum: 10,
            rowList: [10, 20, 30, 40, 50,100,200,300,400,500,1000],
            hoverrows: true,
            sortable: true,
            //width: '70%',
            viewrecords: true,
            caption: 'SubModule Records',
            emptyrecords: 'No SubModule Records are Available to Display',
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            autowidth: true,
            
            height: 'auto',//set auto height
            multiselect: false
        }).navGrid('#jqControls',
        { edit: true, add: true, del: true, search: true, refresh: true },
        {
            zIndex: 100,
            url: '/Auth/Submodule/SavaSubModule',
            closeOnEscape: true,
            width: 'auto',
            height: 'auto',
            closeAfterEdit: true,
            recreateForm: true,
            afterComplete: function (response) {
                Messager.ShowMessage(response.responseText);
            }
        },
        {
            zIndex: 100,
            url: "/Auth/Submodule/SavaSubModule",
            closeOnEscape: true,
            width: 'auto',
            height: 'auto',
            closeAfterAdd: true,
            afterComplete: function (response) {
                Messager.ShowMessage(response.responseText);
            }
        },
        {
            zIndex: 100,
            url: "/Auth/Submodule/DeleteSubModule",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure to delete this Submodule? ",
            afterComplete: function (response) {
                Messager.ShowMessage(response.responseText);
            }
        },
        {
            closeOnEscape: true, multipleSearch: true,
            closeAfterSearch: true
        }
        );
    });
});

