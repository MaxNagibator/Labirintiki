document.getElementsByTagName("head")[0].insertAdjacentHTML(
    "beforeend",
    "<link rel=\"stylesheet\" href=\"_content/BlazorAnimation/animate.css\" />");



var AnimatedComponent = AnimatedComponent || {};
AnimatedComponent.animationend = function (element, dotNet) {
    element.addEventListener('animationend', function(e) {
        if(e.target === this)dotNet.invokeMethodAsync("OnAnimationEnd") 
    });
};