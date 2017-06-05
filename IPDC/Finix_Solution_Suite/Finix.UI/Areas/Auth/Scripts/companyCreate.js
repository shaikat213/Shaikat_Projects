$(document).ready(function () {
    $(function () {
        $("#jqGrid").jqGrid({
            url: "/Auth/CompanyProfile/GetAllActiveCompanies",
            datatype: 'json',
            mtype: 'Get',
            colNames: ['Id', 'Name', 'Address', 'PhoneNo', 'Email', 'Fax', 'ContactPerson', 'CompanyType', 'CompanyTypeName', 'ParentId', 'Parent'], //validatePositive
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: false },
                { key: false, name: 'Name', index: 'Name', label: 'Name', editable: true, editrules: { custom_func: validateText, custom: true, required: true }, searchoptions: { sopt: ['eq', 'ne', 'cn'] } },
                { key: false, name: 'Address', index: 'Address', label: 'Address', editable: true,  searchoptions: { sopt: ['eq', 'ne', 'cn'] } },
                { key: false, name: 'PhoneNo', index: 'PhoneNo', label: 'PhoneNo', editable: true,  searchoptions: { sopt: ['eq', 'ne', 'cn'] } },
                { key: false, name: 'Email', index: 'Email', label: 'Email', editable: true,  searchoptions: { sopt: ['eq', 'ne', 'cn'] } },
                { key: false, name: 'Fax', index: 'Fax', label: 'Fax', editable: true, searchoptions: { sopt: ['eq', 'ne', 'cn'] } },
                { key: false, name: 'ContactPerson', index: 'ContactPerson', label: 'ContactPerson', editable: true, searchoptions: { sopt: ['eq', 'ne', 'cn'] } },
                { key: false, hidden: true, name: 'CompanyType', width: 140, index: 'CompanyType', editable: true, edittype: "select", editoptions: { dataUrl: '/CompanyProfile/GetCompanyProfile', cacheUrlData: true }, editrules: { edithidden: true, required: true }, formoptions: { label: "Company Type" } },
                { key: false, name: 'CompanyTypeName', index: 'CompanyTypeName', label: "Leave Type Name", editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" },
                { key: false, hidden: true, name: 'ParentId', width: 140, index: 'ParentId', editable: true, edittype: "select", editoptions: { dataUrl: '/CompanyProfile/GetAllCompanyListForGrid', cacheUrlData: true }, editrules: { edithidden: true, required: true }, formoptions: { label: "Parent" } },
                { key: false, name: 'ParentName', index: 'ParentName', label: "Parent Name", editable: false, searchoptions: { sopt: ['eq', 'ne', 'cn'] }, classes: "grid-col" }
            ],
            pager: jQuery('#jqControls'),
            rowNum: 10,
            rowList: [10, 20, 30, 40, 50,100,200,300,400,500,1000],
            hoverrows: true,
            sortable: true,
            width: '70%',
            viewrecords: true,
            caption: 'Application List',
            emptyrecords: 'No Records are Available to Display',
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
        { edit: true, add: true, search: true, refresh: true }, // del: true,
        {
            zIndex: 100,
            url: '/Auth/CompanyProfile/SaveCompanyProfile',
            closeOnEscape: true,
            width: 'auto',
            height: 'auto',
            closeAfterEdit: true,
            recreateForm: true,
            afterComplete: function (response) {
                Messager.ShowMessage(response.Message);
            }
        },
        {
            zIndex: 100,
            url: "/Auth/CompanyProfile/SaveCompanyProfile",
            closeOnEscape: true,
            width: 'auto',
            height: 'auto',
            closeAfterAdd: true,
            afterComplete: function (response) {
                Messager.ShowMessage(response.Message);
            }
        },
        //{
        //    zIndex: 100,
        //    url: "/Calendar/DeleteHolidayTypes",
        //    closeOnEscape: true,
        //    closeAfterDelete: true,
        //    recreateForm: true,
        //    msg: "Are you sure to delete this module? ",
        //    afterComplete: function (response) {
        //        Messager.ShowMessage(response.Message);
        //    }
        //},
        {
            closeOnEscape: true, multipleSearch: true,
            closeAfterSearch: true
        }
        );
    });
});