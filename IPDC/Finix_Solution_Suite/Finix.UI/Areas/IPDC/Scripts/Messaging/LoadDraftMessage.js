/*tinymce-knockout-binding v1.1.0|(c) 2014 Michael Papworth|https://raw.github.com/michaelpapworth/tinymce-knockout-binding/master/LICENSE*/
!function (a, b) { var c = { after: ["attr", "value"], defaults: {}, extensions: {}, init: function (e, f, g, h, i) { if (!b.isWriteableObservable(f())) throw "valueAccessor must be writeable and observable"; var j = g.has("wysiwygConfig") ? g.get("wysiwygConfig") : null, k = g.has("wysiwygExtensions") ? g.get("wysiwygExtensions") : [], l = d(c.defaults, k, j, arguments); return a(e).text(f()()), setTimeout(function () { a(e).tinymce(l) }, 0), b.utils.domNodeDisposal.addDisposeCallback(e, function () { a(e).tinymce().remove() }), { controlsDescendantBindings: !0 } }, update: function (b, c, d, e, f) { var g = a(b).tinymce(), h = c()(); g && g.getContent() !== h && (g.setContent(h), g.execCommand("keyup")) } }, d = function (d, e, f, g) { var h = a.extend(!0, {}, d); f && (b.utils.objectForEach(f, function (a) { "[object Array]" === Object.prototype.toString.call(f[a]) && (h[a] || (h[a] = []), f[a] = b.utils.arrayGetDistinctValues(h[a].concat(f[a]))) }), a.extend(!0, h, f)), h.plugins ? a.inArray("paste", h.plugins) && h.plugins.push("paste") : h.plugins = ["paste"]; var i = function (a) { a.on("change keyup nodechange", function (b) { g[1]()(a.getContent()); for (var d in e) e.hasOwnProperty(d) && c.extensions[e[d]](a, b, g[2], g[4]) }) }; if ("function" == typeof h.setup) { var j = h.setup; h.setup = function (a) { j(a), i(a) } } else h.setup = i; return h }; b.bindingHandlers.wysiwyg = c }(jQuery, ko), function (a) { a.extensions.dirty = function (a, b, c, d) { if (c.has("wysiwygDirty")) { var e = c.get("wysiwygDirty"); if (!ko.isWriteableObservable(e)) throw "wysiwygDirty must be writeable and observable"; e(a.isDirty()) } else d.$root.isDirty = a.isDirty() } }(ko.bindingHandlers.wysiwyg), function (a) { a.extensions.wordcount = function (a, b, c, d) { var e = a.plugins.wordcount; if (e && c.has("wysiwygWordCount")) { var f = c.get("wysiwygWordCount"); if (!ko.isWriteableObservable(f)) throw "wysiwygWordCount must be writeable and observable"; f(e.getCount()) } } }(ko.bindingHandlers.wysiwyg);
ko.bindingHandlers['wysiwyg'].defaults = {
    'height': 500,
    'toolbar': 'undo redo | insert | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image',
    'menubar': false,
    'statusbar': false,
    'plugins': [
                "advlist autolink lists link image charmap print preview hr anchor pagebreak",
                "searchreplace wordcount visualblocks visualchars code fullscreen",
                "insertdatetime media nonbreaking save table contextmenu directionality",
                "emoticons template paste textcolor colorpicker textpattern"
    ]
};

$(document).ready(function () {
    //function TyniMceVm() {
    //    tinymce.init({
    //        height: 200,
    //        //width: 600,
    //        selector: "#textarea",
    //        theme: "modern",
    //        menubar: false,
    //        plugins: [
    //            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
    //            "searchreplace wordcount visualblocks visualchars code fullscreen",
    //            "insertdatetime media nonbreaking save table contextmenu directionality",
    //            "emoticons template paste textcolor colorpicker textpattern"
    //        ],
    //        //toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | forecolor backcolor emoticons | fontselect | fontsizeselect",
    //        toolbar1: "undo redo | insert | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
    //        //toolbar2: "print preview media | forecolor backcolor emoticons",
    //        image_advtab: true,
    //        templates: [
    //            { title: 'Test template 1', content: 'Test 1' },
    //            { title: 'Test template 2', content: 'Test 2' }
    //        ]
    //    });
    //}

    //ko.applyBindings(new TyniMceVm());

    var appvm;
    //ko.validation.init({
    //    errorElementClass: 'has-error',
    //    errorMessageClass: 'help-block',
    //    decorateInputElement: true,
    //    grouping: { deep: true, observable: true }
    //});

    function LoadDraftMessageVM() {

        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.AccountTitle = ko.observable();
        self.MessagingOffice = ko.observable();
        self.RepliedTo = ko.observable();

        self.FromEmpId = ko.observable();
        self.FromEmpName = ko.observable();
        self.ToEmpId = ko.observable();
        self.ToEmpName = ko.observable();
        self.FromOfficeDesignationSettingId = ko.observable();
        self.FromDesignationName = ko.observable();
        self.ToOfficeDesignationSettingId = ko.observable();
        self.ToDesignationName = ko.observable();
        self.Message = ko.observable();
        //self.MessageType = ko.observable();
        self.IsDraft = ko.observable();
        self.ApplicationList = ko.observable([]);
        self.FromEmpList = ko.observableArray([]);
        self.ToEmpList = ko.observableArray([]);
        self.FromDesignationList = ko.observableArray([]);
        self.ToDesignationList = ko.observableArray([]);
        self.IpdcMessaging = ko.observableArray([]);

        self.SelectedApplicationId = ko.observable();
        self.SelectedApplicationId.subscribe(function () {
            self.ApplicationId('');
            self.ApplicationNo('');
            self.AccountTitle('');
            if (typeof (self.SelectedApplicationId()) != 'undefined' && self.SelectedApplicationId().Id > 0) {
                self.ApplicationId(self.SelectedApplicationId().Id);
                self.ApplicationNo(self.SelectedApplicationId().ApplicationNo);
                self.AccountTitle(self.SelectedApplicationId().AccountTitle);
            }
        });

        self.GetApplicationList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetAllApplications',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApplicationList(data);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetEployeeList = function () {

            return $.ajax({
                type: "GET",
                url: '/IPDC/Employee/GetEmployeeWithDesignation',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ToEmpList(data);
                    self.FromEmpList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SendMessage = function () {
            self.IsDraft(false);
            self.SaveNewMessage();
        }
        self.SaveNewMessage = function () {

            var SubmitData = {
                Id: self.Id(),
                ApplicationId: self.ApplicationId(),
                ApplicationNo: self.ApplicationNo(),
                AccountTitle: self.AccountTitle(),
                MessagingOffice: self.MessagingOffice(),
                RepliedTo: self.RepliedTo(),
                FromEmpId: self.FromEmpId(), //
                FromEmpName: self.FromEmpName(),
                ToEmpId: self.ToEmpId(),
                ToEmpName: self.ToEmpName(),
                FromOfficeDesignationSettingId: self.FromOfficeDesignationSettingId(),
                FromDesignationName: self.FromDesignationName(),
                ToOfficeDesignationSettingId: self.ToOfficeDesignationSettingId(),
                ToDesignationName: self.ToDesignationName(),
                Message: self.Message(),
                MobileMessage: self.MobileMessage(),
                IsDraft: self.IsDraft()
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/Messaging/SaveNewMessage',
                data: ko.toJSON(SubmitData),
                contentType: "application/json",
                success: function (data) {
                    $('#cibSuccessModal').modal('show');
                    $('#cibSuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadDraftMeassage = function () {
            self.IpdcMessaging([]);
            var url = '/IPDC/Messaging/LoadDraftMessages?msgId=' + self.Id() + '&AppId=' + self.ApplicationId();
            return $.ajax({
                type: "GET",
                url: url,
                contentType: "application/json",
                success: function (data) {
                    //self.IpdcMessaging(data.IpdcMessaging);
                    self.Id(data.Id);
                    self.ApplicationId(data.ApplicationId);
                    self.ApplicationNo(data.ApplicationNo);
                    self.AccountTitle(data.AccountTitle);
                    self.ToEmpId(data.ToEmpId);
                    self.ToEmpName(data.ToEmpName);
                    self.Message(data.Message);
                    //self.MessageType(data.MessageType);
                },
                error: function () {
                    alert(error.status + "<--and-->" + error.statusText);
                }
            });
        };

        

        self.Initialize = function () {
            self.GetApplicationList();
            self.GetEployeeList();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    appvm = new LoadDraftMessageVM();
    //apptynymce = new TyniMceVm();
    appvm.Initialize();
    var qValue = appvm.queryString('AppId');
    appvm.ApplicationId(qValue);
    var selfId = appvm.queryString('msgId');
    appvm.Id(selfId);
    appvm.LoadDraftMeassage();
    ko.applyBindings(appvm, $('#loaddraftmessage')[0]);

});