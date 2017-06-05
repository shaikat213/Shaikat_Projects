$(document).ready(function () {
    $(function () {
        $("#jqGrid").jqGrid({
            url: "/IPDC/OfficeDesignationSetting/GetAllDesignationSettings",
            datatype: 'json',
            mtype: 'Get',
            colNames: ['Id', 'Name', 'Office', 'OfficeId', 'Designation', 'DesignationId', 'Sequence', 'Parent Setting', 'Parent Setting Id'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: false },
                { key: false, name: 'Name', index: 'Name', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'OfficeName', index: 'OfficeName', editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                {
                    key: false, hidden: true, name: 'OfficeId', width: 140, index: 'OfficeId', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Office/GetAllOffice', cacheUrlData: true }, editrules: { edithidden: true, required: true },
                    formoptions: { label: "Office" }
                },
                { key: false, name: 'DesignationName', index: 'DesignationName', editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                {
                    key: false, hidden: true, name: 'DesignationId', width: 140, index: 'DesignationId', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Designation/GetDesignationList', cacheUrlData: true }, editrules: { edithidden: true, required: true },
                    formoptions: { label: "Designations" }
                },
                { key: false, name: 'Sequence', index: 'Sequence', editable: true, editrules: { custom_func: validatePositive, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'ParentDesignationSettingName', index: 'ParentDesignationSettingName', editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                {
                    key: false, hidden: true, name: 'ParentDesignationSettingId', width: 140, index: 'ParentDesignationSettingId', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/OfficeDesignationSetting/GetDesignationSettingList', cacheUrlData: true }, editrules: { edithidden: true },
                    formoptions: { label: "Parent Setting" }
                }
            ],
            ondblClickRow: function (rowid) {
                jQuery("#jqGrid").jqGrid('editGridRow', rowid);
            },
            pager: jQuery('#jqControls'),
            rowNum: 10,
            rowList: [10, 20, 30, 40, 50, 100, 200, 500,1000],
            hoverrows: true,
            sortable: true,
            width: '70%',
            viewrecords: true,
            caption: 'Organogram',
            emptyrecords: 'No Organizations Records are Available to Display',
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
            url: "/IPDC/OfficeDesignationSetting/SaveDesignationSetting",
            closeOnEscape: true,
            closeAfterEdit: true,
            width: 'auto',
            height: 'auto',
            recreateForm: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: "/IPDC/OfficeDesignationSetting/SaveDesignationSetting",
            closeOnEscape: true,
            width: 'auto',
            height: 'auto',
            closeAfterAdd: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
         {
             zIndex: 100,
             url: "/IPDC/OfficeDesignationSetting/DeleteOfficeDesignationSetting",
             closeOnEscape: true,
             closeAfterDelete: true,
             recreateForm: true,
             msg: "Are you sure to delete this Office Designation Mapping? ",
             afterComplete: function (response) {
                 Messager.ShowMessage(response.responseText);
             }
         },
        {
            zIndex: 100,
            url: "/IPDC/OfficeDesignationSetting/SaveDesignationSetting",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure to delete this Designation? ",
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
        {
            closeOnEscape: true, multipleSearch: true,
            closeAfterSearch: true
        }
        );
    });
});