// scripts.js

const user = JSON.parse(localStorage.getItem("user"));

if (user) {
  if (user.rol === "admin") {
    const adminView = document.getElementById("admin-view");
    const adminMenu = document.getElementById("admin-menu-item");

    if (adminView) adminView.style.display = "block";
    if (adminMenu) adminMenu.style.display = "block";

  } else if (user.rol === "user") {
    const userView = document.getElementById("user-view");
    if (userView) userView.style.display = "block";
  }
} else {
  window.location.href = "login.html";
}
