window.scrollToBottom = function (elementId) {
    var element = document.getElementById(elementId);
    element.scrollIntoView({block: 'center', behavior: 'smooth'})
};