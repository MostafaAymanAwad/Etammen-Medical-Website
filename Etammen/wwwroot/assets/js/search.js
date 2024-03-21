const deleteBtn = document.getElementById('delete');
function initializeCustomSelects() {
    const customSelects = document.querySelectorAll("select");
    var choices = new Choices('select', {
        searchEnabled: false,
        itemSelectText: '',
        removeItemButton: true,
    })
    for (let i = 0; i < customSelects.length; i++) {
        customSelects[i].addEventListener('addItem', function (event) {
            if (event.detail.value) {
                let parent = this.parentNode.parentNode
                parent.classList.add('valid')
                parent.classList.remove('invalid')
            }
            else {
                let parent = this.parentNode.parentNode
                parent.classList.add('invalid')
                parent.classList.remove('valid')
            }
        }, false);
    }
};

deleteBtn.addEventListener("click", function (e) {
    e.preventDefault()
    const deleteAll = document.querySelectorAll('.choices__button')
    for (let i = 0; i < deleteAll.length; i++) {
        deleteAll[i].click();
    }
});
initializeCustomSelects();