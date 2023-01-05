var navItems = document.querySelector(".nav-items");
var xBtn = document.querySelector(".X");
var humbergerBtn = document.querySelector(".humberger");

xBtn.addEventListener("click", () => {
    navItems.classList.remove("active");
    xBtn.classList.add("hide");
    humbergerBtn.classList.remove("hide");
});
humbergerBtn.addEventListener("click", () => {
    navItems.classList.add("active");
    xBtn.classList.remove("hide");
    humbergerBtn.classList.add("hide");
});