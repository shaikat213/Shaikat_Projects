function validateNID(value, column) {
    var regex = /^[0-9]{17}$/;
    if (regex.test(value)) {
        return [true, ""];
    }
    else {
        
        //$('#' + column).focus();
        //$('#' + column).val("");
        return [false, column + " :Please enter a valid NID"];
    }
}

function validateText(value, column) {
    var regex = /^[ a-zA-Z0-9'.,-]{2,100}$/;
    if (regex.test(value)) {
        return [true, ""];
    }
    else {
       
        //$('#'+column).focus();
        //$('#'+column).val("");
        return [false, column+" :Please enter a valid text"];
    }
}

function validatePositive(value, column) {
    if (isNaN(value) || value < 0){
        //$('#'+column).focus();
        //$('#' + column).val("");
        return [false,  column+":Please enter a positive value"];
    }
        
    else
        return [true, ""];
}

function validateYear(value, column) {
    var regex = /^(19|20)\d{2}$/;
    if (regex.test(value)) {
        return [true, ""];
    }
    else {
        
        //$('#' + column).focus();
        //$('#' + column).val("");
        return [false, column + " :Please enter a valid year"];
    }
}
function validateFloat(value, column) {
    var regex = /^[0-9][.]?[0-9]+$/;
    if (regex.test(value)) {
        return [true, ""];
    }
    else {
        
        //$('#' + column).focus();
        //$('#' + column).val("");
        return [false, column + " :Please enter a valid float value"];
    }
}

function validateUrlText(value, column) {
    var regex = /^[/a-zA-Z_/ ]{2,100}$/;
    if (regex.test(value)) {
        return [true, ""];
    }
    else {
       
        //$('#' + column).focus();
        //$('#' + column).val("");
        return [false, column+" :Please enter a valid url"];
    }
}

function validatePercentage(value, column) {
    if (isNaN(value) || value < 0) {
        //$('#' + column).focus();
        //$('#' + column).val("");
        return [false, column + ":Please enter a positive value"];
    }
    else if (value > 100) {
        return [false, column + ":Please enter a value between 1 and 100"];
    }
    else
        return [true, ""];
}



