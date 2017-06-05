$(document).ready(function () {
    $(function () {
        $("#jqGrid").jqGrid({
            url: "/IPDC/Organization/GetOrganizations",
            datatype: 'json',
            mtype: 'Get',
            colNames: ['Id', 'Name', 'Address', 'Phone Number', 'Contact Person', 'OrganizationType', 'Organization Type', 'Comment', 'Priority', 'Priority'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: false },
                { key: false, name: 'Name', index: 'Name', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'Address', index: 'Address', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'PhoneNumber', index: 'PhoneNumber', editable: true, editrules: { custom_func: validatePositive, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'ContactPerson', index: 'ContactPerson', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                {
                    key: false, hidden: true, name: 'OrganizationType', width: 140, index: 'OrganizationType', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Organization/GetOrganizationTypes', cacheUrlData: true }, editrules: { edithidden: true, required: true },
                    formoptions: { label: "Grade" }
                },
                { key: false, name: 'OrganizationTypeName', index: 'OrganizationTypeName', label: "Organization Type Name", editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'Comment', index: 'Comment', editable: true, classes: "grid-col" },
                {
                    key: false, hidden: true, name: 'Priority', width: 140, index: 'Priority', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Organization/GetPriorities', cacheUrlData: true }, editrules: { edithidden: true, required: true },
                    formoptions: { label: "Priority" }
                },
                { key: false, name: 'PriorityName', index: 'PriorityName', label: "Priority Name", editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" }
            ],
            ondblClickRow: function (rowid) {
                jQuery("#jqGrid").jqGrid('editGridRow', rowid);
            },
            pager: jQuery('#jqControls'),
            rowNum: 10,
            rowList: [10, 20, 30, 40, 50,100,200,300,400,500,1000],
            hoverrows: true,
            sortable: true,
            width: '70%',
            viewrecords: true,
            caption: 'Organizations Records',
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
        { edit: true, add: true, del: false, search: true, refresh: true },
        {
            zIndex: 100,
            url: '/IPDC/Organization/SaveOrganizations',
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
            url: "/IPDC/Organization/SaveOrganizations",
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
            url: "/IPDC/Designation/DeleteDesign",
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