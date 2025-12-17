(function () {
    const root = document.documentElement;
    const btn = document.getElementById("themeToggleBtn");

    if (!btn || !root) return;

    // Load saved theme from previous visits
    const saved = localStorage.getItem("brs-theme");
    if (saved === "dark" || saved === "light") {
        root.setAttribute("data-theme", saved);
        btn.textContent = saved === "dark" ? "☀️ Light" : "🌙 Dark";
    }

    btn.addEventListener("click", function () {
        const current = root.getAttribute("data-theme") || "light";
        const next = current === "light" ? "dark" : "light";
        root.setAttribute("data-theme", next);
        localStorage.setItem("brs-theme", next);
        btn.textContent = next === "dark" ? "☀️ Light" : "🌙 Dark";
    });
})();