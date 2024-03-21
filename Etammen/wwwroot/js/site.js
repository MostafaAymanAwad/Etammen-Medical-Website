// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function nextPage(pagenumber){

    let gender = $('input[name="gender"]:checked').val();

    var selectedItems = [];
    var checkboxes = document.querySelectorAll('input[name="degree"]:checked');
    checkboxes.forEach(function (checkbox) {
        selectedItems.push(checkbox.value);
    });
    console.log(selectedItems);

}