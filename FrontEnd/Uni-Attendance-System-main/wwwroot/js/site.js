
// Shared toast helper used by forms, the Kanban board and delete confirmations.
window.showToast = function (type, message) {
    const container = document.getElementById("appToasts");
    if (!container || typeof bootstrap === "undefined") return;

    const isSuccess = type === "success";
    const toast = document.createElement("div");
    toast.className = "toast app-toast";
    toast.setAttribute("role", "alert");
    toast.setAttribute("aria-live", "assertive");
    toast.setAttribute("aria-atomic", "true");

    const header = document.createElement("div");
    header.className = "toast-header";

    const icon = document.createElement("i");
    icon.className = isSuccess
        ? "bi bi-check-circle-fill text-success me-2"
        : "bi bi-exclamation-circle-fill text-danger me-2";

    const title = document.createElement("strong");
    title.className = "me-auto";
    title.textContent = isSuccess ? "Success" : "Error";

    const close = document.createElement("button");
    close.type = "button";
    close.className = "btn-close";
    close.setAttribute("data-bs-dismiss", "toast");
    close.setAttribute("aria-label", "Close");

    const body = document.createElement("div");
    body.className = "toast-body";
    body.textContent = message;

    header.append(icon, title, close);
    toast.append(header, body);
    container.appendChild(toast);

    toast.addEventListener("hidden.bs.toast", function () { toast.remove(); });
    bootstrap.Toast.getOrCreateInstance(toast, { delay: 4000 }).show();
};

document.addEventListener("DOMContentLoaded", function () {
    const currentPath = window.location.pathname.toLowerCase();
    const sidebar = document.querySelector(".sidebar");
    const toggleButton = document.getElementById("sidebarToggle");
    const overlay = document.getElementById("sidebarOverlay");

    // Active Navigation Link
    document.querySelectorAll(".sidebar a.nav-item").forEach(link => {
        const href = link.getAttribute("href");

        if (!href || href === "#") return;

        const linkPath = new URL(href, window.location.origin)
            .pathname.toLowerCase();

        if (
            linkPath === currentPath ||
            (linkPath !== "/" && currentPath.startsWith(linkPath + "/"))
        ) {
            link.classList.add("active");
        }
    });

    // Sidebar Controls
    function openSidebar() {
        sidebar?.classList.add("show");
        overlay?.classList.add("show");
        toggleButton?.setAttribute("aria-expanded", "true");
    }

    function closeSidebar() {
        sidebar?.classList.remove("show");
        overlay?.classList.remove("show");
        toggleButton?.setAttribute("aria-expanded", "false");
    }

    toggleButton?.addEventListener("click", function () {
        if (sidebar?.classList.contains("show")) {
            closeSidebar();
        } else {
            openSidebar();
        }
    });

    overlay?.addEventListener("click", closeSidebar);

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            closeSidebar();
        }
    });

    window.addEventListener("resize", function () {
        if (window.innerWidth > 767) {
            closeSidebar();
        }
    });
});
