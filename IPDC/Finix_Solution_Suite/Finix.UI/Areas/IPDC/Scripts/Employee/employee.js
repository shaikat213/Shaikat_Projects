$(document).ready(function () {
    function fathernameformatter(cellvalue, options, rowObject) {
        return rowObject.FatherFirstName + ' ' + rowObject.FatherLastName;
    }
    function mothernameformatter(cellvalue, options, rowObject) {
        return rowObject.MotherFirstName + ' ' + rowObject.MotherLastName;
    }
    function populateDesignations(isEdit) {
        var gradeCombo = $("#tr_GradeId select:first");
        var designationCombo = $("#tr_GradeId select:last");
        $(gradeCombo).attr("id", "GradeId").attr("name", "GradeId");
        $(designationCombo).attr("id", "DesignationId").attr("name", "DesignationId");

        var selectedGradeId = $("#jqGrid").jqGrid('getRowData', $("#jqGrid")[0].p.selrow).GradeId | 0;
        $(gradeCombo)
                     .html("<option value=''>Loading grades...</option>")
                     .attr("disabled", "disabled");
        $.ajax({
            url: '/IPDC/Employee/GetCorrespondingGrade',
            type: "GET",
            success: function (gradeHtml) {
                $(gradeCombo).removeAttr("disabled").html(gradeHtml);

                if (isEdit) {
                    $(gradeCombo).val(selectedGradeId);
                } else {
                    $(gradeCombo).selectedIndex = 0;
                }
                updateDesignationCallBack(isEdit, $(gradeCombo).val(), designationCombo);
            }
        });
        $(gradeCombo).bind("change", function (e) {
            updateDesignationCallBack(false, $(gradeCombo).val(), designationCombo);
        });
    }

    function updateDesignationCallBack(isEdit, selectedGradeId, designationCombo) {
        var url = '/IPDC/Employee/GetCorrespondingDesignations/?gradeid=' + selectedGradeId;
        //
        $(designationCombo)
             .html("<option value=''>Loading designations...</option>")
             .attr("disabled", "disabled");
        $.ajax({
            url: url,
            type: "GET",
            success: function (designJson) {
                var designations = eval(designJson);
                var designationHtml = "";
                $(designations).each(function (i, option) {
                    designationHtml += '<option value="' + option.Id + '">' + option.Name + '</option>';
                });
                $(designationCombo).removeAttr("disabled").html(designationHtml);
                if (isEdit) {
                    var selectedDesignationId = $("#jqGrid").jqGrid('getRowData', $("#jqGrid")[0].p.selrow).DesignationId | 0;
                    $(designationCombo).val(selectedDesignationId);
                } else {
                    $(designationCombo).selectedIndex = 0;
                }
                $(designationCombo).focus();
            }
        });
    }

    $(function () {
        $("#jqGrid").jqGrid({
            url: "/IPDC/Employee/GetEmployeeList",
            datatype: 'json',
            mtype: 'Get',
            colNames: ['Id', 'First Name', 'Last Name', 'Date Of Birth', 'FatherFirstName', 'FatherLastName', 'Father Name', 'MotherFirstName', 'MotherLastName', 'Mother Name', 'CountryId', 'Gender', 'MaritalStatus', 'BloodGroup', 'NID', 'Religion', 'Employeetype Name', 'IMEI'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: false },
                //{ name: 'Edit', search: false, index: 'Edit', width: 60, sortable: false, formatter: editLink },
                //{ key: false, name: 'Name', index: 'Name', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, formoptions: { colpos: 1, rowpos: 1 }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'FirstName', index: 'FirstName', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, formoptions: { colpos: 1, rowpos: 1, label: "First Name"}, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'LastName', index: 'LastName', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, formoptions: { colpos: 2, rowpos: 1, label: "Last Name"}, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                {
                    name: "DateOfBirth", index: 'DateOfBirth', label: "DateOfBirth", formatter: "date", editable: true, editrules: { required: true }, formoptions: { colpos: 1, rowpos: 2, label: "Date Of Birth " }, edittype: "text",
                    editoptions: {
                        dataInit: function (element) {
                            $(element).datepicker({
                                id: 'DateOfBirth_datePicker',
                                dateFormat: 'dd/MM/yy',
                                //minDate: new Date(1970, 0, 1),
                                maxDate: new Date(),
                                //dateFormat: 'dd-M-yy',
                                changeYear: true,
                                showOn: 'focus'
                            });
                        }
                    }
                },
                { key: false, hidden: true, name: 'FatherFirstName', index: 'FatherFirstName', editable: true, editrules: { edithidden: true, required: true }, formoptions: { colpos: 1, rowpos: 3, label: "Father First Name" } },
                { key: false, hidden: true, name: 'FatherLastName', index: 'FatherLastName', editable: true, editrules: { edithidden: true, required: true }, formoptions: { colpos: 2, rowpos: 3, label: "Father Last Name" } },
                { key: false, name: 'FatherName', index: 'FatherName', editable: false, formatter: fathernameformatter, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'MotherFirstName', index: 'MotherFirstName', editable: true, editrules: { edithidden: true, required: true }, formoptions: { colpos: 1, rowpos: 4, label: "Mother First Name" } },
                { key: false, hidden: true, name: 'MotherLastName', index: 'MotherLastName', editable: true, editrules: { edithidden: true, required: true }, formoptions: { colpos: 2, rowpos: 4, label: "Mother Last Name" } },
                { key: false, name: 'MotherName', index: 'MotherName', editable: false, formatter: mothernameformatter, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'Nationality', width: 140, index: 'Nationality', editable: true, editrules: { edithidden: true, required: true },formoptions: { colpos: 1, rowpos: 5, label: "Nationality"}},
                //{ key: false, hidden: true, name: 'Nationality', index: 'Nationality', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Employee/GetAllNationality', cacheUrlData: true }, editrules: { edithidden: true, required: true }, formoptions: { colpos: 2, rowpos: 4, label: "Nationality" }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'Gender', index: 'Gender', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Employee/GetGenders', cacheUrlData: true }, editrules: { edithidden: true, required: true}, formoptions: { colpos: 2, rowpos: 5, label: "Gender" }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'MaritalStatus', index: 'MaritalStatus', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Employee/GetMaritalStatuses', cacheUrlData: true }, editrules: { edithidden: true, required: true }, formoptions: { colpos: 1, rowpos: 6, label: "Marital Status" }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'BloodGroup', index: 'BloodGroup', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Employee/GetBloodGroups', cacheUrlData: true }, editrules: { edithidden: true, required: true }, formoptions: { colpos: 2, rowpos: 6 }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, name: 'NID', index: 'NID', width: 148, editable: true, editrules: { edithidden: true, custom_func: validateNID, custom: true, required: true }, formoptions: { colpos: 1, rowpos: 7, label: "NID#" }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'Religion', index: 'Religion', editable: true, editrules: { edithidden: true, required: true }, edittype: "select", editoptions: { dataUrl: '/IPDC/Employee/GetReligions', cacheUrlData: true }, formoptions: { colpos: 2, rowpos: 7 }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'EmployeeType', index: 'EmployeeType', editable: true, edittype: "select", editoptions: { dataUrl: '/IPDC/Employee/GetEmployeeType', cacheUrlData: true }, editrules: { edithidden: true, required: true }, formoptions: { colpos: 1, rowpos: 8, label: "Emplyee Type" }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'IMEI', index: 'IMEI', editable: true, editrules: { required: false }, formoptions: { colpos: 2, rowpos: 8, label: "IMEI" }, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" }
                
                //{
                //    name: "JoiningDate", index: 'JoiningDate', label: "JoiningDate", formatter: "date", editable: true, editrules: { required: true }, formoptions: { colpos: 2, rowpos: 7, label: "Joining Date" }, edittype: "text",
                //    editoptions: {
                //        dataInit: function (element) {
                //            $(element).datepicker({
                //                id: 'JoiningDate_datePicker',
                //                dateFormat: 'M/d/yy',
                //                //minDate: new Date(1970, 0, 1),
                //                maxDate: new Date(),
                //                //dateFormat: 'dd-M-yy',
                //                changeYear: true,
                //                showOn: 'focus'
                //            });
                //        }
                //    }
                //},
                //{ key: false, name: 'EmployeeTypeName', label: 'EmployeeTypeName', index: 'EmployeeTypeName', editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" }

            ],
            ondblClickRow: function (rowid) {
                jQuery("#jqGrid").jqGrid('editGridRow', rowid);
            },
            pager: jQuery('#jqControls'),
            rowNum: 10,
            rowList: [10, 20, 30, 40, 50, 100, 150, 300, 500, 1000],
            hoverrows: true,
            sortable: true,
            viewrecords: true,
            caption: 'Employee Records',
            emptyrecords: 'No Employee Records are Available to Display',

            //subGridRowExpanded: function(subgrid_id, row_id) {
            //    // we pass two parameters
            //    // subgrid_id is a id of the div tag created within a table
            //    // the row_id is the id of the row
            //    // If we want to pass additional parameters to the url we can use
            //    // the method getRowData(row_id) - which returns associative array in type name-value
            //    // here we can easy construct the following
            //    var subgrid_table_id, pager_id;
            //    subgrid_table_id = subgrid_id + "_t";
            //    pager_id = "p_" + subgrid_table_id;
            //    jQuery("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
            //    var dataFromCellByColumnName = jQuery('#jqGrid').jqGrid('getCell', row_id, 'Id');
            //    var innergrid=jQuery("#" + subgrid_table_id).jqGrid({
            //        url: "/Employee/GetCorrespondingLeaveApplication/" + dataFromCellByColumnName,
            //        datatype: "json",
            //        colNames: ['Employee Id', 'Employee Name', 'Description', 'Total Days'],
            //        colModel: [
            //          { name: "EmployeeId", index: "EmployeeId", width: 80, key: true },
            //          { name: "EmployeeName", index: "EmployeeName", width: 130 },
            //          { name: "Description", index: "Description", width: 130, align: "right" },
            //          { name: "TotalDays", index: "TotalDays", width: 130, align: "right" },

            //        ],
            //        height: '100%',
            //        rowNum: 20,
            //        pager: pager_id,
            //        sortname: 'EmployeeName',
            //        sortorder: "asc",
            //        cmTemplate: { align: 'center', editable: true }

            //    });
            //    jQuery("#" + subgrid_table_id).jqGrid('navGrid', "#" + pager_id, { edit: true, add: true, del: true })

            //},
            //subGridOptions: {
            //    // configure the icons from theme rolloer
            //    plusicon: "ui-icon-triangle-1-e",
            //    minusicon: "ui-icon-triangle-1-s",
            //    openicon: "ui-icon-arrowreturn-1-e"
            //},

            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            autowidth: true,
            height: 'auto',
            multiselect: false
        }).navGrid('#jqControls',
        { edit: false, add: true, del: true, search: true, refresh: true },
        {
            zIndex: 100,
            url: '/IPDC/Employee/SaveEmployee',
            closeOnEscape: true,
            closeAfterEdit: true,
            recreateForm: true,
            width: 'auto',
            height: 'auto',
            recreateForm: true,
            //onInitializeForm: function (formId) { populateDesignations(true); },
            afterComplete: function (response) {
                if (response.responseText) {
                    toastr.success(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            width: 'auto',
            height: 'auto',
            url: "/IPDC/Employee/SaveEmployee",
            closeOnEscape: true,
            closeAfterAdd: true,
            //onInitializeForm: function (formId) { populateDesignations(false); },
            afterComplete: function (response) {
                if (response.responseText) {
                    toastr.success(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: "/IPDC/Employee/DeleteEmployee",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure to delete this Employee? ",
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
        jQuery("#jqGrid").jqGrid('navButtonAdd', '#jqControls', {                   //add icon to pop-up modal
            caption: "",
            position: "first",
            buttonicon: "ui-icon-pencil",
            title: "Edit new",
            onClickButton: function () {
                if (check()) {
                    $('#myModal').modal('show');

                } else {
                    toastr.info("Please select a row");
                }



            }
        });
        
        
       // $("#jqGrid").jqGrid('navGrid', '#jqControls', {}, { width: 500 });

$('#jqGrid').setGroupHeaders(
       {
           useColSpanStyle: true,
           groupHeaders: [
               { "numberOfColumns": 5, "titleText": "General Info", "startColumnName": 'Name' },
               //{ "numberOfColumns": 5, "titleText": "Secondary Details", "startColumnName": 'Nationality' }
           ]
       });


    });
});


//function editLink(cellValue, options, rowdata, action) {
//    return "<a data-toggle='modal' href='#myModal' onclick='check()' class='ui-icon ui-icon-pencil' ></a>";
//}
var val;
function check() {
    var myGrid = $('#jqGrid');
    selRowId = myGrid.jqGrid('getGridParam', 'selrow');
    if (selRowId!=null) {
        celValue = myGrid.jqGrid('getCell', selRowId, 'Id');
        val = celValue;
        return true;
    } else {
        return false;
    }
    
}

//function populateGrades() {
//    // first of all update the city based on the country               
//    updateDesignationCallBack($("#GradeId").val());
//    // then hook the change event of the country dropdown so that it updates cities all the time
//    $("#GradeId").bind("change", function (e) {
//        updateDesignationCallBack($("#GradeId").val());
//    });
//}

//function updateDesignationCallBack(gradeid) {
//    // 
//    //var current = $("#jqGrid").jqGrid('getRowData', $("#jqGrid")[0].p.selrow).SubModuleId;
//    $("#DesignationId")
//         .html("<option value=''>Loading designations...</option>")
//         .attr("disabled", "disabled");
//    $.ajax({
//        url:'Url.Action("GetCorrespondingDesignations","Employee")' + "/" + gradeid,
//        type: "GET",
//        success: function (citiesJson) {
//            var submods = eval(citiesJson);
//            var citiesHtml = "";
//            $(submods).each(function (i, option) {
//                citiesHtml += '<option value="' + option.Id + '">' + option.Name + '</option>';
//            });

//            $("#SubModuleId").removeAttr("disabled").html(citiesHtml);
//        }
//    });
//}