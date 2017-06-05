$(function () {
    $('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#NextFollowUpEdit').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#NextFollowUpDetails').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
});

ko.bindingHandlers.jqAuto = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var options = valueAccessor() || {},
            allBindings = allBindingsAccessor(),
            unwrap = ko.utils.unwrapObservable,
            modelValue = allBindings.jqAutoValue,
            source = allBindings.jqAutoSource,
            valueProp = allBindings.jqAutoSourceValue,
            inputValueProp = allBindings.jqAutoSourceInputValue || valueProp,
            labelProp = allBindings.jqAutoSourceLabel || valueProp;

        //function that is shared by both select and change event handlers
        function writeValueToModel(valueToWrite) {
            if (ko.isWriteableObservable(modelValue)) {
                modelValue(valueToWrite);
            } else {  //write to non-observable
                if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['jqAutoValue'])
                    allBindings['_ko_property_writers']['jqAutoValue'](valueToWrite);
            }
        }

        //on a selection write the proper value to the model
        options.select = function (event, ui) {
            writeValueToModel(ui.item ? ui.item.actualValue : null);
        };

        //on a change, make sure that it is a valid value or clear out the model value
        options.change = function (event, ui) {
            var currentValue = $(element).val();
            var matchingItem = ko.utils.arrayFirst(unwrap(source), function (item) {
                return unwrap(item[inputValueProp]) === currentValue;
            });

            if (!matchingItem) {
                writeValueToModel(null);
            }
        }


        //handle the choices being updated in a DO, to decouple value updates from source (options) updates
        var mappedSource = ko.dependentObservable(function () {
            mapped = ko.utils.arrayMap(unwrap(source), function (item) {
                var result = {};
                result.label = labelProp ? unwrap(item[labelProp]) : unwrap(item).toString();  //show in pop-up choices
                result.value = inputValueProp ? unwrap(item[inputValueProp]) : unwrap(item).toString();  //show in input box
                result.actualValue = valueProp ? unwrap(item[valueProp]) : item;  //store in model
                return result;
            });
            return mapped;
        });

        //whenever the items that make up the source are updated, make sure that autocomplete knows it
        mappedSource.subscribe(function (newValue) {
            $(element).autocomplete("option", "source", newValue);
        });

        options.source = mappedSource();

        //initialize autocomplete
        $(element).autocomplete(options);
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //update value based on a model change
        var allBindings = allBindingsAccessor(),
            unwrap = ko.utils.unwrapObservable,
            modelValue = unwrap(allBindings.jqAutoValue) || '',
            valueProp = allBindings.jqAutoSourceValue,
            inputValueProp = allBindings.jqAutoSourceInputValue || valueProp;

        //if we are writing a different property to the input than we are writing to the model, then locate the object
        if (valueProp && inputValueProp !== valueProp) {
            var source = unwrap(allBindings.jqAutoSource) || [];
            var modelValue = ko.utils.arrayFirst(source, function (item) {
                return unwrap(item[valueProp]) === modelValue;
            }) || {};  //probably don't need the || {}, but just protect against a bad value          
        }

        //update the element with the value that should be shown in the input
        $(element).val(modelValue && inputValueProp !== valueProp ? unwrap(modelValue[inputValueProp]) : modelValue.toString());
    }
};

ko.bindingHandlers.jqAutoCombo = {
    init: function (element, valueAccessor) {
        var autoEl = $("#" + valueAccessor());

        $(element).click(function () {
            // close if already visible
            if (autoEl.autocomplete("widget").is(":visible")) {
                autoEl.autocomplete("close");
                return;
            }

            //autoEl.blur();
            autoEl.autocomplete("search", " ");
            autoEl.focus();

        });

    }
};
//end of auto complete

$(document).ready(function () {

    function EmpDesignationMappingVm() {
        var self = this;
        // for auto complete
        self.minPrefixLength = 1;
        self.prefix = ko.observable('');
        self.poNumberList = ko.observableArray([]);
        self.selectedPoNoKey = ko.observable('');
        self.getData = function (prefix) {
            //$.getJSON("/IPDC/Employee/GetEmployeeNumberList?prefix=" + prefix, function (data) {
            $.getJSON("/IPDC/Employee/GetEmployeeNumberList", function (data) {
                self.EmployeeList(data);
            });
        };
        self.loadData = function () {
            if (self.prefix().length > self.minPrefixLength) {
                self.getData(self.prefix());
            }
        };

        self.loadPO = ko.computed(function () {

            if (self.selectedPoNoKey() && self.selectedPoNoKey().length > 0) {
                self.PONumber(self.selectedPoNoKey());
                self.getPurchaseReceiveByPoNumber();
            }
        });

        // end of auto complete

        //// for auto complete
        //self.minPrefixLength = 1;
        //self.prefix = ko.observable('');
        self.EmployeeList = ko.observableArray([]);
        self.EmployeeId = ko.observable();
        //self.selectedEmpNoKey = ko.observable('');

        //self.getData = function (prefix) {

        //    $.getJSON("/IPDC/Employee/GetEmployeeNumberList?prefix=" + prefix, function (data) {
        //        self.EmployeeList(data);
        //    });
        //};
        //self.loadData = function () {
        //    if (self.prefix().length > self.minPrefixLength) {
        //        self.getData(self.prefix());
        //    }
        //};

        //self.loadEmp = ko.computed(function () {
        //    if (self.selectedEmpNoKey() && self.selectedEmpNoKey().length > 0) {
        //        self.EmployeeId(self.selectedEmpNoKey());
        //        //self.getPurchaseReceiveByPoNumber();
        //    }
        //});
        ////auto complete ends

        self.OfficeLayers = ko.observableArray([]);
        self.OfficeLayerId = ko.observable('');
        self.Offices = ko.observableArray([]);
        self.OfficeId = ko.observable('');
        self.Designations = ko.observableArray([]);
        self.DesignationId = ko.observable('');
        self.DesignatedEmployees = ko.observableArray();
        
        self.EmployeeId = ko.observable();
        
        
        self.Initialize = function () {
            self.LoadOfficeLayers();
            self.getData('');
        };

        self.LoadOfficeLayers = function(){
            return $.getJSON("/IPDC/OfficeLayer/GetOfficeLayers", null, function (data) {
                self.OfficeLayers(data);
            });
        }
        self.LoadOffices = function () {
            if (self.OfficeLayerId() > 0) {
                return $.getJSON("/IPDC/Office/GetOfficeByLayer?officelayerid=" + self.OfficeLayerId(), null, function (data) {
                    self.Offices(data);
                });
            }
        }
        self.LoadDesignations = function () {
            if (self.OfficeId() > 0) {
                return $.getJSON("/IPDC/OfficeDesignationSetting/GetDesignationSettingListByOffice?OfficeId=" + self.OfficeId(), null, function (data) {
                    self.Designations(data);
                });
            }
        }
        self.LoadDesignatedEmployees = function () {
            if (self.DesignationId() > 0) {
                return $.getJSON("/IPDC/OfficeDesignationSetting/GetDesignatedEmployees?SettingId=" + self.DesignationId(), null, function (data) {
                    self.DesignatedEmployees(data);
                });
            }
        }

        self.RemoveDesignation = function (line) {
            var postData = {
                OfficeDesignationSettingId: line.OfficeDesignationSettingId,
                EmployeeId: line.EmployeeId,
                Id: line.Id
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/OfficeDesignationSetting/RemoveEmployeeDesignation',
                data: ko.toJSON(postData),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    self.LoadDesignatedEmployees();
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.Save = function () {
            //self.FollowUpCallTimeText($("#NextFollowUpEdit").val());
            var postData;
            postData = {
                EmployeeId: self.EmployeeId(),
                OfficeDesignationSettingId: self.DesignationId()
            };
            $.ajax({
                type: "POST",
                url: '/IPDC/OfficeDesignationSetting/SaveEmployeeDesignation',
                data: ko.toJSON(postData),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
            ////Ends Here EditLead 
        };
        self.Search = function () {

        }

    }

    var vm = new EmpDesignationMappingVm();
    vm.Initialize();
    ko.applyBindings(vm, document.getElementById("employeeDesignationMapping"));
});