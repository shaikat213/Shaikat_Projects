/*
 * KO-Bootstrap-datetimepicker
 * Copyright 2017 Md. Shariful Siddique
 * All Rights Reserved.
 * Use, reproduction, distribution, and modification of this code is subject to the terms and
 * conditions of the MIT license, available at http://www.opensource.org/licenses/mit-license.php
 *
 * Author: Md. Shariful Siddique
 * 
 * Dependency: bootstrap-datetimepicker.js
 */
$('body').on('click', '.input-group-addon', function (event, object) {
    $(this).siblings('input').focus();
});


ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        //initialize datepicker with some optional options
        var options = {
            format: 'DD/MM/YYYY HH:mm',
            defaultDate: valueAccessor()(),
            minDate: false,
            maxDate: false,
            showTodayButton: true,
            keepInvalid: true
        };

        if (allBindingsAccessor() !== undefined) {
            if (allBindingsAccessor().datepickerOptions !== undefined) {
                options.format = allBindingsAccessor().datepickerOptions.format !== undefined ? allBindingsAccessor().datepickerOptions.format : options.format;
                options.minDate = allBindingsAccessor().datepickerOptions.minDate !== undefined ? allBindingsAccessor().datepickerOptions.minDate : options.minDate;
                options.maxDate = allBindingsAccessor().datepickerOptions.maxDate !== undefined ? allBindingsAccessor().datepickerOptions.maxDate : options.maxDate;
                options.showTodayButton = allBindingsAccessor().datepickerOptions.showTodayButton !== undefined ? allBindingsAccessor().datepickerOptions.showTodayButton : options.showTodayButton;
                //
                options.keepInvalid = allBindingsAccessor().datepickerOptions.keepInvalid !== undefined ? allBindingsAccessor().datepickerOptions.keepInvalid : options.keepInvalid;
            }
        }

        $(element).datetimepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(event.date);
            }
        });

        var defaultVal = $(element).val();
        var value = valueAccessor();
        value(moment(defaultVal, options.format));
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var thisFormat = 'DD/MM/YYYY HH:mm';
        if (allBindingsAccessor() !== undefined) {
            if (allBindingsAccessor().datepickerOptions !== undefined) {
                thisFormat = allBindingsAccessor().datepickerOptions.format !== undefined ? allBindingsAccessor().datepickerOptions.format : thisFormat;
            }
        }
        var value = valueAccessor();
        var unwrapped = ko.utils.unwrapObservable(value());

        if (unwrapped === undefined || unwrapped === null) {
            element.value = new moment(new Date());
        } else {
            element.value = unwrapped.format(thisFormat);
        }
    }
};