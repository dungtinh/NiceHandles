$(document).ready(function () {
    $(".sortTable th").click(function () {
        var index = Array.from(this.parentNode.children).indexOf(this);
        var isNumber = $(this).hasClass("number");
        sortTable(index, isNumber);
    });
});
var updown = true;
function sortTable(index, isNumber) {
    updown = updown ? false : true;
    var table, rows, switching, i, x, y, shouldSwitch;
    table = $(".sortTable")[0];
    switching = true;
    /*Make a loop that will continue until
    no switching has been done:*/
    while (switching) {
        //start by saying: no switching is done:
        switching = false;
        rows = table.rows;
        /*Loop through all table rows (except the
        first, which contains table headers):*/
        for (i = 1; i < (rows.length - 1); i++) {
            //start by saying there should be no switching:
            shouldSwitch = false;
            /*Get the two elements you want to compare,
            one from current row and one from the next:*/
            x = rows[i].getElementsByTagName("TD")[index];
            y = rows[i + 1].getElementsByTagName("TD")[index];
            //check if the two rows should switch place:            
            if (updown) {
                if (isNumber) {
                    if ((parseInt(x.innerHTML, 10) || 0) > (parseInt(y.innerHTML, 10) || 0)) {
                        //if so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }
                }
                else {
                    if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                        //if so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }
                }
            } else {
                if (isNumber) {
                    if ((parseInt(x.innerHTML, 10) || 0) < (parseInt(y.innerHTML, 10) || 0)) {
                        //if so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }
                }
                else {
                    if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                        //if so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }
                }
            }
        }
        if (shouldSwitch) {
            /*If a switch has been marked, make the switch
            and mark that a switch has been done:*/
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
        }
    }
}