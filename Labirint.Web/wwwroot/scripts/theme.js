window.labirintTheme = (() => {
    const storageKey = 'IsDarkMod';
    const barColors = { dark: '#14110d', light: '#e9e2d2' };

    const isSystemDark = () => window.matchMedia('(prefers-color-scheme: light)').matches === false;

    const apply = theme => {
        document.documentElement.setAttribute('data-theme', theme);

        const bar = document.querySelector('meta[name="theme-color"]');

        if (bar !== null) {
            bar.setAttribute('content', barColors[theme]);
        }
    };

    return {
        apply: apply,
        isSystemDark: isSystemDark,
        applyStored: () => {
            let isDark;

            try {
                const stored = localStorage.getItem(storageKey);
                isDark = stored === null ? isSystemDark() : JSON.parse(stored) !== false;
            } catch {
                isDark = isSystemDark();
            }

            apply(isDark ? 'dark' : 'light');
        },
    };
})();

window.labirintTheme.applyStored();
