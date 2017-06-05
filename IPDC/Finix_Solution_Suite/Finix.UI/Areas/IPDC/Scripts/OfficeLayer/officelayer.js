$(document).ready(function () {
    function populateParentOffice(isEdit) {
        
        var parentidCombo = $("#tr_ParentId select");
        
        $(parentidCombo).attr("id", "ParentId").attr("name", "ParentId");
        var selectedOfficeLayerId = $("#jqGrid").jqGrid('getRowData', $("#jqGrid")[0].p.selrow).Id | 0;
        $.ajax({
            url:'/IPDC/Office/GetParentOffices/?officelayerid=' + selectedOfficeLayerId,
            type: "GET",
            success: function (officejson) {
                var offices = eval(officejson);
                var officeHtml = "";
                $(offices).each(function (i, option) {
                    officeHtml += '<option value="' + option.Id + '">' + option.Name + '</option>';
                });
                $(parentidCombo).removeAttr("disabled").html(officeHtml);
                if (isEdit) {
                    var selectedParentId = $("#jqGridDetails").jqGrid('getRowData', $("#jqGridDetails")[0].p.selrow).ParentId | 0;
                    $(parentidCombo).val(selectedParentId);
                } else {
                    $(parentidCombo).selectedIndex = 0;
                }
                $(parentidCombo).focus();
            }
        });
        
    }
    
     $(function () {
        $("#jqGrid").jqGrid({
            url: "/IPDC/OfficeLayer/GetOfficeLayers",
            datatype: 'json',
            mtype: 'Get',
            colNames: ['Id', 'Name', 'Level'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: false },
                { key: false, name: 'Name', index: 'Name', editable: true,editrules: { custom_func: validateText, custom: true, required: true } , searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'Level', index: 'Level', editable: true,editrules: { number: true, maxValue: 100, required: true }, align: "right", searchoptions: { sopt: ['eq', 'ne'] },classes: "grid-col"},
                
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
            caption: 'OfficeLayer Records',
            onSelectRow: function (rowid, selected){
                if (rowid != null) {
                    var dataFromCellByColumnName = jQuery('#jqGrid').jqGrid('getCell', rowid, 'Level');
                    var valueFromCellByColumnName = jQuery('#jqGrid').jqGrid('getCell', rowid, 'Name');
                    jQuery("#jqGridDetails").jqGrid('setGridParam', { url: "/IPDC/Office/GetOfficeByLayers/?officelevel=" + dataFromCellByColumnName, datatype: 'json' }); // the last setting is for demo only
                    jQuery("#jqGridDetails").jqGrid('setCaption', 'Office Records::' + valueFromCellByColumnName);
                    jQuery("#jqGridDetails").trigger("reloadGrid");

                }
            }, // use the onSelectRow that is triggered on row click to show a details grid
            emptyrecords: 'No OfficeLayer Records are Available to Display',
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
            url: '/IPDC/OfficeLayer/SaveOfficeLayer',
            closeOnEscape: true,
            width: 'auto',
            height: 'auto',
            closeAfterEdit: true,
            recreateForm: true,
            
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: "/IPDC/OfficeLayer/SaveOfficeLayer",
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
            url: "/IPDC/OfficeLayer/DeleteOfficeLayer",
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
        //selRowId = $('#jqGrid').jqGrid('getGridParam', 'selrow');
        //IdFromCellByColumnName = $('#jqGrid').jqGrid('getCell', selRowId, 'Id');

    $("#jqGridDetails").jqGrid({
        url: '/IPDC/Grade/EmptyJson',
        mtype: "GET",
        datatype: "json",
       
        colNames: ['Id', 'Name', 'Office Layer Name', 'ParentId', 'Parent Name'],
        colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: false },
                { key: false, name: 'Name', index: 'Name', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, formoptions: { label: "Name" }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'OfficeLayerName', index: 'OfficeLayerName', label: "OfficeLayerName", editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'ParentId', index: 'ParentId', editable: true, edittype: "select", editrules: { edithidden: true }, formoptions: { label: "Parent: " } },

                { key: false, name: 'ParentName', index: 'ParentName', label: "ParentName", editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },

        ],
        ondblClickRow: function (rowid) {
            jQuery("#jqGrid").jqGrid('editGridRow', rowid);
        },
        width: 780,
        rowNum: 5,
        loadonce: false,
        height: '100',
        viewrecords: true,
        caption: 'Detail grid::',
        onSelectRow: function (rowid, selected) {
            if (rowid != null) {
                var dataFromCellByColumnName = jQuery('#jqGridDetails').jqGrid('getCell', rowid, 'Id');
                var valueFromCellByColumnName = jQuery('#jqGridDetails').jqGrid('getCell', rowid, 'Name');
                jQuery("#jqGridUnitDetails").jqGrid('setGridParam', { url: "/OfficeUnit/GetOfficeUnitSettingsByOfficeId/?officeid=" + dataFromCellByColumnName, datatype: 'json' }); // the last setting is for demo only
                jQuery("#jqGridUnitDetails").jqGrid('setCaption', 'Office Unit Records::' + valueFromCellByColumnName);
                jQuery("#jqGridUnitDetails").trigger("reloadGrid");

            }
        }, // use the onSelectRow that is triggered on row click to show a details grid
        pager: "#jqGridDetailsPager"
    }).navGrid('#jqGridDetailsPager',
        { edit: true, add: true, del: true, search: true, refresh: true },
        {
            zIndex: 100,
            url: '/IPDC/Office/SaveOffice',
            closeOnEscape: true,
            closeAfterEdit: true,
            width: 'auto',
            height: 'auto',
            recreateForm: true,
            onInitializeForm: function (formId) { populateParentOffice(true); },
            onclickSubmit: function (params, postdata) {
                postdata = $.extend({}, postdata, { OfficeLayerId: $("#jqGrid").jqGrid('getRowData', $("#jqGrid")[0].p.selrow).Id });
                return postdata;
            },
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: '/IPDC/Office/SaveOffice',
            closeOnEscape: true,
            width: 'auto',
            height: 'auto',
            closeAfterAdd: true,
            onInitializeForm: function (formId) { populateParentOffice(false); },
            onclickSubmit: function (params, postdata) {
                postdata = $.extend({}, postdata, { OfficeLayerId:  $("#jqGrid").jqGrid('getRowData', $("#jqGrid")[0].p.selrow).Id });
                return postdata;
            },
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: "/IPDC/Office/DeleteOffice",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure to delete this Office? ",
            afterComplete: function (response) {
                if (response.responseText) {
                    Messager.ShowMessage(response.responseText);
                }
            }
        },
        {
            closeOnEscape: true, multipleSearch: true,
            closeAfterSearch: true
        });

        
});
   
});
