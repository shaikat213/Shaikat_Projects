$(document).ready(function () {

    function populateSubmodules(isEdit) {
        var moduleCombo = $("#tr_ModuleId select");
        var submoduleCombo = $("#tr_SubModuleId select");
        $(moduleCombo).attr("id", "ModuleId").attr("name", "ModuleId");
        $(submoduleCombo).attr("id", "SubModuleId").attr("name", "SubModuleId");

        var selectedModuleId = $("#jqGrid").jqGrid('getRowData', $("#jqGrid")[0].p.selrow).ModuleId | 0;
        $(moduleCombo)
                     .html("<option value=''>Loading Modules...</option>")
                     .attr("disabled", "disabled");
        $.ajax({
            url: '/Auth/Submodule/GetCorrespondingModules',
            type: "GET",
            success: function (moduleHtml) {
                $(moduleCombo).removeAttr("disabled").html(moduleHtml);

                if (isEdit) {
                    $(moduleCombo).val(selectedModuleId);
                } else {
                    $(moduleCombo).selectedIndex = 0;
                }
                updateSubmoduleCallBack(isEdit, $(moduleCombo).val(), submoduleCombo);
            }
        });
        $(moduleCombo).bind("change", function (e) {
            updateSubmoduleCallBack(false, $(moduleCombo).val(), submoduleCombo);
        });
    }

    function updateSubmoduleCallBack(isEdit, selectedModuleId, submoduleCombo) {
        var url = '/Auth/Menu/GetCorrespondingSubModules/?moduleId=' + selectedModuleId;
        $(submoduleCombo)
             .html("<option value=''>Loading submodules...</option>")
             .attr("disabled", "disabled");
        $.ajax({
            url: url,
            type: "GET",
            success: function (submodJson) {
                var submods = eval(submodJson);
                var subModuleHtml = "";
                $(submods).each(function (i, option) {
                    subModuleHtml += '<option value="' + option.Id + '">' + option.DisplayName + '</option>';
                });
                $(submoduleCombo).removeAttr("disabled").html(subModuleHtml);
                if (isEdit) {
                    var selectedSubmoduleId = $("#jqGrid").jqGrid('getRowData', $("#jqGrid")[0].p.selrow).SubModuleId | 0;
                    $(submoduleCombo).val(selectedSubmoduleId);
                } else {
                    $(submoduleCombo).selectedIndex = 0;
                }
                $(submoduleCombo).focus();
            }
        });
    }
    $(function () {
        $("#jqGrid").jqGrid({
            url: "/Auth/Menu/GetMenus",
            datatype: 'json',
            mtype: 'Get',
            colNames: ['Id', 'Name', 'Display Name', 'Description', 'Sl', 'Url', 'Module Id', 'Module Name', 'SubModule Id', 'SubModule Name'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: false },
                { key: false, name: 'Name', index: 'Name', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'DisplayName', index: 'DisplayName', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, formoptions: { label: "DisplayName" }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'Description', index: 'Description', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'Sl', index: 'Sl', editable: true, editrules: { custom_func: validatePositive, custom: true, required: true }, align: 'right', searchoptions: { sopt: ['eq', 'ne'] }, classes: "grid-col" },
                { key: false, name: 'Url', index: 'Url', editable: true, editrules: {required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'ModuleId', index: 'ModuleId', editable: true, edittype: "select", editrules: { edithidden: true, required: true }, formoptions: { label: "Module: " } },
                { key: false, name: 'ModuleName', index: 'ModuleName', editable: false, label: "Module Name", searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'SubModuleId', index: 'SubModuleId', editable: true, edittype: "select", editrules: { edithidden: true, required: true }, formoptions: { label: "Submodule: " } },
                { key: false, name: 'SubModuleName', index: 'SubModuleName', label: "Submodule Name", editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" }
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
            caption: 'Menu Records',
            emptyrecords: 'No Menu Records are Available to Display',
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
            url: '/Auth/Menu/SaveMenu',
            closeOnEscape: true,
            closeAfterEdit: true,
            width: 'auto',
            height: 'auto',
            recreateForm: true,
            onInitializeForm: function (formId) { populateSubmodules(true); },
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: '/Auth/Menu/SaveMenu',
            closeOnEscape: true,
            width: 'auto',
            height: 'auto',
            closeAfterAdd: true,
            onInitializeForm: function (formId) { populateSubmodules(false); },
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: "/Auth/Menu/DeleteMenu",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure to delete this Submodule? ",
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

        $('#jqGrid').setGroupHeaders(
                {
                    useColSpanStyle: true,
                    groupHeaders: [
                        { "numberOfColumns": 4, "titleText": "General Info", "startColumnName": 'Name' },
                        { "numberOfColumns": 5, "titleText": "Secondary Details", "startColumnName": 'Url' }]

                });

    });
});
