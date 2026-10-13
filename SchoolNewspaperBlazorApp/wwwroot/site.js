// Zamykanie menu bocznego (menu działa na samym CSS, bez Blazora).
(function () {
    function closeMenu() {
        var toggle = document.getElementById('menu-toggle');
        if (toggle) toggle.checked = false;
    }

    // Kliknięcie w link w menu zamyka je (ważne przy nawigacji bez przeładowania strony).
    document.addEventListener('click', function (e) {
        if (e.target.closest && e.target.closest('.drawer-nav a')) closeMenu();
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeMenu();
    });
})();
