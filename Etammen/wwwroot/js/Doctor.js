console.log("hamadaaaaaaaaaaaaaaaaaaaa")
nextPage(pagenumber){

    let gender = $('input[name="gender"]:checked').val();

    var selectedItems = [];
    var checkboxes = document.querySelectorAll('input[name="degree"]:checked');
    checkboxes.forEach(function (checkbox) {
        selectedItems.push(checkbox.value);
    });
    console.log(selectedItems);

}
$(document).ready(function () {

   

   



    //$("button").click(function () {
    //    $.ajax({
    //        url: "https://localhost:44363/api/Department",
    //        dataType: "json",
    //        success: function (result) {
    //            var departmentsHTML = "<ul>";
    //            $.each(result, function (index, department) {
    //                departmentsHTML += "<li><strong>Department Name:</strong> " + department.name + "</li>";
    //                departmentsHTML += "<li><strong>Location:</strong> " + department.location + "</li>";
    //                departmentsHTML += "<li><strong>Open Date:</strong> " + department.openDate + "</li>";
    //                departmentsHTML += "<li><strong>Employees:</strong><ul>";
    //                $.each(department.employeesName, function (i, employeeName) {
    //                    departmentsHTML += "<li>" + employeeName + "</li>";
    //                });
    //                departmentsHTML += "</ul></li>";
    //            });
    //            departmentsHTML += "</ul>";
    //            $("#div1").html(departmentsHTML);
    //        },
    //        error: function (xhr, status, error) {
    //            console.log("Error:", error);
    //        }
    //    });
    //});
});
