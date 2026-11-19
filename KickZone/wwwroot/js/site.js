// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(function () {
    const STORAGE_KEY = 'kickzone-theme';
    const root = document.documentElement;
    const toggle = document.getElementById('themeToggle');
    const icon = document.getElementById('themeIcon');

    function currentTheme() {
        return root.getAttribute('data-bs-theme') === 'dark' ? 'dark' : 'light';
    }

    function applyTheme(theme) {
        root.setAttribute('data-bs-theme', theme);
        if (icon) icon.textContent = theme === 'dark' ? '☀️' : '🌙';
    }

    applyTheme(currentTheme());

    if (toggle) {
        toggle.addEventListener('click', function () {
            const next = currentTheme() === 'dark' ? 'light' : 'dark';
            localStorage.setItem(STORAGE_KEY, next);
            applyTheme(next);
        });
    }
})();
