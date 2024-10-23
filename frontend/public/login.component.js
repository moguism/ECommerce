const btnSignUp = document.getElementById("btn-sign-up");
const btnSingIn = document.getElementById("btn-sign-in");

const container = document.querySelector(".container");

btnSignUp.addEventListener("click", ()=>{
    container.classList.toggle("toggle");
})

btnSingIn.addEventListener("click", ()=>{
    container.classList.toggle("toggle");
})